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

        [HttpGet("GetQueryTaskMetaData"), MapToApiVersion("1.0")]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task SetGlobalState1([FromQuery]int nTaskID)
        {
            


        }
        

        /// <summary>
        /// 获取dbpGLOBAL中GLOBAL_KEY对应的value,和上面函数可合并
        /// </summary>
        /// <remarks>
        /// 例子:
        /// Get api/v1/defaultstc?tcMode=
        /// </remarks>
        /// <param name="tcMode">键值</param>
        /// <returns></returns>
        [HttpGet("defaultstc"), MapToApiVersion("1.0")]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<ResponseMessage<GlobalTcResponse>> GetDefaultSTC(int tcMode)
        {
            ResponseMessage<GlobalTcResponse> Response = new ResponseMessage<GlobalTcResponse>();

            try
            {
                Response.Ext = await _GlobalManager.GetDefaultSTC((TC_MODE)tcMode);
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

        
        [HttpPost("PostLockObject"), MapToApiVersion("1.0")]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<PostLockObject_param_out> OldPostLockObject([FromBody] PostLockObject_param_in pIn)
        {
            PostLockObject_param_out pOut = new PostLockObject_param_out();
            
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
        public async Task<PostLockObject_param_out> OldPostUnlockObject([FromBody] PostLockObject_param_in pIn)
        {

            PostLockObject_param_out pOut = new PostLockObject_param_out();
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

        /// <summary>
        /// 获取Global状态
        /// </summary>
        /// <remarks>
        /// 例子:
        /// Get api/v1/allglobalstate
        /// </remarks>
        /// <returns>获取状态结果</returns>
        [HttpGet("allglobalstate"), MapToApiVersion("1.0")]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<GetGlobalState_OUT> GetGlobalState()
        {
            GetGlobalState_OUT p = new GetGlobalState_OUT();
            p.strErr = no_err;
            p.arrGlobalState = null;
            try
            {
                p = await _GlobalManager.GetAllGlobalState();
            }
            catch (System.Exception ex)
            {
                Logger.Error(ex.ToString());
                p.strErr = ex.Message;
            }
            return p;
        }

        /// <summary>
        /// 获取global value
        /// </summary>
        /// <remarks>
        /// 例子:
        /// Get api/v1/GetValueString
        /// </remarks>
        /// <param name="strKey"></param>
        /// <returns></returns>
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
                Logger.Error(ex.ToString());
                return "";
            }
        }

        /// <summary>
        /// 设置Global value
        /// </summary>
        /// <remarks>
        /// 例子:
        /// Get api/v1/SetValue
        /// </remarks>
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

    }
}
