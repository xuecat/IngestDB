using System;
using System.Collections.Generic;

namespace IngestGlobalPlugin.Models
{
    public partial class DbpGlobalProgram
    {
        public int GlobalId { get; set; }
        public string ProgramName { get; set; }
        public int Signalsrcid { get; set; }
        public int? ProgramId { get; set; }
    }
}
