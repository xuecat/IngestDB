using IngestTaskPlugin.Dto.OldResponse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IngestTaskPlugin.Dto.Request
{
    /// <summary>
    /// 更新vtr任务元数据请求
    /// </summary>
    public class VTRTaskMetaDataRequest
    {
        /// <summary>
        /// 元数据类型
        /// </summary>
        /// <example>1</example>
        public MetaDataType Type { get; set; }

        /// <summary>
        /// 元数据
        /// </summary>
        /// <example>metadata</example>
        public string MetaData { get; set; }
    }
}
