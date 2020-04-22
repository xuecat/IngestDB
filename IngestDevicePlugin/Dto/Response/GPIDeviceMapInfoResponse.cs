using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IngestDevicePlugin.Dto.Response
{
    /// <summary>GPI的映射信息</summary>
    public class GPIDeviceMapInfoResponse
    {
        /// <summary>GPI编号</summary>
        public int GPIID { get; set; } = -1;
        /// <summary>GPI输出的端口</summary>
        public int GPIOutputPort { get; set; } = -1;
        /// <summary>GPI对应的端口</summary>
        public int AVOutputPort { get; set; } = -1;
        /// <summary>GPI对应的采集参数ID</summary>
        public int CaptureParamID { get; set; } = -1;
    }
}
