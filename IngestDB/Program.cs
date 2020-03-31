using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sobey.Core.Log;

namespace IngestDB
{
    public class Program
    {
        private static Sobey.Core.Log.ILogger ExceptionLogger = null;
        public static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                //.AddXmlFile("publicsetting.xml")
                .AddEnvironmentVariables()
                .AddCommandLine(args)
                .Build();

            var logConfig = configuration.GetSection("Logging");
            int maxDays = 10;
            string maxFileSize = "10MB";
            LogLevels logLevel = LogLevels.Info;
            if (logConfig != null)
            {
                Enum.TryParse(logConfig["Level"] ?? "", out logLevel);
                int.TryParse(logConfig["SaveDays"], out maxDays);

                maxFileSize = logConfig["MaxFileSize"];
                if (string.IsNullOrEmpty(maxFileSize))
                {
                    maxFileSize = "10MB";
                }
            }
            LoggerManager.InitLogger(new LogConfig()
            {
                LogBaseDir = logConfig["Path"],
                MaxFileSize = maxFileSize,
                LogLevels = logLevel,
                IsAsync = true,
                LogFileTemplate = LogFileTemplates.PerDayDirAndLogger,
                LogContentTemplate = LogLayoutTemplates.SimpleLayout,
                DeleteDay = maxDays.ToString(),
                //TargetConsole = false
            });
            LoggerManager.SetLoggerAboveLevels(logLevel);

            ExceptionLogger = LoggerManager.GetLogger("Exception");

            //全局异常日志
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            AppDomain.CurrentDomain.FirstChanceException += CurrentDomain_FirstChanceException;

            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseUrls($"http://*:{configuration["Port"]}")
                .Build()
                .Run();
        }


        private static void CurrentDomain_FirstChanceException(object sender, System.Runtime.ExceptionServices.FirstChanceExceptionEventArgs e)
        {
            if (ExceptionLogger != null)
            {
                ExceptionLogger.Error("Exception: {0}", e.Exception.ToString());
            }
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (ExceptionLogger != null)
            {
                ExceptionLogger.Fatal("Crash：\r\n{0}", e.ExceptionObject.ToString());
            }
        }
    }


}
