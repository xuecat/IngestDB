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
        private readonly ILogger Logger = LoggerManager.GetLogger("DeviceInfo21");
        private readonly DeviceManager _deviceManage;
        //private readonly RestClient _restClient;

        public DeviceController(DeviceManager task) { _deviceManage = task; }


        /// <summary>
        /// 获取所有的采集设备全信息(区别于2.0的capturedevice)
        /// </summary>
        /// <remarks>包括设备ip，设备端口号等</remarks>
        /// <returns>采集设备集合</returns>
        [HttpGet("allocdevice")]
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
        /// <param name="deviceid">设备ID</param>
        /// <remarks>包括设备ip，设备端口号等</remarks>
        /// <returns>采集设备单个信息</returns>
        [HttpGet("allocdevice/{deviceid}")]
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


        /// <summary>
        /// 获得所有通道的状态
        /// </summary>
        /// <remarks>原方法 GetAllChannelState</remarks>
        /// <returns>最优通道Id</returns>
        [HttpGet("channelstate/all")]
        [ApiExplorerSettings(GroupName = "v2.1")]
        public async Task<ResponseMessage<List<MSVChannelStateResponse>>> AllChannelState([FromHeader(Name = "sobeyhive-http-site"), BindRequired, DefaultValue("S1")]string site)
        {
            ResponseMessage<List<MSVChannelStateResponse>> response = new ResponseMessage<List<MSVChannelStateResponse>>();
            try
            {
                response.Ext = await _deviceManage.GetAllChannelStateBySiteAsync<MSVChannelStateResponse>(site);
                if (response.Ext == null || response.Ext.Count <= 0)
                {
                    response.Code = ResponseCodeDefines.NotFound;
                    response.Msg = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}:error info: not find data!";
                }
                Logger.Error($"AllChannelState Site Result : {Newtonsoft.Json.JsonConvert.SerializeObject(response.Ext)}");
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
        /// 获取所有采集通道
        /// </summary>
        /// <remarks>原方法 GetAllCaptureChannels</remarks>
        /// <returns>采集通道集合</returns>
        [HttpGet("capturechannel/all")]
        [ApiExplorerSettings(GroupName = "v2.1")]
        public async Task<ResponseMessage<List<CaptureChannelInfoResponse>>> AllCaptureChannels([FromHeader(Name = "sobeyhive-http-site"), BindRequired, DefaultValue("S1")] string site)
        {
            ResponseMessage<List<CaptureChannelInfoResponse>> response = new ResponseMessage<List<CaptureChannelInfoResponse>>();
            try
            {
                response.Ext = await _deviceManage.GetAllCaptureChannelsBySiteAsync<CaptureChannelInfoResponse>(site);
                if (response.Ext == null || response.Ext.Count == 0)
                {
                    response.Code = ResponseCodeDefines.NotFound;
                    response.Msg = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}:error info: not find data!";
                }
                Logger.Error($"AllCaptureChannels Site Result : {Newtonsoft.Json.JsonConvert.SerializeObject(response.Ext)} ");
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
        /// 获取所有节目
        /// </summary>
        /// <remarks>原方法 GetAllProgrammeInfos</remarks>
        /// <returns>节目集合</returns>
        [HttpGet("programme/all")]
        [ApiExplorerSettings(GroupName = "v2.1")]
        public async Task<ResponseMessage<List<ProgrammeInfoResponse>>> AllProgrammeInfos([FromHeader(Name = "sobeyhive-http-site"), BindRequired, DefaultValue("S1")] string site)
        {
            ResponseMessage<List<ProgrammeInfoResponse>> response = new ResponseMessage<List<ProgrammeInfoResponse>>();
            try
            {
                response.Ext = await _deviceManage.GetAllProgrammeInfosBySiteAsync<ProgrammeInfoResponse>(site);
                if (response.Ext == null || response.Ext?.Count <= 0)
                {
                    response.Code = ResponseCodeDefines.NotFound;
                    response.Msg = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}:error info: not find data!";
                }
                Logger.Error($"AllProgrammeInfos Site Result : {Newtonsoft.Json.JsonConvert.SerializeObject(response.Ext)}");
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
        /// 获取输出端口与信号源的映射
        /// </summary>
        /// <remarks>原方法 AllRouterOutPortInfos</remarks>
        /// <returns>输出端口的集合</returns>
        [HttpGet("routeroutport/all")]
        [ApiExplorerSettings(GroupName = "v2.1")]
        public async Task<ResponseMessage<List<RoterOutResponseEx>>> AllRouterOutPortInfos([FromHeader(Name = "sobeyhive-http-site"), BindRequired, DefaultValue("S1")] string site)
        {
            ResponseMessage<List<RoterOutResponseEx>> response = new ResponseMessage<List<RoterOutResponseEx>>();
            try
            {
                response.Ext = await _deviceManage.GetAllRouterOutPortBySiteAsync<RoterOutResponseEx>(site);
                if (response.Ext == null || response.Ext?.Count <= 0)
                {
                    response.Code = ResponseCodeDefines.NotFound;
                    response.Msg = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}:error info: not find data!";
                }
                Logger.Error($"AllRouterOutPortInfos Site Result : {Newtonsoft.Json.JsonConvert.SerializeObject(response.Ext)}");
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
        /// 获取输入端口与信息
        /// </summary>
        /// <remarks>原方法 AllRouterInPortInfos</remarks>
        /// <returns>输入端口的集合</returns>
        [HttpGet("routerinport/all")]
        //device有点特殊，做了监听端口的所以不能全类检验
        [ApiExplorerSettings(GroupName = "v2.1")]
        public async Task<ResponseMessage<List<RouterInResponseEx>>> AllRouterInPortInfos([FromHeader(Name = "sobeyhive-http-site"), BindRequired, DefaultValue("S1")] string site)
        {
            ResponseMessage<List<RouterInResponseEx>> response = new ResponseMessage<List<RouterInResponseEx>>();
            try
            {
                response.Ext = await _deviceManage.GetAllRouterInPortBySiteAsync<RouterInResponseEx>(site);
                if (response.Ext == null || response.Ext?.Count <= 0)
                {
                    response.Code = ResponseCodeDefines.NotFound;
                    response.Msg = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}:error info: not find data!";
                }
                Logger.Error($"AllRouterInPortInfos Site Result : {Newtonsoft.Json.JsonConvert.SerializeObject(response.Ext)}");

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