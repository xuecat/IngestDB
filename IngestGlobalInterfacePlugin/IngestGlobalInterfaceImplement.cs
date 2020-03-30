using IngestDBCore;
using IngestDBCore.Interface;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IngestGlobalInterfacePlugin
{
    public class IngestGlobalInterfaceImplement : IIngestGlobalInterface
    {
        public Task<ResponseMessage> GetGlobalCallBack(GlobalInternals examineResponse)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseMessage> SubmitGlobalCallBack(GlobalInternals examineResponse)
        {
            using (var scope = ApplicationContext.Current.ServiceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                //var response = await scope.ServiceProvider.GetRequiredService<GlobalController>()
                //    .SubmitGlobalCallback();

                //return Mapper.Map<ResponseMessage>(response);
            }
            return null;
        }
    }
}
