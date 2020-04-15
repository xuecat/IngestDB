using System;
using System.Collections.Generic;
using System.Text;

namespace IngestDevicePlugin.Dto
{
    public class OldResponse
    {
    }

    #region EnumType

    /// <summary> 信号来源 </summary>
    public enum emSignalSource
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
    /// <summary> 设备类型 </summary>
    public enum emDeviceType
    {
        /// <summary>MSV</summary>
        emDeviceMSV = 0,
        /// <summary>VTR</summary>
        emDeviceVTR = 1,
        /// <summary>蓝光</summary>
        emDeviceXDCAM = 2
    }
    /// <summary> 采集通道类型 </summary>
    public enum CaptureChannelType
    {
        /// <summary> MSV 采集通道 </summary>
        emMsvChannel = 1,
        /// <summary> IPTS 虚拟的通道 </summary>
        emIPTSChannel,
        /// <summary> 流媒体通道 </summary>
        emStreamChannel
    }
    /// <summary> 任务备份属性 </summary>
    public enum emBackupFlag
    {
        /// <summary> 不允许备份 </summary>
		emNoAllowBackUp = 0,
        /// <summary> 允许备份 </summary>
		emAllowBackUp = 1,
        /// <summary> 只允许作备份 </summary>
		emBackupOnly = 2
    }
    #endregion

    #region DescModel

    /// <summary>输入端口详情</summary>
    public class RoterInportDesc
    {
        /// <summary>设备Id</summary>
        public int nRCDeviceID;
        /// <summary>输入通道索引</summary>
        public int nRCInportIdx;
        /// <summary>信号源Id</summary>
        public int nSignalSrcID;
        /// <summary>信号来源</summary>
        public emSignalSource nSignalSource = emSignalSource.emSatlitlleSource;
    }
    /// <summary>输出端口详情</summary>
    public class RoterOutDesc
    {
        /// <summary>设备ID</summary>
        public int nRCDeviceID;
        /// <summary> 输出通道索引</summary>
        public int nRCOutportIdx;
        /// <summary>通道Id</summary>
        public int nChannelID;
        public emDeviceType DeviceType = emDeviceType.emDeviceMSV;
    }
    /// <summary>信号设备详情</summary>
    public class SignalDeviceMap
    {
        /// <summary>信号源ID</summary>
        public int nSignalID;
        /// <summary>设备ID</summary>
        public int nDeviceID;
        /// <summary>信号设备输出通道索引</summary>
        public int nOutPortIdx;
        /// <summary>信号来源</summary>
        public emSignalSource SignalSource;
    }
    /// <summary>信号源详情</summary>
    public class SignalSrcInfo
    {
        /// <summary>Id</summary>
        public int nID;
        /// <summary>名称</summary>
        public string strName;
        /// <summary>详情描述</summary>
        public string strDesc;
        /// <summary>信号源类型Id（高标清）</summary>
        public int nTypeID;
        /// <summary>图像类型;0、为素材原始比例，1、图像为4:3的方式，2：图像为16:9的方式</summary>
        public int nImageType;
        /// <summary>表示是否是纯音频信号源</summary>
        public int nPureAudio;
    }
    /// <summary>采集通道详情</summary>
    public class CaptureChannelInfo
    {
        /// <summary>Id</summary>
        public int nID = 0;
        /// <summary>名称</summary>
        public string strName = "";
        /// <summary>详情描述</summary>
        public string strDesc = "";
        /// <summary>设备Id</summary>
        public int nCPDeviceID = 0;
        /// <summary>通道索引</summary>
        public int nChannelIndex = 0;
        /// <summary>采集通道类型 当前SDI只会为1 MSV，2表示是IPTS通道</summary>
        public int nDeviceTypeID = (int)CaptureChannelType.emMsvChannel;
        /// <summary>任务备份属性（默认不备份）</summary>
        public emBackupFlag BackState = emBackupFlag.emNoAllowBackUp;
        /// <summary>运营商ID</summary>
        public int nCarrierID = 0;
        /// <summary>序号</summary>
        public int orderCode = -1;
        /// <summary>可以采集的信号源类型，0：Auto，1：SD，2：HD</summary>
        public int nCPSignalType = 0;

        /// <summary>分组ID</summary>
        public int nGroupID = -1;    // Add by chenzhi 2103-07-04
    }
    /// <summary>采集设备详情</summary>
    public class CaptureDeviceInfo
    {
        /// <summary>Id</summary>
        public int nID;
        /// <summary>设备类型 当前为0</summary>
        public int nDeviceTypeID;
        /// <summary>设备名称</summary>
        public string strDeviceName;
        /// <summary>IP</summary>
        public string strIP;
        /// <summary>序号</summary>
        public int nOrderCode;
    }
    #endregion

