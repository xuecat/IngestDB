using System;
using System.Collections.Generic;
using System.Text;

namespace IngestTaskPlugin.Dto.Response
{
    public class MetaDataPolicyResponse
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Desc { get; set; }
        public int DefaultPolicy { get; set; }
        public string ArchiveType { get; set; }
    }
}
