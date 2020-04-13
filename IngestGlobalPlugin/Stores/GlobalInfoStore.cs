using IngestDBCore;
using IngestGlobalPlugin.Dto;
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

        #region Objectstateinfo
        public async Task<TResult> GetObjectstateinfoAsync<TResult>(Func<IQueryable<DbpObjectstateinfo>, IQueryable<TResult>> query, bool notrack = false)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            if (notrack)
            {
                return await query.Invoke(Context.DbpObjectstateinfo.AsNoTracking()).FirstOrDefaultAsync();
            }
            return await query.Invoke(Context.DbpObjectstateinfo).FirstOrDefaultAsync();
        }

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
                objectstateinfo = await Context.DbpObjectstateinfo.SingleOrDefaultAsync(x => ((objectID >= 0 && x.Objectid == objectID) || objectID < 0) && (((int)objectTypeID >= 0 && x.Objecttypeid == (int)objectTypeID) || (int)objectTypeID < 0) && ((!string.IsNullOrEmpty(userName) && x.Username == userName) || string.IsNullOrEmpty(userName)) && (x.Locklock == "" || x.Locklock == null || x.Begintime < DateTime.Now.AddMilliseconds(TimeOut * (-1))));

                if (objectstateinfo != null)
                {
                    objectstateinfo.Locklock = Guid.NewGuid().ToString();
                    await Context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                SobeyRecException.ThrowSelfNoParam(objectID.ToString(), GlobalDictionary.GLOBALDICT_CODE_EXECUTE_COMMAND_ERROR, Logger, ex);
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

                var selectResult = await Context.DbpObjectstateinfo.SingleOrDefaultAsync(x => x.Objectid == objectstateinfo.Objectid && x.Objecttypeid == objectstateinfo.Objecttypeid && x.Username == objectstateinfo.Username);

                if (AddMillSec < dtNow) //锁超时
                {
                    result = true;
                    //UpdateLock(userName, dtNow, Convert.ToInt32(TimeOut), objectID, objectTypeID);
                    if (selectResult != null)
                    {
                        selectResult.Begintime = dtNow;
                        selectResult.Timeout = Convert.ToInt32(TimeOut);
                    }
                }
                //UnLock(objectID, objectTypeID, userName);
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
                //var deleteObj = await Context.DbpObjectstateinfo.FirstOrDefaultAsync(x => x.Username == userName && ((objectID >= 0 && x.Objectid == objectID) || objectID < 0) && ((objectTypeID >= 0 && x.Objecttypeid == (int)objectTypeID) || objectTypeID < 0));

                Context.Remove(arrObjects);
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
                //解锁
                //UnLock(objectID, objectTypeID, userName);
                return ret;
            }
            catch (MySqlException ex)
            {
                Logger.Error(ex.ToString());
                string message = "UnLockObject";
                SobeyRecException.ThrowSelfNoParam(message, GlobalDictionary.GLOBALDICT_CODE_IN_SETUNLOCKOBJECT_READ_DATA_FAILED, Logger, ex);
                return false;
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
            catch(Exception ex)
            {
                throw ex;
            }

            return strValue;
        }

        public async Task UpdateGlobalValueAsync(string strKey, string strValue)
        {
            
            try
            {
                if (!Context.DbpGlobal.AsNoTracking().Any(a => a.GlobalKey == strKey))
                {
                    //add
                    Context.DbpGlobal.Add(new DbpGlobal()
                    {
                        GlobalKey = strKey,
                        GlobalValue = strValue
                    });
                }
                else
                {
                    //update
                    var state = new DbpGlobal()
                    {
                        GlobalKey = strKey,
                        GlobalValue = strValue
                    };
                    Context.Attach(state);
                    Context.Entry(state).Property(x => x.GlobalValue).IsModified = true;
                }
                await Context.SaveChangesAsync();
            }
            catch (System.Exception ex)
            {
                Logger.Error(ex.ToString());
                throw ex;
            }
        }
        #endregion

        #region Globalstate Method
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

        public async Task UpdateGlobalStateAsync(string strLabel)
        {
            if (!Context.DbpGlobalState.AsNoTracking().Any(a => a.Label == strLabel))
            {
                //add
                Context.DbpGlobalState.Add(new DbpGlobalState()
                {
                    Label = strLabel,
                    Lasttime = DateTime.Now  //.ToString("yyyy-MM-dd HH:mm:ss")
                });
            }
            else
            {
                //update
                var state = new DbpGlobalState()
                {
                    Label = strLabel,
                    Lasttime = DateTime.Now  //.ToString("yyyy-MM-dd HH:mm:ss")
                };
                Context.Attach(state);
                Context.Entry(state).Property(x => x.Lasttime).IsModified = true;
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
                    if(saveResult != 0)
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
                if(selectResult != null)
                {
                    selectResult.Locklock = null;
                    Context.SaveChangesAsync();
                }
                
            }
            catch (MySqlException ex)
            {
                Logger.Error("UnLock发生异常:" + ex.Message);
                string message = "UnLock发生异常";
                SobeyRecException.ThrowSelfNoParam(message, GlobalDictionary.GLOBALDICT_CODE_EXECUTE_COMMAND_ERROR,Logger, ex);
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
                SobeyRecException.ThrowSelfNoParam(message,GlobalDictionary.GLOBALDICT_CODE_EXECUTE_COMMAND_ERROR,Logger, ex);
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
            catch (MySqlException ex)
            {
                Logger.Error("LockSelect 发生异常:" + ex.Message);
                string message = "LockSelect";
                SobeyRecException.ThrowSelfNoParam(message,GlobalDictionary.GLOBALDICT_CODE_SELECT_COMMAND_FAILD,Logger, ex);
            }
            return null;
        }

        protected int LockRows(ref string strLock, int TimeOut, int objectID, OTID objectTypeID, string userName)
        {

            int nEffectRow = 0;
            string strTempLock = Guid.NewGuid().ToString();

            var selectResult = Context.DbpObjectstateinfo.FirstOrDefault(x => x.Objectid == objectID && x.Objecttypeid == (int)objectTypeID && x.Username == userName && (x.Locklock == "" || x.Locklock == null || x.Begintime < DateTime.Now.AddMilliseconds(TimeOut * (-1))));

            try
            {
                if (selectResult != null)
                {
                    selectResult.Locklock = strTempLock;

                    nEffectRow = Context.SaveChangesAsync().Result;
                }
            }
            catch(Exception ex)
            {
                string message = strTempLock;
                SobeyRecException.ThrowSelfNoParam(strTempLock, GlobalDictionary.GLOBALDICT_CODE_EXECUTE_COMMAND_ERROR,Logger, ex);
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

                Context.Remove(deleteObj);
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
            catch (MySqlException ex)
            {
                Logger.Error(ex.ToString());
                string message = "UnLockObject";
                SobeyRecException.ThrowSelfNoParam(message, GlobalDictionary.GLOBALDICT_CODE_IN_SETUNLOCKOBJECT_READ_DATA_FAILED, Logger, ex);
                return false;
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

        #region User
        public async Task<TResult> GetUserSettingAsync<TResult>(Func<IQueryable<DbpUsersettings>, IQueryable<TResult>> query, bool notrack = false)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            if (notrack)
            {
                return await query.Invoke(Context.DbpUsersetting.AsNoTracking()).FirstOrDefaultAsync();
            }
            return await query.Invoke(Context.DbpUsersetting).FirstOrDefaultAsync();
        }


        public async Task UpdateUsersettingsAsync(DbpUsersettings usersetting)
        {
            try
            {
                if (!Context.DbpUsersetting.AsNoTracking().Any(x=> x.Usercode == usersetting.Usercode && x.Settingtype == usersetting.Settingtype))
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
        
        #endregion

        #region CaptureTemplate

        
        public async Task<TResult> GetCaptureparamtemplateAsync<TResult>(Func<IQueryable<DbpCaptureparamtemplate>, IQueryable<TResult>> query, bool notrack = false)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            if (notrack)
            {
                return await query.Invoke(Context.DbpCaptureparamtemplate.AsNoTracking()).FirstOrDefaultAsync();
            }
            return await query.Invoke(Context.DbpCaptureparamtemplate).FirstOrDefaultAsync();
        }

        #endregion

        #region UserTemplate

        public async Task<TResult> GetUsertemplateAsync<TResult>(Func<IQueryable<DbpUsertemplate>, IQueryable<TResult>> query, bool notrack = false)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            if (notrack)
            {
                return await query.Invoke(Context.DbpUsertemplate.AsNoTracking()).FirstOrDefaultAsync();
            }
            return await query.Invoke(Context.DbpUsertemplate).FirstOrDefaultAsync();
        }

        public async Task<List<TResult>> GetUsertemplateLstAsync<TResult>(Func<IQueryable<DbpUsertemplate>, IQueryable<TResult>> query, bool notrack = false)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            if (notrack)
            {
                return await query.Invoke(Context.DbpUsertemplate.AsNoTracking()).ToListAsync();
            }
            return await query.Invoke(Context.DbpUsertemplate).ToListAsync();
        }

        public async Task InsertUserTemplateAsync(int templateID, string userCode, string templateName, string templateContent)
        {
            try
            {
                Context.DbpUsertemplate.Add(new DbpUsertemplate() {
                    Templateid = templateID,
                    Usercode = userCode,
                    Templatename = templateName,
                    Templatecontent = templateContent
                });
                await Context.SaveChangesAsync();
            }
            catch(Exception ex)
            {
                Logger.Error("InsertUserTemplateAsync : " + ex.ToString());
                throw ex;
            }

        }

        public async Task ModifyUserTempalteContent(int templateID, string templateContent)
        {
            try
            {
                var userTemplate = new DbpUsertemplate()
                {
                    Templateid = templateID,
                    Templatecontent = templateContent
                };
                Context.Attach(userTemplate);
                Context.Entry(userTemplate).Property(x => x.Templatecontent).IsModified = true;
                await Context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Logger.Error("ModifyUserTempalteContent : " + ex.ToString());
                throw ex;
            }
        }

        

        #endregion

    }
}
