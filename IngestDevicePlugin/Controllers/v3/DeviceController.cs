

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


        /// <summary>
        /// 获取所有的采集设备全信息(区别于2.0的capturedevice)
        /// </summary>
        /// <remarks>包括设备ip，设备端口号等</remarks>
        /// <returns>采集设备集合</returns>
        [HttpGet("allocdevice")]
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
        [HttpGet("allocdevice/{deviceid}")]
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

        /// <summary>
        /// 获取所有信号源
        /// </summary>
        /// <remarks>原方法 GetAllSignalSrcs</remarks>
        /// <returns>信号源信息集合</returns>
        [HttpGet("signalsrc")]
        [ApiExplorerSettings(GroupName = "v3")]
        public async Task<ResponseMessage<List<SignalSrcResponse>>> AllSignalSrcs()
        {
            ResponseMessage<List<SignalSrcResponse>> response = new ResponseMessage<List<SignalSrcResponse>>();
            try
            {
                response.Ext = await _deviceManage.GetAllSignalSrcsAsync<SignalSrcResponse>();
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
        [HttpGet("programme")]
        [ApiExplorerSettings(GroupName = "v3")]
        public async Task<ResponseMessage<List<ProgrammeInfoResponse>>> AllProgrammeInfos()
        {
            ResponseMessage<List<ProgrammeInfoResponse>> response = new ResponseMessage<List<ProgrammeInfoResponse>>();
            try
            {
                response.Ext = await _deviceManage.GetAllProgrammeInfosAsync<ProgrammeInfoResponse>();
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

    }
}