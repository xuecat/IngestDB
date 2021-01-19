using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using IngestDBCore;
using IngestDBCore.Tool;
using IngestTaskPlugin.Dto;
using IngestTaskPlugin.Dto.OldResponse;
using IngestTaskPlugin.Dto.Response;
using IngestTaskPlugin.Models;
using Microsoft.EntityFrameworkCore;
using Sobey.Core.Log;
using CooperantType = IngestTaskPlugin.Dto.OldResponse.CooperantType;
using taskState = IngestTaskPlugin.Dto.OldResponse.taskState;
using TaskType = IngestTaskPlugin.Dto.OldResponse.TaskType;

namespace IngestTaskPlugin.Stores
{

    public class TaskInfoStore : ITaskStore
    {

        public TaskInfoStore(IngestTaskDBContext baseDataDbContext)
        {
            Context = baseDataDbContext;
        }

        protected string ConfictTaskInfo { get; set; }
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

        public async Task<TResult> GetTaskSourceAsync<TResult>(Func<IQueryable<DbpTaskSource>, IQueryable<TResult>> query, bool notrack = false)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            if (notrack)
            {
                return await query.Invoke(Context.DbpTaskSource.AsNoTracking()).SingleOrDefaultAsync();
            }
            return await query.Invoke(Context.DbpTaskSource).SingleOrDefaultAsync();
        }

