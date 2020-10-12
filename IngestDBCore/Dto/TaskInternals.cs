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
            CurrentTasks,
            SetTaskBmp
        }

        public FunctionType funtype { get; set; }

        
        /// </summary>
        public int TaskId { get; set; }

        public string Ext1 { get; set; }
        public string Ext2 { get; set; }
        public object Ext3 { get; set; }
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
        /// <summary>任务id</summary>
        /// <example>1</example>
        public int TaskId { get; set; }
        /// <summary>任务名</summary>
        /// <example>name</example>
        public string TaskName { get; set; }
        /// <summary>任务描述，也放任务图片</summary>
        /// <example>任务描述，也任务图片</example>
        public string TaskDesc { get; set; }
        /// <summary>任务周期属性</summary>
        /// <example>任务周期属性</example>
        public string Classify { get; set; }
        /// <summary>任务通道id</summary>
        /// <example>0</example>
        public int ChannelId { get; set; }
        /// <summary>任务</summary>
        /// <example>0</example>
        public int Unit { get; set; }
        /// <summary>用户的usercode</summary>
        /// <example>123456</example>
        public string UserCode { get; set; }
        /// <summary>信号源id</summary>
        /// <example>0</example>
        public int SignalId { get; set; }
        /// <summary>任务开始时间</summary>
        /// <example>2020-4-02 16:19:33</example>
        public string Begin { get; set; } = DateTime.MinValue.ToString("yyyy-MM-dd HH:mm:ss");
        /// <summary>任务结束时间</summary>
        /// <example>2020-4-02 16:19:33</example>
        public string End { get; set; } = DateTime.MinValue.ToString("yyyy-MM-dd HH:mm:ss");
        /// <summary>任务类型</summary>
        /// <example>TT_NORMAL</example>
        public TaskTypeInterface TaskType { get; set; } = TaskTypeInterface.TT_NORMAL;
        /// <summary>任务备份</summary>
        /// <example>2020-4-02 16:19:33</example>
        public CooperantTypeInterface CooperantType { get; set; } = CooperantTypeInterface.emPureTask;
        /// <summary>任务状态</summary>
        /// <example>normal</example>
        public taskStateInterface State { get; set; }
        /// <summary>任务图片</summary>
        /// <example>path</example>
        public string StampImage { get; set; }
        /// <summary>任务guid</summary>
        /// <example>guid</example>
        public string TaskGuid { get; set; }
        /// <summary>备份vtrid</summary>
        /// <example>1</example>
        public int BackupVtrId { get; set; }
        /// <summary>任务调度</summary>
        /// <example>TP_Normal</example>
        public TaskPriorityInterface Priority { get; set; } = TaskPriorityInterface.TP_Normal;
        /// <summary>任务分段图片位置</summary>
        /// <example>0</example>
        public int StampTitleIndex { get; set; }
        /// <summary>任务图片类型</summary>
        /// <example>1</example>
        public int StampImageType { get; set; }
        /// <summary>成组颜色 rgb值</summary>
        /// <example>0</example>
        public int GroupColor { get; set; }//注意没有S
    }
}
