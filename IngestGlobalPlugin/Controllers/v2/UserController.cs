using IngestDBCore;
using IngestDBCore.Basic;
using IngestGlobalPlugin.Dto;
using IngestGlobalPlugin.Managers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Sobey.Core.Log;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using EditUserTemplateRequest = IngestGlobalPlugin.Dto.EditUserTemplate;

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
        public async Task<ResponseMessage> Post_SetUserSetting([FromBody]DtoSetUserSetting_IN userSettingIn)
        {
            ResponseMessage Response = new ResponseMessage();
            try
            {
                if (string.IsNullOrWhiteSpace(userSettingIn.UserCode))
                {
                    Response.Msg = "The usercode is null.";
                    Response.Code = ResponseCodeDefines.ArgumentNullError;
                    return Response;
                }
                if (string.IsNullOrWhiteSpace(userSettingIn.Settingtype))
                {
                    Response.Msg = "The setting type is null.";
                    Response.Code = ResponseCodeDefines.ArgumentNullError;
                    return Response;
                }

                await _GlobalManager.UpdateUserSettingAsync(userSettingIn.UserCode, userSettingIn.Settingtype, userSettingIn.SettingText);
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
                    Response.Msg = "error info：" + e.Message;
                    Logger.Error("Post_SetUserSetting : " + Response.Msg);
                }
            }
            return Response;
        }

        /// <summary>
        /// 通过用户编码获取配置信息
        /// </summary>
        /// <param name="userCode">usercode</param>
        /// <param name="settingtype">type</param>
        /// <returns> extention为strSettingText </returns>
        /// <remarks>
        /// 例子:
        /// Get api/v2/global/usersetting
        /// </remarks>
        [HttpGet("usersetting")]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<string>> GetUserSetting([FromQuery, DefaultValue("06c70a52172d4393beb1bb6743ca6944")]string userCode, [FromQuery, DefaultValue("UserMoudleData")]string settingtype)
        {
            ResponseMessage<string> Response = new ResponseMessage<string>();
            try
            {
                Response.Ext = await _GlobalManager.GetUserSettingAsync(userCode, settingtype);
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
                    Response.Msg = "error info：" + e.Message;
                    Logger.Error("GetUserSetting : " + Response.Msg);
                }
            }
            return Response;
        }
        #endregion

        #region ParamTemplate

        /// <summary>
        /// 获取captureparamtemplate指定captureid和nflag的值，返回param
        /// </summary>
        /// <param name="captureParamID">模板id</param>
        /// <param name="flag">hd=0，sd=1,uhd=2标识</param>
        /// <returns>Captureparam结果</returns>
        /// <remarks>
        /// 例子:
        /// Get api/v2/global/captureparamtemplate/{nCaptureParamID}
        /// </remarks>
        [HttpGet("captureparamtemplate/{captureParamID}")]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<string>> GetParamTemplateByID([FromRoute, DefaultValue(1)]int captureParamID, [FromQuery, DefaultValue(0)]int flag)
        {
            ResponseMessage<string> Response = new ResponseMessage<string>();
            try
            {
                //读取采集参数模板
                string temp = await _GlobalManager.GetCapParamTemplateByIDAsync(captureParamID);
                temp = _GlobalManager.DealCaptureParam(temp, flag);
                if (string.IsNullOrEmpty(temp))
                {
                    Response.Msg = "There's no CaptureParam!";
                    Response.Code = ResponseCodeDefines.PartialFailure;
                    return Response;
                }
                Response.Ext = temp;
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
                    Response.Msg = "error info：" + e.Message;
                    Logger.Error("GetParamTemplateByID : " + Response.Msg);
                }
            }
            return Response;
        }

        /// <summary>
        /// 获取captureparamtemplate指定captureid的值，返回param
        /// </summary>
        /// <param name="CaptureParamID">模板id</param>
        /// <returns>Captureparam结果</returns>
        /// <remarks>
        /// 例子:
        /// Get api/v2/global/captureparamtemplate/{nCaptureParamID}
        /// </remarks>
        [HttpGet("captureparamtemplatestring/{CaptureParamID}")]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<string>> GetParamTemplateStringByID([FromRoute, BindRequired, DefaultValue(1)]int CaptureParamID)
        {
            ResponseMessage<string> Response = new ResponseMessage<string>();
            try
            {
                //读取采集参数模板
                string temp = await _GlobalManager.GetCapParamTemplateByIDAsync(CaptureParamID);
                if (string.IsNullOrEmpty(temp))
                {
                    Response.Msg = "There's no CaptureParam!";
                    Response.Code = ResponseCodeDefines.PartialFailure;
                    return Response;
                }
                Response.Ext = temp;
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
                    Response.Msg = "error info：" + e.Message;
                    Logger.Error("GetParamTemplateStringByID : " + Response.Msg);
                }
            }
            return Response;
        }

        /// <summary>
        /// 添加或者更新captureparamtemplate，返回paramid
        /// </summary>
        /// <param name="CaptureParamID">模板id</param>
        /// <param name="templatename">模板名字</param>
        /// <param name="template">模板内容</param>
        /// <returns>Captureparam结果id</returns>
        /// <remarks>
        /// 例子:
        /// Get api/v2/global/captureparamtemplate/{nCaptureParamID}
        /// </remarks>
        [HttpPost("captureparamtemplatestring/{CaptureParamID}")]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<int>> UpdateParamTemplate(
            [FromRoute, BindRequired, DefaultValue(1)]int CaptureParamID,
            [FromQuery]string templatename,
            [FromQuery]string template)
        {
            ResponseMessage<int> Response = new ResponseMessage<int>();
            try
            {
                //读取采集参数模板
                Response.Ext = await _GlobalManager.ModifyCaptureParamTemplateAsync(CaptureParamID, templatename, template);
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
                    Response.Msg = "error info：" + e.Message;
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
        /// <param name="userTemplate">用户模板信息</param>
        /// <returns>extention 为用户模版ID</returns>
        [HttpPost("usertemplate/add")]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<int>> AddUserTemplate([FromBody] DtoUserTemplate userTemplate)
        {
            ResponseMessage<int> Response = new ResponseMessage<int>();

            if (userTemplate == null)
            {
                Response.Msg = "userTemplate is null.";
                Response.Code = ResponseCodeDefines.ArgumentNullError;
                return Response;
            }
            if (userTemplate.TemplateID > 0)
            {
                Response.Msg = "Template ID is larger than 0";
                Response.Code = ResponseCodeDefines.ArgumentNullError;
                return Response;
            }
            if (userTemplate.UserCode == string.Empty ||
                 userTemplate.TemplateName == string.Empty)
            {
                Response.Msg = "UserCode or TemplateName is null.";
                Response.Code = ResponseCodeDefines.ArgumentNullError;
                return Response;
            }
            try
            {
                Response.Ext = await _GlobalManager.UserTemplateInsertAsync(userTemplate.TemplateID, userTemplate.UserCode, userTemplate.TemplateName, userTemplate.TemplateContent);
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
                    Response.Msg = "error info：" + e.Message;
                    Logger.Error("AddUserTemplate : " + Response.Msg);
                }
            }
            return Response;
        }

        /// <summary>
        /// 根据模板ID修改模板内容
        /// </summary>
        /// <param name="templateID">模板id</param>
        /// <param name="UserTemplate">修改的用户模板信息</param>
        /// <returns>标准返回信息</returns>
        /// <remarks>
        /// 例子:
        /// Put api/v2/global/usertemplate/modify/{nTemplateID}
        /// </remarks>
        [HttpPut("usertemplate/modify/{templateID}")]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage> ModifyUserTempalte([FromRoute, DefaultValue(2)] int templateID, [FromBody]EditUserTemplateRequest UserTemplate)
        {
            ResponseMessage Response = new ResponseMessage();

            if (templateID <= 0)
            {
                Response.Msg = "TemplateID is smaller or equal 0.";
                Response.Code = ResponseCodeDefines.ArgumentNullError;
            }
            try
            {
                await _GlobalManager.ModifyUserTemplateAsync(templateID, UserTemplate.TemplateContent, UserTemplate.TemplateName);
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
                    Response.Msg = "error info：" + e.Message;
                    Logger.Error("ModifyUserTempalte : " + Response.Msg);
                }
            }
            return Response;

        }


        /// <remarks>
        /// 例子:
        /// Get api/v2/global/usertemplate/all
        /// </remarks>
        /// <param name="userCode">用户Code</param>
        /// <returns>extension 为 获取到的模板数组</returns>
        [HttpGet("usertemplate/all")]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<List<DtoUserTemplate>>> GetUserAllTemplatesByUserCode([FromQuery, DefaultValue("8de083d45c614628b99516740d628e91")] string userCode)
        {
            ResponseMessage<List<DtoUserTemplate>> Response = new ResponseMessage<List<DtoUserTemplate>>();

            if (userCode == string.Empty)
            {
                Response.Msg = "UserCode is null.";
                Response.Code = ResponseCodeDefines.ArgumentNullError;
            }
            try
            {
                Response.Ext = await _GlobalManager.GetUserAllTemplatesAsync<DtoUserTemplate>(userCode);
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
                    Response.Msg = "error info：" + e.Message;
                    Logger.Error("GetUserAllTemplatesByUserCode : " + Response.Msg);
                }
            }
            return Response;
        }

        /// <summary>
        /// 通过iD删除usertemplate
        /// </summary>
        /// <param name="templateID">模板id</param>
        /// <returns></returns>
        /// <remarks>
        /// 例子:
        /// Delete api/v2/global/usertemplate/delete/{templateID}
        /// </remarks>
        [HttpDelete("usertemplate/delete/{templateID}")]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage> DeleteUserTemplateByID([FromRoute, DefaultValue(2)]int templateID)
        {
            ResponseMessage Response = new ResponseMessage();

            try
            {
                if (templateID <= 0)
                {
                    Response.Msg = "TemplateID is smaller or equal 0.";
                    Response.Code = ResponseCodeDefines.ArgumentNullError;
                }
                await _GlobalManager.DeleteUserTemplateAsync(templateID);
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
                    Response.Msg = "error info：" + e.Message;
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
        /// <param name="userCode">UserParamMap的用户Code</param>
        /// <returns>删除结果</returns>
        [HttpDelete("userparammap/delete")]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage> DeleteUserParamTemplateByUserCode([FromQuery, DefaultValue("ingest01")]string userCode)
        {
            ResponseMessage Response = new ResponseMessage();

            try
            {
                if (string.IsNullOrEmpty(userCode))
                {
                    Response.Msg = "UserCode is Empty.";
                    Response.Code = ResponseCodeDefines.ArgumentNullError;
                    return Response;
                }
                await _GlobalManager.DelUserParamTemplateAsync(userCode);
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
                    Response.Msg = "error info：" + e.Message;
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
        /// <param name="userCode">用户Code</param>
        /// <returns>取到的用户信息</returns>
        /// <remarks>
        /// 例子:
        /// Get api/v2/global/userinfo
        /// </remarks>
        [HttpGet("userinfo")]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<DtoCMUserInfo>> GetUserInfoByCode([FromQuery, DefaultValue("897cd4f79531e3c04c2c9a371e4db4ea")]string userCode)
        {
            ResponseMessage<DtoCMUserInfo> Response = new ResponseMessage<DtoCMUserInfo>();
            try
            {
                Response = await _GlobalManager.GetUserInfoByUserCodeAsync<DtoCMUserInfo>(userCode);
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
                    Response.Msg = "error info：" + e.Message;
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
        /// <param name="userToken">用户usertoken</param>
        /// <param name="flag">nFlag：0为标清，1为高清</param>
        /// <returns>采集参数</returns>
        [HttpGet("captureparamtemplate/highorstandard")]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<string>> GetUserHighOrStandardCapParam([FromQuery, DefaultValue("897cd4f79531e3c04c2c9a371e4db4ea")]string userToken, [FromQuery, DefaultValue(0)]int flag)//nFlag：0为标清，1为高清
        {
            ResponseMessage<string> Response = new ResponseMessage<string>();
            try
            {
                int nCaptureParamID = -1;

                nCaptureParamID = await _GlobalManager.GetUserParamTemplateIDAsync(true, userToken);
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
                    Response.Msg = "error info：" + e.Message;
                    Logger.Error("GetUserHighOrStandardParam : " + Response.Msg);
                }
            }
            return Response;
        }

        #endregion
    }
}
