using IngestDBCore;
using IngestDBCore.Basic;
using IngestGlobalPlugin.Dto.Response;
using IngestGlobalPlugin.Managers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Sobey.Core.Log;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using EditUserTemplateRequest = IngestGlobalPlugin.Dto.Response.EditUserTemplate;

namespace IngestGlobalPlugin.Controllers.v2
{

    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("2.0")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ILogger Logger = LoggerManager.GetLogger("GlobalInfo");
        private readonly GlobalManager _GlobalManager;
        //private readonly RestClient _restClient;

        public UserController(GlobalManager global)
        {
            _GlobalManager = global;
            //_restClient = rsc;
        }

        #region User
        /// <summary>
        /// 修改usersetting
        /// </summary>
        /// <remarks>
        /// 例子:
        /// Post api/v2/global/usersetting
        /// </remarks>
        [HttpPost("usersetting")]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage> Post_SetUserSetting([FromBody, BindRequired]DtoSetUserSettingRequest usersetting)
        {
            ResponseMessage Response = new ResponseMessage();
            try
            {
                if (string.IsNullOrWhiteSpace(usersetting.UserCode))
                {
                    Response.Msg = "The usercode is null.";
                    Response.Code = ResponseCodeDefines.ArgumentNullError;
                    return Response;
                }
                if (string.IsNullOrWhiteSpace(usersetting.Settingtype))
                {
                    Response.Msg = "The setting type is null.";
                    Response.Code = ResponseCodeDefines.ArgumentNullError;
                    return Response;
                }

                await _GlobalManager.UpdateUserSettingAsync(usersetting.UserCode, usersetting.Settingtype, usersetting.SettingText);
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
                    Logger.Error("Post_SetUserSetting : " + Response.Msg);
                }
            }
            return Response;
        }

        /// <summary>
        /// 通过用户编码获取配置信息
        /// </summary>
        /// <param name="usercode">usercode</param>
        /// <param name="settingtype">type</param>
        /// <returns> extention为strSettingText </returns>
        /// <remarks>
        /// 例子:
        /// Get api/v2/global/usersetting
        /// </remarks>
        [HttpGet("usersetting")]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<string>> GetUserSetting([FromQuery, DefaultValue("06c70a52172d4393beb1bb6743ca6944")]string usercode, [FromQuery, DefaultValue("UserMoudleData")]string settingtype)
        {
            ResponseMessage<string> Response = new ResponseMessage<string>();
            try
            {
                Response.Ext = await _GlobalManager.GetUserSettingAsync(usercode, settingtype);
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
                    Logger.Error("GetUserSetting : " + Response.Msg);
                }
            }
            return Response;
        }
        #endregion

        #region ParamTemplate

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
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<string>> GetParamTemplateByID([FromRoute, DefaultValue(1)]int captureparamid, [FromQuery, DefaultValue(0)]int flag)
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
                    Response.Msg = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}:error info: 获取数据为空!";
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

        /// <summary>
        /// 获取captureparamtemplate指定captureid的值，返回param
        /// </summary>
        /// <param name="captureparamid">模板id</param>
        /// <returns>Captureparam结果</returns>
        /// <remarks>
        /// 例子:
        /// Get api/v2/global/captureparamtemplate/{nCaptureParamID}
        /// </remarks>
        [HttpGet("captureparamtemplate/{captureparamid}")]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<string>> GetParamTemplateStringByID([FromRoute, BindRequired, DefaultValue(1)]int captureparamid)
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
                    Response.Msg = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}:error info: 获取数据为空!";
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

        /// <summary>
        /// 添加或者更新captureparamtemplate，返回paramid
        /// </summary>
        /// <param name="captureparamid">模板id</param>
        /// <param name="templatename">模板名字</param>
        /// <param name="template">模板内容</param>
        /// <returns>Captureparam结果id</returns>
        /// <remarks>
        /// 例子:
        /// Get api/v2/global/captureparamtemplate/{nCaptureParamID}
        /// </remarks>
        [HttpPost("captureparamtemplate/{captureparamid}")]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<int>> UpdateParamTemplate(
            [FromRoute, BindRequired, DefaultValue(1)]int captureparamid,
            [FromQuery]string templatename,
            [FromQuery]string template)
        {
            ResponseMessage<int> Response = new ResponseMessage<int>();
            try
            {
                //读取采集参数模板
                Response.Ext = await _GlobalManager.ModifyCaptureParamTemplateAsync(captureparamid, templatename, template);
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
                    Logger.Error("UpdateParamTemplate : " + Response.Msg);
                }
            }
            return Response;
        }
        #endregion

        #region UserTemplate
        /// <summary>
        /// 增加一个新的用户模板
        /// </summary>
        /// <remarks>
        /// 例子:
        /// Post api/v2/global/usertemplate/add
        /// </remarks>
        /// <param name="usertemplate">用户模板信息</param>
        /// <returns>extention 为用户模版ID</returns>
        [HttpPost("usertemplate/add")]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<int>> AddUserTemplate([FromBody] DtoUserTemplate usertemplate)
        {
            ResponseMessage<int> Response = new ResponseMessage<int>();

            if (usertemplate == null)
            {
                Response.Msg = "userTemplate is null.";
                Response.Code = ResponseCodeDefines.ArgumentNullError;
                return Response;
            }
            if (usertemplate.TemplateId > 0)
            {
                Response.Msg = "Template ID is larger than 0";
                Response.Code = ResponseCodeDefines.ArgumentNullError;
                return Response;
            }
            if (usertemplate.UserCode == string.Empty ||
                 usertemplate.TemplateName == string.Empty)
            {
                Response.Msg = "UserCode or TemplateName is null.";
                Response.Code = ResponseCodeDefines.ArgumentNullError;
                return Response;
            }
            try
            {
                Response.Ext = await _GlobalManager.UserTemplateInsertAsync(usertemplate.TemplateId, usertemplate.UserCode, usertemplate.TemplateName, usertemplate.TemplateContent);
                Response.Code = ResponseCodeDefines.SuccessCode;
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
                    Logger.Error("AddUserTemplate : " + Response.Msg);
                }
            }
            return Response;
        }

        /// <summary>
        /// 根据模板ID修改模板内容
        /// </summary>
        /// <param name="templateid">模板id</param>
        /// <param name="usertemplate">修改的用户模板信息</param>
        /// <returns>标准返回信息</returns>
        /// <remarks>
        /// 例子:
        /// Put api/v2/global/usertemplate/modify/{nTemplateID}
        /// </remarks>
        [HttpPut("usertemplate/modify/{templateid}")]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage> ModifyUserTempalte([FromRoute, DefaultValue(2)] int templateid, [FromBody]EditUserTemplateRequest usertemplate)
        {
            ResponseMessage Response = new ResponseMessage();

            if (templateid <= 0)
            {
                Response.Msg = "TemplateID is smaller or equal 0.";
                Response.Code = ResponseCodeDefines.ArgumentNullError;
            }
            try
            {
                await _GlobalManager.ModifyUserTemplateAsync(templateid, usertemplate.TemplateContent, usertemplate.TemplateName);
                Response.Code = ResponseCodeDefines.SuccessCode;
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
                    Logger.Error("ModifyUserTempalte : " + Response.Msg);
                }
            }
            return Response;

        }


        /// <remarks>
        /// 例子:
        /// Get api/v2/global/usertemplate/all
        /// </remarks>
        /// <param name="usercode">用户Code</param>
        /// <returns>extension 为 获取到的模板数组</returns>
        [HttpGet("usertemplate/all")]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<List<DtoUserTemplate>>> GetUserAllTemplatesByUserCode([FromQuery, DefaultValue("8de083d45c614628b99516740d628e91")] string usercode)
        {
            ResponseMessage<List<DtoUserTemplate>> Response = new ResponseMessage<List<DtoUserTemplate>>();

            if (usercode == string.Empty)
            {
                Response.Msg = "UserCode is null.";
                Response.Code = ResponseCodeDefines.ArgumentNullError;
            }
            try
            {
                Response.Ext = await _GlobalManager.GetUserAllTemplatesAsync<DtoUserTemplate>(usercode);
                Response.Code = ResponseCodeDefines.SuccessCode;
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
                    Logger.Error("GetUserAllTemplatesByUserCode : " + Response.Msg);
                }
            }
            return Response;
        }

        /// <summary>
        /// 通过iD删除usertemplate
        /// </summary>
        /// <param name="templateid">模板id</param>
        /// <returns></returns>
        /// <remarks>
        /// 例子:
        /// Delete api/v2/global/usertemplate/delete/{templateID}
        /// </remarks>
        [HttpDelete("usertemplate/delete/{templateID}")]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage> DeleteUserTemplateByID([FromRoute, DefaultValue(2)]int templateid)
        {
            ResponseMessage Response = new ResponseMessage();

            try
            {
                if (templateid <= 0)
                {
                    Response.Msg = "TemplateID is smaller or equal 0.";
                    Response.Code = ResponseCodeDefines.ArgumentNullError;
                }
                await _GlobalManager.DeleteUserTemplateAsync(templateid);
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
                    Logger.Error("DeleteUserTemplateByID : " + Response.Msg);
                }
            }
            return Response;
        }

        /// <remarks>
        /// 例子:
        /// Get api/v2/global/userparammap/delete
        /// </remarks>
        /// <summary>
        /// 删除用户Param映射关系UserCode-CaptureParamId
        /// </summary>
        /// <param name="usercode">UserParamMap的用户Code</param>
        /// <returns>删除结果</returns>
        [HttpDelete("userparammap/delete")]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage> DeleteUserParamTemplateByUserCode([FromQuery, DefaultValue("ingest01")]string usercode)
        {
            ResponseMessage Response = new ResponseMessage();

            try
            {
                if (string.IsNullOrEmpty(usercode))
                {
                    Response.Msg = "UserCode is Empty.";
                    Response.Code = ResponseCodeDefines.ArgumentNullError;
                    return Response;
                }
                await _GlobalManager.DelUserParamTemplateAsync(usercode);
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
                    Logger.Error("DeleteUserParamTemplateByUserCode : " + Response.Msg);
                }
            }
            return Response;
        }


        #endregion

        #region CMApi

        /// <summary>
        /// 获得userinfo通过code
        /// </summary>
        /// <param name="usercode">用户Code</param>
        /// <returns>取到的用户信息</returns>
        /// <remarks>
        /// 例子:
        /// Get api/v2/global/userinfo
        /// </remarks>
        [HttpGet("userinfo")]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<DtoCMUserInfo>> GetUserInfoByCode([FromQuery, DefaultValue("897cd4f79531e3c04c2c9a371e4db4ea")]string usercode)
        {
            ResponseMessage<DtoCMUserInfo> Response = new ResponseMessage<DtoCMUserInfo>();
            try
            {
                Response = await _GlobalManager.GetUserInfoByUserCodeAsync<DtoCMUserInfo>(usercode);
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
                    Logger.Error("GetUserInfoByCode : " + Response.Msg);
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
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<string>> GetUserHighOrStandardCapParam([FromQuery, DefaultValue("897cd4f79531e3c04c2c9a371e4db4ea")]string usertoken, [FromQuery, DefaultValue(0)]int flag)//nFlag:0为标清，1为高清
        {
            ResponseMessage<string> Response = new ResponseMessage<string>();
            try
            {
                int nCaptureParamID = -1;

                nCaptureParamID = await _GlobalManager.GetUserParamTemplateIDAsync(true, usertoken);
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

        #endregion
    }
}
