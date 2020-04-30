namespace IngestTaskPlugin.Dto.Request
{
    using System.Collections.Generic;
    using IngestTaskPlugin.Dto.Response.OldVtr;

    /// <summary>
    /// Defines the <see cref="AddVTRUploadTask_in" />.
    /// </summary>
    public class AddVTRUploadTask_in
    {
        /// <summary>
        /// Vtr任务.
        /// </summary>
        public VTRUploadTaskContent vtrTask;

        /// <summary>
        /// Vtr上载元数据
        /// </summary>
        public List<VTR_UPLOAD_MetadataPair> metadatas;
    }
}
