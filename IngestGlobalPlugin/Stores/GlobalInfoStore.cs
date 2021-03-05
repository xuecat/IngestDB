using IngestDBCore;
using IngestGlobalPlugin.Dto;
using IngestGlobalPlugin.Dto.OldResponse;
using IngestGlobalPlugin.Models;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using Sobey.Core.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IngestGlobalPlugin.Stores
{
    public class GlobalInfoStore : IGlobalStore
    {
        private readonly ILogger Logger = LoggerManager.GetLogger("GlobalInfo");
        protected IngestGlobalDBContext Context { get; }

        public GlobalInfoStore(IngestGlobalDBContext baseDataDbContext)
        {
            Context = baseDataDbContext;
        }

        protected Task<List<TResult>> QueryListAsync<TDbp, TResult>(DbSet<TDbp> contextSet, Func<IQueryable<TDbp>, IQueryable<TResult>> query, bool notrack = false)
            where TDbp : class
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            if (notrack)
            {
                return query(contextSet.AsNoTracking()).ToListAsync();
            }
            return query(contextSet).ToListAsync();
        }


        #region Objectstateinfo
        //get objinfo
        public Task<TResult> GetObjectstateinfoAsync<TResult>(Func<IQueryable<DbpObjectstateinfo>, IQueryable<TResult>> query, bool notrack = false)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            if (notrack)
            {
                return query.Invoke(Context.DbpObjectstateinfo.AsNoTracking()).FirstOrDefaultAsync();
            }
            return query.Invoke(Context.DbpObjectstateinfo).FirstOrDefaultAsync();
        }

        //get objinfo list
        public async Task<List<TResult>> GetObjectstateinfoListAsync<TResult>(Func<IQueryable<DbpObjectstateinfo>, IQueryable<TResult>> query, bool notrack = false)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            if (notrack)
            {
                return await query.Invoke(Context.DbpObjectstateinfo.AsNoTracking()).ToListAsync();
            }
            return await query.Invoke(Context.DbpObjectstateinfo).ToListAsync();
        }

        //add objectstate
        public async Task<bool> AddDbpObjStateAsync(int objectID, OTID objectTypeID, string userName, int TimeOut)
        {
            bool result = false;
            try
            {
                Context.DbpObjectstateinfo.Add(new DbpObjectstateinfo()
                {
                    Objectid = objectID,
                    Objecttypeid = (int)objectTypeID,
                    Username = userName,
                    Begintime = DateTime.Now,
                    Timeout = TimeOut
                });

                int saveResult = await Context.SaveChangesAsync();
                result = saveResult != 0 ? true : false;
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
                result = false;
                throw ex;
            }
            return result;
        }

        public async Task<DbpObjectstateinfo> LockRowsByConditionAsync(int objectID, OTID objectTypeID, string userName, int TimeOut = 500)
        {
            DbpObjectstateinfo objectstateinfo = null;
            try
            {
                //objectstateinfo = await Context.DbpObjectstateinfo.SingleOrDefaultAsync(x => ((objectID >= 0 && x.Objectid == objectID) || objectID < 0) && (((int)objectTypeID >= 0 && x.Objecttypeid == (int)objectTypeID) || (int)objectTypeID < 0) && ((!string.IsNullOrEmpty(userName) && x.Username == userName) || string.IsNullOrEmpty(userName)) && (x.Locklock == "" || x.Locklock == null || x.Begintime < DateTime.Now.AddMilliseconds(TimeOut * (-1))));

                DateTime time = DateTime.Now.AddMilliseconds(TimeOut * (-1));
                IQueryable<DbpObjectstateinfo> query = Context.DbpObjectstateinfo.Where(x => string.IsNullOrEmpty(x.Locklock) || x.Begintime < time);
                if (objectID > 0)
                {
                    query = query.Where(x => x.Objectid == objectID);
                }
                if ((int)objectTypeID >= 0)
                {
                    query = query.Where(x => x.Objecttypeid == (int)objectTypeID);
                }
                if (!string.IsNullOrEmpty(userName))
                {
                    query = query.Where(x => x.Username == userName);
                }

                objectstateinfo = await query.FirstOrDefaultAsync();

                if (objectstateinfo != null)
                {
                    objectstateinfo.Locklock = Guid.NewGuid().ToString("N");
                    await Context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                SobeyRecException.ThrowSelfNoParam(objectID.ToString(), GlobalDictionary.GLOBALDICT_CODE_EXECUTE_COMMAND_ERROR, Logger, ex);
            }
            return objectstateinfo;
        }

        //get and update objinfo
        public async Task<DbpObjectstateinfo> UpdateObjectInfoLockAsync(PostLockObject_param_in param_In)
        {
            DbpObjectstateinfo objectstateinfo = null;
            try
            {
                //objectstateinfo = await GetObjectstateinfoAsync(a => a.Where(x => ((param_In.ObjectID >= 0 && x.Objectid == param_In.ObjectID) || param_In.ObjectID < 0) && (((int)param_In.ObjectTypeID >= 0 && x.Objecttypeid == (int)param_In.ObjectTypeID) || (int)param_In.ObjectTypeID < 0) && ((!string.IsNullOrEmpty(param_In.userName) && x.Username == param_In.userName) || string.IsNullOrEmpty(param_In.userName)) && (string.IsNullOrEmpty(x.Locklock) || x.Begintime < DateTime.Now.AddMilliseconds(param_In.TimeOut * (-1)))), true);
                DateTime time = DateTime.Now.AddMilliseconds(param_In.TimeOut * (-1));
                IQueryable <DbpObjectstateinfo> query = Context.DbpObjectstateinfo.Where(x=>string.IsNullOrEmpty(x.Locklock) || x.Begintime < time);
                if (param_In.ObjectID > 0)
                {
                    query = query.Where(x => x.Objectid == param_In.ObjectID);
                }
                if(param_In.ObjectTypeID >= 0)
                {
                    query = query.Where(x => x.Objecttypeid == (int)param_In.ObjectTypeID);
                }
               

                objectstateinfo = await query.FirstOrDefaultAsync();

                if (objectstateinfo != null)
                {
                    objectstateinfo.Locklock = Guid.NewGuid().ToString("N");
                    await Context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                SobeyRecException.ThrowSelfNoParam(param_In.ObjectID.ToString(), GlobalDictionary.GLOBALDICT_CODE_EXECUTE_COMMAND_ERROR, Logger, ex);
            }
            return objectstateinfo;
        }

        public async Task<bool> UnLockRowsAsync(DbpObjectstateinfo objectstateinfo, int TimeOut)
        {
            bool result = false;
            try
            {
                int nTime = -1;
                DateTime dtLock = new DateTime();

                try
                {
                    System.Globalization.DateTimeFormatInfo dtfi = new System.Globalization.CultureInfo("en-US", false).DateTimeFormat;
                    dtfi.ShortTimePattern = "t";
                    //DateTime dt = DateTime.Parse("02-28-12 03:07PM", dtfi);
                    nTime = Convert.ToInt32(objectstateinfo.Timeout);

                    dtLock = DateTime.Parse(objectstateinfo.Begintime.ToString(), dtfi);
                }
                catch (System.Exception ex)
                {
                    //Logger.Error(ex.ToString());
                    Logger.Error("UnLockRowsAsync 时间格式转换不对: " + ex.Message);
                    return false;
                }

                DateTime AddMillSec = dtLock.AddMilliseconds(nTime);
                DateTime dtNow = DateTime.Now;
                var selectResult = await GetObjectstateinfoAsync(a => a.Where(x => x.Objectid == objectstateinfo.Objectid && x.Objecttypeid == objectstateinfo.Objecttypeid ));

                if (AddMillSec < dtNow) //锁超时
                {
                    result = true;
                    if (selectResult != null)
                    {
                        selectResult.Begintime = dtNow;
                        selectResult.Timeout = Convert.ToInt32(TimeOut);
                    }
                }
                if (selectResult != null)
                {
                    selectResult.Locklock = null;
                    await Context.SaveChangesAsync();
                }
                return result;
            }
            catch (Exception ex)
            {
                SobeyRecException.ThrowSelfNoParam(objectstateinfo.Objectid.ToString(), GlobalDictionary.GLOBALDICT_CODE_EXECUTE_COMMAND_ERROR, Logger, ex);
                result = false;
                throw ex;
            }
        }

        //remove or update lock
        public async Task<bool> UnLockObjectAsync(DbpObjectstateinfo arrObjects)
        {
            try
            {
                if (arrObjects == null)
                {
                    Logger.Error("arrObjects == null || arrObjects.Count <= 0 成立，解锁函数return false，解锁失败");
                    return false;
                }
                //加锁
                //var deleteObj = await GetObjectstateinfoAsync(a => a.Where(x => x.Username == arrObjects.Username && ((arrObjects.Objectid >= 0 && x.Objectid == arrObjects.Objectid) || arrObjects.Objectid < 0) && ((arrObjects.Objecttypeid >= 0 && x.Objecttypeid == (int)arrObjects.Objecttypeid) || arrObjects.Objecttypeid < 0)));

                Context.DbpObjectstateinfo.Remove(arrObjects);
                int LineNum = await Context.SaveChangesAsync();

                bool ret = false;
                if (LineNum > 0)
                {
                    ret = true;//删除了锁，才返回正确
                }
                else
                {
                    Logger.Info("UnLockObjectAsync UnLockObject return false，解锁失败");
                    ret = false;

                    arrObjects.Locklock = null;
                    await Context.SaveChangesAsync();
                }

                return ret;
            }
            
            catch (System.Exception ex)
            {
                Logger.Error(ex.ToString());
                string message = "UnLockObject";
                SobeyRecException.ThrowSelfNoParam(message, GlobalDictionary.GLOBALDICT_CODE_IN_SETUNLOCKOBJECT_EXCEPTION, Logger, ex);
                return false;
            }
        }

        #endregion

        #region global method
        //excute global
        public Task<TResult> GetGlobalAsync<TResult>(Func<IQueryable<DbpGlobal>, IQueryable<TResult>> query, bool notrack = false)
        {

            //var result = Context.DbpUsertemplate.FromSql("select next_val('DBP_SQ_UESRTEMPLATEID')");// .ExecuteSqlCommand("select next_val('DBP_SQ_UESRTEMPLATEID')");

            //var result = Context.Database.ExecuteSqlCommand("select next_val('DBP_SQ_UESRTEMPLATEID')");//.FirstOrDefault();

            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            if (notrack)
            {
                return query.Invoke(Context.DbpGlobal.AsNoTracking()).FirstOrDefaultAsync();
            }
            return query.Invoke(Context.DbpGlobal).FirstOrDefaultAsync();
        }

        public async Task<string> GetGlobalValueStringAsync(string strKey)
        {
            string strValue = string.Empty;

            try
            {
                var dbpGlobal = await Context.DbpGlobal.AsNoTracking().SingleOrDefaultAsync(x => x.GlobalKey == strKey);
                if (dbpGlobal != null)
                {
                    strValue = dbpGlobal.GlobalValue;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return strValue;
        }

        //add or update
        public async Task<bool> UpdateGlobalValueAsync(string strKey, string strValue)
        {

            try
            {
                var state = new DbpGlobal()
                {
                    GlobalKey = strKey,
                    GlobalValue = strValue,
                    Changetime = DateTime.Now,
                };
                if (!Context.DbpGlobal.AsNoTracking().Any(a => a.GlobalKey == strKey))
                {
                    //add
                    Context.DbpGlobal.Add(state);
                }
                else
                {
                    //update
                    Context.Attach(state);
                    Context.Entry(state).Property(x => x.GlobalValue).IsModified = true;
                }
                return await Context.SaveChangesAsync()>0;
            }
            catch (System.Exception ex)
            {
                Logger.Error("UpdateGlobalValueAsync : " + ex.ToString());
                throw ex;
            }

        }
        #endregion

        #region Globalstate Method
        //excute global state
        public async Task<TResult> GetGlobalStateAsync<TResult>(Func<IQueryable<DbpGlobalState>, IQueryable<TResult>> query, bool notrack = false)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            if (notrack)
            {
                return await query.Invoke(Context.DbpGlobalState.AsNoTracking()).FirstOrDefaultAsync();
            }
            return await query.Invoke(Context.DbpGlobalState).FirstOrDefaultAsync();
        }

        public Task<List<TResult>> GetGlobalStateListAsync<TResult>(Func<IQueryable<DbpGlobalState>, IQueryable<TResult>> query, bool notrack = false)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            if (notrack)
            {
                return query.Invoke(Context.DbpGlobalState.AsNoTracking()).ToListAsync();
            }
            return query.Invoke(Context.DbpGlobalState).ToListAsync();
        }

        public async Task<List<DbpGlobalState>> GetAllGlobalStateAsync()
        {
            try
            {
                return await Context.DbpGlobalState.AsNoTracking().ToListAsync();
            }
            catch (Exception ex)
            {
                Logger.Error("GetAllGlobalState : " + ex.ToString());
                throw ex;
            }
        }

        //add or update
        public async Task UpdateGlobalStateAsync(string strLabel)
        {
            var dbpState = await Context.DbpGlobalState.SingleOrDefaultAsync(x => x.Label == strLabel);
            if (dbpState != null)
            {
                dbpState.Lasttime = DateTime.Now;
            }
            else
            {
                var state = new DbpGlobalState()
                {
                    Label = strLabel,
                    Lasttime = DateTime.Now  //.ToString("yyyy-MM-dd HH:mm:ss")
                };
                Context.DbpGlobalState.Add(state);
            }

            try
            {
                await Context.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {
                throw e;
            }
        }
        #endregion

        #region lock and unlock
        public async Task<bool> SetLockObject(int objectID, OTID objectTypeID, string userName, int TimeOut)
        {
            if (TimeOut < 0)
                TimeOut *= -1;

            if (!Context.DbpObjectstateinfo.AsNoTracking().Any(a => a.Objectid == objectID && a.Objecttypeid == (int)objectTypeID))
            {
                //add
                Context.DbpObjectstateinfo.Add(new DbpObjectstateinfo()
                {
                    Objectid = objectID,
                    Objecttypeid = (int)objectTypeID,
                    Username = userName,
                    Begintime = DateTime.Now,
                    Timeout = TimeOut
                });

                try
                {
                    int saveResult = await Context.SaveChangesAsync();
                    if (saveResult != 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch (System.Exception ex)
                {
                    //LoggerService.Error(ex.ToString());
                    Logger.Error(ex.ToString());
                    // 直接返回失败
                    return false;
                }
            }


            var arrObjects = LockSelect(3, 500, objectID, objectTypeID, userName);//LOCKLOCK失效的时间为500毫秒

            if (arrObjects == null || arrObjects.Count <= 0)
                return false;
            //判断锁是否超时
            var arrObject = arrObjects[0];


            int nTime = -1;
            DateTime dtLock = new DateTime();


            try
            {
                System.Globalization.DateTimeFormatInfo dtfi = new System.Globalization.CultureInfo("en-US", false).DateTimeFormat;
                dtfi.ShortTimePattern = "t";
                //DateTime dt = DateTime.Parse("02-28-12 03:07PM", dtfi);
                nTime = Convert.ToInt32(arrObject.Timeout);

                dtLock = DateTime.Parse(arrObject.Begintime.ToString(), dtfi);
            }
            catch (System.Exception ex)
            {
                Logger.Error(ex.ToString());
                Logger.Error("时间格式转换不对: " + ex.Message);
                return false;

            }



            bool ret = false;
            DateTime AddMillSec = dtLock.AddMilliseconds(nTime);

            DateTime dtNow = DateTime.Now;

            if (AddMillSec < dtNow) //锁超时
            {
                ret = true;
                UpdateLock(userName, dtNow, Convert.ToInt32(TimeOut), objectID, objectTypeID);
            }
            UnLock(objectID, objectTypeID, userName);
            return ret;
        }

        public void UnLock(int objectID, OTID objectTypeID, string userName)
        {
            var selectResult = Context.DbpObjectstateinfo.FirstOrDefault(x => x.Objectid == objectID && x.Objecttypeid == (int)objectTypeID && x.Username == userName);

            //updateCmd.CommandText = "UPDATE DBP_OBJECTSTATEINFO SET LOCKLOCK=NULL" + SelectStatement();
            try
            {
                if (selectResult != null)
                {
                    selectResult.Locklock = null;
                    Context.SaveChangesAsync();
                }

            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
            }
        }

        public async void UpdateLock(string strUser, DateTime dtBeginTime, int nTime, int objectID, OTID objectTypeID)
        {
            var selectResult = Context.DbpObjectstateinfo.FirstOrDefault(x => x.Objectid == objectID && x.Objecttypeid == (int)objectTypeID && x.Username == strUser);

            try
            {
                if (selectResult != null)
                {
                    selectResult.Username = strUser;
                    selectResult.Begintime = dtBeginTime;
                    selectResult.Timeout = nTime;
                    await Context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                Logger.Error("UpdateLock发生异常:" + ex.Message);
                string message = strUser;
                SobeyRecException.ThrowSelfNoParam(message, GlobalDictionary.GLOBALDICT_CODE_EXECUTE_COMMAND_ERROR, Logger, ex);
            }
        }

        public List<DbpObjectstateinfo> LockSelect(int nTryCount, int TimeOut, int objectID, OTID objectTypeID, string userName)
        {

            //ArrayList retArray = new ArrayList();
            try
            {
                //尝试加锁三次，若都不成功，则返回
                string strLock = string.Empty;
                int nSelect = 0;
                int nTryed = 0;
                while (nSelect < 1 && nTryed < nTryCount)
                {
                    if (nTryed > 0)
                    {
                        System.Threading.Thread.Sleep(100);
                        //ApplicationLog.WriteInfo("SelectObjectStateInfoFactory.cs //LockSelect()//nTryed = "+nTryed.ToString()+",nSelect = "+nSelect.ToString());
                    }

                    nSelect = LockRows(ref strLock, TimeOut, objectID, objectTypeID, userName);

                    nTryed++;
                }
                if (nSelect < 1)
                    return null;

                var retLst = Context.DbpObjectstateinfo.AsNoTracking().Where(x => x.Objectid == objectID && x.Objecttypeid == (int)objectTypeID && x.Username == userName && x.Locklock == strLock);

                return retLst.ToList();
            }
            catch (Exception ex)
            {
                Logger.Error("LockSelect 发生异常:" + ex.Message);
                string message = "LockSelect";
                SobeyRecException.ThrowSelfNoParam(message, GlobalDictionary.GLOBALDICT_CODE_SELECT_COMMAND_FAILD, Logger, ex);
            }
            return null;
        }

        protected int LockRows(ref string strLock, int TimeOut, int objectID, OTID objectTypeID, string userName)
        {

            int nEffectRow = 0;
            string strTempLock = Guid.NewGuid().ToString("N");

            DateTime time = DateTime.Now.AddMilliseconds(TimeOut * (-1));
            var selectResult = Context.DbpObjectstateinfo.FirstOrDefault(x => x.Objectid == objectID && x.Objecttypeid == (int)objectTypeID && x.Username == userName && (x.Locklock == "" || x.Locklock == null || x.Begintime < time));

            try
            {
                if (selectResult != null)
                {
                    selectResult.Locklock = strTempLock;

                    nEffectRow = Context.SaveChangesAsync().Result;
                }
            }
            catch (Exception ex)
            {
                string message = strTempLock;
                SobeyRecException.ThrowSelfNoParam(strTempLock, GlobalDictionary.GLOBALDICT_CODE_EXECUTE_COMMAND_ERROR, Logger, ex);
            }
            strLock = strTempLock;
            return nEffectRow;
        }

        public async Task<bool> SetUnLockObject(int objectID, OTID objectTypeID, string userName)
        {
            try
            {
                var arrObjects = LockSelect(3, 500, objectID, objectTypeID, userName);

                if (arrObjects == null || arrObjects.Count <= 0)
                {
                    Logger.Error("arrObjects == null || arrObjects.Count <= 0 成立，解锁函数return false，解锁失败");
                    return false;
                }
                //加锁
                var deleteObj = await Context.DbpObjectstateinfo.FirstOrDefaultAsync(x => x.Username == userName && ((objectID >= 0 && x.Objectid == objectID) || objectID < 0) && ((objectTypeID >= 0 && x.Objecttypeid == (int)objectTypeID) || objectTypeID < 0));

                Context.DbpObjectstateinfo.Remove(deleteObj);
                int LineNum = await Context.SaveChangesAsync();

                bool ret = false;
                if (LineNum > 0)
                {

                    ret = true;//删除了锁，才返回正确
                }
                else
                {
                    Logger.Info("UnLockObject return false，解锁失败");
                    ret = false;
                }
                //解锁
                UnLock(objectID, objectTypeID, userName);
                return ret;

            }
            
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
                string message = "UnLockObject";
                SobeyRecException.ThrowSelfNoParam(message, GlobalDictionary.GLOBALDICT_CODE_IN_SETUNLOCKOBJECT_EXCEPTION, Logger, ex);
                return false;
            }
        }


        #endregion

        #region User
        public Task<TResult> GetUserSettingAsync<TResult>(Func<IQueryable<DbpUsersettings>, IQueryable<TResult>> query, bool notrack = false)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            if (notrack)
            {
                return query.Invoke(Context.DbpUsersetting.AsNoTracking()).FirstOrDefaultAsync();
            }
            return query.Invoke(Context.DbpUsersetting).FirstOrDefaultAsync();
        }

        //add or update
        public async Task UpdateUsersettingsAsync(DbpUsersettings usersetting)
        {
            try
            {
                if (!Context.DbpUsersetting.AsNoTracking().Any(x => x.Usercode == usersetting.Usercode && x.Settingtype == usersetting.Settingtype))
                {
                    //add
                    Context.DbpUsersetting.Add(usersetting);
                }
                else
                {
                    //update
                    Context.Attach(usersetting);
                    Context.Entry(usersetting).Property(x => x.Settingtext).IsModified = true;
                    Context.Entry(usersetting).Property(x => x.Settingtextlong).IsModified = true;
                }

                await Context.SaveChangesAsync();

            }
            catch (System.Exception ex)
            {
                Logger.Error(ex.ToString());
                //SobeyRecException.ThrowSelf(Locallanguage.LoadString("Fill SetUserSetting Exception "),ex,2,10013019);	
                SobeyRecException.ThrowSelfNoParam("UpdateUsersettingsAsync", GlobalDictionary.GLOBALDICT_CODE_FILL_USER_SETUSERSETTING_EXCEPTION, Logger, ex);
            }
        }

        public async Task AddUserLoginInfoAsync(DbpUserLoginInfo logininfo)
        {
            try
            {
                if (logininfo != null)
                {
                    if (await Context.DbpUserLoginInfo.AsNoTracking().AnyAsync(x => x.Ip == logininfo.Ip && x.Usercode == logininfo.Usercode))
                    {
                        Logger.Info($"AddUserLoginInfoAsync same {logininfo.Ip} {logininfo.Usercode}");
                    }
                    else
                    {

                        Context.DbpUserLoginInfo.Add(logininfo);
                        await Context.SaveChangesAsync();
                    }
                }
            }
            catch (Exception d)
            {

                throw d;
            }
        }

        public async Task<bool> DeleteUserLoginInfoByIPAsync(string ip)
        {
            var have = await Context.DbpUserLoginInfo.AsNoTracking().Where(a => a.Ip == ip).SingleOrDefaultAsync();
            if (have != null)
            {
                Context.DbpUserLoginInfo.Remove(have);
                await Context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<bool> DeleteUserLoginInfoByUserCodeAsync(string usercode)
        {
            var have = await Context.DbpUserLoginInfo.AsNoTracking().Where(a => a.Usercode == usercode).SingleOrDefaultAsync();
            if (have != null)
            {
                Context.DbpUserLoginInfo.Remove(have);
                await Context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<List<DbpUserLoginInfo>> GetAllUserLoginInfoAsync()
        {
            return await Context.DbpUserLoginInfo.AsNoTracking().ToListAsync();
        }

        #endregion

        #region CaptureTemplate


        public Task<TResult> GetCaptureparamtemplateAsync<TResult>(Func<IQueryable<DbpCaptureparamtemplate>, IQueryable<TResult>> query, bool notrack = false)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            if (notrack)
            {
                return query.Invoke(Context.DbpCaptureparamtemplate.AsNoTracking()).SingleOrDefaultAsync();
            }
            return query.Invoke(Context.DbpCaptureparamtemplate).SingleOrDefaultAsync();
        }

        public async Task<List<TResult>> GetCaptureparamtemplateListAsync<TResult>(Func<IQueryable<DbpCaptureparamtemplate>, IQueryable<TResult>> query, bool notrack = false)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            if (notrack)
            {
                return await query.Invoke(Context.DbpCaptureparamtemplate.AsNoTracking()).ToListAsync();
            }
            return await query.Invoke(Context.DbpCaptureparamtemplate).ToListAsync();
        }

        public async Task<int> UpdateCaptureParamTemplateAsync(int nParamTemplateID, string strTemplateName, string strUserCaptureParam)
        {
            try
            {

                var dbpCapParam = await GetCaptureparamtemplateAsync(a => a.Where(x => x.Captureparamid == nParamTemplateID));
                if (dbpCapParam == null)
                {
                    //add
                    nParamTemplateID = GetNextValId("DBP_SQ_PARAMTEMPLATE");// IngestGlobalDBContext.next_val("DBP_SQ_PARAMTEMPLATE");
                    Context.DbpCaptureparamtemplate.Add(new DbpCaptureparamtemplate()
                    {
                        Captureparamid = nParamTemplateID,
                        Captureparam = strUserCaptureParam,
                        Captemplatename = strTemplateName
                    });
                }
                else
                {
                    //update
                    //var dbpCapTemplate = new DbpCaptureparamtemplate()
                    //{
                    //    Captureparamid = nParamTemplateID,
                    //    Captureparam = strUserCaptureParam,
                    //    Captemplatename = strTemplateName
                    //};
                    //Context.Attach(dbpCapTemplate);
                    //Context.Entry(dbpCapTemplate).Property(x => x.Captemplatename).IsModified = true;
                    //Context.Entry(dbpCapTemplate).Property(x => x.Captureparam).IsModified = true;
                    dbpCapParam.Captureparamid = nParamTemplateID;
                    dbpCapParam.Captureparam = strUserCaptureParam;
                    dbpCapParam.Captemplatename = strTemplateName;
                }
                await Context.SaveChangesAsync();
            }
            catch (System.Exception ex)
            {
                Logger.Error("UpdateGlobalValueAsync : " + ex.ToString());
                throw ex;
            }
            return nParamTemplateID;
        }

        public async Task DeleteCaptureParamTemplateAsync(DbpCaptureparamtemplate capTemplate)
        {
            try
            {
                if (capTemplate == null)
                {
                    return;
                }

                Context.DbpCaptureparamtemplate.Remove(capTemplate);
                await Context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Logger.Error("DeleteCaptureParamTemplateAsync : " + ex.ToString());
                throw ex;
            }
        }

        public async Task<DbpCaptureparamtemplate> GetUserParamForDB2Async(string strUserCode)
        {
            try
            {
                 var dbpCapture = await Context.DbpUserparamMap.Where(x=>x.Usercode == strUserCode).Join(Context.DbpCaptureparamtemplate, x => x.Captureparamid, y => y.Captureparamid, (a, b) => new DbpCaptureparamtemplate {  Captureparamid = b.Captureparamid, Captureparam = b.Captureparam, Captemplatename = b.Captemplatename }).FirstOrDefaultAsync();

                return dbpCapture;
            }
            catch (Exception ex)
            {
                Logger.Error("GetUserParamForDB2Async : " + ex.ToString());
                throw ex;
            }
        }


        public async Task UpdateAllUserParamMapAsync(List<DbpUserparamMap> arUserParmMapList)
        {
            try
            {
                var dbuserMaps = await GetUserParamMapListAsync(a => a);
                Context.DbpUserparamMap.RemoveRange(dbuserMaps);

                Context.DbpUserparamMap.AddRange(arUserParmMapList);
                await Context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Logger.Error("UpdateAllUserParamMapAsync : " + ex.ToString());
                throw ex;
            }
            
        }

        #endregion

        #region UserTemplate

        public Task<TResult> GetUsertemplateAsync<TResult>(Func<IQueryable<DbpUsertemplate>, IQueryable<TResult>> query, bool notrack = false)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            if (notrack)
            {
                return query.Invoke(Context.DbpUsertemplate.AsNoTracking()).FirstOrDefaultAsync();
            }
            return query.Invoke(Context.DbpUsertemplate).FirstOrDefaultAsync();
        }

        public Task<List<TResult>> GetUsertemplateLstAsync<TResult>(Func<IQueryable<DbpUsertemplate>, IQueryable<TResult>> query, bool notrack = false)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            if (notrack)
            {
                return query.Invoke(Context.DbpUsertemplate.AsNoTracking()).ToListAsync();
            }
            return query.Invoke(Context.DbpUsertemplate).ToListAsync();
        }

        public async Task InsertUserTemplateAsync(int templateID, string userCode, string templateName, string templateContent)
        {
            try
            {
                Context.DbpUsertemplate.Add(new DbpUsertemplate()
                {
                    Templateid = templateID,
                    Usercode = userCode,
                    Templatename = templateName,
                    Templatecontent = templateContent
                });
                await Context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Logger.Error("InsertUserTemplateAsync : " + ex.ToString());
                throw ex;
            }

        }

        //public async Task UpdateUserTempalteAsync(int templateID,string userCode, string templateContent, string newTemplateName)
        //{
        //    try
        //    {
        //        var userTemplate = new DbpUsertemplate()
        //        {
        //            Templateid = templateID,
        //            Usercode = userCode
        //        };

        //        if (newTemplateName != null && templateContent!=null)
        //        {
        //            userTemplate.Templatename = newTemplateName;
        //            userTemplate.Templatecontent = templateContent;
        //            Context.Attach(userTemplate);
        //            Context.Entry(userTemplate).Property(x => x.Templatename).IsModified = true;
        //            Context.Entry(userTemplate).Property(x => x.Templatecontent).IsModified = true;
        //            await Context.SaveChangesAsync();
        //        }
        //        else if (newTemplateName != null && templateContent== null)
        //        {
        //            userTemplate.Templatecontent = templateContent;
        //            Context.Attach(userTemplate);
        //            Context.Entry(userTemplate).Property(x => x.Templatecontent).IsModified = true;
        //            await Context.SaveChangesAsync();
        //        }
        //        else if (templateContent != null && newTemplateName == null)
        //        {
        //            userTemplate.Templatename = newTemplateName;
        //            Context.Attach(userTemplate);
        //            Context.Entry(userTemplate).Property(x => x.Templatename).IsModified = true;
        //            await Context.SaveChangesAsync();
        //        }
                
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.Error("ModifyUserTempalteContent : " + ex.ToString());
        //        throw ex;
        //    }
        //}

        public async Task UpdateDbpUserTempalteAsync(DbpUsertemplate usertemplate, string templateContent, string newTemplateName)
        {
            try
            {
                if (usertemplate != null)
                {
                    if (!string.IsNullOrWhiteSpace(templateContent))
                    {
                        usertemplate.Templatecontent = templateContent;
                    }
                    if (!string.IsNullOrWhiteSpace(newTemplateName))
                    {
                        usertemplate.Templatename = newTemplateName;
                    }

                    await Context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                Logger.Error("ModifyUserTempalteContent : " + ex.ToString());
                throw ex;
            }
        }

        public Task<TResult> GetUserParamMapAsync<TResult>(Func<IQueryable<DbpUserparamMap>, IQueryable<TResult>> query, bool notrack = false)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            if (notrack)
            {
                return query.Invoke(Context.DbpUserparamMap.AsNoTracking()).FirstOrDefaultAsync();
            }
            return query.Invoke(Context.DbpUserparamMap).FirstOrDefaultAsync();
        }

        public async Task<List<TResult>> GetUserParamMapListAsync<TResult>(Func<IQueryable<DbpUserparamMap>, IQueryable<TResult>> query, bool notrack = false)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            if (notrack)
            {
                return await query.Invoke(Context.DbpUserparamMap.AsNoTracking()).ToListAsync();
            }
            return await query.Invoke(Context.DbpUserparamMap).ToListAsync();
        }

        public async Task DeleteUserParamMapAsync(DbpUserparamMap userparamMap)
        {
            try
            {
                if (userparamMap == null)
                {
                    return;
                }

                Context.DbpUserparamMap.Remove(userparamMap);
                await Context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Logger.Error("DeleteUserParamMapAsync : " + ex.ToString());
                throw ex;
            }
        }

        public async Task DeleteUserTemplateAsync(int TemplateID)
        {
            try
            {
                var dbpUserTemplate = await GetUsertemplateAsync(a => a.Where(x => x.Templateid == TemplateID));

                if (dbpUserTemplate != null)
                {
                    Context.DbpUsertemplate.Remove(dbpUserTemplate);
                    await Context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                Logger.Error("DeleteUserTemplateAsync : " + ex.ToString());
                throw ex;
            }

        }

        public async Task UpdateUserParamMapAsync(string strUserCode, int nParamTemplateID)
        {
            try
            {
                var dbpUserParamMap = new DbpUserparamMap()
                {
                    Usercode = strUserCode,
                    Captureparamid = nParamTemplateID
                };

                if (!Context.DbpUserparamMap.AsNoTracking().Any(a => a.Usercode == strUserCode))
                {
                    //add
                    Context.DbpUserparamMap.Add(dbpUserParamMap);
                }
                else
                {
                    //update
                    Context.Attach(dbpUserParamMap);
                    Context.Entry(dbpUserParamMap).Property(x => x.Captureparamid).IsModified = true;
                }
                await Context.SaveChangesAsync();
            }
            catch (System.Exception ex)
            {
                Logger.Error("UpdateUserParamMapAsync : " + ex.ToString());
                throw ex;
            }

        }
        

        public int GetNextValId(string value)
        {
            return Context.DbpGlobal.Select(x => IngestGlobalDBContext.next_val(value)).FirstOrDefault();
        }


        #endregion

    }
}
