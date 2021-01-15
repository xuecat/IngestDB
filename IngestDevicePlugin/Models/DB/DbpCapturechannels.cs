using System;
using System.Collections.Generic;

namespace IngestDevicePlugin.Models
{
    public partial class DbpCapturechannels
    {
        public int Channelid { get; set; }
        public string Channelname { get; set; }
        public string Channeldesc { get; set; }
        public int Cpdeviceid { get; set; }
        public int? Channelindex { get; set; }
        public int Devicetypeid { get; set; }
        public int? Backupflag { get; set; }
        public int? Cpsignaltype { get; set; }
        public string SystemSite { get; set; }
    }
}
