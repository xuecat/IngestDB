using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using IngestDBCore;
using IngestGlobalPlugin.Stores;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using IngestGlobalPlugin.Dto;
using AutoMapper;
using IngestGlobalPlugin.Models;
using System.Xml;
using IngestDBCore.Tool;
using System.Collections.Specialized;
using Sobey.Core.Log;
using IngestGlobalPlugin.Dto.OldResponse;
using IngestGlobalPlugin.Dto.Response;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;

using UserLoginInfoRequest = IngestGlobalPlugin.Dto.Response.UserLoginInfoResponse;
namespace IngestGlobalPlugin.Managers
{
    public class GlobalManager
    {
        private readonly ILogger Logger = LoggerManager.GetLogger("GlobalInfo");

        protected IGlobalStore Store { get; }
        protected IMapper _mapper { get; }
        protected RestClient _restClient;

        public GlobalManager(IHttpClientFactory httpClientFactory, IGlobalStore store, IMapper mapper, RestClient rsc)
        {
            Store = store;
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _restClient = rsc;// new RestClient(httpClientFactory, "ApiOutClient");
        }


        #region global Manager
        //get global value by key
        public async Task<string> GetValueStringAsync(string strKey)
        {
            var global = await Store.GetGlobalAsync(a => a.Where(x => x.GlobalKey == strKey), true);

            return global != null ? global.GlobalValue : string.Empty;

        }

        //add or update global value
        public Task<bool> UpdateGlobalValueAsync(string strKey, string strValue)
        {
            return Store.UpdateGlobalValueAsync(strKey, strValue);
        }

        public async Task<TResult> GetDefaultSTCAsync<TResult>(TC_MODE tcMode)
        {
            GlobalTcResponse global = new GlobalTcResponse();
            String strKey = String.Empty;
            if (tcMode == TC_MODE.emForLine)
                strKey = "DEFAULT_STC_LINE";
            else
                strKey = "DEFAULT_STC_OTHER";

            try
            {
                string tcType = await GetValueStringAsync(strKey);
                global.TcType = (TC_TYPE)Convert.ToInt32(tcType);
                global.Tc = Convert.ToInt32(await GetValueStringAsync("PRESET_STC"));
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
                SobeyRecException.ThrowSelfNoParam(tcMode.ToString(), GlobalDictionary.GLOBALDICT_CODE_IN_GETDEFAULTSTC_READ_DATA_FAILED, Logger, ex);
                return default(TResult);
            }
            return _mapper.Map<TResult>(global);
        }

        #endregion



        //用于老接口
        public async Task<bool> SetLockObject(int objectID, OTID objectTypeID, string userName, int TimeOut)
        {
            return await Store.SetLockObject(objectID, objectTypeID, userName, TimeOut);
        }

        //用于老街口
        public async Task<bool> SetUnlockObject(int objectID, OTID objectTypeID, string userName)
        {
            return await Store.SetUnLockObject(objectID, objectTypeID, userName);
        }


        #region globalstate manager
        //get all global state
        public async Task<List<TResult>> GetAllGlobalStateAsync<TResult>()
        {
            var globalstates = await Store.GetGlobalStateListAsync(a => a, true);
            return _mapper.Map<List<TResult>>(globalstates);
        }

        //add or update
        public async Task UpdateGlobalStateAsync(string strLabel)
        {
            await Store.UpdateGlobalStateAsync(strLabel);
        }
        #endregion

        #region Lock Objects Manager
        //lock obj
        public async Task<bool> SetLockObjectAsync(int objectID, OTID objectTypeID, string userName, int TimeOut)
        {
            if (TimeOut < 0)
                TimeOut *= -1;

            var dbpObjState = await Store.GetObjectstateinfoAsync(a => a.Where(x => x.Objectid == objectID && x.Objecttypeid == (int)objectTypeID), true);
            if (dbpObjState == null)
            {
                return await Store.AddDbpObjStateAsync(objectID, objectTypeID, userName, TimeOut);
            }

            //设置locklock字段
            PostLockObject_param_in pTemp = new PostLockObject_param_in()
            {
                ObjectID = objectID,
                ObjectTypeID = objectTypeID,
                userName = userName,
                TimeOut = 500
            };
            DbpObjectstateinfo objectstateinfo = await AutoRetry.RunSync(Store.UpdateObjectInfoLockAsync, pTemp, 3, 100);
            if (objectstateinfo == null)
            {
                return false;
            }

            //修改加锁信息，清空locklock
            return await Store.UnLockRowsAsync(objectstateinfo, TimeOut);

        }

