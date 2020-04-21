using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IngestMatrixPlugin.Models.DB;

namespace IngestMatrixPlugin.Stores
{
    public interface IMatrixStore
    {
        #region DbpLevelrelation
        /// <summary>查询层次关系</summary>
        Task<List<TResult>> QueryLevelrelation<TResult>(Func<IQueryable<DbpLevelrelation>, IQueryable<TResult>> query, bool notrack = false);
        /// <summary>查询层次关系</summary>
        Task<TResult> QueryLevelrelation<TResult>(Func<IQueryable<DbpLevelrelation>, Task<TResult>> query, bool notrack = false);
        #endregion

        #region DbpMapoutport
        /// <summary>查询矩阵的输出端口</summary>
        Task<List<TResult>> QueryMapoutportList<TResult>(Func<IQueryable<DbpMapoutport>, IQueryable<TResult>> query, bool notrack = false);
        /// <summary>查询矩阵的输出端口</summary>
        Task<TResult> QueryMapoutport<TResult>(Func<IQueryable<DbpMapoutport>, Task<TResult>> query, bool notrack = false);
        /// <summary>查询真实矩阵的输出端口</summary>
        bool GetRealMatrixOutPort(long lVirtualOutPort, ref long lOutPort, ref long lMatrixID);
        #endregion

        #region DbpMatrixinfo
        /// <summary>查询矩阵rout</summary>
        Task<List<TResult>> QueryMatrixroutList<TResult>(Func<IQueryable<DbpMatrixrout>, IQueryable<TResult>> query, bool notrack = false);
        /// <summary>查询矩阵rout</summary>
        Task<TResult> QueryMatrixrout<TResult>(Func<IQueryable<DbpMatrixrout>, Task<TResult>> query, bool notrack = false);
        /// <summary>删除矩阵rout</summary>
        Task<int> DeleteMatrixrout(Func<IQueryable<DbpMatrixrout>, IQueryable<DbpMatrixrout>> query);
        /// <summary>添加矩阵rout</summary>
        Task<int> AddRangeMatrixrout(List<DbpMatrixrout> dbps);
        /// <summary>添加矩阵rout</summary>
        Task<int> AddMatrixrout(DbpMatrixrout dbps);
        #endregion

        #region DbpMatrixinfo
        /// <summary>查询矩阵信息</summary>
        Task<List<TResult>> QueryMatrixinfo<TResult>(Func<IQueryable<DbpMatrixinfo>, IQueryable<TResult>> query, bool notrack = false);
        /// <summary>查询矩阵信息</summary>
        Task<TResult> QueryMatrixinfo<TResult>(Func<IQueryable<DbpMatrixrout>, Task<TResult>> query, bool notrack = false);
        #endregion

        #region DbpVirtualmatrixportstate
        /// <summary>查询虚拟矩阵端口状态</summary>
        Task<List<TResult>> QueryVirtualmatrixportstate<TResult>(Func<IQueryable<DbpVirtualmatrixportstate>, IQueryable<TResult>> query, bool notrack = false);
        /// <summary>查询虚拟矩阵端口状态</summary>
        Task<TResult> QueryVirtualmatrixportstate<TResult>(Func<IQueryable<DbpVirtualmatrixportstate>, Task<TResult>> query, bool notrack = false);
        /// <summary>添加矩阵rout</summary>
        Task<int> AddRangeVirtualmatrixportstate(List<DbpVirtualmatrixportstate> dbps);
        /// <summary>添加矩阵rout</summary>
        Task<int> AddVirtualmatrixportstate(DbpVirtualmatrixportstate dbps);
        #endregion
                
        /// <summary>更新端口信息</summary>
        Task<bool> UpdatePortInfo(int lInPort, int lOutPort, int bState);



    }
}
