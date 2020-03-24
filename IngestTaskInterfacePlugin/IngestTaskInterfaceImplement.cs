using IngestDBCore;
using IngestDBCore.Interface;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IngestTaskInterfacePlugin
{
    public class IngestTaskInterfaceImplement : IIngestTaskInterface
    {
        public async Task<ResponseMessage> GetTaskCallBack(TaskInternals examineResponse)
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseMessage> SubmitTaskCallBack(TaskInternals examineResponse)
        {
            throw new NotImplementedException();
        }
    }
}
