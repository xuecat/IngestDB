using IngestTaskPlugin.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace IngestTaskPlugin.Stores.Policy
{
    public class PolicyStore : IPolicyStore
    {
        public PolicyStore(IngestTaskDBContext baseDataDbContext)
        {
            Context = baseDataDbContext;
        }
        protected IngestTaskDBContext Context { get; }
        public async Task<List<DbpMetadatapolicy>> GetPolicyListByTaskAsync(int taskid)
        {
            //var lst = await Context.DbpPolicytask.AsNoTracking().Where(x => x.Taskid == taskid).ToListAsync();

            var result = await ( from d in Context.DbpPolicytask.AsNoTracking().Where(x => x.Taskid == taskid)
                         from m in Context.DbpMetadatapolicy.AsNoTracking().Where(x => x.Policyid == d.Policyid)
                         select new DbpMetadatapolicy() {
                             Archivetype = m.Archivetype,
                             Defaultpolicy = m.Defaultpolicy,
                             Policydesc = m.Policydesc,
                             Policyid = m.Policyid,
                             Policyname = m.Policyname
                        }).ToListAsync();
            if (result != null)
            {
                return result;
            }
            return null;
        }

        public async Task<List<DbpMetadatapolicy>> GetAllMetaDataPolicy()
        {
            return await Context.DbpMetadatapolicy.ToListAsync();
        }
    }
}
