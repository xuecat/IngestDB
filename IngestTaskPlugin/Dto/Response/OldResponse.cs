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
    /// <summary>
    /// 任务调度状态
    /// </summary>
    public enum dispatchState
    {
        /// <summary>
        /// 未调度 = 0
        /// </summary>
		dpsNotDispatch = 0,
        /// <summary>
        /// 需要重调度 = 1
        /// </summary>
		dpsRedispatch,
        /// <summary>
        /// 已调度 = 2
        /// </summary>
		dpsDispatched,
        /// <summary>
        /// 调度失败 = 3
        /// </summary>
		dpsDispatchFailed,
        /// <summary>
        /// 无效 = 4
        /// </summary>
		dpsInvalid,
        /// <summary>
        /// 禁用 = 5
        /// </summary>
		dpsDisabled
    };
    /// <summary>
    /// 任务同步状态
    /// </summary>
	public enum syncState
    {
        /// <summary>
        /// 未同步 = 0
        /// </summary>
		ssNot = 0,
        /// <summary>
        /// 已同步 = 1
        /// </summary>
		ssSync,
        /// <summary>
        /// 同步失败 = 2
        /// </summary>
		ssFailed
    };
    /// <summary>
    /// 任务操作类型
    /// </summary>
	public enum opType
    {
        /// <summary>
        /// 添加 = 0
        /// </summary>
		otAdd = 0,
        /// <summary>
        /// 删除 = 1
        /// </summary>
		otDel,
        /// <summary>
        /// 移动 = 2
        /// </summary>
		otMove,
        /// <summary>
        /// 修改 = 3
        /// </summary>
		otModify
    };
    public enum VTRUPLOADTASKSTATE
    {
        VTR_ALL_UPLOAD_STATE = 0, //所有状态
        VTR_UPLOAD_TEMPSAVE = 1, //暂存
        VTR_UPLOAD_COMMIT = 2, //提交待执行
        VTR_UPLOAD_EXECUTE = 3, //正在执行
        VTR_UPLOAD_COMPLETE = 4, //执行完成
        VTR_UPLOAD_FAIL = 5, //执行失败	
        VTR_UPLOAD_DELETE = 6, //删除上载任务
        VTR_UPLOAD_PASSBACK = 7, //任务打回
        VTR_UPLOAD_ARCHIVESUCCESS = 8, //入库成功
        VTR_UPLOAD_ARCHIVEFAIL = 9, //入库失败
        VTR_UPLOAD_ARCHIVCHECKFAIL = 10,//入审片工作站失败
        VTR_UPLOAD_ARCHIVCHECKSUCCESS = 11,//入审片工作站成功
        VTR_UPLOAD_CHECKSUCCESS = 12,//审片通过
        VTR_CAPTURE_COMPLETE_DELETE = 13,//采集完成,但素材已经删除
        VTR_CAPTURE_PASSBACK_DELETE = 14,//审片打回,素材已经删除
        VTR_CAPTURE_ARCHIVESUCCESS_DELETE = 15,//入库成功,但素材已经删除
        VTR_CAPTURE_CONBIMING = 16,//正在合并素材
        VTR_CAPTURE_CONBIMED = 17,//素材合并完成
        VTR_CAPTURE_ARCHIVING = 18,//正在入库
        VTR_CAPTURE_ARCHIVEFILE = 19,//正在迁移文件
        VTR_UPLOAD_PRE_EXECUTE = 20,//预执行状态，表示vtr在卷带的时候
    };

    public class GroupTaskParam_OUT
    {
        public string errStr;
        public bool bRet;
        public List<int> taskResults;
    }
    public class GetQueryTaskMetaData_param
    {
        public string MetaData;
        public string errStr;
        public bool bRet;
    }
    public class PostSetTaskMetaData_IN
    {
        public int nTaskID;
        public MetaDataType Type;
        public string MateData;
        public string TypeID;
    }
    public class PostSetTaskMetaData_OUT
    {
        public string errStr;
        public bool bRet;
    }
    public class SetTaskCustomMetadata_IN
    {
        public int nTaskID;
        public string Metadata;
    }
    public class SetTaskCustomMetadata_OUT
    {
        public string errStr;
        public bool bRet;
    }
    public class GetTaskCustomMetadata_OUT
    {
        public string Metadata;
        public string errStr;
        public bool bRet;
    }

}
