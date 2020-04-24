using AutoMapper;
using IngestGlobalPlugin.Models;
using IngestGlobalPlugin.Stores;
using Sobey.Core.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IngestGlobalPlugin.Managers
{
    public class MaterialManager
    {
        private readonly ILogger Logger = LoggerManager.GetLogger("MaterialInfo");

        protected IMaterialStore Store { get; }
        protected IMapper _mapper { get; }

        public MaterialManager(IMaterialStore store, IMapper mapper)
        {
            Store = store;
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<bool> AddMqMsg<T>(T info)
        {
            /*
             * @brief 记得加上 DateTime tmProcess = DateTime.Now;
            if (!string.IsNullOrEmpty(msg.MsgProcessTime))
            {
                DateTime.TryParse(msg.MsgProcessTime, out tmProcess);
            }
             */
            var msg = _mapper.Map<DbpMsmqmsg>(info);
            if (msg != null)
            {
                await Store.AddMQMsg(msg);
                return true;
            }
            return false;
        }
    }
}
