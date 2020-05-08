using IngestTaskPlugin.Dto;
using IngestTaskPlugin.Managers;
using Microsoft.AspNetCore.Mvc;
using Sobey.Core.Log;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IngestTaskPlugin.Controllers.v1
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
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


        [HttpGet("GetPolicyByTaskID"), MapToApiVersion("1.0")]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<GetPolicyByTaskID_OUT> GetPolicyByTaskID([FromQuery]int nTaskID)
        {
            var Response = new GetPolicyByTaskID_OUT
            {
                bRet = false,
                errStr = "TaskID Is invaild"
            };
            if (nTaskID < 1)
            {
               
                return Response;
            }
            try
            {
                
                Response.arDataPolicyList = await _policyManage.GetPolicyByTaskIDAsync<MetaDataPolicy>(nTaskID);

                Response.bRet = true;
                Response.errStr = "OK";
                Response.nVaildDataCount = Response.arDataPolicyList.Count;

            }
            catch (Exception e)
            {
               
                Response.errStr = "GetPolicyByTaskID error info：" + e.Message;
                Logger.Error(Response.errStr);
                return Response;
            }
            return Response;
        }

        [HttpGet("GetAllMetaDataPolicy"), MapToApiVersion("1.0")]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<GetAllMetaDataPolicy_OUT> GetAllMetaDataPolicy()
        {
            var Response = new GetAllMetaDataPolicy_OUT
            {
                bRet = false,
                errStr = "TaskID Is invaild"
            };
           
            try
            {

                Response.arDataPolicyList = await _policyManage.GetAllMetaDataPolicyAsync<MetaDataPolicy>();

                Response.bRet = true;
                Response.errStr = "OK";
                Response.nVaildDataCount = Response.arDataPolicyList.Count;

            }
            catch (Exception e)
            {

                Response.errStr = "GetAllMetaDataPolicy error info：" + e.Message;
                Logger.Error(Response.errStr);
                return Response;
            }
            return Response;
        }

    }
}
