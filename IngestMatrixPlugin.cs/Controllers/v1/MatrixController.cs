using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IngestDBCore;
using IngestMatrixPlugin.Dto.Response;
using Microsoft.AspNetCore.Mvc;
using ResponseMessage = IngestMatrixPlugin.Dto.Response.ResponseMessage;

namespace IngestMatrixPlugin.Controllers
{
    public partial class MatrixController : ControllerBase
    {
        /// <summary>矩阵开关</summary>
        /// <param name="lInPort"></param>
        /// <param name="lOutPort"></param>
        /// <param name="lTimeOut"></param>
        /// <returns></returns>
        [HttpGet("matrix/SwitchInOut"), MapToApiVersion("1.0")]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<ResponseMessage> SwitchInOut([FromQuery]int lInPort, [FromQuery]int lOutPort, [FromQuery]int lTimeOut)
        {
            async Task Action(ResponseMessage response)
            {
                m_Matrixmt.WaitOne();
                lock (lockObj)  //保证只有一个切换在进行
                {
                    response.nCode = await _matrixManage.SwitchInOutByArea<lInPort, lOutPort, lTimeOut>();
                }
            }
            m_Matrixmt.ReleaseMutex();
            return await TryInvoke((Func<ResponseMessage, Task>)Action);
        }


        public async Task<TResponse> TryInvoke<TResponse>(Func<TResponse, Task> func) where TResponse : ResponseMessage, new()
        {
            TResponse response = new TResponse();
            try
            {
                await func(response);
            }
            catch (Exception e)
            {
                if (e is SobeyRecException se)//sobeyexcep会自动打印错误
                {
                    response.message = se.Message;
                }
                else
                {
                    response.message = $"error info:{e}";
                    Logger.Error(response.message);
                }
                response.nCode = 0;
            }
            return response;
        }
    }
}
