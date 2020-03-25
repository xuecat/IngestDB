using IngestDBCore;
using IngestDBCore.Plugin;
using Microsoft.Extensions.DependencyInjection;
using Sobey.Core.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace IngestDBRun
{
    public class ApplicationContextImpl : ApplicationContext
    {
        private Sobey.Core.Log.ILogger ExceptionLogger = null;
        public ApplicationContextImpl(IServiceCollection service)
            : base(service)
        {
            string pluginPath = System.IO.Path.Combine(AppContext.BaseDirectory, "Plugin");
            if (!System.IO.Directory.Exists(pluginPath))
            {
                System.IO.Directory.CreateDirectory(pluginPath);
            }
            ExceptionLogger = LoggerManager.GetLogger("PluginInitException");

            //所有程序集
            DirectoryLoader dl = new DirectoryLoader();
            List<Assembly> assList = new List<Assembly>();
            var psl = dl.LoadFromDirectory(pluginPath);
            assList.AddRange(psl);
            AdditionalAssembly = assList;
        }

        public async override Task<bool> Init()
        {
            try
            {
                await base.Init();

                string pluginPath = System.IO.Path.Combine(AppContext.BaseDirectory, "Plugin");
                PluginFactory.Load(pluginPath);
                bool isOk = PluginFactory.Init(this).Result;
                return true;
            }
            catch (ReflectionTypeLoadException ex)
            {
                ExceptionLogger.Error("load exception：\r\n{0}", ex.ToString());
                if (ex.LoaderExceptions != null)
                {
                    foreach (Exception e in ex.LoaderExceptions)
                    {
                        ExceptionLogger.Error("{0}", e.ToString());
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                ExceptionLogger.Error("init exception：\r\n{0}", ex.ToString());
                return false;
            }

            return false;
        }

        public async override Task<bool> Start()
        {
            await PluginFactory.Start(this);
            return await base.Start();
        }

        public async override Task<bool> Stop()
        {
            await PluginFactory.Stop(this);
            return await base.Stop();
        }
    }
}
