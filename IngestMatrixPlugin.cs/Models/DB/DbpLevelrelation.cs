using System;
using System.Collections.Generic;

namespace IngestMatrixPlugin.Models.DB
{
    public partial class DbpLevelrelation
    {
        public int Matrixid { get; set; }
        public int Inport { get; set; }
        public int Parentoutport { get; set; }
        public int Parentmatrixid { get; set; }
    }
}
