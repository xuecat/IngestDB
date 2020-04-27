using AutoMapper;
using IngestDBCore;
using IngestGlobalPlugin.Dto;
using IngestGlobalPlugin.Extend;
using IngestGlobalPlugin.Models;
using IngestGlobalPlugin.Stores;
using Microsoft.EntityFrameworkCore;
using Sobey.Core.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IngestGlobalPlugin.Managers
{
    public class MaterialManager
    {
        private readonly ILogger Logger = LoggerManager.GetLogger("MaterialInfo");

        protected IMaterialStore Store { get; }
        protected IMapper _mapper { get; }

        public MaterialManager(IMaterialStore store, IMapper mapper)
        {
            Store = store;
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<bool> AddMqMsg<T>(T info)
        {
            var msg = _mapper.Map<DbpMsmqmsg>(info);

            if (msg.Msgprocesstime == DateTime.MinValue)
            {
                msg.Msgprocesstime = DateTime.Now;
            }
            if (msg != null)
            {
                await Store.AddMQMsg(msg);
                return true;
            }
            return false;
        }

        public async Task<bool> UpdateMqMsgStatus(string msgId, int nActionID, MqmsgStatus msgStatus, int nFailedCount)
        {
            return await Store.UpdateMqMsgStatusAsync(msgId, nActionID, msgStatus, nFailedCount);
        }

        public async Task<bool> DeleteMqMsgStatus(DateTime dt)
        {
            return await Store.DeleteMqMsgStatusAsync(dt);
        }


        ///// <summary>
        ///// 添加素材并更改ArchiveState和ClipState
        ///// </summary>
        //public async Task<bool> AddMaterialAndChangeState(MaterialInfo Info)
        //{
        //    Info = await AddMaterial(Info);
        //    var mtr = await AddMaterialInfo(Info);
        //    Info = mtr.info;
        //    return await Store.UpdateArchiveAndClipState(Info.nID);
        //}
        //private async Task<MaterialInfo> AddMaterial(MaterialInfo Info)
        //{
        //    var mtr = await AddMaterialInfo(Info);
        //    Info = mtr.info;
        //    if (Info.ArchivePolicys != null && Info.ArchivePolicys.Count == 0)
        //    {
        //        Info.ArchivePolicys = null;
        //    }
        //    if (Info.audios != null && Info.audios.Count == 0)
        //    {
        //        Info.audios = null;
        //    }
        //    if (Info.videos != null && Info.videos.Count == 0)
        //    {
        //        Info.videos = null;
        //    }
        //    return Info;
        //}

        public async Task<(MaterialInfo info, int materialId)> AddMaterialInfo(MaterialInfo mtrl)
        {
            //加个判断
            if (mtrl.nDeleteState > (int)DELETED_STATE.DELETEDBYOTHER || mtrl.nDeleteState < 0)
            {
                mtrl.nDeleteState = (int)DELETED_STATE.NOTDELETED;
            }
            var materialInfoList = await Store.GetMaterial(a => a.Where(x => x.Sectionid == mtrl.nSectionID &&
                                                                       x.Taskid == mtrl.nTaskID)
                                                           .OrderBy(x => x.Sectionid));
            if (materialInfoList.Count != 0)//当前分段和任务存在
            {
                int materialID = materialInfoList[0].Materialid;
                await Store.UpdateMaterialVideo(materialID, mtrl.videos);
                await Store.UpdateMaterialAudio(materialID, mtrl.audios);
                //如果还是分段开始的信息,那么修改将Archive值改为0
                //为了满足MSV会发多次分段开始的消息
                if (mtrl.nClipState == (decimal)CLIP_STATE.STARTCAPUTER)//以消息总控发过来的为准,如果是分段开始的话,那么就
                {
                    await ModifyPolicy(materialID);
                }

                var material = await Store.GetMaterial(a => a.SingleOrDefaultAsync(x => x.Materialid == materialID));
                material.Clipstate = mtrl.nClipState;
                await Store.DbContextSaveChange();
                return (mtrl, materialID);
            }

            int nId = await Store.GetMaterial(a => a.MaxAsync(x => x.Materialid)) + 1;

            //添加素材
            mtrl.strGUID = Guid.NewGuid().ToString();

            DateTime tempTime = mtrl.strCreateTime.ToDateTime();   //创建时间
            mtrl.nID = nId;
            mtrl.strCreateTime = tempTime.ToStr();
            await Store.AddMaterial(_mapper.Map<DbpMaterial>(mtrl));

            //添加视频信息
            await Store.AddMaterialVideo(mtrl.nID, mtrl.videos);

            //添加音频信息
            if (mtrl.audios != null)
            {
                await Store.AddMaterialAudio(mtrl.nID, mtrl.audios);
            }

            //添加入库信息
            await ModifyPolicy(mtrl.nTaskID, nId);

            //向DBP_TASK添加缩略图

            if (mtrl.videos != null && mtrl.videos.Count > 0)
            {
                var taskBmps = mtrl.videos.Where(a => mtrl.nSectionID == a.nVideoSource).Select(a => (mtrl.nTaskID, a.strFilename)).ToList();
                if (taskBmps.Count > 0)
                {
                    await Store.UpdateTaskBmp(taskBmps);
                }
            }
            return (mtrl, mtrl.nID);
        }

        //修改入库策略,在重复分段开始的情况下,修改分段开始的情况,最重要是取得分段开始的信息
        private async Task ModifyPolicy(int materialId)
        {
            var material = await Store.GetMaterial(a => a.SingleOrDefaultAsync(x => x.Materialid == materialId));
            if (material != null && material.Clipstate == (int)CLIP_STATE.STARTCAPUTER)
            {
                List<MetaDataPolicy> policies = await GetPolicyByTaskID(material.Taskid);
                int policyID = policies.Last().nID;
                var archive = await Store.GetMaterialArchive(a => a.SingleOrDefaultAsync(x => x.Materialid == materialId && x.Policyid == policyID));
                if (archive != null)
                {
                    archive.Archivestate = 0;//重复发了分段的信息
                    await Store.DbContextSaveChange();
                }
            }
        }

        //修改入库策略
        private async Task ModifyPolicy(int nTaskID, int nMaterialID)
        {
            //获得nTaskID对应的入库策略
            List<MetaDataPolicy> policies = await GetPolicyByTaskID(nTaskID);
            var archives = policies.Select(x => new DbpMaterialArchive
            {
                Materialid = nMaterialID,
                Policyid = x.nID,
                Archivestate = 0,
                Lastresult = 1,
                Isprocessing = 1,
                Failedtimes = 0,
                Nextretry = DateTime.Now,
                Lastupdatetime = DateTime.Now,
                Archiveresult = ""
            }).ToList();
            await Store.AddOrUpdateMaterialArchive(archives);
        }

        private async Task<List<MetaDataPolicy>> GetPolicyByTaskID(int taskId)
        {
            var listIds = await Store.GetPolicyTask(a => a.Where(x => x.Taskid == taskId && x.Policyid == -1).Select(x => x.Taskid), true);
            return _mapper.Map<List<MetaDataPolicy>>(await Store.GetMetadataPolicy(a => a.Where(x => listIds.Contains(x.Policyid)), true));
        }
    }
}
