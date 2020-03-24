using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IngestDBCore.Interface
{
    public interface IIngestTaskInterface
    {
        Task<ResponseMessage> SubmitTaskCallBack(TaskInternals examineResponse);
        Task<ResponseMessage> GetTaskCallBack(TaskInternals examineResponse);
    }
}
