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
using TaskMaterialMetaRequest = IngestTaskPlugin.Dto.TaskMaterialMetaResponse;
using TaskContentMetaRequest = IngestTaskPlugin.Dto.TaskContentMetaResponse;
using TaskPlanningRequest = IngestTaskPlugin.Dto.TaskPlanningResponse;
using TaskSplitRequest = IngestTaskPlugin.Dto.TaskSplitResponse;
using PropertyRequest = IngestTaskPlugin.Dto.PropertyResponse;
using TaskCustomMetadataRequest = IngestTaskPlugin.Dto.TaskCustomMetadataResponse;
using Microsoft.AspNetCore.Mvc.ModelBinding;

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

        public TaskController(RestClient rsc, TaskManager task)
        {
            _taskManage = task;
            _restClient = rsc;
        }

        /// <summary>
        /// 使用路由 /taskmaterialmetadata/{taskid}
        /// </summary>
        /// <remarks>
        /// 例子:
        /// Get api/v2/task/taskmaterialmetadata/1
        /// </remarks>
        /// <returns>素材任务元数据结构体</returns>     
        [HttpGet("taskmaterialmetadata/{taskid}")]
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
                    Response.Msg = "error info：" + e.ToString();
                    Logger.Error(Response.Msg);
                }
            }
            return Response;
        }

        /// <summary>
        /// 使用路由 /taskcontentmetadata/{taskid}
        /// </summary>
        /// <remarks>
        /// 例子:
        /// Get api/v2/task/taskcontentmetadata/1
        /// </remarks>
        /// <returns>任务元数据结构体</returns>     
        [HttpGet("taskcontentmetadata/{taskid}")]
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
                    Response.Msg = "error info：" + e.ToString();
                    Logger.Error(Response.Msg);
                }
            }
            return Response;
        }

        /// <summary>
        /// 使用路由 /taskplanningmetadata/{taskid}
        /// </summary>
        /// <remarks>
        /// 例子:
        /// Get api/v2/task/taskplanningmetadata/1
        /// </remarks>
        /// <returns>任务计划元数据结构体</returns>     
        [HttpGet("taskplanningmetadata/{taskid}")]
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
                    Response.Msg = "error info：" + e.ToString();
                    Logger.Error(Response.Msg);
                }
            }
            return Response;
        }

        /// <summary>
        /// 使用路由 /taskmetadata/{taskid}
        /// </summary>
        /// <remarks>
        /// 假如没有此属性会新加，有此属性会更新
        /// 例子:
        /// Get api/v2/task/taskmetadata/1
        /// </remarks>
        /// <param name="taskid"></param>
        /// <param name="tasktype">元数据类型</param>
        /// <param name="lst">键值对应需要更新的数据，proterty和value</param>
        /// <returns>任务计划元数据结构体</returns>     
        [HttpPost("taskmetadata/{taskid}")]
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
                    Response.Msg = "error info：" + e.ToString();
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
        /// Get api/v2/task/taskcustommetadata/1
        /// </remarks>
        /// <returns>获取任务自定义数据</returns>     
        [HttpGet("taskcustommetadata/{taskid}")]
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
                    Response.Msg = "error info：" + e.ToString();
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
        /// Get api/v2/task/taskcustommetadata/1
        /// </remarks>
        /// <param name="taskid">任务id</param>
        /// <param name="data">更新数据，taskid填不填看你，我不会用</param>
        /// <returns>获取任务自定义数据</returns>     
        [HttpPost("taskcustommetadata/{taskid}")]
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
                    Response.Msg = "error info：" + e.ToString();
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
        /// Get api/v2/task/grouptask/stop/1
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
                    GlobalInternals re = new GlobalInternals() { funtype = FunctionType.SetGlobalState, State = GlobalStateName.MODTASK };
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
                    Response.Msg = "error info：" + e.ToString();
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
        /// Get api/v2/task/grouptask/1
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
                    Response.Msg = "error info：" + e.ToString();
                    Logger.Error(Response.Msg);
                }
            }
            return Response;
        }

        /// <summary>
        /// 添加任务(fow gpi 引入在用, web别用 对应老接口的PostAddTaskSvr)
        /// </summary>
        /// <remarks>
        /// 例子:
        /// Get api/v2/task/withoutpolicytask
        /// </remarks>
        /// <param name="task">添加任务数据</param>
        /// <returns>基本任务信息附带任务id</returns>
        [HttpPost("withoutpolicytask")]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<List<int>>> AddTaskWithoutPolicy([FromBody, BindRequired]int task)
        {
            var Response = new ResponseMessage<List<int>>();
            if (task < 1)
            {
                Response.Code = ResponseCodeDefines.ModelStateInvalid;
                Response.Msg = "请求参数不正确";
            }
            try
            {
                //Response.Ext = await _taskManage.DeleteGroupTaskAsync(taskid);
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
                    Response.Msg = "error info：" + e.ToString();
                    Logger.Error(Response.Msg);
                }
            }
            return Response;
        }
        //////////////////////////
    }
}
