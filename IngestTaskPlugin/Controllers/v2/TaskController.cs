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
        /// 使用路由 /taskmetadata/{taskid}?type=1
        /// </summary>
        /// <param name="type">数据类型</param>
        /// <remarks>
        /// 例子:
        /// Get api/v2/taskmetadata/1?type=2
        /// </remarks>
        /// <returns>任务元数据</returns>     
        [HttpGet("taskmetadata/{taskid}")]
        public async Task<ResponseMessage<TaskMetadataResponse>> GetTaskMetaData([FromRoute]int taskid, [FromQuery]int type)
        {
            var Response = new ResponseMessage<TaskMetadataResponse>();
            if (taskid < 1)
            {
                Response.Code = ResponseCodeDefines.ModelStateInvalid;
                Response.Msg = "请求参数不正确";
            }
            try
            {
                Response.Ext = await _taskManage.GetTaskMetadataAsync<TaskMetadataResponse>(taskid, type);
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
        /// Get api/v2/stopgrouptask/1
        /// </remarks>
        /// <param name="taskid">单个组任务id</param>
        /// <returns>停止的所有任务id</returns>
        [HttpPut("stopgrouptask/{taskid}")]
        public async Task<ResponseMessage<List<int>>> StopGroupTask([FromRoute]int taskid)
        {
            var Response = new ResponseMessage<List<int>>();
            if (taskid < 1)
            {
                Response.Code = ResponseCodeDefines.ModelStateInvalid;
                Response.Msg = "请求参数不正确";
            }
            try
            {
                Response.Ext = await _taskManage.StopGroupTask(taskid);

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
        /// Get api/v2/deletegrouptask/1
        /// </remarks>
        /// <param name="taskid">单个组任务id</param>
        /// <returns>停止的所有任务id</returns>
        [HttpDelete("deletegrouptask/{taskid}")]
        public async Task<ResponseMessage<List<int>>> DeleteGroupTask([FromRoute]int taskid)
        {
            var Response = new ResponseMessage<List<int>>();
            if (taskid < 1)
            {
                Response.Code = ResponseCodeDefines.ModelStateInvalid;
                Response.Msg = "请求参数不正确";
            }
            try
            {
                //Response.Ext = await _taskManage.StopGroupTask(taskid);
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
