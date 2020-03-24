using System;
using System.Collections.Generic;

namespace IngestDevicePlugin.Models
{
    public partial class DbpIpDevice
    {
        public int Deviceid { get; set; }
        public string Devicename { get; set; }
        public string Devicedesc { get; set; }
        public string Ipaddress { get; set; }
        public int? Port { get; set; }
    }
}
