using System;
using System.Collections.Generic;
using System.Text;

namespace IngestTaskPlugin.Dto.Response
{
    public class WarningInfoResponse
    {
        /// <summary>任务id</summary>
        /// <example>1</example>
        public int TaskID { get; set; }
        /// <summary>关联id</summary>
        /// <example>1</example>
        public int RelatedID { get; set; }
        /// <summary>警告等级</summary>
        /// <example>string</example>
        public int WarningLevel { get; set; }
        /// <summary>警告消息</summary>
        /// <example>string</example>
        public string WarningMessage { get; set; }
    }
}
