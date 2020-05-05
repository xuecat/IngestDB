using IngestTaskPlugin.Dto.Request;
using System;
using System.Collections.Generic;
using System.Text;

namespace IngestTaskPlugin.Dto.Response
{
    public class VtrBatchUploadTaskResponse
    {
        //VTR_BUT_ErrorCode 类型的枚举
        public VTR_BUT_ErrorCode errorCode { get; set; }
        public List<int> taskIds { get; set; }
    }
}
