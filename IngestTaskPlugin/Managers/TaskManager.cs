using AutoMapper;
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
using TaskInfoRequest = IngestTaskPlugin.Dto.TaskInfoResponse;
using TaskContentRequest = IngestTaskPlugin.Dto.TaskContentResponse;
using IngestDBCore.Interface;
using Microsoft.Extensions.DependencyInjection;
using IngestTaskPlugin.Models;

namespace IngestTaskPlugin.Managers
{
    public class TaskManager
    {
        public TaskManager(ITaskStore store, IMapper mapper)
        {
            Store = store;
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        protected ITaskStore Store { get; }
        protected IMapper _mapper { get; }
        private readonly ILogger Logger = LoggerManager.GetLogger("TaskInfo");

        public async virtual Task<TResult> GetTaskMetadataAsync<TResult>( int taskid, int ntype)
        {
            var f = await Store.GetTaskMetaDataAsync(a => a.Where(b => b.Taskid == taskid && b.Metadatatype == ntype), true);
            return _mapper.Map<TResult>(f);
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
            .Select(x=>x.Metadatalong), true);

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

        public async Task<TResult> GetTaskInfoByID<TResult>(int taskid)
        {
            return _mapper.Map<TResult>(await Store.GetTaskAsync(a => a.Where(b => b.Taskid == taskid), true));
        }

        public async Task<int> GetTieUpTaskIDByChannelId(int channelid)
        {
            DateTime now = DateTime.Now;
            return await Store.GetTaskAsync(a => a.Where(b => b.Channelid == channelid &&
            b.Tasktype == (int)TaskType.TT_TIEUP && (b.Starttime <= now && b.Endtime >= now)).Select(f =>f.Taskid), true);
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
                        { material = root.Element("MATERIAL"); } break;
                    case (int)MetaDataType.emContentMetaData:
                        { material = root.Element("TaskContentMetaData"); } break;
                    case (int)MetaDataType.emPlanMetaData:
                        { material = root.Element("Planning"); } break;
                    case (int)MetaDataType.emSplitData:
                        { material = root.Element("SplitMetaData"); } break;
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

                    // ApplicationLog.WriteInfo(string.Format("In TrimTaskBeginTime,ENDTIME > dtNow && row.TASKTYPE != (int)TaskType.TT_VTRUPLOAD,TaskID = {0},StartTime = {1}", nTaskID, dtNow.ToString()));
                }
                else if (taskinfo.Tasktype == (int)TaskType.TT_VTRUPLOAD)
                {
                    TimeSpan ts = taskinfo.Endtime - taskinfo.Starttime;
                    taskinfo.Starttime = dtNow;
                    taskinfo.NewBegintime = dtNow;

                    VTROper vtrOper = new VTROper();
                    bool isNeedModifyEndTime = true;
                    vtrOper.AdjustVtrUploadTasksByChannelId((int)row.CHANNELID, (int)row.TASKID, dtNow, ref isNeedModifyEndTime);

                    if (isNeedModifyEndTime)
                    {
                        taskinfo.Endtime = dtNow.Add(ts);
                        taskinfo.NewEndtime = taskinfo.Endtime;
                    }

                    // ApplicationLog.WriteInfo(string.Format("In TrimTaskBeginTime,row.TASKTYPE == (int)TaskType.TT_VTRUPLOAD,TaskID = {0},StartTime = {1}", nTaskID, dtNow.ToString()));
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

                    //ApplicationLog.WriteInfo(string.Format("In TrimTaskBeginTime,Other,TaskID = {0},StartTime = {1},STARTTIME = {2},ENDTIME = {3}",nTaskID,dtNow.ToString(),row.STARTTIME.ToString(),row.ENDTIME.ToString()));
                }

                return taskid;
            }

        }

        public async Task<List<TResult>> QueryTaskContent<TResult>(int unitid, DateTime day, TimeLineType timetype)
        {
            return _mapper.Map<List<TResult>>(await Store.GetTaskListWithMode(1, day, timetype));
        }

        public async Task<TaskSource> GetTaskSource(int taskid)
        {
            return (TaskSource)(await Store.GetTaskSourceAsync(a => a.Where(b => b.Taskid == taskid).Select(x => x.Tasksource), true));
        }

        public async Task<List<int>> StopGroupTaskAsync(int taskid)
        {
            var f = await Store.GetTaskMetaDataAsync(a => a.Where(b => b.Taskid == taskid && b.Metadatatype ==(int)MetaDataType.emContentMetaData));

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
            return _mapper.Map<List<TResult>>(await Store.GetTaskListAsync(a => a.Where( b => b.State == (int)taskState.tsExecuting || b.State == (int)taskState.tsManuexecuting), true));
            
        }

