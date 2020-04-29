namespace IngestTaskPlugin.Dto.Request
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// 录像机上载任务查询条件.
    /// </summary>
    public class VTRUploadConditionRequest
    {
        /// <summary>
        /// Gets or sets the UserCode
        /// Defines the szUserCode..
        /// </summary>
        /// <example>8de083d45c614628b99516740d628e91</example>
        public string UserCode { get; set; } = "";

        /// <summary>
        /// Gets or sets the lTaskID
        /// 上载任务ID.
        /// </summary>
        /// <example>312</example>
        public int TaskId { get; set; } = 0;

        /// <summary>
        /// Gets or sets the state
        /// 上载任务状态.
        /// </summary>
        /// <example>4</example>
        public List<int?> TaskState { get; set; }

        /// <summary>
        /// Gets or sets the lBlankTaskID
        /// VTR占位任务ID，用于删除任务时删除所有的VTR上载任务.
        /// </summary>
        /// <example>0</example>
        public int BlankTaskId { get; set; } = 0;

        /// <summary>
        /// Gets or sets the lVtrID
        /// VTR ID.
        /// </summary>
        /// <example>2</example>
        public int VtrId { get; set; } = -1;

        /// <summary>
        /// Gets or sets the strMaxCommitTime.
        /// </summary>
        /// <example>2019-08-08 00:00:01</example>
        public DateTime MaxCommitTime { get; set; } = DateTime.MinValue;

        /// <summary>
        /// Gets or sets the strMinCommitTime
        /// Defines the strMinCommitTime..
        /// </summary>
        /// <example>2019-08-06 00:00:01</example>
        public DateTime MinCommitTime { get; set; } = DateTime.MinValue;

        /// <summary>
        /// Gets or sets the strTaskName
        /// Defines the strTaskName..
        /// </summary>
        /// <example>ftc ltc</example>
        public string TaskName { get; set; } = "";

        /// <summary>
        /// Gets or sets the strUserToken
        /// Defines the strUserToken..
        /// </summary>
        /// <example></example>
        public string UserToken { get; set; } = "";

        /// <summary>
        /// Gets or sets the nTaskType
        /// 任务类型独立启动 0 中心媒支 1 广告  2 节目  3 E-NET 4.
        /// </summary>
        /// <example>0</example>
        public int TaskType { get; set; } = 0;
    }
}
