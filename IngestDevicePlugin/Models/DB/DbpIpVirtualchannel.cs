using System;
using System.Collections.Generic;

namespace IngestDevicePlugin.Models
{
    public partial class DbpIpVirtualchannel
    {
        public int Channelid { get; set; }
        public string Channelname { get; set; }
        public string Channeldesc { get; set; }
        public int Deviceid { get; set; }
        public int? Deviceindex { get; set; }
        public string Ipaddress { get; set; }
        public int? Ctrlport { get; set; }
        public int? Channelstatus { get; set; }
        public int? Backuptype { get; set; }
        public int? Channeltype { get; set; }
        public int? Carrierid { get; set; }
        public int? Cpsignaltype { get; set; }
    }
}
