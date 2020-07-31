using System;
using System.Collections.Generic;
using System.Text;
using IngestDevicePlugin.Dto.Enum;
using IngestDevicePlugin.Dto.Response;

namespace IngestDevicePlugin.Dto.Response
{
    public class CaptureChannelInfoResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Desc { get; set; }
        public int CpDeviceId { get; set; }
        public int ChannelIndex { get; set; }
        public int DeviceTypeId { get; set; } = (int)CaptureChannelType.emMsvChannel;//当前sdi，只会为1。2表示是IPTS通道
        public emBackupFlag BackState { get; set; } = emBackupFlag.emNoAllowBackUp;
        public int CarrierId { get; set; }//运营商Id
        public int OrderCode { get; set; } = -1;//序号
        public int CpSignalType { get; set; }//可以采集的信号源类型，0：Auto，1：SD，2：HD

        // 分组Id
        public int GroupId { get; set; }    // Add by chenzhi 2103-07-04
        public Device_State DeviceState { get; set; }
    }
}
