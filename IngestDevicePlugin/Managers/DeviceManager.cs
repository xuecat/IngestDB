using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using IngestDBCore;
using IngestDevicePlugin.Dto;
using IngestDevicePlugin.Models;
using IngestDevicePlugin.Stores;
using Microsoft.EntityFrameworkCore;
using Remotion.Linq.Parsing;
using Sobey.Core.Log;

namespace IngestTaskPlugin.Managers
{
    public class DeviceManager
    {
        public DeviceManager(IDeviceStore store, IMapper mapper)
        {
            Store = store;
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        private readonly ILogger Logger = LoggerManager.GetLogger("DeviceInfo");

        /// <summary> 设备（仓储） </summary>
        protected IDeviceStore Store { get; }

        /// <summary> 数据映射器 </summary>
        protected IMapper _mapper { get; }

        #region Private

        /// <summary>可更新 设备状态 集合</summary>
        private readonly int[] UpdateDevState = { (int)Device_State.DISCONNECTTED, (int)Device_State.CONNECTED, (int)Device_State.WORKING };
        /// <summary>可更新 MSV模式 集合</summary>
        private readonly int[] UpdateMSV_Mode = { (int)MSV_Mode.LOCAL, (int)MSV_Mode.NETWORK };

        /// <summary>根据 ProgrammeType 获取 SignalSource</summary>
        private async Task UpdateSignalSourceForProgramme(List<ProgrammeInfo> infos)
        {
            foreach (var info in infos)
            {
                switch (info.emPgmType)
                {
                    case ProgrammeType.PT_SDI:
                        var inport = await Store.GetRcdindescAsync(async a => await a.FirstOrDefaultAsync(x => x.Signalsrcid == info.ProgrammeId), true);
                        if (inport != null)
                        {
                            info.emSignalSourceType = (emSignalSource)inport.Signalsource;
                        }
                        break;

                    case ProgrammeType.PT_IPTS:
                        info.emSignalSourceType = emSignalSource.emIPTS;
                        break;

                    case ProgrammeType.PT_StreamMedia:
                        info.emSignalSourceType = emSignalSource.emStreamMedia;
                        break;

                    default:
                        break;
                }
            }
        }

        #endregion Private

        /// <summary> 获取所有输入端口与信号源 </summary>
        public virtual async Task<List<TResult>> GetAllRouterInPortAsync<TResult>()
        {
            return _mapper.Map<List<TResult>>(await Store.GetRcdindescAsync(a => a, true));
        }

        /// <summary> 获取所有输出端口与信号源 </summary>
        public virtual async Task<List<TResult>> GetAllRouterOutPortAsync<TResult>()
        {
            return _mapper.Map<List<TResult>>(await Store.GetRcdoutdescAsync(a => a, true));
        }

        /// <summary> 获取所有信号源和采集设备 </summary>
        public virtual async Task<List<TResult>> GetAllSignalDeviceMapAsync<TResult>()
        {
            return _mapper.Map<List<TResult>>(await Store.GetSignalDeviceMapAsync(a => a, true));
        }

        /// <summary> 获取所有信号源 </summary>
        public virtual async Task<List<SignalSrcInfo>> GetAllSignalSrcsAsync()
        {
            //获取输入端口信号源的id
            var allRcdindesc = await Store.GetRcdindescAsync(a => a.Select(x => x.Signalsrcid), true);
            return _mapper.Map<List<SignalSrcInfo>>(await Store.GetSignalsrcAsync(a => a.Where(x => allRcdindesc.Contains(x.Signalsrcid)), true));
        }

        /// <summary> 获取所有信号源的扩展信息 </summary>
        public virtual async Task<List<SignalSrcExInfo>> GetAllSignalSrcExsAsync()
        {
            var allSignalSrcExs = _mapper.Map<List<SignalSrcExInfo>>(await Store.GetSignalSrcExsAsync(a => a, true));
            if (allSignalSrcExs.Count > 0)
            {
                allSignalSrcExs.Where(a => a.bIsMainSignalSrc).ToList().ForEach(a =>
                {
                    //找到上级Id为当前nId的，说明此信号为当前的下级
                    var signalSrcEx = allSignalSrcExs.FirstOrDefault(x => x.nMainSignalSrcId == a.nID);
                    if (signalSrcEx != null)
                    {
                        a.nBackupSignalSrcId = signalSrcEx.nID;
                    }
                });
            }
            return allSignalSrcExs;
        }

        /// <summary> 获取所有采集通道 </summary>
        public virtual async Task<List<CaptureChannelInfo>> GetAllCaptureChannelsAsync()
        {
            //获取输出端口的通道Id
            var allRcdoutId = await Store.GetRcdoutdescAsync(a => a.Select(x => x.Channelid), true);
            var captureChannelList = _mapper.Map<List<CaptureChannelInfo>>(await Store.GetCapturechannelsAsync(a => a.Where(x => allRcdoutId.Contains(x.Channelid)), true));
            if (captureChannelList.Count > 0)//获取采集设备信息,修改orderCode
            {
                var captureDevices = await Store.GetCapturedeviceAsync(a => a.Where(x => x.Ordercode != null).Select(x => new { x.Cpdeviceid, x.Ordercode }), true);
                foreach (var channelInfo in captureChannelList)
                {
                    var devices = captureDevices.FirstOrDefault(a => a.Cpdeviceid == channelInfo.nCPDeviceID);
                    if (devices != null)
                    {
                        channelInfo.orderCode = devices.Ordercode ?? -1;
                    }
                }
            }
            //添加虚拟通道
            var virtualChannels = _mapper.Map<List<CaptureChannelInfo>>(await Store.GetIpVirtualchannelAsync(a => a, true));
            captureChannelList.AddRange(virtualChannels);
            //获取Group信息
            var allChannelGroupMap = await Store.GetChannelgroupmapAsync(a => a, true);
            foreach (var channelInfo in captureChannelList)
            {
                var groupMap = allChannelGroupMap.FirstOrDefault(a => a.Channelid == channelInfo.nID);
                if (groupMap != null)
                {
                    channelInfo.nGroupID = groupMap.Groupid;
                }
            }
            return captureChannelList;
        }

        /// <summary> 根据 通道ID 获取采集通道 </summary>
        /// <param name="nID">通道Id</param>
        public virtual async Task<CaptureChannelInfo> GetCaptureChannelByIDAsync(int nID)
        {
            var captureChannel = _mapper.Map<CaptureChannelInfo>(await Store.GetCapturechannelsAsync(async a => await a.FirstOrDefaultAsync(x => x.Channelid == nID), true));
            if (captureChannel == null)
            {
                captureChannel = _mapper.Map<CaptureChannelInfo>(await Store.GetIpVirtualchannelAsync(async a => await a.FirstOrDefaultAsync(x => x.Channelid == nID), true));
                if (captureChannel == null)
                {
                    SobeyRecException.ThrowSelfNoParam(nameof(GetCaptureChannelByIDAsync), GlobalDictionary.GLOBALDICT_CODE_CHANNEL_ID_DOES_NOT_EXIST, Logger, null);
                }
            }
            return captureChannel;
        }

        /// <summary> 获取所有采集设备 </summary>
        public virtual async Task<List<TResult>> GetAllCaptureDevicesAsync<TResult>()
        {
            return _mapper.Map<List<TResult>>(await Store.GetCapturedeviceAsync(a => a.OrderBy(x => x.Ordercode), true));
        }

        /// <summary>获取所有TS设备信息</summary>
        public virtual async Task<List<TSDeviceInfo>> GetAllTSDeviceInfosAsync()
        {
            var deviceList = _mapper.Map<List<TSDeviceInfo>>(await Store.GetIpDeviceAsync(a => a, true));
            if (deviceList.Count > 0)
            {
                var deviceIds = deviceList.Select(a => a.DeviceId).ToList();
                #region TSVirtualChannelInfo

                var virtualGroup = _mapper.Map<List<IGrouping<int, TSVirtualChannelInfo>>>(await Store.GetIpVirtualchannelAsync(a => a.Where(x => deviceIds.Contains(x.Deviceid)).GroupBy(x => x.Deviceid), true));
                foreach (var vir in virtualGroup)
                {
                    deviceList.First(a => a.DeviceId == vir.Key).ChannelInfos = vir.ToList();
                }

                #endregion

                #region TSDataChannelInfo

                var dataList = _mapper.Map<List<TSDataChannelInfo>>(await Store.GetIpDatachannelsAsync(a => a.Where(x => deviceIds.Contains(x.Deviceid)), true));
                if (dataList.Count > 0)
                {
                    var channelIds = dataList.Select(a => a.DataChannelId).ToList();
                    #region TSPgmInfo

                    var tsPgmGroup = _mapper.Map<List<IGrouping<int, TSPgmInfo>>>(await Store.GetIpProgrammeAsync(a => a.Where(x => channelIds.Contains(x.Datachannelid)).GroupBy(x => x.Datachannelid), true));
                    foreach (var pgm in tsPgmGroup)
                    {
                        dataList.First(a => a.DataChannelId == pgm.Key).PgmInfos = pgm.ToList();
                    }
                    #endregion
                    var dataGroup = dataList.GroupBy(x => x.DeviceId);
                    foreach (var data in dataGroup)
                    {
                        deviceList.First(a => a.DeviceId == data.Key).DataChannelInfos = data.ToList();
                    }
                }

                #endregion

            }
            return deviceList;
        }

        /// <summary> 通过信号Id获取设备映射信息 </summary>
        /// <param name="nSignalID">信号Id</param>
        public virtual async Task<SignalDeviceMap> GetSignalDeviceMapBySignalID(int nSignalID)
        {
            return _mapper.Map<SignalDeviceMap>(await Store.GetSignalDeviceMapAsync(async a => await a.FirstOrDefaultAsync(x => x.Signalsrcid == nSignalID), true));
        }

        /// <summary> 查询信号源是否是备份信号源 </summary>
        /// <param name="nSignalID">信号Id</param>
        public virtual async Task<bool> IsBackupSignalSrcByIdAsync(int nSignalID)
        {
            return await Store.GetSignalSrcExsAsync(async a => await a.AnyAsync(x => x.Signalsrcid == nSignalID && x.Ismastersrc == 0), true);
        }

        /// <summary>
        /// 设置信号源和采集设备的对应
        /// </summary>
        /// <param name="nSignalID">信号Id</param>
        /// <param name="nDeviceID">设备Id</param>
        /// <param name="nDeviceOutPortIdx">设备输出端口索引</param>
        /// <param name="SignalSource">信号源</param>
        /// <returns></returns>
        public virtual async Task SaveSignalDeviceMapAsync(int nSignalID, int nDeviceID, int nDeviceOutPortIdx, emSignalSource SignalSource)
        {
            await Store.SaveSignalDeviceMapAsync(new IngestDevicePlugin.Models.DbpSignalDeviceMap
            {
                Signalsrcid = nSignalID,
                Signalsource = (int)SignalSource,
                Deviceid = nDeviceID,
                Deviceoutportidx = nDeviceOutPortIdx
            });
        }

        /// <summary> 根据 信号ID 获取高清还是标清 nType:0标清,1高清 </summary>
        public virtual async Task<int> GetParamTypeByChannleIDAsync(int nSignalID)
        {
            var nType = await Store.GetParamTypeByChannleIDAsync(nSignalID);
            return nType ?? -1;
        }

        /// <summary> 根据 通道ID 获取MSV设备状态信息 </summary>
        public virtual async Task<MSVChannelState> GetMsvChannelStateAsync(int nChannelID)
        {
            return _mapper.Map<MSVChannelState>(await Store.GetMsvchannelStateAsync(async a => await a.FirstOrDefaultAsync(x => x.Channelid == nChannelID), true));
        }

        /// <summary> 获得所有通道的状态 </summary>
        public virtual async Task<List<MSVChannelState>> GetAllChannelStateAsync()
        {
            return _mapper.Map<List<MSVChannelState>>(await Store.GetMsvchannelStateAsync(a => a, true));
        }

        /// <summary> 获得所有信号源分组 </summary>
        public virtual async Task<List<AllSignalGroup>> GetAllSignalGroupAsync()
        {
            return _mapper.Map<List<AllSignalGroup>>(await Store.GetSignalGroupAsync(a => a, true));
        }

        /// <summary> 获取所有信号源分组信息 </summary>
        public virtual async Task<List<SignalGroupState>> GetAllSignalGroupInfoAsync()
        {
            return await Store.GetAllSignalGroupInfoAsync();
        }

        /// <summary> 通过 GPIID 找出该GPI所有的映射 </summary>
        public virtual async Task<List<GPIDeviceMapInfo>> GetGPIMapInfoByGPIIDAsync(int nGPIID)
        {
            return _mapper.Map<List<GPIDeviceMapInfo>>(await Store.GetGPIMapInfoByGPIIDAsync(a => a.Where(x => x.Gpiid == nGPIID).OrderBy(x => x.Gpioutputport), true));
        }

        /// <summary> 获取所有节目 </summary>
        public virtual async Task<List<ProgrammeInfo>> GetAllProgrammeInfosAsync()
        {
            var signalsrcids = await Store.GetRcdindescAsync(a => a.Select(x => x.Signalsrcid), true);
            var programmeInfos = _mapper.Map<List<ProgrammeInfo>>(await Store.GetSignalsrcAsync(a => a.Where(x => signalsrcids.Contains(x.Signalsrcid)), true));

            var inportDescInfos = await Store.GetRcdindescAsync(a => a, true);
            foreach (var info in programmeInfos)
            {
                var inport = inportDescInfos.FirstOrDefault(a => a.Signalsrcid == info.ProgrammeId);
                if (inport != null)
                {
                    info.emSignalSourceType = (emSignalSource)inport.Signalsource;
                }
            }

            var allTSPgmInfo = _mapper.Map<List<ProgrammeInfo>>(await Store.GetIpProgrammeAsync(a => a, true));
            programmeInfos.AddRange(allTSPgmInfo);

            var allStreamMedia = _mapper.Map<List<ProgrammeInfo>>(await Store.GetStreamMediaAsync(a => a, true));
            programmeInfos.AddRange(allStreamMedia);

            var allSignalGroupStates = await Store.GetAllSignalGroupInfoAsync();

            foreach (ProgrammeInfo pgInfo in programmeInfos)
            {
                pgInfo.nGroupID = -1;
                var state = allSignalGroupStates.FirstOrDefault(a => a.signalsrcid == pgInfo.ProgrammeId);
                if (state != null)
                    pgInfo.nGroupID = state.groupid;
            }
            return programmeInfos;
        }

        /// <summary> 根据通道获取相应的节目 </summary>
        public virtual async Task<List<ProgrammeInfo>> GetProgrammeInfosByChannelIdAsync(int channelId)
        {
            CaptureChannelInfo channelInfo = await GetCaptureChannelByIDAsync(channelId);
            if (channelInfo.nDeviceTypeID == (int)CaptureChannelType.emMsvChannel && !await HaveMatrixAsync())
            {
                var signalIds = await Store.GetSignalIdsByChannelIdForNotMatrix(channelId);
                var programmeInfoList = _mapper.Map<List<ProgrammeInfo>>(await Store.GetSignalsrcAsync(a => a.Where(x => signalIds.Contains(x.Signalsrcid)), true));
                await UpdateSignalSourceForProgramme(programmeInfoList);
                return programmeInfoList;
            }
            {
                int signalId = 0;
                if (channelInfo.nDeviceTypeID == (int)CaptureChannelType.emMsvChannel)
                {
                    signalId = await GetChannelSignalSrc(channelInfo.nID);
                }
                return GetProgrammeInfoListMatrix(channelInfo, signalId, await GetAllProgrammeInfosAsync());
            }
        }

        /// <summary>获取筛选条件</summary>
        private List<ProgrammeInfo> GetProgrammeInfoListMatrix(CaptureChannelInfo channelInfo, int signalId, IEnumerable<ProgrammeInfo> programmeInfos)
        {
            switch (channelInfo.nCPSignalType)
            {
                case 0: //是Auto
                    break;

                case 1: //SD
                    programmeInfos = programmeInfos.Where(info => info.TypeId != 1);//排除HD，保留Auto和SD
                    break;

                case 2: //HD
                    programmeInfos = programmeInfos.Where(info => info.TypeId != 0);//排除SD，保留HD和Auto
                    break;

                default:
                    return default;
            }
            switch (channelInfo.nGroupID)
            {
                case -1: //通道没有分组信息，则所有的信号源均可匹配，故这不做任何操作
                    break;

                default:// 通道有分组，需要排除不同分组的信号源
                    programmeInfos = programmeInfos.Where(info =>
                    {
                        var groupmap = Store.GetSignalsrcgroupmapAsync(async a => await a.Where(x => x.Signalsrcid == info.ProgrammeId).FirstOrDefaultAsync(), true).Result;
                        return !(groupmap != null && groupmap.Groupid == -1 && channelInfo.nGroupID != groupmap.Groupid);
                    });
                    break;
            }
            switch (channelInfo.nDeviceTypeID)
            {
                case (int)CaptureChannelType.emMsvChannel:
                    programmeInfos = programmeInfos.Where(info => info.emPgmType == ProgrammeType.PT_SDI);
                    var firstList = programmeInfos.Where(info => info.ProgrammeId == signalId).ToList();
                    var lastList = programmeInfos.Where(info => info.ProgrammeId != signalId);
                    firstList.AddRange(lastList);
                    return firstList;

                case (int)CaptureChannelType.emIPTSChannel:
                    return programmeInfos.Where(info => info.emPgmType == ProgrammeType.PT_IPTS).ToList();

                case (int)CaptureChannelType.emStreamChannel:
                    return programmeInfos.Where(info => info.emPgmType == ProgrammeType.PT_StreamMedia).ToList();

                default:
                    return default;
            }
        }

        /// <summary>根据 通道Id 获取 信号id</summary>
        /// <param name="channelid">通道Id</param>
        public virtual async Task<int> GetChannelSignalSrc(int channelid)
        {
            if (!await HaveMatrixAsync())
            {
                CaptureChannelInfo channelInfo = await GetCaptureChannelByIDAsync(channelid);

                if (channelInfo.nDeviceTypeID == (int)CaptureChannelType.emMsvChannel)
                {
                    var signalIds = await Store.GetSignalIdsByChannelIdForNotMatrix(channelid);

                    if (signalIds.Count > 0)
                    {
                        return signalIds[0];
                    }
                }
            }
            return await Store.GetMatrixChannelBySignalAsync(channelid);
        }


        public virtual async Task<int> GetBackupSignalSrcIdByIdAsync(int nID)
        {
            return await Store.GetSignalsrcMasterbackupAsync(a => a
                                                                    .Where(x => x.Mastersignalsrcid == nID)
                                                                    .Select(x => x.Signalsrcid)
                                                                    .FirstOrDefaultAsync(), true);
        }


        public virtual async Task<bool> HaveMatrixAsync()
        {
            return await Store.HaveMatirxAsync();
        }

        #region Update

        /// <summary>保存 OR 更新 通道扩展信息</summary>
        public virtual async Task<bool> SaveChnExtenddataAsync(UpdateChnExtData_IN pIn)
        {
            if (pIn.strData == null)
                pIn.strData = "";
            pIn.strData = pIn.strData.Replace("\\", "\\\\");
            return await Store.SaveChannelExtenddataAsync(pIn) > 0;
        }

        /// <summary>更改MSV设备状态信息</summary>
        /// <param name="nID">通道Id</param>
        /// <param name="nDevState">设备状态</param>
        /// <param name="nMSVMode">MSV模式</param>
        /// <returns></returns>
        public virtual async Task UpdateMSVChannelStateAsync(int nID, int nDevState, int nMSVMode)
        {
            var state = await Store.GetMsvchannelStateAsync(async a => await a.FirstAsync(x => x.Channelid == nID));
            if (state != null)
            {
                if (UpdateDevState.Contains(nDevState))
                {
                    state.Devstate = nDevState;
                }
                if (UpdateMSV_Mode.Contains(nMSVMode))
                {
                    state.Msvmode = nMSVMode;
                }
                await Store.UpdateMSVChannelStateAsync(state);
            }
            SobeyRecException.ThrowSelfNoParam(nameof(UpdateMSVChannelStateAsync), GlobalDictionary.GLOBALDICT_CODE_DATA_NOTFOUND_BYKEY, Logger, null);
        }



        /// <summary>更新所有的IP收录的设备</summary>
        public virtual async Task<bool> UpdateAllTSDeviceInfosAsync(TSDeviceInfo[] tsDeviceInfos)
        {
            if (tsDeviceInfos != null && tsDeviceInfos.Length > 0)
            {
                var virtualList = _mapper.Map<List<DbpIpVirtualchannel>>(tsDeviceInfos.SelectMany(a => a.ChannelInfos));
                if (virtualList.Count > 0)
                {
                    await Store.SaveIpVirtualchannelAsync(virtualList);
                }
                var dataList = _mapper.Map<List<DbpIpDatachannelinfo>>(tsDeviceInfos.SelectMany(a => a.DataChannelInfos));
                var pgmList = _mapper.Map<List<DbpIpProgramme>>(tsDeviceInfos.SelectMany(a => a.DataChannelInfos.SelectMany(x => x.PgmInfos)));
                if (pgmList.Count > 0)
                {
                    await Store.SaveIpProgrammeAsync(pgmList);
                }
                if (virtualList.Count > 0)
                {
                    await Store.SaveIpDatachannelinfoAsync(dataList);
                }
                var deviceList = _mapper.Map<List<DbpIpDevice>>(tsDeviceInfos);
                await Store.SaveIpDatachannelinfoAsync(dataList);
            }
            return true;
        }

        #endregion Update


        //#region MyRegion

        public async Task<ProgrammeInfoResponse> GetProgrammeInfoByIdAsync(int programeid)
        {
            return await Store.GetSignalInfoAsync(programeid);
        }

        //public async Task<int> GetChannelSignalSrc(int channelid)
        //{
        //    //判断是否是无矩阵
        //    bool isHaveMatrix = await HaveMatrixAsync();
        //    if (!isHaveMatrix)
        //    {
        //        var item = await Store.GetCaptureChannelByIDAsync(channelid);
        //        if (item != null && item.DeviceTypeID == (int)CaptureChannelType.emMsvChannel)
        //        {
        //            return (await Store.GetSignalIdsByChannelIdForNotMatrix(channelid)).ElementAt(0);
        //        }
        //    }
        //    else
        //        return await Store.GetMatrixChannelBySignalAsync(channelid);

        //    Logger.Error("GetChannelSignalSrc error getno");
        //    return 0;
        //}

        //Todo :待确认
        public async virtual Task<List<TResult>> GetChannelsByProgrammeIdAsync<TResult>(int programmid, int state)
        {
            //判断是否是无矩阵
            bool isHaveMatrix = await HaveMatrixAsync();
            var programinfo = await GetProgrammeInfoByIdAsync(programmid);

            var channels = await Store.GetAllCaptureChannelsAsync(state);
            List<CaptureChannelInfoResponse> channelInfoList = new List<CaptureChannelInfoResponse>();
            foreach (var item in channels)
            {
                ////类型匹配
                if (!((programinfo.PgmType == ProgrammeType.PT_SDI && (item.DeviceTypeID == (int)CaptureChannelType.emMsvChannel))
                     || ((programinfo.PgmType == ProgrammeType.PT_IPTS) && (item.DeviceTypeID == (int)CaptureChannelType.emIPTSChannel))
                     || ((programinfo.PgmType == ProgrammeType.PT_StreamMedia) && (item.DeviceTypeID == (int)CaptureChannelType.emStreamChannel))))
                {
                    continue;
                }

                //高标清匹配
                if (item.CPSignalType > 0)//0表示Auto，可以任意匹配，不需要处理,1:SD, 2:HD
                {
                    if (item.CPSignalType == 1)//SD
                    {
                        if (programinfo.TypeId == 1)//排除HD，Auto和SD可以匹配
                            continue;
                    }
                    else if (item.CPSignalType == 2)//HD
                    {
                        if (programinfo.TypeId == 0)//排除SD，保留HD和Auto
                            continue;
                    }
                }

                bool isNeedAdd = true;
                if (programinfo.PgmType == ProgrammeType.PT_SDI)
                {
                    if (!isHaveMatrix)
                    {
                        //需要根据列表对通道进行判断
                        var channelIdListInNotMatrix = await Store.GetChannelIdsBySignalIdForNotMatrix(programmid);

                        isNeedAdd = false;
                        if (channelIdListInNotMatrix.Any(x => x == item.ID))
                        {
                            isNeedAdd = true;
                        }
                    }
                }

                if (isNeedAdd)
                {
                    channelInfoList.Add(item);
                }
            }

            return _mapper.Map<List<TResult>>(channelInfoList);
        }

        public async Task<List<RecUnitMap>> GetAllChannelUnitMap()
        {
            return _mapper.Map<List<RecUnitMap>>(await Store.GetAllChannelUnitMap());
        }
        public async Task<RecUnitMap> GetChannelUnitMap(int channel)
        {
            return _mapper.Map<RecUnitMap>(await Store.GetChannelUnitMap(channel));
        }
        //#endregion
    }
}
