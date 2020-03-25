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
    [Route("api/v0/task")]
    public class OldTaskController : ControllerBase
    {
        private readonly ILogger Logger = LoggerManager.GetLogger("TaskInfo");
        private readonly TaskManager _monthManage;
        private readonly RestClient _restClient;
    }
}
