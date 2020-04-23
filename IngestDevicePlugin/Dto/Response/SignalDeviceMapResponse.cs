using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IngestDevicePlugin.Dto.Enum;

namespace IngestDevicePlugin.Dto.Response
{
    /// <summary>信号设备详情</summary>
    public class SignalDeviceMapResponse
    {
        /// <summary>信号源ID</summary>
        /// <example>1086</example>
        public int SignalID { get; set; }

        /// <summary>设备ID</summary>
        /// <example>86</example>
        public int DeviceID { get; set; }

        /// <summary>信号设备输出通道索引</summary>
        /// <example>68</example>
        public int OutPortIdx { get; set; }

        /// <summary>信号来源</summary>
        /// <example>3</example>
        public emSignalSource SignalSource { get; set; }
    }
}
