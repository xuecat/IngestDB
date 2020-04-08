using System;
using System.Collections.Generic;
using System.Text;

namespace IngestGlobalPlugin.Models
{
    public partial class DbpObjectstateinfo
    {
        public int Objectid { get; set; }
        public int Objecttypeid { get; set; }
        public string Username { get; set; }
        public DateTime Begintime { get; set; }
        public int Timeout { get; set; }
        public string Locklock { get; set; }
    }
}
