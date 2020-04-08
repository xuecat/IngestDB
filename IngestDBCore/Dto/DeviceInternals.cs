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

        public int Status { get; set; }
    }

    //采集通道类型
    public enum CaptureChannelTypeInterface
    {
        emMsvChannel = 1,		//MSV  采集通道
        emIPTSChannel,                //IPTS 虚拟的通道
        emStreamChannel               //流媒体通道  
    }

    /// <summary>
    /// 任务备份属性
    /// </summary>
	public enum BackupFlagInterface
    {
        /// <summary>
        /// 不允许备份 = 0
        /// </summary>
		emNoAllowBackUp = 0,
        /// <summary>
        /// 允许备份 = 1
        /// </summary>
		emAllowBackUp = 1,
        /// <summary>
        /// 只允许作备份 = 2
        /// </summary>
		emBackupOnly = 2
    }

    public class CaptureChannelInfoInterface
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Desc { get; set; }
        public int CPDeviceID { get; set; }
        public int ChannelIndex { get; set; }
        public int DeviceTypeID { get; set; } = (int)CaptureChannelTypeInterface.emMsvChannel;//当前sdi，只会为1。2表示是IPTS通道
        public BackupFlagInterface BackState { get; set; } = BackupFlagInterface.emNoAllowBackUp;
        public int CarrierID { get; set; }//运营商ID
        public int OrderCode { get; set; } = -1;//序号
        public int CPSignalType { get; set; }//可以采集的信号源类型，0：Auto，1：SD，2：HD

        // 分组ID
        public int GroupID { get; set; }    // Add by chenzhi 2103-07-04
    }

}
