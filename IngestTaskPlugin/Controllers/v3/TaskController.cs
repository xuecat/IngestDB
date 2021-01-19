using AutoMapper;
using IngestDBCore;
using IngestDBCore.Interface;
using IngestDBCore.Notify;
using IngestDBCore.Tool;
using IngestTaskPlugin.Dto.OldResponse;
using IngestTaskPlugin.Dto.Response;
using IngestTaskPlugin.Managers;
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
        private readonly NotifyClock _clock;
        public TaskController(TaskManager task, IServiceProvider services, IMapper mapper, NotifyClock clock)
        {
            _taskManage = task;
            _clock = clock;
            _globalInterface = new Lazy<IIngestGlobalInterface>(() => services.GetRequiredService<IIngestGlobalInterface>());
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

                    Task.Run(() => { _clock.InvokeNotify(GlobalStateName.ADDTASK, NotifyPlugin.Kafka, NotifyAction.ADDTASK, addTask); });
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

                    Task.Run(() => { _clock.InvokeNotify(GlobalStateName.MODTASK, NotifyPlugin.Kafka, NotifyAction.MODIFYTASK, addTask); });
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
        ///
        /// </remarks>
        /// <param name="unitid">1是客户查询任务，跨天每天任务做分裂，2是web查询任务，跨天每天任务不做分裂</param>
        /// <param name="day">查询时间yyyy/MM/dd HH:mm:ss </param>
        /// <param name="timemode">查询模式0是24小时模式，1是32小时模式</param>
        /// <returns>任务基础元数据</returns>
        [HttpGet("onedaytask")]
        [ApiExplorerSettings(GroupName = "v3.0")]
        public async Task<ResponseMessage<List<TaskContentResponse>>> QueryTaskContent([FromQuery, BindRequired] int unitid, [FromQuery, BindRequired] string day, [FromQuery, BindRequired] int timemode, [FromHeader(Name = "sobeyhive-http-site"), BindRequired, DefaultValue("S1")] string site)
        {
            var Response = new ResponseMessage<List<TaskContentResponse>>();

            try
            {
                Response.Ext = await _taskManage.QueryTaskContentBySite<TaskContentResponse>(unitid, DateTimeFormat.DateTimeFromString(day), (TimeLineType)timemode, site);
                if (Response.Ext == null)
                {
                    Response.Code = ResponseCodeDefines.NotFound;
                    Response.Msg = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}:error info: not find data!";
                }
                Logger.Error($"QueryTaskContent Site Result : {Newtonsoft.Json.JsonConvert.SerializeObject(Response.Ext)}");
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
                    Response.Msg = "QueryTaskContent error info:" + e.Message;
                    Logger.Error(Response.Msg);
                }
            }
            return Response;
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
        [HttpGet("taskinfo/{taskid}")]
        [ApiExplorerSettings(GroupName = "v3")]
        public async Task<ResponseMessage<TaskInfoResponse>> GetTaskInfoAllByID([FromRoute, BindRequired] int taskid)
        {
            var Response = new ResponseMessage<TaskInfoResponse>();
            if (taskid <= 0)
            {
                Response.Code = ResponseCodeDefines.ModelStateInvalid;
                Response.Msg = "request param error";
            }

            try
            {
                Response.Ext = await _taskManage.GetTaskInfoAll<TaskInfoResponse>(taskid);
                if (Response.Ext == null)
                {
                    Response.Code = ResponseCodeDefines.NotFound;
                    Response.Msg = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}:error info: not find data!";
                }
                Logger.Error($"GetTaskInfoAllByID Site taskid : {taskid} Result : {Newtonsoft.Json.JsonConvert.SerializeObject(Response.Ext)}");
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
                    Response.Msg = "GetTaskInfoAllByID error info:" + e.Message;
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
        [HttpPost("withpolicytask")]
        [ApiExplorerSettings(GroupName = "v3")]
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
                var addTask = await _taskManage.AddTaskWithPolicy(task, false, string.Empty, string.Empty, string.Empty, string.Empty);
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
                    var backtask = await _taskManage.AddTaskWithPolicy(task, true, string.Empty, string.Empty, string.Empty, string.Empty, false);
                    await _taskManage.UpdateBackupTaskMetadata(Response.Ext.TaskId, backtask.Taskid, task.ContentMeta);
                }

                //添加后如果开始时间在2分钟以内，需要调度一次
                if ((DateTimeFormat.DateTimeFromString(task.TaskContent.Begin) - DateTime.Now).TotalSeconds < 120)
                    await _taskManage.UpdateComingTasks();

                if (_globalInterface != null && ApplicationContext.Current.GlobalNotify)
                {
                    GlobalInternals re = new GlobalInternals() { Funtype = IngestDBCore.GlobalInternals.FunctionType.SetGlobalState, State = GlobalStateName.ADDTASK, TaskID = addTask.Channelid.GetValueOrDefault() };
                    var response1 = await _globalInterface.Value.SubmitGlobalCallBack(re);
                    if (response1.Code != ResponseCodeDefines.SuccessCode)
                    {
                        Logger.Error("SetGlobalState modtask error");
                    }

                    Task.Run(() => { _clock.InvokeNotify(GlobalStateName.ADDTASK, NotifyPlugin.Kafka, NotifyAction.ADDTASK, addTask); });
                }
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

    }
}
