using System;
using System.Collections.Generic;

namespace IngestCustomerPlugin.Models
{
    public partial class DbpUsertemplate
    {
        public int Templateid { get; set; }
        public string Usercode { get; set; }
        public string Templatename { get; set; }
        public string Templatecontent { get; set; }
    }
}
