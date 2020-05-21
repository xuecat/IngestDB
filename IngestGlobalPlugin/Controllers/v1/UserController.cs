﻿using IngestDBCore;
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
        string no_err = "OK";

        #region UserTemplate
        /// <summary>
        /// 增加一个新的用户模板
        /// </summary>
        /// <param name="userTemplate">模板信息</param>
        /// <returns>extention 为用户模版ID</returns>
        [HttpPost("AddUserTemplate"), MapToApiVersion("1.0")]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<OldResponseMessage<int>> OldAddUserTemplate([FromBody] UserTemplate userTemplate)
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
                res.extention = await _GlobalManager.UserTemplateInsertAsync(userTemplate.nTemplateID, userTemplate.strUserCode, userTemplate.strTemplateName, userTemplate.strTemplateContent);
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

        /// <summary>
        /// 根据模板ID修改模板内容
        /// </summary>
        /// <param name="nTemplateID">模板id</param>
        /// <param name="strTemplateContent">模板内容</param>
        /// <returns>标准返回信息</returns>
        [HttpPost("ModifyUserTempalteContent"), MapToApiVersion("1.0")]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<OldResponseMessage> OldModifyUserTempalteContent([FromQuery, DefaultValue(2)] int nTemplateID, [FromBody, DefaultValue("<window_positions>...</window_positions>")] string strTemplateContent)
        {
            OldResponseMessage res = new OldResponseMessage();
            res.message = no_err;
            if (nTemplateID <= 0)
            {
                res.message = "TemplateID is smaller or equal 0.";
                res.nCode = 0;
            }
            try
            {
                await _GlobalManager.UpdateUserTempalteContent(nTemplateID, strTemplateContent);
                res.nCode = 1;
            }
            catch (System.Exception ex)
            {
                Logger.Error("OldModifyUserTempalteContent : " + ex.ToString());
                res.message = ex.Message;
                res.nCode = 0;
            }
            return res;
        }

        /// <summary>
        /// 修改采集模板名
        /// </summary>
        /// <param name="nTemplateID">模板Id</param>
        /// <param name="strNewTemplateName">新模板名</param>
        /// <returns></returns>
        [HttpPost("ModifyUserTemplateName"), MapToApiVersion("1.0")]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<OldResponseMessage> OldModifyUserTemplateName([FromQuery, DefaultValue(2)] int nTemplateID, [FromBody, DefaultValue("newName")] string strNewTemplateName)
        {
            OldResponseMessage res = new OldResponseMessage();
            res.message = no_err;
            if (nTemplateID <= 0)
            {
                res.message = "TemplateID is smaller or equal 0.";
                res.nCode = 0;
            }
            try
            {
                await _GlobalManager.ModifyUserTemplateNameAsync(nTemplateID, strNewTemplateName);
                res.nCode = 1;
            }
            catch (System.Exception ex)
            {
                Logger.Error("OldModifyUserTemplateName : " + ex.ToString());
                res.message = ex.Message;
                res.nCode = 0;
            }
            return res;

        }

        /// <summary>
        /// 通过模板id删除UserTemplate
        /// </summary>
        /// <param name="nTemplateID">模板id</param>
        /// <returns>删除结果</returns>
        [HttpDelete("DeleteUserTemplateByID"), MapToApiVersion("1.0")]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<OldResponseMessage> OldDeleteUserTemplateByID([FromQuery, DefaultValue(2)]int nTemplateID)
        {
            OldResponseMessage res = new OldResponseMessage();
            res.message = no_err;
            try
            {
                if (nTemplateID <= 0)
                {
                    res.message = "TemplateID is smaller or equal 0.";
                    res.nCode = 0;
                }
                await _GlobalManager.DeleteUserTemplateAsync(nTemplateID);
            }
            catch (SobeyRecException SBex) //自定义的异常，已经写了日志
            {
                Logger.Error("DeleteUserTemplateByID : " + SBex.ToString());
                res.message = SBex.Message;
                res.nCode = 0;
            }
            catch (Exception ex)//其他未知的异常，写异常日志
            {
                Logger.Error("DeleteUserTemplateByID : " + ex.ToString());
                res.message = ex.Message;
                res.nCode = 0;
            }
            return res;
        }

        /// <summary>
        /// 获得用户所有模板
        /// </summary>
        /// <param name="strUserCode">用户Code</param>
        /// <returns>extension 为 获取到的模板数组</returns>
        [HttpGet("GetUserAllTemplates"), MapToApiVersion("1.0")]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<OldResponseMessage<List<UserTemplate>>> OldGetUserAllTemplates([FromQuery, DefaultValue("8de083d45c614628b99516740d628e91")] string strUserCode)
        {
            OldResponseMessage<List<UserTemplate>> res = new OldResponseMessage<List<UserTemplate>>();
            res.message = no_err;
            res.extention = new List<UserTemplate>();

            if (strUserCode == string.Empty)
            {
                res.message = "UserCode is null.";
                res.nCode = 0;
            }
            try
            {
                res.extention = await _GlobalManager.GetUserAllTemplatesAsync<UserTemplate>(strUserCode);
                res.nCode = 1;
            }
            catch (System.Exception ex)
            {
                Logger.Error("OldGetUserAllTemplates : " + ex.ToString());
                res.message = ex.Message;
                res.nCode = 0;
            }
            return res;
        }

        /// <summary>
        /// 删除用户Param映射关系UserCode-CaptureParamId
        /// </summary>
        /// <param name="szUserCode">UserParamMap的用户Code</param>
        /// <returns>删除结果</returns>
        [HttpGet("DelUserParamTemplate"), MapToApiVersion("1.0")]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<OldResponseMessage> OldDelUserParamTemplate([FromQuery, DefaultValue("ingest01")]string szUserCode)
        {
            OldResponseMessage res = new OldResponseMessage();
            res.message = no_err;
            try
            {
                if (szUserCode == "" || szUserCode == null)
                {
                    res.message = "UserCode is Empty.";
                    res.nCode = 0;
                }
                await _GlobalManager.DelUserParamTemplateAsync(szUserCode);
            }
            catch (SobeyRecException SBex) //自定义的异常，已经写了日志
            {
                Logger.Error("OldDelUserParamTemplate : " + SBex.ToString());
                res.message = SBex.Message;
                res.nCode = 0;
            }
            catch (Exception ex)//其他未知的异常，写异常日志
            {
                Logger.Error("OldDelUserParamTemplate : " + ex.ToString());
                res.message = ex.Message;
                res.nCode = 0;
            }
            return res;
        }

        #endregion

        #region ParamTemplate

        /// <summary>
        /// 得到模板信息通过Id
        /// </summary>
        /// <param name="nCaptureParamID">模板id</param>
        /// <param name="nFlag">hd=0，sd=1,uhd=2标识</param>
        /// <returns></returns>
        [HttpGet("GetParamTemplateByID"), MapToApiVersion("1.0")]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<GetParamTemplateByID_out> OldGetParamTemplateByID([FromQuery, DefaultValue(2)]int nCaptureParamID, [FromQuery, DefaultValue(0)]int nFlag)
        {
            GetParamTemplateByID_out p = new GetParamTemplateByID_out();
            p.errStr = no_err;
            p.strCaptureParam = string.Empty;
            try
            {
                //读取采集参数模板
                string temp = await _GlobalManager.GetCapParamTemplateByIDAsync(nCaptureParamID);
                temp = _GlobalManager.DealCaptureParam(temp, nFlag);
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

        #region User
        /// <summary>
        /// 修改usersetting
        /// </summary>
        /// <param name="pIn">usersetting参数</param>
        /// <returns>修改结果</returns>
        [HttpPost("Post_SetUserSetting"), MapToApiVersion("1.0")]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<PostParam_Out> OldPost_SetUserSetting([FromBody]SetUserSetting_IN pIn)
        {
            PostParam_Out pOut = new PostParam_Out();
            pOut.errStr = no_err;
            try
            {
                if (string.IsNullOrWhiteSpace(pIn.strUserCode))
                {
                    pOut.errStr = "The usercode is null.";
                    pOut.bRet = false;
                    return pOut;
                }

                if (string.IsNullOrWhiteSpace(pIn.strSettingtype))
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
                Logger.Error("OldPost_SetUserSetting : " + ex.ToString());
                pOut.bRet = false;
            }
            return pOut;
        }

        /// <summary>
        /// 通过用户编码获取配置信息
        /// </summary>
        /// <param name="strUserCode">usercode</param>
        /// <param name="strSettingtype">type</param>
        /// <returns> extention为strSettingText </returns>
        [HttpGet("GetUserSetting"), MapToApiVersion("1.0")]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<OldResponseMessage<string>> OldGetUserSetting([FromQuery, DefaultValue("06c70a52172d4393beb1bb6743ca6944")]string strUserCode, [FromQuery, DefaultValue("UserMoudleData")]string strSettingtype)
        {
            OldResponseMessage<string> res = new OldResponseMessage<string>();
            res.message = no_err;
            res.extention = string.Empty;
            try
            {
                res.extention = await _GlobalManager.GetUserSettingAsync(strUserCode, strSettingtype);
                res.nCode = 1;
            }
            catch (Exception ex)//其他未知的异常，写异常日志
            {
                Logger.Error(ex.ToString());
                res.message = ex.Message;
                res.nCode = 0;
            }
            return res;
        }

        #endregion

        #region CMApi
        /// <summary>
        /// 获取UserInfo
        /// </summary>
        /// <param name="strUserCode">用户编码</param>
        /// <returns>用户信息</returns>
        [HttpGet("GetUserInfoByCode"), MapToApiVersion("1.0")]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<OldResponseMessage<CMUserInfo>> OldGetUserInfoByCode([FromQuery, DefaultValue("897cd4f79531e3c04c2c9a371e4db4ea")]string strUserCode)
        {
            OldResponseMessage<CMUserInfo> res = new OldResponseMessage<CMUserInfo>();
            res.message = no_err;
            res.extention = null;
            try
            {
                ResponseMessage<CMUserInfo> reres = await _GlobalManager.GetUserInfoByUserCodeAsync<CMUserInfo>(strUserCode);
                if (reres.Code == ResponseCodeDefines.SuccessCode)
                {
                    res.extention = reres.Ext;
                    res.message = "OK";
                    res.nCode = 1;
                }
                else
                {
                    res.extention = null;
                    res.message = "error";
                }
            }
            catch (Exception ex)//其他未知的异常，写异常日志
            {
                Logger.Error("OldGetUserInfoByCode:" + ex.ToString());
                res.message = ex.Message;
                res.nCode = 0;
            }
            return res;
        }


        /// <summary>
        /// 通过用户ID得到用户高清或标清采集参数=
        /// </summary>
        /// <param name="szUserToken">用户usertoken</param>
        /// <param name="nFlag">nFlag：0为标清，1为高清</param>
        /// <returns>采集参数</returns>
        [HttpGet("GetUserHighOrStandardParam"), MapToApiVersion("1.0")]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<OldResponseMessage<string>> OldGetUserHighOrStandardParam([FromQuery, DefaultValue("897cd4f79531e3c04c2c9a371e4db4ea")]string szUserToken, [FromQuery, DefaultValue(0)]int nFlag)//nFlag：0为标清，1为高清
        {
            OldResponseMessage<string> Res = new OldResponseMessage<string>();
            try
            {
                int nCaptureParamID = -1;

                nCaptureParamID = await _GlobalManager.GetUserParamTemplateIDAsync(true, szUserToken);
                if (nCaptureParamID != -1)
                {
                    GetParamTemplateByID_out ret = await OldGetParamTemplateByID(nCaptureParamID, nFlag);

                    Res.extention = ret.strCaptureParam;
                    Res.message = "OK";
                    Res.nCode = 1;
                }
                else
                {
                    Res.extention = null;
                    Res.nCode = 0;
                }
            }
            catch (System.Exception ex)
            {
                Res.nCode = 0;
                Res.message = ex.Message;
                Logger.Error("OldGetUserHighOrStandardParam : " + ex.ToString());
            }
            return Res;
        }

        #endregion
    }
}