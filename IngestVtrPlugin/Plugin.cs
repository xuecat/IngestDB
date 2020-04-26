using System.Threading.Tasks;
using IngestDBCore;
using IngestDBCore.Plugin;
using IngestDBCore.Tool;
using IngestVtrPlugin.Managers;
using IngestVtrPlugin.Models;
using IngestVtrPlugin.Stores;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace IngestVtrPlugin
{
    public class Plugin : PluginBase
    {
        public override string PluginID => "8C0455DA-FF2C-4272-BED8-B0CE1588A96B";

        public override string PluginName => "IngestVtr";

        public override string Description => "IngestVtrManager";

        public override Task<ResponseMessage> Init(ApplicationContext context)
        {
            context.Services.AddDbContext<IngestVtrDBContext>(options => options.UseMySql(context.ConnectionString), ServiceLifetime.Scoped);
            context.Services.AddScoped<IVtrStore, VtrStore>();
            context.Services.AddScoped<RestClient>();
            context.Services.AddScoped<VtrManager>();

            return base.Init(context);
        }
    }
}
