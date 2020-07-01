using System;
using System.Collections.Generic;
using System.Text;

namespace IngestTaskPlugin.Dto.Response
{
    public class TaskInfoRescheduledResponse
    {
        public int TaskId { get; set; }
        public string TaskName { get; set; }
        public int PreviousChannelId { get; set; }//先前分配的通道ID
        public int CurrentChannelId { get; set; }// 现在获得的通道ID
    }
}
