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

        #region DbpMapinport
        /// <summary>查询矩阵的输入端口</summary>
        Task<List<TResult>> QueryMapinport<TResult>(Func<IQueryable<DbpMapinport>, IQueryable<TResult>> query, bool notrack = false);
        /// <summary>查询矩阵的输入端口</summary>
        Task<TResult> QueryMapinport<TResult>(Func<IQueryable<DbpMapinport>, Task<TResult>> query, bool notrack = false);
        /// <summary>查询真实矩阵的输入端口</summary>
        bool GetVirtualMapInPort(long lMatrixID, long lRealInPort, ref long lVirtualInPort);
        #endregion

        #region DbpMapoutport
        /// <summary>查询矩阵的输出端口</summary>
        Task<List<TResult>> QueryMapoutport<TResult>(Func<IQueryable<DbpMapoutport>, IQueryable<TResult>> query, bool notrack = false);
        /// <summary>查询矩阵的输出端口</summary>
        Task<TResult> QueryMapoutport<TResult>(Func<IQueryable<DbpMapoutport>, Task<TResult>> query, bool notrack = false);
        /// <summary>查询真实矩阵的输出端口</summary>
        bool GetRealMatrixOutPort(long lVirtualOutPort, ref long lOutPort, ref long lMatrixID);
        #endregion

        #region DbpMatrixrout
        /// <summary>查询矩阵路由</summary>
        Task<List<TResult>> QueryMatrixrout<TResult>(Func<IQueryable<DbpMatrixrout>, IQueryable<TResult>> query, bool notrack = false);
        /// <summary>查询矩阵路由</summary>
        Task<TResult> QueryMatrixrout<TResult>(Func<IQueryable<DbpMatrixrout>, Task<TResult>> query, bool notrack = false);
        /// <summary>删除矩阵路由</summary>
        Task<int> DeleteMatrixrout(Func<IQueryable<DbpMatrixrout>, IQueryable<DbpMatrixrout>> query);
        /// <summary>添加或更新矩阵路由</summary>
        Task<int> AddOrUpdateMatrixrout(IEnumerable<DbpMatrixrout> saveList, bool savechange);
        /// <summary>添加矩阵路由</summary>
        Task<int> UpdateRangeMatrixrout(List<DbpMatrixrout> dbps, bool savechange);
        /// <summary>添加矩阵路由</summary>
        Task<int> AddMatrixrout(DbpMatrixrout dbps);
        #endregion

        #region DbpMatrixinfo
        /// <summary>查询矩阵信息</summary>
        Task<List<TResult>> QueryMatrixinfo<TResult>(Func<IQueryable<DbpMatrixinfo>, IQueryable<TResult>> query, bool notrack = false);
        /// <summary>查询矩阵信息</summary>
        Task<TResult> QueryMatrixinfo<TResult>(Func<IQueryable<DbpMatrixinfo>, Task<TResult>> query, bool notrack = false);
        #endregion

        #region DbpMatrixtypeinfo
        /// <summary>查询矩阵信息</summary>
        Task<List<TResult>> QueryMatrixtypeinfo<TResult>(Func<IQueryable<DbpMatrixtypeinfo>, IQueryable<TResult>> query, bool notrack = false);
        /// <summary>查询矩阵信息</summary>
        Task<TResult> QueryMatrixtypeinfo<TResult>(Func<IQueryable<DbpMatrixtypeinfo>, Task<TResult>> query, bool notrack = false);
        #endregion

        #region DbpRcdindesc
        /// <summary>查询矩阵信息</summary>
        Task<List<TResult>> QueryRcdindesc<TResult>(Func<IQueryable<DbpRcdindesc>, IQueryable<TResult>> query, bool notrack = false);
        /// <summary>查询矩阵信息</summary>
        Task<TResult> QueryRcdindesc<TResult>(Func<IQueryable<DbpRcdindesc>, Task<TResult>> query, bool notrack = false);
        #endregion

        #region DbpRcdoutdesc
        /// <summary>查询矩阵信息</summary>
        Task<List<TResult>> QueryRcdoutdesc<TResult>(Func<IQueryable<DbpRcdoutdesc>, IQueryable<TResult>> query, bool notrack = false);
        /// <summary>查询矩阵信息</summary>
        Task<TResult> QueryRcdoutdesc<TResult>(Func<IQueryable<DbpRcdoutdesc>, Task<TResult>> query, bool notrack = false);
        #endregion

        #region DbpVirtualmatrixportstate
        /// <summary>查询虚拟矩阵端口状态</summary>
        Task<List<TResult>> QueryVirtualmatrixportstate<TResult>(Func<IQueryable<DbpVirtualmatrixportstate>, IQueryable<TResult>> query, bool notrack = false);
        /// <summary>查询虚拟矩阵端口状态</summary>
        Task<TResult> QueryVirtualmatrixportstate<TResult>(Func<IQueryable<DbpVirtualmatrixportstate>, Task<TResult>> query, bool notrack = false);
        /// <summary>添加矩阵rout</summary>
        Task<int> AddRangeVirtualmatrixportstate(List<DbpVirtualmatrixportstate> dbps);
        /// <summary>添加矩阵rout</summary>
        Task<int> UpdateVirtualmatrixportstate(DbpVirtualmatrixportstate dbps , bool savechange);
        #endregion

        /// <summary>更新端口信息 ture:1, false:0</summary>
        Task<bool> UpdatePortInfo(long lInPort, long lOutPort, int bState, bool savechange);

        Task<List<DbpUserLoginInfo>> GetAllUserLoginInfos();
    }
}
