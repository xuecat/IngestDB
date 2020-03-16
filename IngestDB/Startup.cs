using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IngestDBCore.Plugin;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace IngestDB
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        private static ApplicationContextImpl applicationContext = null;
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            var configuration = new ConfigurationBuilder()
                //.AddJsonFile("appsettings.json")
                .AddXmlFile("publicsetting.xml")
                .AddEnvironmentVariables()
                .Build();
            services.AddSingleton<IConfigurationRoot>(configuration);
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            //services.AddDbContext<CoreDbContext>(options =>
            //{
            //    options.UseMySql(configuration["Data:DefaultConnection:ConnectionString"]);
            //});
            applicationContext = new ApplicationContextImpl(services);
            applicationContext.PluginFactory = new DefaultPluginFactory();

            applicationContext.VIP = Configuration["PublicSetting:System:Sys_VIP"];
            applicationContext.IngestDBUrl = CreateConfigURI(Configuration, "PublicSetting:System:IngestDBSvr");
            applicationContext.IngestMatrixUrl = CreateConfigURI(Configuration, "PublicSetting:System:IngestDEVCTL");
            applicationContext.CMServerUrl = CreateConfigURI(Configuration, "PublicSetting:System:CMServer");
            applicationContext.CMServerWindowsUrl = CreateConfigURI(Configuration, "PublicSetting:System:CMserver_windows");
            //applicationContext.ConnectionString = configuration["Data:DefaultConnection:ConnectionString"];
        }

        public string CreateConfigURI(IConfiguration config, string key)
        {
            if (config[key].ToString().IndexOf("http:") >= 0 || config[key].ToString().IndexOf("https:") >= 0)
            {
                return config[key];
            }
            else
                return "http://" + config[key];
            return "";
        }

        public string CreateDBConnect(IConfiguration config, string key)
        {
            return string.Format(
                "Server={0};Port={4};Database={1};Uid={2};Pwd={3};Pooling=true;allowuservariables=True;cacheserverproperties=True;minpoolsize=1;MaximumPoolSize=20;SslMode=none;Convert Zero Datetime=True;Allow Zero Datetime=True",
                config["PublicSetting:System:Sys_VIP"], dateName, name, passwd, MysqlPort);

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
