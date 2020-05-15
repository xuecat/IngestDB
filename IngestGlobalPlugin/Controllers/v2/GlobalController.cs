using IngestDBCore;
using IngestDBCore.Basic;
using IngestDBCore.Tool;
using IngestGlobalPlugin.Dto;
using IngestGlobalPlugin.Dto.OldResponse;
using IngestGlobalPlugin.Dto.Response;
using IngestGlobalPlugin.Managers;
using IngestGlobalPlugin.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Sobey.Core.Log;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

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
        /// <param name="objectparamin">锁对象参数</param>
        /// <returns>锁对象结果</returns>
        [HttpPost("objectinfo/lock")]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage> PostLockObject([FromBody] DtoLockObjectParamIn objectparamin)
        {
            ResponseMessage Response = new ResponseMessage();

            try
            {
                if (objectparamin.ObjectID < 0)
                {
                    Logger.Error("PostLockObject : ObjectID < 0 ,参数传递错误");
                    SobeyRecException.ThrowSelfNoParam(objectparamin.ObjectID.ToString(), GlobalDictionary.GLOBALDICT_CODE_LOCK_OBJECTID_WRONG, Logger, null);
                }
                if (objectparamin.ObjectTypeID < OTID.OTID_VTR || objectparamin.ObjectTypeID > OTID.OTID_OTHER)
                {
                    SobeyRecException.ThrowSelfNoParam(objectparamin.ObjectTypeID.ToString(), GlobalDictionary.GLOBALDICT_CODE_LOCK_OBJECT_TPYEID_IS_NOT_EXIST, Logger, null);
                }
                if (string.IsNullOrEmpty(objectparamin.UserName))
                {
                    objectparamin.UserName = "NullUserName";
                }
                if (objectparamin.TimeOut < 0)
                {
                    SobeyRecException.ThrowSelfNoParam(objectparamin.TimeOut.ToString(), GlobalDictionary.GLOBALDICT_CODE_LOCK_OBJECT_TIMEOUT_IS_WRONG, Logger, null);
                }

                bool ret = await _GlobalManager.SetLockObjectAsync(objectparamin.ObjectID, objectparamin.ObjectTypeID, objectparamin.UserName, objectparamin.TimeOut);

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
                    Response.Msg = "error info：" + e.Message;
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
        /// <param name="objectparamin">锁对象参数</param>
        /// <returns>锁对象结果</returns>
        [HttpPost("objectinfo/unlock")]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage> PostUnlockObject([FromBody] DtoLockObjectParamIn objectparamin)
        {

            ResponseMessage Response = new ResponseMessage();

            try
            {
                if (objectparamin.ObjectID < -1)
                {
                    SobeyRecException.ThrowSelfNoParam(objectparamin.ObjectID.ToString(),  GlobalDictionary.GLOBALDICT_CODE_UNLOCK_OBJECTID_IS_WRONG, Logger, null);
                }
                if (objectparamin.ObjectTypeID < OTID.OTID_ALL || objectparamin.ObjectTypeID > OTID.OTID_OTHER)
                {
                    SobeyRecException.ThrowSelfNoParam(objectparamin.ObjectID.ToString(), GlobalDictionary.GLOBALDICT_CODE_UNLOCK_OBJECT_TYPEID_IS_NOT_EXIST, Logger, null);
                }
                if (objectparamin.UserName == "" || objectparamin.UserName == String.Empty)
                {
                    objectparamin.UserName = "NullUserName";
                }

                bool bRet = await _GlobalManager.SetUnlockObjectAsync(objectparamin.ObjectID, objectparamin.ObjectTypeID, objectparamin.UserName);

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
                    Response.Msg = "error info：" + e.Message;
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
                    Response.Msg = "error info：" + e.Message;
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
                    Response.Msg = "error info：" + e.Message;
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
                    Response.Msg = "error info：" + e.Message;
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
                    Response.Msg = "error info：" + e.Message;
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
        public async Task<ResponseMessage<List<DtoGlobalStateResponse>>> GetAllGlobalState()
        {
            ResponseMessage<List<DtoGlobalStateResponse>> Response = new ResponseMessage<List<DtoGlobalStateResponse>>();

            try
            {
                Response.Ext = await _GlobalManager.GetAllGlobalStateAsync<DtoGlobalStateResponse>();
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
                    Response.Msg = "error info：" + e.Message;
                    Logger.Error("GetAllGlobalState : " + Response.Msg);
                }
            }

            return Response;

        }

        #endregion
        
        
        

    }
}
