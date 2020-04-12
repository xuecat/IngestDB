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

    /// <summary>
    /// 任务来源
    /// </summary>
	public enum TaskSource
    {
        /// <summary>
        /// MSV Upload Task = 0
        /// </summary>
        emMSVUploadTask = 0,
        /// <summary>
        /// VTR Upload Task = 1
        /// </summary>
		emVTRUploadTask = 1,
        /// <summary>
        /// XDCAM Upload Task = 2
        /// </summary>
		emXDCAMUploadTask = 2,
        /// <summary>
        /// IP TS流 收录 = 3
        /// </summary>
		emIPTSUploadTask = 3,
        /// <summary>
        /// 流媒体 = 4
        /// </summary>
        emStreamMediaUploadTask = 4,
        /// <summary>
        /// UnknowTask = -1
        /// </summary>
        emUnknowTask = -1
    }

    /// <summary>
    /// 任务附加类型
    /// </summary>
    public enum CooperantType
    {
        /// <summary>
        /// 一般任务 = 0
        /// </summary>
		emPureTask = 0,
        /// <summary>
        /// 同时收录到播出 = 1
        /// </summary>
		emKamataki = 1,
        /// <summary>
        /// vtr上载同时到播出 = 2
        /// </summary>
		emVTRKamataki = 2,
        /// <summary>
        /// VTR备份 = 3
        /// </summary>
		emVTRBackup = 3,
        /// <summary>
        /// Kamataki完成标识 = 4
        /// </summary>
		emKamatakiFinish = 4,
        /// <summary>
        /// VTR备份完成标识 = 5
        /// </summary>
		emVTRBackupFinish = 5,
        /// <summary>
        /// VTR备份失败标识 = 6
        /// </summary>
		emVTRBackupFailed = 6
    }
    /// <summary>
    /// 任务优先级
    /// </summary>
    public enum TaskPriority
    {
        /// <summary>
        /// 最低优先级
        /// </summary>
        TP_Lowest,
        /// <summary>
        /// 较低优先级
        /// </summary>
        TP_BelowNormal,
        /// <summary>
        /// 普通优先级
        /// </summary>
        TP_Normal,
        /// <summary>
        /// 较高优先级
        /// </summary>
        TP_AboveNormal,
        /// <summary>
        /// 最高优先级
        /// </summary>
        TP_Highest
    }

    public class TaskContent
    {
        public int nTaskID = 0;
        public string strTaskName = string.Empty;
        public string strTaskDesc = string.Empty;
        public string strClassify = string.Empty;
        public int nChannelID = 0;
        public int nUnit = 0;
        public string strUserCode = string.Empty;
        public int nSignalID = 0;
        public string strBegin = DateTime.MinValue.ToString("yyyy-MM-dd HH:mm:ss");
        public string strEnd = DateTime.MinValue.ToString("yyyy-MM-dd HH:mm:ss");
        public TaskType emTaskType = TaskType.TT_NORMAL;
        public CooperantType emCooperantType = CooperantType.emPureTask;
        public taskState emState;
        public string strStampImage = string.Empty;
        public string strTaskGUID = string.Empty;
        public int nBackupVTRID = 0;
        public TaskPriority emPriority = TaskPriority.TP_Normal;
        public int nStampTitleIndex = 0;
        public int nStampImageType = 0;
        public int nSGroupColor = 0;
    }

    public class TaskFullInfo
    {
        public TaskContent taskContent;
        public int nOldChannelID;
        public dispatchState emDispatchState;
        public syncState emSyncState;
        public opType emOpType;
        public string strNewBeginTime = DateTime.MinValue.ToString("yyyy-MM-dd HH:mm:ss");
        public string strNewEndTime = DateTime.MinValue.ToString("yyyy-MM-dd HH:mm:ss");
        public string strTaskLock = string.Empty;
        public TaskFullInfo()
        {
            taskContent = new TaskContent();
        }
    }
    public enum TimeLineType
    {
        em24HourDay = 0,
        em32HourDay = 1,
        em36HourDay = 2,
        em48HourDay = 3
    }
    public class MetadataPair
    {
        public int nTaskID;
        public string strMetadata;
        public MetaDataType emtype;
    }
    public class AddTaskSvr_IN
    {
        public TaskContent taskAdd;
        public TaskSource taskSrc;
        public MetadataPair[] metadatas;
    }
    public class AddTaskSvr_OUT
    {
        public int newTaskId;
        public string errStr;
        public bool bRet;
    }
}

