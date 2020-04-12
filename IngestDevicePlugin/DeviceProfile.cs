using AutoMapper;
using IngestDevicePlugin.Dto;
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

            CreateMap<CaptureChannelInfoResponse, CaptureChannelInfo>()
                .ForMember(a => a.nID, (map) => map.MapFrom(b => b.ID))
                .ForMember(a => a.strName, (map) => map.MapFrom(b => b.Name))
                .ForMember(a => a.strDesc, (map) => map.MapFrom(b => b.Desc))
                .ForMember(a => a.nCPDeviceID, (map) => map.MapFrom(b => b.CPDeviceID))
                .ForMember(a => a.nChannelIndex, (map) => map.MapFrom(b => b.ChannelIndex))
                .ForMember(a => a.nDeviceTypeID, (map) => map.MapFrom(b => b.DeviceTypeID))
                .ForMember(a => a.BackState, (map) => map.MapFrom(b => b.BackState))
                .ForMember(a => a.nCarrierID, (map) => map.MapFrom(b => b.CarrierID))
                .ForMember(a => a.orderCode, (map) => map.MapFrom(b => b.OrderCode))
                .ForMember(a => a.nCPSignalType, (map) => map.MapFrom(b => b.CPSignalType))
                .ForMember(a => a.nGroupID, (map) => map.MapFrom(b => b.GroupID));
            //CreateMap<DbpTaskMetadata, TaskMetadataResponse>()
            //    .ForMember(a => a.Metadata, (map) => map.MapFrom(b => b.Metadatalong));

            CreateMap<RecUnitMap, DbpChannelRecmap>()
                .ForMember(a => a.Channelid, (map) => map.MapFrom(b => b.ConnectorID))
                .ForMember(a => a.Recid, (map) => map.MapFrom(b => b.UnitID));


            CreateMap<DbpChannelRecmap ,RecUnitMap>()
                .ForMember(a => a.ConnectorID, (map) => map.MapFrom(b => b.Channelid))
                .ForMember(a => a.UnitID, (map) => map.MapFrom(b => b.Recid));

            //ReverseMap
        }
    }
}
