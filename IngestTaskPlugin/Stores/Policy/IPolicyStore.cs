using IngestTaskPlugin.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IngestTaskPlugin.Stores.Policy
{
    public interface IPolicyStore
    {
        Task<List<DbpMetadatapolicy>> GetPolicyListByTaskAsync(int taskid);
        Task<List<DbpMetadatapolicy>> GetAllMetaDataPolicy();
    }
}
