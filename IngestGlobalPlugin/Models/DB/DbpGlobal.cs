using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace IngestGlobalPlugin.Models
{
    public partial class DbpGlobal
    {
        public string GlobalKey { get; set; }
        public string GlobalValue { get; set; }
        public DateTime Changetime { get; set; }
        public string Paramdesc { get; set; }
        public string Restartmodules { get; set; }
    }
}
