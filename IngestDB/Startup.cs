using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using IngestDBCore;
using IngestDBCore.Plugin;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
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
            applicationContext.ConnectionString = CreateDBConnect(configuration);

            var apppart = services.FirstOrDefault(x => x.ServiceType == typeof(ApplicationPartManager))?.ImplementationInstance;
            if (apppart != null)
            {
                ApplicationPartManager apm = apppart as ApplicationPartManager;
                //所有附件程序集
                ApplicationContextImpl ac = ApplicationContext.Current as ApplicationContextImpl;
                ac.AdditionalAssembly.ForEach((a) =>
                {
                    apm.ApplicationParts.Add(new AssemblyPart(a));
                });
            }
            bool InitIsOk = applicationContext.Init().Result;

            services.AddToolDefined();


            //插件加载之后引用
            services.AddAutoMapper(typeof(Startup));
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

        public string CreateDBConnect(IConfiguration config)
        {
            var dblist = config.GetSection("PublicSetting:DBConfig").GetChildren();
            foreach (var item in dblist)
            {
                if (item["Instance"].CompareTo("ingestdb") == 0)
                {
                    return string.Format(
                "Server={0};Port={4};Database={1};Uid={2};Pwd={3};Pooling=true;allowuservariables=True;cacheserverproperties=True;minpoolsize=1;MaximumPoolSize=20;SslMode=none;Convert Zero Datetime=True;Allow Zero Datetime=True",
                config["PublicSetting:System:Sys_VIP"], item["Instance"], item["Username"], IngestDBCore.Tool.Base64SQL.Base64_Decode(item["Password"]), item["Port"]);
                }
            }

            return "";
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
            //跨域
            app.UseCors(options =>
            {
                options.AllowAnyHeader();
                options.AllowAnyMethod();
                options.AllowAnyOrigin();
                options.AllowCredentials();
            });

            //需要吗？？？
            //app.UseStaticFiles(new StaticFileOptions()  
            //{                                                                       
            //    FileProvider = new PhysicalFileProvider(                                 
            //Path.Combine(Directory.GetCurrentDirectory(), @"MyStaticFiles")),
            //    RequestPath = new PathString("/StaticFiles")         
            //});

            app.UseHttpsRedirection();
            app.UseMvc();

            applicationContext.AppServiceProvider= app.ApplicationServices;
            using (var scope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                applicationContext.ServiceProvider = scope.ServiceProvider;
                //var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                //var pluginFactory = scope.ServiceProvider.GetRequiredService<IPluginFactory>();
            }
            applicationContext.Start();
        }
    }
}
