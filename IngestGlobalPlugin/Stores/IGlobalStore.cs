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
        Task<DbpObjectstateinfo> GetObjStateInfoAsync(Func<IQueryable<DbpObjectstateinfo>, IQueryable<DbpObjectstateinfo>> query, bool notrack = false);
        Task<bool> AddDbpObjStateAsync(int objectID, OTID objectTypeID, string userName, int TimeOut);
        Task<DbpObjectstateinfo> LockRowsAsync(int objectID, OTID objectTypeID, string userName, int TimeOut = 500);
        Task<bool> UnLockRowsAsync(DbpObjectstateinfo objectstateinfo, int TimeOut);
        Task<bool> UnLockObjectAsync(DbpObjectstateinfo arrObjects);

        Task UpdateGlobalStateAsync(string strLabel);
        Task<GlobalTcResponse> GetDefaultSTC(TC_MODE tcMode);
        Task<bool> SetLockObject(int objectID, OTID objectTypeID, string userName, int TimeOut);
        Task<bool> SetUnLockObject(int objectID, OTID objectTypeID, string userName);
        Task<GetGlobalState_OUT> GetAllGlobalState();

        Task<string> GetValueStringAsync(string strKey);
        Task UpdateGlobalValueAsync(string strKey, string strValue);
    }
}
