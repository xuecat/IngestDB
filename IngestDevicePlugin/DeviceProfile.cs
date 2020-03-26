using AutoMapper;
using IngestDevicePlugin.Dto.Response;
using IngestDevicePlugin.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace IngestDevicePlugin
{
   
    public class DeviceProfile : Profile
    {
        public DeviceProfile()
        {
            CreateMap<DbpRcdindesc, RouterInResponse>()
                //.ForMember(a => a.RCDeviceID, (map) => map.MapFrom(b => b.Rcdeviceid))
                .ForMember(a => a.RCInportIdx, (map) => map.MapFrom(b => b.Recinidx));
                //.ForMember(a => a.SignalSource, (map) => map.MapFrom(b => b.Signalsource))
                //.ForMember(a => a.SignalSrcID, (map) => map.MapFrom(b => b.Signalsrcid));
            CreateMap<DbpRcdindesc, RoterInportDesc>()
                .ForMember(a => a.nRCDeviceID, (map) => map.MapFrom(b => b.Rcdeviceid))
                .ForMember(a => a.nRCInportIdx, (map) => map.MapFrom(b => b.Recinidx))
                .ForMember(a => a.nSignalSource, (map) => map.MapFrom(b => b.Signalsource))
                .ForMember(a => a.nSignalSrcID, (map) => map.MapFrom(b => b.Signalsrcid));

            //CreateMap<DbpTaskMetadata, TaskMetadataResponse>()
            //    .ForMember(a => a.Metadata, (map) => map.MapFrom(b => b.Metadatalong));

            //ReverseMap
        }
    }
}
