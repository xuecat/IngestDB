using System;
using System.Collections.Generic;

namespace IngestTaskPlugin.Models
{
    public partial class DbpTaskgroup
    {
        public string GroupName { get; set; }
        public string GroupDesc { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime ChangeTime { get; set; }
        public int GroupId { get; set; }
    }
}
