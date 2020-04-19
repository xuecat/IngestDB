using AutoMapper;
using IngestDBCore;
using IngestDBCore.Interface;
using IngestGlobalPlugin.Controllers;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using static IngestDBCore.GlobalInternals;

namespace IngestGlobalInterfacePlugin
{
    public class IngestGlobalInterfaceImplement : IIngestGlobalInterface
    {
        public async Task<ResponseMessage> GetGlobalCallBack(GlobalInternals examineResponse)
        {
            using (var scope = ApplicationContext.Current.ServiceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var reqService = scope.ServiceProvider.GetRequiredService<GlobalController>();

                switch (examineResponse.funtype)
                {
                    case FunctionType.UserParamTemplateByID:
                        return await reqService.GetParamTemplateStringByID(examineResponse.TemplateID);
                    case FunctionType.MaterialInfo:
                        {
                            //MaterialInfoInterface
                        }
                        break;
                    default:
                        break;
                }
                //var response = await scope.ServiceProvider.GetRequiredService<GlobalController>()
                //    .SubmitGlobalCallback();

                //return Mapper.Map<ResponseMessage>(response);
            }

            return null;
        }

        public async Task<ResponseMessage> SubmitGlobalCallBack(GlobalInternals examineResponse)
        {
            using (var scope = ApplicationContext.Current.ServiceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var reqService = scope.ServiceProvider.GetRequiredService<GlobalController>();

                switch (examineResponse.funtype)
                {
                    case FunctionType.SetGlobalState:
                        return await reqService.SetGlobalState(examineResponse.State);
                    
                    default:
                        break;
                }
                //var response = await scope.ServiceProvider.GetRequiredService<GlobalController>()
                //    .SubmitGlobalCallback();

                //return Mapper.Map<ResponseMessage>(response);
            }

            return null;
        }


        
    }
}
