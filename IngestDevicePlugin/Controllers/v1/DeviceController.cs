using System;
using System.Threading.Tasks;
using IngestDBCore;
using IngestDBCore.Basic;
using IngestDevicePlugin.Dto;
using IngestDevicePlugin.Dto.Enum;
using IngestDevicePlugin.Dto.Response;
using Microsoft.AspNetCore.Mvc;

namespace IngestDevicePlugin.Controllers
{
    public partial class DeviceController : ControllerBase
    {
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
            async Task Action(GetAllRouterInPortInfo_param response)
            {
                response.inportDescs = await _deviceManage.GetAllRouterInPortAsync<RoterInportDesc>();
                response.nVaildDataCount = response.inportDescs.Count;
            }

            return await TryInvoke((Func<GetAllRouterInPortInfo_param, Task>)Action);
        }

        /// <summary> 获取输出端口与信号源的映射 </summary>
        [HttpGet("GetAllRouterOutPortInfo"), MapToApiVersion("1.0")]
        [IngestAuthentication]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<GetAllRouterOutPortInfo_param> GetAllRouterOutPortInfo()
        {
            async Task Action(GetAllRouterOutPortInfo_param response)
            {
                response.outportDescs = await _deviceManage.GetAllRouterOutPortAsync<RoterOutDesc>();
                response.nVaildDataCount = response.outportDescs.Count;
            }

            return await TryInvoke((Func<GetAllRouterOutPortInfo_param, Task>)Action);
        }

        /// <summary> 获取所有信号源和采集设备的对应 </summary>
        [HttpGet("GetAllSignalDeviceMap"), MapToApiVersion("1.0")]
        [IngestAuthentication]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<GetAllSignalDeviceMap_param> GetAllSignalDeviceMap()
        {
            async Task Action(GetAllSignalDeviceMap_param response)
            {
                response.arrSignalDeviceMap = await _deviceManage.GetAllSignalDeviceMapAsync<SignalDeviceMap>();
                response.nVaildDataCount = response.arrSignalDeviceMap.Count;
            }

            return await TryInvoke((Func<GetAllSignalDeviceMap_param, Task>)Action);
        }

        /// <summary> 获取所有信号源 </summary>
        [HttpGet("GetAllSignalSrcs"), MapToApiVersion("1.0")]
        [IngestAuthentication]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<GetAllSignalSrcs_param> GetAllSignalSrcs()
        {
            async Task Action(GetAllSignalSrcs_param response)
            {
                response.signalInfo = await _deviceManage.GetAllSignalSrcsAsync();
                response.nVaildDataCount = response.signalInfo.Count;
            }

            return await TryInvoke((Func<GetAllSignalSrcs_param, Task>)Action);
        }

        /// <summary> 获取所有采集通道 </summary>
        [HttpGet("GetAllCaptureChannels"), MapToApiVersion("1.0")]
        [IngestAuthentication]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<GetAllCaptureChannels_param> GetAllCaptureChannels()
        {
            async Task Action(GetAllCaptureChannels_param response)
            {
                response.captureChannelInfo = await _deviceManage.GetAllCaptureChannelsAsync();
                response.nVaildDataCount = response.captureChannelInfo.Count;
            }

            return await TryInvoke((Func<GetAllCaptureChannels_param, Task>)Action);
        }

        /// <summary> 获取所有的采集设备信息 </summary>
        [HttpGet("GetAllCaptureDevices"), MapToApiVersion("1.0")]
        [IngestAuthentication]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<GetAllCaptureDevices_param> GetAllCaptureDevices()
        {
            async Task Action(GetAllCaptureDevices_param response)
            {
                response.arCaptureDeviceList = await _deviceManage.GetAllCaptureDevicesAsync<CaptureDeviceInfo>();
                response.nVaildDataCount = response.arCaptureDeviceList.Count;
            }

            return await TryInvoke((Func<GetAllCaptureDevices_param, Task>)Action);
        }

