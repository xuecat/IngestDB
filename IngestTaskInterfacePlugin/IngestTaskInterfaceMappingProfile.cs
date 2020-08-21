using AutoMapper;
using IngestDBCore;
using IngestTaskPlugin.Dto;
using IngestTaskPlugin.Dto.OldResponse;
using IngestTaskPlugin.Dto.Response;
using System;
using System.Collections.Generic;
using System.Text;

namespace IngestTaskInterfacePlugin
{
    
    public class IngestTaskInterfaceMappingProfile : Profile
    {
        public IngestTaskInterfaceMappingProfile()
        {
            //CreateMap<TaskContent, TaskContentInterface>();
            CreateMap<TaskContentResponse, TaskContentInterface>().ReverseMap();
            CreateMap<ResponseMessage<List<TaskContentResponse>>, ResponseMessage<List<TaskContentInterface>>>().ReverseMap();
        }
    }
}
