using IngestTaskPlugin.Dto.OldResponse;
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
        /// <example>313</example>
        public int TaskId { get; set; } = 0;

        /// <summary>
        /// 上载VTRID
        /// </summary>
        /// <example>2</example>
        public int VtrId { get; set; } = 0;

        /// <summary>
        /// 表示上载时，是入点加长度，还是入点加出点
        /// </summary>
        /// <example>0</example>
        public int BlankTaskId { get; set; } = 0;

        /// <summary>
        /// Defines the emVtrTaskType.
        /// </summary>
        /// <example>1</example>
        public VTRUPLOADTASKTYPE VtrTaskType = VTRUPLOADTASKTYPE.VTR_SCHEDULE_UPLOAD;

        /// <summary>
        /// 收录通道ID
        /// </summary>
        /// <example>10</example>
        public int ChannelId { get; set; } = 0;

        /// <summary>
        /// 磁带入点
        /// </summary>
        /// <example>73216</example>
        public int TrimIn { get; set; } = 0;

        /// <summary>
        /// 磁带出点
        /// </summary>
        /// <example>196608</example>
        public int TrimOut { get; set; } = 0;

        /// <summary>
        /// 信号源ID
        /// </summary>
        /// <example>20</example>
        public int SignalId { get; set; } = 0;

        /// <summary>
        /// 磁带状态
        /// </summary>
        /// <example>4</example>
        public VTRUPLOADTASKSTATE TaskState { get; set; } = VTRUPLOADTASKSTATE.VTR_UPLOAD_TEMPSAVE;

        /// <summary>
        /// 上载用户编码
        /// </summary>
        /// <example>8de083d45c614628b99516740d628e91</example>
        public string UserCode { get; set; } = string.Empty;

        /// <summary>
        /// 提交时间
        /// </summary>
        /// <example>2019-08-07 21:54:26</example>
        public string CommitTime { get; set; } = DateTime.MinValue.ToString("yyyy-MM-dd HH:mm:ss");

        /// <summary>
        /// 上载顺序
        /// </summary>
        /// <example>0</example>
        public int Order { get; set; } = 0;

        /// <summary>
        /// Defines the strTaskGUID.
        /// </summary>
        /// <example>40036bd7204b4aa8914ac3bb339d9042</example>
        public string TaskGuid { get; set; } = string.Empty;

        /// <summary>
        /// 磁带ID
        /// </summary>
        /// <example>-1</example>
        public int TapeId { get; set; } = 0;

        /// <summary>
        /// 上载任务名
        /// </summary>
        /// <example>S</example>
        public string TaskName { get; set; } = string.Empty;

        /// <summary>
        /// 用户令牌
        /// </summary>
        /// <example>VTRBATCHUPLOAD_ERROR_OK</example>
        public string UserToken { get; set; } = string.Empty;

        /// <summary>
        /// 磁带CTL入点
        /// </summary>
        /// <example>73216</example>
        public int TrimInCtl { get; set; } = 0;

        /// <summary>
        /// 磁带CTL出点
        /// </summary>
        /// <example>196608</example>
        public int TrimOutCtl { get; set; } = 0;

        /// <summary>
        /// 与这个strStampImage一样
        /// </summary>
        /// <example>\\\\storage.test.com\\hivefiles\\sobeyhive\\buckets\\u-qhbyb6s9mp763brm\\hv_res\\2019-08-07\\2019_08_07\\ftc_ltc_20190807215232_0_0_221_312__000__high.mxf.bmp</example>
        public string TaskDesc { get; set; } = string.Empty;

        /// <summary>
        /// 任务类型描述
        /// </summary>
        /// <example>A</example>
        public string Classify { get; set; } = string.Empty;

        /// <summary>
        /// Defines the nUnit.
        /// </summary>
        /// <example>1</example>
        public int Unit { get; set; } = 1;

        /// <summary>
        /// 开始时间
        /// </summary>
        /// <example>2019-08-07 21:52:39</example>
        public DateTime BeginTime { get; set; } = DateTime.MinValue;

        /// <summary>
        /// 结束时间
        /// </summary>
        /// <example>2019-08-07 21:53:38</example>
        public DateTime EndTime { get; set; } = DateTime.MinValue;

        /// <summary>
        /// Defines the emTaskType.
        /// </summary>
        /// <example>6</example>
        public TaskType TaskType { get; set; } = TaskType.TT_VTRUPLOAD;

        /// <summary>
        /// Defines the emCooperantType.
        /// </summary>
        /// <example>0</example>
        public CooperantType CooperantType { get; set; } = CooperantType.emPureTask;

        /// <summary>
        /// Defines the emState.
        /// </summary>
        /// <example>2</example>
        public taskState State { get; set; } = taskState.tsReady;

        /// <summary>
        /// Defines the strStampImage.
        /// </summary>
        /// <example>\\\\storage.test.com\\hivefiles\\sobeyhive\\buckets\\u-qhbyb6s9mp763brm\\hv_res\\2019-08-07\\2019_08_07\\ftc_ltc_20190807215232_0_0_221_312__000__high.mxf.bmp</example>
        public string StampImage { get; set; } = string.Empty;
    }
}
