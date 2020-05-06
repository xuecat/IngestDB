using IngestDBCore;
using IngestDBCore.Plugin;
using IngestGlobalPlugin.Managers;
using IngestGlobalPlugin.Stores;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace IngestGlobalPlugin
{
    public class Plugin : PluginBase
    {
        public override string Description => "IngestGlobalManager";

        public override string PluginID => "AE7A95D5-7143-42B8-827C-EA7D45597796";

        public override string PluginName => "IngestGlobal";

        public override Task<ResponseMessage> Init(ApplicationContext context)
        {
            context.Services.AddDbContext<Models.IngestGlobalDBContext>(options => options.UseMySql(context.ConnectionString), ServiceLifetime.Scoped);
            context.Services.AddScoped<IGlobalStore, GlobalInfoStore>();
            context.Services.AddScoped<IMaterialStore, MaterialStore>();
            context.Services.AddScoped<GlobalManager>();
            context.Services.AddScoped<MaterialManager>();

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
