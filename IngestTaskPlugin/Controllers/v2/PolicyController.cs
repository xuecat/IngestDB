using IngestDBCore;
using IngestTaskPlugin.Dto.Response;
using IngestTaskPlugin.Managers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Sobey.Core.Log;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IngestTaskPlugin.Controllers.v2
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("2.0")]
    [ApiController]
    public class PolicyController : ControllerBase
    {
        private readonly ILogger Logger = LoggerManager.GetLogger("PolicyInfo");
        private readonly PolicyManager _policyManage;
        //private readonly Lazy<IIngestGlobalInterface> _globalInterface;
        //private readonly IMapper _mapper;

        public PolicyController(PolicyManager task, IServiceProvider services/*, IMapper mapper*/)
        {
            _policyManage = task;
            //_globalInterface = new Lazy<IIngestGlobalInterface>(() => services.GetRequiredService<IIngestGlobalInterface>());
            //_mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// <summary>
        /// 获取任务相关策略
        /// </summary>
        /// <remarks>
        /// 例子:
        ///     
        /// </remarks>
        /// <param name="taskid">任务id</param>
        /// <returns>素材任务元数据结构体</returns>     
        [HttpGet("{taskid}")]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ResponseMessage<List<MetaDataPolicyResponse>>> GetTaskMaterialMetaData([FromRoute, BindRequired]int taskid)
        {
            var Response = new ResponseMessage<List<MetaDataPolicyResponse>>();
            if (taskid < 1)
            {
                Response.Code = ResponseCodeDefines.ModelStateInvalid;
                Response.Msg = "请求参数不正确";
            }
            try
            {
                Response.Ext = await _policyManage.GetPolicyByTaskIDAsync<MetaDataPolicyResponse>(taskid);
                if (Response.Ext == null || Response.Ext.Count <= 0)
                {
                    Response.Code = ResponseCodeDefines.NotFound;
                    Response.Msg = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}：error info: 获取数据为空!";
                }
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
                    Response.Msg = "GetTaskMaterialMetaData error info：" + e.Message;
                    Logger.Error(Response.Msg);
                }
            }
            return Response;
        }

    }
}
