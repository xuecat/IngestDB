using IngestDBCore;
using IngestDBCore.Plugin;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace KafKaNotifyPlugin
{
    public class Plugin : PluginBase
    {
       // {A1D8E69B-D099-4EF2-80F8-C324A3CD0FB7}


        public override string PluginID
        {
            get
            {
                return "A1D8E69B-D099-4EF2-80F8-C324A3CD0FB7";
            }
        }

        public override string PluginName
        {
            get
            {
                return "MsvNotify";
            }
        }

        public override string Description
        {
            get
            {
                return "MsvNotifyManager";
            }
        }


        public override Task<ResponseMessage> Init(ApplicationContext context)
        {

            var notify = new KafKaNotify();
            notify.Subscribe(context.NotifyClock);
            context.Services.AddSingleton<KafKaNotify>(notify);


            return base.Init(context);
        }


        public override Task<ResponseMessage> Start(ApplicationContext context)
        {
            return base.Start(context);
        }

        public override Task<ResponseMessage> Stop(ApplicationContext context)
        {
            return base.Stop(context);
        }
    }
}
