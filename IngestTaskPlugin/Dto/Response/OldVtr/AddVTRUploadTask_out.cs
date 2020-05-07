namespace IngestTaskPlugin.Dto.Response.OldVtr
{
    /// <summary>
    /// Defines the <see cref="AddVTRUploadTask_out" />.
    /// </summary>
    public class AddVTRUploadTask_out
    {
        /// <summary>
        /// Vtr上载任务及内容
        /// </summary>
        public VTRUploadTaskContent vtrTask;

        /// <summary>
        /// 错误代码
        /// </summary>
        public int errorCode;
    }

    public class AddVTRUploadTaskResponse
    {
        /// <summary>
        /// Vtr上载任务及内容
        /// </summary>
        public VTRUploadTaskContentResponse VtrTask { get; set; }

        /// <summary>
        /// 错误代码
        /// </summary>
        public int ErrorCode { get; set; }
    }
}
