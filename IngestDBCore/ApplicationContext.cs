using IngestDBCore.Interface;
using IngestDBCore.Notify;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace IngestDBCore
{
    public class ApplicationContext : ApplicationConfig
    {
        public List<Assembly> AdditionalAssembly { get; set; }

        public IServiceCollection Services { get; protected set; }
        public static ApplicationContext Current { get; private set; }
        //public ApplicationConfig Config { get; protected set; }
        //public IServiceProvider ServiceProvider { get; set; }
        public IServiceProvider AppServiceProvider { get; set; }
        public IPluginFactory PluginFactory { get; set; }
        public IApplicationBuilder ApplicationBuilder { get; set; }
        
        public NotifyClock NotifyClock { get; set;}
        public ApplicationContext(IServiceCollection serviceContainer)
        {
            Current = this;
            Services = serviceContainer;
        }

        public async virtual Task<bool> Init()
        {
            return true;
        }

        public async virtual Task<bool> Start()
        {
            return true;
        }

        public async virtual Task<bool> Stop()
        {
            return true;
        }
    }
}
