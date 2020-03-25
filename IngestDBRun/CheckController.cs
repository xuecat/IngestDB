using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IngestDBRun
{
    [Route("check")]
    public class CheckController : Controller
    {
        // GET api/values
        [HttpGet("get")]
        [HttpHead]
        public string Get()
        {
            return "OK";
        }


    }
}
