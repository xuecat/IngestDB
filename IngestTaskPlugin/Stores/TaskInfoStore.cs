using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IngestDBCore;
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
