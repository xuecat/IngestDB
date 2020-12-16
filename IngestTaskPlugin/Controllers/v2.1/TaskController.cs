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
using System.Text;
using System.Threading.Tasks;

namespace IngestTaskPlugin.Controllers.v2._1
{
    
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("2.1")]
    [ApiController]
    public partial class TaskController : ControllerBase
    {
        private readonly ILogger Logger = LoggerManager.GetLogger("TaskInfo21");
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
        [ApiExplorerSettings(GroupName = "v2.1")]
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
        [ApiExplorerSettings(GroupName = "v2.1")]
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
        [ApiExplorerSettings(GroupName = "v2.1")]
        public async Task<ResponseMessage<TaskContentResponse>> AddRescheduleTaskByold([FromRoute, BindRequired]int taskid)
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
                    GlobalInternals re = new GlobalInternals() { Funtype = IngestDBCore.GlobalInternals.FunctionType.SetGlobalState, State = GlobalStateName.ADDTASK, TaskID = addTask.Channelid.GetValueOrDefault() };
                    var response1 = await _globalInterface.Value.SubmitGlobalCallBack(re);
                    if (response1.Code != ResponseCodeDefines.SuccessCode)
                    {
                        Logger.Error("SetGlobalState modtask error");
                    }

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
                    Response.Msg = "AddRescheduleTaskByold 21 error info:" + e.Message;
                    Logger.Error(Response.Msg);
                }
            }
            return Response;
        }
    }
}
