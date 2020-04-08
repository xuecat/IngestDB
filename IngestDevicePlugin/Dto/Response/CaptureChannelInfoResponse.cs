using System;
using System.Collections.Generic;
using System.Text;

namespace IngestDevicePlugin.Dto
{
    public class CaptureChannelInfoResponse
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Desc { get; set; }
        public int CPDeviceID { get; set; }
        public int ChannelIndex { get; set; }
        public int DeviceTypeID { get; set; } = (int)CaptureChannelType.emMsvChannel;//当前sdi，只会为1。2表示是IPTS通道
        public emBackupFlag BackState { get; set; } = emBackupFlag.emNoAllowBackUp;
        public int CarrierID { get; set; }//运营商ID
        public int OrderCode { get; set; } = -1;//序号
        public int CPSignalType { get; set; }//可以采集的信号源类型，0：Auto，1：SD，2：HD

        // 分组ID
        public int GroupID { get; set; }    // Add by chenzhi 2103-07-04
    }
}
