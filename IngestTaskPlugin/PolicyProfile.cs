using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using IngestTaskPlugin.Dto;
using IngestTaskPlugin.Dto.OldResponse;
using IngestTaskPlugin.Dto.Request;
using IngestTaskPlugin.Dto.Response;
using IngestTaskPlugin.Extend;
using IngestTaskPlugin.Models;

namespace IngestTaskPlugin
{
    public class PolicyProfile : Profile
    {
        public PolicyProfile()
        {

            CreateMap<MetaDataPolicyResponse, MetaDataPolicy>()
               .ForMember(a => a.nID, (map) => map.MapFrom(b => b.ID))
               .ForMember(a => a.nDefaultPolicy, (map) => map.MapFrom(b => b.DefaultPolicy))
               .ForMember(a => a.strArchiveType, (map) => map.MapFrom(b => b.ArchiveType))
               .ForMember(a => a.strDesc, (map) => map.MapFrom(b => b.Desc))
               .ForMember(a => a.strName, (map) => map.MapFrom(b => b.Name)).ReverseMap();

            CreateMap<DbpMetadatapolicy, MetaDataPolicy>()
               .ForMember(a => a.nID, (map) => map.MapFrom(b => b.Policyid))
               .ForMember(a => a.nDefaultPolicy, (map) => map.MapFrom(b => b.Defaultpolicy))
               .ForMember(a => a.strArchiveType, (map) => map.MapFrom(b => b.Archivetype))
               .ForMember(a => a.strDesc, (map) => map.MapFrom(b => b.Policydesc))
               .ForMember(a => a.strName, (map) => map.MapFrom(b => b.Policyname)).ReverseMap();

            CreateMap<DbpMetadatapolicy, MetaDataPolicyResponse>()
               .ForMember(a => a.ID, (map) => map.MapFrom(b => b.Policyid))
               .ForMember(a => a.DefaultPolicy, (map) => map.MapFrom(b => b.Defaultpolicy))
               .ForMember(a => a.ArchiveType, (map) => map.MapFrom(b => b.Archivetype))
               .ForMember(a => a.Desc, (map) => map.MapFrom(b => b.Policydesc))
               .ForMember(a => a.Name, (map) => map.MapFrom(b => b.Policyname)).ReverseMap();

        }
    }
}