        //un lock obj
        public async Task<bool> SetUnlockObjectAsync(int objectID, OTID objectTypeID, string userName)
        {
            //设置locklock字段
            PostLockObject_param_in pTemp = new PostLockObject_param_in()
            {
                ObjectID = objectID,
                ObjectTypeID = objectTypeID,
                userName = userName,
                TimeOut = 500
            };
            DbpObjectstateinfo objectstateinfo = await AutoRetry.RunSync(Store.UpdateObjectInfoLockAsync, pTemp, 3, 100);
            if (objectstateinfo == null)
            {
                return false;
            }

            return await Store.UnLockObjectAsync(objectstateinfo);
        }

        //GetAllUnlockObjects
        public async Task<List<ObjectContent>> GetVTRUnlockObjectsAsync()
        {
            DateTime time = DateTime.Now;
            return await Store.GetObjectstateinfoListAsync<ObjectContent>(x => x.Where(a => a.Objecttypeid == (int)OTID.OTID_VTR && string.IsNullOrEmpty(a.Locklock) && a.Begintime.AddMilliseconds(Convert.ToInt32(a.Timeout)) > time)
            .Select(res => new ObjectContent
            {
                ObjectID = res.Objectid,
                ObjectType = (OTID)res.Objecttypeid,
                UserName = res.Username,
                BeginTime = res.Begintime,
                TimeOut = res.Timeout
            }), true);
        }

        //check channel is lock?
        public async Task<bool> IsChannelLock(int nChannel)
        {
            bool ret = false;
            try
            {
                DateTime time = DateTime.Now;
                var objectsateinfo = await Store.GetObjectstateinfoAsync(a => a.Where(x => x.Objectid == nChannel && x.Objecttypeid == (int)OTID.OTID_CHANNEL && x.Begintime.AddMilliseconds(x.Timeout) > time));

                ret = objectsateinfo == null ? true : false;//true 无锁
            }
            catch (Exception ex)
            {
                SobeyRecException.ThrowSelfNoParam(nChannel.ToString(), GlobalDictionary.GLOBALDICT_CODE_IN_ISCHANNELLOCK_READ_DATA_FAILED, null, ex);
            }
            return ret;

        }

        #endregion

        #region User

        public Task AddUserLoginInfo(UserLoginInfoRequest logininfo)
        {
            if (logininfo.Ip != "0.0.0.0")
            {
                logininfo.Logintime = DateTime.Now;
                logininfo.Port = 5566;
                return Store.AddUserLoginInfoAsync(_mapper.Map<DbpUserLoginInfo>(logininfo));
            }
            return Task.CompletedTask;
        }

        public Task<bool> DeleteUserLoginInfoByIP(string ip)
        {
            return Store.DeleteUserLoginInfoByIPAsync(ip);
        }

        public Task<bool> DeleteUserLoginInfoByUserCode(string usercode)
        {
            return Store.DeleteUserLoginInfoByUserCodeAsync(usercode);
        }

        public async Task<List<UserLoginInfoResponse>> GetAllUserLoginInfo()
        {
            return _mapper.Map<List<UserLoginInfoResponse>>(await Store.GetAllUserLoginInfoAsync());
        }

        //get user setting
        public async Task<string> GetUserSettingAsync(string UserCode, string Settingtype)
        {
            string settingText = string.Empty;
            var userSetting = await Store.GetUserSettingAsync(a => a.Where(x => x.Usercode == UserCode && x.Settingtype == Settingtype), true);

            if (userSetting != null)
            {
                settingText = !string.IsNullOrWhiteSpace(userSetting.Settingtext) ? userSetting.Settingtext : userSetting.Settingtextlong;
            }
            return settingText;
        }

