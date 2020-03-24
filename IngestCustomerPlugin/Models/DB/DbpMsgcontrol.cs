using System;
using System.Collections.Generic;

namespace IngestCustomerPlugin.Models
{
    public partial class DbpMsgcontrol
    {
        public int Msgcontrolid { get; set; }
        public string Msgcontrolname { get; set; }
        public string Msgcontroldesc { get; set; }
        public string Ipaddress { get; set; }
    }
}
