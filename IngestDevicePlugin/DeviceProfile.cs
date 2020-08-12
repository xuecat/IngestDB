using AutoMapper;
using IngestDevicePlugin.Dto;
using IngestDevicePlugin.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using IngestDevicePlugin.Dto.Enum;
using IngestDevicePlugin.Dto.Response;
using IngestDevicePlugin.Dto.OldResponse;

namespace IngestDevicePlugin
{

    public class DeviceProfile : Profile
    {
        public DeviceProfile()
        {

            CreateMap<DbpMsvchannelState, MSVChannelStateResponse>()
                    .ForMember(a => a.ChannelId, (map) => map.MapFrom(b => b.Channelid))
                    .ForMember(a => a.DevState, (map) => map.MapFrom(b => b.Devstate))
                    .ForMember(a => a.MsvMode, (map) => map.MapFrom(b => b.Msvmode))
                    .ForMember(a => a.VtrId, (map) => map.MapFrom(b => b.Sourcevtrid))
                    .ForMember(a => a.UserCode, (map) => map.MapFrom(b => b.Curusercode))
                    .ForMember(a => a.KamatakiInfo, (map) => map.MapFrom(b => b.Kamatakiinfo))
                    .ForMember(a => a.UploadMode, (map) => map.MapFrom(b => b.Uploadstate));

            #region DbpRcdindesc To RoterInportDesc、RouterInResponse
            //V2
            CreateMap<DbpRcdindesc, RouterInResponse>()
               //.ForMember(a => a.RCDeviceID, (map) => map.MapFrom(b => b.Rcdeviceid))
               .ForMember(a => a.RcInportIdx, (map) => map.MapFrom(b => b.Recinidx));
            //.ForMember(a => a.SignalSource, (map) => map.MapFrom(b => b.Signalsource))
            //.ForMember(a => a.SignalSrcID, (map) => map.MapFrom(b => b.Signalsrcid));
            //V1
            CreateMap<DbpRcdindesc, RoterInportDesc>()
                .ForMember(a => a.nRCDeviceID, (map) => map.MapFrom(b => b.Rcdeviceid))
                .ForMember(a => a.nRCInportIdx, (map) => map.MapFrom(b => b.Recinidx))
                .ForMember(a => a.nSignalSource, (map) => map.MapFrom(b => b.Signalsource))
                .ForMember(a => a.nSignalSrcID, (map) => map.MapFrom(b => b.Signalsrcid));
            #endregion

            #region DbpRcdoutdesc To RoterOutDesc、RoterOutResponse
            //V2
            CreateMap<DbpRcdoutdesc, RoterOutResponse>()
                .ForMember(a => a.RcDeviceId, (map) => map.MapFrom(b => b.Rcdeviceid))
                .ForMember(a => a.RcOutportIdx, (map) => map.MapFrom(b => b.Recoutidx))
                .ForMember(a => a.ChannelId, (map) => map.MapFrom(b => b.Channelid))
                .ForMember(a => a.DeviceType, (map) => map.MapFrom(b => b.Devicetype));
            //V1
            CreateMap<DbpRcdoutdesc, RoterOutDesc>()
                .ForMember(a => a.nRCDeviceID, (map) => map.MapFrom(b => b.Rcdeviceid))
                .ForMember(a => a.nRCOutportIdx, (map) => map.MapFrom(b => b.Recoutidx))
                .ForMember(a => a.nChannelID, (map) => map.MapFrom(b => b.Channelid))
                .ForMember(a => a.DeviceType, (map) => map.MapFrom(b => b.Devicetype));
            #endregion

            #region DbpSignalDeviceMap To SignalDeviceMap
            //V2
            CreateMap<DbpSignalDeviceMap, SignalDeviceMapResponse>()
                .ForMember(a => a.SignalId, (map) => map.MapFrom(b => b.Signalsrcid))
                .ForMember(a => a.DeviceId, (map) => map.MapFrom(b => b.Deviceid))
                .ForMember(a => a.OutPortIdx, (map) => map.MapFrom(b => b.Deviceoutportidx))
                .ForMember(a => a.SignalSource, (map) => map.MapFrom(b => b.Signalsource));
            //V1
            CreateMap<DbpSignalDeviceMap, SignalDeviceMap>()
                .ForMember(a => a.nSignalID, (map) => map.MapFrom(b => b.Signalsrcid))
                .ForMember(a => a.nDeviceID, (map) => map.MapFrom(b => b.Deviceid))
                .ForMember(a => a.nOutPortIdx, (map) => map.MapFrom(b => b.Deviceoutportidx))
                .ForMember(a => a.SignalSource, (map) => map.MapFrom(b => b.Signalsource));
            #endregion

            #region DbpSignalsrc To SignalSrcInfo
            //V2
            CreateMap<DbpSignalsrc, SignalSrcResponse>()
                .ForMember(a => a.Id, (map) => map.MapFrom(b => b.Signalsrcid))
                .ForMember(a => a.Name, (map) => map.MapFrom(b => b.Name))
                .ForMember(a => a.Desc, (map) => map.MapFrom(b => b.Signaldesc))
                .ForMember(a => a.TypeId, (map) => map.MapFrom(b => b.Signaltypeid))
                .ForMember(a => a.ImageType, (map) => map.MapFrom(b => b.Imagetype))
                .ForMember(a => a.PureAudio, (map) => map.MapFrom(b => b.Pureaudio));
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
                .ForMember(a => a.nPureAudio, (map) => map.MapFrom(b => b.Pureaudio))
                .AfterMap((b, a) =>
                {
                    a.emPgmType = ProgrammeType.PT_SDI;
                }); ;
            #endregion

            #region DbpCapturechannels To CaptureChannelInfo、CaptureChannelInfoResponse
            //V2
            CreateMap<DbpIpVirtualchannel, CaptureChannelInfoResponse>()
                .ForMember(a => a.Id, (map) => map.MapFrom(b => b.Channelid))
                .ForMember(a => a.Name, (map) => map.MapFrom(b => b.Channelname))
                .ForMember(a => a.Desc, (map) => map.MapFrom(b => b.Channeldesc))
                .ForMember(a => a.CpDeviceId, (map) => map.MapFrom(b => b.Deviceid))
                .ForMember(a => a.ChannelIndex, (map) => map.MapFrom(b => b.Deviceindex))
                .ForMember(a => a.DeviceTypeId, (map) => map.MapFrom(b => b.Channeltype))
                .ForMember(a => a.BackState, (map) => map.MapFrom(b => b.Backuptype))
                //.ForMember(a => a.CarrierID, (map) => map.MapFrom(b => b.nCarrierID))
                //.ForMember(a => a.OrderCode, (map) => map.MapFrom(b => b.orderCode))
                //.ForMember(a => a.GroupID, (map) => map.MapFrom(b => b.nGroupID))
                .ForMember(a => a.CpSignalType, (map) => map.MapFrom(b => b.Cpsignaltype));

            CreateMap<DbpCapturechannels, CaptureChannelInfoResponse>()
                .ForMember(a => a.Id, (map) => map.MapFrom(b => b.Channelid))
                .ForMember(a => a.Name, (map) => map.MapFrom(b => b.Channelname))
                .ForMember(a => a.Desc, (map) => map.MapFrom(b => b.Channeldesc))
                .ForMember(a => a.CpDeviceId, (map) => map.MapFrom(b => b.Cpdeviceid))
                .ForMember(a => a.ChannelIndex, (map) => map.MapFrom(b => b.Channelindex))
                .ForMember(a => a.DeviceTypeId, (map) => map.MapFrom(b => b.Devicetypeid))
                .ForMember(a => a.BackState, (map) => map.MapFrom(b => b.Backupflag))
                //.ForMember(a => a.CarrierID, (map) => map.MapFrom(b => b.nCarrierID))
                //.ForMember(a => a.OrderCode, (map) => map.MapFrom(b => b.orderCode))
                //.ForMember(a => a.GroupID, (map) => map.MapFrom(b => b.nGroupID))
                .ForMember(a => a.CpSignalType, (map) => map.MapFrom(b => b.Cpsignaltype));
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
            CreateMap<CaptureChannelInfoResponse, CaptureChannelInfo>()
                .ForMember(a => a.nID, (map) => map.MapFrom(b => b.Id))
                .ForMember(a => a.strName, (map) => map.MapFrom(b => b.Name))
                .ForMember(a => a.strDesc, (map) => map.MapFrom(b => b.Desc))
                .ForMember(a => a.nCPDeviceID, (map) => map.MapFrom(b => b.CpDeviceId))
                .ForMember(a => a.nChannelIndex, (map) => map.MapFrom(b => b.ChannelIndex))
                .ForMember(a => a.nDeviceTypeID, (map) => map.MapFrom(b => b.DeviceTypeId))
                .ForMember(a => a.BackState, (map) => map.MapFrom(b => b.BackState))
                .ForMember(a => a.nCarrierID, (map) => map.MapFrom(b => b.CarrierId))
                .ForMember(a => a.orderCode, (map) => map.MapFrom(b => b.OrderCode))
                .ForMember(a => a.nGroupID, (map) => map.MapFrom(b => b.GroupId))
                .ForMember(a => a.nCPSignalType, (map) => map.MapFrom(b => b.CpSignalType));
            #endregion

            #region DbpCapturedevice To CaptureDeviceInfo、CaptureDeviceInfoResponse
            //V2
            CreateMap<DbpCapturedevice, CaptureDeviceInfoResponse>()
                .ForMember(a => a.Id, (map) => map.MapFrom(b => b.Cpdeviceid))
                .ForMember(a => a.DeviceTypeId, (map) => map.MapFrom(b => b.Devicetypeid))
                .ForMember(a => a.DeviceName, (map) => map.MapFrom(b => b.Devicename))
                .ForMember(a => a.Ip, (map) => map.MapFrom(b => b.Ipaddress))
                .ForMember(a => a.OrderCode, (map) => map.MapFrom(b => b.Ordercode));
            //V1
            CreateMap<DbpCapturedevice, CaptureDeviceInfo>()
                .ForMember(a => a.nID, (map) => map.MapFrom(b => b.Cpdeviceid))
                .ForMember(a => a.nDeviceTypeID, (map) => map.MapFrom(b => b.Devicetypeid))
                .ForMember(a => a.strDeviceName, (map) => map.MapFrom(b => b.Devicename))
                .ForMember(a => a.strIP, (map) => map.MapFrom(b => b.Ipaddress))
                .ForMember(a => a.nOrderCode, (map) => map.MapFrom(b => b.Ordercode));
            #endregion

            #region DbpIpDatachannelinfo To TSDataChannelInfo
            //V2

            //V1
            CreateMap<DbpIpDatachannelinfo, TSDataChannelInfo>()
                .ForMember(a => a.DataChannelId, (map) => map.MapFrom(b => b.Datachannelid))
                .ForMember(a => a.DataChannelName, (map) => map.MapFrom(b => b.Datachannelname))
                .ForMember(a => a.DeviceId, (map) => map.MapFrom(b => b.Deviceid))
                .ForMember(a => a.DataChannelIndex, (map) => map.MapFrom(b => b.Datachannelindex));
            CreateMap<TSDataChannelInfo, DbpIpDatachannelinfo>()
                .ForMember(a => a.Datachannelid, (map) => map.MapFrom(b => b.DataChannelId))
                .ForMember(a => a.Datachannelname, (map) => map.MapFrom(b => b.DataChannelName))
                .ForMember(a => a.Deviceid, (map) => map.MapFrom(b => b.DeviceId))
                .ForMember(a => a.Datachannelindex, (map) => map.MapFrom(b => b.DataChannelIndex));
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

            #region DbpIpVirtualchannel To TSVirtualChannelInfo
            //V2

            //V1
            CreateMap<DbpIpVirtualchannel, TSVirtualChannelInfo>()
                .ForMember(a => a.ChannelId, (map) => map.MapFrom(b => b.Channelid))
                .ForMember(a => a.ChannelName, (map) => map.MapFrom(b => b.Channelname))
                .ForMember(a => a.ChannelDesc, (map) => map.MapFrom(b => b.Channeldesc))
                .ForMember(a => a.ChannelIPAddress, (map) => map.MapFrom(b => b.Ipaddress))
                .ForMember(a => a.CtrlPort, (map) => map.MapFrom(b => b.Ctrlport))
                .ForMember(a => a.DeviceId, (map) => map.MapFrom(b => b.Deviceid))
                .ForMember(a => a.DeviceIndex, (map) => map.MapFrom(b => b.Deviceindex))
                .ForMember(a => a.emChannelStatus, (map) => map.MapFrom(b => b.Channelstatus))
                .ForMember(a => a.emChannelType, (map) => map.MapFrom(b => b.Channeltype))
                .ForMember(a => a.nCarrierID, (map) => map.MapFrom(b => b.Carrierid))
                .ForMember(a => a.emBackUpType, (map) => map.MapFrom(b => b.Backuptype))
                .ForMember(a => a.nCPSignalType, (map) => map.MapFrom(b => b.Cpsignaltype));
            CreateMap<TSVirtualChannelInfo, DbpIpVirtualchannel>()
                .ForMember(a => a.Channelid, (map) => map.MapFrom(b => b.ChannelId))
                .ForMember(a => a.Channelname, (map) => map.MapFrom(b => b.ChannelName))
                .ForMember(a => a.Channeldesc, (map) => map.MapFrom(b => b.ChannelDesc))
                .ForMember(a => a.Ipaddress, (map) => map.MapFrom(b => b.ChannelIPAddress))
                .ForMember(a => a.Ctrlport, (map) => map.MapFrom(b => b.CtrlPort))
                .ForMember(a => a.Deviceid, (map) => map.MapFrom(b => b.DeviceId))
                .ForMember(a => a.Deviceindex, (map) => map.MapFrom(b => b.DeviceIndex))
                .ForMember(a => a.Channelstatus, (map) => map.MapFrom(b => b.emChannelStatus))
                //.ForMember(a => a.Channeltype, (map) => map.MapFrom(b => b.emChannelType))
                .ForMember(a => a.Carrierid, (map) => map.MapFrom(b => b.nCarrierID))
                .ForMember(a => a.Backuptype, (map) => map.MapFrom(b => b.emBackUpType));
            //.ForMember(a => a.Cpsignaltype, (map) => map.MapFrom(b => b.nCPSignalType));
            #endregion

            #region DbpSignalsrcMasterbackup To SignalSrcExInfo
            //V2
            CreateMap<DbpSignalsrcMasterbackup, SignalSrcExResponse>()
                .ForMember(a => a.Id, (map) => map.MapFrom(b => b.Signalsrcid))
                .ForMember(a => a.SignalSrcType, (map) => map.MapFrom(b => b.Signalsrctype))
                .ForMember(a => a.IsMainSignalSrc, (map) => map.MapFrom(b => b.Ismastersrc == 1))
                .ForMember(a => a.MainSignalSrcId, (map) => map.MapFrom(b => b.Mastersignalsrcid));
            //V1
            CreateMap<SignalSrcExResponse, SignalSrcExInfo>()
                .ForMember(a => a.nID, (map) => map.MapFrom(b => b.Id))
                .ForMember(a => a.nSignalSrcType, (map) => map.MapFrom(b => b.SignalSrcType))
                .ForMember(a => a.bIsMainSignalSrc, (map) => map.MapFrom(b => b.IsMainSignalSrc))
                .ForMember(a => a.nMainSignalSrcId, (map) => map.MapFrom(b => b.MainSignalSrcId));
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
            CreateMap<DbpSignalgroup, SignalGroupResponse>()
                .ForMember(a => a.GroupId, (map) => map.MapFrom(b => b.Groupid))
                .ForMember(a => a.GroupName, (map) => map.MapFrom(b => b.Groupname))
                .ForMember(a => a.GroupDesc, (map) => map.MapFrom(b => b.Groupdesc));
            //V1
            CreateMap<DbpSignalgroup, AllSignalGroup>()
                .ForMember(a => a.groupid, (map) => map.MapFrom(b => b.Groupid))
                .ForMember(a => a.groupname, (map) => map.MapFrom(b => b.Groupname))
                .ForMember(a => a.groupdesc, (map) => map.MapFrom(b => b.Groupdesc));
            #endregion

            #region DbpGpiMap To GPIDeviceInfo、GpiDeviceInfoResponse
            //V2
            CreateMap<DbpGpiInfo, GPIDeviceInfoResponse>()
                .ForMember(a => a.GpiId, (map) => map.MapFrom(b => b.Gpiid))
                .ForMember(a => a.GpiName, (map) => map.MapFrom(b => b.Gpiname))
                .ForMember(a => a.ComPort, (map) => map.MapFrom(b => b.Comport))
                .ForMember(a => a.OutputPortCount, (map) => map.MapFrom(b => b.Outputportcount))
                .ForMember(a => a.Description, (map) => map.MapFrom(b => b.Description));
            //V1
            CreateMap<DbpGpiInfo, GPIDeviceInfo>()
                .ForMember(a => a.nGPIID, (map) => map.MapFrom(b => b.Gpiid))
                .ForMember(a => a.strGPIName, (map) => map.MapFrom(b => b.Gpiname))
                .ForMember(a => a.nComPort, (map) => map.MapFrom(b => b.Comport))
                .ForMember(a => a.nOutputPortCount, (map) => map.MapFrom(b => b.Outputportcount))
                .ForMember(a => a.strDescription, (map) => map.MapFrom(b => b.Description));
            #endregion

            #region DbpGpiMap To GPIDeviceMapInfo
            //V2
            CreateMap<DbpGpiMap, GPIDeviceMapInfoResponse>()
                .ForMember(a => a.GpiId, (map) => map.MapFrom(b => b.Gpiid))
                .ForMember(a => a.GpiOutputPort, (map) => map.MapFrom(b => b.Gpioutputport))
                .ForMember(a => a.AvOutputPort, (map) => map.MapFrom(b => b.Avoutputport))
                .ForMember(a => a.CaptureParamID, (map) => map.MapFrom(b => b.Captureparamid));
            //V1
            CreateMap<DbpGpiMap, GPIDeviceMapInfo>()
                .ForMember(a => a.nGPIID, (map) => map.MapFrom(b => b.Gpiid))
                .ForMember(a => a.nGPIOutputPort, (map) => map.MapFrom(b => b.Gpioutputport))
                .ForMember(a => a.nAVOutputPort, (map) => map.MapFrom(b => b.Avoutputport))
                .ForMember(a => a.nCaptureParamID, (map) => map.MapFrom(b => b.Captureparamid));
            #endregion

            #region DbpIpDevice To TSDeviceInfo
            //V2

            //V1
            CreateMap<DbpIpDevice, TSDeviceInfo>()
                .ForMember(a => a.DeviceId, (map) => map.MapFrom(b => b.Deviceid))
                .ForMember(a => a.DeviceName, (map) => map.MapFrom(b => b.Devicename))
                .ForMember(a => a.DeviceDesc, (map) => map.MapFrom(b => b.Devicedesc))
                .ForMember(a => a.IPAddress, (map) => map.MapFrom(b => b.Ipaddress))
                .ForMember(a => a.Port, (map) => map.MapFrom(b => b.Port));
            CreateMap<TSDeviceInfo, DbpIpDevice>()
                .ForMember(a => a.Deviceid, (map) => map.MapFrom(b => b.DeviceId))
                .ForMember(a => a.Devicename, (map) => map.MapFrom(b => b.DeviceName))
                .ForMember(a => a.Devicedesc, (map) => map.MapFrom(b => b.DeviceDesc))
                .ForMember(a => a.Ipaddress, (map) => map.MapFrom(b => b.IPAddress))
                .ForMember(a => a.Port, (map) => map.MapFrom(b => b.Port));
            #endregion

            #region DbpIpProgramme To TSPgmInfo
            //V2

            //V1
            CreateMap<DbpIpProgramme, TSPgmInfo>()
                .ForMember(a => a.PgmId, (map) => map.MapFrom(b => b.Programmeid))
                .ForMember(a => a.PgmName, (map) => map.MapFrom(b => b.Programmename))
                .ForMember(a => a.PgmDesc, (map) => map.MapFrom(b => b.Programmedesc))
                .ForMember(a => a.DataChannelId, (map) => map.MapFrom(b => b.Datachannelid))
                .ForMember(a => a.PgmIndex, (map) => map.MapFrom(b => b.Programmeindex))
                .ForMember(a => a.PgmTypeId, (map) => map.MapFrom(b => b.Programmetype))
                .ForMember(a => a.emImageType, (map) => map.MapFrom(b => b.Imagetype))
                .ForMember(a => a.TSSingalInfo, (map) => map.MapFrom(b => b.Tssignalinfo))
                .ForMember(a => a.MulticastIP, (map) => map.MapFrom(b => b.Multicastip))
                .ForMember(a => a.MulticastPort, (map) => map.MapFrom(b => b.Multicastport))
                .ForMember(a => a.ExtendParams, (map) => map.MapFrom(b => b.Extendparams))
                .ForMember(a => a.nPureAudio, (map) => map.MapFrom(b => b.Pureaudio));
            CreateMap<TSPgmInfo, DbpIpProgramme>()
                .ForMember(a => a.Programmeid, (map) => map.MapFrom(b => b.PgmId))
                .ForMember(a => a.Programmename, (map) => map.MapFrom(b => b.PgmName))
                .ForMember(a => a.Programmedesc, (map) => map.MapFrom(b => b.PgmDesc))
                .ForMember(a => a.Datachannelid, (map) => map.MapFrom(b => b.DataChannelId))
                .ForMember(a => a.Programmeindex, (map) => map.MapFrom(b => b.PgmIndex))
                .ForMember(a => a.Programmetype, (map) => map.MapFrom(b => b.PgmTypeId))
                .ForMember(a => a.Imagetype, (map) => map.MapFrom(b => b.emImageType))
                .ForMember(a => a.Tssignalinfo, (map) => map.MapFrom(b => b.TSSingalInfo))
                .ForMember(a => a.Multicastip, (map) => map.MapFrom(b => b.MulticastIP))
                .ForMember(a => a.Multicastport, (map) => map.MapFrom(b => b.MulticastPort))
                .ForMember(a => a.Extendparams, (map) => map.MapFrom(b => b.ExtendParams))
                .ForMember(a => a.Pureaudio, (map) => map.MapFrom(b => b.nPureAudio));
            #endregion

            //V2

            //V1
            CreateMap<DbpIpProgramme, ProgrammeInfo>()
                .ForMember(a => a.ProgrammeId, (map) => map.MapFrom(b => b.Programmeid))
                .ForMember(a => a.ProgrammeName, (map) => map.MapFrom(b => b.Programmename))
                .ForMember(a => a.TypeId, (map) => map.MapFrom(b => b.Programmetype))
                .ForMember(a => a.emImageType, (map) => map.MapFrom(b => b.Imagetype))
                .ForMember(a => a.nPureAudio, (map) => map.MapFrom(b => b.Pureaudio))
                .AfterMap((b, a) =>
                {
                    a.ProgrammeDesc = MergeTSPgmInfoMultiIPAndPort(b.Programmeindex, b.Multicastip, b.Multicastport, b.Programmedesc, b.Extendparams);
                    a.emPgmType = ProgrammeType.PT_IPTS;
                    a.emSignalSourceType = emSignalSource.emIPTS;
                });
            //V2

            //V1
            CreateMap<DbpStreammedia, ProgrammeInfo>()
                .ForMember(a => a.ProgrammeId, (map) => map.MapFrom(b => b.Streammediaid))
                .ForMember(a => a.ProgrammeName, (map) => map.MapFrom(b => b.Streammedianame))
                .ForMember(a => a.TypeId, (map) => map.MapFrom(b => b.Streammediatype))
                .ForMember(a => a.emImageType, (map) => map.MapFrom(b => b.Imagetype))
                .ForMember(a => a.nPureAudio, (map) => map.MapFrom(b => b.Pureaudio))
                .ForMember(a => a.nCarrierID, (map) => map.MapFrom(b => b.Carrierid))
                .AfterMap((b, a) =>
                {
                    a.ProgrammeDesc = MergeStreamMediaURLAndDesc(b.Streammediaurl, b.Urltype, b.Streammediadesc);
                    a.emPgmType = ProgrammeType.PT_StreamMedia;
                    a.emSignalSourceType = emSignalSource.emStreamMedia;
                });

            CreateMap<DbpIpProgramme, ProgrammeInfoResponse>()
                .ForMember(a => a.ProgrammeId, (map) => map.MapFrom(b => b.Programmeid))
                .ForMember(a => a.ProgrammeName, (map) => map.MapFrom(b => b.Programmename))
                .ForMember(a => a.TypeId, (map) => map.MapFrom(b => b.Programmetype))
                .ForMember(a => a.ImageType, (map) => map.MapFrom(b => b.Imagetype))
                .ForMember(a => a.PureAudio, (map) => map.MapFrom(b => b.Pureaudio))
                .AfterMap((b, a) =>
                {
                    a.ProgrammeDesc = MergeTSPgmInfoMultiIPAndPort(b.Programmeindex, b.Multicastip, b.Multicastport, b.Programmedesc, b.Extendparams);
                    a.PgmType = ProgrammeType.PT_IPTS;
                    a.SignalSourceType = emSignalSource.emIPTS;
                });

            CreateMap<ProgrammeInfoResponse, ProgrammeInfo>()
                .ForMember(a => a.ProgrammeId, (map) => map.MapFrom(b => b.ProgrammeId))
                .ForMember(a => a.ProgrammeName, (map) => map.MapFrom(b => b.ProgrammeName))
                .ForMember(a => a.TypeId, (map) => map.MapFrom(b => b.TypeId))
                .ForMember(a => a.emImageType, (map) => map.MapFrom(b => b.ImageType))
                .ForMember(a => a.nPureAudio, (map) => map.MapFrom(b => b.PureAudio))
                .ForMember(a => a.nCarrierID, (map) => map.MapFrom(b => b.CarrierId))
                .ForMember(a => a.ProgrammeDesc, (map) => map.MapFrom(b => b.ProgrammeDesc))
                .ForMember(a => a.emPgmType, (map) => map.MapFrom(b => b.PgmType))
                .ForMember(a => a.emSignalSourceType, (map) => map.MapFrom(b => b.SignalSourceType))
                .ForMember(a => a.nGroupID, (map) => map.MapFrom(b => b.GroupId));

            //V2

            //V1
            CreateMap<DbpStreammedia, ProgrammeInfoResponse>()
                .ForMember(a => a.ProgrammeId, (map) => map.MapFrom(b => b.Streammediaid))
                .ForMember(a => a.ProgrammeName, (map) => map.MapFrom(b => b.Streammedianame))
                .ForMember(a => a.TypeId, (map) => map.MapFrom(b => b.Streammediatype))
                .ForMember(a => a.ImageType, (map) => map.MapFrom(b => b.Imagetype))
                .ForMember(a => a.PureAudio, (map) => map.MapFrom(b => b.Pureaudio))
                .ForMember(a => a.CarrierId, (map) => map.MapFrom(b => b.Carrierid))
                .AfterMap((b, a) =>
                {
                    a.ProgrammeDesc = MergeStreamMediaURLAndDesc(b.Streammediaurl, b.Urltype, b.Streammediadesc);
                    a.PgmType = ProgrammeType.PT_StreamMedia;
                    a.SignalSourceType = emSignalSource.emStreamMedia;
                });

            CreateMap<SignalGroupStateResponse, SignalGroupState>()
                .ForMember(a => a.signalsrcid, (map) => map.MapFrom(b => b.SignalSrcId))
                .ForMember(a => a.groupid, (map) => map.MapFrom(b => b.GroupId))
                .ForMember(a => a.groupname, (map) => map.MapFrom(b => b.GroupName))
                .ForMember(a => a.groupdesc, (map) => map.MapFrom(b => b.GroupDesc));

            //CreateMap<CaptureChannelInfoResponse, CaptureChannelInfo>()
            //    .ForMember(a => a.nID, (map) => map.MapFrom(b => b.ID))
            //    .ForMember(a => a.strName, (map) => map.MapFrom(b => b.Name))
            //    .ForMember(a => a.strDesc, (map) => map.MapFrom(b => b.Desc))
            //    .ForMember(a => a.nCPDeviceID, (map) => map.MapFrom(b => b.CPDeviceID))
            //    .ForMember(a => a.nChannelIndex, (map) => map.MapFrom(b => b.ChannelIndex))
            //    .ForMember(a => a.nDeviceTypeID, (map) => map.MapFrom(b => b.DeviceTypeID))
            //    .ForMember(a => a.BackState, (map) => map.MapFrom(b => b.BackState))
            //    .ForMember(a => a.nCarrierID, (map) => map.MapFrom(b => b.CarrierID))
            //    .ForMember(a => a.orderCode, (map) => map.MapFrom(b => b.OrderCode))
            //    .ForMember(a => a.nCPSignalType, (map) => map.MapFrom(b => b.CPSignalType))
            //    .ForMember(a => a.nGroupID, (map) => map.MapFrom(b => b.GroupID));
            //CreateMap<DbpTaskMetadata, TaskMetadataResponse>()
            //    .ForMember(a => a.Metadata, (map) => map.MapFrom(b => b.Metadatalong));

            CreateMap<RecUnitMap, DbpChannelRecmap>()
                .ForMember(a => a.Channelid, (map) => map.MapFrom(b => b.ConnectorId))
                .ForMember(a => a.Recid, (map) => map.MapFrom(b => b.UnitId));


            CreateMap<DbpChannelRecmap, RecUnitMap>()
                .ForMember(a => a.ConnectorId, (map) => map.MapFrom(b => b.Channelid))
                .ForMember(a => a.UnitId, (map) => map.MapFrom(b => b.Recid));

            //ReverseMap

            #region XdcamDevice , dbpxdcamdevice
            //v1
            CreateMap<DbpXdcamDevice, XDCAMDeviceInfo>()
                .ForMember(a => a.nDeviceID, (map) => map.MapFrom(b => b.Deviceid))
                .ForMember(a => a.nDeviceType, (map) => map.MapFrom(b => b.Devicetype))
                .ForMember(a => a.strDeviceName, (map) => map.MapFrom(b => b.Devicename))
                .ForMember(a => a.strDeviceDesc, (map) => map.MapFrom(b => b.Devicedesc))
                .ForMember(a => a.strDeviceAddr, (map) => map.MapFrom(b => b.Ftpaddress))
                .ForMember(a => a.strLoginName, (map) => map.MapFrom(b => b.Loginname))
                .ForMember(a => a.strLoginPwd, (map) => map.MapFrom(b => b.Password))
                .ForMember(a => a.strStoragePath, (map) => map.MapFrom(b => b.Storagepath))
                .ForMember(a => a.nWorkMode, (map) => map.MapFrom(b => b.Workmode))
                .ForMember(a => a.nDiscID, (map) => map.MapFrom(b => b.Discid))
                .ForMember(a => a.strServerIP, (map) => map.MapFrom(b => b.Serverip))
                .ForMember(a => a.nDeviceState, (map) => map.MapFrom(b => b.Devicestate));
            //v2
            CreateMap<DbpXdcamDevice, XDCAMDeviceResponse>()
                .ForMember(a => a.DeviceId, (map) => map.MapFrom(b => b.Deviceid))
                .ForMember(a => a.DeviceType, (map) => map.MapFrom(b => b.Devicetype))
                .ForMember(a => a.DeviceName, (map) => map.MapFrom(b => b.Devicename))
                .ForMember(a => a.DeviceDesc, (map) => map.MapFrom(b => b.Devicedesc))
                .ForMember(a => a.DeviceAddr, (map) => map.MapFrom(b => b.Ftpaddress))
                .ForMember(a => a.LoginName, (map) => map.MapFrom(b => b.Loginname))
                .ForMember(a => a.LoginPwd, (map) => map.MapFrom(b => b.Password))
                .ForMember(a => a.StoragePath, (map) => map.MapFrom(b => b.Storagepath))
                .ForMember(a => a.WorkMode, (map) => map.MapFrom(b => b.Workmode))
                .ForMember(a => a.DiscId, (map) => map.MapFrom(b => b.Discid))
                .ForMember(a => a.ServerIp, (map) => map.MapFrom(b => b.Serverip))
                .ForMember(a => a.DeviceState, (map) => map.MapFrom(b => b.Devicestate));

            #endregion

        }

