using System;
using System.Collections.Generic;

namespace IngestCustomerPlugin.Models
{
    public partial class DbpMessages
    {
        public int MessageId { get; set; }
        public string MessageName { get; set; }
        public string MessageDesc { get; set; }
        public int? MessageType { get; set; }
    }
}
