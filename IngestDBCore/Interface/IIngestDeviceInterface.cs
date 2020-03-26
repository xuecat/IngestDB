using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IngestDBCore.Interface
{
    public interface IIngestDeviceInterface
    {
        Task<ResponseMessage> SubmitDeviceCallBack(TaskInternals examineResponse);
        Task<ResponseMessage> GetDeviceCallBack(TaskInternals examineResponse);
    }
}
