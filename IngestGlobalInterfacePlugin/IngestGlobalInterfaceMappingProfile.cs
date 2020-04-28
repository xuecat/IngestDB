using AutoMapper;
using IngestDBCore;
using IngestGlobalPlugin.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace IngestTaskInterfacePlugin
{
    
    public class IngestGlobalInterfaceMappingProfile : Profile
    {
        public IngestGlobalInterfaceMappingProfile()
        {
            CreateMap<VideoInfoResponse, VideoInfoInterface>().ReverseMap();
            CreateMap<AudioInfoResponse, AudioInfoInterface>().ReverseMap();

            CreateMap<MaterialInfoResponse, MaterialInfoInterface>().ReverseMap();

            CreateMap<ResponseMessage<List<MaterialInfoResponse>>, ResponseMessage<List<MaterialInfoInterface>>>();
        }
    }
}
