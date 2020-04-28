using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using IngestTaskPlugin.Dto.Request;
using IngestTaskPlugin.Dto.Response;
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

            //V1
            CreateMap<VtrTapelist, VTRTapeInfo>()
               .ForMember(a => a.nTapeID, (map) => map.MapFrom(b => b.Tapeid))
               .ForMember(a => a.strTapeName, (map) => map.MapFrom(b => b.Tapename))
               .ForMember(a => a.strTapeDesc, (map) => map.MapFrom(b => b.Tapedesc));
            #endregion

            #region VtrUploadtask To VTRUploadTaskInfo
            //V2

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
        }
    }
}
