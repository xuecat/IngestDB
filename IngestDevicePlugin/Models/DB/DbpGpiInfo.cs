using System;
using System.Collections.Generic;

namespace IngestDevicePlugin.Models
{
    public partial class DbpGpiInfo
    {
        public int Gpiid { get; set; }
        public string Gpiname { get; set; }
        public int? Comport { get; set; }
        public int? Outputportcount { get; set; }
        public string Description { get; set; }
    }
}
