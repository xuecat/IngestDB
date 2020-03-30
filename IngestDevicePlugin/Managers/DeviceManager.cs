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

        protected IDeviceStore Store { get; }
        protected IMapper _mapper { get; }

        public async virtual Task<List<TResult>> GetAllRouterInPortAsync<TResult>()
        { 
            return _mapper.Map<List<TResult>>(await Store.GetAllRouterInPortInfoAsync(a=>a, true));
        }
    }
}
