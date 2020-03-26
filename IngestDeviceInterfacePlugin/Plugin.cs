using IngestDBCore;
using IngestDBCore.Interface;
using IngestDBCore.Plugin;
using IngestDevicePlugin.Controllers;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace IngestTaskInterfacePlugin
{
    public class Plugin : PluginBase
    {
        // {628D9AF8-08AA-4E1C-A260-67F02C712A9F}

        public override string Description => "IngestDeviceInterfaceManager";

        public override string PluginID => "628D9AF8-08AA-4E1C-A260-67F02C712A9F";
        // {}

        public override string PluginName => "IngestDeviceInterface";


        public override Task<ResponseMessage> Init(ApplicationContext context)
        {
            context.Services.AddScoped<IIngestDeviceInterface, IngestDeviceInterfaceImplement>();
            context.Services.AddScoped<DeviceController>();
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
