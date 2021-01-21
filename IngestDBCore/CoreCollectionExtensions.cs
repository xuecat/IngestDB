using IngestDBCore.Notify;
using IngestDBCore.Tool;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Net;
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

                var endpoint = client.GetGlobalParam(false, "admin", "IngestTaskEndPoints").Result;
                if (!string.IsNullOrEmpty(endpoint))
                {
                    var points = endpoint.Split(";");
                    
                    ApplicationContext.Current.IngestTask = new System.Net.IPEndPoint[points.Length];
                    for (int i = 0; i < points.Length; i++)
                    {
                        var ipinfo = points[i].Split(":");
                        ApplicationContext.Current.IngestTask[i] = new System.Net.IPEndPoint(IPAddress.Parse(ipinfo[0]), int.Parse(ipinfo[1]));
                    }
                }
            }
            services.AddSingleton<RestClient>(provider => new RestClient(provider.GetService<IHttpClientFactory>()));
           
            services.AddSingleton<NotifyClock>();
        }
    }
}
