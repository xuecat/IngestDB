using System;
using System.Threading.Tasks;
using IngestDBCore;
using IngestDBCore.Basic;
using IngestDevicePlugin.Dto;
using Microsoft.AspNetCore.Mvc;

namespace IngestDevicePlugin.Controllers
{
    public partial class DeviceController : ControllerBase
    {
        //private readonly ILogger Logger = LoggerManager.GetLogger("TaskInfo");
        //private readonly TaskManager _monthManage;
        //private readonly RestClient _restClient;

        #region GetController

        /// <summary> 获取输入端口与信号源的映射 </summary>
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
                response.arGPIDeviceMapInfo = await _deviceManage.GetGPIMapInfoByGPIIDAsync(nGPIID);
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


        /// <summary>获取该信号源的备份信号源ID</summary>
        [HttpGet("GetParamTypeBySignalID"), MapToApiVersion("1.0")]
        [IngestAuthentication]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<GetParamTypeBySignalID_OUT> GetParamTypeBySignalID(int nSignalID)//nType:0标清,1高清
        {
            async Task Action(GetParamTypeBySignalID_OUT response)
            {
                response.nType = await _deviceManage.GetParamTypeByChannleIDAsync(nSignalID);
                if (response.nType == -1)
                {
                    response.bRet = false;
                    response.errStr = "No Such Value!";
                }
            }
            return await TryInvoke((Func<GetParamTypeBySignalID_OUT, Task>)Action);
        }

        //根据节目ID获取相应的通道，有矩阵模式和无矩阵模式的区别
        [HttpGet("GetChannelsByProgrammeId"), MapToApiVersion("1.0")]
        [IngestAuthentication]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<GetChannelsByProgrammeId_out> GetChannelsByProgrammeId(int programmeId)
        {
            async Task Action(GetChannelsByProgrammeId_out response)
            {
                response.channelInfos = await _deviceManage.GetChannelsByProgrammeIdAsync<CaptureChannelInfo>(programmeId, 0);
                response.validCount = response.channelInfos.Count;
            }
            return await TryInvoke((Func<GetChannelsByProgrammeId_out, Task>)Action);
        }

        //[HttpGet("GetChannelsByProgrammeId"), MapToApiVersion("1.0")]
        //[IngestAuthentication]//device有点特殊，做了监听端口的所以不能全类检验
        //[ApiExplorerSettings(GroupName = "v1")]
        //public async Task<GetChannelsByProgrammeId_out> GetChannelsByProgrammeId(int programeid)
        //{
        //    var Response = new GetChannelsByProgrammeId_out()
        //    {
        //        bRet = true,
        //        errStr = "OK",
        //    };

        //    try
        //    {
        //        Response.channelInfos = await _deviceManage.GetChannelsByProgrammeIdAsync<CaptureChannelInfo>(programeid, 0);
        //    }
        //    catch (Exception e)
        //    {
        //        if (e.GetType() == typeof(SobeyRecException))//sobeyexcep会自动打印错误
        //        {
        //            SobeyRecException se = e as SobeyRecException;
        //            Response.bRet = false;
        //            Response.errStr = se.Message;
        //        }
        //        else
        //        {
        //            Response.bRet = false;
        //            Response.errStr = "error info：" + e.ToString();
        //            Logger.Error(Response.errStr);
        //        }
        //    }
        //    return Response;
        //}

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
                response.bRet = await _deviceManage.SaveChnExtenddataAsync(pIn);
            }

            return await TryInvoke((Func<UpdateChnExtData_OUT, Task>)Action);
        }

        #endregion UpdateController

        /// <summary> Try执行 </summary>
        /// <typeparam name="T">返回类型</typeparam>
        /// <param name="action">执行内容</param>
        private async Task<T> TryInvoke<T>(Func<T, Task> action) where T : Base_param
        {
            T Response = default;
            try
            {
                await action(Response);
            }
            catch (Exception e)
            {
                if (e.GetType() == typeof(SobeyRecException))//sobeyexcep会自动打印错误
                {
                    SobeyRecException se = e as SobeyRecException;
                    Response.bRet = false;
                    Response.errStr = se.Message;
                }
                else
                {
                    Response.bRet = false;
                    Response.errStr = $"error info:{e.ToString()}";
                    Logger.Error(Response.errStr);
                }
            }
            return Response;
        }
    }
}
