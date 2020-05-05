using IngestTaskPlugin.Dto.Response;
using System;
using System.Collections.Generic;
using System.Text;
using VTRUploadTaskContentRequest = IngestTaskPlugin.Dto.Response.VTRUploadTaskContentResponse;

namespace IngestTaskPlugin.Dto.Request
{
    public class VTRBatchUploadTaskRequest
    {
        public List<VTRUploadTaskContentResponse> VtrTasks { get; set; }
        public List<VTRUploadMetadataPair> Metadatas { get; set; }
        public bool IgnoreWrong { get; set; }
    }
}