        //add or update
        public async Task UpdateUserSettingAsync(string userCode, string settingType, string settingText)
        {
            if (userCode == string.Empty || settingType == string.Empty)
            {
                return;
            }

            int byteLength = 0;
            byteLength = Encoding.UTF8.GetByteCount(settingText);

            //修改为当字符串的字节数小于4000时，将METADATA存入到METADATA中，反之存到METADATALONG，不过在存之前先判断字符串的字符数，
            //若字符数小于或等于4000，将补足空格到4001个字符，存入到METADATALONG中。
            var usersetting = new DbpUsersettings()
            {
                Usercode = userCode,
                Settingtype = settingType
            };

            if (byteLength > 4000)
            {
                usersetting.Settingtextlong = FillBlankToString(settingText, byteLength);
            }
            else
            {
                usersetting.Settingtext = settingText;
            }

            await Store.UpdateUsersettingsAsync(usersetting);
        }

        private string FillBlankToString(string strTemp, int byteLength)
        {
            //升级到VS2005时删除
            int charLength = strTemp.Length;
            if (byteLength > 4000 && charLength <= 4000)
            {
                strTemp = strTemp.PadRight(4001);
            }

            return strTemp;
        }



        #region CaptureTemplate
        //get Captureparam
        public async Task<string> GetCapParamTemplateByIDAsync(int nCaptureParamID)
        {
            var capturetemplate = await Store.GetCaptureparamtemplateAsync(a => a.Where(x => x.Captureparamid == nCaptureParamID), true);

            if (capturetemplate == null || string.IsNullOrEmpty(capturetemplate.Captureparam))
            {
                return string.Empty;
            }

            return capturetemplate.Captureparam;
        }
        //deal captureparam xml
        public string DealCaptureParam(string captureparam, int nFlag)
        {
            string strCaptureparam = null;
            //在模板头尾加上根节点以便于xml解析
            string temp = captureparam;
            temp = "<root>" + temp + "</root>";
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(temp);
            int i = 0;
            foreach (XmlNode n in xml.SelectNodes("/root/CAPTUREPARAM"))
            {
                //HD
                if (nFlag == 1 && i == 0)
                {
                    strCaptureparam = n.OuterXml;
                    break;
                }
                //SD
                else if (nFlag == 0 && i == 1)
                {
                    strCaptureparam = n.OuterXml;
                    break;
                }
                //UHD
                else if (nFlag == 2 && i == 2)
                {
                    strCaptureparam = n.OuterXml;
                    break;
                }
                ++i;
            }
            return strCaptureparam;
        }


        public async Task<int> ModifyCaptureParamTemplateAsync(int nParamTemplateID, string strTemplateName, string strUserCaptureParam)
        {
            if (string.IsNullOrEmpty(strTemplateName) || string.IsNullOrEmpty(strUserCaptureParam))
            {
                return -1;
            }

            return await Store.UpdateCaptureParamTemplateAsync(nParamTemplateID, strTemplateName, strUserCaptureParam);
        }

        public async Task DelCaptureParamTemplateAsync(int nParamTemplateID)
        {
            var dbpCapTemplate = await Store.GetCaptureparamtemplateAsync(a => a.Where(x => x.Captureparamid == nParamTemplateID));

            if (dbpCapTemplate == null)
            {
                SobeyRecException.ThrowSelfNoParam(nParamTemplateID.ToString(), GlobalDictionary.GLOBALDICT_CODE_USER_PARAM_ID_DOES_NOT_EXIST, Logger, null);
                return;
            }

            await Store.DeleteCaptureParamTemplateAsync(dbpCapTemplate);
        }

        //可能有问题
        public async Task<List<TResult>> GetAllCatureParamTemplateAsync<TResult>()
        {
            var result = await Store.GetCaptureparamtemplateListAsync(a => a, true);

            return _mapper.Map<List<TResult>>(result);
        }

