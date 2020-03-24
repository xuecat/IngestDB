using IngestDBCore;
using IngestDBCore.Basic;
using IngestDBCore.Tool;
using IngestTaskPlugin.Dto;
using IngestTaskPlugin.Managers;
using Microsoft.AspNetCore.Mvc;
using Sobey.Core.Log;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IngestTaskPlugin.Controllers.v1
{
    [IngestAuthentication]
    [Produces("application/json")]
    [Route("api/v1.1/task")]
    public class TaskController : ControllerBase
    {
        private readonly ILogger Logger = LoggerManager.GetLogger("TaskInfo");
        private readonly NormalTaskManager _taskManage;
        private readonly RestClient _restClient;

        public TaskController(RestClient rsc, NormalTaskManager task)
        {
            _taskManage = task;
            _restClient = rsc;
        }

        /// <summary>
        /// 使用路由 /taskmetadata/{taskid}?type=1
        /// </summary>
        /// <param name="testinfo"></param>
        /// <returns></returns>
        [HttpGet("taskmetadata")]
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
                //var lastmonth = await _monthManage.GetLastMonth();
                //Response.Extension = lastmonth.SettleTime.GetValueOrDefault();
            }
            catch (Exception e)
            {
                Response.Code = ResponseCodeDefines.ServiceError;
                Response.Msg = "error info：" + e.ToString();
                Logger.Error(Response.Msg);
            }
            return Response;
        }
    }
}
