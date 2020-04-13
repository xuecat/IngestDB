using AutoMapper;
using IngestDBCore.Tool;
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

            CreateMap<TaskContentResponse, DbpTask>()
                .ForMember(a => a.Recunitid, (map) => map.MapFrom(b => b.Unit))
                .ForMember(a => a.Category, (map) => map.MapFrom(b => b.Classify))
                .ForMember(a => a.Description, (map) => map.MapFrom(b => b.TaskDesc))
                .ForMember(a => a.Starttime, (map) => map.MapFrom(b => DateTimeFormat.DateTimeFromString(b.Begin)))
                .ForMember(a => a.Endtime, (map) => map.MapFrom(b => DateTimeFormat.DateTimeFromString(b.End)))
                .ForMember(a => a.Sgroupcolor, (map) => map.MapFrom(b => b.GroupColor))
                .ForMember(a => a.Stampimagetype, (map) => map.MapFrom(b => b.StampImageType))
                .ForMember(a => a.Taskpriority, (map) => map.MapFrom(b => b.Priority))
                .ForMember(a => a.Backtype, (map) => map.MapFrom(b => b.CooperantType));

            CreateMap<DbpTask, TaskContentResponse>()
                .ForMember(a => a.Unit, (map) => map.MapFrom(b => b.Recunitid))
                .ForMember(a => a.Classify, (map) => map.MapFrom(b => b.Category))
                .ForMember(a => a.TaskDesc, (map) => map.MapFrom(b => b.Description))
                .ForMember(a => a.Begin, (map) => map.MapFrom(b => DateTimeFormat.DateTimeToString(b.Starttime)))
                .ForMember(a => a.End, (map) => map.MapFrom(b => DateTimeFormat.DateTimeToString(b.Endtime)))
                .ForMember(a => a.GroupColor, (map) => map.MapFrom(b => b.Sgroupcolor))
                .ForMember(a => a.StampImageType, (map) => map.MapFrom(b => b.Stampimagetype))
                .ForMember(a => a.Priority, (map) => map.MapFrom(b => b.Taskpriority))
                .ForMember(a => a.CooperantType, (map) => map.MapFrom(b => b.Backtype));

            CreateMap<DbpTask, TaskContent>()
                .ForMember(a => a.nTaskID, (y) => y.MapFrom( b => b.Taskid))
                .ForMember(a => a.strTaskName, (y) => y.MapFrom(b => b.Taskname))
                .ForMember(a => a.strTaskDesc, (y) => y.MapFrom(b => b.Description))
                .ForMember(a => a.strClassify, (y) => y.MapFrom(b => b.Category))
                .ForMember(a => a.nChannelID, (y) => y.MapFrom(b => b.Channelid))
                .ForMember(a => a.nUnit, (y) => y.MapFrom(b => b.Recunitid))
                .ForMember(a => a.strUserCode, (y) => y.MapFrom(b => b.Usercode))
                .ForMember(a => a.nSignalID, (y) => y.MapFrom(b => b.Signalid))
                .ForMember(a => a.strBegin, (y) => y.MapFrom(b => DateTimeFormat.DateTimeToString(b.Starttime)))
                .ForMember(a => a.strEnd, (y) => y.MapFrom(b => DateTimeFormat.DateTimeToString(b.Endtime)))

                .ForMember(a => a.emTaskType, (y) => y.MapFrom(b => b.Tasktype))
                .ForMember(a => a.emCooperantType, (y) => y.MapFrom(b => b.Backtype))
                .ForMember(a => a.emState, (y) => y.MapFrom(b => b.State))
                .ForMember(a => a.strStampImage, (y) => y.MapFrom(b => b.Description))
                .ForMember(a => a.strTaskGUID, (y) => y.MapFrom(b => b.Taskguid))
                .ForMember(a => a.nBackupVTRID, (y) => y.MapFrom(b => b.Backupvtrid))

                .ForMember(a => a.emPriority, (y) => y.MapFrom(b => b.Taskpriority))
                .ForMember(a => a.nStampTitleIndex, (y) => y.MapFrom(b => b.Stamptitleindex))
                .ForMember(a => a.nStampImageType, (y) => y.MapFrom(b => b.Stampimagetype))
                .ForMember(a => a.nSGroupColor, (y) => y.MapFrom(b => b.Sgroupcolor));

            CreateMap<TaskContent, TaskContentResponse>()
                .ForMember(x => x.TaskID, (y) => y.MapFrom(z => z.nTaskID))
                .ForMember(x => x.TaskName, (y) => y.MapFrom(z => z.strTaskName))
                .ForMember(x => x.TaskDesc, (y) => y.MapFrom(z => z.strTaskDesc))
                .ForMember(x => x.Classify, (y) => y.MapFrom(z => z.strClassify))
                .ForMember(x => x.ChannelID, (y) => y.MapFrom(z => z.nChannelID))
                .ForMember(x => x.Unit, (y) => y.MapFrom(z => z.nUnit))
                .ForMember(x => x.UserCode, (y) => y.MapFrom(z => z.strUserCode))
                .ForMember(x => x.SignalID, (y) => y.MapFrom(z => z.nSignalID))
                .ForMember(x => x.Begin, (y) => y.MapFrom(z => z.strBegin))
                .ForMember(x => x.End, (y) => y.MapFrom(z => z.strEnd))
                .ForMember(x => x.TaskType, (y) => y.MapFrom(z => z.emTaskType))
                .ForMember(x => x.CooperantType, (y) => y.MapFrom(z => z.emCooperantType))
                .ForMember(x => x.State, (y) => y.MapFrom(z => z.emState))
                .ForMember(x => x.StampImage, (y) => y.MapFrom(z => z.strStampImage))
                .ForMember(x => x.TaskGUID, (y) => y.MapFrom(z => z.strTaskGUID))
                .ForMember(x => x.BackupVTRID, (y) => y.MapFrom(z => z.nBackupVTRID))
                .ForMember(x => x.Priority, (y) => y.MapFrom(z => z.emPriority))
                .ForMember(x => x.StampTitleIndex, (y) => y.MapFrom(z => z.nStampTitleIndex))
                .ForMember(x => x.StampImageType, (y) => y.MapFrom(z => z.nStampImageType))
                .ForMember(x => x.GroupColor, (y) => y.MapFrom(z => z.nSGroupColor));
            CreateMap<AddTaskSvr_IN, TaskInfoResponse>() 
                .ForMember(d => d.TaskContent, y => y.MapFrom(s => s.taskAdd))
                .ForMember(d => d.TaskSource, y => y.MapFrom(s => s.taskSrc));


            //ReverseMap
        }
    }
}
