using System;
using System.Collections.Generic;
using System.Text;

namespace IngestTaskPlugin.Dto
{
    /// <summary>调度任务信息</summary>
    public class RescheduledTaskInfoResponse
    {
        /// <summary>Id</summary>
        public int TaskID { get; set; }
        /// <summary>任务名</summary>
        public string TaskName { get; set; }
        /// <summary>先前分配通道id</summary>
        public int PreviousChannelID { get; set; }//先前分配的通道ID
        /// <summary>现在获取的通道id</summary>
        public int CurrentChannelID { get; set; }// 现在获得的通道ID
    }
}
