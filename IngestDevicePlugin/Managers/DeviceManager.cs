using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using IngestDBCore;
using IngestDBCore.Interface;
using IngestDevicePlugin.Dto;
using IngestDevicePlugin.Dto.Enum;
using IngestDevicePlugin.Dto.Response;
using IngestDevicePlugin.Extend;
using IngestDevicePlugin.Models;
using IngestDevicePlugin.Stores;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Remotion.Linq.Parsing;
using Sobey.Core.Log;
using taskState = IngestDevicePlugin.Dto.Enum.taskState;

namespace IngestDevicePlugin.Managers
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

        /// <summary>获取信号源</summary>
        private async Task MapSignalSourceForProgramme(List<ProgrammeInfo> infos)
        {
            var sdiList = infos.Where(a => a.emPgmType == ProgrammeType.PT_SDI).ToList();
            var sdiIds = sdiList.Select(a => a.ProgrammeId);
            var inports = await Store.GetRcdindescAsync(a => a.Where(x => sdiIds.Contains(x.Signalsrcid)), true);
            foreach (var info in sdiList)
            {
                info.emSignalSourceType = (emSignalSource)inports.First(a => a.Signalsrcid == info.ProgrammeId).Signalsource;
            }
            var iptsList = infos.Where(a => a.emPgmType == ProgrammeType.PT_IPTS).ToList();
            foreach (var info in iptsList)
            {
                info.emSignalSourceType = emSignalSource.emIPTS;
            }
            var streamList = infos.Where(a => a.emPgmType == ProgrammeType.PT_StreamMedia).ToList();
            foreach (var info in streamList)
            {
                info.emSignalSourceType = emSignalSource.emStreamMedia;
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
            return _mapper.Map<List<SignalSrcInfo>>(await Store.GetAllSignalsrcForRcdinAsync(true));
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

        public async Task<ProgrammeInfoResponse> GetBackProgramInfoBySrgid(int srgid)
        {
            return await Store.GetSignalInfoAsync(await Store.GetBackUpSignalInfoByID(srgid));
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
        public virtual async Task<bool> SaveSignalDeviceMapAsync(int nSignalID, int nDeviceID, int nDeviceOutPortIdx, emSignalSource SignalSource)
        {
            await Store.SaveSignalDeviceMapAsync(new IngestDevicePlugin.Models.DbpSignalDeviceMap
            {
                Signalsrcid = nSignalID,
                Signalsource = (int)SignalSource,
                Deviceid = nDeviceID,
                Deviceoutportidx = nDeviceOutPortIdx
            });
            return true;
        }

        /// <summary> 根据 通道ID 获取高清还是标清 nType:0标清,1高清 </summary>
        public virtual async Task<int> GetParamTypeByChannleIDAsync(int nChannelID)
        {
            var nType = await Store.GetParamTypeByChannleIDAsync(nChannelID);
            return nType ?? -1;
        }

        /// <summary>根据信号源获取是高清还是标清</summary>
        public virtual async Task<int> GetParamTypeBySignalIDAsync(int nSignalID)
        {
            var id = await Store.GetParamTypeBySignalIDAsync(nSignalID);
            return id ?? -1;
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
        public virtual async Task<List<TResult>> GetGPIMapInfoByGPIIDAsync<TResult>(int nGPIID)
        {
            return _mapper.Map<List<TResult>>(await Store.GetGPIMapInfoByGPIIDAsync(a => a.Where(x => x.Gpiid == nGPIID).OrderBy(x => x.Gpioutputport), true));
        }

        /// <summary> 获取所有节目 </summary>
        public virtual async Task<List<ProgrammeInfo>> GetAllProgrammeInfosAsync()
        {
            var programmeInfos = _mapper.Map<List<ProgrammeInfo>>(await Store.GetAllSignalsrcForRcdinAsync(true));

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
                await MapSignalSourceForProgramme(programmeInfoList);
                return programmeInfoList;
            }
            else
            {
                int signalId = 0;
                if (channelInfo.nDeviceTypeID == (int)CaptureChannelType.emMsvChannel)
                {
                    signalId = await GetChannelSignalSrcAsync(channelInfo.nID);
                }
                return GetProgrammeInfoListMatrix(channelInfo, signalId, await GetAllProgrammeInfosAsync());
            }
        }

        /// <summary>获取筛选结果</summary>
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
                    return null;
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
                    return null;
            }
        }

        /// <summary>根据 通道Id 获取 信号id</summary>
        /// <param name="channelid">通道Id</param>
        public virtual async Task<int> GetChannelSignalSrcAsync(int channelid)
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

        public virtual async Task<int> GetBestChannelIdBySignalIDAsync(int signalID, string userCode)
        {
            var signalsrcs = await Store.GetAllSignalsrcForRcdinAsync(true);
            if (signalsrcs == null && signalsrcs.Count <= 0)
            {
                return 0;
            }
            DateTime dtNow = DateTime.Now;
            var _globalinterface = ApplicationContext.Current.ServiceProvider.GetRequiredService<IIngestTaskInterface>();
            if (_globalinterface != null)
            {
                var channelIds = await GetUserHiddenChannels(userCode);     //获得该用户的隐藏通道
                List<MSVChannelState> arrMsvChannelState =
                    _mapper.Map<List<MSVChannelState>>(
                        await Store.GetMsvchannelStateAsync(a => a, true)); //获得所有通道的状态，查看是否在做KAMATAKI任务
                //获得所有采集通道
                List<CaptureChannelInfo> captureChannels = await GetAllCaptureChannelsAsync();
                //获得将要和正在执行的任务
                TaskInternals reWillBeginAndCapturingTasks = new TaskInternals() { funtype = IngestDBCore.TaskInternals.FunctionType.WillBeginAndCapturingTasks };
                var willResponse = await _globalinterface.GetTaskCallBack(reWillBeginAndCapturingTasks);
                var taskContents = (willResponse as IngestDBCore.ResponseMessage<List<TaskContentInterface>>)?.Ext;

                //获得当前任务
                TaskInternals reCurrentTasks = new TaskInternals() { funtype = IngestDBCore.TaskInternals.FunctionType.CurrentTasks };
                var currentResponse = await _globalinterface.GetTaskCallBack(reCurrentTasks);
                var currentTasks = (currentResponse as IngestDBCore.ResponseMessage<List<TaskContentInterface>>)?.Ext;
                var curIds = currentTasks?.Select(a => a.nChannelID).ToList();
                //获得当前通道与信号源的映射
                var channel2SignalSrcMaps = await Store.GetAllChannel2SignalSrcMapAsync();//获得当前通道与信号源的映射

                var captureChannelInfos = captureChannels.Where(a => channelIds.Contains(a.nID) &&
                                                                     arrMsvChannelState.Any(s => s.nChannelID == a.nID &&
                                                                                                 s.emMSVMode == MSV_Mode.NETWORK &&
                                                                                                 s.emDevState != Device_State.DISCONNECTTED &&
                                                                                                 string.IsNullOrEmpty(s.kamatakiInfo)) &&
                                                                                                 curIds.Contains(a.nID))
                                                         .Select(a => new ChannelScore { Id = a.nID }).ToList();
                if (captureChannelInfos.Count > 0)
                    return 0;
                foreach (var channel in captureChannelInfos)
                {
                    var bIsExist = false;
                    var task = taskContents?.FirstOrDefault(t => t != null && t.nChannelID == channel.Id);
                    if (task != null)
                    {
                        bIsExist = true;
                        double totalSeconds = (task.strBegin.ToDateTime() - dtNow).TotalSeconds;
                        if (totalSeconds <= 10)//最近10s钟要运行的任务，那么将分值设置为负值
                        {
                            channel.Score = -1;
                        }
                        if (totalSeconds > 2 * 60 * 60)//2个小时
                        {
                            totalSeconds = 2 * 60 * 60;
                        }
                        channel.Score = totalSeconds / 12;
                    }
                    if (!bIsExist)//没有查到，估计是2个小时外的
                    {
                        channel.Score = 600;
                    }
                    bIsExist = false;
                    var srcMaps = channel2SignalSrcMaps.FirstOrDefault(a => channel.Score >= 0 && a.nChannelID == channel.Id);
                    if (srcMaps != null)
                    {
                        bIsExist = true;//连接上通过的
                        string lastUserCode = string.Empty;
                        TimeSpan tsLong = dtNow - GetChannelLastOperTime(srcMaps.nChannelID, out lastUserCode);
                        double totalSeconds = (double)tsLong.TotalSeconds;
                        //如果是刚刚用户分配到的，这项得分也是满分
                        if (string.Compare(userCode, lastUserCode, true) == 0)
                        {
                            totalSeconds = 600;
                        }

                        if (totalSeconds > 600)
                        {
                            totalSeconds = 600;
                        }
                        channel.Score += ((double)totalSeconds) / 2;
                    }

                    if (channel.Score >= 0 && !bIsExist)//没有连接的，人为加上100分
                    {
                        channel.Score += 100;
                    }
                }

                captureChannelInfos = captureChannelInfos.OrderByDescending(a => a.Score).ToList();

                return captureChannelInfos.Where(a => a.Score >= 0).Select(a => a.Id).ToList()?[0] ?? 0;
            }
            return 0;
        }

        //TODO:未找到此表 获得通道的最后被分配的时间
        private DateTime GetChannelLastOperTime(int channelID, out string lastUserCode)
        {
            DateTime lastOperTime = DateTime.MinValue;
            lastUserCode = string.Empty;

            //DevicesSet.DBP_CHANNEL_DISTRIBUTIONIFNORow row = ds4ChannelDistributionInfo.DBP_CHANNEL_DISTRIBUTIONIFNO.FindByCHANNELID((decimal)channelID);

            //if (row != null)
            //{
            //    lastOperTime = row.IsLASTOPERTIMENull() ? DateTime.MinValue : row.LASTOPERTIME;
            //    lastUserCode = row.IsLASTUSERCODENull() ? string.Empty : row.LASTUSERCODE;
            //}
            lastUserCode = string.Empty;
            return lastOperTime;
        }

        //TODO:未找到此表 设置通道的最后被分配的时间
        private bool SetChannelLastOperTime(int channelID, DateTime lastOperTime, string userCode)
        {
            if (channelID <= 0)
            {
                return false;
            }

            //if (ds4ChannelDistributionInfo == null)
            //{
            //    ds4ChannelDistributionInfo = new DevicesSet();
            //}

            //DevicesSet.DBP_CHANNEL_DISTRIBUTIONIFNORow row = ds4ChannelDistributionInfo.DBP_CHANNEL_DISTRIBUTIONIFNO.FindByCHANNELID((decimal)channelID);

            //if (row != null)
            //{
            //    row.LASTOPERTIME = lastOperTime;
            //    row.LASTUSERCODE = userCode;
            //}
            //else
            //{
            //    DevicesSet.DBP_CHANNEL_DISTRIBUTIONIFNORow newRow = ds4ChannelDistributionInfo.DBP_CHANNEL_DISTRIBUTIONIFNO.NewDBP_CHANNEL_DISTRIBUTIONIFNORow();
            //    newRow.CHANNELID = (decimal)channelID;
            //    newRow.LASTOPERTIME = lastOperTime;
            //    newRow.LASTUSERCODE = userCode;

            //    ds4ChannelDistributionInfo.DBP_CHANNEL_DISTRIBUTIONIFNO.Rows.Add(newRow);
            //}

            return true;
        }
        private async Task<List<int>> GetUserHiddenChannels(string userCode)
        {
            try
            {
                var userSetting = await Store.GetUserSettingAsync(a => a.SingleOrDefaultAsync(x => x.Usercode == userCode && x.Settingtype == "USER_HIDDEN_CHANNELS"), true);
                string settingText = "";
                if (userSetting != null)
                {
                    settingText = !string.IsNullOrWhiteSpace(userSetting.Settingtext) ? userSetting.Settingtext : userSetting.Settingtextlong;
                }
                var userHiddenChannels = settingText.Split('|');
                return userHiddenChannels.Where(a => !string.IsNullOrWhiteSpace(a)).Select(a => a.ToInt32()).ToList();
            }
            catch (Exception ex)
            {
                SobeyRecException.ThrowSelfNoParam(nameof(GetUserHiddenChannels), GlobalDictionary.GLOBALDICT_CODE_FILL_USER_GETUSERSETTING_EXCEPTION, Logger, ex);
                throw ex;
            }
        }

        public virtual async Task<int> GetBestPreviewChnForSignalAsync(int nSignalID)
        {
            var signalSrcInfos = _mapper.Map<List<SignalSrcInfo>>(await Store.GetAllSignalsrcForRcdinAsync(true));

            if (signalSrcInfos == null || signalSrcInfos.Count <= 0)
            {
                return 0;
            }

            var captureChannels = await GetChannelsByProgrammeIdAsync<CaptureChannelInfo>(nSignalID);//获得所有采集通道
            //获得所有通道的状态，查看是否在做KAMATAKI任务
            var arrMsvChannelState = await GetAllChannelStateAsync();
            var channel2SignalSrcMaps = await Store.GetAllChannel2SignalSrcMapAsync();//获得当前通道与信号源的映射

            var ids = captureChannels.Select(a => a.nID).ToList();
            var map = channel2SignalSrcMaps.FirstOrDefault(a => ids.Contains(a.nChannelID) &&
                                                      a.nSignalSrcID == nSignalID &&
                                                      a.state == Channel2SignalSrc_State.emConnection);
            if (map != null)
            {
                return map.nChannelID;
            }

            var _globalinterface = ApplicationContext.Current.ServiceProvider.GetRequiredService<IIngestTaskInterface>();
            if (_globalinterface != null)
            {
                TaskInternals re = new TaskInternals() { funtype = IngestDBCore.TaskInternals.FunctionType.WillBeginAndCapturingTasks };
                var taskResponse = await _globalinterface.GetTaskCallBack(re);
                var taskContents = (taskResponse as IngestDBCore.ResponseMessage<List<TaskContentInterface>>).Ext;

                var tempList = captureChannels.Where(a => a.nDeviceTypeID == (int)CaptureChannelType.emMsvChannel &&
                                              IsDeviceOk(a.nID, arrMsvChannelState) &&
                                            taskContents.Any(x => x.emState == taskStateInterface.tsExecuting && x.nChannelID == a.nID))
                               .Select(a => new ChannelScore { Id = a.nID }).ToList();

                if (tempList.Count <= 0)
                    return 0;
                foreach (var item in tempList)
                {
                    var bIsExist = false;
                    var tasks = taskContents.Where(t => t != null && t.nChannelID == item.Id && t.nSignalID != nSignalID).ToList();
                    if (tasks != null && tasks.Count > 0)
                    {
                        tasks.ForEach(t =>
                        {
                            item.Score = (t.strBegin.ToDateTime() - DateTime.Now).TotalSeconds;
                        });
                    }
                    else
                    {
                        item.Score = -1;
                    }
                }
                var resList = tempList.OrderByDescending(a => a.Score).ToList();
                //返回得分最低的
                if (resList != null && resList.Count > 0)
                {
                    return resList[0].Id;
                }
            }
            return 0;
        }

        private bool IsDeviceOk(int channelId, List<MSVChannelState> arrMsvChannelState)
        {
            if (arrMsvChannelState == null || arrMsvChannelState.Count <= 0)
            {
                return false;
            }

            return arrMsvChannelState.Any(s => s.nChannelID == channelId && s.emMSVMode == MSV_Mode.NETWORK && s.emDevState != Device_State.DISCONNECTTED);
        }

        private bool IsHaveCapturingTask(int channelId, List<TaskContent> capturingTasks)
        {
            if (capturingTasks != null)
            {
                foreach (TaskContent task in capturingTasks)
                {
                    if (task.nChannelID == channelId
                        && task.emState == taskState.tsExecuting)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public async Task<List<TResult>> GetAllGPIInfoAsync<TResult>()
        {
            return _mapper.Map<List<TResult>>(await Store.GetGPIInfoAsync(a => a, true));
        }


        public async Task<int> GetSignalCaptureTemplateAsync(int nSignalID)
        {
            return (await Store.GetProgramparamMapAsync(a => a.Where(x => x.Programid == nSignalID).Select(x => x.Paramid).SingleOrDefaultAsync(), true)).ToInt32();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>旧名称GetChannelsByProgrammeId</remarks>
        /// <param name="programmeId"></param>
        /// <returns></returns>
        public async Task<List<TResult>> GetChannelsByProgrammeIdAsync<TResult>(int programmeId)
        {
            var programme = _mapper.Map<ProgrammeInfo>(await Store.GetSignalsrcAsync(a => a.SingleOrDefaultAsync(x => programmeId == x.Signalsrcid), true));
            var channelInfos = await GetAllCaptureChannelsAsync();

            // TODO: 获取所对应的分组ID
            int nGroupId = await Store.GetSignalsrcgroupmapAsync(async a => await a.Where(x => x.Signalsrcid == programmeId)
                                                                                   .Select(x => x.Groupid)
                                                                                   .FirstOrDefaultAsync(), true);

            Func<CaptureChannelInfo, bool> groupWhere = a => true;
            if (nGroupId != -1)
            {
                // 有分组，则排除分组不同的通道，只保留同组的通道和无分组信息的通道
                groupWhere = a => a.nGroupID != -1 && a.nGroupID != nGroupId;
            }
            Func<CaptureChannelInfo, bool> emWhere;
            //判断是否是无矩阵
            bool isHaveMatrix = await HaveMatrixAsync();
            List<int> channelIds = await Store.GetChannelIdsBySignalIdForNotMatrix(programmeId);
            switch (programme.emPgmType)//类型匹配
            {
                case ProgrammeType.PT_SDI:
                    emWhere = a => (a.nDeviceTypeID == (int)CaptureChannelType.emMsvChannel) && (isHaveMatrix || channelIds.Contains(a.nID));
                    break;
                case ProgrammeType.PT_IPTS:
                    emWhere = a => a.nDeviceTypeID == (int)CaptureChannelType.emIPTSChannel;
                    break;
                case ProgrammeType.PT_StreamMedia:
                    emWhere = a => a.nDeviceTypeID == (int)CaptureChannelType.emIPTSChannel;
                    break;
                default:
                    emWhere = a => false;
                    break;
            }
            Func<CaptureChannelInfo, bool> typeIdwhere;
            switch (programme.TypeId)
            {
                case 0:
                    typeIdwhere = a => a.nCPSignalType != 2;
                    break;
                case 1:
                    typeIdwhere = a => a.nCPSignalType != 1;
                    break;
                default:
                    typeIdwhere = a => true;
                    break;
            }
            return _mapper.Map<List<TResult>>(channelInfos.Where(groupWhere).Where(emWhere).Where(typeIdwhere).ToList());
        }

        public virtual async Task<bool> HaveMatrixAsync()
        {
            return await Store.HaveMatirxAsync();
        }


        #region Update

        /// <summary>保存 OR 更新 通道扩展信息</summary>
        public virtual async Task<bool> SaveChnExtenddataAsync(int nChnID, int type, string data)
        {
            if (data == null)
                data = "";
            data = data.Replace("\\", "\\\\");
            return await Store.SaveChannelExtenddataAsync(nChnID, type, data) > 0;
        }

        /// <summary>更改MSV设备状态信息</summary>
        /// <param name="nID">通道Id</param>
        /// <param name="nDevState">设备状态</param>
        /// <param name="nMSVMode">MSV模式</param>
        /// <returns></returns>
        public virtual async Task<bool> UpdateMSVChannelStateAsync(int nID, int nDevState, int nMSVMode)
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
                return await Store.UpdateMSVChannelStateAsync(state) > 0;
            }
            SobeyRecException.ThrowSelfNoParam(nameof(UpdateMSVChannelStateAsync), GlobalDictionary.GLOBALDICT_CODE_DATA_NOTFOUND_BYKEY, Logger, null);
            return false;
        }

        public virtual async Task<bool> ModifySourceVTRIDAndUserCodeAsync(int nSourceVTRID, string userCode, params int[] nID)
        {
            return (await Store.ModifySourceVTRIDAndUserCodeAsync(nSourceVTRID, userCode, nID)) > 0;
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
