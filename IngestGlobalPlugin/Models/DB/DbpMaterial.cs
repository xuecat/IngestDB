using System;
using System.Collections.Generic;

namespace IngestGlobalPlugin.Models
{
    public partial class DbpMaterial
    {
        public int Materialid { get; set; }
        public string Name { get; set; }
        public string Remark { get; set; }
        public DateTime Createtime { get; set; }
        public int Taskid { get; set; }
        public int? Sectionid { get; set; }
        public string Guid { get; set; }
        public int? Clipstate { get; set; }
        public string Usercode { get; set; }
        public int? Deletedstate { get; set; }
    }
}
