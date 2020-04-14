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

namespace IngestGlobalPlugin.Managers
{
    public class GlobalManager
    {
        protected IGlobalStore Store { get; }
        protected IMapper _mapper { get; }
        protected RestClient _restClient;

        public GlobalManager(IGlobalStore store, IMapper mapper, RestClient rsc)
        {
            Store = store;
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _restClient = rsc;
        }


        #region global Manager
        public async Task<string> GetValueStringAsync(string strKey)
        {
            return await Store.GetGlobalValueStringAsync(strKey);
        }

        public async Task UpdateGlobalValueAsync(string strKey, string strValue)
        {
            await Store.UpdateGlobalValueAsync(strKey, strValue);
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
        public async Task<TResult> GetAllGlobalStateAsync<TResult>()
        {
            var globalstates = await Store.GetAllGlobalStateAsync();
            return _mapper.Map<TResult>(globalstates);

        }

        public async Task UpdateGlobalStateAsync(string strLabel)
        {
            await Store.UpdateGlobalStateAsync(strLabel);
        }
        #endregion

        #region Lock Objects Manager
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
            DbpObjectstateinfo objectstateinfo = null;
            for (int i = 0; i < 3 && objectstateinfo == null; i++)
            {
                if (i > 0)
                {
                    System.Threading.Thread.Sleep(100);
                }

                objectstateinfo = await Store.LockRowsByConditionAsync(objectID, objectTypeID, userName);
            }

            if (objectstateinfo == null)
            {
                return false;
            }

            //修改加锁信息，清空locklock
            return await Store.UnLockRowsAsync(objectstateinfo, TimeOut);
            //return await Store.SetLockObject(objectID, objectTypeID, userName, TimeOut);
        }

        public async Task<bool> SetUnlockObjectAsync(int objectID, OTID objectTypeID, string userName)
        {
            //设置locklock字段
            DbpObjectstateinfo objectstateinfo = null;
            for (int i = 0; i < 3 && objectstateinfo == null; i++)
            {
                if (i > 0)
                {
                    System.Threading.Thread.Sleep(100);
                }

                objectstateinfo = await Store.LockRowsByConditionAsync(objectID, objectTypeID, userName);
            }

            if (objectstateinfo == null)
            {
                return false;
            }

            return await Store.UnLockObjectAsync(objectstateinfo);
            //return await Store.SetUnLockObject(objectID, objectTypeID, userName);
        }

        //GetAllUnlockObjects
        public async Task<List<ObjectContent>> GetVTRUnlockObjects()
        {
            return await Store.GetObjectstateinfoListAsync<ObjectContent>(x => x.Where(a => a.Objecttypeid == (int)OTID.OTID_VTR && string.IsNullOrEmpty(a.Locklock) && a.Begintime.AddMilliseconds(Convert.ToInt32(a.Timeout)) > DateTime.Now)
            .Select(res => new ObjectContent
            {
                ObjectID = res.Objectid,
                ObjectType = (OTID)res.Objecttypeid,
                UserName = res.Username,
                BeginTime = res.Begintime,
                TimeOut = res.Timeout
            }), true);
        }

