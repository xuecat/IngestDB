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


 		/// <remarks>
        /// 例子:
        /// Get api/v2/global/captureparamtemplate/highorstandard
        /// </remarks>
        /// <summary>
        /// 通过用户ID得到用户高清或标清采集参数=
        /// </summary>
        /// <param name="usertoken">用户usertoken</param>
        /// <param name="flag">nFlag:0为标清，1为高清</param>
        /// <returns>采集参数</returns>
        [HttpGet("capturetemplate/param/highorstandard")]
        [ApiExplorerSettings(GroupName = "v3.0")]
        public async Task<ResponseMessage<string>> GetUserHighOrStandardCapParam([FromQuery, DefaultValue("897cd4f79531e3c04c2c9a371e4db4ea")] string usertoken, [FromQuery, DefaultValue(0)] int flag, [FromHeader(Name = "sobeyhive-http-site"), BindRequired, DefaultValue("S1")] string site)//nFlag:0为标清，1为高清
        {
            ResponseMessage<string> Response = new ResponseMessage<string>();
            try
            {
                int nCaptureParamID = -1;

                nCaptureParamID = await _GlobalManager.GetUserParamTemplateIDBySiteAsync(true, usertoken, site);
                if (nCaptureParamID != -1)
                {
                    Response = await GetParamTemplateByID(nCaptureParamID, flag);
                }
                else
                {
                    Response.Ext = null;
                    Response.Code = ResponseCodeDefines.ServiceError;
                }
            }
            catch (System.Exception e)
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
                    Logger.Error("GetUserHighOrStandardParam : " + Response.Msg);
                }
            }
            return Response;
        }

        /// <summary>
        /// 获取captureparamtemplate指定captureid和nflag的值，返回param string
        /// </summary>
        /// <param name="captureparamid">模板id</param>
        /// <param name="flag">hd=0，sd=1,uhd=2标识</param>
        /// <returns>Captureparam结果</returns>
        /// <remarks>
        /// 例子:
        /// Get api/v2/global/captureparamtemplate/{nCaptureParamID}
        /// </remarks>
        [HttpGet("capturetemplate/param/{captureparamid}")]
        [ApiExplorerSettings(GroupName = "v3.0")]
        public async Task<ResponseMessage<string>> GetParamTemplateByID([FromRoute, DefaultValue(1)] int captureparamid, [FromQuery] int? flag)
        {
            ResponseMessage<string> Response = new ResponseMessage<string>();
            try
            {
                //读取采集参数模板
                string temp = await _GlobalManager.GetCapParamTemplateByIDAsync(captureparamid);
                if (flag != null)
                {
                    temp = _GlobalManager.DealCaptureParam(temp, (int)flag);
                }
                Response.Ext = temp;
                if (string.IsNullOrEmpty(Response.Ext))
                {
                    Response.Msg = "There's no CaptureParam!";
                    Response.Code = ResponseCodeDefines.PartialFailure;
                    return Response;
                }
                else
                {
                    Response.Code = ResponseCodeDefines.SuccessCode;
                }

                Logger.Info($"GetParamTemplateByID Site captureparamid : {captureparamid}, flag: {flag}, Result : {Response.Ext}" );

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
                    Logger.Error("GetParamTemplateByID : " + Response.Msg);
                }
            }
            return Response;
        }

    }
}
