using System;
using System.Collections.Generic;

namespace IngestDevicePlugin.Models
{
    public partial class DbpIpDatachannelinfo
    {
        public int Datachannelid { get; set; }
        public string Datachannelname { get; set; }
        public int? Datachannelindex { get; set; }
        public int Deviceid { get; set; }
    }
}
