using System;
using System.Collections.Generic;

namespace IngestCustomerPlugin.Models
{
    public partial class DbpScheduler
    {
        public int Schedulerid { get; set; }
        public string Name { get; set; }
        public string Schedulerdesc { get; set; }
        public string Ipaddress { get; set; }
    }
}
