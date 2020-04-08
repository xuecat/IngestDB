using AutoMapper;
using IngestDBCore;
using IngestDBCore.Interface;
using IngestDevicePlugin.Controllers;
using IngestDevicePlugin.Dto;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using static IngestDBCore.DeviceInternals;

namespace IngestTaskInterfacePlugin
{
    public class IngestDeviceInterfaceImplement : IIngestDeviceInterface
    {
        public IngestDeviceInterfaceImplement(IMapper mapper)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        protected IMapper _mapper { get; }
        public async Task<ResponseMessage> GetDeviceCallBack(DeviceInternals examineResponse)
        {
            using (var scope = ApplicationContext.Current.ServiceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var reqService = scope.ServiceProvider.GetRequiredService<DeviceController>();

                switch (examineResponse.funtype)
                {
                    case FunctionType.ChannelInfoBySrc:
                        {
                            var f = await reqService.ChannelsByProgrammeId(examineResponse.SrcId);
                            var ret = new ResponseMessage<List<CaptureChannelInfoInterface>>()
                            {
                                Code = f.Code,
                                Msg = f.Msg,
                                Ext = _mapper.Map<List<CaptureChannelInfoInterface>>(f.Ext),
                            };
                            return ret;
                        } break;
                        
                    default:
                        break;
                }
                //var response = await scope.ServiceProvider.GetRequiredService<GlobalController>()
                //    .SubmitGlobalCallback();

                //return Mapper.Map<ResponseMessage>(response);
            }

            return null;
        }
        
        public async Task<ResponseMessage> SubmitDeviceCallBack(DeviceInternals examineResponse)
        {
            throw new NotImplementedException();
        }
        
    }
}
