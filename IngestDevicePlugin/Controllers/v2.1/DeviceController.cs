using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IngestDBCore;
using IngestDBCore.Basic;
using IngestDevicePlugin.Dto;
using IngestDevicePlugin.Dto.OldResponse;
using IngestDevicePlugin.Dto.Request;
using IngestDevicePlugin.Dto.Response;
using IngestDevicePlugin.Managers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Sobey.Core.Log;
using SignalDeviceRequest = IngestDevicePlugin.Dto.Response.SignalDeviceMapResponse;
using TsDeviceInfoResponse = IngestDevicePlugin.Dto.OldResponse.TSDeviceInfo;

namespace IngestDevicePlugin.Controllers.v2._1
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("2.1")]
    [ApiController]
    public class DeviceController : ControllerBase
    {
        private readonly ILogger Logger = LoggerManager.GetLogger("DeviceInfo");
        private readonly DeviceManager _deviceManage;
        //private readonly RestClient _restClient;

        public DeviceController(DeviceManager task) { _deviceManage = task; }


        /// <summary>
        /// 获取所有的采集设备全信息(区别于2.0的capturedevice)
        /// </summary>
        /// <remarks>包括设备ip，设备端口号等</remarks>
        /// <returns>采集设备集合</returns>
        [HttpGet("device/all")]
        [ApiExplorerSettings(GroupName = "v2.1")]
        public async Task<ResponseMessage<List<DeviceInfoResponse>>> AllDevicesForTask()
        {
            ResponseMessage<List<DeviceInfoResponse>> response = new ResponseMessage<List<DeviceInfoResponse>>();
            try
            {
                response.Ext = await _deviceManage.GetAllDeviceInfoAsync<DeviceInfoResponse>();
                if (response.Ext == null)
                {
                    response.Code = ResponseCodeDefines.NotFound;
                    response.Msg = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}:error info: not find data!";
                }
            }
            catch (Exception e)
            {
                if (e is SobeyRecException se)//sobeyexcep会自动打印错误
                {
                    response.Code = se.ErrorCode.ToString();
                    response.Msg = se.Message;
                }
                else
                {
                    response.Code = ResponseCodeDefines.ServiceError;
                    response.Msg = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}:error info:{e.Message}";
                    Logger.Error(response.Msg);
                }
            }
            return response;
        }

        /// <summary>
        /// 获取指定采集设备全信息(区别于2.0的capturedevice)
        /// </summary>
        /// <remarks>包括设备ip，设备端口号等</remarks>
        /// <returns>采集设备单个信息</returns>
        [HttpGet("device/{deviceid}")]
        [ApiExplorerSettings(GroupName = "v2.1")]
        public async Task<ResponseMessage<DeviceInfoResponse>> GetCaptureDeviceByidForTask([FromRoute, BindRequired, DefaultValue(39)]int deviceid)
        {
            ResponseMessage<DeviceInfoResponse> response = new ResponseMessage<DeviceInfoResponse>();
            try
            {
                response.Ext = await _deviceManage.GetDeviceInfoByIDAsync<DeviceInfoResponse>(deviceid);
                if (response.Ext == null)
                {
                    response.Code = ResponseCodeDefines.NotFound;
                    response.Msg = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}:error info: not find data!";
                }
            }
            catch (Exception e)
            {
                if (e is SobeyRecException se)//sobeyexcep会自动打印错误
                {
                    response.Code = se.ErrorCode.ToString();
                    response.Msg = se.Message;
                }
                else
                {
                    response.Code = ResponseCodeDefines.ServiceError;
                    response.Msg = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}:error info:{e.Message}";
                    Logger.Error(response.Msg);
                }
            }
            return response;
        }
        
       
    }
}