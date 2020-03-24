using IngestDBCore.Basic;
using IngestDBCore.Tool;
using IngestTaskPlugin.Managers;
using Microsoft.AspNetCore.Mvc;
using Sobey.Core.Log;
using System;
using System.Collections.Generic;
using System.Text;

namespace IngestTaskPlugin.Controllers.v0
{
    [IngestAuthentication]
    [Produces("application/json")]
    [Route("api/v1.0/task")]
    public class TaskController : ControllerBase
    {
        private readonly ILogger Logger = LoggerManager.GetLogger("TaskInfo");
        private readonly NormalTaskManager _monthManage;
        private readonly RestClient _restClient;
    }
}
