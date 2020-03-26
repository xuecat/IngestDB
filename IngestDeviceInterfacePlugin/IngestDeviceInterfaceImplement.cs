using IngestDBCore;
using IngestDBCore.Interface;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IngestTaskInterfacePlugin
{
    public class IngestDeviceInterfaceImplement : IIngestDeviceInterface
    {
        public Task<ResponseMessage> GetDeviceCallBack(TaskInternals examineResponse)
        {
            throw new NotImplementedException();
        }
        
        public Task<ResponseMessage> SubmitDeviceCallBack(TaskInternals examineResponse)
        {
            throw new NotImplementedException();
        }
        
    }
}
