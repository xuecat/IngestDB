using IngestDBCore;
using IngestDBCore.Basic;
using IngestDBCore.Tool;
using IngestGlobalPlugin.Dto;
using IngestGlobalPlugin.Managers;
using IngestGlobalPlugin.Models;
using Microsoft.AspNetCore.Mvc;
using Sobey.Core.Log;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IngestGlobalPlugin.Controllers
{

    [IngestAuthentication]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    [ApiController]
    public partial class GlobalController : ControllerBase
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
        /// Post api/v2/objectinfo/lock
        /// </remarks>
        /// <param name="pIn">锁对象参数</param>
        /// <returns>锁对象结果</returns>
        [HttpPost("objectinfo/lock")]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage> PostLockObject([FromBody] PostLockObject_param_in pIn)
        {
            ResponseMessage Response = new ResponseMessage();

            try
            {
                if (pIn.ObjectID < 0)
                {
                    Logger.Error("PostLockObject : ObjectID < 0 ,参数传递错误");
                    SobeyRecException.ThrowSelfNoParam(pIn.ObjectID.ToString(), GlobalDictionary.GLOBALDICT_CODE_LOCK_OBJECTID_WRONG, Logger, null);
                }
                if (pIn.ObjectTypeID < OTID.OTID_VTR || pIn.ObjectTypeID > OTID.OTID_OTHER)
                {
                    SobeyRecException.ThrowSelfNoParam(pIn.ObjectTypeID.ToString(), GlobalDictionary.GLOBALDICT_CODE_LOCK_OBJECT_TPYEID_IS_NOT_EXIST, Logger, null);
                }
                if (string.IsNullOrEmpty(pIn.userName))
                {
                    pIn.userName = "NullUserName";
                }
                if (pIn.TimeOut < 0)
                {
                    SobeyRecException.ThrowSelfNoParam(pIn.TimeOut.ToString(), GlobalDictionary.GLOBALDICT_CODE_LOCK_OBJECT_TIMEOUT_IS_WRONG, Logger, null);
                }

                bool ret = await _GlobalManager.SetLockObjectAsync(pIn.ObjectID, pIn.ObjectTypeID, pIn.userName, pIn.TimeOut);

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
        /// Post api/v1/objectinfo/unlock
        /// </remarks>
        /// <param name="pIn">锁对象参数</param>
        /// <returns>锁对象结果</returns>
        [HttpPost("objectinfo/unlock")]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage> PostUnlockObject([FromBody] PostLockObject_param_in pIn)
        {

            ResponseMessage Response = new ResponseMessage();

            try
            {
                if (pIn.ObjectID < -1)
                {
                    SobeyRecException.ThrowSelfNoParam(pIn.ObjectID.ToString(),  GlobalDictionary.GLOBALDICT_CODE_UNLOCK_OBJECTID_IS_WRONG, Logger, null);
                }
                if (pIn.ObjectTypeID < OTID.OTID_ALL || pIn.ObjectTypeID > OTID.OTID_OTHER)
                {
                    SobeyRecException.ThrowSelfNoParam(pIn.ObjectID.ToString(), GlobalDictionary.GLOBALDICT_CODE_UNLOCK_OBJECT_TYPEID_IS_NOT_EXIST, Logger, null);
                }
                if (pIn.userName == "" || pIn.userName == String.Empty)
                {
                    pIn.userName = "NullUserName";
                }

                bool bRet = await _GlobalManager.SetUnlockObjectAsync(pIn.ObjectID, pIn.ObjectTypeID, pIn.userName);

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
        /// Get api/v2/globalvalue
        /// </remarks>
        /// <param name="strKey">键值</param>
        /// <returns>获取key对应的字段</returns>
        [HttpGet("globalvalue")]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<string>> GetValueString([FromQuery]string strKey)
        {
            var Response = new ResponseMessage<string>();
            if (string.IsNullOrEmpty(strKey))
            {
                Response.Code = ResponseCodeDefines.ArgumentNullError;
                Response.Msg = "请求参数不正确";
                return Response;
            }
            try
            {
                Response.Ext = await _GlobalManager.GetValueStringAsync(strKey);
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
        /// Post api/v2/globalvalue
        /// </remarks>
        /// <param name="strKey">global键</param>
        /// <param name="strValue">global值</param>
        /// <returns></returns>
        [HttpPost("globalvalue")]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage> SetGlobalValue([FromQuery]string strKey, [FromQuery]string strValue)
        {
            ResponseMessage Response = new ResponseMessage();
            try
            {
                if (strValue == null)
                    strValue = "";
                await _GlobalManager.UpdateGlobalValueAsync(strKey, strValue);
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

        /// <summary>
        /// 获取dbpGLOBAL中GLOBAL_KEY对应的value,和上面函数可合并
        /// </summary>
        /// <remarks>
        /// 例子:
        /// Get api/v2/defaultstc/{tcMode}
        /// </remarks>
        /// <param name="tcMode">键值</param>
        /// <returns></returns>
        [HttpGet("defaultstc/{tcMode}")]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<GlobalTcResponse>> GetDefaultSTC(int tcMode)
        {
            ResponseMessage<GlobalTcResponse> Response = new ResponseMessage<GlobalTcResponse>();

            try
            {
                string strKey = string.Empty;
                if ((TC_MODE)tcMode == TC_MODE.emForLine)
                    strKey = "DEFAULT_STC_LINE";
                else
                    strKey = "DEFAULT_STC_OTHER";

                string tcType = await _GlobalManager.GetValueStringAsync(strKey);
                Response.Ext.TcType = (TC_TYPE)Convert.ToInt32(tcType);
                Response.Ext.TC = Convert.ToInt32(await _GlobalManager.GetValueStringAsync("PRESET_STC"));

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
                    Logger.Error("GetDefaultSTC : " + Response.Msg);
                }
            }

            return Response;

        }

        /// <summary>
        /// 获取dbpGLOBAL中GLOBAL_KEY对应的value,和上面函数可合并
        /// </summary>
        /// <remarks>
        /// 例子:
        /// Get api/v2/defaultstcext/{tcMode}
        /// </remarks>
        /// <param name="tcMode">键值</param>
        /// <returns></returns>
        [HttpGet("defaultstcext/{tcMode}")]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<GlobalTcResponse>> GetDefaultSTCExt(int tcMode)
        {
            ResponseMessage<GlobalTcResponse> Response = new ResponseMessage<GlobalTcResponse>();

            try
            {
                Response.Ext = await _GlobalManager.GetDefaultSTCAsync<GlobalTcResponse>((TC_MODE)tcMode);
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
        /// Post api/v2/globalstate
        /// </remarks>
        /// <param name="strLabel">GlobalStateName枚举</param>
        /// <returns></returns>
        [HttpPost("globalstate")]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage> SetGlobalState([FromQuery]string strLabel)
        {
            var Response = new ResponseMessage();
            if (string.IsNullOrEmpty(strLabel))
            {
                Response.Code = ResponseCodeDefines.ArgumentNullError;
                Response.Msg = "请求参数不正确";
                return Response;
            }
            try
            {
                await _GlobalManager.UpdateGlobalStateAsync(strLabel);
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
        /// Get api/v2/globalstate/all
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
        /// 获取globalstate表结果
        /// </summary>
        /// <returns>globalstate表结果</returns>
        /// <remarks>
        /// 例子:
        /// Post api/v2/usersetting
        /// </remarks>
        [HttpPost("usersetting")]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage> Post_SetUserSetting([FromBody]DtoSetUserSetting_IN pIn)
        {
            ResponseMessage Response = new ResponseMessage();
            try
            {
                if (string.IsNullOrWhiteSpace(pIn.UserCode)) 
                {
                    Response.Msg = "The usercode is null.";
                    Response.Code = ResponseCodeDefines.ArgumentNullError;
                    return Response;
                }
                if (string.IsNullOrWhiteSpace(pIn.Settingtype))
                {
                    Response.Msg = "The setting type is null.";
                    Response.Code = ResponseCodeDefines.ArgumentNullError;
                    return Response;
                }

                await _GlobalManager.UpdateUserSettingAsync(pIn.UserCode, pIn.Settingtype, pIn.SettingText);
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
        /// <param name="strUserCode">usercode</param>
        /// <param name="strSettingtype">type</param>
        /// <returns> extention为strSettingText </returns>
        /// <remarks>
        /// 例子:
        /// Get api/v2/usersetting
        /// </remarks>
        [HttpGet("usersetting")]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<string>> GetUserSetting([FromQuery]string strUserCode, [FromQuery]string strSettingtype)
        {
            ResponseMessage<string> Response = new ResponseMessage<string>();
            try
            {
                Response.Ext = await _GlobalManager.GetUserSettingAsync(strUserCode, strSettingtype);
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
        /// <param name="nCaptureParamID">capid</param>
        /// <param name="nFlag">类型</param>
        /// <returns>globalstate表结果</returns>
        /// <remarks>
        /// 例子:
        /// Get api/v2/captureparamtemplate/{nCaptureParamID}
        /// </remarks>
        [HttpGet("captureparamtemplate/{nCaptureParamID}")]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<string>> GetParamTemplateByID(int nCaptureParamID, [FromQuery]int nFlag)
        {
            ResponseMessage<string> Response = new ResponseMessage<string>();
            try
            {
                //读取采集参数模板
                string temp = await _GlobalManager.GetCapParamTemplateByIDAsync(nCaptureParamID);
                temp = _GlobalManager.DealCaptureParam(temp, nFlag);
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

        #endregion

        #region UserTemplate
        /// <summary>
        /// 增加一个新的用户模板
        /// </summary>
        /// <remarks>
        /// 例子:
        /// Post api/v2/usertemplate/add
        /// </remarks>
        /// <param name="userTemplate"></param>
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

        //通过采集参数ID获得采集参数
        /// <summary>
        /// 根据模板ID修改模板内容
        /// </summary>
        /// <param name="nTemplateID"></param>
        /// <param name="UserTemplate"></param>
        /// <returns>标准返回信息</returns>
        /// <remarks>
        /// 例子:
        /// Put api/v2/usertemplate/modify/{nTemplateID}
        /// </remarks>
        [HttpPut("usertemplate/modify/{nTemplateID}")]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage> ModifyUserTempalte([FromQuery] int nTemplateID, [FromBody]  EUserTemplate UserTemplate)
        {
            ResponseMessage Response = new ResponseMessage();

            if (nTemplateID <= 0)
            {
                Response.Msg = "TemplateID is smaller or equal 0.";
                Response.Code = ResponseCodeDefines.ArgumentNullError;
            }
            try
            {
                await _GlobalManager.ModifyUserTemplateAsync(nTemplateID,UserTemplate.TemplateName, UserTemplate.TemplateContent);
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
        

        /// <summary>
        /// 获得用户所有模板
        /// </summary>
        /// <param name="strUserCode"></param>
        /// <returns>extension 为 获取到的模板数组</returns>
        /// <remarks>
        /// 例子:
        /// Get api/v2/usertemplate/all
        /// </remarks>
        [HttpGet("usertemplate/all")]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<List<DtoUserTemplate>>> GetUserAllTemplatesByUserCode([FromQuery] string strUserCode)
        {
            ResponseMessage<List<DtoUserTemplate>> Response = new ResponseMessage<List<DtoUserTemplate>>();

            if (strUserCode == string.Empty)
            {
                Response.Msg = "UserCode is null.";
                Response.Code = ResponseCodeDefines.ArgumentNullError;
            }
            try
            {
                Response.Ext = await _GlobalManager.GetUserAllTemplatesAsync<DtoUserTemplate>(strUserCode);
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
        /// <param name="nTemplateID">模板id</param>
        /// <returns></returns>
        /// <remarks>
        /// 例子:
        /// Delete api/v2/usertemplate/delete/{nTemplateID}
        /// </remarks>
        [HttpDelete("usertemplate/delete/{nTemplateID}")]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage> DeleteUserTemplateByID([FromQuery]int nTemplateID)
        {
            ResponseMessage Response = new ResponseMessage();

            try
            {
                if (nTemplateID <= 0)
                {
                    Response.Msg = "TemplateID is smaller or equal 0.";
                    Response.Code = ResponseCodeDefines.ArgumentNullError;
                }
                await _GlobalManager.DeleteUserTemplateAsync(nTemplateID);
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

        /// <summary>
        /// 获得用户所有模板
        /// </summary>
        /// <param name="UserCode"></param>
        /// <returns></returns>
        /// <remarks>
        /// 例子:
        /// Get api/v2/userparammap/delete
        /// </remarks>
        [HttpDelete("userparammap/delete")]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage> DeleteUserParamTemplateByUserCode([FromQuery]string UserCode)
        {
            ResponseMessage Response = new ResponseMessage();

            try
            {
                if (string.IsNullOrEmpty(UserCode))
                {
                    Response.Msg = "UserCode is Empty.";
                    Response.Code = ResponseCodeDefines.ArgumentNullError;
                    return Response;
                }
                await _GlobalManager.DelUserParamTemplateAsync(UserCode);
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
        /// <param name="strUserCode"></param>
        /// <returns>取到的用户信息</returns>
        /// <remarks>
        /// 例子:
        /// Get api/v2/userinfo
        /// </remarks>
        [HttpGet("userinfo")]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<DtoCMUserInfo>> GetUserInfoByCode([FromQuery]string strUserCode)
        {
            ResponseMessage<DtoCMUserInfo> Response = new ResponseMessage<DtoCMUserInfo>();
            try
            {
                Response = await _GlobalManager.GetUserInfoByUserCodeAsync<DtoCMUserInfo>(strUserCode);
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

        /// <summary>
        /// 通过用户ID得到用户高清或标清采集参数=
        /// </summary>
        /// <param name="szUserToken"></param>
        /// <param name="nFlag"></param>
        /// <returns>取到的用户信息</returns>
        /// <remarks>
        /// 例子:
        /// Get api/v2/captureparamtemplate/highorstandard
        /// </remarks>
        [HttpGet("captureparamtemplate/highorstandard")]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<string>> GetUserHighOrStandardParam([FromQuery]string szUserToken, [FromQuery]int nFlag)//nFlag：0为标清，1为高清
        {
            ResponseMessage<string> Response = new ResponseMessage<string>();
            try
            {
                int nCaptureParamID = -1;

                ResponseMessage<etparam> res = await _GlobalManager.GetHighOrStandardParamAsync<etparam>(szUserToken);
                if (res.Code == ResponseCodeDefines.SuccessCode)
                {
                    nCaptureParamID = Convert.ToInt32(res.Ext.paramvalue);
                    Response = await GetParamTemplateByID(nCaptureParamID, nFlag);
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
