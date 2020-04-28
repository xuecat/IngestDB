using IngestDBCore;
using IngestDBCore.Basic;
using IngestGlobalPlugin.Dto;
using IngestGlobalPlugin.Managers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Sobey.Core.Log;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using FileFormateInfoRequest = IngestGlobalPlugin.Dto.FileFormateInfoResponse;
using MqMsgInfoResponse = IngestGlobalPlugin.Dto.MqMsgInfoRequest;

namespace IngestGlobalPlugin.Controllers.v2
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("2.0")]
    [ApiController]
    public class MaterialController : ControllerBase
    {
        private readonly ILogger Logger = LoggerManager.GetLogger("MaterialInfo");
        private readonly MaterialManager _materialManage;
        //private readonly IMapper _mapper;

        public MaterialController(MaterialManager task/*, IMapper mapper*/)
        {
            _materialManage = task;
            //_restClient = rsc;
            //_mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }



        /// <summary>
        /// 获取material信息通过taskid
        /// </summary>
        /// <remarks>
        /// 获取material信息通过taskid
        ///     
        /// </remarks>
        /// <param name="taskid"> taskid </param>
        /// <returns>消息组</returns>     
        [HttpPost("material/{taskid}")]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<List<MaterialInfoResponse>>> GetMaterailInfo([FromRoute]int taskid)
        {
            var Response = new ResponseMessage<List<MaterialInfoResponse>>();

            try
            {
                Response.Ext = await _materialManage.GetMaterialInfo(taskid);
            }
            catch (Exception e)
            {
                if (e.GetType() == typeof(SobeyRecException))//sobeyexcep会自动打印错误
                {
                    SobeyRecException se = e as SobeyRecException;
                    Response.Code = se.ErrorCode.ToString();
                    Response.Msg = se.Message;
                }
                else
                {
                    Response.Code = ResponseCodeDefines.ServiceError;
                    Response.Msg = "GetMaterailInfo error info：" + e.ToString();
                    Logger.Error("GetMaterailInfo error info：" + e.ToString());
                }
            }
            return Response;
        }

        /// <summary>
        /// 获取需要重新处理的消息
        /// </summary>
        /// <remarks>
        /// 获取需要重新处理的消息
        ///     
        /// </remarks>
        /// <returns>消息组</returns>     
        [HttpGet("mqmsg")]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<List<MqMsgInfoResponse>>> GetNeedProcessedMqMsg()
        {
            var Response = new ResponseMessage<List<MqMsgInfoResponse>>();
            
            try
            {
                Response.Ext = await _materialManage.GetNeedProcessMsg<MqMsgInfoResponse>();
            }
            catch (Exception e)
            {
                if (e.GetType() == typeof(SobeyRecException))//sobeyexcep会自动打印错误
                {
                    SobeyRecException se = e as SobeyRecException;
                    Response.Code = se.ErrorCode.ToString();
                    Response.Msg = se.Message;
                }
                else
                {
                    Response.Code = ResponseCodeDefines.ServiceError;
                    Response.Msg = "GetNeedProcessedMqMsg error info：" + e.ToString();
                    Logger.Error(Response.Msg);
                }
            }
            return Response;
        }

        /// <summary>
        /// 通过id查询处理失败的消息信息
        /// </summary>
        /// <remarks>
        /// 通过id查询处理失败的消息信息
        ///     
        /// </remarks>
        /// <param name="taskid"> 待查询的taskid数组 </param>
        /// <returns>id</returns>     
        [HttpGet("failedrecord")]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<List<MsgFailedRecord>>> GetMsgFailedRecord([FromBody, BindRequired] List<int> taskid)
        {
            var Response = new ResponseMessage<List<MsgFailedRecord>>();
            if (taskid == null)
            {
                Response.Code = ResponseCodeDefines.ModelStateInvalid;
                Response.Msg = "请求参数不正确";
            }
            try
            {
                Response.Ext = await _materialManage.GetMsgFailedRecordList(taskid);
            }
            catch (Exception e)
            {
                if (e.GetType() == typeof(SobeyRecException))//sobeyexcep会自动打印错误
                {
                    SobeyRecException se = e as SobeyRecException;
                    Response.Code = se.ErrorCode.ToString();
                    Response.Msg = se.Message;
                }
                else
                {
                    Response.Code = ResponseCodeDefines.ServiceError;
                    Response.Msg = "GetMsgFailedRecord error info：" + e.ToString();
                    Logger.Error(Response.Msg);
                }
            }
            return Response;
        }

        /// <summary>
        /// 添加失败入库得消息
        /// </summary>
        /// <remarks>
        /// 查询当前任务是否有处理失败的消息
        ///     
        /// </remarks>
        /// <param name="msgInfo">添加消息体</param>
        /// <returns>id</returns>     
        [HttpDelete("failedrecord/delete/{taskid}")]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<int>> DeleteMsgFaieldRecord([FromBody , BindRequired] MsgFailedRecord msgInfo)
        {
            var Response = new ResponseMessage<int>();
            if (msgInfo == null)
            {
                Response.Code = ResponseCodeDefines.ModelStateInvalid;
                Response.Msg = "请求参数不正确";
            }
            try
            {
                Response.Ext = await _materialManage.AddMsgFailedRecord(msgInfo);
            }
            catch (Exception e)
            {
                if (e.GetType() == typeof(SobeyRecException))//sobeyexcep会自动打印错误
                {
                    SobeyRecException se = e as SobeyRecException;
                    Response.Code = se.ErrorCode.ToString();
                    Response.Msg = se.Message;
                }
                else
                {
                    Response.Code = ResponseCodeDefines.ServiceError;
                    Response.Msg = "DeleteMsgFaieldRecord error info：" + e.ToString();
                    Logger.Error(Response.Msg);
                }
            }
            return Response;
        }

        /// <summary>
        /// 获取当前任务处理消息失败次数  查询当前任务是否有处理失败的消息
        /// </summary>
        /// <remarks>
        /// 查询当前任务是否有处理失败的消息
        ///     
        /// </remarks>
        /// <param name="taskid">要统计的任务id</param>
        /// <param name="sectionid">分段id</param>
        /// <returns>次数</returns>     
        [HttpDelete("failedrecord/delete/{taskid}")]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<int>> DeleteMsgFaieldRecord([FromRoute, BindRequired]int taskid, [FromQuery, BindRequired]int sectionid)
        {
            var Response = new ResponseMessage<int>();
            if (taskid < 1)
            {
                Response.Code = ResponseCodeDefines.ModelStateInvalid;
                Response.Msg = "请求参数不正确";
            }
            try
            {
                Response.Ext = await _materialManage.DeleteMsgFailedRecord(taskid, sectionid);
            }
            catch (Exception e)
            {
                if (e.GetType() == typeof(SobeyRecException))//sobeyexcep会自动打印错误
                {
                    SobeyRecException se = e as SobeyRecException;
                    Response.Code = se.ErrorCode.ToString();
                    Response.Msg = se.Message;
                }
                else
                {
                    Response.Code = ResponseCodeDefines.ServiceError;
                    Response.Msg = "DeleteMsgFaieldRecord error info：" + e.ToString();
                    Logger.Error(Response.Msg);
                }
            }
            return Response;
        }

        /// <summary>
        /// 获取当前任务处理消息失败次数  查询当前任务是否有处理失败的消息
        /// </summary>
        /// <remarks>
        /// 查询当前任务是否有处理失败的消息
        ///     
        /// </remarks>
        /// <param name="taskid">要统计的任务id</param>
        /// <returns>次数</returns>     
        [HttpGet("failedrecord/failedcount/{taskid}")]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<int>> GetTaskFailedCount([FromRoute, BindRequired]int taskid)
        {
            var Response = new ResponseMessage<int>();
            if (taskid < 1)
            {
                Response.Code = ResponseCodeDefines.ModelStateInvalid;
                Response.Msg = "请求参数不正确";
            }
            try
            {
                Response.Ext = await _materialManage.CountFailedRecordTask(taskid);
            }
            catch (Exception e)
            {
                if (e.GetType() == typeof(SobeyRecException))//sobeyexcep会自动打印错误
                {
                    SobeyRecException se = e as SobeyRecException;
                    Response.Code = se.ErrorCode.ToString();
                    Response.Msg = se.Message;
                }
                else
                {
                    Response.Code = ResponseCodeDefines.ServiceError;
                    Response.Msg = "GetTaskFailedCount error info：" + e.ToString();
                    Logger.Error(Response.Msg);
                }
            }
            return Response;
        }

        /// <summary>
        /// 获取入库消息通过taskid
        /// </summary>
        /// <remarks>
        /// 例子:
        ///     
        /// </remarks>
        /// <param name="taskid">文件格式关键字</param>
        /// <returns>文件格式元数据</returns>     
        [HttpGet("mqmsgcontent/{taskid}")]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<List<FailedMessageParam>>> GetMsgContentByTaskID([FromRoute, BindRequired]int taskid)
        {
            var Response = new ResponseMessage<List<FailedMessageParam>>();
            if (taskid <1)
            {
                Response.Code = ResponseCodeDefines.ModelStateInvalid;
                Response.Msg = "请求参数不正确";
            }
            try
            {
                Response.Ext = await _materialManage.GetMsgContentByTaskid(taskid);
            }
            catch (Exception e)
            {
                if (e.GetType() == typeof(SobeyRecException))//sobeyexcep会自动打印错误
                {
                    SobeyRecException se = e as SobeyRecException;
                    Response.Code = se.ErrorCode.ToString();
                    Response.Msg = se.Message;
                }
                else
                {
                    Response.Code = ResponseCodeDefines.ServiceError;
                    Response.Msg = "GetMsgContentByTaskID error info：" + e.ToString();
                    Logger.Error(Response.Msg);
                }
            }
            return Response;
        }

        /// <summary>
        /// 获取入库的文件格式
        /// </summary>
        /// <remarks>
        /// 例子:
        ///     
        /// </remarks>
        /// <param name="key">文件格式关键字</param>
        /// <returns>文件格式元数据</returns>     
        [HttpGet("formateinfo")]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<FileFormateInfoResponse>> FindFormatInfo([FromQuery, BindRequired]string key)
        {
            var Response = new ResponseMessage<FileFormateInfoResponse>();
            if (string.IsNullOrEmpty(key))
            {
                Response.Code = ResponseCodeDefines.ModelStateInvalid;
                Response.Msg = "请求参数不正确";
            }
            try
            {
                Response.Ext = await _materialManage.FindFormateInfo<FileFormateInfoResponse>(key);
            }
            catch (Exception e)
            {
                if (e.GetType() == typeof(SobeyRecException))//sobeyexcep会自动打印错误
                {
                    SobeyRecException se = e as SobeyRecException;
                    Response.Code = se.ErrorCode.ToString();
                    Response.Msg = se.Message;
                }
                else
                {
                    Response.Code = ResponseCodeDefines.ServiceError;
                    Response.Msg = "FindFormatInfo error info：" + e.ToString();
                    Logger.Error(Response.Msg);
                }
            }
            return Response;
        }

        /// <summary>
        /// 修改或者更新入库的文件格式
        /// </summary>
        /// <remarks>
        /// 
        ///     没有就添加，有就修改
        /// </remarks>
        /// <param name="formateinfo">传入的格式信息</param>
        /// <returns>文件格式元数据</returns>     
        [HttpPost("formateinfo")]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<FileFormateInfoResponse>> UpdateFormateInfo([FromBody, BindRequired]FileFormateInfoResponse formateinfo)
        {
            var Response = new ResponseMessage<FileFormateInfoResponse>();
            if (formateinfo == null)
            {
                Response.Code = ResponseCodeDefines.ModelStateInvalid;
                Response.Msg = "请求参数不正确";
            }
            try
            {
                Response.Ext = await _materialManage.UpdateFormateInfo<FileFormateInfoResponse, FileFormateInfoResponse>(formateinfo);
            }
            catch (Exception e)
            {
                if (e.GetType() == typeof(SobeyRecException))//sobeyexcep会自动打印错误
                {
                    SobeyRecException se = e as SobeyRecException;
                    Response.Code = se.ErrorCode.ToString();
                    Response.Msg = se.Message;
                }
                else
                {
                    Response.Code = ResponseCodeDefines.ServiceError;
                    Response.Msg = "UpdateFormateInfo error info：" + e.ToString();
                    Logger.Error(Response.Msg);
                }
            }
            return Response;
        }
        
    }
}