        public async Task<List<TResult>> GetAllCatureParamTemplateAsync<TResult>(int nFlag)
        {
            var result = await Store.GetCaptureparamtemplateListAsync(a => a, true);
            foreach (var item in result)
            {
                string strUserCaptureParam = item.Captureparam;
                int pos = strUserCaptureParam.IndexOf("</CAPTUREPARAM>");
                if (nFlag == 1)
                {
                    strUserCaptureParam = strUserCaptureParam.Substring(0, pos + 15);
                }
                else
                {
                    int nLen = strUserCaptureParam.Length - pos - 15;
                    if (nLen <= 0)
                    {
                        strUserCaptureParam = "";
                    }
                    else
                        strUserCaptureParam = strUserCaptureParam.Substring(pos + 15, nLen);
                }
                item.Captureparam = strUserCaptureParam;
            }
            return _mapper.Map<List<TResult>>(result);
        }

        public async Task<string> GetCapParamTemplateByUserCodeAsync(string UserCode)
        {
            var dbpUserParamMap = await Store.GetUserParamMapAsync(a => a.Where(x => x.Usercode == UserCode), true);
            if (dbpUserParamMap == null)
            {
                return string.Empty;
            }

            var dbpCapParamTemplate = await Store.GetCaptureparamtemplateAsync(a => a.Where(x => x.Captureparamid == dbpUserParamMap.Captureparamid), true);

            if (dbpCapParamTemplate == null)
            {
                SobeyRecException.ThrowSelfNoParam(UserCode, GlobalDictionary.GLOBALDICT_CODE_USER_PARAM_ID_DOES_NOT_EXIST, Logger, null);
            }

            return dbpCapParamTemplate.Captureparam;
        }

        public async Task<CapParamTemplate> GetCapParamTemplateByUserCodeDB2Async(string strUserCode)
        {

            var result = await Store.GetUserParamForDB2Async(strUserCode);
            return _mapper.Map<CapParamTemplate>(result);
        }

        public async Task<CapParamTemplate> GetCapParamTemplateByUserV2(string strUserCode)
        {
            var dbpUserParamMap = await Store.GetUserParamMapAsync(a => a.Where(x => x.Usercode == strUserCode), true);
            if (dbpUserParamMap == null)
            {
                SobeyRecException.ThrowSelfNoParam(strUserCode, GlobalDictionary.GLOBALDICT_CODE_USER_PARAM_DOES_NOT_EXIST, Logger, null);
            }

            var dbpCapParamTemplate = await Store.GetCaptureparamtemplateAsync(a => a.Where(x => x.Captureparamid == dbpUserParamMap.Captureparamid), true);

            if (dbpCapParamTemplate == null)
            {
                SobeyRecException.ThrowSelfNoParam(strUserCode, GlobalDictionary.GLOBALDICT_CODE_USER_PARAM_ID_DOES_NOT_EXIST, Logger, null);
            }
            return _mapper.Map<CapParamTemplate>(dbpCapParamTemplate);
        }

        public async Task<List<UserParmMap>> GetAllUserParamMap()
        {
            var userParamMap = await Store.GetUserParamMapListAsync(a => a, true);
            return _mapper.Map<List<UserParmMap>>(userParamMap);
        }

        public async Task ModifyAllUserParamMapAsync(List<UserParmMap> arUserParmMapList)
        {
            var dbpUserMaps = _mapper.Map<List<DbpUserparamMap>>(arUserParmMapList);
            await Store.UpdateAllUserParamMapAsync(dbpUserMaps);
        }

        #endregion

