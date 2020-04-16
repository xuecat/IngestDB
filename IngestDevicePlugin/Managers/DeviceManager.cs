using AutoMapper;
using IngestDevicePlugin.Stores;
using IngestDevicePlugin.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sobey.Core.Log;
using Microsoft.EntityFrameworkCore;

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

        /// <summary> 获取所有输入端口与信号源 </summary>
        public async virtual Task<List<TResult>> GetAllRouterInPortAsync<TResult>()
        {
            return _mapper.Map<List<TResult>>(await Store.GetRcdindescAsync(a => a, true));
        }
        /// <summary> 获取所有输出端口与信号源 </summary>
        public async virtual Task<List<TResult>> GetAllRouterOutPortAsync<TResult>()
        {
            return _mapper.Map<List<TResult>>(await Store.GetRcdoutdescAsync(a => a, true));
        }
        /// <summary> 获取所有信号源和采集设备 </summary>
        public async virtual Task<List<TResult>> GetAllSignalDeviceMapAsync<TResult>()
        {
            return _mapper.Map<List<TResult>>(await Store.GetSignalDeviceMapAsync(a => a, true));
        }

        /// <summary> 获取所有信号源 </summary>
        public async virtual Task<List<SignalSrcInfo>> GetAllSignalSrcsAsync()
        {
            //获取输入端口信号源的id
            var allRcdindesc = await Store.GetRcdindescAsync(a => a.Select(x => x.Signalsrcid), true);
            return _mapper.Map<List<SignalSrcInfo>>(await Store.GetSignalsrcAsync(a => a.Where(x => allRcdindesc.Contains(x.Signalsrcid)), true));
        }

        /// <summary> 获取所有信号源的扩展信息 </summary>
        public async virtual Task<List<SignalSrcExInfo>> GetAllSignalSrcExsAsync()
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
        public async virtual Task<List<CaptureChannelInfo>> GetAllCaptureChannelsAsync()
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
            var virtualChannels = await Store.GetIpVirtualchannelAsync(a => a, true);
            captureChannelList.AddRange(_mapper.Map<List<CaptureChannelInfo>>(virtualChannels));
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

        /// <summary> 获取所有采集设备 </summary>
        public async virtual Task<List<TResult>> GetAllCaptureDevicesAsync<TResult>()
        {
            return _mapper.Map<List<TResult>>(await Store.GetCapturedeviceAsync(a => a.OrderBy(x => x.Ordercode), true));
        }

        /// <summary> 通过信号Id获取设备映射信息 </summary>
        /// <param name="nSignalID">信号Id</param>
        /// <param name="nDeviceID">设备Id</param>
        /// <param name="nDeviceOutPortIdx">设备输出端口索引</param>
        /// <param name="SignalSource">信号源</param>
        public async virtual Task<SignalDeviceMap> GetSignalDeviceMapBySignalID(int nSignalID)
        {
            return _mapper.Map<SignalDeviceMap>(await Store.GetSignalDeviceMapAsync(async a => await a.FirstOrDefaultAsync(x => x.Signalsrcid == nSignalID), true));
        }

        /// <summary> 查询信号源是否是备份信号源 </summary>
        /// <param name="nSignalID">信号Id</param>
        public async virtual Task<bool> IsBackupSignalSrcByIdAsync(int nSignalID)
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
        public async virtual Task SetSignalDeviceMap(int nSignalID, int nDeviceID, int nDeviceOutPortIdx, emSignalSource SignalSource)
        {
            await Store.SaveSignalDeviceMap(new IngestDevicePlugin.Models.DbpSignalDeviceMap
            {
                Signalsrcid = nSignalID,
                Signalsource = (int)SignalSource,
                Deviceid = nDeviceID,
                Deviceoutportidx = nDeviceOutPortIdx
            });
        }

        /// <summary> 根据ChannleID获取高清还是标清 nType:0标清,1高清 </summary>
        public async virtual Task<int> GetParamTypeByChannleIDAsync(int nSignalID)
        {
            var nType = await Store.GetParamTypeByChannleIDAsync(nSignalID);
            return nType ?? -1;
        }

        /// <summary> 获取MSV设备状态信息 </summary>
        public async virtual Task<MSVChannelState> GetDbpMsvchannelStateAsync(int nChannelID)
        {
            return _mapper.Map<MSVChannelState>(await Store.GetMsvchannelStateAsync(async a => await a.FirstOrDefaultAsync(x => x.Channelid == nChannelID), true));
        }

        /// <summary> 获得所有信号源分组 </summary>
        public async virtual Task<List<AllSignalGroup>> GetAllSignalGroupAsync()
        {
            return _mapper.Map<List<AllSignalGroup>>(await Store.GetSignalGroupAsync(a => a, true));
        }

        /// <summary> 获取所有信号源分组信息 </summary>
        public async virtual Task<List<SignalGroupState>> GetAllSignalGroupInfoAsync()
        {
            return await Store.GetAllSignalGroupInfoAsync();
        }

        /// <summary> 通过 GPIID 找出该GPI所有的映射 </summary>
        public async virtual Task<List<GPIDeviceMapInfo>> GetGPIMapInfoByGPIIDAsync(int nGPIID)
        {
            return _mapper.Map<List<GPIDeviceMapInfo>>(await Store.GetGPIMapInfoByGPIIDAsync(a => a.Where(x => x.Gpiid == nGPIID).OrderBy(x => x.Gpioutputport), true));
        }

        /// <summary> 获取所有节目 </summary>
        //public async virtual Task<List<ProgrammeInfo>> GetAllProgrammeInfosAsync()
        //{
        //    var allRcdindesc = await Store.GetRcdindescAsync(a => a.Select(x => x.Signalsrcid), true);
        //    var programmeInfos = _mapper.Map<List<ProgrammeInfo>>(await Store.GetSignalsrcAsync(a => a.Where(x => allRcdindesc.Contains(x.Signalsrcid)), true));

        //    var inportDescInfos = await Store.GetRcdindescAsync(a => a, true);
        //    foreach (var info in programmeInfos)
        //    {
        //        var inport = inportDescInfos.FirstOrDefault(a => a.Signalsrcid == info.ProgrammeId);
        //        if (inport != null)
        //        {
        //            info.emSignalSourceType = (emSignalSource)inport.Signalsource;
        //        }
        //    }


        //}

        #region MyRegion
        public async Task<bool> HaveMatrixAsync()
        {
            return await Store.HaveMatirxAsync();
        }

        public async Task<ProgrammeInfoResponse> GetProgrammeInfoByIdAsync(int programeid)
        {
            return await Store.GetSignalInfoAsync(programeid);
        }

        public async Task<int> GetChannelSignalSrc(int channelid)
        {
            //判断是否是无矩阵
            bool isHaveMatrix = await HaveMatrixAsync();
            if (!isHaveMatrix)
            {
                var item = await Store.GetCaptureChannelByIDAsync(channelid);
                if (item != null && item.DeviceTypeID == (int)CaptureChannelType.emMsvChannel)
                {
                    return (await Store.GetSignalIdsByChannelIdForNotMatrix(channelid)).ElementAt(0);
                }
            }
            else
                return await Store.GetMatrixChannelBySignal(channelid);

            Logger.Error("GetChannelSignalSrc error getno");
            return 0;
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

        public async Task<ProgrammeInfoResponse> GetBackProgramInfoBySrgid(int srgid)
        {
            return await Store.GetSignalInfoAsync(await Store.GetBackUpSignalInfoByID(srgid));
        }
        #endregion
    }
}
