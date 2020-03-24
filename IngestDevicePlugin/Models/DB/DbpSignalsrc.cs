using System;
using System.Collections.Generic;

namespace IngestDevicePlugin.Models
{
    public partial class DbpSignalsrc
    {
        public int Signalsrcid { get; set; }
        public string Name { get; set; }
        public string Signaldesc { get; set; }
        public int Signaltypeid { get; set; }
        public int? Imagetype { get; set; }
        public int? Pureaudio { get; set; }
    }
}
