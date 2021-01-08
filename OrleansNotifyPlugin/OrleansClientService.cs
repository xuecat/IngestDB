

namespace OrleansNotifyPlugin
{
    using IngestDBCore;
    using IngestTask.Abstraction.Constants;
    using IngestTask.Abstraction.Grains;
    using Microsoft.Extensions.Hosting;
    using Orleans;
    using Orleans.Configuration;
    using Orleans.Hosting;
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    public abstract class OrleansClientService : IHostedService, IDisposable
    {       
        public IClusterClient Client { get; }
        private bool _disposed;
        OrleansClientService()
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
                        .AddApplicationPart(typeof(IDispatcherGrain).Assembly)
                        .WithReferences())
            //.AddSimpleMessageStreamProvider(StreamProviderName.Default);
            .Build();
        }

        public virtual async Task StartAsync(CancellationToken cancellationToken)
        {
            await Client.Connect();
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
