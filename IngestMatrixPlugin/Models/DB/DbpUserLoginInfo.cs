using System;
using System.Collections.Generic;
using System.Text;

namespace IngestMatrixPlugin.Models.DB
{
    public partial class DbpUserLoginInfo
    {
        public string Ip { get; set; }
        public int Port { get; set; }
        public string Usercode { get; set; }
        public DateTime Logintime { get; set; }
    }

}
