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
        public IngestTaskInterfaceImplement(IMapper mapper, TaskController task)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _controller = task;
        }

        private TaskController _controller { get; }
        protected IMapper _mapper { get; }
        public async Task<ResponseMessage> GetTaskCallBack(TaskInternals examineResponse)
        {
            
            switch (examineResponse.funtype)
            {
                case FunctionType.WillBeginAndCapturingTasks:
                    {
                        return _mapper.Map<ResponseMessage<List<TaskContentInterface>>>(await _controller.GetWillBeginAndCapturingTasks());
                    }
                case FunctionType.CurrentTasks:
                    {
                        return _mapper.Map<ResponseMessage<List<TaskContentInterface>>>(await _controller.GetCurrentTasks());
                    }
                default:
                        break;
            }
            
            return null;
        }

        public async Task<ResponseMessage> SubmitTaskCallBack(TaskInternals examineResponse)
        {
            switch (examineResponse.funtype)
            {
                case FunctionType.SetTaskBmp:
                    return _mapper.Map<ResponseMessage<bool>>(await _controller.ModifyTaskBmp(examineResponse.Ext3 as Dictionary<int, string>));
                    break;
                default:
                    break;
            }
            return null;
        }
    }
}
