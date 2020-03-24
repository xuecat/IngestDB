using System;
using System.Collections.Generic;

namespace IngestCustomerPlugin.Models
{
    public partial class DbpMaterialAudioBackup
    {
        public int Materialid { get; set; }
        public string Audiofilename { get; set; }
        public int Audiotypeid { get; set; }
        public int Audiosource { get; set; }
    }
}
