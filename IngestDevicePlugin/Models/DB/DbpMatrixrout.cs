using System;
using System.Collections.Generic;

namespace IngestDevicePlugin.Models
{
    public partial class DbpMatrixrout
    {
        public int Matrixid { get; set; }
        public int Inport { get; set; }
        public int Outport { get; set; }
        public int Virtualoutport { get; set; }
        public int Virtualinport { get; set; }
        public int State { get; set; }
        public DateTime Begintime { get; set; }
        public DateTime Endtime { get; set; }
    }
}
