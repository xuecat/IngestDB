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
        private readonly ILogger Logger = LoggerManager.GetLogger("MatrixStore");
        protected IngestMatrixDBContext Context { get; }

        public Task<List<TResult>> QueryList<TEntity, TResult>(Func<IQueryable<TEntity>, IQueryable<TResult>> query, bool notrack = false) where TEntity : class
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            if (notrack)
            {
                return query(Context.Set<TEntity>().AsNoTracking()).ToListAsync();
            }
            return query(Context.Set<TEntity>()).ToListAsync();
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
        public async Task<List<TResult>> QueryLevelrelation<TResult>(Func<IQueryable<DbpLevelrelation>, IQueryable<TResult>> query, bool notrack = false)
        {
            return await QueryList(query, notrack);
        }
        public async Task<TResult> QueryLevelrelation<TResult>(Func<IQueryable<DbpLevelrelation>, Task<TResult>> query, bool notrack = false)
        {
            return await QueryModel(query, notrack);
        }
        #endregion

        #region DbpMapinport
        public async Task<List<TResult>> QueryMapinport<TResult>(Func<IQueryable<DbpMapinport>, IQueryable<TResult>> query, bool notrack = false)
        {
            return await QueryList(query, notrack);
        }
        public async Task<TResult> QueryMapinport<TResult>(Func<IQueryable<DbpMapinport>, Task<TResult>> query, bool notrack = false)
        {
            return await QueryModel(query, notrack);
        }
        public bool GetVirtualMapInPort(long lMatrixID, long lRealInPort, ref long lVirtualInPort)
        {
            var inport = Context.DbpMapinport.SingleOrDefault(a => a.Matrixid == lMatrixID && a.Inport == lRealInPort);
            if (inport == null)
            {
                lVirtualInPort = -1;
                return false;
            }
            lVirtualInPort = inport.Virtualinport;
            return true;
        }
        #endregion

        #region DbpMapoutport
        public async Task<List<TResult>> QueryMapoutport<TResult>(Func<IQueryable<DbpMapoutport>, IQueryable<TResult>> query, bool notrack = false)
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
            lOutPort = outport.Outport;
            lMatrixID = outport.Matrixid;
            return true;
        }
        #endregion

        #region DbpMatrixrout
        public async Task<List<TResult>> QueryMatrixrout<TResult>(Func<IQueryable<DbpMatrixrout>, IQueryable<TResult>> query, bool notrack = false)
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
        public async Task<int> AddOrUpdateMatrixrout(IEnumerable<DbpMatrixrout> saveList, bool savechange)
        {
            foreach (var matrixRout in saveList)
            {
                var matrix = await Context.DbpMatrixrout.SingleOrDefaultAsync(a => a.Matrixid == matrixRout.Matrixid &&
                a.Virtualoutport == matrixRout.Virtualoutport);

                if (matrix != null)
                {
                    //matrix = matrixRout;

                    matrix.Inport = matrixRout.Inport;
                    matrix.Matrixid = matrixRout.Matrixid;
                    matrix.Outport = matrixRout.Outport;
                    matrix.State = matrixRout.State;
                    matrix.Virtualinport = matrixRout.Virtualinport;
                    matrix.Virtualoutport = matrixRout.Virtualoutport;
                    if(matrixRout.Begintime != DateTime.MinValue)
                    {
                        matrix.Begintime = matrixRout.Begintime;
                    }
                    if (matrixRout.Endtime != DateTime.MinValue)
                    {
                        matrix.Endtime = matrixRout.Endtime;
                    }
                }
                else
                    Context.DbpMatrixrout.Add(matrixRout);
            }

            if (savechange)
            {
                try
                {
                    return await Context.SaveChangesAsync();
                }
                catch (Exception e)
                {

                    throw e;
                }
               
            }
            return 1;
        }
        public async Task<int> UpdateRangeMatrixrout(List<DbpMatrixrout> dbps, bool savechange)
        {
            foreach (var item in dbps)
            {
                var itm = await Context.DbpMatrixrout.Where(x => x.Matrixid == item.Matrixid && x.Virtualoutport == item.Virtualoutport).SingleOrDefaultAsync();
                if (itm != null)
                {
                    itm.Inport = item.Inport;
                    itm.Matrixid = item.Matrixid;
                    itm.Outport = item.Outport;
                    itm.State = item.State;
                    itm.Virtualinport = item.Virtualinport;
                    itm.Virtualoutport = item.Virtualoutport;
                    itm.Begintime = item.Begintime;
                    itm.Endtime = item.Endtime;
                }
                else
                {
                    Context.DbpMatrixrout.Add(item);
                }
            }

            try
            {
                if (savechange)
                {
                    return await Context.SaveChangesAsync();
                }
            }
            catch (Exception e)
            {

                throw e;
            }

            return dbps.Count;
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
        public async Task<TResult> QueryMatrixinfo<TResult>(Func<IQueryable<DbpMatrixinfo>, Task<TResult>> query, bool notrack = false)
        {
            return await QueryModel(query, notrack);
        }
        #endregion

        #region DbpMatrixtypeinfo
        public async Task<List<TResult>> QueryMatrixtypeinfo<TResult>(Func<IQueryable<DbpMatrixtypeinfo>, IQueryable<TResult>> query, bool notrack = false)
        {
            return await QueryList(query, notrack);
        }
        public async Task<TResult> QueryMatrixtypeinfo<TResult>(Func<IQueryable<DbpMatrixtypeinfo>, Task<TResult>> query, bool notrack = false)
        {
            return await QueryModel(query, notrack);
        }
        #endregion

        #region DbpRcdindesc
        public Task<List<TResult>> QueryRcdindesc<TResult>(Func<IQueryable<DbpRcdindesc>, IQueryable<TResult>> query, bool notrack = false)
        {
            return QueryList(query, notrack);
        }
        public async Task<TResult> QueryRcdindesc<TResult>(Func<IQueryable<DbpRcdindesc>, Task<TResult>> query, bool notrack = false)
        {
            return await QueryModel(query, notrack);
        }
        #endregion

        #region DbpRcdoutdesc
        public async Task<List<TResult>> QueryRcdoutdesc<TResult>(Func<IQueryable<DbpRcdoutdesc>, IQueryable<TResult>> query, bool notrack = false)
        {
            return await QueryList(query, notrack);
        }
        public async Task<TResult> QueryRcdoutdesc<TResult>(Func<IQueryable<DbpRcdoutdesc>, Task<TResult>> query, bool notrack = false)
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
        public async Task<int> UpdateVirtualmatrixportstate(DbpVirtualmatrixportstate dbps, bool savechange)
        {
            if (dbps == null)
            {
                return 0;
            }

            var item = await Context.DbpVirtualmatrixportstate.Where(x => x.Virtualinport == dbps.Virtualinport && x.Virtualoutport == dbps.Virtualoutport).SingleOrDefaultAsync();
            if (item == null)
            {
                Context.DbpVirtualmatrixportstate.Add(dbps);
            }
            else
            {
                item.Virtualinport = dbps.Virtualinport;
                item.Virtualoutport = dbps.Virtualoutport;
                item.State = dbps.State;
                item.Matrixid = dbps.Matrixid;
            }
            if (savechange)
            {
                return await Context.SaveChangesAsync();
            }
            return 1;    
        }
        #endregion

        public async Task<bool> UpdatePortInfo(long lInPort, long lOutPort, int bState, bool savechange)
        {
            var matrixId = await Context.DbpMatrixinfo.AsNoTracking().Where(a => a.Matrixtypeid == 1).Select(a => a.Matrixid).SingleAsync();
            var hasData = await Context.DbpVirtualmatrixportstate.Where(a => a.Virtualinport == lInPort && a.Virtualoutport == lOutPort).SingleOrDefaultAsync();

            if (hasData != null)
            {
                hasData.State = bState;
                hasData.Lastoprtime = DateTime.Now;
                hasData.Virtualinport = (int)lInPort;
                hasData.Virtualoutport = (int)lOutPort;
            }
            else
            {
                Context.DbpVirtualmatrixportstate.Add(new DbpVirtualmatrixportstate() {
                    Virtualinport = (int)lInPort,
                    Virtualoutport = (int)lOutPort,
                    Lastoprtime = DateTime.Now,
                    Matrixid = matrixId,
                    State = bState
                });
            }

            if (savechange)
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
           
            return true;
        }


    }
}
