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
        //IQueryable<TaskInfo> SimpleQuery { get; }

        Task<List<DbpRcdindesc>> GetAllRouterInPortInfoAsync<TResult>(Func<IQueryable<DbpRcdindesc>, IQueryable<TResult>> query, bool notrack = false);

        Task<bool> HaveMatirxAsync();

        Task<ProgrammeInfoDto> GetSignalInfoAsync(int srcid);

        Task<CaptureChannelInfoDto> GetCaptureChannelByIDAsync(int channelid);
        Task<List<CaptureChannelInfoDto>> GetAllCaptureChannelsAsync(int status);
        Task<List<int>> GetChannelIdsBySignalIdForNotMatrix(int signalid);
        Task<List<int>> GetSignalIdsByChannelIdForNotMatrix(int channelid);
        Task<List<DbpChannelRecmap>> GetAllChannelUnitMap();
        Task<DbpChannelRecmap> GetChannelUnitMap(int channel);

        Task<int> GetMatrixChannelBySignal(int channelid);
    }
}
