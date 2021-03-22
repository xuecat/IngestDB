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

    public class KafKaNotify : ISubNotify
    {
        protected readonly ProducerConfig _config;
        private readonly ILogger Logger = LoggerManager.GetLogger("KafKaNotify");
        public KafKaNotify()
        {
            _config = new ProducerConfig() { BootstrapServers = ApplicationContext.Current.KafkaUrl };

            Logger.Info($"kafka init {ApplicationContext.Current.KafkaUrl}");
        }
        public void ActionNotify(object theClock, NotifyArgs ti)
        {
            //发送通知
            if ((ti.Intent & NotifyPlugin.Kafka) > 0)
            {
                try
                {
                    if (ti.Action == NotifyAction.SENDMSVNOTIFY)//先暂时用表示
                    {
                        Logger.Error("MSV_NOTIFY Log : " + ApplicationContext.Current.KafkaUrl + ", Action :" + ti.Action + ", " + ti.Param);
                        using (var p = new ProducerBuilder<Null, byte[]>(_config).Build())
                        {
                            try
                            {
                                p.Produce("MSV_NOTIFY", new Message<Null, byte[]> { Value = Encoding.Unicode.GetBytes(ti.Param.ToString()) });
                            }
                            catch (Exception e)
                            {

                                Logger.Error("KafKaNotify MSV_NOTIFY error " + ti.Type + e.Message);
                            }
                        }
                    }
                    else
                    {
                        using (var p = new ProducerBuilder<Null, string>(_config).Build())
                        {
                            try
                            {
                                p.Produce("ingestdbnotify", new Message<Null, string> { Value = JsonHelper.ToJson(ti) });
                            }
                            catch (Exception e)
                            {

                                Logger.Error("KafKaNotify error " + ti.Type + e.Message);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error("KafKaNotify_ex MSV_NOTIFY error " + ti.Type + ex.Message);
                }
            }

        }
    }
}
