using IngestDBCore;
using IngestDBCore.Basic;
using IngestDevicePlugin.Dto;
using IngestDevicePlugin.Managers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Sobey.Core.Log;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using IngestDevicePlugin.Dto.Request;
using SignalDeviceRequest = IngestDevicePlugin.Dto.SignalDeviceMap;

namespace IngestDevicePlugin.Controllers
{


    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    [ApiController]
    public partial class DeviceController : ControllerBase
    {
        private readonly ILogger Logger = LoggerManager.GetLogger("DeviceInfo");
        private readonly DeviceManager _deviceManage;
        //private readonly RestClient _restClient;

        public DeviceController(DeviceManager task)
        {
            _deviceManage = task;
        }
        /// <summary>
        /// 监听接口 /get/
        /// </summary>
        /// <returns></returns>
        [HttpGet("Get")]
        [ApiExplorerSettings(GroupName = "v2")]
        public string Get()
        {

            return "DBPlatform Service is already startup at " + DateTime.Now.ToString();
        }

        #region Capture
        /// <summary>获取所有采集通道</summary>
        /// <remarks>原方法 GetAllCaptureChannels</remarks>
        /// <returns>采集通道集合</returns>
        [HttpGet("CaptureChannel/All")]
        [IngestAuthentication]//device有点特殊，做了监听端口的所以不能全类检验
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<List<CaptureChannelInfo>>> AllCaptureChannels()
        {
            ResponseMessage<List<CaptureChannelInfo>> response = new ResponseMessage<List<CaptureChannelInfo>>();
            try
            {
                response.Ext = await _deviceManage.GetAllCaptureChannelsAsync();
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
                    response.Msg = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}：error info:{e}";
                    Logger.Error(response.Msg);
                }
            }
            return response;
        }

        /// <summary>通过 通道ID 获取采集通道</summary>
        /// <remarks>原方法 GetCaptureChannelByID</remarks>
        /// <param name="nChannelID">通道ID</param>
        /// <returns>采集通道</returns>
        [HttpGet("CaptureChannel/{nChannelID}")]
        [IngestAuthentication]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<CaptureChannelInfo>> CaptureChannelByID([FromRoute, BindRequired]int nChannelID)
        {
            ResponseMessage<CaptureChannelInfo> response = new ResponseMessage<CaptureChannelInfo>();
            try
            {
                response.Ext = await _deviceManage.GetCaptureChannelByIDAsync(nChannelID);
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
                    response.Msg = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}：error info:{e}";
                    Logger.Error(response.Msg);
                }
            }
            return response;
        }

        /// <summary>获取所有的采集设备信息</summary>
        /// <remarks>原方法 GetAllCaptureDevices</remarks>
        /// <returns>采集设备集合</returns>
        [HttpGet("CaptureDevice/All")]
        [IngestAuthentication]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<List<CaptureDeviceInfo>>> AllCaptureDevices()
        {
            ResponseMessage<List<CaptureDeviceInfo>> response = new ResponseMessage<List<CaptureDeviceInfo>>();
            try
            {
                response.Ext = await _deviceManage.GetAllCaptureDevicesAsync<CaptureDeviceInfo>();
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
                    response.Msg = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}：error info:{e}";
                    Logger.Error(response.Msg);
                }
            }
            return response;
        }

        /// <summary>根据 信号源Id 获取绑定的采集参数</summary>
        /// <remarks>原方法 GetCaptureTemplateIDBySignalID</remarks>
        /// <returns>采集参数</returns>
        [HttpGet("CaptureTemplate/Id/{nSignalID}")]
        [IngestAuthentication]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<int>> CaptureTemplateId([FromRoute, BindRequired]int nSignalID)
        {
            ResponseMessage<int> response = new ResponseMessage<int>();
            try
            {
                response.Ext = await _deviceManage.GetSignalCaptureTemplateAsync(nSignalID);
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
                    response.Msg = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}：error info:{e}";
                    Logger.Error(response.Msg);
                }
            }
            return response;
        }
        #endregion

        #region Channel
        /// <summary>根据 信号源Id 为信号源选择一个合适的预监通道</summary>
        /// <remarks>原方法 GetBestPreviewChannelForSignal</remarks>
        /// <returns>预监通道Id</returns>
        [HttpGet("BestPreviewChannel/Id/{nSignalID}")]
        [IngestAuthentication]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<int>> BestPreviewChannelId([FromRoute, BindRequired]int nSignalID)
        {
            ResponseMessage<int> response = new ResponseMessage<int>();
            try
            {
                response.Ext = await _deviceManage.GetBestPreviewChnForSignalAsync(nSignalID);
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
                    response.Msg = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}：error info:{e}";
                    Logger.Error(response.Msg);
                }
            }
            return response;
        }

        /// <summary> 更新通道的扩展数据 </summary>
        /// <remarks>原方法 PostUpdateChnExtData</remarks>
        /// <returns>是否成功</returns>
        [HttpPost("Channel/ExtendData/{nChnID}")]
        [IngestAuthentication]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<bool>> UpdateChnExtData([FromQuery, BindRequired]int nChnID, [FromBody, BindRequired]DeviceChannelExtdataRequest data)
        {
            ResponseMessage<bool> response = new ResponseMessage<bool>();
            try
            {
                response.Ext = await _deviceManage.SaveChnExtenddataAsync(nChnID, data.Datatype, data.Extenddata);
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
                    response.Msg = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}：error info:{e}";
                    Logger.Error(response.Msg);
                }
            }
            return response;
        }

        /// <summary>根据 信号源Id,用户Code 自动匹配最优通道</summary>
        /// <remarks>原方法 GetBestChannelIDBySignalID</remarks>
        /// <param name="nSignalID">信号源Id</param>
        /// <param name="strUserCode">用户Code</param>
        /// <returns>最优通道Id</returns>
        [HttpGet("BestChannel/Id/{nSignalID}")]
        [IngestAuthentication]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<int>> BestChannelId([FromRoute, BindRequired]int nSignalID, [FromQuery, BindRequired]string strUserCode)
        {
            ResponseMessage<int> response = new ResponseMessage<int>();
            try
            {
                response.Ext = await _deviceManage.GetBestChannelIdBySignalIDAsync(nSignalID, strUserCode);
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
                    response.Msg = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}：error info:{e}";
                    Logger.Error(response.Msg);
                }
            }
            return response;
        }

        /// <summary>根据 programmeId 获取相应的通道，有矩阵模式和无矩阵模式的区别</summary>
        /// <remarks>原方法 GetChannelsByProgrammeId</remarks>
        /// <param name="programmeId">programmeId</param>
        /// <returns>采集通道集合</returns>
        [HttpGet("Channel/{programmeId}")]
        [IngestAuthentication]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<List<CaptureChannelInfo>>> Channels([FromRoute, BindRequired]int programmeId)
        {
            ResponseMessage<List<CaptureChannelInfo>> response = new ResponseMessage<List<CaptureChannelInfo>>();
            try
            {
                response.Ext = await _deviceManage.GetChannelsByProgrammeIdAsync<CaptureChannelInfo>(programmeId);
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
                    response.Msg = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}：error info:{e}";
                    Logger.Error(response.Msg);
                }
            }
            return response;
        }

        /// <summary> 获得所有通道的状态 </summary>
        /// <remarks>原方法 GetAllChannelState</remarks>
        /// <returns>最优通道Id</returns>
        [HttpGet("ChannelState/All")]
        [IngestAuthentication]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<List<MSVChannelState>>> AllChannelState()
        {
            ResponseMessage<List<MSVChannelState>> response = new ResponseMessage<List<MSVChannelState>>();
            try
            {
                response.Ext = await _deviceManage.GetAllChannelStateAsync();
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
                    response.Msg = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}：error info:{e}";
                    Logger.Error(response.Msg);
                }
            }
            return response;
        }

        /// <summary>更改MSV设备状态信息</summary>
        /// <remarks>原方法 GetModifyDevState</remarks>
        /// <param name="request">更新的对象</param>
        /// <returns>是否成功</returns>
        [HttpPost("DevState")]
        [IngestAuthentication]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<bool>> GetModifyDevState([FromBody, BindRequired] DeviceMSVChannelStateRequest request)
        {
            ResponseMessage<bool> response = new ResponseMessage<bool>();
            try
            {
                response.Ext = await _deviceManage.UpdateMSVChannelStateAsync(request.nID, request.nDevState, request.nMSVMode);
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
                    response.Msg = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}：error info:{e}";
                    Logger.Error(response.Msg);
                }
            }
            return response;
        }
        #endregion

        #region Device

        /// <summary>获得所有的IP收录的设备</summary>
        /// <remarks>原方法 GetAllTSDeviceInfos</remarks>
        /// <returns>最优通道Id</returns>
        [HttpGet("TSDeviceInfo/All")]
        [IngestAuthentication]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<List<TSDeviceInfo>>> AllGTSDeviceInfos()
        {
            ResponseMessage<List<TSDeviceInfo>> response = new ResponseMessage<List<TSDeviceInfo>>();
            try
            {
                response.Ext = await _deviceManage.GetAllTSDeviceInfosAsync();
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
                    response.Msg = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}：error info:{e}";
                    Logger.Error(response.Msg);
                }
            }
            return response;
        }

        /// <summary>更新所有的IP收录的设备</summary>
        /// <remarks>原方法 PostUpdateAllTSDeviceInfos</remarks>
        /// <param name="deviceInfos">更新的对象</param>
        /// <returns>是否成功</returns>
        [HttpPost("TSDeviceInfo/All")]
        [IngestAuthentication]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<bool>> PostUpdateAllTSDeviceInfos([FromBody, BindRequired]DeviceTSDeviceInfoRequest deviceInfos)
        {
            ResponseMessage<bool> response = new ResponseMessage<bool>();
            try
            {
                response.Ext = await _deviceManage.UpdateAllTSDeviceInfosAsync(deviceInfos.deviceInfos.ToArray());
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
                    response.Msg = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}：error info:{e}";
                    Logger.Error(response.Msg);
                }
            }
            return response;
        }

        /// <summary>获取所有GPI设备</summary>
        /// <remarks>原方法 GetAllGPIDevices</remarks>
        /// <returns>GPI设备集合</returns>
        [HttpGet("GPIDevices/All")]
        [IngestAuthentication]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<List<GPIDeviceInfo>>> AllGPIDevices()
        {
            ResponseMessage<List<GPIDeviceInfo>> response = new ResponseMessage<List<GPIDeviceInfo>>();
            try
            {
                response.Ext = await _deviceManage.GetAllGPIInfoAsync<GPIDeviceInfo>();
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
                    response.Msg = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}：error info:{e}";
                    Logger.Error(response.Msg);
                }
            }
            return response;
        }

        /// <summary> 通过 GPIID 找出该GPI所有的映射 </summary>
        /// <remarks>原方法 GetAllGPIDevices</remarks>
        /// <returns>GPI设备集合</returns>
        [HttpGet("GPIMapInfo/{nGPIID}")]
        [IngestAuthentication]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<List<GPIDeviceMapInfo>>> GPIMapInfo([FromRoute, BindRequired]int nGPIID)
        {
            ResponseMessage<List<GPIDeviceMapInfo>> response = new ResponseMessage<List<GPIDeviceMapInfo>>();
            try
            {
                response.Ext = await _deviceManage.GetGPIMapInfoByGPIIDAsync<GPIDeviceMapInfo>(nGPIID);
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
                    response.Msg = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}：error info:{e}";
                    Logger.Error(response.Msg);
                }
            }
            return response;
        }

        /// <summary> 根据 通道Id 获取MSV设备状态信息 </summary>
        /// <remarks>原方法 GetMSVChannelState</remarks>
        /// <param name="nChannleID">通道Id</param>
        /// <returns>GPI设备集合</returns>
        [HttpGet("MSVChannelState/{nChannleID}")]
        [IngestAuthentication]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<MSVChannelState>> MSVChannelState([FromRoute, BindRequired]int nChannleID)
        {
            ResponseMessage<MSVChannelState> response = new ResponseMessage<MSVChannelState>();
            try
            {
                response.Ext = await _deviceManage.GetMsvChannelStateAsync(nChannleID);
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
                    response.Msg = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}：error info:{e}";
                    Logger.Error(response.Msg);
                }
            }
            return response;
        }

        /// <summary>更改ModifySourceVTRIDAndUserCodeByChannelIDArray</summary>
        /// <remarks>原方法 ModifySourceVTRIDAndUserCodeByChannelIDArray</remarks>
        /// <param name="request">更新对象</param>
        /// <returns>是否成功</returns>
        [HttpPost("ChannelState/UserCode/VTRID")]
        [IngestAuthentication]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<bool>> ModifySourceVTRIDAndUserCodeByChannelIDArray([FromBody, BindRequired]DeviceMSVVTRAndUserCodeRequest request)
        {
            ResponseMessage<bool> response = new ResponseMessage<bool>();
            try
            {
                response.Ext = await _deviceManage.ModifySourceVTRIDAndUserCodeAsync(request.nSourceVTRID, request.userCode, request.nIDArray);
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
                    response.Msg = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}：error info:{e}";
                    Logger.Error(response.Msg);
                }
            }
            return response;
        }
        #endregion

        #region ParamTyp

        /// <summary> 根据 通道Id 获取高清还是标清 nType:0标清,1高清 </summary>
        /// <remarks>原方法 GetParamTypeByChannleID</remarks>
        /// <param name="nChannelID">通道Id</param>
        /// <returns>0标清,1高清</returns>
        [HttpGet("ParamType/Type")]
        [IngestAuthentication]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<int>> ParamTypeByChannleID([FromQuery, BindRequired]int nChannelID)
        {
            ResponseMessage<int> response = new ResponseMessage<int>();
            try
            {
                response.Ext = await _deviceManage.GetParamTypeByChannleIDAsync(nChannelID);
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
                    response.Msg = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}：error info:{e}";
                    Logger.Error(response.Msg);
                }
            }
            return response;
        }

        /// <summary>根据 信号Id 获取是高清还是标清</summary>
        /// <remarks>原方法 GetParamTypeBySignalID</remarks>
        /// <param name="nSignalID">信号Id</param>
        /// <returns>0标清,1高清</returns>
        [HttpGet("ParamType/Type")]
        [IngestAuthentication]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<int>> ParamTypeBySignalID([FromQuery, BindRequired]int nSignalID)
        {
            ResponseMessage<int> response = new ResponseMessage<int>();
            try
            {
                response.Ext = await _deviceManage.GetParamTypeBySignalIDAsync(nSignalID);
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
                    response.Msg = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}：error info:{e}";
                    Logger.Error(response.Msg);
                }
            }
            return response;
        }

        #endregion

        #region ProgrammeInfo

        /// <summary>根据 通道Id 获取相应的节目，有矩阵模式和无矩阵模式的区别</summary>
        /// <remarks>原方法 GetProgrammeInfosByChannelId</remarks>
        /// <param name="nChannelID">通道Id</param>
        /// <returns>节目集合</returns>
        [HttpGet("ProgrammeInfo/{nChannelID}")]
        [IngestAuthentication]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<List<ProgrammeInfo>>> ProgrammeInfos([FromRoute, BindRequired]int nChannelID)
        {
            ResponseMessage<List<ProgrammeInfo>> response = new ResponseMessage<List<ProgrammeInfo>>();
            try
            {
                response.Ext = await _deviceManage.GetProgrammeInfosByChannelIdAsync(nChannelID);
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
                    response.Msg = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}：error info:{e}";
                    Logger.Error(response.Msg);
                }
            }
            return response;
        }

        /// <summary> 获取所有节目 </summary>
        /// <remarks>原方法 GetAllProgrammeInfos</remarks>
        /// <returns>节目集合</returns>
        [HttpGet("Programme/All")]
        [IngestAuthentication]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<List<ProgrammeInfo>>> AllProgrammeInfos()
        {
            ResponseMessage<List<ProgrammeInfo>> response = new ResponseMessage<List<ProgrammeInfo>>();
            try
            {
                response.Ext = await _deviceManage.GetAllProgrammeInfosAsync();
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
                    response.Msg = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}：error info:{e}";
                    Logger.Error(response.Msg);
                }
            }
            return response;
        }

        #endregion

        #region Router

        /// <summary>获取输入端口与信息</summary>
        /// <remarks>原方法 AllRouterInPortInfos</remarks>
        /// <returns>输入端口的集合</returns>
        [HttpGet("RouterInPort/All")]
        [IngestAuthentication]//device有点特殊，做了监听端口的所以不能全类检验
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<List<RouterInResponse>>> AllRouterInPortInfos()
        {
            ResponseMessage<List<RouterInResponse>> response = new ResponseMessage<List<RouterInResponse>>();
            try
            {
                response.Ext = await _deviceManage.GetAllRouterInPortAsync<RouterInResponse>();
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
                    response.Msg = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}：error info:{e}";
                    Logger.Error(response.Msg);
                }
            }
            return response;
        }

        /// <summary> 获取输出端口与信号源的映射 </summary>
        /// <remarks>原方法 AllRouterOutPortInfos</remarks>
        /// <returns>输出端口的集合</returns>
        [HttpGet("RouterOutPort/All")]
        [IngestAuthentication]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<List<RoterOutDesc>>> AllRouterOutPortInfos()
        {
            ResponseMessage<List<RoterOutDesc>> response = new ResponseMessage<List<RoterOutDesc>>();
            try
            {
                response.Ext = await _deviceManage.GetAllRouterOutPortAsync<RoterOutDesc>();
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
                    response.Msg = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}：error info:{e}";
                    Logger.Error(response.Msg);
                }
            }
            return response;
        }

        #endregion

        #region Signal

        /// <summary> 获取所有信号源和采集设备的对应 </summary>
        /// <remarks>原方法 GetAllSignalDeviceMap</remarks>
        /// <returns>信号源和设备的Map</returns>
        [HttpGet("SignalDeviceMaps/All")]
        [IngestAuthentication]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<List<SignalDeviceMap>>> AllSignalDeviceMap()
        {
            ResponseMessage<List<SignalDeviceMap>> response = new ResponseMessage<List<SignalDeviceMap>>();
            try
            {
                response.Ext = await _deviceManage.GetAllSignalDeviceMapAsync<SignalDeviceMap>();
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
                    response.Msg = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}：error info:{e}";
                    Logger.Error(response.Msg);
                }
            }
            return response;
        }

        /// <summary> 获取指定信号源和采集设备的对应 </summary>
        /// <remarks>原方法 GetSignalDeviceMapBySignalID</remarks>
        /// <param name="nSignalID">信号ID</param>
        /// <returns>信号源和设备的Map</returns>
        [HttpGet("SignalDeviceMap/BySignalID")]
        [IngestAuthentication]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<SignalDeviceMap>> SignalDeviceMap([FromRoute, BindRequired]int nSignalID)
        {
            ResponseMessage<SignalDeviceMap> response = new ResponseMessage<SignalDeviceMap>();
            try
            {
                response.Ext = await _deviceManage.GetSignalDeviceMapBySignalID(nSignalID);
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
                    response.Msg = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}：error info:{e}";
                    Logger.Error(response.Msg);
                }
            }
            return response;
        }

        /// <summary> 设置信号源和采集设备的对应 </summary>
        /// <remarks>原方法 GetSignalDeviceMapBySignalID</remarks>
        /// <param name="request">设置的对象</param>
        /// <returns>是否成功</returns>
        [HttpPost("GetSetSignalDeviceMap")]
        [IngestAuthentication]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<bool>> UpdateSignalDeviceMap([FromBody, BindRequired]SignalDeviceRequest request)
        {
            ResponseMessage<bool> response = new ResponseMessage<bool>();
            try
            {
                response.Ext = await _deviceManage.SaveSignalDeviceMapAsync(request.nSignalID, request.nDeviceID, request.nOutPortIdx, request.SignalSource);
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
                    response.Msg = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}：error info:{e}";
                    Logger.Error(response.Msg);
                }
            }
            return response;
        }

        /// <summary> 获得所有信号源分组 </summary>
        /// <remarks>原方法 GetAllSignalGroup</remarks>
        /// <returns>信号分组信息集合</returns>
        [HttpGet("SignalGroups/All")]
        [IngestAuthentication]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<List<AllSignalGroup>>> AllSignalGroup()
        {
            ResponseMessage<List<AllSignalGroup>> response = new ResponseMessage<List<AllSignalGroup>>();
            try
            {
                response.Ext = await _deviceManage.GetAllSignalGroupAsync();
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
                    response.Msg = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}：error info:{e}";
                    Logger.Error(response.Msg);
                }
            }
            return response;
        }

        /// <summary> 获取所有信号源分组状态信息 </summary>
        /// <remarks>原方法 GetAllSignalGroupInfo</remarks>
        /// <returns>信号分组状态集合</returns>
        [HttpGet("SignalGroupInfo/All")]
        [IngestAuthentication]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<List<SignalGroupState>>> AllSignalGroupInfos()
        {
            ResponseMessage<List<SignalGroupState>> response = new ResponseMessage<List<SignalGroupState>>();
            try
            {
                response.Ext = await _deviceManage.GetAllSignalGroupInfoAsync();
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
                    response.Msg = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}：error info:{e}";
                    Logger.Error(response.Msg);
                }
            }
            return response;
        }

        /// <summary> 查询所有信号源的扩展信息 </summary>
        /// <remarks>原方法 GetAllSignalSrcExs</remarks>
        /// <returns>信号源的扩展信息集合</returns>
        [HttpGet("SignalSrcExs/All")]
        [IngestAuthentication]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<List<SignalSrcExInfo>>> AllSignalSrcExs()
        {
            ResponseMessage<List<SignalSrcExInfo>> response = new ResponseMessage<List<SignalSrcExInfo>>();
            try
            {
                response.Ext = await _deviceManage.GetAllSignalSrcExsAsync();
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
                    response.Msg = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}：error info:{e}";
                    Logger.Error(response.Msg);
                }
            }
            return response;
        }

        /// <summary> 获取所有信号源 </summary>
        /// <remarks>原方法 GetAllSignalSrcs</remarks>
        /// <returns>信号源信息集合</returns>
        [HttpGet("SignalSrcs/All")]
        [IngestAuthentication]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<List<SignalSrcInfo>>> AllSignalSrcs()
        {
            ResponseMessage<List<SignalSrcInfo>> response = new ResponseMessage<List<SignalSrcInfo>>();
            try
            {
                response.Ext = await _deviceManage.GetAllSignalSrcsAsync();
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
                    response.Msg = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}：error info:{e}";
                    Logger.Error(response.Msg);
                }
            }
            return response;
        }

        /// <summary>获取该信号源的备份信号源ID</summary>
        /// <remarks>原方法 GetBackupSignalSrcInfo</remarks>
        /// <param name="nSignalSrcId">信号源ID</param>
        /// <returns>备份信号源ID</returns>
        [HttpGet("BackupSignalSrc/Id/{nSignalSrcId}")]
        [IngestAuthentication]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<int>> BackupSignalSrcInfo([FromRoute, BindRequired]int nSignalSrcId)
        {
            ResponseMessage<int> response = new ResponseMessage<int>();
            try
            {
                response.Ext = await _deviceManage.GetBackupSignalSrcIdByIdAsync(nSignalSrcId);
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
                    response.Msg = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}：error info:{e}";
                    Logger.Error(response.Msg);
                }
            }
            return response;
        }

        /// <summary> 根据 信号源Id 查询信号源是否是备份信号源 </summary>
        /// <remarks>原方法 GetIsBackupSignalSrcByID</remarks>
        /// <param name="nSignalSrcId">信号源ID</param>
        /// <returns>是否是备份信号源</returns>
        [HttpGet("SignalSrc/IsBackup/{nSignalSrcId}")]
        [IngestAuthentication]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<bool>> IsBackupSignalSrc([FromRoute, BindRequired]int nSignalSrcId)
        {
            ResponseMessage<bool> response = new ResponseMessage<bool>();
            try
            {
                response.Ext = await _deviceManage.IsBackupSignalSrcByIdAsync(nSignalSrcId);
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
                    response.Msg = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}：error info:{e}";
                    Logger.Error(response.Msg);
                }
            }
            return response;
        }
        #endregion

        #region MyRegion



        #endregion











        /// <summary>
        /// 根据节目ID获取相应的通道，有矩阵模式和无矩阵模式的区别
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="programmeId">信号源id</param>
        /// <param name="status">int 0是不选返回所有通道信息，1是选通道和msv连接正常的通道信息</param>
        /// <returns>当前信号源匹配通道，是list</returns>
        [HttpGet("programme/{programmeId}")]
        [IngestAuthentication]//device有点特殊，做了监听端口的所以不能全类检验
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<List<CaptureChannelInfoResponse>>> ChannelsByProgrammeId([FromRoute, BindRequired]int programmeId, [FromQuery, BindRequired]int status)
        {
            ResponseMessage<List<CaptureChannelInfoResponse>> response = new ResponseMessage<List<CaptureChannelInfoResponse>>();
            try
            {
                response.Ext = await _deviceManage.GetChannelsByProgrammeIdAsync<CaptureChannelInfoResponse>(programmeId, status);
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
                    response.Msg = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}：error info:{e}";
                    Logger.Error(response.Msg);
                }
            }
            return response;
        }

        /// <summary>
        /// 根据通道ID获取相应的信号源id
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="channelid">通道id</param>
        [HttpGet("signalinfo/id/{channelid}")]
        [IngestAuthentication]//device有点特殊，做了监听端口的所以不能全类检验
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<int>> GetChannelSignalSrc([FromRoute, BindRequired]int channelid)
        {
            ResponseMessage<int> response = new ResponseMessage<int>();
            try
            {
                response.Ext = await _deviceManage.GetChannelSignalSrcAsync(channelid);
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
                    response.Msg = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}：error info:{e}";
                    Logger.Error(response.Msg);
                }
            }
            return response;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// GetAllChannelUnitMap
        [HttpGet("channelunitmap/all")]
        [IngestAuthentication]//device有点特殊，做了监听端口的所以不能全类检验
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<List<RecUnitMap>>> GetAllChannelUnitMap()
        {
            ResponseMessage<List<RecUnitMap>> response = new ResponseMessage<List<RecUnitMap>>();
            try
            {
                response.Ext = await _deviceManage.GetAllChannelUnitMap();
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
                    response.Msg = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}error info:{e}";
                    Logger.Error(response.Msg);
                }
            }
            return response;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// GetAllChannelUnitMap
        [HttpGet("channelunitmap/id/{channel}")]
        [IngestAuthentication]//device有点特殊，做了监听端口的所以不能全类检验
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<int>> GetChannelUnitMapID([FromRoute, BindRequired]int channel)
        {
            ResponseMessage<int> response = new ResponseMessage<int>();
            try
            {
                var f = await _deviceManage.GetChannelUnitMap(channel);
                if (f != null)
                {
                    response.Ext = f.UnitID;
                }
                else
                    response.Ext = -1;
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
                    response.Msg = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}error info:{e}";
                    Logger.Error(response.Msg);
                }
            }
            return response;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// GetAllChannelUnitMap
        [HttpGet("signalinfo/backsignal/{mastersignalid}")]
        [IngestAuthentication]//device有点特殊，做了监听端口的所以不能全类检验
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<ProgrammeInfoResponse>> GetBackProgramInfoBySrgid(int mastersignalid)
        {
            async Task Action(ResponseMessage<ProgrammeInfoResponse> response)
            {
                response.Ext = await _deviceManage.GetBackProgramInfoBySrgidAsync(mastersignalid);
            }
            return await TryInvoke((Func<ResponseMessage<ProgrammeInfoResponse>, Task>)Action);
        }


        /// <summary> Try执行 </summary>
        /// <typeparam name="T">返回类型</typeparam>
        /// <param name="action">执行内容</param>
        private async Task<ResponseMessage<T>> TryInvoke<T>(Func<ResponseMessage<T>, Task> action)
        {
            ResponseMessage<T> response = new ResponseMessage<T>();
            try
            {
                await action(response);
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
                    response.Msg = $"error info:{e}";
                    Logger.Error(response.Msg);
                }
            }
            return response;
        }
    }
}
