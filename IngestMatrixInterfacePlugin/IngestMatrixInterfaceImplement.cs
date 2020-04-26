using AutoMapper;
using IngestDBCore;
using IngestDBCore.Dto;
using IngestDBCore.Interface;
using IngestMatrixPlugin.Controllers;
using IngestMatrixPlugin.Controllers.v2;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IngestGlobalInterfacePlugin
{
    public class IngestMatrixInterfaceImplement : IIngestMatrixInterface
    {
        public async Task<ResponseMessage> GetMatrixCallBack(MatrixInternals examineResponse)
        {
            return null;
        }

        public async Task<ResponseMessage> SubmitMatrixCallBack(MatrixInternals examineResponse)
        {
            return null;
        }
    }
}
