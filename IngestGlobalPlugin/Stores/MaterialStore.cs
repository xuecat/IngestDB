using IngestGlobalPlugin.Dto.Request;
using IngestGlobalPlugin.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IngestGlobalPlugin.Stores
{
    public class MaterialStore
    {
        protected IngestGlobalDBContext Context { get; }

        public MaterialStore(IngestGlobalDBContext baseDataDbContext)
        {
            Context = baseDataDbContext;
        }
        public async Task<bool> AddMQMsg(DbpMsmqmsg info)
        {
            if (!Context.DbpMsmqmsg.Any(a => a.Msgid == info.Msgid))
            {
                info.Nextretry = DateTime.Now;
                Context.DbpMsmqmsg.Add(info);

                try
                {
                    await Context.SaveChangesAsync();
                }
                catch (Exception)
                {

                    throw;
                }
            }
            
            return true;
        }

        public async Task<TResult> GetMqMsgAsync<TResult>(Func<IQueryable<DbpMsmqmsg>, IQueryable<TResult>> query, bool notrack = false)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            if (notrack)
            {
                return await query.Invoke(Context.DbpMsmqmsg.AsNoTracking()).FirstOrDefaultAsync();
            }
            return await query.Invoke(Context.DbpMsmqmsg).FirstOrDefaultAsync();
        }
    }
}
