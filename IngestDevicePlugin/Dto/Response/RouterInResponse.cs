using System;
using System.Collections.Generic;
using System.Text;
using IngestDevicePlugin.Dto.Enum;
using IngestDevicePlugin.Dto.Response;

namespace IngestDevicePlugin.Dto.Response
{
    public class RouterInResponse
    {
        public int RcDeviceId { get; set; }
        public int RcInportIdx { get; set; }
        public int SignalSrcId { get; set; }
        public emSignalSource SignalSource { get; set; } = emSignalSource.emSatlitlleSource;//信号来源， 0:卫星 1:总控矩阵 2 视频服务器 3: VTR 4: MSV  5 蓝光  其他以后再扩展
    }
}
