using ShardingCore.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IngestTaskPlugin.Models
{
    public partial class DbpTaskMetadata : IShardingTable
    {
        [ShardingTableKey(TailPrefix ="")]
        public int Taskid { get; set; }
        public int Metadatatype { get; set; }

        public string Metadata { get; set; }
        public string Metadatalong { get; set; }
    }
}