    #region ParamModel
    /// <summary> 输入端口信息 </summary>
    public class GetAllRouterInPortInfo_param : BaseCount_param
    {
        public List<RoterInportDesc> inportDescs;
    }
    //public class CaptureChannelInfo
    //{
    //    public int nID = 0;
    //    public string strName = "";
    //    public string strDesc = "";
    //    public int nCPDeviceID = 0;
    //    public int nChannelIndex = 0;
    //    public int nDeviceTypeID = (int)CaptureChannelType.emMsvChannel;//当前sdi，只会为1。2表示是IPTS通道
    //    public emBackupFlag BackState = emBackupFlag.emNoAllowBackUp;
    //    public int nCarrierID = 0;//运营商ID
    //    public int orderCode = -1;//序号
    //    public int nCPSignalType = 0;//可以采集的信号源类型，0：Auto，1：SD，2：HD

    //    // 分组ID
    //    public int nGroupID = -1;    // Add by chenzhi 2103-07-04
    //}

    /// <summary>
    /// 任务备份属性
    /// </summary>
	//public enum emBackupFlag
 //   {
 //       /// <summary>
 //       /// 不允许备份 = 0
 //       /// </summary>
	//	emNoAllowBackUp = 0,
 //       /// <summary>
 //       /// 允许备份 = 1
 //       /// </summary>
	//	emAllowBackUp = 1,
 //       /// <summary>
 //       /// 只允许作备份 = 2
 //       /// </summary>
	//	emBackupOnly = 2
 //   }

    //采集通道类型
    //public enum CaptureChannelType
    //{
    //    emMsvChannel = 1,		//MSV  采集通道
    //    emIPTSChannel,                //IPTS 虚拟的通道
    //    emStreamChannel               //流媒体通道  
    //}
    public enum ProgrammeType
    {
        PT_Null = -1,
        PT_SDI,
        PT_IPTS,
        PT_StreamMedia
    }

    public enum ImageType
    {
        IT_Null = -1,
        IT_Original = 0,
        IT_SD_4_3 = 1,
        IT_SD_16_9 = 2,
        IT_HD_16_9 = 4
    }

    public class ProgrammeInfo
    {
        public int ProgrammeId { set; get; }
        public string ProgrammeName { set; get; }
        public string ProgrammeDesc { set; get; }

        //高标清
        public int TypeId { set; get; }
        public ProgrammeType emPgmType { set; get; }
        public ImageType emImageType { set; get; }
        public emSignalSource emSignalSourceType { set; get; }
        public int nPureAudio { set; get; }
        public int nCarrierID { set; get; }//运营商的ID
        public int nGroupID { set; get; } // Add by chenzhi 2013-07-08 分组ID
    }
    public class GetChannelsByProgrammeId_out
    {
        public List<CaptureChannelInfo> channelInfos;
        public int validCount;
        public string errStr;
        public bool bRet;
    }
    
    /// <summary> 输出端口信息 </summary>
    public class GetAllRouterOutPortInfo_param : BaseCount_param
    {
        public List<RoterOutDesc> outportDescs;
    }
    /// <summary> 信号设备信息 </summary>
    public class GetAllSignalDeviceMap_param : BaseCount_param
    {
        public List<SignalDeviceMap> arrSignalDeviceMap;
    }
    /// <summary> 信号源信息 </summary>
    public class GetAllSignalSrcs_param : BaseCount_param
    {
        public List<SignalSrcInfo> signalInfo;
    }
    /// <summary> 采集通道信息 </summary>
    public class GetAllCaptureChannels_param : BaseCount_param
    {
        public List<CaptureChannelInfo> captureChannelInfo;
    }
    /// <summary> 采集设备信息 </summary>
    public class GetAllCaptureDevices_param : BaseCount_param
    {
        public List<CaptureDeviceInfo> arCaptureDeviceList;
    }

    public class GetSignalDeviceMapBySignalID_param : Base_param
    {
        public int nDeviceID;
        public int nDeviceOutPortIdx;
        public emSignalSource SignalSource;
    }

    /// <summary> 基础Count模型 </summary>
    public class BaseCount_param : Base_param
    {
        /// <summary>数据量</summary>
        public int nVaildDataCount = 0;
    }

    /// <summary> 基础模型 </summary>
    public class Base_param
    {
        /// <summary>返回消息</summary>
        public string errStr = "OK";
        /// <summary>是否正确返回</summary>
        public bool bRet = true;
    }
    #endregion
}
