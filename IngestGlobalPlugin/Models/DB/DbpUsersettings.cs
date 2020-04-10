using System;
using System.Collections.Generic;
using System.Text;

namespace IngestGlobalPlugin.Models
{
    public partial class DbpUsersettings
    {
        public string Usercode { get; set; }
        public string Settingtype { get; set; }
        public string Settingtext { get; set; }
        public string Settingtextlong { get; set; }
    }
}
