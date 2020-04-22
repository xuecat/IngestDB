using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IngestDevicePlugin.Dto.Enum;

namespace IngestDevicePlugin.Dto
{
    /// <summary>MSV设备状态信息</summary>
    public class MSVChannelStateResponse
    {
        /// <summary>通道ID</summary>
        public int ChannelID { get; set; }

        /// <summary>设备状态</summary>
        public Device_State DevState { get; set; } = Device_State.DISCONNECTTED;

        /// <summary>MSV模式</summary>
        public MSV_Mode MSVMode { get; set; } = MSV_Mode.LOCAL;

        /// <summary>vtrID</summary>
        public int VtrID { get; set; } = -1;

        /// <summary>当前用户Code</summary>
        public string UserCode { get; set; } = string.Empty;

        /// <summary>kamataki信息</summary>
        public string KamatakiInfo { get; set; } = string.Empty;

        /// <summary>上载模式</summary>
        public Upload_Mode UploadMode { get; set; } = Upload_Mode.NOUPLOAD;

        /// <summary>通道索引</summary>
        public int ChannelIndex { get; set; }
    }
}
