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
                        var f = await _controller.GetWillBeginAndCapturingTasks();
                        var ret = new ResponseMessage<List<TaskContentInterface>>()
                        {
                            Code = f.Code,
                            Msg = f.Msg,
                            Ext = _mapper.Map<List<TaskContentInterface>>(f.Ext),
                        };
                        return ret;
                    }
                case FunctionType.CurrentTasks:
                    {
                        var f = await _controller.GetCurrentTasks();
                        var ret = new ResponseMessage<List<TaskContentInterface>>()
                        {
                            Code = f.Code,
                            Msg = f.Msg,
                            Ext = _mapper.Map<List<TaskContentInterface>>(f.Ext),
                        };
                        return ret;
                    }
                default:
                    break;
            }
            
            return null;
        }

        public async Task<ResponseMessage> SubmitTaskCallBack(TaskInternals examineResponse)
        {
            throw new NotImplementedException();
        }
    }
}
