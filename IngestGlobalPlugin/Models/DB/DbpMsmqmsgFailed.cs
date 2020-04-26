using System;
using System.Collections.Generic;

namespace IngestGlobalPlugin.Models
{
    public partial class DbpMsmqmsgFailed
    {
        public string Msgid { get; set; }
        public int Actionid { get; set; }
        public string Msgcontent { get; set; }
        public DateTime Msgsendtime { get; set; }
        public DateTime Msgrevtime { get; set; }
        public int? Msgstatus { get; set; }
        public DateTime Msgprocesstime { get; set; }
        public int? Failedcount { get; set; }
        public int? Msgtype { get; set; }
        public DateTime Nextretry { get; set; }
        public string Lockdata { get; set; }
    }
}
