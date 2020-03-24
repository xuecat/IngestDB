using System;
using System.Collections.Generic;

namespace IngestTaskPlugin.Models
{
    public partial class VtrTaskMeatdata
    {
        public int Vtrtaskid { get; set; }
        public int Metadatatype { get; set; }
        public string Matadata { get; set; }
    }
}
