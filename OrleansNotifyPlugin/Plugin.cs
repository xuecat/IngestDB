using IngestDBCore;
using IngestDBCore.Plugin;
using System;
using System.Threading.Tasks;

namespace OrleansNotifyPlugin
{
    public class Plugin : PluginBase
    {
        // {8D82BB78-EAB5-41E7-9EFE-2EE7A71C35CC}


        public override string PluginID
        {
            get
            {
                return "8D82BB78-EAB5-41E7-9EFE-2EE7A71C35CC";
            }
        }

        public override string PluginName
        {
            get
            {
                return "OrleansNotify";
            }
        }

        public override string Description
        {
            get
            {
                return "OrleansNotifyManager";
            }
        }


        public override Task<ResponseMessage> Init(ApplicationContext context)
        {
            
            //context.Services.AddMassTransit();
            //context.Services.configure


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
