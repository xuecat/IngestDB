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
        private readonly ILogger Logger = LoggerManager.GetLogger("Matrixinfo");
        private readonly MatrixManager _matrixManage;
        //private readonly RestClient _restClient;

        public MatrixController(MatrixManager task) { _matrixManage = task; }

       

        /// <summary>
        /// 矩阵开关
        /// </summary>
        /// <param name="outPort"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        [HttpGet("SwitchRtmpdUrl"), MapToApiVersion("1.0")]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<IngestMatrixPlugin.Dto.OldResponse.v1.MatrixOldResponseMessage> SwitchRtmpUrl([FromQuery]int outPort, [FromQuery]string url)
        {
            IngestMatrixPlugin.Dto.OldResponse.v1.MatrixOldResponseMessage response 
                = new IngestMatrixPlugin.Dto.OldResponse.v1.MatrixOldResponseMessage();

            try
            {
                if (outPort <= 0 )
                {
                    throw new Exception("Switch failed！ param is invailed");
                }

                response.nCode = await _matrixManage.SwitchRtmpUrl(outPort, url) ? 1 : 0;
            }
            catch (Exception e)
            {
                if (e is SobeyRecException se)//sobeyexcep会自动打印错误
                {
                    response.message = se.Message;
                }
                else
                {
                    response.message = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}:error info:{e.Message}";
                    Logger.Error(response.message);
                }
                response.nCode = 0;
            }
            return response;
        }

        /// <summary>
        /// 矩阵开关
        /// </summary>
        /// <param name="lInPort"></param>
        /// <param name="lOutPort"></param>
        /// <param name="lTimeOut"></param>
        /// <returns></returns>
        [HttpGet("SwitchInOut"), MapToApiVersion("1.0")]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<IngestMatrixPlugin.Dto.OldResponse.v1.MatrixOldResponseMessage> SwitchInOut([FromQuery]int lInPort,
                                                                                       [FromQuery]int lOutPort,
                                                                                       [FromQuery]int lTimeOut)
        {
            IngestMatrixPlugin.Dto.OldResponse.v1.MatrixOldResponseMessage response = new IngestMatrixPlugin.Dto.OldResponse.v1.MatrixOldResponseMessage();
            try
            {
                if (lInPort <= 0 || lOutPort <= 0)
                {
                    throw new Exception("Switch failed！ param is invailed");
                }
                
                response.nCode = await _matrixManage.SwitchInOutAsync(lInPort, lOutPort, null, null) ? 1 : 0;
            }
            catch (Exception e)
            {
                if (e is SobeyRecException se)//sobeyexcep会自动打印错误
                {
                    response.message = se.Message;
                }
                else
                {
                    response.message = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}:error info:{e.Message}";
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
        public async Task<Dto.OldResponse.v1.MatrixOldResponseMessage<long>> GetInPortFromOutPort([FromQuery]long OutPort)
        {
            Dto.OldResponse.v1.MatrixOldResponseMessage<long> response = new Dto.OldResponse.v1.MatrixOldResponseMessage<long>();
            try
            {
                response.extention = await _matrixManage.GetInPortFromOutPortAsync(OutPort);
                if(response.extention == -1)
                {
                    response.message = "Inport error";
                    response.nCode = 0;
                }
            } catch(Exception e)
            {
                if(e is SobeyRecException se)//sobeyexcep会自动打印错误
                {
                    response.message = se.Message;
                } else
                {
                    response.message = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}:error info:{e.Message}";
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
        public async Task<Dto.OldResponse.v1.MatrixOldResponseMessage<string>> QueryLinkState()
        {
            Dto.OldResponse.v1.MatrixOldResponseMessage<string> res = new Dto.OldResponse.v1.MatrixOldResponseMessage<string>();
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
                    res.message = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}:error info:{e.Message}";
                    Logger.Error(res.message);
                }
                res.nCode = 0;
            }
            return res;
        }
    }
}
