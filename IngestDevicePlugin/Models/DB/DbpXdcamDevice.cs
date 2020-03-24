using System;
using System.Collections.Generic;

namespace IngestDevicePlugin.Models
{
    public partial class DbpXdcamDevice
    {
        public int Deviceid { get; set; }
        public string Devicename { get; set; }
        public int Devicetype { get; set; }
        public string Devicedesc { get; set; }
        public string Ftpaddress { get; set; }
        public string Loginname { get; set; }
        public string Password { get; set; }
        public string Storagepath { get; set; }
        public int? Workmode { get; set; }
        public string Serverip { get; set; }
        public int? Devicestate { get; set; }
        public int? Discid { get; set; }
    }
}
