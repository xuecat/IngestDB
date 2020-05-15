namespace IngestTaskPlugin.Dto.Response.OldVtr
{

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
