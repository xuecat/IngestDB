using System;
using System.Collections.Generic;
using System.Text;

namespace IngestTaskPlugin.Dto
{
    public class TaskCondition
    {
        public int RecUnit { get; set; } = -1;  //收录单元
        public int SignalID { get; set; } = -1; //信号员
        public int ChannelID { get; set; } = -1; //收录通道ID 
        public List<int> IncludeLst { get; set; } //状态包含
        public List<int> ExcludeLst { get; set; }
        public List<int> StateIncludeLst { get; set; }
        public List<int> SyncStateIncludeLst { get; set; }
        //public List<int> StateExcludeLst { get; set; }
        public List<int> DispatchStateIncludeLst { get; set; }
        //public List<int> DispatchStateExcludeLst { get; set; }
        public List<int> TaskTypeIncludeLst { get; set; } //任务类型包括
        //public List<int> TaskTypeExcludeLst { get; set; }//任务类型不包括
        public string LockStr { get; set; }
        public DateTime MaxBeginTime { get; set; } = DateTime.MinValue;
        public DateTime MinBeginTime { get; set; } = DateTime.MinValue;
        public DateTime MaxEndTime { get; set; } = DateTime.MinValue;
        public DateTime MinEndTime { get; set; } = DateTime.MinValue;
        public DateTime MaxNewBeginTime { get; set; } = DateTime.MinValue;
        public DateTime MinNewBeginTime { get; set; } = DateTime.MinValue;
        public DateTime MaxNewEndTime { get; set; } = DateTime.MinValue;
        public DateTime MinNewEndTime { get; set; } = DateTime.MinValue;
    }
}
