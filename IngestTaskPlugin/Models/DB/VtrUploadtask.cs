using System;
using System.Collections.Generic;

namespace IngestTaskPlugin.Models
{
    public partial class VtrUploadtask
    {
        public int Taskid { get; set; }
        public int? Vtrid { get; set; }
        public int? Recchannelid { get; set; }
        public int? Vtrtaskid { get; set; }
        public int? Trimin { get; set; }
        public int? Trimout { get; set; }
        public int? Signalid { get; set; }
        public int? Taskstate { get; set; }
        public string Usercode { get; set; }
        public DateTime Committime { get; set; }
        public int? Uploadorder { get; set; }
        public string Taskguid { get; set; }
        public int? Tapeid { get; set; }
        public string Taskname { get; set; }
        public string Usertoken { get; set; }
        public int? Triminctl { get; set; }
        public int? Trimoutctl { get; set; }
        public int? Vtrtasktype { get; set; }
    }
}
