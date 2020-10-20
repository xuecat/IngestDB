using IngestTaskPlugin.Dto.OldResponse;
using System;
using System.Collections.Generic;
using System.Text;

namespace IngestTaskPlugin.Dto.Response
{
    public class TaskContentResponse
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
        public TaskType TaskType { get; set; } = TaskType.TT_NORMAL;
        /// <summary>任务备份</summary>
        /// <example>2020-4-02 16:19:33</example>
        public CooperantType CooperantType { get; set; } = CooperantType.emPureTask;
        /// <summary>任务状态</summary>
        /// <example>normal</example>
        public taskState State { get; set; }
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
        public TaskPriority Priority { get; set; } = TaskPriority.TP_Normal;
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

    public class TaskInfoResponse
    {
        /// <summary>是否备份任务</summary>
        /// <example>false</example>
        public bool BackUpTask { get; set; }
        /// <summary>任务来源</summary>
        /// <example>emMSVUploadTask</example>
        public TaskSource TaskSource { get; set; } = TaskSource.emUnknowTask;
        /// <summary>任务基础元数据</summary>
        public TaskContentResponse TaskContent { get; set; }
        /// <summary>任务素材元数据</summary>
        public TaskMaterialMetaResponse MaterialMeta { get; set; }
        /// <summary>任务content元数据</summary>
        public TaskContentMetaResponse ContentMeta { get; set; }
        /// <summary>任务planning元数据</summary>
        public TaskPlanningResponse PlanningMeta { get; set; }
        /// <summary>任务split元数据，分裂的</summary>
        public TaskSplitResponse SplitMeta { get; set; }
        /// <summary>任务采集参数</summary>
        /// <example>string</example>
        public string CaptureMeta { get; set; }
    }

    public class CHSelCondition
    {
        public bool BackupCHSel { get; set; } = true;
        public bool CheckCHCurState { get; set; } = false;
        public bool MoveExcutingOpenTask { get; set; } = false;//是否排除有正在执行手动任务的通道
        public bool OnlyLocalChannel { get; set; } = true;//1:只能在本通道；其他:优先在本通道
        public int BaseChId { get; set; } = -1;//这个ID是外面穿进来的，如果>0,那么将跟这个通道在同一个物理机上的通道优先级降低
    }

    public class TaskSimpleTime
    {
        public int TaskId { get; set; }
        public DateTime Begin { get; set; }
        public DateTime End { get; set; }
    }
}
