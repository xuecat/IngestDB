using System;
using System.Collections.Generic;

namespace IngestDevicePlugin.Models
{
    public partial class DbpXdcamDevPlanMap
    {
        public int Deviceid { get; set; }
        public string Planningguid { get; set; }
        public string Planninginfo { get; set; }
    }
}
