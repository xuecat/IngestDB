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
            ResponseMessage response = new ResponseMessage();
            try
            {
                if (lInPort <= 0 || lOutPort <= 0)
                {
                    throw new Exception("Switch failed！ param is invailed");
                }
                m_Matrixmt.WaitOne();
                lock (lockObj)  //保证只有一个切换在进行
                {
                    response.nCode = _matrixManage.SwitchInOutAsync(lInPort, lOutPort).Result ? 1 : 0;
                }
                m_Matrixmt.ReleaseMutex();
            }
            catch (Exception e)
            {
                if (e is SobeyRecException se)//sobeyexcep会自动打印错误
                {
                    response.message = se.Message;
                }
                else
                {
                    response.message = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}：error info:{e}";
                    Logger.Error(response.message);
                }
                response.nCode = 0;
            }
            return response;
        }

        /// <summary>
        /// 通过出点获取入点
        /// </summary>
        /// <param name="OutPort"></param>
        /// <returns>nCode,message,扩展字段为inPort</returns>
        [HttpGet("matrix/GetInPortFromOutPort"), MapToApiVersion("1.0")]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<Dto.Response.ResponseMessage<long>> GetInPortFromOutPort([FromQuery]long OutPort)
        {
            Dto.Response.ResponseMessage<long> response = new Dto.Response.ResponseMessage<long>();
            try
            {
                response.extention = await _matrixManage.GetInPortFromOutPortAsync(OutPort);
                if (response.extention == -1)
                {
                    response.message = "获取矩阵入口出错";
                    response.nCode = 0;
                }
            }
            catch (Exception e)
            {
                if (e is SobeyRecException se)//sobeyexcep会自动打印错误
                {
                    response.message = se.Message;
                }
                else
                {
                    response.message = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}：error info:{e}";
                    Logger.Error(response.message);
                }
                response.nCode = 0;
            }
            return response;
        }

        /// <summary>
        /// 查询矩阵链接状态
        /// </summary>
        /// <returns></returns>
        [HttpGet("matrix/QueryLinkState"), MapToApiVersion("1.0")]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<Dto.Response.ResponseMessage<string>> QueryLinkState()
        {
            Dto.Response.ResponseMessage<string> res = new Dto.Response.ResponseMessage<string>();
            try
            {
                res.extention = await _matrixManage.QueryLinkStateAsync();
                if (string.IsNullOrEmpty(res.extention))
                {
                    res.extention = "<?xml version=\"1.0\" encoding=\"UTF-8\" ?><MatrixLinkState/>";
                    res.nCode = 0;
                }
            }
            catch (Exception e)
            {
                if (e is SobeyRecException se)//sobeyexcep会自动打印错误
                {
                    res.message = se.Message;
                }
                else
                {
                    res.message = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}：error info:{e}";
                    Logger.Error(res.message);
                }
                res.nCode = 0;
            }
            return res;
        }
    }
}
