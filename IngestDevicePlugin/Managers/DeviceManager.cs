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
using ProgrammeInfoDto = IngestDevicePlugin.Dto.Response.ProgrammeInfoResponse;
using CaptureChannelInfoDto = IngestDevicePlugin.Dto.Response.CaptureChannelInfoResponse;
using emSignalSource = IngestDevicePlugin.Dto.Enum.emSignalSource;
using IngestDevicePlugin.Dto.OldResponse;
using IngestDBCore.Tool;
using System.Text.RegularExpressions;

namespace IngestDevicePlugin.Managers
{
    public class DeviceManager
    {
        public DeviceManager(IDeviceStore store, IMapper mapper, IServiceProvider services)
        {
            Store = store;
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _taskInterface = new Lazy<IIngestTaskInterface>(() => services.GetRequiredService<IIngestTaskInterface>()); ;
        }

        private readonly ILogger Logger = LoggerManager.GetLogger("DeviceInfo");

        /// <summary> 设备（仓储） </summary>
        protected IDeviceStore Store { get; }
        private Lazy<IIngestTaskInterface> _taskInterface { get; }
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
        public virtual async Task<List<TResult>> GetAllSignalSrcsAsync<TResult>()
        {
            return _mapper.Map<List<TResult>>((await Store.GetAllSignalsrcForRcdinAsync(true)).CustomSort().ToList());
        }


        /// <summary> 获取所有信号源的扩展信息 </summary>
        public async Task<List<TResult>> GetAllSignalSrcExsAsync<TResult>()
        {
            var allSignalSrcExs = _mapper.Map<List<SignalSrcExResponse>>(await Store.GetSignalSrcExsAsync(a => a, true));
            if (allSignalSrcExs.Count > 0)
            {
                allSignalSrcExs.Where(a => a.IsMainSignalSrc).ToList().ForEach(a =>
                {
                    //找到上级Id为当前nId的，说明此信号为当前的下级
                    var signalSrcEx = allSignalSrcExs.FirstOrDefault(x => x.MainSignalSrcId == a.Id);
                    if (signalSrcEx != null)
                    {
                        a.BackupSignalSrcId = signalSrcEx.Id;
                    }
                });
            }
            return _mapper.Map<List<TResult>>(allSignalSrcExs);
        }

        public async Task<ProgrammeInfoResponse> GetBackProgramInfoBySrgid(int srgid)
        {
            return await Store.GetSignalInfoAsync(await Store.GetBackUpSignalInfoByID(srgid));
        }


        /// <summary> 获取所有采集通道 </summary>
        public virtual async Task<List<TResult>> GetAllCaptureChannelsAsync<TResult>()
        {
            return _mapper.Map<List<TResult>>(await Store.GetAllCaptureChannelsAsync(0));
        }

