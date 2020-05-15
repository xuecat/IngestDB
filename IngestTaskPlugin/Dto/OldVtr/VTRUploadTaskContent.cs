namespace IngestTaskPlugin.Dto.Response.OldVtr
{
    using IngestTaskPlugin.Dto.OldResponse;
    using System;

    /// <summary>
    /// Defines the <see cref="VTRUploadTaskContent" />.
    /// </summary>
    public class VTRUploadTaskContent
    {
        /// <summary>
        /// VTR任务ID
        /// </summary>
        public int nTaskId = 0;//

        /// <summary>
        /// 上载VTRID
        /// </summary>
        public int nVtrId = 0;//

        /// <summary>
        /// 表示上载时，是入点加长度，还是入点加出点
        /// </summary>
        public int nBlankTaskId = 0;//

        /// <summary>
        /// Defines the emVtrTaskType.
        /// </summary>
        public VTRUPLOADTASKTYPE emVtrTaskType = VTRUPLOADTASKTYPE.VTR_SCHEDULE_UPLOAD;

        /// <summary>
        /// 收录通道ID
        /// </summary>
        public int nChannelId = 0;//

        /// <summary>
        /// 磁带入点
        /// </summary>
        public int nTrimIn = 0;//

        /// <summary>
        /// 磁带出点
        /// </summary>
        public int nTrimOut = 0;//

        /// <summary>
        /// 信号源ID
        /// </summary>
        public int nSignalId = 0;//

        /// <summary>
        /// 磁带状态
        /// </summary>
        public VTRUPLOADTASKSTATE emTaskState = VTRUPLOADTASKSTATE.VTR_UPLOAD_TEMPSAVE;//

        /// <summary>
        /// 上载用户编码
        /// </summary>
        public string strUserCode = string.Empty;//

        /// <summary>
        /// 提交时间
        /// </summary>
        public string strCommitTime = DateTime.MinValue.ToString("yyyy-MM-dd HH:mm:ss");//

        /// <summary>
        /// 上载顺序
        /// </summary>
        public int nOrder = 0;//

        /// <summary>
        /// Defines the strTaskGUID.
        /// </summary>
        public string strTaskGUID = string.Empty;

        /// <summary>
        /// 磁带ID
        /// </summary>
        public int nTapeId = 0;//

        /// <summary>
        /// 上载任务名
        /// </summary>
        public string strTaskName = string.Empty;//

        /// <summary>
        /// 用户令牌
        /// </summary>
        public string strUserToken = string.Empty;//

        /// <summary>
        /// 磁带CTL入点
        /// </summary>
        public int nTrimInCTL = 0;//

        /// <summary>
        /// 磁带CTL出点
        /// </summary>
        public int nTrimOutCTL = 0;//

        /// <summary>
        /// 与这个strStampImage一样
        /// </summary>
        public string strTaskDesc = string.Empty;//

        /// <summary>
        /// 任务类型描述
        /// </summary>
        public string strClassify = string.Empty;//

        /// <summary>
        /// Defines the nUnit.
        /// </summary>
        public int nUnit = 1;

        /// <summary>
        /// 开始时间
        /// </summary>
        public string strBegin = DateTime.MinValue.ToString("yyyy-MM-dd HH:mm:ss");

        /// <summary>
        /// 结束时间
        /// </summary>
        public string strEnd = DateTime.MinValue.ToString("yyyy-MM-dd HH:mm:ss");

        /// <summary>
        /// Defines the emTaskType.
        /// </summary>
        public int emTaskType = (int)TaskType.TT_VTRUPLOAD;

        /// <summary>
        /// Defines the emCooperantType.
        /// </summary>
        public int emCooperantType = (int)CooperantType.emPureTask;

        /// <summary>
        /// Defines the emState.
        /// </summary>
        public int emState = (int)taskState.tsReady;

        /// <summary>
        /// Defines the strStampImage.
        /// </summary>
        public string strStampImage = string.Empty;
    }
}