        /// <summary> 获取指定信号源和采集设备的对应 </summary>
        [HttpGet("GetSignalDeviceMapBySignalID"), MapToApiVersion("1.0")]
        [IngestAuthentication]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<GetSignalDeviceMapBySignalID_param> GetSignalDeviceMapBySignalID(int nSignalID)
        {
            async Task Action(GetSignalDeviceMapBySignalID_param response)
            {
                var res = await _deviceManage.GetSignalDeviceMapBySignalID(nSignalID);
                if (res != null)
                {
                    response.nDeviceID = res.nDeviceID;
                    response.nDeviceOutPortIdx = res.nOutPortIdx;
                    response.SignalSource = res.SignalSource;
                }
            }

            return await TryInvoke((Func<GetSignalDeviceMapBySignalID_param, Task>)Action);
        }

        /// <summary> 设置信号源和采集设备的对应 </summary>
        /// <param name="nSignalID">信号Id</param>
        /// <param name="nDeviceID">设备Id</param>
        /// <param name="nDeviceOutPortIdx">设备输出端口索引</param>
        /// <param name="SignalSource">信号源</param>
        [HttpGet("GetSetSignalDeviceMap"), MapToApiVersion("1.0")]
        [IngestAuthentication]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<string> GetSetSignalDeviceMap(int nSignalID, int nDeviceID, int nDeviceOutPortIdx, emSignalSource SignalSource)
        {
            async Task Action(Base_param response)
            {
                await _deviceManage.SaveSignalDeviceMapAsync(nSignalID, nDeviceID, nDeviceOutPortIdx, SignalSource);
            }

            var r = await TryInvoke((Func<Base_param, Task>)Action);
            return r.errStr;
        }

        /// <summary> 查询所有信号源的扩展信息 </summary>
        [HttpGet("GetAllSignalSrcExs"), MapToApiVersion("1.0")]
        [IngestAuthentication]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<GetAllSignalSrcExs_param> GetAllSignalSrcExs()
        {
            async Task Action(GetAllSignalSrcExs_param response)
            {
                response.signalInfo = await _deviceManage.GetAllSignalSrcExsAsync();
                response.nVaildDataCount = response.signalInfo.Count;
            }

            return await TryInvoke((Func<GetAllSignalSrcExs_param, Task>)Action);
        }

        /// <summary> 根据 信号源Id 查询信号源是否是备份信号源 </summary>
        [HttpGet("GetIsBackupSignalSrcByID"), MapToApiVersion("1.0")]
        [IngestAuthentication]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<string> GetIsBackupSignalSrcByID(int nSignalSrcId)
        {
            async Task Action(GetAllSignalSrcExs_param response)
            {
                if (!await _deviceManage.IsBackupSignalSrcByIdAsync(nSignalSrcId))
                {
                    response.errStr = null;
                }
            }

            return (await TryInvoke((Func<GetAllSignalSrcExs_param, Task>)Action)).errStr;
        }

        /// <summary> 根据 通道Id 获取高清还是标清 nType:0标清,1高清 </summary>
        [HttpGet("GetParamTypeByChannleID"), MapToApiVersion("1.0")]
        [IngestAuthentication]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<GetParamTypeByChannleID_param> GetParamTypeByChannleID(int nChannelID)
        {
            async Task Action(GetParamTypeByChannleID_param response)
            {
                response.nType = await _deviceManage.GetParamTypeByChannleIDAsync(nChannelID);
                if (response.nType == -1)
                {
                    response.errStr = "No Such Value!";
                    response.bRet = false;
                }
            }

            return await TryInvoke((Func<GetParamTypeByChannleID_param, Task>)Action);
        }

        /// <summary> 根据 通道Id 获取MSV设备状态信息 </summary>
        [HttpGet("GetMSVChannelState"), MapToApiVersion("1.0")]
        [IngestAuthentication]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<GetMSVChannelState_param> GetMSVChannelState(int nID)
        {
            async Task Action(GetMSVChannelState_param response)
            {
                response.channelStata = await _deviceManage.GetMsvChannelStateAsync(nID);
            }

            return await TryInvoke((Func<GetMSVChannelState_param, Task>)Action);
        }

        /// <summary> 获得所有信号源分组 </summary>
        [HttpGet("GetAllSignalGroup"), MapToApiVersion("1.0")]
        [IngestAuthentication]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<GetAllSignalGroup_OUT> GetAllSignalGroup()
        {
            async Task Action(GetAllSignalGroup_OUT response)
            {
                response.arAllSignalGroup = await _deviceManage.GetAllSignalGroupAsync();
                response.nVaildDataCount = response.arAllSignalGroup.Count;
            }

            return await TryInvoke((Func<GetAllSignalGroup_OUT, Task>)Action);
        }

