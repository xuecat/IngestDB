using IngestGlobalPlugin.Dto;
using IngestGlobalPlugin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IngestGlobalPlugin.Stores
{
    public interface IGlobalStore
    {
        Task UpdateGlobalStateAsync(string strLabel);
        Task<GlobalTcResponse> GetDefaultSTC(TC_MODE tcMode);
        Task<bool> SetLockObject(int objectID, OTID objectTypeID, string userName, int TimeOut);
        Task<bool> SetUnLockObject(int objectID, OTID objectTypeID, string userName);
        Task<GetGlobalState_OUT> GetAllGlobalState();

        Task<string> GetValueStringAsync(string strKey);
        Task<bool> UpdateGlobalValueAsync(string strKey, string strValue);
    }
}
