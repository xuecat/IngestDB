using System;
using System.Collections.Generic;
using System.Text;

namespace IngestDBInterface
{
    public interface IIngestTaskInterface
    {
        Task<GatewayInterface.Dto.ResponseMessage> SubmitHumanCallback(ExamineResponse examineResponse);
        Task<GatewayInterface.Dto.ResponseMessage> UpdateRecordHumanCallback(ExamineResponse examineResponse);
    }
}
