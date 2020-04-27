using AutoMapper;
using IngestDBCore;
using IngestDBCore.Tool;
using IngestGlobalPlugin.Dto;
using IngestGlobalPlugin.Models;
using IngestGlobalPlugin.Stores;
using Sobey.Core.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using FileFormateInfoRequest = IngestGlobalPlugin.Dto.FileFormateInfoResponse;

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

        public async Task<List<FailedMessageParam>> GetMsgContentByTaskid(int taskid)
        {
            return await Store.GetMsgContentByTaskid(taskid);
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

        public async Task<List<MsgFailedRecord>> GetMsgFailedRecordList(List<int> red)
        {
            var lst = await Store.GetMsgFailedRecordListAsync(a => a.Where(b => red.Contains(b.TaskId)), true);
            return _mapper.Map<List<MsgFailedRecord>>(lst);
        }

        public async Task<List<T>> GetNeedProcessMsg<T>()
        {
            return _mapper.Map<List<T>>(await Store.GetNeedProcessMsg((int)MqmsgStatus.Processed, DateTime.Now.AddDays(-1)));
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
                int id = IngestGlobalDBContext.next_val("DBP_SQ_MATERIALID");
                //添加素材
                string strGUID = Guid.NewGuid().ToString();
                int nClipState = (state == SAVE_IN_DB_STATE.SECOND_END || state == SAVE_IN_DB_STATE.SECOND_READY) ? 1 : 0;

                bool bRet = (strResult == "1") ? true : false;
                strResult = UpdateArchiveResult("", (SAVE_IN_DB_STATE)0, true);
                strResult = UpdateArchiveResult(strResult, state, bRet);


                await Store.AddMaterial(new DbpMaterial() {
                    Materialid = id,
                    Name = "Unknow",
                    Remark = "",
                    Createtime = DateTime.Now,
                    Taskid = nTaskID,
                    Sectionid = nSectionID,
                    Guid = Guid.NewGuid().ToString(),
                    Clipstate = nClipState
                }, false);

                await Store.UpdateMaterialArchive(new DbpMaterialArchive() {
                    Materialid = id,
                    Policyid = nPolicyID,
                    Archivestate = (int)state,
                    Nextretry = DateTime.Now,
                    Archiveresult = strResult
                }, true);
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
    }
}
