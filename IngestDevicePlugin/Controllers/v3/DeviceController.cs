

namespace IngestDevicePlugin.Controllers.v3
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using IngestDBCore;
    using IngestDBCore.Basic;
    using IngestDBCore.Notify;
    using IngestDevicePlugin.Dto;
    using IngestDevicePlugin.Dto.Enum;
    using IngestDevicePlugin.Dto.OldResponse;
    using IngestDevicePlugin.Dto.Request;
    using IngestDevicePlugin.Dto.Response;
    using IngestDevicePlugin.Managers;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.ModelBinding;
    using Microsoft.Extensions.DependencyInjection;
    using Sobey.Core.Log;
    using SignalDeviceRequest = IngestDevicePlugin.Dto.Response.SignalDeviceMapResponse;
    using TsDeviceInfoResponse = IngestDevicePlugin.Dto.OldResponse.TSDeviceInfo;

    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("3.0")]
    [ApiController]
    public partial class DeviceController : ControllerBase
    {
        private readonly ILogger Logger = LoggerManager.GetLogger("DeviceInfo3");
        private readonly DeviceManager _deviceManage;
        //private readonly RestClient _restClient;
        private readonly Lazy<NotifyClock> _clock;
        public DeviceController(DeviceManager task, IServiceProvider services)
        { 
            _deviceManage = task;
            _clock = new Lazy<NotifyClock>(() => services.GetRequiredService<NotifyClock>());
        }

        #region Device
        /// <summary>
        /// 获取所有的采集设备信息
        /// </summary>
        /// <remarks>原方法 GetAllCaptureDevices</remarks>
        /// <returns>采集设备集合</returns>
        [HttpGet("device")]
        [ApiExplorerSettings(GroupName = "v3.0")]
        public async Task<ResponseMessage<List<CaptureDeviceInfoResponse>>> AllCaptureDevices()
        {
            ResponseMessage<List<CaptureDeviceInfoResponse>> response = new ResponseMessage<List<CaptureDeviceInfoResponse>>();
            try
            {
                response.Ext = await _deviceManage.GetAllCaptureDevicesAsync<CaptureDeviceInfoResponse>();
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
        /// 获取所有的采集设备全信息(区别于2.0的capturedevice)
        /// </summary>
        /// <remarks>包括设备ip，设备端口号等</remarks>
        /// <returns>采集设备集合</returns>
        [HttpGet("device-channel")] //直接用device不能和上面区分，返回值也不一样
        [ApiExplorerSettings(GroupName = "v3")]
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
        [HttpGet("device-channel/{deviceid}")]
        [ApiExplorerSettings(GroupName = "v3")]
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

        
        #endregion

        #region in,out
        /// <summary>
        /// 获取所有信号源
        /// </summary>
        /// <remarks>原方法 GetAllSignalSrcs</remarks>
        /// <returns>信号源信息集合</returns>
        [HttpGet("in/signal")]
        [ApiExplorerSettings(GroupName = "v3")]
        public async Task<ResponseMessage<List<SignalSrcResponse>>> AllSignalSrcs(
            //[FromHeader(Name = "sobeyhive-http-site"), BindRequired, DefaultValue("S1")] string site
            )
        {
            ResponseMessage<List<SignalSrcResponse>> response = new ResponseMessage<List<SignalSrcResponse>>();
            try
            {
                response.Ext = await _deviceManage.GetAllSignalSrcsAsync<SignalSrcResponse>();
                //_deviceManage.GetAllSignalSrcsBySiteAsync<SignalSrcResponse>(site);
                if (response.Ext == null || response.Ext?.Count <= 0)
                {
                    response.Code = ResponseCodeDefines.NotFound;
                    response.Msg = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}:error info: not find data!";
                }

                Logger.Info($"AllSignalSrcs v3 result : {Newtonsoft.Json.JsonConvert.SerializeObject(response.Ext)}");
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
        [HttpGet("in/signal-in-group")]//关于节目的url不太好
        [ApiExplorerSettings(GroupName = "v3")]
        public async Task<ResponseMessage<List<ProgrammeInfoResponse>>> AllProgrammeInfos(
            //[FromHeader(Name = "sobeyhive-http-site"), BindRequired, DefaultValue("S1")] string site
            )
        {
            ResponseMessage<List<ProgrammeInfoResponse>> response = new ResponseMessage<List<ProgrammeInfoResponse>>();
            try
            {
                response.Ext = await _deviceManage.GetAllProgrammeInfosAsync<ProgrammeInfoResponse>();
                //await _deviceManage.GetAllProgrammeInfosBySiteAsync<ProgrammeInfoResponse>(site);
                if (response.Ext == null || response.Ext?.Count <= 0)
                {
                    response.Code = ResponseCodeDefines.NotFound;
                    response.Msg = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}:error info: not find data!";
                }
                Logger.Info($"AllProgrammeInfos v3 Result : {Newtonsoft.Json.JsonConvert.SerializeObject(response.Ext)}");
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
        ///
        /// </summary>
        /// <remarks>
        ///
        /// </remarks>
        /// GetAllChannelUnitMap
        [HttpGet("in/backup/{mastersignalid}")]//关于节目的url不太好
        //device有点特殊，做了监听端口的所以不能全类检验
        [ApiExplorerSettings(GroupName = "v3.0")]
        public async Task<ResponseMessage<ProgrammeInfoResponse>> GetBackProgramInfoBySrgid([FromRoute, BindRequired, DefaultValue(20)] int mastersignalid)
        {
            var Response = new ResponseMessage<ProgrammeInfoResponse>();

            try
            {
                Response.Ext = await _deviceManage.GetBackProgramInfoBySrgid(mastersignalid);
                if (Response.Ext == null)
                {
                    Response.Code = ResponseCodeDefines.NotFound;
                    Response.Msg = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}:error info: not find data!";
                }
            }
            catch (Exception e)
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
                    Response.Msg = "GetBackProgramInfoBySrgid error info:" + e.ToString();
                    Logger.Error(Response.Msg);
                }
            }
            return Response;
        }


        /// <summary>
        /// 根据节目ID获取相应的通道，有矩阵模式和无矩阵模式的区别
        /// </summary>
        /// <remarks>
        ///
        /// </remarks>
        /// <param name="signalid">信号源id</param>
        /// <param name="status">int 0是不选返回所有通道信息，1是选通道和msv连接正常的通道信息</param>
        /// <returns>当前信号源匹配通道，是list</returns>
        [HttpGet("in/{signalid}/channel")]
        //device有点特殊，做了监听端口的所以不能全类检验
        [ApiExplorerSettings(GroupName = "v3.0")]
        public async Task<ResponseMessage<List<CaptureChannelInfoResponse>>> ChannelsOnAreaByProgrammeId([FromRoute, BindRequired, DefaultValue(39)] int signalid, [FromQuery, BindRequired, DefaultValue("0")] int status)
        {
            ResponseMessage<List<CaptureChannelInfoResponse>> response = new ResponseMessage<List<CaptureChannelInfoResponse>>();

            try
            {
                response.Ext = await _deviceManage.GetChannelsOnAreaByProgrammeIdAsync<CaptureChannelInfoResponse>(signalid, status);

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
        ///
        /// </summary>
        /// <remarks>
        ///
        /// </remarks>
        /// GetAllChannelUnitMap
        [HttpGet("in/{signalid}")]
        //device有点特殊，做了监听端口的所以不能全类检验
        [ApiExplorerSettings(GroupName = "v3.0")]
        public async Task<ResponseMessage<ProgrammeInfoResponse>> GetProgramInfoBySrgid([FromRoute, BindRequired, DefaultValue(40)] int signalid)
        {
            var Response = new ResponseMessage<ProgrammeInfoResponse>();

            try
            {
                Response.Ext = await _deviceManage.GetProgrammeInfoByIdAsync(signalid);
                if (Response.Ext == null)
                {
                    Response.Code = ResponseCodeDefines.NotFound;
                    Response.Msg = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}:error info: not find data!";
                }
            }
            catch (Exception e)
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
                    Response.Msg = "GetProgramInfoBySrgid error info:" + e.ToString();
                    Logger.Error(Response.Msg);
                }
            }
            return Response;
        }

        /// <summary>
        /// 获取输出端口与信号源的映射
        /// </summary>
        /// <remarks>原方法 AllRouterOutPortInfos</remarks>
        /// <returns>输出端口的集合</returns>
        [HttpGet("out")]
        [ApiExplorerSettings(GroupName = "v3.0")]
        public async Task<ResponseMessage<List<RoterOutResponse>>> AllRouterOutPortInfos(
            //[FromHeader(Name = "sobeyhive-http-site"), BindRequired, DefaultValue("S1")] string site
            )
        {
            ResponseMessage<List<RoterOutResponse>> response = new ResponseMessage<List<RoterOutResponse>>();
            try
            {
                response.Ext = await _deviceManage.GetAllRouterOutPortAsync<RoterOutResponse>();
                    //_deviceManage.GetAllRouterOutPortBySiteAsync<RoterOutResponse>(site);
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
        [HttpGet("in")]
        //device有点特殊，做了监听端口的所以不能全类检验
        [ApiExplorerSettings(GroupName = "v3.0")]
        public async Task<ResponseMessage<List<RouterInResponse>>> AllRouterInPortInfos(
            //[FromHeader(Name = "sobeyhive-http-site"), BindRequired] string site
            )
        {
            ResponseMessage<List<RouterInResponse>> response = new ResponseMessage<List<RouterInResponse>>();
            try
            {
                response.Ext = await _deviceManage.GetAllRouterInPortAsync<RouterInResponse>();
                //_deviceManage.GetAllRouterInPortBySiteAsync<RouterInResponseEx>();
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
        [HttpGet("in/{signalid}/capturetemplate/id")]
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

        #endregion

        #region channel
        /// <summary>
        /// 获取所有采集通道
        /// </summary>
        /// <remarks>原方法 GetAllCaptureChannels</remarks>
        /// <returns>采集通道集合</returns>
        [HttpGet("channel")]
        [ApiExplorerSettings(GroupName = "v3")]
        public async Task<ResponseMessage<List<CaptureChannelInfoResponse>>> AllCaptureChannels()
        {
            ResponseMessage<List<CaptureChannelInfoResponse>> response = new ResponseMessage<List<CaptureChannelInfoResponse>>();
            try
            {
                response.Ext = await _deviceManage.GetAllCaptureChannelsAsync<CaptureChannelInfoResponse>();
                if (response.Ext == null || response.Ext.Count == 0)
                {
                    response.Code = ResponseCodeDefines.NotFound;
                    response.Msg = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}:error info: not find data!";
                }
                Logger.Info($"AllCaptureChannels v3 Result : {Newtonsoft.Json.JsonConvert.SerializeObject(response.Ext)}");
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
        [ApiExplorerSettings(GroupName = "v3")]
        public async Task<ResponseMessage<List<MSVChannelStateResponse>>> AllChannelState()
        {
            ResponseMessage<List<MSVChannelStateResponse>> response = new ResponseMessage<List<MSVChannelStateResponse>>();
            try
            {
                response.Ext = await _deviceManage.GetAllChannelStateAsync<MSVChannelStateResponse>();
                if (response.Ext == null || response.Ext.Count <= 0)
                {
                    response.Code = ResponseCodeDefines.NotFound;
                    response.Msg = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}:error info: not find data!";
                }
                Logger.Info($"AllChannelState v3 Result : {Newtonsoft.Json.JsonConvert.SerializeObject(response.Ext)}");
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
        /// 更改MSV设备状态信息
        /// </summary>
        /// <remarks>原方法 GetModifyDevState</remarks>
        /// <param name="id">对象id</param>
        /// <param name="data">更新的对象</param>
        /// <returns>是否成功</returns>
        [HttpPost("channel/{id}/state")]
        [ApiExplorerSettings(GroupName = "v3")]
        public async Task<ResponseMessage<bool>> UpdateMSVChannelState([FromRoute, BindRequired, DefaultValue(20)] int id,
                                                                       [FromBody, BindRequired] DeviceMSVChannelStateRequest data)
        {
            Logger.Info($"UpdateMSVChannelState v3 id : {id} , data : {Newtonsoft.Json.JsonConvert.SerializeObject(data)}");
            ResponseMessage<bool> response = new ResponseMessage<bool>();
            try
            {
                response.Ext = await _deviceManage.UpdateMSVChannelStateAsync(id, data.DevState, data.MSVMode);
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
        /// 获取通道的扩展数据
        /// </summary>
        /// <remarks>原方法 PostUpdateChnExtData</remarks>
        /// <returns>是否成功</returns>
        [HttpGet("channel/{channelid}/extenddata")]
        [ApiExplorerSettings(GroupName = "v3.0")]
        public async Task<ResponseMessage<string>> GetChannelExtendData([FromRoute, BindRequired, DefaultValue(24)] int channelid,
                                                                        [FromQuery, BindRequired, DefaultValue(2)] int type)
        {
            ResponseMessage<string> response = new ResponseMessage<string>();
            try
            {
                response.Ext = await _deviceManage.GetChannelExtendData(channelid, type);
                if (string.IsNullOrEmpty(response.Ext))
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
        ///
        /// </summary>
        /// <remarks>
        ///
        /// </remarks>
        /// GetAllChannelUnitMap
        [HttpGet("channel/{channelid}/unitid")]
        //device有点特殊，做了监听端口的所以不能全类检验
        [ApiExplorerSettings(GroupName = "v3.0")]
        public async Task<ResponseMessage<int>> GetChannelUnitMapID([FromRoute, BindRequired, DefaultValue(911)] int channelid)
        {
            ResponseMessage<int> response = new ResponseMessage<int>();
            try
            {
                var f = await _deviceManage.GetChannelUnitMap(channelid);
                if (f != null)
                {
                    response.Ext = f.UnitId;
                }
                else
                    response.Ext = -1;
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
                    response.Msg = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}error info:{e.Message}";
                    Logger.Error(response.Msg);
                }
            }
            return response;
        }

        /// <summary>
        /// 根据通道ID获取相应的信号源id
        /// </summary>
        /// <remarks>
        ///
        /// </remarks>
        /// <param name="channelid">通道id</param>
        /// <param name="SignalStrict">根据可选参数来判断是否要有返回</param>
        [HttpGet("channel/{channelid}/signalid")]
        //device有点特殊，做了监听端口的所以不能全类检验
        [ApiExplorerSettings(GroupName = "v3.0")]
        public async Task<ResponseMessage<int>> GetChannelSignalSrc([FromRoute, BindRequired, DefaultValue(16)] int channelid, [FromQuery] bool SignalStrict = true)
        {
            ResponseMessage<int> response = new ResponseMessage<int>();
            try
            {
                response.Ext = await _deviceManage.GetChannelSignalSrcAsync(channelid, SignalStrict);
                if (response.Ext < 0)
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
        /// 通知设备变化消息
        /// </summary>
        /// <returns></returns>
        [HttpPut("notify/{type}")]
        [ApiExplorerSettings(GroupName = "v3")]
        public async Task<ResponseMessage> NotifyDeviceChange([FromRoute, BindRequired]DeviceNotify type, [FromQuery, BindRequired]int data)
        {
            ResponseMessage response = new ResponseMessage();
            try
            {
                string notifyaction = string.Empty;
                switch (type)
                {
                    case DeviceNotify.ChannelChange:
                        notifyaction = GlobalStateName.CHANNELCHANGE;
                        break;
                    case DeviceNotify.ChannelDelete:
                        notifyaction = GlobalStateName.CHANNELDELETE;
                        break;
                    case DeviceNotify.DeviceChange:
                        notifyaction = GlobalStateName.DEVICECHANGE;
                        break;
                    case DeviceNotify.DeviceDelete:
                        notifyaction = GlobalStateName.DEVICEDELETE;
                        break;
                    case DeviceNotify.InChange:
                        notifyaction = GlobalStateName.INCHANGE;
                        break;
                    case DeviceNotify.InDelete:
                        notifyaction = GlobalStateName.INDELETE;
                        break;
                    case DeviceNotify.OutChange:
                        notifyaction = GlobalStateName.OUTCHANGE;
                        break;
                    case DeviceNotify.OutDelete:
                        notifyaction = GlobalStateName.OUTDELETE;
                        break;
                    default:
                        break;
                }
                if (!string.IsNullOrEmpty(notifyaction))
                {
                    Task.Run(() =>
                    {
                        _clock.Value.InvokeNotify(notifyaction, NotifyPlugin.Orleans,
                                                    NotifyAction.STOPGROUPTASK, data);
                    });
                }
                

                Logger.Info($"NotifyDeviceChange v3 Result ok {type}");
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
        #endregion

    }
}