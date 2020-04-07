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
        [HttpGet("GetAllRouterInPortInfo"), MapToApiVersion("1.0")]
        [IngestAuthentication]//device有点特殊，做了监听端口的所以不能全类检验
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<GetAllRouterInPortInfo_param> GetAllRouterInPortInfo()
        {
            var Response = new GetAllRouterInPortInfo_param()
            {
                bRet = true,
                errStr = "OK",
                nVaildDataCount = 0,
            };

            try
            {
                Response.inportDescs = await _deviceManage.GetAllRouterInPortAsync<RoterInportDesc>();
                Response.nVaildDataCount = Response.inportDescs.Count;
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
                Response.channelInfos = await _deviceManage.GetChannelsByProgrammeIdAsync<CaptureChannelInfo>(programeid);
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
    }
}
