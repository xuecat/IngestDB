using System;
using System.Collections.Generic;

namespace IngestTaskPlugin.Models
{
    public partial class DbpXdcamTaskfile
    {
        public int Taskfileid { get; set; }
        public int Taskid { get; set; }
        public string Filename { get; set; }
        public int? Filetype { get; set; }
        public string Fileumid { get; set; }
        public int? Progress { get; set; }
        public int? Filestatus { get; set; }
        public int? Inpoint { get; set; }
        public int? Outpoint { get; set; }
    }
}
