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
                System.Net.Sockets.SocketException tempExcption = null;
                if (e.Exception is System.Net.Sockets.SocketException)
                {
                    tempExcption = (System.Net.Sockets.SocketException)e.Exception;
                }

                if(tempExcption == null || tempExcption.ErrorCode != 125)
                {
                    ExceptionLogger.Error("Exception: {0} ", e.Exception.ToString());
                }
            }
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (ExceptionLogger != null)
            {
                System.Diagnostics.StackTrace st = new System.Diagnostics.StackTrace();
                System.Diagnostics.StackFrame[] sfs = st.GetFrames();
                //过虑的方法名称,以下方法将不会出现在返回的方法调用列表中
                string _filterdName = "ResponseWrite,ResponseWriteError,";
                string _fullName = string.Empty, _methodName = string.Empty;
                for (int i = 1; i < sfs.Length; ++i)
                {
                    //非用户代码,系统方法及后面的都是系统调用，不获取用户代码调用结束
                    if (System.Diagnostics.StackFrame.OFFSET_UNKNOWN == sfs[i].GetILOffset()) break;
                    _methodName = sfs[i].GetMethod().Name;//方法名称
                                                          //sfs[i].GetFileLineNumber();//没有PDB文件的情况下将始终返回0
                    if (_filterdName.Contains(_methodName)) continue;
                    _fullName = _methodName + "()->" + _fullName;
                }
                st = null;
                sfs = null;
                _filterdName = _methodName = null;

                ExceptionLogger.Fatal("Crash：\r\n{0}", e.ExceptionObject.ToString(), _fullName);
            }
        }
    }


}
