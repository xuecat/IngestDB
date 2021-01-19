using IngestDBCore.Notify;
using IngestDBCore.Tool;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace IngestDBCore
{
    public static class CoreCollectionExtensions
    {
        public static void AddToolDefined(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }
            

            if (!string.IsNullOrEmpty(ApplicationContext.Current.KafkaUrl))
            {
                ApplicationContext.Current.KafkaUrl = ApplicationContext.Current.KafkaUrl.Replace(";", ",");
            }

            using (var client = new RestClient())
            {
                ApplicationContext.Current.KafkaUrl = client.GetGlobalParam(false, "admin", "KafkaAddress").Result;
                ApplicationContext.Current.IngestMatrixUrl = "http://" + client.GetGlobalParam(false, "admin", "IngestDeviceCtrlIP").Result;
                ApplicationContext.Current.IngestMatrixUrl += ":";
                ApplicationContext.Current.IngestMatrixUrl += client.GetGlobalParam(false, "admin", "IngestDeviceCtrlPort").Result;
            }
            services.AddSingleton<RestClient>(provider => new RestClient(provider.GetService<IHttpClientFactory>()));
           
            ApplicationContext.Current.NotifyClock = new NotifyClock();
            services.AddSingleton<NotifyClock>(ApplicationContext.Current.NotifyClock);
        }
    }
}
