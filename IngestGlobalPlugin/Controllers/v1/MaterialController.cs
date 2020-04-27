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
                Logger.Error("PostlockObject 异常发生: " + ex.ToString());
                //pOut.bRet = false;
            }

            return false;
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
                var mtr = await _materialManage.AddMaterialInfo(Info);
                p.nMaterialID = mtr.materialId;
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
