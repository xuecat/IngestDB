using AutoMapper;
using IngestDBCore;
using IngestDBCore.Interface;
using IngestTaskPlugin.Controllers.v2;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using static IngestDBCore.TaskInternals;

namespace IngestTaskInterfacePlugin
{
    public class IngestTaskInterfaceImplement : IIngestTaskInterface
    {
        public IngestTaskInterfaceImplement(IMapper mapper)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        protected IMapper _mapper { get; }
        public async Task<ResponseMessage> GetTaskCallBack(TaskInternals examineResponse)
        {
            using (var scope = ApplicationContext.Current.ServiceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var reqService = scope.ServiceProvider.GetRequiredService<TaskController>();

                switch (examineResponse.funtype)
                {
                    case FunctionType.WillBeginAndCapturingTasks:
                        {
                            return _mapper.Map<ResponseMessage<TaskContentInterface>>(await reqService.GetWillBeginAndCapturingTasks());
                        }
                    case FunctionType.CurrentTasks:
                        {
                            return _mapper.Map<ResponseMessage<TaskContentInterface>>(await reqService.GetCurrentTasks());
                        }
                    default:
                        break;
                }
            }
            return null;
        }

        public async Task<ResponseMessage> SubmitTaskCallBack(TaskInternals examineResponse)
        {
            throw new NotImplementedException();
        }
    }
}
