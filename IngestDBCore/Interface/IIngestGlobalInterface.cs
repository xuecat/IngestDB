using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IngestDBCore.Interface
{
    public interface IIngestGlobalInterface
    {
        Task<ResponseMessage> SubmitGlobalCallBack(GlobalInternals examineResponse);
        Task<ResponseMessage> GetGlobalCallBack(GlobalInternals examineResponse);
    }
}
