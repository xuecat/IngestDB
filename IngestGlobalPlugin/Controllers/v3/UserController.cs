using IngestDBCore;
using IngestGlobalPlugin.Dto.Response;
using IngestGlobalPlugin.Managers;
using Microsoft.AspNetCore.Mvc;
using Sobey.Core.Log;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IngestGlobalPlugin.Controllers.v3
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("2.0")]
    [ApiController]

    public partial class UserController : ControllerBase
    {
        private readonly ILogger Logger = LoggerManager.GetLogger("GlobalInfo3");
        private readonly GlobalManager _GlobalManager;
        //private readonly RestClient _restClient;

        public UserController(GlobalManager global)
        {
            _GlobalManager = global;
            //_restClient = rsc;
        }


        /// <summary>
        /// 获取所有用户登录信息
        /// </summary>
        /// <remarks>
        /// 例子:
        /// Post api/v2/user/userlogininfo/all
        /// </remarks>
        [HttpDelete("logininfo")]
        [ApiExplorerSettings(GroupName = "v3")]
        public async Task<ResponseMessage<List<UserLoginInfoResponse>>> GetAllUserLoginInfos()
        {
            ResponseMessage<List<UserLoginInfoResponse>> Response = new ResponseMessage<List<UserLoginInfoResponse>>();
            try
            {
                var bret = await _GlobalManager.GetAllUserLoginInfo();
                if (bret != null)
                {
                    Response.Code = ResponseCodeDefines.SuccessCode;
                    Response.Ext = bret;
                }
                else
                    Response.Code = ResponseCodeDefines.NotFound;

            }
            catch (Exception e)//其他未知的异常，写异常日志
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
                    Response.Msg = "error info:" + e.Message;
                    Logger.Error("GetAllUserLoginInfos : " + Response.Msg);
                }
            }
            return Response;
        }

    }
}
