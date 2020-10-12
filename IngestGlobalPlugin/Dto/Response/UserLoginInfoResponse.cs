using System;
using System.Collections.Generic;
using System.Text;

namespace IngestGlobalPlugin.Dto.Response
{
    public class UserLoginInfoResponse
    {
        public string Ip { get; set; }
        public int Port { get; set; }
        public string Usercode { get; set; }
        public DateTime Logintime { get; set; }
    }
}
