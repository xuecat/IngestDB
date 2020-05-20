﻿using IngestDBCore;
using IngestDBCore.Basic;
using IngestDBCore.Interface;
using IngestDBCore.Tool;
using IngestTaskPlugin.Dto.Response;
using IngestTaskPlugin.Managers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Sobey.Core.Log;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;

using TaskMaterialMetaRequest = IngestTaskPlugin.Dto.Response.TaskMaterialMetaResponse;
using TaskContentMetaRequest = IngestTaskPlugin.Dto.Response.TaskContentMetaResponse;
using TaskPlanningRequest = IngestTaskPlugin.Dto.Response.TaskPlanningResponse;
using TaskSplitRequest = IngestTaskPlugin.Dto.Response.TaskSplitResponse;
using PropertyRequest = IngestTaskPlugin.Dto.Response.PropertyResponse;
using TaskInfoRequest = IngestTaskPlugin.Dto.Response.TaskInfoResponse;
using TaskContentRequest = IngestTaskPlugin.Dto.Response.TaskContentResponse;
using TaskCustomMetadataRequest = IngestTaskPlugin.Dto.Response.TaskCustomMetadataResponse;
using IngestDBCore.Notify;
using IngestTaskPlugin.Dto;
using IngestTaskPlugin.Dto.OldResponse;

/// 
///MADEBYINGEST没更新完
///
///manager有个addtaskwithpolicy
///AddTaskWithPolicy 有个GetTaskSourceBySignalId 还有addtask有些地方 是直接加数据库
///代码美观对齐
///

