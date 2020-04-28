namespace IngestTaskPlugin.Stores.VTR
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using IngestTaskPlugin.Dto.Request;
    using IngestTaskPlugin.Dto.Response.OldVtr;
    using IngestTaskPlugin.Models;

    /// <summary>
    /// VTR.
    /// </summary>
    public interface IVtrStore
    {
        Task<List<TResult>> GetMetadatapolicy<TResult>(Func<IQueryable<DbpMetadatapolicy>, IQueryable<TResult>> query, bool notrack = false);

        Task<TResult> GetMetadatapolicy<TResult>(Func<IQueryable<DbpMetadatapolicy>, Task<TResult>> query, bool notrack = false);

        Task<List<TResult>> GetPolicyuser<TResult>(Func<IQueryable<DbpPolicyuser>, IQueryable<TResult>> query, bool notrack = false);

        Task<TResult> GetPolicyuser<TResult>(Func<IQueryable<DbpPolicyuser>, Task<TResult>> query, bool notrack = false);

        /// <summary>
        /// The GetTapelist.
        /// </summary>
        /// <typeparam name="TResult">.</typeparam>
        /// <param name="query">The query<see cref="Func{IQueryable{VtrTapelist}, IQueryable{TResult}}"/>.</param>
        /// <param name="notrack">The notrack<see cref="bool"/>.</param>
        /// <returns>The <see cref="Task{List{TResult}}"/>.</returns>
        Task<List<TResult>> GetTapelist<TResult>(Func<IQueryable<VtrTapelist>, IQueryable<TResult>> query, bool notrack = false);

        /// <summary>
        /// The GetTapelist.
        /// </summary>
        /// <typeparam name="TResult">.</typeparam>
        /// <param name="query">The query<see cref="Func{IQueryable{VtrTapelist}, Task{TResult}}"/>.</param>
        /// <param name="notrack">The notrack<see cref="bool"/>.</param>
        /// <returns>The <see cref="Task{TResult}"/>.</returns>
        Task<TResult> GetTapelist<TResult>(Func<IQueryable<VtrTapelist>, Task<TResult>> query, bool notrack = false);

        /// <summary>
        /// The SaveTaplist.
        /// </summary>
        /// <param name="tapelists">The tapelists<see cref="IEnumerable{VtrTapelist}"/>.</param>
        /// <returns>The <see cref="Task{bool}"/>.</returns>
        Task<bool> SaveTaplist(IEnumerable<VtrTapelist> tapelists);

        /// <summary>
        /// The SaveTaplist.
        /// </summary>
        /// <param name="tapelist">The tapelist<see cref="VtrTapelist"/>.</param>
        /// <returns>The <see cref="Task{int}"/>.</returns>
        Task<int> SaveTaplist(VtrTapelist tapelist);

        /// <summary>
        /// The GetTapeVtrMap.
        /// </summary>
        /// <typeparam name="TResult">.</typeparam>
        /// <param name="query">The query<see cref="Func{IQueryable{VtrTapeVtrMap}, IQueryable{TResult}}"/>.</param>
        /// <param name="notrack">The notrack<see cref="bool"/>.</param>
        /// <returns>The <see cref="Task{List{TResult}}"/>.</returns>
        Task<List<TResult>> GetTapeVtrMap<TResult>(Func<IQueryable<VtrTapeVtrMap>, IQueryable<TResult>> query, bool notrack = false);

        /// <summary>
        /// The GetTapeVtrMap.
        /// </summary>
        /// <typeparam name="TResult">.</typeparam>
        /// <param name="query">The query<see cref="Func{IQueryable{VtrTapeVtrMap}, Task{TResult}}"/>.</param>
        /// <param name="notrack">The notrack<see cref="bool"/>.</param>
        /// <returns>The <see cref="Task{TResult}"/>.</returns>
        Task<TResult> GetTapeVtrMap<TResult>(Func<IQueryable<VtrTapeVtrMap>, Task<TResult>> query, bool notrack = false);

        /// <summary>
        /// The SaveTapeVtrMap.
        /// </summary>
        /// <param name="tapeMap">The tapeMap<see cref="IEnumerable{VtrTapeVtrMap}"/>.</param>
        /// <returns>The <see cref="Task{List{bool}}"/>.</returns>
        Task<List<bool>> SaveTapeVtrMap(IEnumerable<VtrTapeVtrMap> tapeMap);

        /// <summary>
        /// The SaveTapeVtrMap.
        /// </summary>
        /// <param name="tapeMap">The tapeMap<see cref="VtrTapeVtrMap"/>.</param>
        /// <returns>The <see cref="Task{bool}"/>.</returns>
        Task<bool> SaveTapeVtrMap(VtrTapeVtrMap tapeMap);

        /// <summary>
        /// The GetTask.
        /// </summary>
        /// <typeparam name="TResult">.</typeparam>
        /// <param name="query">The query<see cref="Func{IQueryable{DbpTask}, Task{TResult}}"/>.</param>
        /// <param name="notrack">The notrack<see cref="bool"/>.</param>
        /// <returns>The <see cref="Task{TResult}"/>.</returns>
        Task<TResult> GetTask<TResult>(Func<IQueryable<DbpTask>, Task<TResult>> query, bool notrack = false);

        /// <summary>
        /// The GetTypeinfo.
        /// </summary>
        /// <typeparam name="TResult">.</typeparam>
        /// <param name="query">The query<see cref="Func{IQueryable{VtrTypeinfo}, IQueryable{TResult}}"/>.</param>
        /// <param name="notrack">The notrack<see cref="bool"/>.</param>
        /// <returns>The <see cref="Task{List{TResult}}"/>.</returns>
        Task<List<TResult>> GetTypeinfo<TResult>(Func<IQueryable<VtrTypeinfo>, IQueryable<TResult>> query, bool notrack = false);

        /// <summary>
        /// The GetTypeinfo.
        /// </summary>
        /// <typeparam name="TResult">.</typeparam>
        /// <param name="query">The query<see cref="Func{IQueryable{VtrTypeinfo}, Task{TResult}}"/>.</param>
        /// <param name="notrack">The notrack<see cref="bool"/>.</param>
        /// <returns>The <see cref="Task{TResult}"/>.</returns>
        Task<TResult> GetTypeinfo<TResult>(Func<IQueryable<VtrTypeinfo>, Task<TResult>> query, bool notrack = false);

        /// <summary>
        /// The GetDetailinfo.
        /// </summary>
        /// <typeparam name="TResult">.</typeparam>
        /// <param name="query">The query<see cref="Func{IQueryable{VtrDetailinfo}, IQueryable{TResult}}"/>.</param>
        /// <param name="notrack">The notrack<see cref="bool"/>.</param>
        /// <returns>The <see cref="Task{List{TResult}}"/>.</returns>
        Task<List<TResult>> GetDetailinfo<TResult>(Func<IQueryable<VtrDetailinfo>, IQueryable<TResult>> query, bool notrack = false);

        /// <summary>
        /// The GetDetailinfo.
        /// </summary>
        /// <typeparam name="TResult">.</typeparam>
        /// <param name="query">The query<see cref="Func{IQueryable{VtrDetailinfo}, Task{TResult}}"/>.</param>
        /// <param name="notrack">The notrack<see cref="bool"/>.</param>
        /// <returns>The <see cref="Task{TResult}"/>.</returns>
        Task<TResult> GetDetailinfo<TResult>(Func<IQueryable<VtrDetailinfo>, Task<TResult>> query, bool notrack = false);

        Task<List<VTRUploadTaskContent>> GetUploadTaskContent(VTRUploadCondition Condition);

        Task<List<VtrUploadtask>> GetUploadtaskInfo(VTRUploadCondition Condition, bool bTaskMoreThanZero);

        /// <summary>
        /// The GetUploadtask.
        /// </summary>
        /// <typeparam name="TResult">.</typeparam>
        /// <param name="query">The query<see cref="Func{IQueryable{VtrUploadtask}, IQueryable{TResult}}"/>.</param>
        /// <param name="notrack">The notrack<see cref="bool"/>.</param>
        /// <returns>The <see cref="Task{List{TResult}}"/>.</returns>
        Task<List<TResult>> GetUploadtask<TResult>(Func<IQueryable<VtrUploadtask>, IQueryable<TResult>> query, bool notrack = false);

        /// <summary>
        /// The GetUploadtask.
        /// </summary>
        /// <typeparam name="TResult">.</typeparam>
        /// <param name="query">The query<see cref="Func{IQueryable{VtrUploadtask}, Task{TResult}}"/>.</param>
        /// <param name="notrack">The notrack<see cref="bool"/>.</param>
        /// <returns>The <see cref="Task{TResult}"/>.</returns>
        Task<TResult> GetUploadtask<TResult>(Func<IQueryable<VtrUploadtask>, Task<TResult>> query, bool notrack = false);

        /// <summary>
        /// The AddUploadtask.
        /// </summary>
        /// <param name="task">The task<see cref="VtrUploadtask"/>.</param>
        /// <returns>The <see cref="Task{bool}"/>.</returns>
        Task<bool> AddUploadtask(VtrUploadtask task);

        Task<bool> UpdateUploadtask(VtrUploadtask task);
    }
}
