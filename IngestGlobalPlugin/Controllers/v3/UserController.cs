using IngestDBCore;
using IngestGlobalPlugin.Managers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Sobey.Core.Log;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;

namespace IngestGlobalPlugin.Controllers.v3
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("3.0")]
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
        /// 获取captureparamtemplate指定captureid的值，返回param
        /// </summary>
        /// <param name="captureparamid">模板id</param>
        /// <returns>Captureparam结果</returns>
        /// <remarks>
        /// 例子:
        /// Get api/v2.1/global/captureparamtemplate/{nCaptureParamID}
        /// </remarks>
        [HttpGet("captureparamtemplate/{captureparamid}")]
        [ApiExplorerSettings(GroupName = "v3")]
        public async Task<ResponseMessage<string>> GetParamTemplateStringByID([FromRoute, BindRequired, DefaultValue(1)] int captureparamid)
        {
            ResponseMessage<string> Response = new ResponseMessage<string>();
            try
            {
                //读取采集参数模板
                string temp = await _GlobalManager.GetCapParamTemplateByIDAsync(captureparamid);
                if (string.IsNullOrEmpty(temp))
                {
                    Response.Msg = "There's no CaptureParam!";
                    Response.Code = ResponseCodeDefines.PartialFailure;
                    return Response;
                }
                Response.Ext = temp;
                if (string.IsNullOrEmpty(Response.Ext))
                {
                    Response.Code = ResponseCodeDefines.NotFound;
                    Response.Msg = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}:error info: not find data!";
                }
                else
                    Response.Code = ResponseCodeDefines.SuccessCode;
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
                    Logger.Error("GetParamTemplateStringByID : " + Response.Msg);
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
        [HttpGet("captureparamtemplate/highorstandard")]
        [ApiExplorerSettings(GroupName = "v3")]
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
        [HttpGet("captureparamtemplate/flag/{captureparamid}")]
        [ApiExplorerSettings(GroupName = "v3")]
        public async Task<ResponseMessage<string>> GetParamTemplateByID([FromRoute, DefaultValue(1)] int captureparamid, [FromQuery, DefaultValue(0)] int flag)
        {
            ResponseMessage<string> Response = new ResponseMessage<string>();
            try
            {
                //读取采集参数模板
                string temp = await _GlobalManager.GetCapParamTemplateByIDAsync(captureparamid);
                temp = _GlobalManager.DealCaptureParam(temp, flag);
                if (string.IsNullOrEmpty(temp))
                {
                    Response.Msg = "There's no CaptureParam!";
                    Response.Code = ResponseCodeDefines.PartialFailure;
                    return Response;
                }
                Response.Ext = temp;
                if (string.IsNullOrEmpty(Response.Ext))
                {
                    Response.Code = ResponseCodeDefines.NotFound;
                    Response.Msg = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}:error info: not find data!";
                }
                else
                    Response.Code = ResponseCodeDefines.SuccessCode;
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
