using AutoMapper;
using IngestDBCore;
using IngestDevicePlugin.Dto;
using IngestDevicePlugin.Dto.Response;
using System;
using System.Collections.Generic;
using System.Text;

namespace IngestDeviceInterfacePlugin
{
  
    public class IngesetDeviceInterfaceMappingProfile : Profile
    {
        public IngesetDeviceInterfaceMappingProfile()
        {
            
            //CreateMap<CaptureChannelInfoResponse, CaptureChannelInfoInterface>();
            CreateMap<CaptureChannelInfoResponse, CaptureChannelInfoInterface>().ReverseMap();
            //CreateMap<RecUnitMap, RecUnitMapInterface>();
            CreateMap<RecUnitMap, RecUnitMapInterface>().ReverseMap();
            CreateMap<ProgrammeInfoInterface, ProgrammeInfoResponse>();
            CreateMap<ProgrammeInfoResponse, ProgrammeInfoInterface>();

            CreateMap<MSVChannelStateInterface, MSVChannelStateResponse>().ReverseMap();
            CreateMap<ResponseMessage<List<CaptureChannelInfoResponse>>, ResponseMessage<List<CaptureChannelInfoInterface>>>().ReverseMap();
            CreateMap<ResponseMessage<ProgrammeInfoResponse>, ResponseMessage<ProgrammeInfoInterface>>().ReverseMap();

            CreateMap<ResponseMessage<List<MSVChannelStateResponse>>, ResponseMessage<List<MSVChannelStateInterface>>>().ReverseMap();
            CreateMap<ResponseMessage<List<CaptureDeviceInfoResponse>>, ResponseMessage<List<CaptureDeviceInfoInterface>>>().ReverseMap();

        }
    }
}