        public async Task<TResult> GetChannelCapturingTask<TResult>(int channelid)
        {
            return _mapper.Map<TResult>(await Store.GetTaskAsync(a => 
            a.Where(b =>b.Channelid == channelid && (b.State == (int)taskState.tsExecuting || b.State == (int)taskState.tsManuexecuting)), true));
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
                var _globalinterface = ApplicationContext.Current.ServiceProvider.GetRequiredService<IIngestDeviceInterface>();
                if (_globalinterface != null)
                {
                    DeviceInternals re = new DeviceInternals() { funtype = IngestDBCore.DeviceInternals.FunctionType.ChannelInfoBySrc,
                        SrcId = taskModify.SignalID, Status = 1 };

                    var response1 = await _globalinterface.GetDeviceCallBack(re);
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
                    SobeyRecException.ThrowSelfOneParam("ModifyTask vtrtask empty", GlobalDictionary.GLOBALDICT_CODE_CAN_NOT_FIND_THE_TASK_ONEPARAM,  Logger, findtask.Taskid, null);
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

            List<TaskContentResponse> lsttask = null;
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

        public async Task<int> UpdateComingTasks()
        {
            TaskCondition condition = new TaskCondition();
            var lst = await Store.GetTaskListAsync(c => c.Where(f => f.State == (int)syncState.ssSync
            && f.DispatchState == (int)dispatchState.dpsNotDispatch
            && (f.State!= (int)taskState.tsDelete && f.State != (int)taskState.tsConflict && f.State != (int)taskState.tsInvaild)
            && (f.Starttime > DateTime.Now.AddHours(-24) && f.Starttime < DateTime.Now.AddHours(1)) ));

            
            if (lst!= null && lst.Count > 0)
            {
                string log = string.Empty;
                lst.ForEach(a => { a.SyncState = (int)syncState.ssNot; a.DispatchState = (int)dispatchState.dpsDisabled; a.Tasklock = string.Empty; log += ","+a.Taskid; });

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
                    tsDuration = new TimeSpan(0, 0, vtrtask.Trimoutctl.GetValueOrDefault()/vtrtask.Vtrtaskid.GetValueOrDefault());
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

        /// <summary>
        /// 从占位的时间段中获取到空闲的时间段
        /// </summary>
        /// <param name="id"></param>
        /// <param name="tieupTimePeriods"></param>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        private List<TimePeriod> GetFreeTimePeriodsByTieup(int id, List<TimePeriod> tieupTimePeriods, DateTime beginTime, DateTime endTime)
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

        public async Task<TaskContentResponse> AddTaskWithoutPolicy<TResult>(TResult info,string CaptureMeta, string ContentMeta, string MatiralMeta, string PlanningMeta)
        {
            var taskinfo = _mapper.Map<TaskInfoRequest>(info);
            if (taskinfo.TaskContent.TaskType == TaskType.TT_MANUTASK)
            {
                var _globalinterface = ApplicationContext.Current.ServiceProvider.GetRequiredService<IIngestDeviceInterface>();
                if (_globalinterface != null)
                {
                    DeviceInternals re = new DeviceInternals() { funtype = IngestDBCore.DeviceInternals.FunctionType.ChannelUnitMap, ChannelId = taskinfo.TaskContent.ChannelID };
                    var response1 = await _globalinterface.GetDeviceCallBack(re);
                    if (response1.Code != ResponseCodeDefines.SuccessCode)
                    {
                        Logger.Error("AddTaskWithoutPolicy ChannelInfoBySrc error");
                        return null;
                    }
                    var fr = response1 as ResponseMessage<int>;
                    taskinfo.TaskContent.Unit = fr.Ext;

                    var lst = await Store.GetTaskListAsync(new TaskCondition() {
                        StateIncludeLst = new List<int>() { ((int)taskState.tsReady), },
                        MaxBeginTime = DateTime.Now,
                        MinEndTime = DateTime.Now,
                        ChannelID = taskinfo.TaskContent.ChannelID,
                        TaskTypeIncludeLst = new List<int>() { ((int)TaskType.TT_TIEUP)}
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
                        string.IsNullOrEmpty(ContentMeta) ? taskinfo.CaptureMeta:CaptureMeta, 
                        string.IsNullOrEmpty(ContentMeta)?ConverTaskContentMetaString(taskinfo.ContentMeta): ContentMeta,
                        string.IsNullOrEmpty(MatiralMeta)?ConverTaskMaterialMetaString(taskinfo.MaterialMeta): MatiralMeta,
                        string.IsNullOrEmpty(PlanningMeta)?ConverTaskPlanningMetaString(taskinfo.PlanningMeta):PlanningMeta,
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
                    var _globalinterface = ApplicationContext.Current.ServiceProvider.GetRequiredService<IIngestDeviceInterface>();
                    if (_globalinterface != null)
                    {
                        DeviceInternals re = new DeviceInternals() { funtype = IngestDBCore.DeviceInternals.FunctionType.SingnalInfoByChannel, ChannelId= taskinfo.TaskContent.ChannelID};
                        var response1 = await _globalinterface.GetDeviceCallBack(re);
                        if (response1.Code != ResponseCodeDefines.SuccessCode)
                        {
                            Logger.Error("AddTaskWithoutPolicy ChannelInfoBySrc error");
                            return null;
                        }
                        var fr= response1 as ResponseMessage<int>;
                        taskinfo.TaskContent.SignalID = fr.Ext;
                    }
                        
                }

                var back = await Store.AddTaskWithPolicys(_mapper.Map<TaskContentResponse, DbpTask>(taskinfo.TaskContent, opt =>
                opt.AfterMap((src, dest) => {
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
            var _globalinterface = ApplicationContext.Current.ServiceProvider.GetRequiredService<IIngestDeviceInterface>();
            if (_globalinterface != null)
            {
                DeviceInternals re = new DeviceInternals() { funtype = IngestDBCore.DeviceInternals.FunctionType.ChannelInfoBySrc, SrcId = SignalID, Status = condition.CheckCHCurState?1:0};
                var response1 = await _globalinterface.GetDeviceCallBack(re);
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
                    return fresponse.Ext.OrderByDescending(x => x.BackState).Select(y =>y.ID).ToList();/// 如果存在onlybackup属性的通道，优先考虑
                }
            }

            return null;
        }
    }
}
