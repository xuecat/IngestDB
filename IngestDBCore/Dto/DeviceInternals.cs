using System;
using System.Collections.Generic;
using System.Text;

namespace IngestDBCore
{
    
    public class DeviceInternals
    {
        public enum FunctionType
        {
            ChannelInfoBySrc
        }

        public FunctionType funtype { get; set; }

        public int SrcId { get; set; }
    }
}
