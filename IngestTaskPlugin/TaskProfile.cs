using AutoMapper;
using IngestTaskPlugin.Dto;
using IngestTaskPlugin.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace IngestTaskPlugin
{
    public class TaskInfoConvertResolver: IValueResolver<>
    {

    }
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

            CreateMap<TaskContentResponse, DbpTask>()
                .ForMember(a => a.Recunitid, (map) => map.MapFrom(b => b.Unit))
                .ForMember(a => a.Category, (map) => map.MapFrom(b => b.Classify))
                .ForMember(a => a.Description, (map) => map.MapFrom(b => b.TaskDesc))
                .ForMember(a => a.Starttime, (map) => map.MapFrom(b => b.Begin))
                .ForMember(a => a.Endtime, (map) => map.MapFrom(b => b.End))
                .ForMember(a => a.Sgroupcolor, (map) => map.MapFrom(b => b.GroupColor))
                .ForMember(a => a.Stampimagetype, (map) => map.MapFrom(b => b.StampImageType))
                .ForMember(a => a.Taskpriority, (map) => map.MapFrom(b => b.Priority))
                .ForMember(a => a.Backtype, (map) => map.MapFrom(b => b.CooperantType));

            CreateMap<DbpTask, TaskContentResponse>()
                .ForMember(a => a.Unit, (map) => map.MapFrom(b => b.Recunitid))
                .ForMember(a => a.Classify, (map) => map.MapFrom(b => b.Category))
                .ForMember(a => a.TaskDesc, (map) => map.MapFrom(b => b.Description))
                .ForMember(a => a.Begin, (map) => map.MapFrom(b => b.Starttime))
                .ForMember(a => a.End, (map) => map.MapFrom(b => b.Endtime))
                .ForMember(a => a.GroupColor, (map) => map.MapFrom(b => b.Sgroupcolor))
                .ForMember(a => a.StampImageType, (map) => map.MapFrom(b => b.Stampimagetype))
                .ForMember(a => a.Priority, (map) => map.MapFrom(b => b.Taskpriority))
                .ForMember(a => a.CooperantType, (map) => map.MapFrom(b => b.Backtype));


            CreateMap<AddTaskSvr_IN, TaskContentResponse>();


            //ReverseMap
        }
    }
}
