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

namespace IngestGlobalPlugin.Managers
{
    public class GlobalManager
    {
        protected IGlobalStore Store { get; }
        protected IMapper _mapper { get; }

        public GlobalManager(IGlobalStore store, IMapper mapper)
        {
            Store = store;
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
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

            var dbpObjState = await Store.GetObjectstateinfoAsync(a => a.Where(x => x.Objectid == objectID && x.Objecttypeid == (int)objectTypeID));
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
            .Select(res => new ObjectContent {
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

        public async Task<string> GetParamTemplateByIDAsync(int nCaptureParamID, int nFlag)
        {
            string strCaptureparam = null;
            var capturetemplate = await Store.GetCaptureparamtemplateAsync(a => a.Where(x=>x.Captureparamid == nCaptureParamID));

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

        #region UserTemplate
        public async Task<int> AddUserTemplateAsync(UserTemplate userTemplate)
        {
            var usertemplate = await Store.GetUsertemplateAsync(a => a.Where(x => x.Usercode == userTemplate.strUserCode && x.Templatename == userTemplate.strTemplateName && x.Templateid != userTemplate.nTemplateID));

            if(usertemplate != null && usertemplate.Templateid > 0)
            {
                SobeyRecException.ThrowSelfOneParam(userTemplate.strTemplateName, GlobalDictionary.GLOBALDICT_CODE_THE_USER_TEMPLATE_HAS_EXISTS_ONEPARAM, null, string.Format(GlobalDictionary.Instance.GetMessageByCode(GlobalDictionary.GLOBALDICT_CODE_THE_USER_TEMPLATE_HAS_EXISTS_ONEPARAM),
                    userTemplate.strTemplateName), null);
                return -1;
            }
            
            //if (userTemplate.nTemplateID <= 0)
            //{
            //    userTemplate.nTemplateID = SequenceFactory.GetSequenceGennerator().GetSequenceID("DBP_SQ_UESRTEMPLATEID");
            //}

            //DBACCESS.AddUserTemplate(userTemplate);

            return userTemplate.nTemplateID;
        }

        #endregion

        #endregion

    }
}
