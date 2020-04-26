using AutoMapper;
using IngestDBCore.Dto;
using IngestDBCore.Tool;
using IngestGlobalPlugin.Dto;
using IngestGlobalPlugin.Dto.Request;
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
                .ForMember( a=> a.tcType, (map) => map.MapFrom(b => b.TcType))
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
                .ForMember(a => a.TemplateID, (map) => map.MapFrom(b => b.Templateid))
                .ForMember(a => a.UserCode, (map) => map.MapFrom(b => b.Usercode))
                .ForMember(a => a.TemplateName, (map) => map.MapFrom(b => b.Templatename))
                .ForMember(a => a.TemplateContent, (map) => map.MapFrom(b => b.Templatecontent));

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

            CreateMap<VideoInfoResponse, VideoInfo>()
                .ForMember(a => a.nVideoSource, (map) => map.MapFrom(b => b.VideoSource))
                .ForMember(a => a.nVideoTypeID, (map) => map.MapFrom(b => b.VideoTypeID))
                .ForMember(a => a.strFilename, (map) => map.MapFrom(b => b.Filename));
            //ReverseMap
        }

    }
}
