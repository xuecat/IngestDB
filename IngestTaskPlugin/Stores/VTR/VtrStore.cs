namespace IngestTaskPlugin.Stores.VTR
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using IngestDBCore;
    using IngestTaskPlugin.Dto;
    using IngestTaskPlugin.Dto.Request;
    using IngestTaskPlugin.Dto.Response;
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

        /// <summary>
        /// The GetMetadatapolicy.
        /// </summary>
        /// <typeparam name="TResult">.</typeparam>
        /// <param name="query">The query<see cref="Func{IQueryable{DbpMetadatapolicy}, IQueryable{TResult}}"/>.</param>
        /// <param name="notrack">The notrack<see cref="bool"/>.</param>
        /// <returns>The <see cref="Task{List{TResult}}"/>.</returns>
        public async Task<List<TResult>> GetMetadatapolicy<TResult>(Func<IQueryable<DbpMetadatapolicy>, IQueryable<TResult>> query, bool notrack = false)
        {
            return await this.QueryListAsync(query, notrack);
        }

        /// <summary>
        /// The GetMetadatapolicy.
        /// </summary>
        /// <typeparam name="TResult">.</typeparam>
        /// <param name="query">The query<see cref="Func{IQueryable{DbpMetadatapolicy}, Task{TResult}}"/>.</param>
        /// <param name="notrack">The notrack<see cref="bool"/>.</param>
        /// <returns>The <see cref="Task{TResult}"/>.</returns>
        public async Task<TResult> GetMetadatapolicy<TResult>(Func<IQueryable<DbpMetadatapolicy>, Task<TResult>> query, bool notrack = false)
        {
            return await this.QueryModelAsync(query, notrack);
        }

        /// <summary>
        /// The GetPolicyuser.
        /// </summary>
        /// <typeparam name="TResult">.</typeparam>
        /// <param name="query">The query<see cref="Func{IQueryable{DbpPolicyuser}, IQueryable{TResult}}"/>.</param>
        /// <param name="notrack">The notrack<see cref="bool"/>.</param>
        /// <returns>The <see cref="Task{List{TResult}}"/>.</returns>
        public async Task<List<TResult>> GetPolicyuser<TResult>(Func<IQueryable<DbpPolicyuser>, IQueryable<TResult>> query, bool notrack = false)
        {
            return await this.QueryListAsync(query, notrack);
        }

        /// <summary>
        /// The GetPolicyuser.
        /// </summary>
        /// <typeparam name="TResult">.</typeparam>
        /// <param name="query">The query<see cref="Func{IQueryable{DbpPolicyuser}, Task{TResult}}"/>.</param>
        /// <param name="notrack">The notrack<see cref="bool"/>.</param>
        /// <returns>The <see cref="Task{TResult}"/>.</returns>
        public async Task<TResult> GetPolicyuser<TResult>(Func<IQueryable<DbpPolicyuser>, Task<TResult>> query, bool notrack = false)
        {
            return await this.QueryModelAsync(query, notrack);
        }

        /// <summary>
        /// The GetTapelist.
        /// </summary>
        /// <typeparam name="TResult">.</typeparam>
        /// <param name="query">The query<see cref="Func{IQueryable{DbpTaskMetadataBackup}, IQueryable{TResult}}"/>.</param>
        /// <param name="notrack">The notrack<see cref="bool"/>.</param>
        /// <returns>The <see cref="Task{List{TResult}}"/>.</returns>
        public async Task<List<TResult>> GetTaskMetadataBackup<TResult>(Func<IQueryable<DbpTaskMetadataBackup>, IQueryable<TResult>> query, bool notrack = false)
        {
            return await this.QueryListAsync(query, notrack);
        }

        /// <summary>
        /// The GetTapelist.
        /// </summary>
        /// <typeparam name="TResult">.</typeparam>
        /// <param name="query">The query<see cref="Func{IQueryable{DbpTaskMetadataBackup}, Task{TResult}}"/>.</param>
        /// <param name="notrack">The notrack<see cref="bool"/>.</param>
        /// <returns>The <see cref="Task{TResult}"/>.</returns>
        public async Task<TResult> GetTaskMetadataBackup<TResult>(Func<IQueryable<DbpTaskMetadataBackup>, Task<TResult>> query, bool notrack = false)
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

        /// <summary>
        /// The GetUploadTaskContent.
        /// </summary>
        /// <param name="Condition">The Condition<see cref="VTRUploadConditionRequest"/>.</param>
        /// <returns>The <see cref="Task{List{VTRUploadTaskContentResponse}}"/>.</returns>
        public async Task<List<VTRUploadTaskContentResponse>> GetUploadTaskContent(VTRUploadConditionRequest Condition)
        {
            IQueryable<VtrUploadtask> query = Context.VtrUploadtask.AsNoTracking();
            if (Condition.BlankTaskId > 0) query = query.Where(a => a.Vtrtaskid == Condition.BlankTaskId);
            if (string.IsNullOrEmpty(Condition.TaskName)) query = query.Where(a => a.Taskname.Contains(Condition.TaskName));
            if (Condition.TaskId > 0) query = query.Where(a => a.Taskid == Condition.TaskId);
            if (Condition.VtrId > 0) query = query.Where(a => a.Vtrid == Condition.VtrId);
            if (Condition.TaskState != null && Condition.TaskState.Count > 0) query = query.Where(a => Condition.TaskState.Contains(a.Taskstate));
            if (!string.IsNullOrEmpty(Condition.UserCode)) query = query.Where(a => a.Usercode == Condition.UserCode);
            if (!string.IsNullOrEmpty(Condition.UserToken)) query = query.Where(a => a.Usertoken == Condition.UserToken);
            if (Condition.MaxCommitTime != DateTime.MinValue) query = query.Where(a => a.Committime <= Condition.MaxCommitTime);
            if (Condition.MinCommitTime != DateTime.MinValue) query = query.Where(a => a.Committime >= Condition.MinCommitTime);
            return await GetUploadTaskContent(query, Context.DbpTask.AsNoTracking());
        }

        /// <summary>
        /// The GetNeedScheduleExecuteVTRUploadTasks.
        /// </summary>
        /// <param name="dtBegin">The dtBegin<see cref="DateTime"/>.</param>
        /// <returns>The <see cref="Task{List{VTRUploadTaskContentResponse}}"/>.</returns>
        public async Task<List<VTRUploadTaskContentResponse>> GetNeedScheduleExecuteVTRUploadTasks(DateTime dtBegin)
        {
            IQueryable<VtrUploadtask> uploadQuery = Context.VtrUploadtask.AsNoTracking();
            uploadQuery.Where(a => a.Vtrtasktype == 1 && a.Taskstate == 2);
            IQueryable<DbpTask> taskQeruy = Context.DbpTask.AsNoTracking();
            taskQeruy.Where(a => a.Tasktype == 6 &&
                                 a.Tasklock == "" &&
                                 a.SyncState == 0 &&
                                 a.Starttime < dtBegin.AddSeconds(5) &&
                                 a.Starttime > dtBegin.AddDays(-1));

            return await GetUploadTaskContent(uploadQuery, taskQeruy);
        }

        /// <summary>
        /// The GetWillExecuteVTRUploadTasks.
        /// </summary>
        /// <param name="minute">The minute<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{List{VTRUploadTaskContentResponse}}"/>.</returns>
        public async Task<List<VTRUploadTaskContentResponse>> GetWillExecuteVTRUploadTasks(int minute)
        {
            DateTime dtNow = DateTime.Now;
            IQueryable<VtrUploadtask> uploadQuery = Context.VtrUploadtask.AsNoTracking();
            uploadQuery.Where(a => a.Vtrtasktype == 1 && a.Taskstate == 2);
            IQueryable<DbpTask> taskQeruy = Context.DbpTask.AsNoTracking();
            taskQeruy.Where(a => a.Tasktype == 6 &&
                                 a.Tasklock == null &&
                                 a.SyncState == 0 &&
                                 a.Starttime < dtNow.AddMinutes(minute) &&
                                 a.Starttime > dtNow.AddDays(-1));

            return await GetUploadTaskContent(uploadQuery, taskQeruy);
        }

        /// <summary>
        /// The GetUploadTaskContent.
        /// </summary>
        /// <param name="uploadQuery">The uploadQuery<see cref="IQueryable{VtrUploadtask}"/>.</param>
        /// <param name="taskQeruy">The taskQeruy<see cref="IQueryable{DbpTask}"/>.</param>
        /// <returns>The <see cref="Task{List{VTRUploadTaskContentResponse}}"/>.</returns>
        private async Task<List<VTRUploadTaskContentResponse>> GetUploadTaskContent(IQueryable<VtrUploadtask> uploadQuery, IQueryable<DbpTask> taskQeruy)
        {
            var ab = await uploadQuery.Join(taskQeruy, a => a.Taskid, b => b.Taskid, (a, b) => new { task = b, vtr = a }).ToListAsync();
            if (ab != null && ab.Count > 0)
            {
                return ab.Select(a => new VTRUploadTaskContentResponse
                {
                    TaskId = a.task.Taskid,
                    TaskName = a.task.Taskname,
                    Unit = (int)a.task.Recunitid,
                    SignalId = (int)a.task.Signalid,
                    ChannelId = (int)a.task.Channelid,
                    State = (taskState)a.task.State,
                    BeginTime = a.task.Starttime,
                    EndTime = a.task.Endtime,
                    Classify = a.task.Category,
                    TaskDesc = a.task.Description,
                    StampImage = a.task.Description,
                    TaskType = (TaskType)a.task.Tasktype,
                    CooperantType = (CooperantType)a.task.Backtype,
                    TaskGUID = a.task.Taskguid,

                    VtrId = (int)a.vtr.Vtrid,
                    BlankTaskId = (int)a.vtr.Vtrtaskid,
                    TrimIn = (int)a.vtr.Trimin,
                    TrimOut = (int)a.vtr.Trimout,
                    TaskState = (VTRUPLOADTASKSTATE)a.vtr.Taskstate,
                    UserCode = a.vtr.Usercode,
                    CommitTime = a.vtr.Committime.ToString(),
                    Order = (int)a.vtr.Uploadorder,
                    TapeId = (int)a.vtr.Tapeid,
                    UserToken = a.vtr.Usertoken,
                    TrimInCTL = (int)a.vtr.Triminctl,
                    TrimOutCTL = (int)a.vtr.Trimoutctl,
                    VtrTaskType = (VTRUPLOADTASKTYPE)a.vtr.Vtrtasktype,
                }).ToList();
            }
            return new List<VTRUploadTaskContentResponse>();
        }

        /// <summary>
        /// The GetUploadtaskInfo.
        /// </summary>
        /// <param name="Condition">The Condition<see cref="VTRUploadConditionRequest"/>.</param>
        /// <param name="bTaskMoreThanZero">The bTaskMoreThanZero<see cref="bool"/>.</param>
        /// <returns>The <see cref="Task{List{VtrUploadtask}}"/>.</returns>
        public async Task<List<VtrUploadtask>> GetUploadtaskInfo(VTRUploadConditionRequest Condition, bool bTaskMoreThanZero)
        {
            IQueryable<VtrUploadtask> query = Context.VtrUploadtask.AsNoTracking();
            if (Condition.BlankTaskId > 0) query = query.Where(a => a.Vtrtaskid == Condition.BlankTaskId);
            if (string.IsNullOrEmpty(Condition.TaskName)) query = query.Where(a => a.Taskname.Contains(Condition.TaskName));
            if (!bTaskMoreThanZero && Condition.TaskId >= 0) query = query.Where(a => a.Taskid == Condition.TaskId);
            if (bTaskMoreThanZero && Condition.TaskId > 0) query = query.Where(a => a.Taskid == Condition.TaskId);
            if (Condition.VtrId > 0) query = query.Where(a => a.Vtrid == Condition.VtrId);
            if (Condition.TaskState != null && Condition.TaskState.Count > 0) query = query.Where(a => Condition.TaskState.Contains(a.Taskstate));
            if (!string.IsNullOrEmpty(Condition.UserCode)) query = query.Where(a => a.Usercode == Condition.UserCode);
            if (!string.IsNullOrEmpty(Condition.UserToken)) query = query.Where(a => a.Usertoken == Condition.UserToken);
            if (Condition.MaxCommitTime != DateTime.MinValue) query = query.Where(a => a.Committime <= Condition.MaxCommitTime);
            if (Condition.MinCommitTime != DateTime.MinValue) query = query.Where(a => a.Committime >= Condition.MinCommitTime);
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


        public async Task<List<DbpMetadatapolicy>> GetMetadatapoliciesByUserCode(string usercode)
        {
            return await Context.DbpPolicyuser.AsNoTracking().Where(x => x.Usercode == usercode).Join(Context.DbpMetadatapolicy, user => user.Policyid, policy => policy.Policyid, (x, y) => y).ToListAsync();
        }

    }
}