        /// <summary> 根据 通道ID 获取采集通道 </summary>
        /// <param name="id">通道Id</param>
        public virtual async Task<TResult> GetCaptureChannelByIDAsync<TResult>(int id)
        {
            var captureChannel = _mapper.Map<TResult>(await Store.GetCaptureChannelByIDAsync(id));
            if (captureChannel == null)
            {
                captureChannel = _mapper.Map<TResult>(await Store.GetIpVirtualchannelAsync(async a => await a.FirstOrDefaultAsync(x => x.Channelid == id), true));
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
        /// <param name="signalID">信号Id</param>
        public virtual async Task<TResult> GetSignalDeviceMapBySignalID<TResult>(int signalID)
        {
            return _mapper.Map<TResult>(await Store.GetSignalDeviceMapAsync(async a => await a.FirstOrDefaultAsync(x => x.Signalsrcid == signalID), true));
        }

        /// <summary> 查询信号源是否是备份信号源 </summary>
        /// <param name="signalID">信号Id</param>
        public virtual Task<bool> IsBackupSignalSrcByIdAsync(int signalID)
        {
            return Store.GetSignalSrcExsAsync(async a => await a.AnyAsync(x => x.Signalsrcid == signalID && x.Ismastersrc == 0), true);
        }

        /// <summary>
        /// 设置信号源和采集设备的对应
        /// </summary>
        /// <param name="signalID">信号Id</param>
        /// <param name="deviceID">设备Id</param>
        /// <param name="deviceOutPortIdx">设备输出端口索引</param>
        /// <param name="signalSource">信号源</param>
        /// <returns></returns>
        public virtual async ValueTask<bool> SaveSignalDeviceMapAsync(int signalID, int deviceID, int deviceOutPortIdx, emSignalSource signalSource)
        {
            await Store.SaveSignalDeviceMapAsync(new IngestDevicePlugin.Models.DbpSignalDeviceMap
            {
                Signalsrcid = signalID,
                Signalsource = (int)signalSource,
                Deviceid = deviceID,
                Deviceoutportidx = deviceOutPortIdx
            });
            return true;
        }

        /// <summary> 根据 通道ID 获取高清还是标清 nType:0标清,1高清 </summary>
        public virtual async ValueTask<int> GetParamTypeByChannelIDAsync(int channelID)
        {
            var nType = await Store.GetParamTypeByChannelIDAsync(channelID);
            return nType ?? -1;
        }

        /// <summary>根据信号源获取是高清还是标清</summary>
        public virtual async ValueTask<int> GetParamTypeBySignalIDAsync(int signalID)
        {
            var id = await Store.GetParamTypeBySignalIDAsync(signalID);
            return id ?? -1;
        }

        /// <summary> 根据 通道ID 获取MSV设备状态信息 </summary>
        public virtual async Task<TResult> GetMsvChannelStateAsync<TResult>(int channelID)
        {
            return _mapper.Map<TResult>(await Store.GetMsvchannelStateAsync(async a => await a.FirstOrDefaultAsync(x => x.Channelid == channelID), true));
        }

        /// <summary> 获得所有通道的状态 </summary>
        public virtual async Task<List<TResult>> GetAllChannelStateAsync<TResult>()
        {
            return _mapper.Map<List<TResult>>(await Store.GetMsvchannelStateAsync(a => a, true));
        }

        /// <summary> 获得所有信号源分组 </summary>
        public virtual async Task<List<TResult>> GetAllSignalGroupAsync<TResult>()
        {
            return _mapper.Map<List<TResult>>(await Store.GetSignalGroupAsync(a => a, true));
        }

        /// <summary> 获取所有信号源分组信息 </summary>
        public virtual async Task<List<TResult>> GetAllSignalGroupInfoAsync<TResult>()
        {
            return _mapper.Map<List<TResult>>(await Store.GetAllSignalGroupInfoAsync());
        }

        /// <summary> 通过 GPIID 找出该GPI所有的映射 </summary>
        public virtual async Task<List<TResult>> GetGPIMapInfoByGPIIDAsync<TResult>(int gpiID)
        {
            return _mapper.Map<List<TResult>>(await Store.GetGPIMapInfoByGPIIDAsync(a => a.Where(x => x.Gpiid == gpiID).OrderBy(x => x.Gpioutputport), true));
        }

        /// <summary> 获取所有节目 </summary>
        public virtual async Task<List<TResult>> GetAllProgrammeInfosAsync<TResult>()
        {
            var programmeInfos = _mapper.Map<List<TResult>>(await Store.GetAllProgrammeInfoAsync());

            var allTSPgmInfo = _mapper.Map<List<TResult>>(await Store.GetIpProgrammeAsync(a => a, true));
            programmeInfos.AddRange(allTSPgmInfo);

            var allStreamMedia = _mapper.Map<List<TResult>>(await Store.GetStreamMediaAsync(a => a, true));
            programmeInfos.AddRange(allStreamMedia);

            return programmeInfos.CustomSortT().ToList();
        }

        /// <summary> 根据通道获取相应的节目 </summary>
        public virtual async Task<List<TResult>> GetProgrammeInfosByChannelIdAsync<TResult>(int channelId)
        {
            var channelInfo = await GetCaptureChannelByIDAsync<CaptureChannelInfoDto>(channelId);
            if ((channelInfo.DeviceTypeId == (int)CaptureChannelType.emMsvChannel || channelInfo.DeviceTypeId == (int)CaptureChannelType.emDefualtChannel)
                && !await HaveMatrixAsync())
            {
                var signalIds = await Store.GetSignalIdsByChannelIdForNotMatrix(channelId);

                return _mapper.Map<List<TResult>>(await Store.GetSignalInfoByListAsync(signalIds));

                //var programmeInfoList = _mapper.Map<List<ProgrammeInfo>>(await Store.GetSignalsrcAsync(a => a.Where(x => signalIds.Contains(x.Signalsrcid)), true));
                //await MapSignalSourceForProgramme(programmeInfoList);
                //return programmeInfoList;
            }
            else
            {
                int signalId = 0;
                if ((channelInfo.DeviceTypeId == (int)CaptureChannelType.emMsvChannel || channelInfo.DeviceTypeId == (int)CaptureChannelType.emDefualtChannel))
                {
                    signalId = await GetChannelSignalSrcAsync(channelInfo.Id);
                }
                return _mapper.Map<List<TResult>>(GetProgrammeInfoListMatrix(channelInfo, signalId, await Store.GetAllProgrammeInfoAsync()));
            }
        }

        /// <summary>获取筛选结果</summary>
        private List<ProgrammeInfoDto> GetProgrammeInfoListMatrix(CaptureChannelInfoDto channelInfo, int signalId, List<ProgrammeInfoDto> programmeInfos)
        {
            List<ProgrammeInfoDto> lstback = new List<ProgrammeInfoDto>();
            foreach (var item in programmeInfos)
            {
                //首先判断高标清
                if (channelInfo.CpSignalType > 0)//不是Auto
                {
                    if (channelInfo.CpSignalType == 1)//SD
                    {
                        if (item.TypeId == 1)//排除HD，Auto和SD可以匹配
                            continue;
                    }
                    else if (channelInfo.CpSignalType == 2)//HD
                    {
                        if (item.TypeId == 0)//排除SD，保留HD和Auto
                            continue;
                    }
                }

                if (channelInfo.GroupId > 0 && item.GroupId > 0 && channelInfo.GroupId != item.GroupId)
                {
                    continue;
                }

                if (channelInfo.DeviceTypeId == (int)CaptureChannelType.emMsvChannel
                    || channelInfo.DeviceTypeId == (int)CaptureChannelType.emDefualtChannel)
                {
                    if (item.PgmType == ProgrammeType.PT_SDI /* && signalId == info.ProgrammeId */)
                    {
                        // Add by chenzhi 2013-08-21
                        // TODO: Fix Bug: snp4100051546
                        // TODO: 这里修改此函数的功能，如果是SDI通道和信号源通过矩阵服务直接连接，
                        // 则优先返回此信号源，注意:这需要在客户端限制不同分组的信号源和通道通过矩阵直接连接
                        if (signalId == item.ProgrammeId)
                        {
                            lstback.Insert(0, item);
                        }
                        else
                        {
                            lstback.Add(item);
                        }
                        // ----------------------- The End 2013-08-21 -----------------------
                    }
                }
                else if (channelInfo.DeviceTypeId == (int)CaptureChannelType.emIPTSChannel)
                {
                    if (item.PgmType == ProgrammeType.PT_IPTS/* || info.emPgmType == ProgrammeType.PT_StreamMedia*/)
                    {
                        lstback.Add(item);
                    }
                }
                else if (channelInfo.DeviceTypeId == (int)CaptureChannelType.emStreamChannel)
                {
                    // Delete by chenzhi 2013-07-09
                    // TODO: 运营商的概念已经被分组所取代，故移除该部分代码

                    if (item.PgmType == ProgrammeType.PT_StreamMedia || item.SignalSourceType == emSignalSource.emStreamMedia/*&& info.nCarrierID == channelInfo.nCarrierID */)//运营商信息也要判断
                    {
                        lstback.Add(item);
                    }
                }
            }


            return lstback;
        }

        /// <summary>根据 通道Id 获取 信号id</summary>
        /// <param name="channelid">通道Id</param>
        public virtual async Task<int> GetChannelSignalSrcAsync(int channelid)
        {
            if (!await HaveMatrixAsync())
            {
                CaptureChannelInfoDto channelInfo = await GetCaptureChannelByIDAsync<CaptureChannelInfoDto>(channelid);

                if (channelInfo.DeviceTypeId == (int)CaptureChannelType.emMsvChannel
                    || channelInfo.DeviceTypeId == (int)CaptureChannelType.emDefualtChannel)
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


        public virtual async Task<int> GetBackupSignalSrcIdByIdAsync(int id)
        {
            return await Store.GetSignalsrcMasterbackupAsync(a => a.Where(x => x.Mastersignalsrcid == id)
                                                                   .Select(x => x.Signalsrcid)
                                                                   .FirstOrDefaultAsync(), true);
        }

        public virtual async Task<int> GetBestChannelIdBySignalIDAsync(int signalID, string userCode)
        {
            DateTime dtNow = DateTime.Now;

            if (_taskInterface != null)
            {
                var channelIds = await GetUserHiddenChannels(userCode);     //获得该用户的隐藏通道
                List<DbpMsvchannelState> arrMsvChannelState = await Store.GetMsvchannelStateAsync(async a => 
                await a.Where(b => b.Devstate.GetValueOrDefault() != (int)Device_State.DISCONNECTTED).ToListAsync(),
                    true); //获得所有通道的状态，查看是否在做KAMATAKI任务
                //获得所有采集通道
                List<CaptureChannelInfoResponse> captureChannels = await Store.GetAllCaptureChannelsAsync(0);
                //获得将要和正在执行的任务
                TaskInternals reWillBeginAndCapturingTasks = new TaskInternals() { funtype = IngestDBCore.TaskInternals.FunctionType.WillBeginAndCapturingTasks };
                var willResponse = await _taskInterface.Value.GetTaskCallBack(reWillBeginAndCapturingTasks);
                var taskContents = (willResponse as IngestDBCore.ResponseMessage<List<TaskContentInterface>>)?.Ext;

                //获得当前任务
                //获得当前通道与信号源的映射
                var channel2SignalSrcMaps = await Store.GetAllChannel2SignalSrcMapAsync();//获得当前通道与信号源的映射

                var captureChannelInfos = captureChannels.Where(a => !channelIds.Contains(a.Id) &&
                                                                     arrMsvChannelState.Any(s => s.Channelid == a.Id &&
                                                                                                 s.Msvmode == (int)MSV_Mode.NETWORK &&
                                                                                                 string.IsNullOrEmpty(s.Kamatakiinfo)))
                                                         .Select(a => new ChannelScore { Id = a.Id }).ToList();
                if (captureChannelInfos.Count <= 0)
                    return 0;
                foreach (var channel in captureChannelInfos)
                {
                    var bIsExist = false;
                    var task = taskContents?.FirstOrDefault(t => t != null && t.ChannelId == channel.Id);
                    if (task != null)
                    {
                        bIsExist = true;
                        double totalSeconds = (DateTimeFormat.DateTimeFromString(task.Begin) - dtNow).TotalSeconds;

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

                var captureInfos = captureChannelInfos.Where(a => a.Score >= 0).Select(a => a.Id).ToList();
                return captureInfos.Count > 0 ? captureInfos[0] : 0;
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
                if (!string.IsNullOrEmpty(settingText))
                {
                    var userHiddenChannels = settingText.Split('|');
                    return userHiddenChannels.Where(a => !string.IsNullOrWhiteSpace(a)).Select(a => Convert.ToInt32(a)).ToList();
                }
                return new List<int>();
            }
            catch (Exception ex)
            {
                SobeyRecException.ThrowSelfNoParam(nameof(GetUserHiddenChannels), GlobalDictionary.GLOBALDICT_CODE_FILL_USER_GETUSERSETTING_EXCEPTION, Logger, ex);
                throw ex;
            }
        }

        private bool IsDeviceOk(int channelId, List<MSVChannelState> arrMsvChannelState)
        {
            if (arrMsvChannelState == null || arrMsvChannelState.Count <= 0)
            {
                return false;
            }

            return arrMsvChannelState.Any(s => s.nChannelID == channelId && s.emMSVMode == MSV_Mode.NETWORK && s.emDevState != Device_State.DISCONNECTTED);
        }

        public virtual async Task<int> GetBestPreviewChnForSignalAsync(int signalID)
        {
            var captureChannels = await GetChannelsByProgrammeIdAsync<CaptureChannelInfo>(signalID);//获得所有采集通道

            if (captureChannels == null)
            {
                Logger.Error("GetBestPreviewChnForSignalAsync GetChannelsByProgrammeIdAsync error");
                return 0;
            }
            //获得所有通道的状态，查看是否在做KAMATAKI任务
            var arrMsvChannelState = (await GetAllChannelStateAsync<MSVChannelState>()).Where(a => 
            a.emMSVMode == MSV_Mode.NETWORK && a.emDevState != Device_State.DISCONNECTTED).Select(x=> x.nChannelID).ToList();

            var channel2SignalSrcMaps = await Store.GetAllChannel2SignalSrcMapAsync();//获得当前通道与信号源的映射

            var ids = captureChannels.Select(a => a.nID).ToList();
            var map = channel2SignalSrcMaps.FirstOrDefault(a => ids.Contains(a.nChannelID) &&
                                                      a.nSignalSrcID == signalID &&
                                                      a.state == Channel2SignalSrc_State.emConnection);
            if (map != null)
            {
                return map.nChannelID;
            }

            if (_taskInterface != null)
            {
                TaskInternals re = new TaskInternals() { funtype = IngestDBCore.TaskInternals.FunctionType.WillBeginAndCapturingTasks };
                var taskResponse = await _taskInterface.Value.GetTaskCallBack(re);
                var taskContents = (taskResponse as IngestDBCore.ResponseMessage<List<TaskContentInterface>>).Ext;
                //需进行测试
                var selectlist = captureChannels.Where(a => (a.nDeviceTypeID == (int)CaptureChannelType.emMsvChannel ||
                                                            a.nDeviceTypeID == (int)CaptureChannelType.emDefualtChannel) &&
                                              arrMsvChannelState.Contains(a.nID));
                if (taskContents !=null && taskContents.Count > 0)
                {
                    selectlist = selectlist.Where(a => taskContents.Any(x => x.State != taskStateInterface.tsExecuting && x.ChannelId == a.nID));
                }

                var tempList = selectlist.Select(a => new ChannelScore { Id = a.nID }).ToList();

                if (tempList.Count <= 0)
                    return 0;
                foreach (var item in tempList)
                {
                    var bIsExist = false;
                    var tasks = taskContents.Where(t => t != null && t.ChannelId == item.Id && t.SignalId != signalID).ToList();
                    if (tasks != null && tasks.Count > 0)
                    {
                        tasks.ForEach(t =>
                        {
                            item.Score = (DateTimeFormat.DateTimeFromString(t.Begin) - DateTime.Now).TotalSeconds;
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

        public async Task<List<TResult>> GetAllGPIInfoAsync<TResult>()
        {
            return _mapper.Map<List<TResult>>(await Store.GetGPIInfoAsync(a => a, true));
        }


        public async Task<int> GetSignalCaptureTemplateAsync(int signalID)
        {
            return (await Store.GetProgramparamMapAsync(a => a.Where(x => x.Programid == signalID).Select(x => x.Paramid).SingleOrDefaultAsync(), true)).GetValueOrDefault();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>旧名称GetChannelsByProgrammeId</remarks>
        /// <param name="programmeId"></param>
        /// <returns></returns>
        public async Task<List<TResult>> GetChannelsByProgrammeIdAsync<TResult>(int programmeId)
        {
            var programme = await Store.GetSignalInfoAsync(programmeId);
            var channelInfos = await Store.GetAllCaptureChannelsAsync(0);

            List<CaptureChannelInfoResponse> lstchn = new List<CaptureChannelInfoResponse>();

            foreach (var item in channelInfos)
            {
                ////类型匹配
                if (!((programme.PgmType == ProgrammeType.PT_SDI && item.DeviceTypeId == (int)CaptureChannelType.emMsvChannel && programme.SignalSourceType != emSignalSource.emStreamMedia)
                     || (programme.PgmType == ProgrammeType.PT_SDI && programme.SignalSourceType == emSignalSource.emStreamMedia && item.DeviceTypeId == (int)CaptureChannelType.emStreamChannel)
                     || ((programme.PgmType == ProgrammeType.PT_IPTS) && (item.DeviceTypeId == (int)CaptureChannelType.emIPTSChannel))
                     || ((programme.PgmType == ProgrammeType.PT_StreamMedia || programme.SignalSourceType == emSignalSource.emStreamMedia) && (item.DeviceTypeId == (int)CaptureChannelType.emStreamChannel))))
                {
                    continue;
                }

                //高标清匹配
                if (item.CpSignalType > 0)//0表示Auto，可以任意匹配，不需要处理,1:SD, 2:HD
                {
                    if (item.CpSignalType == 1)//SD
                    {
                        if (programme.TypeId == 1)//排除HD，Auto和SD可以匹配
                            continue;
                    }
                    else if (item.CpSignalType == 2)//HD
                    {
                        if (programme.TypeId == 0)//排除SD，保留HD和Auto
                            continue;
                    }
                }

                if (programme.GroupId > 0 && item.GroupId > 0 && item.GroupId != programme.GroupId)
                {
                    continue;
                }

                /*
                 * @brief 查对应out和in的index 我认为没有矩阵直连填信号也没用，没事的
                 */
                //bool isNeedAdd = true;
                //if (programmeInfo.emPgmType == ProgrammeType.PT_SDI)
                //{
                //    if (!isHaveMatrix)
                //    {
                //        //需要根据列表对通道进行判断
                //        channelIdListInNotMatrix = deviceAccess.GetChannelIdsBySignalIdForNotMatrix(programmeId);

                //        isNeedAdd = false;
                //        foreach (int channelId in channelIdListInNotMatrix)
                //        {
                //            if (channelId == channelInfo.nID)
                //            {
                //                isNeedAdd = true;
                //                break;
                //            }
                //        }
                //    }
                //}
                lstchn.Add(item);
            }
            // Add by chenzhi 2013-07-09
            // TODO: 通道需要排序，同一个分组的排在前面，无分组的排在后面
            lstchn.OrderBy(x => x.GroupId);
            return _mapper.Map<List<TResult>>(lstchn);
        }

        public virtual Task<bool> HaveMatrixAsync()
        {
            return Store.HaveMatirxAsync();
        }


        #region Update

        /// <summary>保存 OR 更新 通道扩展信息</summary>
        public virtual async Task<bool> SaveChnExtenddataAsync(int channelid, int type, string data)
        {
            if (data == null)
                data = "";
            //data = data.Replace("\\\\\\\\", "\\\\");
            return await Store.SaveChannelExtenddataAsync(channelid, type, data) > 0;
        }

        public virtual async Task<string> GetChannelExtendData(int channelId, int type)
        {
            return await Store.GetChannelExtendDataAsync(a => a.Where(b => b.Channaelid == channelId && b.Datatype == type)
                                                                .Select(c => c.Extenddata)
                                                                .FirstOrDefaultAsync(), true);
        }

        /// <summary>更改MSV设备状态信息</summary>
        /// <param name="id">通道Id</param>
        /// <param name="devState">设备状态</param>
        /// <param name="MSVMode">MSV模式</param>
        /// <returns></returns>
        public virtual async Task<bool> UpdateMSVChannelStateAsync(int id, int devState, int MSVMode)
        {
            var state = await Store.GetMsvchannelStateAsync(async a => await a.FirstOrDefaultAsync(x => x.Channelid == id));
            if (state != null)
            {
                if (UpdateDevState.Contains(devState))
                {
                    state.Devstate = devState;
                }
                if (UpdateMSV_Mode.Contains(MSVMode))
                {
                    state.Msvmode = MSVMode;
                }
                return await Store.UpdateMSVChannelStateAsync(state) > 0;
            }
            SobeyRecException.ThrowSelfNoParam(nameof(UpdateMSVChannelStateAsync), GlobalDictionary.GLOBALDICT_CODE_DATA_NOTFOUND_BYKEY, Logger, null);
            return false;
        }

        public virtual async Task<bool> ModifySourceVTRIDAndUserCodeAsync(int sourceVTRID, string userCode, params int[] ids)
        {
            return (await Store.ModifySourceVTRIDAndUserCodeAsync(sourceVTRID, userCode, ids)) > 0;
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
                if (!((programinfo.PgmType == ProgrammeType.PT_SDI && item.DeviceTypeId == (int)CaptureChannelType.emMsvChannel && programinfo.SignalSourceType != emSignalSource.emStreamMedia)
                     || (programinfo.PgmType == ProgrammeType.PT_SDI && programinfo.SignalSourceType == emSignalSource.emStreamMedia && item.DeviceTypeId == (int)CaptureChannelType.emStreamChannel)
                     || ((programinfo.PgmType == ProgrammeType.PT_IPTS) && (item.DeviceTypeId == (int)CaptureChannelType.emIPTSChannel))
                     || ((programinfo.PgmType == ProgrammeType.PT_StreamMedia) && (item.DeviceTypeId == (int)CaptureChannelType.emStreamChannel))))
                {
                    continue;
                }

                //高标清匹配
                if (item.CpSignalType > 0)//0表示Auto，可以任意匹配，不需要处理,1:SD, 2:HD
                {
                    if (item.CpSignalType == 1)//SD
                    {
                        if (programinfo.TypeId == 1)//排除HD，Auto和SD可以匹配
                            continue;
                    }
                    else if (item.CpSignalType == 2)//HD
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
                        if (channelIdListInNotMatrix.Any(x => x == item.Id))
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



        #region xdcamdevice

        public async Task<List<TResult>> GetAllXDCAMDeviceAsync<TResult>()
        {
            var dbpXdcams = await Store.GetXdcamDeviceListAsync(a => a, true);
            return _mapper.Map<List<TResult>>(dbpXdcams);
        }

        #endregion

    }

    public static class MyDbpSignalsrcExtensions
    {
        public static IEnumerable<DbpSignalsrc> CustomSort(this IEnumerable<DbpSignalsrc> list)
        {
            int maxLen = list.Select(s => s.Name.Length).Max();

            return list.Select(s => new
            {
                OrgStr = s,
                SortStr = Regex.Replace(s.Name, @"(\d+)|(\D+)", m => m.Value.PadLeft(maxLen, char.IsDigit(m.Value[0]) ? ' ' : '\xffff'))
            })
            .OrderBy(x => x.SortStr)
            .Select(x => x.OrgStr);
        } 
        public static IEnumerable<TResult> CustomSortT<TResult>(this IEnumerable<TResult> list)
        {
            int maxLen = list.Select(s => s.GetType().GetProperty("ProgrammeName").GetValue(s).ToString().Length).Max();//ProgrammeName
            return list.Select(s => new
            {
                OrgStr = s,
                SortStr = Regex.Replace(s.GetType().GetProperty("ProgrammeName").GetValue(s).ToString(), @"(\d+)|(\D+)", m => m.Value.PadLeft(maxLen, char.IsDigit(m.Value[0]) ? ' ' : '\xffff'))
            })
            .OrderBy(x => x.SortStr)
            .Select(x => x.OrgStr); 
        }

    }
}
