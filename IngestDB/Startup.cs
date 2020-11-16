using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;
using AutoMapper;
using IngestDBCore;
using IngestDBCore.Basic;
using IngestDBCore.Plugin;
using IngestDBCore.Tool;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.SwaggerGen;

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
            #region polly熔断机制
            var CircuitBreakerOpenTriggerCount= Convert.ToInt32(Configuration["PollySetting:CircuitBreakerOpenTriggerCount"]);
            //通用策略
            services.AddHttpClientPolly("ApiClient", options =>
            {
                options.TimeoutTime = Convert.ToInt32(Configuration["PollySetting:TimeoutTime"]);
                options.RetryCount = Convert.ToInt32(Configuration["PollySetting:RetryCount"]);
                options.CircuitBreakerOpenFallCount = Convert.ToInt32(Configuration["PollySetting:CircuitBreakerOpenFallCount"]);
                options.CircuitBreakerDownTime = Convert.ToInt32(Configuration["PollySetting:CircuitBreakerDownTime"]);
                options.CircuitBreakerAction = (p =>
                {
                    int rcount = (int)p;
                    if (rcount == CircuitBreakerOpenTriggerCount)
                        Environment.Exit(0);//断路器触发超过CircuitBreakerOpenTriggerCount次-退出程序
                });
            });
            #endregion

            //加载字典
            IngestDBCore.GlobalDictionary.Instance.GetType();

            var cfg = new ConfigurationBuilder()
                //.AddXmlFile("publicsetting.xml") //好气  用不了,只能自己手动解析
                .AddEnvironmentVariables()
                .Build();

            var logger = Sobey.Core.Log.LoggerManager.GetLogger("Startup");
            services.AddSingleton<IConfigurationRoot>(cfg);
            services.AddMvc(option => { option.Filters.Add(typeof(IngestAuthentication)); })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .AddJsonOptions(options => { options.SerializerSettings.ContractResolver = new ShouldSerializeContractResolver(); });
            //.AddJsonOptions(options =>//为swagger加的
            //options.SerializerSettings.Converters.Add(new StringEnumConverter()));
            string fileName = "publicsetting.xml";
            string path = string.Empty;
            if ((Environment.OSVersion.Platform == PlatformID.Unix) || (Environment.OSVersion.Platform == PlatformID.MacOSX))
            {
                //str = string.Format(@"{0}/{1}", System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase, fileName);
                path = '/' + fileName;
            }
            else
            {
                path = AppDomain.CurrentDomain.BaseDirectory.Replace("\\", "/") + "/" + fileName;
            }

            logger.Info(path);
            if (File.Exists(path))
            {
                try
                {
                    XDocument xd = new XDocument();
                    xd = XDocument.Load(path);
                    XElement ps = xd.Element("PublicSetting");
                    XElement sys = ps.Element("System");

                    applicationContext = new ApplicationContextImpl(services);
                    applicationContext.UseSwagger = false;
                    applicationContext.PluginFactory = new DefaultPluginFactory();

                    applicationContext.VIP = sys.Element("Sys_VIP").Value;//cfg["PublicSetting:System:Sys_VIP"];
                    applicationContext.IngestDBUrl = CreateConfigURI(sys.Element("IngestDBSvr").Value);
                    applicationContext.IngestMatrixUrl = CreateConfigURI(sys.Element("IngestDEVCTL").Value);
                    applicationContext.CMServerUrl = CreateConfigURI(sys.Element("CMServer").Value);
                    applicationContext.CMServerWindowsUrl = CreateConfigURI(sys.Element("CMserver_windows").Value);
                    applicationContext.ConnectionString = CreateDBConnect(ps, applicationContext.VIP);
                    applicationContext.Limit24Hours = Convert.ToBoolean(Configuration["Limit24Hours"]);
                    applicationContext.NotifyUdpInfomation = Convert.ToBoolean(Configuration["NotifyUdpInfomation"]);

                    logger.Info(path + sys.ToString() + applicationContext.ConnectionString);
                }
                catch (Exception e)
                {

                    logger.Error("ConfigureServices load json error " +e.Message);
                }
                
            }
            else
            { //此处加日志
                logger.Error("no file xml");
                return;
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
            
            //单例注入RestClient等
            services.AddToolDefined(services.BuildServiceProvider().GetService<IHttpClientFactory>());
            bool InitIsOk = applicationContext.Init().Result;

            services.AddApiVersioning(o =>
            {
                o.ReportApiVersions = true;
                //o.AssumeDefaultVersionWhenUnspecified = true;
                o.DefaultApiVersion = new ApiVersion(1, 0);
                //o.ApiVersionReader = new QueryStringApiVersionReader();
                o.AssumeDefaultVersionWhenUnspecified = true;
                //o.ApiVersionSelector = new CurrentImplementationApiVersionSelector(o);
            });

            ;
            var basePath = AppContext.BaseDirectory + "Plugin//";
            var xmlPath1 = basePath + applicationContext.PluginFactory.GetPluginInfo("AE7A95D5-7143-42B8-827C-EA7D45597796").SwaggerXml;
            var xmlPath2 = basePath + applicationContext.PluginFactory.GetPluginInfo("D018511A-DBE7-45D6-B9AD-7A43360450C6").SwaggerXml;
            var xmlPath3 = basePath + applicationContext.PluginFactory.GetPluginInfo("e7acec14-a68b-4116-b9a0-7d07be69de58").SwaggerXml;
            var xmlPath4 = basePath + applicationContext.PluginFactory.GetPluginInfo("464E27F2-D1E8-4900-8293-A700265A5C9F").SwaggerXml;
            logger.Info($"swagger {xmlPath1} {xmlPath2} {xmlPath3} {xmlPath4}");
            if (File.Exists(xmlPath1) && File.Exists(xmlPath2) && File.Exists(xmlPath3))
            {
                applicationContext.UseSwagger = true;

                services.AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v1", new OpenApiInfo
                    {
                        Version = "v1",
                        Title = "> 收录老版本网关接口文档",
                        Description = "**A simple example Ingest Web API**(接口设计原则: `Post`->新加和修改，`Post`->新加；`Put`->修改)",
                        Contact = new OpenApiContact { Name = "XueCat", Email = "", Url = new Uri("http://ingest.com") },
                        License = new OpenApiLicense { Name = "Sobey", Url = new Uri("http://www.sobey.com") }
                        //TermsOfService = new Uri("None"),
                    });

                    c.SwaggerDoc("v2", new OpenApiInfo
                    {
                        Version = "v2",
                        Title = "> 收录新版本网关接口文档",
                        Description = "**Ingest Web API**(接口设计原则: `Post`->新加和修改，`Post`->新加；`Put`->修改, 所有路由和参数均是小写, 所有返回值均是驼峰(注释的是大写, 实际返回驼峰))",
                        Contact = new OpenApiContact { Name = "XueCat", Email = "", Url = new Uri("http://ingest.com") },
                        License = new OpenApiLicense { Name = "Sobey", Url = new Uri("http://www.sobey.com") }
                        //TermsOfService = new Uri("None"),
                    });

                    c.SwaggerDoc("v2.1", new OpenApiInfo
                    {
                        Version = "v2.1",
                        Title = "> 收录新版本网关接口文档(部分为task服务的接口)",
                        Description = "**Ingest Web API**(接口设计原则: `Post`->新加和修改，`Post`->新加；`Put`->修改, 所有路由和参数均是小写, 所有返回值均是驼峰(注释的是大写, 实际返回驼峰))",
                        Contact = new OpenApiContact { Name = "XueCat", Email = "", Url = new Uri("http://ingest.com") },
                        License = new OpenApiLicense { Name = "Sobey", Url = new Uri("http://www.sobey.com") }
                        //TermsOfService = new Uri("None"),
                    });

                    c.OrderActionsBy((apiDesc) => $"{apiDesc.ActionDescriptor.RouteValues["controller"]}_{apiDesc.HttpMethod}");

                    if ((Environment.OSVersion.Platform == PlatformID.Unix) || (Environment.OSVersion.Platform == PlatformID.MacOSX))
                    {
                        xmlPath1 = xmlPath1.ToLower();
                        xmlPath2 = xmlPath2.ToLower();
                        xmlPath3 = xmlPath3.ToLower();
                    }
                    // http://localhost:9024/swagger/v1/swagger.json
                    // http://localhost:9024/swagger/
                    c.IncludeXmlComments(xmlPath1);
                    c.IncludeXmlComments(xmlPath2);
                    c.IncludeXmlComments(xmlPath3);
                    c.IncludeXmlComments(xmlPath4);
                    //c.DescribeAllEnumsAsStrings();
                    c.OperationFilter<HttpHeaderOperation>(); // 添加httpHeader参数
                    c.OperationFilter<DefaultValueOperation>(); // 添加defaultValue参数

                    //c.OperationFilter<RemoveVersionFromParameter>();
                    c.DocumentFilter<ReplaceVersionWithExactValueInPath>();

                    c.DocInclusionPredicate((version, desc) =>
                    {

                        if (!desc.TryGetMethodInfo(out System.Reflection.MethodInfo methodInfo)) return false;
                        var versions = methodInfo.DeclaringType
                            .GetCustomAttributes(true)
                            .OfType<ApiVersionAttribute>()
                            .SelectMany(attr => attr.Versions);


                        var maps = methodInfo
                            .GetCustomAttributes(true)
                            .OfType<MapToApiVersionAttribute>()
                            .SelectMany(attr => attr.Versions)
                            .ToArray();

                        return versions.Any(v => $"v{v.MajorVersion.ToString()}" == version || $"v{v.MajorVersion.ToString()}.{v.MinorVersion.ToString()}" == version)
                               && (!maps.Any() || maps.Any(v => $"v{v.MajorVersion.ToString()}" == version));
                    });
                });
            }


            //插件加载之后引用
            services.AddAutoMapper(applicationContext.AdditionalAssembly);
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
                "Server={0};Port={4};Database={1};Uid={2};Pwd={3};Pooling=true;minpoolsize=0;MaximumPoolSize=40;SslMode=none;",
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

            if (applicationContext.UseSwagger)
            {
                app.UseSwagger().UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "IngestGateway API V1");
                    c.SwaggerEndpoint("/swagger/v2/swagger.json", "IngestGateway API V2");
                    c.SwaggerEndpoint("/swagger/v2.1/swagger.json", "IngestGateway API V2.1");
                    //c.ShowRequestHeaders();
                });
            }

            //需要吗？？？
            //app.UseStaticFiles(new StaticFileOptions()  
            //{                                                                       
            //    FileProvider = new PhysicalFileProvider(                                 
            //Path.Combine(Directory.GetCurrentDirectory(), @"MyStaticFiles")),
            //    RequestPath = new PathString("/StaticFiles")         
            //});

            //app.UseHttpsRedirection();
            app.UseMvc();
            applicationContext.AppServiceProvider = app.ApplicationServices;
            using (var scope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                //applicationContext.ServiceProvider = scope.ServiceProvider;
                //var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                //var pluginFactory = scope.ServiceProvider.GetRequiredService<IPluginFactory>();
            }
            applicationContext.Start();
        }
    }
}
