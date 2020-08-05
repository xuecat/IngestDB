using AutoMapper;
using IngestDBCore;
using IngestDBCore.Interface;
using IngestGlobalPlugin.Controllers;
using IngestGlobalPlugin.Controllers.v2;
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
        public IngestGlobalInterfaceImplement(IMapper mapper, UserController user, MaterialController mate, GlobalController global)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _userController = user;
            _materialController = mate;
            _globalController = global;
        }
        private UserController _userController { get; }
        private MaterialController _materialController { get; }
        private GlobalController _globalController { get; }
        protected IMapper _mapper { get; }
        public async Task<ResponseMessage> GetGlobalCallBack(GlobalInternals examineResponse)
        {
            
            
            //var reqService = scope.ServiceProvider.GetRequiredService<GlobalController>();

            switch (examineResponse.Funtype)
            {
                case FunctionType.UserParamTemplateByID:
                    return await _userController.GetParamTemplateStringByID(examineResponse.TemplateID);
                case FunctionType.MaterialInfo:
                    {
                        //MaterialInfoInterface
                        return _mapper.Map<ResponseMessage<List<MaterialInfoInterface>>>(
                            await _materialController.GetMaterailInfo(examineResponse.TaskID)
                            );
                    }
                    break;
                default:
                    break;
            }
            //var response = await scope.ServiceProvider.GetRequiredService<GlobalController>()
            //    .SubmitGlobalCallback();

            //return Mapper.Map<ResponseMessage>(response);
            

            return null;
        }

        public async Task<ResponseMessage> SubmitGlobalCallBack(GlobalInternals examineResponse)
        {
            
            switch (examineResponse.Funtype)
            {
                case FunctionType.SetGlobalState:
                    {
                        return await _globalController.SetGlobalState(examineResponse.State);
                    } break;
                    
                    
                default:
                    break;
            }
            //var response = await scope.ServiceProvider.GetRequiredService<GlobalController>()
            //    .SubmitGlobalCallback();

            //return Mapper.Map<ResponseMessage>(response);
            

            return null;
        }


        
    }
}
