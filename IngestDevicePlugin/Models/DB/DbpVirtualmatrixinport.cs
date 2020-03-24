using System;
using System.Collections.Generic;

namespace IngestDevicePlugin.Models
{
    public partial class DbpVirtualmatrixinport
    {
        public int Matrixid { get; set; }
        public int Virtualinport { get; set; }
        public int Sourcetype { get; set; }
        public int? Signaltype { get; set; }
    }
}
