using IngestDBCore;
using IngestDBCore.Interface;
using IngestDBCore.Plugin;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace IngestGlobalInterfacePlugin
{
    public class Plugin : PluginBase
    {

        public override string Description => "IngestGlobalInterfaceManager";

        public override string PluginID => "1C8D6D59-0304-4FA6-94E6-2453ECD325B1";
        // {}

        public override string PluginName => "IngestGlobalInterface";


        public override Task<ResponseMessage> Init(ApplicationContext context)
        {

            context.Services.AddScoped<IIngestGlobalInterface, IngestGlobalInterfaceImplement>();
            context.Services.AddScoped<IngestGlobalPlugin.Controllers.GlobalController>();
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
