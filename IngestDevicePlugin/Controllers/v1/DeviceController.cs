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
        public async Task<GetSignalDeviceMapBySignalID_param> GetSignalDeviceMapBySignalID(int nSignalID)
        {
            Func<GetSignalDeviceMapBySignalID_param, Task> action = async response =>
            {
                _deviceManage.GetSignalDeviceMapBySignalID(nSignalID,ref response.nDeviceID,ref response.nDeviceOutPortIdx, ref response.SignalSource);
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
        public async Task<string> GetSetSignalDeviceMap(int nSignalID, int nDeviceID, int nDeviceOutPortIdx, emSignalSource SignalSource)
        {
            Func<Base_param, Task> action = async response =>
            {
                await _deviceManage.SetSignalDeviceMap(nSignalID, nDeviceID, nDeviceOutPortIdx, SignalSource);
            };
            var r = await TryInvoke(action);
            return r.errStr;
        }

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
<<<<<<< HEAD


        [HttpGet("GetChannelsByProgrammeId"), MapToApiVersion("1.0")]
        [IngestAuthentication]//device有点特殊，做了监听端口的所以不能全类检验
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<GetChannelsByProgrammeId_out> GetChannelsByProgrammeId(int programeid)
        {
            var Response = new GetChannelsByProgrammeId_out()
            {
                bRet = true,
                errStr = "OK",
            };

            try
            {
                Response.channelInfos = await _deviceManage.GetChannelsByProgrammeIdAsync<CaptureChannelInfo>(programeid, 0);
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
                    Response.errStr = "error info：" + e.ToString();
                    Logger.Error(Response.errStr);
                }
            }
            return Response;
        }
=======
        #endregion

>>>>>>> c54df3525cc4842bedd1e0e0873a87b1235b4529
    }
}