        #region UserTemplate
        public async Task<int> UserTemplateInsertAsync(int templateID, string userCode, string templateName, string templateContent)
        {
            var usertemplate = await Store.GetUsertemplateAsync(a => a.Where(x => x.Usercode == userCode && x.Templatename == templateName && x.Templateid != templateID && x.Templateid > 0), true);

            if (usertemplate != null)
            {
                SobeyRecException.ThrowSelfOneParam(templateName, GlobalDictionary.GLOBALDICT_CODE_THE_USER_TEMPLATE_HAS_EXISTS_ONEPARAM, null, string.Format(GlobalDictionary.Instance.GetMessageByCode(GlobalDictionary.GLOBALDICT_CODE_THE_USER_TEMPLATE_HAS_EXISTS_ONEPARAM),
                    templateName), null);
                return -1;
            }

            if (templateID <= 0)
            {
                templateID = Store.GetNextValId("DBP_SQ_UESRTEMPLATEID");
            }

            //DBACCESS.AddUserTemplate(userTemplate);
            if (!string.IsNullOrWhiteSpace(templateContent))
            {
                int byteLength = Encoding.UTF8.GetByteCount(templateContent);
                templateContent = FillBlankToString(templateContent, byteLength);
            }
            await Store.InsertUserTemplateAsync(templateID, userCode, templateName, templateContent);

            return templateID;
        }

        //modify
        public async Task ModifyUserTemplateAsync(int TemplateID, string TemplateContent, string NewTemplateName)
        {
            DbpUsertemplate userTemplate = await Store.GetUsertemplateAsync(a => a.Where(x => x.Templateid == TemplateID));

            if (userTemplate == null || userTemplate.Templateid <= 0)
            {
                SobeyRecException.ThrowSelfOneParam("get user template by id is null", GlobalDictionary.GLOBALDICT_CODE_CAN_NOT_FIND_THE_TEMPLATE_ID_IS_ONEPARAM, Logger, TemplateID, null);
                return;
            }

            if (!string.IsNullOrWhiteSpace(NewTemplateName))
            {
                var userTemplateOther = await Store.GetUsertemplateAsync(a => a.Where(x => x.Usercode == userTemplate.Usercode && x.Templatename == NewTemplateName && x.Templateid != TemplateID), true);
                if (userTemplateOther != null && userTemplateOther.Templateid > 0)
                {
                    SobeyRecException.ThrowSelfOneParam("user template name is exist", GlobalDictionary.GLOBALDICT_CODE_THE_USER_TEMPLATE_HAS_EXISTS_ONEPARAM, Logger, TemplateID, null);
                    return;
                }
            }

            await Store.UpdateDbpUserTempalteAsync(userTemplate, TemplateContent, NewTemplateName);

        }

        public async Task UpdateUserTempalteContent(int nTemplateID, string strTemplateContent)
        {
            DbpUsertemplate userTemplate = await Store.GetUsertemplateAsync(a => a.Where(x => x.Templateid == nTemplateID));
            if (userTemplate == null || userTemplate.Templateid <= 0)
            {
                SobeyRecException.ThrowSelfOneParam(nTemplateID.ToString(), GlobalDictionary.GLOBALDICT_CODE_CAN_NOT_FIND_THE_TEMPLATE_ID_IS_ONEPARAM, null, string.Format(GlobalDictionary.Instance.GetMessageByCode(GlobalDictionary.GLOBALDICT_CODE_CAN_NOT_FIND_THE_TEMPLATE_ID_IS_ONEPARAM),
                    nTemplateID), null);
                return;
            }

            await Store.UpdateDbpUserTempalteAsync(userTemplate, strTemplateContent, null);
        }

        public async Task ModifyUserTemplateNameAsync(int nTemplateID, string strNewTemplateName)
        {
            DbpUsertemplate userTemplate = await Store.GetUsertemplateAsync(a => a.Where(x => x.Templateid == nTemplateID));

            if (userTemplate == null || userTemplate.Templateid <= 0)
            {
                //SobeyRecException.ThrowSelf(string.Format("Can not find the template.ID = {0}",templateID),10013004);
                SobeyRecException.ThrowSelfOneParam("get user template by id is null", GlobalDictionary.GLOBALDICT_CODE_CAN_NOT_FIND_THE_TEMPLATE_ID_IS_ONEPARAM, Logger, nTemplateID, null);
                return;
            }

            //string userCode = userTemplate.Usercode;

            var userTemplateOther = await Store.GetUsertemplateAsync(a => a.Where(x => x.Usercode == userTemplate.Usercode && x.Templatename == strNewTemplateName && x.Templateid != nTemplateID), true);
            if (userTemplateOther != null && userTemplateOther.Templateid > 0)
            {
                SobeyRecException.ThrowSelfOneParam("user template name is exist", GlobalDictionary.GLOBALDICT_CODE_THE_USER_TEMPLATE_HAS_EXISTS_ONEPARAM, Logger, nTemplateID, null);
                return;
            }
            await Store.UpdateDbpUserTempalteAsync(userTemplate, null, strNewTemplateName);

        }

