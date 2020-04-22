using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IngestDevicePlugin.Dto.Response
{
    /// <summary>GPI设备信息</summary>
    public class GPIDeviceInfoResponse
    {
        /// <summary>GPI编号</summary>
        public int GPIID { get; set; } = -1;
        /// <summary>GPI名字</summary>
        public string GPIName { get; set; } = string.Empty;
        /// <summary>GPI位于哪个Com端口上</summary>
        public int ComPort { get; set; } = -1;
        /// <summary>GPI总的出口数</summary>
        public int OutputPortCount { get; set; } = -1;
        /// <summary>GPI的描述</summary>
        public string Description { get; set; } = string.Empty;
    }
}
