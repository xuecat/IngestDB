using IngestDevicePlugin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IngestDevicePlugin.Stores
{
    
    public interface IDeviceStore
    {
        //IQueryable<TaskInfo> SimpleQuery { get; }

        Task<List<DbpRcdindesc>> GetAllRouterInPortInfoAsync<TResult>(Func<IQueryable<DbpRcdindesc>, IQueryable<TResult>> query, bool notrack = false);

    }
}
