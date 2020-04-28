namespace IngestTaskPlugin.Stores.VTR
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using IngestDBCore;
    using IngestDBCore.Tool;
    using IngestTaskPlugin.Dto;
    using IngestTaskPlugin.Dto.Request;
    using IngestTaskPlugin.Dto.Response.OldVtr;
    using IngestTaskPlugin.Models;
    using Microsoft.EntityFrameworkCore;
    using Sobey.Core.Log;

    /// <summary>
    /// Defines the <see cref="VtrStore" />.
    /// </summary>
    public class VtrStore : IVtrStore
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VtrStore"/> class.
        /// </summary>
        /// <param name="baseDataDbContext">The baseDataDbContext<see cref="IngestTaskDBContext"/>.</param>
        public VtrStore(IngestTaskDBContext baseDataDbContext)
        {
            Context = baseDataDbContext;
        }

        /// <summary>
        /// Defines the Logger.
        /// </summary>
        private readonly ILogger Logger = LoggerManager.GetLogger($"{nameof(VtrStore)}");

        /// <summary>
        /// Gets the Context.
        /// </summary>
        protected IngestTaskDBContext Context { get; }

        /// <summary>
        /// The QueryListAsync.
        /// </summary>
        /// <typeparam name="TEntity">.</typeparam>
        /// <typeparam name="TResult">.</typeparam>
        /// <param name="query">The query<see cref="Func{IQueryable{TEntity}, IQueryable{TResult}}"/>.</param>
        /// <param name="notrack">The notrack<see cref="bool"/>.</param>
        /// <returns>The <see cref="Task{List{TResult}}"/>.</returns>
        public async Task<List<TResult>> QueryListAsync<TEntity, TResult>(Func<IQueryable<TEntity>, IQueryable<TResult>> query, bool notrack = false) where TEntity : class
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            if (notrack)
            {
                return await query(Context.Set<TEntity>().AsNoTracking()).ToListAsync();
            }
            return await query(Context.Set<TEntity>()).ToListAsync();
        }

        /// <summary>
        /// The QueryModelAsync.
        /// </summary>
        /// <typeparam name="TEntity">.</typeparam>
        /// <typeparam name="TResult">.</typeparam>
        /// <param name="query">The query<see cref="Func{IQueryable{TEntity}, Task{TResult}}"/>.</param>
        /// <param name="notrack">The notrack<see cref="bool"/>.</param>
        /// <returns>The <see cref="Task{TResult}"/>.</returns>
        public async Task<TResult> QueryModelAsync<TEntity, TResult>(Func<IQueryable<TEntity>, Task<TResult>> query, bool notrack = false) where TEntity : class
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            if (notrack)
            {
                return await query(Context.Set<TEntity>().AsNoTracking());
            }
            return await query(Context.Set<TEntity>());
        }

        public async Task<List<TResult>> GetMetadatapolicy<TResult>(Func<IQueryable<DbpMetadatapolicy>, IQueryable<TResult>> query, bool notrack = false)
        {
            return await this.QueryListAsync(query, notrack);
        }

        public async Task<TResult> GetMetadatapolicy<TResult>(Func<IQueryable<DbpMetadatapolicy>, Task<TResult>> query, bool notrack = false)
        {
            return await this.QueryModelAsync(query, notrack);
        }

        public async Task<List<TResult>> GetPolicyuser<TResult>(Func<IQueryable<DbpPolicyuser>, IQueryable<TResult>> query, bool notrack = false)
        {
            return await this.QueryListAsync(query, notrack);
        }

        public async Task<TResult> GetPolicyuser<TResult>(Func<IQueryable<DbpPolicyuser>, Task<TResult>> query, bool notrack = false)
        {
            return await this.QueryModelAsync(query, notrack);
        }

        /// <summary>
        /// The GetTapelist.
        /// </summary>
        /// <typeparam name="TResult">.</typeparam>
        /// <param name="query">The query<see cref="Func{IQueryable{VtrTapelist}, IQueryable{TResult}}"/>.</param>
        /// <param name="notrack">The notrack<see cref="bool"/>.</param>
        /// <returns>The <see cref="Task{List{TResult}}"/>.</returns>
        public async Task<List<TResult>> GetTapelist<TResult>(Func<IQueryable<VtrTapelist>, IQueryable<TResult>> query, bool notrack = false)
        {
            return await this.QueryListAsync(query, notrack);
        }

        /// <summary>
        /// The GetTapelist.
        /// </summary>
        /// <typeparam name="TResult">.</typeparam>
        /// <param name="query">The query<see cref="Func{IQueryable{VtrTapelist}, Task{TResult}}"/>.</param>
        /// <param name="notrack">The notrack<see cref="bool"/>.</param>
        /// <returns>The <see cref="Task{TResult}"/>.</returns>
        public async Task<TResult> GetTapelist<TResult>(Func<IQueryable<VtrTapelist>, Task<TResult>> query, bool notrack = false)
        {
            return await this.QueryModelAsync(query, notrack);
        }

        /// <summary>
        /// The SaveTaplist.
        /// </summary>
        /// <param name="tapelists">The tapelists<see cref="IEnumerable{VtrTapelist}"/>.</param>
        /// <returns>The <see cref="Task{bool}"/>.</returns>
        public Task<bool> SaveTaplist(IEnumerable<VtrTapelist> tapelists)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The SaveTaplist.
        /// </summary>
        /// <param name="tapelist">The tapelist<see cref="VtrTapelist"/>.</param>
        /// <returns>The <see cref="Task{int}"/>.</returns>
        public async Task<int> SaveTaplist(VtrTapelist tapelist)
        {
            if (await Context.VtrTapelist.AnyAsync(a => a.Tapeid == tapelist.Tapeid))
            {
                var entry = Context.VtrTapelist.Update(tapelist);
                entry.State = EntityState.Modified;
            }
            else
            {
                var oldTape = await Context.VtrTapelist.FirstOrDefaultAsync(a => a.Tapename == tapelist.Tapename);
                if (tapelist != null && oldTape != null)
                {
                    oldTape.Tapedesc = tapelist.Tapedesc;
                }
                else
                {
                    tapelist.Tapeid = await Context.VtrTapelist.MaxAsync(a => a.Tapeid) + 1;
                    if (tapelist.Tapeid < 11) tapelist.Tapeid = 11;
                    await Context.VtrTapelist.AddAsync(tapelist);
                }
            }
            await Context.SaveChangesAsync();
            return tapelist.Tapeid;
        }

        /// <summary>
        /// The GetTapeVtrMap.
        /// </summary>
        /// <typeparam name="TResult">.</typeparam>
        /// <param name="query">The query<see cref="Func{IQueryable{VtrTapeVtrMap}, IQueryable{TResult}}"/>.</param>
        /// <param name="notrack">The notrack<see cref="bool"/>.</param>
        /// <returns>The <see cref="Task{List{TResult}}"/>.</returns>
        public async Task<List<TResult>> GetTapeVtrMap<TResult>(Func<IQueryable<VtrTapeVtrMap>, IQueryable<TResult>> query, bool notrack = false)
        {
            return await this.QueryListAsync(query, notrack);
        }

        /// <summary>
        /// The GetTapeVtrMap.
        /// </summary>
        /// <typeparam name="TResult">.</typeparam>
        /// <param name="query">The query<see cref="Func{IQueryable{VtrTapeVtrMap}, Task{TResult}}"/>.</param>
        /// <param name="notrack">The notrack<see cref="bool"/>.</param>
        /// <returns>The <see cref="Task{TResult}"/>.</returns>
        public async Task<TResult> GetTapeVtrMap<TResult>(Func<IQueryable<VtrTapeVtrMap>, Task<TResult>> query, bool notrack = false)
        {
            return await this.QueryModelAsync(query, notrack);
        }

        /// <summary>
        /// The SaveTapeVtrMap.
        /// </summary>
        /// <param name="tapeMap">The tapeMap<see cref="IEnumerable{VtrTapeVtrMap}"/>.</param>
        /// <returns>The <see cref="Task{List{bool}}"/>.</returns>
        public async Task<List<bool>> SaveTapeVtrMap(IEnumerable<VtrTapeVtrMap> tapeMap)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The SaveTapeVtrMap.
        /// </summary>
        /// <param name="tapeMap">The tapeMap<see cref="VtrTapeVtrMap"/>.</param>
        /// <returns>The <see cref="Task{bool}"/>.</returns>
        public async Task<bool> SaveTapeVtrMap(VtrTapeVtrMap tapeMap)
        {
            var oldMap = await Context.VtrTapeVtrMap.FirstOrDefaultAsync(a => a.Vtrid == tapeMap.Vtrid);
            if (oldMap != null)
            {
                var tape = await Context.VtrTapelist.FirstOrDefaultAsync(a => a.Tapeid == tapeMap.Tapeid);
                if (tape == null)
                {
                    SobeyRecException.ThrowSelfNoParam(nameof(SaveTapeVtrMap), GlobalDictionary.GLOBALDICT_CODE_IN_SETVTRTAPEMAP_TAPEID_IS_NOT_EXIST_ONEPARAM, Logger, null);
                }
                oldMap.Tapeid = tape.Tapeid;
            }
            else
            {
                if (!await Context.VtrDetailinfo.AnyAsync(a => a.Vtrid == tapeMap.Vtrid))
                {
                    return false;
                }
                var tape = await Context.VtrTapelist.FirstOrDefaultAsync(a => a.Tapeid == tapeMap.Tapeid);
                if (tape == null)
                {
                    SobeyRecException.ThrowSelfNoParam(nameof(SaveTapeVtrMap), GlobalDictionary.GLOBALDICT_CODE_IN_SETVTRTAPEMAP_TAPEID_IS_NOT_EXIST_ONEPARAM, Logger, null);
                }
                await Context.VtrTapeVtrMap.AddAsync(tapeMap);
            }
            return await Context.SaveChangesAsync() > 0;
        }

        /// <summary>
        /// The GetTapeVtrMap.
        /// </summary>
        /// <typeparam name="TResult">.</typeparam>
        /// <param name="query">The query<see cref="Func{IQueryable{DbpTask}, Task{TResult}}"/>.</param>
        /// <param name="notrack">The notrack<see cref="bool"/>.</param>
        /// <returns>The <see cref="Task{List{TResult}}"/>.</returns>
        public async Task<TResult> GetTask<TResult>(Func<IQueryable<DbpTask>, Task<TResult>> query, bool notrack = false)
        {
            return await this.QueryModelAsync(query, notrack);
        }

        /// <summary>
        /// The GetTypeinfo.
        /// </summary>
        /// <typeparam name="TResult">.</typeparam>
        /// <param name="query">The query<see cref="Func{IQueryable{VtrTypeinfo}, IQueryable{TResult}}"/>.</param>
        /// <param name="notrack">The notrack<see cref="bool"/>.</param>
        /// <returns>The <see cref="Task{List{TResult}}"/>.</returns>
        public async Task<List<TResult>> GetTypeinfo<TResult>(Func<IQueryable<VtrTypeinfo>, IQueryable<TResult>> query, bool notrack = false)
        {
            return await this.QueryListAsync(query, notrack);
        }

        /// <summary>
        /// The GetTypeinfo.
        /// </summary>
        /// <typeparam name="TResult">.</typeparam>
        /// <param name="query">The query<see cref="Func{IQueryable{VtrTypeinfo}, Task{TResult}}"/>.</param>
        /// <param name="notrack">The notrack<see cref="bool"/>.</param>
        /// <returns>The <see cref="Task{TResult}"/>.</returns>
        public async Task<TResult> GetTypeinfo<TResult>(Func<IQueryable<VtrTypeinfo>, Task<TResult>> query, bool notrack = false)
        {
            return await this.QueryModelAsync(query, notrack);
        }

        /// <summary>
        /// The GetDetailinfo.
        /// </summary>
        /// <typeparam name="TResult">.</typeparam>
        /// <param name="query">The query<see cref="Func{IQueryable{VtrDetailinfo}, IQueryable{TResult}}"/>.</param>
        /// <param name="notrack">The notrack<see cref="bool"/>.</param>
        /// <returns>The <see cref="Task{List{TResult}}"/>.</returns>
        public async Task<List<TResult>> GetDetailinfo<TResult>(Func<IQueryable<VtrDetailinfo>, IQueryable<TResult>> query, bool notrack = false)
        {
            return await this.QueryListAsync(query, notrack);
        }

        /// <summary>
        /// The GetDetailinfo.
        /// </summary>
        /// <typeparam name="TResult">.</typeparam>
        /// <param name="query">The query<see cref="Func{IQueryable{VtrDetailinfo}, Task{TResult}}"/>.</param>
        /// <param name="notrack">The notrack<see cref="bool"/>.</param>
        /// <returns>The <see cref="Task{TResult}"/>.</returns>
        public async Task<TResult> GetDetailinfo<TResult>(Func<IQueryable<VtrDetailinfo>, Task<TResult>> query, bool notrack = false)
        {
            return await this.QueryModelAsync(query, notrack);
        }

        public async Task<List<VTRUploadTaskContent>> GetUploadTaskContent(VTRUploadCondition Condition)
        {
            IQueryable<VtrUploadtask> query = Context.VtrUploadtask.AsNoTracking();
            if (Condition.lBlankTaskID > 0) query = query.Where(a => a.Vtrtaskid == Condition.lBlankTaskID);
            if (string.IsNullOrEmpty(Condition.strTaskName)) query = query.Where(a => a.Taskname.Contains(Condition.strTaskName));
            if (Condition.lTaskID > 0) query = query.Where(a => a.Taskid == Condition.lTaskID);
            if (Condition.lVtrID > 0) query = query.Where(a => a.Vtrid == Condition.lVtrID);
            if (Condition.state != null && Condition.state.Count > 0)
            {
                var states = Condition.state.Select(a => (int?)a).ToList();
                query = query.Where(a => states.Contains(a.Taskstate));
            }
            if (!string.IsNullOrEmpty(Condition.szUserCode)) query = query.Where(a => a.Usercode == Condition.szUserCode);
            if (!string.IsNullOrEmpty(Condition.strUserToken)) query = query.Where(a => a.Usertoken == Condition.strUserToken);
            string szMinTime = DateTime.MinValue.ToString("yyyy-MM-dd HH:mm:ss");
            if (!string.IsNullOrEmpty(Condition.strMaxCommitTime) && Condition.strMaxCommitTime != szMinTime)
            {
                var maxCommitTime = DateTimeFormat.DateTimeFromString(Condition.strMaxCommitTime);
                query = query.Where(a => a.Committime <= maxCommitTime);
            }
            if (!string.IsNullOrEmpty(Condition.strMinCommitTime) && Condition.strMinCommitTime != szMinTime)
            {
                var minCommitTime = DateTimeFormat.DateTimeFromString(Condition.strMinCommitTime);
                query = query.Where(a => a.Committime >= minCommitTime);
            }
            var ab = await query.Join(Context.DbpTask.AsNoTracking(), a => a.Taskid, b => b.Taskid, (a, b) => new { task = b, vtr = a }).ToListAsync();
            if (ab != null && ab.Count > 0)
            {
                return ab.Select(a => new VTRUploadTaskContent
                {
                    nTaskId = a.task.Taskid,
                    strTaskName = a.task.Taskname,
                    nUnit = (int)a.task.Recunitid,
                    nSignalId = (int)a.task.Signalid,
                    nChannelId = (int)a.task.Channelid,
                    emState = (int)a.task.State,
                    strBegin = a.task.Starttime.ToString(),
                    strEnd = a.task.Endtime.ToString(),
                    strClassify = a.task.Category,
                    strTaskDesc = a.task.Description,
                    strStampImage = a.task.Description,
                    emTaskType = (int)a.task.Tasktype,
                    emCooperantType = (int)a.task.Backtype,
                    strTaskGUID = a.task.Taskguid,

                    nVtrId = (int)a.vtr.Vtrid,
                    nBlankTaskId = (int)a.vtr.Vtrtaskid,
                    nTrimIn = (int)a.vtr.Trimin,
                    nTrimOut = (int)a.vtr.Trimout,
                    emTaskState = (VTRUPLOADTASKSTATE)a.vtr.Taskstate,
                    strUserCode = a.vtr.Usercode,
                    strCommitTime = a.vtr.Committime.ToString(),
                    nOrder = (int)a.vtr.Uploadorder,
                    nTapeId = (int)a.vtr.Tapeid,
                    strUserToken = a.vtr.Usertoken,
                    nTrimInCTL = (int)a.vtr.Triminctl,
                    nTrimOutCTL = (int)a.vtr.Trimoutctl,
                    emVtrTaskType = (VTRUPLOADTASKTYPE)a.vtr.Vtrtasktype,
                }).ToList();
            }
            return new List<VTRUploadTaskContent>();
        }

        public async Task<List<VtrUploadtask>> GetUploadtaskInfo(VTRUploadCondition Condition, bool bTaskMoreThanZero)
        {
            IQueryable<VtrUploadtask> query = Context.VtrUploadtask.AsNoTracking();
            if (Condition.lBlankTaskID > 0)
            {
                query = query.Where(a => a.Vtrtaskid == Condition.lBlankTaskID);
            }
            if (string.IsNullOrEmpty(Condition.strTaskName))
            {
                query = query.Where(a => a.Taskname.Contains(Condition.strTaskName));
            }
            if (!bTaskMoreThanZero)
            {
                if (Condition.lTaskID >= 0)
                {
                    query = query.Where(a => a.Taskid == Condition.lTaskID);
                }
            }
            else if (Condition.lTaskID > 0)
            {
                query = query.Where(a => a.Taskid == Condition.lTaskID);
            }
            if (Condition.lVtrID > 0)
            {
                query = query.Where(a => a.Vtrid == Condition.lVtrID);
            }
            if (Condition.state != null && Condition.state.Count > 0)
            {
                var states = Condition.state.Select(a => (int?)a).ToList();
                query = query.Where(a => states.Contains(a.Taskstate));
            }
            if (!string.IsNullOrEmpty(Condition.szUserCode))
            {
                query = query.Where(a => a.Usercode == Condition.szUserCode);
            }
            if (!string.IsNullOrEmpty(Condition.strUserToken))
            {
                query = query.Where(a => a.Usertoken == Condition.strUserToken);
            }
            string szMinTime = DateTime.MinValue.ToString("yyyy-MM-dd HH:mm:ss");
            if (!string.IsNullOrEmpty(Condition.strMaxCommitTime) && Condition.strMaxCommitTime != szMinTime)
            {
                var maxCommitTime = DateTimeFormat.DateTimeFromString(Condition.strMaxCommitTime);
                query = query.Where(a => a.Committime <= maxCommitTime);
            }
            if (!string.IsNullOrEmpty(Condition.strMinCommitTime) && Condition.strMinCommitTime != szMinTime)
            {
                var minCommitTime = DateTimeFormat.DateTimeFromString(Condition.strMinCommitTime);
                query = query.Where(a => a.Committime >= minCommitTime);
            }
            return await query.ToListAsync();
        }

        /// <summary>
        /// The GetUploadtask.
        /// </summary>
        /// <typeparam name="TResult">.</typeparam>
        /// <param name="query">The query<see cref="Func{IQueryable{VtrUploadtask}, IQueryable{TResult}}"/>.</param>
        /// <param name="notrack">The notrack<see cref="bool"/>.</param>
        /// <returns>The <see cref="Task{List{TResult}}"/>.</returns>
        public async Task<List<TResult>> GetUploadtask<TResult>(Func<IQueryable<VtrUploadtask>, IQueryable<TResult>> query, bool notrack = false)
        {
            return await this.QueryListAsync(query, notrack);
        }

        /// <summary>
        /// The GetUploadtask.
        /// </summary>
        /// <typeparam name="TResult">.</typeparam>
        /// <param name="query">The query<see cref="Func{IQueryable{VtrUploadtask}, Task{TResult}}"/>.</param>
        /// <param name="notrack">The notrack<see cref="bool"/>.</param>
        /// <returns>The <see cref="Task{TResult}"/>.</returns>
        public async Task<TResult> GetUploadtask<TResult>(Func<IQueryable<VtrUploadtask>, Task<TResult>> query, bool notrack = false)
        {
            return await this.QueryModelAsync(query, notrack);
        }

        /// <summary>
        /// The AddUploadtask.
        /// </summary>
        /// <param name="task">The task<see cref="VtrUploadtask"/>.</param>
        /// <returns>The <see cref="Task{bool}"/>.</returns>
        public async Task<bool> AddUploadtask(VtrUploadtask task)
        {
            if (task != null)
            {
                await Context.VtrUploadtask.AddAsync(task);
                return await Context.SaveChangesAsync() > 0;
            }
            return false;
        }

        /// <summary>
        /// The UpdateUploadtask.
        /// </summary>
        /// <param name="task">The task<see cref="VtrUploadtask"/>.</param>
        /// <returns>The <see cref="Task{bool}"/>.</returns>
        public async Task<bool> UpdateUploadtask(VtrUploadtask task)
        {
            if (await Context.VtrUploadtask.AnyAsync(a => a.Taskid == task.Taskid))
            {
                Context.Update(task).State = EntityState.Modified;
                return await Context.SaveChangesAsync() > 0;
            }
            return false;
        }
    }
}
