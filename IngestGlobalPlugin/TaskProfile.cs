using AutoMapper;
using IngestGlobalPlugin.Dto;
using IngestGlobalPlugin.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace IngestGlobalPlugin
{
    public class TaskProfile : Profile
    {
        public TaskProfile()
        {

            CreateMap<DbpGlobalState, GlobalState>()
                .ForMember(a => a.strLabel, (map) => map.MapFrom(b => b.Label))
                .ForMember(a => a.dtLastTime, (map) => map.MapFrom(b => b.Lasttime));

            //ReverseMap
        }

    }
}
