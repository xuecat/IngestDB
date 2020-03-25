using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IngestTaskPlugin.Models;
using Microsoft.EntityFrameworkCore;

namespace IngestTaskPlugin.Stores
{
    public class TaskInfoStore : ITaskStore
    {
        public TaskInfoStore(IngestTaskDBContext baseDataDbContext)
        {
            Context = baseDataDbContext;
        }

        protected IngestTaskDBContext Context { get; }


        public async Task<TResult> GetTaskMetaDataAsync<TResult>(Func<IQueryable<DbpTaskMetadata>, IQueryable<TResult>> query)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            return await query.Invoke(Context.DbpTaskMetadata).SingleOrDefaultAsync();
        }
    }
}
