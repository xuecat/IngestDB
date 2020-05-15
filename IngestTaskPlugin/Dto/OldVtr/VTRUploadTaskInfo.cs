using IngestTaskPlugin.Dto.OldResponse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IngestTaskPlugin.Dto.OldVtr
{
    /// <summary>
    /// VTR上传任务信息
    /// </summary>
    public class VTRUploadTaskInfo
    {
        /// <summary>
        /// VTR任务ID
        /// </summary>
        public int nVTRTaskID = 0;
        /// <summary>
        /// 上载VTRID
        /// </summary>
        public int nVtrID = 0;
        /// <summary>
        /// 占位上载任务ID
        /// </summary>
        public int nBlankTaskID = 0;
        /// <summary>
        /// VTR任务类型
        /// </summary>
        public VTRUPLOADTASKTYPE emVtrTaskType = 0;
        /// <summary>
        /// 收录通道ID
        /// </summary>
        public int nChannelID = 0;
        /// <summary>
        /// 磁带入点
        /// </summary>
        public int nTrimIn = 0;
        /// <summary>
        /// 磁带出点
        /// </summary>
        public int nTrimOut = 0;
        /// <summary>
        /// 信号源ID
        /// </summary>
        public int nSignalID = 0;
        /// <summary>
        /// 磁带状态
        /// </summary>
        public VTRUPLOADTASKSTATE emTaskState = 0;
        /// <summary>
        /// 上载用户编码
        /// </summary>
        public string strUserCode = "";
        /// <summary>
        /// 提交时间
        /// </summary>
        public string strCommitTime = DateTime.MinValue.ToString("yyyy-MM-dd HH:mm:ss");
        /// <summary>
        /// 上载顺序
        /// </summary>
        public int nOrder = 0;
        /// <summary>
        /// 任务GUID
        /// </summary>
        public string strTaskGUID = string.Empty;
        /// <summary>
        /// 磁带ID
        /// </summary>
        public int nTapeID = 0;
        /// <summary>
        /// 上载任务名
        /// </summary>
        public string strTaskName = "";
        /// <summary>
        /// 用户令牌
        /// </summary>
        public string strUserToken = "";
        /// <summary>
        /// 磁带CTL入点
        /// </summary>
        public int nTrimInCTL = 0;
        /// <summary>
        /// 磁带CTL出点
        /// </summary>
        public int nTrimOutCTL = 0;
    }
}
