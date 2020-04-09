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
    }
}
