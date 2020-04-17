using System;
using System.Collections.Generic;

namespace IngestDevicePlugin.Models
{
    public partial class DbpStreammedia
    {
        public int Streammediaid { get; set; }
        public string Streammedianame { get; set; }
        public string Streammediadesc { get; set; }
        public string Streammediaurl { get; set; }
        public int? Streammediatype { get; set; }
        public int? Imagetype { get; set; }
        public string Urltype { get; set; }
        public string Extendparams { get; set; }
        public int? Carrierid { get; set; }
        public int? Pureaudio { get; set; }
    }
}
