using IngestDBCore;
using IngestDevicePlugin.Dto.Response;
using IngestDevicePlugin.Managers;
using Microsoft.AspNetCore.Mvc;
using Sobey.Core.Log;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IngestDevicePlugin.Controllers.v2
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("2.0")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "v2")]
    public class XdcamController : ControllerBase
    {
        private readonly ILogger Logger = LoggerManager.GetLogger("XdcamInfoV2");
        private readonly DeviceManager _deviceManage;
        private string no_err = "S_OK";

        public XdcamController(DeviceManager deviceManage)
        {
            this._deviceManage = deviceManage;
        }

        /// <summary>
        /// 获取所有蓝光机设备
        /// </summary>
        /// <remarks>
        /// example:
        /// Get api/v2/xdcam/xdcamdevice/all
        /// </remarks>
        /// <returns>所有xdcam设备信息</returns>
        [HttpGet("xdcamdevice/all")]
        public async Task<ResponseMessage<List<XDCAMDeviceResponse>>> GetAllXDCAMDevice()
        {
            ResponseMessage<List<XDCAMDeviceResponse>> Response = new ResponseMessage<List<XDCAMDeviceResponse>>();
            Response.Msg = no_err;
            
            try
            {
                Response.Ext = await _deviceManage.GetAllXDCAMDeviceAsync<XDCAMDeviceResponse>();
                if (Response.Ext == null || Response.Ext.Count <= 0)
                {
                    Response.Code = ResponseCodeDefines.NotFound;
                    Response.Msg = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}:error info: not find data!";
                }
                else
                Response.Code = ResponseCodeDefines.SuccessCode;
            }
            catch (Exception e)//其他未知的异常，写异常日志
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
                    Response.Msg = "error info:" + e.Message;
                    Logger.Error("GetAllXDCAMDevice v2 : " + Response.Msg);
                }
            }

            return Response;
        }

    }
}