        /// <summary> 获取所有信号源分组信息 </summary>
        [HttpGet("GetAllSignalGroupInfo"), MapToApiVersion("1.0")]
        [IngestAuthentication]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<GetAllSignalGroupState_OUT> GetAllSignalGroupInfo()
        {
            async Task Action(GetAllSignalGroupState_OUT response)
            {
                response.arAllSignalGroupState = await _deviceManage.GetAllSignalGroupInfoAsync();
                response.nVaildDataCount = response.arAllSignalGroupState.Count;
            }

            return await TryInvoke((Func<GetAllSignalGroupState_OUT, Task>)Action);
        }

        /// <summary> 通过 GPIID 找出该GPI所有的映射 </summary>
        [HttpGet("GetGPIMapInfoByGPIID"), MapToApiVersion("1.0")]
        [IngestAuthentication]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<GetGPIMapInfoByGPIID_OUT> GetGPIMapInfoByGPIID(int nGPIID)
        {
            async Task Action(GetGPIMapInfoByGPIID_OUT response)
            {
                response.arGPIDeviceMapInfo = await _deviceManage.GetGPIMapInfoByGPIIDAsync<GPIDeviceMapInfo>(nGPIID);
                response.nVaildDataCount = response.arGPIDeviceMapInfo.Count;
            }

            return await TryInvoke((Func<GetGPIMapInfoByGPIID_OUT, Task>)Action);
        }

        /// <summary> 获取所有节目 </summary>
        [HttpGet("GetAllProgrammeInfos"), MapToApiVersion("1.0")]
        [IngestAuthentication]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<GetAllProgrammeInfos_OUT> GetAllProgrammeInfos()
        {
            async Task Action(GetAllProgrammeInfos_OUT response)
            {
                response.programmeInfos = await _deviceManage.GetAllProgrammeInfosAsync();
                response.nValidDataCount = response.programmeInfos.Count;
            }

            return await TryInvoke((Func<GetAllProgrammeInfos_OUT, Task>)Action);
        }

        /// <summary> 获得所有通道的状态 </summary>
        [HttpGet("GetAllChannelState"), MapToApiVersion("1.0")]
        [IngestAuthentication]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<GetAllChannelState_OUT> GetAllChannelState()
        {
            async Task Action(GetAllChannelState_OUT response)
            {
                response.arMSVChannelState = await _deviceManage.GetAllChannelStateAsync();
                response.nVaildDataCount = response.arMSVChannelState.Count;
            }

            return await TryInvoke((Func<GetAllChannelState_OUT, Task>)Action);
        }

        //根据通道获取相应的节目，有矩阵模式和无矩阵模式的区别"
        [HttpGet("GetProgrammeInfosByChannelId"), MapToApiVersion("1.0")]
        [IngestAuthentication]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<GetProgrammeInfosByChannelId_OUT> GetProgrammeInfosByChannelId(int channelId)
        {
            async Task Action(GetProgrammeInfosByChannelId_OUT response)
            {
                if (channelId <= 0)
                {
                    response.errStr = "Param wrong.";
                }

                response.programmeInfos = await _deviceManage.GetProgrammeInfosByChannelIdAsync(channelId);
                response.validCount = response.programmeInfos.Count;
            }

            return await TryInvoke((Func<GetProgrammeInfosByChannelId_OUT, Task>)Action);
        }

        //通过 通道ID获取采集通道
        [HttpGet("GetCaptureChannelByID"), MapToApiVersion("1.0")]
        [IngestAuthentication]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<GetCaptureChannelByID_OUT> GetCaptureChannelByID(int nChannelID)
        {
            async Task Action(GetCaptureChannelByID_OUT response)
            {
                response.captureChannelInfo = await _deviceManage.GetCaptureChannelByIDAsync(nChannelID);
            }

            return await TryInvoke((Func<GetCaptureChannelByID_OUT, Task>)Action);
        }

