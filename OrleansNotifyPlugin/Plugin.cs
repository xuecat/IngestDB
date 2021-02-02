

namespace OrleansNotifyPlugin
{
    using IngestDBCore;
    using IngestDBCore.Notify;
    using IngestDBCore.Plugin;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Orleans;
    using System;
    using System.Threading.Tasks;
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
            context.Services.AddSingleton<OrleansClientService>();
            context.Services.AddSingleton<IHostedService>(sp => sp.GetService<OrleansClientService>());
            context.Services.AddSingleton<IClusterClient>(sp => sp.GetService<OrleansClientService>().Client);

            context.Services.AddSingleton<ISubNotify, OrleansNotify>();
            //using (var scope = context.Services.BuildServiceProvider())
            //{
            //    var client = scope.GetService<IClusterClient>();
                
            //}
            

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
