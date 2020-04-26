using IngestDBCore;
using IngestDBCore.Basic;
using IngestMatrixPlugin.Managers;
using Microsoft.AspNetCore.Mvc;
using Sobey.Core.Log;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace IngestMatrixPlugin.Controllers.v1
{
    /// <summary>
    /// V1矩阵接口控制器
    /// </summary>
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    public partial class MatrixController : ControllerBase
    {
        private readonly ILogger Logger = LoggerManager.GetLogger("DeviceInfo");
        private readonly MatrixManager _matrixManage;
        //private readonly RestClient _restClient;

        public MatrixController(MatrixManager task) { _matrixManage = task; }

        /// <summary>
        /// 矩阵开关
        /// </summary>
        /// <param name="lInPort"></param>
        /// <param name="lOutPort"></param>
        /// <param name="lTimeOut"></param>
        /// <returns></returns>
        [HttpGet("SwitchInOut"), MapToApiVersion("1.0")]
        [ApiExplorerSettings(GroupName = "v1")]
        public IngestMatrixPlugin.Dto.Response.v1.MatrixOldResponseMessage SwitchInOut([FromQuery]int lInPort,
                                                                                       [FromQuery]int lOutPort,
                                                                                       [FromQuery]int lTimeOut)
        {
            IngestMatrixPlugin.Dto.Response.v1.MatrixOldResponseMessage response = new IngestMatrixPlugin.Dto.Response.v1.MatrixOldResponseMessage();
            try
            {
                if (lInPort <= 0 || lOutPort <= 0)
                {
                    throw new Exception("Switch failed！ param is invailed");
                }
                IngestMatrixPlugin.Controllers.v2.MatrixController.m_MatrixMt.WaitOne();
                lock (IngestMatrixPlugin.Controllers.v2.MatrixController.lockObj)  //保证只有一个切换在进行
                {
                    response.nCode = _matrixManage.SwitchInOutAsync(lInPort, lOutPort).Result ? 1 : 0;
                }
                IngestMatrixPlugin.Controllers.v2.MatrixController.m_MatrixMt.ReleaseMutex();
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
        [HttpGet("GetInPortFromOutPort"), MapToApiVersion("1.0")]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<Dto.Response.v1.MatrixOldResponseMessage<long>> GetInPortFromOutPort([FromQuery]long OutPort)
        {
            Dto.Response.v1.MatrixOldResponseMessage<long> response = new Dto.Response.v1.MatrixOldResponseMessage<long>();
            try
            {
                response.extention = await _matrixManage.GetInPortFromOutPortAsync(OutPort);
                if(response.extention == -1)
                {
                    response.message = "获取矩阵入口出错";
                    response.nCode = 0;
                }
            } catch(Exception e)
            {
                if(e is SobeyRecException se)//sobeyexcep会自动打印错误
                {
                    response.message = se.Message;
                } else
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
        [HttpGet("QueryLinkState"), MapToApiVersion("1.0")]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<Dto.Response.v1.MatrixOldResponseMessage<string>> QueryLinkState()
        {
            Dto.Response.v1.MatrixOldResponseMessage<string> res = new Dto.Response.v1.MatrixOldResponseMessage<string>();
            try
            {
                res.extention = await _matrixManage.QueryLinkStateAsync();
                if(string.IsNullOrEmpty(res.extention))
                {
                    res.extention = "<?xml version=\"1.0\" encoding=\"UTF-8\" ?><MatrixLinkState/>";
                    res.nCode = 0;
                }
            } catch(Exception e)
            {
                if(e is SobeyRecException se)//sobeyexcep会自动打印错误
                {
                    res.message = se.Message;
                } else
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
