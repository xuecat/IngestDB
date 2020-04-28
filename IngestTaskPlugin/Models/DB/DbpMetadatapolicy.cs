using System;
using System.Collections.Generic;

namespace IngestTaskPlugin.Models
{
    public partial class DbpMetadatapolicy
    {
        public int Policyid { get; set; }
        public string Policyname { get; set; }
        public string Policydesc { get; set; }
        public int? Defaultpolicy { get; set; }
        public string Archivetype { get; set; }
    }
}
