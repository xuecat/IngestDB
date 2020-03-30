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

        public async Task SetVtrUploadTaskListState(List<int> lsttaskid, VTRUPLOADTASKSTATE vtrstate, string errinfo)
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

            try
            {
                await Context.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {
                throw e;
            }
        }

        public async Task SetVtrUploadTaskState(int taskid, VTRUPLOADTASKSTATE vtrstate, string errinfo)
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

            try
            {
                await Context.SaveChangesAsync();
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
            }
            ));

            ntaskid = ltask.Taskid;
            if (ltask.Tasktype == (int)TaskType.TT_VTRUPLOAD)
            {
                //vtr更新
                await SetVtrUploadTaskState(ltask.Taskid, VTRUPLOADTASKSTATE.VTR_UPLOAD_COMPLETE, string.Empty);
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
                    await SetVtrUploadTaskListState(item.Select(f => f.Taskid).ToList(), VTRUPLOADTASKSTATE.VTR_UPLOAD_COMPLETE, string.Empty);
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
                Tasktype = ite.Tasktype,
                Endtime = ite.Endtime,
                NewEndtime = ite.Endtime,
                Recunitid = ite.Recunitid,
            }
            ));

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
                    await SetVtrUploadTaskListState(item.Select(f => f.Taskid).ToList(), VTRUPLOADTASKSTATE.VTR_UPLOAD_COMPLETE, string.Empty);
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
