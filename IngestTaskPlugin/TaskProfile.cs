using AutoMapper;
using IngestTaskPlugin.Dto;
using IngestTaskPlugin.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace IngestTaskPlugin
{
    public class TaskProfile : Profile
    {
        public TaskProfile()
        {
            CreateMap<TaskMetadataResponse, DbpTaskMetadata>()
                .ForMember(a => a.Metadatalong, (map) => map.MapFrom(b => b.Metadata));

            CreateMap<DbpTaskMetadata, TaskMetadataResponse>()
                .ForMember(a => a.Metadata, (map) => map.MapFrom(b => b.Metadatalong));

            CreateMap<DbpTaskMetadata, GetQueryTaskMetaData_param>()
                .ForMember(a => a.MetaData, (map) => map.MapFrom(b => b.Metadatalong))
                .AfterMap((a, b) => {
                    if (!string.IsNullOrEmpty(a.Metadatalong))
                    {
                        b.bRet = true;
                        b.errStr = "OK";
                    }
                });

            CreateMap<DbpTaskCustommetadata, TaskCustomMetadataResponse>();
            CreateMap<DbpTaskCustommetadata, GetTaskCustomMetadata_OUT>();
                
            //ReverseMap
        }
    }
}
