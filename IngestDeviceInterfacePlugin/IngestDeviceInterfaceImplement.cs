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
                            var f = await reqService.ChannelsByProgrammeId(examineResponse.SrcId, examineResponse.Status);
                            var ret = new ResponseMessage<List<CaptureChannelInfoInterface>>()
                            {
                                Code = f.Code,
                                Msg = f.Msg,
                                Ext = _mapper.Map<List<CaptureChannelInfoInterface>>(f.Ext),
                            };
                            return ret;
                        } break;

                    case FunctionType.SingnalInfoByChannel:
                        {
                            return await reqService.GetChannelSignalSrc(examineResponse.ChannelId);
                        }

                    case FunctionType.ChannelUnitMap:
                        {
                             return await reqService.GetChannelUnitMapID(examineResponse.ChannelId);
                        }

                    case FunctionType.BackSignalByID:
                        {
                            var f = await reqService.GetBackProgramInfoBySrgid(examineResponse.SrcId);
                            var ret = new ResponseMessage<ProgrammeInfoInterface>()
                            {
                                Code = f.Code,
                                Msg = f.Msg,
                                Ext = _mapper.Map<ProgrammeInfoInterface>(f.Ext),
                            };
                            return ret;
                        }

                    case FunctionType.SignalCaptureID:
                        {

                        } break;

                    case FunctionType.AllChannelState:
                        {
                            //MSVChannelStateInterface
                        }break;
                    case FunctionType.ChannelExtendData:
                        {
                            //string
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
        
        public async Task<ResponseMessage> SubmitDeviceCallBack(DeviceInternals examineResponse)
        {
            throw new NotImplementedException();
        }
        
    }
}
