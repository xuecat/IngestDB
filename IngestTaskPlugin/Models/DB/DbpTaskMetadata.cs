using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IngestTaskPlugin.Models
{
    public partial class DbpTaskMetadata
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Taskid { get; set; }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Metadatatype { get; set; }
        public string Metadata { get; set; }
        public string Metadatalong { get; set; }
    }
}
