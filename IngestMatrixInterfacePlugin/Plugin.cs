using IngestDBCore;
using IngestDBCore.Interface;
using IngestDBCore.Plugin;
using IngestGlobalInterfacePlugin;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace IngestMatrixInterfacePlugin
{
    public class Plugin : PluginBase
    {
        // {CA2B1067-CAEB-47D3-AE0C-D045116EDEA8}
     

        public override string Description => "IngestMatrixInterfaceManager";

        public override string PluginID => "CA2B1067-CAEB-47D3-AE0C-D045116EDEA8";
        // {}

        public override string PluginName => "IngestMatrixInterface";


        public override Task<ResponseMessage> Init(ApplicationContext context)
        {

            context.Services.AddScoped<IIngestMatrixInterface, IngestMatrixInterfaceImplement>();
            context.Services.AddScoped<IngestMatrixPlugin.Controllers.v2.MatrixController>();
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
