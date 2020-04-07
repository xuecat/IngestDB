using System;
using System.Collections.Generic;
using System.Text;

namespace IngestDevicePlugin.Dto
{
    public class OldResponse
    {
    }
    public enum emSignalSource//信号来源
    {
        emSatlitlleSource = 0,//卫星
        emCtrlMatrixSource = 1,//总控矩阵
        emVideoServerSource = 2,//视频服务器 
        emVtrSource = 3,//VTR
        emMSVSource = 4,//MSV
        emXDCAM = 5,//蓝光
        emIPTS = 6,//IPTS流
        emStreamMedia = 7 //流媒体
    }
    public class RoterInportDesc
    {
        public int nRCDeviceID;
        public int nRCInportIdx;
        public int nSignalSrcID;
        public emSignalSource nSignalSource = emSignalSource.emSatlitlleSource;//信号来源， 0:卫星 1:总控矩阵 2 视频服务器 3: VTR 4: MSV  5 蓝光  其他以后再扩展
    }
    public class GetAllRouterInPortInfo_param
    {
        public List<RoterInportDesc> inportDescs;
        public int nVaildDataCount;
        public string errStr;
        public bool bRet;
    }
    public class CaptureChannelInfo
    {
        public int nID = 0;
        public string strName = "";
        public string strDesc = "";
        public int nCPDeviceID = 0;
        public int nChannelIndex = 0;
        public int nDeviceTypeID = (int)CaptureChannelType.emMsvChannel;//当前sdi，只会为1。2表示是IPTS通道
        public emBackupFlag BackState = emBackupFlag.emNoAllowBackUp;
        public int nCarrierID = 0;//运营商ID
        public int orderCode = -1;//序号
        public int nCPSignalType = 0;//可以采集的信号源类型，0：Auto，1：SD，2：HD

        // 分组ID
        public int nGroupID = -1;    // Add by chenzhi 2103-07-04
    }

    /// <summary>
    /// 任务备份属性
    /// </summary>
	public enum emBackupFlag
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

    //采集通道类型
    public enum CaptureChannelType
    {
        emMsvChannel = 1,		//MSV  采集通道
        emIPTSChannel,                //IPTS 虚拟的通道
        emStreamChannel               //流媒体通道  
    }
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

}
