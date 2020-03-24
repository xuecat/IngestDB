using System;
using System.Collections.Generic;

namespace IngestTaskPlugin.Models
{
    public partial class VtrRecordtask
    {
        public int Taskid { get; set; }
        public int? Vtrid { get; set; }
        public int? Recchannelid { get; set; }
        public int? Taskstate { get; set; }
        public string Usercode { get; set; }
        public DateTime Committime { get; set; }
        public int? Recordorder { get; set; }
        public string Taskguid { get; set; }
        public int? Tapeid { get; set; }
        public string Taskname { get; set; }
        public int? Tapetrimin { get; set; }
        public int? Tapetrimout { get; set; }
    }
}
