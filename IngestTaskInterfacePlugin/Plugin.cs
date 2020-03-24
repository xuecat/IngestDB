using IngestDBCore;
using IngestDBCore.Plugin;
using System;
using System.Threading.Tasks;

namespace IngestTaskInterfacePlugin
{
    public class Plugin : PluginBase
    {
        public override string Description => "IngestTaskInterfaceManager";

        public override string PluginID => "8104363e-eb80-4082-94aa-8108030e397c";
        // {}

        public override string PluginName => "IngestTaskInterface";


        public override Task<ResponseMessage> Init(ApplicationContext context)
        {
            
            //context.Services.AddScoped<IHumanInterface, HumanInterfaceImplement>();
            //context.Services.AddScoped<HumanInfoController>();
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
