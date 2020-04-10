using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IngestDBCore;
using IngestDBCore.Tool;
using IngestTaskPlugin.Dto;
using IngestTaskPlugin.Models;
using Microsoft.EntityFrameworkCore;
using Sobey.Core.Log;

namespace IngestTaskPlugin.Stores
{

    public class TaskInfoStore : ITaskStore
    {

        public TaskInfoStore(IngestTaskDBContext baseDataDbContext)
        {
            Context = baseDataDbContext;
        }

        protected IngestTaskDBContext Context { get; }
        private readonly ILogger Logger = LoggerManager.GetLogger("TaskInfo");
        public async Task<TResult> GetTaskAsync<TResult>(Func<IQueryable<DbpTask>, IQueryable<TResult>> query, bool notrack = false)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            if (notrack)
            {
                return await query.Invoke(Context.DbpTask.AsNoTracking()).SingleOrDefaultAsync();
            }
            return await query.Invoke(Context.DbpTask).SingleOrDefaultAsync();
        }

        public async Task<List<TResult>> GetTaskListAsync<TResult>(Func<IQueryable<DbpTask>, IQueryable<TResult>> query, bool notrack = false)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            if (notrack)
            {
                return await query.Invoke(Context.DbpTask.AsNoTracking()).ToListAsync();
            }
            return await query.Invoke(Context.DbpTask).ToListAsync();
        }

        public async Task<TResult> GetTaskMetaDataAsync<TResult>(Func<IQueryable<DbpTaskMetadata>, IQueryable<TResult>> query, bool notrack = false)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            if (notrack)
            {
                return await query.Invoke(Context.DbpTaskMetadata.AsNoTracking()).SingleOrDefaultAsync();
            }
            return await query.Invoke(Context.DbpTaskMetadata).SingleOrDefaultAsync();
        }

        public async Task<TResult> GetTaskCustomMetaDataAsync<TResult>(Func<IQueryable<DbpTaskCustommetadata>, IQueryable<TResult>> query, bool notrack = false)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            if (notrack)
            {
                return await query.Invoke(Context.DbpTaskCustommetadata.AsNoTracking()).SingleOrDefaultAsync();
            }
            return await query.Invoke(Context.DbpTaskCustommetadata).SingleOrDefaultAsync();
        }

        public async Task UpdateTaskMetaDataAsync(int taskid, int type, string metadata)
        {
            var item = await Context.DbpTaskMetadata.Where(x => x.Taskid == taskid && x.Metadatatype == type).SingleAsync();
            if (item == null)
            {
                await Context.DbpTaskMetadata.AddAsync(new DbpTaskMetadata()
                {
                    Taskid = taskid,
                    Metadatatype = type,
                    Metadatalong = metadata
                });
            }
            else
                item.Metadatalong = metadata;

            try
            {
                await Context.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {
                throw e;
            }
        }

        public async Task UpdateTaskCutomMetaDataAsync(int taskid, string metadata)
        {
            var item = await Context.DbpTaskCustommetadata.Where(x => x.Taskid == taskid).SingleAsync();
            if (item == null)
            {
                await Context.DbpTaskCustommetadata.AddAsync(new DbpTaskCustommetadata()
                {
                    Taskid = taskid,
                    Metadata = metadata
                });
            }
            else
                item.Metadata = metadata;

            try
            {
                await Context.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {
                throw e;
            }
        }

        public async Task SageChangeAsync()
        {
            try
            {
                await Context.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {
                throw e;
            }
        }

        public async Task UpdateVtrUploadTaskListStateAsync(List<int> lsttaskid, VTRUPLOADTASKSTATE vtrstate, string errinfo, bool savechange = true)
        {
            var lsttask = await Context.VtrUploadtask.Where(a => lsttaskid.Contains(a.Taskid))
                .Select(item => new VtrUploadtask {
                    Taskid = item.Taskid,
                    Taskguid = item.Taskguid,
                    Usertoken = item.Usertoken,
                    Taskstate = item.Taskstate
                }).ToListAsync();

            foreach (var itm in lsttask)
            {

                if ((vtrstate == VTRUPLOADTASKSTATE.VTR_UPLOAD_COMMIT)
                        && (itm.Taskstate == (Decimal)VTRUPLOADTASKSTATE.VTR_UPLOAD_COMPLETE))
                {
                    itm.Taskguid = Guid.NewGuid().ToString();
                }

                itm.Taskstate = (int)vtrstate;
                if (vtrstate == VTRUPLOADTASKSTATE.VTR_UPLOAD_FAIL)
                {
                    itm.Usertoken = errinfo;
                }
                Context.Attach(itm);
                var entry = Context.Entry(itm);
                entry.Property(x => x.Taskguid).IsModified = true;
                entry.Property(x => x.Taskstate).IsModified = true;
                entry.Property(x => x.Usertoken).IsModified = true;
            }

            if (savechange)
            {
                try
                {
                    await Context.SaveChangesAsync();
                }
                catch (DbUpdateException e)
                {
                    throw e;
                }
            }
        }

        public async Task UpdateVtrUploadTaskStateAsync(int taskid, VTRUPLOADTASKSTATE vtrstate, string errinfo, bool savechange= true)
        {
            var itm = await Context.VtrUploadtask.Where(a => taskid == a.Taskid)
                .Select(item => new VtrUploadtask
                {
                    Taskid = item.Taskid,
                    Taskguid = item.Taskguid,
                    Usertoken = item.Usertoken,
                    Taskstate = item.Taskstate
                }).SingleOrDefaultAsync();

            if ((vtrstate == VTRUPLOADTASKSTATE.VTR_UPLOAD_COMMIT)
                    && (itm.Taskstate == (Decimal)VTRUPLOADTASKSTATE.VTR_UPLOAD_COMPLETE))
            {
                itm.Taskguid = Guid.NewGuid().ToString();
            }

            itm.Taskstate = (int)vtrstate;
            if (vtrstate == VTRUPLOADTASKSTATE.VTR_UPLOAD_FAIL)
            {
                itm.Usertoken = errinfo;
            }
            Context.Attach(itm);
            var entry = Context.Entry(itm);
            entry.Property(x => x.Taskguid).IsModified = true;
            entry.Property(x => x.Taskstate).IsModified = true;
            entry.Property(x => x.Usertoken).IsModified = true;

            if (savechange)
            {
                try
                {
                    await Context.SaveChangesAsync();
                }
                catch (DbUpdateException e)
                {
                    throw e;
                }
            }
        }
        
        public async Task DeleteVtrUploadTaskAsync(int taskid, DbpTask task, bool savechange = true)
        {
            var itm = await Context.VtrUploadtask.Where(a => taskid == a.Taskid)
                .Select(item => new VtrUploadtask
                {
                    Taskid = item.Taskid,
                    Usertoken = item.Usertoken,
                    Taskstate = item.Taskstate
                }).SingleOrDefaultAsync();

            if (task == null)
            {
                task = await GetTaskAsync<DbpTask>(a => a
                    .Where(x => x.Taskid == taskid)
                    .Select(ite =>
                    new DbpTask
                    {
                        Tasktype = ite.Tasktype,
                        State = ite.State,
                        DispatchState = ite.DispatchState,
                        OpType = ite.OpType,
                        SyncState = ite.SyncState,
                        Endtime = ite.Endtime,
                        NewEndtime = ite.Endtime,
                        Recunitid = ite.Recunitid,
                    }
                    ));
            }

            if (itm.Taskstate == (int)VTRUPLOADTASKSTATE.VTR_UPLOAD_TEMPSAVE ||
                                itm.Taskstate == (int)VTRUPLOADTASKSTATE.VTR_UPLOAD_FAIL ||
                                itm.Taskstate == (int)VTRUPLOADTASKSTATE.VTR_UPLOAD_DELETE ||
                                itm.Taskstate == (int)VTRUPLOADTASKSTATE.VTR_UPLOAD_COMMIT)
            {
                Context.VtrUploadtask.Attach(itm);
                Context.VtrUploadtask.Remove(itm);
                Context.DbpTask.Attach(task);
                Context.DbpTask.Remove(task);
            }
            else if (itm.Taskstate == (int)VTRUPLOADTASKSTATE.VTR_UPLOAD_PRE_EXECUTE)
            {
                itm.Taskstate = (int)VTRUPLOADTASKSTATE.VTR_UPLOAD_DELETE;
                task.State = (int)taskState.tsDelete;
                task.OpType = (int)opType.otDel;
                task.DispatchState = (int)dispatchState.dpsDispatchFailed;

                Context.Attach(itm);
                var itmentry = Context.Entry(itm);
                itmentry.Property(x => x.Taskstate).IsModified = true;

                Context.Attach(task);
                var entry = Context.Entry(task);
                entry.Property(x => x.State).IsModified = true;
                entry.Property(x => x.OpType).IsModified = true;
                entry.Property(x => x.DispatchState).IsModified = true;
            }
            else if (itm.Taskstate == (int)VTRUPLOADTASKSTATE.VTR_UPLOAD_EXECUTE)
            {
                itm.Taskstate = (int)VTRUPLOADTASKSTATE.VTR_UPLOAD_DELETE;
                task.SyncState = (int)syncState.ssNot;
                task.OpType = (int)opType.otDel;
                task.DispatchState = (int)dispatchState.dpsInvalid;
                task.State = (int)taskState.tsInvaild;
                task.Endtime = task.NewEndtime = DateTime.Now;

                Context.Attach(itm);
                var itmentry = Context.Entry(itm);
                itmentry.Property(x => x.Taskstate).IsModified = true;

                Context.Attach(task);
                var entry = Context.Entry(task);
                entry.Property(x => x.Endtime).IsModified = true;
                entry.Property(x => x.NewEndtime).IsModified = true;
                entry.Property(x => x.SyncState).IsModified = true;
                entry.Property(x => x.State).IsModified = true;
                entry.Property(x => x.OpType).IsModified = true;
                entry.Property(x => x.DispatchState).IsModified = true;
            }

            if (savechange)
            {
                try
                {
                    await Context.SaveChangesAsync();
                }
                catch (DbUpdateException e)
                {
                    throw e;
                }
            }
        }

        public async Task<int> StopCapturingChannelAsync(int Channel)
        {
            int ntaskid = 0;
            var ltask = await GetTaskAsync<DbpTask>(a => a
            .Where(x => x.Channelid == Channel)
            .Select(ite =>
            new DbpTask
            {
                Tasktype = ite.Tasktype,
                Endtime = ite.Endtime,
                NewEndtime = ite.Endtime,
                Recunitid = ite.Recunitid,
            }));

            ntaskid = ltask.Taskid;
            if (ltask.Tasktype == (int)TaskType.TT_VTRUPLOAD)
            {
                //vtr更新
                await UpdateVtrUploadTaskStateAsync(ltask.Taskid, VTRUPLOADTASKSTATE.VTR_UPLOAD_COMPLETE, string.Empty, false);
            }
            else
            {
                ltask.Endtime = DateTime.Now;
                ltask.NewEndtime = ltask.Endtime;
                ltask.Recunitid = ltask.Recunitid | 0x8000;

                if (ltask.Tasktype == (int)TaskType.TT_MANUTASK
                    || ltask.Tasktype == (int)TaskType.TT_OPENEND
                    || ltask.Tasktype == (int)TaskType.TT_TIEUP)
                {
                    ltask.Tasktype = (int)TaskType.TT_NORMAL;
                }

                Context.Attach(ltask);
                var entry = Context.Entry(ltask);
                entry.Property(x => x.Endtime).IsModified = true;
                entry.Property(x => x.NewEndtime).IsModified = true;
                entry.Property(x => x.Recunitid).IsModified = true;

                try
                {
                    await Context.SaveChangesAsync();
                }
                catch (DbUpdateException e)
                {
                    throw e;
                }
                
            }
            return ntaskid;
        }

        public async Task<List<int>> StopCapturingListChannelAsync(List<int> lstChaneel)
        {
            List<int> lstnreture = new List<int>();
            //获取采集中任务列表
            var lsttask = await GetTaskListAsync<DbpTask>(a => a
            .Where(x => lstChaneel.Contains(x.Channelid.GetValueOrDefault()) && (x.State == (int)taskState.tsExecuting || x.State == (int)taskState.tsManuexecuting))
            .Select(ite =>
            new DbpTask {
                Taskid = ite.Taskid,
                Taskguid = ite.Taskguid,
                Tasktype = ite.Tasktype,
                Endtime = ite.Endtime,
                NewEndtime = ite.Endtime,
                Recunitid = ite.Recunitid,
            }
            ));

            //把里面vtr任务和普通任务,list分出来
            var filtertasks = lsttask.GroupBy<DbpTask, int ,DbpTask>(x => x.Tasktype.GetValueOrDefault(), y => y, new VtrComparer());

            foreach (var item in filtertasks)
            {
                if (item.Key == (int)TaskType.TT_VTRUPLOAD)
                {
                    //vtr更新
                    await UpdateVtrUploadTaskListStateAsync(item.Select(f => f.Taskid).ToList(), VTRUPLOADTASKSTATE.VTR_UPLOAD_COMPLETE, string.Empty, false);
                }
                else
                {
                    foreach (var itm in item)
                    {
                        itm.Endtime = DateTime.Now;
                        itm.NewEndtime = itm.Endtime;
                        itm.Recunitid = itm.Recunitid | 0x8000;

                        if (itm.Tasktype == (int)TaskType.TT_MANUTASK
                            || itm.Tasktype == (int)TaskType.TT_OPENEND
                            || itm.Tasktype == (int)TaskType.TT_TIEUP)
                        {
                            itm.Tasktype = (int)TaskType.TT_NORMAL;
                        }

                        Context.Attach(itm);
                        var entry = Context.Entry(itm);
                        entry.Property(x => x.Endtime).IsModified = true;
                        entry.Property(x => x.NewEndtime).IsModified = true;
                        entry.Property(x => x.Recunitid).IsModified = true;
                        lstnreture.Add(itm.Taskid);
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
            }
            return lstnreture;
        }

        public async Task<int> DeleteCapturingChannelAsync(int Channel)
        {
            int ntaskid = 0;
            var ltask = await GetTaskAsync<DbpTask>(a => a
            .Where(x => x.Channelid == Channel)
            .Select(ite =>
            new DbpTask
            {
                Taskid = ite.Taskid,
                //                Taskguid = ite.Taskguid,
                Tasktype = ite.Tasktype,
                State = ite.State,
                OpType = ite.OpType,
                SyncState = ite.SyncState,
                Endtime = ite.Endtime,
                NewEndtime = ite.Endtime,
                Recunitid = ite.Recunitid,
                DispatchState = ite.DispatchState,
            }));

            if (ltask.Tasktype == (int)TaskType.TT_VTRUPLOAD)
            {
                //vtr更新
                await DeleteVtrUploadTaskAsync(ltask.Taskid, ltask, false);
                ntaskid = ltask.Taskid;
            }
            else
            {
                
                if (ltask.State == (int)taskState.tsComplete)
                {
                    await UnLockTask(ltask.Taskid);
                    SobeyRecException.ThrowSelfNoParam(ltask.Taskid.ToString(), GlobalDictionary.GLOBALDICT_CODE_CAN_NOT_DELETE_THE_COMPLETE_TASK, Logger, null);
                }

                bool isNeedDelFromDB = false;
                ltask.OpType = (int)opType.otDel;//先暂时注释，看看后面会有更新不
                if (ltask.DispatchState == (int)dispatchState.dpsNotDispatch ||
                    ltask.DispatchState == (int)dispatchState.dpsRedispatch ||
                    ltask.DispatchState == (int)dispatchState.dpsDispatchFailed ||
                    (ltask.DispatchState == (int)dispatchState.dpsDispatched && ltask.SyncState == (int)syncState.ssNot))
                {
                    //对未分发任务的处理
                    ltask.DispatchState = (int)dispatchState.dpsDispatched;
                    ltask.SyncState = (int)syncState.ssSync;
                    ltask.State = (int)taskState.tsDelete;
                    isNeedDelFromDB = true;
                }
                else
                {
                    ltask.SyncState = (int)syncState.ssNot;
                    ltask.Endtime = DateTime.Now;

                    Logger.Info("strEnd = GlobalFun.DateTimeToString(DateTime.Now) = dpsDispatched nTaskID = {0}", ltask.Taskid);
                }

                //解锁
                ltask.Tasklock = string.Empty;
                //Tie Up 直接删除
                if (ltask.Tasktype == (int)TaskType.TT_TIEUP)
                {
                    ltask.SyncState = (int)syncState.ssSync;
                    ltask.State = (int)taskState.tsDelete;
                    isNeedDelFromDB = true;
                }

                //zmj 2010-12-06 修改个别任务由于开始时间接收得比较晚，造成状态有问题
                //此状态，任务已经开始执行了，但是由于消息队列原因，导致接收的时间比较晚，状态还没有变成正在执行状态
                //这个时间，也需要将任务的结束时间改变，保证正常结束
                if (ltask.State == (int)taskState.tsExecuting
                    || (ltask.DispatchState == (int)dispatchState.dpsDispatched && ltask.SyncState == (int)syncState.ssSync))
                {
                    ltask.Endtime = DateTime.Now;
                    ltask.Recunitid = ltask.Recunitid | 0x8000;
                }

                if (isNeedDelFromDB)
                {
                    Logger.Info("delete task", ltask.Taskid);
                    Context.Attach(ltask);
                    Context.DbpTask.Remove(ltask);
                }
                else
                {
                    //接下来是modifytask逻辑
                    ltask.Tasklock = string.Empty;

                    Context.Attach(ltask);
                    var entry = Context.Entry(ltask);
                    entry.Property(x => x.Endtime).IsModified = true;
                    entry.Property(x => x.SyncState).IsModified = true;
                    entry.Property(x => x.NewEndtime).IsModified = true;
                    entry.Property(x => x.Recunitid).IsModified = true;
                }

                ntaskid = ltask.Taskid;

                try
                {
                    await Context.SaveChangesAsync();
                }
                catch (DbUpdateException e)
                {
                    throw e;
                }
            }

            return ntaskid;
        }

        public async Task<List<int>> DeleteCapturingListChannelAsync(List<int> lstChaneel)
        {
            List<int> lstnreture = new List<int>();
            //获取采集中任务列表
            var lsttask = await GetTaskListAsync<DbpTask>(a => a
            .Where(x => lstChaneel.Contains(x.Channelid.GetValueOrDefault()) && (x.State == (int)taskState.tsExecuting || x.State == (int)taskState.tsManuexecuting))
            .Select(ite =>
            new DbpTask
            {
                Taskid = ite.Taskid,
//                Taskguid = ite.Taskguid,
                Tasktype = ite.Tasktype,
                State = ite.State,
                OpType = ite.OpType,
                SyncState = ite.SyncState,
                Endtime = ite.Endtime,
                NewEndtime = ite.Endtime,
                Recunitid = ite.Recunitid,
                DispatchState = ite.DispatchState,
            }
            ));

            //把里面vtr任务和普通任务,list分出来
            var filtertasks = lsttask.GroupBy<DbpTask, int, DbpTask>(x => x.Tasktype.GetValueOrDefault(), y => y, new VtrComparer());
            foreach (var item in filtertasks)
            {
                if (item.Key == (int)TaskType.TT_VTRUPLOAD)
                {
                    //vtr更新
                    foreach (var itm in item)
                    {
                        await DeleteVtrUploadTaskAsync(itm.Taskid, itm, false);
                        lstnreture.Add(itm.Taskid);
                    }
                }
                else
                {
                    foreach (var itm in item)
                    {
                        if (itm.State == (int)taskState.tsComplete)
                        {
                            await UnLockTask(itm.Taskid);
                            SobeyRecException.ThrowSelfNoParam(itm.Taskid.ToString(), GlobalDictionary.GLOBALDICT_CODE_CAN_NOT_DELETE_THE_COMPLETE_TASK, Logger, null);
                        }

                        bool isNeedDelFromDB = false;
                        itm.OpType = (int)opType.otDel;//先暂时注释，看看后面会有更新不
                        if (itm.DispatchState == (int)dispatchState.dpsNotDispatch ||
                            itm.DispatchState == (int)dispatchState.dpsRedispatch ||
                            itm.DispatchState == (int)dispatchState.dpsDispatchFailed ||
                            (itm.DispatchState == (int)dispatchState.dpsDispatched && itm.SyncState == (int)syncState.ssNot))
                        {
                            //对未分发任务的处理
                            itm.DispatchState = (int)dispatchState.dpsDispatched;
                            itm.SyncState = (int)syncState.ssSync;
                            itm.State = (int)taskState.tsDelete;
                            isNeedDelFromDB = true;
                        }
                        else
                        {
                            itm.SyncState = (int)syncState.ssNot;
                            itm.Endtime = DateTime.Now;

                            Logger.Info("strEnd = GlobalFun.DateTimeToString(DateTime.Now) = dpsDispatched nTaskID = {0}", itm.Taskid);
                        }

                        //解锁
                        itm.Tasklock = string.Empty;
                        //Tie Up 直接删除
                        if (itm.Tasktype == (int)TaskType.TT_TIEUP)
                        {
                            itm.SyncState = (int)syncState.ssSync;
                            itm.State = (int)taskState.tsDelete;
                            isNeedDelFromDB = true;
                        }

                        //zmj 2010-12-06 修改个别任务由于开始时间接收得比较晚，造成状态有问题
                        //此状态，任务已经开始执行了，但是由于消息队列原因，导致接收的时间比较晚，状态还没有变成正在执行状态
                        //这个时间，也需要将任务的结束时间改变，保证正常结束
                        if (itm.State == (int)taskState.tsExecuting
                            || (itm.DispatchState == (int)dispatchState.dpsDispatched && itm.SyncState == (int)syncState.ssSync))
                        {
                            itm.Endtime = DateTime.Now;
                            itm.Recunitid = itm.Recunitid | 0x8000;
                        }

                        if (isNeedDelFromDB)
                        {
                            Logger.Info("delete task", itm.Taskid);
                            Context.Attach(itm);
                            Context.DbpTask.Remove(itm);
                        }
                        else
                        {
                            //接下来是modifytask逻辑
                            itm.Tasklock = string.Empty;

                            Context.Attach(itm);
                            var entry = Context.Entry(itm);
                            entry.Property(x => x.Endtime).IsModified = true;
                            entry.Property(x => x.SyncState).IsModified = true;
                            entry.Property(x => x.NewEndtime).IsModified = true;
                            entry.Property(x => x.Recunitid).IsModified = true;
                        }
                        
                        lstnreture.Add(itm.Taskid);
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
            }

            return lstnreture;
        }

        public async Task<List<int>> GetFreeChannels(List<int> lst, DateTime begin, DateTime end)
        {
            var lstchn = await Context.DbpTask.AsNoTracking().Where(x => x.Endtime > DateTime.Now
            && ((x.Starttime >= begin && x.Starttime < end) 
            || (x.Endtime > begin && x.Endtime <= end))
            && (x.State != (int)taskState.tsConflict && x.State != (int)taskState.tsDelete && x.State != (int)taskState.tsInvaild)
            && x.DispatchState != (int)dispatchState.dpsInvalid
            && x.OpType != (int)opType.otDel
            && (x.Tasktype != (int)TaskType.TT_PERIODIC && x.Tasktype != (int)TaskType.TT_OPENEND && x.Tasktype != (int)TaskType.TT_OPENENDEX))
            .Select(y => y.Channelid).ToListAsync();

            Logger.Info("GetFreeChannels normal " + string.Join(",", lstchn));

            bool overday = false;
            if (begin.Date != end.Date)
            {
                overday = true;
            }

            DateTime periodbegin = begin; periodbegin.AddDays(-1);
            DateTime periodend = end; periodend.AddDays(-1);

            int preweek = (int)begin.DayOfWeek;
            int preday = begin.Day;
            int week = (int)end.DayOfWeek;
            int day = end.Day;

            List<DbpTask> lstperiod = null;
            if (overday)
            {
                lstperiod = await Context.DbpTask.AsNoTracking().Where(x =>
                ((x.Starttime >= periodbegin && x.Starttime <= periodend)
                || (x.Endtime >= periodbegin && x.Endtime <= periodend)
                || (x.Starttime <= periodbegin && x.Endtime >= periodend))
                && (!string.IsNullOrEmpty(x.Category)
                && (x.Category.IndexOf($"W{preweek}+") >= 0 || x.Category.IndexOf($"W{week}+") >= 0) 
                || x.Category.IndexOf($"M{preday}+") >= 0 || x.Category.IndexOf($"M{day}+") >= 0
                || x.Category.IndexOf($"D") >= 0)
                && x.OpType != (int)opType.otDel
                && x.Tasktype == (int)TaskType.TT_PERIODIC
                && x.DispatchState != (int)dispatchState.dpsInvalid
                ).ToListAsync();
            }
            else
            {
                lstperiod = await Context.DbpTask.AsNoTracking().Where(x =>
                ((x.Starttime >= periodbegin && x.Starttime <= periodend)
                || (x.Endtime >= periodbegin && x.Endtime <= periodend)
                || (x.Starttime <= periodbegin && x.Endtime >= periodend))
                && (!string.IsNullOrEmpty(x.Category)
                && (x.Category.IndexOf($"W{week}+") >= 0)
                || x.Category.IndexOf($"M{day}+") >= 0
                || x.Category.IndexOf($"D") >= 0)
                && x.OpType != (int)opType.otDel
                && x.Tasktype == (int)TaskType.TT_PERIODIC
                && x.DispatchState != (int)dispatchState.dpsInvalid
                ).ToListAsync();

            }

            if (lstperiod != null && lstperiod.Count > 0)
            {
                Logger.Info("GetFreeChannels period " + string.Join(",", lstperiod.Select(y=>y.Taskid).ToList()));
            }

            List<DbpTask> filtertask = new List<DbpTask>();
            foreach (var item in lstperiod)
            {
                bool isAdd2 = false;

                var fakeTask = FixPeroidcTaskTimeDisplay(item, overday?begin:end, TimeLineType.em24HourDay, ref isAdd2);
                
                //排除例外的任务
                if (IsInvalidPerodicTask(item.Category, item.Starttime))
                    continue;

                if (fakeTask != null)
                {
                    if (fakeTask.State == (int)taskState.tsDelete)
                        continue;
                    filtertask.Add(fakeTask);
                    if (!isAdd2)
                        continue;
                }
                filtertask.Add(item);
            }

            if (filtertask.Count > 0)
            {
                var periodch = filtertask.Select(x => x.Channelid).ToList();
                lst.RemoveAll(z => periodch.Contains(z));
            }
            
            lst.RemoveAll(z => lstchn.Contains(z));
            return lst;
        }

        //需要比较和普通任务的冲突，以及和周期任务的冲突
        public async Task<List<int>> GetFreePerodiChannels(List<int> lst, int nTaskID, int nUnitID, int nSigID, int nChannelID, string Category, DateTime begin, DateTime end)
        {
            //先得到所有可用通道
            //和普通任务的冲突：1.把所有时间段上冲突的选出来 2.对比下看实际的冲突情况，过滤出真正冲突的。
            //和周期任务的冲突：1.把所有周期任务选出来 2.在内存中对比是否冲突。
            //从可用通道中去掉冲突的，就是可用的
            
            
            DateTime dtNowEnd = end;
            DateTime dtNowBegin = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, begin.Hour, begin.Minute, begin.Second);
            dtNowBegin = dtNowBegin.AddSeconds(1);
            dtNowEnd = dtNowEnd.AddSeconds(-1);

            var lsttask = await Context.DbpTask.AsNoTracking().Where(x => 
            //我也不知道老代码这些部分啥意思
            //(x.Starttime >= dtNowBegin.AddDays(-1) && x.Starttime<= dtNowEnd)
            //&& (x.Endtime >=dtNowBegin && x.Endtime <= dtNowEnd.AddDays(1))
            ((x.Starttime >= dtNowBegin && x.Starttime <= dtNowEnd)
             || (x.Endtime >= dtNowBegin && x.Endtime <= dtNowEnd) || (x.Starttime < dtNowBegin && x.Endtime>dtNowEnd))
            && (x.State != (int)taskState.tsDelete /*|| x.State == (int)taskState.tsExecuting*/)
            && (x.DispatchState == (int)dispatchState.dpsDispatched || x.DispatchState == (int)dispatchState.dpsNotDispatch)
            && (nChannelID<=0||(nChannelID>0 && x.Channelid == nChannelID))
            && (nUnitID <=0 || (nUnitID>0&&x.Recunitid == nUnitID))
            //&& (bIncludePerodic || (!bIncludePerodic && x.Tasktype != (int)TaskType.TT_PERIODIC))
            ).ToListAsync();

            if (lsttask != null && lsttask.Count > 0)
            {
                List<DbpTask> filterconficttasklst = new List<DbpTask>();
                foreach (var item in lsttask)
                {
                    if (item.Tasktype == (int)TaskType.TT_PERIODIC)
                    {
                        int addflag = -1;
                        int compflag = -1;
                        DateTime dtStartCheck = (begin > item.Starttime) ? begin : item.Starttime;
                        DateTime dtEndCheck = (end < item.Endtime) ? end : item.Endtime;

                        var addlist = GetDayList(Category, ref addflag);
                        var complist = GetDayList(item.Category, ref compflag);

                        if (addflag > 0 && compflag > 0)
                        {
                            //zmj2008-10-10修改周期任务与周期任务之前的冲突判断
                            var addExcludeList = GetDateTimeFromString(Category);
                            var cmpExcludeList = GetDateTimeFromString(item.Category);

                            while (dtStartCheck.Date <= dtEndCheck.Date)
                            {
                                int m = 0; //判断nAddFlag是否不为0.1.2
                                int n = 0;//判断nCmpFlag是否不为0.1.2
                                bool isExistInaddArray = addExcludeList.Any(x => dtStartCheck.Date == x.Date);
                                bool isExistIncmpArray = cmpExcludeList.Any(x => dtStartCheck.Date == x.Date);

                                if (addflag == 0 && !isExistInaddArray)
                                {
                                    m = 1;
                                }
                                else if (addflag == 1)
                                {
                                    if (addlist.IndexOf((int)dtStartCheck.DayOfWeek) >= 0 && !isExistInaddArray)
                                        m = 1;
                                }
                                else if (addflag == 2)
                                {
                                    if (addlist.IndexOf((int)dtStartCheck.Day) >= 0 && !isExistInaddArray)
                                        m = 1;
                                }
                                if (m == 0)
                                {
                                    dtStartCheck = dtStartCheck.AddDays(1);
                                    continue;
                                }

                                if (compflag == 0 && !isExistIncmpArray)
                                    n = 1;
                                else if (compflag == 1)
                                {
                                    if (complist.IndexOf((int)dtStartCheck.DayOfWeek) >= 0 && !isExistIncmpArray)
                                        n = 1;
                                }
                                else if (compflag == 2)
                                {
                                    if (complist.IndexOf((int)dtStartCheck.Day) >= 0 && !isExistIncmpArray)
                                        n = 1;
                                }
                                if ((m & n) == 1)
                                {
                                    //outCheckContent = checkContent;
                                    //outCheckContent.strBegin = dtStartCheck.ToString();
                                    filterconficttasklst.Add(item);
                                    //return false;
                                    break;
                                }
                                try
                                {
                                    dtStartCheck = dtStartCheck.AddDays(1);
                                }
                                catch (Exception ex)
                                {
                                    Logger.Error($"GetFreePerodiChannels error {ex.Message}");

                                    filterconficttasklst.Add(item);
                                    break;
                                    //return false;
                                }
                            }
                        }
                    }
                    else
                    {
                        var lstdate = GetAllValidePerodicTask(begin, end, Category);

                        Logger.Info("GetFreePerodiChannels period task " + string.Join(",", lsttask.Select(y => y.Taskid).ToList()));

                        if (lstdate != null && lstdate.Count > 0)
                        {
                            Logger.Info("GetFreePerodiChannels period data" + string.Join(",", lstdate.Select(y => y.Begin.ToString() + "  " + y.End.ToString()).ToList()));

                            if (lstdate.Any(y =>
                            (y.End > item.Starttime && y.End < item.Endtime)
                            || (y.Begin < item.Starttime && y.End > item.Endtime)
                            || (y.Begin > item.Starttime && y.End < item.Endtime)
                            || (y.Begin > item.Starttime && y.Begin < item.Endtime)))
                            {
                                filterconficttasklst.Add(item);
                            }

                            //var lsttaskfilter = lsttask.FindAll(x =>
                            //lstdate.Any(y =>
                            //((y.End > x.Starttime && y.End < x.Endtime)
                            //|| (y.Begin < x.Starttime && y.End > x.Endtime)
                            //|| (y.Begin > x.Starttime && y.End < x.Endtime)
                            //|| (y.Begin > x.Starttime && y.Begin < x.Endtime)))).Select(z => z.Channelid).ToList();

                            ////////////////////////////////
                            //判断跨天,我不明白为啥判断跨天
                            if (end.TimeOfDay < begin.TimeOfDay)
                            {

                            }

                            ////////////////////////////////

                            //if (lsttaskfilter != null && lsttaskfilter.Count > 0)
                            //{
                            //    Logger.Info("GetFreePerodiChannels period lsttaskfilter" + string.Join(",", lsttaskfilter));

                            //    lst.RemoveAll(z => lsttaskfilter.Contains(z));
                            //}
                        }
                    }
                }

                if (filterconficttasklst != null && filterconficttasklst.Count > 0)
                {
                    Logger.Info("GetFreePerodiChannels period filterconficttasklst" + string.Join(",", filterconficttasklst));

                    lst.RemoveAll(z => filterconficttasklst.Select(x=> x.Channelid).Contains(z));
                }

            }
            else
                Logger.Info("GetFreePerodiChannels empty");


            return lst;
        }



        public List<TaskSimpleTime> GetAllValidePerodicTask(DateTime tmBegin, DateTime tmEnd, string strPerodicDesc)
        {
            List<TaskSimpleTime> taskList = new List<TaskSimpleTime>();
            bool bOverDay = false;

            if (tmBegin.TimeOfDay > tmEnd.TimeOfDay)
                bOverDay = true;//跨天

            DateTime tmTmpBegin = tmBegin;
            if (strPerodicDesc.IndexOf("D") >= 0) //Daily
            {
                while (tmTmpBegin < tmEnd)
                {
                    TaskSimpleTime tpTask = new TaskSimpleTime();
                    int nAddDay = bOverDay ? 1 : 0;
                    tpTask.Begin = new DateTime(tmTmpBegin.Year, tmTmpBegin.Month, tmTmpBegin.Day, tmBegin.Hour, tmBegin.Minute, tmBegin.Second);
                    tpTask.End = new DateTime(tmTmpBegin.AddDays(nAddDay).Year, tmTmpBegin.AddDays(nAddDay).Month, tmTmpBegin.AddDays(nAddDay).Day, tmEnd.Hour, tmEnd.Minute, tmEnd.Second);
                    taskList.Add(tpTask);
                    tmTmpBegin = tmTmpBegin.AddDays(1);
                }

            }
            else if (strPerodicDesc.IndexOf("W") >= 0)  //Weekly
            {
                while (tmTmpBegin < tmEnd)
                {
                    int nDayofWeek = (int)tmTmpBegin.DayOfWeek;
                    TaskSimpleTime tpTask = new TaskSimpleTime();
                    if (strPerodicDesc.IndexOf("W" + Convert.ToString(nDayofWeek) + "+") >= 0)
                    {
                        int nAddDay = bOverDay ? 1 : 0;
                        tpTask.Begin = new DateTime(tmTmpBegin.Year, tmTmpBegin.Month, tmTmpBegin.Day, tmBegin.Hour, tmBegin.Minute, tmBegin.Second);
                        tpTask.End = new DateTime(tmTmpBegin.AddDays(nAddDay).Year, tmTmpBegin.AddDays(nAddDay).Month, tmTmpBegin.AddDays(nAddDay).Day, tmEnd.Hour, tmEnd.Minute, tmEnd.Second);
                        break;
                    }
                    taskList.Add(tpTask);
                    tmTmpBegin = tmTmpBegin.AddDays(1);
                }


            }
            else if (strPerodicDesc.IndexOf("M") >= 0)  //Monthly
            {

                while (tmTmpBegin < tmEnd)
                {
                    int nDayofMonth = tmTmpBegin.Day;
                    TaskSimpleTime tpTask = new TaskSimpleTime();
                    if (strPerodicDesc.IndexOf("M" + Convert.ToString(nDayofMonth) + "+") >= 0)
                    {
                        int nAddDay = bOverDay ? 1 : 0;
                        tpTask.Begin = new DateTime(tmTmpBegin.Year, tmTmpBegin.Month, tmTmpBegin.Day, tmBegin.Hour, tmBegin.Minute, tmBegin.Second);
                        tpTask.End = new DateTime(tmTmpBegin.AddDays(nAddDay).Year, tmTmpBegin.AddDays(nAddDay).Month, tmTmpBegin.AddDays(nAddDay).Day, tmEnd.Hour, tmEnd.Minute, tmEnd.Second);
                        break;
                    }
                    taskList.Add(tpTask);
                    tmTmpBegin = tmTmpBegin.AddDays(1);
                }

            }
            return taskList;
        }

        public DbpTask DeepClone(DbpTask obj)
        {
            var f = new DbpTask();
            f.Backtype = obj.Backtype;
            f.Backupvtrid = obj.Backupvtrid;
            f.Category = obj.Category;
            f.Channelid = obj.Channelid;
            f.Description = obj.Description;
            f.DispatchState = obj.DispatchState;
            f.Endtime = obj.Endtime;
            f.NewBegintime = obj.NewBegintime;
            f.NewEndtime = obj.NewEndtime;
            f.OldChannelid = obj.OldChannelid;
            f.OpType = obj.OpType;
            f.Recunitid = obj.Recunitid;
            f.Sgroupcolor = obj.Sgroupcolor;
            f.Signalid = obj.Signalid;
            f.Stampimagetype = obj.Stampimagetype;
            f.Stamptitleindex = obj.Stamptitleindex;
            f.Starttime = obj.Starttime;
            f.State = obj.State;
            f.SyncState = obj.SyncState;
            f.Taskguid = obj.Taskguid;
            f.Taskid = obj.Taskid;
            f.Tasklock = obj.Tasklock;
            f.Taskname = obj.Taskname;
            f.Taskpriority = obj.Taskpriority;
            f.Tasktype = obj.Tasktype;
            f.Usercode = obj.Usercode;
            return f;
        }
        public DbpTask FixPeroidcTaskTimeDisplay(DbpTask taskContent, DateTime tmDay, TimeLineType nTimeMode, ref bool isAdd2)
        {
            DbpTask fakeTask = null;
            DateTime DBTaskDateTimeBegin = taskContent.Starttime;
            DateTime DBTaskDateTimeEnd = taskContent.Endtime;
            if (DBTaskDateTimeEnd < DBTaskDateTimeBegin)
            {//结束时间小于开始时间，那么这条任务肯定是无效的，不予显示
                taskContent.Starttime = DBTaskDateTimeEnd;
                taskContent.Endtime = DBTaskDateTimeEnd;
                return null;
            }
            if (DBTaskDateTimeEnd.Date < tmDay.Date)
            {
                //这样就不会显示
                taskContent.Starttime = DBTaskDateTimeEnd;
                taskContent.Endtime = DBTaskDateTimeEnd;
                return null;
            }
            else if (DBTaskDateTimeBegin.Date > tmDay.Date && nTimeMode == TimeLineType.em24HourDay)
            {
                //这样就不会显示
                taskContent.Starttime = DBTaskDateTimeBegin;
                taskContent.Endtime = DBTaskDateTimeBegin;
                return null;
            }
            else if (DBTaskDateTimeBegin > new DateTime(tmDay.AddDays(1).Year, tmDay.AddDays(1).Month, tmDay.AddDays(1).Day, 7, 59, 59) && nTimeMode == TimeLineType.em32HourDay)
            {
                //这样就不会显示
                taskContent.Starttime = DBTaskDateTimeBegin;
                taskContent.Endtime = DBTaskDateTimeBegin;
                return null;
            }

            if (DBTaskDateTimeBegin.TimeOfDay <= DBTaskDateTimeEnd.TimeOfDay) //没有跨天(加上等于的情况，避免将意外出现的开始等于结束时间的任务设置成跨天任务)
            {
                //在32小时模式下，开始时间处于0－8点之间的任务，今天执行、明天执行和今明两天执行都需要被显示出来
                //A：如果今天不是执行日，明天是，那么需要把任务的日期改成明天
                //B: 如果今天是执行日，明天不是，那么任务日期应该是今天
                //C: 如果今天和明天都是执行日，那么需要再复制一条任务出来显示
                if (nTimeMode == TimeLineType.em32HourDay)
                {
                    if (DBTaskDateTimeBegin.TimeOfDay >= new DateTime(1, 1, 1, 0, 0, 0).TimeOfDay && DBTaskDateTimeBegin.TimeOfDay < new DateTime(1, 1, 1, 8, 0, 0).TimeOfDay)
                    {
                        bool bTorrowDayExec = IsExecuteDay(tmDay.AddDays(1), taskContent.Category, taskContent.Starttime, taskContent.Endtime);
                        bool bTodayExec = IsExecuteDay(tmDay, taskContent.Category, taskContent.Starttime, taskContent.Endtime);
                        //A:
                        if (!bTodayExec && bTorrowDayExec)
                        {
                            taskContent.Starttime = new DateTime(tmDay.AddDays(1).Year, tmDay.AddDays(1).Month, tmDay.AddDays(1).Day, DBTaskDateTimeBegin.Hour, DBTaskDateTimeBegin.Minute, DBTaskDateTimeBegin.Second);
                            taskContent.Endtime = new DateTime(tmDay.AddDays(1).Year, tmDay.AddDays(1).Month, tmDay.AddDays(1).Day, DBTaskDateTimeEnd.Hour, DBTaskDateTimeEnd.Minute, DBTaskDateTimeEnd.Second);
                            return null;
                        }
                        else if (bTodayExec && !bTorrowDayExec)
                        {
                            taskContent.Starttime = new DateTime(tmDay.Year, tmDay.Month, tmDay.Day, DBTaskDateTimeBegin.Hour, DBTaskDateTimeBegin.Minute, DBTaskDateTimeBegin.Second);
                            taskContent.Endtime = new DateTime(tmDay.Year, tmDay.Month, tmDay.Day, DBTaskDateTimeEnd.Hour, DBTaskDateTimeEnd.Minute, DBTaskDateTimeEnd.Second);
                            return null;
                        }
                        else if (bTodayExec && bTorrowDayExec)
                        {//如果该任务是非待执行任务且当前时间已经过了该任务时间，将不会从该任务里面克隆一条任务
                            DateTime outPutDateTimeBegin32 = new DateTime(tmDay.Year, tmDay.Month, tmDay.Day, DBTaskDateTimeBegin.Hour, DBTaskDateTimeBegin.Minute, DBTaskDateTimeBegin.Second);
                            DateTime outPutDateTimeEnd32 = new DateTime(tmDay.Year, tmDay.Month, tmDay.Day, DBTaskDateTimeEnd.Hour, DBTaskDateTimeEnd.Minute, DBTaskDateTimeEnd.Second);
                            taskContent.Starttime = outPutDateTimeBegin32;
                            taskContent.Endtime = outPutDateTimeEnd32;
                            if (taskContent.State != (int)taskState.tsReady && outPutDateTimeBegin32 < DateTime.Now)
                                return null;

                            fakeTask = DeepClone(taskContent);

                            fakeTask.Taskid *= -1;
                            fakeTask.Starttime = new DateTime(tmDay.AddDays(1).Year, tmDay.AddDays(1).Month, tmDay.AddDays(1).Day, DBTaskDateTimeBegin.Hour, DBTaskDateTimeBegin.Minute, DBTaskDateTimeBegin.Second);
                            fakeTask.Endtime = new DateTime(tmDay.AddDays(1).Year, tmDay.AddDays(1).Month, tmDay.AddDays(1).Day, DBTaskDateTimeEnd.Hour, DBTaskDateTimeEnd.Minute, DBTaskDateTimeEnd.Second);
                            isAdd2 = true;
                            return fakeTask;
                        }
                        else
                        {//今天和明天都不是执行日的任务，不予显示
                            taskContent.Starttime = DBTaskDateTimeBegin;
                            taskContent.Endtime = DBTaskDateTimeBegin;
                            return null;
                        }

                    }
                }
                taskContent.Starttime = new DateTime(tmDay.Year, tmDay.Month, tmDay.Day, DBTaskDateTimeBegin.Hour, DBTaskDateTimeBegin.Minute, DBTaskDateTimeBegin.Second);
                taskContent.Endtime = new DateTime(tmDay.Year, tmDay.Month, tmDay.Day, DBTaskDateTimeEnd.Hour, DBTaskDateTimeEnd.Minute, DBTaskDateTimeEnd.Second);
            }
            else    //跨天，要看今天以及昨天是不是执行日
                    //A-今天是执行日，昨天不是：任务从今天执行到明天
                    //B-今天不是是执行日，昨天是：任务从昨天执行到今天
                    //C-昨天和今天都是执行日：有两个任务？？这种情况复制一个任务出来，ID为负值，作为从昨天执行到今天的任务				
                    //D-昨天和今天都不是执行日，说明任务是明天的，现在又有两种情况，如果任务开始
                    //时间是明天0-8点之间，今天应该显示一部分，如果不是0-8点之间，不予显示
            {
                bool bYesterDayExec = IsExecuteDay(tmDay.AddDays(-1), taskContent.Category, taskContent.Starttime, taskContent.Endtime);
                bool bTodayExec = IsExecuteDay(tmDay, taskContent.Category, taskContent.Starttime, taskContent.Endtime);
                if (bTodayExec && !bYesterDayExec)  //A
                {
                    taskContent.Starttime = new DateTime(tmDay.Year, tmDay.Month, tmDay.Day, DBTaskDateTimeBegin.Hour, DBTaskDateTimeBegin.Minute, DBTaskDateTimeBegin.Second);
                    taskContent.Endtime = new DateTime(tmDay.AddDays(1).Year, tmDay.AddDays(1).Month, tmDay.AddDays(1).Day, DBTaskDateTimeEnd.Hour, DBTaskDateTimeEnd.Minute, DBTaskDateTimeEnd.Second);
                }
                else if (!bTodayExec && bYesterDayExec) //B
                {
                    taskContent.Starttime = new DateTime(tmDay.AddDays(-1).Year, tmDay.AddDays(-1).Month, tmDay.AddDays(-1).Day, DBTaskDateTimeBegin.Hour, DBTaskDateTimeBegin.Minute, DBTaskDateTimeBegin.Second);
                    taskContent.Endtime = new DateTime(tmDay.Year, tmDay.Month, tmDay.Day, DBTaskDateTimeEnd.Hour, DBTaskDateTimeEnd.Minute, DBTaskDateTimeEnd.Second);
                }
                else if (bTodayExec && bYesterDayExec) //C
                { 
                    //					Trace.WriteLine("[DBA]"+taskContent.strTaskName+tmDay.ToString()+"今天和昨天是执行日！");
                    fakeTask = DeepClone(taskContent);
                    fakeTask.Taskid *= -1;
                    fakeTask.Starttime = new DateTime(tmDay.AddDays(-1).Year, tmDay.AddDays(-1).Month, tmDay.AddDays(-1).Day, DBTaskDateTimeBegin.Hour, DBTaskDateTimeBegin.Minute, DBTaskDateTimeBegin.Second);
                    fakeTask.Endtime = new DateTime(tmDay.Year, tmDay.Month, tmDay.Day, DBTaskDateTimeEnd.Hour, DBTaskDateTimeEnd.Minute, DBTaskDateTimeEnd.Second);

                    taskContent.Starttime = new DateTime(tmDay.Year, tmDay.Month, tmDay.Day, DBTaskDateTimeBegin.Hour, DBTaskDateTimeBegin.Minute, DBTaskDateTimeBegin.Second);
                    taskContent.Endtime = new DateTime(tmDay.AddDays(1).Year, tmDay.AddDays(1).Month, tmDay.AddDays(1).Day, DBTaskDateTimeEnd.Hour, DBTaskDateTimeEnd.Minute, DBTaskDateTimeEnd.Second);

                    isAdd2 = true;
                }
                else if (!bTodayExec && !bYesterDayExec)
                {
                    //开始时间位于0-8点之间
                    if (DBTaskDateTimeBegin.TimeOfDay >= new DateTime(1, 1, 1, 0, 0, 0).TimeOfDay && DBTaskDateTimeBegin.TimeOfDay < new DateTime(1, 1, 1, 8, 0, 0).TimeOfDay)
                    {
                        fakeTask = DeepClone(taskContent);
                        fakeTask.Starttime = new DateTime(tmDay.AddDays(1).Year, tmDay.AddDays(1).Month, tmDay.AddDays(1).Day, DBTaskDateTimeBegin.Hour, DBTaskDateTimeBegin.Minute, DBTaskDateTimeBegin.Second);
                        fakeTask.Endtime = new DateTime(tmDay.AddDays(2).Year, tmDay.AddDays(2).Month, tmDay.AddDays(2).Day, DBTaskDateTimeEnd.Hour, DBTaskDateTimeEnd.Minute, DBTaskDateTimeEnd.Second);
                    }
                    else
                    {
                        fakeTask = DeepClone(taskContent);
                        fakeTask.State = (int)taskState.tsDelete;
                    }
                }
            }
            return fakeTask;
        }
        private bool IsExecuteDay(DateTime tmDay, string strPerodicDesc, DateTime dtPerodicValidDateMin, DateTime dtPerodicValidDateMax)
        {
            int nDayOfWeek = (int)tmDay.DayOfWeek;
            int nDayOfMonth = tmDay.Day;
            string strDayOfWeek = "W" + Convert.ToString(nDayOfWeek) + "+";
            string strDayOfMonth = "M" + Convert.ToString(nDayOfMonth) + "+";
            string strEveryDay = "D";
            //zmj2008-10-17修改判断排除掉例外的任务
            var arrinValidList = GetDateTimeFromString(strPerodicDesc);
            bool isInInValid = IsExistInArrayForStatic(arrinValidList, tmDay);
            if (strPerodicDesc.IndexOf(strDayOfWeek) != -1 && dtPerodicValidDateMin.Date <= tmDay.Date && dtPerodicValidDateMax.Date >= tmDay.Date && !isInInValid)
                return true;
            if (strPerodicDesc.IndexOf(strDayOfMonth) != -1 && dtPerodicValidDateMin.Date <= tmDay.Date && dtPerodicValidDateMax.Date >= tmDay.Date && !isInInValid)
                return true;
            if (strPerodicDesc.IndexOf(strEveryDay) != -1 && dtPerodicValidDateMin.Date <= tmDay.Date && dtPerodicValidDateMax.Date >= tmDay.Date && !isInInValid)
                return true;
            return false;
        }

        private bool IsInvalidPerodicTask(string strClassify, DateTime begin)
        {
            int nStart = 0;
            int nPos = strClassify.IndexOf('[', nStart);
            bool bInvalide = false;
            while (nPos > 0)
            {
                if (strClassify.Length < nPos + 20)
                    break;

                DateTime dtInValidDay = DateTimeFormat.DateTimeFromString(strClassify.Substring(nPos + 1, 19));
                if (dtInValidDay.Date == begin.Date)
                {
                    bInvalide = true;
                    break;
                }
                nStart = nPos + 20;
                nPos = strClassify.IndexOf('[', nStart);
            }
            return bInvalide;
        }
        private bool IsExistInArrayForStatic(List<DateTime> arrList, DateTime dtTime)
        {
            foreach (DateTime dt in arrList)
            {
                if (dt.Date == dtTime.Date)
                {
                    return true;
                }
            }
            return false;
        }

        //根据CATEGORY获取执行日列表
        private List<int> GetDayList(string str, ref int nFlag)//0每天 1周任务 2月任务
        {
            List<int> list = new List<int>();
            if (str.IndexOf("D") >= 0)
            {
                nFlag = 0;
            }
            else if (str.IndexOf("W") >= 0)
            {
                nFlag = 1;
                for (int i = 0; i < 7; i++)
                {
                    string strDay = "W" + i.ToString() + "+";
                    if (str.IndexOf(strDay) >= 0)
                        list.Add(i);
                }
            }
            else
            {
                nFlag = 2;
                for (int i = 1; i < 32; i++)
                {
                    string strDay = "M" + i.ToString() + "+";
                    if (str.IndexOf(strDay) >= 0)
                        list.Add(i);
                }
            }
            return list;
        }
        private List<DateTime> GetDateTimeFromString(string str)
        {
            List<DateTime> DatetimeArray = new List<DateTime>();
            int nLPos = 0;
            int nRPos = 0;
            try
            {
                while (nRPos < str.Length - 1 && nRPos != -1 && nLPos != -1)
                {
                    nLPos = str.IndexOf('[', nRPos);
                    if (nLPos != -1)
                        nRPos = str.IndexOf(']', nLPos);
                    if (nRPos != -1 && nLPos != -1)
                        DatetimeArray.Add(DateTimeFormat.DateTimeFromString(str.Substring(nLPos + 1, nRPos - nLPos - 1)));
                    //nLPos = str.IndexOf('[',nRPos);
                    //nRPos = str.IndexOf(']',nLPos);
                }
                return DatetimeArray;
            }
            catch (System.Exception)
            {
                DatetimeArray.Add(DateTime.MinValue);
                return DatetimeArray;
            }

        }

        public async Task LockTask(int taskid)
        {
            
            //Context.Entry(await Context.DbpTask.FirstOrDefaultAsync(x => x.Taskid == taskid)).CurrentValues.SetValues();
            //(await Context.SaveChangesAsync()) > 0;
        }

        public async Task UnLockTask(int taskid)
        {
            DbpTask obj = new DbpTask()
            {
                Taskid = taskid,
                Tasklock = string.Empty,
            };
            Context.Attach(obj);
            Context.Entry(obj).Property("Tasklock").IsModified = true;
            await Context.SaveChangesAsync();
        }

        /////////////////////
    }
}
