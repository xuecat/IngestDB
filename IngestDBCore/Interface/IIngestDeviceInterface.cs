using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IngestDBCore.Interface
{
    public interface IIngestDeviceInterface
    {
        Task<ResponseMessage> SubmitDeviceCallBack(DeviceInternals examineResponse);
        Task<ResponseMessage> GetDeviceCallBack(DeviceInternals examineResponse);
    }
}
