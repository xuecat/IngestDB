using IngestDBCore;
using IngestDBCore.Basic;
using IngestDBCore.Dto;
using IngestGlobalPlugin.Dto;
using IngestGlobalPlugin.Dto.OldResponse;
using IngestGlobalPlugin.Managers;
using Microsoft.AspNetCore.Mvc;
using Sobey.Core.Log;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IngestGlobalPlugin.Controllers.v1
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    public class GlobalController : ControllerBase
    {
        private readonly ILogger Logger = LoggerManager.GetLogger("GlobalInfo");
        private readonly GlobalManager _GlobalManager;
        //private readonly RestClient _restClient;

        public GlobalController(GlobalManager global)
        {
            _GlobalManager = global;
            //_restClient = rsc;
        }
        string no_err = "OK";

        //[HttpGet("SetGlobalState"), MapToApiVersion("1.0")]
        //[ApiExplorerSettings(GroupName = "v1")]
        //public async Task OldSetGlobalState([FromQuery]string strLabel)
        //{
        //    if (string.IsNullOrEmpty(strLabel))
        //    {
        //        return;
        //    }

        //    try
        //    {
        //        await _GlobalManager.UpdateGlobalStateAsync(strLabel);
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.Error("OldSetGlobalState : " + ex.ToString());
        //    }
        //}

        #region lock obj
        [HttpPost("PostLockObject"), MapToApiVersion("1.0")]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<PostParam_Out> OldPostLockObject([FromBody] PostLockObject_param_in pIn)
        {
            PostParam_Out pOut = new PostParam_Out();

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

                pOut.bRet = await _GlobalManager.SetLockObjectAsync(pIn.ObjectID, pIn.ObjectTypeID, pIn.userName, pIn.TimeOut);

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
        public async Task<PostParam_Out> OldPostUnlockObject([FromBody] PostLockObject_param_in pIn)
        {

            PostParam_Out pOut = new PostParam_Out();
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

                pOut.bRet = await _GlobalManager.SetUnlockObjectAsync(pIn.ObjectID, pIn.ObjectTypeID, pIn.userName);
            }
            catch (Exception ex)
            {
                Logger.Error("PostUnlockObject 异常发生: " + ex.ToString());
                pOut.errStr = ex.Message;
                pOut.bRet = false;
            }

            return pOut;
        }

        #endregion

        #region GlobalState Controller
        /// <summary>
        /// 获取所有globalstate表结果
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
                p.arrGlobalState = await _GlobalManager.GetAllGlobalStateAsync<GlobalState>();
                if (p.arrGlobalState.Count < 1)
                {
                    p.strErr = "No record in the table";
                    p.bRet = false;
                }
                p.bRet = true;
            }
            catch (System.Exception ex)
            {
                Logger.Error("OldGetGlobalState is error:" + ex.ToString());
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
        public async Task<string> OldGetValueString([FromQuery, DefaultValue("DEFAULT_CATALOG")]string strKey)
        {
            try
            {
                string str = await _GlobalManager.GetValueStringAsync(strKey);
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
        public async Task<OldResponseMessage> OldSetValue([FromQuery, DefaultValue("DEFAULT_CATALOG")]string strKey, [FromQuery, DefaultValue("\\Public Material")]string strValue)
        {
            OldResponseMessage res = new OldResponseMessage();
            try
            {
                if (strValue == null)
                    strValue = "";
                bool ret = await _GlobalManager.UpdateGlobalValueAsync(strKey, strValue);
                res.nCode = ret? 1:0;
            }
            catch (Exception ex)
            {
                Logger.Error("OldSetValue:" + ex.ToString());
                res.nCode = 0;
                res.message = ex.Message;
            }
            return res;

        }


        [HttpGet("GetDefaultSTC"), MapToApiVersion("1.0")]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<GetDefaultSTC_param> OldGetDefaultSTC([FromQuery, DefaultValue(0)]int tcMode)
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
                Logger.Error("OldGetDefaultSTC : " + ex.ToString());
                p.errStr = ex.Message;
                p.bRet = false;
            }
            return p;
        }

        /// <summary>
        /// 获取默认Global STC
        /// </summary>
        /// <param name="tcMode">0=emForLine,1=emForOther</param>
        /// <returns></returns>
        [HttpGet("GetDefaultSTCExt"), MapToApiVersion("1.0")]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<GetDefaultSTC_param> OldGetDefaultSTCExt([FromQuery, DefaultValue(0)]int tcMode)
        {

            GetDefaultSTC_param p = new GetDefaultSTC_param();
            p.errStr = no_err;
            p.tcType = (int)0;
            p.nTC = 0;
            try
            {
                p = await _GlobalManager.GetDefaultSTCAsync<GetDefaultSTC_param>((TC_MODE)tcMode);
                p.bRet = true;
            }
            catch (Exception ex)
            {
                Logger.Error("OldGetDefaultSTCExt : " + ex.ToString());
                p.errStr = ex.Message;
                p.bRet = false;
            }
            return p;
        }

        #endregion

        
        

        

    }
}
