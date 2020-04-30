﻿using AutoMapper;
using IngestDBCore;
using IngestDBCore.Tool;
using IngestTaskPlugin.Dto;
using IngestTaskPlugin.Stores;
using Sobey.Core.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TaskType = IngestTaskPlugin.Dto.TaskType;
using taskState = IngestTaskPlugin.Dto.taskState;
using IngestDBCore.Interface;
using Microsoft.Extensions.DependencyInjection;
using IngestTaskPlugin.Models;
using TaskInfoRequest = IngestTaskPlugin.Dto.TaskInfoResponse;
using TaskContentRequest = IngestTaskPlugin.Dto.TaskContentResponse;
using TaskInfoRescheduledRequest = IngestTaskPlugin.Dto.Response.TaskInfoRescheduledResponse;
using CooperantType = IngestTaskPlugin.Dto.CooperantType;
using IngestTaskPlugin.Extend;

namespace IngestTaskPlugin.Managers
{
    public class TaskManager
    {
        public TaskManager(ITaskStore store, IMapper mapper, RestClient client, IIngestDeviceInterface device, IIngestGlobalInterface global)
        {
            Store = store;
            _restClient = client;
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _deviceInterface = device;
            _globalInterface = global;
        }
        private IIngestGlobalInterface _globalInterface { get; }
        private IIngestDeviceInterface _deviceInterface { get; }
        private RestClient _restClient { get; }
        protected ITaskStore Store { get; }
        protected IMapper _mapper { get; }
        private readonly ILogger Logger = LoggerManager.GetLogger("TaskInfo");

        public async virtual Task<TResult> GetTaskMetadataAsync<TResult>(int taskid, int ntype)
        {
            var f = await Store.GetTaskMetaDataAsync(a => a.Where(b => b.Taskid == taskid && b.Metadatatype == ntype), true);
            return _mapper.Map<TResult>(f);
        }

        public async virtual Task<List<TResult>> GetTaskMetadataListAsync<TResult>(List<int> taskid)
        {
            var f = await Store.GetTaskMetaDataListAsync(a => a.Where(b => taskid.Contains(b.Taskid)), true);
            return _mapper.Map<List<TResult>>(f);
        }

        public string ConverTaskMaterialMetaString(TaskMaterialMetaResponse re)
        {
            XDocument xdoc = new XDocument(
                new XElement("MATERIAL",
                 new XElement("TITLE", re.Title),
                 new XElement("MATERIALID", re.MaterialID),
                 new XElement("RIGHTS", re.Rights),
                 new XElement("COMMENTS", re.Comments),
                 new XElement("DESTINATION", re.Destination),
                 new XElement("FOLDERID", re.FolderID),
                 new XElement("ITEMNAME", re.ItemName),
                 new XElement("JOURNALIST", re.JournaList),
                 new XElement("CATEGORY", re.CateGory),
                 new XElement("PROGRAMNAME", re.ProgramName),
                 new XElement("DATEFOLDER", re.Datefolder))
                );
            return xdoc.ToString();
        }
        public string ConverTaskContentMetaString(TaskContentMetaResponse re)
        {
            var par = new XElement("PARAMS");
            foreach (var item in re.PeriodParam.Params)
            {
                par.Add(new XElement("DAY", item));
            }
            XDocument xdoc = new XDocument(
                new XElement("TaskContentMetaData",
                 new XElement("HOUSETC", re.HouseTC),
                 new XElement("PRESETSTAMP", re.Presetstamp),
                 new XElement("SIXTEENTONINE", re.SixteenToNine),
                 new XElement("SOURCETAPEID", re.SourceTapeID),
                 new XElement("DELETEFLAG", re.DeleteFlag),
                 new XElement("SOURCETAPEBARCODE", re.SourceTapeBarcode),
                 new XElement("BACKTAPEID", re.BackTapeID),
                 new XElement("USERMEDIAID", re.UserMediaID),
                 new XElement("UserToken", re.UserToken),
                 new XElement("VTRSTART", re.VtrStart),
                 new XElement("TCMODE", re.TcMode),
                 new XElement("ClipSum", re.ClipSum),
                 new XElement("TransState", re.TransState),
                 new XElement("PERIODPARAM",
                    new XElement("BEGINDATE", re.PeriodParam.BeginDate),
                    new XElement("ENDDATE", re.PeriodParam.EndDate),
                    new XElement("APPDATE", re.PeriodParam.AppDate),
                    new XElement("APPDATEFORMAT", re.PeriodParam.AppDateFormat),
                    new XElement("MODE", re.PeriodParam.Mode),
                    par
                    ))
                );

            return xdoc.ToString();
        }
        public string ConverTaskPlanningMetaString(TaskPlanningResponse re)
        {
            XDocument xdoc = new XDocument(
                new XElement("Planning",
                 new XElement("PLANGUID", re.PlanGuid),
                 new XElement("PLANNAME", re.PlanName),
                 new XElement("CREATORNAME", re.CreaToRName),
                 new XElement("CREATEDATE", re.CreateDate),
                 new XElement("MODIFYNAME", re.ModifyName),
                 new XElement("MODIFYDATE", re.ModifyDate),
                 new XElement("VERSION", re.Version),
                 new XElement("PLACE", re.Place),
                 new XElement("PLANNINGDATE", re.PlanningDate),
                 new XElement("DIRECTOR", re.Director),
                 new XElement("PHOTOGRAPHER", re.Photographer),
                 new XElement("REPORTER", re.Reporter),
                 new XElement("OTHER", re.Other),
                 new XElement("EQUIPMENT", re.Equipment),
                 new XElement("CONTACTINFO", re.ContactInfo),
                 new XElement("PLANNINGXML", re.PlanningXml))
                );
            return xdoc.ToString();
        }

        public async virtual Task<TaskMaterialMetaResponse> GetTaskMaterialMetadataAsync(int taskid)
        {
            var f = await Store.GetTaskMetaDataAsync(a => a
            .Where(b => b.Taskid == taskid && b.Metadatatype == (int)MetaDataType.emStoreMetaData)
            .Select(x => x.Metadatalong), true);

            try
            {
                var root = XDocument.Parse(f);
                var material = root.Element("MATERIAL");

                TaskMaterialMetaResponse ret = new TaskMaterialMetaResponse();
                ret.Title = material?.Element("TITLE")?.Value;
                ret.MaterialID = material?.Element("MATERIALID")?.Value;
                ret.Rights = material?.Element("RIGHTS")?.Value;
                ret.Comments = material?.Element("COMMENTS")?.Value;
                ret.Destination = material?.Element("DESTINATION")?.Value;
                ret.FolderID = int.Parse(material?.Element("FOLDERID")?.Value);
                ret.ItemName = material?.Element("ITEMNAME")?.Value;
                ret.JournaList = material?.Element("JOURNALIST")?.Value;
                ret.CateGory = material?.Element("CATEGORY")?.Value;
                ret.ProgramName = material?.Element("PROGRAMNAME")?.Value;
                ret.Datefolder = int.Parse(material?.Element("DATEFOLDER")?.Value);

                return ret;
            }
            catch (Exception e)
            {
                SobeyRecException.ThrowSelfNoParam(taskid.ToString(), GlobalDictionary.GLOBALDICT_CODE_FILL_GETTASKMETADATA_EXCEPTION, Logger, e);
            }
            return null;
            //return _mapper.Map<TResult>(f);
        }

        public async virtual Task<TaskContentMetaResponse> GetTaskContentMetadataAsync(int taskid)
        {
            var f = await Store.GetTaskMetaDataAsync(a => a
            .Where(b => b.Taskid == taskid && b.Metadatatype == (int)MetaDataType.emContentMetaData)
            .Select(x => x.Metadatalong), true);

            try
            {
                var root = XDocument.Parse(f);
                var material = root.Element("TaskContentMetaData");

                TaskContentMetaResponse ret = new TaskContentMetaResponse();
                ret.HouseTC = int.Parse(material?.Element("HOUSETC")?.Value);
                ret.Presetstamp = int.Parse(material?.Element("PRESETSTAMP")?.Value);
                ret.SixteenToNine = int.Parse(material?.Element("SIXTEENTONINE")?.Value);
                ret.SourceTapeID = int.Parse(material?.Element("SOURCETAPEID")?.Value);
                ret.DeleteFlag = int.Parse(material?.Element("DELETEFLAG")?.Value);
                ret.SourceTapeBarcode = int.Parse(material?.Element("SOURCETAPEBARCODE")?.Value);
                ret.BackTapeID = int.Parse(material?.Element("BACKTAPEID")?.Value);
                ret.UserMediaID = int.Parse(material?.Element("USERMEDIAID")?.Value);
                ret.UserToken = material?.Element("UserToken")?.Value;
                ret.VtrStart = material?.Element("VTRSTART")?.Value;
                ret.TcMode = int.Parse(material?.Element("TCMODE")?.Value);
                ret.ClipSum = int.Parse(material?.Element("ClipSum")?.Value);
                ret.TransState = int.Parse(material?.Element("TransState")?.Value);

                var period = material?.Element("PERIODPARAM");
                ret.PeriodParam = new PeriodParamResponse();
                ret.PeriodParam.BeginDate = period?.Element("BEGINDATE").Value;
                ret.PeriodParam.EndDate = period?.Element("ENDDATE").Value;
                ret.PeriodParam.AppDate = int.Parse(period?.Element("APPDATE").Value);
                ret.PeriodParam.AppDateFormat = period?.Element("APPDATEFORMAT").Value;
                ret.PeriodParam.Mode = int.Parse(period?.Element("MODE").Value);

                ret.PeriodParam.Params = period.Descendants("DAY").Select(x => int.Parse(x.Value)).ToList();

                return ret;
            }
            catch (Exception e)
            {
                SobeyRecException.ThrowSelfNoParam(taskid.ToString(), GlobalDictionary.GLOBALDICT_CODE_FILL_GETTASKMETADATA_EXCEPTION, Logger, e);
            }
            return null;
            //return _mapper.Map<TResult>(f);
        }

        public async virtual Task<TaskPlanningResponse> GetTaskPlanningMetadataAsync(int taskid)
        {
            var f = await Store.GetTaskMetaDataAsync(a => a
            .Where(b => b.Taskid == taskid && b.Metadatatype == (int)MetaDataType.emPlanMetaData)
            .Select(x => x.Metadatalong), true);

            try
            {
                var root = XDocument.Parse(f);
                var material = root.Element("Planning");

                TaskPlanningResponse ret = new TaskPlanningResponse();
                ret.PlanGuid = material?.Element("PLANGUID")?.Value;
                ret.PlanName = material?.Element("PLANNAME")?.Value;
                ret.CreaToRName = material?.Element("CREATORNAME")?.Value;
                ret.CreateDate = material?.Element("CREATEDATE")?.Value;
                ret.ModifyName = material?.Element("MODIFYNAME")?.Value;
                ret.ModifyDate = material?.Element("MODIFYDATE")?.Value;
                ret.Version = int.Parse(material?.Element("VERSION")?.Value);
                ret.Place = material?.Element("PLACE")?.Value;
                ret.PlanningDate = material?.Element("PLANNINGDATE")?.Value;
                ret.Director = material?.Element("DIRECTOR")?.Value;
                ret.Photographer = material?.Element("PHOTOGRAPHER")?.Value;
                ret.Reporter = material?.Element("REPORTER")?.Value;
                ret.Other = material?.Element("OTHER")?.Value;
                ret.Equipment = material?.Element("EQUIPMENT")?.Value;
                ret.ContactInfo = material?.Element("CONTACTINFO")?.Value;
                ret.PlanningXml = material?.Element("PLANNINGXML")?.Value;

                return ret;
            }
            catch (Exception e)
            {
                SobeyRecException.ThrowSelfNoParam(taskid.ToString(), GlobalDictionary.GLOBALDICT_CODE_FILL_GETTASKMETADATA_EXCEPTION, Logger, e);
            }
            return null;
            //return _mapper.Map<TResult>(f);
        }

        public async Task<TResult> GetTaskInfoByID<TResult>(int taskid, int change)
        {
            var item = await Store.GetTaskAsync(a => a.Where(b => b.Taskid == taskid), true);
            if (item.DispatchState == (int)dispatchState.dpsInvalid)
            {
                item.State = (int)taskState.tsDelete;
            }
            else if (item.DispatchState == (int)dispatchState.dpsRedispatch)
            {
                item.State = (int)taskState.tsInvaild;
            }
            return _mapper.Map<TResult>(item);
        }

        public async Task<List<TResult>> GetScheduleFailedTasks<TResult>()
        {
            return _mapper.Map<List<TResult>>(await Store.GetTaskListAsync(a => a.Where(b =>
                        (b.DispatchState == (int)dispatchState.dpsDispatchFailed || b.DispatchState == (int)dispatchState.dpsRedispatch)
                        && b.State != (int)taskState.tsDelete
                        && (b.Endtime > DateTime.Now && b.Endtime < DateTime.Now.AddDays(1))
                        && (b.Tasktype != (int)TaskType.TT_OPENEND && b.Tasktype != (int)TaskType.TT_OPENENDEX)), true));
        }

        public async Task<int> GetTieUpTaskIDByChannelId(int channelid)
        {
            DateTime now = DateTime.Now;
            return await Store.GetTaskAsync(a => a.Where(b => b.Channelid == channelid &&
            b.Tasktype == (int)TaskType.TT_TIEUP && (b.Starttime <= now && b.Endtime >= now)).Select(f => f.Taskid), true);
        }

        public async Task<List<T>> GetAutoManuConflict<T>(int channel)
        {
            var findtask = await Store.GetTaskListAsync(x => x.Where(y => y.Channelid == channel && y.Tasktype == (int)TaskType.TT_MANUTASK 
                                && (y.State == (int)taskState.tsExecuting 
                                    || (y.State == (int)taskState.tsReady && y.NewBegintime>DateTime.Now && y.NewBegintime<DateTime.Now.AddMinutes(3)))), true);

            List<WarningInfoResponse> lstback = new List<WarningInfoResponse>();
            if (findtask != null)
            {
                int manTaskID = 0;
                bool findexcuting = false;
                foreach (var item in findtask)
                {
                    if (item.State == (int)taskState.tsReady)
                    {
                        if (item.Starttime > item.Endtime)
                        {
                            continue;
                        }

                        var isExist = false;
                        var ArrayTime = Store.GetDateTimeFromString(item.Category);
                        foreach (DateTime dtTime in ArrayTime)
                        {
                            //zmj2008-11-04既然提前3分钟判断，那么应该跟3分钟后的那天一样的话，才符合要求
                            if (dtTime.Date == DateTime.Now.AddMinutes(3).Date)
                            {
                                isExist = true;
                                break;
                            }
                        }
                        if (isExist)
                        {
                            continue;
                        }

                        lstback.Add(new WarningInfoResponse()
                        {
                            TaskID = item.Taskid,
                            RelatedID = channel,
                            WarningLevel = 0,
                            WarningMessage = string.Format(GlobalDictionary.Instance.GetMessageByCode(GlobalDictionary.GLOBALDICT_CODE_SCHEDULED_TASK_WILL_BEGIN_AT_TWOPARAM), item.Taskname, item.NewBegintime.ToString())
                        });
                    }
                    else if (item.State == (int)taskState.tsExecuting)
                    {
                        findexcuting = true;
                        manTaskID = item.Taskid;
                    }
                }

                if (findexcuting)
                {
                    lstback.ForEach(a => a.RelatedID = manTaskID);
                    return _mapper.Map<List<T>>(lstback);

                }
            }
            return null;
        }
        public async Task<List<T>> GetBadChannelTask<T>(int channel)
        {
            var findtask = await Store.GetTaskListAsync(x => x.Where(y => y.Channelid == channel 
                                    && (y.State == (int)taskState.tsReady && y.NewBegintime > DateTime.Now && y.NewBegintime < DateTime.Now.AddMinutes(3))), true);

            List<WarningInfoResponse> lstback = new List<WarningInfoResponse>();
            if (findtask != null)
            {
                foreach (var item in findtask)
                {
                    if (item.Starttime > item.Endtime)
                    {
                        continue;
                    }

                    bool isExist = false;
                    var ArrayTime = Store.GetDateTimeFromString(item.Category);
                    foreach (DateTime dtTime in ArrayTime)
                    {
                        //zmj2008-11-04既然提前3分钟判断，那么应该跟3分钟后的那天一样的话，才符合要求
                        if (dtTime.Date == DateTime.Now.AddMinutes(3).Date)
                        {
                            isExist = true;
                            break;
                        }
                    }
                    if (isExist)
                    {
                        continue;
                    }

                    lstback.Add( new WarningInfoResponse()
                        {
                            TaskID = item.Taskid,
                            RelatedID = channel,
                            WarningLevel = 0,
                            WarningMessage = string.Format(GlobalDictionary.Instance.GetMessageByCode(GlobalDictionary.GLOBALDICT_CODE_SCHEDULED_TASK_WILL_BEGIN_AT_TWOPARAM), item.Taskname, item.NewBegintime.ToString())
                        });

                }
                return _mapper.Map<List<T>>(lstback);
            }
            return null;
        }

