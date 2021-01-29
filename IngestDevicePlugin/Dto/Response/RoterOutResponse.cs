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
        public int RcDeviceId { get; set; }

        /// <summary> 输出通道索引</summary>
        public int RcOutportIdx { get; set; }

        /// <summary>通道Id</summary>
        public int ChannelId { get; set; }

        /// <summary>设备类型</summary>
        public emDeviceType DeviceType { get; set; } = emDeviceType.emDeviceMSV;
        /// <summary>
        /// out口区域
        /// </summary>
        public int Area { get; set; }
    }
}
