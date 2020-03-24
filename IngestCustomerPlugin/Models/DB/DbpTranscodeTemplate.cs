using System;
using System.Collections.Generic;

namespace IngestCustomerPlugin.Models
{
    public partial class DbpTranscodeTemplate
    {
        public int Trancodeid { get; set; }
        public string Transcodename { get; set; }
        public string TranscodeInfo { get; set; }
    }
}