        public async Task<string> GetChannelCapturingLowMaterial(int channelid)
        {
            if (_deviceInterface != null)
            {
                var response1 = await _deviceInterface.GetDeviceCallBack(new DeviceInternals()
                {
                    funtype = IngestDBCore.DeviceInternals.FunctionType.ChannelExtendData,
                    ChannelId = channelid,
                    Status = (int)CHN_EXT_DATATYPE.CHN_EXT_PreviewVideo
                });

                if (response1.Code != ResponseCodeDefines.SuccessCode)
                {
                    Logger.Error("GetChannelCapturingLowMaterial ChannelExtendData error");
                    return null;
                }

                var fresponse = response1 as ResponseMessage<string>;
                if (fresponse != null && !string.IsNullOrEmpty(fresponse.Ext))
                {
                    return fresponse.Ext;
                }

                var findtask = await GetChannelCapturingTask<TaskContentResponse>(channelid);
                if (findtask != null)
                {
                    if (_globalInterface != null)
                    {
                        var rep = await _globalInterface.GetGlobalCallBack(new GlobalInternals() {
                            funtype = GlobalInternals.FunctionType.MaterialInfo,
                            TaskID = findtask.TaskID
                        });

                        var reps = rep as ResponseMessage<List<MaterialInfoInterface>>;
                        if (reps != null && reps.Ext != null && reps.Ext.Count> 0)
                        {
                            int nSectionIndex = -1;
                            MaterialInfoInterface lastSectionInfo = null;
                            foreach (MaterialInfoInterface mInfo in reps.Ext)
                            {
                                if (mInfo.SectionID > nSectionIndex)
                                {
                                    nSectionIndex = mInfo.SectionID;
                                    lastSectionInfo = mInfo;
                                }
                            }
                            if (lastSectionInfo != null)
                            {
                                if (lastSectionInfo.Videos != null)
                                {
                                    foreach (VideoInfoInterface vInfo in lastSectionInfo.Videos)
                                    {
                                        if (vInfo.VideoSource == 1) //低质量
                                        {
                                            return vInfo.Filename;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

            }
            return string.Empty;
        }

        private async Task<List<int>> TryDispatchTask(DbpTask taskinfo, bool backup)
        {
            if (_deviceInterface != null)
            {
                var response1 = await _deviceInterface.GetDeviceCallBack(new DeviceInternals() {
                    funtype = IngestDBCore.DeviceInternals.FunctionType.AllCaptureChannels });
                if (response1.Code != ResponseCodeDefines.SuccessCode)
                {
                    Logger.Error("TryDispatchTask AllCaptureChannels error");
                    return null;
                }

                var rep = response1 as ResponseMessage<List<CaptureChannelInfoInterface>>;

                if (taskinfo.Tasktype != (int)TaskType.TT_PERIODIC)
                {
                    if (rep != null && rep.Ext != null)
                    {
                        var lstchn = await Store.GetFreeChannels(rep.Ext.Where(b => b.BackState != BackupFlagInterface.emNoAllowBackUp).Select(a => a.ID).ToList(), taskinfo.Starttime, taskinfo.Endtime);

                        if (lstchn.Count > 0)
                        {
                            Logger.Info("TryDispatchTask GetFreeChannels {0} ", string.Join(",", lstchn));
                            return lstchn;
                        }
                    }
                }
                else
                {
                    var lstchn = await Store.GetFreePerodiChannels(rep.Ext.Where(b => b.BackState != BackupFlagInterface.emNoAllowBackUp).Select(a => a.ID).ToList(),
                                                taskinfo.Taskid, taskinfo.Recunitid.GetValueOrDefault(), taskinfo.Signalid.GetValueOrDefault(), -1, taskinfo.Category, taskinfo.Starttime, taskinfo.Endtime);
                    if (lstchn.Count > 0)
                    {
                        Logger.Info("TryDispatchTask GetFreePerodiChannels {0} ", string.Join(",", lstchn));
                        return lstchn;
                    }
                }
            }

            return null;
        }

        private async Task<int> GetRescheduleChannel(DbpTask taskinfo, List<MSVChannelStateInterface> lstmsvstate)
        {
            var lstchn = await TryDispatchTask(taskinfo, true);

            var backlst = lstchn.FindAll(a => lstmsvstate.Any(b => b.ChannelID == a && b.DevState != Device_StateInterface.DISCONNECTTED && b.MSVMode != MSV_ModeInterface.LOCAL));

            /*
             * @brief 正常代码有ChooseChannelForPeriod 有ChooseChannel，疯了，晕。直接用这个，我认为通过过滤应该在getfree就做好
             */
            return ChooseBestChannel(_mapper.Map<TaskContentRequest>(taskinfo), backlst, new CHSelCondition() { OnlyLocalChannel = true, BackupCHSel = true, CheckCHCurState = true});
        }

        public async Task<List<T>> RescheduleTasks<T>()
        {
            //重调度任务
            //TaskInfoRescheduledRequest
            //抄GetScheduleFailedTasks

            List<T> lstback = new List<T>();

            var lsttask = await Store.GetTaskListAsync(a => a.Where(b =>
                        (b.DispatchState == (int)dispatchState.dpsDispatchFailed || b.DispatchState == (int)dispatchState.dpsRedispatch)
                        && b.State != (int)taskState.tsDelete
                        && (b.Endtime > DateTime.Now && b.Endtime < DateTime.Now.AddDays(1))
                        && (b.Tasktype != (int)TaskType.TT_OPENEND && b.Tasktype != (int)TaskType.TT_OPENENDEX)));

            
            if (_deviceInterface != null)
            {
                var response1 = await _deviceInterface.GetDeviceCallBack(new DeviceInternals()
                {
                    funtype = IngestDBCore.DeviceInternals.FunctionType.AllChannelState,
                });
                if (response1.Code != ResponseCodeDefines.SuccessCode)
                {
                    Logger.Error("RescheduleTasks AllChannelState error");
                    return null;
                }

                var fresponse = response1 as ResponseMessage<List<MSVChannelStateInterface>>;
                if (fresponse != null)
                {
                    bool isUpdateGlobalState = false;//zmj2009-02-16 是否需要更新Global状态
                    List<TaskContentResponse> lstfind = new List<TaskContentRequest>();

                    foreach (var item in lsttask)
                    {
                        if (item.Recunitid < 0 || item.Endtime < DateTime.Now)
                        {
                            item.Tasklock = string.Empty;
                            continue;
                        }

                        //zmj2009-01-05 将重调度的顺序改变，先在原通道试验一下，若不成功再去备用通道进行选择
                        if (fresponse.Ext.Any(a => a.ChannelID == item.Channelid 
                                && a.DevState != Device_StateInterface.DISCONNECTTED
                                && a.MSVMode != MSV_ModeInterface.LOCAL))
                        {
                            item.SyncState = (int)syncState.ssNot;
                            item.DispatchState = (int)dispatchState.dpsDispatched;
                            /*
                             * @brief 这种任务现在没用了，所以我不写了
                             */
                            //zmj2008-10-24当kamataki任务遇到自动任务时，若该任务重调度不成功，则将此任务置为删除状态
                            //TaskFullInfo CapturingTask = TASKOPER.GetChannelCapturingTask(info.taskContent.nChannelID);
                            //if (CapturingTask != null)
                            //{
                            //    if (CapturingTask.taskContent.emCooperantType == GlobalDefines.CooperantType.emKamataki)
                            //    {
                            //        //ApplicationLog.WriteInfo("There is kamatati Task，TaskID is " + CapturingTask.taskContent.nTaskID.ToString() + ",TaskName is " + CapturingTask.taskContent.strTaskName);
                            //        //ApplicationLog.WriteInfo("Delete task,TaskID is " + info.taskContent.nTaskID.ToString() + ",TaskName is " + info.taskContent.strTaskName);
                            //        info.taskContent.emState = GlobalDefines.taskState.tsDelete;
                            //    }
                            //}
                            isUpdateGlobalState = true;
                        }
                        else
                        {
                            //原通道不用可时
                            var copyitem = Store.DeepClone(item);
                            int newchannelid = await GetRescheduleChannel(copyitem, fresponse.Ext);
                            if (newchannelid > 0)
                            {
                                copyitem.OldChannelid = item.Channelid;//为了map转换成nPreviousChannelID用

                                item.Channelid = newchannelid;
                                item.SyncState = (int)syncState.ssNot;
                                item.DispatchState = (int)dispatchState.dpsDispatched;
                                isUpdateGlobalState = true;

                                lstback.Add(_mapper.Map<T>(copyitem));
                            }
                            else
                            {
                                item.SyncState = (int)syncState.ssNot;
                                item.DispatchState = (int)dispatchState.dpsRedispatch;
                                isUpdateGlobalState = true;

                            }
                        }
                        item.Tasklock = string.Empty;

                    }
                    if (isUpdateGlobalState)
                    {
                        await Store.SaveChangeAsync();
                    }

                    return lstback;
                }
            }
            return null;
            ///////return
        }

        public async Task<List<TResult>> GetKamakatiFailTasks<TResult>()
        {
            var lst = await Store.GetTaskListAsync(a => a.Where(b => b.Backtype >= 65535 || b.Backtype == (int)CooperantType.emKamataki), true);
            List<DbpTask> lsttask = new List<DbpTask>();

            foreach (var item in lst)
            {
                int backtyp = item.Backtype.GetValueOrDefault() >> 16;
                if (backtyp > 0 && backtyp< 3)
                {
                    if (item.Backtype == (int)CooperantType.emKamataki)
                    {
                        lsttask.Add(item);
                    }
                }
            }

            return _mapper.Map<List<TResult>>(lsttask);

        }

        public async Task SetTaskBmp(int taskid, string bmppath)
        {
            if (string.IsNullOrEmpty(bmppath))
            {
                return;
            }

            var findtask = await Store.GetTaskAsync(a => a.Where(b => b.Taskid == taskid));
            findtask.Description = bmppath;

            await Store.SaveChangeAsync();
        }

        //这个接口是为老写的
        public async Task UpdateTaskMetaDataAsync(int taskid, MetaDataType type, string metadata)
        {
            await Store.UpdateTaskMetaDataAsync(taskid, type, metadata);
        }

        

        public async virtual Task<string> UpdateMetadataPropertyAsync(int taskid, int type, List<PropertyResponse> lst)
        {
            var f = await Store.GetTaskMetaDataAsync(a => a
            .Where(b => b.Taskid == taskid && b.Metadatatype == type));

            try
            {
                var root = XDocument.Load(f.Metadatalong);

                XElement material = null;
                switch (type)
                {
                    case (int)MetaDataType.emStoreMetaData:
                        { material = root.Element("MATERIAL"); }
                        break;
                    case (int)MetaDataType.emContentMetaData:
                        { material = root.Element("TaskContentMetaData"); }
                        break;
                    case (int)MetaDataType.emPlanMetaData:
                        { material = root.Element("Planning"); }
                        break;
                    case (int)MetaDataType.emSplitData:
                        { material = root.Element("SplitMetaData"); }
                        break;
                    default:
                        break;
                }

                foreach (var item in lst)
                {
                    var pro = material?.Descendants(item.Property).FirstOrDefault();
                    if (pro == null)
                    {
                        material?.Add(new XElement(item.Property, item.Value));
                    }
                    else
                        pro.Value = item.Value;
                }

                f.Metadatalong = root.ToString();
                await Store.SaveChangeAsync();
                return f.Metadatalong;
            }
            catch (Exception e)
            {
                SobeyRecException.ThrowSelfNoParam(taskid.ToString(), GlobalDictionary.GLOBALDICT_CODE_FILL_GETTASKMETADATA_EXCEPTION, Logger, e);
            }
            return string.Empty;
            //return _mapper.Map<TResult>(f);
        }

        public async Task<bool> SetTaskCooperType(int taskid, CooperantType ct)
        {
            var findtask = await Store.GetTaskAsync(a => a.Where(b => b.Taskid == taskid));
            findtask.Backtype = (int)ct;
            await Store.SaveChangeAsync();
            return true;
        }

        /// <summary>
        /// 通过信号源ID或用户编码获得采集参数
        /// </summary>
        /// <param name="SignalId">信号源ID</param>
        /// <param name="usetokencode">用户编码true是token false是code</param>
        /// <param name="userCode">采集参数</param>
        /// <returns>成功与否</returns>
        /// <remarks>
        /// Add by chenzhi 2013-07-30
        /// </remarks>
        public async Task<string> GetCaptureTemplateBySignalIdAndUserCode(int SignalId, bool usetokencode, string userCode)
        {
            string strCaptureTemplate = string.Empty;

            if (SignalId < 1)
            {
                SobeyRecException.ThrowSelfNoParam("",GlobalDictionary.GLOBALDICT_CODE_NO_SINGAL_SRC,Logger, null);
            }
            
            if (_deviceInterface != null)
            {
                var response1 = await _deviceInterface.GetDeviceCallBack(new DeviceInternals()
                {
                    funtype = IngestDBCore.DeviceInternals.FunctionType.CaptureTemplateIDBySignal,
                    SrcId = SignalId,
                    Status = 1
                });
                if (response1.Code != ResponseCodeDefines.SuccessCode)
                {
                    Logger.Error("GetCaptureTemplateBySignalIdAndUserCode SignalCaptureID error");
                    return null;
                }

                var fresponse = response1 as ResponseMessage<int>;
                if (fresponse != null)
                {
                    Logger.Info("GetCaptureTemplateBySignalIdAndUserCode " + fresponse.Ext);
                    if (fresponse.Ext <= 0)
                    {
                        if (string.IsNullOrEmpty(userCode))
                        {
                            return string.Empty;
                        }
                        else
                        {
                            fresponse.Ext =  await _restClient.GetUserParamTemplateID(usetokencode, userCode);
                        }
                    }

                    
                    if (_globalInterface != null)
                    {
                        var response2 = await _globalInterface.GetGlobalCallBack(new GlobalInternals()
                        {
                            funtype = GlobalInternals.FunctionType.UserParamTemplateByID,
                            TemplateID = fresponse.Ext
                        });
                        if (response2.Code != ResponseCodeDefines.SuccessCode)
                        {
                            Logger.Error("GetCaptureTemplateBySignalIdAndUserCode UserParamTemplateByID error");
                            return null;
                        }

                        var gresponse = response2 as ResponseMessage<string>;
                        strCaptureTemplate = gresponse.Ext;
                    }

                    if (string.IsNullOrEmpty(strCaptureTemplate))
                    {
                        return string.Empty;
                    }
                    
                    string strCapParamSD = "";
                    string strCapParamHD = "";
                    string strCapParamUHD = "";

                    int pos = strCaptureTemplate.IndexOf("</CAPTUREPARAM>");
                    strCapParamHD = strCaptureTemplate.Substring(0, pos + 15);
                    strCapParamHD = await ModifyCaptureParamPath(usetokencode, userCode, strCapParamHD);
                    int nLen = strCaptureTemplate.Length - pos - 15;
                    if (nLen <= 0)
                    {
                        strCapParamSD = "";
                        return string.Empty;
                    }

                    int npos2 = strCaptureTemplate.IndexOf("</CAPTUREPARAM>", pos + 15);
                    strCapParamSD = strCaptureTemplate.Substring(pos + 15, npos2 - pos);
                    strCapParamSD = await ModifyCaptureParamPath(usetokencode, userCode, strCapParamSD);

                    strCapParamUHD = strCaptureTemplate.Substring(npos2 + 15);
                    strCapParamUHD = await ModifyCaptureParamPath(usetokencode, userCode, strCapParamUHD);

                    //TS和SDI支持高标清自适应，流媒体不支持
                    //ProgrammeInfo info = DEVICESACCESS.GetProgrammeInfoById(nSignalId);

                    //if (info.emPgmType == ProgrammeType.PT_StreamMedia)
                    //{
                    //    strCaptureTemplate = (0 == info.TypeId) ? strCapParamSD : strCapParamHD;
                    //}
                    //else//(taskSrc != TaskSource.emStreamMediaUploadTask)
                    //{
                        //构造总的采集参数串
                        
                        var doc = new XElement("CaptureMetaAll", 
                            new XElement("SDCaptureMeta", strCapParamSD),
                            new XElement("HDCaptureMeta", strCapParamHD),
                            new XElement("UHDCaptureMeta", strCapParamUHD));

                        strCaptureTemplate = doc.ToString();
                    //}
                }
                return strCaptureTemplate;

            }
            return string.Empty;
        }


        public async Task<string> ModifyCaptureParamPath(bool busetokencode, string userToken, string strCaptureParam)
        {
            try
            {
                Logger.Info("GpiCtrServiceLog begin to ModifyCaptureParamPath");

                var root = XElement.Parse(strCaptureParam);

                string curPath = "";
                string storageType = "";
                string disk = "";
                string strPath = "";
                int nPos01 = -1;
                int nPos02 = -1;
                string strTemp = "";
                if (null != root)
                {

                    var pathNode0 = root.Descendants("path0FileName").FirstOrDefault();
                    if (null != pathNode0)
                        curPath = pathNode0.Value;

                    Logger.Info("ModifyCaptureParamPath, curPath:{0}", curPath);

                    if (curPath != "")
                    {
                        nPos01 = curPath.IndexOf('\\');
                        if (nPos01 > -1)
                        {
                            disk = curPath.Substring(0, nPos01);

                        }
                        nPos02 = curPath.IndexOf('\\', nPos01 + 1);
                        storageType = curPath.Substring(nPos01 + 1, nPos02 - nPos01 - 1);
                        int len = curPath.Length;
                        strTemp = curPath.Substring(nPos02 + 1, len - nPos02 - 1);

                    }
                    Logger.Info("ModifyCaptureParamPath, storageType:{0}, disk:{1}", storageType, disk);
                    if (storageType != "" && disk != "")
                    {
                        strPath = await _restClient.GetUserPath(busetokencode, userToken, storageType, disk);
                        if (!string.IsNullOrEmpty(strPath))
                        {
                            Logger.Info("ModifyCaptureParamPath, GetUserPath is success!,strPath:{0}", strPath);
                        }
                        else
                        {
                            Logger.Info("GpiCtrServiceLog", "ModifyCaptureParamPath, GetUserPath is failed!", "");
                        }
                    }
                    string fileName0 = "";
                    if (storageType == "nas")
                    {
                        fileName0 = strPath + strTemp;
                    }
                    else if (storageType == "oss")
                    {
                        int nPos = strPath.IndexOf('?');
                        if (nPos > -1)
                        {
                            strTemp = strTemp.Replace('\\', '/');
                            fileName0 = strPath.Insert(nPos, '/' + strTemp);
                        }
                    }
                    //pathNode0.InnerText = fileName0 == "" ? pathNode0.InnerText : fileName0;
                    pathNode0.Value = fileName0 == "" ? pathNode0.Value : fileName0;

                    Logger.Info("ModifyCaptureParamPath, filename0:{0}", fileName0);
                    storageType = "";
                    disk = "";
                    var pathNode1 = root.Descendants("path1FileName").FirstOrDefault();
                    if (null != pathNode1)
                        curPath = pathNode1.Value;
                    if (curPath != "")
                    {
                        nPos01 = curPath.IndexOf('\\');
                        if (nPos01 > -1)
                        {
                            disk = curPath.Substring(0, nPos01);

                        }
                        nPos02 = curPath.IndexOf('\\', nPos01 + 1);
                        storageType = curPath.Substring(nPos01 + 1, nPos02 - nPos01 - 1);
                        int len = curPath.Length;
                        strTemp = curPath.Substring(nPos02 + 1, len - nPos02 - 1);

                    }
                    if (storageType != "" && disk != "")
                    {
                        strPath = await _restClient.GetUserPath(busetokencode, userToken, storageType, disk);
                        if (!string.IsNullOrEmpty(strPath))
                        {
                            Logger.Info("ModifyCaptureParamPath, GetUserPath is success!,strPath:{0}", strPath);
                        }
                        else
                        {
                            Logger.Info("ModifyCaptureParamPath, GetUserPath is failed!", "");
                        }
                    }
                    string fileName1 = "";
                    if (storageType == "nas")
                    {
                        fileName1 = strPath + strTemp;
                    }
                    else if (storageType == "oss")
                    {
                        int nPos = strPath.IndexOf('?');
                        if (nPos > -1)
                        {
                            strTemp = strTemp.Replace('\\', '/');
                            fileName1 = strPath.Insert(nPos, '/' + strTemp);
                        }
                    }
                    pathNode1.Value = fileName1 == "" ? pathNode1.Value : fileName1;
                    strCaptureParam = root.ToString();
                }

                return strCaptureParam;
            }
            catch (System.Exception e)
            {
                Logger.Error("ModifyCaptureParamPath Exception,ErroInfo:{0}", e.Message);
            }
            return string.Empty;
        }

        public async Task<string> Update24HoursTask(int ntaskid, long oldlen, int oldclipnum, string newname, string newguid, int index)
        {
            Logger.Info(string.Format("Update24HoursTask {0} {1} {2} {3}", ntaskid, newname, oldlen, newguid));

            var taskinfo = await Store.GetTaskAsync(a => a.Where(b => b.Taskid == ntaskid));

            if (taskinfo == null)
            {
                Logger.Error("Update24HoursTask no find" + ntaskid);
                SobeyRecException.ThrowSelfNoParam("", GlobalDictionary.GLOBALDICT_CODE_TASK_ID_DOES_NOT_EXIST, Logger, null);
            }

            DbpTaskMetadata splitmeta = null;
            DbpTaskMetadata materilmeta = null;

            var taskmeta = await Store.GetTaskMetaDataListAsync(a => a.Where(b => b.Taskid == ntaskid 
            && (b.Metadatatype == (int)MetaDataType.emSplitData || b.Metadatatype == (int)MetaDataType.emStoreMetaData)));

            foreach (var item in taskmeta)
            {
                if (item.Metadatatype == (int)MetaDataType.emSplitData)
                {
                    splitmeta = item;
                }
                else
                    materilmeta = item;
            }

            if (splitmeta == null)
            {
                splitmeta = new DbpTaskMetadata() { Taskid = ntaskid, Metadatatype = (int)MetaDataType.emSplitData, Metadatalong = "<SplitMetaData></SplitMetaData>" };
            }

            var root = XDocument.Parse(splitmeta.Metadatalong);
            var taskcontentnode = root.Element("SplitMetaData");

            var splits = taskcontentnode.Element("SplitClips");
            if (splits == null)
            {
                splits = new XElement("SplitClips");
                taskcontentnode.Add(splits);
            }

            var splititem = new XElement("SplitItem");
            splititem.Add(new XElement("SplitGuid", newguid), new XElement("SplitClipNum", oldclipnum), new XElement("SplitLen", oldlen));
            splits.Add(splititem);

            var mlguid = taskcontentnode.Element("MLTOTASKGUID");
            if (mlguid == null)
            {
                taskcontentnode.Add(new XElement("MLTOTASKGUID", newguid));
            }
            else
                mlguid.Value = newguid;

            if (index == 1)
            {
                var orgtitle = taskcontentnode.Element("ORGTITLE");
                if (orgtitle == null)
                {
                    taskcontentnode.Add(new XElement("ORGTITLE", taskinfo.Taskname));
                }
                else
                    orgtitle.Value = taskinfo.Taskname;
            }

            if (materilmeta != null)
            {
                var mroot = XDocument.Parse(materilmeta.Metadatalong);
                var title = mroot?.Element("MATERIAL")?.Element("TITLE");
                if (title == null)
                {
                    mroot.Element("MATERIAL").Add(new XElement("TITLE", newname));
                }
                else
                    title.Value = newname;

                materilmeta.Metadatalong = mroot.ToString();
            }
           
            taskinfo.Taskname = newname;
            taskinfo.Taskguid = newguid;
            
            splitmeta.Metadatalong = root.ToString();

            await Store.SaveChangeAsync();
            return newguid;
        }
        

        public async Task<TaskContentResponse> SplitTask(int taskid, string newguid, string newname)
        {
            var findtask = await Store.GetTaskAsync(a => a.Where(b => b.Taskid == taskid), true);

            if (findtask != null)
            {
                if (findtask.Tasktype != (int)TaskType.TT_MANUTASK
                    && findtask.Tasktype != (int)TaskType.TT_NORMAL
                    && findtask.Tasktype != (int)TaskType.TT_OPENEND
                    && findtask.Tasktype != (int)TaskType.TT_OPENENDEX)
                {
                    SobeyRecException.ThrowSelfNoParam("SplitTask", GlobalDictionary.GLOBALDICT_CODE_CAN_NOT_SPLIT_LOOP_PERIODIC_TASK, Logger, null);
                }

                findtask.DispatchState = (int)dispatchState.dpsDispatched;
                findtask.OpType = (int)opType.otAdd;
                findtask.SyncState = (int)syncState.ssSync;
                findtask.Tasklock = string.Empty;

                int nOldTaskID = findtask.Taskid;
                string strOldTaskName = findtask.Taskname;
                findtask.Taskid = -1;
                findtask.Taskguid = newguid;
                //移除分段任务的组任务属性
                findtask.Sgroupcolor = 0;
                //对于自动任务，上条任务与该条任务无缝靠的是StopCapture和StartCapture的消息通知
                //这里可以做一个大概的时间
                if (findtask.Tasktype == (int)TaskType.TT_NORMAL)
                {
                    findtask.Starttime = DateTime.Now;
                    //zmj2010-02-24 普通任务将它改为准备状态，由消息总控将它改为可执行状态
                    //newTaskInfo.taskContent.emState = taskState.tsReady;
                    //yangchuang20120921如果离结束时间太近了，不允许再分段了
                    if (findtask.Endtime <= DateTime.Now.AddSeconds(5))
                    {
                        SobeyRecException.ThrowSelfNoParam("SplitTask", GlobalDictionary.GLOBALDICT_CODE_DO_NOT_ALLOW_TO_SPLIT_THE_TASK, Logger, null);
                    }
                }

                if (findtask.Tasktype == (int)TaskType.TT_MANUTASK
                    || findtask.Tasktype == (int)TaskType.TT_OPENEND
                    || findtask.Tasktype == (int)TaskType.TT_OPENENDEX // Add by chenzhi 2012-07-25
                    )
                {
                    findtask.Starttime = DateTime.Now;
                    findtask.Endtime = findtask.Starttime;
                }


                findtask.Description = string.Empty;
                findtask.Taskname = newname;

                //  如果有VTR备份，则不受影响，前一条的备份状态被改为结束，后一条

                /********************************
                emPureTask			= 0,	//一般任务
                emKamataki			= 1,	//同时收录到播出
                emVTRKamataki		= 2,
                emVTRBackup			= 3,
                emKamatakiFinish	= 4,
                emVTRBackupFinish	= 5
                *********************************/

                if (findtask.Backtype == (int)CooperantType.emVTRBackup)
                {
                    await SetTaskCooperType(nOldTaskID, CooperantType.emVTRBackupFinish);
                }

                TaskSource taskSrc = await GetTaskSource(nOldTaskID);

                //MetaDataPolicy[] arrMetaDataPolicy = PPLICYACCESS.GetPolicyByTaskID(nTaskID);
                //List<int> listPolicyId = new List<int>();
                //foreach (MetaDataPolicy item in arrMetaDataPolicy)
                //{
                //    listPolicyId.Add(item.nID);
                //}
                //TASKOPER.AddTaskWithPolicys(ref newTaskInfo, true, taskSrc, listPolicyId.ToArray());

                var lsttaskmeta = await Store.GetTaskMetaDataListAsync(a => a.Where(b => b.Taskid == taskid), true);;
                string strCapatureMetaData = string.Empty, strStoreMetaData = string.Empty, strContentMetaData = string.Empty, strPlanMetaData = string.Empty, strSplitMetaData = string.Empty;
                foreach (var item in lsttaskmeta)
                {
                    if (item.Metadatatype == (int)MetaDataType.emCapatureMetaData)
                    {
                        strCapatureMetaData = item.Metadatalong;
                    }
                    else if (item.Metadatatype == (int)MetaDataType.emContentMetaData)
                    {
                        strContentMetaData = item.Metadatalong;
                    }
                    else if (item.Metadatatype == (int)MetaDataType.emStoreMetaData)
                    {
                        strStoreMetaData = item.Metadatalong;
                    }
                    else if (item.Metadatatype == (int)MetaDataType.emPlanMetaData)
                    {
                        strPlanMetaData = item.Metadatalong;
                    }
                    else if (item.Metadatatype == (int)MetaDataType.emSplitData)
                    {
                        strSplitMetaData = item.Metadatalong;
                    }
                }

                if (!string.IsNullOrEmpty(strSplitMetaData))
                {
                    var sroot = XElement.Parse(strPlanMetaData);
                    var item = sroot.Descendants("ORGTITLE").FirstOrDefault();
                    if (item != null)
                    {
                        item.Value = strOldTaskName;
                        strSplitMetaData = sroot.ToString();
                    }
                }

                if (!string.IsNullOrEmpty(strStoreMetaData))
                {
                    var mroot = XElement.Parse(strStoreMetaData);
                    var item = mroot.Descendants("TITLE").FirstOrDefault();
                    if (item != null)
                    {
                        item.Value = newname;
                    }
                    item = mroot.Descendants("MATERIALID").FirstOrDefault();
                    if (item != null)
                    {
                        item.Value = string.Empty;
                    }
                    strStoreMetaData = mroot.ToString();
                }

                if (!string.IsNullOrEmpty(strContentMetaData))
                {
                    var mroot = XElement.Parse(strContentMetaData);
                    mroot.Descendants("GroupItems").Remove();
                    mroot.Descendants("GroupColor").Remove();
                    mroot.Descendants("GroupID").Remove();
                    mroot.Descendants("RealStampIndex").Remove();

                    strContentMetaData = mroot.ToString();
                }

                var addinfo = new TaskInfoRequest();
                addinfo.BackUpTask = false;
                addinfo.TaskSource = taskSrc;
                addinfo.TaskContent = _mapper.Map<TaskContentRequest>(findtask);
                var backinfo = await AddTaskWithoutPolicy(addinfo, strCapatureMetaData, strContentMetaData, strStoreMetaData, strPlanMetaData);
                return _mapper.Map<TaskContentRequest>(backinfo);
            }
            return null;
        }


        public async Task<int> ChooseUsealbeChannel(List<int> lstchannelid, DateTime dtbegin, DateTime dtend)
        {
            
            if (_deviceInterface != null)
            {
                var response1 = await _deviceInterface.GetDeviceCallBack(new DeviceInternals()
                {
                    funtype = IngestDBCore.DeviceInternals.FunctionType.AllChannelState,
                });
                if (response1.Code != ResponseCodeDefines.SuccessCode)
                {
                    Logger.Error("ChooseUsealbeChannel AllChannelState error");
                    return 0;
                }

                var fresponse = response1 as ResponseMessage<List<MSVChannelStateInterface>>;
                if (fresponse != null)
                {
                    foreach (var item in lstchannelid)
                    { 
                        if (fresponse.Ext.Any(a => a.ChannelID == item
                                && a.DevState != Device_StateInterface.DISCONNECTTED
                                && a.MSVMode != MSV_ModeInterface.LOCAL))
                        {
                            var backlst = await Store.GetFreeChannels(new List<int>() { item }, dtbegin, dtend);
                            if (backlst != null && backlst.Count > 0)
                            {
                                return backlst[0];
                            }
                        }
                    }
                }
            }
            return 0;
        }

        

        public async Task<bool> LockTask(int taskid)
        {
            await Store.LockTask(taskid);
            return true;
        }

        

        public async Task<bool> CompleteRescheduleTasks<T>(T re)
        {
            var taskinfo = _mapper.Map<TaskContentRequest>(re);

            if (taskinfo.ChannelID > 0)
            {
                var findtask = await Store.GetTaskAsync(a => a.Where(b => b.Taskid == taskinfo.TaskID));
                findtask.SyncState = (int)syncState.ssNot;
                findtask.DispatchState = (int)dispatchState.dpsDispatched;

                await Store.SaveChangeAsync();
                return true;
                /*
                 * @brief emKamataki现没用，所以我去了
                 */
                //zmj2008-10-24当kamataki任务遇到自动任务时，若该任务重调度不成功，则将此任务置为删除状态
                //TaskFullInfo capturingTask = TASKOPER.GetChannelCapturingTask(info.taskContent.nChannelID);
                //if (capturingTask != null)
                //{
                //    if (capturingTask.taskContent.emCooperantType == GlobalDefines.CooperantType.emKamataki)
                //    {
                //        info.taskContent.emState = GlobalDefines.taskState.tsDelete;

                //        //ApplicationLog.WriteInfo("There is kamatati Task，TaskID is " + capturingTask.taskContent.nTaskID.ToString()
                //        // + ",TaskName is " + capturingTask.taskContent.strTaskName);

                //        //ApplicationLog.WriteInfo("Delete task,TaskID is " + info.taskContent.nTaskID.ToString()
                //        //  + ",TaskName is " + info.taskContent.strTaskName);
                //    }
                //}

            
            }
            return true;
        }

        public async Task UpdateCustomMetadataAsync(int taskid, string metadata)
        {
            await Store.UpdateTaskCutomMetaDataAsync(taskid, metadata);
        }

        public async virtual Task<TResult> GetCustomMetadataAsync<TResult>(int taskid)
        {
            var f = await Store.GetTaskCustomMetaDataAsync(a => a.Where(b => b.Taskid == taskid), true);
            return _mapper.Map<TResult>(f);
        }

        public async Task<int> StopTask(int taskid, DateTime dt)
        {
            return await Store.StopTask(taskid, dt);
        }

        public async Task<int> DeleteTask(int taskid)
        {
            Logger.Info("DeleteTask " + taskid);
            return await Store.DeleteTask(taskid);
        }

        public async Task<int> SetTaskClassify(int taskid, string classify)
        {
            var taskinf = await Store.GetTaskAsync(a => a.Where(b => b.Taskid == taskid));
            taskinf.Category = classify;
            await Store.SaveChangeAsync();
            return taskid;
        }

        public async Task<int> SetTaskState(int taskid, int state)
        {
            var taskinfo = await Store.GetTaskAsync(a => a.Where(b => b.Taskid == taskid));

            if (taskinfo == null)
            {
                Logger.Info("SetTaskState error empty " + taskid);
                return 0;
            }

            //如果任务是周期任务的模版，只能是准备状态，或删除状态zmj2008-9-2
            if (taskinfo.OldChannelid <= 0 && taskinfo.Tasktype == (int)TaskType.TT_PERIODIC)
            {
                if (state == (int)taskState.tsComplete)
                {
                    SobeyRecException.ThrowSelfOneParam("ModifyTask match empty", GlobalDictionary.GLOBALDICT_CODE_TASK_IS_A_STENCIL_PLATE_TASK_OF_PERIODIC_TASKS_ONEPARAM, Logger, taskid, null);
                }
            }

            //任务原来就是删除状态不能再修改任务状态
            if (taskinfo.State == (int)taskState.tsDelete)
            {
                //row.TASKLOCK = string.Empty;
                return taskid;
            }

            //任务原来就是完成状态不能再修改任务状态
            if (taskinfo.State == (int)taskState.tsComplete)
            {
                //row.TASKLOCK = string.Empty;
                return taskid;
            }

            if (state == (int)taskState.tsInvaild)
            {
                taskinfo.Endtime = DateTime.Now;
            }
            taskinfo.State = state;
            //zmj 2010-11-22 不该把锁去掉，会导致再次被调用出来
            await Store.SaveChangeAsync();
            return taskid;
        }

        public async Task<int> TrimTaskBeginTime(int taskid, string StartTime)
        {
            var taskinfo = await Store.GetTaskAsync(a => a.Where(b => b.Taskid == taskid));

            if (taskinfo == null)
            {
                Logger.Info("TrimTaskBeginTime error empty " + taskid);
                SobeyRecException.ThrowSelfNoParam(taskid.ToString(), GlobalDictionary.GLOBALDICT_CODE_TASK_ID_DOES_NOT_EXIST,
                    Logger, null);
                return 0;
            }

            //对普通任务和可执行的周期任务做同样的操作，将开始时间换成当前时间zmj2008-9-1
            if (taskinfo.Tasktype != (int)TaskType.TT_PERIODIC || (taskinfo.Tasktype == (int)TaskType.TT_PERIODIC && taskinfo.OldChannelid > 0))
            {
                DateTime dtNow = new DateTime();
                if (string.IsNullOrEmpty(StartTime))
                {
                    dtNow = DateTime.Now;
                }
                else
                {
                    dtNow = DateTimeFormat.DateTimeFromString(StartTime);
                }

                if (taskinfo.Endtime == taskinfo.Starttime)
                {
                    taskinfo.Starttime = dtNow;
                    taskinfo.NewBegintime = dtNow;
                    taskinfo.Endtime = dtNow;
                    taskinfo.NewEndtime = dtNow;
                }
                else if (taskinfo.Endtime > dtNow && taskinfo.Tasktype != (int)TaskType.TT_VTRUPLOAD)
                {
                    taskinfo.Starttime = dtNow;
                    taskinfo.NewBegintime = dtNow;

                    Logger.Info(string.Format("In TrimTaskBeginTime,ENDTIME > dtNow && row.TASKTYPE != (int)TaskType.TT_VTRUPLOAD,TaskID = {0},StartTime = {1}", taskinfo.Taskid, dtNow.ToString()));
                }
                else if (taskinfo.Tasktype == (int)TaskType.TT_VTRUPLOAD)
                {
                    TimeSpan ts = taskinfo.Endtime - taskinfo.Starttime;
                    taskinfo.Starttime = dtNow;
                    taskinfo.NewBegintime = dtNow;

                    if (await Store.AdjustVtrUploadTasksByChannelId(taskinfo.Channelid.GetValueOrDefault(), taskinfo.Taskid, dtNow))
                    {
                        taskinfo.Endtime = dtNow.Add(ts);
                        taskinfo.NewEndtime = taskinfo.Endtime;
                    }

                    Logger.Info(string.Format("In TrimTaskBeginTime,row.TASKTYPE == (int)TaskType.TT_VTRUPLOAD,TaskID = {0},StartTime = {1}", taskinfo.Taskid, dtNow.ToString()));
                }
                else
                {
                    //zmj 2011-03-22 snp4100034097
                    if (dtNow >= taskinfo.Endtime)
                    {
                        TimeSpan ts = taskinfo.Endtime - taskinfo.Starttime;

                        taskinfo.Starttime = dtNow;
                        taskinfo.NewBegintime = dtNow;
                        taskinfo.Endtime = dtNow.Add(ts);
                        taskinfo.NewEndtime = taskinfo.Endtime;
                    }

                    Logger.Info(string.Format("In TrimTaskBeginTime,Other,TaskID = {0},StartTime = {1},STARTTIME = {2},ENDTIME = {3}", taskinfo.Taskid, dtNow.ToString(), taskinfo.Starttime.ToString(), taskinfo.Endtime.ToString()));
                }

                await Store.SaveChangeAsync();
            }
            return taskid;
        }

        public async Task<List<TResult>> QueryTaskContent<TResult>(int unitid, DateTime day, TimeLineType timetype)
        {
            return _mapper.Map<List<TResult>>(await Store.GetTaskListWithMode(1, day, timetype));
        }

        public async Task<TaskSource> GetTaskSource(int taskid)
        {
            return (TaskSource)(await Store.GetTaskSourceAsync(a => a.Where(b => b.Taskid == taskid).Select(x => x.Tasksource), true));
        }

        public async Task<List<TResult>> GetNeedSynTasksNew<TResult>()
        {
            var lstfinishtask = await Store.GetNeedFinishTasks();
            var lstunsyntask = await Store.GetNeedUnSynTasks();

            Logger.Info("GetNeedFinishTasks {0} ", string.Join(",", lstfinishtask.Select(x=>x.Taskid).ToList()));
            Logger.Info("GetNeedUnSynTasks {0} ", string.Join(",", lstunsyntask.Select(x => x.Taskid).ToList()));

            List<DbpTask> lstunsync = new List<DbpTask>();
            List<DbpTask> lstModify = new List<DbpTask>();
            //周期任务快要开始的时候，就把周期任务复制一条出来，任务类型是当天的任务，有效期只有一天,OldChannelID用来记载关联的原周期任务
            //然后把原来的周期任务的时间变为下一次执行的日期      
            foreach (var item in lstunsyntask)
            {
                if ((
                        (item.Tasktype == (int)TaskType.TT_NORMAL && item.Backtype == (int)CooperantType.emPureTask)
                        || item.Tasktype == (int)TaskType.TT_LOOP
                    )
                    && item.OpType != (int)opType.otDel)
                {
                    //如果今天日期已经超过当前任务的结束日期，把任务状态设置成taskState.tsInvaild
                    if (DateTime.Now > item.NewEndtime)
                    {
                        item.SyncState = (int)syncState.ssSync;//过期的任务置为已同步状态。
                        item.Tasklock = string.Empty;//解锁

                        lstModify.Add(item);
                        //zmj2009-01-09过期任务置无效后，就不应该被返回到任务总控那边进行操作
                        continue;
                    }
                }
                lstunsync.Add(item);
            }

            await Store.UpdateTaskListAsync(lstModify);

            lstunsync.AddRange(lstfinishtask);
            
            return _mapper.Map<List<TResult>>(lstunsync);

        }

        public async Task CompleteSynTasks<T>(T reqq)
        {
            CompleteSyncTaskRequest req = _mapper.Map<CompleteSyncTaskRequest>(reqq);

            var taskinfo = await Store.GetTaskAsync(a => a.Where(b => b.Taskid == req.TaskID));

            if (taskinfo == null)
            {
                Logger.Error("CompleteSynTasks no find" + req.TaskID);
                SobeyRecException.ThrowSelfNoParam("",GlobalDictionary.GLOBALDICT_CODE_TASK_ID_DOES_NOT_EXIST, Logger, null);
            }

            if (req.DispatchState >= 0)
            {
                taskinfo.DispatchState = req.DispatchState;
                if (!req.IsFinish && req.DispatchState == (int)dispatchState.dpsRedispatch)
                {
                    uint uUnit = (uint)taskinfo.Recunitid;
                    uUnit |= 0x80010000;
                    taskinfo.Recunitid = (int)uUnit;
                }
            }

            if (req.TaskState >= 0)
            {
                //任务原来就是完成状态只能修改为删除状态
                if (taskinfo.State == (int)taskState.tsComplete)
                {
                    if (req.TaskState == (int)taskState.tsDelete)
                    {
                        taskinfo.State = (int)taskState.tsDelete;
                    }
                }
                else
                {
                    //任务原来就是删除状态不能再修改任务状态
                    if (taskinfo.State != (int)taskState.tsDelete)
                        taskinfo.State = req.TaskState;
                }
            }

            if (req.SynState >= 0)
                taskinfo.SyncState = req.SynState;
            //解锁
            taskinfo.Tasklock = string.Empty;

            if (taskinfo.Tasktype != (int)TaskType.TT_PERIODIC)
            {
                taskinfo.Category = "A";
                taskinfo.NewBegintime = taskinfo.Starttime;
                taskinfo.NewEndtime = taskinfo.Endtime;
            }
            else
            {
                //DateTime NewBeginTime = taskinfo.NewBegintime;
                //DateTime NewEndTime = taskinfo.NewEndtime;
                //DateTime oldStart = taskinfo.Starttime;
                //DateTime oldEnd = taskinfo.Endtime;
                //DateTime modiStart = GlobalFun.DateTimeFromString(taskInfo.taskContent.strBegin);
                //DateTime modiEnd = GlobalFun.DateTimeFromString(taskInfo.taskContent.strEnd);

                //DateTime nowAllStart = new DateTime(oldStart.Year, oldStart.Month, oldStart.Day, modiStart.Hour, modiStart.Minute, modiStart.Second);
                //DateTime nowAllEnd = new DateTime(oldEnd.Year, oldEnd.Month, oldEnd.Day, modiEnd.Hour, modiEnd.Minute, modiEnd.Second);
                
                //bool bIsValid = true;
                //System.Text.StringBuilder traceSB = new System.Text.StringBuilder();
                //if (nowAllStart > nowAllEnd) //已经无效了!
                //{
                //    bIsValid = false;
                //}

                //if (NewBeginTime.Date > nowAllEnd.Date)
                //{
                //    bIsValid = false;
                //}

                //if (bIsValid)
                //{
                //    taskInfo.strNewBeginTime = GlobalFun.DateTimeToString(NewBeginTime);
                //    taskInfo.strNewEndTime = GlobalFun.DateTimeToString(NewEndTime);
                //    taskInfo.taskContent.strBegin = GlobalFun.DateTimeToString(NewBeginTime);
                //    taskInfo.taskContent.strEnd = GlobalFun.DateTimeToString(new DateTime(oldEnd.Year, oldEnd.Month, oldEnd.Day, NewEndTime.Hour, NewEndTime.Minute, NewEndTime.Second));
                //}
                //else //无效,可以删除了
                //{
                //    taskInfo.taskContent.strBegin = GlobalFun.DateTimeToString(nowAllStart);
                //    taskInfo.taskContent.strEnd = GlobalFun.DateTimeToString(nowAllEnd);
                //    taskInfo.taskContent.emState = (int)taskState.tsDelete;
                //}
            }
            await Store.SaveChangeAsync();
        }

        public async Task<List<int>> StopGroupTaskAsync(int taskid)
        {
            var f = await Store.GetTaskMetaDataAsync(a => a.Where(b => b.Taskid == taskid && b.Metadatatype == (int)MetaDataType.emContentMetaData));

            try
            {
                XDocument xd = new XDocument();
                xd = XDocument.Load(f.Metadatalong);

                var content = xd.Element("TaskContentMetaData");
                int groupcolor = int.Parse(content.Element("GroupColor").Value);

                var groupitems = content.Element("GroupItems").Elements();

                var channellist = groupitems.Select(l => int.Parse(l.Element("ItemData").Value)).ToList();

                return await Store.StopCapturingListChannelAsync(channellist);
            }
            catch (Exception e)
            {
                SobeyRecException.ThrowSelfNoParam(taskid.ToString(), GlobalDictionary.GLOBALDICT_CODE_FILL_GETTASKMETADATA_EXCEPTION, Logger, e);
            }
            return null;
        }

        public async Task<List<int>> DeleteGroupTaskAsync(int taskid)
        {
            var f = await Store.GetTaskMetaDataAsync(a => a.Where(b => b.Taskid == taskid && b.Metadatatype == (int)MetaDataType.emContentMetaData));

            try
            {
                XDocument xd = new XDocument();
                xd = XDocument.Load(f.Metadatalong);

                var content = xd.Element("TaskContentMetaData");
                int groupcolor = int.Parse(content.Element("GroupColor").Value);

                var groupitems = content.Element("GroupItems").Elements();

                var channellist = groupitems.Select(l => int.Parse(l.Element("ItemData").Value)).ToList();

                return await Store.DeleteCapturingListChannelAsync(channellist);
            }
            catch (Exception e)
            {
                SobeyRecException.ThrowSelfNoParam(taskid.ToString(), GlobalDictionary.GLOBALDICT_CODE_FILL_GETTASKMETADATA_EXCEPTION, Logger, e);
            }
            return null;
        }

        public async Task<int> GetTaskIDByTaskGUID(string taskguid)
        {
            var task = await Store.GetTaskAsync(a => a.Where(b => b.Taskguid.Equals(taskguid, StringComparison.OrdinalIgnoreCase)).Select(f => f.Taskid), true);
            if (task <= 0)
            {
                return await Store.GetTaskBackupAsync(a => a.Where(b => b.Taskguid.Equals(taskguid, StringComparison.OrdinalIgnoreCase)).Select(f => f.Taskid), true);
            }
            else
                return task;
        }

        public async Task<List<TResult>> GetAllChannelCapturingTask<TResult>()
        {
            return _mapper.Map<List<TResult>>(await Store.GetTaskListAsync(a => a.Where(b => b.State == (int)taskState.tsExecuting || b.State == (int)taskState.tsManuexecuting), true));

        }

        public async Task<TResult> GetChannelCapturingTask<TResult>(int channelid)
        {
            return _mapper.Map<TResult>(await Store.GetTaskAsync(a =>
            a.Where(b => b.Channelid == channelid && (b.State == (int)taskState.tsExecuting || b.State == (int)taskState.tsManuexecuting)), true));
        }
        public async Task ModifyTaskName(int taskid, string taskname)
        {
            var taskmeta = await Store.GetTaskMetaDataAsync(a => a.Where(b => b.Taskid == taskid && b.Metadatatype == (int)MetaDataType.emStoreMetaData));

            if (taskmeta != null && !string.IsNullOrEmpty(taskmeta.Metadatalong))
            {
                var meta = XElement.Parse(taskmeta.Metadatalong);
                var item = meta?.Descendants("TITLE").FirstOrDefault();
                if (item != null)
                {
                    item.Value = taskname;
                }
                taskmeta.Metadatalong = meta.ToString();
            }

            var findtask = await Store.GetTaskAsync(a => a.Where(b => b.Taskid == taskid));

            if (findtask.Tasktype == (int)TaskType.TT_VTRUPLOAD)
            {
                var vtrtask = await Store.GetVtrUploadTaskAsync(a => a.Where(b => b.Taskid == taskid));
                vtrtask.Taskname = taskname;
            }
            else
            {
                findtask.Taskname = taskname;
            }

            await Store.SaveChangeAsync();
        }

        public async Task<int> ModifyPeriodTask<TResult>(TResult taskmodify, bool isall)
        {
            
            if (isall)
            {
                var f = await ModifyTask<TResult>(taskmodify, string.Empty, string.Empty, string.Empty, string.Empty);
                return f.TaskID;
            }
            else
            {
                int newTaskId = -1;
                TaskContentRequest modifyinfo = _mapper.Map<TaskContentRequest>(taskmodify);

                //分出任务
                string strOldTaskClassify = modifyinfo.Classify;
                int nOrignalTaskID = modifyinfo.TaskID;
                string taskClassify = strOldTaskClassify;
                taskClassify += string.Format("[{0}]", modifyinfo.Begin);

                await Store.SetTaskClassify(modifyinfo.TaskID, modifyinfo.Classify, false);

                //添加新任务
                bool isNeedAppDate = false;
                string newTaskName = modifyinfo.TaskName;
                if (DateTimeFormat.DateTimeFromString(modifyinfo.Begin) == DateTime.MinValue)
                {
                    return 0;
                }

                var f = await Store.GetTaskMetaDataListAsync(a => a.Where(b => b.Taskid == modifyinfo.TaskID), true);
                string strCapatureMetaData = string.Empty, strStoreMetaData = string.Empty, strContentMetaData = string.Empty, strPlanMetaData = string.Empty, strSplitMetaData = string.Empty;
                foreach (var item in f)
                {
                    if (item.Metadatatype == (int)MetaDataType.emCapatureMetaData)
                    {
                        strCapatureMetaData = item.Metadatalong;
                    }
                    else if (item.Metadatatype == (int)MetaDataType.emContentMetaData)
                    {
                        strContentMetaData = item.Metadatalong;
                        GetPeriodTaskNewName(ref newTaskName, DateTimeFormat.DateTimeFromString(modifyinfo.Begin), true, ref strContentMetaData);
                    }
                    else if (item.Metadatatype == (int)MetaDataType.emStoreMetaData)
                    {
                        var root = XElement.Parse(item.Metadatalong);
                        if (root != null)
                        {
                            var itm = root.Descendants("TITLE").FirstOrDefault();
                            if (itm != null)
                            {
                                itm.Value = newTaskName;
                            }
                            strStoreMetaData = root.ToString();
                        }
                        else
                            strStoreMetaData = item.Metadatalong;

                    }
                    else if (item.Metadatatype == (int)MetaDataType.emPlanMetaData)
                    {
                        strPlanMetaData = item.Metadatalong;
                    }
                    else if (item.Metadatatype == (int)MetaDataType.emSplitData)
                    {
                        strSplitMetaData = item.Metadatalong;
                    }
                }

                modifyinfo.TaskID = -1;
                modifyinfo.TaskName = newTaskName;
                modifyinfo.Classify = "A";
                modifyinfo.State = (int)taskState.tsReady;
                modifyinfo.TaskType = (int)TaskType.TT_NORMAL;
                modifyinfo.TaskGUID = string.Empty;
                modifyinfo.TaskDesc = string.Empty;

                //这里自己处理异常，如果添加新任务失败了，那么之前的修改要复原
                try
                {
                    //newTaskId = AddTaskWithoutPolicy(taskModify, TaskSource.emUnknowTask, metadatas, false, null);
                    var addinfo = new TaskInfoRequest();
                    addinfo.BackUpTask = false;
                    addinfo.TaskSource = TaskSource.emUnknowTask;
                    addinfo.TaskContent = modifyinfo;
                    var backinfo = await AddTaskWithPolicy(addinfo, false, strCapatureMetaData, strContentMetaData, strStoreMetaData, strPlanMetaData);
                    return backinfo.TaskID;
                }
                catch (SobeyRecException e)
                {
                    //ApplicationLog.WriteInfo(String.Format("GLOBALDICT_CODE in ModifyPeriodTask, err:{0}", e.Message));
                    await Store.SetTaskClassify(nOrignalTaskID, strOldTaskClassify, true);
                    newTaskId = -1;

                    //继续外抛出去
                    throw e;
                }

            }
            return 0;
        }
        public async Task<TaskContentResponse> ModifyTask<TResult>(TResult task, string CaptureMeta, string ContentMeta, string MatiralMeta, string PlanningMeta)
        {
            var taskModify = _mapper.Map<TaskContentRequest>(task);

            if (DateTimeFormat.DateTimeFromString(taskModify.End) < DateTimeFormat.DateTimeFromString(taskModify.Begin))
            {
                SobeyRecException.ThrowSelfNoParam("ModifyTask time empty", GlobalDictionary.GLOBALDICT_CODE_TASK_END_TIME_IS_SMALLER_THAN_BEING_TIME, Logger, null);
            }

            var findtask = await Store.GetTaskAsync(a => a.Where(b => b.Taskid == taskModify.TaskID));

            if (findtask == null)
            {
                SobeyRecException.ThrowSelfNoParam("ModifyTask findtask empty", GlobalDictionary.GLOBALDICT_CODE_TASKSET_IS_NULL, Logger, null);
            }

            //如果是已分发、已同步的任务，不允许改变通道
            if (findtask.Channelid != taskModify.ChannelID
                && findtask.DispatchState == (int)dispatchState.dpsDispatched
                && findtask.SyncState == (int)syncState.ssSync)
            {
                await Store.UnLockTask(findtask.Taskid);
                SobeyRecException.ThrowSelfNoParam("ModifyTask findtask empty", GlobalDictionary.GLOBALDICT_CODE_TASK_IS_LOCKED, Logger, null);
            }

            //如果是改变了信号源或者通道，判断一下信号源和通道是不是匹配的
            bool match = false;
            if (findtask.Channelid != taskModify.ChannelID || findtask.Signalid != taskModify.SignalID)
            {
                
                if (_deviceInterface != null)
                {
                    
                    var response1 = await _deviceInterface.GetDeviceCallBack(new DeviceInternals()
                    {
                        funtype = IngestDBCore.DeviceInternals.FunctionType.ChannelInfoBySrc,
                        SrcId = taskModify.SignalID,
                        Status = 1
                    });

                    if (response1.Code != ResponseCodeDefines.SuccessCode)
                    {
                        Logger.Error("GetMatchedChannelForSignal ChannelInfoBySrc error");
                        return null;
                    }

                    var fresponse = response1 as ResponseMessage<List<CaptureChannelInfoInterface>>;
                    if (fresponse != null && fresponse.Ext?.Count > 1)
                    {
                        if (fresponse.Ext.Any(x => x.ID == taskModify.ChannelID))
                            match = true;
                    }
                }

                if (!match)
                {
                    await Store.UnLockTask(findtask.Taskid);
                    SobeyRecException.ThrowSelfNoParam("ModifyTask match empty", GlobalDictionary.GLOBALDICT_CODE_SIGNAL_AND_CHANNEL_IS_MISMATCHED, Logger, null);
                }
            }

            //如果是手动任务改成自动任务，强行改成已调度已同步状态
            if (findtask.Tasktype == (int)TaskType.TT_MANUTASK && taskModify.TaskType == TaskType.TT_NORMAL)
            {
                findtask.SyncState = (int)syncState.ssSync;
                findtask.DispatchState = (int)dispatchState.dpsDispatched;
            }

            if (findtask.Tasktype == (int)TaskType.TT_VTRUPLOAD)
            {
                var vtrtask = await Store.GetVtrUploadTaskAsync(a => a.Where(b => b.Taskid == findtask.Taskid), true);
                if (vtrtask == null)
                {
                    await Store.UnLockTask(findtask.Taskid);
                    SobeyRecException.ThrowSelfOneParam("ModifyTask vtrtask empty", GlobalDictionary.GLOBALDICT_CODE_CAN_NOT_FIND_THE_TASK_ONEPARAM, Logger, findtask.Taskid, null);
                }

                VTRTimePeriods vtrFreeTimePeriods = new VTRTimePeriods(vtrtask.Vtrid.GetValueOrDefault());
                vtrFreeTimePeriods.Periods = new List<TimePeriod>();
                await GetFreeTimePeriodByVtrId(vtrFreeTimePeriods, findtask.Taskid, DateTimeFormat.DateTimeFromString(taskModify.Begin), vtrtask);

                TimePeriod tp = new TimePeriod(vtrtask.Vtrid.GetValueOrDefault(), DateTimeFormat.DateTimeFromString(taskModify.Begin), DateTimeFormat.DateTimeFromString(taskModify.End));
                if (!IsTimePeriodInVTRTimePeriods(tp, vtrFreeTimePeriods))
                {
                    //VTRDetailInfo vtrInfo = new VTRDetailInfo();
                    //vtrInfo = vtrOper.GetVTRDetailInfoByID(vtrTasks[0].nVtrId);
                    await Store.UnLockTask(findtask.Taskid);
                    //SobeyRecException.ThrowSelf(Locallanguage.LoadString(vtrInfo.szVTRDetailName + " has been used by other tasks"), 3);
                    SobeyRecException.ThrowSelfOneParam("ModifyTask match empty", GlobalDictionary.GLOBALDICT_CODE_VTR_HAS_BEEN_USED_BY_OTHER_TASKS_ONEPARAM, Logger, vtrtask.Vtrid, null);
                }
            }

            DateTime modifybegin = DateTimeFormat.DateTimeFromString(taskModify.Begin);
            DateTime modifyend = DateTimeFormat.DateTimeFromString(taskModify.End);

            //List<TaskContentResponse> lsttask = null;
            //看修改的时间是否冲突,如果是周期任务，传入真实的beginTime.EndTime
            if (taskModify.TaskType != TaskType.TT_PERIODIC)
            {
                //获取冲突任务列表
                //if (conflictContent.Length == 1 && conflictContent[0].nTaskID == info.taskContent.nTaskID)
                //{

                //}
                //else
                //{
                //    TASKOPER.UnlockTask(taskModify.nTaskID);
                //    //SobeyRecException.ThrowSelf(Locallanguage.LoadString("Can Not Modify Time,Conflict Tasks:")
                //    //    +GetTaskDesc(conflictContent),2);
                //    SobeyRecException.ThrowSelf(string.Format(GlobalDictionary.Instance.GetMessageByCode(GlobalDictionary.GLOBALDICT_CODE_CAN_NOT_MODIFY_TIME_CONFLICT_TASKS),
                //                GetTaskDesc(conflictContent)), GlobalDictionary.GLOBALDICT_CODE_CAN_NOT_MODIFY_TIME_CONFLICT_TASKS);
                //}
                List<int> chl = new List<int>() { taskModify.ChannelID };
                var freelst = await Store.GetFreeChannels(chl, modifybegin, modifyend);
                if (freelst == null || freelst.Count < 1)
                {
                    await Store.UnLockTask(findtask.Taskid);
                    SobeyRecException.ThrowSelfOneParam("ModifyTask GetFreeChannels empty",
                        GlobalDictionary.GLOBALDICT_CODE_CAN_NOT_MODIFY_TIME_CONFLICT_TASKS_ONEPARAM, Logger, taskModify.TaskID, null);

                }
            }
            else
            {
                //不论是否为分出来的还是不是分出来的任务，都要进行冲突判断
                if (findtask.OldChannelid <= 0)
                {
                    /*
                     * @brief 老版本会跨天判断，并多次查询，跨天的会分俩次查询，我大胆使用新的代码
                     */
                    if (findtask.Starttime.Date == findtask.Endtime.Date)
                    {
                    }

                    List<int> chl = new List<int>() { taskModify.ChannelID };

                    DateTime dtTmpModiStart = new DateTime(findtask.Starttime.Year, findtask.Starttime.Month, findtask.Starttime.Day,
                            modifybegin.Hour, modifybegin.Minute, modifybegin.Second);
                    DateTime dtTmpModiEnd = new DateTime(findtask.Endtime.Year, findtask.Endtime.Month, findtask.Endtime.Day,
                        modifyend.Hour, modifyend.Minute, modifyend.Second);

                    var freelst = await Store.GetFreePerodiChannels(chl, taskModify.TaskID, taskModify.Unit, taskModify.SignalID,
                        taskModify.ChannelID, taskModify.Classify, dtTmpModiStart, dtTmpModiEnd);

                    if (freelst == null || freelst.Count < 1)
                    {
                        await Store.UnLockTask(findtask.Taskid);
                        SobeyRecException.ThrowSelfOneParam("ModifyTask GetFreePerodiChannels1 empty",
                            GlobalDictionary.GLOBALDICT_CODE_CAN_NOT_MODIFY_TIME_CONFLICT_TASKS_ONEPARAM, Logger, taskModify.TaskID, null);

                    }
                }
                else
                {

                    if (modifyend - modifybegin > new TimeSpan(0, 23, 59, 59))
                    {
                        await Store.UnLockTask(findtask.Taskid);
                        SobeyRecException.ThrowSelfNoParam("ModifyTask match over 24", GlobalDictionary.GLOBALDICT_CODE_TASK_TIME_IS_OVER_24_HOURS, Logger, null);

                    }

                    List<int> chl = new List<int>() { taskModify.ChannelID };
                    var freelst = await Store.GetFreePerodiChannels(chl, taskModify.TaskID, taskModify.Unit, taskModify.SignalID,
                        taskModify.ChannelID, taskModify.Classify, modifybegin, modifyend);

                    if (freelst == null || freelst.Count < 1)
                    {
                        await Store.UnLockTask(findtask.Taskid);
                        SobeyRecException.ThrowSelfOneParam("ModifyTask GetFreePerodiChannels2 empty",
                            GlobalDictionary.GLOBALDICT_CODE_CAN_NOT_MODIFY_TIME_CONFLICT_TASKS_ONEPARAM, Logger, taskModify.TaskID, null);

                    }
                }
            }

            findtask.Recunitid = taskModify.Unit;
            findtask.Taskname = taskModify.TaskName;
            findtask.Description = taskModify.TaskDesc;
            findtask.Channelid = taskModify.ChannelID;
            findtask.Signalid = taskModify.SignalID;


            //对于周期任务，不允许改变任务日期
            if (findtask.Tasktype == (int)TaskType.TT_PERIODIC && findtask.OldChannelid <= 0)
            {
                findtask.Starttime = new DateTime(findtask.Starttime.Year, findtask.Starttime.Month, findtask.Starttime.Day,
                    modifybegin.Hour, modifybegin.Minute, modifybegin.Second);
                findtask.Endtime = new DateTime(findtask.Endtime.Year, findtask.Endtime.Month, findtask.Endtime.Day,
                    modifyend.Hour, modifyend.Minute, modifyend.Second);
            }
            else
            {
                TimeSpan span = findtask.Starttime - DateTime.Now;
                int tkState = findtask.State.GetValueOrDefault();
                //如果任务已经或者即将开始，不允许改变开始时间
                if ((tkState == (int)taskState.tsExecuting || tkState != (int)taskState.tsReady || tkState != (int)taskState.tsManuexecuting)
                    && span.TotalSeconds < 30)
                {
                    //原有的开始时间不等于现有的开始时间，但是长度相等，表明是拖动整体移动，不能修改时间，
                    //否则可以更改结束时间
                    TimeSpan tmpSpan1 = findtask.Endtime - findtask.Starttime;
                    TimeSpan tmpSpan2 = modifyend - modifybegin;
                    if (findtask.Starttime != modifybegin && tmpSpan1 == tmpSpan2)
                    {
                        /*
                         * @brief 这里会判断冲突，我去掉了，因为感觉没有必要
                         */
                    }
                    else
                        findtask.Endtime = modifyend;
                }
                else
                {
                    findtask.Starttime = modifybegin;
                    findtask.Endtime = modifyend;
                }
            }

            findtask.Tasktype = (int)taskModify.TaskType;
            findtask.Backupvtrid = taskModify.BackupVTRID;
            findtask.Backtype = (int)taskModify.CooperantType;
            findtask.Stampimagetype = taskModify.StampImageType;
            findtask.Tasklock = string.Empty;

            try
            {
                return _mapper.Map<TaskContentResponse>(await Store.ModifyTask(findtask, false, CaptureMeta, ContentMeta, MatiralMeta, PlanningMeta));
            }
            catch (Exception e)
            {
                await Store.UnLockTask(findtask.Taskid);
                SobeyRecException.ThrowSelfNoParam("ModifyTask match ModifyTask", GlobalDictionary.GLOBALDICT_CODE_SET_TASKMETADATA_FAIL, Logger, e);

            }
            return null;

        }

        public async Task<TResult> IsVTRCollide<TResult>(int VTR_ID, string begintime, string endtime, int TaskID)
        {
            //返回true，证明有冲突
            //返回false,证明没有冲突
            DateTime BeginTime = DateTimeFormat.DateTimeFromString(begintime);
            //zmj2009-02-05如果开始时间小于当前时间，那么就以当前时间做为开始时间
            if (BeginTime <= DateTime.Now)
            {
                BeginTime = DateTime.Now;
            }
            DateTime EndTime = DateTimeFormat.DateTimeFromString(endtime);
            //zmj2009-09-22增加开始时间和结束时间的范围，相当于拉大间距
            BeginTime = BeginTime.AddSeconds(-1);
            EndTime = EndTime.AddSeconds(1);

            var tc_fristday = await Store.GetTaskListWithMode(1, BeginTime.AddDays(-1), TimeLineType.em24HourDay);
            var tc_secondday = await Store.GetTaskListWithMode(1, BeginTime, TimeLineType.em24HourDay);
            tc_fristday.AddRange(tc_secondday);
            if (BeginTime.Date != EndTime.Date)//证明是跨天的，需要查询第三天的任务
            {
                var TC_ThirdDay = await Store.GetTaskListWithMode(1, BeginTime.AddDays(1), TimeLineType.em24HourDay);
                tc_fristday.AddRange(TC_ThirdDay);
            }

            //List<DbpTask> lsttask = new List<DbpTask>();
            DbpTask mintask = new DbpTask() { Starttime = DateTime.MinValue};
            bool isExistVTRCollide = false;
            foreach (var item in tc_fristday)
            {
                if (item.Backupvtrid!= VTR_ID)
                {
                    continue;//如果任务的VTRID不符，则跳过
                }
                if (TaskID == item.Taskid
                    || TaskID == item.Taskid * (-1))
                {
                    continue;//zmj2008-12-11排除掉与自己冲突
                }
                if (item.Backtype == (int)CooperantType.emVTRBackupFailed)
                {
                    continue;//zmj2008-12-17排队VTR失败的任务
                }
                if (item.State == (int)taskState.tsExecuting ||//正在执行的任务，包括了正在执行的手动任务
                    item.State == (int)taskState.tsReady ||//未调度，正准备的任务
                    item.State == (int)taskState.tsPause ||//暂停的任务
                    item.State == (int)taskState.tsInvaild)//zmj2009-02-06任务无效，仅仅是MSV无效，也有可能备份VTR有效
                {
                    DateTime TC_BeginTime = item.Starttime;
                    DateTime TC_EndTime = item.Endtime;

                    if (item.Tasktype == (int)TaskType.TT_MANUTASK)
                    {
                        TC_EndTime = TC_BeginTime.AddDays(1);//手动任务没有结束时间的，将结束时间设为开始日间之后的24小时
                    }


                    if ((TC_BeginTime >= BeginTime && TC_BeginTime <= EndTime) ||
                        (TC_EndTime >= BeginTime && TC_EndTime <= EndTime) ||
                        (TC_BeginTime <= BeginTime && TC_EndTime >= EndTime))
                    {
                        //zmj2008-12-9 增加日志对VTR冲突进行定位
                        //						string strTemp = string.Format("In IsVTRCollide,TaskID is {0} ,TaskName is {1},BeginTime is {2},EndTime is {3},TaskType is {4},BackupVTRID is {5},TaskState is {6}",
                        //							CollideTaskContent.nTaskID,CollideTaskContent.strTaskName,CollideTaskContent.strBegin,CollideTaskContent.strEnd,CollideTaskContent.emTaskType,CollideTaskContent.nBackupVTRID,CollideTaskContent.emState);
                        //                      ApplicationLog.WriteInfo(strTemp);
                        isExistVTRCollide = true;
                        if (item.Starttime > mintask.Starttime)
                        {
                            mintask = item;
                        }
                        //lsttask.Add(item);
                    }
                }
                else
                {
                    continue;//如果是其他状态的任务也排除掉，如状态为为4（被删除）
                }
            }

            if (isExistVTRCollide)
            {
                return _mapper.Map<TResult>(mintask);
            }
            return default(TResult);
        }

        public async Task WriteVTRUploadTaskDB<TResult>(TResult taskAdd)
        {
            TaskContentRequest taskinfo = _mapper.Map<TaskContentRequest>(taskAdd);

            
            if (_deviceInterface != null)
            {
                var response1 = await _deviceInterface.GetDeviceCallBack(new DeviceInternals() {
                    funtype = IngestDBCore.DeviceInternals.FunctionType.ChannelUnitMap, ChannelId = taskinfo.ChannelID
                });

                if (response1.Code != ResponseCodeDefines.SuccessCode)
                {
                    Logger.Error("WriteVTRUploadTaskDB ChannelUnitMap error");
                    return;
                }
                var fr = response1 as ResponseMessage<int>;

                taskinfo.Unit = fr.Ext;

                await Store.DeleteTaskDB(taskinfo.TaskID, false);

                await Store.AddTaskWithPolicys(_mapper.Map<TaskContentRequest, DbpTask>(taskinfo, opt =>
                opt.AfterMap((src, des) =>
                {
                    des.State = (int)taskState.tsReady;
                    des.SyncState = (int)syncState.ssSync;
                    des.DispatchState = (int)dispatchState.dpsDispatched;
                })
                ), false, TaskSource.emMSVUploadTask, string.Empty, string.Empty, string.Empty, string.Empty, null);
            }

        }

        private bool IsTimePeriodInVTRTimePeriods(TimePeriod tp, VTRTimePeriods vtrFreeTimePeriods)
        {
            if (vtrFreeTimePeriods.Periods != null)
            {
                foreach (TimePeriod vftp in vtrFreeTimePeriods.Periods)
                {
                    if (vftp.EndTime >= tp.EndTime && vftp.StartTime <= tp.StartTime)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public async Task<bool> UnlockAllTasks()
        {
            await Store.UnLockAllTask();
            return await SetPeriodTaskToNextTime();
        }

        //zmj2008-11-28增加新功能，是否需要修改周期任务名
        private async Task<string> IsNeedModPeriodicTaskName(int nTaskID)
        {
            var metadata = await Store.GetTaskMetaDataAsync(a => a.Where(b => b.Taskid == nTaskID && b.Metadatatype == (int)MetaDataType.emContentMetaData), true);

            if (metadata != null && metadata.Metadatalong != string.Empty)
            {
                var f = XElement.Parse(metadata.Metadatalong);
                var node = f.Descendants("APPDATE").FirstOrDefault();
                //XmlNode node = doc.SelectSingleNode("/TaskContentMetaData/PERIODPARAM/APPDATE");
                if (node != null)
                {
                    if (node.Value != string.Empty)
                    {
                        if (node.Value == "1")
                        {
                            return metadata.Metadatalong;
                        }
                    }
                }
            }

            return "";
        }

        private bool GetPeriodTaskNewName(ref string newTaskName, DateTime dtBegin, bool isNeedDeletePeriodTaskParam, ref string strTaskContentMetadata)
        {
            bool isNeedAppDate = false;
            var root = XElement.Parse(strTaskContentMetadata);

            //XmlNode node = doc.SelectSingleNode("/TaskContentMetaData/PERIODPARAM/APPDATE");
            var node = root.Descendants("APPDATE").FirstOrDefault();
            if (node != null)
            {
                if (node.Value != string.Empty)
                {
                    if (node.Value == "1")
                    {
                        isNeedAppDate = true;
                        string appDateString = dtBegin.ToString("MM-dd-yyyy HH_mm_ss");
                        var appDateFormatNode = root.Descendants("APPDATEFORMAT").FirstOrDefault();//doc.SelectSingleNode("/TaskContentMetaData/PERIODPARAM/APPDATEFORMAT");
                        if (appDateFormatNode != null && !string.IsNullOrEmpty(appDateFormatNode.Value))
                        {
                            try
                            {
                                appDateString = dtBegin.ToString(appDateFormatNode.Value);
                            }
                            catch (System.FormatException ex)
                            {
                                throw ex;
                            }
                            catch (System.Exception ex)
                            {
                                throw ex;
                            }
                        }

                        if (string.IsNullOrEmpty(appDateString))
                        {
                            appDateString = dtBegin.ToString("MM-dd-yyyy HH_mm_ss");
                        }

                        newTaskName += "_" + appDateString;
                    }
                }

                if (isNeedDeletePeriodTaskParam)
                {
                    root.Descendants("PERIODPARAM").Remove();
                    strTaskContentMetadata = root.ToString();
                }
            }

            return isNeedAppDate;
        }

        public async Task<TResult> CreateNewTaskFromPeriodicTask<TResult>(int periodicTaskId)
        {
            var findtask = await Store.GetTaskAsync(a => a.Where(b => b.Taskid == periodicTaskId), true);
            TaskSource src = await GetTaskSource(periodicTaskId);

            findtask.Taskid = -1;
            string strmetadata = await IsNeedModPeriodicTaskName(periodicTaskId);

            string newTaskName = findtask.Taskname;
            if (!string.IsNullOrEmpty(strmetadata))
            {
                string strContentMetadata = "";
                strContentMetadata = strmetadata;//QueryTaskMetaData(periodicTaskId, MetaDataType.emContentMetaData,  strContentMetadata).Result;
                GetPeriodTaskNewName(ref newTaskName, findtask.NewBegintime, true, ref strContentMetadata);
                findtask.Taskname = newTaskName;
            }

            findtask.OldChannelid = findtask.Taskid;
            findtask.Taskguid = Guid.NewGuid().ToString("N"); //GUID
            findtask.Description = string.Empty;

            string Category = findtask.Category;

            findtask.Category = "D";
            //有效期只有当前执行的这一天
            findtask.Starttime = findtask.NewBegintime;
            findtask.Endtime = findtask.NewEndtime;
            //如果这天已经被禁用，就成为一个被删除的任务
            if (Store.IsInvalidPerodicTask(Category, findtask.Starttime))
            {
                findtask.State = (int)taskState.tsDelete;
            }

            //如果当天任务已经被分过了，不要分出下面一天的来，这里以1分钟为界
            if (DateTime.Now.AddMinutes(1) <= findtask.NewBegintime)
            {
                SobeyRecException.ThrowSelfNoParam("ModifyTask match ModifyTask", GlobalDictionary.GLOBALDICT_CODE_NEXT_TIME_IS_MORE_THAN_1_MINUTE, Logger, null);
            }

            bool isTaskInvaild = false;
            //如果今天日期已经超过当前新任务的结束日期，把任务状态设置成taskState.tsInvaild
            if (DateTime.Now > findtask.NewEndtime)
            {
                findtask.State = (int)taskState.tsInvaild;
                findtask.SyncState = (int)syncState.ssSync;//过期的任务置为已同步						
                isTaskInvaild = true;
            }
            //int[] policyids = null;
            //MetaDataPolicy[] arrPolicyMeta = PPLICYACCESS.GetPolicyByTaskID(periodicTaskId);
            //if (arrPolicyMeta.Length > 0)
            //{
            //    policyids = new int[arrPolicyMeta.Length];
            //    for (int i = 0; i < arrPolicyMeta.Length; i++)
            //    {
            //        policyids[i] = arrPolicyMeta[i].nID;
            //    }
            //}

            var lsttaskmeta = await Store.GetTaskMetaDataListAsync(a => a.Where(b => b.Taskid==periodicTaskId), true); ;
            string strCapatureMetaData = string.Empty, strStoreMetaData = string.Empty, strContentMetaData = string.Empty, strPlanMetaData = string.Empty, strSplitMetaData = string.Empty;
            foreach (var item in lsttaskmeta)
            {
                if (item.Metadatatype == (int)MetaDataType.emCapatureMetaData)
                {
                    strCapatureMetaData = item.Metadatalong;
                }
                else if (item.Metadatatype == (int)MetaDataType.emContentMetaData)
                {
                    strContentMetaData = item.Metadatalong;
                }
                else if (item.Metadatatype == (int)MetaDataType.emStoreMetaData)
                {
                    strStoreMetaData = item.Metadatalong;
                }
                else if (item.Metadatatype == (int)MetaDataType.emPlanMetaData)
                {
                    strPlanMetaData = item.Metadatalong;
                }
                else if (item.Metadatatype == (int)MetaDataType.emSplitData)
                {
                    strSplitMetaData = item.Metadatalong;
                }
            }

            if (!string.IsNullOrEmpty(strStoreMetaData))
            {
                var root = XElement.Parse(strStoreMetaData);
                if (root != null)
                {
                    var mate = root.Descendants("MATERIALID").FirstOrDefault();
                    if (mate != null && !string.IsNullOrEmpty(mate.Value))
                    {
                        mate.Value = string.Empty;
                    }
                    var title = root.Descendants("TITLE").FirstOrDefault();
                    if (title != null && !string.IsNullOrEmpty(title.Value))
                    {
                        title.Value = newTaskName;
                    }

                    strStoreMetaData = root.ToString();
                }
            }

            if (!string.IsNullOrEmpty(strContentMetaData))
            {
                var root = XElement.Parse(strContentMetaData);
                var content= root.Element("TaskContentMetaData");
                var offset = content?.Element("ContentTime")?.Element("OffsetDay");
                if (offset != null)
                {
                    int offsetday = int.Parse(offset.Value);

                    var contentbegin = content.Element("ContentTime")?.Element("BeginTime");
                    var contentend = content.Element("ContentTime")?.Element("EndTime");

                    var contbegin = DateTimeFormat.DateTimeFromString(contentbegin.Value);
                    var contend = DateTimeFormat.DateTimeFromString(contentend.Value);

                    //母任务的长度
                    TimeSpan ContentLen = contend - contbegin;

                    DateTime TaskTimeDay = new DateTime(findtask.Starttime.Year, findtask.Starttime.Month, findtask.Starttime.Day, 0, 0, 0);
                    DateTime ContentDay = TaskTimeDay.AddDays(-offsetday);

                    //加上Offset,根据Offset重新计算内容采集开始时间
                    DateTime TimeContentBeginNew = new DateTime(ContentDay.Year, ContentDay.Month, ContentDay.Day, contbegin.Hour, contbegin.Minute, contbegin.Second);
                    DateTime TimeContentEndNew = TimeContentBeginNew.Add(ContentLen);

                    contentbegin.Value = DateTimeFormat.DateTimeToString(TimeContentBeginNew);
                    contentend.Value = DateTimeFormat.DateTimeToString(TimeContentEndNew);

                    strContentMetaData = root.ToString();
                }
            }

            
            await Store.AddTaskWithPolicys(findtask, true, src, strCapatureMetaData, strContentMetaData, strStoreMetaData, strPlanMetaData, null);
            return _mapper.Map<TResult>(findtask);
        }

        public async Task<bool> SetPeriodTaskToNextTime()
        {
            var lsttask = await Store.GetTaskListAsync(a => a.Where(b => b.Tasktype == (int)TaskType.TT_PERIODIC && b.OldChannelid == 0
            && (b.State<4 || b.State>4)
            && b.NewBegintime<= DateTime.Now
            && b.Starttime < b.Endtime));

            if (lsttask == null || lsttask.Count <= 0)
            {
                return false;
            }

            foreach (var item in lsttask)
            {
                DateTime dtNewStart = new DateTime();
                DateTime dtNewEnd = new DateTime();
                Store.GetPerodicTaskNextExectueTime(item.NewBegintime, item.NewEndtime, item.Category, ref dtNewStart, ref dtNewEnd);

                item.NewBegintime = dtNewStart;
                item.NewEndtime = dtNewEnd;
            }

            await Store.UpdateTaskListAsync(lsttask);
            return true;
        }

        public async Task<int> UpdateComingTasks()
        {
            TaskCondition condition = new TaskCondition();
            var lst = await Store.GetTaskListAsync(c => c.Where(f => f.State == (int)syncState.ssSync
            && f.DispatchState == (int)dispatchState.dpsNotDispatch
            && (f.State != (int)taskState.tsDelete && f.State != (int)taskState.tsConflict && f.State != (int)taskState.tsInvaild)
            && (f.Starttime > DateTime.Now.AddHours(-24) && f.Starttime < DateTime.Now.AddHours(1))));


            if (lst != null && lst.Count > 0)
            {
                string log = string.Empty;
                lst.ForEach(a => { a.SyncState = (int)syncState.ssNot; a.DispatchState = (int)dispatchState.dpsDisabled; a.Tasklock = string.Empty; log += "," + a.Taskid; });

                Logger.Info("UpdateComingTasks {0} ", string.Join(",", log));

                await Store.UpdateTaskListAsync(lst);
                return lst.Count;
            }
            return 0;
        }

        public async Task<VTRTimePeriods> GetFreeTimePeriodByVtrId(VTRTimePeriods vtrFreeTimePeriods, int exTaskId, DateTime beginCheckTime, VtrUploadtask vtrtask)
        {
            if (beginCheckTime == DateTime.MinValue)
            {
                beginCheckTime = DateTime.Now;
            }

            vtrFreeTimePeriods.Periods.Clear();
            VTRTimePeriods vtrTimePeriods = new VTRTimePeriods();

            int vtrId = vtrFreeTimePeriods.VTRId;

            //查询手动的任务占用的VTR时间，已经在执行的

            if (vtrtask.Vtrtasktype == (int)VTRUPLOADTASKTYPE.VTR_MANUAL_UPLOAD
                && (vtrtask.Taskstate == (int)VTRUPLOADTASKSTATE.VTR_UPLOAD_COMMIT || vtrtask.Taskstate == (int)VTRUPLOADTASKSTATE.VTR_UPLOAD_PRE_EXECUTE
                || vtrtask.Taskstate == (int)VTRUPLOADTASKSTATE.VTR_UPLOAD_EXECUTE))
            {
                DateTime beginTime = vtrtask.Committime;
                TimeSpan tsDuration = new TimeSpan();
                if (vtrtask.Vtrtaskid > 0)//入点加长度
                {
                    tsDuration = new TimeSpan(0, 0, vtrtask.Trimoutctl.GetValueOrDefault() / vtrtask.Vtrtaskid.GetValueOrDefault());
                }
                else
                {
                    SB_TimeCode inSTC = new SB_TimeCode((uint)vtrtask.Triminctl);
                    SB_TimeCode outSTC = new SB_TimeCode((uint)vtrtask.Trimoutctl);
                    tsDuration = outSTC - inSTC;
                }

                DateTime endTime = beginTime + tsDuration;

                vtrTimePeriods.Periods.Add(new TimePeriod(vtrId, beginTime, endTime));
            }

            //查询计划任务，开始时间和结束时间，还未执行的
            List<TimePeriod> scheduleTPs = await Store.GetTimePeriodsByScheduleVBUTasks(vtrId, exTaskId);
            if (scheduleTPs != null && scheduleTPs.Count > 0)
            {
                vtrTimePeriods.Periods.AddRange(scheduleTPs);
            }

            DateTime thirdDay = new DateTime(beginCheckTime.Year, beginCheckTime.Month, beginCheckTime.Day, 23, 59, 59);
            thirdDay = thirdDay.AddDays(2);
            vtrFreeTimePeriods.Periods = GetFreeTimePeriodsByTieup(vtrId, vtrTimePeriods.Periods, beginCheckTime.AddSeconds(-3), thirdDay);

            //DropDurationIn3Sec(ref vtrFreeTimePeriods.Periods);
            for (int i = 0; i < vtrFreeTimePeriods.Periods.Count; i++)
            {
                if (vtrFreeTimePeriods.Periods[i].Duration.TotalSeconds <= 3)
                {
                    vtrFreeTimePeriods.Periods.RemoveAt(i);
                    i--;
                }
            }

            return vtrFreeTimePeriods;
        }

        public async Task<bool> StartTieupTask(int taskid)
        {
            var findtask = await Store.GetTaskAsync(a => a.Where(b => b.Taskid == taskid));

            if (findtask.Tasktype != (int)TaskType.TT_TIEUP)
            {
                return false;
            }

            findtask.Tasktype = (int)TaskType.TT_NORMAL;
            findtask.SyncState = (int)syncState.ssNot;
            findtask.DispatchState = (int)dispatchState.dpsDispatched;

            await Store.SaveChangeAsync();
            return true;
        }

        /// <summary>
        /// 从占位的时间段中获取到空闲的时间段
        /// </summary>
        /// <param name="id"></param>
        /// <param name="tieupTimePeriods"></param>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        internal List<TimePeriod> GetFreeTimePeriodsByTieup(int id, List<TimePeriod> tieupTimePeriods, DateTime beginTime, DateTime endTime)
        {
            List<TimePeriod> freeTimePeriods = new List<TimePeriod>();
            if (tieupTimePeriods == null || tieupTimePeriods.Count == 0)
            {
                freeTimePeriods.Add(new TimePeriod(id, beginTime, endTime));
            }
            else
            {
                tieupTimePeriods.Sort(TimePeriod.CompareAscByStartTime);

                for (int i = 0; i < tieupTimePeriods.Count; i++)
                {
                    if (i == 0)
                    {
                        if (tieupTimePeriods[i].StartTime > beginTime)
                        {
                            freeTimePeriods.Add(new TimePeriod(id, beginTime, tieupTimePeriods[i].StartTime));
                        }

                        //只有一个的情况下
                        if (i == tieupTimePeriods.Count - 1)
                        {
                            freeTimePeriods.Add(new TimePeriod(id, tieupTimePeriods[i].EndTime, endTime));
                        }
                    }
                    else
                    {
                        freeTimePeriods.Add(new TimePeriod(id, tieupTimePeriods[i - 1].EndTime, tieupTimePeriods[i].StartTime));

                        if (i == tieupTimePeriods.Count - 1)
                        {
                            if (tieupTimePeriods[i].EndTime < endTime)
                            {
                                freeTimePeriods.Add(new TimePeriod(id, tieupTimePeriods[i].EndTime, endTime));
                            }
                        }
                    }
                }
            }

            return freeTimePeriods;
        }

        public TaskContent ConvertTaskResponse(TaskContentResponse task)
        {
            return _mapper.Map<TaskContent>(task);
        }

        public async Task<TaskContentResponse> AutoAddTaskByOldTask(int oldtask, DateTime starttime)
        {
            var findtask = await Store.GetTaskAsync(a => a.Where(b => b.Taskid == oldtask));
            var newtaskinfo = Store.DeepClone(findtask);

            if (findtask != null)
            {
                if (findtask.Tasktype == (int)TaskType.TT_MANUTASK
                || findtask.Tasktype == (int)TaskType.TT_OPENEND)
                {
                    //yangchuang20111017原来直接不支持手动任务，如果是手动任务，这里可以重新创建一个OpenEnd类型的任务
                    newtaskinfo.Tasktype = (int)TaskType.TT_OPENEND;
                }
                else if (findtask.Endtime <= starttime.AddSeconds(5))
                {
                    // Delete by chenzhi 2013-08-01
                    // TODO: 这种情况是系统正常的处理过程，不应该作为错误处理

                    // 作为成功操作返回出去
                    return null;
                    // ----------------- The End 2013-08-01 -----------------
                }
            }

            if (findtask.Backtype == (int)CooperantType.emVTRBackup)
            {
                findtask.Backtype = (int)CooperantType.emVTRBackupFinish;
            }
            else if (findtask.Backtype == (int)CooperantType.emKamataki)
            {
                //Kamataki任务改成普通任务
                newtaskinfo.Backtype = (int)CooperantType.emPureTask;
            }

            // Add by chenzhi 2013-07-23
            // TODO: 如果原任务是一个周期任务的子任务，则需要在这改为普通任务

            if (findtask.Tasktype == (int)TaskType.TT_PERIODIC)
            {
                // 改为普通任务
                newtaskinfo.Tasktype = (int)TaskType.TT_NORMAL;
            }

            //MetaDataPolicy[] arrMetaDataPolicy = PPLICYACCESS.GetPolicyByTaskID(oldTaskID);
            //List<int> listPolicyId = new List<int>();
            //foreach (MetaDataPolicy item in arrMetaDataPolicy)
            //{
            //    listPolicyId.Add(item.nID);
            //}
            //int[] arrPolicyId = listPolicyId.ToArray();

            Store.StopTaskNoChange(findtask, starttime);

            newtaskinfo.Taskguid = Guid.NewGuid().ToString("N");
            newtaskinfo.Taskid = -1;
            newtaskinfo.Starttime = starttime.AddSeconds(1);
            newtaskinfo.NewBegintime = newtaskinfo.Starttime;
            newtaskinfo.Taskname += "_1";

            
            if (_deviceInterface != null)
            {
                var response1 = await _deviceInterface.GetDeviceCallBack(new DeviceInternals()
                {
                    funtype = DeviceInternals.FunctionType.SingnalIDByChannel,
                    ChannelId = findtask.Channelid.GetValueOrDefault()

                });
                if (response1.Code != ResponseCodeDefines.SuccessCode)
                {
                    Logger.Error("AutoAddTaskByOldTask SingnalIDByChannel error");
                    return null;
                }
                var fr = response1 as ResponseMessage<int>;
                if (fr != null && fr.Ext > 0)
                {
                    newtaskinfo.Signalid = fr.Ext;
                }
            }

            newtaskinfo.Tasklock = string.Empty;
            newtaskinfo.SyncState = (int)syncState.ssSync;
            newtaskinfo.DispatchState = (int)dispatchState.dpsNotDispatch;
            newtaskinfo.OpType = (int)opType.otAdd;
            newtaskinfo.Description = string.Empty;
            newtaskinfo.State = (int)taskState.tsReady;//先改成准备状态

            //OpenEnd任务的结束时间和开始时间一样
            if (newtaskinfo.Tasktype == (int)TaskType.TT_OPENEND)
            {
                newtaskinfo.Endtime = newtaskinfo.Starttime;
                newtaskinfo.NewEndtime = newtaskinfo.Starttime;
            }

            TaskSource src = await GetTaskSource(findtask.Taskid);

            var lsttaskmeta = await Store.GetTaskMetaDataListAsync(a => a.Where(b => b.Taskid == findtask.Taskid), true); ;
            string strCapatureMetaData = string.Empty, strStoreMetaData = string.Empty, strContentMetaData = string.Empty, strPlanMetaData = string.Empty, strSplitMetaData = string.Empty;
            foreach (var item in lsttaskmeta)
            {
                if (item.Metadatatype == (int)MetaDataType.emCapatureMetaData)
                {
                    strCapatureMetaData = await GetCaptureTemplateBySignalIdAndUserCode(newtaskinfo.Signalid.GetValueOrDefault(), false, findtask.Usercode);

                    if (string.IsNullOrEmpty(strCapatureMetaData))
                    {
                        strCapatureMetaData = item.Metadatalong;
                    }
                }
                else if (item.Metadatatype == (int)MetaDataType.emContentMetaData)
                {
                    strContentMetaData = item.Metadatalong;
                }
                else if (item.Metadatatype == (int)MetaDataType.emStoreMetaData)
                {
                    strStoreMetaData = item.Metadatalong;
                }
                else if (item.Metadatatype == (int)MetaDataType.emPlanMetaData)
                {
                    strPlanMetaData = item.Metadatalong;
                }
                else if (item.Metadatatype == (int)MetaDataType.emSplitData)
                {
                    strSplitMetaData = item.Metadatalong;
                }
            }

            if (!string.IsNullOrEmpty(strStoreMetaData))
            {
                //<MADEBYINGEST></MADEBYINGEST>这个玩意会妨碍xml解析，居然有这个玩意，日了
                var root = XElement.Parse(strStoreMetaData);
                if (root != null)
                {
                    var ma = root.Element("MATERIAL");
                    if (ma != null)
                    {
                        var t = ma.Element("TITLE");
                        if (t != null) t.Value = newtaskinfo.Taskname;
                        var m = ma.Element("MATERIALID");
                        if (m != null) m.Value = string.Empty;
                    }
                }
            }

            if (!string.IsNullOrEmpty(strContentMetaData))
            {
                var root = XElement.Parse(strContentMetaData);
                root.Descendants("RealStampIndex").Remove();
                root.Descendants("PERIODPARAM").Remove();
            }

            var info = await Store.AddTaskWithPolicys(findtask, true, src,
                                                        strCapatureMetaData,
                                                        strContentMetaData,
                                                        strStoreMetaData,
                                                        strPlanMetaData, null);

            return _mapper.Map<TaskContentResponse>(info);
        }

        
       

        public async Task<TaskContentResponse> AddTaskWithPolicy<TResult>(TResult info, bool backup, string CaptureMeta, string ContentMeta, string MatiralMeta, string PlanningMeta)
        {
            var taskinfo = _mapper.Map<TaskInfoRequest>(info);

            //var _globalinterface = ApplicationContext.Current.ServiceProvider.GetRequiredService<IIngestDeviceInterface>();

            TaskSource ts = TaskSource.emUnknowTask;
            if (backup)
            {

                if (_deviceInterface != null)
                {
                    // 获得备份信号源信息
                    var response1 = await _deviceInterface.GetDeviceCallBack(new DeviceInternals() {
                        funtype = IngestDBCore.DeviceInternals.FunctionType.BackSignalByID, SrcId = taskinfo.TaskContent.SignalID
                    });

                    if (response1.Code != ResponseCodeDefines.SuccessCode)
                    {
                        Logger.Error("AddTaskWithPolicy BackSignalByID error");
                        return null;
                    }
                    var fr = response1 as ResponseMessage<ProgrammeInfoInterface>;

                    if (fr.Ext != null)
                    {
                        // 将信号源ID修改为备份信号源的ID
                        taskinfo.TaskContent.SignalID = fr.Ext.ProgrammeId;
                        switch (fr.Ext.PgmType)
                        {
                            case ProgrammeTypeInterface.PT_Null:
                                ts = TaskSource.emUnknowTask;
                                break;
                            case ProgrammeTypeInterface.PT_SDI:
                                ts = TaskSource.emMSVUploadTask;
                                break;
                            case ProgrammeTypeInterface.PT_IPTS:
                                ts = TaskSource.emIPTSUploadTask;
                                break;
                            case ProgrammeTypeInterface.PT_StreamMedia:
                                ts = TaskSource.emStreamMediaUploadTask;
                                break;
                            default:
                                ts = TaskSource.emUnknowTask;
                                break;
                        }

                    }
                }
                taskinfo.TaskSource = ts;
                taskinfo.TaskContent.TaskName = "BK_" + taskinfo.TaskContent.TaskName;
                taskinfo.TaskContent.TaskGUID = Guid.NewGuid().ToString("N");

                MatiralMeta = ConverTaskMaterialMetaString(taskinfo.MaterialMeta);
                if (!string.IsNullOrEmpty(MatiralMeta))
                {
                    var mroot = XDocument.Parse(MatiralMeta);
                    var f = mroot.Element("MATERIAL")?.Element("TITLE");
                    if (f != null)
                    {
                        f.Value = taskinfo.TaskContent.TaskName;
                    }
                    f = mroot.Element("MATERIAL")?.Element("MATERIALID");
                    if (f != null)
                    {
                        f.Value = taskinfo.TaskContent.TaskGUID;
                    }
                    MatiralMeta = mroot.ToString();
                }

                ContentMeta = ConverTaskContentMetaString(taskinfo.ContentMeta);
                if (!string.IsNullOrEmpty(ContentMeta))
                {
                    var mroot = XDocument.Parse(ContentMeta);
                    var f = mroot.Element("TaskContentMetaData")?.Element("BACKUP");
                    if (f != null)
                    {
                        f.Value = taskinfo.TaskContent.SignalID.ToString();
                    }
                    else
                        mroot.Element("TaskContentMetaData").Add(new XElement("BACKUP", taskinfo.TaskContent.SignalID));

                    if (taskinfo.TaskContent.GroupColor > 0)
                    {
                        mroot.Descendants().Where(e => e.Name == "GroupColor" || e.Name == "GroupID" || e.Name == "GroupItem" || e.Name == "").Remove();
                    }
                    ContentMeta = mroot.ToString();
                }

            }
            else
            {
                if (_deviceInterface != null)
                {
                    var response1 = await _deviceInterface.GetDeviceCallBack(new DeviceInternals() {
                        funtype = IngestDBCore.DeviceInternals.FunctionType.SignalInfoByID, SrcId = taskinfo.TaskContent.SignalID
                    });

                    if (response1.Code != ResponseCodeDefines.SuccessCode)
                    {
                        Logger.Error("AddTaskWithPolicy SignalInfoByID error");
                        return null;
                    }
                    var fr = response1 as ResponseMessage<ProgrammeInfoInterface>;

                    if (fr.Ext != null)
                    {
                        // 将信号源ID修改为备份信号源的ID
                        taskinfo.TaskContent.SignalID = fr.Ext.ProgrammeId;
                        switch (fr.Ext.PgmType)
                        {
                            case ProgrammeTypeInterface.PT_Null:
                                ts = TaskSource.emUnknowTask;
                                break;
                            case ProgrammeTypeInterface.PT_SDI:
                                ts = TaskSource.emMSVUploadTask;
                                break;
                            case ProgrammeTypeInterface.PT_IPTS:
                                ts = TaskSource.emIPTSUploadTask;
                                break;
                            case ProgrammeTypeInterface.PT_StreamMedia:
                                ts = TaskSource.emStreamMediaUploadTask;
                                break;
                            default:
                                ts = TaskSource.emUnknowTask;
                                break;
                        }

                    }
                }
                taskinfo.TaskSource = ts;
            }

            if (taskinfo.TaskContent.TaskType == TaskType.TT_MANUTASK)
            {
                if (_deviceInterface != null)
                {
                    DeviceInternals re = new DeviceInternals() {
                        funtype = IngestDBCore.DeviceInternals.FunctionType.ChannelUnitMap, ChannelId = taskinfo.TaskContent.ChannelID
                    };

                    var response1 = await _deviceInterface.GetDeviceCallBack(re);
                    if (response1.Code != ResponseCodeDefines.SuccessCode)
                    {
                        Logger.Error("AddTaskWithPolicy ChannelUnitMap error");
                        return null;
                    }
                    var fr = response1 as ResponseMessage<int>;
                    taskinfo.TaskContent.Unit = fr.Ext;


                    var lst = await Store.GetTaskListAsync(new TaskCondition()
                    {
                        StateIncludeLst = new List<int>() { ((int)taskState.tsReady), },
                        MaxBeginTime = DateTime.Now,
                        MinEndTime = DateTime.Now,
                        ChannelID = taskinfo.TaskContent.ChannelID,
                        TaskTypeIncludeLst = new List<int>() { ((int)TaskType.TT_TIEUP) }
                    }, true, false);
                    if (lst != null && lst.Count > 0)
                    {
                        lst[0].Tasktype = (int)TaskType.TT_NORMAL;
                        lst[0].SyncState = (int)syncState.ssNot;
                        lst[0].DispatchState = (int)dispatchState.dpsDispatched;
                        lst[0].Taskguid = taskinfo.TaskContent.TaskGUID;

                        //await Store.SaveChangeAsync();
                        await Store.UpdateTaskMetaDataAsync(taskinfo.TaskContent.TaskID, MetaDataType.emCapatureMetaData, string.IsNullOrEmpty(ContentMeta) ? taskinfo.CaptureMeta : CaptureMeta);
                        return _mapper.Map<TaskContentResponse>(lst[0]);
                    }
                    else
                    {
                        var back = await Store.AddTaskWithPolicys(_mapper.Map<TaskContentResponse, DbpTask>(taskinfo.TaskContent, opt =>
                        opt.AfterMap((src, dest) =>
                        {
                            dest.OpType = (int)opType.otAdd;
                            dest.DispatchState = (int)dispatchState.dpsDispatched;
                            dest.State = (int)taskState.tsExecuting;
                            dest.SyncState = (int)syncState.ssNot;
                            dest.Tasklock = string.Empty;
                            dest.Taskid = -1;
                        })), true, TaskSource.emMSVUploadTask,
                        string.IsNullOrEmpty(ContentMeta) ? taskinfo.CaptureMeta : CaptureMeta,
                        string.IsNullOrEmpty(ContentMeta) ? ConverTaskContentMetaString(taskinfo.ContentMeta) : ContentMeta,
                        string.IsNullOrEmpty(MatiralMeta) ? ConverTaskMaterialMetaString(taskinfo.MaterialMeta) : MatiralMeta,
                        string.IsNullOrEmpty(PlanningMeta) ? ConverTaskPlanningMetaString(taskinfo.PlanningMeta) : PlanningMeta,
                        null);
                    }
                }
            }
            else //非手动任务选通道
            {
                if (taskinfo.TaskContent.TaskType == TaskType.TT_PERIODIC)
                {
                    DateTime EndTime = DateTimeFormat.DateTimeFromString(taskinfo.ContentMeta.PeriodParam.EndDate);
                    DateTime TaskEnd = DateTimeFormat.DateTimeFromString(taskinfo.TaskContent.End);
                    DateTime RealEnd = new DateTime(EndTime.Year, EndTime.Month, EndTime.Day, TaskEnd.Hour, TaskEnd.Minute, TaskEnd.Second);
                    taskinfo.TaskContent.End = DateTimeFormat.DateTimeToString(RealEnd);

                    DateTime BeginTime = DateTimeFormat.DateTimeFromString(taskinfo.TaskContent.Begin);
                    if (BeginTime.TimeOfDay <= RealEnd.TimeOfDay)//是跨天的任务
                    {
                        DateTime dtFirstDayEndTime = new DateTime(BeginTime.Year, BeginTime.Month, BeginTime.Day, RealEnd.Hour, RealEnd.Minute, RealEnd.Second);
                        DateTime dtNow = DateTime.Now;

                        //zmj2008-11-11修改考虑到当前时间前几天的问题
                        if (dtFirstDayEndTime < dtNow)//如果当天任务的结束时间结束后，开始时间往后推后到当天的后一天
                        {
                            DateTime dtNewBeginTime = new DateTime(dtNow.Year, dtNow.Month, dtNow.Day, BeginTime.Hour, BeginTime.Minute, BeginTime.Second);
                            DateTime dtNowDayEndTime = new DateTime(dtNow.Year, dtNow.Month, dtNow.Day, RealEnd.Hour, RealEnd.Minute, RealEnd.Second);

                            if (dtNowDayEndTime < dtNow)
                            {
                                taskinfo.TaskContent.Begin = DateTimeFormat.DateTimeToString(dtNewBeginTime.AddDays(1));
                            }
                            else
                            {
                                taskinfo.TaskContent.Begin = DateTimeFormat.DateTimeToString(dtNewBeginTime);
                            }
                        }
                    }
                }

                if (string.IsNullOrEmpty(taskinfo.CaptureMeta) && string.IsNullOrEmpty(CaptureMeta))
                {
                    SobeyRecException.ThrowSelfNoParam("AddTaskWithoutPolicy CaptureMeta empty", GlobalDictionary.GLOBALDICT_CODE_NO_CAPTURE_PARAM, Logger, null);
                }

                bool bLockChannel = taskinfo.TaskContent.ChannelID > 0 ? false : true;
                CHSelCondition condition = new CHSelCondition();
                condition.BackupCHSel = backup;
                condition.CheckCHCurState = true;//检查当前通道状态

                /*
                 * @brief 当前没有openend任务，估计路透就会有了，所以这个我暂时用false来表示
                 */
                condition.MoveExcutingOpenTask = false;
                condition.OnlyLocalChannel = true;
                condition.BaseCHID = -1;
                //condition. = true;

                int nSelCH = -1;
                if (taskinfo.TaskContent.TaskType != TaskType.TT_PERIODIC)
                {

                    nSelCH = await CHSelectForNormalTask(taskinfo.TaskContent, condition);
                }
                else
                {
                    nSelCH = await CHSelectForPeriodicTask(taskinfo.TaskContent, condition);
                }

                if (nSelCH <= 0)
                {
                    if (taskinfo.TaskContent.ChannelID > 0)
                    {
                        SobeyRecException.ThrowSelfNoParam("AddTaskWithoutPolicy GLOBALDICT_CODE_SELECTED_CHANNEL_IS_BUSY_OR_CAN_NOT_BE_SUITED_TO_PROGRAMME", GlobalDictionary.GLOBALDICT_CODE_SELECTED_CHANNEL_IS_BUSY_OR_CAN_NOT_BE_SUITED_TO_PROGRAMME, Logger, null);
                    }
                    else
                    {
                        SobeyRecException.ThrowSelfNoParam("AddTaskWithoutPolicy GLOBALDICT_CODE_ALL_USEABLE_CHANNELS_ARE_BUSY", GlobalDictionary.GLOBALDICT_CODE_ALL_USEABLE_CHANNELS_ARE_BUSY, Logger, null);
                    }
                }
                else
                {
                    taskinfo.TaskContent.ChannelID = nSelCH;
                }

                if (taskinfo.TaskContent.GroupColor > 0)
                {
                    if (_deviceInterface != null)
                    {
                        DeviceInternals re = new DeviceInternals() { funtype = IngestDBCore.DeviceInternals.FunctionType.SingnalIDByChannel, ChannelId = taskinfo.TaskContent.ChannelID };
                        var response1 = await _deviceInterface.GetDeviceCallBack(re);
                        if (response1.Code != ResponseCodeDefines.SuccessCode)
                        {
                            Logger.Error("AddTaskWithPolicy SingnalIDByChannel error");
                            return null;
                        }
                        var fr = response1 as ResponseMessage<int>;
                        taskinfo.TaskContent.SignalID = fr.Ext;
                    }

                }

                var back = await Store.AddTaskWithPolicys(_mapper.Map<TaskContentResponse, DbpTask>(taskinfo.TaskContent, opt =>
                opt.AfterMap((src, dest) =>
                {
                    dest.OpType = (int)opType.otAdd;
                    dest.DispatchState = (int)dispatchState.dpsNotDispatch;
                    dest.State = (int)taskState.tsReady;
                    dest.SyncState = (int)syncState.ssSync;
                    dest.Tasklock = string.Empty;
                    dest.Taskid = -1;
                })), true, taskinfo.TaskSource,
                string.IsNullOrEmpty(ContentMeta) ? taskinfo.CaptureMeta : CaptureMeta,
                string.IsNullOrEmpty(ContentMeta) ? ConverTaskContentMetaString(taskinfo.ContentMeta) : ContentMeta,
                string.IsNullOrEmpty(MatiralMeta) ? ConverTaskMaterialMetaString(taskinfo.MaterialMeta) : MatiralMeta,
                string.IsNullOrEmpty(PlanningMeta) ? ConverTaskPlanningMetaString(taskinfo.PlanningMeta) : PlanningMeta,
                null);
                //taskinfo.TaskContent.

                if (back.Taskid > 0)
                {
                    //存metadata
                }

                return _mapper.Map<TaskContentResponse>(back);
            }

            return null;
        }

        public async Task<TaskContentResponse> AddTaskWithoutPolicy<TResult>(TResult info, string CaptureMeta, string ContentMeta, string MatiralMeta, string PlanningMeta)
        {
            var taskinfo = _mapper.Map<TaskInfoRequest>(info);
            if (default(TResult).GetType() == typeof(AddTaskExDb_in))
            {
                taskinfo.TaskSource = TaskSource.emMSVUploadTask;
            }

            if (taskinfo.TaskContent.TaskType == TaskType.TT_MANUTASK)
            {
                //var _globalinterface = ApplicationContext.Current.ServiceProvider.GetRequiredService<IIngestDeviceInterface>();
                if (_deviceInterface != null)
                {
                    var response1 = await _deviceInterface.GetDeviceCallBack(new DeviceInternals() {
                        funtype = IngestDBCore.DeviceInternals.FunctionType.ChannelUnitMap, ChannelId = taskinfo.TaskContent.ChannelID
                    });

                    if (response1.Code != ResponseCodeDefines.SuccessCode)
                    {
                        Logger.Error("AddTaskWithoutPolicy ChannelUnitMap error");
                        return null;
                    }
                    var fr = response1 as ResponseMessage<int>;
                    taskinfo.TaskContent.Unit = fr.Ext;

                    var lst = await Store.GetTaskListAsync(new TaskCondition()
                    {
                        StateIncludeLst = new List<int>() { ((int)taskState.tsReady), },
                        MaxBeginTime = DateTime.Now,
                        MinEndTime = DateTime.Now,
                        ChannelID = taskinfo.TaskContent.ChannelID,
                        TaskTypeIncludeLst = new List<int>() { ((int)TaskType.TT_TIEUP) }
                    }, true, false);
                    if (lst != null && lst.Count > 0)
                    {
                        lst[0].Tasktype = (int)TaskType.TT_NORMAL;
                        lst[0].SyncState = (int)syncState.ssNot;
                        lst[0].DispatchState = (int)dispatchState.dpsDispatched;
                        lst[0].Taskguid = taskinfo.TaskContent.TaskGUID;

                        //await Store.SaveChangeAsync();
                        await Store.UpdateTaskMetaDataAsync(taskinfo.TaskContent.TaskID, MetaDataType.emCapatureMetaData, string.IsNullOrEmpty(ContentMeta) ? taskinfo.CaptureMeta : CaptureMeta);
                        return _mapper.Map<TaskContentResponse>(lst[0]);
                    }
                    else
                    {
                        var back = await Store.AddTaskWithPolicys(_mapper.Map<TaskContentResponse, DbpTask>(taskinfo.TaskContent, opt =>
                        opt.AfterMap((src, dest) =>
                        {
                            dest.OpType = (int)opType.otAdd;
                            dest.DispatchState = (int)dispatchState.dpsDispatched;
                            dest.State = (int)taskState.tsExecuting;
                            dest.SyncState = (int)syncState.ssNot;
                            dest.Tasklock = string.Empty;
                            dest.Taskid = -1;
                        })), true, TaskSource.emMSVUploadTask,
                        string.IsNullOrEmpty(ContentMeta) ? taskinfo.CaptureMeta : CaptureMeta,
                        string.IsNullOrEmpty(ContentMeta) ? ConverTaskContentMetaString(taskinfo.ContentMeta) : ContentMeta,
                        string.IsNullOrEmpty(MatiralMeta) ? ConverTaskMaterialMetaString(taskinfo.MaterialMeta) : MatiralMeta,
                        string.IsNullOrEmpty(PlanningMeta) ? ConverTaskPlanningMetaString(taskinfo.PlanningMeta) : PlanningMeta,
                        null);
                    }
                }
            }
            else //非手动任务选通道
            {
                if (taskinfo.TaskContent.TaskType == TaskType.TT_PERIODIC)
                {
                    DateTime EndTime = DateTimeFormat.DateTimeFromString(taskinfo.ContentMeta.PeriodParam.EndDate);
                    DateTime TaskEnd = DateTimeFormat.DateTimeFromString(taskinfo.TaskContent.End);
                    DateTime RealEnd = new DateTime(EndTime.Year, EndTime.Month, EndTime.Day, TaskEnd.Hour, TaskEnd.Minute, TaskEnd.Second);
                    taskinfo.TaskContent.End = DateTimeFormat.DateTimeToString(RealEnd);

                    DateTime BeginTime = DateTimeFormat.DateTimeFromString(taskinfo.TaskContent.Begin);
                    if (BeginTime.TimeOfDay <= RealEnd.TimeOfDay)//是跨天的任务
                    {
                        DateTime dtFirstDayEndTime = new DateTime(BeginTime.Year, BeginTime.Month, BeginTime.Day, RealEnd.Hour, RealEnd.Minute, RealEnd.Second);
                        DateTime dtNow = DateTime.Now;

                        //zmj2008-11-11修改考虑到当前时间前几天的问题
                        if (dtFirstDayEndTime < dtNow)//如果当天任务的结束时间结束后，开始时间往后推后到当天的后一天
                        {
                            DateTime dtNewBeginTime = new DateTime(dtNow.Year, dtNow.Month, dtNow.Day, BeginTime.Hour, BeginTime.Minute, BeginTime.Second);
                            DateTime dtNowDayEndTime = new DateTime(dtNow.Year, dtNow.Month, dtNow.Day, RealEnd.Hour, RealEnd.Minute, RealEnd.Second);

                            if (dtNowDayEndTime < dtNow)
                            {
                                taskinfo.TaskContent.Begin = DateTimeFormat.DateTimeToString(dtNewBeginTime.AddDays(1));
                            }
                            else
                            {
                                taskinfo.TaskContent.Begin = DateTimeFormat.DateTimeToString(dtNewBeginTime);
                            }
                        }
                    }
                }

                if (string.IsNullOrEmpty(taskinfo.CaptureMeta) && string.IsNullOrEmpty(CaptureMeta))
                {
                    SobeyRecException.ThrowSelfNoParam("AddTaskWithoutPolicy CaptureMeta empty", GlobalDictionary.GLOBALDICT_CODE_NO_CAPTURE_PARAM, Logger, null);
                }

                bool bLockChannel = taskinfo.TaskContent.ChannelID > 0 ? false : true;
                CHSelCondition condition = new CHSelCondition();
                condition.BackupCHSel = false;
                condition.CheckCHCurState = true;//检查当前通道状态
                condition.MoveExcutingOpenTask = false;
                condition.OnlyLocalChannel = true;
                condition.BaseCHID = -1;
                //condition. = true;

                int nSelCH = -1;
                if (taskinfo.TaskContent.TaskType != TaskType.TT_PERIODIC)
                {

                    nSelCH = await CHSelectForNormalTask(taskinfo.TaskContent, condition);
                }
                else
                {
                    nSelCH = await CHSelectForPeriodicTask(taskinfo.TaskContent, condition);
                }

                if (nSelCH <= 0)
                {
                    if (taskinfo.TaskContent.ChannelID > 0)
                    {
                        SobeyRecException.ThrowSelfNoParam("AddTaskWithoutPolicy GLOBALDICT_CODE_SELECTED_CHANNEL_IS_BUSY_OR_CAN_NOT_BE_SUITED_TO_PROGRAMME", GlobalDictionary.GLOBALDICT_CODE_SELECTED_CHANNEL_IS_BUSY_OR_CAN_NOT_BE_SUITED_TO_PROGRAMME, Logger, null);
                    }
                    else
                    {
                        SobeyRecException.ThrowSelfNoParam("AddTaskWithoutPolicy GLOBALDICT_CODE_ALL_USEABLE_CHANNELS_ARE_BUSY", GlobalDictionary.GLOBALDICT_CODE_ALL_USEABLE_CHANNELS_ARE_BUSY, Logger, null);
                    }
                }
                else
                {
                    taskinfo.TaskContent.ChannelID = nSelCH;
                }

                if (taskinfo.TaskContent.GroupColor > 0)
                {
                    //var _globalinterface = ApplicationContext.Current.ServiceProvider.GetRequiredService<IIngestDeviceInterface>();
                    if (_deviceInterface != null)
                    {
                        var response1 = await _deviceInterface.GetDeviceCallBack(new DeviceInternals() {
                            funtype = IngestDBCore.DeviceInternals.FunctionType.SingnalIDByChannel,
                            ChannelId = taskinfo.TaskContent.ChannelID
                        });

                        if (response1.Code != ResponseCodeDefines.SuccessCode)
                        {
                            Logger.Error("AddTaskWithoutPolicy SingnalIDByChannel error");
                            return null;
                        }
                        var fr = response1 as ResponseMessage<int>;
                        taskinfo.TaskContent.SignalID = fr.Ext;
                    }

                }

                var back = await Store.AddTaskWithPolicys(_mapper.Map<TaskContentResponse, DbpTask>(taskinfo.TaskContent, opt =>
                opt.AfterMap((src, dest) =>
                {
                    dest.OpType = (int)opType.otAdd;
                    dest.DispatchState = (int)dispatchState.dpsNotDispatch;
                    dest.State = (int)taskState.tsReady;
                    dest.SyncState = (int)syncState.ssSync;
                    dest.Tasklock = string.Empty;
                    dest.Taskid = -1;
                })), true, taskinfo.TaskSource,
                string.IsNullOrEmpty(ContentMeta) ? taskinfo.CaptureMeta : CaptureMeta,
                string.IsNullOrEmpty(ContentMeta) ? ConverTaskContentMetaString(taskinfo.ContentMeta) : ContentMeta,
                string.IsNullOrEmpty(MatiralMeta) ? ConverTaskMaterialMetaString(taskinfo.MaterialMeta) : MatiralMeta,
                string.IsNullOrEmpty(PlanningMeta) ? ConverTaskPlanningMetaString(taskinfo.PlanningMeta) : PlanningMeta,
                null);
                //taskinfo.TaskContent.

                if (back.Taskid > 0)
                {
                    //存metadata
                }

                return _mapper.Map<TaskContentResponse>(back);
            }

            return null;
        }

        public async Task<int> CHSelectForNormalTask(TaskContentRequest request, CHSelCondition condition)
        {
            Logger.Info((string.Format("###Begin to Select channel for Normal task:{0}, signalid:{1}, channelid:{2}, bBackupCH:{3}, bCheckState:{4}, nBaseCH:{5}, OnlyLocal:{6},bExcuting:{7}",
            request.TaskID, request.SignalID, request.ChannelID,
            condition.BackupCHSel, condition.CheckCHCurState, condition.BaseCHID, condition.OnlyLocalChannel, condition.MoveExcutingOpenTask)));

            if (request.SignalID <= 0)
            {
                return -1;
            }

            //1. 为信号源选择所有匹配的通道
            //2. 根据条件筛选通道，这些条件是固定属性，放在前面筛选掉，不用互斥
            var matchlst = await GetMatchedChannelForSignal(request.SignalID, request.ChannelID, condition);
            if (matchlst != null && matchlst.Count > 0)
            {
                Logger.Info("CHSelectForNormalTask matchcount {0}", matchlst.Count);

                DateTime Begin = DateTimeFormat.DateTimeFromString(request.Begin);
                DateTime End = DateTimeFormat.DateTimeFromString(request.End);
                List<int> freeChannelIdList = await Store.GetFreeChannels(matchlst, Begin, End);

                Logger.Info("GetFreeChannels freeChannelIdList {0}", freeChannelIdList.Count);

                if (freeChannelIdList?.Count > 0)
                {
                    /*
                     @ brief 这里按照老逻辑会赛选下被锁住的通道，全是超时锁
                     @ 选择通道时锁住通道，添加任务完了再释放锁，这什么鬼逻辑
                     @ 决定放弃这个通道锁这个逻辑，后面不行再补上
                     */
                    return ChooseBestChannel(request, freeChannelIdList, condition);
                }
            }
            else
                Logger.Error("CHSelectForNormalTask matchcount error");

            return -1;
        }

        public async Task<int> CHSelectForPeriodicTask(TaskContentRequest request, CHSelCondition condition)
        {
            Logger.Info((string.Format("###Begin to Select channel for Normal task:{0}, signalid:{1}, channelid:{2}, bBackupCH:{3}, bCheckState:{4}, nBaseCH:{5}, OnlyLocal:{6},bExcuting:{7}",
            request.TaskID, request.SignalID, request.ChannelID,
            condition.BackupCHSel, condition.CheckCHCurState, condition.BaseCHID, condition.OnlyLocalChannel, condition.MoveExcutingOpenTask)));

            if (request.SignalID <= 0)
            {
                return -1;
            }

            //1. 为信号源选择所有匹配的通道
            //2. 根据条件筛选通道，这些条件是固定属性，放在前面筛选掉，不用互斥
            var matchlst = await GetMatchedChannelForSignal(request.SignalID, request.ChannelID, condition);
            if (matchlst != null && matchlst.Count > 0)
            {
                Logger.Info("CHSelectForNormalTask matchcount {0}", matchlst.Count);

                DateTime Begin = DateTimeFormat.DateTimeFromString(request.Begin);
                DateTime End = DateTimeFormat.DateTimeFromString(request.End);
                List<int> freeChannelIdList = await Store.GetFreePerodiChannels(matchlst, -1, request.Unit, request.SignalID, request.ChannelID, request.Classify, Begin, End);

                Logger.Info("GetFreeChannels freeChannelIdList {0}", freeChannelIdList.Count);

                if (freeChannelIdList?.Count > 0)
                {
                    /*
                     @ brief 这里按照老逻辑会赛选下被锁住的通道，全是超时锁
                     @ 选择通道时锁住通道，添加任务完了再释放锁，这什么鬼逻辑
                     @ 决定放弃这个通道锁这个逻辑，后面不行再补上
                     */
                    return ChooseBestChannel(request, freeChannelIdList, condition);
                }
            }
            else
                Logger.Error("CHSelectForNormalTask matchcount error");

            return -1;
        }

        public int ChooseBestChannel(TaskContentRequest request, List<int> freeChannelIdList, CHSelCondition condition)
        {
            /*
             * @ 我擦还要通过ip排序nBaseCHID，有没有天理啊，艹，随便排了，不管了
             */

            Logger.Info("ChooseBestChannel {0} {1} {2}", request.ChannelID, string.Join(",", freeChannelIdList), condition.OnlyLocalChannel);
            int nSelCH = -1;
            //如果只能在指定通道创建任务，那么这里单独判断
            if (request.ChannelID > 0 && condition.OnlyLocalChannel)
            {
                for (int i = 0; i < freeChannelIdList.Count; i++)
                {
                    if (request.ChannelID == freeChannelIdList[i])
                    {
                        nSelCH = request.ChannelID;
                        break;
                    }
                }
                //对于外面指定了通道的情况，目前通常是由终端已经锁定了通道，所以这里就不再加锁，否则一定会失败
                //if (nSelCH > 0 && LockChannelById(nSelCH, 3000))
                if (nSelCH > 0)
                {
                    //return nSelCH;
                    Logger.Info(string.Format("The OnlyLocalChannel taskAdd.nChannelID:{0} is useable", request.ChannelID));
                }
                else
                {
                    string strErr = GlobalDictionary.Instance.GetMessageByCode(GlobalDictionary.GLOBALDICT_CODE_SELECTED_CHANNEL_IS_BUSY_OR_CAN_NOT_BE_SUITED_TO_PROGRAMME);
                    Logger.Info(string.Format("The Channel is not useable,nSelCH:{0}, err:{1}", nSelCH, strErr));
                    nSelCH = -1;
                }
            }
            else
            {
                //按照已经排定的顺序依次去锁定通道，锁成功了，就表示可用
                nSelCH = freeChannelIdList[0];
                Logger.Info(string.Format("The zero taskAdd.nChannelID:{0} is useable", nSelCH));
            }
            return nSelCH;
        }
        public async Task<List<int>> GetMatchedChannelForSignal(int SignalID, int ChID, CHSelCondition condition)
        {
            
            if (_deviceInterface != null)
            {
                DeviceInternals re = new DeviceInternals() { funtype = IngestDBCore.DeviceInternals.FunctionType.ChannelInfoBySrc, SrcId = SignalID, Status = condition.CheckCHCurState ? 1 : 0 };
                var response1 = await _deviceInterface.GetDeviceCallBack(re);
                if (response1.Code != ResponseCodeDefines.SuccessCode)
                {
                    Logger.Error("GetMatchedChannelForSignal ChannelInfoBySrc error");
                    return null;
                }

                Logger.Info(string.Format("GetMatchedChannelForSignal match {0}  {1}  {2}", condition.BaseCHID, condition.BackupCHSel, ChID));

                var fresponse = response1 as ResponseMessage<List<CaptureChannelInfoInterface>>;
                if (fresponse != null && fresponse.Ext?.Count > 1)
                {
                    fresponse.Ext.RemoveAll(x => ChID != x.ID && (x.BackState == BackupFlagInterface.emAllowBackUp && ChID != -1));

                    /// 如果存在onlybackup属性的通道，优先考虑
                    return fresponse.Ext.OrderByDescending(x => x.BackState).Select(y => y.ID).ToList();/// 如果存在onlybackup属性的通道，优先考虑
                }
            }

            return null;
        }


        public async Task<List<TaskContent>> GetWillBeginAndCapturingTasksAsync()
        {
            List<TaskContent> capturingTasks = await GetAllChannelCapturingTask<TaskContent>();//获得所有通道正在采集的任务.
            List<TaskContent> willBeginTasks = await GetWillBeginTasksInLast2Hours<TaskContent>();
            capturingTasks.AddRange(willBeginTasks);
            return capturingTasks;
        }

        private async Task<List<TSource>> GetWillBeginTasksInLast2Hours<TSource>()
        {
            var tasks = await Store.GetTaskListAsync(a => a.Where(x => x.State == (int)taskState.tsReady && x.NewBegintime > DateTime.Now && x.NewBegintime < DateTime.Now.AddHours(2))
                                                           .GroupBy(x => x.Channelid), true);
            if (tasks.Count > 0)
            {
                return _mapper.Map<List<TSource>>(tasks.Select(a => a.MaxItem(x => x.Starttime)));
            }
            return new List<TSource>();
        }

        public async Task<List<TSource>> GetCurrentTasksAsync<TSource>()
        {
            return _mapper.Map<List<TSource>>(await Store.GetTaskListAsync(
                a => a.Where(x => x.State == (int)taskState.tsReady && x.NewBegintime < DateTime.MaxValue && x.NewEndtime > DateTime.Now), true));
        }

    }
}
