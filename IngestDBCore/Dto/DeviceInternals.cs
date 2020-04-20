using System;
using System.Collections.Generic;
using System.Text;

namespace IngestDBCore
{
    
    public class DeviceInternals
    {
        public enum FunctionType
        {
            /// //////////////get信息//////
            ChannelInfoBySrc,
            SingnalInfoByChannel,
            AllChannelUnitMap,
            ChannelUnitMap,
            BackSignalByID,
            SignalInfoByID,
            SignalCaptureID,
            AllChannelState,
            ChannelExtendData
           /////////////////////////////set信息//
        }

        public FunctionType funtype { get; set; }

        public int SrcId { get; set; }
        public int ChannelId { get; set; }

        public int Status { get; set; }
    }
    public class RecUnitMapInterface
    {
        public int UnitID { get; set; }
        public int ConnectorID { get; set; }
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
    public enum ProgrammeTypeInterface
    {
        PT_Null = -1,
        PT_SDI,
        PT_IPTS,
        PT_StreamMedia
    }
    /// <summary>图像类型</summary>
    public enum ImageTypeInterface
    {
        IT_Null = -1,
        IT_Original = 0,
        IT_SD_4_3 = 1,
        IT_SD_16_9 = 2,
        IT_HD_16_9 = 4
    }
    /// <summary> 信号来源 </summary>
    public enum emSignalSourceInterface
    {
        /// <summary>卫星</summary>
        emSatlitlleSource = 0,
        /// <summary>总控矩阵</summary>
        emCtrlMatrixSource = 1,
        /// <summary>视频服务器</summary>
        emVideoServerSource = 2,
        /// <summary>VTR</summary>
        emVtrSource = 3,
        /// <summary>MSV</summary>
        emMSVSource = 4,
        /// <summary>蓝光</summary>
        emXDCAM = 5,
        /// <summary>IPTS流</summary>
        emIPTS = 6,
        /// <summary>流媒体</summary>
        emStreamMedia = 7
    }
    public class ProgrammeInfoInterface
    {
        public int ProgrammeId { set; get; }
        public string ProgrammeName { set; get; }
        public string ProgrammeDesc { set; get; }

        //高标清
        public int TypeId { set; get; }
        public ProgrammeTypeInterface PgmType { set; get; }
        public ImageTypeInterface ImageType { set; get; }
        public emSignalSourceInterface SignalSourceType { set; get; }
        public int PureAudio { set; get; }
        public int CarrierID { set; get; }//运营商的ID
        public int GroupID { set; get; } // Add by chenzhi 2013-07-08 分组ID
    }

    /// <summary>设备状态</summary>
    public enum Device_StateInterface
    {
        /// <summary>没有连接</summary>
        DISCONNECTTED,

        /// <summary>已经连接</summary>
        CONNECTED,

        /// <summary>正在采集</summary>
        WORKING
    }

    /// <summary>MSV模式</summary>
    public enum MSV_ModeInterface
    {
        /// <summary>本地</summary>
        LOCAL,

        /// <summary>网络</summary>
        NETWORK
    }

    /// <summary>上载模式</summary>
    public enum Upload_ModeInterface
    {
        /// <summary></summary>
        NOUPLOAD,

        /// <summary></summary>
        CANUPLOAD,

        /// <summary>上载独占</summary>
        ONLYUPLOAD
    }


    public class MSVChannelStateInterface
    {
        /// <summary>通道ID</summary>
        public int ChannelID { get; set; }

        /// <summary>设备状态</summary>
        public Device_StateInterface DevState { get; set; }

        /// <summary>MSV模式</summary>
        public MSV_ModeInterface MSVMode { get; set; }

        /// <summary>vtrID</summary>
        public int VtrID { get; set; } = -1;

        /// <summary>当前用户Code</summary>
        public string UserCode { get; set; }

        /// <summary>kamataki信息</summary>
        public string KamatakiInfo { get; set; }

        /// <summary>上载模式</summary>
        public Upload_ModeInterface UploadMode { get; set; }

        /// <summary>通道索引</summary>
        public int ChannelIndex { get; set; }
    }

}
