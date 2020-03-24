using System;
using System.Collections.Generic;

namespace IngestTaskPlugin.Models
{
    public partial class DbpTaskMetadata
    {
        public int Taskid { get; set; }
        public int Metadatatype { get; set; }
        public string Metadata { get; set; }
        public string Metadatalong { get; set; }
    }
}
