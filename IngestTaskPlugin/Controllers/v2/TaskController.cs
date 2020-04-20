using IngestDBCore;
using IngestDBCore.Basic;
using IngestDBCore.Interface;
using IngestDBCore.Tool;
using IngestTaskPlugin.Dto;
using IngestTaskPlugin.Managers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Sobey.Core.Log;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;

using TaskMaterialMetaRequest = IngestTaskPlugin.Dto.TaskMaterialMetaResponse;
using TaskContentMetaRequest = IngestTaskPlugin.Dto.TaskContentMetaResponse;
using TaskPlanningRequest = IngestTaskPlugin.Dto.TaskPlanningResponse;
using TaskSplitRequest = IngestTaskPlugin.Dto.TaskSplitResponse;
using PropertyRequest = IngestTaskPlugin.Dto.PropertyResponse;
using TaskInfoRequest = IngestTaskPlugin.Dto.TaskInfoResponse;
using TaskContentRequest = IngestTaskPlugin.Dto.TaskContentResponse;
using TaskCustomMetadataRequest = IngestTaskPlugin.Dto.TaskCustomMetadataResponse;
using AutoMapper;

/// 
///关于taskfullinfo的返回，我这里不标准，需要看看
///
///

namespace IngestTaskPlugin.Controllers
{
    [IngestAuthentication]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    [ApiController]
    public partial class TaskController : ControllerBase
    {
        private readonly ILogger Logger = LoggerManager.GetLogger("TaskInfo");
        private readonly TaskManager _taskManage;
        private readonly RestClient _restClient;
        //private readonly IMapper _mapper;

