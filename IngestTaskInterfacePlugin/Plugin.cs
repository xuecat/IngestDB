using IngestDBCore;
using IngestDBCore.Interface;
using IngestDBCore.Plugin;
using IngestTaskPlugin.Controllers;
using Microsoft.Extensions.DependencyInjection;
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
            
            context.Services.AddScoped<IIngestTaskInterface, IngestTaskInterfaceImplement>();
            context.Services.AddScoped<IngestTaskPlugin.Controllers.v2.TaskController>();
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
