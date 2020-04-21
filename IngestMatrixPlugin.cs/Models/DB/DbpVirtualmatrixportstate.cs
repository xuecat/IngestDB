using System;
using System.Collections.Generic;

namespace IngestMatrixPlugin.Models.DB
{
    public partial class DbpVirtualmatrixportstate
    {
        public int Virtualinport { get; set; }
        public int Virtualoutport { get; set; }
        public int? Matrixid { get; set; }
        public int State { get; set; }
        public DateTime Lastoprtime { get; set; }
    }
}
