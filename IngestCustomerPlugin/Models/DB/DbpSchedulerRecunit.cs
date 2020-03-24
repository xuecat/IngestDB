using System;
using System.Collections.Generic;

namespace IngestCustomerPlugin.Models
{
    public partial class DbpSchedulerRecunit
    {
        public int Schedulerid { get; set; }
        public int Recid { get; set; }
        public DateTime Respond { get; set; }
        public int? Activesch { get; set; }
    }
}
