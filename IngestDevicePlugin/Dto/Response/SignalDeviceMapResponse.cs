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
        public int SignalID;

        /// <summary>设备ID</summary>
        public int DeviceID;

        /// <summary>信号设备输出通道索引</summary>
        public int OutPortIdx;

        /// <summary>信号来源</summary>
        public emSignalSource SignalSource;
    }
}
