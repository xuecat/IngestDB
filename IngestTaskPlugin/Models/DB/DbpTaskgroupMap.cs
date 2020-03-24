using System;
using System.Collections.Generic;

namespace IngestTaskPlugin.Models
{
    public partial class DbpTaskgroupMap
    {
        public int TaskId { get; set; }
        public DateTime ChangeDate { get; set; }
        public int? GroupId { get; set; }
    }
}
