using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IngestDevicePlugin.Dto.Request
{
    public class DeviceTSDeviceInfoRequest
    {
        /// <summary>TS设备信息集</summary>
        public List<TSDeviceInfo> deviceInfos { get; set; }
    }
}
