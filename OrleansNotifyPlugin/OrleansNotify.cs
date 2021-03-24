

namespace OrleansNotifyPlugin
{
    using Confluent.Kafka;
    using IngestDBCore;
    using IngestDBCore.Notify;
    using IngestDBCore.Tool;
    using IngestTask.Abstraction.Constants;
    using IngestTask.Abstraction.Grains;
    using IngestTask.Dto;
    using Microsoft.Extensions.Hosting;
    using Orleans;
    using Orleans.Configuration;
    using Orleans.Hosting;
    using Sobey.Core.Log;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    public class OrleansNotify : ISubNotify, IHostedService
    {
        private readonly ILogger Logger = LoggerManager.GetLogger("OrleansNotify");

        private IClusterClient Client;
        private string[] excludeNotifyAction = { NotifyAction.MODIFYTASKNAME, NotifyAction.MODIFYTASKSTATE};
        private bool _disposed;
        public OrleansNotify()
        {
            _disposed = true;
        }

        public virtual async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                
                Client = new ClientBuilder()
                            .Configure<ClusterOptions>(options =>
                            {
                                options.ClusterId = Cluster.ClusterId;
                                options.ServiceId = Cluster.ServiceId;
                            })
                            .UseAdoNetClustering(opt =>
                            {
                                opt.Invariant = "MySql.Data.MySqlClient";
                                opt.ConnectionString = ApplicationContext.Current.ConnectionString;
                            })
                            //.UseStaticClustering(ApplicationContext.Current.IngestTask)
                            .Configure<GatewayOptions>(opts => opts.GatewayListRefreshPeriod = TimeSpan.FromSeconds(30))
                            .ConfigureApplicationParts(
                                    parts => parts
                                        .AddApplicationPart(typeof(IDispatcherGrain).Assembly).WithReferences())
                            .Build();
                
            }
            catch (Exception e)
            {
                Logger.Error($"orleannotify error {e.Message}");
            }

        }
        public virtual async Task StopAsync(CancellationToken cancellationToken)
        {
            if (Client != null)
            {
                await Client.Close();
                Client.Dispose();
            }
            
            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~OrleansNotify()
        {
            //必须为false
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;
            if (disposing)
            {

            }
            if (Client != null)
            {
                Client.Dispose();
            }
            _disposed = true;
        }

        public void ActionNotify(object theClock, NotifyArgs ti)
        {
            //发送通知
            if ((ti.Intent & NotifyPlugin.Orleans) > 0 && !excludeNotifyAction.Contains(ti.Action))
            {
                
                try
                {
                    if (Client != null && !Client.IsInitialized)
                    {
                        Client.Connect().Wait();
                       
                    }
                }
                catch (Exception e)
                {
                    Task.Delay(1000).Wait();
                    if (Client != null)
                    {
                        Client.Dispose();
                        Client = new ClientBuilder()
                            .Configure<ClusterOptions>(options =>
                            {
                                options.ClusterId = Cluster.ClusterId;
                                options.ServiceId = Cluster.ServiceId;
                            })
                            .UseAdoNetClustering(opt =>
                            {
                                opt.Invariant = "MySql.Data.MySqlClient";
                                opt.ConnectionString = ApplicationContext.Current.ConnectionString;
                            })
                            //.UseStaticClustering(ApplicationContext.Current.IngestTask)
                            .Configure<GatewayOptions>(opts => opts.GatewayListRefreshPeriod = TimeSpan.FromSeconds(30))
                            .ConfigureApplicationParts(
                                    parts => parts
                                        .AddApplicationPart(typeof(IDispatcherGrain).Assembly).WithReferences())
                            .Build();
                        Client.Connect().Wait();
                    }
                        
                    Logger.Error("client connect error retryed" + e.Message);
                }

                if (Client == null)
                {
                    Logger.Error("orlean client null");
                    return;
                }

                switch (ti.Type)
                {
                    case GlobalStateName.ADDTASK:
                        {
                            var grain = Client.GetGrain<IDispatcherGrain>(0);
                            try
                            {
                                DispatchTask task = new DispatchTask();
                                ObjectTool.CopyObjectData(ti.Param, task, "", BindingFlags.Public | BindingFlags.Instance);
                                AutoRetry.RunSync(() => grain.AddTaskAsync(task).Wait());
                            }
                            catch (Exception e)
                            {
                                Logger.Error("orleans client error ADDTASK" + e.Message);
                            }
                        }
                        break;
                    case GlobalStateName.MODTASK:
                        {
                            var grain = Client.GetGrain<IDispatcherGrain>(0);
                            try
                            {
                                DispatchTask task = new DispatchTask();
                                ObjectTool.CopyObjectData(ti.Param, task, "", BindingFlags.Public | BindingFlags.Instance);
                                AutoRetry.RunSync(() => grain.UpdateTaskAsync(task).Wait());
                                
                            }
                            catch (Exception e)
                            {
                                Logger.Error("orleans client error MODTASK" + e.Message);
                            }
                        }
                        break;
                    case GlobalStateName.DELTASK:
                        {
                            var grain = Client.GetGrain<IDispatcherGrain>(0);
                            try
                            {
                                DispatchTask task = new DispatchTask();
                                ObjectTool.CopyObjectData(ti.Param, task, "", BindingFlags.Public | BindingFlags.Instance);
                                AutoRetry.RunSync(() => grain.DeleteTaskAsync(task).Wait());
                                
                            }
                            catch (Exception e)
                            {
                                Logger.Error("orleans client error DELTASK" + e.Message);
                            }
                        }
                        break;
                    case GlobalStateName.MLCRT:
                        { }
                        break;
                    case GlobalStateName.MLCPT:
                        { }
                        break;
                    case GlobalStateName.BACKUP:
                        { }
                        break;
                    case GlobalStateName.INCHANGE:
                        {}
                        break;
                    case GlobalStateName.INDELETE:
                        { }
                        break;
                    case GlobalStateName.OUTCHANGE:
                        { }
                        break;
                    case GlobalStateName.OUTDELETE:
                        { }
                        break;
                    case GlobalStateName.CHANNELCHANGE:
                        {
                            var grain = Client.GetGrain<IDeviceInspections>(0);
                            try
                            {
                                AutoRetry.RunSync(() => grain.NotifyDeviceChangeAsync().Wait());
                            }
                            catch (Exception e)
                            {
                                Logger.Error("orleans client error CHANNELCHANGE" + e.Message);
                            }
                        }
                        break;
                    case GlobalStateName.CHANNELDELETE:
                        {
                            var grain = Client.GetGrain<IDeviceInspections>(0);
                            try
                            {
                                AutoRetry.RunSync(() => grain.NotifyChannelDeleteAsync((int)ti.Param).Wait());
                            }
                            catch (Exception e)
                            {
                                Logger.Error("orleans client error CHANNELDELETE" + e.Message);
                            }
                        }
                        break;
                    case GlobalStateName.DEVICECHANGE:
                        {
                            var grain = Client.GetGrain<IDeviceInspections>(0);
                            try
                            {
                                AutoRetry.RunSync(() => grain.NotifyDeviceDeleteAsync((int)ti.Param).Wait());
                            }
                            catch (Exception e)
                            {
                                Logger.Error("orleans client errorDEVICECHANGE" + e.Message);
                            }
                        }
                        break;
                    case GlobalStateName.DEVICEDELETE:
                        {
                            var grain = Client.GetGrain<IDeviceInspections>(0);
                            try
                            {
                                AutoRetry.RunSync(() => grain.NotifyDeviceChangeAsync().Wait());
                            }
                            catch (Exception e)
                            {
                                Logger.Error("orleans client error DEVICEDELETE" + e.Message);
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
