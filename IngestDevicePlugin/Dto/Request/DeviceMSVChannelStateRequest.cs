using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IngestDevicePlugin.Dto.Enum;

namespace IngestDevicePlugin.Dto.Request
{
    /// <summary>更改设备MSV通道状态</summary>
    public class DeviceMSVChannelStateRequest
    {
        /// <summary>设备状态</summary>
        /// <example>1</example>
        public int DevState { get; set; }
        /// <summary>MSV模式</summary>
        /// <example>0</example>
        public int MSVMode { get; set; }
    }
}
