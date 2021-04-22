using EFCore.Sharding;
using IngestDBCore;
using IngestDBCore.Plugin;
using IngestDBCore.Tool;
using IngestTaskPlugin.Managers;
using IngestTaskPlugin.Models;
using IngestTaskPlugin.Stores;
using IngestTaskPlugin.Stores.Policy;
using IngestTaskPlugin.Stores.VTR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

using ShardingCore.MySql;

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

            //context.Services.AddEFCoreSharding(config => {
            //    config.CreateShardingTableOnStarting(true);
            //    config.EnableShardingMigration(true);
            //    config.AddDataSource(context.ConnectionString, ReadWriteType.Read|ReadWriteType.Write, DatabaseType.MySql);
            //    config.SetDateSharding<DbpTask>(nameof(DbpTask.Endtime), ExpandByDateMode.PerMonth, DateTimeFormat.DateTimeFromString("2021-03-19 10:06:54"));
            //});
            context.Services.AddShardingMySql(o =>{
                o.EnsureCreatedWithOutShardingTable = true;
                o.CreateShardingTableOnStart = true;
                o.AddShardingDbContextWithShardingTable<ShardingDBTaskContext>("ingestdb", context.ConnectionString, config => 
                {
                    config.AddShardingTableRoute<ShardingDBTaskRoute>();
                    config.AddShardingTableRoute<ShardingDBTaskMetadataRoute>();
                });
                o.IgnoreCreateTableError = true;
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
