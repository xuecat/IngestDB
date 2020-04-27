using System;
using System.Collections.Generic;

namespace IngestGlobalPlugin.Models
{
    public partial class DbpMaterialArchive
    {
        public int Materialid { get; set; }
        public int Policyid { get; set; }
        public int? Archivestate { get; set; }
        public int? Lastresult { get; set; }
        public int? Isprocessing { get; set; }
        public int? Failedtimes { get; set; }
        public DateTime Nextretry { get; set; }
        public DateTime Lastupdatetime { get; set; }
        public string Archiveresult { get; set; }
    }
}
