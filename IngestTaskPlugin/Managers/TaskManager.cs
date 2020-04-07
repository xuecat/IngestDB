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

        public async virtual Task<TaskMaterialMetaResponse> GetTaskMaterialMetadataAsync(int taskid)
        {
            var f = await Store.GetTaskMetaDataAsync(a => a
            .Where(b => b.Taskid == taskid && b.Metadatatype == (int)MetaDataType.emStoreMetaData)
            .Select(x=>x.Metadatalong), true);

            try
            {
                var root = XDocument.Load(f);
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
                var root = XDocument.Load(f);
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
                var root = XDocument.Load(f);
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

        //这个接口是为老写的
        public async Task UpdateTaskMetaDataAsync(int taskid, int type, string metadata)
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
                await Store.SageChangeAsync();
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

        public async Task<TaskContentResponse> AddTaskWithoutPolicy(TaskInfoRequest taskinfo)
        {
            
            
            if (taskinfo.TaskContent.TaskType == TaskType.TT_MANUTASK)
            {

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

                if (string.IsNullOrEmpty(taskinfo.CaptureMeta))
                {
                    SobeyRecException.ThrowSelfNoParam("AddTaskWithoutPolicy CaptureMeta empty", GlobalDictionary.GLOBALDICT_CODE_NO_CAPTURE_PARAM, Logger, null);
                }

                bool bLockChannel = taskinfo.TaskContent.ChannelID > 0 ? false : true;


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

        }

        public async Task<int> CHSelectForPeriodicTask(TaskContentRequest request, CHSelCondition condition)
        { }

        public async List<int> GetMatchedChannelForSignal(int SignalID, int ChID, CHSelCondition condition)
        {
            var _globalinterface = ApplicationContext.Current.ServiceProvider.GetRequiredService<IIngestDeviceInterface>();
            if (_globalinterface != null)
            {
                DeviceInternals re = new DeviceInternals() { funtype = IngestDBCore.DeviceInternals.FunctionType.ChannelInfoBySrc, SrcId = SignalID };
                var response1 = await _globalinterface.GetDeviceCallBack(re);
                if (response1.Code != ResponseCodeDefines.SuccessCode)
                {
                    Logger.Error("SetGlobalState modtask error");
                }
            }


        }
        ///////////////////////////////
    }
}
