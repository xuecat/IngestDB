using System;
using System.Collections.Generic;

namespace IngestCustomerPlugin.Models
{
    public partial class DbpPlans
    {
        public int Planid { get; set; }
        public string Metadata { get; set; }
        public string Plancontent { get; set; }
        public DateTime Createdate { get; set; }
    }
}
