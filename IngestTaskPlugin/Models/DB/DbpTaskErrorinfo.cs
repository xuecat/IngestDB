using System;
using System.Collections.Generic;

namespace IngestTaskPlugin.Models
{
    public partial class DbpTaskErrorinfo
    {
        public string Infoid { get; set; }
        public int Taskid { get; set; }
        public int? Errorcode { get; set; }
        public string Errtime { get; set; }
        public int? Errlevel { get; set; }
        public int? Errtype { get; set; }
        public string Errdesc { get; set; }
        public string Errmodule { get; set; }
    }
}
