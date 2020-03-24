using System;
using System.Collections.Generic;

namespace IngestCustomerPlugin.Models
{
    public partial class LoginUserAddress
    {
        public int Port { get; set; }
        public string Ip { get; set; }
        public string Username { get; set; }
        public DateTime Logintime { get; set; }
    }
}
