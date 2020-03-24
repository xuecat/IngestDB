using System;
using System.Collections.Generic;

namespace IngestCustomerPlugin.Models
{
    public partial class DbpDevConn
    {
        public int DevType { get; set; }
        public int? DevMaxconn { get; set; }
    }
}
