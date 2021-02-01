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
            ApplicationContext.Current.IngestMatrixUrl = "http://"+ client.GetGlobalParam(false, "admin", "IngestDeviceCtrlIP").Result;
            ApplicationContext.Current.IngestMatrixUrl += ":";
            ApplicationContext.Current.IngestMatrixUrl += client.GetGlobalParam(false, "admin", "IngestDeviceCtrlPort").Result;

            if (!string.IsNullOrEmpty(ApplicationContext.Current.KafkaUrl))
            {
                ApplicationContext.Current.KafkaUrl = ApplicationContext.Current.KafkaUrl.Replace(";", ",");
            }

            //获取配置的网管配置CLIP_SUFFIX， DEFAULT_SUFFIX_CHECKBOX
            ApplicationContext.Current.SplitTaskNameTemplate = client.GetGlobalParam(false, "admin", "CLIP_SUFFIX").Result;
            ApplicationContext.Current.SplitTaskNameType = int.Parse(client.GetGlobalParam(false, "admin", "DEFAULT_SUFFIX_CHECKBOX").Result);

            services.AddSingleton<RestClient>(client);
           
            ApplicationContext.Current.NotifyClock = new NotifyClock();
            services.AddSingleton<NotifyClock>(ApplicationContext.Current.NotifyClock);
        }
    }
}
