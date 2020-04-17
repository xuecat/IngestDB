using AutoMapper;
using IngestDBCore;
using IngestDevicePlugin.Dto;
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

        }
    }
}