namespace IngestTaskPlugin.Controllers.v2
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("2.0")]
    [ApiController]
    public partial class TaskController : ControllerBase
    {
        private readonly ILogger Logger = LoggerManager.GetLogger("TaskInfo");
        private readonly TaskManager _taskManage;
        private readonly NotifyClock _clock;
        private readonly Lazy<IIngestGlobalInterface> _globalInterface;
        //private readonly IMapper _mapper;

        public TaskController( TaskManager task, IServiceProvider services, NotifyClock clock/*, IMapper mapper*/)
        {
            _taskManage = task;
            _clock = clock;
            _globalInterface = new Lazy<IIngestGlobalInterface>(() => services.GetRequiredService<IIngestGlobalInterface>());
            //_mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// <summary>
        /// 获取material元数据
        /// </summary>
        /// <remarks>
        /// 例子:
        ///     Get api/v2/task/taskmaterialmetadata/1
        /// </remarks>
        /// <param name="taskid">任务id</param>
        /// <returns>素材任务元数据结构体</returns>     
        [HttpGet("materialmetadata/{taskid}")]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<TaskMaterialMetaResponse>> GetTaskMaterialMetaData([FromRoute, BindRequired]int taskid)
        {
            var Response = new ResponseMessage<TaskMaterialMetaResponse>();
            if (taskid < 1)
            {
                Response.Code = ResponseCodeDefines.ModelStateInvalid;
                Response.Msg = "请求参数不正确";
            }
            try
            {
                Response.Ext = await _taskManage.GetTaskMaterialMetadataAsync(taskid);
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
                    Response.Msg = "GetTaskMaterialMetaData error info：" +e.Message;
                    Logger.Error(Response.Msg);
                }
            }
            return Response;
        }

        /// <summary>
        /// 获取content元数据
        /// </summary>
        /// <remarks>
        /// 例子:
        ///     Get api/v2/task/taskcontentmetadata/1
        /// </remarks>
        /// <param name="taskid">任务id</param>
        /// <returns>任务元数据结构体</returns>     
        [HttpGet("contentmetadata/{taskid}")]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<TaskContentMetaResponse>> GetTaskContentMetaData([FromRoute, BindRequired]int taskid)
        {
            var Response = new ResponseMessage<TaskContentMetaResponse>();
            if (taskid < 1)
            {
                Response.Code = ResponseCodeDefines.ModelStateInvalid;
                Response.Msg = "请求参数不正确";
            }
            try
            {
                Response.Ext = await _taskManage.GetTaskContentMetadataAsync(taskid);
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
                    Response.Msg = "GetTaskContentMetaData error info：" +e.Message;
                    Logger.Error(Response.Msg);
                }
            }
            return Response;
        }

        /// <summary>
        /// 获取planning元数据
        /// </summary>
        /// <remarks>
        /// 例子:
        ///     Get api/v2/task/taskplanningmetadata/1
        /// </remarks>
        /// <param name="taskid">任务id</param>
        /// <returns>任务计划元数据结构体</returns>     
        [HttpGet("planningmetadata/{taskid}")]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<TaskPlanningResponse>> GetTaskPlanningMetaData([FromRoute, BindRequired]int taskid)
        {
            var Response = new ResponseMessage<TaskPlanningResponse>();
            if (taskid < 1)
            {
                Response.Code = ResponseCodeDefines.ModelStateInvalid;
                Response.Msg = "请求参数不正确";
            }
            try
            {
                Response.Ext = await _taskManage.GetTaskPlanningMetadataAsync(taskid);
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
                    Response.Msg = "GetTaskPlanningMetaData error info：" +e.Message;
                    Logger.Error(Response.Msg);
                }
            }
            return Response;
        }

        /// <summary>
        /// 更新所有任务的元数据
        /// </summary>
        /// <remarks>
        /// 假如没有此属性会新加，有此属性会更新
        /// 例子:
        ///     Get api/v2/task/taskmetadata/1
        /// </remarks>
        /// <param name="taskid"></param>
        /// <param name="tasktype">元数据类型</param>
        /// <param name="lst">键值对应需要更新的数据，proterty和value</param>
        /// <returns>任务计划元数据结构体</returns>     
        [HttpPost("metadata/{taskid}")]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<string>> UpdateTaskMetaData([FromRoute, BindRequired]int taskid, [FromQuery, BindRequired]int tasktype, [FromBody, BindRequired]List<PropertyResponse> lst)
        {
            var Response = new ResponseMessage<string>();
            if (taskid < 1)
            {
                Response.Code = ResponseCodeDefines.ModelStateInvalid;
                Response.Msg = "请求参数不正确";
            }
            try
            {
                Response.Ext = await _taskManage.UpdateMetadataPropertyAsync(taskid, tasktype, lst);
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
                    Response.Msg = "UpdateTaskMetaData error info：" +e.Message;
                    Logger.Error(Response.Msg);
                }
            }
            return Response;
        }

        /// <summary>
        /// 获取自定义元数据
        /// </summary>
        /// <remarks>
        /// 例子:
        ///     Get api/v2/task/taskcustommetadata/1
        /// </remarks>
        /// <param name="taskid">任务id</param>
        /// <returns>获取任务自定义数据</returns>     
        [HttpGet("custommetadata/{taskid}")]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<TaskCustomMetadataResponse>> GetTaskCustomMetaData([FromRoute, BindRequired]int taskid)
        {
            var Response = new ResponseMessage<TaskCustomMetadataResponse>();
            if (taskid < 1)
            {
                Response.Code = ResponseCodeDefines.ModelStateInvalid;
                Response.Msg = "请求参数不正确";
            }
            try
            {
                Response.Ext = await _taskManage.GetCustomMetadataAsync<TaskCustomMetadataResponse>(taskid);
                if (Response.Ext == null)
                {
                    Response.Code = ResponseCodeDefines.NotFound;
                    Response.Msg = "not find metadata"
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
                    Response.Msg = "GetTaskCustomMetaData error info：" +e.Message;
                    Logger.Error(Response.Msg);
                }
            }
            return Response;
        }

        /// <summary>
        /// 更新任务自定义元数据
        /// </summary>
        /// <remarks>
        /// 例子:
        ///     Get api/v2/task/taskcustommetadata/1
        /// </remarks>
        /// <param name="taskid">任务id</param>
        /// <param name="data">更新数据，taskid填不填看你，我不会用</param>
        /// <returns>任务id</returns>     
        [HttpPost("custommetadata/{taskid}")]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<int>> UpdateTaskCustomMetaData([FromRoute, BindRequired]int taskid, [FromBody, BindRequired]TaskCustomMetadataRequest data)
        {
            var Response = new ResponseMessage<int>();
            if (taskid < 1)
            {
                Response.Code = ResponseCodeDefines.ModelStateInvalid;
                Response.Msg = "请求参数不正确";
            }
            try
            {
                await _taskManage.UpdateCustomMetadataAsync(taskid, data.Metadata);
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
                    Response.Msg = "UpdateTaskCustomMetaData error info：" +e.Message;
                    Logger.Error(Response.Msg);
                }
            }
            return Response;
        }

        /// <summary>
        /// 停止组任务
        /// </summary>
        /// <remarks>
        /// 例子:
        ///     Get api/v2/task/grouptask/stop/1
        /// </remarks>
        /// <param name="taskid">单个组任务id</param>
        /// <returns>停止的所有任务id</returns>
        [HttpPut("grouptask/stop/{taskid}")]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<List<int>>> StopGroupTask([FromRoute, BindRequired]int taskid)
        {
            var Response = new ResponseMessage<List<int>>();
            if (taskid < 1)
            {
                Response.Code = ResponseCodeDefines.ModelStateInvalid;
                Response.Msg = "请求参数不正确";
            }
            try
            {
                Response.Ext = await _taskManage.StopGroupTaskAsync(taskid);

                if (_globalInterface != null)
                {
                    GlobalInternals re = new GlobalInternals() { Funtype = IngestDBCore.GlobalInternals.FunctionType.SetGlobalState, State = GlobalStateName.MODTASK };
                    var response1 = await _globalInterface.Value.SubmitGlobalCallBack(re);
                    if (response1.Code != ResponseCodeDefines.SuccessCode)
                    {
                        Logger.Error("SetGlobalState modtask error");
                    }

                    foreach (var item in Response.Ext)
                    {
                        await Task.Run(() => { _clock.InvokeNotify(GlobalStateName.MODTASK, NotifyPlugin.Kafka, NotifyAction.STOPGROUPTASK, item); });
                    }

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
                    Response.Msg = "StopGroupTask error info：" +e.Message;
                    Logger.Error(Response.Msg);
                }
            }
            return Response;
        }

        /// <summary>
        /// 删除组任务
        /// </summary>
        /// <remarks>
        /// 例子:
        ///     Get api/v2/task/grouptask/1
        /// </remarks>
        /// <param name="taskid">单个组任务id</param>
        /// <returns>删除的所有任务id</returns>
        [HttpDelete("grouptask/{taskid}")]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<List<int>>> DeleteGroupTask([FromRoute, BindRequired]int taskid)
        {
            var Response = new ResponseMessage<List<int>>();
            if (taskid < 1)
            {
                Response.Code = ResponseCodeDefines.ModelStateInvalid;
                Response.Msg = "请求参数不正确";
            }
            try
            {
                Response.Ext = await _taskManage.DeleteGroupTaskAsync(taskid);

                
                if (_globalInterface != null)
                {
                    GlobalInternals re = new GlobalInternals() { Funtype = IngestDBCore.GlobalInternals.FunctionType.SetGlobalState, State = GlobalStateName.MODTASK };
                    var response1 = await _globalInterface.Value.SubmitGlobalCallBack(re);
                    if (response1.Code != ResponseCodeDefines.SuccessCode)
                    {
                        Logger.Error("SetGlobalState modtask error");
                    }

                    foreach (var item in Response.Ext)
                    {
                        await Task.Run(() => { _clock.InvokeNotify(GlobalStateName.MODTASK, NotifyPlugin.Kafka, NotifyAction.STOPGROUPTASK, item); });
                    }
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
                    Response.Msg = "DeleteGroupTask error info：" +e.Message;
                    Logger.Error(Response.Msg);
                }
            }
            return Response;
        }

        /// <summary>
        /// 仅仅添加任务对应老接口的PostAddTaskSvr (fow gpi 引入在用;;;   web别用 )
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
        [HttpPost("withoutpolicytask")]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<TaskContentResponse>> AddTaskWithoutPolicy([FromBody, BindRequired]TaskInfoRequest task)
        {
            var Response = new ResponseMessage<TaskContentResponse>();
            if (task == null)
            {
                Response.Code = ResponseCodeDefines.ModelStateInvalid;
                Response.Msg = "请求参数不正确";
            }
            try
            {
                Response.Ext = await _taskManage.AddTaskWithoutPolicy(task, string.Empty, string.Empty, string.Empty, string.Empty);

                //添加后如果开始时间在2分钟以内，需要调度一次
                if ((DateTimeFormat.DateTimeFromString(task.TaskContent.Begin) - DateTime.Now).TotalSeconds < 120)
                    await _taskManage.UpdateComingTasks();
                
                if (_globalInterface != null)
                {
                    GlobalInternals re = new GlobalInternals() { Funtype = IngestDBCore.GlobalInternals.FunctionType.SetGlobalState, State = GlobalStateName.ADDTASK };
                    var response1 = await _globalInterface.Value.SubmitGlobalCallBack(re);
                    if (response1.Code != ResponseCodeDefines.SuccessCode)
                    {
                        Logger.Error("SetGlobalState modtask error");
                    }

                    await Task.Run(() => { _clock.InvokeNotify(GlobalStateName.ADDTASK, NotifyPlugin.Kafka, NotifyAction.ADDTASK, Response.Ext.TaskID); });
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
                    Response.Msg = "AddTaskWithoutPolicy error info：" +e.Message;
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
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<TaskContentResponse>> AddTaskWithPolicy([FromBody, BindRequired]TaskInfoRequest task)
        {
            //处理任务名中含有分号的时候，元数据xml不对劲，导致任务总控无法调度，同时含有单斜线的时候，mysql会自动消化掉一个斜线
            var Response = new ResponseMessage<TaskContentResponse>();
            if (task == null)
            {
                Response.Code = ResponseCodeDefines.ModelStateInvalid;
                Response.Msg = "请求参数不正确";
            }
            try
            {
                Response.Ext = await _taskManage.AddTaskWithPolicy(task, false, string.Empty, string.Empty, string.Empty, string.Empty);

                if (task.BackUpTask)
                {
                    task.TaskContent = Response.Ext;
                    await _taskManage.AddTaskWithPolicy(task, true, string.Empty, string.Empty, string.Empty, string.Empty);
                }

                //添加后如果开始时间在2分钟以内，需要调度一次
                if ((DateTimeFormat.DateTimeFromString(task.TaskContent.Begin) - DateTime.Now).TotalSeconds < 120)
                    await _taskManage.UpdateComingTasks();
                
                if (_globalInterface != null)
                {
                    GlobalInternals re = new GlobalInternals() { Funtype = IngestDBCore.GlobalInternals.FunctionType.SetGlobalState, State = GlobalStateName.ADDTASK };
                    var response1 = await _globalInterface.Value.SubmitGlobalCallBack(re);
                    if (response1.Code != ResponseCodeDefines.SuccessCode)
                    {
                        Logger.Error("SetGlobalState modtask error");
                    }

                    await Task.Run(() => { _clock.InvokeNotify(GlobalStateName.ADDTASK, NotifyPlugin.Kafka, NotifyAction.ADDTASK, Response.Ext.TaskID); });
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
                    Response.Msg = "AddTaskWithoutPolicy error info：" +e.Message;
                    Logger.Error(Response.Msg);
                }
            }
            return Response;
        }

        /// <summary>
        /// 通过guid返回taskid(taskguid和素材guid一样,入库使用同一个)
        /// </summary>
        /// <remarks>
        /// 例子:
        ///
        /// </remarks>
        /// <param name="taskguid">guid信息</param>
        /// <returns>任务id</returns>
        [HttpGet("backid")]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<int>> TaskIDByTaskGUID([FromQuery, BindRequired]string taskguid)
        {
            var Response = new ResponseMessage<int>();
            if (string.IsNullOrEmpty(taskguid))
            {
                Response.Code = ResponseCodeDefines.ModelStateInvalid;
                Response.Msg = "请求参数不正确";
            }
            try
            {
                Response.Ext = await _taskManage.GetTaskIDByTaskGUID(taskguid);
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
                    Response.Msg = "TaskIDByTaskGUID error info：" +e.Message;
                    Logger.Error(Response.Msg);
                }
            }
            return Response;
        }

        /// <summary>
        /// 返回所有正在采集任务
        /// </summary>
        /// <remarks>
        /// 例子:
        ///
        /// </remarks>
        /// <returns>采集任务信息</returns>
        [HttpGet("capturing/all")]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<List<TaskContentResponse>>> GetAllChannelCapturingTaskInfo()
        {
            var Response = new ResponseMessage<List<TaskContentResponse>>();

            try
            {
                Response.Ext = await _taskManage.GetAllChannelCapturingTask<TaskContentResponse>();
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
                    Response.Msg = "GetAllChannelCapturingTaskInfo error info：" +e.Message;
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
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<TaskContentResponse>> GetChannelCapturingTaskInfo([FromRoute, BindRequired]int channelid)
        {
            var Response = new ResponseMessage<TaskContentResponse>();
            if (channelid < 0)
            {
                Response.Code = ResponseCodeDefines.ModelStateInvalid;
                Response.Msg = "请求参数不正确";
            }
            try
            {
                Response.Ext = await _taskManage.GetChannelCapturingTask<TaskContentResponse>(channelid);
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
                    Response.Msg = "TaskIDByTaskGUID error info：" +e.Message;
                    Logger.Error(Response.Msg);
                }
            }
            return Response;
        }

        /// <summary>
        /// 修改任务元数据，不包括任务metadata数据
        /// </summary>
        /// <remarks>
        /// 例子:
        ///
        /// </remarks>
        /// <param name="taskid">任务id，给不给值无所谓只是为了好看</param>
        /// <param name="req">修改请求体，请填入taskid信息</param>
        /// <returns>任务信息</returns>
        [HttpPut("content/{taskid}")]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<TaskContentResponse>> ModifyTask([FromRoute, BindRequired]int taskid, [FromBody, BindRequired]TaskContentRequest req)
        {
            var Response = new ResponseMessage<TaskContentResponse>();
            if (req == null)
            {
                Response.Code = ResponseCodeDefines.ModelStateInvalid;
                Response.Msg = "请求参数不正确";
            }

            try
            {
                Response.Ext = await _taskManage.ModifyTask<TaskContentResponse>(req, string.Empty, string.Empty, string.Empty, string.Empty);

                //添加后如果开始时间在2分钟以内，需要调度一次
                if ((DateTimeFormat.DateTimeFromString(req.Begin) - DateTime.Now).TotalSeconds < 120)
                    await _taskManage.UpdateComingTasks();
                
                if (_globalInterface != null)
                {
                    GlobalInternals re = new GlobalInternals() { Funtype = IngestDBCore.GlobalInternals.FunctionType.SetGlobalState, State = GlobalStateName.MODTASK };
                    var response1 = await _globalInterface.Value.SubmitGlobalCallBack(re);
                    if (response1.Code != ResponseCodeDefines.SuccessCode)
                    {
                        Logger.Error("SetGlobalState modtask error");
                    }

                    await Task.Run(() => { _clock.InvokeNotify(GlobalStateName.MODTASK, NotifyPlugin.Kafka, NotifyAction.MODIFYTASK, Response.Ext.TaskID); });
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
                    Response.Msg = "TaskIDByTaskGUID error info：" +e.Message;
                    Logger.Error(Response.Msg);
                }
            }
            return Response;
        }

        /// <summary>
        /// 修改任务所有的全元数据，包括任务metadata数据（注意metadata传入的数据要做判断空处理，当成员属性为空时，就不会更新进去， 注意:采集参数还是全更新）
        /// </summary>
        /// <remarks>
        /// 例子:
        ///
        /// </remarks>
        /// <param name="taskid">任务id，给不给值无所谓只是为了好看</param>
        /// <param name="req">修改请求体，请填入taskid信息</param>
        /// <returns>任务元数据信息</returns>
        [HttpPut("taskinfo/{taskid}")]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<TaskContentResponse>> ModifyAllTask([FromRoute, BindRequired]int taskid, [FromBody, BindRequired]TaskInfoRequest req)
        {
            var Response = new ResponseMessage<TaskContentResponse>();
            if (req == null)
            {
                Response.Code = ResponseCodeDefines.ModelStateInvalid;
                Response.Msg = "请求参数不正确";
            }

            try
            {
                Response.Ext = await _taskManage.ModifyTask<TaskContentResponse>(req.TaskContent, req.CaptureMeta,
                    _taskManage.ConverTaskContentMetaString(req.ContentMeta),
                    _taskManage.ConverTaskMaterialMetaString(req.MaterialMeta),
                    _taskManage.ConverTaskPlanningMetaString(req.PlanningMeta));

                //添加后如果开始时间在2分钟以内，需要调度一次
                if ((DateTimeFormat.DateTimeFromString(req.TaskContent.Begin) - DateTime.Now).TotalSeconds < 120)
                    await _taskManage.UpdateComingTasks();

                if (_globalInterface != null)
                {
                    GlobalInternals re = new GlobalInternals() { Funtype = IngestDBCore.GlobalInternals.FunctionType.SetGlobalState, State = GlobalStateName.MODTASK };
                    var response1 = await _globalInterface.Value.SubmitGlobalCallBack(re);
                    if (response1.Code != ResponseCodeDefines.SuccessCode)
                    {
                        Logger.Error("SetGlobalState modtask error");
                    }

                    await Task.Run(() => { _clock.InvokeNotify(GlobalStateName.MODTASK, NotifyPlugin.Kafka, NotifyAction.MODIFYTASK, Response.Ext.TaskID); });
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
                    Response.Msg = "TaskIDByTaskGUID error info：" + e.Message;
                    Logger.Error(Response.Msg);
                }
            }
            return Response;
        }

        /// <summary>
        /// 获取任务基础元数据
        /// </summary>
        /// <remarks>
        /// 例子:
        ///
        /// </remarks>
        /// <param name="taskid">任务id，</param>
        /// <param name="changestate">fsw和web用，传1；显示任务状态时会由服务自动修改状态，1 未分发和采集中被删除任务，会变成为删除状态  0 不做服务器更新</param>
        /// <returns>任务内容信息</returns>
        [HttpGet("taskinfo/{taskid}")]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<TaskContentResponse>> GetTaskInfoByID([FromRoute, BindRequired]int taskid, [FromQuery, BindRequired]int changestate)
        {
            var Response = new ResponseMessage<TaskContentResponse>();
            if (taskid <= 0)
            {
                Response.Code = ResponseCodeDefines.ModelStateInvalid;
                Response.Msg = "请求参数不正确";
            }

            try
            {
                Response.Ext = await _taskManage.GetTaskInfoByID<TaskContentResponse>(taskid, changestate);

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
                    Response.Msg = "TaskIDByTaskGUID error info：" +e.Message;
                    Logger.Error(Response.Msg);
                }
            }
            return Response;
        }

        /// <summary>
        /// 获取当前时间段此通道的占位任务id
        /// </summary>
        /// <remarks>
        /// 例子:
        ///
        /// </remarks>
        /// <param name="channelid">通道id，</param>
        /// <returns>任务id</returns>
        [HttpGet("tieupid/{channelid}")]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<int>> TieUpTaskIDByChannelID([FromRoute, BindRequired]int channelid)
        {
            var Response = new ResponseMessage<int>();
            if (channelid <= 0)
            {
                Response.Code = ResponseCodeDefines.ModelStateInvalid;
                Response.Msg = "请求参数不正确";
            }

            try
            {
                Response.Ext = await _taskManage.GetTieUpTaskIDByChannelId(channelid);
                if (Response.Ext <= 0)
                {
                    Response.Code = ResponseCodeDefines.NotFound;
                    Response.Msg = "not found task";
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
                    Response.Msg = "TaskIDByTaskGUID error info：" +e.Message;
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
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<int>> StopTask([FromRoute, BindRequired]int taskid)
        {
            var Response = new ResponseMessage<int>();
            if (taskid <= 0)
            {
                Response.Code = ResponseCodeDefines.ModelStateInvalid;
                Response.Msg = "请求参数不正确";
            }

            try
            {
                Response.Ext = await _taskManage.StopTask(taskid, DateTime.MinValue);
                if (Response.Ext <= 0)
                {
                    Response.Code = ResponseCodeDefines.NotFound;
                    Response.Msg = "not found task";
                }
                
                if (_globalInterface != null)
                {
                    GlobalInternals re = new GlobalInternals() { Funtype = IngestDBCore.GlobalInternals.FunctionType.SetGlobalState, State = GlobalStateName.MODTASK };
                    var response1 = await _globalInterface.Value.SubmitGlobalCallBack(re);
                    if (response1.Code != ResponseCodeDefines.SuccessCode)
                    {
                        Logger.Error("SetGlobalState modtask error");
                    }

                    await Task.Run(() => { _clock.InvokeNotify(GlobalStateName.MODTASK, NotifyPlugin.Kafka, NotifyAction.STOPTASK, Response.Ext); });
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
                    Response.Msg = "StopTask error info：" +e.Message;
                    Logger.Error(Response.Msg);
                }
            }
            return Response;
        }

        /// <summary>
        /// 根据任务id,在指定的时间上停止任务
        /// </summary>
        /// <remarks>
        /// 例子:
        ///
        /// </remarks>
        /// <param name="taskid">任务id，</param>
        /// <param name="endtime">指定的时间 yyyy/MM/dd HH:mm:ss yyyy-MM-dd HH:mm:ss</param>
        /// <returns>任务id</returns>
        [HttpPut("stoptime/{taskid}")]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<int>> StopTask([FromRoute, BindRequired]int taskid, [FromQuery, BindRequired]string endtime)
        {
            var Response = new ResponseMessage<int>();
            if (taskid <= 0)
            {
                Response.Code = ResponseCodeDefines.ModelStateInvalid;
                Response.Msg = "请求参数不正确";
            }

            try
            {
                if (string.IsNullOrEmpty(endtime))
                {
                    Response.Ext = await _taskManage.StopTask(taskid, DateTime.Now);
                }
                else
                    Response.Ext = await _taskManage.StopTask(taskid, DateTimeFormat.DateTimeFromString(endtime));

                if (Response.Ext <= 0)
                {
                    Response.Code = ResponseCodeDefines.NotFound;
                    Response.Msg = "not found task";
                }
                
                if (_globalInterface != null)
                {
                    GlobalInternals re = new GlobalInternals() { Funtype = IngestDBCore.GlobalInternals.FunctionType.SetGlobalState, State = GlobalStateName.MODTASK };
                    var response1 = await _globalInterface.Value.SubmitGlobalCallBack(re);
                    if (response1.Code != ResponseCodeDefines.SuccessCode)
                    {
                        Logger.Error("SetGlobalState modtask error");
                    }

                    await Task.Run(() => { _clock.InvokeNotify(GlobalStateName.MODTASK, NotifyPlugin.Kafka, NotifyAction.STOPTASK, Response.Ext); });
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
                    Response.Msg = "StopTask error info：" +e.Message;
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
        [HttpPut("state/{taskid}")]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<int>> SetTaskState([FromRoute, BindRequired]int taskid, [FromQuery, BindRequired]int state)
        {
            var Response = new ResponseMessage<int>();
            if (taskid <= 0)
            {
                Response.Code = ResponseCodeDefines.ModelStateInvalid;
                Response.Msg = "请求参数不正确";
            }

            try
            {
                Response.Ext = await _taskManage.SetTaskState(taskid, state);
                if (Response.Ext <= 0)
                {
                    Response.Code = ResponseCodeDefines.NotFound;
                    Response.Msg = "not found task";
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
                    Response.Msg = "StopTask error info：" +e.Message;
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
        /// <param name="unitid">随便填</param>
        /// <param name="day">查询时间yyyy/MM/dd HH:mm:ss </param>
        /// <param name="timemode">查询模式0是24小时模式，1是32小时模式</param>
        /// <returns>任务基础元数据</returns>
        [HttpGet("onedaytask")]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<List<TaskContentResponse>>> QueryTaskContent([FromQuery, BindRequired]int unitid, [FromQuery, BindRequired]string day, [FromQuery, BindRequired]int timemode)
        {
            var Response = new ResponseMessage<List<TaskContentResponse>>();

            try
            {
                Response.Ext = await _taskManage.QueryTaskContent<TaskContentResponse>(unitid, DateTimeFormat.DateTimeFromString(day), (TimeLineType)timemode);
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
                    Response.Msg = "QueryTaskContent error info：" +e.Message;
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
        [HttpGet("tasksource/{taskid}")]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<TaskSource>> GetTaskSourceById([FromRoute, BindRequired]int taskid)
        {
            var Response = new ResponseMessage<TaskSource>();

            try
            {
                Response.Ext = await _taskManage.GetTaskSource(taskid);
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
                    Response.Msg = "GetTaskSource error info：" +e.Message;
                    Logger.Error(Response.Msg);
                }
            }
            return Response;
        }

        /// <summary>
        /// 获取任务实际开始时间
        /// </summary>
        /// <remarks>
        /// 例子:
        ///
        /// </remarks>
        /// <param name="taskid">查询任务id</param>
        /// <param name="starttime">任务开始时间 yyyy/MM/dd HH:mm:ss yyyy-MM-dd HH:mm:ss</param>
        /// <returns>任务id</returns>
        [HttpPut("trimin/{taskid}")]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<int>> TrimTaskBeginTime([FromRoute, BindRequired]int taskid, [FromQuery, BindRequired]string starttime)
        {
            var Response = new ResponseMessage<int>();

            try
            {
                Response.Ext = await _taskManage.TrimTaskBeginTime(taskid, starttime);
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
                    Response.Msg = "TrimTaskBeginTime error info：" +e.Message;
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
        [HttpDelete("delete/{taskid}")]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<int>> DeleteTask([FromRoute, BindRequired]int taskid)
        {
            var Response = new ResponseMessage<int>();

            try
            {
                Response.Ext = await _taskManage.DeleteTask(taskid);

                await Task.Run(() => { _clock.InvokeNotify(GlobalStateName.DELTASK, NotifyPlugin.Kafka, NotifyAction.DELETETASK, taskid); });
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
                    Response.Msg = "DeleteTask error info：" +e.Message;
                    Logger.Error(Response.Msg);
                }
            }
            return Response;
        }

        /// <summary>
        /// 设置任务周期信息
        /// </summary>
        /// <remarks>
        /// 例子:
        ///
        /// </remarks>
        /// <param name="taskid">任务id</param>
        /// <param name="classify">周期信息</param>
        /// <returns>任务id</returns>
        [HttpPut("classify/{taskid}")]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<int>> SetTaskInfoClassify([FromRoute, BindRequired]int taskid, [FromQuery, BindRequired]string classify)
        {
            var Response = new ResponseMessage<int>();

            try
            {
                Response.Ext = await _taskManage.SetTaskClassify(taskid, classify);
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
                    Response.Msg = "SetTaskInfoClassify error info：" +e.Message;
                    Logger.Error(Response.Msg);
                }
            }
            return Response;
        }

        /// <summary>
        /// //"Set all period task which were dispatched in time to the next excuting time")]
        /// </summary>
        /// <remarks>
        /// 例子:
        ///
        /// </remarks>
        /// <returns></returns>
        [HttpPost("periodic/nexttime")]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<bool>> SetPeriodTaskInfoToNextTime()
        {
            var Response = new ResponseMessage<bool>();

            try
            {
                Response.Ext = await _taskManage.SetPeriodTaskToNextTime();
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
                    Response.Msg = "SetPeriodTaskToNextTime error info：" +e.Message;
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
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<List<TaskContentResponse>>> GetNeedSyncTasks()
        {
            var Response = new ResponseMessage<List<TaskContentResponse>>();

            try
            {
                await _taskManage.UpdateComingTasks();

                Response.Ext = await _taskManage.GetNeedSynTasksNew<TaskContentResponse>();
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
                    Response.Msg = "SetPeriodTaskToNextTime error info：" +e.Message;
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
        [HttpGet("completesyn")]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage> CompleteSynTasks([FromBody, BindRequired]CompleteSyncTaskRequest req)
        {
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
                    Response.Msg = "CompleteSynTasks error info：" +e.Message;
                    Logger.Error(Response.Msg);
                }
            }
            return Response;
        }

        /// <summary>
        /// 设置任务的分裂信息
        /// </summary>
        /// <remarks>
        /// 例子:
        ///
        /// </remarks>
        /// <param name="taskid">任务id</param>
        /// <param name="oldlen">前面一段的分裂长度</param>
        /// <param name="oldclipnum">前面一段采了多少段</param>
        /// <param name="newname">新分裂的素材名</param>
        /// <param name="newguid">新分裂的素材guid</param>
        /// <param name="index">新分裂的段号</param>
        /// <returns>任务guid</returns>
        [HttpPost("splittaskwithclip/{taskid}")]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<string>> SplitClip(
            [FromRoute, BindRequired]int taskid,
            [FromQuery, BindRequired]long oldlen,
            [FromQuery, BindRequired]int oldclipnum,
            [FromQuery, BindRequired]string newname,
            [FromQuery, BindRequired]string newguid,
            [FromQuery, BindRequired]int index)
        {
            var Response = new ResponseMessage<string>();

            try
            {
                Response.Ext = await _taskManage.Update24HoursTask(taskid, oldlen, oldclipnum, newname, newguid, index);
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
                    Response.Msg = "CompleteSynTasks error info：" +e.Message;
                    Logger.Error(Response.Msg);
                }
            }
            return Response;
        }

        /// <summary>
        /// 查询需要重调度的任务,执行完后，需要对该任务进行解锁
        /// </summary>
        /// <remarks>
        /// 例子:
        ///
        /// </remarks>
        /// <returns></returns>
        [HttpGet("needrescheduletasks")]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<List<TaskContentResponse>>> NeedRescheduleTasks()
        {
            var Response = new ResponseMessage<List<TaskContentResponse>>();

            try
            {
                Response.Ext = await _taskManage.GetScheduleFailedTasks<TaskContentResponse>();
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
                    Response.Msg = "NeedRescheduleTasks error info：" +e.Message;
                    Logger.Error(Response.Msg);
                }
            }
            return Response;
        }

        /// <summary>
        /// 完成重调度，解锁"
        /// </summary>
        /// <remarks>
        /// 例子:
        ///
        /// </remarks>
        /// <param name="req">请求体</param>
        /// <returns></returns>
        [HttpGet("completereschedule")]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<bool>> CompleteRescheduleTasks([FromBody, BindRequired]TaskContentRequest req)
        {
            var Response = new ResponseMessage<bool>();

            try
            {
                Response.Ext = await _taskManage.CompleteRescheduleTasks<TaskContentRequest>(req);
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
                    Response.Msg = "CompleteRescheduleTasks error info：" +e.Message;
                    Logger.Error(Response.Msg);
                }
            }
            return Response;
        }

        /// <summary>
        /// 修改CooperTask
        /// </summary>
        /// <remarks>
        /// 例子:
        ///
        /// </remarks>
        /// <param name="taskid">任务id</param>
        /// <param name="cooptype">要设置的类型 枚举</param>
        /// <returns></returns>
        [HttpPut("coopertype/{taskid}")]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<bool>> ModifyCooperTask([FromRoute, BindRequired]int taskid, [FromQuery, BindRequired]int cooptype)
        {
            var Response = new ResponseMessage<bool>();

            try
            {
                Response.Ext = await _taskManage.SetTaskCooperType(taskid, (CooperantType)cooptype);
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
                    Response.Msg = "ModifyCooperTask error info：" +e.Message;
                    Logger.Error(Response.Msg);
                }
            }
            return Response;
        }

        /// <summary>
        /// 重新调度任务, 所有失败的任务
        /// </summary>
        /// <remarks>
        /// 例子:
        ///
        /// </remarks>
        /// <returns>所有重新调度的任务列表</returns>
        [HttpGet("rescheduletask")]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<List<RescheduledTaskInfoResponse>>> RescheduleTasks()
        {
            var Response = new ResponseMessage<List<RescheduledTaskInfoResponse>>();

            try
            {
                Response.Ext = await _taskManage.RescheduleTasks<RescheduledTaskInfoResponse>();
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
                    Response.Msg = "RescheduleTasks error info：" +e.Message;
                    Logger.Error(Response.Msg);
                }
            }
            return Response;
        }

        /// <summary>
        /// 获取warringinfos
        /// </summary>
        /// <remarks>
        /// 例子:
        ///
        /// </remarks>
        /// <param name="channelid">通道id</param>
        /// <param name="channelAlive"></param>
        /// <returns></returns>
        [HttpGet("warninginfos")]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<List<WarningInfoResponse>>> WarningInfos([FromQuery, BindRequired]int channelid, [FromQuery, BindRequired]int channelAlive)
        {
            var Response = new ResponseMessage<List<WarningInfoResponse>>();

            try
            {
                if (channelAlive == 1)
                    Response.Ext = await _taskManage.GetAutoManuConflict<WarningInfoResponse>(channelid);
                else
                    Response.Ext = await _taskManage.GetBadChannelTask<WarningInfoResponse>(channelid);
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
                    Response.Msg = "WarningInfos error info：" +e.Message;
                    Logger.Error(Response.Msg);
                }
            }
            return Response;
        }

        /// <summary>
        /// 查询正在采集任务的低质量素材文件名
        /// </summary>
        /// <remarks>
        /// 例子:
        ///
        /// </remarks>
        /// <param name="channelid">通道id</param>
        /// <returns>素材路径名</returns>
        [HttpGet("channelcapturinglowmaterial")]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<string>> ChannelCapturingLowMaterial([FromQuery, BindRequired]int channelid)
        {
            var Response = new ResponseMessage<string>();

            try
            {
                Response.Ext = await _taskManage.GetChannelCapturingLowMaterial(channelid, _globalInterface.Value);
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
                    Response.Msg = "ChannelCapturingLowMaterial error info：" +e.Message;
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
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<TaskContentResponse>> SplitTaskInfo([FromRoute, BindRequired]int taskid, [FromQuery, BindRequired]string newguid, [FromQuery, BindRequired]string newname)
        {
            var Response = new ResponseMessage<TaskContentResponse>();

            try
            {
                Response.Ext = await _taskManage.SplitTask(taskid, newguid, newname);
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
                    Response.Msg = "ChannelCapturingLowMaterial error info：" +e.Message;
                    Logger.Error(Response.Msg);
                }
            }
            return Response;
        }



        /// <summary>
        /// 获取当前ready任务
        /// </summary>
        /// <remarks>
        /// Decivce通讯接口
        /// </remarks>
        /// <returns>当前任务</returns>
        [HttpPost("currenttasks")]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<List<TaskContentResponse>>> GetCurrentTasks()
        {
            var Response = new ResponseMessage<List<TaskContentResponse>>();

            try
            {
                Response.Ext = await _taskManage.GetCurrentTasksAsync<TaskContentResponse>();
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
                    Response.Msg = $"SetTaskInfoClassify error info：{e.Message}";
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
        [HttpPost("periodic/createtask/{taskid}")]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<TaskContentResponse>> CreatePeriodicTask([FromRoute, BindRequired]int taskid)
        {
            var Response = new ResponseMessage<TaskContentResponse>();

            try
            {
                Response.Ext = await _taskManage.CreateNewTaskFromPeriodicTask<TaskContentResponse>(taskid);

                var custom = await _taskManage.GetCustomMetadataAsync<TaskCustomMetadataResponse>(taskid);
                if (custom != null)
                {
                    await _taskManage.UpdateCustomMetadataAsync(Response.Ext.TaskID, custom.Metadata);
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
                    Response.Msg = $"CreateNewTaskFromPeriodicTask error info：{e.Message}";
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
        [HttpPost("starttieuptask/{taskid}")]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<bool>> StartTieUpTask([FromRoute, BindRequired]int taskid)
        {
            var Response = new ResponseMessage<bool>();

            try
            {
                Response.Ext = await _taskManage.StartTieupTask(taskid);
                
                if (_globalInterface != null)
                {
                    GlobalInternals re = new GlobalInternals() { Funtype = IngestDBCore.GlobalInternals.FunctionType.SetGlobalState, State = GlobalStateName.MODTASK };
                    var response1 = await _globalInterface.Value.SubmitGlobalCallBack(re);
                    if (response1.Code != ResponseCodeDefines.SuccessCode)
                    {
                        Logger.Error("SetGlobalState modtask error");
                    }

                    await Task.Run(() => { _clock.InvokeNotify(GlobalStateName.MODTASK, NotifyPlugin.Kafka, NotifyAction.MODIFYTASK, taskid); });
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
                    Response.Msg = $"StartTieUpTask error info：{e.Message}";
                    Logger.Error(Response.Msg);
                }
            }
            return Response;
        }

        /// <summary>
        /// 从传入的通道列表中选出可用的通道
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <param name="lstchannelid">占位任务id</param>
        /// <param name="begin">开始时间</param>
        /// <param name="end">结束时间</param>
        /// <returns>选择后的通道id</returns>
        [HttpPost("chooseuseablechannel")]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<int>> ChooseUseableChannelID([FromQuery, BindRequired]List<int> lstchannelid, [FromQuery, BindRequired]string begin, [FromQuery, BindRequired]string end)
        {
            var Response = new ResponseMessage<int>();

            try
            {
                Response.Ext = await _taskManage.ChooseUsealbeChannel(lstchannelid, DateTimeFormat.DateTimeFromString(begin), DateTimeFormat.DateTimeFromString(end));
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
                    Response.Msg = $"ChooseUseableChannelID error info：{e.Message}";
                    Logger.Error(Response.Msg);
                }
            }
            return Response;
        }

        /// <summary>
        /// 修改任务名
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <param name="taskid">任务id</param>
        /// <param name="taskname">任务名字</param>
        /// <returns>无返回</returns>
        [HttpPut("name/{taskid}")]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage> ModifyTaskInfoName([FromRoute, BindRequired]int taskid, [FromQuery, BindRequired]string taskname)
        {
            var Response = new ResponseMessage();

            try
            {
                await _taskManage.ModifyTaskName(taskid, taskname);
                
                if (_globalInterface != null)
                {
                    GlobalInternals re = new GlobalInternals() { Funtype = IngestDBCore.GlobalInternals.FunctionType.SetGlobalState, State = GlobalStateName.MODTASK };
                    var response1 = await _globalInterface.Value.SubmitGlobalCallBack(re);
                    if (response1.Code != ResponseCodeDefines.SuccessCode)
                    {
                        Logger.Error("SetGlobalState modtask error");
                    }

                    await Task.Run(() => { _clock.InvokeNotify(GlobalStateName.MODTASK, NotifyPlugin.Kafka, NotifyAction.MODIFYTASK, taskid); });
                }
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
                    Response.Msg = $"ModifyTaskInfoNmae error info：{e.Message}";
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
        [HttpPost("periodic/{taskid}")]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<int>> ModifyPeriodTaskInfo([FromQuery, BindRequired]int isall, [FromBody, BindRequired]TaskContentRequest req)
        {
            var Response = new ResponseMessage<int>();

            try
            {
                Response.Ext = await _taskManage.ModifyPeriodTask<TaskContentRequest>(req, isall == 1 ? true : false);

                //添加后如果开始时间在2分钟以内，需要调度一次
                if ((DateTimeFormat.DateTimeFromString(req.Begin) - DateTime.Now).TotalSeconds < 120)
                    await _taskManage.UpdateComingTasks();
                
                if (_globalInterface != null)
                {
                    GlobalInternals re = new GlobalInternals() { Funtype = IngestDBCore.GlobalInternals.FunctionType.SetGlobalState, State = GlobalStateName.MODTASK };
                    var response1 = await _globalInterface.Value.SubmitGlobalCallBack(re);
                    if (response1.Code != ResponseCodeDefines.SuccessCode)
                    {
                        Logger.Error("SetGlobalState modtask error");
                    }

                    await Task.Run(() => { _clock.InvokeNotify(GlobalStateName.MODTASK, NotifyPlugin.Kafka, NotifyAction.MODIFYPERIODCTASK, Response.Ext); });
                }
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
                    Response.Msg = $"ModifyTaskInfoNmae error info：{e.Message}";
                    Logger.Error(Response.Msg);
                }
            }
            return Response;
        }

        /// <summary>
        /// 检测VTR是否冲突
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <param name="taskid"></param>
        /// <param name="begintime"></param>
        /// <param name="endtime"></param>
        /// <param name="vtrid"></param>
        /// <returns>冲突信息</returns>
        [HttpGet("isvtrcollide/{taskid}")]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<IsVtrCollide>> IsTaskVTRCollide(
            [FromRoute, BindRequired]int taskid,
            [FromQuery, BindRequired]string begintime,
            [FromQuery, BindRequired]string endtime,
            [FromQuery, BindRequired]int vtrid)
        {
            var Response = new ResponseMessage<IsVtrCollide>();

            try
            {
                if (string.IsNullOrEmpty(begintime))
                {
                    Response.Code = ResponseCodeDefines.ArgumentNullError;
                    Response.Msg = $"检测VTR是否冲突过程中，任务开始时间不合法";
                    return Response;
                }
                if (string.IsNullOrEmpty(endtime))
                {
                    Response.Code = ResponseCodeDefines.ArgumentNullError;
                    Response.Msg = $"检测VTR是否冲突过程中，任务结束时间不合法";
                    return Response;
                }

                if (vtrid <= 0)
                {
                    Response.Code = ResponseCodeDefines.ArgumentNullError;
                    Response.Msg = $"检测VTR是否冲突过程中，VTR的ID不合法";
                    return Response;
                }
                if (taskid < -1)
                {
                    Response.Code = ResponseCodeDefines.ArgumentNullError;
                    Response.Msg = $"检测VTR是否冲突过程中，任务的ID不合法";
                    return Response;
                }

                Response.Ext.Result = VTRCollideResult.emVTRNotDefine;
                Response.Ext.CollideTaskContent = await _taskManage.IsVTRCollide<TaskContentResponse>(vtrid, begintime, endtime, taskid);
                if (Response.Ext.CollideTaskContent != null)
                {
                    Response.Ext.Result = VTRCollideResult.emHaveVTRCollide;
                }
                else
                    Response.Ext.Result = VTRCollideResult.emNotVTRCollide;
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
                    Response.Msg = $"IsTaskVTRCollide error info：{e.Message}";
                    Logger.Error(Response.Msg);
                }
            }
            return Response;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <param name="req">请求体</param>
        /// <returns>分裂后的任务</returns>
        [HttpPost("writevtruploadtask")]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage> WriteVTRUploadTask([FromBody, BindRequired]TaskContentRequest req)
        {
            var Response = new ResponseMessage();

            try
            {
                await _taskManage.WriteVTRUploadTaskDB<TaskContentRequest>(req);
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
                    Response.Msg = $"WriteVTRUploadTask error info：{e.Message}";
                    Logger.Error(Response.Msg);
                }
            }
            return Response;
        }

        /// <summary>
        /// 获得失败的KAMATAKI任务
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <returns>extention为TaskContent数组</returns>
        [HttpGet("kamakatitasks")]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<List<TaskContentResponse>>> GetAllKamakatiFailTasks()
        {
            var Response = new ResponseMessage<List<TaskContentResponse>>();

            try
            {
                Response.Ext = await _taskManage.GetKamakatiFailTasks<TaskContentResponse>();
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
                    Response.Msg = $"GetAllKamakatiFailTasks error info：{e.Message}";
                    Logger.Error(Response.Msg);
                }
            }
            return Response;
        }

        /// <summary>
        /// 修改任务的缩略图
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <param name="taskid">任务id</param>
        /// <param name="bmppath">图片路径</param>
        /// <returns></returns>
        [HttpPost("stampbmp/{taskid}")]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage> SetTaskInfoStampBmp([FromRoute, BindRequired]int taskid, [FromBody, BindRequired]string bmppath)
        {
            var Response = new ResponseMessage();

            try
            {
                bmppath = bmppath.Replace("\\", "\\\\");
                bmppath = bmppath.Replace("'", "''");
                await _taskManage.SetTaskBmp(taskid, bmppath);
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
                    Response.Msg = $"SetTaskStampBmp error info：{e.Message}";
                    Logger.Error(Response.Msg);
                }
            }
            return Response;
        }

        /// <summary>
        /// 根据老的任务，自动增加一个接口
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <param name="oldtaskid">任务id</param>
        /// <param name="starttime">老任务结束时间</param>
        /// <returns></returns>
        [HttpPost("withoutpolicytask/addbyoldtask/{oldtaskid}")]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<TaskContentResponse>> AddTaskByOld([FromRoute, BindRequired]int oldtaskid, [FromQuery, BindRequired]string starttime)
        {
            var Response = new ResponseMessage<TaskContentResponse>();

            try
            {
                Response.Ext = await _taskManage.AutoAddTaskByOldTask(oldtaskid, DateTimeFormat.DateTimeFromString(starttime), _globalInterface.Value);
                
                if (_globalInterface != null)
                {
                    GlobalInternals re = new GlobalInternals() { Funtype = IngestDBCore.GlobalInternals.FunctionType.SetGlobalState, State = GlobalStateName.ADDTASK };
                    var response1 = await _globalInterface.Value.SubmitGlobalCallBack(re);
                    if (response1.Code != ResponseCodeDefines.SuccessCode)
                    {
                        Logger.Error("SetGlobalState modtask error");
                    }

                    await Task.Run(() => { _clock.InvokeNotify(GlobalStateName.MODTASK, NotifyPlugin.Kafka, NotifyAction.MODIFYTASK, oldtaskid); });
                    await Task.Run(() => { _clock.InvokeNotify(GlobalStateName.ADDTASK, NotifyPlugin.Kafka, NotifyAction.ADDTASK, Response.Ext.TaskID); });
                }
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
                    Response.Msg = $"AddTaskByOld error info：{e.Message}";
                    Logger.Error(Response.Msg);
                }
            }
            return Response;
        }

        /// <summary>
        /// Device调用接口，获取将要和正在执行的任务
        /// </summary>
        /// <returns>将要和正在执行的任务</returns>
        [HttpGet("willbeginandcapturing")]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<List<TaskContentResponse>>> GetWillBeginAndCapturingTasks()
        {
            var Response = new ResponseMessage<List<TaskContentResponse>>();
            try
            {
                Response.Ext = await _taskManage.GetWillBeginAndCapturingTasksAsync<TaskContentResponse>();
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
                    Response.Msg = $"GetWillBeginAndCapturingTasks error info：{e.Message}";
                    Logger.Error(Response.Msg);
                }
            }
            return Response;
        }


        //[HttpPost("withoutpolicytask/addrescheduletask/{oldtaskid}")]
        //[ApiExplorerSettings(GroupName = "v2")]
        //public async Task<ResponseMessage<TaskContentResponse>> AddReScheduleTask([FromRoute, BindRequired]int oldtaskid, [FromQuery, BindRequired]string starttime)
        //{
        //    var Response = new ResponseMessage<TaskContentResponse>();

        //    try
        //    {
        //        Response.Ext = await _taskManage.AutoAddTaskByOldTask(oldtaskid, DateTimeFormat.DateTimeFromString(starttime));

        //        var _globalinterface = ApplicationContext.Current.ServiceProvider.GetRequiredService<IIngestGlobalInterface>();
        //        if (_globalinterface != null)
        //        {
        //            GlobalInternals re = new GlobalInternals() { funtype = IngestDBCore.GlobalInternals.FunctionType.SetGlobalState, State = GlobalStateName.ADDTASK };
        //            var response1 = await _globalinterface.SubmitGlobalCallBack(re);
        //            if (response1.Code != ResponseCodeDefines.SuccessCode)
        //            {
        //                Logger.Error("SetGlobalState modtask error");
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        if (e is SobeyRecException se)//sobeyexcep会自动打印错误
        //        {
        //            Response.Code = se.ErrorCode.ToString();
        //            Response.Msg = se.Message;
        //        }
        //        else
        //        {
        //            Response.Code = ResponseCodeDefines.ServiceError;
        //            Response.Msg = $"AddTaskByOld error info：{e.Message}";
        //            Logger.Error(Response.Msg);
        //        }
        //    }
        //    return Response;
        //}
        //////////////////////////
    }
}
