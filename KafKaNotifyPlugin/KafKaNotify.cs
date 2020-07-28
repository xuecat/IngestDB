using Confluent.Kafka;
using IngestDBCore;
using IngestDBCore.Notify;
using IngestDBCore.Tool;
using Sobey.Core.Log;
using System;
using System.Collections.Generic;
using System.Text;

namespace KafKaNotifyPlugin
{
    
    public class KafKaNotify : SubNotify
    {
        protected readonly ProducerConfig _config;
        private readonly ILogger Logger = LoggerManager.GetLogger("KafKaNotify");
        public KafKaNotify()
        {
            _config = new ProducerConfig() { BootstrapServers = ApplicationContext.Current.KafkaUrl};

            Logger.Info($"kafka init {ApplicationContext.Current.KafkaUrl}");
        }
        public override void ActionNotify(object theClock, NotifyArgs ti)
        {
            //发送通知
            if ((ti.Intent & NotifyPlugin.Kafka) > 0)
            {
                using (var p = new ProducerBuilder<Null, string>(_config).Build())
                {
                    try
                    {
                        var f = p.ProduceAsync("ingestdbnotify", new Message<Null, string> { Value = JsonHelper.ToJson(ti) }).Result;
                    }
                    catch (Exception e)
                    {

                        Logger.Error("KafKaNotify error " + ti.Type + e.Message);
                    }
                }
            }
        }
    }
}
