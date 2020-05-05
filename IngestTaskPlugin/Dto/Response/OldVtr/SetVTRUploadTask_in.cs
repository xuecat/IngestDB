using IngestTaskPlugin.Dto.Request;
using System;
using System.Collections.Generic;
using System.Text;

namespace IngestTaskPlugin.Dto.Response.OldVtr
{
    public class SetVTRUploadTask_in
    {
        public VTRUploadTaskContent vtrTask;
        public List<VTR_UPLOAD_MetadataPair> metadatas;
        public long lMask;
        public VTRUploadTaskMask uploadTaskMask;
    }
}
