using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Polly;
using Polly.Timeout;
using Sobey.Core.Log;

namespace IngestDBCore
{
    /// <summary>
    /// http请求熔断机制-工具
    /// </summary>
    public static class PollyHttpClientServiceCollectionExtensions
    {
        private readonly static ILogger logger = Sobey.Core.Log.LoggerManager.GetLogger("Polly");
        /// <summary>
        /// Httpclient扩展方法
        /// </summary>
        /// <param name="services">ioc容器</param>
        /// <param name="name">HttpClient 名称(针对不同的服务进行熔断，降级)</param>
        /// <param name="action">熔断降级配置</param>
        /// <param name="TResult">降级处理错误的结果</param>
        /// <returns></returns>
        public static IServiceCollection AddHttpClientPolly(this IServiceCollection services, string name, Action<HttpClientPollyOptions> action)
        {
            // 1、创建选项配置类
            HttpClientPollyOptions options = new HttpClientPollyOptions();
            action(options);
            // 2、配置httpClient,熔断降级策略
            services.AddHttpClient(name)
           //1.1 降级策略
           .AddPolicyHandler(Policy<HttpResponseMessage>.HandleInner<Exception>().FallbackAsync(options.httpResponseMessage, async b =>
           {
               // 1、降级打印异常
               Console.WriteLine($"服务{name}开始降级,异常消息：{b.Exception.Message}");
               logger.Warn($"服务{name}开始降级,异常消息：{b.Exception.Message}");
               // 2、降级后的数据
               Console.WriteLine($"服务{name}降级内容响应：{options.httpResponseMessage.Content.ReadAsStringAsync().Result}");
               logger.Warn($"服务{name}降级内容响应：{options.httpResponseMessage.Content.ReadAsStringAsync().Result}");

               await Task.CompletedTask;
           }))
            // 1.2 断路器策略
            .AddPolicyHandler(Policy<HttpResponseMessage>.Handle<Exception>().CircuitBreakerAsync(options.CircuitBreakerOpenFallCount, TimeSpan.FromSeconds(options.CircuitBreakerDownTime), (ex, ts) =>
            {
                Console.WriteLine($"服务{name}断路器开启，异常消息：{ex.Exception.Message}");
                Console.WriteLine($"服务{name}断路器开启时间：{ts.TotalSeconds}s");

                logger.Warn($"服务{name}断路器开启，异常消息：{ex.Exception.Message} {ts.TotalSeconds}");
            }, () =>
            {
                logger.Warn($"服务{name}断路器关闭");
                Console.WriteLine($"服务{name}断路器关闭");
            }, () =>
            {
                logger.Warn($"服务{name}断路器半开启(时间控制，自动开关)");
                Console.WriteLine($"服务{name}断路器半开启(时间控制，自动开关)");
            }))
            // 1.3 重试策略 次数/数组(第一次100ms 二次200ms)
            
             //.AddPolicyHandler(Policy<HttpResponseMessage>
            //   .Handle<TimeoutRejectedException>()
            //   .WaitAndRetryAsync(options.RetryTimeoutArray)
            //)
             .AddPolicyHandler(Policy<HttpResponseMessage>
            .Handle<Exception>()
            .RetryAsync(options.RetryCount)
            )
            // 1.4 超时策略
            .AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(options.TimeoutTime)));
            return services;
        }

    }
}
