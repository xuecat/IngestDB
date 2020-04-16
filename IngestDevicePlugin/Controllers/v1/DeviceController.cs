using IngestDBCore;
using IngestDBCore.Basic;
using IngestDevicePlugin.Dto;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

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
            Func<GetAllRouterInPortInfo_param, Task> action = async response =>
            {
                response.inportDescs = await _deviceManage.GetAllRouterInPortAsync<RoterInportDesc>();
                response.nVaildDataCount = response.inportDescs.Count;
            };
            return await TryInvoke(action);
        }

        /// <summary> 获取输出端口与信号源的映射 </summary>
        [HttpGet("GetAllRouterOutPortInfo"), MapToApiVersion("1.0")]
        [IngestAuthentication]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<GetAllRouterOutPortInfo_param> GetAllRouterOutPortInfo()
        {
            Func<GetAllRouterOutPortInfo_param, Task> action = async response =>
            {
                response.outportDescs = await _deviceManage.GetAllRouterOutPortAsync<RoterOutDesc>();
                response.nVaildDataCount = response.outportDescs.Count;
            };
            return await TryInvoke(action);
        }

        /// <summary> 获取所有信号源和采集设备的对应 </summary>
        [HttpGet("GetAllSignalDeviceMap"), MapToApiVersion("1.0")]
        [IngestAuthentication]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<GetAllSignalDeviceMap_param> GetAllSignalDeviceMap()
        {
            Func<GetAllSignalDeviceMap_param, Task> action = async response =>
            {
                response.arrSignalDeviceMap = await _deviceManage.GetAllSignalDeviceMapAsync<SignalDeviceMap>();
                response.nVaildDataCount = response.arrSignalDeviceMap.Count;
            };
            return await TryInvoke(action);
        }

        /// <summary> 获取所有信号源 </summary>
        [HttpGet("GetAllSignalSrcs"), MapToApiVersion("1.0")]
        [IngestAuthentication]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<GetAllSignalSrcs_param> GetAllSignalSrcs()
        {
            Func<GetAllSignalSrcs_param, Task> action = async response =>
            {
                response.signalInfo = await _deviceManage.GetAllSignalSrcsAsync();
                response.nVaildDataCount = response.signalInfo.Count;
            };
            return await TryInvoke(action);
        }

        /// <summary> 获取所有采集通道 </summary>
        [HttpGet("GetAllCaptureChannels"), MapToApiVersion("1.0")]
        [IngestAuthentication]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<GetAllCaptureChannels_param> GetAllCaptureChannels()
        {
            Func<GetAllCaptureChannels_param, Task> action = async response =>
            {
                response.captureChannelInfo = await _deviceManage.GetAllCaptureChannelsAsync();
                response.nVaildDataCount = response.captureChannelInfo.Count;
            };
            return await TryInvoke(action);
        }

        /// <summary> 获取所有的采集设备信息 </summary>
        [HttpGet("GetAllCaptureDevices"), MapToApiVersion("1.0")]
        [IngestAuthentication]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<GetAllCaptureDevices_param> GetAllCaptureDevices()
        {
            Func<GetAllCaptureDevices_param, Task> action = async response =>
            {
                response.arCaptureDeviceList = await _deviceManage.GetAllCaptureDevicesAsync<CaptureDeviceInfo>();
                response.nVaildDataCount = response.arCaptureDeviceList.Count;
            };
            return await TryInvoke(action);
        }

        /// <summary> 获取指定信号源和采集设备的对应 </summary>
        [HttpGet("GetSignalDeviceMapBySignalID"), MapToApiVersion("1.0")]
        [IngestAuthentication]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<GetSignalDeviceMapBySignalID_param> GetSignalDeviceMapBySignalID(int nSignalID)
        {
            Func<GetSignalDeviceMapBySignalID_param, Task> action = async response =>
            {
                var res = await _deviceManage.GetSignalDeviceMapBySignalID(nSignalID);
                if (res != null)
                {
                    response.nDeviceID = res.nDeviceID;
                    response.nDeviceOutPortIdx = res.nOutPortIdx;
                    response.SignalSource = res.SignalSource;
                }
            };
            return await TryInvoke(action);
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
            Func<Base_param, Task> action = async response =>
            {
                await _deviceManage.SetSignalDeviceMap(nSignalID, nDeviceID, nDeviceOutPortIdx, SignalSource);
            };
            var r = await TryInvoke(action);
            return r.errStr;
        }

        /// <summary> 查询所有信号源的扩展信息 </summary>
        [HttpGet("GetAllSignalSrcExs"), MapToApiVersion("1.0")]
        [IngestAuthentication]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<GetAllSignalSrcExs_param> GetAllSignalSrcExs()
        {
            Func<GetAllSignalSrcExs_param, Task> action = async response =>
            {
                response.signalInfo = await _deviceManage.GetAllSignalSrcExsAsync();
                response.nVaildDataCount = response.signalInfo.Count;
            };
            return await TryInvoke(action);
        }

        /// <summary> 根据 信号源Id 查询信号源是否是备份信号源 </summary>
        [HttpGet("GetIsBackupSignalSrcByID"), MapToApiVersion("1.0")]
        [IngestAuthentication]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<string> GetIsBackupSignalSrcByID(int nSignalSrcId)
        {
            Func<GetAllSignalSrcExs_param, Task> action = async response =>
            {
                if (!await _deviceManage.IsBackupSignalSrcByIdAsync(nSignalSrcId))
                {
                    response.errStr = null;
                }
            };
            return (await TryInvoke(action)).errStr;
        }

        /// <summary> 根据 通道Id 获取高清还是标清 nType:0标清,1高清 </summary>
        [HttpGet("GetParamTypeByChannleID"), MapToApiVersion("1.0")]
        [IngestAuthentication]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<GetParamTypeByChannleID_param> GetParamTypeByChannleID(int nChannelID)
        {
            Func<GetParamTypeByChannleID_param, Task> action = async response =>
            {
                response.nType = await _deviceManage.GetParamTypeByChannleIDAsync(nChannelID);
                if (response.nType == -1)
                {
                    response.errStr = "No Such Value!";
                    response.bRet = false;
                }
            };
            return await TryInvoke(action);
        }

        /// <summary> 根据 通道Id 获取MSV设备状态信息 </summary>
        [HttpGet("GetMSVChannelState"), MapToApiVersion("1.0")]
        [IngestAuthentication]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<GetMSVChannelState_param> GetMSVChannelState(int nID)
        {
            Func<GetMSVChannelState_param, Task> action = async response =>
            {
                response.channelStata = await _deviceManage.GetDbpMsvchannelStateAsync(nID);
            };
            return await TryInvoke(action);
        }

        /// <summary> 获得所有信号源分组 </summary>
        [HttpGet("GetAllSignalGroup"), MapToApiVersion("1.0")]
        [IngestAuthentication]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<GetAllSignalGroup_OUT> GetAllSignalGroup()
        {
            Func<GetAllSignalGroup_OUT, Task> action = async response =>
            {
                response.arAllSignalGroup = await _deviceManage.GetAllSignalGroupAsync();
                response.nVaildDataCount = response.arAllSignalGroup.Count;
            };
            return await TryInvoke(action);
        }

        /// <summary> 获取所有信号源分组信息 </summary>
        [HttpGet("GetAllSignalGroupInfo"), MapToApiVersion("1.0")]
        [IngestAuthentication]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<GetAllSignalGroupState_OUT> GetAllSignalGroupInfo()
        {
            Func<GetAllSignalGroupState_OUT, Task> action = async response =>
            {
                response.arAllSignalGroupState = await _deviceManage.GetAllSignalGroupInfoAsync();
                response.nVaildDataCount = response.arAllSignalGroupState.Count;
            };
            return await TryInvoke(action);
        }

        /// <summary> 通过 GPIID 找出该GPI所有的映射 </summary>
        [HttpGet("GetGPIMapInfoByGPIID"), MapToApiVersion("1.0")]
        [IngestAuthentication]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<GetGPIMapInfoByGPIID_OUT> GetGPIMapInfoByGPIID(int nGPIID)
        {
            Func<GetGPIMapInfoByGPIID_OUT, Task> action = async response =>
            {
                response.arGPIDeviceMapInfo = await _deviceManage.GetGPIMapInfoByGPIIDAsync(nGPIID);
                response.nVaildDataCount = response.arGPIDeviceMapInfo.Count;
            };
            return await TryInvoke(action);
        }

        /// <summary> 获取所有节目 </summary>
        //[HttpGet("GetAllProgrammeInfos"), MapToApiVersion("1.0")]
        //[IngestAuthentication]
        //[ApiExplorerSettings(GroupName = "v1")]
        //public GetAllProgrammeInfos_OUT GetAllProgrammeInfos()
        //{
        //    GetAllProgrammeInfos_OUT p = new GetAllProgrammeInfos_OUT();
        //    p.programmeInfos = null;
        //    p.nValidDataCount = 0;
        //    p.errStr = no_err;
        //    try
        //    {
        //        p.errStr = no_err;
        //        p.programmeInfos = DEVICEACCESS.GetAllProgrammeInfos();
        //        if (p.programmeInfos != null)
        //        {
        //            p.nValidDataCount = p.programmeInfos.Length;
        //        }
        //        else
        //        {
        //            p.nValidDataCount = 0;
        //        }

        //        if (p.nValidDataCount == 0)
        //        {
        //            p.programmeInfos = new ProgrammeInfo[1];
        //        }

        //        p.bRet = true;
        //    }
        //    catch (Exception ex)//其他未知的异常，写异常日志
        //    {
        //        p.errStr = ex.Message;
        //        LoggerService.Error("Interface:GetAllProgrammeInfos-> error occur:" + ex.Message);
        //        p.bRet = false;
        //    }
        //    return p;
        //}







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
        #endregion

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
