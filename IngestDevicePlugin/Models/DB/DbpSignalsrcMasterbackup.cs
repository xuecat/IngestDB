using System;
using System.Collections.Generic;

namespace IngestDevicePlugin.Models
{
    public partial class DbpSignalsrcMasterbackup
    {
        public int Signalsrcid { get; set; }
        public int Signalsrctype { get; set; }
        public int Ismastersrc { get; set; }
        public int? Mastersignalsrcid { get; set; }
    }
}
