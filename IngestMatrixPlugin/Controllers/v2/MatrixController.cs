using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using IngestMatrixPlugin.Managers;
using Microsoft.AspNetCore.Mvc;
using Sobey.Core.Log;

namespace IngestMatrixPlugin.Controllers
{
    public partial class MatrixController : ControllerBase
    {
        private readonly ILogger Logger = LoggerManager.GetLogger("DeviceInfo");
        private readonly MatrixManager _matrixManage;
        private static object lockObj = new object();
        public static Mutex m_Matrixmt = new Mutex();
        //private readonly RestClient _restClient;


        public MatrixController(MatrixManager task)
        {
            _matrixManage = task;
        }


        /// <summary>
        /// 心跳监听接口 /get/
        /// </summary>
        /// <returns></returns>
        [HttpGet("Get")]
        [ApiExplorerSettings(GroupName = "v2")]
        public string Get()
        {
            return "DBPlatform Service is already startup at " + DateTime.Now.ToString();
        }
    }
}
