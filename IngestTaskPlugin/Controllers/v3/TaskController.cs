using AutoMapper;
using IngestDBCore;
using IngestDBCore.Interface;
using IngestDBCore.Notify;
using IngestDBCore.Tool;
using IngestTaskPlugin.Dto;
using IngestTaskPlugin.Dto.OldResponse;
using IngestTaskPlugin.Dto.Response;
using IngestTaskPlugin.Managers;
using IngestTaskPlugin.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using Sobey.Core.Log;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;

using TaskInfoRequest = IngestTaskPlugin.Dto.Response.TaskInfoResponse;
using TaskContentRequest = IngestTaskPlugin.Dto.Response.TaskContentResponse;

namespace IngestTaskPlugin.Controllers.v3
{
    
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("3.0")]
    [ApiController]
    public partial class TaskController : ControllerBase
    {
        private readonly ILogger Logger = LoggerManager.GetLogger("TaskInfo3");
        private readonly TaskManager _taskManage;
        private readonly Lazy<IIngestGlobalInterface> _globalInterface;
        private readonly IMapper _mapper;
        private readonly Lazy<NotifyClock> _clock;
        public TaskController(TaskManager task, IServiceProvider services, IMapper mapper)
        {
            _taskManage = task;
            _clock = _clock = new Lazy<NotifyClock>(() => services.GetRequiredService<NotifyClock>());
            _globalInterface = ApplicationContext.Current.GlobalNotify ? new Lazy<IIngestGlobalInterface>(() => services.GetRequiredService<IIngestGlobalInterface>()) : null ;
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// <summary>
        /// 获取任务全部的元数据
        /// </summary>
        /// <remarks>
        /// 例子:
        ///
        /// </remarks>
        /// <param name="taskid">任务id，</param>
        /// <returns>任务内容全部信息包含元数据</returns>
        [HttpGet("{taskid}")]
        [ApiExplorerSettings(GroupName = "v3.0")]
        public async Task<ResponseMessage<TaskFullInfoResponse>> GetTaskFullInfoByID([FromRoute, BindRequired]int taskid)
        {
            var Response = new ResponseMessage<TaskFullInfoResponse>();
            if (taskid <= 0)
            {
                Response.Code = ResponseCodeDefines.ModelStateInvalid;
                Response.Msg = "request param error";
            }

            try
            {
                Response.Ext = await _taskManage.GetTaskInfoAll<TaskFullInfoResponse>(taskid);
                if (Response.Ext == null)
                {
                    Response.Code = ResponseCodeDefines.NotFound;
                    Response.Msg = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}:error info: not find data!";
                }
            }
            catch (Exception e)
            {
                if (e.GetType() == typeof(SobeyRecException))//sobeyexcep会自动打印错误
                {
                    SobeyRecException se = e as SobeyRecException;
                    Response.Code = se.ErrorCode.ToString();
                    Response.Msg = se.Message;
                }
                else
                {
                    Response.Code = ResponseCodeDefines.ServiceError;
                    Response.Msg = "GetTaskFullInfoByID 21 error info:" + e.Message;
                    Logger.Error(Response.Msg);
                }
            }
            return Response;
        }

        /// <summary>
        /// 获取任务存数据库db对应的数据
        /// </summary>
        /// <remarks>
        /// 例子:
        ///
        /// </remarks>
        /// <param name="taskid">任务id，</param>
        /// <returns>任务内容全部信息包含元数据</returns>
        [HttpGet("db/{taskid}")]
        [ApiExplorerSettings(GroupName = "v3.0")]
        public async Task<ResponseMessage<Models.DbpTask>> GetTaskDBInfoByID([FromRoute, BindRequired]int taskid)
        {
            var Response = new ResponseMessage<Models.DbpTask>();
            if (taskid <= 0)
            {
                Response.Code = ResponseCodeDefines.ModelStateInvalid;
                Response.Msg = "request param error";
            }

            try
            {
                Response.Ext = await _taskManage.GetTaskInfoByID<Models.DbpTask>(taskid, 0);
                if (Response.Ext == null)
                {
                    Response.Code = ResponseCodeDefines.NotFound;
                    Response.Msg = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}:error info: not find data!";
                }
            }
            catch (Exception e)
            {
                if (e.GetType() == typeof(SobeyRecException))//sobeyexcep会自动打印错误
                {
                    SobeyRecException se = e as SobeyRecException;
                    Response.Code = se.ErrorCode.ToString();
                    Response.Msg = se.Message;
                }
                else
                {
                    Response.Code = ResponseCodeDefines.ServiceError;
                    Response.Msg = "GetTaskDBInfoByID 21 error info:" + e.Message;
                    Logger.Error(Response.Msg);
                }
            }
            return Response;
        }

        /// <summary>
        [HttpGet("reschedule")]
        [ApiExplorerSettings(GroupName = "v3.0")]
        public async Task<ResponseMessage<List<RescheduledTaskInfoResponse>>> RescheduleTasks()
        {
            var Response = new ResponseMessage<List<RescheduledTaskInfoResponse>>();
            try
            {
                Response.Ext = await _taskManage.RescheduleTasks<RescheduledTaskInfoResponse>();
                if (Response.Ext == null || Response.Ext.Count <= 0)
                {
                    Response.Code = ResponseCodeDefines.NotFound;
                    Response.Msg = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}:error info: not find data!";
                }
            }
            catch (Exception e)
            {
                if (e.GetType() == typeof(SobeyRecException))//sobeyexcep会自动打印错误
                {
                    SobeyRecException se = e as SobeyRecException;
                    Response.Code = se.ErrorCode.ToString();
                    Response.Msg = se.Message;
                }
                else
                {
                    Response.Code = ResponseCodeDefines.ServiceError;
                    Response.Msg = "RescheduleTasks error info:" + e.Message;
                    Logger.Error(Response.Msg);
                }
            }
            return Response;
        }
        /// 根据以前任务添加重调度任务, 一般任务调度失败，开始msv失败才会调，这样产生新的任务重新开始采集
        /// </summary>
        /// <remarks>
        /// 例子:
        ///
        /// </remarks>
        /// <param name="taskid">老任务id，</param>
        /// <returns>任务内容全部信息包含元数据</returns>
        [HttpPost("schedule/{taskid}")]
        [ApiExplorerSettings(GroupName = "v3.0")]
        public async Task<ResponseMessage<TaskContentResponse>> AddRescheduleTaskByID([FromRoute, BindRequired]int taskid)
        {
            var Response = new ResponseMessage<TaskContentResponse>();
            if (taskid <= 0)
            {
                Response.Code = ResponseCodeDefines.ModelStateInvalid;
                Response.Msg = "request param error";
            }

            try
            {
                var addTask = await _taskManage.AddReScheduleTaskSvr(taskid);
                Response.Ext = _mapper.Map<TaskContentResponse>(addTask);
                if (Response.Ext == null)
                {
                    Response.Code = ResponseCodeDefines.NotFound;
                    Response.Msg = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}:error info: not find data!";
                }

                if (_globalInterface != null)
                {
                    //GlobalInternals re = new GlobalInternals() { Funtype = IngestDBCore.GlobalInternals.FunctionType.SetGlobalState, State = GlobalStateName.ADDTASK, TaskID = addTask.Channelid.GetValueOrDefault() };
                    //var response1 = await _globalInterface.Value.SubmitGlobalCallBack(re);
                    //if (response1.Code != ResponseCodeDefines.SuccessCode)
                    //{
                    //    Logger.Error("SetGlobalState modtask error");
                    //}

                    Task.Run(() => { _clock.Value.InvokeNotify(GlobalStateName.ADDTASK, NotifyPlugin.Kafka, NotifyAction.ADDTASK, addTask); });
                }
            }
            catch (Exception e)
            {
                if (e.GetType() == typeof(SobeyRecException))//sobeyexcep会自动打印错误
                {
                    SobeyRecException se = e as SobeyRecException;
                    Response.Code = se.ErrorCode.ToString();
                    Response.Msg = se.Message;
                }
                else
                {
                    Response.Code = ResponseCodeDefines.ServiceError;
                    Response.Msg = "AddRescheduleTaskByID 21 error info:" + e.Message;
                    Logger.Error(Response.Msg);
                }
            }
            return Response;
        }

        /// <summary>
        /// 根据以前任务重新调度修改任务通道信息
        /// </summary>
        /// <remarks>
        /// 例子:
        ///
        /// </remarks>
        /// <param name="taskid">老任务id，</param>
        /// <returns>任务内容全部信息包含元数据</returns>
        [HttpPut("reschedule/channel/{taskid}")]
        [ApiExplorerSettings(GroupName = "v3.0")]
        public async Task<ResponseMessage<TaskContentResponse>> RescheduleTaskChannelByID([FromRoute, BindRequired]int taskid)
        {
            var Response = new ResponseMessage<TaskContentResponse>();
            if (taskid <= 0)
            {
                Response.Code = ResponseCodeDefines.ModelStateInvalid;
                Response.Msg = "request param error";
            }

            try
            {
                var addTask = await _taskManage.ReScheduleTaskChannel(taskid);

                if (_globalInterface != null)
                {
                    //GlobalInternals re = new GlobalInternals() { Funtype = IngestDBCore.GlobalInternals.FunctionType.SetGlobalState, State = GlobalStateName.MODTASK, TaskID = addTask.Channelid.GetValueOrDefault() };
                    //var response1 = await _globalInterface.Value.SubmitGlobalCallBack(re);
                    //if (response1.Code != ResponseCodeDefines.SuccessCode)
                    //{
                    //    Logger.Error("SetGlobalState modtask error");
                    //}

                    Task.Run(() => { _clock.Value.InvokeNotify(GlobalStateName.MODTASK, NotifyPlugin.Kafka, NotifyAction.MODIFYTASK, addTask); });
                }
            }
            catch (Exception e)
            {
                if (e.GetType() == typeof(SobeyRecException))//sobeyexcep会自动打印错误
                {
                    SobeyRecException se = e as SobeyRecException;
                    Response.Code = se.ErrorCode.ToString();
                    Response.Msg = se.Message;
                }
                else
                {
                    Response.Code = ResponseCodeDefines.ServiceError;
                    Response.Msg = "AddRescheduleTaskByold 21 error info:" + e.Message;
                    Logger.Error(Response.Msg);
                }
            }
            return Response;
        }


        /// <summary>
        /// 查询1天的任务
        /// </summary>
        /// <remarks>
        /// 例子:
        /// 这个接口是专门为web提供，新功能有个为任务创建临时rtmp url的。创建好了web端要显示那个url
        /// 但是这个url元数据是放在了metada里面的，查询一天任务的接口不提供。
        /// 最好的方法是修改数据库底层把signal改成string，这样搞要重新发网管，msg，task，客户端所有玩意。太坑了。
        /// 所以单独给webingest提供一个接口，当遇到rtmp url的任务时，多查询一次，并返回给他地址
        /// </remarks>
        /// <param name="unitid">1是客户查询任务，跨天每天任务做分裂，2是web查询任务，跨天每天任务不做分裂</param>
        /// <param name="day">查询时间yyyy/MM/dd HH:mm:ss </param>
        /// <param name="timemode">查询模式0是24小时模式，1是32小时模式</param>
        /// <returns>任务基础元数据</returns>
        [HttpGet("onedaytask/rtmpsignal")]
        [ApiExplorerSettings(GroupName = "v3.0")]
        public async Task<ResponseMessage<List<TaskContentSignalUrlResponse>>> QueryTaskContentForRtmpUrl([FromQuery, BindRequired] int unitid, [FromQuery, BindRequired] string day, [FromQuery, BindRequired] int timemode
            //,[FromHeader(Name = "sobeyhive-http-site"), BindRequired, DefaultValue("S1")] string site
            )
        {
            var Response = new ResponseMessage<List<TaskContentSignalUrlResponse>>();

            try
            {
                Response.Ext = await _taskManage.QueryTaskSignalUrlContent(unitid, DateTimeFormat.DateTimeFromString(day), (TimeLineType)timemode);//_taskManage.QueryTaskSignalUrlContentBySite(unitid, DateTimeFormat.DateTimeFromString(day), (TimeLineType)timemode, site);
                if (Response.Ext == null)
                {
                    Response.Code = ResponseCodeDefines.NotFound;
                    Response.Msg = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}:error info: not find data!";
                }
            }
            catch (Exception e)
            {
                if (e.GetType() == typeof(SobeyRecException))//sobeyexcep会自动打印错误
                {
                    SobeyRecException se = e as SobeyRecException;
                    Response.Code = se.ErrorCode.ToString();
                    Response.Msg = se.Message;
                }
                else
                {
                    Response.Code = ResponseCodeDefines.ServiceError;
                    Response.Msg = "QueryTaskContentForRtmpUrl error info:" + e.Message;
                    Logger.Error(Response.Msg);
                }
            }
            return Response;
        }

        /// <summary>
        /// 获取需要同步的任务(短时间
        /// </summary>
        /// <remarks>
        /// 例子:
        ///
        /// </remarks>
        /// <returns></returns>
        [HttpGet("sync/ready")]  
        [ApiExplorerSettings(GroupName = "v3.0")]
        public async Task<ResponseMessage<List<TaskContentResponse>>> GetNeedSyncTasks()
        {
            var Response = new ResponseMessage<List<TaskContentResponse>>();

            try
            {
                await _taskManage.UpdateComingTasks();

                Response.Ext = await _taskManage.GetNeedSynTasksNew<TaskContentResponse>();

                Logger.Info($"GetNeedSyncTasks v3 result : {Newtonsoft.Json.JsonConvert.SerializeObject(Response.Ext)} ");
            }
            catch (Exception e)
            {
                if (e.GetType() == typeof(SobeyRecException))//sobeyexcep会自动打印错误
                {
                    SobeyRecException se = e as SobeyRecException;
                    Response.Code = se.ErrorCode.ToString();
                    Response.Msg = se.Message;
                }
                else
                {
                    Response.Code = ResponseCodeDefines.ServiceError;
                    Response.Msg = "SetPeriodTaskToNextTime error info:" + e.Message;
                    Logger.Error(Response.Msg);
                }
            }
            return Response;
        }

        /// <summary>
        /// 完成同步任务并置相应状态(-1表示不修改该状态)，解锁
        /// </summary>
        /// <remarks>
        /// 例子:
        ///
        /// </remarks>
        /// <param name="req">请求结构体</param>
        /// <returns></returns>
        [HttpPut("sync/complete")]
        [ApiExplorerSettings(GroupName = "v3.0")]
        public async Task<ResponseMessage> CompleteSyncTasks([FromBody, BindRequired] CompleteSyncTaskRequest req)
        {
            Logger.Info($"CompleteSynTasks v3 CompleteSyncTaskRequest : {req}");

            var Response = new ResponseMessage();

            try
            {
                await _taskManage.CompleteSynTasks(req);
            }
            catch (Exception e)
            {
                if (e.GetType() == typeof(SobeyRecException))//sobeyexcep会自动打印错误
                {
                    SobeyRecException se = e as SobeyRecException;
                    Response.Code = se.ErrorCode.ToString();
                    Response.Msg = se.Message;
                }
                else
                {
                    Response.Code = ResponseCodeDefines.ServiceError;
                    Response.Msg = "CompleteSynTasks error info:" + e.Message;
                    Logger.Error(Response.Msg);
                }
            }
            return Response;
        }


        /// <summary>
        /// 添加任务附加策略和备份信息
        /// </summary>
        /// <remarks>
        /// 例子:
        ///
        ///     POST /Todo
        ///     {
        ///        "id": 1,
        ///        "name": "Item1",
        ///        "isComplete": true
        ///     }
        ///
        /// </remarks>
        /// <param name="task">添加任务数据</param>
        /// <returns>基本任务信息附带任务id</returns>
        [HttpPost("withpolicy")]
        [ApiExplorerSettings(GroupName = "v3.0")]
        public async Task<ResponseMessage<TaskContentResponse>> AddTaskWithPolicy([FromBody, BindRequired] TaskInfoRequest task, [FromHeader(Name = "sobeyhive-http-site"), BindRequired, DefaultValue("S1")] string site)
        {
            Logger.Info($"AddTaskWithPolicy Site task {Request.Host.Value} : {JsonHelper.ToJson(task)}");
            //处理任务名中含有分号的时候，元数据xml不对劲，导致任务总控无法调度，同时含有单斜线的时候，mysql会自动消化掉一个斜线
            var Response = new ResponseMessage<TaskContentResponse>();
            if (task == null)
            {
                Response.Code = ResponseCodeDefines.ModelStateInvalid;
                Response.Msg = "request param error";
                return Response;
            }

            if (string.IsNullOrEmpty(task.TaskContent.Begin) || task.TaskContent.Begin == "0000-00-00 00:00:00")
            {
                Response.Code = ResponseCodeDefines.ModelStateInvalid;
                Response.Msg = "request begin of param error";
                return Response;
            }
            if (string.IsNullOrEmpty(task.TaskContent.End) || task.TaskContent.End == "0000-00-00 00:00:00")
            {
                Response.Code = ResponseCodeDefines.ModelStateInvalid;
                Response.Msg = "request end of param error";
                return Response;
            }

            try
            {
                //Response.Ext = await _taskManage.AddTaskWithPolicy(task, false, string.Empty, string.Empty, string.Empty, string.Empty);
                DbpTask addTask = addTask = await _taskManage.AddTaskWithPolicy(task, false, string.Empty, string.Empty, string.Empty, string.Empty, true
                    //, site
                    );
                
                Response.Ext = _mapper.Map<TaskContentResponse>(addTask);
                if (Response.Ext == null)
                {
                    Response.Code = ResponseCodeDefines.NotFound;
                    Response.Msg = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}:error info: not find data!";
                    return Response;
                }
                if (task.BackUpTask)
                {
                    task.TaskContent = Response.Ext;
                    var backtask = await _taskManage.AddTaskWithPolicy(task, true, string.Empty, string.Empty, string.Empty, string.Empty, false
                        //, site
                        );
                    await _taskManage.UpdateBackupTaskMetadata(Response.Ext.TaskId, backtask.Taskid, task.ContentMeta);
                }

                //添加后如果开始时间在2分钟以内，需要调度一次
                if ((DateTimeFormat.DateTimeFromString(task.TaskContent.Begin) - DateTime.Now).TotalSeconds < 120)
                    await _taskManage.UpdateComingTasks();

                if (ApplicationContext.Current.GlobalNotify && _globalInterface != null)
                {
                    GlobalInternals re = new GlobalInternals() { Funtype = IngestDBCore.GlobalInternals.FunctionType.SetGlobalState, State = GlobalStateName.ADDTASK, TaskID = addTask.Channelid.GetValueOrDefault() };
                    var response1 = await _globalInterface.Value.SubmitGlobalCallBack(re);
                    if (response1.Code != ResponseCodeDefines.SuccessCode)
                    {
                        Logger.Error("SetGlobalState modtask error");
                    }

                }

                Task.Run(() => { _clock.Value.InvokeNotify(GlobalStateName.ADDTASK, NotifyPlugin.Kafka, NotifyAction.ADDTASK, addTask); });
                //SetGTMTaskInfo
                //添加后如果开始时间在2分钟以内，需要调度一次
                //这玩意我完全不知道有啥，放弃，后面改
                //if ((GlobalFun.DateTimeFromString(pIn.taskAdd.strBegin) - DateTime.Now).TotalSeconds < 120)
                //    TASKSERVICE.UpdateComingTasks();
            }
            catch (Exception e)
            {
                if (e.GetType() == typeof(SobeyRecException))//sobeyexcep会自动打印错误
                {
                    SobeyRecException se = e as SobeyRecException;
                    Response.Code = se.ErrorCode.ToString();
                    Response.Msg = se.Message;
                }
                else
                {
                    Response.Code = ResponseCodeDefines.ServiceError;
                    Response.Msg = "AddTaskWithoutPolicy error info:" + e.Message;
                    Logger.Error(Response.Msg);
                }
            }
            return Response;
        }

        /// <summary>
        /// 返回通道中所有正在采集任务
        /// </summary>
        /// <remarks>
        /// 例子:
        ///
        /// </remarks>
        /// <param name="channelid">通道信息</param>
        /// <returns>当前正在采集的任务</returns>
        [HttpGet("capturing/{channelid}")]
        [ApiExplorerSettings(GroupName = "v3.0")]
        public async Task<ResponseMessage<TaskContentResponse>> GetChannelCapturingTaskInfo([FromRoute, BindRequired] int channelid, [FromQuery] int newest)
        {
            var Response = new ResponseMessage<TaskContentResponse>();
            if (channelid < 0)
            {
                Response.Code = ResponseCodeDefines.ModelStateInvalid;
                Response.Msg = "request param error";
            }
            try
            {
                Response.Ext = await _taskManage.GetChannelCapturingTask<TaskContentResponse>(channelid, newest);
                if (Response.Ext == null)
                {
                    Response.Code = ResponseCodeDefines.NotFound;
                    Response.Msg = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}:error info: not find data!";
                }

                Logger.Info($"GetChannelCapturingTaskInfo v3 channelid : {channelid} , newest : {newest} , result : {Newtonsoft.Json.JsonConvert.SerializeObject(Response.Ext)}");
            }
            catch (Exception e)
            {
                if (e.GetType() == typeof(SobeyRecException))//sobeyexcep会自动打印错误
                {
                    SobeyRecException se = e as SobeyRecException;
                    Response.Code = se.ErrorCode.ToString();
                    Response.Msg = se.Message;
                }
                else
                {
                    Response.Code = ResponseCodeDefines.ServiceError;
                    Response.Msg = "GetChannelCapturingTaskInfo error info:" + e.Message;
                    Logger.Error(Response.Msg);
                }
            }
            return Response;
        }

        /// <summary>
        /// 分裂周期任务
        /// </summary>
        /// <remarks>
        /// Decivce通讯接口
        /// </remarks>
        /// <param name="taskid">周期任务id</param>
        /// <returns>分裂后的任务</returns>
        [HttpPost("periodic/{taskid}")]
        [ApiExplorerSettings(GroupName = "v3.0")]
        public async Task<ResponseMessage<TaskContentResponse>> CreatePeriodicTask([FromRoute, BindRequired] int taskid)
        {
            Logger.Info($"CreatePeriodicTask  taskid :{Request.Host.Value} {taskid}");

            var Response = new ResponseMessage<TaskContentResponse>();

            try
            {
                Response.Ext = await _taskManage.CreateNewTaskFromPeriodicTask<TaskContentResponse>(taskid);
                if (Response.Ext == null)
                {
                    Response.Code = ResponseCodeDefines.NotFound;
                    Response.Msg = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}:error info: not find data!";
                    return Response;
                }
                var custom = await _taskManage.GetCustomMetadataAsync<TaskCustomMetadataResponse>(taskid);
                if (custom != null)
                {
                    await _taskManage.UpdateCustomMetadataAsync(Response.Ext.TaskId, custom.Metadata);
                }

                //await _taskManage.
            }
            catch (Exception e)
            {
                if (e is SobeyRecException se)//sobeyexcep会自动打印错误
                {
                    Response.Code = se.ErrorCode.ToString();
                    Response.Msg = se.Message;
                }
                else
                {
                    Response.Code = ResponseCodeDefines.ServiceError;
                    Response.Msg = $"CreateNewTaskFromPeriodicTask error info:{e.Message}";
                    Logger.Error(Response.Msg);
                }
            }
            return Response;
        }

        /// <summary>
        /// 修改周期任务
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <param name="isall">是否全部修改</param>
        /// <param name="req">请求任务元数据基础结构体 体</param>
        /// <returns>任务id</returns>
        [HttpPut("periodic/{taskid}")]
        [ApiExplorerSettings(GroupName = "v3.0")]
        public async Task<ResponseMessage<int>> ModifyPeriodTaskInfo([FromQuery, BindRequired] int isall, [FromBody, BindRequired] TaskContentRequest req)
        {
            Logger.Info($"ModifyPeriodTaskInfo  isall {Request.Host.Value}: {isall}, TaskContentRequest : {JsonHelper.ToJson(req)}");

            var Response = new ResponseMessage<int>();

            try
            {
                //Response.Ext = await _taskManage.ModifyPeriodTask<TaskContentRequest>(req, isall == 1 ? true : false);
                var modifyTask = await _taskManage.ModifyPeriodTask<TaskContentRequest>(req, isall == 1 ? true : false);
                Response.Ext = modifyTask != null ? modifyTask.Taskid : 0;
                if (Response.Ext <= 0)
                {
                    Response.Code = ResponseCodeDefines.NotFound;
                    Response.Msg = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}:error info: not find data!";
                    return Response;
                }
                //添加后如果开始时间在2分钟以内，需要调度一次
                if ((DateTimeFormat.DateTimeFromString(req.Begin) - DateTime.Now).TotalSeconds < 120)
                    await _taskManage.UpdateComingTasks();

                if (ApplicationContext.Current.GlobalNotify && _globalInterface != null)
                {
                    GlobalInternals re = new GlobalInternals() { Funtype = IngestDBCore.GlobalInternals.FunctionType.SetGlobalState, State = GlobalStateName.MODTASK };
                    var response1 = await _globalInterface.Value.SubmitGlobalCallBack(re);
                    if (response1.Code != ResponseCodeDefines.SuccessCode)
                    {
                        Logger.Error("SetGlobalState modtask error");
                    }

                }

                Task.Run(() => { _clock.Value.InvokeNotify(GlobalStateName.MODTASK, NotifyPlugin.Kafka, NotifyAction.MODIFYPERIODCTASK, modifyTask); });
            }
            catch (Exception e)
            {
                if (e is SobeyRecException se)//sobeyexcep会自动打印错误
                {
                    Response.Code = se.ErrorCode.ToString();
                    Response.Msg = se.Message;
                }
                else
                {
                    Response.Code = ResponseCodeDefines.ServiceError;
                    Response.Msg = $"ModifyTaskInfoNmae error info:{e.Message}";
                    Logger.Error(Response.Msg);
                }
            }
            return Response;
        }

        /// <summary>
        /// 获取任务来源
        /// </summary>
        /// <remarks>
        /// 例子:
        ///
        /// </remarks>
        /// <param name="taskid">查询任务id</param>
        /// <returns>任务来源source 枚举</returns>
        [HttpGet("{taskid}/tasksource")]
        [ApiExplorerSettings(GroupName = "v3.0")]
        public async Task<ResponseMessage<TaskSource>> GetTaskSourceById([FromRoute, BindRequired] int taskid)
        {
            var Response = new ResponseMessage<TaskSource>();

            try
            {
                Response.Ext = await _taskManage.GetTaskSource(taskid);

                Logger.Info($"GetTaskSourceById v3 taskid : {taskid} , result : {Newtonsoft.Json.JsonConvert.SerializeObject(Response.Ext)}");
            }
            catch (Exception e)
            {
                if (e.GetType() == typeof(SobeyRecException))//sobeyexcep会自动打印错误
                {
                    SobeyRecException se = e as SobeyRecException;
                    Response.Code = se.ErrorCode.ToString();
                    Response.Msg = se.Message;
                }
                else
                {
                    Response.Code = ResponseCodeDefines.ServiceError;
                    Response.Msg = "GetTaskSource error info:" + e.Message;
                    Logger.Error(Response.Msg);
                }
            }
            return Response;
        }
		
		
		/// <summary>
        /// 任务分段
        /// </summary>
        /// <remarks>
        /// 例子:
        ///
        /// </remarks>
        /// <param name="taskid">老任务id</param>
        /// <param name="newguid">新id</param>
        /// <param name="newname">新名字</param>
        /// <returns>任务信息元数据</returns>
        [HttpPost("split/{taskid}")]
        [ApiExplorerSettings(GroupName = "v3.0")]
        public async Task<ResponseMessage<TaskContentResponse>> SplitTaskInfo([FromRoute, BindRequired] int taskid, [FromQuery, BindRequired] string newguid, [FromQuery, BindRequired] string newname)
        {
            Logger.Info($"SplitTaskInfo  taskid : {taskid}, newguid : {newguid}, newname : {newname}");

            var Response = new ResponseMessage<TaskContentResponse>();

            try
            {
                Response.Ext = await _taskManage.SplitTask<TaskContentResponse>(taskid, newguid, newname);
                if (Response.Ext == null)
                {
                    Response.Code = ResponseCodeDefines.NotFound;
                    Response.Msg = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}:error info: not find data!";
                }
            }
            catch (Exception e)
            {
                if (e.GetType() == typeof(SobeyRecException))//sobeyexcep会自动打印错误
                {
                    SobeyRecException se = e as SobeyRecException;
                    Response.Code = se.ErrorCode.ToString();
                    Response.Msg = se.Message;
                }
                else
                {
                    Response.Code = ResponseCodeDefines.ServiceError;
                    Response.Msg = "ChannelCapturingLowMaterial error info:" + e.Message;
                    Logger.Error(Response.Msg);
                }
            }
            return Response;
        }

        
        /// <summary>
        /// 删除任务
        /// </summary>
        /// <remarks>
        /// 例子:
        ///
        /// </remarks>
        /// <param name="taskid">任务id</param>
        /// <returns>任务id</returns>
        [HttpDelete("{taskid}")]
        [ApiExplorerSettings(GroupName = "v3.0")]
        public async Task<ResponseMessage<int>> DeleteTask([FromRoute, BindRequired] int taskid)
        {
            var Response = new ResponseMessage<int>();

            try
            {
                Logger.Info($"delete task {Request.Host.Value} {taskid}");
                var task = await _taskManage.DeleteTask(taskid);
                if (_globalInterface != null)
                {
                    GlobalInternals re = new GlobalInternals() { Funtype = IngestDBCore.GlobalInternals.FunctionType.SetGlobalState, State = GlobalStateName.DELTASK };
                    var response1 = await _globalInterface.Value.SubmitGlobalCallBack(re);
                    if (response1.Code != ResponseCodeDefines.SuccessCode)
                    {
                        Logger.Error("SetGlobalState modtask error");
                    }

                    if (task == null)
                    {
                        Task.Run(() => { _clock.Value.InvokeNotify(GlobalStateName.DELTASK, NotifyPlugin.NotifyTask, NotifyAction.DELETETASK, new DbpTask() { Taskid = taskid }); });
                    }
                    else
                    {
                        Task.Run(() => { _clock.Value.InvokeNotify(GlobalStateName.MODTASK, NotifyPlugin.NotifyTask, NotifyAction.DELETETASK, task); });
                    }
                }

                Response.Ext = taskid;

            }
            catch (Exception e)
            {
                if (e.GetType() == typeof(SobeyRecException))//sobeyexcep会自动打印错误
                {
                    SobeyRecException se = e as SobeyRecException;
                    Response.Code = se.ErrorCode.ToString();
                    Response.Msg = se.Message;
                }
                else
                {
                    Response.Code = ResponseCodeDefines.ServiceError;
                    Response.Msg = "DeleteTask error info:" + e.Message;
                    Logger.Error(Response.Msg);
                }
            }
            return Response;
        }

        /// <summary>
        /// 根据任务id设置任务状态
        /// </summary>
        /// <remarks>
        /// 例子:
        ///
        /// </remarks>
        /// <param name="taskid">任务id</param>
        /// <param name="state">任务状态</param>
        /// <returns>任务id</returns>
        [HttpPut("{taskid}/state")]
        [ApiExplorerSettings(GroupName = "v3.0")]
        public async Task<ResponseMessage<int>> SetTaskState([FromRoute, BindRequired] int taskid, [FromQuery, BindRequired] int state)
        {
            Logger.Info($"SetTaskState taskid : {taskid}, state : {state}");

            var Response = new ResponseMessage<int>();
            if (taskid <= 0)
            {
                Response.Code = ResponseCodeDefines.ModelStateInvalid;
                Response.Msg = "request param error";
            }

            try
            {
                Response.Ext = await _taskManage.SetTaskState(taskid, state);
                if (Response.Ext <= 0)
                {
                    Response.Code = ResponseCodeDefines.NotFound;
                    Response.Msg = "not found task";
                    return Response;
                }

                Task.Run(() => { _clock.Value.InvokeNotify(GlobalStateName.MODTASK, NotifyPlugin.NotifyTask, NotifyAction.MODIFYTASKSTATE, new DbpTask() { Taskid = taskid, State = state }); });
            }
            catch (Exception e)
            {
                if (e.GetType() == typeof(SobeyRecException))//sobeyexcep会自动打印错误
                {
                    SobeyRecException se = e as SobeyRecException;
                    Response.Code = se.ErrorCode.ToString();
                    Response.Msg = se.Message;
                }
                else
                {
                    Response.Code = ResponseCodeDefines.ServiceError;
                    Response.Msg = "SetTaskState error info:" + e.Message;
                    Logger.Error(Response.Msg);
                }
            }
            return Response;
        }

		
		 /// <summary>
        /// 根据任务id停止任务
        /// </summary>
        /// <remarks>
        /// 例子:
        ///
        /// </remarks>
        /// <param name="taskid">任务id，</param>
        /// <returns>任务id</returns>
        [HttpPut("stop/{taskid}")]
        [ApiExplorerSettings(GroupName = "v3.0")]
        public async Task<ResponseMessage<int>> StopTask([FromRoute, BindRequired] int taskid)
        {
            Logger.Info($"StopTask taskid {Request.Host.Value}: {taskid}");
            var Response = new ResponseMessage<int>();
            if (taskid <= 0)
            {
                Response.Code = ResponseCodeDefines.ModelStateInvalid;
                Response.Msg = "request param error";
            }

            try
            {
                var task = await _taskManage.StopTask(taskid, DateTime.MinValue);

                if (task == null || task.Taskid < 1)
                {
                    Response.Code = ResponseCodeDefines.NotFound;
                    Response.Msg = "not found task";
                }

                Response.Ext = task.Taskid;

                if (ApplicationContext.Current.GlobalNotify && _globalInterface != null)
                {
                    GlobalInternals re = new GlobalInternals() { Funtype = IngestDBCore.GlobalInternals.FunctionType.SetGlobalState, State = GlobalStateName.MODTASK };
                    var response1 = await _globalInterface.Value.SubmitGlobalCallBack(re);
                    if (response1.Code != ResponseCodeDefines.SuccessCode)
                    {
                        Logger.Error("SetGlobalState modtask error");
                    }

                }

                Task.Run(() => { _clock.Value.InvokeNotify(GlobalStateName.MODTASK, NotifyPlugin.Kafka, NotifyAction.STOPTASK, task); });
            }
            catch (Exception e)
            {
                if (e.GetType() == typeof(SobeyRecException))//sobeyexcep会自动打印错误
                {
                    SobeyRecException se = e as SobeyRecException;
                    Response.Code = se.ErrorCode.ToString();
                    Response.Msg = se.Message;
                }
                else
                {
                    Response.Code = ResponseCodeDefines.ServiceError;
                    Response.Msg = "StopTask error info:" + e.Message;
                    Logger.Error(Response.Msg);
                }
            }
            return Response;
        }

       
        /// <summary>
        /// 根据传入的任务ID(占位任务ID),修改任务类型,开始占位任务的执行
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <param name="taskid">占位任务id</param>
        /// <returns>分裂后的任务</returns>
        [HttpPost("starttieup/{taskid}")]
        [ApiExplorerSettings(GroupName = "v3.0")]
        public async Task<ResponseMessage<bool>> StartTieUpTask([FromRoute, BindRequired] int taskid)
        {
            Logger.Info($"StartTieUpTask  taskid : {taskid}");

            var Response = new ResponseMessage<bool>();

            try
            {
                //Response.Ext = await _taskManage.StartTieupTask(taskid);
                var tieupTask = await _taskManage.StartTieupTask(taskid);
                Response.Ext = (tieupTask != null && tieupTask.Tasktype == (int)TaskType.TT_TIEUP) ? true : false;

                if (ApplicationContext.Current.GlobalNotify && _globalInterface != null)
                {
                    GlobalInternals re = new GlobalInternals() { Funtype = IngestDBCore.GlobalInternals.FunctionType.SetGlobalState, State = GlobalStateName.MODTASK };
                    var response1 = await _globalInterface.Value.SubmitGlobalCallBack(re);
                    if (response1.Code != ResponseCodeDefines.SuccessCode)
                    {
                        Logger.Error("SetGlobalState modtask error");
                    }

                }

                Task.Run(() => { _clock.Value.InvokeNotify(GlobalStateName.MODTASK, NotifyPlugin.Kafka, NotifyAction.MODIFYTASK, tieupTask); });
                //await _taskManage.
            }
            catch (Exception e)
            {
                if (e is SobeyRecException se)//sobeyexcep会自动打印错误
                {
                    Response.Code = se.ErrorCode.ToString();
                    Response.Msg = se.Message;
                }
                else
                {
                    Response.Code = ResponseCodeDefines.ServiceError;
                    Response.Msg = $"StartTieUpTask error info:{e.Message}";
                    Logger.Error(Response.Msg);
                }
            }
            return Response;
        }

    }
}
