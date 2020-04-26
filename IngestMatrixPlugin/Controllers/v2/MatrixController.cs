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
        /// <summary>锁</summary>
        public static object lockObj = new object();
        /// <summary>互斥锁</summary>
        public static Mutex m_MatrixMt = new Mutex();
        private readonly MatrixManager _matrixManage;
        private readonly ILogger Logger = LoggerManager.GetLogger("DeviceInfo");
        //private readonly RestClient _restClient;


        public MatrixController(MatrixManager task) { _matrixManage = task; }

        /// <summary>
        /// 矩阵开关
        /// </summary>
        /// <param name="inPort">输入端口</param>
        /// <param name="outPort">输出端口</param>
        /// <returns>是否切换成功</returns>
        [HttpGet("matrix/switch"), MapToApiVersion("2.0")]
        [ApiExplorerSettings(GroupName = "v2")]
        public ResponseMessage<bool> SwitchInOut([FromQuery, BindRequired]int inPort,
                                                 [FromQuery, BindRequired]int outPort)
        {
            ResponseMessage<bool> response = new ResponseMessage<bool>();
            try
            {
                if (inPort <= 0 || outPort <= 0)
                {
                    throw new Exception("Switch failed！ param is invailed");
                }
                m_MatrixMt.WaitOne();
                lock (lockObj)  //保证只有一个切换在进行
                {
                    response.Ext = _matrixManage.SwitchInOutAsync(inPort, outPort).Result;
                }
                m_MatrixMt.ReleaseMutex();
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
                    response.Msg = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}：error info:{e}";
                    response.Code = ResponseCodeDefines.ServiceError;
                    Logger.Error(response.Msg);
                }
            }
            return response;
        }

        /// <summary>
        /// 通过出点获取入点
        /// </summary>
        /// <param name="outPort">输出端口</param>
        /// <returns>输入端口</returns>
        [HttpGet("matrix/inport/{outPort}"), MapToApiVersion("2.0")]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<long>> GetInPortFromOutPort([FromRoute, BindRequired, DefaultValue(1)]long outPort)
        {
            ResponseMessage<long> response = new ResponseMessage<long>();
            try
            {
                response.Ext = await _matrixManage.GetInPortFromOutPortAsync(outPort);
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
                    response.Msg = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}：error info:{e}";
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
        [HttpGet("matrix/linkstate"), MapToApiVersion("2.0")]
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
                    response.Msg = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}：error info:{e}";
                    response.Code = ResponseCodeDefines.ServiceError;
                    Logger.Error(response.Msg);
                }
            }
            return response;
        }
    }
}
