using IngestTaskPlugin.Dto.Request;
using System;
using System.Collections.Generic;
using System.Text;

namespace IngestTaskPlugin.Dto.Response.OldVtr
{
    public class AddVTRBatchUploadTasks_in
    {
        public List<VTRUploadTaskContent> vtrTasks;
        public List<VTR_UPLOAD_MetadataPair> metadatas;
        public bool ignoreWrong;
    }
    
}
