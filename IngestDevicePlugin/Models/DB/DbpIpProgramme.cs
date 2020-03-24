using System;
using System.Collections.Generic;

namespace IngestDevicePlugin.Models
{
    public partial class DbpIpProgramme
    {
        public int Programmeid { get; set; }
        public string Programmename { get; set; }
        public string Programmedesc { get; set; }
        public int Datachannelid { get; set; }
        public int? Programmeindex { get; set; }
        public int? Programmetype { get; set; }
        public int? Imagetype { get; set; }
        public string Tssignalinfo { get; set; }
        public string Multicastip { get; set; }
        public int? Multicastport { get; set; }
        public string Extendparams { get; set; }
        public int? Pureaudio { get; set; }
    }
}
