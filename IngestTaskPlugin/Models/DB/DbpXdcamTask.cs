using System;
using System.Collections.Generic;

namespace IngestTaskPlugin.Models
{
    public partial class DbpXdcamTask
    {
        public int Taskid { get; set; }
        public int? Diskid { get; set; }
        public DateTime Createtime { get; set; }
        public int? Taskstate { get; set; }
        public string Statedesc { get; set; }
        public int? Progress { get; set; }
        public string Catalogpath { get; set; }
        public string Objectumid { get; set; }
        public int? Objecttype { get; set; }
        public string Taskguid { get; set; }
        public int? Visible { get; set; }
        public int? Priority { get; set; }
        public DateTime Committime { get; set; }
        public string Usercode { get; set; }
        public string Clipname { get; set; }
    }
}
