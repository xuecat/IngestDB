using System;
using System.Collections.Generic;

namespace IngestDevicePlugin.Models
{
    public partial class DbpXdcamDiscMaterial
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int? Status { get; set; }
        public string Stamp { get; set; }
        public string Streamchannel { get; set; }
        public int? Duration { get; set; }
        public int? Inpoint { get; set; }
        public int? Outpoint { get; set; }
        public string Planningguid { get; set; }
        public int? Progress { get; set; }
    }
}
