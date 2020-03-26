using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IngestDevicePlugin.Models;
using Microsoft.EntityFrameworkCore;

namespace IngestDevicePlugin.Stores
{
    public class DeviceInfoStore : IDeviceStore
    {
        public DeviceInfoStore(IngestDeviceDBContext baseDataDbContext)
        {
            Context = baseDataDbContext;
        }

        protected IngestDeviceDBContext Context { get; }

        public async Task<List<DbpRcdindesc>> GetAllRouterInPortInfoAsync<TResult>(Func<IQueryable<DbpRcdindesc>, IQueryable<TResult>> query)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            return await Context.DbpRcdindesc.ToListAsync();
        }

    }
}
