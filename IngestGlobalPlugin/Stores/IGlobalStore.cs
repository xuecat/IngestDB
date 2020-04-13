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
        Task<bool> SetLockObject(int objectID, OTID objectTypeID, string userName, int TimeOut);
        Task<bool> SetUnLockObject(int objectID, OTID objectTypeID, string userName);
        //Task<GetGlobalState_OUT> GetAllGlobalState();

        #region global
        Task<string> GetGlobalValueStringAsync(string strKey);
        Task UpdateGlobalValueAsync(string strKey, string strValue);
        #endregion

        #region globalstate interface
        Task<List<DbpGlobalState>> GetAllGlobalStateAsync();
        Task UpdateGlobalStateAsync(string strLabel);
        #endregion

        #region Objectstateinfo
        Task<TResult> GetObjectstateinfoAsync<TResult>(Func<IQueryable<DbpObjectstateinfo>, IQueryable<TResult>> query, bool notrack = false);
        Task<List<TResult>> GetObjectstateinfoListAsync<TResult>(Func<IQueryable<DbpObjectstateinfo>, IQueryable<TResult>> query, bool notrack = false);
        Task<bool> AddDbpObjStateAsync(int objectID, OTID objectTypeID, string userName, int TimeOut);
        Task<DbpObjectstateinfo> LockRowsByConditionAsync(int objectID, OTID objectTypeID, string userName, int TimeOut = 500);
        Task<bool> UnLockRowsAsync(DbpObjectstateinfo objectstateinfo, int TimeOut);
        Task<bool> UnLockObjectAsync(DbpObjectstateinfo arrObjects);
        #endregion

        #region user
        Task<TResult> GetUserSettingAsync<TResult>(Func<IQueryable<DbpUsersettings>, IQueryable<TResult>> query, bool notrack = false);

        Task UpdateUsersettingsAsync(DbpUsersettings usersetting);

        #endregion

        #region Capturetemplate

        Task<TResult> GetCaptureparamtemplateAsync<TResult>(Func<IQueryable<DbpCaptureparamtemplate>, IQueryable<TResult>> query, bool notrack = false);

        #endregion

        #region Usertemplate

        Task<TResult> GetUsertemplateAsync<TResult>(Func<IQueryable<DbpUsertemplate>, IQueryable<TResult>> query, bool notrack = false);

        Task<List<TResult>> GetUsertemplateLstAsync<TResult>(Func<IQueryable<DbpUsertemplate>, IQueryable<TResult>> query, bool notrack = false);

        Task InsertUserTemplateAsync(int templateID, string userCode, string templateName, string templateContent);
        Task ModifyUserTempalteContent(int templateID, string templateContent);

        #endregion

    }
}
