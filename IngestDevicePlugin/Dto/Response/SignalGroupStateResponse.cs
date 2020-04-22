using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IngestDevicePlugin.Dto.Response
{
    /// <summary>信号源分组信息</summary>
    public class SignalGroupStateResponse
    {
        /// <summary>信号源ID</summary>
        public int SignalSrcID;

        /// <summary>信号源对应的分组ID</summary>
        public int GroupID;

        /// <summary>信号源的名称</summary>
        public string GroupName;

        /// <summary>信号源的描述</summary>
        public string GroupDesc;
    }
}
