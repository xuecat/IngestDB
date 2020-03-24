using System;
using System.Collections.Generic;

namespace IngestCustomerPlugin.Models
{
    public partial class Sequence
    {
        public string Name { get; set; }
        public int CurrentVal { get; set; }
        public int IncrementSize { get; set; }
    }
}
