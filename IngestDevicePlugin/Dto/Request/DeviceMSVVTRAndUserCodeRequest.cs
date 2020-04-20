using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IngestDevicePlugin.Dto.Request
{
    public class DeviceMSVVTRAndUserCodeRequest
    {
        public int[] nIDArray;
        public int nSourceVTRID;
        public string userCode;
    }
}
