using System;
using System.Collections.Generic;

namespace IngestTaskPlugin.Models
{
    public partial class DbpXdcamTaskMetadata
    {
        public int Taskid { get; set; }
        public int Type { get; set; }
        public string Metadata { get; set; }
    }
}
