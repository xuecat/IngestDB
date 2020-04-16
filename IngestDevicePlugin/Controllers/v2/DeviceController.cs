using IngestDBCore;
using IngestDBCore.Basic;
using IngestDevicePlugin.Dto;
using IngestTaskPlugin.Managers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Sobey.Core.Log;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

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

        /// <summary>
        /// 使用路由 /routerinport/all
        /// </summary>
        /// <returns></returns>
        [HttpGet("routerinport/all")]
        [IngestAuthentication]//device有点特殊，做了监听端口的所以不能全类检验
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<List<RouterInResponse>>> AllRouterInPortInfo()
        {
            var Response = new ResponseMessage<List<RouterInResponse>>();
            
            try
            {
                Response.Ext = await _deviceManage.GetAllRouterInPortAsync<RouterInResponse>();
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
                    Response.Msg = "error info：" + e.ToString();
                    Logger.Error(Response.Msg);
                }
            }
            return Response;
        }

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
            var Response = new ResponseMessage<List<CaptureChannelInfoResponse>>();

            try
            {
                Response.Ext = await _deviceManage.GetChannelsByProgrammeIdAsync<CaptureChannelInfoResponse>(programmeId, status);
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
                    Response.Msg = "error info：" + e.ToString();
                    Logger.Error(Response.Msg);
                }
            }
            return Response;
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
            var Response = new ResponseMessage<int>();

            try
            {
                Response.Ext = await _deviceManage.GetChannelSignalSrc(channelid);
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
                    Response.Msg = "error info：" + e.ToString();
                    Logger.Error(Response.Msg);
                }
            }
            return Response;
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
            var Response = new ResponseMessage<List<RecUnitMap>>();

            try
            {
                Response.Ext = await _deviceManage.GetAllChannelUnitMap();
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
                    Response.Msg = "error info：" + e.ToString();
                    Logger.Error(Response.Msg);
                }
            }
            return Response;
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
            var Response = new ResponseMessage<int>();

            try
            {
                var f = await _deviceManage.GetChannelUnitMap(channel);
                if (f != null)
                {
                    Response.Ext = f.UnitID;
                }
                else
                    Response.Ext = -1;
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
                    Response.Msg = "error info：" + e.ToString();
                    Logger.Error(Response.Msg);
                }
            }
            return Response;
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
            var Response = new ResponseMessage<ProgrammeInfoResponse>();

            try
            {
                Response.Ext = await _deviceManage.GetBackProgramInfoBySrgid(mastersignalid);
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
                    Response.Msg = "GetBackProgramInfoBySrgid error info：" + e.ToString();
                    Logger.Error(Response.Msg);
                }
            }
            return Response;
        }
    }
}
