using System;
using System.Collections.Generic;

namespace IngestTaskPlugin.Models
{
    public partial class VtrTaskInout
    {
        public int Taskid { get; set; }
        public int Inpoint { get; set; }
        public int Outpoint { get; set; }
    }
}
