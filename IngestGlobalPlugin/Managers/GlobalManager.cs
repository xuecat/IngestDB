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


        public async Task SetGlobalState(string strLabel)
        {
            await Store.UpdateGlobalStateAsync(strLabel);
        }

        public async Task<string> GetValueStringAsync(string strKey)
        {
            return await Store.GetValueStringAsync(strKey);
        }

        public async Task<GlobalTcResponse> GetDefaultSTC(TC_MODE tcMode)
        {
            return await Store.GetDefaultSTC(tcMode);
        }

        public async Task<bool> SetLockObjectAsync(int objectID, OTID objectTypeID, string userName, int TimeOut)
        {
            if (TimeOut < 0)
                TimeOut *= -1;

            var dbpObjState = await Store.GetObjStateInfoAsync(a => a.Where(x => x.Objectid == objectID && x.Objecttypeid == (int)objectTypeID));
            if(dbpObjState == null)
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

                objectstateinfo = await Store.LockRowsAsync(objectID, objectTypeID, userName);
            }

            if(objectstateinfo == null)
            {
                return false;
            }

            //修改加锁信息，清空locklock
            return await Store.UnLockRowsAsync(objectstateinfo, TimeOut);
            //return await Store.SetLockObject(objectID, objectTypeID, userName, TimeOut);
        }

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

                objectstateinfo = await Store.LockRowsAsync(objectID, objectTypeID, userName);
            }

            if (objectstateinfo == null)
            {
                return false;
            }
            
            return await Store.UnLockObjectAsync(objectstateinfo);
            //return await Store.SetUnLockObject(objectID, objectTypeID, userName);
        }

        internal async Task<GetGlobalState_OUT> GetAllGlobalState()
        {
            return await Store.GetAllGlobalState();
        }

        public async Task UpdateGlobalValueAsync(string strKey, string strValue)
        {
            await Store.UpdateGlobalValueAsync(strKey, strValue);
        }
    }
}
