using IngestDBCore;
using IngestDBCore.Basic;
using IngestGlobalPlugin.Dto;
using IngestGlobalPlugin.Managers;
using Microsoft.AspNetCore.Mvc;
using Sobey.Core.Log;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IngestGlobalPlugin.Controllers.v1
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
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



        [HttpPost("AddMqMsg"), MapToApiVersion("1.0")]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<bool> AddMqMsg([FromBody] MQmsgInfo msg)
        {
            try
            {
                return await _materialManage.AddMqMsg<MQmsgInfo>(msg);
            }
            catch (Exception ex)
            {
                //pOut.errStr = ex.Message;
                Logger.Error("AddMqMsg 异常发生: " + ex.ToString());
                //pOut.bRet = false;
            }

            return false;
        }

        /// <summary>
        /// 更新任务所有分段的入库状态
        /// </summary>
        /// <remarks>
        /// 更新任务所有分段的入库状态
        ///     
        /// </remarks>
        /// <returns>消息组</returns>     
        [HttpGet("PostUpdateSaveInDBStateForTask"), MapToApiVersion("1.0")]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<UpdateSaveInDBStateForTask_OUT> PostUpdateSaveInDBStateForTask([FromBody]UpdateSaveInDBStateForTask_IN pIn)
        {
            var Response = new UpdateSaveInDBStateForTask_OUT();
            Response.errStr = "OK";

            try
            {
                await _materialManage.UpdateSaveInDBStateForTask(pIn.nTaskID, pIn.nPolicyID, pIn.nSectionID, (SAVE_IN_DB_STATE)pIn.state, pIn.strResult);

                Response.bRet = true;
            }
            catch (Exception e)
            {
                if (e.GetType() == typeof(SobeyRecException))//sobeyexcep会自动打印错误
                {
                    SobeyRecException se = e as SobeyRecException;
                    //Response.Code = se.ErrorCode.ToString();
                    //Response.Msg = se.Message;
                    Response.bRet = false;
                    Response.errStr = e.Message;
                }
                else
                {
                    Response.bRet = false;
                    //Response.Code = ResponseCodeDefines.ServiceError;
                    //Response.Msg = "GetNeedProcessedMqMsg error info：" + e.ToString();
                    Logger.Error("GetNeedProcessedMqMsg error info：" + e.Message);
                    Response.errStr = e.Message;
                }
            }
            return Response;
        }

        /// <summary>
        /// 获取需要重新处理的消息
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetNeedProcessedMqMsg"), MapToApiVersion("1.0")]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<OldResponseMessage<List<MQmsgInfo>>> GetNeedProcessedMqMsg()
        {
            var Response = new OldResponseMessage<List<MQmsgInfo>>();
            Response.message = "OK";
            try
            {
                Response.extention = await _materialManage.GetNeedProcessMsg<MQmsgInfo>();

                Response.nCode = 1;

                return Response;
            }
            catch (Exception e)
            {
                if (e.GetType() == typeof(SobeyRecException))//sobeyexcep会自动打印错误
                {
                    SobeyRecException se = e as SobeyRecException;
                    Response.nCode = 0;// int.Parse(se.ErrorCode.ToString());
                    Response.message = e.Message;
                }
                else
                {
                    Response.nCode = 0;
                    Response.message = "error info：" + e.Message;
                    Logger.Error("GetNeedProcessedMqMsg" + e.Message);
                }
            }
            return Response;
        }

        /// <summary>
        /// 通过id查询处理失败的消息信息
        /// </summary>
        /// <param name="taskids"> 待查询的taskid数组 </param>
        /// <returns> extention 为结构体数组 </returns>
        [HttpPost("QueryMsgFailedRecord"), MapToApiVersion("1.0")]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<OldResponseMessage<List<MsgFailedRecord>>> QueryMsgFailedRecord([FromBody]List<int> taskids)
        {
            var Response = new OldResponseMessage<List<MsgFailedRecord>>();
            try
            {
                Response.extention = await _materialManage.GetMsgFailedRecordList(taskids);
                Response.nCode = 1;
                return Response;
            }
            catch (Exception e)
            {
                if (e.GetType() == typeof(SobeyRecException))//sobeyexcep会自动打印错误
                {
                    SobeyRecException se = e as SobeyRecException;
                    Response.nCode = 0;// int.Parse(se.ErrorCode.ToString());
                    Response.message = e.Message;
                }
                else
                {
                    Response.nCode = 0;
                    Response.message = "error info：" + e.Message;
                    Logger.Error("QueryMsgFailedRecord" + e.Message);
                }
            }
            return Response;
        }

        [HttpPost("AddMsgFailedRecord"), MapToApiVersion("1.0")]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<OldResponseMessage> AddMsgFailedRecord([FromBody] MsgFailedRecord msgInfo)
        {
            var Response = new OldResponseMessage();
            try
            {
                await _materialManage.AddMsgFailedRecord(msgInfo);

                return Response;
            }
            catch (Exception e)
            {
                if (e.GetType() == typeof(SobeyRecException))//sobeyexcep会自动打印错误
                {
                    SobeyRecException se = e as SobeyRecException;
                    Response.nCode = 0;// int.Parse(se.ErrorCode.ToString());
                    Response.message = e.Message;
                }
                else
                {
                    Response.nCode = 0;
                    Response.message = "error info：" + e.Message;
                    Logger.Error("QueryTaskResultByIDAndSection" + e.Message);
                }
            }
            return Response;
        }

        /// <summary>
        /// 查询任务除指定分段外，是否还有其它分段入库失败
        /// </summary>
        /// <param name="taskID"></param>
        /// <param name="sectionID"></param>
        /// <returns></returns>
        [HttpGet("DeleteMsgFaieldRecord"), MapToApiVersion("1.0")]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<OldResponseMessage<bool>> DeleteMsgFaieldRecord([FromQuery]int taskID, [FromQuery]int sectionID)
        {
            var Response = new OldResponseMessage<bool>();
            try
            {
                await _materialManage.DeleteMsgFailedRecord(taskID, sectionID);

                Response.extention = true;
                Response.nCode = 1;

                return Response;
            }
            catch (Exception e)
            {
                if (e.GetType() == typeof(SobeyRecException))//sobeyexcep会自动打印错误
                {
                    SobeyRecException se = e as SobeyRecException;
                    Response.nCode = 0;// int.Parse(se.ErrorCode.ToString());
                    Response.message = e.Message;
                }
                else
                {
                    Response.nCode = 0;
                    Response.message = "error info：" + e.Message;
                    Logger.Error("DeleteMsgFaieldRecord" + e.Message);
                }
            }
            return Response;
        }

        /// <summary>
        /// 查询任务除指定分段外，是否还有其它分段入库失败
        /// </summary>
        /// <param name="taskID"></param>
        /// <param name="sectionID"></param>
        /// <returns></returns>
        [HttpGet("QueryTaskResultByIDAndSection"), MapToApiVersion("1.0")]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<OldResponseMessage<bool>> QueryTaskResultByIDAndSection([FromQuery]int taskID, [FromQuery]int sectionID)
        {
            var Response = new OldResponseMessage<bool>();
            try
            {
                var f = await _materialManage.CountFailedRecordTaskAndSection(taskID, sectionID);
                if (f == 0)
                {
                    Response.extention = false;
                }
                else
                {
                    Response.extention = true;
                    Response.nCode = f;
                }

                return Response;
            }
            catch (Exception e)
            {
                if (e.GetType() == typeof(SobeyRecException))//sobeyexcep会自动打印错误
                {
                    SobeyRecException se = e as SobeyRecException;
                    Response.nCode = 0;// int.Parse(se.ErrorCode.ToString());
                    Response.message = e.Message;
                    Response.extention = false;
                }
                else
                {
                    Response.nCode = 0;
                    Response.message = "error info：" + e.Message;
                    Logger.Error("QueryTaskResultByIDAndSection" + e.Message);
                    Response.extention = false;
                }
            }
            return Response;
        }

        [HttpGet("QueryTaskResultByID"), MapToApiVersion("1.0")]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<OldResponseMessage<bool>> QueryTaskResultByID([FromQuery]int taskID)
        {
            var Response = new OldResponseMessage<bool>();
            try
            {
                var f = await _materialManage.CountFailedRecordTask(taskID);
                if (f == 0)
                {
                    Response.extention = false;
                }
                else
                {
                    Response.extention = true;
                    Response.nCode = f;
                }

                return Response;
            }
            catch (Exception e)
            {

                if (e.GetType() == typeof(SobeyRecException))//sobeyexcep会自动打印错误
                {
                    SobeyRecException se = e as SobeyRecException;
                    Response.nCode = 0;// int.Parse(se.ErrorCode.ToString());
                    Response.message = e.Message;
                    Response.extention = false;
                }
                else
                {
                    Response.nCode = 0;
                    Response.message = "error info：" + e.Message;
                    Logger.Error("QueryTaskResultByID" + e.Message);
                    Response.extention = false;
                }
            }
            return Response;
        }

        [HttpGet("GetMsgContentByTaskID"), MapToApiVersion("1.0")]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<OldResponseMessage<List<FailedMessageParam>>> GetMsgContentByTaskID([FromQuery]int taskID)
        {
            var Response = new OldResponseMessage<List<FailedMessageParam>>();
            try
            {
                var f = await _materialManage.GetMsgContentByTaskid(taskID);
                Response.extention = f;
                return Response;
            }
            catch (Exception e)
            {

                if (e.GetType() == typeof(SobeyRecException))//sobeyexcep会自动打印错误
                {
                    SobeyRecException se = e as SobeyRecException;
                    //Response.nCode = int.Parse(se.ErrorCode.ToString());
                    //Response.errStr = e.Message;
                    Response.nCode = 0;
                    Response.message = e.Message;
                    Response.extention = null;
                }
                else
                {
                    //Response.nCode = 0;
                    //Response.errStr = "error info：" + e.ToString();
                    Logger.Error("GetMsgContentByTaskID" + e.ToString());
                    Response.nCode = 0;
                    Response.message = e.Message;
                    Response.extention = null;
                }
            }
            return Response;
        }

        [HttpGet("FindFormatInfo"), MapToApiVersion("1.0")]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<FileFormatInfo_out> FindFormatInfo([FromQuery]string strKey)
        {
            var Response = new FileFormatInfo_out() { nCode = 0, errStr = "no"};
            try
            {
                var f = await _materialManage.FindFormateInfo<FileFormatInfo_out>(strKey);
                f.errStr = "OK";
                f.nCode = 1;
                return f;
            }
            catch (Exception e)
            {
                
                if (e.GetType() == typeof(SobeyRecException))//sobeyexcep会自动打印错误
                {
                    SobeyRecException se = e as SobeyRecException;
                    Response.nCode = 0;// int.Parse(se.ErrorCode.ToString());
                    Response.errStr = e.Message;
                }
                else
                {
                    Response.nCode = 0;
                    Response.errStr = "error info：" + e.Message;
                    Logger.Error("FindFormatInfo" + e.Message);
                }
            }
            return Response;
        }


        [HttpPost("AddFormatInfo"), MapToApiVersion("1.0")]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<FileFormatInfo_out> AddFormatInfo([FromBody]FileFormatInfo_in pIn)
        {
            var Response = new FileFormatInfo_out() { nCode = 0, errStr = "no" };
            try
            {
                var f = await _materialManage.UpdateFormateInfo<FileFormatInfo_out, FileFormatInfo_in>(pIn);
                f.errStr = "OK";
                f.nCode = 1;
                return f;
            }
            catch (Exception e)
            {

                if (e.GetType() == typeof(SobeyRecException))//sobeyexcep会自动打印错误
                {
                    SobeyRecException se = e as SobeyRecException;
                    Response.nCode = 0;// int.Parse(se.ErrorCode.ToString());
                    Response.errStr = e.Message;
                }
                else
                {
                    Response.nCode = 0;
                    Response.errStr = "error info：" + e.Message;
                    Logger.Error("AddFormatInfo" + e.Message);
                }
            }
            return Response;
        }

        [HttpPost("UpdateFormatInfo"), MapToApiVersion("1.0")]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<FileFormatInfo_out> UpdateFormatInfo(FileFormatInfo_in pIn)
        {
            var Response = new FileFormatInfo_out() { nCode = 0, errStr = "no" };
            try
            {
                var f = await _materialManage.UpdateFormateInfo<FileFormatInfo_out, FileFormatInfo_in>(pIn);
                f.errStr = "OK";
                f.nCode = 1;
                return f;
            }
            catch (Exception e)
            {

                if (e.GetType() == typeof(SobeyRecException))//sobeyexcep会自动打印错误
                {
                    SobeyRecException se = e as SobeyRecException;
                    Response.nCode = 0;// int.Parse(se.ErrorCode.ToString());
                    Response.errStr = e.Message;
                }
                else
                {
                    Response.nCode = 0;
                    Response.errStr = "error info：" + e.Message;
                    Logger.Error("UpdateFormatInfo" + e.Message);
                }
            }
            return Response;
        }
        //[HttpPost("AddMaterialAndChangeState"), MapToApiVersion("1.0")]
        //[ApiExplorerSettings(GroupName = "v1")]
        //public async Task<bool> PostAddMaterialAndChangeState([FromBody]MaterialInfo Info)
        //{
        //    try
        //    {
        //        return await _materialManage.AddMaterialAndChangeState(Info);
        //    }
        //    catch (Exception ex)//其他未知的异常，写异常日志
        //    {
        //        Logger.Error("PostAddMaterialAndChangeState 异常发生: " + ex.ToString());
        //    }
        //    return false;
        //}

        [HttpPost("UpdateMqMsgStatus"), MapToApiVersion("1.0")]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<bool> PostUpdateMqMsgStatus([FromQuery] string msgId, [FromQuery] int nActionID, [FromQuery] int nFailedCount, [FromBody] MqmsgStatus msgStatus)
        {
            try
            {
                return await _materialManage.UpdateMqMsgStatus(msgId, nActionID, msgStatus, nFailedCount);
            }
            catch (System.Exception ex)
            {
                Logger.Error("PostUpdateMqMsgStatus 异常发生: " + ex.ToString());
                return false;
            }
        }

        /// <summary>
        /// 新的添加素材接口
        /// </summary>
        [HttpPost("AddMaterialNew"), MapToApiVersion("1.0")]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<AddMaterialNew_OUT> PostAddMaterialNew([FromBody]MaterialInfo Info)
        {
            AddMaterialNew_OUT p = new AddMaterialNew_OUT();
            try
            {
                p.nMaterialID = await _materialManage.AddMaterialInfo(Info);
                p.errStr = "OK";
                p.bRet = true;
            }
            catch (Exception ex)//其他未知的异常，写异常日志
            {
                Logger.Error("PostAddMaterialNew 异常发生: " + ex.ToString());
                p.nMaterialID = -1;
                p.errStr = ex.Message;
                p.bRet = false;
            }
            return p;
        }

        /// <summary>
        /// 按照时间删除MQ消息
        /// </summary>
        [HttpGet("GetDeleteMqMsg"), MapToApiVersion("1.0")]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<DeleteMqMsg_OUT> GetDeleteMqMsg(string tmDelete)
        {
            DeleteMqMsg_OUT p = new DeleteMqMsg_OUT();
            try
            {
                DateTime dt = DateTime.Parse(tmDelete);
                p.bRet = await _materialManage.DeleteMqMsgStatus(dt);
                p.errStr = "OK";
            }
            catch (Exception ex)
            {
                Logger.Error("GetDeleteMqMsg 异常发生: " + ex.ToString());
                p.errStr = ex.Message;
                p.bRet = false;
            }
            return p;
        }
    }
}