        //get all user template by usercode
        public async Task<List<TResult>> GetUserAllTemplatesAsync<TResult>(string UserCode)
        {
            var dbpUserTempList = await Store.GetUsertemplateLstAsync(a => a.Where(x => x.Usercode == UserCode), true);

            return _mapper.Map<List<TResult>>(dbpUserTempList);
        }

        //delete by templateId
        public Task DeleteUserTemplateAsync(int TemplateID)
        {
            return Store.DeleteUserTemplateAsync(TemplateID);
        }

        //delete user param map
        public async Task DelUserParamTemplateAsync(string UserCode)
        {
            var dbpParamMap = await Store.GetUserParamMapAsync(a => a.Where(x => x.Usercode == UserCode));

            if (dbpParamMap == null)
            {
                SobeyRecException.ThrowSelfNoParam(UserCode, GlobalDictionary.GLOBALDICT_CODE_USER_ID_DOES_NOT_EXIST, Logger, null);
                return;
            }

            await Store.DeleteUserParamMapAsync(dbpParamMap);
        }

        public async Task ModUserParamTemplateAsync(string strUserCode, int nParamTemplateID)
        {
            await Store.UpdateUserParamMapAsync(strUserCode, nParamTemplateID);

        }

        public async Task<int> GetUserCaptureParamIdAsync(string strUserCode)
        {
            var dbpParamMap = await Store.GetUserParamMapAsync(a => a.Where(x => x.Usercode == strUserCode), true);

            if (dbpParamMap == null)
            {
                SobeyRecException.ThrowSelfNoParam(strUserCode, GlobalDictionary.GLOBALDICT_CODE_USER_PARAM_ID_DOES_NOT_EXIST, Logger, null);
            }

            return dbpParamMap.Captureparamid == null ? -1 : (int)dbpParamMap.Captureparamid;
        }

        public async Task<List<TResult>> GetUserTemplateAsync<TResult>(string userCode, string templateName)
        {
            var dbpUserTempLst = await Store.GetUsertemplateLstAsync(a => a.Where(x => x.Usercode == userCode && x.Templatename == templateName), true);

            return _mapper.Map<List<TResult>>(dbpUserTempLst);
        }

        public async Task<TResult> GetUserTemplateByIDAsync<TResult>(int templateID)
        {
            var dbpUserTemp = await Store.GetUsertemplateAsync(a => a.Where(x => x.Templateid == templateID));
            return _mapper.Map<TResult>(dbpUserTemp);
        }

        #endregion

        #region CMapi user

        /// <summary>
        /// 调用cmapi获取userInfo
        /// </summary>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="userCode"></param>
        /// <returns></returns>
        public async Task<TResponse> GetUserInfoByUserCodeAsync<TResponse>(string userCode)
        {
            var userInfo = await _restClient.GetUserInfo(false, "admin", userCode);

            return _mapper.Map<TResponse>(userInfo);
        }


        /// <summary>
        /// 调用cmapi获取paramvalue
        /// </summary>
        /// <param name="useTokenCode"></param>
        /// <param name="userCode"></param>
        /// <returns></returns>
        public async Task<int> GetUserParamTemplateIDAsync(bool useTokenCode, string userCode)
        {
            return await _restClient.GetUserParamTemplateID(useTokenCode, userCode);
        }


        #endregion

        #endregion

    }
}
