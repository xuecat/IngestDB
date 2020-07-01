using System;
using System.Collections.Generic;
using System.Text;

namespace IngestGlobalPlugin.Dto.Response
{
    public class FileFormateInfoResponse
    {
        /// <summary>key标识</summary>
        /// <example>123345</example>
        public string Key { get; set; }
        /// <summary>文件格式</summary>
        /// <example>1</example>
        public long FormatId { get; set; }
        /// <summary>视频格式</summary>
        /// <example>1</example>
        public long VideoStrandId { get; set; }
        /// <summary>视频格式guid</summary>
        /// <example>123345</example>
        public string VideoStrandGuid { get; set; }
        /// <summary>额外信息存宽幅，码率，帧率</summary>
        /// <example>12344</example>
        public string ExtraInfo { get; set; }
    }
}
