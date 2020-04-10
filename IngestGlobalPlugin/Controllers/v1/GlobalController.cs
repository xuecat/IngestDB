using IngestDBCore;
using IngestGlobalPlugin.Dto;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IngestGlobalPlugin.Controllers
{
    public partial class GlobalController : ControllerBase
    {
        string no_err = "OK";

        [HttpGet("SetGlobalState"), MapToApiVersion("1.0")]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task OldSetGlobalState([FromQuery]int nTaskID)
        {
            


        }

        

        [HttpPost("PostLockObject"), MapToApiVersion("1.0")]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<OldPostParam_Out> OldPostLockObject([FromBody] PostLockObject_param_in pIn)
        {
            OldPostParam_Out pOut = new OldPostParam_Out();
            
            try
            {
                if (pIn.ObjectID < 0)
                {
                    Logger.Error("ObjectID < 0 ,参数传递错误");
                    SobeyRecException.ThrowSelfNoParam(pIn.ObjectID.ToString(), GlobalDictionary.GLOBALDICT_CODE_LOCK_OBJECTID_WRONG, Logger, null);
                }
                if (pIn.ObjectTypeID < OTID.OTID_VTR || pIn.ObjectTypeID > OTID.OTID_OTHER)
                {
                    SobeyRecException.ThrowSelfNoParam(pIn.ObjectTypeID.ToString(), GlobalDictionary.GLOBALDICT_CODE_LOCK_OBJECT_TPYEID_IS_NOT_EXIST, Logger, null);
                }
                if (string.IsNullOrEmpty(pIn.userName))//   == "" || pIn.userName == string.Empty)
                {
                    pIn.userName = "NullUserName";
                }
                if (pIn.TimeOut < 0)
                {
                    SobeyRecException.ThrowSelfNoParam(pIn.TimeOut.ToString(), GlobalDictionary.GLOBALDICT_CODE_LOCK_OBJECT_TIMEOUT_IS_WRONG, Logger, null);
                }

                pOut.bRet = await _GlobalManager.SetLockObject(pIn.ObjectID, pIn.ObjectTypeID, pIn.userName, pIn.TimeOut);
                
            }
            catch (Exception ex)
            {
                pOut.errStr = ex.Message;
                Logger.Error("PostlockObject 异常发生: " + ex.ToString());
                pOut.bRet = false;
            }

            return pOut;
        }

        /// <summary>
        /// 解锁对象
        /// </summary>
        /// <remarks>
        /// 例子:
        /// Post api/v1/unlockobject
        /// </remarks>
        /// <param name="pIn">锁对象参数</param>
        /// <returns>锁对象结果</returns>
        [HttpPost("PostUnlockObject"), MapToApiVersion("1.0")]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<OldPostParam_Out> OldPostUnlockObject([FromBody] PostLockObject_param_in pIn)
        {

            OldPostParam_Out pOut = new OldPostParam_Out();
            pOut.errStr = no_err;
            pOut.bRet = true;

            try
            {
                if (pIn.ObjectID < -1)
                {
                    //SobeyRecException.ThrowSelf(GlobalDictionary.GLOBALDICT_CODE_UNLOCK_OBJECTID_IS_WRONG);
                }
                if (pIn.ObjectTypeID < OTID.OTID_ALL || pIn.ObjectTypeID > OTID.OTID_OTHER)
                {
                    //SobeyRecException.ThrowSelf(GlobalDictionary.GLOBALDICT_CODE_UNLOCK_OBJECT_TYPEID_IS_NOT_EXIST);
                }
                if (pIn.userName == "" || pIn.userName == String.Empty)
                {
                    pIn.userName = "NullUserName";
                    //ApplicationLog.WriteInfo("userName is null!");
                }

                pOut.bRet = await _GlobalManager.SetUnlockObject(pIn.ObjectID, pIn.ObjectTypeID, pIn.userName);
            }
            catch (Exception ex)
            {
                Logger.Error("PostUnlockObject 异常发生: " + ex.ToString());
                pOut.errStr = ex.Message;
                Logger.Error(ex.ToString());
                pOut.bRet = false;
            }

            return pOut;
        }

        #region GlobalState Controller
        /// <summary>
        /// 获取globalstate表结果
        /// </summary>
        /// <returns>globalstate表结果</returns>
        [HttpGet("GetGlobalState"), MapToApiVersion("1.0")]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<GetGlobalState_OUT> OldGetGlobalState()
        {
            GetGlobalState_OUT p = new GetGlobalState_OUT();
            p.strErr = no_err;
            p.arrGlobalState = null;
            try
            {
                p.arrGlobalState = await _GlobalManager.GetAllGlobalStateAsync<GlobalState[]>();
                if (p.arrGlobalState.Length < 1)
                {
                    p.strErr = "No record in the table";
                    p.bRet = false;
                }
                p.bRet = true;
            }
            catch (System.Exception ex)
            {
                Logger.Error("OldGetGlobalState is error:"+ ex.ToString());
                p.strErr = ex.Message;
            }
            return p;
        }
        #endregion

        #region Global controller
        /// <summary>
        /// 获取global value
        /// </summary>
        /// <param name="strKey">global strKey值</param>
        /// <returns>获取global value</returns>
        [HttpGet("GetValueString"), MapToApiVersion("1.0")]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<string> OldGetValueString(string strKey)
        {
            //LoggerService.WriteCallLog("GetValueString", true);
            try
            {
                string str = await _GlobalManager.GetValueStringAsync(strKey);
                //LoggerService.WriteCallLog("GetValueString", false);
                return str;
            }
            catch (System.Exception ex)
            {
                Logger.Error("OldGetValueString : " + ex.ToString());
                return "";
            }
        }

        /// <summary>
        /// 设置Global value
        /// </summary>
        /// <returns>获取状态结果</returns>
        [HttpGet("SetValue"), MapToApiVersion("1.0")]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<OldResponseMessage> OldSetValue([FromQuery]string strKey, [FromQuery]string strValue)
        {
            OldResponseMessage res = new OldResponseMessage();
            try
            {
                if (strValue == null)
                    strValue = "";
                await _GlobalManager.UpdateGlobalValueAsync(strKey, strValue);
                res.nCode = 1;
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
                res.nCode = 0;
                res.message = ex.Message;
            }
            return res;

        }

        [HttpGet("GetDefaultSTC"), MapToApiVersion("1.0")]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<GetDefaultSTC_param> OldGetDefaultSTC(int tcMode)
        {

            GetDefaultSTC_param p = new GetDefaultSTC_param();
            p.errStr = no_err;
            p.tcType = (int)0;
            p.nTC = 0;
            try
            {
                string strKey = string.Empty;
                if ((TC_MODE)tcMode == TC_MODE.emForLine)
                    strKey = "DEFAULT_STC_LINE";
                else
                    strKey = "DEFAULT_STC_OTHER";

                string tcType = await _GlobalManager.GetValueStringAsync(strKey);
                p.tcType = (TC_TYPE)Convert.ToInt32(tcType);
                p.nTC = Convert.ToInt32(await _GlobalManager.GetValueStringAsync("PRESET_STC"));
                p.bRet = true;
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
                p.errStr = ex.Message;
                p.bRet = false;
            }
            return p;
        }
        #endregion

        #region User
        [HttpPost("Post_SetUserSetting"), MapToApiVersion("1.0")]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<OldPostParam_Out> OldPost_SetUserSetting(SetUserSetting_IN pIn)
        {
            OldPostParam_Out pOut = new OldPostParam_Out();
            pOut.errStr = no_err;
            try
            {
                if (string.IsNullOrWhiteSpace(pIn.strUserCode)) // pIn.strUserCode == null || pIn.strUserCode == "" || pIn.strUserCode == string.Empty)
                {
                    pOut.errStr = "The usercode is null.";
                    pOut.bRet = false;
                    return pOut;
                }

                if (string.IsNullOrWhiteSpace(pIn.strSettingtype))// == null || pIn.strSettingtype == "" || pIn.strSettingtype == string.Empty)
                {
                    pOut.errStr = "The setting type is null.";
                    pOut.bRet = false;
                    return pOut;
                }
                
                await _GlobalManager.UpdateUserSettingAsync(pIn.strUserCode, pIn.strSettingtype, pIn.strSettingText);
                pOut.bRet = true;
            }
            catch (Exception ex)//其他未知的异常，写异常日志
            {
                pOut.errStr = ex.Message;
                Logger.Error("Post_SetUserSetting : " + ex.ToString());
                pOut.bRet = false;
            }
            return pOut;
        }
        #endregion

        #region ParamTemplate
        
        [HttpGet("GetParamTemplateByID"), MapToApiVersion("1.0")]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<GetParamTemplateByID_out> OldGetParamTemplateByID(int nCaptureParamID, int nFlag)
        {
            GetParamTemplateByID_out p = new GetParamTemplateByID_out();
            p.errStr = no_err;
            p.strCaptureParam = string.Empty;
            try
            {
                //读取采集参数模板
                string temp = await _GlobalManager.GetParamTemplateByIDAsync(nCaptureParamID, nFlag);
                if (string.IsNullOrEmpty(temp))
                {
                    p.errStr = "There's no CaptureParam!";
                    p.bRet = false;
                    return p;
                }
                p.strCaptureParam = temp;
                p.bRet = true;
            }
            catch (Exception ex)//其他未知的异常，写异常日志
            {
                Logger.Error("OldGetParamTemplateByID : " + ex.ToString());
                p.errStr = ex.Message;
                p.bRet = false;
            }
            return p;
        }
        #endregion

        #region UserTemplate
        /// <summary>
        /// 增加一个新的用户模板
        /// </summary>
        /// <param name="userTemplate"></param>
        /// <returns>extention 为用户模版ID</returns>
        [HttpPost("AddUserTemplate"), MapToApiVersion("1.0")]
        [ApiExplorerSettings(GroupName = "v1")]
        public OldResponseMessage<int> OldAddUserTemplate([FromBody] UserTemplate userTemplate)
        {
            OldResponseMessage<int> res = new OldResponseMessage<int>();
            res.message = no_err;
            res.extention = -1;

            if (userTemplate == null)
            {
                res.message = "userTemplate is null.";
                res.nCode = 0;
                return res;
            }
            if (userTemplate.nTemplateID > 0)
            {
                res.message = "Template ID is larger than 0";
                res.nCode = 0;
                return res;
            }
            if (userTemplate.strUserCode == string.Empty ||
                 userTemplate.strTemplateName == string.Empty)
            {
                res.message = "UserCode or TemplateName is null.";
                res.nCode = 0;
                return res;
            }
            try
            {
                //res.extention = _GlobalManager.AddUserTemplateAsync(userTemplate);
                res.nCode = 1;
            }
            catch (System.Exception ex)
            {
                Logger.Error("OldAddUserTemplate : " + ex.ToString());
                res.message = ex.Message;
                res.nCode = 0;
            }
            return res;
        }

        #endregion

    }
}
