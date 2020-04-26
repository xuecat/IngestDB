using System;
using System.Threading.Tasks;
using IngestDBCore;
using IngestDBCore.Plugin;
using IngestDBCore.Tool;
using IngestMatrixPlugin.Managers;
using IngestMatrixPlugin.Models;
using IngestMatrixPlugin.Stores;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace IngestMatrixPlugin
{
    public class Plugin : PluginBase
    {
        public override string PluginID => "464E27F2-D1E8-4900-8293-A700265A5C9F";

        public override string PluginName => "IngestMatrix";

        public override string Description => "IngestMatrixManager";

        public override Task<ResponseMessage> Init(ApplicationContext context)
        {
            context.Services.AddDbContext<IngestMatrixDBContext>(options => options.UseMySql(context.ConnectionString), ServiceLifetime.Scoped);
            context.Services.AddScoped<IMatrixStore, MatrixStore>();
            context.Services.AddScoped<RestClient>();
            context.Services.AddScoped<MatrixManager>();

            return base.Init(context);
        }


        //public override Task<ResponseMessage> Start(ApplicationContext context)
        //{
        //    return base.Start(context);
        //}

        //public override Task<ResponseMessage> Stop(ApplicationContext context)
        //{
        //    return base.Stop(context);
        //}
    }
}