        public async Task<TResult> GetVtrUploadTaskAsync<TResult>(Func<IQueryable<VtrUploadtask>, IQueryable<TResult>> query, bool notrack = false)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            if (notrack)
            {
                return await query.Invoke(Context.VtrUploadtask.AsNoTracking()).SingleOrDefaultAsync();
            }
            return await query.Invoke(Context.VtrUploadtask).SingleOrDefaultAsync();
        }

        public Task<List<TResult>> GetVtrUploadTaskListAsync<TResult>(Func<IQueryable<VtrUploadtask>, IQueryable<TResult>> query, bool notrack = false)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            if (notrack)
            {
                return query.Invoke(Context.VtrUploadtask.AsNoTracking()).ToListAsync();
            }
            return query.Invoke(Context.VtrUploadtask).ToListAsync();
        }

        public async Task<TResult> GetTaskBackupAsync<TResult>(Func<IQueryable<DbpTaskBackup>, IQueryable<TResult>> query, bool notrack = false)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            if (notrack)
            {
                return await query.Invoke(Context.DbpTaskBackup.AsNoTracking()).SingleOrDefaultAsync();
            }
            return await query.Invoke(Context.DbpTaskBackup).SingleOrDefaultAsync();
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


        public async Task UpdateTaskListAsync(List<DbpTask> lst)
        {
            Context.DbpTask.UpdateRange(lst);
            try
            {
                await Context.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {
                throw e;
            }
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

        public async Task<List<TResult>> GetTaskMetaDataListAsync<TResult>(Func<IQueryable<DbpTaskMetadata>, IQueryable<TResult>> query, bool notrack = false)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            if (notrack)
            {
                return await query.Invoke(Context.DbpTaskMetadata.AsNoTracking()).ToListAsync();
            }
            return await query.Invoke(Context.DbpTaskMetadata).ToListAsync();
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

        /// <summary>
        /// lock现在没有用
        /// </summary>
        /// <param name="uselock">没用</param>
        /// <param name="Track">Track模式</param>
        /// <param name="condition">查询条件</param>
        /// <returns></returns>
        public async Task<List<DbpTask>> GetTaskListAsync(TaskCondition condition, bool Track, bool uselock)
        {
            IQueryable<DbpTask> lst = null;
            if (!Track)
            {
                lst = Context.DbpTask.AsNoTracking();

            }
            else
            {
                lst = Context.DbpTask;
            }
            if (condition.ChannelId > 0)
            {
                lst = lst.Where(x => x.Channelid == condition.ChannelId);
            }
            if (condition.RecUnit > 0)
            {
                lst = lst.Where(x => x.Recunitid == condition.RecUnit);
            }
            if (condition.SignalId > 0)
            {
                lst = lst.Where(x => x.Signalid == condition.SignalId);
            }
            if (!string.IsNullOrEmpty(condition.LockStr))
            {
                lst = lst.Where(x => x.Tasklock == condition.LockStr);
            }
            if (condition.StateIncludeLst != null && condition.StateIncludeLst.Count > 0)
            {
                lst = lst.Where(x => condition.StateIncludeLst.Contains(x.State.GetValueOrDefault()));
            }
            if (condition.TaskTypeIncludeLst != null && condition.TaskTypeIncludeLst.Count > 0)
            {
                lst = lst.Where(x => condition.TaskTypeIncludeLst.Contains(x.Tasktype.GetValueOrDefault()));
            }
            if (condition.SyncStateIncludeLst != null && condition.SyncStateIncludeLst.Count > 0)
            {
                lst = lst.Where(x => condition.SyncStateIncludeLst.Contains(x.SyncState.GetValueOrDefault()));
            }
            if (condition.DispatchStateIncludeLst != null && condition.DispatchStateIncludeLst.Count > 0)
            {
                lst = lst.Where(x => condition.DispatchStateIncludeLst.Contains(x.DispatchState.GetValueOrDefault()));
            }
            if (condition.MaxBeginTime > DateTime.MinValue)
            {
                lst = lst.Where(x => x.Starttime < condition.MaxBeginTime);
            }
            if (condition.MinBeginTime > DateTime.MinValue)
            {
                lst = lst.Where(x => x.Starttime > condition.MinBeginTime);
            }
            if (condition.MaxEndTime > DateTime.MinValue)
            {
                lst = lst.Where(x => x.Endtime < condition.MaxEndTime);
            }
            if (condition.MinEndTime > DateTime.MinValue)
            {
                lst = lst.Where(x => x.Endtime > condition.MinEndTime);
            }
            if (condition.MaxNewBeginTime > DateTime.MinValue)
            {
                lst = lst.Where(x => x.NewBegintime < condition.MaxNewBeginTime);
            }
            if (condition.MinNewBeginTime > DateTime.MinValue)
            {
                lst = lst.Where(x => x.NewBegintime > condition.MinNewBeginTime);
            }
            if (condition.MaxNewEndTime > DateTime.MinValue)
            {
                lst = lst.Where(x => x.NewEndtime < condition.MaxNewEndTime);
            }
            if (condition.MinNewEndTime > DateTime.MinValue)
            {
                lst = lst.Where(x => x.NewEndtime > condition.MinNewEndTime);
            }
            return await lst.ToListAsync();
        }

        public async Task<List<TimePeriod>> GetTimePeriodsByScheduleVBUTasks(int vtrid, int extaskid)
        {
            return await (from task in Context.DbpTask
                          join vtr in Context.VtrUploadtask on task.Taskid equals vtr.Taskid into ps
                          from p in ps.DefaultIfEmpty()
                          where task.Taskid != extaskid && task.NewEndtime > DateTime.Now && task.State != (int)taskState.tsDelete
                          && task.State != (int)taskState.tsInvaild && p != null && p.Vtrtasktype == (int)VTRUPLOADTASKTYPE.VTR_SCHEDULE_UPLOAD
                          && p.Vtrid == vtrid
                          select new TimePeriod
                          {
                              Id = vtrid,
                              StartTime = task.NewBegintime,
                              EndTime = task.NewEndtime
                          }).ToListAsync();
        }

        public async Task<List<DbpTask>> GetNeedFinishTasks()
        {
            var date = DateTime.Now.AddSeconds(600);

            var fdate = DateTime.Now.AddSeconds(-86390);//提前10秒

            if (ApplicationContext.Current.Limit24Hours)
            {
                return await Context.DbpTask.AsNoTracking().Where(a => string.IsNullOrEmpty(a.Tasklock)
            && ((a.NewEndtime < date
                && (
                     (a.DispatchState == (int)dispatchState.dpsDispatched && ((a.SyncState == (int)syncState.ssNot && a.OpType == (int)opType.otDel) || (a.SyncState == (int)syncState.ssSync && a.State == (int)taskState.tsExecuting && (a.Tasktype != (int)TaskType.TT_MANUTASK && a.Tasktype != (int)TaskType.TT_TIEUP && a.Tasktype != (int)TaskType.TT_VTRUPLOAD))))
                     || (a.Backtype == (int)CooperantType.emKamataki && a.SyncState == (int)syncState.ssSync)
                     || (a.Backtype == (int)CooperantType.emVTRBackup && a.SyncState == (int)syncState.ssSync && (a.Tasktype != (int)TaskType.TT_PERIODIC || (a.Tasktype == (int)TaskType.TT_PERIODIC && a.OldChannelid > 0)))
                   )
                && a.State != (int)taskState.tsDelete
                && a.Starttime != a.Endtime)
               || (a.State == (int)taskState.tsExecuting && (a.Tasktype == (int)TaskType.TT_MANUTASK || a.Tasktype == (int)TaskType.TT_OPENEND) && a.Starttime <= fdate))//这里本来是starttime..AddSeconds(86400) 怕翻译问题
                ).ToListAsync();
            }
            else
            {
                return await Context.DbpTask.AsNoTracking().Where(a => string.IsNullOrEmpty(a.Tasklock)
            && ((a.NewEndtime < date
                && (
                     (a.DispatchState == (int)dispatchState.dpsDispatched && ((a.SyncState == (int)syncState.ssNot && a.OpType == (int)opType.otDel) || (a.SyncState == (int)syncState.ssSync && a.State == (int)taskState.tsExecuting && (a.Tasktype != (int)TaskType.TT_MANUTASK && a.Tasktype != (int)TaskType.TT_TIEUP && a.Tasktype != (int)TaskType.TT_VTRUPLOAD))))
                     || (a.Backtype == (int)CooperantType.emKamataki && a.SyncState == (int)syncState.ssSync)
                     || (a.Backtype == (int)CooperantType.emVTRBackup && a.SyncState == (int)syncState.ssSync && (a.Tasktype != (int)TaskType.TT_PERIODIC || (a.Tasktype == (int)TaskType.TT_PERIODIC && a.OldChannelid > 0)))
                   )
                && a.State != (int)taskState.tsDelete
                && a.Starttime != a.Endtime))//这里本来是starttime..AddSeconds(86400) 怕翻译问题
                ).ToListAsync();
            }

        }

        public async Task<List<DbpTask>> GetNeedUnSynTasks()
        {
            var now = DateTime.Now;
            var endtime = now.AddMinutes(-3);//周期任务给endtime时间限定，都过时间了就不要再分裂了
            var date = now.AddMinutes(3);//开始后3分钟没调度成功就认为是个错误任务
            var fdate = date.AddDays(-1);
            return await Context.DbpTask.AsNoTracking().Where(x => string.IsNullOrEmpty(x.Tasklock)
                && x.DispatchState == (int)dispatchState.dpsDispatched
                && x.SyncState == (int)syncState.ssNot
                && x.Tasktype != (int)TaskType.TT_VTRUPLOAD
                && x.State != (int)taskState.tsDelete
                && ((x.State != (int)taskState.tsExecuting && x.State != (int)taskState.tsManuexecuting) || x.Backtype != (int)CooperantType.emKamataki)
                && (x.NewBegintime > fdate && x.NewBegintime < date && x.Endtime > endtime)).ToListAsync();
        }

        public async Task UpdateTaskMetaDataAsync(int taskid, MetaDataType type, string metadata)
        {
            var item = await Context.DbpTaskMetadata.Where(x => x.Taskid == taskid && x.Metadatatype == (int)type).FirstOrDefaultAsync();
            if (item == null)
            {
                await Context.DbpTaskMetadata.AddAsync(new DbpTaskMetadata()
                {
                    Taskid = taskid,
                    Metadatatype = (int)type,
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
            var item = await Context.DbpTaskCustommetadata.Where(x => x.Taskid == taskid).SingleOrDefaultAsync();
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

        public async Task SaveChangeAsync()
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

        public async Task<bool> AdjustVtrUploadTasksByChannelId(int channelId, int taskId, DateTime dtCurTaskBegin)
        {
            if (channelId < 0 || taskId < 0)
            {
                return false;
            }

            bool isNeedModifyEndTime = true;
            var taskinfo = await GetTaskAsync(a => a.Where(b => b.Taskid == taskId), true);

            DateTime beginTime = DateTime.Now;
            DateTime endTime = DateTime.Now;

            if (taskinfo != null)
            {
                beginTime = taskinfo.NewBegintime;
                endTime = taskinfo.NewEndtime;
            }

            beginTime = beginTime.AddHours(-2);
            endTime = beginTime.AddHours(50);

            var tasklst = await GetTaskListAsync(a => a.Where(b => b.Channelid == channelId
            && (b.State != (int)taskState.tsDelete && b.State != (int)taskState.tsInvaild)
            && (b.Starttime > beginTime && b.Starttime < endTime)).OrderBy(x=>x.Starttime));// order by CHANNELID, STARTTIME 

            if (tasklst == null || tasklst.Count <= 0)
            {
                return false;
            }
            bool isBegin = false;
            int cout = tasklst.Count;
            for (int i = 0; i < cout; i++)
            {
                var row = tasklst.ElementAt(i);

                Logger.Info("AdjustVtrUploadTasksByChannelId normal item {0} {1} {2} {3}", row.Taskid, row.Tasktype, row.Starttime, row.Endtime);

                if (!isBegin)
                {
                    if (row.Taskid == taskId)
                    {
                        isBegin = true;
                        //往回退一个
                        i--;
                    }

                    continue;
                }

                DbpTask preRow = null;
                DbpTask nextRow = null;
                if (i - 1 >= 0)
                {
                    preRow = tasklst.ElementAt(i - 1);
                }
                if (i + 1 < cout)
                {
                    nextRow = tasklst.ElementAt(i + 1);
                }

                TimeSpan tsDuration = row.NewEndtime - row.NewBegintime;
                DateTime dtNewTaskBeginTime = dtCurTaskBegin;//zmj 2010-07-12外部已经对该时间进行修改
                DateTime dtNewTaskEndTime = dtNewTaskBeginTime.Add(tsDuration);
                bool isNeedJudgeNextTask = false;//标识是否与下一个任务进行判断

                if (row.Taskid == taskId)
                {
                    isNeedJudgeNextTask = true;
                }
                else
                {
                    if (preRow != null)
                    {
                        //判断当前的任务，如果不移的话，会不会与上一个任务冲突
                        //如果不冲突，那么就可以不用移动，提前结束循环
                        if (row.NewBegintime >= preRow.NewEndtime.AddSeconds(3))
                        {
                            break;
                        }
                        else
                        {
                            dtNewTaskBeginTime = preRow.NewEndtime.AddSeconds(3);
                            dtNewTaskEndTime = dtNewTaskBeginTime.Add(tsDuration);

                            isNeedJudgeNextTask = true;
                        }
                    }
                }

                if (isNeedJudgeNextTask)
                {
                    if (nextRow != null)
                    {
                        if (nextRow.Tasktype != (int)TaskType.TT_VTRUPLOAD)
                        {
                            if (dtNewTaskEndTime.AddSeconds(3) > nextRow.NewBegintime)
                            {
                                Logger.Info(string.Format("In AdjustVtrUploadTasksByChannelId,TaskId = {0},meet a schedule task.", taskId));
                                //设置该任务在VTR_UploadTask表中为失败状态
                                //置该任务为失败
                                row.State = (int)taskState.tsDelete;

                                await SetVTRUploadTaskState((int)row.Taskid, VTRUPLOADTASKSTATE.VTR_UPLOAD_FAIL, "VTRBATCHUPLOAD_ERROR_SCHEDULETASKCOLLIDE", false);

                                if (taskId == (int)row.Taskid)
                                {
                                    isNeedModifyEndTime = false;
                                }
                                break;
                            }
                        }
                    }
                }

                Logger.Info(string.Format("In AdjustVtrUploadTasksByChannelId,TaskId = {0},preStartTime = {1},preEndTime = {2}"
                    , row.Taskid, row.Starttime, row.Endtime));

                row.NewBegintime = dtNewTaskBeginTime;
                row.Starttime = dtNewTaskBeginTime;

                row.NewEndtime = dtNewTaskEndTime;
                row.Endtime = dtNewTaskEndTime;

                Logger.Info(string.Format("In AdjustVtrUploadTasksByChannelId,TaskId = {0},nowStartTime = {1},nowEndTime = {2}"
                    , row.Taskid, row.Starttime, row.Endtime));
            }
            try
            {
                await Context.SaveChangesAsync();
            }

            catch (DbUpdateException e)
            {
                throw e;
            }
            return isNeedModifyEndTime;
        }

        public async Task SetVTRUploadTaskState(int TaskId, VTRUPLOADTASKSTATE vtrTaskState, string errorContent, bool savechange)
        {
            var tasklst = await GetVtrUploadTaskAsync(b => b.Where(a => a.Taskid == TaskId));

            if (tasklst != null)
            {
                if (tasklst.Taskstate == (int)VTRUPLOADTASKSTATE.VTR_UPLOAD_COMPLETE
                    && vtrTaskState == VTRUPLOADTASKSTATE.VTR_UPLOAD_COMMIT)//已入库素材重新上载是，改变GUID以保证再次入库时不会覆盖前面的素材
                {
                    tasklst.Taskguid = Guid.NewGuid().ToString("N");
                }
                //else

                tasklst.Taskstate = (int)vtrTaskState;
                if (vtrTaskState == VTRUPLOADTASKSTATE.VTR_UPLOAD_FAIL)
                {
                    tasklst.Usertoken = errorContent;
                }
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

        public async Task UpdateVtrUploadTaskListStateAsync(List<int> lsttaskid, VTRUPLOADTASKSTATE vtrstate, string errinfo, bool savechange = true)
        {
            var lsttask = await Context.VtrUploadtask.Where(a => lsttaskid.Contains(a.Taskid))
                .Select(item => new VtrUploadtask
                {
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
                    itm.Taskguid = Guid.NewGuid().ToString("N");
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

        public async Task UpdateVtrUploadTaskStateAsync(int taskid, VTRUPLOADTASKSTATE vtrstate, string errinfo, bool savechange = true)
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
                itm.Taskguid = Guid.NewGuid().ToString("N");
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

        public async Task<DbpTask> DeleteVtrUploadTaskAsync(int taskid, DbpTask task, bool savechange = true)
        {
            DbpTask backtask = null;

            var itm = await Context.VtrUploadtask.Where(a => taskid == a.Taskid)
                .Select(item => new VtrUploadtask
                {
                    Taskid = item.Taskid,
                    Usertoken = item.Usertoken,
                    Taskstate = item.Taskstate
                }).SingleOrDefaultAsync();

            if (itm == null)
            {
                SobeyRecException.ThrowSelfOneParam("", GlobalDictionary.GLOBALDICT_CODE_CAN_NOT_FIND_THE_VTRTASK_ONEPARAM, Logger, taskid, null);
            }

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
                backtask = task;
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
                backtask = task;
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

            return backtask;
        }

        public async Task<int> DeleteTaskDB(int taskid, bool change)
        {
            var dbptask = await GetTaskAsync(a => a.Where(x => x.Taskid == taskid));
            Context.DbpTask.Remove(dbptask);
            var dbptaskMetadatas = await GetTaskMetaDataListAsync(a => a.Where(x => x.Taskid == taskid));
            Context.DbpTaskMetadata.RemoveRange(dbptaskMetadatas);

            if (change)
            {
                try
                {
                    await Context.SaveChangesAsync();
                }
                catch (Exception e)
                {

                    throw e;
                }
            }

            return taskid;
        }

        public async Task<DbpTask> DeleteTask(int taskid)
        {
            var taskinfo = await GetTaskAsync(a => a.Where(b => b.Taskid == taskid));
            if (taskinfo == null)
            {
                Logger.Info("DeleteTask error empty " + taskid);
                SobeyRecException.ThrowSelfNoParam(taskid.ToString(), GlobalDictionary.GLOBALDICT_CODE_TASK_ID_DOES_NOT_EXIST,
                    Logger, null);
                return null;
            }

            if (taskinfo.State == (int)taskState.tsComplete)
            {
                await UnLockTask(taskinfo, true);
                SobeyRecException.ThrowSelfNoParam(taskid.ToString(), GlobalDictionary.GLOBALDICT_CODE_CAN_NOT_DELETE_THE_COMPLETE_TASK,
                    Logger, null);
                return null;
            }

            if (taskinfo.OpType == (int)opType.otDel && (taskinfo.Recunitid & 0x8000) > 0)
            {
                SobeyRecException.ThrowSelfNoParam(taskid.ToString(), GlobalDictionary.GLOBALDICT_CODE_CAN_NOT_DELETE_DELETING_TASK,
                    Logger, null);
                return null;
            }

            bool isNeedDelFromDB = false;
            taskinfo.OpType = (int)opType.otDel;
            if (taskinfo.DispatchState == (int)dispatchState.dpsNotDispatch ||
                taskinfo.DispatchState == (int)dispatchState.dpsRedispatch ||
                taskinfo.DispatchState == (int)dispatchState.dpsDispatchFailed ||
                (taskinfo.DispatchState == (int)dispatchState.dpsDispatched && taskinfo.SyncState == (int)syncState.ssNot))
            {
                //对未分发任务的处理
                //	info.emDispatchState = dispatchState.dpsInvalid;
                taskinfo.DispatchState = (int)dispatchState.dpsDispatched;
                taskinfo.SyncState = (int)syncState.ssSync;
                taskinfo.State = (int)taskState.tsDelete;
                isNeedDelFromDB = true;
            }
            else
            {
                taskinfo.SyncState = (int)syncState.ssNot;
                // Add by chenzhi 2013-08-08
                // TODO: 这种状态下的任务，修改任务结束时间为当前时间
                //string strlog = "strEnd = GlobalFun.DateTimeToString(DateTime.Now) = dpsDispatched nTaskID = ";
                //strlog += info.taskContent.nTaskID.ToString();
                Logger.Info("strEnd = GlobalFun.DateTimeToString(DateTime.Now) = dpsDispatched nTaskID = ");
                //Sobey.Ingest.Log.LoggerService.Info(strlog);
                //taskinfo.taskContent.strEnd = GlobalFun.DateTimeToString(DateTime.Now);
                taskinfo.Endtime = DateTime.Now;
            }

            //unlock
            taskinfo.Tasklock = string.Empty;
            //Tie Up 直接删除
            if (taskinfo.Tasktype == (int)TaskType.TT_TIEUP)
            {
                taskinfo.SyncState = (int)syncState.ssSync;
                taskinfo.State = (int)taskState.tsDelete;
                isNeedDelFromDB = true;
            }

            //zmj 2010-12-06 修改个别任务由于开始时间接收得比较晚，造成状态有问题
            if (taskinfo.State == (int)taskState.tsExecuting
                //此状态，任务已经开始执行了，但是由于消息队列原因，导致接收的时间比较晚，状态还没有变成正在执行状态
                //这个时间，也需要将任务的结束时间改变，保证正常结束
                || (taskinfo.DispatchState == (int)dispatchState.dpsDispatched && taskinfo.SyncState == (int)syncState.ssSync))
            {
                taskinfo.Endtime = DateTime.Now;
                //删除的任务肯定是手动停止的，需要设置标记，强制停止流媒体任务
                taskinfo.Recunitid = taskinfo.Recunitid | 0x8000;
            }


            if (taskinfo.Tasktype == (int)TaskType.TT_VTRUPLOAD)
            {
                return await DeleteVtrUploadTaskAsync(taskinfo.Taskid, taskinfo, true);
            }
            else
            {
                if (isNeedDelFromDB)
                {
                    Context.DbpTask.Remove(taskinfo);

                    Context.DbpTaskMetadata.RemoveRange(Context.DbpTaskMetadata.Where(x => x.Taskid == taskinfo.Taskid).ToList());

                    try
                    {
                        await Context.SaveChangesAsync();
                    }
                    catch (Exception e)
                    {

                        throw e;
                    }
                    return null;
                }
                else
                {
                    Logger.Info("Before deletetask TASKOPER.ModifyTask" + taskid);
                    await ModifyTask(taskinfo, false, true, true, string.Empty, string.Empty, string.Empty, string.Empty);
                }
            }
            return taskinfo;
        }

        public int StopTaskNoChange(DbpTask taskinfo, DateTime dt)
        {
            Logger.Info("StopTaskNoChange " + taskinfo.Taskid);

            if (dt == DateTime.MinValue)//自动停
            {
                taskinfo.Endtime = DateTime.Now;
                taskinfo.NewEndtime = taskinfo.Endtime;
                taskinfo.Recunitid = taskinfo.Recunitid | 0x8000;
                if (taskinfo.Tasktype == (int)TaskType.TT_MANUTASK
                        || taskinfo.Tasktype == (int)TaskType.TT_OPENEND
                        || taskinfo.Tasktype == (int)TaskType.TT_TIEUP)
                {
                    taskinfo.Tasktype = (int)TaskType.TT_NORMAL;
                }
            }
            else
            {
                if (taskinfo.OldChannelid <= 0 && taskinfo.Tasktype == (int)TaskType.TT_PERIODIC)
                {
                    SobeyRecException.ThrowSelfOneParam("StopTask match periodic", GlobalDictionary.GLOBALDICT_CODE_IN_STOPCAPTURE_TASK_IS_A_STENCIL_PLATE_TASK_ONEPARAM, Logger, taskinfo.Taskid, null);
                }

                taskinfo.Endtime = dt;
                taskinfo.NewEndtime = taskinfo.Endtime;
                taskinfo.State = (int)taskState.tsComplete;
            }
            return taskinfo.Taskid;
        }

        public async Task<DbpTask> StopTask(int taskid, DateTime dt)
        {
            var taskinfo = await GetTaskAsync(a => a.Where(b => b.Taskid == taskid));
            if (taskinfo == null)
            {
                Logger.Info("StopTask error empty " + taskid);
                return null;
            }

            if (taskinfo.State == (int)taskState.tsComplete)
            {
                Logger.Info("StopTask task is tscomplete : " + taskid);
                /*
                 * 运行再次修改任务的停止时间
                 */
                //return taskinfo;
            }

            Logger.Info("StopTask " + taskid);

            if (dt == DateTime.MinValue)//自动停
            {
                taskinfo.Endtime = DateTime.Now;
                taskinfo.NewEndtime = taskinfo.Endtime;
                taskinfo.Recunitid = taskinfo.Recunitid | 0x8000;
                if (taskinfo.Tasktype == (int)TaskType.TT_MANUTASK
                        || taskinfo.Tasktype == (int)TaskType.TT_OPENEND
                        || taskinfo.Tasktype == (int)TaskType.TT_TIEUP)
                {
                    taskinfo.Tasktype = (int)TaskType.TT_NORMAL;
                }
            }
            else
            {
                if (taskinfo.OldChannelid <= 0 && taskinfo.Tasktype == (int)TaskType.TT_PERIODIC)
                {
                    SobeyRecException.ThrowSelfOneParam("StopTask match periodic", GlobalDictionary.GLOBALDICT_CODE_IN_STOPCAPTURE_TASK_IS_A_STENCIL_PLATE_TASK_ONEPARAM, Logger, taskid, null);
                }

                taskinfo.Endtime = dt;
                taskinfo.NewEndtime = taskinfo.Endtime;
                taskinfo.State = (int)taskState.tsComplete;
            }



            //Context.Attach(ltask);
            //var entry = Context.Entry(ltask);
            //entry.Property(x => x.Endtime).IsModified = true;
            //entry.Property(x => x.NewEndtime).IsModified = true;
            //entry.Property(x => x.Recunitid).IsModified = true;

            try
            {
                await Context.SaveChangesAsync();
                return taskinfo;
            }
            catch (DbUpdateException e)
            {
                throw e;
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



                Context.Attach(ltask);
                var entry = Context.Entry(ltask);
                if (ltask.Tasktype == (int)TaskType.TT_MANUTASK
                    || ltask.Tasktype == (int)TaskType.TT_OPENEND
                    || ltask.Tasktype == (int)TaskType.TT_TIEUP)
                {
                    ltask.Tasktype = (int)TaskType.TT_NORMAL;
                    entry.Property(x => x.Tasktype).IsModified = true;
                }

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
            new DbpTask
            {
                Taskid = ite.Taskid,
                Taskguid = ite.Taskguid,
                Tasktype = ite.Tasktype,
                Endtime = ite.Endtime,
                NewEndtime = ite.Endtime,
                Recunitid = ite.Recunitid,
            }
            ));

            //把里面vtr任务和普通任务,list分出来
            var filtertasks = lsttask.GroupBy<DbpTask, int, DbpTask>(x => x.Tasktype.GetValueOrDefault(), y => y, new VtrComparer());

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



                        Context.Attach(itm);
                        var entry = Context.Entry(itm);

                        if (itm.Tasktype == (int)TaskType.TT_MANUTASK
                            || itm.Tasktype == (int)TaskType.TT_OPENEND
                            || itm.Tasktype == (int)TaskType.TT_TIEUP)
                        {
                            itm.Tasktype = (int)TaskType.TT_NORMAL;
                            entry.Property(x => x.Tasktype).IsModified = true;
                        }

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
                    int tasktempId = ltask.Taskid;
                    Logger.Info("delete task", tasktempId);
                    Context.Attach(ltask);
                    Context.DbpTask.Remove(ltask);
                }
                else
                {
                    //接下来是modifytask逻辑
                    ltask.Tasklock = string.Empty;

                    Context.Attach(ltask);
                    var entry = Context.Entry(ltask);
                    entry.Property(x => x.OpType).IsModified = true;
                    entry.Property(x => x.State).IsModified = true;

                    entry.Property(x => x.DispatchState).IsModified = true;

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
                            //Logger.Info("delete task", itm.Taskid);
                            Context.Attach(itm);
                            Context.DbpTask.Remove(itm);
                        }
                        else
                        {
                            //接下来是modifytask逻辑
                            itm.Tasklock = string.Empty;

                            Context.Attach(itm);
                            var entry = Context.Entry(itm);
                            entry.Property(x => x.DispatchState).IsModified = true;
                            entry.Property(x => x.State).IsModified = true;
                            entry.Property(x => x.OpType).IsModified = true;
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



        public async Task<List<int>> GetFreeChannels(List<int> lst, int nTaskID, int backupVtrid, DateTime begin, DateTime end, bool choosefilter = false)
        {
            /*
             * @brief 老代码有个endtime > datetime.now, 我不明白为啥，应该没有道理的，我这里先屏蔽了看看
             */
            DateTime dtnow = DateTime.Now;
            var lsttask = await Context.DbpTask.AsNoTracking().Where(x =>
                                                                        (((x.Starttime >= begin && x.Starttime < end)
                                                                        || (x.Endtime > begin && x.Endtime <= end) || (x.Starttime < begin && x.Endtime > end))
                                                                        && (x.State != (int)taskState.tsConflict && x.State != (int)taskState.tsDelete && x.State != (int)taskState.tsInvaild)
                                                                        && x.DispatchState != (int)dispatchState.dpsInvalid
                                                                        && x.OpType != (int)opType.otDel
                                                                        && (x.Tasktype != (int)TaskType.TT_PERIODIC && x.Tasktype != (int)TaskType.TT_OPENEND && x.Tasktype != (int)TaskType.TT_OPENENDEX)) || (x.Tasktype == (int)TaskType.TT_MANUTASK && x.State == (int)taskState.tsExecuting && x.Starttime < begin && dtnow > begin))//禁止手动任务范围修改任务
                                                                        .ToListAsync();

            ConfictTaskInfo = string.Empty;

            List<DbpTask> lstfiltertask = new List<DbpTask>();
            /*
             * @brief 这是ChooseUseableChannels里面的，加了层过滤，估计以后要用到
             */
            if (choosefilter)
            {
                foreach (var item in lsttask)
                {
                    bool isNeedRemove = false;

                    // 手动任务在执行，那么也不能调度过去
                    //yangchuang20130122手动任务在执行,手动任务如果不是执行状态，那么肯定是出问题了，
                    //OpenEnd/OpenEndEx有可能刚添加进去还没有执行，这个时候也不能占用通道
                    if ((item.State == (int)taskState.tsManuexecuting || item.State == (int)taskState.tsExecuting)
                            && (item.Tasktype == (int)TaskType.TT_OPENEND || item.Tasktype == (int)TaskType.TT_OPENENDEX // Add by chenzhi 2012-07-25
                            || item.Tasktype == (int)TaskType.TT_MANUTASK))
                    {
                        // ApplicationLog.WriteInfo("There is a executing manutask or openend task");
                        isNeedRemove = true;
                    }
                    else if ((item.Tasktype == (int)TaskType.TT_OPENEND || item.Tasktype == (int)TaskType.TT_OPENENDEX)
                        && item.State == (int)taskState.tsReady)
                    {
                        //在一定时间内认为这个这个任务即将被开始，要是都过了一段时间了，那这个任务就不用管了
                        DateTime dtNeedJudgeBegin = item.Starttime;
                        if (dtNeedJudgeBegin.AddSeconds(10) > DateTime.Now)
                        {
                            isNeedRemove = true;
                        }
                    }
                    else if (item.Tasktype == (int)TaskType.TT_PERIODIC || item.State == (int)taskState.tsExecuting)
                    {
                        //ApplicationLog.WriteInfo(String.Format("Begin to Judge taks start:{0}, end:{1}", needJudgeTask.strBegin, needJudgeTask.strEnd));
                        DateTime dtNeedJudgeBegin = item.Starttime;//.AddSeconds(-1);
                        DateTime dtNeedJudgeEnd = item.Endtime;//.AddSeconds(1);

                        //ApplicationLog.WriteInfo(String.Format("After change time, begin:{0}, end:{1}", GlobalFun.DateTimeToString(dtNeedJudgeBegin), GlobalFun.DateTimeToString(dtNeedJudgeEnd)));

                        //zmj2009-07-28增加一个对正在执行的KAMATAKI任务的判断,由于KAMATAKI任务的特殊性,属于普通任务,时间又跟手动任务一样
                        if (item.Tasktype != (int)TaskType.TT_PERIODIC)
                        {
                            //if (dtNeedJudgeBegin.AddSeconds(1) == dtNeedJudgeEnd.AddSeconds(-1))//判断开始时间和结束时间是否一致,如果一致,说明是一个不定长的任务
                            if (dtNeedJudgeBegin == dtNeedJudgeEnd)
                            {
                                //不能肯定KAMATAKI任务是在什么时候结束的,但是一个任务最长的时间就是24小时
                                dtNeedJudgeEnd = dtNeedJudgeBegin.AddDays(1);
                            }
                            else if (dtNeedJudgeEnd < DateTime.Now)//流媒体任务如果结束时间都已经过了，但是还是采集状态的，当成openend任务处理，不能占用
                            {
                                TaskSource srcType = TaskSource.emUnknowTask;

                                await GetTaskSourceAsync(a => a.Where(b => b.Taskid == item.Taskid), true);
                                if (TaskSource.emStreamMediaUploadTask == srcType)
                                {
                                    dtNeedJudgeEnd = DateTime.Now.AddSeconds(5);//每次都加5秒作为估计要结束的时间，这样在重调度判断时间冲突的时候错开当前通道
                                }
                            }
                        }

                        //yangchuang20111010这个地方怎么能直接递减啦！！！！！！！！
                        //dtBegin = dtBegin.AddSeconds(-1);
                        //dtEnd = dtEnd.AddSeconds(1);
                        /////////////////////////////////////////////////////////////////

                        //zmj2008-10-21解决没有考虑到相等时间的冲突
                        if (IsConflict(dtNeedJudgeBegin, dtNeedJudgeEnd, begin, end))
                        {
                            isNeedRemove = true;
                        }
                    }
                    else
                    {
                        if (IsConflict(item.Starttime, item.Endtime, begin, end))
                        {
                            isNeedRemove = true;
                        }
                    }

                    if (isNeedRemove)
                    {
                        lstfiltertask.Add(item);
                    }
                }
                lsttask = lstfiltertask;
            }

            if (nTaskID > 0)
            {
                lsttask.RemoveAll(x => x.Taskid == nTaskID);
            }

            var lstbackvtr = lsttask.Select(x => x.Backupvtrid).ToList();
            if (backupVtrid > 0 && lstbackvtr.Count > 0)
            {
                bool bfind = false;
                foreach (var item in lstbackvtr)
                {
                    if (item == backupVtrid)
                    {
                        bfind = true;
                        break;
                    }
                }

                if (bfind)
                {
                    throw new Exception(" backup VTR has been used by other task.");
                }
            }

            var lstchn = lsttask.Select(x => x.Channelid).ToList();

            ConfictTaskInfo += "conficttask normal " + string.Join(",", lstchn);
            Logger.Info(ConfictTaskInfo);

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
                && (x.Category.Contains($"W{preweek}+") || x.Category.Contains($"W{week}+"))
                || x.Category.Contains($"M{preday}+") || x.Category.Contains($"M{day}+")
                || x.Category.Contains($"D"))
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
                && (x.Category.Contains($"W{week}+"))
                || x.Category.Contains($"M{day}+")
                || x.Category.Contains($"D"))
                && x.OpType != (int)opType.otDel
                && x.Tasktype == (int)TaskType.TT_PERIODIC
                && x.DispatchState != (int)dispatchState.dpsInvalid
                ).ToListAsync();

            }

            List<DbpTask> filtertask = new List<DbpTask>();
            foreach (var item in lstperiod)
            {
                bool isAdd2 = false;

                var fakeTask = FixPeroidcTaskTimeDisplay(item, overday ? begin : end, TimeLineType.em24HourDay, ref isAdd2);

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

            if (filtertask != null && filtertask.Count > 0)
            {
                filtertask.RemoveAll(x => (!IsConflict(x.Starttime, x.Endtime, begin, end)) || (nTaskID > 0 && x.Taskid == nTaskID));

                if (filtertask.Count > 0)
                {
                    string info = "GetFreeChannels period conficttask " + string.Join(",", filtertask.Select(y => y.Taskid).ToList());
                    Logger.Info(info);

                    ConfictTaskInfo += info;

                    var periodch = filtertask.Select(x => x.Channelid).ToList();
                    lst.RemoveAll(z => periodch.Contains(z));
                }
            }

            if (lstchn != null && lstchn.Count > 0)
            {
                lst.RemoveAll(z => lstchn.Contains(z));
            }

            return lst;
        }

        private bool IsConflict(DateTime dtNeedJudgeBegin, DateTime dtNeedJudgeEnd, DateTime dtBegin, DateTime dtEnd)
        {
            //if ((dtNeedJudgeEnd >= dtBegin && dtNeedJudgeEnd <= dtEnd)
            //    || (dtNeedJudgeBegin <= dtBegin && dtNeedJudgeEnd >= dtEnd)
            //    || (dtNeedJudgeBegin >= dtBegin && dtNeedJudgeEnd <= dtEnd)
            //    || (dtNeedJudgeBegin >= dtBegin && dtNeedJudgeBegin <= dtEnd))
            //{
            //    return true;
            //}
            if ((dtNeedJudgeEnd > dtBegin && dtNeedJudgeEnd <= dtEnd)
                || (dtNeedJudgeBegin <= dtBegin && dtNeedJudgeEnd >= dtEnd)
                || (dtNeedJudgeBegin >= dtBegin && dtNeedJudgeEnd <= dtEnd)
                || (dtNeedJudgeBegin >= dtBegin && dtNeedJudgeBegin < dtEnd))
            {
                return true;
            }
            return false;
        }

        public async Task<List<DbpTask>> GetTaskListWithMode(int cut, DateTime day, TimeLineType timetype)
        {
            DateTime dtDay = day;

            int Week = (int)dtDay.DayOfWeek;
            int MDay = dtDay.Day;
            int ProvWeek = (int)dtDay.AddDays(-1).DayOfWeek;
            int ProvMDay = dtDay.AddDays(-1).Day;

            //加入后一天的周期任务
            int NextWeekly = (int)dtDay.AddDays(1).DayOfWeek;
            int NextMonthDay = dtDay.AddDays(1).Day;

            //先把前一天的周期任务选出来，避免漏掉跨天任务，然后在外面过滤
            DateTime dtDayBegin = new DateTime(dtDay.Year, dtDay.Month, dtDay.Day, 0, 0, 0);
            DateTime dtDayEnd = new DateTime(dtDay.Year, dtDay.Month, dtDay.Day, 23, 59, 59);

            DateTime subDays = DateTime.Now.AddDays(-1);
            DateTime addDyas = DateTime.Now.AddSeconds(5);

            bool queryexturingtask = true;

            List<DbpTask> lst = null;
            List<DbpTask> retlst = new List<DbpTask>();
            switch (timetype)
            {
                case TimeLineType.em24HourDay:
                    {
                        if (addDyas < dtDayBegin)//今天查明天日期任务
                        {
                            queryexturingtask = false;
                        }

                        IQueryable<DbpTask> onelst = null;
                        if (queryexturingtask)
                        {
                            onelst = Context.DbpTask.AsNoTracking().Where(a =>
                            (((a.Starttime >= dtDayBegin && a.Starttime <= dtDayEnd)
                            || (a.Endtime >= dtDayBegin && a.Endtime <= dtDayEnd)
                            || (a.Starttime <= dtDayBegin && a.Endtime >= dtDayEnd))
                            && (a.Category.Contains($"W{Week}+") || a.Category.Contains($"W{ProvWeek}+")
                                || a.Category.Contains($"M{MDay}+") || a.Category.Contains($"M{ProvMDay}+")
                                || a.Category.Contains($"D") || a.Category.Contains("A"))
                            && (a.DispatchState == (int)dispatchState.dpsNotDispatch || a.DispatchState == (int)dispatchState.dpsDispatched || a.DispatchState == (int)dispatchState.dpsInvalid || a.DispatchState == (int)dispatchState.dpsRedispatch)
                            && (a.State == (int)taskState.tsReady || a.State == (int)taskState.tsComplete || a.State == (int)taskState.tsPause || a.State == (int)taskState.tsInvaild || a.State == (int)taskState.tsExecuting))
                            || (a.Tasktype == (int)TaskType.TT_MANUTASK && a.State == (int)taskState.tsExecuting && a.Starttime <= addDyas && a.Starttime <= dtDayEnd)
                            /*
                             * @breif 老版本会对手动任务，open任务，tsExecuting附加上，不明白为啥，直接全部返回，我这里
                             */
                            );
                        }
                        else
                        {
                            onelst = Context.DbpTask.AsNoTracking().Where(a =>
                            (((a.Starttime >= dtDayBegin && a.Starttime <= dtDayEnd)
                            || (a.Endtime >= dtDayBegin && a.Endtime <= dtDayEnd)
                            || (a.Starttime <= dtDayBegin && a.Endtime >= dtDayEnd))
                            && (a.Category.Contains($"W{Week}+") || a.Category.Contains($"W{ProvWeek}+")
                                || a.Category.Contains($"M{MDay}+") || a.Category.Contains($"M{ProvMDay}+")
                                || a.Category.Contains($"D") || a.Category.Contains("A"))
                            && (a.DispatchState == (int)dispatchState.dpsNotDispatch || a.DispatchState == (int)dispatchState.dpsDispatched || a.DispatchState == (int)dispatchState.dpsInvalid || a.DispatchState == (int)dispatchState.dpsRedispatch)
                            && (a.State == (int)taskState.tsReady || a.State == (int)taskState.tsComplete || a.State == (int)taskState.tsPause || a.State == (int)taskState.tsInvaild || a.State == (int)taskState.tsExecuting))
                            /*
                             * @breif 老版本会对手动任务，open任务，tsExecuting附加上，不明白为啥，直接全部返回，我这里
                             */
                            );
                        }


                        if (ApplicationContext.Current.Limit24Hours)
                        {
                            onelst = onelst.Where(a => a.Starttime >= subDays);
                        }

                        lst = await onelst.ToListAsync();
                        if (lst == null || lst.Count <= 0)
                        {
                            IQueryable<DbpTask> twolst = null;
                            if (queryexturingtask)
                            {
                                twolst = Context.DbpTaskBackup.AsNoTracking().Where(a =>
                                   (((a.Starttime >= dtDayBegin && a.Starttime <= dtDayEnd)
                                   || (a.Endtime >= dtDayBegin && a.Endtime <= dtDayEnd)
                                   || (a.Starttime <= dtDayBegin && a.Endtime >= dtDayEnd))
                                   && (a.Category.Contains($"W{Week}+") || a.Category.Contains($"W{ProvWeek}+")
                                       || a.Category.Contains($"M{MDay}+") || a.Category.Contains($"M{ProvMDay}+")
                                       || a.Category.Contains($"D") || a.Category.Contains("A"))
                                   && (a.DispatchState == (int)dispatchState.dpsNotDispatch || a.DispatchState == (int)dispatchState.dpsDispatched || a.DispatchState == (int)dispatchState.dpsInvalid || a.DispatchState == (int)dispatchState.dpsRedispatch)
                                   && (a.State == (int)taskState.tsReady || a.State == (int)taskState.tsComplete || a.State == (int)taskState.tsPause || a.State == (int)taskState.tsInvaild || a.State == (int)taskState.tsExecuting))
                                   || (a.Tasktype == (int)TaskType.TT_MANUTASK && a.State == (int)taskState.tsExecuting && a.Starttime <= addDyas && a.Starttime <= dtDayEnd)
                               /*
                                * @breif 老版本会对手动任务，open任务，tsExecuting附加上，不明白为啥，直接全部返回，我这里
                                */
                               ).Select(x => new DbpTask
                               {
                                   Taskid = x.Taskid,
                                   Taskname = x.Taskname,
                                   Recunitid = x.Recunitid,
                                   Usercode = x.Usercode,
                                   Signalid = x.Signalid,
                                   Channelid = x.Channelid,
                                   OldChannelid = x.OldChannelid,
                                   State = x.State,
                                   Starttime = x.Starttime,
                                   Endtime = x.Endtime,
                                   NewBegintime = x.NewBegintime,
                                   NewEndtime = x.NewEndtime,
                                   Category = x.Category,
                                   Description = x.Description,
                                   Tasktype = x.Tasktype,
                                   Backtype = x.Backtype,
                                   DispatchState = x.DispatchState,
                                   SyncState = x.SyncState,
                                   OpType = x.OpType,
                                   Tasklock = x.Tasklock,
                                   Taskguid = x.Taskguid,
                                   Backupvtrid = x.Backupvtrid,
                                   Taskpriority = x.Taskpriority,
                                   Stamptitleindex = x.Stamptitleindex,
                                   Stampimagetype = x.Stampimagetype
                               });
                            }
                            else
                            {
                                twolst = Context.DbpTaskBackup.AsNoTracking().Where(a =>
                                   (((a.Starttime >= dtDayBegin && a.Starttime <= dtDayEnd)
                                   || (a.Endtime >= dtDayBegin && a.Endtime <= dtDayEnd)
                                   || (a.Starttime <= dtDayBegin && a.Endtime >= dtDayEnd))
                                   && (a.Category.Contains($"W{Week}+") || a.Category.Contains($"W{ProvWeek}+")
                                       || a.Category.Contains($"M{MDay}+") || a.Category.Contains($"M{ProvMDay}+")
                                       || a.Category.Contains($"D") || a.Category.Contains("A"))
                                   && (a.DispatchState == (int)dispatchState.dpsNotDispatch || a.DispatchState == (int)dispatchState.dpsDispatched || a.DispatchState == (int)dispatchState.dpsInvalid || a.DispatchState == (int)dispatchState.dpsRedispatch)
                                   && (a.State == (int)taskState.tsReady || a.State == (int)taskState.tsComplete || a.State == (int)taskState.tsPause || a.State == (int)taskState.tsInvaild || a.State == (int)taskState.tsExecuting))
                               /*
                                * @breif 老版本会对手动任务，open任务，tsExecuting附加上，不明白为啥，直接全部返回，我这里
                                */
                               ).Select(x => new DbpTask
                               {
                                   Taskid = x.Taskid,
                                   Taskname = x.Taskname,
                                   Recunitid = x.Recunitid,
                                   Usercode = x.Usercode,
                                   Signalid = x.Signalid,
                                   Channelid = x.Channelid,
                                   OldChannelid = x.OldChannelid,
                                   State = x.State,
                                   Starttime = x.Starttime,
                                   Endtime = x.Endtime,
                                   NewBegintime = x.NewBegintime,
                                   NewEndtime = x.NewEndtime,
                                   Category = x.Category,
                                   Description = x.Description,
                                   Tasktype = x.Tasktype,
                                   Backtype = x.Backtype,
                                   DispatchState = x.DispatchState,
                                   SyncState = x.SyncState,
                                   OpType = x.OpType,
                                   Tasklock = x.Tasklock,
                                   Taskguid = x.Taskguid,
                                   Backupvtrid = x.Backupvtrid,
                                   Taskpriority = x.Taskpriority,
                                   Stamptitleindex = x.Stamptitleindex,
                                   Stampimagetype = x.Stampimagetype
                               });
                            }

                            if (ApplicationContext.Current.Limit24Hours)
                            {
                                twolst = twolst.Where(a => a.Starttime >= subDays);
                            }
                            lst = await twolst.ToListAsync();
                        }

                        //Context.DbpTaskBackup;
                    }
                    break;
                case TimeLineType.em32HourDay:
                    {
                        dtDayBegin = new DateTime(dtDay.Year, dtDay.Month, dtDay.Day, 0, 0, 0);
                        dtDayEnd = new DateTime(dtDay.AddDays(1).Year, dtDay.AddDays(1).Month, dtDay.AddDays(1).Day, 7, 59, 59);

                        if (addDyas < dtDayBegin)//今天查明天日期任务
                        {
                            queryexturingtask = false;
                        }

                        IQueryable<DbpTask> onelst = null;

                        if (queryexturingtask)
                        {
                            onelst = Context.DbpTask.AsNoTracking().Where(a =>
                            (((a.Starttime >= dtDayBegin && a.Starttime <= dtDayEnd)
                            || (a.Endtime >= dtDayBegin && a.Endtime <= dtDayEnd)
                            || (a.Starttime <= dtDayBegin && a.Endtime >= dtDayEnd))
                            && (a.Category.Contains($"W{Week}+") || a.Category.Contains($"W{ProvWeek}+") || a.Category.Contains($"W{NextWeekly}+")
                                || a.Category.Contains($"M{MDay}+") || a.Category.Contains($"M{ProvMDay}+") || a.Category.Contains($"M{NextMonthDay}+")
                                || a.Category.Contains($"D") || a.Category.Contains("A"))
                            && (a.DispatchState == (int)dispatchState.dpsNotDispatch || a.DispatchState == (int)dispatchState.dpsDispatched || a.DispatchState == (int)dispatchState.dpsInvalid || a.DispatchState == (int)dispatchState.dpsRedispatch)
                            && (a.State == (int)taskState.tsReady || a.State == (int)taskState.tsComplete || a.State == (int)taskState.tsPause || a.State == (int)taskState.tsInvaild || a.State == (int)taskState.tsExecuting)
                            )
                            || (a.Tasktype == (int)TaskType.TT_MANUTASK && a.State == (int)taskState.tsExecuting && a.Starttime <= addDyas && a.Starttime <= dtDayEnd)
                            /*
                             * @breif 老版本会对手动任务，open任务，tsExecuting附加上，不明白为啥，直接全部返回，我这里
                             */
                            );
                        }
                        else
                        {
                            onelst = Context.DbpTask.AsNoTracking().Where(a =>
                            (((a.Starttime >= dtDayBegin && a.Starttime <= dtDayEnd)
                            || (a.Endtime >= dtDayBegin && a.Endtime <= dtDayEnd)
                            || (a.Starttime <= dtDayBegin && a.Endtime >= dtDayEnd))
                            && (a.Category.Contains($"W{Week}+") || a.Category.Contains($"W{ProvWeek}+") || a.Category.Contains($"W{NextWeekly}+")
                                || a.Category.Contains($"M{MDay}+") || a.Category.Contains($"M{ProvMDay}+") || a.Category.Contains($"M{NextMonthDay}+")
                                || a.Category.Contains($"D") || a.Category.Contains("A"))
                            && (a.DispatchState == (int)dispatchState.dpsNotDispatch || a.DispatchState == (int)dispatchState.dpsDispatched || a.DispatchState == (int)dispatchState.dpsInvalid || a.DispatchState == (int)dispatchState.dpsRedispatch)
                            && (a.State == (int)taskState.tsReady || a.State == (int)taskState.tsComplete || a.State == (int)taskState.tsPause || a.State == (int)taskState.tsInvaild || a.State == (int)taskState.tsExecuting)
                            )
                            /*
                             * @breif 老版本会对手动任务，open任务，tsExecuting附加上，不明白为啥，直接全部返回，我这里
                             */
                            );
                        }


                        if (ApplicationContext.Current.Limit24Hours)
                        {
                            onelst = onelst.Where(a => a.Starttime >= subDays);
                        }
                        lst = await onelst.ToListAsync();

                        if (lst == null || lst.Count <= 0)
                        {

                            IQueryable<DbpTask> twolst = null;
                            if (queryexturingtask)
                            {
                                twolst = Context.DbpTaskBackup.AsNoTracking().Where(a =>
                                   (((a.Starttime >= dtDayBegin && a.Starttime <= dtDayEnd)
                                   || (a.Endtime >= dtDayBegin && a.Endtime <= dtDayEnd)
                                   || (a.Starttime <= dtDayBegin && a.Endtime >= dtDayEnd))
                                   && (a.Category.Contains($"W{Week}+") || a.Category.Contains($"W{ProvWeek}+") || a.Category.Contains($"W{NextWeekly}+")
                                       || a.Category.Contains($"M{MDay}+") || a.Category.Contains($"M{ProvMDay}+") || a.Category.Contains($"M{NextMonthDay}+")
                                       || a.Category.Contains($"D") || a.Category.Contains("A"))
                                   && (a.DispatchState == (int)dispatchState.dpsNotDispatch || a.DispatchState == (int)dispatchState.dpsDispatched || a.DispatchState == (int)dispatchState.dpsInvalid || a.DispatchState == (int)dispatchState.dpsRedispatch)
                                   && (a.State == (int)taskState.tsReady || a.State == (int)taskState.tsComplete || a.State == (int)taskState.tsPause || a.State == (int)taskState.tsInvaild || a.State == (int)taskState.tsExecuting))
                                   || (a.Tasktype == (int)TaskType.TT_MANUTASK && a.State == (int)taskState.tsExecuting && a.Starttime <= addDyas && a.Starttime <= dtDayEnd)
                               /*
                                * @breif 老版本会对手动任务，open任务，tsExecuting附加上，不明白为啥，直接全部返回，我这里
                                */
                               ).Select(x => new DbpTask
                               {
                                   Taskid = x.Taskid,
                                   Taskname = x.Taskname,
                                   Recunitid = x.Recunitid,
                                   Usercode = x.Usercode,
                                   Signalid = x.Signalid,
                                   Channelid = x.Channelid,
                                   OldChannelid = x.OldChannelid,
                                   State = x.State,
                                   Starttime = x.Starttime,
                                   Endtime = x.Endtime,
                                   NewBegintime = x.NewBegintime,
                                   NewEndtime = x.NewEndtime,
                                   Category = x.Category,
                                   Description = x.Description,
                                   Tasktype = x.Tasktype,
                                   Backtype = x.Backtype,
                                   DispatchState = x.DispatchState,
                                   SyncState = x.SyncState,
                                   OpType = x.OpType,
                                   Tasklock = x.Tasklock,
                                   Taskguid = x.Taskguid,
                                   Backupvtrid = x.Backupvtrid,
                                   Taskpriority = x.Taskpriority,
                                   Stamptitleindex = x.Stamptitleindex,
                                   Stampimagetype = x.Stampimagetype
                               });
                            }
                            else
                            {
                                twolst = Context.DbpTaskBackup.AsNoTracking().Where(a =>
                                   (((a.Starttime >= dtDayBegin && a.Starttime <= dtDayEnd)
                                   || (a.Endtime >= dtDayBegin && a.Endtime <= dtDayEnd)
                                   || (a.Starttime <= dtDayBegin && a.Endtime >= dtDayEnd))
                                   && (a.Category.Contains($"W{Week}+") || a.Category.Contains($"W{ProvWeek}+") || a.Category.Contains($"W{NextWeekly}+")
                                       || a.Category.Contains($"M{MDay}+") || a.Category.Contains($"M{ProvMDay}+") || a.Category.Contains($"M{NextMonthDay}+")
                                       || a.Category.Contains($"D") || a.Category.Contains("A"))
                                   && (a.DispatchState == (int)dispatchState.dpsNotDispatch || a.DispatchState == (int)dispatchState.dpsDispatched || a.DispatchState == (int)dispatchState.dpsInvalid || a.DispatchState == (int)dispatchState.dpsRedispatch)
                                   && (a.State == (int)taskState.tsReady || a.State == (int)taskState.tsComplete || a.State == (int)taskState.tsPause || a.State == (int)taskState.tsInvaild || a.State == (int)taskState.tsExecuting))
                               /*
                                * @breif 老版本会对手动任务，open任务，tsExecuting附加上，不明白为啥，直接全部返回，我这里
                                */
                               ).Select(x => new DbpTask
                               {
                                   Taskid = x.Taskid,
                                   Taskname = x.Taskname,
                                   Recunitid = x.Recunitid,
                                   Usercode = x.Usercode,
                                   Signalid = x.Signalid,
                                   Channelid = x.Channelid,
                                   OldChannelid = x.OldChannelid,
                                   State = x.State,
                                   Starttime = x.Starttime,
                                   Endtime = x.Endtime,
                                   NewBegintime = x.NewBegintime,
                                   NewEndtime = x.NewEndtime,
                                   Category = x.Category,
                                   Description = x.Description,
                                   Tasktype = x.Tasktype,
                                   Backtype = x.Backtype,
                                   DispatchState = x.DispatchState,
                                   SyncState = x.SyncState,
                                   OpType = x.OpType,
                                   Tasklock = x.Tasklock,
                                   Taskguid = x.Taskguid,
                                   Backupvtrid = x.Backupvtrid,
                                   Taskpriority = x.Taskpriority,
                                   Stamptitleindex = x.Stamptitleindex,
                                   Stampimagetype = x.Stampimagetype
                               });
                            }


                            if (ApplicationContext.Current.Limit24Hours)
                            {
                                twolst = twolst.Where(a => a.Starttime >= subDays);
                            }
                            lst = await twolst.ToListAsync();

                            //lst = backlst.ToList();
                        }
                    }
                    break;
                case TimeLineType.em36HourDay:
                case TimeLineType.em48HourDay:
                default:
                    break;
            }

            if (lst != null && lst.Count > 0)
            {
                foreach (var item in lst)
                {
                    //Logger.Info("GetTaskListWithMode task {0} {1} {2} {3}", item.Taskid, item.Tasktype, item.Starttime, item.Endtime);
                    if (item.DispatchState == (int)dispatchState.dpsInvalid)
                    {
                        item.State = (int)taskState.tsDelete;
                    }

                    if (item.Tasktype == (int)TaskType.TT_PERIODIC && item.OldChannelid == 0)
                    {
                        //排除上一天、并且没有跨天的周期任务
                        DateTime DBTaskDateTimeBegin = item.Starttime;
                        DateTime DBTaskDateTimeEnd = item.Endtime;
                        if (DBTaskDateTimeBegin.TimeOfDay <= DBTaskDateTimeEnd.TimeOfDay) //没有跨天，加上等于情况，避免意外
                        {
                            string strDayOfWeek = "W" + Week + "+";
                            string strDayOfMonth = "M" + Convert.ToString(MDay) + "+";

                            if (item.Category.IndexOf(strDayOfWeek) == -1
                                && item.Category.IndexOf(strDayOfMonth) == -1
                                && item.Category.IndexOf("D") == -1)
                            {
                                if (timetype == TimeLineType.em32HourDay && DBTaskDateTimeBegin.TimeOfDay >= new DateTime(1, 1, 1, 0, 0, 0).TimeOfDay && DBTaskDateTimeBegin.TimeOfDay < new DateTime(1, 1, 1, 8, 0, 0).TimeOfDay)
                                {
                                    Week = (int)day.AddDays(1).DayOfWeek;
                                    MDay = day.AddDays(1).Day;
                                    strDayOfWeek = "W" + Convert.ToString(Week) + "+";
                                    strDayOfMonth = "M" + Convert.ToString(MDay) + "+";
                                    if (item.Category.IndexOf(strDayOfWeek) == -1
                                        && item.Category.IndexOf(strDayOfMonth) == -1
                                        && item.Category.IndexOf("D") == -1)
                                        continue;
                                }
                                else
                                    continue;
                            }
                        }
                        bool isAdd2 = false;
                        var fakeTask = FixPeroidcTaskTimeDisplay(item, day, timetype, ref isAdd2);

                        //排除例外的任务
                        if (IsInvalidPerodicTask(item.Category, item.Starttime))
                            continue;

                        if (timetype == TimeLineType.em24HourDay)
                        {
                            //排除不是在这一天的任务
                            if (day.Date != item.Starttime.Date && day.Date != item.Endtime.Date)
                            {
                                continue;
                            }
                        }
                        else
                        {
                            //排除32小时之外的任务
                            DateTime tm32Begin = day.Date;
                            DateTime tm32End = day.Date.AddDays(1).AddHours(7).AddMinutes(59).AddSeconds(59);
                            if (item.Endtime < tm32Begin || item.Starttime > tm32End)
                            {
                                continue;
                            }
                        }
                        if (fakeTask != null && cut != 2)
                        {
                            if (fakeTask.State == (int)taskState.tsDelete)
                                continue;

                            retlst.Add(fakeTask);

                            if (!isAdd2)
                                continue;
                        }

                    }

                    #region 外部用的饿，这里不会用
                    if (cut > 0)
                    {
                        if (item.Tasktype == (int)TaskType.TT_MANUTASK && item.State == (int)taskState.tsExecuting)
                        {
                            item.Endtime = DateTime.Now;
                        }

                        if (item.Tasktype == (int)TaskType.TT_OPENEND && item.State == (int)taskState.tsExecuting)
                        {
                            item.Endtime = DateTime.Now;
                        }

                        // Add by chenzhi 2012-07-25
                        if (item.Tasktype == (int)TaskType.TT_OPENENDEX && item.State == (int)taskState.tsExecuting)
                        {
                            item.Endtime = DateTime.Now;
                        }

                        //如果是开始时间等于结束时间并且正在执行，是个OpenEnd的自动任务
                        if (item.Starttime == item.Endtime && item.Backtype == (int)CooperantType.emKamataki && item.State == (int)taskState.tsExecuting)
                        {
                            item.Endtime = DateTime.Now;
                        }
                    }
                    #endregion

                    retlst.Add(item);
                }
            }

            return retlst;
        }

        public string GetConfictTaskInfo()
        {
            return ConfictTaskInfo;
        }
        //需要比较和普通任务的冲突，以及和周期任务的冲突
        public async Task<List<int>> GetFreePerodiChannels(List<int> lst, int nTaskID, int nUnitID, int nSigID, int nChannelID, string Category, DateTime begin, DateTime end)
        {
            //先得到所有可用通道
            //和普通任务的冲突:1.把所有时间段上冲突的选出来 2.对比下看实际的冲突情况，过滤出真正冲突的。
            //和周期任务的冲突:1.把所有周期任务选出来 2.在内存中对比是否冲突。
            //从可用通道中去掉冲突的，就是可用的


            DateTime dtNowEnd = end;
            DateTime dtNowBegin = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, begin.Hour, begin.Minute, begin.Second);
            dtNowBegin = dtNowBegin.AddSeconds(1);
            dtNowEnd = dtNowEnd.AddSeconds(-1);

            var query = Context.DbpTask.AsNoTracking().Where(x =>
            //我也不知道老代码这些部分啥意思
            //(x.Starttime >= dtNowBegin.AddDays(-1) && x.Starttime<= dtNowEnd)
            //&& (x.Endtime >=dtNowBegin && x.Endtime <= dtNowEnd.AddDays(1))
            ((x.Starttime >= dtNowBegin && x.Starttime <= dtNowEnd)
             || (x.Endtime >= dtNowBegin && x.Endtime <= dtNowEnd) || (x.Starttime < dtNowBegin && x.Endtime > dtNowEnd))
            && (x.State != (int)taskState.tsDelete /*|| x.State == (int)taskState.tsExecuting*/)
            && (x.DispatchState == (int)dispatchState.dpsDispatched || x.DispatchState == (int)dispatchState.dpsNotDispatch));

            if (nChannelID > 0)
            {
                query = query.Where(x => x.Channelid == nChannelID);
            }

            if (nUnitID > 0)
            {
                query = query.Where(x => x.Recunitid == nUnitID);
            }

            if (nTaskID > 0)
            {
                query = query.Where(x => x.Taskid != nTaskID);
            }

            var lsttask = await query.ToListAsync();

            ConfictTaskInfo = string.Empty;

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

                        if (addflag >= 0 && compflag >= 0)
                        {
                            //zmj2008-10-10修改周期任务与周期任务之前的冲突判断
                            var addExcludeList = GetDateTimeFromString(Category);
                            var cmpExcludeList = GetDateTimeFromString(item.Category);


                            while (dtStartCheck.Date <= dtEndCheck.Date)
                            {
                                bool exita = false;
                                bool exitb = false;

                                int m = 0; //判断nAddFlag是否不为0.1.2
                                int n = 0;//判断nCmpFlag是否不为0.1.2
                                bool isExistInaddArray = addExcludeList.Any(x => dtStartCheck.Date == x.Date);
                                bool isExistIncmpArray = cmpExcludeList.Any(x => dtStartCheck.Date == x.Date);

                                if (addflag == 0 && !isExistInaddArray)
                                {
                                    exita = true;
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
                                {
                                    exitb = true;
                                    n = 1;
                                }
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

                                    DateTime dtnewitembegin = new DateTime(dtStartCheck.Year, dtStartCheck.Month, dtStartCheck.Day,
                                        item.Starttime.Hour, item.Starttime.Minute, item.Starttime.Second);
                                    DateTime dtnewitemend = new DateTime(dtStartCheck.Year, dtStartCheck.Month, dtStartCheck.Day,
                                        item.Endtime.Hour, item.Endtime.Minute, item.Endtime.Second);
                                    if (dtnewitemend < dtnewitembegin)//处理跨天任务
                                    {
                                        dtnewitemend = dtnewitemend.AddDays(1);
                                    }

                                    DateTime dtbegin = new DateTime(dtStartCheck.Year, dtStartCheck.Month, dtStartCheck.Day,
                                        begin.Hour, begin.Minute, begin.Second);
                                    DateTime dtend = new DateTime(dtStartCheck.Year, dtStartCheck.Month, dtStartCheck.Day,
                                        end.Hour, end.Minute, end.Second);
                                    if (dtend < dtbegin)
                                    {
                                        dtend = dtend.AddDays(1);
                                    }

                                    if ((dtend >= dtnewitembegin && dtend <= dtnewitemend)
                                            || (dtbegin <= dtnewitembegin && dtend >= dtnewitemend)
                                            || (dtbegin >= dtnewitembegin && dtend <= dtnewitemend)
                                            || (dtbegin >= dtnewitembegin && dtbegin <= dtnewitemend))
                                    {
                                        filterconficttasklst.Add(item);

                                        ConfictTaskInfo += $"{item.Taskid} {dtStartCheck}";
                                        break;
                                    }
                                    else
                                    {
                                        dtStartCheck = dtStartCheck.AddDays(1);
                                        //return false;
                                        if (exita && exitb)
                                        {
                                            break;
                                        }
                                        continue;
                                    }
                                }
                                try
                                {
                                    if (exita && exitb)
                                    {
                                        break;
                                    }

                                    dtStartCheck = dtStartCheck.AddDays(1);
                                }
                                catch (Exception ex)
                                {
                                    Logger.Error($"GetFreePerodiChannels error {ex.Message}");

                                    filterconficttasklst.Add(item);
                                    ConfictTaskInfo += $"{item.Taskid} {dtStartCheck}";
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
                                ConfictTaskInfo += $"{item.Taskid} ";
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

                    lst.RemoveAll(z => filterconficttasklst.Any(h => h.Channelid == z));
                }

            }
            else
                Logger.Info("GetFreePerodiChannels empty");


            return lst;
        }

        public async Task<DbpTask> ModifyTask(DbpTask task, bool bPerodic2Next, bool autoupdate, bool savechange, string CaptureMeta, string ContentMeta, string MatiralMeta, string PlanningMeta)
        {
            Logger.Info($"ModifyTask {task.Taskid} {task.State} {task.SyncState} {task.Starttime} {task.Endtime}");
            if (task.Tasktype != (int)TaskType.TT_PERIODIC)
            {
                task.Category = "A";
                task.NewBegintime = task.Starttime;
                task.NewEndtime = task.Endtime;
            }
            else if (task.OldChannelid > 0 && task.Tasktype == (int)TaskType.TT_PERIODIC)
            {
                task.NewBegintime = task.Starttime;
                task.NewEndtime = task.Endtime;
            }
            else if (task.OpType == (int)opType.otDel && task.Tasktype == (int)TaskType.TT_PERIODIC)
            {
                task.NewBegintime = task.Starttime;
                task.NewEndtime = task.Endtime;
            }
            else
            {
                DateTime NewBeginTime = task.NewBegintime;
                DateTime NewEndTime = task.NewEndtime;


                bool bIsValid = true;
                if (bPerodic2Next) //置为下一次的执行时间
                    bIsValid = SetPerodicTask2NextExectueTime(task.Starttime, task.Endtime, task.Category, ref NewBeginTime, ref NewEndTime);
                else
                    bIsValid = GetPerodicTaskNextExectueTime(task.Starttime, task.Endtime, task.Category, ref NewBeginTime, ref NewEndTime);

                if (NewBeginTime > task.Endtime)
                {
                    bIsValid = false;
                }

                Logger.Info($"ModifyTask periodic {task.Taskid} {bIsValid} {task.State} {NewBeginTime}");
                if (bIsValid)
                {
                    task.NewBegintime = NewBeginTime;
                    task.NewEndtime = NewEndTime;
                    //if (bPerodic2Next)
                    //{
                    if (bPerodic2Next)
                    {
                        task.Starttime = NewBeginTime;
                    }

                    //}
                    //else
                    //    task.Starttime = ;
                    task.Endtime = new DateTime(task.Endtime.Year, task.Endtime.Month, task.Endtime.Day, NewEndTime.Hour, NewEndTime.Minute, NewEndTime.Second);

                }
                else//无效,可以删除了
                {
                    task.State = (int)taskState.tsDelete;
                }

            }

            Logger.Info($"ModifyTask UpdateTask {task.Taskid} {task.State} {task.SyncState} {task.Starttime} {task.Endtime}");

            Context.DbpTask.Update(task);

            if (!string.IsNullOrEmpty(CaptureMeta))
            {
                //var itm = new DbpTaskMetadata() { Taskid = task.Taskid, Metadatatype = (int)MetaDataType.emCapatureMetaData, Metadatalong = CaptureMeta };
                //Context.Attach(itm);
                //Context.Entry(itm).Property(x => x.Metadatalong).IsModified = true;
                //因为主键ID默认是不赋值的，只给其他项目赋值了 id是int类型，int类型如果不允许为空那么会被默认为0，所以插入第二条数据时，数据库中已经有了主键为0的数据

                var item = Context.DbpTaskMetadata.Where(x => x.Taskid == task.Taskid && x.Metadatatype == (int)MetaDataType.emCapatureMetaData).SingleOrDefault();
                if (item != null)
                {
                    item.Metadatalong = CaptureMeta;
                }
                //Context.DbpTaskMetadata.Update(itm);
            }
            if (!string.IsNullOrEmpty(MatiralMeta))
            {
                //var itm = new DbpTaskMetadata() { Taskid = task.Taskid, Metadatatype = (int)MetaDataType.emStoreMetaData, Metadatalong = MatiralMeta };
                //Context.Attach(itm);
                //Context.Entry(itm).Property(x => x.Metadatalong).IsModified = true;

                var item = Context.DbpTaskMetadata.Where(x => x.Taskid == task.Taskid && x.Metadatatype == (int)MetaDataType.emStoreMetaData).SingleOrDefault();
                if (item != null)
                {

                    if (autoupdate)
                    {
                        var org = XDocument.Parse(item.Metadatalong);
                        var modify = XDocument.Parse(MatiralMeta);

                        foreach (var m in modify.Element("MATERIAL").Elements())
                        {
                            var f = org.Element("MATERIAL").Elements().FirstOrDefault(x => x.Name == m.Name);
                            if (f != null)
                            {
                                //f.Value = m.Value;
                                f.Remove();
                            }
                            org.Element("MATERIAL").Add(m);
                        }

                        item.Metadatalong = org.ToString();
                    }
                    else
                    {
                        item.Metadatalong = MatiralMeta;
                    }

                }
                //Context.DbpTaskMetadata.Update(itm);
            }
            if (!string.IsNullOrEmpty(ContentMeta))
            {
                //var itm = new DbpTaskMetadata() { Taskid = task.Taskid, Metadatatype = (int)MetaDataType.emContentMetaData, Metadatalong = ContentMeta };
                //Context.Attach(itm);
                //Context.Entry(itm).Property(x => x.Metadatalong).IsModified = true;

                var item = Context.DbpTaskMetadata.Where(x => x.Taskid == task.Taskid && x.Metadatatype == (int)MetaDataType.emContentMetaData).SingleOrDefault();
                if (item != null)
                {

                    if (autoupdate)
                    {
                        var org = XDocument.Parse(item.Metadatalong);
                        var modify = XDocument.Parse(ContentMeta);

                        foreach (var m in modify.Element("TaskContentMetaData").Elements())
                        {
                            var f = org.Element("TaskContentMetaData").Elements().FirstOrDefault(x => x.Name == m.Name);
                            if (f != null)
                            {
                                f.Remove();
                            }
                            org.Element("TaskContentMetaData").Add(m);
                        }

                        item.Metadatalong = org.ToString();
                    }
                    else
                    {
                        item.Metadatalong = ContentMeta;
                    }

                }
            }

            if (!string.IsNullOrEmpty(PlanningMeta))
            {
                //var itm = new DbpTaskMetadata() { Taskid = task.Taskid, Metadatatype = (int)MetaDataType.emPlanMetaData, Metadatalong = PlanningMeta };
                //Context.Attach(itm);
                //Context.Entry(itm).Property(x => x.Metadatalong).IsModified = true;

                var item = Context.DbpTaskMetadata.Where(x => x.Taskid == task.Taskid && x.Metadatatype == (int)MetaDataType.emPlanMetaData).SingleOrDefault();
                if (item != null)
                {

                    if (autoupdate)
                    {
                        var org = XDocument.Parse(item.Metadatalong);
                        var modify = XDocument.Parse(PlanningMeta);

                        foreach (var m in modify.Element("Planning").Elements())
                        {
                            var f = org.Element("Planning").Elements().FirstOrDefault(x => x.Name == m.Name);
                            if (f != null)
                            {
                                //f.Value = m.Value;
                                f.Remove();
                            }
                            org.Element("Planning").Add(m);
                        }

                        item.Metadatalong = org.ToString();
                    }
                    else
                    {
                        item.Metadatalong = PlanningMeta;
                    }

                }
            }

            if (savechange)
            {
                try
                {
                    await Context.SaveChangesAsync();
                    return task;
                }
                catch (DbUpdateException e)
                {
                    throw e;
                }
            }

            return task;
        }

        public async Task<DbpTask> AddTaskWithPolicys(DbpTask task, bool bAddForInDB, TaskSource taskSrc,
            string CaptureMeta, string ContentMeta, string MatiralMeta, string PlanningMeta, int[] arrPolicys)
        {
            if (task.Taskid <= 0)
            {
                task.Taskid = GetNextValId("DBP_SQ_TASKID");
            }
            if (task.Tasktype != (int)TaskType.TT_PERIODIC)
            {
                task.Category = "A";
                task.NewBegintime = task.Starttime;
                task.NewEndtime = task.Endtime;
            }
            else
            {
                DateTime NewBeginTime = task.NewBegintime;
                DateTime NewEndTime = task.NewEndtime;
                Logger.Info("AddTask GetPerodicTaskNextExectueTime ID:{0},NewBeginTime:{1}, taskcontent.begin:{2}", task.Taskid, NewBeginTime, task.Starttime);

                bool bIsValid = GetPerodicTaskNextExectueTime(task.Starttime,
                    task.Endtime, task.Category, ref NewBeginTime, ref NewEndTime);
                if (bIsValid)
                {
                    task.NewBegintime = NewBeginTime;
                    task.NewEndtime = NewEndTime;
                }
                else
                {
                    task.NewBegintime = DateTime.MinValue;
                    task.NewEndtime = DateTime.MinValue;
                }
            }

            if (string.IsNullOrEmpty(task.Taskguid))
            {
                task.Taskguid = Guid.NewGuid().ToString("N");
            }

            Logger.Info(" AddTaskWithPolicys add source and policy " + task.Taskid);
            if (bAddForInDB)
            {
                await Context.DbpTaskSource.AddAsync(new DbpTaskSource() { Taskid = task.Taskid, Tasksource = (int)taskSrc });

                //目前只有一种入库策略，何必再写，全部省略

                //MetaDataPolicy[] PolicyList = PPLICYACCESS.GetPolicyByUserID(taskInfo.taskContent.strUserCode);
                //foreach (MetaDataPolicy policy in PolicyList)
                //{
                //PPLICYACCESS.AddPolicyTask(policy.nID, taskInfo.taskContent.nTaskID);
                await Context.DbpPolicytask.AddAsync(new DbpPolicytask() { Policyid = 1, Taskid = task.Taskid });
                //}
            }
            else
            {
                if (!await Context.DbpTaskSource.AsNoTracking().AnyAsync(a => a.Taskid == task.Taskid))
                {
                    await Context.DbpTaskSource.AddAsync(new DbpTaskSource() { Taskid = task.Taskid, Tasksource = (int)taskSrc });
                }

                if (!await Context.DbpPolicytask.AsNoTracking().AnyAsync(b => b.Taskid == task.Taskid))
                {
                    await Context.DbpPolicytask.AddAsync(new DbpPolicytask() { Policyid = 1, Taskid = task.Taskid });
                }
            }

            Logger.Info(" AddTaskWithPolicys add task " + task.Taskid);
            Context.DbpTask.Add(task);

            if (!string.IsNullOrEmpty(ContentMeta))
            {
                Context.DbpTaskMetadata.Add(new DbpTaskMetadata() { Taskid = task.Taskid, Metadatatype = (int)MetaDataType.emContentMetaData, Metadatalong = ContentMeta });
            }
            if (!string.IsNullOrEmpty(MatiralMeta))
            {
                Context.DbpTaskMetadata.Add(new DbpTaskMetadata() { Taskid = task.Taskid, Metadatatype = (int)MetaDataType.emStoreMetaData, Metadatalong = MatiralMeta });
            }
            if (!string.IsNullOrEmpty(PlanningMeta))
            {
                Context.DbpTaskMetadata.Add(new DbpTaskMetadata() { Taskid = task.Taskid, Metadatatype = (int)MetaDataType.emPlanMetaData, Metadatalong = PlanningMeta });
            }
            if (!string.IsNullOrEmpty(CaptureMeta))
            {
                Context.DbpTaskMetadata.Add(new DbpTaskMetadata() { Taskid = task.Taskid, Metadatatype = (int)MetaDataType.emCapatureMetaData, Metadatalong = CaptureMeta });
            }

            try
            {
                await Context.SaveChangesAsync();
                return task;
            }
            catch (DbUpdateException e)
            {
                throw e;
            }
            return null;
        }

        public async Task SetTaskClassify(int taskid, string taskclassify, bool change)
        {
            var task = await GetTaskAsync(a => a.Where(b => b.Taskid == taskid));
            task.Category = taskclassify;

            if (change)
            {
                await Context.SaveChangesAsync();
            }

        }
        private bool SetPerodicTask2NextExectueTime(DateTime tmBegin, DateTime tmEnd, string strPerodicDesc, ref DateTime tmExecuteBegin, ref DateTime tmExecuteEnd)
        {
            DateTime dtNow = DateTime.Now;

            bool bOverDay = false;
            if (tmBegin.TimeOfDay > tmEnd.TimeOfDay)
                bOverDay = true;//跨天

            DateTime dtCountBegin = dtNow;
            if (tmBegin.Date > dtNow.Date)
            {
                dtCountBegin = tmBegin;
            }
            PerodicHandle2(strPerodicDesc, tmEnd, tmBegin, dtCountBegin, bOverDay, ref tmExecuteBegin, ref tmExecuteEnd);

            DateTime dtTmpCount = dtCountBegin; ;
            //如果出来的这天已经被标记无效了，那么就推到再下一次执行
            while (IsInvalidPerodicTask(strPerodicDesc, tmExecuteBegin))
            {
                dtTmpCount = dtTmpCount.AddDays(1);
                DateTime tmpExecBegin = tmExecuteBegin;
                PerodicHandle2(strPerodicDesc, tmEnd, tmBegin, dtTmpCount, bOverDay, ref tmExecuteBegin, ref tmExecuteEnd);
                if (tmpExecBegin.Date >= tmExecuteBegin.Date) //算法有问题，怎么会这一次的还大于等于下一次的执行日期呢？
                {
                    return false;
                }
                if (tmExecuteBegin.Date > tmEnd.Date) //已经过期了!
                {
                    return false;
                }
            }

            return true;
        }

        public bool GetPerodicTaskNextExectueTime(DateTime tmBegin, DateTime tmEnd, string strPerodicDesc, ref DateTime tmExecuteBegin, ref DateTime tmExecuteEnd)
        {

            DateTime dtNow = DateTime.Now;

            bool bOverDay = false;
            if (tmBegin.TimeOfDay > tmEnd.TimeOfDay)
                bOverDay = true;//跨天

            DateTime dtCountBegin = dtNow;
            if (tmBegin.Date > dtNow.Date)
            {
                dtCountBegin = tmBegin;
            }
            PerodicHandle(strPerodicDesc, tmEnd, tmBegin, dtCountBegin, bOverDay, ref tmExecuteBegin, ref tmExecuteEnd);

            DateTime dtTmpCount = dtCountBegin;
            //如果出来的这天已经被标记无效了，那么就推到再下一次执行
            while (IsInvalidPerodicTask(strPerodicDesc, tmExecuteBegin))
            {
                dtTmpCount = dtTmpCount.AddDays(1);
                DateTime tmpExecBegin = tmExecuteBegin;
                PerodicHandle(strPerodicDesc, tmEnd, tmBegin, dtTmpCount, bOverDay, ref tmExecuteBegin, ref tmExecuteEnd);
                if (tmpExecBegin.Date >= tmExecuteBegin.Date) //算法有问题，怎么会这一次的还大于等于下一次的执行日期呢？
                {
                    return false;
                }
                if (tmExecuteBegin.Date > tmEnd.Date) //已经过期了!
                {
                    return false;
                }
            }

            return true;
        }
        private void PerodicHandle(string strPerodicDesc, DateTime tmEnd, DateTime tmBegin, DateTime dtCountBegin, bool bOverDay, ref DateTime tmExecuteBegin, ref DateTime tmExecuteEnd)
        {
            if (strPerodicDesc.IndexOf("D") >= 0) //Daily
            {
                if (tmEnd.TimeOfDay > dtCountBegin.TimeOfDay) //当天任务还没结束
                {
                    if (bOverDay)
                    {
                        int timeDiff = DateTime.Now.DayOfWeek - tmBegin.DayOfWeek;
                        if (timeDiff >= 0)//zmj2008-11-06修改两天的周期任务，分任务时出现了错误
                        {
                            tmExecuteBegin = new DateTime(dtCountBegin.Year, dtCountBegin.Month, dtCountBegin.Day, tmBegin.Hour, tmBegin.Minute, tmBegin.Second);
                            tmExecuteEnd = new DateTime(dtCountBegin.AddDays(1).Year, dtCountBegin.AddDays(1).Month, dtCountBegin.AddDays(1).Day, tmEnd.Hour, tmEnd.Minute, tmEnd.Second);

                        }
                        else
                        {
                            tmExecuteBegin = new DateTime(dtCountBegin.AddDays(-1).Year, dtCountBegin.AddDays(-1).Month, dtCountBegin.AddDays(-1).Day, tmBegin.Hour, tmBegin.Minute, tmBegin.Second);
                            tmExecuteEnd = new DateTime(dtCountBegin.Year, dtCountBegin.Month, dtCountBegin.Day, tmEnd.Hour, tmEnd.Minute, tmEnd.Second);

                        }
                    }
                    else
                    {
                        tmExecuteBegin = new DateTime(dtCountBegin.Year, dtCountBegin.Month, dtCountBegin.Day, tmBegin.Hour, tmBegin.Minute, tmBegin.Second);
                        tmExecuteEnd = new DateTime(dtCountBegin.Year, dtCountBegin.Month, dtCountBegin.Day, tmEnd.Hour, tmEnd.Minute, tmEnd.Second);
                    }

                }
                else
                {
                    if (bOverDay)
                    {
                        tmExecuteBegin = new DateTime(dtCountBegin.Year, dtCountBegin.Month, dtCountBegin.Day, tmBegin.Hour, tmBegin.Minute, tmBegin.Second);
                        tmExecuteEnd = new DateTime(dtCountBegin.AddDays(1).Year, dtCountBegin.AddDays(1).Month, dtCountBegin.AddDays(1).Day, tmEnd.Hour, tmEnd.Minute, tmEnd.Second);
                    }
                    else
                    {
                        tmExecuteBegin = new DateTime(dtCountBegin.AddDays(1).Year, dtCountBegin.AddDays(1).Month, dtCountBegin.AddDays(1).Day, tmBegin.Hour, tmBegin.Minute, tmBegin.Second);
                        tmExecuteEnd = new DateTime(dtCountBegin.AddDays(1).Year, dtCountBegin.AddDays(1).Month, dtCountBegin.AddDays(1).Day, tmEnd.Hour, tmEnd.Minute, tmEnd.Second);
                    }
                }

            }
            else if (strPerodicDesc.IndexOf("W") >= 0)  //Weekly
            {
                DateTime tmSearchDay = dtCountBegin;
                //没有考虑到跨天任务的处理,每周任务在跨0点时，添加时出现错误zmj2008-8-29
                if (tmEnd.TimeOfDay < dtCountBegin.TimeOfDay
                    && !bOverDay //zmj2008-8-29
                    ) //当天任务已经结束，搜索任务时从明天开始
                {
                    tmSearchDay = dtCountBegin.AddDays(1);
                }
                for (int i = 0; i < 8; i++)
                {
                    int nDayofWeek = (int)tmSearchDay.DayOfWeek;
                    if (strPerodicDesc.IndexOf("W" + Convert.ToString(nDayofWeek) + "+") >= 0)
                    {
                        int nAddDay = bOverDay ? 1 : 0;
                        tmExecuteBegin = new DateTime(tmSearchDay.Year, tmSearchDay.Month, tmSearchDay.Day, tmBegin.Hour, tmBegin.Minute, tmBegin.Second);
                        tmExecuteEnd = new DateTime(tmSearchDay.AddDays(nAddDay).Year, tmSearchDay.AddDays(nAddDay).Month, tmSearchDay.AddDays(nAddDay).Day, tmEnd.Hour, tmEnd.Minute, tmEnd.Second);
                        break;
                    }
                    tmSearchDay = tmSearchDay.AddDays(1);
                }


            }
            else if (strPerodicDesc.IndexOf("M") >= 0)  //Monthly
            {
                DateTime tmSearchDay = dtCountBegin;
                if (tmEnd.TimeOfDay < dtCountBegin.TimeOfDay
                    && !bOverDay//zmj2008-8-29
                    ) //当天任务已经结束，搜索任务时从明天开始
                {
                    tmSearchDay = dtCountBegin.AddDays(1);
                }

                for (int i = 0; i < 63; i++)//找两个月，肯定可以找到对应的天,连续的两个月最多62天
                {
                    int nDayofMonth = tmSearchDay.Day;
                    if (strPerodicDesc.IndexOf("M" + Convert.ToString(nDayofMonth) + "+") >= 0)
                    {
                        int nAddDay = bOverDay ? 1 : 0;
                        tmExecuteBegin = new DateTime(tmSearchDay.Year, tmSearchDay.Month, tmSearchDay.Day, tmBegin.Hour, tmBegin.Minute, tmBegin.Second);
                        tmExecuteEnd = new DateTime(tmSearchDay.AddDays(nAddDay).Year, tmSearchDay.AddDays(nAddDay).Month, tmSearchDay.AddDays(nAddDay).Day, tmEnd.Hour, tmEnd.Minute, tmEnd.Second);
                        break;
                    }
                    tmSearchDay = tmSearchDay.AddDays(1);
                }

            }
        }

        private static void PerodicHandle2(string strPerodicDesc, DateTime tmEnd, DateTime tmBegin, DateTime dtCountBegin, bool bOverDay, ref DateTime tmExecuteBegin, ref DateTime tmExecuteEnd)//把时间推到下次执行时间
        {
            if (strPerodicDesc.IndexOf("D") >= 0) //Daily
            {
                if (bOverDay)
                {
                    tmExecuteBegin = new DateTime(dtCountBegin.AddDays(1).Year, dtCountBegin.AddDays(1).Month, dtCountBegin.AddDays(1).Day, tmBegin.Hour, tmBegin.Minute, tmBegin.Second);
                    tmExecuteEnd = new DateTime(dtCountBegin.AddDays(2).Year, dtCountBegin.AddDays(2).Month, dtCountBegin.AddDays(2).Day, tmEnd.Hour, tmEnd.Minute, tmEnd.Second);
                }
                else
                {
                    tmExecuteBegin = new DateTime(dtCountBegin.AddDays(1).Year, dtCountBegin.AddDays(1).Month, dtCountBegin.AddDays(1).Day, tmBegin.Hour, tmBegin.Minute, tmBegin.Second);
                    tmExecuteEnd = new DateTime(dtCountBegin.AddDays(1).Year, dtCountBegin.AddDays(1).Month, dtCountBegin.AddDays(1).Day, tmEnd.Hour, tmEnd.Minute, tmEnd.Second);
                }

            }
            else if (strPerodicDesc.IndexOf("W") >= 0)  //Weekly
            {
                DateTime tmSearchDay = dtCountBegin;
                tmSearchDay = dtCountBegin.AddDays(1);
                for (int i = 0; i < 8; i++)
                {
                    int nDayofWeek = (int)tmSearchDay.DayOfWeek;
                    if (strPerodicDesc.IndexOf("W" + Convert.ToString(nDayofWeek) + "+") >= 0)
                    {
                        int nAddDay = bOverDay ? 1 : 0;
                        tmExecuteBegin = new DateTime(tmSearchDay.Year, tmSearchDay.Month, tmSearchDay.Day, tmBegin.Hour, tmBegin.Minute, tmBegin.Second);
                        tmExecuteEnd = new DateTime(tmSearchDay.AddDays(nAddDay).Year, tmSearchDay.AddDays(nAddDay).Month, tmSearchDay.AddDays(nAddDay).Day, tmEnd.Hour, tmEnd.Minute, tmEnd.Second);
                        break;
                    }
                    tmSearchDay = tmSearchDay.AddDays(1);
                }

            }
            else if (strPerodicDesc.IndexOf("M") >= 0)  //Monthly
            {
                DateTime tmSearchDay = dtCountBegin;
                tmSearchDay = dtCountBegin.AddDays(1);
                for (int i = 0; i < 63; i++)//找两个月，肯定可以找到对应的天,连续的两个月最多62天
                {
                    int nDayofMonth = tmSearchDay.Day;
                    if (strPerodicDesc.IndexOf("M" + Convert.ToString(nDayofMonth) + "+") >= 0)
                    {
                        int nAddDay = bOverDay ? 1 : 0;
                        tmExecuteBegin = new DateTime(tmSearchDay.Year, tmSearchDay.Month, tmSearchDay.Day, tmBegin.Hour, tmBegin.Minute, tmBegin.Second);
                        tmExecuteEnd = new DateTime(tmSearchDay.AddDays(nAddDay).Year, tmSearchDay.AddDays(nAddDay).Month, tmSearchDay.AddDays(nAddDay).Day, tmEnd.Hour, tmEnd.Minute, tmEnd.Second);
                        break;
                    }
                    tmSearchDay = tmSearchDay.AddDays(1);
                }

            }
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
                //A:如果今天不是执行日，明天是，那么需要把任务的日期改成明天
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
                    //A-今天是执行日，昨天不是:任务从今天执行到明天
                    //B-今天不是是执行日，昨天是:任务从昨天执行到今天
                    //C-昨天和今天都是执行日:有两个任务？？这种情况复制一个任务出来，ID为负值，作为从昨天执行到今天的任务				
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

        public bool IsInvalidPerodicTask(string strClassify, DateTime begin)
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
        public List<DateTime> GetDateTimeFromString(string str)
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

        public async Task UnLockTask(int taskid)
        {

            var item = await Context.DbpTask.Where(x => x.Taskid == taskid).SingleOrDefaultAsync();

            if (item != null)
            {
                item.Tasklock = string.Empty;
                await Context.SaveChangesAsync();
            }
        }

        public async Task UnLockTask(DbpTask taskid, bool savechange)
        {
            taskid.Tasklock = string.Empty;
            if (savechange)
            {
                await Context.SaveChangesAsync();
            }
        }

        public async Task LockTask(int taskid)
        {

            var item = await Context.DbpTask.Where(x => x.Taskid == taskid).SingleOrDefaultAsync();

            if (item != null)
            {
                item.Tasklock = Guid.NewGuid().ToString("N");
                await Context.SaveChangesAsync();
            }
            //Context.Entry(await Context.DbpTask.FirstOrDefaultAsync(x => x.Taskid == taskid)).CurrentValues.SetValues();
            //(await Context.SaveChangesAsync()) > 0;
        }

        public async Task UnLockAllTask()
        {
            var lst = await Context.DbpTask.Where(a => string.IsNullOrEmpty(a.Tasklock)).ToListAsync();
            if (lst != null && lst.Count > 0)
            {
                lst.ForEach(a => a.Tasklock = string.Empty);
            }

            try
            {
                await Context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                throw e;
            }
        }


        public int GetNextValId(string value)
        {
            return Context.DbpTask.Select(x => IngestTaskDBContext.next_val(value)).FirstOrDefault();
        }

        public async Task<bool> AddTaskSource(DbpTaskSource taskSource)
        {
            if (taskSource != null)
            {
                await Context.DbpTaskSource.AddAsync(taskSource);
                return await Context.SaveChangesAsync() > 0;
            }
            return false;
        }

        public async Task<int> ResetTaskErrorInfo(int taskid)
        {
            var task = await Context.DbpTask.Where(a => a.Taskid == taskid).SingleOrDefaultAsync();
            if (task != null)
            {
                task.Recunitid = (task.Recunitid & 0x100);//后面八位都是给task显示error的
            }

            var info = Context.DbpTaskErrorinfo.Where(a => a.Taskid == taskid);
            if (info != null && info.Count() > 0)
            {
                int ret = info.Count();

                Context.DbpTaskErrorinfo.RemoveRange(info);

                await Context.SaveChangesAsync();

                return ret;
            }
            return 0;
        }

        public async Task<bool> AddTaskErrorInfo(DbpTaskErrorinfo taskSource)
        {
            if (taskSource != null)
            {
                await Context.DbpTaskErrorinfo.AddAsync(taskSource);
                return await Context.SaveChangesAsync() > 0;
            }
            return false;
        }

        public async Task<List<TResult>> GetTaskErrorInfoListAsync<TResult>(Func<IQueryable<DbpTaskErrorinfo>, IQueryable<TResult>> query, bool notrack = false)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            if (notrack)
            {
                return await query.Invoke(Context.DbpTaskErrorinfo.AsNoTracking()).ToListAsync();
            }
            return await query.Invoke(Context.DbpTaskErrorinfo).ToListAsync();
        }

        public async Task<bool> AddPolicyTask(List<DbpPolicytask> policytasks)
        {
            if (policytasks != null && policytasks.Count > 0)
            {
                await Context.DbpPolicytask.AddRangeAsync();
                return await Context.SaveChangesAsync() > 0;
            }
            return false;
        }
        /////////////////////
        ///
        public async Task<bool> UpdateTaskSource(DbpTaskSource taskSource)
        {
            try
            {

                var dbpCapParam = await GetTaskSourceAsync(a => a.Where(x => x.Taskid == taskSource.Taskid), true);
                if (dbpCapParam == null)
                {
                    //add
                    Context.DbpTaskSource.Add(taskSource);
                }
                else
                {
                    //update
                    Context.Attach(taskSource);
                    Context.Entry(taskSource).Property(x => x.Tasksource).IsModified = true;
                }
                await Context.SaveChangesAsync();
            }
            catch (System.Exception ex)
            {
                Logger.Error("UpdateGlobalValueAsync : " + ex.ToString());
                throw ex;
            }
            return true;
        }

        public async Task<bool> UpdateTaskSource(DbpTaskSource taskSource, bool submitFlag)
        {
            try
            {

                var dbpCapParam = await GetTaskSourceAsync(a => a.Where(x => x.Taskid == taskSource.Taskid), true);
                if (dbpCapParam == null)
                {
                    //add
                    Context.DbpTaskSource.Add(taskSource);
                }
                else
                {
                    //update
                    Context.Attach(taskSource);
                    Context.Entry(taskSource).Property(x => x.Tasksource).IsModified = true;
                }

                if (submitFlag)
                {
                    await Context.SaveChangesAsync();
                }
            }
            catch (System.Exception ex)
            {
                Logger.Error("UpdateGlobalValueAsync : " + ex.ToString());
                throw ex;
            }
            return true;
        }


        public async Task<bool> AddTaskSourceList(List<DbpTaskSource> taskSources, bool submitFlag)
        {
            if (taskSources != null && taskSources.Count > 0)
            {
                await Context.DbpTaskSource.AddRangeAsync(taskSources);
            }

            if (submitFlag)
            {
                return await Context.SaveChangesAsync() > 0;
            }

            return true;
        }

        public async Task<bool> AddTask(DbpTask tasks, bool savechange)
        {
            if (tasks != null)
            {
                await Context.DbpTask.AddAsync(tasks);
            }

            if (savechange)
            {
                return await Context.SaveChangesAsync() > 0;
            }
            else
            {
                return true;
            }

        }

        public async Task<bool> AddTaskList(List<DbpTask> tasks, bool savechange)
        {
            if (tasks != null && tasks.Count > 0)
            {
                await Context.DbpTask.AddRangeAsync(tasks);
            }

            if (savechange)
            {
                return await Context.SaveChangesAsync() > 0;
            }
            else
            {
                return true;
            }

        }

        public async Task<bool> AddPolicyTask(List<DbpPolicytask> policytasks, bool submitFlag)
        {
            if (policytasks != null && policytasks.Count > 0)
            {
                await Context.DbpPolicytask.AddRangeAsync(policytasks);
            }

            if (submitFlag)
            {
                return await Context.SaveChangesAsync() > 0;
            }

            return true;
        }
        public async Task UpdateTaskAsync(DbpTask item, bool savechange)
        {
            if (item != null)
            {
                Context.DbpTask.Update(item);
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

        public async Task UpdateTaskListAsync(List<DbpTask> lst, bool submitFlag)
        {
            if (lst != null && lst.Count > 0)
            {
                Context.DbpTask.UpdateRange(lst);
            }

            if (submitFlag)
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


        public async Task UpdateTaskMetaDataAsync(int taskid, MetaDataType type, string metadata, bool submitFlag)
        {
            if (string.IsNullOrEmpty(metadata))
            {
                return;
            }
            var item = await Context.DbpTaskMetadata.Where(x => x.Taskid == taskid && x.Metadatatype == (int)type).FirstOrDefaultAsync();
            if (item == null)
            {
                await Context.DbpTaskMetadata.AddAsync(new DbpTaskMetadata()
                {
                    Taskid = taskid,
                    Metadatatype = (int)type,
                    Metadatalong = metadata
                });
            }
            else
                item.Metadatalong = metadata;

            try
            {
                if (submitFlag)
                {
                    await Context.SaveChangesAsync();
                }
            }
            catch (DbUpdateException e)
            {
                throw e;
            }
        }

        public async Task UpdateTaskMetaDataListAsync(List<Dto.Request.SubmitMetadata> metadatas)
        {
            foreach (var metadata in metadatas)
            {
                var item = await Context.DbpTaskMetadata.Where(x => x.Taskid == metadata.taskId && x.Metadatatype == (int)metadata.type).SingleOrDefaultAsync();
                if (item == null)
                {
                    await Context.DbpTaskMetadata.AddAsync(new DbpTaskMetadata()
                    {
                        Taskid = metadata.taskId,
                        Metadatatype = (int)metadata.type,
                        Metadatalong = metadata.metadata
                    });
                }
                else
                    item.Metadatalong = metadata.metadata;
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

        public async Task<bool> UpdateTaskBmp(Dictionary<int, string> taskPmp)
        {
            var taskIds = taskPmp.Select(a => a.Key).ToList();
            var tasks = await Context.DbpTask.Where(a => taskIds.Contains(a.Taskid)).ToListAsync();
            if (tasks != null && tasks.Count > 0)
            {
                tasks.ForEach(a => a.Description = taskPmp.First(x => x.Key == a.Taskid).Value);
                return await Context.SaveChangesAsync() > 0;
            }
            return false;
        }


        #region 3.0

        
        #endregion

    }
}
