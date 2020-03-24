using System;
using System.Collections.Generic;

namespace IngestCustomerPlugin.Models
{
    public partial class DbpMaterialDuration
    {
        public int Materialid { get; set; }
        public int? Duration { get; set; }
        public int? Taskid { get; set; }
        public int? Trimin { get; set; }
    }
}
