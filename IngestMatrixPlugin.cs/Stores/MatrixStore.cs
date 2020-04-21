using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IngestMatrixPlugin.Models;
using IngestMatrixPlugin.Models.DB;
using Microsoft.EntityFrameworkCore;
using Sobey.Core.Log;

namespace IngestMatrixPlugin.Stores
{
    public class MatrixStore : IMatrixStore
    {
        public MatrixStore(IngestMatrixDBContext baseDataDbContext)
        {
            Context = baseDataDbContext;
        }
        private readonly ILogger Logger = LoggerManager.GetLogger("DeviceInfo");
        protected IngestMatrixDBContext Context { get; }

        public async Task<List<TResult>> QueryList<TEntity, TResult>(Func<IQueryable<TEntity>, IQueryable<TResult>> query, bool notrack = false) where TEntity : class
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

        public async Task<TResult> QueryModel<TEntity, TResult>(Func<IQueryable<TEntity>, Task<TResult>> query, bool notrack = false) where TEntity : class
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

        #region DbpLevelrelation
        public Task<List<TResult>> QueryLevelrelation<TResult>(Func<IQueryable<DbpLevelrelation>, IQueryable<TResult>> query, bool notrack = false)
        {
            return await QueryList(query, notrack);
        }

        public Task<TResult> QueryLevelrelation<TResult>(Func<IQueryable<DbpLevelrelation>, Task<TResult>> query, bool notrack = false)
        {
            return await QueryModel(query, notrack);
        }
        #endregion

        #region DbpMapoutport
        public async Task<List<TResult>> QueryMapoutportList<TResult>(Func<IQueryable<DbpMapoutport>, IQueryable<TResult>> query, bool notrack = false)
        {
            return await QueryList(query, notrack);
        }
        public async Task<TResult> QueryMapoutport<TResult>(Func<IQueryable<DbpMapoutport>, Task<TResult>> query, bool notrack = false)
        {
            return await QueryModel(query, notrack);
        }
        public bool GetRealMatrixOutPort(long lVirtualOutPort, ref long lOutPort, ref long lMatrixID)
        {
            var outport = Context.DbpMapoutport.SingleOrDefault(a => a.Virtualoutport == lVirtualOutPort);
            if (outport == null)
            {
                lOutPort = -1;
                lMatrixID = -1;
                return false;
            }
            lOutPort = Convert.ToInt64(outport.Outport);
            lMatrixID = Convert.ToInt64(outport.Matrixid);
            return true;
        }
        #endregion

        #region DbpMatrixrout
        public async Task<List<TResult>> QueryMatrixroutList<TResult>(Func<IQueryable<DbpMatrixrout>, IQueryable<TResult>> query, bool notrack = false)
        {
            return await QueryList(query, notrack);
        }
        public async Task<TResult> QueryMatrixrout<TResult>(Func<IQueryable<DbpMatrixrout>, Task<TResult>> query, bool notrack = false)
        {
            return await QueryModel(query, notrack);
        }
        public async Task<int> DeleteMatrixrout(Func<IQueryable<DbpMatrixrout>, IQueryable<DbpMatrixrout>> query)
        {
            var deleteList = await query(Context.DbpMatrixrout).ToListAsync();
            Context.DbpMatrixrout.RemoveRange(deleteList);
            return await Context.SaveChangesAsync();
        }
        public async Task<int> AddRangeMatrixrout(List<DbpMatrixrout> dbps)
        {
            Context.DbpMatrixrout.AddRange(dbps);
            return await Context.SaveChangesAsync();
        }
        public async Task<int> AddMatrixrout(DbpMatrixrout dbps)
        {
            Context.DbpMatrixrout.AddRange(dbps);
            return await Context.SaveChangesAsync();
        }
        #endregion

        #region DbpMatrixinfo
        public async Task<List<TResult>> QueryMatrixinfo<TResult>(Func<IQueryable<DbpMatrixinfo>, IQueryable<TResult>> query, bool notrack = false)
        {
            return await QueryList(query, notrack);
        }
        public async Task<TResult> QueryMatrixinfo<TResult>(Func<IQueryable<DbpMatrixrout>, Task<TResult>> query, bool notrack = false)
        {
            return await QueryModel(query, notrack);
        }
        #endregion

        #region   DbpVirtualmatrixportstate
        public async Task<List<TResult>> QueryVirtualmatrixportstate<TResult>(Func<IQueryable<DbpVirtualmatrixportstate>, IQueryable<TResult>> query, bool notrack = false)
        {
            return await QueryList(query, notrack);
        }
        public async Task<TResult> QueryVirtualmatrixportstate<TResult>(Func<IQueryable<DbpVirtualmatrixportstate>, Task<TResult>> query, bool notrack = false)
        {
            return await QueryModel(query, notrack);
        }
        public async Task<int> AddRangeVirtualmatrixportstate(List<DbpVirtualmatrixportstate> dbps)
        {
            Context.DbpVirtualmatrixportstate.AddRange(dbps);
            return await Context.SaveChangesAsync();
        }
        public async Task<int> AddVirtualmatrixportstate(DbpVirtualmatrixportstate dbps)
        {
            Context.DbpVirtualmatrixportstate.AddRange(dbps);
            return await Context.SaveChangesAsync();
        }
        #endregion

        public async Task<bool> UpdatePortInfo(int lInPort, int lOutPort, int bState)
        {
            var matrixId = await Context.DbpMatrixinfo.Where(a => a.Matrixtypeid == 1).Select(a => a.Matrixid).SingleAsync();
            var hasData = await Context.DbpVirtualmatrixportstate.Where(a => a.Virtualinport == lInPort && a.Virtualoutport == lOutPort).ToListAsync();
            if (hasData.Count > 0)
            {
                hasData.ForEach(a => a.State = bState);
                await Context.SaveChangesAsync();
            }
            else
            {
                var addData = new DbpVirtualmatrixportstate
                {
                    Virtualinport = lInPort,
                    Virtualoutport = lOutPort,
                    State = bState,
                    Matrixid = matrixId,
                    Lastoprtime = DateTime.Now
                };
            }
            return true;
        }
    }
}
