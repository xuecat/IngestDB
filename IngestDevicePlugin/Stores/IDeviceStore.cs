using IngestDevicePlugin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IngestDevicePlugin.Stores
{
    
    public interface IDeviceStore
    {
        //IQueryable<TaskInfo> SimpleQuery { get; }
        /// <summary> 获取所有输入端口与信号源 </summary>
        Task<List<TResult>> GetAllRouterInPortInfoAsync<TResult>(Func<IQueryable<DbpRcdindesc>, IQueryable<TResult>> query, bool notrack = false);
        /// <summary> 获取所有输出端口与信号源 </summary>
        Task<List<TResult>> GetAllRouterOutPortInfoAsync<TResult>(Func<IQueryable<DbpRcdoutdesc>, IQueryable<TResult>> query, bool notrack = false);
        /// <summary> 获取所有信号源和采集设备 </summary>
        Task<List<TResult>> GetAllSignalDeviceMapAsync<TResult>(Func<IQueryable<DbpSignalDeviceMap>, IQueryable<TResult>> query, bool notrack = false);
        /// <summary> 获取所有信号源 </summary>
        Task<List<TResult>> GetAllSignalSrcsAsync<TResult>(Func<IQueryable<DbpSignalsrc>, IQueryable<TResult>> query, bool notrack = false);
        /// <summary> 获取所有采集通道 </summary>
        Task<List<TResult>> GetAllCaptureChannelsAsync<TResult>(Func<IQueryable<DbpCapturechannels>, IQueryable<TResult>> query, bool notrack = false);
        /// <summary> 获取所有的采集设备信息 </summary>
        Task<List<TResult>> GetAllCaptureDevicesAsync<TResult>(Func<IQueryable<DbpCapturedevice>, IQueryable<TResult>> query, bool notrack = false);
        /// <summary> 获取虚拟通道信息 </summary>
        Task<List<TResult>> GetAllVirtualChannelsAsync<TResult>(Func<IQueryable<DbpIpVirtualchannel>, IQueryable<TResult>> query, bool notrack = false);
        /// <summary> 获取通道分组信息 </summary>
        Task<List<TResult>> GetAllChannelGroupMapAsync<TResult>(Func<IQueryable<DbpChannelgroupmap>, IQueryable<TResult>> query, bool notrack = false);
        /// <summary> 获取信号设备映射信息 </summary>
        TResult GetSignalDeviceMap<TResult>(Func<IQueryable<DbpSignalDeviceMap>, IQueryable<TResult>> query, bool notrack = false);
        /// <summary> 设置信号源和采集设备的对应 </summary>
        Task<int> SetSignalDeviceMap(DbpSignalDeviceMap model);
    }
}
