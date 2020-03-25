using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using AutoMapper;
using IngestDBCore;
using IngestDBCore.Plugin;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IngestDBRun
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
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });


            IngestDBCore.GlobalDictionary.Instance.GetType();

            var cfg = new ConfigurationBuilder()
                //.AddXmlFile("publicsetting.xml") //好气  用不了,只能自己手动解析
                .AddEnvironmentVariables()
                .Build();

            services.AddSingleton<IConfigurationRoot>(cfg);
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            string path = AppDomain.CurrentDomain.BaseDirectory.Replace("\\", "/") + "/publicsetting.xml";
            if (File.Exists(path))
            {
                XDocument xd = new XDocument();
                xd = XDocument.Load(path);
                XElement ps = xd.Element("PublicSetting");
                XElement sys = ps.Element("System");

                applicationContext = new ApplicationContextImpl(services);
                applicationContext.PluginFactory = new DefaultPluginFactory();

                applicationContext.VIP = sys.Element("Sys_VIP").Value;//cfg["PublicSetting:System:Sys_VIP"];
                applicationContext.IngestDBUrl = CreateConfigURI(sys.Element("IngestDBSvr").Value);
                applicationContext.IngestMatrixUrl = CreateConfigURI(sys.Element("IngestDEVCTL").Value);
                applicationContext.CMServerUrl = CreateConfigURI(sys.Element("CMServer").Value);
                applicationContext.CMServerWindowsUrl = CreateConfigURI(sys.Element("CMserver_windows").Value);
                applicationContext.ConnectionString = CreateDBConnect(ps, applicationContext.VIP);
            }

            //services.AddDbContext<CoreDbContext>(options =>
            //{
            //    options.UseMySql(configuration["Data:DefaultConnection:ConnectionString"]);
            //});
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
        public string CreateConfigURI(string str)
        {
            if (str.IndexOf("http:") >= 0 || str.IndexOf("https:") >= 0)
            {
                return str;
            }
            else
                return "http://" + str;
            return "";
        }

        public string CreateDBConnect(XElement config, string vip)
        {
            var dblist = config.Element("DBConfig");
            foreach (var item in dblist.Elements())
            {
                if (item.Attribute("module").Value.CompareTo("INGESTDB") == 0)
                {
                    return string.Format(
                "Server={0};Port={4};Database={1};Uid={2};Pwd={3};Pooling=true;allowuservariables=True;cacheserverproperties=True;minpoolsize=1;MaximumPoolSize=20;SslMode=none;Convert Zero Datetime=True;Allow Zero Datetime=True",
                vip, item.Element("Instance").Value,
                item.Element("Username").Value,
                IngestDBCore.Tool.Base64SQL.Base64_Decode(item.Element("Password").Value),
                item.Element("Port").Value);
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
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMvc();
            applicationContext.AppServiceProvider = app.ApplicationServices;
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
