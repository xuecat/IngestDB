using System;
using System.Collections.Generic;

namespace IngestGlobalPlugin.Models
{
    public partial class DbpMaterialAudio
    {
        public int Materialid { get; set; }
        public string Audiofilename { get; set; }
        public int Audiotypeid { get; set; }
        public int Audiosource { get; set; }
    }
}
