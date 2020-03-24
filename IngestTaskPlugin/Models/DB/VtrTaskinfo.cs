using System;
using System.Collections.Generic;

namespace IngestTaskPlugin.Models
{
    public partial class VtrTaskinfo
    {
        public int Vtrtaskid { get; set; }
        public string Taskname { get; set; }
        public DateTime Begintime { get; set; }
        public DateTime Endtime { get; set; }
        public int? Vtrid { get; set; }
        public int? Taskstate { get; set; }
        public string Usercode { get; set; }
        public string Barcode { get; set; }
        public int? Loopflag { get; set; }
        public int? Signalid { get; set; }
        public string Description { get; set; }
        public int? Uploadflag { get; set; }
        public int? Syncflag { get; set; }
        public string Taskguid { get; set; }
    }
}