        public TaskController(RestClient rsc, TaskManager task/*, IMapper mapper*/)
        {
            _taskManage = task;
            _restClient = rsc;
            //_mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// <summary>
        /// 使用路由 /taskinfo/materialmetadata/{taskid}
        /// </summary>
        /// <remarks>
        /// 例子:
        ///     Get api/v2/task/taskmaterialmetadata/1
        /// </remarks>
        /// <returns>素材任务元数据结构体</returns>     
        [HttpGet("taskinfo/materialmetadata/{taskid}")]
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
                    Response.Msg = "GetTaskMaterialMetaData error info：" + e.ToString();
                    Logger.Error(Response.Msg);
                }
            }
            return Response;
        }

        /// <summary>
        /// 使用路由 /taskinfo/contentmetadata/{taskid}
        /// </summary>
        /// <remarks>
        /// 例子:
        ///     Get api/v2/task/taskcontentmetadata/1
        /// </remarks>
        /// <returns>任务元数据结构体</returns>     
        [HttpGet("taskinfo/contentmetadata/{taskid}")]
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
                    Response.Msg = "GetTaskContentMetaData error info：" + e.ToString();
                    Logger.Error(Response.Msg);
                }
            }
            return Response;
        }

        /// <summary>
        /// 使用路由 /taskinfo/planningmetadata/{taskid}
        /// </summary>
        /// <remarks>
        /// 例子:
        ///     Get api/v2/task/taskplanningmetadata/1
        /// </remarks>
        /// <returns>任务计划元数据结构体</returns>     
        [HttpGet("taskinfo/planningmetadata/{taskid}")]
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
                    Response.Msg = "GetTaskPlanningMetaData error info：" + e.ToString();
                    Logger.Error(Response.Msg);
                }
            }
            return Response;
        }

        /// <summary>
        /// 使用路由 /taskinfo/metadata//{taskid}
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
        [HttpPost("taskinfo/metadata/{taskid}")]
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
                    Response.Msg = "UpdateTaskMetaData error info：" + e.ToString();
                    Logger.Error(Response.Msg);
                }
            }
            return Response;
        }

        /// <summary>
        /// 使用路由 /taskcustommetadata/{taskid}
        /// </summary>
        /// <remarks>
        /// 例子:
        ///     Get api/v2/task/taskcustommetadata/1
        /// </remarks>
        /// <returns>获取任务自定义数据</returns>     
        [HttpGet("taskinfo/custommetadata/{taskid}")]
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
                    Response.Msg = "GetTaskCustomMetaData error info：" + e.ToString();
                    Logger.Error(Response.Msg);
                }
            }
            return Response;
        }

        /// <summary>
        /// 使用路由 /taskinfo/custommetadata/{taskid}
        /// </summary>
        /// <remarks>
        /// 例子:
        ///     Get api/v2/task/taskcustommetadata/1
        /// </remarks>
        /// <param name="taskid">任务id</param>
        /// <param name="data">更新数据，taskid填不填看你，我不会用</param>
        /// <returns>获取任务自定义数据</returns>     
        [HttpPost("taskinfo/custommetadata/{taskid}")]
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
                    Response.Msg = "UpdateTaskCustomMetaData error info：" + e.ToString();
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

                var _globalinterface = ApplicationContext.Current.ServiceProvider.GetRequiredService<IIngestGlobalInterface>();
                if (_globalinterface != null)
                {
                    GlobalInternals re = new GlobalInternals() { funtype = IngestDBCore.GlobalInternals.FunctionType.SetGlobalState, State = GlobalStateName.MODTASK };
                    var response1 = await _globalinterface.SubmitGlobalCallBack(re);
                    if (response1.Code != ResponseCodeDefines.SuccessCode)
                    {
                        Logger.Error("SetGlobalState modtask error");
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
                    Response.Msg = "StopGroupTask error info：" + e.ToString();
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
        /// <returns>停止的所有任务id</returns>
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

                var _globalinterface = ApplicationContext.Current.ServiceProvider.GetRequiredService<IIngestGlobalInterface>();
                if (_globalinterface != null)
                {
                    GlobalInternals re = new GlobalInternals() { funtype = IngestDBCore.GlobalInternals.FunctionType.SetGlobalState, State = GlobalStateName.MODTASK };
                    var response1 = await _globalinterface.SubmitGlobalCallBack(re);
                    if (response1.Code != ResponseCodeDefines.SuccessCode)
                    {
                        Logger.Error("SetGlobalState modtask error");
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
                    Response.Msg = "DeleteGroupTask error info：" + e.ToString();
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

                var _globalinterface = ApplicationContext.Current.ServiceProvider.GetRequiredService<IIngestGlobalInterface>();
                if (_globalinterface != null)
                {
                    GlobalInternals re = new GlobalInternals() { funtype = IngestDBCore.GlobalInternals.FunctionType.SetGlobalState, State = GlobalStateName.ADDTASK };
                    var response1 = await _globalinterface.SubmitGlobalCallBack(re);
                    if (response1.Code != ResponseCodeDefines.SuccessCode)
                    {
                        Logger.Error("SetGlobalState modtask error");
                    }
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
                    Response.Msg = "AddTaskWithoutPolicy error info：" + e.ToString();
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

                var _globalinterface = ApplicationContext.Current.ServiceProvider.GetRequiredService<IIngestGlobalInterface>();
                if (_globalinterface != null)
                {
                    GlobalInternals re = new GlobalInternals() { funtype = IngestDBCore.GlobalInternals.FunctionType.SetGlobalState, State = GlobalStateName.ADDTASK };
                    var response1 = await _globalinterface.SubmitGlobalCallBack(re);
                    if (response1.Code != ResponseCodeDefines.SuccessCode)
                    {
                        Logger.Error("SetGlobalState modtask error");
                    }
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
                    Response.Msg = "AddTaskWithoutPolicy error info：" + e.ToString();
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
        [HttpGet("taskinfo/id")]
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
                    Response.Msg = "TaskIDByTaskGUID error info：" + e.ToString();
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
                    Response.Msg = "GetAllChannelCapturingTaskInfo error info：" + e.ToString();
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
        /// <returns>任务id</returns>
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
                    Response.Msg = "TaskIDByTaskGUID error info：" + e.ToString();
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
        /// <returns>任务id</returns>
        [HttpPut("taskinfo/content/{taskid}")]
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

                var _globalinterface = ApplicationContext.Current.ServiceProvider.GetRequiredService<IIngestGlobalInterface>();
                if (_globalinterface != null)
                {
                    GlobalInternals re = new GlobalInternals() { funtype = IngestDBCore.GlobalInternals.FunctionType.SetGlobalState, State = GlobalStateName.MODTASK };
                    var response1 = await _globalinterface.SubmitGlobalCallBack(re);
                    if (response1.Code != ResponseCodeDefines.SuccessCode)
                    {
                        Logger.Error("SetGlobalState modtask error");
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
                    Response.Msg = "TaskIDByTaskGUID error info：" + e.ToString();
                    Logger.Error(Response.Msg);
                }
            }
            return Response;
        }

        /// <summary>
        /// 获取任务内容元数据
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
                    Response.Msg = "TaskIDByTaskGUID error info：" + e.ToString();
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
                    Response.Msg = "TaskIDByTaskGUID error info：" + e.ToString();
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
        [HttpPut("taskinfo/stop/{taskid}")]
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

                var _globalinterface = ApplicationContext.Current.ServiceProvider.GetRequiredService<IIngestGlobalInterface>();
                if (_globalinterface != null)
                {
                    GlobalInternals re = new GlobalInternals() { funtype = IngestDBCore.GlobalInternals.FunctionType.SetGlobalState, State = GlobalStateName.MODTASK };
                    var response1 = await _globalinterface.SubmitGlobalCallBack(re);
                    if (response1.Code != ResponseCodeDefines.SuccessCode)
                    {
                        Logger.Error("SetGlobalState modtask error");
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
                    Response.Msg = "StopTask error info：" + e.ToString();
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
        [HttpPut("taskinfo/stoptime/{taskid}")]
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

                var _globalinterface = ApplicationContext.Current.ServiceProvider.GetRequiredService<IIngestGlobalInterface>();
                if (_globalinterface != null)
                {
                    GlobalInternals re = new GlobalInternals() { funtype = IngestDBCore.GlobalInternals.FunctionType.SetGlobalState, State = GlobalStateName.MODTASK };
                    var response1 = await _globalinterface.SubmitGlobalCallBack(re);
                    if (response1.Code != ResponseCodeDefines.SuccessCode)
                    {
                        Logger.Error("SetGlobalState modtask error");
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
                    Response.Msg = "StopTask error info：" + e.ToString();
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
        [HttpPut("taskinfo/state/{taskid}")]
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
                    Response.Msg = "StopTask error info：" + e.ToString();
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
        /// <param name="day">查询时间</param>
        /// <param name="timemode">查询模式0是24小时模式，1是32小时模式</param>
        /// <returns>任务id</returns>
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
                    Response.Msg = "QueryTaskContent error info：" + e.ToString();
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
        /// <returns>任务id</returns>
        [HttpGet("taskinfo/tasksource/{taskid}")]
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
                    Response.Msg = "GetTaskSource error info：" + e.ToString();
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
        [HttpPut("taskinfo/trimin/{taskid}")]
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
                    Response.Msg = "TrimTaskBeginTime error info：" + e.ToString();
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
        [HttpDelete("taskinfo/delete/{taskid}")]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<int>> DeleteTask([FromRoute, BindRequired]int taskid)
        {
            var Response = new ResponseMessage<int>();

            try
            {
                Response.Ext = await _taskManage.DeleteTask(taskid);
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
                    Response.Msg = "DeleteTask error info：" + e.ToString();
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
        [HttpPut("taskinfo/classify/{taskid}")]
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
                    Response.Msg = "SetTaskInfoClassify error info：" + e.ToString();
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
        [HttpPost("nexttimeperiod")]
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
                    Response.Msg = "SetPeriodTaskToNextTime error info：" + e.ToString();
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
                    Response.Msg = "SetPeriodTaskToNextTime error info：" + e.ToString();
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
                    Response.Msg = "CompleteSynTasks error info：" + e.ToString();
                    Logger.Error(Response.Msg);
                }
            }
            return Response;
        }

        /// <summary>
        /// 分裂素材用
        /// </summary>
        /// <remarks>
        /// 例子:
        ///
        /// </remarks>
        /// <param name="taskid">任务id</param>
        /// <param name="oldlen">任务id</param>
        /// <param name="oldclipnum">任务id</param>
        /// <param name="newname">任务id</param>
        /// <param name="newguid">任务id</param>
        /// <param name="index">任务id</param>
        /// <returns></returns>
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
                    Response.Msg = "CompleteSynTasks error info：" + e.ToString();
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
                    Response.Msg = "NeedRescheduleTasks error info：" + e.ToString();
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
                    Response.Msg = "CompleteRescheduleTasks error info：" + e.ToString();
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
        /// <param name="cooptype">要设置的类型</param>
        /// <returns></returns>
        [HttpPut("taskinfo/coopertype/{taskid}")]
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
                    Response.Msg = "ModifyCooperTask error info：" + e.ToString();
                    Logger.Error(Response.Msg);
                }
            }
            return Response;
        }

        public RescheduleTasks_OUT GetRescheduleTasks()
        {
            RescheduleTasks_OUT p = new RescheduleTasks_OUT();
            p.taskInfoRescheduled = null;//初始化
            p.nValidDataCount = 0;
            p.errStr = no_err;
            try
            {

                TASKSERVICE.RescheduleTasks(out p.taskInfoRescheduled);

                if (p.taskInfoRescheduled == null)
                {
                    p.taskInfoRescheduled = new TaskInfoRescheduled[1];

                    p.nValidDataCount = 0;
                }
                else
                {
                    p.nValidDataCount = p.taskInfoRescheduled.Length;
                }

                BackupTaskAndMaterial();
                p.bRet = true;
            }

            catch (Exception ex)//其他未知的异常，写异常日志
            {
                LoggerService.Error(ex.ToString());
                p.errStr = ex.Message;
                p.bRet = false;
            }
            return p;
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
                    Response.Msg = "WarningInfos error info：" + e.ToString();
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
                Response.Ext = await _taskManage.GetChannelCapturingLowMaterial(channelid);
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
                    Response.Msg = "ChannelCapturingLowMaterial error info：" + e.ToString();
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
        /// <returns>素材路径名</returns>
        [HttpPost("splittask/{taskid}")]
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
                    Response.Msg = "ChannelCapturingLowMaterial error info：" + e.ToString();
                    Logger.Error(Response.Msg);
                }
            }
            return Response;
        }

        //////////////////////////

        /// <summary>
        /// 获取当前任务
        /// </summary>
        /// <remarks>
        /// Decivce通讯接口
        /// </remarks>
        /// <returns>当前任务</returns>
        [HttpPost("taskinfo/CurrentTasks")]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<List<TaskContent>>> GetCurrentTasks()
        {
            var Response = new ResponseMessage<List<TaskContent>>();

            try
            {
                Response.Ext = await _taskManage.GetCurrentTasksAsync<TaskContent>();
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
                    Response.Msg = $"SetTaskInfoClassify error info：{e}";
                    Logger.Error(Response.Msg);
                }
            }
            return Response;
        }
    }
}
