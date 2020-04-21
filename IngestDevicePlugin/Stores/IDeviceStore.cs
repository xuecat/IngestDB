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
        Task<ProgrammeInfoDto> GetSignalInfoAsync(int srcid);
        Task<CaptureChannelInfoDto> GetCaptureChannelByIDAsync(int channelid);
        Task<List<CaptureChannelInfoDto>> GetAllCaptureChannelsAsync(int status);
        /// <summary> 通过 信号Id 获取 通道id（非矩阵） </summary>
        Task<List<int>> GetChannelIdsBySignalIdForNotMatrix(int signalid);
        /// <summary> 通过 通道id 获取 信号Id（非矩阵） </summary>
        Task<List<int>> GetSignalIdsByChannelIdForNotMatrix(int channelid);
        Task<List<DbpChannelRecmap>> GetAllChannelUnitMap();
        Task<DbpChannelRecmap> GetChannelUnitMap(int channel);
        Task<int> GetMatrixChannelBySignalAsync(int channelid);
        Task<int> GetBackUpSignalInfoByID(int srgid);
        #endregion

        /// <summary> 获取输入端口与信号源 </summary>
        Task<List<TResult>> GetRcdindescAsync<TResult>(Func<IQueryable<DbpRcdindesc>, IQueryable<TResult>> query, bool notrack = false);
        /// <summary> 获取输入端口与信号源 </summary>
        Task<TResult> GetRcdindescAsync<TResult>(Func<IQueryable<DbpRcdindesc>, Task<TResult>> query, bool notrack = false);

        /// <summary> 获取输出端口与信号源 </summary>
        Task<List<TResult>> GetRcdoutdescAsync<TResult>(Func<IQueryable<DbpRcdoutdesc>, IQueryable<TResult>> query, bool notrack = false);


        /// <summary> 获取信号源 </summary>
        Task<List<TResult>> GetSignalsrcAsync<TResult>(Func<IQueryable<DbpSignalsrc>, IQueryable<TResult>> query, bool notrack = false);
        /// <summary> 获取信号源 </summary>
        Task<TResult> GetSignalsrcAsync<TResult>(Func<IQueryable<DbpSignalsrc>, Task<TResult>> query, bool notrack = false);



        /// <summary>获取所有输入宽口信号源</summary>
        Task<List<DbpSignalsrc>> GetAllSignalsrcForRcdinAsync(bool notrack = false);


        /// <summary> 获取所有采集通道 </summary>
        Task<List<TResult>> GetCapturechannelsAsync<TResult>(Func<IQueryable<DbpCapturechannels>, IQueryable<TResult>> query, bool notrack = false);
        /// <summary> 获取所有采集通道 </summary>
        Task<TResult> GetCapturechannelsAsync<TResult>(Func<IQueryable<DbpCapturechannels>, Task<TResult>> query, bool notrack = false);


        /// <summary> 获取数据通道信息 </summary>
        Task<List<TResult>> GetIpDatachannelsAsync<TResult>(Func<IQueryable<DbpIpDatachannelinfo>, IQueryable<TResult>> query, bool notrack = false);
        /// <summary> 获取数据通道信息 </summary>
        Task<TResult> GetIpDatachannelsAsync<TResult>(Func<IQueryable<DbpIpDatachannelinfo>, Task<TResult>> query, bool notrack = false);


        /// <summary> 获取的采集设备信息 </summary>
        Task<List<TResult>> GetCapturedeviceAsync<TResult>(Func<IQueryable<DbpCapturedevice>, IQueryable<TResult>> query, bool notrack = false);


        /// <summary> 获取虚拟通道信息 </summary>
        Task<List<TResult>> GetIpVirtualchannelAsync<TResult>(Func<IQueryable<DbpIpVirtualchannel>, IQueryable<TResult>> query, bool notrack = false);
        /// <summary> 获取虚拟通道信息 </summary>
        Task<TResult> GetIpVirtualchannelAsync<TResult>(Func<IQueryable<DbpIpVirtualchannel>, Task<TResult>> query, bool notrack = false);


        /// <summary> 获取通道分组信息 </summary>
        Task<List<TResult>> GetChannelgroupmapAsync<TResult>(Func<IQueryable<DbpChannelgroupmap>, IQueryable<TResult>> query, bool notrack = false);


        /// <summary> 更新或新增通道的扩展数据 </summary>
        Task<int> SaveChannelExtenddataAsync(int nChnID, int type, string data);

        /// <summary> 查询信号源的扩展信息 </summary>
        Task<List<TResult>> GetSignalSrcExsAsync<TResult>(Func<IQueryable<DbpSignalsrcMasterbackup>, IQueryable<TResult>> query, bool notrack = false);
        /// <summary> 查询信号源的扩展信息 </summary>
        Task<TResult> GetSignalSrcExsAsync<TResult>(Func<IQueryable<DbpSignalsrcMasterbackup>, Task<TResult>> query, bool notrack = false);



        /// <summary> 查询信号源的扩展信息 </summary>
        Task<List<TResult>> GetProgramparamMapAsync<TResult>(Func<IQueryable<DbpProgramparamMap>, IQueryable<TResult>> query, bool notrack = false);
        /// <summary> 查询信号源的扩展信息 </summary>
        Task<TResult> GetProgramparamMapAsync<TResult>(Func<IQueryable<DbpProgramparamMap>, Task<TResult>> query, bool notrack = false);


        /// <summary>SignalDeviceMap 查询信号分组映射信息 </summary>
        Task<List<TResult>> GetSignalsrcgroupmapAsync<TResult>(Func<IQueryable<DbpSignalsrcgroupmap>, IQueryable<TResult>> query, bool notrack = false);
        /// <summary>SignalDeviceMap 查询信号分组映射信息 </summary>
        Task<TResult> GetSignalsrcgroupmapAsync<TResult>(Func<IQueryable<DbpSignalsrcgroupmap>, Task<TResult>> query, bool notrack = false);


        /// <summary>SignalDeviceMap 查询信号分组映射信息 </summary>
        Task<List<TResult>> GetSignalsrcMasterbackupAsync<TResult>(Func<IQueryable<DbpSignalsrcMasterbackup>, IQueryable<TResult>> query, bool notrack = false);
        /// <summary>SignalDeviceMap 查询信号分组映射信息 </summary>
        Task<TResult> GetSignalsrcMasterbackupAsync<TResult>(Func<IQueryable<DbpSignalsrcMasterbackup>, Task<TResult>> query, bool notrack = false);


        /// <summary>SignalDeviceMap 查询信号设备映射信息 </summary>
        Task<List<TResult>> GetSignalDeviceMapAsync<TResult>(Func<IQueryable<DbpSignalDeviceMap>, IQueryable<TResult>> query, bool notrack = false);
        /// <summary>SignalDeviceMap 查询信号设备映射信息 </summary>
        Task<TResult> GetSignalDeviceMapAsync<TResult>(Func<IQueryable<DbpSignalDeviceMap>, Task<TResult>> query, bool notrack = false);


        /// <summary>SignalDeviceMap 设置信号源和采集设备映射 </summary>
        Task<int> SaveSignalDeviceMapAsync(DbpSignalDeviceMap model);


        /// <summary> 查询MSV设备状态信息 </summary>
        Task<List<TResult>> GetMsvchannelStateAsync<TResult>(Func<IQueryable<DbpMsvchannelState>, IQueryable<TResult>> query, bool notrack = false);
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


        /// <summary>查询GPI信息</summary>
        Task<List<TResult>> GetGPIInfoAsync<TResult>(Func<IQueryable<DbpGpiInfo>, IQueryable<TResult>> query, bool notrack = false);
        /// <summary>查询GPI信息</summary>
        Task<TResult> GetGPIInfoAsync<TResult>(Func<IQueryable<DbpGpiInfo>, Task<TResult>> query, bool notrack = false);


        /// <summary>查询设备信息</summary>
        Task<List<TResult>> GetIpDeviceAsync<TResult>(Func<IQueryable<DbpIpDevice>, IQueryable<TResult>> query, bool notrack = false);
        /// <summary>查询设备信息</summary>
        Task<TResult> GetIpDeviceAsync<TResult>(Func<IQueryable<DbpIpDevice>, Task<TResult>> query, bool notrack = false);


        /// <summary>查询节目信息</summary>
        Task<List<TResult>> GetIpProgrammeAsync<TResult>(Func<IQueryable<DbpIpProgramme>, IQueryable<TResult>> query, bool notrack = false);
        /// <summary>查询节目信息</summary>
        Task<TResult> GetIpProgrammeAsync<TResult>(Func<IQueryable<DbpIpProgramme>, Task<TResult>> query, bool notrack = false);


        /// <summary>查询流媒体信息</summary>
        Task<List<TResult>> GetStreamMediaAsync<TResult>(Func<IQueryable<DbpStreammedia>, IQueryable<TResult>> query, bool notrack = false);
        /// <summary>查询流媒体信息</summary>
        Task<TResult> GetStreamMediaAsync<TResult>(Func<IQueryable<DbpStreammedia>, Task<TResult>> query, bool notrack = false);


        /// <summary>根据通道Id获取参数类型</summary>
        Task<int?> GetParamTypeByChannleIDAsync(int nChannelID);
        /// <summary>根据信号源获取是高清还是标清</summary>
        Task<int?> GetParamTypeBySignalIDAsync(int nSignalID);


        /// <summary> 获取所有信号源分组信息 </summary>
        Task<List<SignalGroupState>> GetAllSignalGroupInfoAsync();

        /// <summary> </summary>
        Task<List<Channel2SignalSrcMap>> GetAllChannel2SignalSrcMapAsync();

        
        Task<List<TResult>> GetUserSettingAsync<TResult>(Func<IQueryable<DbpUsersettings>, IQueryable<TResult>> query, bool notrack = false);
        Task<TResult> GetUserSettingAsync<TResult>(Func<IQueryable<DbpUsersettings>, Task<TResult>> query, bool notrack = false);


        Task<int> ModifySourceVTRIDAndUserCodeAsync(int nSourceVTRID, string userCode, params int[] nID);

        Task<int> UpdateMSVChannelStateAsync(DbpMsvchannelState model);


        Task<int> SaveIpVirtualchannelAsync(IEnumerable<DbpIpVirtualchannel> models);

        Task<int> SaveIpProgrammeAsync(IEnumerable<DbpIpProgramme> models);

        Task<int> SaveIpDatachannelinfoAsync(IEnumerable<DbpIpDatachannelinfo> models);

        Task<int> SaveIpDeviceAsync(IEnumerable<DbpIpDevice> models);

        /// <summary> 是否矩阵 </summary>
        Task<bool> HaveMatirxAsync();
    }
}
