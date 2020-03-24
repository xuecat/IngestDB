using System;
using System.Collections.Generic;

namespace IngestDevicePlugin.Models
{
    public partial class DbpRcdoutdesc
    {
        public int Channelid { get; set; }
        public int Rcdeviceid { get; set; }
        public int Recoutidx { get; set; }
        public int? Devicetype { get; set; }
    }
}
