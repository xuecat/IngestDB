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

        public MatrixController(MatrixManager task) { _matrixManage = task; }

        /// <summary>
        /// 矩阵开关
        /// </summary>
        /// <param name="inport">输入端口</param>
        /// <param name="outport">输出端口</param>
        /// <returns>是否切换成功</returns>
        [HttpGet("switch")]
        [ApiExplorerSettings(GroupName = "v3")]
        public async Task<ResponseMessage<bool>> SwitchInOut([FromQuery, BindRequired] int inport,
                                                 [FromQuery, BindRequired] int outport)
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

    }
}
