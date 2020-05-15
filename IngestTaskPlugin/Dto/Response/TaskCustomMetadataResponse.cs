using System;
using System.Collections.Generic;
using System.Text;

namespace IngestTaskPlugin.Dto.Response
{
    public class TaskCustomMetadataResponse
    {
        /// <summary>任务id</summary>
        /// <example>1</example>
        public int TaskID { get; set; }
        /// <summary>任务元数据</summary>
        /// <example>任务元数据</example>
        public string Metadata { get; set; }
    }
}
