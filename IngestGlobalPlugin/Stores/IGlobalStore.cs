using IngestGlobalPlugin.Dto;
using IngestGlobalPlugin.Dto.OldResponse;
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
        Task<TResult> GetGlobalAsync<TResult>(Func<IQueryable<DbpGlobal>, IQueryable<TResult>> query, bool notrack = false);
        //Task<string> GetGlobalValueStringAsync(string strKey);
        Task<bool> UpdateGlobalValueAsync(string strKey, string strValue);
        #endregion

        #region globalstate interface
        Task<TResult> GetGlobalStateAsync<TResult>(Func<IQueryable<DbpGlobalState>, IQueryable<TResult>> query, bool notrack = false);
        Task<List<TResult>> GetGlobalStateListAsync<TResult>(Func<IQueryable<DbpGlobalState>, IQueryable<TResult>> query, bool notrack = false);
        Task<List<DbpGlobalState>> GetAllGlobalStateAsync();
        Task UpdateGlobalStateAsync(string strLabel);
        #endregion

        #region Objectstateinfo
        Task<TResult> GetObjectstateinfoAsync<TResult>(Func<IQueryable<DbpObjectstateinfo>, IQueryable<TResult>> query, bool notrack = false);
        Task<List<TResult>> GetObjectstateinfoListAsync<TResult>(Func<IQueryable<DbpObjectstateinfo>, IQueryable<TResult>> query, bool notrack = false);
        Task<bool> AddDbpObjStateAsync(int objectID, OTID objectTypeID, string userName, int TimeOut);
        Task<DbpObjectstateinfo> LockRowsByConditionAsync(int objectID, OTID objectTypeID, string userName, int TimeOut = 500);
        Task<DbpObjectstateinfo> UpdateObjectInfoLockAsync(PostLockObject_param_in param_In);
        Task<bool> UnLockRowsAsync(DbpObjectstateinfo objectstateinfo, int TimeOut);
        Task<bool> UnLockObjectAsync(DbpObjectstateinfo arrObjects);
        #endregion

        #region user

        Task<TResult> GetUserSettingAsync<TResult>(Func<IQueryable<DbpUsersettings>, IQueryable<TResult>> query, bool notrack = false);

        Task UpdateUsersettingsAsync(DbpUsersettings usersetting);

        Task AddUserLoginInfoAsync(DbpUserLoginInfo logininfo);
        Task<bool> DeleteUserLoginInfoByIPAsync(string ip);
        Task<bool> DeleteUserLoginInfoByUserCodeAsync(string usercode);
        Task<List<DbpUserLoginInfo>> GetAllUserLoginInfoAsync();

        #endregion

        #region Capturetemplate

        Task<TResult> GetCaptureparamtemplateAsync<TResult>(Func<IQueryable<DbpCaptureparamtemplate>, IQueryable<TResult>> query, bool notrack = false);
        Task<List<TResult>> GetCaptureparamtemplateListAsync<TResult>(Func<IQueryable<DbpCaptureparamtemplate>, IQueryable<TResult>> query, bool notrack = false);

        Task<int> UpdateCaptureParamTemplateAsync(int nParamTemplateID, string strTemplateName, string strUserCaptureParam);

        Task DeleteCaptureParamTemplateAsync(DbpCaptureparamtemplate userTemplate);

        Task<DbpCaptureparamtemplate> GetUserParamForDB2Async(string strUserCode);

        Task UpdateAllUserParamMapAsync(List<DbpUserparamMap> arUserParmMapList);

        #endregion

        #region Usertemplate

        Task<TResult> GetUsertemplateAsync<TResult>(Func<IQueryable<DbpUsertemplate>, IQueryable<TResult>> query, bool notrack = false);

        Task<List<TResult>> GetUsertemplateLstAsync<TResult>(Func<IQueryable<DbpUsertemplate>, IQueryable<TResult>> query, bool notrack = false);

        Task InsertUserTemplateAsync(int templateID, string userCode, string templateName, string templateContent);
        //Task UpdateUserTempalteAsync(int templateID,string userCode, string templateContent, string newTemplateName);
        Task UpdateDbpUserTempalteAsync(DbpUsertemplate usertemplate, string templateContent, string newTemplateName);

        Task<TResult> GetUserParamMapAsync<TResult>(Func<IQueryable<DbpUserparamMap>, IQueryable<TResult>> query, bool notrack = false);
        Task<List<TResult>> GetUserParamMapListAsync<TResult>(Func<IQueryable<DbpUserparamMap>, IQueryable<TResult>> query, bool notrack = false);

        Task DeleteUserTemplateAsync(int nTemplateID);

        Task DeleteUserParamMapAsync(DbpUserparamMap userparamMap);

        Task UpdateUserParamMapAsync(string strUserCode, int nParamTemplateID);

        int GetNextValId(string value);
        #endregion

    }
}
