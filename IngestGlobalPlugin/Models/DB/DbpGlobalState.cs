using System;
using System.Collections.Generic;

namespace IngestGlobalPlugin.Models
{
    public partial class DbpGlobalState
    {
        public string Label { get; set; }
        //[ConcurrencyCheck]
        public DateTime Lasttime { get; set; }
    }
}