        /// <summary>更改MSV设备状态信息</summary>
        /// <param name="nID">通道Id</param>
        /// <param name="nDevState">设备状态</param>
        /// <param name="nMSVMode">MSV模式</param>
        [HttpGet("GetModifyDevState"), MapToApiVersion("1.0")]
        [IngestAuthentication]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<bool> GetModifyDevState(int nID, int nDevState, int nMSVMode)
        {
            async Task Action(Base_param response)
            {
                await _deviceManage.UpdateMSVChannelStateAsync(nID, nDevState, nMSVMode);
            }
            return (await TryInvoke((Func<Base_param, Task>)Action)).bRet;
        }


        /// <summary>获得所有的IP收录的设备</summary>
        [HttpGet("GetAllTSDeviceInfos"), MapToApiVersion("1.0")]
        [IngestAuthentication]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<GetAllTSDeviceInfos_OUT> GetAllTSDeviceInfos()
        {
            async Task Action(GetAllTSDeviceInfos_OUT response)
            {
                response.deviceInfos = await _deviceManage.GetAllTSDeviceInfosAsync();
                response.nValidCount = response.deviceInfos.Count;
            }
            return await TryInvoke((Func<GetAllTSDeviceInfos_OUT, Task>)Action);
        }

        /// <summary>获取该信号源的备份信号源ID</summary>
        [HttpGet("GetBackupSignalSrcInfo"), MapToApiVersion("1.0")]
        [IngestAuthentication]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<GetBackupSignalSrcInfo_OUT> GetBackupSignalSrcInfo(int nSignalSrcId)
        {
            async Task Action(GetBackupSignalSrcInfo_OUT response)
            {
                response.nBackupSignalSrcId = await _deviceManage.GetBackupSignalSrcIdByIdAsync(nSignalSrcId);
                response.bIsHavingBackupSglSrc = response.nBackupSignalSrcId > 0;
            }
            return await TryInvoke((Func<GetBackupSignalSrcInfo_OUT, Task>)Action);
        }


        /// <summary>根据信号源获取是高清还是标清</summary>
        [HttpGet("GetParamTypeBySignalID"), MapToApiVersion("1.0")]
        [IngestAuthentication]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<GetParamTypeBySignalID_OUT> GetParamTypeBySignalID(int nSignalID)//nType:0标清,1高清
        {
            async Task Action(GetParamTypeBySignalID_OUT response)
            {
                response.nType = await _deviceManage.GetParamTypeBySignalIDAsync(nSignalID);
                if (response.nType == -1)
                {
                    response.bRet = false;
                    response.errStr = "No Such Value!";
                }
            }
            return await TryInvoke((Func<GetParamTypeBySignalID_OUT, Task>)Action);
        }

        /// <summary>根据节目ID获取相应的通道，有矩阵模式和无矩阵模式的区别</summary>
        [HttpGet("api/device/GetChannelsByProgrammeId"), MapToApiVersion("1.0")]
        [IngestAuthentication]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<GetChannelsByProgrammeId_out> GetChannelsByProgrammeId(int programmeId)
        {
            async Task Action(GetChannelsByProgrammeId_out response)
            {
                response.channelInfos = await _deviceManage.GetChannelsByProgrammeIdAsync<CaptureChannelInfo>(programmeId);
                response.validCount = response.channelInfos.Count;
            }
            return await TryInvoke((Func<GetChannelsByProgrammeId_out, Task>)Action);
        }

        /// <summary>根据信号源,用户名,自动匹配最优通道</summary>
        [HttpGet("device/GetBestChannelIDBySignalID"), MapToApiVersion("1.0")]
        [IngestAuthentication]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<GetBestChannelIDBySignalID_out> GetBestChannelIDBySignalID(int nSignalID, string strUserCode)
        {
            async Task Action(GetBestChannelIDBySignalID_out response)
            {
                response.nChannelID = await _deviceManage.GetBestChannelIdBySignalIDAsync(nSignalID, strUserCode);
            }
            return await TryInvoke((Func<GetBestChannelIDBySignalID_out, Task>)Action);
        }

        //为信号源选择一个合适的预监通道
        [HttpGet("GetBestPreviewChannelForSignal"), MapToApiVersion("1.0")]
        [IngestAuthentication]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<GetBestPreviewChannelForSignal_out> GetBestPreviewChannelForSignal(int nSignalID)
        {
            GetBestChannelIDBySignalID_out p = new GetBestChannelIDBySignalID_out();
            async Task Action(GetBestPreviewChannelForSignal_out response)
            {
                if (nSignalID <= 0)
                {
                    p.errStr = "Signal ID less than 0.";
                    p.bRet = false;
                }
                else
                {
                    response.nChnID = await _deviceManage.GetBestPreviewChnForSignalAsync(nSignalID);
                }
            }
            return await TryInvoke((Func<GetBestPreviewChannelForSignal_out, Task>)Action);
        }


