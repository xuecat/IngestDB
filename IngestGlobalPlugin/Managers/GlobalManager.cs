using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using IngestDBCore;
using IngestGlobalPlugin.Stores;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using IngestGlobalPlugin.Dto;

namespace IngestGlobalPlugin.Managers
{
    public class GlobalManager
    {
        protected IGlobalStore Store { get; }

        public GlobalManager(IGlobalStore store)
        {
            Store = store;
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

        internal async Task<bool> SetLockObject(int objectID, OTID objectTypeID, string userName, int TimeOut)
        {


            return await Store.SetLockObject(objectID, objectTypeID, userName, TimeOut);
        }

        internal async Task<bool> SetUnlockObject(int objectID, OTID objectTypeID, string userName)
        {
            return await Store.SetUnLockObject(objectID, objectTypeID, userName);
        }

        internal async Task<GetGlobalState_OUT> GetAllGlobalState()
        {
            return await Store.GetAllGlobalState();
        }

        internal async Task<bool> UpdateGlobalValueAsync(string strKey, string strValue)
        {
            return await Store.UpdateGlobalValueAsync(strKey, strValue);
        }
    }
}
