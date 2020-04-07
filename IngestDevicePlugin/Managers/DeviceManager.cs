using AutoMapper;
using IngestDevicePlugin.Stores;
using IngestDevicePlugin.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        protected IDeviceStore Store { get; }
        protected IMapper _mapper { get; }

        public async virtual Task<List<TResult>> GetAllRouterInPortAsync<TResult>()
        { 
            return _mapper.Map<List<TResult>>(await Store.GetAllRouterInPortInfoAsync(a=>a, true));
        }

        public async Task<bool> HaveMatrixAsync()
        {
            return await Store.HaveMatirxAsync();
        }

        public async Task<ProgrammeInfoResponse> GetProgrammeInfoByIdAsync(int programeid)
        {
            return await Store.GetSignalInfoAsync(programeid);
        }

        public async virtual Task<List<TResult>> GetChannelsByProgrammeIdAsync<TResult>(int programmid)
        {
            //判断是否是无矩阵
            bool isHaveMatrix = await HaveMatrixAsync();
            var programinfo = await GetProgrammeInfoByIdAsync(programmid);

            var channels = await Store.GetAllCaptureChannelsAsync();
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

            return  _mapper.Map<List<TResult>>(channelInfoList);
        }
        ////////////////////
    }
}
