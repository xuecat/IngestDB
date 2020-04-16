using IngestDevicePlugin.Dto;
using IngestDevicePlugin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProgrammeInfoDto = IngestDevicePlugin.Dto.ProgrammeInfoResponse;
using CaptureChannelInfoDto = IngestDevicePlugin.Dto.CaptureChannelInfoResponse;
namespace IngestDevicePlugin.Stores
{
    
    public interface IDeviceStore
    {
        #region MyRegion
        //IQueryable<TaskInfo> SimpleQuery { get; }
        Task<bool> HaveMatirxAsync();
        Task<ProgrammeInfoDto> GetSignalInfoAsync(int srcid);
        Task<CaptureChannelInfoDto> GetCaptureChannelByIDAsync(int channelid);
        Task<List<CaptureChannelInfoDto>> GetAllCaptureChannelsAsync(int status);
        Task<List<int>> GetChannelIdsBySignalIdForNotMatrix(int signalid);
        Task<List<int>> GetSignalIdsByChannelIdForNotMatrix(int channelid);
        Task<List<DbpChannelRecmap>> GetAllChannelUnitMap();
        Task<DbpChannelRecmap> GetChannelUnitMap(int channel);
        Task<int> GetMatrixChannelBySignal(int channelid);
        #endregion

        /// <summary> 获取所有输入端口与信号源 </summary>
        Task<List<TResult>> GetRcdindescAsync<TResult>(Func<IQueryable<DbpRcdindesc>, IQueryable<TResult>> query, bool notrack = false);
        /// <summary> 获取所有输出端口与信号源 </summary>
        Task<List<TResult>> GetRcdoutdescAsync<TResult>(Func<IQueryable<DbpRcdoutdesc>, IQueryable<TResult>> query, bool notrack = false);
        /// <summary> 获取所有信号源 </summary>
        Task<List<TResult>> GetSignalsrcAsync<TResult>(Func<IQueryable<DbpSignalsrc>, IQueryable<TResult>> query, bool notrack = false);
        /// <summary> 获取所有采集通道 </summary>
        Task<List<TResult>> GetCapturechannelsAsync<TResult>(Func<IQueryable<DbpCapturechannels>, IQueryable<TResult>> query, bool notrack = false);
        /// <summary> 获取所有的采集设备信息 </summary>
        Task<List<TResult>> GetCapturedeviceAsync<TResult>(Func<IQueryable<DbpCapturedevice>, IQueryable<TResult>> query, bool notrack = false);
        /// <summary> 获取虚拟通道信息 </summary>
        Task<List<TResult>> GetIpVirtualchannelAsync<TResult>(Func<IQueryable<DbpIpVirtualchannel>, IQueryable<TResult>> query, bool notrack = false);
        /// <summary> 获取通道分组信息 </summary>
        Task<List<TResult>> GetChannelgroupmapAsync<TResult>(Func<IQueryable<DbpChannelgroupmap>, IQueryable<TResult>> query, bool notrack = false);

        /// <summary> 查询信号源的扩展信息 </summary>
        Task<List<TResult>> GetSignalSrcExsAsync<TResult>(Func<IQueryable<DbpSignalsrcMasterbackup>, IQueryable<TResult>> query, bool notrack = false);
        /// <summary> 查询信号源的扩展信息 </summary>
        Task<TResult> GetSignalSrcExsAsync<TResult>(Func<IQueryable<DbpSignalsrcMasterbackup>, Task<TResult>> query, bool notrack = false);


        /// <summary>SignalDeviceMap 查询信号设备映射信息 </summary>
        Task<List<TResult>> GetSignalDeviceMapAsync<TResult>(Func<IQueryable<DbpSignalDeviceMap>, IQueryable<TResult>> query, bool notrack = false);
        /// <summary>SignalDeviceMap 查询信号设备映射信息 </summary>
        Task<TResult> GetSignalDeviceMapAsync<TResult>(Func<IQueryable<DbpSignalDeviceMap>, Task<TResult>> query, bool notrack = false);
        /// <summary>SignalDeviceMap 设置信号源和采集设备映射 </summary>
        Task<int> SaveSignalDeviceMap(DbpSignalDeviceMap model);


        /// <summary> 查询MSV设备状态信息 </summary>
        Task<TResult> GetMsvchannelStateAsync<TResult>(Func<IQueryable<DbpMsvchannelState>, Task<TResult>> query, bool notrack = false);


        /// <summary> 查询信号源分组信息 </summary>
        Task<List<TResult>> GetSignalGroupAsync<TResult>(Func<IQueryable<DbpSignalgroup>, IQueryable<TResult>> query, bool notrack = false);
        /// <summary> 查询信号源分组信息 </summary>
        Task<TResult> GetSignalGroupAsync<TResult>(Func<IQueryable<DbpSignalgroup>, Task<TResult>> query, bool notrack = false);


        /// <summary>查询GPI的映射信息</summary>
        Task<List<TResult>> GetGPIMapInfoByGPIIDAsync<TResult>(Func<IQueryable<DbpGpiMap>, IQueryable<TResult>> query, bool notrack = false);
        /// <summary>查询GPI的映射信息</summary>
        Task<TResult> GetGPIMapInfoByGPIIDAsync<TResult>(Func<IQueryable<DbpGpiMap>, Task<TResult>> query, bool notrack = false);


        /// <summary>根据通道Id获取参数类型</summary>
        Task<int?> GetParamTypeByChannleIDAsync(int nChannelID);

        /// <summary> 获取所有信号源分组信息 </summary>
        Task<List<SignalGroupState>> GetAllSignalGroupInfoAsync();
    }
}
