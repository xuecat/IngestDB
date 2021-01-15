using System;
using System.Collections.Generic;

namespace IngestDevicePlugin.Models
{
    public partial class DbpRcdindesc
    {
        public int Signalsrcid { get; set; }
        public int Rcdeviceid { get; set; }
        public int Recinidx { get; set; }
        public int? Signalsource { get; set; }

        public int? Area { get; set; } 

        public string SystemSite { get; set; }
    }
}
