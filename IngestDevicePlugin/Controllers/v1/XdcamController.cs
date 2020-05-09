using IngestDevicePlugin.Dto.Old.Response;
using IngestDevicePlugin.Dto.Response;
using IngestDevicePlugin.Managers;
using Microsoft.AspNetCore.Mvc;
using Sobey.Core.Log;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IngestDevicePlugin.Controllers.v1
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "v1")]
    public class XdcamController :ControllerBase
    {
        private readonly ILogger Logger = LoggerManager.GetLogger("XdcamInfo");
        private readonly DeviceManager _deviceManage;
        private string no_err = "S_OK";

        public XdcamController(DeviceManager deviceManager)
        {
            this._deviceManage = deviceManager;
        }

        /// <summary>
        /// 获取所有蓝光机设备
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetAllXDCAMDevice")]
        public async Task<ResponseMessage<List<XDCAMDeviceInfo>>> GetAllXDCAMDevice()
        {
            ResponseMessage<List<XDCAMDeviceInfo>> res = new ResponseMessage<List<XDCAMDeviceInfo>>();
            res.message = no_err;
            res.extention = null;
            try
            {
                res.extention = await _deviceManage.GetAllXDCAMDeviceAsync<XDCAMDeviceInfo>();
                res.nCode = 1;
            }
            catch (Exception ex)//其他未知的异常，写异常日志
            {
                Logger.Error("Old GetAllXDCAMDevice : " + ex.ToString());
                res.message = ex.Message;
                res.nCode = 0;
            }

            return res;
        }

    }
}
