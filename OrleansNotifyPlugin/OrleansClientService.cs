

namespace OrleansNotifyPlugin
{
    using IngestDBCore;
    using IngestTask.Abstraction.Constants;
    using IngestTask.Abstraction.Grains;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Orleans;
    using Orleans.Configuration;
    using Orleans.Hosting;
    using Sobey.Core.Log;
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    public class OrleansClientService : IHostedService
    {
        public IClusterClient Client { get; }
        //private readonly ILogger Logger = LoggerManager.GetLogger("OrleansNotify");
        private bool _disposed;
        public OrleansClientService(ILoggerProvider loggerProvider)
        {
            Client = new ClientBuilder()
            .Configure<ClusterOptions>(options =>
            {
                options.ClusterId = Cluster.ClusterId;
                options.ServiceId = Cluster.ServiceId;
            })
            .UseAdoNetClustering(
            options =>
            {
                options.Invariant = "MySql.Data.MySqlClient";
                options.ConnectionString = ApplicationContext.Current.ConnectionString;
            })
            .Configure<GatewayOptions>(opts => opts.GatewayListRefreshPeriod = TimeSpan.FromMinutes(3))
            .ConfigureApplicationParts(
                    parts => parts
                        .AddApplicationPart(typeof(IDispatcherGrain).Assembly))
            //.AddSimpleMessageStreamProvider(StreamProviderName.Default);
            .Build();
        }

        public virtual async Task StartAsync(CancellationToken cancellationToken)
        {
            //try
            //{
            //    await Client.Connect();
            //}
            //catch (Exception e)
            //{
            //    Logger.Error("client connect error" + e.Message);
            //}

        }

        public virtual async Task StopAsync(CancellationToken cancellationToken)
        {
            await Client.Close();
            Client.Dispose();
            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~OrleansClientService()
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
    }
}
