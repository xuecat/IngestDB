using System;
using System.Collections.Generic;

namespace IngestDevicePlugin.Models
{
    public partial class DbpSignalDeviceMap
    {
        public int Signalsrcid { get; set; }
        public int Deviceid { get; set; }
        public int? Deviceoutportidx { get; set; }
        public int? Signalsource { get; set; }
    }
}
