using System;
using System.Collections.Generic;

namespace IngestGlobalPlugin.Models
{
    public partial class DbpMsgFailedrecord
    {
        public string MsgGuid { get; set; }
        public int TaskId { get; set; }
        public int SectionId { get; set; }
        public DateTime? DealTime { get; set; }
        public string DealMsg { get; set; }
    }
}
