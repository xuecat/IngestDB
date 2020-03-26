using IngestDBCore;
using IngestDBCore.Basic;
using IngestDevicePlugin.Dto.Response;
using IngestTaskPlugin.Managers;
using Microsoft.AspNetCore.Mvc;
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

        public string Get()
        {

            return "DBPlatform Service is already startup at " + DateTime.Now.ToString();
        }

        /// <summary>
        /// 使用路由 /taskmetadata/{taskid}?type=1
        /// </summary>
        /// <param name="testinfo"></param>
        /// <returns></returns>
        [HttpGet("routerinport")]
        [IngestAuthentication]//device有点特殊，做了监听端口的所以不能全类检验
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
    }
}
