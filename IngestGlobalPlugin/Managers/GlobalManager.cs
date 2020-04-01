using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using IngestDBCore;
using IngestGlobalPlugin.Stores;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace IngestGlobalPlugin.Managers
{
    public class GlobalManager
    {
        protected IGlobalStore Store { get; }

        public GlobalManager(IGlobalStore store)
        {
            Store = store;
        }


        public async Task SetGlobalState2(string strLabel)
        {
            await Store.GetGlobalStateAsync(strLabel);
        }
    }
}
