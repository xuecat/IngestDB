using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using AutoMapper;
using IngestDBCore;
using IngestDBCore.Basic;
using IngestDBCore.Plugin;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
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
            //加载字典
            IngestDBCore.GlobalDictionary.Instance.GetType();

            var cfg = new ConfigurationBuilder()
                //.AddXmlFile("publicsetting.xml") //好气  用不了,只能自己手动解析
                .AddEnvironmentVariables()
                .Build();

            services.AddSingleton<IConfigurationRoot>(cfg);
            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            //.AddJsonOptions(options =>//为swagger加的
            //options.SerializerSettings.Converters.Add(new StringEnumConverter()));

            string path = AppDomain.CurrentDomain.BaseDirectory.Replace("\\", "/") + "/publicsetting.xml";
            if (File.Exists(path))
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
            services.AddToolDefined();

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


            var basePath = AppContext.BaseDirectory;
            var xmlPath1 = Path.Combine(basePath, "Plugin", "IngestGlobalPlugin.xml");
            var xmlPath2 = Path.Combine(basePath, "Plugin", "IngestDevicePlugin.xml");
            var xmlPath3 = Path.Combine(basePath, "Plugin", "IngestTaskPlugin.xml");

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
                        Contact = new OpenApiContact { Name = "XueCat", Email = "", Url = new Uri("http://xuecat.com") },
                        License = new OpenApiLicense { Name = "Sobey", Url = new Uri("http://www.sobey.com") }
                        //TermsOfService = new Uri("None"),
                    });

                    c.SwaggerDoc("v2", new OpenApiInfo
                    {
                        Version = "v2",
                        Title = "> 收录新版本网关接口文档",
                        Description = "**Ingest Web API**(接口设计原则: `Post`->新加和修改，`Post`->新加；`Put`->修改)",
                        Contact = new OpenApiContact { Name = "XueCat", Email = "", Url = new Uri("http://xuecat.com") },
                        License = new OpenApiLicense { Name = "Sobey", Url = new Uri("http://www.sobey.com") }
                        //TermsOfService = new Uri("None"),
                    });

                    c.OrderActionsBy((apiDesc) => $"{apiDesc.ActionDescriptor.RouteValues["controller"]}_{apiDesc.HttpMethod}");

                    // http://localhost:9024/swagger/v1/swagger.json
                    // http://localhost:9024/swagger/
                    c.IncludeXmlComments(xmlPath1);
                    c.IncludeXmlComments(xmlPath2);
                    c.IncludeXmlComments(xmlPath3);
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

                        return versions.Any(v => $"v{v.MajorVersion.ToString()}" == version)
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
                "Server={0};Port={4};Database={1};Uid={2};Pwd={3};Pooling=true;minpoolsize=1;MaximumPoolSize=30;SslMode=none;",
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
                applicationContext.ServiceProvider = scope.ServiceProvider;
                //var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                //var pluginFactory = scope.ServiceProvider.GetRequiredService<IPluginFactory>();
            }
            applicationContext.Start();
        }
    }
}
