using System;
using System.Collections.Generic;
using System.Text;

namespace IngestTaskPlugin.Dto
{
    public class OldResponse
    {
    }
    public enum MetaDataType
    {
        /// <summary>
        /// 采集元数据 = 0
        /// </summary>
		emCapatureMetaData = 0,
        /// <summary>
        /// 存储元数据 = 1
        /// </summary>
		emStoreMetaData,
        /// <summary>
        /// 任务上下文关联元数据 = 2
        /// </summary>
		emContentMetaData,
        /// <summary>
        /// Plan元数据 = 3
        /// </summary>
        emPlanMetaData,
        /// <summary>
        /// 编目信息元数据 = 4
        /// </summary>
		emCatalogueMetaData,
        /// <summary>
        /// 审查打回意见 = 5
        /// </summary>
        emCheckPassBackData,
        /// <summary>
        /// MSV本地素材GUID = 6
        /// </summary>
        emAmfsData,
        /// <summary>
        ///  = 7
        /// </summary>
        emOriginalMetaData,
        emSplitData
    }
    /// <summary>
    /// 任务类型
    /// </summary>
	public enum TaskType
    {
        /// <summary>
        /// 普通任务 = 0
        /// </summary>
		TT_NORMAL = 0,
        /// <summary>
        /// 周期任务 = 1
        /// </summary>
		TT_PERIODIC = 1,
        /// <summary>
        /// OpenEnd任务 = 2
        /// </summary>
		TT_OPENEND = 2,
        /// <summary>
        /// 循环任务 = 3
        /// </summary>
		TT_LOOP = 3,
        /// <summary>
        /// 占位任务 = 4
        /// </summary>
		TT_TIEUP = 4,
        /// <summary>
        /// 手动任务 = 5
        /// </summary>
		TT_MANUTASK = 5,
        /// <summary>
        /// VTR上载任务 = 6
        /// </summary>
		TT_VTRUPLOAD = 6,
        /// <summary>
        /// 扩展的OpenEnd任务 = 7
        /// </summary>
        TT_OPENENDEX = 7
    }
    public enum taskState
    {
        /// 就绪 = 0
		tsReady = 0,
        /// 正在执行任务 = 1
		tsExecuting,
        /// 执行完成任务 = 2
		tsComplete,
        /// 暂停标记 = 3
        tsPause,
        /// 删除标记 = 4      
        tsDelete,
        /// <summary>
        /// 冲突任务 = 5
        /// </summary>
		tsConflict,
        /// <summary>
        /// 无效任务 = 6
        /// </summary>
		tsInvaild,
        /// <summary>
        /// 正在采集的手动任务 = 7
        /// </summary>
		tsManuexecuting,
        /// <summary>
        /// 禁用的任务 = 8
        /// </summary>
		tsDisabled
    }
    public class GetQueryTaskMetaData_param
    {
        public string MetaData;
        public string errStr;
        public bool bRet;
    }
}
