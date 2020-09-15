using IngestDBCore.Basic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace IngestDevicePlugin.Controllers.h
{
    [ApiController]
    public class DeviceController : ControllerBase
    {
        /// <summary>
        /// 监听接口 /get/
        /// </summary>
        /// <returns></returns>
        [IngestIgnoreFilter]
        [HttpGet("api/device/get")]
        public string Get()
        {

            return "DBPlatform Service is already startup at " + DateTime.Now.ToString();
        }
    }
}
