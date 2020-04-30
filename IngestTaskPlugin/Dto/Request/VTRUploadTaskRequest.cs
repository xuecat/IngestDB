namespace IngestTaskPlugin.Dto.Request
{
    using System.Collections.Generic;
    using IngestTaskPlugin.Dto.Response;
    using IngestTaskPlugin.Dto.Response.OldVtr;

    /// <summary>
    /// Defines the <see cref="VTRUploadTaskRequest" />.
    /// </summary>
    public class VTRUploadTaskRequest
    {
        /// <summary>
        /// Vtr任务
        /// </summary>
        public VTRUploadTaskContentResponse VtrTask { get; set; }

        /// <summary>
        /// 元数据
        /// </summary>
        public List<VTRUploadMetadataPair> Metadatas { get; set; }

        /// <summary>
        /// 掩码
        /// </summary>
        /// <example>134217728</example>
        public long Mask { get; set; }

        /// <summary>
        /// 上载任务掩码
        /// </summary>
        /// <example>134217728</example>
        public VTRUploadTaskMask UploadTaskMask { get; set; }
    }

    public class VTRUploadMetadataPair
    {
        /// <summary>
        /// Vtr任务
        /// </summary>
        /// <example>1</example>
        public int TaskId;
        /// <summary>
        /// 元数据
        /// </summary>
        /// <example>VTRUploadMetadataPair元数据</example>
        public string Metadata;
        /// <summary>
        /// 元数据
        /// </summary>
        /// <example>1</example>
        public int Type;
    }
}
