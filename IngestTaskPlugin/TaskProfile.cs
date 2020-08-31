using AutoMapper;
using IngestDBCore.Tool;
using IngestTaskPlugin.Dto;
using IngestTaskPlugin.Dto.OldResponse;
using IngestTaskPlugin.Dto.Response;
using IngestTaskPlugin.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace IngestTaskPlugin
{
    public class DateTimeTypeConverter : ITypeConverter<string, DateTime>
    {
        public DateTime Convert(string source, DateTime destination, ResolutionContext context)
        {
            return DateTimeFormat.DateTimeFromString(source);
        }
    }

    public class DateTimeStringTypeConverter : ITypeConverter<DateTime, string>
    {
        public string Convert(DateTime source, string destination, ResolutionContext context)
        {
            return DateTimeFormat.DateTimeToString(source);
        }
    }
    public class TaskProfile : Profile
    {
        
        public TaskProfile()
        {
            CreateMap<string, DateTime>().ConvertUsing(new DateTimeTypeConverter());
            CreateMap<DateTime, string>().ConvertUsing(new DateTimeStringTypeConverter());

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

            CreateMap<DbpTask, TaskContent>()
                .ForMember(a => a.nTaskID, (y) => y.MapFrom( b => b.Taskid))
                .ForMember(a => a.strTaskName, (y) => y.MapFrom(b => b.Taskname))
                .ForMember(a => a.strTaskDesc, (y) => y.MapFrom(b => b.Description))
                .ForMember(a => a.strClassify, (y) => y.MapFrom(b => b.Category))
                .ForMember(a => a.nChannelID, (y) => y.MapFrom(b => b.Channelid))
                .ForMember(a => a.nUnit, (y) => y.MapFrom(b => b.Recunitid))
                .ForMember(a => a.strUserCode, (y) => y.MapFrom(b => b.Usercode))
                .ForMember(a => a.nSignalID, (y) => y.MapFrom(b => b.Signalid))
                .ForMember(a => a.strBegin, (y) => y.MapFrom(b => b.Starttime))
                .ForMember(a => a.strEnd, (y) => y.MapFrom(b => b.Endtime))

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
                .ForMember(x => x.TaskId, (y) => y.MapFrom(z => z.nTaskID))
                .ForMember(x => x.TaskName, (y) => y.MapFrom(z => z.strTaskName))
                .ForMember(x => x.TaskDesc, (y) => y.MapFrom(z => z.strTaskDesc))
                .ForMember(x => x.Classify, (y) => y.MapFrom(z => z.strClassify))
                .ForMember(x => x.ChannelId, (y) => y.MapFrom(z => z.nChannelID))
                .ForMember(x => x.Unit, (y) => y.MapFrom(z => z.nUnit))
                .ForMember(x => x.UserCode, (y) => y.MapFrom(z => z.strUserCode))
                .ForMember(x => x.SignalId, (y) => y.MapFrom(z => z.nSignalID))
                .ForMember(x => x.Begin, (y) => y.MapFrom(z => z.strBegin))
                .ForMember(x => x.End, (y) => y.MapFrom(z => z.strEnd))
                .ForMember(x => x.TaskType, (y) => y.MapFrom(z => z.emTaskType))
                .ForMember(x => x.CooperantType, (y) => y.MapFrom(z => z.emCooperantType))
                .ForMember(x => x.State, (y) => y.MapFrom(z => z.emState))
                .ForMember(x => x.StampImage, (y) => y.MapFrom(z => z.strStampImage))
                .ForMember(x => x.TaskGuid, (y) => y.MapFrom(z => z.strTaskGUID))
                .ForMember(x => x.BackupVtrId, (y) => y.MapFrom(z => z.nBackupVTRID))
                .ForMember(x => x.Priority, (y) => y.MapFrom(z => z.emPriority))
                .ForMember(x => x.StampTitleIndex, (y) => y.MapFrom(z => z.nStampTitleIndex))
                .ForMember(x => x.StampImageType, (y) => y.MapFrom(z => z.nStampImageType))
                .ForMember(x => x.GroupColor, (y) => y.MapFrom(z => z.nSGroupColor)).ReverseMap();

            CreateMap<AddTaskExDb_in, TaskInfoResponse>()
                .ForMember(d => d.TaskContent, y => y.MapFrom(s => s.taskAdd))
                .AfterMap((a, b) =>
                {
                    b.TaskSource = TaskSource.emMSVUploadTask;
                    b.BackUpTask = false;
                });

            CreateMap<DbpTask, TaskInfoRescheduled>()
                .ForMember(d => d.nTaskID, y => y.MapFrom(s => s.Taskid))
                .ForMember(d => d.strTaskName, y => y.MapFrom(s => s.Taskname))
                .ForMember(d => d.nPreviousChannelID, y => y.MapFrom(s => s.OldChannelid))
                .ForMember(d => d.nCurrentChannelID, y => y.MapFrom(s => s.Channelid));

            CreateMap<DbpTask, RescheduledTaskInfoResponse>()
                .ForMember(d => d.TaskId, y => y.MapFrom(s => s.Taskid))
                .ForMember(d => d.TaskName, y => y.MapFrom(s => s.Taskname))
                .ForMember(d => d.PreviousChannelId, y => y.MapFrom(s => s.OldChannelid))
                .ForMember(d => d.CurrentChannelId, y => y.MapFrom(s => s.Channelid));

            CreateMap<AddTaskSvr_IN, TaskInfoResponse>() 
                .ForMember(d => d.TaskContent, y => y.MapFrom(s => s.taskAdd))
                .ForMember(d => d.TaskSource, y => y.MapFrom(s => s.taskSrc));

            CreateMap<AddReScheduleTaskSvr_in, TaskInfoResponse>()
                .ForMember(d => d.TaskContent, y => y.MapFrom(s => s.taskAdd))
                .ForMember(d => d.TaskSource, y => y.MapFrom(s => s.taskSrc));

            CreateMap<AddTaskSvrPolicysAndBackupFlag_IN, TaskInfoResponse>()
               .ForMember(d => d.TaskContent, y => y.MapFrom(s => s.taskAdd))
               .ForMember(d => d.TaskSource, y => y.MapFrom(s => s.taskSrc));
            
            CreateMap<DbpTask, DbpTaskBackup>().ReverseMap();

            CreateMap<DbpTask, TaskFullInfo>()
                .ForMember(d => d.taskContent, y => y.MapFrom(s => s))
                .ForPath(d => d.nOldChannelID, y => y.MapFrom(s => s.OldChannelid))
                .ForPath(d => d.emDispatchState, y => y.MapFrom(s => s.DispatchState))
                .ForPath(d => d.emSyncState, y => y.MapFrom(s => s.SyncState))
                .ForPath(d => d.emOpType, y => y.MapFrom(s => s.OpType))
                .ForPath(d => d.strNewBeginTime, y => y.MapFrom(s => s.NewBegintime))
                .ForPath(d => d.strNewEndTime, y => y.MapFrom(s => s.NewEndtime))
                .ForPath(d => d.strTaskLock, y => y.MapFrom(s => s.Tasklock));

            CreateMap<DbpTaskMetadata, MetadataPair>()
                .ForMember(x => x.emtype, (y) => y.MapFrom(z => z.Metadatatype))
                .ForMember(x => x.nTaskID, (y) => y.MapFrom(z => z.Taskid))
                .ForMember(x => x.strMetadata, (y) => y.MapFrom(z => z.Metadatalong));

            CreateMap<CompleteSynTasks_IN, CompleteSyncTaskRequest>()
                .ForMember(x => x.IsFinish, (y) => y.MapFrom(z => z.bIsFinish))
                .ForMember(x => x.Perodic2Next, (y) => y.MapFrom(z => z.bPerodic2Next))
                .ForMember(x => x.TaskID, (y) => y.MapFrom(z => z.nTaskID))
                .ForMember(x => x.TaskState, (y) => y.MapFrom(z => z.nTaskState))
                .ForMember(x => x.DispatchState, (y) => y.MapFrom(z => z.nDispatchState))
                .ForMember(x => x.SynState, (y) => y.MapFrom(z => z.nSynState)).ReverseMap();

            CreateMap<WarningInfo, WarningInfoResponse>()
                .ForMember(x => x.RelatedId, (y) => y.MapFrom(z => z.nRelatedID))
                .ForMember(x => x.TaskId, (y) => y.MapFrom(z => z.nTaskID))
                .ForMember(x => x.WarningLevel, (y) => y.MapFrom(z => z.nWarningLevel))
                .ForMember(x => x.WarningMessage, (y) => y.MapFrom(z => z.strWarningMessage)).ReverseMap();

            CreateMap<TaskErrorInfoResponse, DbpTaskErrorinfo>().ReverseMap();
            //ReverseMap
        }
    }
}
