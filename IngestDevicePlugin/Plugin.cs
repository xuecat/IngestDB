using IngestDBCore;
using IngestDBCore.Plugin;
using IngestDevicePlugin.Models;
using IngestDevicePlugin.Stores;
using IngestTaskPlugin.Managers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace IngestDevicePlugin
{
    public enum DBVersion
    {
        V1,
        V2
    }
    public class Plugin : PluginBase
    {
        public override string PluginID
        {
            get
            {
                return "D018511A-DBE7-45D6-B9AD-7A43360450C6";
            }
        }
       

        public override string PluginName
        {
            get
            {
                return "IngestDevice";
            }
        }

        public override string Description
        {
            get
            {
                return "IngestDeviceManager";
            }
        }


        public override Task<ResponseMessage> Init(ApplicationContext context)
        {
            context.Services.AddDbContext<IngestDeviceDBContext>(options => options.UseMySql(context.ConnectionString), ServiceLifetime.Scoped);
            context.Services.AddScoped<IDeviceStore, DeviceInfoStore>();
            context.Services.AddScoped<DeviceManager>();

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
