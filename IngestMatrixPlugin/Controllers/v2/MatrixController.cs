using IngestDBCore;
using IngestDBCore.Basic;
using IngestMatrixPlugin.Managers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Sobey.Core.Log;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace IngestMatrixPlugin.Controllers.v2
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("2.0")]
    [ApiController]
    public partial class MatrixController : ControllerBase
    {
        ///// <summary>锁</summary>
        //public static object lockObj = new object();
        ///// <summary>互斥锁</summary>
        //public static Mutex m_MatrixMt = new Mutex();
        private readonly MatrixManager _matrixManage;
        private readonly ILogger Logger = LoggerManager.GetLogger("Matrixinfo");
        //private readonly RestClient _restClient;


        public MatrixController(MatrixManager task) { _matrixManage = task; }

        /// <summary>
        /// 矩阵切换指定rtmp的url
        /// </summary>
        /// <param name="outPort">输入端口</param>
        /// <param name="url">指定rtmp地址</param>
        /// <returns>是否切换成功</returns>
        [HttpGet("switchrtmpurl"), MapToApiVersion("2.0")]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<bool>> SwitchRtmpUrl([FromQuery, BindRequired]int outPort, [FromQuery]string url)
        {
            ResponseMessage<bool> response = new ResponseMessage<bool>();
            try
            {
                if (outPort<=0)
                {
                    throw new Exception("Switch failed！ param is invailed");
                }

                response.Ext = await _matrixManage.SwitchRtmpUrl(outPort, url);

            }
            catch (Exception e)
            {
                if (e is SobeyRecException se)//sobeyexcep会自动打印错误
                {
                    response.Msg = se.Message;
                    response.Code = se.ErrorCode.ToString();
                }
                else
                {
                    response.Msg = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}:error info:{e.Message}";
                    response.Code = ResponseCodeDefines.ServiceError;
                    Logger.Error(response.Msg);
                }
            }
            return response;
        }

       
        /// <summary>
        /// 矩阵切换指定信号和通道
        /// </summary>
        /// <param name="signal">输入信号id</param>
        /// <param name="channel">输出通道id</param>
        /// <returns>是否切换成功</returns>
        [HttpGet("switchsignalchannel"), MapToApiVersion("2.0")]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<bool>> SwitchSignalChannel([FromQuery, BindRequired]int signal,
                                                 [FromQuery, BindRequired]int channel)
        {
            ResponseMessage<bool> response = new ResponseMessage<bool>();
            try
            {
                if (channel <= 0)
                {
                    throw new Exception("SwitchSignalChannel failed！ param is invailed");
                }

                response.Ext = await _matrixManage.SwitchSignalChannelAsync(signal, channel);

            }
            catch (Exception e)
            {
                if (e is SobeyRecException se)//sobeyexcep会自动打印错误
                {
                    response.Msg = se.Message;
                    response.Code = se.ErrorCode.ToString();
                }
                else
                {
                    response.Msg = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}:error info:{e.Message}";
                    response.Code = ResponseCodeDefines.ServiceError;
                    Logger.Error(response.Msg);
                }
            }
            return response;
        }

        /// <summary>
        /// 矩阵开关
        /// </summary>
        /// <param name="inport">输入端口</param>
        /// <param name="outport">输出端口</param>
        /// <returns>是否切换成功</returns>
        [HttpGet("switch"), MapToApiVersion("2.0")]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<bool>> SwitchInOut([FromQuery, BindRequired]int inport,
                                                 [FromQuery, BindRequired]int outport)
        {
            ResponseMessage<bool> response = new ResponseMessage<bool>();
            try
            {
                if (inport <= 0 || outport <= 0)
                {
                    throw new Exception("Switch failed！ param is invailed");
                }
               
                response.Ext = await _matrixManage.SwitchInOutAsync(inport, outport, null, null);

            }
            catch (Exception e)
            {
                if (e is SobeyRecException se)//sobeyexcep会自动打印错误
                {
                    response.Msg = se.Message;
                    response.Code = se.ErrorCode.ToString();
                }
                else
                {
                    response.Msg = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}:error info:{e.Message}";
                    response.Code = ResponseCodeDefines.ServiceError;
                    Logger.Error(response.Msg);
                }
            }
            return response;
        }

        /// <summary>
        /// 通过出点获取入点
        /// </summary>
        /// <param name="outport">输出端口</param>
        /// <returns>输入端口</returns>
        [HttpGet("inport/{outport}"), MapToApiVersion("2.0")]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<long>> GetInPortFromOutPort([FromRoute, BindRequired, DefaultValue(1)]long outport)
        {
            ResponseMessage<long> response = new ResponseMessage<long>();
            try
            {
                response.Ext = await _matrixManage.GetInPortFromOutPortAsync(outport);
                if (response.Ext == -1)
                {
                    response.Msg = "获取矩阵入口出错";
                    response.Code = ResponseCodeDefines.ServiceError;
                }
            }
            catch (Exception e)
            {
                if (e is SobeyRecException se)//sobeyexcep会自动打印错误
                {
                    response.Msg = se.Message;
                    response.Code = se.ErrorCode.ToString();
                }
                else
                {
                    response.Msg = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}:error info:{e.Message}";
                    response.Code = ResponseCodeDefines.ServiceError;
                    Logger.Error(response.Msg);
                }
            }
            return response;
        }

        /// <summary>
        /// 查询矩阵链接状态
        /// </summary>
        /// <returns></returns>
        [HttpGet("linkstate"), MapToApiVersion("2.0")]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<string>> QueryLinkState()
        {
            ResponseMessage<string> response = new ResponseMessage<string>();
            try
            {
                response.Ext = await _matrixManage.QueryLinkStateAsync();
                if (string.IsNullOrEmpty(response.Ext))
                {
                    response.Ext = "<?xml version=\"1.0\" encoding=\"UTF-8\" ?><MatrixLinkState/>";
                    response.Code = ResponseCodeDefines.ServiceError;
                }
            }
            catch (Exception e)
            {
                if (e is SobeyRecException se)//sobeyexcep会自动打印错误
                {
                    response.Msg = se.Message;
                    response.Code = se.ErrorCode.ToString();
                }
                else
                {
                    response.Msg = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}:error info:{e.Message}";
                    response.Code = ResponseCodeDefines.ServiceError;
                    Logger.Error(response.Msg);
                }
            }
            return response;
        }
    }
}
