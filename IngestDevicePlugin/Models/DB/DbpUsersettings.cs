using System;
using System.Collections.Generic;

namespace IngestDevicePlugin.Models
{
    public partial class DbpUsersettings
    {
        public string Usercode { get; set; }
        public string Settingtype { get; set; }
        public string Settingtext { get; set; }
        public string Settingtextlong { get; set; }
    }
}
