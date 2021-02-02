using IngestDBCore;
using IngestMatrixPlugin.Managers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Sobey.Core.Log;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IngestMatrixPlugin.Controllers.v3
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("3.0")]
    [ApiController]
    public partial class MatrixController : ControllerBase
    {
        private readonly MatrixManager _matrixManage;
        private readonly ILogger Logger = LoggerManager.GetLogger("Matrixinfo3");
        //private readonly RestClient _restClient;


        public MatrixController(MatrixManager task) { _matrixManage = task; }


        /// <summary>
        /// 矩阵开关
        /// </summary>
        /// <param name="inport">输入端口</param>
        /// <param name="outport">输出端口</param>
        /// <returns>是否切换成功</returns>
        [HttpGet("switch"), MapToApiVersion("3.0")]
        [ApiExplorerSettings(GroupName = "v3")]
        public async Task<ResponseMessage<bool>> SwitchInOut([FromQuery, BindRequired] int inport,
                                                 [FromQuery, BindRequired] int outport)
        {
            Logger.Info($"SwitchInOut v3 inport : {inport}, outport : {outport}");

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
        /// 矩阵切换指定信号和通道
        /// </summary>
        /// <param name="signal">输入信号id</param>
        /// <param name="channel">输出通道id</param>
        /// <returns>是否切换成功</returns>
        [HttpGet("switch/signalchannel"), MapToApiVersion("3.0")]
        [ApiExplorerSettings(GroupName = "v3")]
        public async Task<ResponseMessage<bool>> SwitchSignalChannel([FromQuery, BindRequired] int signal,
                                                 [FromQuery, BindRequired] int channel)
        {
            Logger.Info($"SwitchSignalChannel v3 signal : {signal} , channel : {channel}");
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
        /// 矩阵切换指定通道的rtmp的url
        /// </summary>
        /// <param name="channelid">输入端口</param>
        /// <param name="url">指定rtmp地址</param>
        /// <returns>是否切换成功</returns>
        [HttpGet("switch/channelrtmpurl"), MapToApiVersion("3.0")]
        [ApiExplorerSettings(GroupName = "v3")]
        public async Task<ResponseMessage<bool>> SwitchChannelRtmpUrl([FromQuery, BindRequired] int channelid, [FromQuery] string url)
        {
            Logger.Info($"SwitchChannelRtmpUrl v3 channelid : {channelid} , url : {url}");
            ResponseMessage<bool> response = new ResponseMessage<bool>();
            try
            {
                if (channelid <= 0)
                {
                    throw new Exception("Switch failed！ param is invailed");
                }

                response.Ext = await _matrixManage.SwitchChannelRtmpAsync(channelid, url);

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
        /// 矩阵切换指定rtmp的url
        /// </summary>
        /// <param name="outPort">输入端口</param>
        /// <param name="url">指定rtmp地址</param>
        /// <returns>是否切换成功</returns>
        [HttpGet("switch/rtmpurl"), MapToApiVersion("3.0")]
        [ApiExplorerSettings(GroupName = "v3")]
        public async Task<ResponseMessage<bool>> SwitchRtmpUrl([FromQuery, BindRequired] int outPort, [FromQuery] string url)
        {
            Logger.Info($"SwitchRtmpUrl v3 outport : {outPort} , url : {url}");

            ResponseMessage<bool> response = new ResponseMessage<bool>();
            try
            {
                if (outPort <= 0)
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


    }
}
