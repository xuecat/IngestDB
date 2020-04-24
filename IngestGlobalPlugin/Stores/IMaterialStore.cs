using IngestGlobalPlugin.Dto.Request;
using IngestGlobalPlugin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IngestGlobalPlugin.Stores
{
    public interface  IMaterialStore
    {
        Task<bool> AddMQMsg(DbpMsmqmsg info);
        Task<TResult> GetMqMsgAsync<TResult>(Func<IQueryable<DbpMsmqmsg>, IQueryable<TResult>> query, bool notrack = false);
    }
}