        //
        public async Task<bool> IsChannelLock(int nChannel)
        {
            bool ret = false;
            try
            {
                var objectsateinfo = await Store.GetObjectstateinfoAsync(a => a.Where(x => x.Objectid == nChannel && x.Objecttypeid == (int)OTID.OTID_CHANNEL && x.Begintime.AddMilliseconds(x.Timeout) > DateTime.Now));

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

        public async Task<string> GetUserSettingAsync(string strUserCode, string strSettingtype)
        {
            string settingText = string.Empty;
            var userSetting = await Store.GetUserSettingAsync(a => a.Where(x => x.Usercode == strUserCode && x.Settingtype == strSettingtype), true);
            
            if(userSetting!= null)
            {
                settingText = !string.IsNullOrWhiteSpace(userSetting.Settingtext) ? userSetting.Settingtext : userSetting.Settingtextlong;
            }
            return settingText;
        }

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
        public async Task<string> GetParamTemplateByIDAsync(int nCaptureParamID, int nFlag)
        {
            string strCaptureparam = null;
            var capturetemplate = await Store.GetCaptureparamtemplateAsync(a => a.Where(x => x.Captureparamid == nCaptureParamID), true);

            if (capturetemplate == null || string.IsNullOrEmpty(capturetemplate.Captureparam))
            {
                return string.Empty;
            }

            //在模板头尾加上根节点以便于xml解析
            string temp = capturetemplate.Captureparam;
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
                templateID = IngestGlobalDBContext.next_val("DBP_SQ_UESRTEMPLATEID");
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

        public async Task UpdateUserTempalteContent(int nTemplateID, string strTemplateContent)
        {
            DbpUsertemplate userTemplate = await Store.GetUsertemplateAsync(a => a.Where( x=> x.Templateid == nTemplateID), true);
            if (userTemplate !=null && userTemplate.Templateid <= 0)
            {
                SobeyRecException.ThrowSelfOneParam(nTemplateID.ToString(), GlobalDictionary.GLOBALDICT_CODE_CAN_NOT_FIND_THE_TEMPLATE_ID_IS_ONEPARAM,null,  string.Format(GlobalDictionary.Instance.GetMessageByCode(GlobalDictionary.GLOBALDICT_CODE_CAN_NOT_FIND_THE_TEMPLATE_ID_IS_ONEPARAM),
                    nTemplateID),  null);
                return;
            }

            await Store.ModifyUserTempalteContent(nTemplateID, strTemplateContent);
        }

        public async Task<List<TResult>> GetUserAllTemplatesAsync<TResult>(string strUserCode)
        {
            var dbpUserTempList = await Store.GetUsertemplateLstAsync(a => a.Where(x => x.Usercode == strUserCode), true);

            return _mapper.Map<List<TResult>>(dbpUserTempList);
        }


        #endregion

        #region CMapi user


        public async Task<ResponseMessage<TResponse>> GetUserInfoByUserCodeAsync<TResponse>(string strUserCode)
        {
            var result = await AutoRetry.Run(ExcuteUserInfo<TResponse>, strUserCode);
            //return _mapper.Map<TResponse>(result);
            return result;
        }
        

        public async Task<ResponseMessage<TResult>> ExcuteUserInfo<TResult>(string strUserCode)
        {
            ResponseMessage<TResult> result = new ResponseMessage<TResult>();
            string uri = string.Format("http://{0}/CMApi/api/basic/account/getuserinfobyusercode?usercode={1}", ApplicationContext.Current.CMServerUrl, strUserCode);
            
            ResponseMessageN<OldCMUserInfo> reres = await _restClient.GetCmApi<ResponseMessageN<OldCMUserInfo>>(uri);

            if (reres.Code == "0")
            {
                result.Ext = _mapper.Map<TResult>(reres.ext);
                result.Code = ResponseCodeDefines.SuccessCode;
            }
            else
            {
                result.Ext = default(TResult);
                result.Code = ResponseCodeDefines.PartialFailure;
            }

            return result;
        }

        public async Task<ResponseMessage<TResponse>> GetHighOrStandardParamAsync<TResponse>(string szUserToken)
        {
            var result = await AutoRetry.Run(GetHighOrStandardParam<TResponse>, szUserToken);
            return result;
        }

        public async Task<ResponseMessage<TResult>> GetHighOrStandardParam<TResult>(string szUserToken)
        {
            userparameter param = new userparameter();
            param.tool = "DEFAULT";
            param.paramname = "HIGH_RESOLUTION";
            param.system = "INGEST";

            string uri = string.Format("http://{0}/CMApi/api/basic/config/getuserparam/", ApplicationContext.Current.CMServerUrl);

            ResponseMessage<TResult> result = new ResponseMessage<TResult>();
            var dicHeader = new Dictionary<string, string>();
            dicHeader.Add("sobeyhive-http-token", szUserToken);
            ResponseMessageN<etparam> reres = await _restClient.Post<ResponseMessageN<etparam>>(uri, param, dicHeader);

            if (reres.Code == "0")
            {
                result.Ext = _mapper.Map<TResult>(reres.ext);
                result.Code = ResponseCodeDefines.SuccessCode;
            }
            else
            {
                result.Ext = default(TResult);
                result.Code = ResponseCodeDefines.PartialFailure;
            }
            
            return result;
        }

        #endregion

        #endregion

    }
}
