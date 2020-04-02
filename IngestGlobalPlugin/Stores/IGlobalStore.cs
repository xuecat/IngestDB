using IngestGlobalPlugin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IngestGlobalPlugin.Stores
{
    public interface IGlobalStore
    {
        Task UpdateGlobalStateAsync(string strLabel);
    }
}