        /// <summary>获取所有GPI设备</summary>
        [HttpGet("/api/device/GetAllGPIDevices"), MapToApiVersion("1.0")]
        [IngestAuthentication]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<IngestDevicePlugin.Dto.Response.ResponseMessage<GPIDeviceInfo[]>> GetAllGPIDevices()
        {
            IngestDevicePlugin.Dto.Response.ResponseMessage<GPIDeviceInfo[]> p = new IngestDevicePlugin.Dto.Response.ResponseMessage<GPIDeviceInfo[]>();
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

        /// <summary>根据GPID获取GPI映射信息</summary>
        [HttpGet("/api/device/GetGPIMapInfoByGPIID"), MapToApiVersion("1.0")]
        [IngestAuthentication]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<Dto.Response.ResponseMessage<GPIDeviceMapInfo[]>> GetGPIMapInfoByGPIID2(int nGPIID)
        {
            Dto.Response.ResponseMessage<GPIDeviceMapInfo[]> p = new Dto.Response.ResponseMessage<GPIDeviceMapInfo[]>();
            try
            {
                p.extention = (await _deviceManage.GetGPIMapInfoByGPIIDAsync<GPIDeviceMapInfo>(nGPIID)).ToArray();
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

        /// <summary>根据信号源获取绑定的采集参数</summary>
        [HttpGet("api/device/GetCaptureTemplateIDBySignalID"), MapToApiVersion("1.0")]
        [IngestAuthentication]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<Dto.Response.ResponseMessage<int>> GetCaptureTemplateIDBySignalID(int nSignalID)
        {
            Dto.Response.ResponseMessage<int> p = new Dto.Response.ResponseMessage<int>();
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
        [HttpPost("UpdateAllTSDeviceInfos_IN"), MapToApiVersion("1.0")]
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
            async Task Action(UpdateChnExtData_OUT response)
            {
                response.bRet = await _deviceManage.SaveChnExtenddataAsync(pIn.nChnID, pIn.type, pIn.strData);
            }

            return await TryInvoke((Func<UpdateChnExtData_OUT, Task>)Action);
        }


        /// <summary>更改ModifySourceVTRIDAndUserCode</summary>
        [HttpGet("ModifySourceVTRIDAndUserCode"), MapToApiVersion("1.0")]
        [IngestAuthentication]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<ModifySourceVTRIDAndUserCode_out> ModifySourceVTRIDAndUserCode(int nID, int nSourceVTRID, string userCode)
        {
            async Task Action(ModifySourceVTRIDAndUserCode_out response)
            {
                await _deviceManage.ModifySourceVTRIDAndUserCodeAsync(nSourceVTRID, userCode, nID);
            }
            return await TryInvoke((Func<ModifySourceVTRIDAndUserCode_out, Task>)Action);
        }

        //更改ModifySourceVTRIDAndUserCodeByChannelIDArray"
        [HttpGet("ModifySourceVTRIDAndUserCodeByChannelIDArray"), MapToApiVersion("1.0")]
        [IngestAuthentication]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<ModifySourceVTRIDAndUserCode_out> ModifySourceVTRIDAndUserCodeByChannelIDArray(ModifySourceVTR_in pIn)
        {
            async Task Action(ModifySourceVTRIDAndUserCode_out response)
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
            return await TryInvoke((Func<ModifySourceVTRIDAndUserCode_out, Task>)Action);
        }
        #endregion UpdateController

        /// <summary> Try执行 </summary>
        /// <typeparam name="T">返回类型</typeparam>
        /// <param name="action">执行内容</param>
        private async Task<T> TryInvoke<T>(Func<T, Task> action) where T : Base_param, new()
        {
            T response = new T();
            try
            {
                await action(response);
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
                    response.errStr = $"error info:{e}";
                    Logger.Error(response.errStr);
                }
            }
            return response;
        }
    }
}
