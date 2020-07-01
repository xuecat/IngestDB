using IngestTaskPlugin.Dto.OldResponse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IngestTaskPlugin.Dto.Response
{
    /// <summary>
    /// VTR上传任务信息
    /// </summary>
    public class VTRUploadTaskInfoResponse
    {
        /// <summary>
        /// VTR任务ID
        /// </summary>
        public int VtrTaskId { get; set; } = 0;
        /// <summary>
        /// 上载VTRID
        /// </summary>
        public int VtrId { get; set; } = 0;
        /// <summary>
        /// 占位上载任务ID
        /// </summary>
        public int BlankTaskId { get; set; } = 0;
        /// <summary>
        /// VTR任务类型
        /// </summary>
        public VTRUPLOADTASKTYPE VtrTaskType { get; set; } = 0;
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
        public VTRUPLOADTASKSTATE TaskState { get; set; } = 0;
        /// <summary>
        /// 上载用户编码
        /// </summary>
        public string UserCode { get; set; } = "";
        /// <summary>
        /// 提交时间
        /// </summary>
        public DateTime CommitTime { get; set; } = DateTime.MinValue;
        /// <summary>
        /// 上载顺序
        /// </summary>
        public int Order { get; set; } = 0;
        /// <summary>
        /// 任务GUID
        /// </summary>
        public string TaskGuid { get; set; } = string.Empty;
        /// <summary>
        /// 磁带ID
        /// </summary>
        public int TapeId { get; set; } = 0;
        /// <summary>
        /// 上载任务名
        /// </summary>
        public string TaskName { get; set; } = "";
        /// <summary>
        /// 用户令牌
        /// </summary>
        public string UserToken { get; set; } = "";
        /// <summary>
        /// 磁带CTL入点
        /// </summary>
        public int TrimInCtl { get; set; } = 0;
        /// <summary>
        /// 磁带CTL出点
        /// </summary>
        public int TrimOutCtl { get; set; } = 0;
    }

}