        public string MergeTSPgmInfoMultiIPAndPort(int? pgmIndex, string multiIP, int? multiPort, string desc, string extendParams)
        {
            StringBuilder strBuilder = new StringBuilder();

            using (StringWriter sw = new StringWriter(strBuilder))
            {
                using (XmlTextWriter writer = new XmlTextWriter(sw))
                {
                    writer.Formatting = Formatting.None;

                    writer.WriteStartElement("TSPgmInfo");
                    writer.WriteElementString("PgmIndex", pgmIndex.ToString());
                    writer.WriteElementString("MultiIPAddress", multiIP);
                    writer.WriteElementString("MultiPort", multiPort.ToString());
                    writer.WriteElementString("Desc", desc);
                    writer.WriteRaw(extendParams);

                    writer.WriteEndElement();
                    writer.Flush();

                    writer.Close();
                }
                sw.Close();
            }

            return strBuilder.ToString();
        }

        public string MergeStreamMediaURLAndDesc(string url, string urlType, string desc)
        {
            StringBuilder strBuilder = new StringBuilder();

            using (StringWriter sw = new StringWriter(strBuilder))
            {
                XmlTextWriter writer = new XmlTextWriter(sw);
                writer.Formatting = Formatting.None;

                writer.WriteStartElement("StreamMedia");
                writer.WriteElementString("URL", url);
                writer.WriteElementString("URLType", urlType);
                writer.WriteElementString("Desc", desc);

                writer.WriteEndElement();
                writer.Flush();

                writer.Close();
                sw.Close();
            }

            return strBuilder.ToString();
        }
    }
}
