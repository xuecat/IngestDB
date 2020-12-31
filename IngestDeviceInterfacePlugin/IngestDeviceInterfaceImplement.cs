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
        public IngestDeviceInterfaceImplement(IMapper mapper, DeviceController controller)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _controller = controller;
        }

        private DeviceController _controller { get; }
        protected IMapper _mapper { get; }
        public async Task<ResponseMessage> GetDeviceCallBack(DeviceInternals examineResponse)
        {
            switch (examineResponse.funtype)
            {
                case FunctionType.ChannelInfoBySrc:
                    {
                        return _mapper.Map<ResponseMessage<List<CaptureChannelInfoInterface>>>(
                            await _controller.ChannelsByProgrammeId(examineResponse.SrcId, examineResponse.Status));
                    }
                    break;
                case FunctionType.DeviceInfoByID:
                    {
                        return _mapper.Map<ResponseMessage<DeviceInfoInterface>>(
                        await _controller.GetCaptureDeviceByID(examineResponse.DeviceId));
                    }

                case FunctionType.SingnalIDByChannel:
                    {
                        return await _controller.GetChannelSignalSrc(examineResponse.ChannelId, examineResponse.SignalStrict);
                    }

                case FunctionType.ChannelUnitMap:
                    {
                        return await _controller.GetChannelUnitMapID(examineResponse.ChannelId);
                    }

                case FunctionType.BackSignalByID:
                    {
                        return _mapper.Map<ResponseMessage<ProgrammeInfoInterface>>(
                            await _controller.GetBackProgramInfoBySrgid(examineResponse.SrcId)
                            );
                    }

                case FunctionType.CaptureTemplateIDBySignal:
                    {
                        return await _controller.CaptureTemplateId(examineResponse.SrcId);
                    }

                case FunctionType.AllChannelState:
                    {
                        return _mapper.Map<ResponseMessage<List<MSVChannelStateInterface>>>(
                            await _controller.AllChannelState()
                            );
                    }
                    break;
                case FunctionType.ChannelExtendData:
                    {
                        return await _controller.GetChannelExtendData(examineResponse.ChannelId, examineResponse.Status);
                    }

                    break;
                case FunctionType.SignalInfoByID:
                    {
                        return _mapper.Map<ResponseMessage<ProgrammeInfoInterface>>(
                            await _controller.GetProgramInfoBySrgid(examineResponse.SrcId)
                            );
                    }
                    break;
                case FunctionType.AllCaptureChannels:
                    {
                        return _mapper.Map<ResponseMessage<List<CaptureChannelInfoInterface>>>(
                            await _controller.AllCaptureChannels()
                            );
                    }
                    break;
                case FunctionType.AllRouterInPort:
                    return _mapper.Map<ResponseMessage<List<RouterInInterface>>>(
                            await _controller.AllRouterInPortInfos()
                            );
                case FunctionType.AllCaptureDevice:
                    return _mapper.Map<ResponseMessage<List<CaptureDeviceInfoInterface>>>(await _controller.AllCaptureDevices());
                case FunctionType.RtmpCaptureChannels:
                    return _mapper.Map<ResponseMessage<List<CaptureChannelInfoInterface>>>(await _controller.RtmpCaptureChannels());
                default:
                    break;
            }
            //var response = await scope.ServiceProvider.GetRequiredService<GlobalController>()
            //    .SubmitGlobalCallback();

            //return Mapper.Map<ResponseMessage>(response);

            //var response = await scope.ServiceProvider.GetRequiredService<GlobalController>()
            //    .SubmitGlobalCallback();

            //return Mapper.Map<ResponseMessage>(response);


            return null;
        }

        public async Task<ResponseMessage> SubmitDeviceCallBack(DeviceInternals examineResponse)
        {
            throw new NotImplementedException();
        }

    }
}
