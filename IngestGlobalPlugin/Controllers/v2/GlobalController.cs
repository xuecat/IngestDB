using IngestDBCore;
using IngestDBCore.Basic;
using IngestDBCore.Tool;
using IngestGlobalPlugin.Dto;
using IngestGlobalPlugin.Managers;
using IngestGlobalPlugin.Models;
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
    public  class GlobalController : ControllerBase
    {
        private readonly ILogger Logger = LoggerManager.GetLogger("GlobalInfo");
        private readonly GlobalManager _GlobalManager;
        //private readonly RestClient _restClient;

        public GlobalController(GlobalManager global)
        {
            _GlobalManager = global;
            //_restClient = rsc;
        }

        #region lock or unlockobj
        /// <summary>
        /// 锁对象
        /// </summary>
        /// <remarks>
        /// 例子:
        /// Post api/v2/global/objectinfo/lock
        /// </remarks>
        /// <param name="objectParamIn">锁对象参数</param>
        /// <returns>锁对象结果</returns>
        [HttpPost("objectinfo/lock")]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage> PostLockObject([FromBody] DtoLockObjectParamIn objectParamIn)
        {
            ResponseMessage Response = new ResponseMessage();

            try
            {
                if (objectParamIn.ObjectID < 0)
                {
                    Logger.Error("PostLockObject : ObjectID < 0 ,参数传递错误");
                    SobeyRecException.ThrowSelfNoParam(objectParamIn.ObjectID.ToString(), GlobalDictionary.GLOBALDICT_CODE_LOCK_OBJECTID_WRONG, Logger, null);
                }
                if (objectParamIn.ObjectTypeID < OTID.OTID_VTR || objectParamIn.ObjectTypeID > OTID.OTID_OTHER)
                {
                    SobeyRecException.ThrowSelfNoParam(objectParamIn.ObjectTypeID.ToString(), GlobalDictionary.GLOBALDICT_CODE_LOCK_OBJECT_TPYEID_IS_NOT_EXIST, Logger, null);
                }
                if (string.IsNullOrEmpty(objectParamIn.UserName))
                {
                    objectParamIn.UserName = "NullUserName";
                }
                if (objectParamIn.TimeOut < 0)
                {
                    SobeyRecException.ThrowSelfNoParam(objectParamIn.TimeOut.ToString(), GlobalDictionary.GLOBALDICT_CODE_LOCK_OBJECT_TIMEOUT_IS_WRONG, Logger, null);
                }

                bool ret = await _GlobalManager.SetLockObjectAsync(objectParamIn.ObjectID, objectParamIn.ObjectTypeID, objectParamIn.UserName, objectParamIn.TimeOut);

                Response.Code = ret ? ResponseCodeDefines.SuccessCode : ResponseCodeDefines.PartialFailure;
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
                    Logger.Error("PostLockObject : " + Response.Msg);
                }
            }

            return Response;
        }


        /// <summary>
        /// 解锁对象
        /// </summary>
        /// <remarks>
        /// 例子:
        /// Post api/v1/global/objectinfo/unlock
        /// </remarks>
        /// <param name="objectParamIn">锁对象参数</param>
        /// <returns>锁对象结果</returns>
        [HttpPost("objectinfo/unlock")]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage> PostUnlockObject([FromBody] DtoLockObjectParamIn objectParamIn)
        {

            ResponseMessage Response = new ResponseMessage();

            try
            {
                if (objectParamIn.ObjectID < -1)
                {
                    SobeyRecException.ThrowSelfNoParam(objectParamIn.ObjectID.ToString(),  GlobalDictionary.GLOBALDICT_CODE_UNLOCK_OBJECTID_IS_WRONG, Logger, null);
                }
                if (objectParamIn.ObjectTypeID < OTID.OTID_ALL || objectParamIn.ObjectTypeID > OTID.OTID_OTHER)
                {
                    SobeyRecException.ThrowSelfNoParam(objectParamIn.ObjectID.ToString(), GlobalDictionary.GLOBALDICT_CODE_UNLOCK_OBJECT_TYPEID_IS_NOT_EXIST, Logger, null);
                }
                if (objectParamIn.UserName == "" || objectParamIn.UserName == String.Empty)
                {
                    objectParamIn.UserName = "NullUserName";
                }

                bool bRet = await _GlobalManager.SetUnlockObjectAsync(objectParamIn.ObjectID, objectParamIn.ObjectTypeID, objectParamIn.UserName);

                Response.Code = bRet ? ResponseCodeDefines.SuccessCode : ResponseCodeDefines.PartialFailure;
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
        #endregion

        #region Global Controller
        /// <summary>
        /// 获取key对应的字段
        /// </summary>
        /// <remarks>
        /// 例子:
        /// Get api/v2/global/globalvalue
        /// </remarks>
        /// <param name="key">键值</param>
        /// <returns>获取key对应的字段</returns>
        [HttpGet("globalvalue")]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<string>> GetValueString([FromQuery, DefaultValue("DEFAULT_CATALOG")]string key)
        {
            var Response = new ResponseMessage<string>();
            if (string.IsNullOrEmpty(key))
            {
                Response.Code = ResponseCodeDefines.ArgumentNullError;
                Response.Msg = "请求参数不正确";
                return Response;
            }
            try
            {
                Response.Ext = await _GlobalManager.GetValueStringAsync(key);
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
                    Logger.Error("GetValueString : " + Response.Msg);
                }
            }

            return Response;
        }

        /// <summary>
        /// 设置DbpGlobal中globalkey，globalvalue
        /// </summary>
        /// <remarks>
        /// 例子:
        /// Post api/v2/global/globalvalue
        /// </remarks>
        /// <param name="key">global键</param>
        /// <param name="value">global值</param>
        /// <returns></returns>
        [HttpPost("globalvalue")]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage> SetGlobalValue([FromQuery, DefaultValue("DEFAULT_CATALOG")]string key, [FromQuery, DefaultValue("\\Public Material")]string value)
        {
            ResponseMessage Response = new ResponseMessage();
            try
            {
                if (key == null)
                    value = "";
                await _GlobalManager.UpdateGlobalValueAsync(key, value);
                Response.Code = ResponseCodeDefines.SuccessCode;
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
                    Logger.Error("SetGlobalValue : " + Response.Msg);
                }
            }
            return Response;
        }

        ///// <summary>
        ///// 获取dbpGLOBAL中GLOBAL_KEY对应的value,和上面函数可合并
        ///// </summary>
        ///// <remarks>
        ///// 例子:
        ///// Get api/v2/global/defaultstc/{tcMode}
        ///// </remarks>
        ///// <param name="mode">键值</param>
        ///// <returns></returns>
        //[HttpGet("defaultstc/{mode}")]
        //[ApiExplorerSettings(GroupName = "v2")]
        //public async Task<ResponseMessage<GlobalTcResponse>> GetDefaultSTC([FromRoute, DefaultValue(0)]int mode)
        //{
        //    ResponseMessage<GlobalTcResponse> Response = new ResponseMessage<GlobalTcResponse>();

        //    try
        //    {
        //        string strKey = string.Empty;
        //        if ((TC_MODE)mode == TC_MODE.emForLine)
        //            strKey = "DEFAULT_STC_LINE";
        //        else
        //            strKey = "DEFAULT_STC_OTHER";

        //        string tcType = await _GlobalManager.GetValueStringAsync(strKey);
        //        Response.Ext.TcType = (TC_TYPE)Convert.ToInt32(tcType);
        //        Response.Ext.TC = Convert.ToInt32(await _GlobalManager.GetValueStringAsync("PRESET_STC"));

        //        Response.Code = ResponseCodeDefines.SuccessCode;
        //    }
        //    catch (Exception e)
        //    {
        //        if (e.GetType() == typeof(SobeyRecException))//sobeyexcep会自动打印错误
        //        {
        //            SobeyRecException se = e as SobeyRecException;
        //            Response.Code = se.ErrorCode.ToString();
        //            Response.Msg = se.Message;
        //        }
        //        else
        //        {
        //            Response.Code = ResponseCodeDefines.ServiceError;
        //            Response.Msg = "error info：" + e.ToString();
        //            Logger.Error("GetDefaultSTC : " + Response.Msg);
        //        }
        //    }

        //    return Response;

        //}

        /// <summary>
        /// 获取dbpGLOBAL中GLOBAL_KEY对应的value,和上面函数可合并
        /// </summary>
        /// <remarks>
        /// 例子:
        /// Get api/v2/global/defaultstc/{mode}
        /// </remarks>
        /// <param name="mode">0=emForLine,1=emForOther</param>
        /// <returns></returns>
        [HttpGet("defaultstc/{mode}")]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<GlobalTcResponse>> GetDefaultSTCExt([FromRoute, DefaultValue(0)]int mode)
        {
            ResponseMessage<GlobalTcResponse> Response = new ResponseMessage<GlobalTcResponse>();

            try
            {
                Response.Ext = await _GlobalManager.GetDefaultSTCAsync<GlobalTcResponse>((TC_MODE)mode);
                Response.Code = ResponseCodeDefines.SuccessCode;
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
                    Logger.Error("GetDefaultSTCExt : " + Response.Msg);
                }
            }

            return Response;

        }
        #endregion

        #region globalstate Controller
        /// <summary>
        /// 设置GlobalState2
        /// </summary>
        /// <remarks>
        /// 例子:
        /// Post api/v2/global/globalstate
        /// </remarks>
        /// <param name="label">GlobalStateName枚举</param>
        /// <returns></returns>
        [HttpPost("globalstate")]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage> SetGlobalState([FromQuery,DefaultValue("TASK_ADD")]string label)
        {
            var Response = new ResponseMessage();
            if (string.IsNullOrEmpty(label))
            {
                Response.Code = ResponseCodeDefines.ArgumentNullError;
                Response.Msg = "请求参数不正确";
                return Response;
            }
            try
            {
                await _GlobalManager.UpdateGlobalStateAsync(label);
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
                    Logger.Error("SetGlobalState : " + Response.Msg);
                }
            }

            return Response;
        }


        /// <summary>
        /// 获取globalstate表结果
        /// </summary>
        /// <returns>globalstate表结果</returns>
        /// <remarks>
        /// 例子:
        /// Get api/v2/global/globalstate/all
        /// </remarks>
        [HttpGet("globalstate/all")]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<List<DtoGlobalState>>> GetAllGlobalState()
        {
            ResponseMessage<List<DtoGlobalState>> Response = new ResponseMessage<List<DtoGlobalState>>();

            try
            {
                Response.Ext = await _GlobalManager.GetAllGlobalStateAsync<DtoGlobalState>();
                if(Response.Ext.Count < 1)
                {
                    Response.Code = ResponseCodeDefines.PartialFailure;
                    Response.Msg = "No record in the table";
                }
                Response.Code = ResponseCodeDefines.SuccessCode;
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
                    Logger.Error("GetAllGlobalState : " + Response.Msg);
                }
            }

            return Response;

        }

        #endregion

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
                    Response.Msg = "error info：" + e.ToString();
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
                    Response.Msg = "error info：" + e.ToString();
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
                    Response.Msg = "error info：" + e.ToString();
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
                    Response.Msg = "error info：" + e.ToString();
                    Logger.Error("GetParamTemplateStringByID : " + Response.Msg);
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
                    Response.Msg = "error info：" + e.ToString();
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
        public async Task<ResponseMessage> ModifyUserTempalte([FromRoute, DefaultValue(2)] int templateID, [FromBody]  EditUserTemplateRequest UserTemplate)
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
                    Response.Msg = "error info：" + e.ToString();
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
                    Response.Msg = "error info：" + e.ToString();
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
                    Response.Msg = "error info：" + e.ToString();
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
                    Response.Msg = "error info：" + e.ToString();
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
                    Response.Msg = "error info：" + e.ToString();
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
                if(nCaptureParamID != -1)
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
                    Response.Msg = "error info：" + e.ToString();
                    Logger.Error("GetUserHighOrStandardParam : " + Response.Msg);
                }
            }
            return Response;
        }

        #endregion

    }
}
