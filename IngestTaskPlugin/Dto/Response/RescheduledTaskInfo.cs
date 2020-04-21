using System;
using System.Collections.Generic;
using System.Text;

namespace IngestTaskPlugin.Dto
{
    public class RescheduledTaskInfo
    {
        public int TaskID { get; set; }
        public string TaskName { get; set; }
        public int PreviousChannelID { get; set; }//先前分配的通道ID
        public int CurrentChannelID { get; set; }// 现在获得的通道ID
    }
}
