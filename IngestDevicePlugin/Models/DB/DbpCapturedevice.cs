using System;
using System.Collections.Generic;

namespace IngestDevicePlugin.Models
{
    public partial class DbpCapturedevice
    {
        public int Cpdeviceid { get; set; }
        public int Devicetypeid { get; set; }
        public string Devicename { get; set; }
        public string Ipaddress { get; set; }
        public int? Ordercode { get; set; }
    }
}
