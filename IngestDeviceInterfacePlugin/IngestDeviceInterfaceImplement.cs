using AutoMapper;
using IngestDBCore;
using IngestDBCore.Interface;
using IngestDevicePlugin.Controllers.v2;
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
                            return _mapper.Map< ResponseMessage <List< CaptureChannelInfoInterface >>>(
                                await reqService.ChannelsByProgrammeId(examineResponse.SrcId, examineResponse.Status));
                        } break;

                    case FunctionType.SingnalIDByChannel:
                        {
                            return await reqService.GetChannelSignalSrc(examineResponse.ChannelId);
                        }

                    case FunctionType.ChannelUnitMap:
                        {
                             return await reqService.GetChannelUnitMapID(examineResponse.ChannelId);
                        }

                    case FunctionType.BackSignalByID:
                        {
                            return _mapper.Map<ResponseMessage<ProgrammeInfoInterface>>(
                                await reqService.GetBackProgramInfoBySrgid(examineResponse.SrcId)
                                );
                        }

                    case FunctionType.CaptureTemplateIDBySignal:
                        {
                            return await reqService.CaptureTemplateId(examineResponse.SrcId);
                        } 

                    case FunctionType.AllChannelState:
                        {
                            return _mapper.Map<ResponseMessage<List<MSVChannelStateInterface>>>(
                                await reqService.AllChannelState()
                                );
                        }
                        break;
                    case FunctionType.ChannelExtendData:
                        {
                            return await reqService.GetChannelExtendData(examineResponse.ChannelId, examineResponse.Status);
                        }

                        break;
                    case FunctionType.SignalInfoByID:
                        {
                            return _mapper.Map<ResponseMessage<ProgrammeInfoInterface>>(
                                await reqService.GetProgramInfoBySrgid(examineResponse.SrcId)
                                );
                        } break;
                    case FunctionType.AllCaptureChannels:
                        {
                            return _mapper.Map<ResponseMessage<List<CaptureChannelInfoInterface>>>(
                                await reqService.AllCaptureChannels()
                                );
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
