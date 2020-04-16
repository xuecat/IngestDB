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
            #region DbpRcdindesc To RoterInportDesc、RouterInResponse
            //V2
            CreateMap<DbpRcdindesc, RouterInResponse>()
               //.ForMember(a => a.RCDeviceID, (map) => map.MapFrom(b => b.Rcdeviceid))
               .ForMember(a => a.RCInportIdx, (map) => map.MapFrom(b => b.Recinidx));
            //.ForMember(a => a.SignalSource, (map) => map.MapFrom(b => b.Signalsource))
            //.ForMember(a => a.SignalSrcID, (map) => map.MapFrom(b => b.Signalsrcid));
            //V1
            CreateMap<DbpRcdindesc, RoterInportDesc>()
                .ForMember(a => a.nRCDeviceID, (map) => map.MapFrom(b => b.Rcdeviceid))
                .ForMember(a => a.nRCInportIdx, (map) => map.MapFrom(b => b.Recinidx))
                .ForMember(a => a.nSignalSource, (map) => map.MapFrom(b => b.Signalsource))
                .ForMember(a => a.nSignalSrcID, (map) => map.MapFrom(b => b.Signalsrcid));
            #endregion

            #region DbpRcdoutdesc To RoterOutDesc
            //V2

            //V1
            CreateMap<DbpRcdoutdesc, RoterOutDesc>()
                .ForMember(a => a.nRCDeviceID, (map) => map.MapFrom(b => b.Rcdeviceid))
                .ForMember(a => a.nRCOutportIdx, (map) => map.MapFrom(b => b.Recoutidx))
                .ForMember(a => a.nChannelID, (map) => map.MapFrom(b => b.Channelid))
                .ForMember(a => a.DeviceType, (map) => map.MapFrom(b => b.Devicetype));
            #endregion

            #region DbpSignalDeviceMap To SignalDeviceMap
            //V2

            //V1
            CreateMap<DbpSignalDeviceMap, SignalDeviceMap>()
                .ForMember(a => a.nSignalID, (map) => map.MapFrom(b => b.Signalsrcid))
                .ForMember(a => a.nDeviceID, (map) => map.MapFrom(b => b.Deviceid))
                .ForMember(a => a.nOutPortIdx, (map) => map.MapFrom(b => b.Deviceoutportidx))
                .ForMember(a => a.SignalSource, (map) => map.MapFrom(b => b.Signalsource));
            #endregion

            #region DbpSignalsrc To SignalSrcInfo
            //V2

            //V1
            CreateMap<DbpSignalsrc, SignalSrcInfo>()
                .ForMember(a => a.nID, (map) => map.MapFrom(b => b.Signalsrcid))
                .ForMember(a => a.strName, (map) => map.MapFrom(b => b.Name))
                .ForMember(a => a.strDesc, (map) => map.MapFrom(b => b.Signaldesc))
                .ForMember(a => a.nTypeID, (map) => map.MapFrom(b => b.Signaltypeid))
                .ForMember(a => a.nImageType, (map) => map.MapFrom(b => b.Imagetype))
                .ForMember(a => a.nPureAudio, (map) => map.MapFrom(b => b.Pureaudio));
            #endregion

            #region DbpSignalsrc To ProgrammeInfo
            //V2

            //V1
            CreateMap<DbpSignalsrc, ProgrammeInfo>()
                .ForMember(a => a.ProgrammeId, (map) => map.MapFrom(b => b.Signalsrcid))
                .ForMember(a => a.ProgrammeName, (map) => map.MapFrom(b => b.Name))
                .ForMember(a => a.ProgrammeDesc, (map) => map.MapFrom(b => b.Signaldesc))
                .ForMember(a => a.TypeId, (map) => map.MapFrom(b => b.Signaltypeid))
                .ForMember(a => a.emPgmType, (map) => map.MapFrom(b => ProgrammeType.PT_SDI))
                .ForMember(a => a.emImageType, (map) => map.MapFrom(b => b.Imagetype))
                .ForMember(a => a.nPureAudio, (map) => map.MapFrom(b => b.Pureaudio));
            #endregion

            #region DbpCapturechannels To CaptureChannelInfo
            //V2

            //V1
            CreateMap<DbpCapturechannels, CaptureChannelInfo>()
                .ForMember(a => a.nID, (map) => map.MapFrom(b => b.Channelid))
                .ForMember(a => a.strName, (map) => map.MapFrom(b => b.Channelname))
                .ForMember(a => a.strDesc, (map) => map.MapFrom(b => b.Channeldesc))
                .ForMember(a => a.nCPDeviceID, (map) => map.MapFrom(b => b.Cpdeviceid))
                .ForMember(a => a.nChannelIndex, (map) => map.MapFrom(b => b.Channelindex))
                .ForMember(a => a.nDeviceTypeID, (map) => map.MapFrom(b => b.Devicetypeid))
                .ForMember(a => a.BackState, (map) => map.MapFrom(b => b.Backupflag))
                //.ForMember(a => a.nCarrierID, (map) => map.MapFrom(b => b.nCarrierID))
                //.ForMember(a => a.orderCode, (map) => map.MapFrom(b => b.orderCode))
                //.ForMember(a => a.nGroupID, (map) => map.MapFrom(b => b.nGroupID))
                .ForMember(a => a.nCPSignalType, (map) => map.MapFrom(b => b.Cpsignaltype));
            #endregion

            #region DbpCapturedevice To CaptureDeviceInfo
            //V2

            //V1
            CreateMap<DbpCapturedevice, CaptureDeviceInfo>()
                .ForMember(a => a.nID, (map) => map.MapFrom(b => b.Cpdeviceid))
                .ForMember(a => a.nDeviceTypeID, (map) => map.MapFrom(b => b.Devicetypeid))
                .ForMember(a => a.strDeviceName, (map) => map.MapFrom(b => b.Devicename))
                .ForMember(a => a.strIP, (map) => map.MapFrom(b => b.Ipaddress))
                .ForMember(a => a.nOrderCode, (map) => map.MapFrom(b => b.Ordercode));
            #endregion

            #region DbpIpVirtualchannel To CaptureChannelInfo
            //V2

            //V1
            CreateMap<DbpIpVirtualchannel, CaptureChannelInfo>()
                .ForMember(a => a.nID, (map) => map.MapFrom(b => b.Channelid))
                .ForMember(a => a.strName, (map) => map.MapFrom(b => b.Channelname))
                .ForMember(a => a.strDesc, (map) => map.MapFrom(b => b.Channeldesc))
                .ForMember(a => a.nCPDeviceID, (map) => map.MapFrom(b => b.Deviceid))
                .ForMember(a => a.nChannelIndex, (map) => map.MapFrom(b => b.Deviceindex))
                .ForMember(a => a.nDeviceTypeID, (map) => map.MapFrom(b => b.Channeltype))
                .ForMember(a => a.BackState, (map) => map.MapFrom(b => b.Backuptype))
                .ForMember(a => a.nCarrierID, (map) => map.MapFrom(b => b.Carrierid))
                .ForMember(a => a.orderCode, (map) => map.MapFrom(b => b.Deviceindex))
                //.ForMember(a => a.nGroupID, (map) => map.MapFrom(b => b.nGroupID))
                .ForMember(a => a.nCPSignalType, (map) => map.MapFrom(b => b.Cpsignaltype));
            #endregion

            #region DbpSignalsrcMasterbackup To SignalSrcExInfo
            //V2

            //V1
            CreateMap<DbpSignalsrcMasterbackup, SignalSrcExInfo>()
                .ForMember(a => a.nID, (map) => map.MapFrom(b => b.Signalsrcid))
                .ForMember(a => a.nSignalSrcType, (map) => map.MapFrom(b => b.Signalsrctype))
                .ForMember(a => a.bIsMainSignalSrc, (map) => map.MapFrom(b => b.Ismastersrc == 1))
                .ForMember(a => a.nMainSignalSrcId, (map) => map.MapFrom(b => b.Mastersignalsrcid));
            #endregion

            #region DbpMsvchannelState To MSVChannelState
            //V2

            //V1
            CreateMap<DbpMsvchannelState, MSVChannelState>()
                .ForMember(a => a.nChannelID, (map) => map.MapFrom(b => b.Channelid))
                .ForMember(a => a.emDevState, (map) => map.MapFrom(b => b.Devstate))
                .ForMember(a => a.emMSVMode, (map) => map.MapFrom(b => b.Msvmode))
                .ForMember(a => a.vtrID, (map) => map.MapFrom(b => b.Sourcevtrid))
                .ForMember(a => a.curUserCode, (map) => map.MapFrom(b => b.Curusercode))
                .ForMember(a => a.kamatakiInfo, (map) => map.MapFrom(b => b.Kamatakiinfo))
                .ForMember(a => a.uploadMode, (map) => map.MapFrom(b => b.Uploadstate));
            #endregion

            #region DbpSignalgroup To AllSignalGroup
            //V2

            //V1
            CreateMap<DbpSignalgroup, AllSignalGroup>()
                .ForMember(a => a.groupid, (map) => map.MapFrom(b => b.Groupid))
                .ForMember(a => a.groupname, (map) => map.MapFrom(b => b.Groupname))
                .ForMember(a => a.groupdesc, (map) => map.MapFrom(b => b.Groupdesc));
            #endregion

            #region DbpGpiMap To GPIDeviceMapInfo
            //V2

            //V1
            CreateMap<DbpGpiMap, GPIDeviceMapInfo>()
                .ForMember(a => a.nGPIID, (map) => map.MapFrom(b => b.Gpiid))
                .ForMember(a => a.nGPIOutputPort, (map) => map.MapFrom(b => b.Gpioutputport))
                .ForMember(a => a.nAVOutputPort, (map) => map.MapFrom(b => b.Avoutputport))
                .ForMember(a => a.nCaptureParamID, (map) => map.MapFrom(b => b.Captureparamid));
            #endregion

            #region MyRegion

            #endregion

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


            CreateMap<DbpChannelRecmap, RecUnitMap>()
                .ForMember(a => a.ConnectorID, (map) => map.MapFrom(b => b.Channelid))
                .ForMember(a => a.UnitID, (map) => map.MapFrom(b => b.Recid));

            //ReverseMap
        }
    }
}
