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

namespace IngestDevicePlugin.Controllers.v3
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("3.0")]
    [ApiController]
    public partial class DeviceController : ControllerBase
    {
        private readonly ILogger Logger = LoggerManager.GetLogger("DeviceInfo3");
        private readonly DeviceManager _deviceManage;
        //private readonly RestClient _restClient;

        public DeviceController(DeviceManager task) { _deviceManage = task; }


        /// <summary>
        /// 获取所有的采集设备全信息(区别于2.0的capturedevice)
        /// </summary>
        /// <remarks>包括设备ip，设备端口号等</remarks>
        /// <returns>采集设备集合</returns>
        [HttpGet("allocdevice")]
        [ApiExplorerSettings(GroupName = "v3.0")]
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
        [ApiExplorerSettings(GroupName = "v3.0")]
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
        [HttpGet("channel/state")]
        [ApiExplorerSettings(GroupName = "v3.0")]
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
                Logger.Info($"AllChannelState Site Result : {Newtonsoft.Json.JsonConvert.SerializeObject(response.Ext)}");
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
        [HttpGet("channel")]
        [ApiExplorerSettings(GroupName = "v3.0")]
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
                Logger.Info($"AllCaptureChannels Site Result : {Newtonsoft.Json.JsonConvert.SerializeObject(response.Ext)} ");
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
        /// 通过 通道ID 获取采集通道
        /// </summary>
        /// <remarks>原方法 GetCaptureChannelByID</remarks>
        /// <param name="channelid">通道Id</param>
        /// <returns>采集通道</returns>
        [HttpGet("channel/{channelid}")]
        [ApiExplorerSettings(GroupName = "v3.0")]
        public async Task<ResponseMessage<CaptureChannelInfoResponse>> CaptureChannelByID([FromRoute, BindRequired, DefaultValue(2)] int channelid)
        {
            ResponseMessage<CaptureChannelInfoResponse> response = new ResponseMessage<CaptureChannelInfoResponse>();
            try
            {
                response.Ext = await _deviceManage.GetSiteCaptureChannelByIDAsync<CaptureChannelInfoResponse>(channelid);
                if (response.Ext == null)
                {
                    response.Code = ResponseCodeDefines.NotFound;
                    response.Msg = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}:error info: not find data!";
                }
                Logger.Info($"CaptureChannelByID Site chnId : {channelid} , Result : {Newtonsoft.Json.JsonConvert.SerializeObject(response.Ext)}");
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
        [HttpGet("programme")]
        [ApiExplorerSettings(GroupName = "v3.0")]
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
                Logger.Info($"AllProgrammeInfos Site Result : {Newtonsoft.Json.JsonConvert.SerializeObject(response.Ext)}");
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
        /// 根据节目ID获取相应的通道，有矩阵模式和无矩阵模式的区别
        /// </summary>
        /// <remarks>
        ///
        /// </remarks>
        /// <param name="programmeid">信号源id</param>
        /// <param name="status">int 0是不选返回所有通道信息，1是选通道和msv连接正常的通道信息</param>
        /// <returns>当前信号源匹配通道，是list</returns>
        [HttpGet("programme/{programmeid}")]
        //device有点特殊，做了监听端口的所以不能全类检验
        [ApiExplorerSettings(GroupName = "v3.0")]
        public async Task<ResponseMessage<List<CaptureChannelInfoResponse>>> ChannelsOnAreaByProgrammeId([FromRoute, BindRequired, DefaultValue(39)] int programmeid, [FromQuery, BindRequired, DefaultValue("0")] int status)
        {
            ResponseMessage<List<CaptureChannelInfoResponse>> response = new ResponseMessage<List<CaptureChannelInfoResponse>>();
            try
            {
                response.Ext = await _deviceManage.GetChannelsOnAreaByProgrammeIdAsync<CaptureChannelInfoResponse>(programmeid,
                                                                                                             status);
                if (response.Ext == null || response.Ext?.Count <= 0)
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
        /// 获取Rtmp的采集设备信息
        /// </summary>
        /// <remarks></remarks>
        /// <returns>采集设备集合</returns>
        [HttpGet("channel/rtmp")]
        [ApiExplorerSettings(GroupName = "v3.0")]
        public async Task<ResponseMessage<List<CaptureChannelInfoInterface>>> RtmpCaptureChannels([FromHeader(Name = "sobeyhive-http-site"), BindRequired] string site)
        {
            ResponseMessage<List<CaptureChannelInfoInterface>> response = new ResponseMessage<List<CaptureChannelInfoInterface>>();
            try
            {
                response.Ext = await _deviceManage.GetRtmpCaptureChannelsBySiteAreaAsync<CaptureChannelInfoInterface>(site, -1);
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
        /// 获取输出端口与信号源的映射
        /// </summary>
        /// <remarks>原方法 AllRouterOutPortInfos</remarks>
        /// <returns>输出端口的集合</returns>
        [HttpGet("routeroutport")]
        [ApiExplorerSettings(GroupName = "v3.0")]
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
                Logger.Info($"AllRouterOutPortInfos Site Result : {Newtonsoft.Json.JsonConvert.SerializeObject(response.Ext)}");
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
        [HttpGet("routerinport")]
        //device有点特殊，做了监听端口的所以不能全类检验
        [ApiExplorerSettings(GroupName = "v3.0")]
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
                Logger.Info($"AllRouterInPortInfos Site Result : {Newtonsoft.Json.JsonConvert.SerializeObject(response.Ext)}");

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
        /// 根据 信号源Id 获取绑定的采集参数 id
        /// </summary>
        /// <remarks>原方法 GetCaptureTemplateIDBySignalID</remarks>
        /// <param name="signalid">信号ID</param>
        /// <returns>采集参数</returns>
        /// <example>1111</example>
        [HttpGet("capturetemplate/id/{signalid}")]
        [ApiExplorerSettings(GroupName = "v3.0")]
        public async Task<ResponseMessage<int>> CaptureTemplateId([FromRoute, BindRequired, DefaultValue(39)] int signalid)
        {
            ResponseMessage<int> response = new ResponseMessage<int>();
            try
            {
                response.Ext = await _deviceManage.GetSignalCaptureTemplateAsync(signalid);
                if (response.Ext <= 0)
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
        /// 获取所有信号源
        /// </summary>
        /// <remarks>原方法 GetAllSignalSrcs</remarks>
        /// <returns>信号源信息集合</returns>
        [HttpGet("signalsrc")]
        [ApiExplorerSettings(GroupName = "v3.0")]
        public async Task<ResponseMessage<List<SignalSrcResponse>>> AllSignalSrcs([FromHeader(Name = "sobeyhive-http-site"), BindRequired, DefaultValue("S1")] string site)
        {
            ResponseMessage<List<SignalSrcResponse>> response = new ResponseMessage<List<SignalSrcResponse>>();
            try
            {
                response.Ext = await _deviceManage.GetAllSignalSrcsBySiteAsync<SignalSrcResponse>(site);
                if (response.Ext == null || response.Ext?.Count <= 0)
                {
                    response.Code = ResponseCodeDefines.NotFound;
                    response.Msg = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}:error info: not find data!";
                }
                Logger.Info($"AllSignalSrcs Site : {site}, result  : {Newtonsoft.Json.JsonConvert.SerializeObject(response.Ext)}");
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