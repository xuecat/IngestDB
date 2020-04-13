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

            CreateMap<DbpUsertemplate, OldUserTemplate>()
                .ForMember(a => a.nTemplateID, (map) => map.MapFrom(b => b.Templateid))
                .ForMember(a => a.strUserCode, (map) => map.MapFrom(b => b.Usercode))
                .ForMember(a => a.strTemplateName, (map) => map.MapFrom(b => b.Templatename))
                .ForMember(a => a.strTemplateContent, (map) => map.MapFrom(b => b.Templatecontent));

            CreateMap<DbpUsertemplate, UserTemplate>()
                .ForMember(a => a.TemplateID, (map) => map.MapFrom(b => b.Templateid))
                .ForMember(a => a.UserCode, (map) => map.MapFrom(b => b.Usercode))
                .ForMember(a => a.TemplateName, (map) => map.MapFrom(b => b.Templatename))
                .ForMember(a => a.TemplateContent, (map) => map.MapFrom(b => b.Templatecontent));


            //ReverseMap
        }

    }
}
