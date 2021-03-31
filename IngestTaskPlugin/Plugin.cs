using EFCore.Sharding;
using IngestDBCore;
using IngestDBCore.Plugin;
using IngestTaskPlugin.Managers;
using IngestTaskPlugin.Stores;
using IngestTaskPlugin.Stores.Policy;
using IngestTaskPlugin.Stores.VTR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace IngestTaskPlugin
{
    
    public class Plugin : PluginBase
    {
        public override string PluginID
        {
            get
            {
                return "e7acec14-a68b-4116-b9a0-7d07be69de58";
            }
        }

        public override string PluginName
        {
            get
            {
                return "IngestTask";
            }
        }

        public override string Description
        {
            get
            {
                return "IngestTaskManager";
            }
        }


        public override Task<ResponseMessage> Init(ApplicationContext context)
        {
            context.Services.AddDbContext<IngestTaskPlugin.Models.IngestTaskDBContext>(options => options.UseMySql(context.ConnectionString), ServiceLifetime.Scoped);
            context.Services.AddScoped<ITaskStore, TaskInfoStore>();
            context.Services.AddScoped<TaskManager>();
            context.Services.AddScoped<IVtrStore, VtrStore>();
            context.Services.AddScoped<IPolicyStore, PolicyStore>();
            context.Services.AddScoped<VtrManager>();
            context.Services.AddScoped<PolicyManager>();

            context.Services.AddEFCoreSharding(config => {
                config.AddDataSource(context.ConnectionString, ReadWriteType.Read|ReadWriteType.Write, DatabaseType.MySql);
            });

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
