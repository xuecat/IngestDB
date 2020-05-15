using System;
using System.Collections.Generic;

namespace IngestTaskPlugin.Dto.OldVtr
{
    
    /// <summary>
    /// 录像机上载任务查询条件.
    /// </summary>
    public class VTRUploadCondition
    {
        /// <summary>
        /// Defines the szUserCode.
        /// </summary>
        public string szUserCode = "";

        /// <summary>
        /// 上载任务ID
        /// </summary>
        public int lTaskID = 0;

        /// <summary>
        /// 上载任务状态
        /// </summary>
        public List<int> state;

        /// <summary>
        /// VTR占位任务ID，用于删除任务时删除所有的VTR上载任务
        /// </summary>
        public int lBlankTaskID = 0;

        /// <summary>
        /// VTR ID
        /// </summary>
        public int lVtrID = -1;

        /// <summary>
        /// Defines the strMaxCommitTime.
        /// </summary>
        public string strMaxCommitTime = DateTime.MinValue.ToString("yyyy-MM-dd HH:mm:ss");

        /// <summary>
        /// Defines the strMinCommitTime.
        /// </summary>
        public string strMinCommitTime = DateTime.MinValue.ToString("yyyy-MM-dd HH:mm:ss");

        /// <summary>
        /// Defines the strTaskName.
        /// </summary>
        public string strTaskName = "";

        /// <summary>
        /// Defines the strUserToken.
        /// </summary>
        public string strUserToken = "";

        /// <summary>
        /// 任务类型独立启动 0 中心媒支 1 广告  2 节目  3 E-NET 4
        /// </summary>
        public int nTaskType = 0;
    }

;
}
