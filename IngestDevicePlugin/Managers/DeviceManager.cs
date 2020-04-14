using AutoMapper;
using IngestDevicePlugin.Stores;
using IngestDevicePlugin.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IngestDevicePlugin.Dto.Response;

namespace IngestTaskPlugin.Managers
{
    public class DeviceManager
    {
        public DeviceManager(IDeviceStore store, IMapper mapper)
        {
            Store = store;
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        /// <summary> 设备（仓储） </summary>
        protected IDeviceStore Store { get; }
        /// <summary> 数据映射器 </summary>
        protected IMapper _mapper { get; }

        /// <summary> 获取所有输入端口与信号源的映射 </summary>
        /// <typeparam name="TResult">映射返回类型</typeparam>
        public async virtual Task<List<TResult>> GetAllRouterInPortAsync<TResult>()
        {
            return _mapper.Map<List<TResult>>(await Store.GetAllRouterInPortInfoAsync(a => a, true));
        }
        /// <summary> 获取所有输出端口与信号源的映射 </summary>
        /// <typeparam name="TResult">映射返回类型</typeparam>
        public async virtual Task<List<TResult>> GetAllRouterOutPortAsync<TResult>()
        {
            return _mapper.Map<List<TResult>>(await Store.GetAllRouterOutPortInfoAsync(a => a, true));
        }
        /// <summary> 获取所有信号源和采集设备的映射 </summary>
        /// <typeparam name="TResult">映射返回类型</typeparam>
        public async virtual Task<List<TResult>> GetAllSignalDeviceMapAsync<TResult>()
        {
            return _mapper.Map<List<TResult>>(await Store.GetAllSignalDeviceMapAsync(a => a, true));
        }

        /// <summary> 获取所有信号源的映射 </summary>
        public async virtual Task<List<SignalSrcInfo>> GetAllSignalSrcsAsync()
        {
            //获取输入端口信号源的id
            var allRcdindesc = await Store.GetAllRouterInPortInfoAsync(a => a.Select(x => x.Signalsrcid), true);
            return _mapper.Map<List<SignalSrcInfo>>(await Store.GetAllSignalSrcsAsync(a => a.Where(x => allRcdindesc.Contains(x.Signalsrcid)), true));
        }

        /// <summary> 获取所有采集通道的映射 </summary>
        public async virtual Task<List<CaptureChannelInfo>> GetAllCaptureChannelsAsync()
        {
            //获取输出端口的通道Id
            var allRcdoutId = await Store.GetAllRouterOutPortInfoAsync(a => a.Select(x => x.Channelid), true);
            var captureChannelList = _mapper.Map<List<CaptureChannelInfo>>(await Store.GetAllCaptureChannelsAsync(a => a.Where(x => allRcdoutId.Contains(x.Channelid)), true));
            if (captureChannelList.Count > 0)//获取采集设备信息,修改orderCode
            {
                var captureDevices = await Store.GetAllCaptureDevicesAsync(a => a.Where(x => x.Ordercode != null).Select(x => new { x.Cpdeviceid, x.Ordercode }), true);
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
            var virtualChannels = await Store.GetAllVirtualChannelsAsync(a => a, true);
            captureChannelList.AddRange(_mapper.Map<List<CaptureChannelInfo>>(virtualChannels));
            //获取Group信息
            var allChannelGroupMap = await Store.GetAllChannelGroupMapAsync(a => a, true);
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

        /// <summary> 获取所有采集设备的映射 </summary>
        public async virtual Task<List<TResult>> GetAllCaptureDevicesAsync<TResult>()
        {
            return _mapper.Map<List<TResult>>(await Store.GetAllCaptureDevicesAsync(a => a.OrderBy(x => x.Ordercode), true));
        }

        /// <summary> 通过信号Id获取设备映射信息 </summary>
        /// <param name="nSignalID">信号Id</param>
        /// <param name="nDeviceID">设备Id</param>
        /// <param name="nDeviceOutPortIdx">设备输出端口索引</param>
        /// <param name="SignalSource">信号源</param>
        public virtual void GetSignalDeviceMapBySignalID(int nSignalID, ref int nDeviceID, ref int nDeviceOutPortIdx, ref emSignalSource SignalSource)
        {
            var signalDeviceMap = Store.GetSignalDeviceMap(a => a.Where(x => x.Signalsrcid == nSignalID), true);
            if (signalDeviceMap != null)
            {
                nDeviceID = signalDeviceMap.Deviceid;
                nDeviceOutPortIdx = (int)signalDeviceMap.Deviceoutportidx;
                SignalSource = (emSignalSource)signalDeviceMap.Signalsource;
            }
        }

        /// <summary>
        /// 设置信号源和采集设备的对应
        /// </summary>
        /// <param name="nSignalID">信号Id</param>
        /// <param name="nDeviceID">设备Id</param>
        /// <param name="nDeviceOutPortIdx">设备输出端口索引</param>
        /// <param name="SignalSource">信号源</param>
        /// <returns></returns>
        public async Task SetSignalDeviceMap(int nSignalID, int nDeviceID, int nDeviceOutPortIdx, emSignalSource SignalSource)
        {
            await Store.SetSignalDeviceMap(new IngestDevicePlugin.Models.DbpSignalDeviceMap
            {
                Signalsrcid = nSignalID,
                Signalsource = (int)SignalSource,
                Deviceid = nDeviceID,
                Deviceoutportidx = nDeviceOutPortIdx
            });
        }
    }
}
