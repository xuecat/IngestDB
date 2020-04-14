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

        public async Task<List<TResult>> GetAllRouterInPortInfoAsync<TResult>(Func<IQueryable<DbpRcdindesc>, IQueryable<TResult>> query, bool notrack = false)
        {
            return await QueryModelAsync(Context.DbpRcdindesc, query, notrack);
        }

        public async Task<List<TResult>> GetAllRouterOutPortInfoAsync<TResult>(Func<IQueryable<DbpRcdoutdesc>, IQueryable<TResult>> query, bool notrack = false)
        {
            return await QueryModelAsync(Context.DbpRcdoutdesc, query, notrack);
        }

        public async Task<List<TResult>> GetAllSignalDeviceMapAsync<TResult>(Func<IQueryable<DbpSignalDeviceMap>, IQueryable<TResult>> query, bool notrack = false)
        {
            return await QueryModelAsync(Context.DbpSignalDeviceMap, query, notrack);
        }

        public async Task<List<TResult>> GetAllSignalSrcsAsync<TResult>(Func<IQueryable<DbpSignalsrc>, IQueryable<TResult>> query, bool notrack = false)
        {
            return await QueryModelAsync(Context.DbpSignalsrc, query, notrack);
        }

        public async Task<List<TResult>> GetAllCaptureChannelsAsync<TResult>(Func<IQueryable<DbpCapturechannels>, IQueryable<TResult>> query, bool notrack = false)
        {
            return await QueryModelAsync(Context.DbpCapturechannels, query, notrack);
        }

        public async Task<List<TResult>> GetAllCaptureDevicesAsync<TResult>(Func<IQueryable<DbpCapturedevice>, IQueryable<TResult>> query, bool notrack = false)
        {
            return await QueryModelAsync(Context.DbpCapturedevice, query, notrack);
        }

        public async Task<List<TResult>> GetAllVirtualChannelsAsync<TResult>(Func<IQueryable<DbpIpVirtualchannel>, IQueryable<TResult>> query, bool notrack = false)
        {
            return await QueryModelAsync(Context.DbpIpVirtualchannel, query, notrack);
        }

        public async Task<List<TResult>> GetAllChannelGroupMapAsync<TResult>(Func<IQueryable<DbpChannelgroupmap>, IQueryable<TResult>> query, bool notrack = false)
        {
            return await QueryModelAsync(Context.DbpChannelgroupmap, query, notrack);
        }

        public TResult GetSignalDeviceMap<TResult>(Func<IQueryable<DbpSignalDeviceMap>, IQueryable<TResult>> query, bool notrack = false)
        {
            return GetFirstOrDefault(Context.DbpSignalDeviceMap, query, notrack);
        }

        public async Task<int> SetSignalDeviceMap(DbpSignalDeviceMap model)
        {
            var deviceMap = await Context.DbpSignalDeviceMap.FindAsync(model.Signalsrcid);
            if (deviceMap == null)
            {
                await Context.DbpSignalDeviceMap.AddAsync(model);
            }
            else
            {
                deviceMap.Deviceid = model.Deviceid;
                deviceMap.Deviceoutportidx = model.Deviceid;
                deviceMap.Signalsource = model.Signalsource;
            }
            try
            {
                return await Context.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {
                throw e;
            }
        }

        protected TResult GetFirstOrDefault<TDbp, TResult>(DbSet<TDbp> contextSet, Func<IQueryable<TDbp>, IQueryable<TResult>> query, bool notrack = false) where TDbp : class
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            if (notrack)
            {
                return query(contextSet.AsNoTracking()).FirstOrDefault();
            }
            return query(contextSet).FirstOrDefault();
        }

        protected async Task<List<TResult>> QueryModelAsync<TDbp, TResult>(DbSet<TDbp> contextSet, Func<IQueryable<TDbp>, IQueryable<TResult>> query, bool notrack = false) where TDbp : class
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            if (notrack)
            {
                return await query(contextSet.AsNoTracking()).ToListAsync();
            }
            return await query(contextSet).ToListAsync();
        }
    }
}
