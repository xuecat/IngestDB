using AutoMapper;
using IngestDBCore;
using IngestTaskPlugin.Dto;
using IngestTaskPlugin.Dto.OldResponse;
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
            CreateMap<TaskContent, TaskContentInterface>().ReverseMap();
            CreateMap<ResponseMessage<List<TaskContent>>, ResponseMessage<List<TaskContentInterface>>>().ReverseMap();
        }
    }
}
