namespace IngestTaskPlugin.Dto.Request
{
    /// <summary>
    /// Defines the 更新Vtr上传任务状态请求 <see cref="VTRUploadStateRequest" />.
    /// </summary>
    public class VTRUploadStateRequest
    {
        /// <summary>
        /// Gets or sets the 任务状态.
        /// </summary>
        /// <example>5</example>
        public VTRUPLOADTASKSTATE TaskState { get; set; }

        /// <summary>
        /// Gets or sets the 错误内容.
        /// </summary>
        /// <example>"错误内容"</example>
        public string ErrorContent { get; set; }
    }
}
