using System;
using System.Threading.Tasks;
using IngestDBCore;
using IngestDBCore.Basic;
using IngestDevicePlugin.Dto;
using IngestDevicePlugin.Dto.Enum;
using IngestDevicePlugin.Managers;
using Microsoft.AspNetCore.Mvc;
using Sobey.Core.Log;

namespace IngestDevicePlugin.Controllers.v1
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    public class DeviceController : ControllerBase
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
        [HttpGet("get")]
        [ApiExplorerSettings(GroupName = "v1")]
        public string Get()
        {

            return "DBPlatform Service is already startup at " + DateTime.Now.ToString();
        }

        //private readonly ILogger Logger = LoggerManager.GetLogger("TaskInfo");
        //private readonly TaskManager _monthManage;
        //private readonly RestClient _restClient;

        #region GetController

        /// <summary> 获取输入端口与信号源 </summary>
        [HttpGet("GetAllRouterInPortInfo"), MapToApiVersion("1.0")]
        [IngestAuthentication]//device有点特殊，做了监听端口的所以不能全类检验
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<GetAllRouterInPortInfo_param> GetAllRouterInPortInfo()
        {
            GetAllRouterInPortInfo_param response = new GetAllRouterInPortInfo_param();
            try
            {
                response.inportDescs = await _deviceManage.GetAllRouterInPortAsync<RoterInportDesc>();
                response.nVaildDataCount = response.inportDescs.Count;
            }
            catch (Exception e)
            {
                if (e is SobeyRecException se)//sobeyexcep会自动打印错误
                {
                    response.bRet = false;
                    response.errStr = se.Message;
                }
                else
                {
                    response.bRet = false;
                    response.errStr = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}：error info:{e}";
                    Logger.Error(response.errStr);
                }
            }
            return response;
        }

        /// <summary> 获取输出端口与信号源的映射 </summary>
        [HttpGet("GetAllRouterOutPortInfo"), MapToApiVersion("1.0")]
        [IngestAuthentication]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<GetAllRouterOutPortInfo_param> GetAllRouterOutPortInfo()
        {
            GetAllRouterOutPortInfo_param response = new GetAllRouterOutPortInfo_param();
            try
            {
                response.outportDescs = await _deviceManage.GetAllRouterOutPortAsync<RoterOutDesc>();
                response.nVaildDataCount = response.outportDescs.Count;
            }
            catch (Exception e)
            {
                if (e is SobeyRecException se)//sobeyexcep会自动打印错误
                {
                    response.bRet = false;
                    response.errStr = se.Message;
                }
                else
                {
                    response.bRet = false;
                    response.errStr = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}：error info:{e}";
                    Logger.Error(response.errStr);
                }
            }
            return response;
        }

        /// <summary> 获取所有信号源和采集设备的对应 </summary>
        [HttpGet("GetAllSignalDeviceMap"), MapToApiVersion("1.0")]
        [IngestAuthentication]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<GetAllSignalDeviceMap_param> GetAllSignalDeviceMap()
        {
            GetAllSignalDeviceMap_param response = new GetAllSignalDeviceMap_param();
            try
            {
                response.arrSignalDeviceMap = await _deviceManage.GetAllSignalDeviceMapAsync<SignalDeviceMap>();
                response.nVaildDataCount = response.arrSignalDeviceMap.Count;
            }
            catch (Exception e)
            {
                if (e is SobeyRecException se)//sobeyexcep会自动打印错误
                {
                    response.bRet = false;
                    response.errStr = se.Message;
                }
                else
                {
                    response.bRet = false;
                    response.errStr = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}：error info:{e}";
                    Logger.Error(response.errStr);
                }
            }
            return response;
        }

        /// <summary> 获取所有信号源 </summary>
        [HttpGet("GetAllSignalSrcs"), MapToApiVersion("1.0")]
        [IngestAuthentication]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<GetAllSignalSrcs_param> GetAllSignalSrcs()
        {
            GetAllSignalSrcs_param response = new GetAllSignalSrcs_param();
            try
            {
                response.signalInfo = await _deviceManage.GetAllSignalSrcsAsync<SignalSrcInfo>();
                response.nVaildDataCount = response.signalInfo.Count;
            }
            catch (Exception e)
            {
                if (e is SobeyRecException se)//sobeyexcep会自动打印错误
                {
                    response.bRet = false;
                    response.errStr = se.Message;
                }
                else
                {
                    response.bRet = false;
                    response.errStr = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}：error info:{e}";
                    Logger.Error(response.errStr);
                }
            }
            return response;
        }

        /// <summary> 获取所有采集通道 </summary>
        [HttpGet("GetAllCaptureChannels"), MapToApiVersion("1.0")]
        [IngestAuthentication]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<GetAllCaptureChannels_param> GetAllCaptureChannels()
        {
            GetAllCaptureChannels_param response = new GetAllCaptureChannels_param();
            try
            {
                response.captureChannelInfo = await _deviceManage.GetAllCaptureChannelsAsync<CaptureChannelInfo>();
                response.nVaildDataCount = response.captureChannelInfo.Count;
            }
            catch (Exception e)
            {
                if (e is SobeyRecException se)//sobeyexcep会自动打印错误
                {
                    response.bRet = false;
                    response.errStr = se.Message;
                }
                else
                {
                    response.bRet = false;
                    response.errStr = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}：error info:{e}";
                    Logger.Error(response.errStr);
                }
            }
            return response;
        }

        /// <summary> 获取所有的采集设备信息 </summary>
        [HttpGet("GetAllCaptureDevices"), MapToApiVersion("1.0")]
        [IngestAuthentication]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<GetAllCaptureDevices_param> GetAllCaptureDevices()
        {
            GetAllCaptureDevices_param response = new GetAllCaptureDevices_param();
            try
            {
                response.arCaptureDeviceList = await _deviceManage.GetAllCaptureDevicesAsync<CaptureDeviceInfo>();
                response.nVaildDataCount = response.arCaptureDeviceList.Count;
            }
            catch (Exception e)
            {
                if (e is SobeyRecException se)//sobeyexcep会自动打印错误
                {
                    response.bRet = false;
                    response.errStr = se.Message;
                }
                else
                {
                    response.bRet = false;
                    response.errStr = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}：error info:{e}";
                    Logger.Error(response.errStr);
                }
            }
            return response;
        }

        /// <summary> 获取指定信号源和采集设备的对应 </summary>
        [HttpGet("GetSignalDeviceMapBySignalID"), MapToApiVersion("1.0")]
        [IngestAuthentication]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<GetSignalDeviceMapBySignalID_param> GetSignalDeviceMapBySignalID([FromQuery, DefaultValue(711)]int nSignalID)
        {
            GetSignalDeviceMapBySignalID_param response = new GetSignalDeviceMapBySignalID_param();
            try
            {
                var res = await _deviceManage.GetSignalDeviceMapBySignalID<SignalDeviceMap>(nSignalID);
                if (res != null)
                {
                    response.nDeviceID = res.nDeviceID;
                    response.nDeviceOutPortIdx = res.nOutPortIdx;
                    response.SignalSource = res.SignalSource;
                }
            }
            catch (Exception e)
            {
                if (e is SobeyRecException se)//sobeyexcep会自动打印错误
                {
                    response.bRet = false;
                    response.errStr = se.Message;
                }
                else
                {
                    response.bRet = false;
                    response.errStr = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}：error info:{e}";
                    Logger.Error(response.errStr);
                }
            }
            return response;
        }

        /// <summary> 设置信号源和采集设备的对应 </summary>
        /// <param name="nSignalID">信号Id</param>
        /// <param name="nDeviceID">设备Id</param>
        /// <param name="nDeviceOutPortIdx">设备输出端口索引</param>
        /// <param name="SignalSource">信号源</param>
        [HttpGet("GetSetSignalDeviceMap"), MapToApiVersion("1.0")]
        [IngestAuthentication]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<string> GetSetSignalDeviceMap([FromQuery, DefaultValue(711)]int nSignalID, [FromQuery, DefaultValue(666)]int nDeviceID, [FromQuery, DefaultValue(1)]int nDeviceOutPortIdx, [FromQuery, DefaultValue(1)]emSignalSource SignalSource)
        {
            Base_param response = new Base_param();
            try
            {
                await _deviceManage.SaveSignalDeviceMapAsync(nSignalID, nDeviceID, nDeviceOutPortIdx, SignalSource);
            }
            catch (Exception e)
            {
                if (e is SobeyRecException se)//sobeyexcep会自动打印错误
                {
                    response.bRet = false;
                    response.errStr = se.Message;
                }
                else
                {
                    response.bRet = false;
                    response.errStr = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}：error info:{e}";
                    Logger.Error(response.errStr);
                }
            }
            return response.errStr;
        }

        /// <summary> 查询所有信号源的扩展信息 </summary>
        [HttpGet("GetAllSignalSrcExs"), MapToApiVersion("1.0")]
        [IngestAuthentication]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<GetAllSignalSrcExs_param> GetAllSignalSrcExs()
        {
            GetAllSignalSrcExs_param response = new GetAllSignalSrcExs_param();
            try
            {
                response.signalInfo = await _deviceManage.GetAllSignalSrcExsAsync<SignalSrcExInfo>();
                response.nVaildDataCount = response.signalInfo.Count;
            }
            catch (Exception e)
            {
                if (e is SobeyRecException se)//sobeyexcep会自动打印错误
                {
                    response.bRet = false;
                    response.errStr = se.Message;
                }
                else
                {
                    response.bRet = false;
                    response.errStr = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}：error info:{e}";
                    Logger.Error(response.errStr);
                }
            }
            return response;
        }

        /// <summary> 根据 信号源Id 查询信号源是否是备份信号源 </summary>
        [HttpGet("GetIsBackupSignalSrcByID"), MapToApiVersion("1.0")]
        [IngestAuthentication]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<string> GetIsBackupSignalSrcByID([FromQuery, DefaultValue(711)]int nSignalSrcId)
        {
            GetAllSignalSrcExs_param response = new GetAllSignalSrcExs_param();
            try
            {
                if (!await _deviceManage.IsBackupSignalSrcByIdAsync(nSignalSrcId))
                {
                    response.errStr = null;
                }
            }
            catch (Exception e)
            {
                if (e is SobeyRecException se)//sobeyexcep会自动打印错误
                {
                    response.bRet = false;
                    response.errStr = se.Message;
                }
                else
                {
                    response.bRet = false;
                    response.errStr = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}：error info:{e}";
                    Logger.Error(response.errStr);
                }
            }
            return response.errStr;
        }

        /// <summary> 根据 通道Id 获取高清还是标清 nType:0标清,1高清 </summary>
        [HttpGet("GetParamTypeByChannleID"), MapToApiVersion("1.0")]
        [IngestAuthentication]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<GetParamTypeByChannleID_param> GetParamTypeByChannleID([FromQuery, DefaultValue(911)]int nChannelID)
        {
            GetParamTypeByChannleID_param response = new GetParamTypeByChannleID_param();
            try
            {
                response.nType = await _deviceManage.GetParamTypeByChannleIDAsync(nChannelID);
                if (response.nType == -1)
                {
                    response.errStr = "No Such Value!";
                    response.bRet = false;
                }
            }
            catch (Exception e)
            {
                if (e is SobeyRecException se)//sobeyexcep会自动打印错误
                {
                    response.bRet = false;
                    response.errStr = se.Message;
                }
                else
                {
                    response.bRet = false;
                    response.errStr = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}：error info:{e}";
                    Logger.Error(response.errStr);
                }
            }
            return response;
        }

        /// <summary> 根据 通道Id 获取MSV设备状态信息 </summary>
        [HttpGet("GetMSVChannelState"), MapToApiVersion("1.0")]
        [IngestAuthentication]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<GetMSVChannelState_param> GetMSVChannelState([FromQuery, DefaultValue(666)]int nID)
        {
            GetMSVChannelState_param response = new GetMSVChannelState_param();
            try
            {
                response.channelStata = await _deviceManage.GetMsvChannelStateAsync<MSVChannelState>(nID);
            }
            catch (Exception e)
            {
                if (e is SobeyRecException se)//sobeyexcep会自动打印错误
                {
                    response.bRet = false;
                    response.errStr = se.Message;
                }
                else
                {
                    response.bRet = false;
                    response.errStr = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}：error info:{e}";
                    Logger.Error(response.errStr);
                }
            }
            return response;
        }

        /// <summary> 获得所有信号源分组 </summary>
        [HttpGet("GetAllSignalGroup"), MapToApiVersion("1.0")]
        [IngestAuthentication]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<GetAllSignalGroup_OUT> GetAllSignalGroup()
        {
            GetAllSignalGroup_OUT response = new GetAllSignalGroup_OUT();
            try
            {
                response.arAllSignalGroup = await _deviceManage.GetAllSignalGroupAsync<AllSignalGroup>();
                response.nVaildDataCount = response.arAllSignalGroup.Count;
            }
            catch (Exception e)
            {
                if (e is SobeyRecException se)//sobeyexcep会自动打印错误
                {
                    response.bRet = false;
                    response.errStr = se.Message;
                }
                else
                {
                    response.bRet = false;
                    response.errStr = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}：error info:{e}";
                    Logger.Error(response.errStr);
                }
            }
            return response;
        }

        /// <summary> 获取所有信号源分组信息 </summary>
        [HttpGet("GetAllSignalGroupInfo"), MapToApiVersion("1.0")]
        [IngestAuthentication]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<GetAllSignalGroupState_OUT> GetAllSignalGroupInfo()
        {
            GetAllSignalGroupState_OUT response = new GetAllSignalGroupState_OUT();
            try
            {
                response.arAllSignalGroupState = await _deviceManage.GetAllSignalGroupInfoAsync<SignalGroupState>();
                response.nVaildDataCount = response.arAllSignalGroupState.Count;
            }
            catch (Exception e)
            {
                if (e is SobeyRecException se)//sobeyexcep会自动打印错误
                {
                    response.bRet = false;
                    response.errStr = se.Message;
                }
                else
                {
                    response.bRet = false;
                    response.errStr = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}：error info:{e}";
                    Logger.Error(response.errStr);
                }
            }
            return response;
        }

        /// <summary> 通过 GPIID 找出该GPI所有的映射 </summary>
        [HttpGet("GetGPIMapInfoByGPIID"), MapToApiVersion("1.0")]
        [IngestAuthentication]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<GetGPIMapInfoByGPIID_OUT> GetGPIMapInfoByGPIID([FromQuery, DefaultValue(666)]int nGPIID)
        {
            GetGPIMapInfoByGPIID_OUT response = new GetGPIMapInfoByGPIID_OUT();
            try
            {
                response.arGPIDeviceMapInfo = await _deviceManage.GetGPIMapInfoByGPIIDAsync<GPIDeviceMapInfo>(nGPIID);
                response.nVaildDataCount = response.arGPIDeviceMapInfo.Count;
            }
            catch (Exception e)
            {
                if (e is SobeyRecException se)//sobeyexcep会自动打印错误
                {
                    response.bRet = false;
                    response.errStr = se.Message;
                }
                else
                {
                    response.bRet = false;
                    response.errStr = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}：error info:{e}";
                    Logger.Error(response.errStr);
                }
            }
            return response;
        }

        /// <summary> 获取所有节目 </summary>
        [HttpGet("GetAllProgrammeInfos"), MapToApiVersion("1.0")]
        [IngestAuthentication]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<GetAllProgrammeInfos_OUT> GetAllProgrammeInfos()
        {
            GetAllProgrammeInfos_OUT response = new GetAllProgrammeInfos_OUT();
            try
            {
                response.programmeInfos = await _deviceManage.GetAllProgrammeInfosAsync();
                response.nValidDataCount = response.programmeInfos.Count;
            }
            catch (Exception e)
            {
                if (e is SobeyRecException se)//sobeyexcep会自动打印错误
                {
                    response.bRet = false;
                    response.errStr = se.Message;
                }
                else
                {
                    response.bRet = false;
                    response.errStr = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}：error info:{e}";
                    Logger.Error(response.errStr);
                }
            }
            return response;
        }

        /// <summary> 获得所有通道的状态 </summary>
        [HttpGet("GetAllChannelState"), MapToApiVersion("1.0")]
        [IngestAuthentication]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<GetAllChannelState_OUT> GetAllChannelState()
        {
            GetAllChannelState_OUT response = new GetAllChannelState_OUT();
            try
            {
                response.arMSVChannelState = await _deviceManage.GetAllChannelStateAsync<MSVChannelState>();
                response.nVaildDataCount = response.arMSVChannelState.Count;
            }
            catch (Exception e)
            {
                if (e is SobeyRecException se)//sobeyexcep会自动打印错误
                {
                    response.bRet = false;
                    response.errStr = se.Message;
                }
                else
                {
                    response.bRet = false;
                    response.errStr = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}：error info:{e}";
                    Logger.Error(response.errStr);
                }
            }
            return response;
        }

        //根据通道获取相应的节目，有矩阵模式和无矩阵模式的区别"
        [HttpGet("GetProgrammeInfosByChannelId"), MapToApiVersion("1.0")]
        [IngestAuthentication]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<GetProgrammeInfosByChannelId_OUT> GetProgrammeInfosByChannelId([FromQuery, DefaultValue(911)]int channelId)
        {
            GetProgrammeInfosByChannelId_OUT response = new GetProgrammeInfosByChannelId_OUT();
            try
            {
                if (channelId <= 0)
                {
                    response.errStr = "Param wrong.";
                }

                response.programmeInfos = await _deviceManage.GetProgrammeInfosByChannelIdAsync(channelId);
                response.validCount = response.programmeInfos.Count;
            }
            catch (Exception e)
            {
                if (e is SobeyRecException se)//sobeyexcep会自动打印错误
                {
                    response.bRet = false;
                    response.errStr = se.Message;
                }
                else
                {
                    response.bRet = false;
                    response.errStr = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}：error info:{e}";
                    Logger.Error(response.errStr);
                }
            }
            return response;
        }

        //通过 通道ID获取采集通道
        [HttpGet("GetCaptureChannelByID"), MapToApiVersion("1.0")]
        [IngestAuthentication]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<GetCaptureChannelByID_OUT> GetCaptureChannelByID([FromQuery, DefaultValue(911)]int nChannelID)
        {
            GetCaptureChannelByID_OUT response = new GetCaptureChannelByID_OUT();
            try
            {
                response.captureChannelInfo = await _deviceManage.GetCaptureChannelByIDAsync<CaptureChannelInfo>(nChannelID);
            }
            catch (Exception e)
            {
                if (e is SobeyRecException se)//sobeyexcep会自动打印错误
                {
                    response.bRet = false;
                    response.errStr = se.Message;
                }
                else
                {
                    response.bRet = false;
                    response.errStr = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}：error info:{e}";
                    Logger.Error(response.errStr);
                }
            }
            return response;
        }

        /// <summary>更改MSV设备状态信息</summary>
        /// <param name="nID">通道Id</param>
        /// <param name="nDevState">设备状态</param>
        /// <param name="nMSVMode">MSV模式</param>
        [HttpGet("GetModifyDevState"), MapToApiVersion("1.0")]
        [IngestAuthentication]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<bool> GetModifyDevState([FromQuery]int nID, [FromQuery, DefaultValue(0)]int nDevState, [FromQuery, DefaultValue(0)]int nMSVMode)
        {
            GetAllTSDeviceInfos_OUT response = new GetAllTSDeviceInfos_OUT();
            try
            {
                await _deviceManage.UpdateMSVChannelStateAsync(nID, nDevState, nMSVMode);
            }
            catch (Exception e)
            {
                if (e is SobeyRecException se)//sobeyexcep会自动打印错误
                {
                    response.bRet = false;
                    response.errStr = se.Message;
                }
                else
                {
                    response.bRet = false;
                    response.errStr = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}：error info:{e}";
                    Logger.Error(response.errStr);
                }
            }
            return response.bRet;
        }


        /// <summary>获得所有的IP收录的设备</summary>
        [HttpGet("GetAllTSDeviceInfos"), MapToApiVersion("1.0")]
        [IngestAuthentication]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<GetAllTSDeviceInfos_OUT> GetAllTSDeviceInfos()
        {
            GetAllTSDeviceInfos_OUT response = new GetAllTSDeviceInfos_OUT();
            try
            {
                response.deviceInfos = await _deviceManage.GetAllTSDeviceInfosAsync();
                response.nValidCount = response.deviceInfos.Count;
            }
            catch (Exception e)
            {
                if (e is SobeyRecException se)//sobeyexcep会自动打印错误
                {
                    response.bRet = false;
                    response.errStr = se.Message;
                }
                else
                {
                    response.bRet = false;
                    response.errStr = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}：error info:{e}";
                    Logger.Error(response.errStr);
                }
            }
            return response;
        }

        /// <summary>获取该信号源的备份信号源ID</summary>
        [HttpGet("GetBackupSignalSrcInfo"), MapToApiVersion("1.0")]
        [IngestAuthentication]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<GetBackupSignalSrcInfo_OUT> GetBackupSignalSrcInfo([FromQuery, DefaultValue(711)]int nSignalSrcId)
        {
            GetBackupSignalSrcInfo_OUT response = new GetBackupSignalSrcInfo_OUT();
            try
            {
                response.nBackupSignalSrcId = await _deviceManage.GetBackupSignalSrcIdByIdAsync(nSignalSrcId);
                response.bIsHavingBackupSglSrc = response.nBackupSignalSrcId > 0;
            }
            catch (Exception e)
            {
                if (e is SobeyRecException se)//sobeyexcep会自动打印错误
                {
                    response.bRet = false;
                    response.errStr = se.Message;
                }
                else
                {
                    response.bRet = false;
                    response.errStr = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}：error info:{e}";
                    Logger.Error(response.errStr);
                }
            }
            return response;
        }


        /// <summary>根据信号源获取是高清还是标清</summary>
        [HttpGet("GetParamTypeBySignalID"), MapToApiVersion("1.0")]
        [IngestAuthentication]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<GetParamTypeBySignalID_OUT> GetParamTypeBySignalID([FromQuery, DefaultValue(711)]int nSignalID)//nType:0标清,1高清
        {
            GetParamTypeBySignalID_OUT response = new GetParamTypeBySignalID_OUT();
            try
            {
                response.nType = await _deviceManage.GetParamTypeBySignalIDAsync(nSignalID);
                if (response.nType == -1)
                {
                    response.bRet = false;
                    response.errStr = "No Such Value!";
                }
            }
            catch (Exception e)
            {
                if (e is SobeyRecException se)//sobeyexcep会自动打印错误
                {
                    response.bRet = false;
                    response.errStr = se.Message;
                }
                else
                {
                    response.bRet = false;
                    response.errStr = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}：error info:{e}";
                    Logger.Error(response.errStr);
                }
            }
            return response;
        }

        /// <summary>根据节目ID获取相应的通道，有矩阵模式和无矩阵模式的区别</summary>
        [HttpGet("GetChannelsByProgrammeId"), MapToApiVersion("1.0")]
        [IngestAuthentication]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<GetChannelsByProgrammeId_out> GetChannelsByProgrammeId([FromQuery, DefaultValue(666)]int programmeId)
        {
            GetChannelsByProgrammeId_out response = new GetChannelsByProgrammeId_out();
            try
            {
                response.channelInfos = await _deviceManage.GetChannelsByProgrammeIdAsync<CaptureChannelInfo>(programmeId);
                response.validCount = response.channelInfos.Count;
            }
            catch (Exception e)
            {
                if (e is SobeyRecException se)//sobeyexcep会自动打印错误
                {
                    response.bRet = false;
                    response.errStr = se.Message;
                }
                else
                {
                    response.bRet = false;
                    response.errStr = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}：error info:{e}";
                    Logger.Error(response.errStr);
                }
            }
            return response;
        }

        /// <summary>根据信号源,用户名,自动匹配最优通道</summary>
        [HttpGet("GetBestChannelIDBySignalID"), MapToApiVersion("1.0")]
        [IngestAuthentication]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<GetBestChannelIDBySignalID_out> GetBestChannelIDBySignalID([FromQuery, DefaultValue(711)]int nSignalID, [FromQuery, DefaultValue("9527")]string strUserCode)
        {
            GetBestChannelIDBySignalID_out response = new GetBestChannelIDBySignalID_out();
            try
            {
                response.nChannelID = await _deviceManage.GetBestChannelIdBySignalIDAsync(nSignalID, strUserCode);
            }
            catch (Exception e)
            {
                if (e is SobeyRecException se)//sobeyexcep会自动打印错误
                {
                    response.bRet = false;
                    response.errStr = se.Message;
                }
                else
                {
                    response.bRet = false;
                    response.errStr = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}：error info:{e}";
                    Logger.Error(response.errStr);
                }
            }
            return response;
        }

        //为信号源选择一个合适的预监通道
        [HttpGet("GetBestPreviewChannelForSignal"), MapToApiVersion("1.0")]
        [IngestAuthentication]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<GetBestPreviewChannelForSignal_out> GetBestPreviewChannelForSignal([FromQuery, DefaultValue(711)]int nSignalID)
        {
            GetBestPreviewChannelForSignal_out response = new GetBestPreviewChannelForSignal_out();
            try
            {
                if (nSignalID <= 0)
                {
                    response.errStr = "Signal ID less than 0.";
                    response.bRet = false;
                }
                else
                {
                    response.nChnID = await _deviceManage.GetBestPreviewChnForSignalAsync(nSignalID);
                }
            }
            catch (Exception e)
            {
                if (e is SobeyRecException se)//sobeyexcep会自动打印错误
                {
                    response.bRet = false;
                    response.errStr = se.Message;
                }
                else
                {
                    response.bRet = false;
                    response.errStr = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}：error info:{e}";
                    Logger.Error(response.errStr);
                }
            }
            return response;
        }


        /// <summary>获取所有GPI设备</summary>
        [HttpGet("GetAllGPIDevices"), MapToApiVersion("1.0")]
        [IngestAuthentication]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<Dto.Old.Response.ResponseMessage<GPIDeviceInfo[]>> GetAllGPIDevices()
        {
            Dto.Old.Response.ResponseMessage<GPIDeviceInfo[]> p = new Dto.Old.Response.ResponseMessage<GPIDeviceInfo[]>();
            try
            {
                p.extention = (await _deviceManage.GetAllGPIInfoAsync<GPIDeviceInfo>()).ToArray();
            }
            catch (Exception ex)
            {
                if (ex.GetType() == typeof(SobeyRecException))//sobeyexcep会自动打印错误
                {
                    SobeyRecException se = ex as SobeyRecException;
                    p.nCode = 0;
                    p.message = se.Message;
                }
                else
                {
                    p.nCode = 0;
                    p.message = $"error info:{ex.ToString()}";
                    Logger.Error(p.message);
                }
            }
            return p;
        }

        ///// <summary>根据GPID获取GPI映射信息</summary>
        //[HttpGet("GetGPIMapInfoByGPIID"), MapToApiVersion("1.0")]
        //[IngestAuthentication]
        //[ApiExplorerSettings(GroupName = "v1")]
        //public async Task<Dto.Old.Response.ResponseMessage<GPIDeviceMapInfo[]>> GetGPIMapInfoByGPIID2([FromQuery]int nGPIID)
        //{
        //    Dto.Old.Response.ResponseMessage<GPIDeviceMapInfo[]> p = new Dto.Old.Response.ResponseMessage<GPIDeviceMapInfo[]>();
        //    try
        //    {
        //        p.extention = (await _deviceManage.GetGPIMapInfoByGPIIDAsync<GPIDeviceMapInfo>(nGPIID)).ToArray();
        //    }
        //    catch (Exception ex)
        //    {
        //        if (ex.GetType() == typeof(SobeyRecException))//sobeyexcep会自动打印错误
        //        {
        //            SobeyRecException se = ex as SobeyRecException;
        //            p.nCode = 0;
        //            p.message = se.Message;
        //        }
        //        else
        //        {
        //            p.nCode = 0;
        //            p.message = $"error info:{ex.ToString()}";
        //            Logger.Error(p.message);
        //        }
        //    }
        //    return p;
        //}

        /// <summary>根据信号源获取绑定的采集参数</summary>
        [HttpGet("GetCaptureTemplateIDBySignalID"), MapToApiVersion("1.0")]
        [IngestAuthentication]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<IngestDevicePlugin.Dto.Old.Response.ResponseMessage<int>> GetCaptureTemplateIDBySignalID([FromQuery, DefaultValue(711)]int nSignalID)
        {
            IngestDevicePlugin.Dto.Old.Response.ResponseMessage<int> p = new IngestDevicePlugin.Dto.Old.Response.ResponseMessage<int>();
            try
            {
                p.extention = await _deviceManage.GetSignalCaptureTemplateAsync(nSignalID);
            }
            catch (Exception ex)
            {
                if (ex.GetType() == typeof(SobeyRecException))//sobeyexcep会自动打印错误
                {
                    SobeyRecException se = ex as SobeyRecException;
                    p.nCode = 0;
                    p.message = se.Message;
                }
                else
                {
                    p.nCode = 0;
                    p.message = $"error info:{ex.ToString()}";
                    Logger.Error(p.message);
                }
            }
            return p;
        }

        #endregion GetController

        #region UpdateController

        //Todo:Post
        /// <summary>更新所有的IP收录的设备</summary>
        [HttpPost("UpdateAllTSDeviceInfos"), MapToApiVersion("1.0")]
        [IngestAuthentication]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<bool> PostUpdateAllTSDeviceInfos([FromBody]UpdateAllTSDeviceInfos_IN pIn)
        {
            try
            {
                return await _deviceManage.UpdateAllTSDeviceInfosAsync(pIn.deviceInfos);
            }
            catch (Exception ex)//其他未知的异常，写异常日志
            {
                Logger.Error("Interface:PostUpdateAllTSDeviceInfos-> error occur:" + ex.Message);
                return false;
            }
        }

        /// <summary> 更新通道的扩展数据 </summary>
        [HttpPost("PostUpdateChnExtData"), MapToApiVersion("1.0")]
        [IngestAuthentication]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<UpdateChnExtData_OUT> PostUpdateChnExtData([FromBody]UpdateChnExtData_IN pIn)
        {
            UpdateChnExtData_OUT response = new UpdateChnExtData_OUT();
            try
            {
                response.bRet = await _deviceManage.SaveChnExtenddataAsync(pIn.nChnID, pIn.type, pIn.strData);
            }
            catch (Exception e)
            {
                if (e is SobeyRecException se)//sobeyexcep会自动打印错误
                {
                    response.bRet = false;
                    response.errStr = se.Message;
                }
                else
                {
                    response.bRet = false;
                    response.errStr = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}：error info:{e}";
                    Logger.Error(response.errStr);
                }
            }
            return response;
        }


        /// <summary>更改ModifySourceVTRIDAndUserCode</summary>
        [HttpGet("ModifySourceVTRIDAndUserCode"), MapToApiVersion("1.0")]
        [IngestAuthentication]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<ModifySourceVTRIDAndUserCode_out> ModifySourceVTRIDAndUserCode([FromQuery, DefaultValue(666)]int nID, [FromQuery, DefaultValue(777)]int nSourceVTRID, [FromQuery, DefaultValue(9527)]string userCode)
        {
            ModifySourceVTRIDAndUserCode_out response = new ModifySourceVTRIDAndUserCode_out();
            try
            {
                await _deviceManage.ModifySourceVTRIDAndUserCodeAsync(nSourceVTRID, userCode, nID);
            }
            catch (Exception e)
            {
                if (e is SobeyRecException se)//sobeyexcep会自动打印错误
                {
                    response.bRet = false;
                    response.errStr = se.Message;
                }
                else
                {
                    response.bRet = false;
                    response.errStr = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}：error info:{e}";
                    Logger.Error(response.errStr);
                }
            }
            return response;
        }

        //更改ModifySourceVTRIDAndUserCodeByChannelIDArray"
        [HttpPost("ModifySourceVTRIDAndUserCodeByChannelIDArray"), MapToApiVersion("1.0")]
        [IngestAuthentication]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<ModifySourceVTRIDAndUserCode_out> ModifySourceVTRIDAndUserCodeByChannelIDArray([FromBody]ModifySourceVTR_in pIn)
        {
            ModifySourceVTRIDAndUserCode_out response = new ModifySourceVTRIDAndUserCode_out();
            try
            {
                if (pIn.nIDArray != null)
                {
                    pIn.nIDArray = new int[0];
                    if (pIn.nIDArray.Length <= 0)
                    {
                        response.errStr = "no channelid";
                        response.bRet = false;
                    }
                    else
                    {
                        await _deviceManage.ModifySourceVTRIDAndUserCodeAsync(pIn.nSourceVTRID, pIn.userCode, pIn.nIDArray);
                    }
                }
            }
            catch (Exception e)
            {
                if (e is SobeyRecException se)//sobeyexcep会自动打印错误
                {
                    response.bRet = false;
                    response.errStr = se.Message;
                }
                else
                {
                    response.bRet = false;
                    response.errStr = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}：error info:{e}";
                    Logger.Error(response.errStr);
                }
            }
            return response;
        }
        #endregion UpdateController

    }
}
