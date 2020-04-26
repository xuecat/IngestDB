using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IngestDBCore.Dto;

namespace IngestDBCore.Interface
{
    public interface IIngestMatrixInterface
    {
        Task<ResponseMessage> SubmitMatrixCallBack(MatrixInternals examineResponse);
        Task<ResponseMessage> GetMatrixCallBack(MatrixInternals examineResponse);
    }
}
