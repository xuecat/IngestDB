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
        private readonly RestClient _restClient;

        public GlobalController(RestClient rsc, GlobalManager global)
        {
            _GlobalManager = global;
            _restClient = rsc;
        }

        #region lock or unlockobj
        /// <summary>
        /// 锁对象
        /// </summary>
        /// <remarks>
        /// 例子:
        /// Post api/v2/lockobject
        /// </remarks>
        /// <param name="pIn">锁对象参数</param>
        /// <returns>锁对象结果</returns>
        [HttpPost("lockobject")]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage> PostLockObject([FromBody] PostLockObject_param_in pIn)
        {
            ResponseMessage Response = new ResponseMessage();

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
                    Logger.Error(Response.Msg);
                }
            }

            return Response;
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
        [HttpPost("unlockobject")]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage> PostUnlockObject([FromBody] PostLockObject_param_in pIn)
        {

            ResponseMessage Response = new ResponseMessage();
            //pOut.errStr = no_err;
            //pOut.bRet = true;

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
                    //ApplicationLog.WriteInfo("userName is null!");
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
                    Logger.Error(Response.Msg);
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
        public async Task<ResponseMessage> SetValue([FromQuery]string strKey, [FromQuery]string strValue)
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
                    Logger.Error(Response.Msg);
                }
            }
            return Response;
        }

        /// <summary>
        /// 获取dbpGLOBAL中GLOBAL_KEY对应的value,和上面函数可合并
        /// </summary>
        /// <remarks>
        /// 例子:
        /// Get api/v2/defaultstc
        /// </remarks>
        /// <param name="tcMode">键值</param>
        /// <returns></returns>
        [HttpGet("defaultstc")]
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
                Response.Ext.tcType = (TC_TYPE)Convert.ToInt32(tcType);
                Response.Ext.nTC = Convert.ToInt32(await _GlobalManager.GetValueStringAsync("PRESET_STC"));

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
                    Logger.Error(Response.Msg);
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
        ///     Get api/v2/globalstate
        /// </remarks>
        /// <param name="strLabel">GlobalStateName枚举</param>
        /// <returns></returns>
        [HttpPost("globalstate")]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage> SetGlobalState([FromQuery]string strLabel)
        {
            var Response = new ResponseMessage();
            if (strLabel == null || strLabel == string.Empty)
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
                    Logger.Error(Response.Msg);
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
        /// Get api/v2/allglobalstate
        /// </remarks>
        [HttpGet("allglobalstate")]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<List<DbpGlobalState>>> GetGlobalState()
        {
            ResponseMessage<List<DbpGlobalState>> Response = new ResponseMessage<List<DbpGlobalState>>();

            try
            {
                Response.Ext = await _GlobalManager.GetAllGlobalStateAsync<List<DbpGlobalState>>();
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
                    Logger.Error(Response.Msg);
                }
            }

            return Response;

        }

        #endregion
    }
}
