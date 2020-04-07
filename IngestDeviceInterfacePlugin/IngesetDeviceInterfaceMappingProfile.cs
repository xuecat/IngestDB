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
            
            CreateMap<CaptureChannelInfoResponse, CaptureChannelInfoInterface>();


        }
    }
}
