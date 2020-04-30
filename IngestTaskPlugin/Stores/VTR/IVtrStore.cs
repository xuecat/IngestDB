namespace IngestTaskPlugin.Stores.VTR
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using IngestTaskPlugin.Dto.Request;
    using IngestTaskPlugin.Dto.Response;
    using IngestTaskPlugin.Dto.Response.OldVtr;
    using IngestTaskPlugin.Models;

    /// <summary>
    /// VTR.
    /// </summary>
    public interface IVtrStore
    {
        /// <summary>
        /// The 查询元数据策略.
        /// </summary>
        /// <typeparam name="TResult">返回类型.</typeparam>
        /// <param name="query">The 条件<see cref="Func{IQueryable{DbpMetadatapolicy}, IQueryable{TResult}}"/>.</param>
        /// <param name="notrack">The 是否跟踪<see cref="bool"/>.</param>
        /// <returns>The 查询结果<see cref="Task{List{TResult}}"/>.</returns>
        Task<List<TResult>> GetMetadatapolicy<TResult>(Func<IQueryable<DbpMetadatapolicy>, IQueryable<TResult>> query, bool notrack = false);

        /// <summary>
        /// The 查询元数据策略.
        /// </summary>
        /// <typeparam name="TResult">返回类型.</typeparam>
        /// <param name="query">The 条件<see cref="Func{IQueryable{DbpMetadatapolicy}, Task{TResult}}"/>.</param>
        /// <param name="notrack">The 是否跟踪<see cref="bool"/>.</param>
        /// <returns>The 查询结果<see cref="Task{TResult}"/>.</returns>
        Task<TResult> GetMetadatapolicy<TResult>(Func<IQueryable<DbpMetadatapolicy>, Task<TResult>> query, bool notrack = false);

        /// <summary>
        /// The 查询用户策略.
        /// </summary>
        /// <typeparam name="TResult">返回类型.</typeparam>
        /// <param name="query">The 条件<see cref="Func{IQueryable{DbpPolicyuser}, IQueryable{TResult}}"/>.</param>
        /// <param name="notrack">The 是否跟踪<see cref="bool"/>.</param>
        /// <returns>The 查询结果 <see cref="Task{List{TResult}}"/>.</returns>
        Task<List<TResult>> GetPolicyuser<TResult>(Func<IQueryable<DbpPolicyuser>, IQueryable<TResult>> query, bool notrack = false);

        /// <summary>
        /// The 查询用户策略.
        /// </summary>
        /// <typeparam name="TResult">返回类型.</typeparam>
        /// <param name="query">The 条件<see cref="Func{IQueryable{DbpPolicyuser}, Task{TResult}}"/>.</param>
        /// <param name="notrack">The 是否跟踪<see cref="bool"/>.</param>
        /// <returns>The 查询结果 <see cref="Task{TResult}"/>.</returns>
        Task<TResult> GetPolicyuser<TResult>(Func<IQueryable<DbpPolicyuser>, Task<TResult>> query, bool notrack = false);

        /// <summary>
        /// The 任务元数据备份.
        /// </summary>
        /// <typeparam name="TResult">返回类型.</typeparam>
        /// <param name="query">The 条件<see cref="Func{IQueryable{DbpTaskMetadataBackup}, IQueryable{TResult}}"/>.</param>
        /// <param name="notrack">The 是否跟踪<see cref="bool"/>.</param>
        /// <returns>The 查询结果 <see cref="Task{List{TResult}}"/>.</returns>
        Task<List<TResult>> GetTaskMetadataBackup<TResult>(Func<IQueryable<DbpTaskMetadataBackup>, IQueryable<TResult>> query, bool notrack = false);

        /// <summary>
        /// The 任务元数据备份.
        /// </summary>
        /// <typeparam name="TResult">返回类型.</typeparam>
        /// <param name="query">The 条件<see cref="Func{IQueryable{DbpTaskMetadataBackup}, Task{TResult}}"/>.</param>
        /// <param name="notrack">The 是否跟踪<see cref="bool"/>.</param>
        /// <returns>The 查询结果 <see cref="Task{TResult}"/>.</returns>
        Task<TResult> GetTaskMetadataBackup<TResult>(Func<IQueryable<DbpTaskMetadataBackup>, Task<TResult>> query, bool notrack = false);

        /// <summary>
        /// The 查询磁带列表.
        /// </summary>
        /// <typeparam name="TResult">返回类型.</typeparam>
        /// <param name="query">The 条件<see cref="Func{IQueryable{VtrTapelist}, IQueryable{TResult}}"/>.</param>
        /// <param name="notrack">The 是否跟踪<see cref="bool"/>.</param>
        /// <returns>The 查询结果 <see cref="Task{List{TResult}}"/>.</returns>
        Task<List<TResult>> GetTapelist<TResult>(Func<IQueryable<VtrTapelist>, IQueryable<TResult>> query, bool notrack = false);

        /// <summary>
        /// The 查询磁带列表.
        /// </summary>
        /// <typeparam name="TResult">返回类型.</typeparam>
        /// <param name="query">The 条件<see cref="Func{IQueryable{VtrTapelist}, Task{TResult}}"/>.</param>
        /// <param name="notrack">The 是否跟踪<see cref="bool"/>.</param>
        /// <returns>The 查询结果 <see cref="Task{TResult}"/>.</returns>
        Task<TResult> GetTapelist<TResult>(Func<IQueryable<VtrTapelist>, Task<TResult>> query, bool notrack = false);

        /// <summary>
        /// The 保存磁带列表.
        /// </summary>
        /// <param name="tapelists">The 磁带列表<see cref="IEnumerable{VtrTapelist}"/>.</param>
        /// <returns>The 是否成功<see cref="Task{bool}"/>.</returns>
        Task<bool> SaveTaplist(IEnumerable<VtrTapelist> tapelists);

        /// <summary>
        /// The 保存磁带列表.
        /// </summary>
        /// <param name="tapelist">The 磁带列表<see cref="VtrTapelist"/>.</param>
        /// <returns>The 磁带Id <see cref="Task{int}"/>.</returns>
        Task<int> SaveTaplist(VtrTapelist tapelist);

        /// <summary>
        /// The 获取Vtr磁带映射.
        /// </summary>
        /// <typeparam name="TResult">返回类型.</typeparam>
        /// <param name="query">The 条件<see cref="Func{IQueryable{VtrTapeVtrMap}, IQueryable{TResult}}"/>.</param>
        /// <param name="notrack">The 是否跟踪<see cref="bool"/>.</param>
        /// <returns>The 查询结果<see cref="Task{List{TResult}}"/>.</returns>
        Task<List<TResult>> GetTapeVtrMap<TResult>(Func<IQueryable<VtrTapeVtrMap>, IQueryable<TResult>> query, bool notrack = false);

        /// <summary>
        /// The 获取Vtr磁带映射.
        /// </summary>
        /// <typeparam name="TResult">返回类型.</typeparam>
        /// <param name="query">The 条件<see cref="Func{IQueryable{VtrTapeVtrMap}, Task{TResult}}"/>.</param>
        /// <param name="notrack">The 是否跟踪<see cref="bool"/>.</param>
        /// <returns>The 查询结果<see cref="Task{TResult}"/>.</returns>
        Task<TResult> GetTapeVtrMap<TResult>(Func<IQueryable<VtrTapeVtrMap>, Task<TResult>> query, bool notrack = false);

        /// <summary>
        /// The 新增或修改 Vtr磁带映射.
        /// </summary>
        /// <param name="tapeMap">The 保存的磁带映射<see cref="IEnumerable{VtrTapeVtrMap}"/>.</param>
        /// <returns>The 是否成功<see cref="Task{List{bool}}"/>.</returns>
        Task<List<bool>> SaveTapeVtrMap(IEnumerable<VtrTapeVtrMap> tapeMap);

        /// <summary>
        /// The 新增或修改 Vtr磁带映射.
        /// </summary>
        /// <param name="tapeMap">The 保存的磁带映射<see cref="VtrTapeVtrMap"/>.</param>
        /// <returns>The 是否成功<see cref="Task{bool}"/>.</returns>
        Task<bool> SaveTapeVtrMap(VtrTapeVtrMap tapeMap);

        /// <summary>
        /// The 查询Task任务.
        /// </summary>
        /// <typeparam name="TResult">返回类型.</typeparam>
        /// <param name="query">The 条件<see cref="Func{IQueryable{DbpTask}, Task{TResult}}"/>.</param>
        /// <param name="notrack">The 是否追踪<see cref="bool"/>.</param>
        /// <returns>The 查询结果<see cref="Task{TResult}"/>.</returns>
        Task<TResult> GetTask<TResult>(Func<IQueryable<DbpTask>, Task<TResult>> query, bool notrack = false);

        /// <summary>
        /// The 查询Vtr类型信息.
        /// </summary>
        /// <typeparam name="TResult">返回类型.</typeparam>
        /// <param name="query">The 条件<see cref="Func{IQueryable{VtrTypeinfo}, IQueryable{TResult}}"/>.</param>
        /// <param name="notrack">The 是否追踪<see cref="bool"/>.</param>
        /// <returns>The 查询结果<see cref="Task{List{TResult}}"/>.</returns>
        Task<List<TResult>> GetTypeinfo<TResult>(Func<IQueryable<VtrTypeinfo>, IQueryable<TResult>> query, bool notrack = false);

        /// <summary>
        /// The 查询Vtr类型信息.
        /// </summary>
        /// <typeparam name="TResult">返回类型.</typeparam>
        /// <param name="query">The 条件<see cref="Func{IQueryable{VtrTypeinfo}, Task{TResult}}"/>.</param>
        /// <param name="notrack">The 是否追踪<see cref="bool"/>.</param>
        /// <returns>The 查询结果<see cref="Task{TResult}"/>.</returns>
        Task<TResult> GetTypeinfo<TResult>(Func<IQueryable<VtrTypeinfo>, Task<TResult>> query, bool notrack = false);

        /// <summary>
        /// The 查询Vtr详细信息.
        /// </summary>
        /// <typeparam name="TResult">返回类型.</typeparam>
        /// <param name="query">The 条件<see cref="Func{IQueryable{VtrDetailinfo}, IQueryable{TResult}}"/>.</param>
        /// <param name="notrack">The 是否追踪<see cref="bool"/>.</param>
        /// <returns>The 查询结果<see cref="Task{List{TResult}}"/>.</returns>
        Task<List<TResult>> GetDetailinfo<TResult>(Func<IQueryable<VtrDetailinfo>, IQueryable<TResult>> query, bool notrack = false);

        /// <summary>
        /// The 查询Vtr详细信息.
        /// </summary>
        /// <typeparam name="TResult">返回类型.</typeparam>
        /// <param name="query">The 条件<see cref="Func{IQueryable{VtrDetailinfo}, Task{TResult}}"/>.</param>
        /// <param name="notrack">The 是否追踪<see cref="bool"/>.</param>
        /// <returns>The 查询结果<see cref="Task{TResult}"/>.</returns>
        Task<TResult> GetDetailinfo<TResult>(Func<IQueryable<VtrDetailinfo>, Task<TResult>> query, bool notrack = false);

        /// <summary>
        /// The 查询Vtr上传任务及内容.
        /// </summary>
        /// <param name="Condition">The 条件<see cref="VTRUploadConditionRequest"/>.</param>
        /// <returns>The 查询结果<see cref="Task{List{VTRUploadTaskContentResponse}}"/>.</returns>
        Task<List<VTRUploadTaskContentResponse>> GetUploadTaskContent(VTRUploadConditionRequest Condition);

        /// <summary>
        /// The 查询 需要调度的 Vtr上传任务及内容.
        /// </summary>
        /// <param name="dtBegin">The 时间段<see cref="DateTime"/>.</param>
        /// <returns>The 查询结果<see cref="Task{List{VTRUploadTaskContentResponse}}"/>.</returns>
        Task<List<VTRUploadTaskContentResponse>> GetNeedScheduleExecuteVTRUploadTasks(DateTime dtBegin);

        /// <summary>
        /// The 查询 未开始的 Vtr上传任务及内容.
        /// </summary>
        /// <param name="minute">The 延时分钟<see cref="int"/>.</param>
        /// <returns>The 查询结果<see cref="Task{List{VTRUploadTaskContentResponse}}"/>.</returns>
        Task<List<VTRUploadTaskContentResponse>> GetWillExecuteVTRUploadTasks(int minute);

        /// <summary>
        /// The 查询 Vtr上传任务.
        /// </summary>
        /// <param name="Condition">The 条件<see cref="VTRUploadConditionRequest"/>.</param>
        /// <param name="bTaskMoreThanZero">The bTaskMoreThanZero<see cref="bool"/>.</param>
        /// <returns>The 查询结果<see cref="List{VtrUploadtask}"/>.</returns>
        Task<List<VtrUploadtask>> GetUploadtaskInfo(VTRUploadConditionRequest Condition, bool bTaskMoreThanZero);

        /// <summary>
        /// 查询 Vtr上传任务
        /// </summary>
        /// <typeparam name="TResult">返回类型.</typeparam>
        /// <param name="query">The 条件 <see cref="Func{IQueryable{VtrUploadtask}, IQueryable{TResult}}"/>.</param>
        /// <param name="notrack">The 是否追踪<see cref="bool"/>.</param>
        /// <returns>The 查询结果 <see cref="List{TResult}"/>.</returns>
        Task<List<TResult>> GetUploadtask<TResult>(Func<IQueryable<VtrUploadtask>, IQueryable<TResult>> query, bool notrack = false);

        /// <summary>
        /// 查询 Vtr上传任务
        /// </summary>
        /// <typeparam name="TResult">返回类型.</typeparam>
        /// <param name="query">The 条件 <see cref="Func{IQueryable{VtrUploadtask}, Task{TResult}}"/>.</param>
        /// <param name="notrack">The 是否追踪 <see cref="bool"/>.</param>
        /// <returns>The 查询结果<see cref="Task{TResult}"/>.</returns>
        Task<TResult> GetUploadtask<TResult>(Func<IQueryable<VtrUploadtask>, Task<TResult>> query, bool notrack = false);

        /// <summary>
        /// The 添加 Vtr上传任务.
        /// </summary>
        /// <param name="task">The Vtr上传任务<see cref="VtrUploadtask"/>.</param>
        /// <returns>The 是否添加成功<see cref="bool"/>.</returns>
        Task<bool> AddUploadtask(VtrUploadtask task);

        /// <summary>
        /// The 更新 Vtr上传任务.
        /// </summary>
        /// <param name="task">The Vtr上传任务<see cref="VtrUploadtask"/>.</param>
        /// <returns>The 是否更新成功<see cref="Task{bool}"/>.</returns>
        Task<bool> UpdateUploadtask(VtrUploadtask task);
    }
}
