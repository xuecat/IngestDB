using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IngestDevicePlugin.Dto.Response
{
    /// <summary>信号源扩展信息</summary>
    public class SignalSrcExResponse
    {
        /// <summary>信号源ID</summary>
        public int ID;

        /// <summary>类型</summary>
        public int SignalSrcType;

        /// <summary>是否是主信号源</summary>
        public bool IsMainSignalSrc;

        /// <summary>主信号源ID</summary>
        public int MainSignalSrcId;

        /// <summary>备信号源ID</summary>
        public int BackupSignalSrcId;
    }
}
