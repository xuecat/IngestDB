using AutoMapper;
using IngestDBCore;
using IngestDBCore.Interface;
using IngestDBCore.Tool;
using IngestGlobalPlugin.Dto;
using IngestGlobalPlugin.Dto.OldResponse;
using IngestGlobalPlugin.Dto.Response;
using IngestGlobalPlugin.Models;
using IngestGlobalPlugin.Stores;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Sobey.Core.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using FileFormateInfoRequest = IngestGlobalPlugin.Dto.Response.FileFormateInfoResponse;

namespace IngestGlobalPlugin.Managers
{
    public class MaterialManager
    {
        private readonly ILogger Logger = LoggerManager.GetLogger("MaterialInfo");

        protected IMaterialStore Store { get; }
        protected IMapper _mapper { get; }
        private Lazy<IIngestTaskInterface> _taskInterface { get; }
        public MaterialManager(IMaterialStore store, IMapper mapper, IServiceProvider services)
        {
            Store = store;
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

            _taskInterface = new Lazy<IIngestTaskInterface>(() => services.GetRequiredService<IIngestTaskInterface>());
        }

        public async Task<bool> AddMqMsg<T>(T info)
        {
            var msg = _mapper.Map<DbpMsmqmsg>(info);

            if (msg != null)
            {
                if (msg.Msgprocesstime == DateTime.MinValue)
                {
                    msg.Msgprocesstime = DateTime.Now;
                }
                if(msg.TaskId <= 0)
                {
                    string pattern = "<TaskID>(?<taskId>.*?)</TaskID>";
                    Match match = Regex.Match(msg.Msgcontent, pattern);
                    if (match != null)
                    {
                        try
                        {
                            msg.TaskId = int.Parse(match.Groups["taskId"]?.ToString());
                        }
                        catch(Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    }
                }

                await Store.AddMQMsg(msg);
                return true;
            }
            return false;
        }

        public async Task<bool> DeleteMqMsgByTaskId(int taskid)
        {
            return await Store.DeleteMQMsgByTaskId(taskid);
        }

        public async Task<T> FindFormateInfo<T>(string key)
        {
            var f= await Store.GetFormateInfoAsync(a => a.Where(b => b.Key == key), true);

            if (f != null)
            {
                var formate = JsonHelper.ToObject<FileFormatInfo_in>(f.Formatinfo);

                if (formate != null)
                {
                    return _mapper.Map<T>(formate);
                }
            }

            return default(T);
        }

        public async Task<TR> UpdateFormateInfo<TR, TP>(TP pin)
        {
            var info = _mapper.Map<FileFormatInfo_in>(pin);

            await Store.UpdateFormateInfo(new DbpFileformatinfo() { Key = info.key, Formatinfo = JsonHelper.ToJson(info)});

            return _mapper.Map<TR>(info);
        }

        public async Task<List<TResult>> GetMsgContentByTaskid<TResult>(int taskid)
        {
            return _mapper.Map<List<TResult>>(await Store.GetMsgContentByTaskid(taskid));
        }
        
        public async Task<int> CountFailedRecordTask(int taskid)
        {
            return await Store.CountFailedRecordTask(taskid);
        }
        public async Task<int> CountFailedRecordTaskAndSection(int taskid, int section)
        {
            return await Store.CountFailedRecordTaskAndSection(taskid, section);
        }

        public async Task<int> DeleteMsgFailedRecord(int taskid, int sectionid)
        {
            await Store.RemoveMsgFailedRecord(taskid, sectionid);
            return taskid;
        }

        public async Task<int> AddMsgFailedRecord(MsgFailedRecord dpb)
        {
            await Store.AddMsgFailedRecord(_mapper.Map<DbpMsgFailedrecord>(dpb));
            return (int)dpb.TaskID;
        }

        public async Task<List<TResult>> GetMsgFailedRecordList<TResult>(List<int> red)
        {
            var lst = await Store.GetMsgFailedRecordListAsync(a => a.Where(b => red.Contains(b.TaskId)), true);
            return _mapper.Map<List<TResult>>(lst);
        }

        public async Task<List<T>> GetNeedProcessMsg<T>()
        {
            return _mapper.Map<List<T>>(await Store.GetNeedProcessMsg((int)MqmsgStatus.Processed, DateTime.Now.AddDays(-1)));
        }

        public async Task<List<MaterialInfoResponse>> GetMaterialInfo(int taskid)
        {
            var items = _mapper.Map<List<MaterialInfoResponse>>(await Store.GetMaterialListAsync(a => a.Where(b => b.Taskid == taskid), true));

            if (items != null && items.Count > 0)
            {
                foreach (var item in items)
                {
                    var lstpolicy = await Store.GetMaterialArchiveListAsync(a => a.Where(b => b.Materialid == item.Id).Select(z => z.Policyid), true);

                    item.ArchivePolicys = lstpolicy;

                    item.Audios = _mapper.Map<List<AudioInfoResponse>>(await Store.GetMaterialAudioListAsync(a => a.Where(b => b.Materialid == item.Id), true));
                    item.Videos = _mapper.Map<List<VideoInfoResponse>>(await Store.GetMaterialVideoListAsync(a => a.Where(b => b.Materialid == item.Id), true));
                }

                return items;
            }

            return null;
        }

        public async Task UpdateSaveInDBStateForTask(int nTaskID, int nPolicyID, int nSectionID, SAVE_IN_DB_STATE state, string strResult)
        {
            bool findmaterial = false;
            DbpMaterialArchive finditem = null;
            if (nSectionID >= 0)
            {
                var mlst = await Store.GetMaterialListAsync(a => a.Where(b => b.Taskid == nTaskID && b.Sectionid == nSectionID), true);
                if (mlst != null && mlst.Count>0)
                {
                    var midlst = mlst.Select(z => z.Materialid);
                    var aclst = await Store.GetMaterialArchiveListAsync(a => a.Where(b => 
                    (nPolicyID <= 0 ||b.Policyid == nPolicyID)
                    && midlst.Contains(b.Materialid)
                    ));

                    if (aclst == null || aclst.Count < 1)
                    {
                        findmaterial = false;
                        //SobeyRecException.ThrowSelfTwoParam("GetMaterialArchiveListAsync error 0",
                        //    GlobalDictionary.GLOBALDICT_CODE_FAILED_TO_GET_MATERIAL_BY_TASKID_FOR_POLICYID_TWOPARAM,
                        //    Logger,
                        //    nTaskID,
                        //    nPolicyID,
                        //    null);
                    }
                    else
                    {
                        finditem = aclst.FirstOrDefault();
                        findmaterial = true;
                    }
                        
                }

            }
            else
            {
                var mlst = await Store.GetMaterialListAsync(a => a.Where(b => b.Taskid == nTaskID), true);
                if (mlst != null && mlst.Count > 0)
                {
                    var midlst = mlst.Select(z => z.Materialid);
                    var aclst = await Store.GetMaterialArchiveListAsync(a => a.Where(b =>
                    (nPolicyID <= 0 || b.Policyid == nPolicyID)
                    && midlst.Contains(b.Materialid)
                    ));

                    if (aclst == null || aclst.Count < 1)
                    {
                        findmaterial = false;
                        //SobeyRecException.ThrowSelfTwoParam("GetMaterialArchiveListAsync error 1",
                        //    GlobalDictionary.GLOBALDICT_CODE_FAILED_TO_GET_MATERIAL_BY_TASKID_FOR_POLICYID_TWOPARAM,
                        //    Logger,
                        //    nTaskID,
                        //    nPolicyID,
                        //    null);
                    }
                    else
                    {
                        findmaterial = true;
                        finditem = aclst.FirstOrDefault();
                    }
                        
                }
                
            }

            if (!findmaterial)
            {
                //
                int id = Store.GetNextValId("DBP_SQ_MATERIALID");

                if (id > 0)
                {
                    //添加素材
                    string strGUID = Guid.NewGuid().ToString("N");
                    int nClipState = (state == SAVE_IN_DB_STATE.SECOND_END || state == SAVE_IN_DB_STATE.SECOND_READY) ? 1 : 0;

                    bool bRet = (strResult == "1") ? true : false;
                    strResult = UpdateArchiveResult("", (SAVE_IN_DB_STATE)0, true);
                    strResult = UpdateArchiveResult(strResult, state, bRet);


                    await Store.AddMaterial(new DbpMaterial()
                    {
                        Materialid = id,
                        Name = "Unknow",
                        Remark = "",
                        Createtime = DateTime.Now,
                        Taskid = nTaskID,
                        Sectionid = nSectionID,
                        Guid = Guid.NewGuid().ToString("N"),
                        Clipstate = nClipState
                    }, false);

                    await Store.UpdateMaterialArchive(new DbpMaterialArchive()
                    {
                        Materialid = id,
                        Policyid = nPolicyID,
                        Archivestate = (int)state,
                        Nextretry = DateTime.Now,
                        Archiveresult = strResult
                    }, true);
                }
                
                return;
            }
            else
            {
                bool bResult = (strResult == "1") ? true : false;
                if (finditem != null)
                {
                    if (finditem.Archivestate < (int)state)
                    {
                        finditem.Archivestate = (int)state;
                    }
                    //失败次数
                    int nFailedCount = Convert.ToInt32(finditem.Failedtimes);
                    int bIsProcessing = Convert.ToInt32(finditem.Isprocessing);
                    if (finditem.Archivestate == (int)SAVE_IN_DB_STATE.FIRST_READY || finditem.Archivestate == (int)SAVE_IN_DB_STATE.SECOND_READY)
                    {
                        bIsProcessing = 1;
                        bResult = Convert.ToBoolean(finditem.Lastresult);//保持不变
                    }
                    else
                    {
                        bIsProcessing = 0;
                    }


                    if (finditem.Archivestate< (int)SAVE_IN_DB_STATE.SECOND_READY && state >= SAVE_IN_DB_STATE.SECOND_READY)
                        nFailedCount = 0;

                    DateTime dtRetry = DateTime.Now;
                    if (!bResult)
                    {
                        nFailedCount = nFailedCount + 1;
                        if (nFailedCount > 30) nFailedCount = 30;
                        int nSpan = nFailedCount;
                        if (nFailedCount > 3)
                        {
                            nSpan = (int)Math.Pow(2.0, nFailedCount);
                        }

                        TimeSpan sp = new TimeSpan(0, 0, nSpan);
                        dtRetry = dtRetry + sp;
                    }
                    else
                    {
                        nFailedCount = 0;
                    }

                    if (string.IsNullOrEmpty(finditem.Archiveresult))//初次,没有更新结果
                    {
                        strResult = UpdateArchiveResult("", (SAVE_IN_DB_STATE)0, true);
                    }
                    else
                    {
                        strResult = finditem.Archiveresult;
                    }

                    strResult = UpdateArchiveResult(strResult, state, bResult);
                    if (strResult.Length <= 1024)//过长就不写入了
                    {
                        finditem.Archiveresult = strResult;
                    }

                    finditem.Lastresult = Convert.ToInt32(bResult);
                    finditem.Isprocessing= Convert.ToInt32(bIsProcessing);
                    finditem.Failedtimes= nFailedCount;
                    finditem.Lastupdatetime = DateTime.Now;
                    finditem.Nextretry= dtRetry;

                    await Store.SaveChangeAsync();
                }
            }

        }

        /// <summary>
		/// 将入库结果转化
		/// </summary>
		/// <param name="strXml">上一次的结果</param>
		/// <param name="emArchiveState">有四种,0.1.2.3,分别是第一次入库准备,结束,第二次入库准备,结束</param>
		/// <param name="bResult">成功与否</param>
		/// <returns>这次的结果</returns>
		public string UpdateArchiveResult(string strXml, SAVE_IN_DB_STATE emArchiveState, bool bResult)
        {
            string strResult = "";

            if (strXml == "" || strXml == string.Empty)
            {
                return "<Result Version=\"1.0\"><LastResult></LastResult><First></First><Second></Second></Result>";
            }
            else
            {
                try
                {
                    //XmlDocument doc = new XmlDocument();
                    //doc.LoadXml(strXml);
                    //XmlNode lastRuselt = doc.SelectSingleNode("/Result/LastResult");
                    //XmlNode firstChild = doc.FirstChild;

                    var root = XElement.Parse(strXml);
                    var last = root.Element("Result")?.Element("LastResult");
                    var first = root.Element("Result")?.Element("First");
                    var second = root.Element("Result")?.Element("Second");

                    if (emArchiveState == (SAVE_IN_DB_STATE)1)//指第一次入库结束
                    {
                        if (bResult)
                        {
                            if (first != null)
                            {
                                first.Add(new XElement("item", 1));
                                last.Value = "1";
                            }
                        }
                        else
                        {
                            if (first != null)
                            {
                                first.Add(new XElement("item", 0));
                                last.Value = "0";
                            }
                        }
                    }

                    if (emArchiveState == (SAVE_IN_DB_STATE)3)//指第二次入库结束
                    {
                        //只要有第二次的结果了，那么第一次就更新成成功，不再重试
                        if (bResult)
                        {
                            if (first != null)
                            {
                                first.Add(new XElement("item", 1));
                            }
                            if (second != null)
                            {
                                second.Add(new XElement("item", 1));
                                last.Value = "1";
                            }
                        }
                        else
                        {
                            if (first != null)
                            {
                                first.Add(new XElement("item", 1));
                            }
                            if (second != null)
                            {
                                second.Add(new XElement("item", 0));
                                last.Value = "0";
                            }
                        }
                    }

                    //if (firstChild.Attributes[0].Name == "Version"
                    //    && firstChild.Attributes[0].Value == "1.0")
                    //{
                    //    //if (emArchiveState == (SAVE_IN_DB_STATE)2)//指第二次入库准备时
                    //    //{
                    //    //    lastRuselt.InnerXml = "3";
                    //    //}					
                    //}

                    strResult = root.ToString();
                }
                catch (System.Exception ex)
                {
                    throw ex;
                }
            }

            return strResult;
        }
        public Task<bool> UpdateMqMsgStatus(string msgId, int nActionID, MqmsgStatus msgStatus, int nFailedCount)
        {
            return Store.UpdateMqMsgStatusAsync(msgId, nActionID, msgStatus, nFailedCount);
        }

        public Task<bool> DeleteMqMsgStatus(DateTime dt)
        {
            return Store.DeleteMqMsgStatusAsync(dt);
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

        public async Task<int> AddMaterialInfo(MaterialInfo mtrl)
        {
            //加个判断
            if (mtrl.nDeleteState > (int)DELETED_STATE.DELETEDBYOTHER || mtrl.nDeleteState < 0)
            {
                mtrl.nDeleteState = (int)DELETED_STATE.NOTDELETED;
            }
            var materialInfoList = await Store.GetMaterial(a => a.Where(x => x.Sectionid == mtrl.nSectionID &&
                                                                       x.Taskid == mtrl.nTaskID)
                                                           .OrderBy(x => x.Sectionid));
            if (materialInfoList != null && materialInfoList.Count > 0)//当前分段和任务存在
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
                return  materialID;
            }

            int nId = Store.GetNextValId("DBP_SQ_MATERIALID");

            //添加素材
            mtrl.strGUID = Guid.NewGuid().ToString("N");
            
            mtrl.nID = nId;
            await Store.AddMaterial(_mapper.Map<DbpMaterial>(mtrl), false);

            //添加视频信息
            if (mtrl.videos != null && mtrl.videos.Count> 0)
            {
                await Store.AddMaterialVideo(mtrl.nID, mtrl.videos, false);
            }

            //添加音频信息
            if (mtrl.audios != null && mtrl.audios.Count > 0)
            {
                await Store.AddMaterialAudio(mtrl.nID, mtrl.audios, false);
            }

            //添加入库信息
            await ModifyPolicy(mtrl.nTaskID, nId, true);

            //向DBP_TASK添加缩略图

            if (mtrl.videos != null && mtrl.videos.Count > 0)
            {
                /*
                 * 1. 取个图片值，免得老是更新成.mp4这种
                 * 2. 取个最后值，标识分段后的最新图片
                 */
                var taskitem = mtrl.videos.Where(a => mtrl.nSectionID == a.nVideoSource && (a.strFilename.Contains(".bmp") || a.strFilename.Contains(".jpg") || a.strFilename.Contains(".jpeg"))).LastOrDefault();

                if (taskitem != null)
                {
                    Dictionary<int, string> taskbmp = new Dictionary<int, string>();
                    taskbmp.Add(mtrl.nTaskID, taskitem.strFilename);
                    //.ToDictionary(a => mtrl.nTaskID, b=>b.strFilename);

                    //await Store.UpdateTaskBmp(taskBmps);
                    if (_taskInterface != null)
                    {
                        var response1 = await _taskInterface.Value.SubmitTaskCallBack(new TaskInternals()
                        {
                            funtype = IngestDBCore.TaskInternals.FunctionType.SetTaskBmp,
                            Ext3 = taskbmp
                        });

                        if (response1.Code != ResponseCodeDefines.SuccessCode)
                        {
                            Logger.Error("AddMaterialInfo SetTaskBmp error");
                        }

                    }
                }
                else
                {
                    Logger.Warn($"AddMaterialInfo update task bmp not image {JsonHelper.ToJson(mtrl.videos)}");
                }
            }
            return mtrl.nID;
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
        private async Task ModifyPolicy(int nTaskID, int nMaterialID, bool savechange = true)
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
            if (archives != null && archives.Count > 0)
            {
                await Store.AddOrUpdateMaterialArchive(archives, false);
            }

            if (savechange)
            {
                await Store.SaveChangeAsync();
            }
        }

        private async Task<List<MetaDataPolicy>> GetPolicyByTaskID(int taskId)
        {
            var listIds = await Store.GetPolicyTask(a => a.Where(x => x.Taskid == taskId && x.Policyid == -1).Select(x => x.Taskid), true);
            return _mapper.Map<List<MetaDataPolicy>>(await Store.GetMetadataPolicy(a => a.Where(x => listIds.Contains(x.Policyid)), true));
        }
    }

    public static class DistinctByClass
    {
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> seenKeys = new HashSet<TKey>();
            foreach (TSource element in source)
            {
                if (seenKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }
    }
}
