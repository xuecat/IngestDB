using System;
using System.Collections.Generic;

namespace IngestCustomerPlugin.Models
{
    public partial class DbpNewimportRemote
    {
        public string RemoteId { get; set; }
        public string ClipId { get; set; }
        public string TaskId { get; set; }
        public int? RemoteState { get; set; }
    }
}
