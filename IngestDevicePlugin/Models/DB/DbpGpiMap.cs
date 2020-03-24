using System;
using System.Collections.Generic;

namespace IngestDevicePlugin.Models
{
    public partial class DbpGpiMap
    {
        public int Gpiid { get; set; }
        public int Gpioutputport { get; set; }
        public int? Avoutputport { get; set; }
        public int? Captureparamid { get; set; }
    }
}
