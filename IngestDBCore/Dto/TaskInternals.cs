using System;
using System.Collections.Generic;
using System.Text;

namespace IngestDBCore
{
    public class TaskInternals
    {
        public enum FunctionType
        {
            WillBeginAndCapturingTasks,
            CurrentTasks
        }

        public FunctionType funtype { get; set; }

        

        /// </summary>
        public int TaskId { get; set; }

        public string Ext1 { get; set; }
        public string Ext2 { get; set; }
    }
    /// <summary>任务类型</summary>
    public enum TaskTypeInterface
    {
        /// <summary>普通任务</summary>
		TT_NORMAL = 0,
        /// <summary>周期任务</summary>
		TT_PERIODIC = 1,
        /// <summary>OpenEnd任务</summary>
		TT_OPENEND = 2,
        /// <summary>循环任务</summary>
		TT_LOOP = 3,
        /// <summary>占位任务</summary>
		TT_TIEUP = 4,
        /// <summary>手动任务</summary>
		TT_MANUTASK = 5,
        /// <summary>VTR上载任务</summary>
		TT_VTRUPLOAD = 6,
        /// <summary>扩展的OpenEnd任务</summary>
        TT_OPENENDEX = 7
    }
    /// <summary>任务附加类型</summary>
    public enum CooperantTypeInterface
    {
        /// <summary>一般任务</summary>
		emPureTask = 0,
        /// <summary>同时收录到播出</summary>
		emKamataki = 1,
        /// <summary>vtr上载同时到播出</summary>
		emVTRKamataki = 2,
        /// <summary>VTR备份</summary>
		emVTRBackup = 3,
        /// <summary>Kamataki完成标识</summary>
		emKamatakiFinish = 4,
        /// <summary>VTR备份完成标识</summary>
		emVTRBackupFinish = 5,
        /// <summary>VTR备份失败标识</summary>
		emVTRBackupFailed = 6
    }
    /// <summary>任务附加类型</summary>
    public enum taskStateInterface
    {
        /// <summary>就绪</summary>
        tsReady = 0,
        /// <summary>正在执行任务</summary>
		tsExecuting,
        /// <summary>执行完成任务</summary>
		tsComplete,
        /// <summary>暂停标记</summary>
        tsPause,
        /// <summary>删除标记</summary> 
        tsDelete,
        /// <summary>冲突任务</summary>
		tsConflict,
        /// <summary>无效任务</summary>
		tsInvaild,
        /// <summary>正在采集的手动任务</summary>
		tsManuexecuting,
        /// <summary>禁用的任务</summary>
		tsDisabled
    }
    /// <summary>任务优先级</summary>
    public enum TaskPriorityInterface
    {
        /// <summary>最低优先级</summary>
        TP_Lowest,
        /// <summary>较低优先级</summary>
        TP_BelowNormal,
        /// <summary>普通优先级</summary>
        TP_Normal,
        /// <summary>较高优先级</summary>
        TP_AboveNormal,
        /// <summary>最高优先级</summary>
        TP_Highest
    }
    public class TaskContentInterface
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
        public TaskTypeInterface emTaskType = TaskTypeInterface.TT_NORMAL;
        public CooperantTypeInterface emCooperantType = CooperantTypeInterface.emPureTask;
        public taskStateInterface emState;
        public string strStampImage = string.Empty;
        public string strTaskGUID = string.Empty;
        public int nBackupVTRID = 0;
        public TaskPriorityInterface emPriority = TaskPriorityInterface.TP_Normal;
        public int nStampTitleIndex = 0;
        public int nStampImageType = 0;
        public int nSGroupColor = 0;
    }
}
