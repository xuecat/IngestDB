using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using IngestTaskPlugin.Dto.OldResponse;
using IngestTaskPlugin.Dto.Request;
using IngestTaskPlugin.Dto.Response;
using IngestTaskPlugin.Dto.Response.OldVtr;
using IngestTaskPlugin.Extend;
using IngestTaskPlugin.Models;

namespace IngestTaskPlugin
{
    public class VtrProfile : Profile
    {
        public VtrProfile()
        {
            #region VtrTapelist To VTRTapeInfo
            //V2

            CreateMap<VtrBatchUploadTaskResponse, VtrBatchUploadTask>().ReverseMap();

            //V1
            CreateMap<VtrTapelist, VTRTapeInfo>()
               .ForMember(a => a.nTapeID, (map) => map.MapFrom(b => b.Tapeid))
               .ForMember(a => a.strTapeName, (map) => map.MapFrom(b => b.Tapename))
               .ForMember(a => a.strTapeDesc, (map) => map.MapFrom(b => b.Tapedesc));

            CreateMap<VtrTapelist, VTRTapeInfoResponse>()
               .ForMember(a => a.TapeID, (map) => map.MapFrom(b => b.Tapeid))
               .ForMember(a => a.TapeName, (map) => map.MapFrom(b => b.Tapename))
               .ForMember(a => a.TapeDesc, (map) => map.MapFrom(b => b.Tapedesc));
            #endregion

            #region VtrUploadtask To VTRUploadTaskInfo、 VTRUploadTaskInfoResponse
            //V2
            CreateMap<VtrUploadtask, VTRUploadTaskInfoResponse>()
               .ForMember(a => a.VtrTaskId, (map) => map.MapFrom(b => b.Taskid))
               .ForMember(a => a.BlankTaskId, (map) => map.MapFrom(b => b.Vtrtaskid))
               .ForMember(a => a.VtrId, (map) => map.MapFrom(b => b.Vtrid))
               .ForMember(a => a.ChannelId, (map) => map.MapFrom(b => b.Recchannelid))
               .ForMember(a => a.TrimIn, (map) => map.MapFrom(b => b.Trimin))
               .ForMember(a => a.TrimOut, (map) => map.MapFrom(b => b.Trimout))
               .ForMember(a => a.SignalId, (map) => map.MapFrom(b => b.Signalid))
               .ForMember(a => a.TaskState, (map) => map.MapFrom(b => b.Taskstate))
               .ForMember(a => a.UserCode, (map) => map.MapFrom(b => b.Usercode ?? ""))
               .ForMember(a => a.CommitTime, (map) => map.MapFrom(b => b.Committime))
               .ForMember(a => a.Order, (map) => map.MapFrom(b => b.Uploadorder))
               .ForMember(a => a.TaskGUID, (map) => map.MapFrom(b => b.Taskguid ?? ""))
               .ForMember(a => a.TaskName, (map) => map.MapFrom(b => b.Taskname))
               .ForMember(a => a.UserToken, (map) => map.MapFrom(b => b.Usertoken ?? "Sobey"))
               .ForMember(a => a.TapeId, (map) => map.MapFrom(b => b.Tapeid))
               .ForMember(a => a.TrimInCTL, (map) => map.MapFrom(b => b.Triminctl))
               .ForMember(a => a.TrimOutCTL, (map) => map.MapFrom(b => b.Trimoutctl))
               .ForMember(a => a.VtrTaskType, (map) => map.MapFrom(b => b.Vtrtasktype)).ReverseMap();
            //V1
            CreateMap<VtrUploadtask, VTRUploadTaskInfo>()
               .ForMember(a => a.nVTRTaskID, (map) => map.MapFrom(b => b.Taskid))
               .ForMember(a => a.nBlankTaskID, (map) => map.MapFrom(b => b.Vtrtaskid))
               .ForMember(a => a.nVtrID, (map) => map.MapFrom(b => b.Vtrid))
               .ForMember(a => a.nChannelID, (map) => map.MapFrom(b => b.Recchannelid))
               .ForMember(a => a.nTrimIn, (map) => map.MapFrom(b => b.Trimin))
               .ForMember(a => a.nTrimOut, (map) => map.MapFrom(b => b.Trimout))
               .ForMember(a => a.nSignalID, (map) => map.MapFrom(b => b.Signalid))
               .ForMember(a => a.emTaskState, (map) => map.MapFrom(b => b.Taskstate))
               .ForMember(a => a.strUserCode, (map) => map.MapFrom(b => b.Usercode ?? ""))
               .ForMember(a => a.strCommitTime, (map) => map.MapFrom(b => b.Committime.ToStr()))
               .ForMember(a => a.nOrder, (map) => map.MapFrom(b => b.Uploadorder))
               .ForMember(a => a.strTaskGUID, (map) => map.MapFrom(b => b.Taskguid ?? ""))
               .ForMember(a => a.strTaskName, (map) => map.MapFrom(b => b.Taskname))
               .ForMember(a => a.strUserToken, (map) => map.MapFrom(b => b.Usertoken ?? "Sobey"))
               .ForMember(a => a.nTapeID, (map) => map.MapFrom(b => b.Tapeid))
               .ForMember(a => a.nTrimInCTL, (map) => map.MapFrom(b => b.Triminctl))
               .ForMember(a => a.nTrimOutCTL, (map) => map.MapFrom(b => b.Trimoutctl))
               .ForMember(a => a.emVtrTaskType, (map) => map.MapFrom(b => b.Vtrtasktype)).ReverseMap();
            #endregion

            #region VtrDetailinfo To VTRDetailInfo、VTRDetailInfoResponse
            //V2
            CreateMap<VtrDetailinfo, VTRDetailInfoResponse>()
               .ForMember(a => a.VtrId, (map) => map.MapFrom(b => b.Vtrid))
               .ForMember(a => a.VtrTypeId, (map) => map.MapFrom(b => b.Vtrtypeid))
               .ForMember(a => a.VtrSubTypeId, (map) => map.MapFrom(b => b.Vtrsubtype))
               .ForMember(a => a.VtrDetailName, (map) => map.MapFrom(b => b.Vtrname))
               .ForMember(a => a.VtrDetailDesc, (map) => map.MapFrom(b => b.Vtrddescribe))
               .ForMember(a => a.VtrVComPortIdx, (map) => map.MapFrom(b => b.Vtrcomport))
               .ForMember(a => a.LoopFlag, (map) => map.MapFrom(b => b.Looprecord))
               .ForMember(a => a.ServerIP, (map) => map.MapFrom(b => b.Vtrserverip))
               .ForMember(a => a.PreRolFrame, (map) => map.MapFrom(b => b.Prerolframenum))
               .ForMember(a => a.BaudRate, (map) => map.MapFrom(b => b.Baudrate))
               .ForMember(a => a.BackUpType, (map) => map.MapFrom(b => b.Backuptype))
               .ForMember(a => a.VtrState, (map) => map.MapFrom(b => b.Vtrstate))
               .ForMember(a => a.FrameRate, (map) => map.MapFrom(b => b.Framerate))
               .ForMember(a => a.WorkMode, (map) => map.MapFrom(b => b.Workmode))
               .ForMember(a => a.VtrSignalType, (map) => map.MapFrom(b => b.Vtrsignaltype));
            //V1
            CreateMap<VtrDetailinfo, VTRDetailInfo>()
               .ForMember(a => a.nVTRID, (map) => map.MapFrom(b => b.Vtrid))
               .ForMember(a => a.lVTRTypeID, (map) => map.MapFrom(b => b.Vtrtypeid))
               .ForMember(a => a.lVTRSubTypeID, (map) => map.MapFrom(b => b.Vtrsubtype))
               .ForMember(a => a.szVTRDetailName, (map) => map.MapFrom(b => b.Vtrname))
               .ForMember(a => a.szVTRDetailDesc, (map) => map.MapFrom(b => b.Vtrddescribe))
               .ForMember(a => a.nVTRVComPortIdx, (map) => map.MapFrom(b => b.Vtrcomport))
               .ForMember(a => a.nLoopFlag, (map) => map.MapFrom(b => b.Looprecord))
               .ForMember(a => a.szServerIP, (map) => map.MapFrom(b => b.Vtrserverip))
               .ForMember(a => a.nPreRolFrame, (map) => map.MapFrom(b => b.Prerolframenum))
               .ForMember(a => a.nBaudRate, (map) => map.MapFrom(b => b.Baudrate))
               .ForMember(a => a.emBackUpType, (map) => map.MapFrom(b => b.Backuptype))
               .ForMember(a => a.emVtrState, (map) => map.MapFrom(b => b.Vtrstate))
               .ForMember(a => a.dbFrameRate, (map) => map.MapFrom(b => b.Framerate))
               .ForMember(a => a.emWorkMode, (map) => map.MapFrom(b => b.Workmode))
               .ForMember(a => a.emVtrSignalType, (map) => map.MapFrom(b => b.Vtrsignaltype));
            #endregion

            #region VtrUploadtask To VTRUploadTaskContent、VTRUploadTaskContentResponse
            //V2
            CreateMap<VtrUploadtask, VTRUploadTaskContentResponse>()
               .ForMember(a => a.TaskId, (map) => map.MapFrom(b => b.Taskid))
               .ForMember(a => a.BlankTaskId, (map) => map.MapFrom(b => b.Vtrtaskid))
               .ForMember(a => a.VtrId, (map) => map.MapFrom(b => b.Vtrid))
               .ForMember(a => a.ChannelId, (map) => map.MapFrom(b => b.Recchannelid))
               .ForMember(a => a.TrimIn, (map) => map.MapFrom(b => b.Trimin))
               .ForMember(a => a.TrimOut, (map) => map.MapFrom(b => b.Trimout))
               .ForMember(a => a.SignalId, (map) => map.MapFrom(b => b.Signalid))
               .ForMember(a => a.TaskState, (map) => map.MapFrom(b => b.Taskstate))
               .ForMember(a => a.TapeId, (map) => map.MapFrom(b => b.Tapeid))
               .ForMember(a => a.TaskName, (map) => map.MapFrom(b => b.Taskname))
               .ForMember(a => a.UserCode, (map) => map.MapFrom(b => b.Usercode ?? ""))
               .ForMember(a => a.CommitTime, (map) => map.MapFrom(b => b.Committime.ToString("yyyy-MM-dd HH:mm:ss")))
               .ForMember(a => a.Order, (map) => map.MapFrom(b => b.Uploadorder))
               .ForMember(a => a.TaskGUID, (map) => map.MapFrom(b => b.Taskguid ?? ""))
               .ForMember(a => a.UserToken, (map) => map.MapFrom(b => b.Usertoken ?? "Sobey"))
               .ForMember(a => a.TrimInCTL, (map) => map.MapFrom(b => b.Triminctl))
               .ForMember(a => a.TrimOutCTL, (map) => map.MapFrom(b => b.Trimoutctl))
               .ForMember(a => a.VtrTaskType, (map) => map.MapFrom(b => b.Vtrtasktype)).ReverseMap();

            CreateMap<VTRUploadTaskContentResponse, VtrUploadtask>()
               .ForMember(a => a.Taskid, (map) => map.MapFrom(b => b.TaskId))
               .ForMember(a => a.Vtrtaskid, (map) => map.MapFrom(b => b.BlankTaskId))
               .ForMember(a => a.Vtrid, (map) => map.MapFrom(b => b.VtrId))
               .ForMember(a => a.Recchannelid, (map) => map.MapFrom(b => b.ChannelId))
               .ForMember(a => a.Trimin, (map) => map.MapFrom(b => b.TrimIn))
               .ForMember(a => a.Trimout, (map) => map.MapFrom(b => b.TrimOut))
               .ForMember(a => a.Signalid, (map) => map.MapFrom(b => b.SignalId))
               .ForMember(a => a.Taskstate, (map) => map.MapFrom(b => b.TaskState))
               .ForMember(a => a.Tapeid, (map) => map.MapFrom(b => b.TapeId))
               .ForMember(a => a.Taskname, (map) => map.MapFrom(b => b.TaskName))
               .ForMember(a => a.Usercode, (map) => map.MapFrom(b => b.UserCode))
               .ForMember(a => a.Committime, (map) => map.MapFrom(b => Convert.ToDateTime(b.CommitTime)))
               .ForMember(a => a.Uploadorder, (map) => map.MapFrom(b => b.Order))
               .ForMember(a => a.Taskguid, (map) => map.MapFrom(b => b.TaskGUID))
               .ForMember(a => a.Usertoken, (map) => map.MapFrom(b => b.UserCode))
               .ForMember(a => a.Triminctl, (map) => map.MapFrom(b => b.TrimInCTL))
               .ForMember(a => a.Trimoutctl, (map) => map.MapFrom(b => b.TrimOutCTL))
               .ForMember(a => a.Vtrtasktype, (map) => map.MapFrom(b => b.VtrTaskType));
            //V1
            CreateMap<VtrUploadtask, VTRUploadTaskContent>()
               .ForMember(a => a.nTaskId, (map) => map.MapFrom(b => b.Taskid))
               .ForMember(a => a.nBlankTaskId, (map) => map.MapFrom(b => b.Vtrtaskid))
               .ForMember(a => a.nVtrId, (map) => map.MapFrom(b => b.Vtrid))
               .ForMember(a => a.nChannelId, (map) => map.MapFrom(b => b.Recchannelid))
               .ForMember(a => a.nTrimIn, (map) => map.MapFrom(b => b.Trimin))
               .ForMember(a => a.nTrimOut, (map) => map.MapFrom(b => b.Trimout))
               .ForMember(a => a.nSignalId, (map) => map.MapFrom(b => b.Signalid))
               .ForMember(a => a.emTaskState, (map) => map.MapFrom(b => b.Taskstate))
               .ForMember(a => a.nTapeId, (map) => map.MapFrom(b => b.Tapeid))
               .ForMember(a => a.strTaskName, (map) => map.MapFrom(b => b.Taskname))
               .ForMember(a => a.strUserCode, (map) => map.MapFrom(b => b.Usercode ?? ""))
               .ForMember(a => a.strCommitTime, (map) => map.MapFrom(b => b.Committime.ToString("yyyy-MM-dd HH:mm:ss")))
               .ForMember(a => a.nOrder, (map) => map.MapFrom(b => b.Uploadorder))
               .ForMember(a => a.strTaskGUID, (map) => map.MapFrom(b => b.Taskguid ?? ""))
               .ForMember(a => a.strUserToken, (map) => map.MapFrom(b => b.Usertoken ?? "Sobey"))
               .ForMember(a => a.nTrimInCTL, (map) => map.MapFrom(b => b.Triminctl))
               .ForMember(a => a.nTrimOutCTL, (map) => map.MapFrom(b => b.Trimoutctl))
               .ForMember(a => a.emVtrTaskType, (map) => map.MapFrom(b => b.Vtrtasktype));
            #endregion

            #region VTRUploadTaskContentResponse To VTRUploadTaskContent
            CreateMap<VTRUploadTaskContentResponse, VTRUploadTaskContent>()
               .ForMember(a => a.nTaskId, (map) => map.MapFrom(b => b.TaskId))
               .ForMember(a => a.nVtrId, (map) => map.MapFrom(b => b.VtrId))
               .ForMember(a => a.nBlankTaskId, (map) => map.MapFrom(b => b.BlankTaskId))
               .ForMember(a => a.emVtrTaskType, (map) => map.MapFrom(b => b.VtrTaskType))
               .ForMember(a => a.nChannelId, (map) => map.MapFrom(b => b.ChannelId))
               .ForMember(a => a.nTrimIn, (map) => map.MapFrom(b => b.TrimIn))
               .ForMember(a => a.nTrimOut, (map) => map.MapFrom(b => b.TrimOut))
               .ForMember(a => a.nSignalId, (map) => map.MapFrom(b => b.SignalId))
               .ForMember(a => a.emTaskState, (map) => map.MapFrom(b => b.TaskState))
               .ForMember(a => a.strUserCode, (map) => map.MapFrom(b => b.UserCode ?? ""))
               .ForMember(a => a.strCommitTime, (map) => map.MapFrom(b => b.CommitTime))
               .ForMember(a => a.nOrder, (map) => map.MapFrom(b => b.Order))
               .ForMember(a => a.strTaskGUID, (map) => map.MapFrom(b => b.TaskGUID ?? ""))
               .ForMember(a => a.nTapeId, (map) => map.MapFrom(b => b.TapeId))
               .ForMember(a => a.strTaskName, (map) => map.MapFrom(b => b.TaskName))
               .ForMember(a => a.strUserToken, (map) => map.MapFrom(b => b.UserToken ?? "Sobey"))
               .ForMember(a => a.nTrimInCTL, (map) => map.MapFrom(b => b.TrimInCTL))
               .ForMember(a => a.strTaskDesc, (map) => map.MapFrom(b => b.TaskDesc))
               .ForMember(a => a.strClassify, (map) => map.MapFrom(b => b.Classify))
               .ForMember(a => a.nUnit, (map) => map.MapFrom(b => b.Unit))
               .ForMember(a => a.strBegin, (map) => map.MapFrom(b => b.BeginTime))
               .ForMember(a => a.strEnd, (map) => map.MapFrom(b => b.EndTime))
               .ForMember(a => a.emTaskType, (map) => map.MapFrom(b => b.TaskType))
               .ForMember(a => a.emCooperantType, (map) => map.MapFrom(b => b.CooperantType))
               .ForMember(a => a.emState, (map) => map.MapFrom(b => b.State))
               .ForMember(a => a.strStampImage, (map) => map.MapFrom(b => b.StampImage)).ReverseMap();
            #endregion

            #region VTRUploadCondition To VTRUploadConditionRequest
            CreateMap<VTRUploadCondition, VTRUploadConditionRequest>()
               .ForMember(a => a.UserCode, (map) => map.MapFrom(b => b.szUserCode))
               .ForMember(a => a.TaskId, (map) => map.MapFrom(b => b.lTaskID))
               .ForMember(a => a.TaskState, (map) => map.MapFrom(b => b.state))
               .ForMember(a => a.BlankTaskId, (map) => map.MapFrom(b => b.lBlankTaskID))
               .ForMember(a => a.VtrId, (map) => map.MapFrom(b => b.lVtrID))
               .ForMember(a => a.MaxCommitTime, (map) => map.MapFrom(b => b.strMaxCommitTime))
               .ForMember(a => a.MinCommitTime, (map) => map.MapFrom(b => b.strMinCommitTime))
               .ForMember(a => a.TaskName, (map) => map.MapFrom(b => b.strTaskName))
               .ForMember(a => a.UserToken, (map) => map.MapFrom(b => b.strUserToken))
               .ForMember(a => a.TaskType, (map) => map.MapFrom(b => b.nTaskType));
            #endregion

            #region VTRUploadTaskInfo To VTRUploadTaskInfoResponse
            CreateMap<VTRUploadTaskInfo, VTRUploadTaskInfoResponse>()
               .ForMember(a => a.VtrTaskId, (map) => map.MapFrom(b => b.nVTRTaskID))
               .ForMember(a => a.BlankTaskId, (map) => map.MapFrom(b => b.nBlankTaskID))
               .ForMember(a => a.VtrId, (map) => map.MapFrom(b => b.nVtrID))
               .ForMember(a => a.ChannelId, (map) => map.MapFrom(b => b.nChannelID))
               .ForMember(a => a.TrimIn, (map) => map.MapFrom(b => b.nTrimIn))
               .ForMember(a => a.TrimOut, (map) => map.MapFrom(b => b.nTrimOut))
               .ForMember(a => a.SignalId, (map) => map.MapFrom(b => b.nSignalID))
               .ForMember(a => a.TaskState, (map) => map.MapFrom(b => b.emTaskState))
               .ForMember(a => a.UserCode, (map) => map.MapFrom(b => b.strUserCode ?? ""))
               .ForMember(a => a.CommitTime, (map) => map.MapFrom(b => b.strCommitTime))
               .ForMember(a => a.Order, (map) => map.MapFrom(b => b.nOrder))
               .ForMember(a => a.TaskGUID, (map) => map.MapFrom(b => b.strTaskGUID ?? ""))
               .ForMember(a => a.TaskName, (map) => map.MapFrom(b => b.strTaskName))
               .ForMember(a => a.UserToken, (map) => map.MapFrom(b => b.strUserToken ?? "Sobey"))
               .ForMember(a => a.TapeId, (map) => map.MapFrom(b => b.nTapeID))
               .ForMember(a => a.TrimInCTL, (map) => map.MapFrom(b => b.nTrimInCTL))
               .ForMember(a => a.TrimOutCTL, (map) => map.MapFrom(b => b.nTrimOutCTL))
               .ForMember(a => a.VtrTaskType, (map) => map.MapFrom(b => b.emVtrTaskType)).ReverseMap();
            #endregion

            CreateMap<VTRUploadTaskRequest, SetVTRUploadTask_in>()
               .ForMember(a => a.vtrTask, (map) => map.MapFrom(b => b.VtrTask))
               .ForMember(a => a.metadatas, (map) => map.MapFrom(b => b.Metadatas))
               .ForMember(a => a.lMask, (map) => map.MapFrom(b => b.Mask))
               .ForMember(a => a.uploadTaskMask, (map) => map.MapFrom(b => b.UploadTaskMask));

            CreateMap<VTRUploadMetadataPair, VTR_UPLOAD_MetadataPair>()
               .ForMember(a => a.nTaskID, (map) => map.MapFrom(b => b.TaskId))
               .ForMember(a => a.strMetadata, (map) => map.MapFrom(b => b.Metadata))
               .ForMember(a => a.emType, (map) => map.MapFrom(b => b.Type));

            CreateMap<AddVTRUploadTask_out, AddVTRUploadTaskResponse>()
               .ForMember(a => a.VtrTask, (map) => map.MapFrom(b => b.vtrTask))
               .ForMember(a => a.ErrorCode, (map) => map.MapFrom(b => b.errorCode));

            //CreateMap<AddVTRUploadTask_out, VTRUploadTaskContentResponse>()
            //   .ForMember(a => a, (map) => map.MapFrom(b => b.vtrTask));

            //CreateMap<VTRUPLOADMetadataPairRequest, VTR_UPLOAD_MetadataPair>()
            //   .ForMember(a => a.nTaskID, (map) => map.MapFrom(b => b.taskid))
            //   .ForMember(a => a.strMetadata, (map) => map.MapFrom(b => b.metadata))
            //   .ForMember(a => a.emType, (map) => map.MapFrom(b => b.type));
        }
    }
}
