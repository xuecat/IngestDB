using IngestDBCore.Notify;
using IngestDBCore.Tool;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
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
            services.AddSingleton<RestClient>();

           
            ApplicationContext.Current.NotifyClock = new NotifyClock();
            services.AddSingleton<NotifyClock>(ApplicationContext.Current.NotifyClock);
        }
    }
}
