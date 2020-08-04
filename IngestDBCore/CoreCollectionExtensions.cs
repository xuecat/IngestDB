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
        public static void AddToolDefined(this IServiceCollection services, IHttpClientFactory httpClientFactory)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }
            var client = new RestClient(httpClientFactory);
            ApplicationContext.Current.KafkaUrl = client.GetGlobalParam(false, "admin", "KafkaAddress").Result;

            if (!string.IsNullOrEmpty(ApplicationContext.Current.KafkaUrl))
            {
                ApplicationContext.Current.KafkaUrl = ApplicationContext.Current.KafkaUrl.Replace(";", ",");
            }
            

            services.AddSingleton<RestClient>(client);
           
            ApplicationContext.Current.NotifyClock = new NotifyClock();
            services.AddSingleton<NotifyClock>(ApplicationContext.Current.NotifyClock);
        }
    }
}
