using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IngestDevicePlugin.Dto.Enum;

namespace IngestDevicePlugin.Dto.Response
{
    /// <summary>输出端口详情</summary>
    public class RoterOutResponse
    {
        /// <summary>设备ID</summary>
        public int RCDeviceID { get; set; }

        /// <summary> 输出通道索引</summary>
        public int RCOutportIdx { get; set; }

        /// <summary>通道Id</summary>
        public int ChannelID { get; set; }

        /// <summary>设备类型</summary>
        public emDeviceType DeviceType { get; set; } = emDeviceType.emDeviceMSV;
    }
}
