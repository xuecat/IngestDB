using IngestGlobalPlugin.Models;
using Microsoft.EntityFrameworkCore;
using Sobey.Core.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IngestGlobalPlugin.Stores
{
    public class GlobalInfoStore : IGlobalStore
    {
        private readonly ILogger Logger = LoggerManager.GetLogger("GlobalInfo");
        protected IngestGlobalDBContext Context { get; }

        public GlobalInfoStore(IngestGlobalDBContext baseDataDbContext)
        {
            Context = baseDataDbContext;
        }

        //public async Task GetTaskAsync( IQueryable query, bool notrack = false)
        //{
        //    if (query == null)
        //    {
        //        throw new ArgumentNullException(nameof(query));
        //    }
        //    if (notrack)
        //    {
        //        return await query.Invoke(Context.DbpTask.AsNoTracking()).SingleOrDefaultAsync();
        //    }
        //    return await query.Invoke(Context.DbpTask).SingleOrDefaultAsync();
        //}

        public async Task UpdateGlobalStateAsync(string strLabel)
        {
            if(!Context.DbpGlobalState.AsNoTracking().Any(a => a.Label == strLabel))
            {
                //add
                Context.DbpGlobalState.Add(new DbpGlobalState() {
                    Label = strLabel,
                    Lasttime = DateTime.Now  //.ToString("yyyy-MM-dd HH:mm:ss")
                });
            }
            else
            {
                //update
                var state = new DbpGlobalState()
                {
                    Label = strLabel,
                    Lasttime = DateTime.Now  //.ToString("yyyy-MM-dd HH:mm:ss")
                };
                Context.Attach(state);
                Context.Entry(state).Property(x => x.Lasttime).IsModified = true;
            }

            try
            {
                await Context.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {
                throw e;
            }
        }
        
    }
}
