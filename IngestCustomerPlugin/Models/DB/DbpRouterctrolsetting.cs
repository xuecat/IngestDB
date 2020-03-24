using System;
using System.Collections.Generic;

namespace IngestCustomerPlugin.Models
{
    public partial class DbpRouterctrolsetting
    {
        public int Rcdeviceid { get; set; }
        public int Devicetypeid { get; set; }
        public string Rcdevicetype { get; set; }
        public string Rcdevicename { get; set; }
        public string Rcdevicedesc { get; set; }
        public string Rccomtrolcomport { get; set; }
        public int? Rcinportnum { get; set; }
        public int? Rcoutportnum { get; set; }
        public string Rcipaddress { get; set; }
    }
}
