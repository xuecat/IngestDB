﻿using IngestDBCore;
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

namespace IngestTaskPlugin.Controllers
{
    //[IngestAuthentication]
    //[Produces("application/json")]
    //[ApiVersion("1.0")]
    //[Route("api/v0/task")]
    //[ApiController]
    public partial class TaskController : ControllerBase
    {
        //private readonly ILogger Logger = LoggerManager.GetLogger("TaskInfo");
        //private readonly TaskManager _monthManage;
        //private readonly RestClient _restClient;
        [HttpGet("taskmetadata/{taskid}"), MapToApiVersion("1.0")]
        public async Task<ResponseMessage<TaskMetadataResponse>> OldGetTaskMetaData([FromRoute]int taskid, [FromQuery]int type)
        {
            var Response = new ResponseMessage<TaskMetadataResponse>();
            if (taskid < 1)
            {
                Response.Code = ResponseCodeDefines.ModelStateInvalid;
                Response.Msg = "请求参数不正确";
            }
            try
            {
                Response.Ext = await _taskManage.GetTaskMetadataAsync(taskid, type);
            }
            catch (Exception e)
            {
                if (e.GetType() == typeof(SobeyRecException))//sobeyexcep会自动打印错误
                {
                    SobeyRecException se = (SobeyRecException)e;
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
    }
}
