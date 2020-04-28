using AutoMapper;
using IngestDBCore.Dto;
using IngestDBCore.Tool;
using IngestGlobalPlugin.Dto;
using IngestGlobalPlugin.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace IngestGlobalPlugin
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

            CreateMap<GlobalTcResponse, GetDefaultSTC_param>()
                .ForMember(a => a.tcType, (map) => map.MapFrom(b => b.TcType))
                .ForMember(a => a.nTC, (map) => map.MapFrom(b => b.TC));

            CreateMap<DbpGlobalState, GlobalState>()
                .ForMember(a => a.strLabel, (map) => map.MapFrom(b => b.Label))
                .ForMember(a => a.dtLastTime, (map) => map.MapFrom(b => b.Lasttime));

            CreateMap<DbpGlobalState, DtoGlobalState>()
                .ForMember(a => a.Label, (map) => map.MapFrom(b => b.Label))
                .ForMember(a => a.LastTime, (map) => map.MapFrom(b => b.Lasttime));

            CreateMap<DbpUsertemplate, UserTemplate>()
                .ForMember(a => a.nTemplateID, (map) => map.MapFrom(b => b.Templateid))
                .ForMember(a => a.strUserCode, (map) => map.MapFrom(b => b.Usercode))
                .ForMember(a => a.strTemplateName, (map) => map.MapFrom(b => b.Templatename))
                .ForMember(a => a.strTemplateContent, (map) => map.MapFrom(b => b.Templatecontent));

            CreateMap<DbpUsertemplate, DtoUserTemplate>()
                //.ForMember(a => a.TemplateID, (map) => map.MapFrom(b => b.Templateid))
                //.ForMember(a => a.UserCode, (map) => map.MapFrom(b => b.Usercode))
                //.ForMember(a => a.TemplateName, (map) => map.MapFrom(b => b.Templatename))
                //.ForMember(a => a.TemplateContent, (map) => map.MapFrom(b => b.Templatecontent))
                ;

            CreateMap<CMUserInfo, DtoCMUserInfo>()
                .ForMember(a => a.Id, (map) => map.MapFrom(b => b.id))
                .ForMember(a => a.CreateTime, (map) => map.MapFrom(b => b.createtime))
                .ForMember(a => a.Disabled, (map) => map.MapFrom(b => b.disabled))
                .ForMember(a => a.Email, (map) => map.MapFrom(b => b.email))
                .ForMember(a => a.LoginName, (map) => map.MapFrom(b => b.loginname))
                .ForMember(a => a.NickName, (map) => map.MapFrom(b => b.nickname));

            CreateMap<DbpCaptureparamtemplate, OldCapParamTemplate>()
                .ForMember(a => a.nID, (map) => map.MapFrom(b => b.Captureparamid))
                .ForMember(a => a.strTemplateName, (map) => map.MapFrom(b => b.Captemplatename))
                .ForMember(a => a.strParamTemplate, (map) => map.MapFrom(b => b.Captureparam));

            CreateMap<DbpCaptureparamtemplate, CapParamTemplate>()
                .ForMember(a => a.ID, (map) => map.MapFrom(b => b.Captureparamid))
                .ForMember(a => a.TemplateName, (map) => map.MapFrom(b => b.Captemplatename))
                .ForMember(a => a.ParamTemplate, (map) => map.MapFrom(b => b.Captureparam));

            CreateMap<DbpUserparamMap, UserParmMap>()
                .ForMember(a => a.nCapatureParamID, (map) => map.MapFrom(b => b.Captureparamid))
                .ForMember(a => a.szClassCode, (map) => map.MapFrom(b => b.Usercode));

            CreateMap<UserParmMap, DbpUserparamMap>()
                .ForMember(a => a.Captureparamid, (map) => map.MapFrom(b => b.nCapatureParamID))
                .ForMember(a => a.Usercode, (map) => map.MapFrom(b => b.szClassCode));


            CreateMap<MqMsgInfoRequest, MQmsgInfo>()
                .ForMember(a => a.type, (map) => map.MapFrom(b => b.Type))
                .ForMember(a => a.sNextRetry, (map) => map.MapFrom(b => b.NextRetry))
                .ForMember(a => a.strLock, (map) => map.MapFrom(b => b.Lock))
                .ForMember(a => a.nActionID, (map) => map.MapFrom(b => b.ActionID)).ReverseMap();
            CreateMap<DbpMsmqmsg, MqMsgInfoRequest>()
                .ForMember(a => a.MsgSendTime, (map) => map.MapFrom(b => b.Msgsendtime))
                .ForMember(a => a.MsgRevTime, (map) => map.MapFrom(b => b.Msgrevtime))
                .ForMember(a => a.MsgStatus, (map) => map.MapFrom(b => b.Msgstatus))
                .ForMember(a => a.MsgProcessTime, (map) => map.MapFrom(b => b.Msgprocesstime))
                .ForMember(a => a.Type, (map) => map.MapFrom(b => b.Msgtype))
                .ForMember(a => a.NextRetry, (map) => map.MapFrom(b => b.Nextretry))
                .ForMember(a => a.Lock, (map) => map.MapFrom(b => b.Lockdata));
            CreateMap<DbpMsmqmsg, MQmsgInfo>()
                .ForMember(a => a.MsgSendTime, (map) => map.MapFrom(b => b.Msgsendtime))
                .ForMember(a => a.MsgRevTime, (map) => map.MapFrom(b => b.Msgrevtime))
                .ForMember(a => a.MsgStatus, (map) => map.MapFrom(b => b.Msgstatus))
                .ForMember(a => a.MsgProcessTime, (map) => map.MapFrom(b => b.Msgprocesstime))
                .ForMember(a => a.type, (map) => map.MapFrom(b => b.Msgtype))
                .ForMember(a => a.sNextRetry, (map) => map.MapFrom(b => b.Nextretry))
                .ForMember(a => a.strLock, (map) => map.MapFrom(b => b.Lockdata));

            CreateMap<VideoInfoResponse, VideoInfo>()
                .ForMember(a => a.nVideoSource, (map) => map.MapFrom(b => b.VideoSource))
                .ForMember(a => a.nVideoTypeID, (map) => map.MapFrom(b => b.VideoTypeID))
                .ForMember(a => a.strFilename, (map) => map.MapFrom(b => b.Filename)).ReverseMap();

            CreateMap<FileFormateInfoResponse, FileFormatInfo_in>()
                .ForMember(a => a.extrainfo, (map) => map.MapFrom(b => b.ExtraInfo))
                .ForMember(a => a.key, (map) => map.MapFrom(b => b.Key))
                .ForMember(a => a.nformatid, (map) => map.MapFrom(b => b.FormatID))
                .ForMember(a => a.videostrandguid, (map) => map.MapFrom(b => b.VideoStrandGuid))
                .ForMember(a => a.videostrandid, (map) => map.MapFrom(b => b.VideoStrandID))
                .ReverseMap();

            CreateMap<FileFormatInfo_in, FileFormatInfo_out>().ReverseMap();
            CreateMap<FileFormateInfoResponse, FileFormatInfo_out>()
                .ForMember(a => a.extrainfo, (map) => map.MapFrom(b => b.ExtraInfo))
                .ForMember(a => a.key, (map) => map.MapFrom(b => b.Key))
                .ForMember(a => a.nformatid, (map) => map.MapFrom(b => b.FormatID))
                .ForMember(a => a.videostrandguid, (map) => map.MapFrom(b => b.VideoStrandGuid))
                .ForMember(a => a.videostrandid, (map) => map.MapFrom(b => b.VideoStrandID));

            CreateMap<MsgFailedRecord, DbpMsgFailedrecord>()
                .ForMember(a => a.TaskId, (map) => map.MapFrom(b => b.TaskID))
                .ForMember(a => a.SectionId, (map) => map.MapFrom(b => b.SectionID))
                .ForMember(a => a.DealTime, (map) => map.MapFrom(b => b.DealTime)).ReverseMap();


            CreateMap<DbpMetadatapolicy, MetaDataPolicy>()
                .ForMember(a => a.nID, (map) => map.MapFrom(b => b.Policyid))
                .ForMember(a => a.strName, (map) => map.MapFrom(b => b.Policyname))
                .ForMember(a => a.strDesc, (map) => map.MapFrom(b => b.Policydesc))
                .ForMember(a => a.nDefaultPolicy, (map) => map.MapFrom(b => b.Defaultpolicy))
                .ForMember(a => a.strArchiveType, (map) => map.MapFrom(b => b.Archivetype));

            CreateMap<MaterialInfo, DbpMaterial>()
                .ForMember(a => a.Materialid, (map) => map.MapFrom(b => b.nID))
                .ForMember(a => a.Name, (map) => map.MapFrom(b => b.strName))
                .ForMember(a => a.Remark, (map) => map.MapFrom(b => b.strRemark))
                .ForMember(a => a.Createtime, (map) => map.MapFrom(b => b.strCreateTime))
                .ForMember(a => a.Taskid, (map) => map.MapFrom(b => b.nTaskID))
                .ForMember(a => a.Sectionid, (map) => map.MapFrom(b => b.nSectionID))
                .ForMember(a => a.Guid, (map) => map.MapFrom(b => b.strGUID))
                .ForMember(a => a.Clipstate, (map) => map.MapFrom(b => b.nClipState))
                .ForMember(a => a.Usercode, (map) => map.MapFrom(b => b.strUserCode))
                .ForMember(a => a.Deletedstate, (map) => map.MapFrom(b => b.nDeleteState));

            CreateMap<DbpMaterial, MaterialInfoResponse>()
                .ForMember(a => a.ID, (map) => map.MapFrom(b => b.Materialid))
                .ForMember(a => a.CreateTime, (map) => map.MapFrom(b => b.Createtime))
                .ForMember(a => a.DeleteState, (map) => map.MapFrom(b => b.Deletedstate));

            CreateMap<DbpMaterialVideo, VideoInfoResponse > ()
                .ForMember(a => a.Filename, (map) => map.MapFrom(b => b.Videofilename)).ReverseMap();

            CreateMap<DbpMaterialAudio, AudioInfoResponse>()
                .ForMember(a => a.Filename, (map) => map.MapFrom(b => b.Audiofilename)).ReverseMap();
            //ReverseMap
        }

    }
}
