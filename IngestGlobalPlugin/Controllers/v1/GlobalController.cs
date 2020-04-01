using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IngestGlobalPlugin.Controllers
{
    public partial class GlobalController : ControllerBase
    {
        [HttpGet("GetQueryTaskMetaData"), MapToApiVersion("1.0")]
        public async Task SetGlobalState1([FromQuery]int nTaskID)
        {
            


        }

    }
}
