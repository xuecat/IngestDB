using System;
using System.Collections.Generic;

namespace IngestCustomerPlugin.Models
{
    public partial class DbpUserparamlog
    {
        public int Logid { get; set; }
        public int? Userid { get; set; }
        public int? Opratype { get; set; }
        public DateTime Opratime { get; set; }
        public int? Moddifyuserid { get; set; }
    }
}
