using System;
using System.Collections.Generic;

namespace IngestDevicePlugin.Models
{
    public partial class DbpXdcamDiskinfo
    {
        public int Diskid { get; set; }
        public int? Deviceid { get; set; }
        public string Diskname { get; set; }
        public DateTime Createtime { get; set; }
        public string Diskumid { get; set; }
        public string DiskinfoDesc { get; set; }
        public int? Diskstate { get; set; }
        public string Storagepath { get; set; }
        public int? Progress { get; set; }
        public float Capacity { get; set; }
        public float DiskinfoUsage { get; set; }
    }
}
