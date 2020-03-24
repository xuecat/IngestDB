using System;
using System.Collections.Generic;

namespace IngestTaskPlugin.Models
{
    public partial class DbpTask
    {
        public int Taskid { get; set; }
        public string Taskname { get; set; }
        public int? Recunitid { get; set; }
        public string Usercode { get; set; }
        public int? Signalid { get; set; }
        public int? Channelid { get; set; }
        public int? OldChannelid { get; set; }
        public int? State { get; set; }
        public DateTime Starttime { get; set; }
        public DateTime Endtime { get; set; }
        public DateTime NewBegintime { get; set; }
        public DateTime NewEndtime { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public int? Tasktype { get; set; }
        public int? Backtype { get; set; }
        public int? DispatchState { get; set; }
        public int? SyncState { get; set; }
        public int? OpType { get; set; }
        public string Tasklock { get; set; }
        public string Taskguid { get; set; }
        public int? Backupvtrid { get; set; }
        public int? Taskpriority { get; set; }
        public int? Stamptitleindex { get; set; }
        public int? Stampimagetype { get; set; }
        public int? Sgroupcolor { get; set; }
    }
}
