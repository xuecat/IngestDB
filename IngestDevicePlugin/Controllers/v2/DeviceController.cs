using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IngestDBCore;
using IngestDBCore.Basic;
using IngestDevicePlugin.Dto;
using IngestDevicePlugin.Dto.Request;
using IngestDevicePlugin.Dto.Response;
using IngestDevicePlugin.Managers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Sobey.Core.Log;
using SignalDeviceRequest = IngestDevicePlugin.Dto.Response.SignalDeviceMapResponse;
using TsDeviceInfoResponse = IngestDevicePlugin.Dto.TSDeviceInfo;

namespace IngestDevicePlugin.Controllers.v2
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("2.0")]
    [ApiController]
    public class DeviceController : ControllerBase
    {
        private readonly ILogger Logger = LoggerManager.GetLogger("DeviceInfo");
        private readonly DeviceManager _deviceManage;
        //private readonly RestClient _restClient;

        public DeviceController(DeviceManager task) { _deviceManage = task; }

        #region Capture

        /// <summary>
        /// 获取所有采集通道
        /// </summary>
        /// <remarks>原方法 GetAllCaptureChannels</remarks>
        /// <returns>采集通道集合</returns>
        [HttpGet("capturechannel/all")]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<List<CaptureChannelInfoResponse>>> AllCaptureChannels()
        {
            ResponseMessage<List<CaptureChannelInfoResponse>> response = new ResponseMessage<List<CaptureChannelInfoResponse>>();
            try
            {
                response.Ext = await _deviceManage.GetAllCaptureChannelsAsync<CaptureChannelInfoResponse>();
            } catch(Exception e)
            {
                if(e is SobeyRecException se)//sobeyexcep会自动打印错误
                {
                    response.Code = se.ErrorCode.ToString();
                    response.Msg = se.Message;
                } else
                {
                    response.Code = ResponseCodeDefines.ServiceError;
                    response.Msg = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}：error info:{e.Message}";
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
        [HttpGet("capturechannel/{channelid}")]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<CaptureChannelInfoResponse>> CaptureChannelByID([FromRoute, BindRequired, DefaultValue(2)]int channelid)
        {
            ResponseMessage<CaptureChannelInfoResponse> response = new ResponseMessage<CaptureChannelInfoResponse>();
            try
            {
                response.Ext = await _deviceManage.GetCaptureChannelByIDAsync<CaptureChannelInfoResponse>(channelid);
            } catch(Exception e)
            {
                if(e is SobeyRecException se)//sobeyexcep会自动打印错误
                {
                    response.Code = se.ErrorCode.ToString();
                    response.Msg = se.Message;
                } else
                {
                    response.Code = ResponseCodeDefines.ServiceError;
                    response.Msg = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}：error info:{e.Message}";
                    Logger.Error(response.Msg);
                }
            }
            return response;
        }

        /// <summary>
        /// 获取所有的采集设备信息
        /// </summary>
        /// <remarks>原方法 GetAllCaptureDevices</remarks>
        /// <returns>采集设备集合</returns>
        [HttpGet("capturedevice/all")]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<List<CaptureDeviceInfoResponse>>> AllCaptureDevices()
        {
            ResponseMessage<List<CaptureDeviceInfoResponse>> response = new ResponseMessage<List<CaptureDeviceInfoResponse>>();
            try
            {
                response.Ext = await _deviceManage.GetAllCaptureDevicesAsync<CaptureDeviceInfoResponse>();
            } catch(Exception e)
            {
                if(e is SobeyRecException se)//sobeyexcep会自动打印错误
                {
                    response.Code = se.ErrorCode.ToString();
                    response.Msg = se.Message;
                } else
                {
                    response.Code = ResponseCodeDefines.ServiceError;
                    response.Msg = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}：error info:{e.Message}";
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
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<int>> CaptureTemplateId([FromRoute, BindRequired, DefaultValue(39)]int signalid)
        {
            ResponseMessage<int> response = new ResponseMessage<int>();
            try
            {
                response.Ext = await _deviceManage.GetSignalCaptureTemplateAsync(signalid);
            } catch(Exception e)
            {
                if(e is SobeyRecException se)//sobeyexcep会自动打印错误
                {
                    response.Code = se.ErrorCode.ToString();
                    response.Msg = se.Message;
                } else
                {
                    response.Code = ResponseCodeDefines.ServiceError;
                    response.Msg = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}：error info:{e.Message}";
                    Logger.Error(response.Msg);
                }
            }
            return response;
        }

        #endregion Capture

        #region Channel

        /// <summary>
        /// 根据 信号源Id 为信号源选择一个合适的预监通道 id
        /// </summary>
        /// <remarks>原方法 GetBestPreviewChannelForSignal</remarks>
        /// <param name="signalid">信号ID</param>
        /// <returns>预监通道Id</returns>

        [HttpGet("bestpreviewchannel/id/{signalid}")]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<int>> BestPreviewChannelId([FromRoute, BindRequired, DefaultValue(39)]int signalid)
        {
            ResponseMessage<int> response = new ResponseMessage<int>();
            try
            {
                response.Ext = await _deviceManage.GetBestPreviewChnForSignalAsync(signalid);
            } catch(Exception e)
            {
                if(e is SobeyRecException se)//sobeyexcep会自动打印错误
                {
                    response.Code = se.ErrorCode.ToString();
                    response.Msg = se.Message;
                } else
                {
                    response.Code = ResponseCodeDefines.ServiceError;
                    response.Msg = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}：error info:{e.Message}";
                    Logger.Error(response.Msg);
                }
            }
            return response;
        }

        /// <summary>
        /// 更新通道的扩展数据
        /// </summary>
        /// <remarks>原方法 PostUpdateChnExtData</remarks>
        /// <param name="channelid">通道ID</param>
        /// <param name="data">设备通道扩展数据</param>
        /// <returns>是否成功</returns>
        [HttpPost("channel/extenddata/{channelid}")]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<bool>> UpdateChnExtData([FromRoute, BindRequired, DefaultValue(911)]int channelid,
                                                                  [FromBody, BindRequired]DeviceChannelExtdataRequest data)
        {
            ResponseMessage<bool> response = new ResponseMessage<bool>();
            try
            {
                response.Ext = await _deviceManage.SaveChnExtenddataAsync(channelid, data.Datatype, data.Extenddata);
            } catch(Exception e)
            {
                if(e is SobeyRecException se)//sobeyexcep会自动打印错误
                {
                    response.Code = se.ErrorCode.ToString();
                    response.Msg = se.Message;
                } else
                {
                    response.Code = ResponseCodeDefines.ServiceError;
                    response.Msg = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}：error info:{e.Message}";
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
        [HttpGet("channel/extenddata/{channelid}")]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<string>> GetChannelExtendData([FromRoute, BindRequired, DefaultValue(24)]int channelid,
                                                                        [FromQuery, BindRequired, DefaultValue(2)]int type)
        {
            ResponseMessage<string> response = new ResponseMessage<string>();
            try
            {
                response.Ext = await _deviceManage.GetChannelExtendData(channelid, type);
            } catch(Exception e)
            {
                if(e is SobeyRecException se)//sobeyexcep会自动打印错误
                {
                    response.Code = se.ErrorCode.ToString();
                    response.Msg = se.Message;
                } else
                {
                    response.Code = ResponseCodeDefines.ServiceError;
                    response.Msg = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}：error info:{e.Message}";
                    Logger.Error(response.Msg);
                }
            }
            return response;
        }

        /// <summary>
        /// 根据 信号源Id,用户Code 自动匹配最优通道
        /// </summary>
        /// <remarks>原方法 GetBestChannelIDBySignalID</remarks>
        /// <param name="signalid">信号源Id</param>
        /// <param name="usercode">用户Code</param>
        /// <returns>最优通道Id</returns>
        [HttpGet("bestchannel/id/{signalid}")]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<int>> BestChannelId([FromRoute, BindRequired, DefaultValue(20)]int signalid,
                                                              [FromQuery, BindRequired, DefaultValue("8de083d45c614628b99516740d628e91")]string usercode)
        {
            ResponseMessage<int> response = new ResponseMessage<int>();
            try
            {
                response.Ext = await _deviceManage.GetBestChannelIdBySignalIDAsync(signalid, usercode);
            } catch(Exception e)
            {
                if(e is SobeyRecException se)//sobeyexcep会自动打印错误
                {
                    response.Code = se.ErrorCode.ToString();
                    response.Msg = se.Message;
                } else
                {
                    response.Code = ResponseCodeDefines.ServiceError;
                    response.Msg = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}：error info:{e.Message}";
                    Logger.Error(response.Msg);
                }
            }
            return response;
        }

        /// <summary>
        /// 根据 programmeId 获取相应的通道，有矩阵模式和无矩阵模式的区别
        /// </summary>
        /// <remarks>原方法 GetChannelsByProgrammeId</remarks>
        /// <param name="programmeid">programmeId</param>
        /// <returns>采集通道集合</returns>
        [HttpGet("channel/{programmeid}")]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<List<CaptureChannelInfoResponse>>> Channels([FromRoute, BindRequired, DefaultValue(12)]int programmeid)
        {
            ResponseMessage<List<CaptureChannelInfoResponse>> response = new ResponseMessage<List<CaptureChannelInfoResponse>>();
            try
            {
                response.Ext = await _deviceManage.GetChannelsByProgrammeIdAsync<CaptureChannelInfoResponse>(programmeid);
            } catch(Exception e)
            {
                if(e is SobeyRecException se)//sobeyexcep会自动打印错误
                {
                    response.Code = se.ErrorCode.ToString();
                    response.Msg = se.Message;
                } else
                {
                    response.Code = ResponseCodeDefines.ServiceError;
                    response.Msg = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}：error info:{e.Message}";
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
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<List<MSVChannelStateResponse>>> AllChannelState()
        {
            ResponseMessage<List<MSVChannelStateResponse>> response = new ResponseMessage<List<MSVChannelStateResponse>>();
            try
            {
                response.Ext = await _deviceManage.GetAllChannelStateAsync<MSVChannelStateResponse>();
            } catch(Exception e)
            {
                if(e is SobeyRecException se)//sobeyexcep会自动打印错误
                {
                    response.Code = se.ErrorCode.ToString();
                    response.Msg = se.Message;
                } else
                {
                    response.Code = ResponseCodeDefines.ServiceError;
                    response.Msg = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}：error info:{e.Message}";
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
        [HttpPost("channelstate/{id}")]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<bool>> UpdateMSVChannelState([FromRoute, BindRequired, DefaultValue(20)]int id,
                                                                       [FromBody, BindRequired] DeviceMSVChannelStateRequest data)
        {
            ResponseMessage<bool> response = new ResponseMessage<bool>();
            try
            {
                response.Ext = await _deviceManage.UpdateMSVChannelStateAsync(id, data.DevState, data.MSVMode);
            } catch(Exception e)
            {
                if(e is SobeyRecException se)//sobeyexcep会自动打印错误
                {
                    response.Code = se.ErrorCode.ToString();
                    response.Msg = se.Message;
                } else
                {
                    response.Code = ResponseCodeDefines.ServiceError;
                    response.Msg = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}：error info:{e.Message}";
                    Logger.Error(response.Msg);
                }
            }
            return response;
        }

        //#endregion

        //#region Device

        /// <summary>
        /// 获得所有的IP收录的设备
        /// </summary>
        /// <remarks>原方法 GetAllTSDeviceInfos</remarks>
        /// <returns>最优通道Id</returns>
        [HttpGet("tsdeviceinfo/all")]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<List<TsDeviceInfoResponse>>> AllGTSDeviceInfos()
        {
            ResponseMessage<List<TsDeviceInfoResponse>> response = new ResponseMessage<List<TsDeviceInfoResponse>>();
            try
            {
                response.Ext = await _deviceManage.GetAllTSDeviceInfosAsync();
            } catch(Exception e)
            {
                if(e is SobeyRecException se)//sobeyexcep会自动打印错误
                {
                    response.Code = se.ErrorCode.ToString();
                    response.Msg = se.Message;
                } else
                {
                    response.Code = ResponseCodeDefines.ServiceError;
                    response.Msg = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}：error info:{e.Message}";
                    Logger.Error(response.Msg);
                }
            }
            return response;
        }

        /// <summary>
        /// 更新所有的IP收录的设备
        /// </summary>
        /// <remarks>原方法 PostUpdateAllTSDeviceInfos</remarks>
        /// <param name="datas">更新的对象</param>
        /// <returns>是否成功</returns>
        [HttpPost("tsdeviceinfo/all")]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<bool>> UpdateAllTSDeviceInfos([FromBody, BindRequired]DeviceTSDeviceInfoRequest datas)
        {
            ResponseMessage<bool> response = new ResponseMessage<bool>();
            try
            {
                response.Ext = await _deviceManage.UpdateAllTSDeviceInfosAsync(datas.deviceInfos.ToArray());
            } catch(Exception e)
            {
                if(e is SobeyRecException se)//sobeyexcep会自动打印错误
                {
                    response.Code = se.ErrorCode.ToString();
                    response.Msg = se.Message;
                } else
                {
                    response.Code = ResponseCodeDefines.ServiceError;
                    response.Msg = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}：error info:{e.Message}";
                    Logger.Error(response.Msg);
                }
            }
            return response;
        }

        /// <summary>
        /// 获取所有GPI设备
        /// </summary>
        /// <remarks>原方法 GetAllGPIDevices</remarks>
        /// <returns>GPI设备集合</returns>
        [HttpGet("gpidevices/all")]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<List<GPIDeviceInfoResponse>>> AllGPIDevices()
        {
            ResponseMessage<List<GPIDeviceInfoResponse>> response = new ResponseMessage<List<GPIDeviceInfoResponse>>();
            try
            {
                response.Ext = await _deviceManage.GetAllGPIInfoAsync<GPIDeviceInfoResponse>();
            } catch(Exception e)
            {
                if(e is SobeyRecException se)//sobeyexcep会自动打印错误
                {
                    response.Code = se.ErrorCode.ToString();
                    response.Msg = se.Message;
                } else
                {
                    response.Code = ResponseCodeDefines.ServiceError;
                    response.Msg = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}：error info:{e.Message}";
                    Logger.Error(response.Msg);
                }
            }
            return response;
        }

        /// <summary>
        /// 通过 GPIID 找出该GPI所有的映射
        /// </summary>
        /// <remarks>原方法 GetGPIMapInfoByGPIID</remarks>
        /// <param name="gpiid">GPIID</param>
        /// <returns>GPI设备集合</returns>
        [HttpGet("gpimapinfo/{gpiid}")]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<List<GPIDeviceMapInfoResponse>>> GPIMapInfo([FromRoute, BindRequired, DefaultValue(666)]int gpiid)
        {
            ResponseMessage<List<GPIDeviceMapInfoResponse>> response = new ResponseMessage<List<GPIDeviceMapInfoResponse>>();
            try
            {
                response.Ext = await _deviceManage.GetGPIMapInfoByGPIIDAsync<GPIDeviceMapInfoResponse>(gpiid);
            } catch(Exception e)
            {
                if(e is SobeyRecException se)//sobeyexcep会自动打印错误
                {
                    response.Code = se.ErrorCode.ToString();
                    response.Msg = se.Message;
                } else
                {
                    response.Code = ResponseCodeDefines.ServiceError;
                    response.Msg = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}：error info:{e.Message}";
                    Logger.Error(response.Msg);
                }
            }
            return response;
        }

        /// <summary>
        /// 根据 通道Id 获取MSV设备状态信息
        /// </summary>
        /// <remarks>原方法 GetMSVChannelState</remarks>
        /// <param name="channelid">通道Id</param>
        /// <returns>GPI设备集合</returns>
        [HttpGet("msvchannelstate/{channelid}")]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<MSVChannelStateResponse>> MSVChannelState([FromRoute, BindRequired, DefaultValue(12)]int channelid)
        {
            ResponseMessage<MSVChannelStateResponse> response = new ResponseMessage<MSVChannelStateResponse>();
            try
            {
                response.Ext = await _deviceManage.GetMsvChannelStateAsync<MSVChannelStateResponse>(channelid);
            } catch(Exception e)
            {
                if(e is SobeyRecException se)//sobeyexcep会自动打印错误
                {
                    response.Code = se.ErrorCode.ToString();
                    response.Msg = se.Message;
                } else
                {
                    response.Code = ResponseCodeDefines.ServiceError;
                    response.Msg = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}：error info:{e.Message}";
                    Logger.Error(response.Msg);
                }
            }
            return response;
        }

        /// <summary>
        /// 更改ModifySourceVTRIDAndUserCodeByChannelIDArray
        /// </summary>
        /// <remarks>原方法 ModifySourceVTRIDAndUserCodeByChannelIDArray</remarks>
        /// <param name="request">更新对象</param>
        /// <returns>是否成功</returns>
        [HttpPost("channelstate/usercode/vtrid")]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<bool>> ModifySourceVTRIDAndUserCodeByChannelIDArray([FromBody, BindRequired]DeviceMSVVTRAndUserCodeRequest request)
        {
            ResponseMessage<bool> response = new ResponseMessage<bool>();
            try
            {
                response.Ext = await _deviceManage.ModifySourceVTRIDAndUserCodeAsync(request.SourceVTRID,
                                                                                     request.UserCode,
                                                                                     request.IDArray);
            } catch(Exception e)
            {
                if(e is SobeyRecException se)//sobeyexcep会自动打印错误
                {
                    response.Code = se.ErrorCode.ToString();
                    response.Msg = se.Message;
                } else
                {
                    response.Code = ResponseCodeDefines.ServiceError;
                    response.Msg = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}：error info:{e.Message}";
                    Logger.Error(response.Msg);
                }
            }
            return response;
        }

        #endregion Channel

        #region ParamTyp

        /// <summary>
        /// 根据 通道Id 获取高清还是标清 nType:0标清,1高清
        /// </summary>
        /// <remarks>原方法 GetParamTypeByChannleID</remarks>
        /// <param name="channelid">通道Id</param>
        /// <returns>0标清,1高清</returns>
        [HttpGet("channelparamtype/{channelid}")]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<int>> ParamTypeByChannelID([FromRoute, BindRequired, DefaultValue(14)]int channelid)
        {
            ResponseMessage<int> response = new ResponseMessage<int>();
            try
            {
                response.Ext = await _deviceManage.GetParamTypeByChannelIDAsync(channelid);
            } catch(Exception e)
            {
                if(e is SobeyRecException se)//sobeyexcep会自动打印错误
                {
                    response.Code = se.ErrorCode.ToString();
                    response.Msg = se.Message;
                } else
                {
                    response.Code = ResponseCodeDefines.ServiceError;
                    response.Msg = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}：error info:{e.Message}";
                    Logger.Error(response.Msg);
                }
            }
            return response;
        }

        /// <summary>
        /// 根据 信号Id 获取是高清还是标清
        /// </summary>
        /// <remarks>原方法 GetParamTypeBySignalID</remarks>
        /// <param name="signalid">信号Id</param>
        /// <returns>0标清,1高清</returns>
        [HttpGet("signalparamtype/type/{signalid}")]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<int>> ParamTypeBySignalID([FromRoute, BindRequired, DefaultValue(14)]int signalid)
        {
            ResponseMessage<int> response = new ResponseMessage<int>();
            try
            {
                response.Ext = await _deviceManage.GetParamTypeBySignalIDAsync(signalid);
            } catch(Exception e)
            {
                if(e is SobeyRecException se)//sobeyexcep会自动打印错误
                {
                    response.Code = se.ErrorCode.ToString();
                    response.Msg = se.Message;
                } else
                {
                    response.Code = ResponseCodeDefines.ServiceError;
                    response.Msg = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}：error info:{e.Message}";
                    Logger.Error(response.Msg);
                }
            }
            return response;
        }

        #endregion ParamTyp

        #region ProgrammeInfo

        /// <summary>
        /// 根据 通道Id 获取相应的节目，有矩阵模式和无矩阵模式的区别
        /// </summary>
        /// <remarks>原方法 GetProgrammeInfosByChannelId</remarks>
        /// <param name="channelid">通道Id</param>
        /// <returns>节目集合</returns>
        [HttpGet("programmeinfo/{channelid}")]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<List<ProgrammeInfo>>> ProgrammeInfos([FromRoute, BindRequired, DefaultValue(14)]int channelid)
        {
            ResponseMessage<List<ProgrammeInfo>> response = new ResponseMessage<List<ProgrammeInfo>>();
            try
            {
                response.Ext = await _deviceManage.GetProgrammeInfosByChannelIdAsync(channelid);
            } catch(Exception e)
            {
                if(e is SobeyRecException se)//sobeyexcep会自动打印错误
                {
                    response.Code = se.ErrorCode.ToString();
                    response.Msg = se.Message;
                } else
                {
                    response.Code = ResponseCodeDefines.ServiceError;
                    response.Msg = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}：error info:{e.Message}";
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
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<List<ProgrammeInfo>>> AllProgrammeInfos()
        {
            ResponseMessage<List<ProgrammeInfo>> response = new ResponseMessage<List<ProgrammeInfo>>();
            try
            {
                response.Ext = await _deviceManage.GetAllProgrammeInfosAsync();
            } catch(Exception e)
            {
                if(e is SobeyRecException se)//sobeyexcep会自动打印错误
                {
                    response.Code = se.ErrorCode.ToString();
                    response.Msg = se.Message;
                } else
                {
                    response.Code = ResponseCodeDefines.ServiceError;
                    response.Msg = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}：error info:{e.Message}";
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
        [HttpGet("signalinfo/backsignal/{mastersignalid}")]
        //device有点特殊，做了监听端口的所以不能全类检验
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<ProgrammeInfoResponse>> GetBackProgramInfoBySrgid([FromRoute, BindRequired, DefaultValue(20)]int mastersignalid)
        {
            var Response = new ResponseMessage<ProgrammeInfoResponse>();

            try
            {
                Response.Ext = await _deviceManage.GetBackProgramInfoBySrgid(mastersignalid);
            } catch(Exception e)
            {
                if(e.GetType() == typeof(SobeyRecException))//sobeyexcep会自动打印错误
                {
                    SobeyRecException se = e as SobeyRecException;
                    Response.Code = se.ErrorCode.ToString();
                    Response.Msg = se.Message;
                } else
                {
                    Response.Code = ResponseCodeDefines.ServiceError;
                    Response.Msg = "GetBackProgramInfoBySrgid error info：" + e.ToString();
                    Logger.Error(Response.Msg);
                }
            }
            return Response;
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks>
        ///
        /// </remarks>
        /// GetAllChannelUnitMap
        [HttpGet("signalinfo/signal/{signalid}")]
        //device有点特殊，做了监听端口的所以不能全类检验
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<ProgrammeInfoResponse>> GetProgramInfoBySrgid([FromRoute, BindRequired, DefaultValue(40)]int signalid)
        {
            var Response = new ResponseMessage<ProgrammeInfoResponse>();

            try
            {
                Response.Ext = await _deviceManage.GetProgrammeInfoByIdAsync(signalid);
            } catch(Exception e)
            {
                if(e.GetType() == typeof(SobeyRecException))//sobeyexcep会自动打印错误
                {
                    SobeyRecException se = e as SobeyRecException;
                    Response.Code = se.ErrorCode.ToString();
                    Response.Msg = se.Message;
                } else
                {
                    Response.Code = ResponseCodeDefines.ServiceError;
                    Response.Msg = "GetProgramInfoBySrgid error info：" + e.ToString();
                    Logger.Error(Response.Msg);
                }
            }
            return Response;
        }

        #endregion ProgrammeInfo

        #region Router

        /// <summary>
        /// 获取输入端口与信息
        /// </summary>
        /// <remarks>原方法 AllRouterInPortInfos</remarks>
        /// <returns>输入端口的集合</returns>
        [HttpGet("routerinport/all")]
        //device有点特殊，做了监听端口的所以不能全类检验
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<List<RouterInResponse>>> AllRouterInPortInfos()
        {
            ResponseMessage<List<RouterInResponse>> response = new ResponseMessage<List<RouterInResponse>>();
            try
            {
                response.Ext = await _deviceManage.GetAllRouterInPortAsync<RouterInResponse>();
            } catch(Exception e)
            {
                if(e is SobeyRecException se)//sobeyexcep会自动打印错误
                {
                    response.Code = se.ErrorCode.ToString();
                    response.Msg = se.Message;
                } else
                {
                    response.Code = ResponseCodeDefines.ServiceError;
                    response.Msg = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}：error info:{e.Message}";
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
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<List<RoterOutResponse>>> AllRouterOutPortInfos()
        {
            ResponseMessage<List<RoterOutResponse>> response = new ResponseMessage<List<RoterOutResponse>>();
            try
            {
                response.Ext = await _deviceManage.GetAllRouterOutPortAsync<RoterOutResponse>();
            } catch(Exception e)
            {
                if(e is SobeyRecException se)//sobeyexcep会自动打印错误
                {
                    response.Code = se.ErrorCode.ToString();
                    response.Msg = se.Message;
                } else
                {
                    response.Code = ResponseCodeDefines.ServiceError;
                    response.Msg = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}：error info:{e.Message}";
                    Logger.Error(response.Msg);
                }
            }
            return response;
        }

        #endregion Router

        #region Signal

        /// <summary>
        /// 获取所有信号源和采集设备的对应
        /// </summary>
        /// <remarks>原方法 GetAllSignalDeviceMap</remarks>
        /// <returns>信号源和设备的Map</returns>
        [HttpGet("signaldevicemap/all")]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<List<SignalDeviceMapResponse>>> AllSignalDeviceMap()
        {
            ResponseMessage<List<SignalDeviceMapResponse>> response = new ResponseMessage<List<SignalDeviceMapResponse>>();
            try
            {
                response.Ext = await _deviceManage.GetAllSignalDeviceMapAsync<SignalDeviceMapResponse>();
            } catch(Exception e)
            {
                if(e is SobeyRecException se)//sobeyexcep会自动打印错误
                {
                    response.Code = se.ErrorCode.ToString();
                    response.Msg = se.Message;
                } else
                {
                    response.Code = ResponseCodeDefines.ServiceError;
                    response.Msg = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}：error info:{e.Message}";
                    Logger.Error(response.Msg);
                }
            }
            return response;
        }

        /// <summary>
        /// 获取指定信号源和采集设备的对应
        /// </summary>
        /// <remarks>原方法 GetSignalDeviceMapBySignalID</remarks>
        /// <param name="signalid">信号ID</param>
        /// <returns>信号源和设备的Map</returns>
        [HttpGet("signaldevicemap/{signalid}")]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<SignalDeviceMapResponse>> SignalDeviceMap([FromRoute, BindRequired, DefaultValue(39)]int signalid)
        {
            ResponseMessage<SignalDeviceMapResponse> response = new ResponseMessage<SignalDeviceMapResponse>();
            try
            {
                response.Ext = await _deviceManage.GetSignalDeviceMapBySignalID<SignalDeviceMapResponse>(signalid);
            } catch(Exception e)
            {
                if(e is SobeyRecException se)//sobeyexcep会自动打印错误
                {
                    response.Code = se.ErrorCode.ToString();
                    response.Msg = se.Message;
                } else
                {
                    response.Code = ResponseCodeDefines.ServiceError;
                    response.Msg = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}：error info:{e.Message}";
                    Logger.Error(response.Msg);
                }
            }
            return response;
        }

        /// <summary>
        /// 设置信号源和采集设备的对应
        /// </summary>
        /// <remarks>原方法 GetSignalDeviceMapBySignalID</remarks>
        /// <param name="data">保存的对象</param>
        /// <returns>是否成功</returns>
        [HttpPost("signaldevicemap")]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<bool>> UpdateSignalDeviceMap([FromBody, BindRequired]SignalDeviceRequest data)
        {
            ResponseMessage<bool> response = new ResponseMessage<bool>();
            try
            {
                response.Ext = await _deviceManage.SaveSignalDeviceMapAsync(data.SignalID,
                                                                            data.DeviceID,
                                                                            data.OutPortIdx,
                                                                            data.SignalSource);
            } catch(Exception e)
            {
                if(e is SobeyRecException se)//sobeyexcep会自动打印错误
                {
                    response.Code = se.ErrorCode.ToString();
                    response.Msg = se.Message;
                } else
                {
                    response.Code = ResponseCodeDefines.ServiceError;
                    response.Msg = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}：error info:{e.Message}";
                    Logger.Error(response.Msg);
                }
            }
            return response;
        }

        /// <summary>
        /// 获得所有信号源分组
        /// </summary>
        /// <remarks>原方法 GetAllSignalGroup</remarks>
        /// <returns>信号分组信息集合</returns>
        [HttpGet("signalgroup/all")]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<List<SignalGroupResponse>>> AllSignalGroup()
        {
            ResponseMessage<List<SignalGroupResponse>> response = new ResponseMessage<List<SignalGroupResponse>>();
            try
            {
                response.Ext = await _deviceManage.GetAllSignalGroupAsync<SignalGroupResponse>();
            } catch(Exception e)
            {
                if(e is SobeyRecException se)//sobeyexcep会自动打印错误
                {
                    response.Code = se.ErrorCode.ToString();
                    response.Msg = se.Message;
                } else
                {
                    response.Code = ResponseCodeDefines.ServiceError;
                    response.Msg = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}：error info:{e.Message}";
                    Logger.Error(response.Msg);
                }
            }
            return response;
        }

        /// <summary>
        /// 获取所有信号源分组状态信息
        /// </summary>
        /// <remarks>原方法 GetAllSignalGroupInfo</remarks>
        /// <returns>信号分组状态集合</returns>
        [HttpGet("signalgroup/withsignalid/all")]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<List<SignalGroupStateResponse>>> AllSignalGroupInfos()
        {
            ResponseMessage<List<SignalGroupStateResponse>> response = new ResponseMessage<List<SignalGroupStateResponse>>();
            try
            {
                response.Ext = await _deviceManage.GetAllSignalGroupInfoAsync<SignalGroupStateResponse>();
            } catch(Exception e)
            {
                if(e is SobeyRecException se)//sobeyexcep会自动打印错误
                {
                    response.Code = se.ErrorCode.ToString();
                    response.Msg = se.Message;
                } else
                {
                    response.Code = ResponseCodeDefines.ServiceError;
                    response.Msg = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}：error info:{e.Message}";
                    Logger.Error(response.Msg);
                }
            }
            return response;
        }

        /// <summary>
        /// 查询所有信号源的扩展信息
        /// </summary>
        /// <remarks>原方法 GetAllSignalSrcExs</remarks>
        /// <returns>信号源的扩展信息集合</returns>
        [HttpGet("signalsrcex/all")]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<List<SignalSrcExResponse>>> AllSignalSrcExs()
        {
            ResponseMessage<List<SignalSrcExResponse>> response = new ResponseMessage<List<SignalSrcExResponse>>();
            try
            {
                response.Ext = await _deviceManage.GetAllSignalSrcExsAsync<SignalSrcExResponse>();
            } catch(Exception e)
            {
                if(e is SobeyRecException se)//sobeyexcep会自动打印错误
                {
                    response.Code = se.ErrorCode.ToString();
                    response.Msg = se.Message;
                } else
                {
                    response.Code = ResponseCodeDefines.ServiceError;
                    response.Msg = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}：error info:{e.Message}";
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
        [HttpGet("signalsrc/all")]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<List<SignalSrcResponse>>> AllSignalSrcs()
        {
            ResponseMessage<List<SignalSrcResponse>> response = new ResponseMessage<List<SignalSrcResponse>>();
            try
            {
                response.Ext = await _deviceManage.GetAllSignalSrcsAsync<SignalSrcResponse>();
            } catch(Exception e)
            {
                if(e is SobeyRecException se)//sobeyexcep会自动打印错误
                {
                    response.Code = se.ErrorCode.ToString();
                    response.Msg = se.Message;
                } else
                {
                    response.Code = ResponseCodeDefines.ServiceError;
                    response.Msg = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}：error info:{e.Message}";
                    Logger.Error(response.Msg);
                }
            }
            return response;
        }

        /// <summary>
        /// 获取该信号源的备份信号源ID
        /// </summary>
        /// <remarks>原方法 GetBackupSignalSrcInfo</remarks>
        /// <param name="signalsrcid">信号源ID</param>
        /// <returns>备份信号源ID</returns>
        [HttpGet("signalsrc/backupid/{signalsrcid}")]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<int>> BackupSignalSrcInfo([FromRoute, BindRequired, DefaultValue(0)]int signalsrcid)
        {
            ResponseMessage<int> response = new ResponseMessage<int>();
            try
            {
                response.Ext = await _deviceManage.GetBackupSignalSrcIdByIdAsync(signalsrcid);
            } catch(Exception e)
            {
                if(e is SobeyRecException se)//sobeyexcep会自动打印错误
                {
                    response.Code = se.ErrorCode.ToString();
                    response.Msg = se.Message;
                } else
                {
                    response.Code = ResponseCodeDefines.ServiceError;
                    response.Msg = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}：error info:{e.Message}";
                    Logger.Error(response.Msg);
                }
            }
            return response;
        }

        /// <summary>
        /// 根据 信号源Id 查询信号源是否是备份信号源
        /// </summary>
        /// <remarks>原方法 GetIsBackupSignalSrcByID</remarks>
        /// <param name="signalsrcid">信号源ID</param>
        /// <returns>是否是备份信号源</returns>
        [HttpGet("signalsrc/isbackup/{signalsrcid}")]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<bool>> IsBackupSignalSrc([FromRoute, BindRequired, DefaultValue(39)]int signalsrcid)
        {
            ResponseMessage<bool> response = new ResponseMessage<bool>();
            try
            {
                response.Ext = await _deviceManage.IsBackupSignalSrcByIdAsync(signalsrcid);
            } catch(Exception e)
            {
                if(e is SobeyRecException se)//sobeyexcep会自动打印错误
                {
                    response.Code = se.ErrorCode.ToString();
                    response.Msg = se.Message;
                } else
                {
                    response.Code = ResponseCodeDefines.ServiceError;
                    response.Msg = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}：error info:{e.Message}";
                    Logger.Error(response.Msg);
                }
            }
            return response;
        }

        #endregion Signal

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
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<List<CaptureChannelInfoResponse>>> ChannelsByProgrammeId([FromRoute, BindRequired, DefaultValue(39)]int programmeid,
                                                                                                   [FromQuery, BindRequired, DefaultValue("0")]int status)
        {
            ResponseMessage<List<CaptureChannelInfoResponse>> response = new ResponseMessage<List<CaptureChannelInfoResponse>>();
            try
            {
                response.Ext = await _deviceManage.GetChannelsByProgrammeIdAsync<CaptureChannelInfoResponse>(programmeid,
                                                                                                             status);
            } catch(Exception e)
            {
                if(e is SobeyRecException se)//sobeyexcep会自动打印错误
                {
                    response.Code = se.ErrorCode.ToString();
                    response.Msg = se.Message;
                } else
                {
                    response.Code = ResponseCodeDefines.ServiceError;
                    response.Msg = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}：error info:{e.Message}";
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
        [HttpGet("signalinfo/id/{channelid}")]
        //device有点特殊，做了监听端口的所以不能全类检验
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<int>> GetChannelSignalSrc([FromRoute, BindRequired, DefaultValue(16)]int channelid)
        {
            ResponseMessage<int> response = new ResponseMessage<int>();
            try
            {
                response.Ext = await _deviceManage.GetChannelSignalSrcAsync(channelid);
            } catch(Exception e)
            {
                if(e is SobeyRecException se)//sobeyexcep会自动打印错误
                {
                    response.Code = se.ErrorCode.ToString();
                    response.Msg = se.Message;
                } else
                {
                    response.Code = ResponseCodeDefines.ServiceError;
                    response.Msg = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}：error info:{e.Message}";
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
        [HttpGet("channelunitmap/all")]
        //device有点特殊，做了监听端口的所以不能全类检验
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<List<RecUnitMap>>> GetAllChannelUnitMap()
        {
            ResponseMessage<List<RecUnitMap>> response = new ResponseMessage<List<RecUnitMap>>();
            try
            {
                response.Ext = await _deviceManage.GetAllChannelUnitMap();
            } catch(Exception e)
            {
                if(e is SobeyRecException se)//sobeyexcep会自动打印错误
                {
                    response.Code = se.ErrorCode.ToString();
                    response.Msg = se.Message;
                } else
                {
                    response.Code = ResponseCodeDefines.ServiceError;
                    response.Msg = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}error info:{e.Message}";
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
        [HttpGet("channelunitmap/id/{channel}")]
        //device有点特殊，做了监听端口的所以不能全类检验
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<int>> GetChannelUnitMapID([FromRoute, BindRequired, DefaultValue(911)]int channel)
        {
            ResponseMessage<int> response = new ResponseMessage<int>();
            try
            {
                var f = await _deviceManage.GetChannelUnitMap(channel);
                if(f != null)
                {
                    response.Ext = f.UnitID;
                } else
                    response.Ext = -1;
            } catch(Exception e)
            {
                if(e is SobeyRecException se)//sobeyexcep会自动打印错误
                {
                    response.Code = se.ErrorCode.ToString();
                    response.Msg = se.Message;
                } else
                {
                    response.Code = ResponseCodeDefines.ServiceError;
                    response.Msg = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}error info:{e.Message}";
                    Logger.Error(response.Msg);
                }
            }
            return response;
        }
    }
}