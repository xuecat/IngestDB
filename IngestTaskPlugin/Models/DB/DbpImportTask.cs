using System;
using System.Collections.Generic;

namespace IngestTaskPlugin.Models
{
    public partial class DbpImportTask
    {
        public int Id { get; set; }
        public string Guid { get; set; }
        public string Usercode { get; set; }
        public string Metadatas { get; set; }
        public string Files { get; set; }
        public string Excursus { get; set; }
    }
}
