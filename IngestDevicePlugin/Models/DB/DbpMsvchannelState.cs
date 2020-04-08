using System;
using System.Collections.Generic;

namespace IngestDevicePlugin.Models
{
    public partial class DbpMsvchannelState
    {
        public int Channelid { get; set; }
        public int? Devstate { get; set; }
        public int? Msvmode { get; set; }
        public int? Sourcevtrid { get; set; }
        public string Curusercode { get; set; }
        public string Kamatakiinfo { get; set; }
        public int? Uploadstate { get; set; }
    }
}
