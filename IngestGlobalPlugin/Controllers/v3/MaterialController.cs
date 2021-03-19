using IngestDBCore;
using IngestDBCore.Notify;
using IngestGlobalPlugin.Dto.OldResponse;
using IngestGlobalPlugin.Managers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using Sobey.Core.Log;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IngestGlobalPlugin.Controllers.v3
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("3.0")]
    [ApiController]
    public class MaterialController : ControllerBase
    {
        private readonly ILogger Logger = LoggerManager.GetLogger("MaterialInfoV3");
        
        private readonly MaterialManager _materialManage;
        private readonly NotifyClock _clock;
        //private readonly IMapper _mapper;



        public MaterialController(MaterialManager task, IServiceProvider services/*, IMapper mapper*/)
        {
            _materialManage = task;
            //_restClient = rsc;
            //_mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _clock = services.GetRequiredService<NotifyClock>().Register(services);
        }


        [HttpGet("sendkfk/{taskid}")]
        [ApiExplorerSettings(GroupName = "v3.0")]
        public async Task<ResponseMessage<bool>> SendKafkaInfo([FromRoute, BindRequired] int taskid, [FromQuery, BindRequired] int mode, [FromQuery, BindRequired] int kafkacmd)
        {
            Logger.Info($"SendKafkaInfo  taskid : {taskid}, mode : {mode}, kafkacmd : {kafkacmd}");

            var Response = new ResponseMessage<bool>();

            try
            {
                KafkaMode kafkamode = (KafkaMode)mode;
                List<string> kafkainfos = null;

                switch (kafkamode)
                {
                    case KafkaMode.CreateDefaultKafka:
                        kafkainfos = _materialManage.GetKafkaInfoByCreateDefault(taskid, kafkacmd);
                        break;
                    case KafkaMode.GetKafkaFromDb:
                        kafkainfos = await _materialManage.GetKafkaInfoFromDb(taskid, kafkacmd);
                        break;
                }

                if(kafkainfos != null && kafkainfos.Count > 0)
                {
                    Task.Run(() => 
                    {
                        foreach (var item in kafkainfos)
                        {
                            _clock.InvokeNotify(null, NotifyPlugin.Kafka, NotifyAction.SENDMSVNOTIFY, item);//先用NotifyAction.SENDMSVNOTIFY表示传到MSV_NOTIFY主题
                        }
                    });
                }

                Response.Ext = true;

            }
            catch (Exception e)
            {
                if (e is SobeyRecException se)//sobeyexcep会自动打印错误
                {
                    Response.Code = se.ErrorCode.ToString();
                    Response.Msg = se.Message;
                }
                else
                {
                    Response.Code = ResponseCodeDefines.ServiceError;
                    Response.Msg = $"SendKafkaInfo error info:{e.Message}";
                    Logger.Error(Response.Msg);
                }
            }
            return Response;
        }
    }
}
