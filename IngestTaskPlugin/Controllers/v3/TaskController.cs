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
using System.Text;
using System.Threading.Tasks;

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
        [ApiExplorerSettings(GroupName = "v3")]
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
        [ApiExplorerSettings(GroupName = "v3")]
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
        [ApiExplorerSettings(GroupName = "v3")]
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
        [ApiExplorerSettings(GroupName = "v3")]
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
        /// 获取需要同步的任务(短时间
        /// </summary>
        /// <remarks>
        /// 例子:
        ///
        /// </remarks>
        /// <returns></returns>
        [HttpGet("needsync")]
        [ApiExplorerSettings(GroupName = "v3")]
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
        /// 返回通道中所有正在采集任务
        /// </summary>
        /// <remarks>
        /// 例子:
        ///
        /// </remarks>
        /// <param name="channelid">通道信息</param>
        /// <returns>当前正在采集的任务</returns>
        [HttpGet("capturing/{channelid}")]
        [ApiExplorerSettings(GroupName = "v3")]
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
        /// 获取任务来源
        /// </summary>
        /// <remarks>
        /// 例子:
        ///
        /// </remarks>
        /// <param name="taskid">查询任务id</param>
        /// <returns>任务来源source 枚举</returns>
        [HttpGet("{taskid}/tasksource")]
        [ApiExplorerSettings(GroupName = "v3")]
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
        /// 完成同步任务并置相应状态(-1表示不修改该状态)，解锁
        /// </summary>
        /// <remarks>
        /// 例子:
        ///
        /// </remarks>
        /// <param name="req">请求结构体</param>
        /// <returns></returns>
        [HttpPut("completesync")]
        [ApiExplorerSettings(GroupName = "v3")]
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
        /// 删除任务
        /// </summary>
        /// <remarks>
        /// 例子:
        ///
        /// </remarks>
        /// <param name="taskid">任务id</param>
        /// <returns>任务id</returns>
        [HttpDelete("{taskid}")]
        [ApiExplorerSettings(GroupName = "v2")]
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
        [ApiExplorerSettings(GroupName = "v3")]
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
        /// 分裂周期任务
        /// </summary>
        /// <remarks>
        /// Decivce通讯接口
        /// </remarks>
        /// <param name="taskid">周期任务id</param>
        /// <returns>分裂后的任务</returns>
        [HttpPost("periodic/{taskid}")]
        [ApiExplorerSettings(GroupName = "v3")]
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


    }
}
