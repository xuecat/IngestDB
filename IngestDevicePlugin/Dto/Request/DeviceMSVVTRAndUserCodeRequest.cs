using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IngestDevicePlugin.Dto.Request
{
    public class DeviceMSVVTRAndUserCodeRequest
    {
        /// <summary>ID集</summary>
        /// <example>1,2,3</example>
        public int[] IDArray { get; set; }
        /// <summary>资源VTRID</summary>
        /// <example>1</example>
        public int SourceVTRID { get; set; }
        /// <summary>用户Code</summary>
        /// <example>ingest_admin</example>
        public string UserCode { get; set; }
    }
}
