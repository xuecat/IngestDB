using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IngestTaskPlugin.Dto.Response
{
    /// <summary>
    /// 上载任务 及 内容
    /// </summary>
    public class VTRUploadTaskContentResponse
    {
        /// <summary>
        /// VTR任务ID
        /// </summary>
        public int TaskId { get; set; } = 0;

        /// <summary>
        /// 上载VTRID
        /// </summary>
        public int VtrId { get; set; } = 0;

        /// <summary>
        /// 表示上载时，是入点加长度，还是入点加出点
        /// </summary>
        public int BlankTaskId { get; set; } = 0;

        /// <summary>
        /// Defines the emVtrTaskType.
        /// </summary>
        public VTRUPLOADTASKTYPE VtrTaskType = VTRUPLOADTASKTYPE.VTR_SCHEDULE_UPLOAD;

        /// <summary>
        /// 收录通道ID
        /// </summary>
        public int ChannelId { get; set; } = 0;

        /// <summary>
        /// 磁带入点
        /// </summary>
        public int TrimIn { get; set; } = 0;

        /// <summary>
        /// 磁带出点
        /// </summary>
        public int TrimOut { get; set; } = 0;

        /// <summary>
        /// 信号源ID
        /// </summary>
        public int SignalId { get; set; } = 0;

        /// <summary>
        /// 磁带状态
        /// </summary>
        public VTRUPLOADTASKSTATE TaskState { get; set; } = VTRUPLOADTASKSTATE.VTR_UPLOAD_TEMPSAVE;

        /// <summary>
        /// 上载用户编码
        /// </summary>
        public string UserCode { get; set; } = string.Empty;

        /// <summary>
        /// 提交时间
        /// </summary>
        public string CommitTime { get; set; } = DateTime.MinValue.ToString("yyyy-MM-dd HH:mm:ss");

        /// <summary>
        /// 上载顺序
        /// </summary>
        public int Order { get; set; } = 0;

        /// <summary>
        /// Defines the strTaskGUID.
        /// </summary>
        public string TaskGUID { get; set; } = string.Empty;

        /// <summary>
        /// 磁带ID
        /// </summary>
        public int TapeId { get; set; } = 0;

        /// <summary>
        /// 上载任务名
        /// </summary>
        public string TaskName { get; set; } = string.Empty;

        /// <summary>
        /// 用户令牌
        /// </summary>
        public string UserToken { get; set; } = string.Empty;

        /// <summary>
        /// 磁带CTL入点
        /// </summary>
        public int TrimInCTL { get; set; } = 0;

        /// <summary>
        /// 磁带CTL出点
        /// </summary>
        public int TrimOutCTL { get; set; } = 0;

        /// <summary>
        /// 与这个strStampImage一样
        /// </summary>
        public string TaskDesc { get; set; } = string.Empty;

        /// <summary>
        /// 任务类型描述
        /// </summary>
        public string Classify { get; set; } = string.Empty;

        /// <summary>
        /// Defines the nUnit.
        /// </summary>
        public int Unit { get; set; } = 1;

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime BeginTime { get; set; } = DateTime.MinValue;

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime EndTime { get; set; } = DateTime.MinValue;

        /// <summary>
        /// Defines the emTaskType.
        /// </summary>
        public TaskType TaskType { get; set; } = TaskType.TT_VTRUPLOAD;

        /// <summary>
        /// Defines the emCooperantType.
        /// </summary>
        public CooperantType CooperantType { get; set; } = CooperantType.emPureTask;

        /// <summary>
        /// Defines the emState.
        /// </summary>
        public taskState State { get; set; } = taskState.tsReady;

        /// <summary>
        /// Defines the strStampImage.
        /// </summary>
        public string StampImage { get; set; } = string.Empty;
    }
}
