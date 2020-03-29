using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IngestTaskPlugin.Dto;
using IngestTaskPlugin.Models;
using Microsoft.EntityFrameworkCore;

namespace IngestTaskPlugin.Stores
{
    public class TaskInfoStore : ITaskStore
    {
        public TaskInfoStore(IngestTaskDBContext baseDataDbContext)
        {
            Context = baseDataDbContext;
        }

        protected IngestTaskDBContext Context { get; }

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

        //public async Task<List<DbpTask>> GetCapturingTaskListAsync(List<int> lstchannel)
        //{
        //    if (lstchannel == null)
        //    {
        //        throw new ArgumentNullException(nameof(lstchannel));
        //    }

            
        //}

        public async Task StopCapturingChannelAsync(int Channel)
        {
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

            if (ltask.Tasktype == (int)TaskType.TT_VTRUPLOAD)
            {
                //vtr更新
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
        }

        public async Task StopCapturingListChannelAsync(List<int> lstChaneel)
        {
            //获取采集中任务列表
            var lsttask = await GetTaskListAsync<DbpTask>(a => a
            .Where(x => lstChaneel.Contains(x.Channelid.GetValueOrDefault()) && (x.State == (int)taskState.tsExecuting || x.State == (int)taskState.tsManuexecuting))
            .Select(ite =>
            new DbpTask {
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
        }
    }
}
