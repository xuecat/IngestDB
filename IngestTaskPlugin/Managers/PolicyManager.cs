using AutoMapper;
using IngestTaskPlugin.Stores.Policy;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IngestTaskPlugin.Managers
{
    public class PolicyManager
    {
        public PolicyManager(IPolicyStore store, IMapper mapper)
        {
            Store = store;
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

            //_deviceInterface = new Lazy<IIngestDeviceInterface>(() => services.GetRequiredService<IIngestDeviceInterface>());
        }
        protected IPolicyStore Store { get; }
        protected IMapper _mapper { get; }


        public async virtual Task<List<TResult>> GetPolicyByTaskIDAsync<TResult>(int taskid)
        {
            return _mapper.Map<List<TResult>>(await Store.GetPolicyListByTaskAsync(taskid));
        }

        public async virtual Task<List<TResult>> GetAllMetaDataPolicyAsync<TResult>()
        {
            return _mapper.Map<List<TResult>>(await Store.GetAllMetaDataPolicy());
        }

    }
}
