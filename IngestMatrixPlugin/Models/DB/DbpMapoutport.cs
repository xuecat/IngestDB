using System;
using System.Collections.Generic;

namespace IngestMatrixPlugin.Models.DB
{
    public partial class DbpMapoutport
    {
        public int Virtualoutport { get; set; }
        public int Outport { get; set; }
        public int Matrixid { get; set; }
    }
}
