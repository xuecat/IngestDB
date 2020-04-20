using System;
using System.Collections.Generic;
using IngestDevicePlugin.Dto.Enum;
using IngestDevicePlugin.Dto.Response;

namespace IngestDevicePlugin.Dto
{
    public class OldResponse
    {
    }
    
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

    /// <summary>信号源扩展信息</summary>
    public class SignalSrcExInfo
    {
        /// <summary>信号源ID</summary>
        public int nID;

        /// <summary>类型</summary>
        public int nSignalSrcType;

        /// <summary>是否是主信号源</summary>
        public bool bIsMainSignalSrc;

        /// <summary>主信号源ID</summary>
        public int nMainSignalSrcId;

        /// <summary>备信号源ID</summary>
        public int nBackupSignalSrcId;
    }

    /// <summary>MSV设备状态信息</summary>
    public class MSVChannelState
    {
        /// <summary>通道ID</summary>
        public int nChannelID = 0;

        /// <summary>设备状态</summary>
        public Device_State emDevState = 0;

        /// <summary>MSV模式</summary>
        public MSV_Mode emMSVMode = 0;

        /// <summary>vtrID</summary>
        public int vtrID = -1;

        /// <summary>当前用户Code</summary>
        public string curUserCode = string.Empty;

        /// <summary>kamataki信息</summary>
        public string kamatakiInfo = string.Empty;

        /// <summary>上载模式</summary>
        public Upload_Mode uploadMode = 0;

        /// <summary>通道索引</summary>
        public int nChannelIndex = 0;
    }

    /// <summary>信号源分组</summary>
    public class AllSignalGroup
    {
        public int groupid;//信号源对应的分组ID
        public string groupname;//信号源的名称
        public string groupdesc;//信号源的描述
    }

    /// <summary>信号源分组信息</summary>
    public class SignalGroupState
    {
        /// <summary>信号源ID</summary>
        public int signalsrcid;

        /// <summary>信号源对应的分组ID</summary>
        public int groupid;

        /// <summary>信号源的名称</summary>
        public string groupname;

        /// <summary>信号源的描述</summary>
        public string groupdesc;
    }

    /// <summary>GPI的映射信息</summary>
    public class GPIDeviceMapInfo
    {
        public int nGPIID = -1;//GPI编号
        public int nGPIOutputPort = -1;//GPI输出的端口
        public int nAVOutputPort = -1;//GPI对应的端口
        public int nCaptureParamID = -1;//GPI对应的采集参数ID
    }

    /// <summary>节目信息</summary>
    public class ProgrammeInfo
    {
        public int ProgrammeId { set; get; }
        public string ProgrammeName { set; get; }
        public string ProgrammeDesc { set; get; }
        public int TypeId { set; get; } //高标清
        public ProgrammeType emPgmType { set; get; }
        public ImageType emImageType { set; get; }
        public emSignalSource emSignalSourceType { set; get; }
        public int nPureAudio { set; get; }
        public int nCarrierID { set; get; }//运营商的ID
        public int nGroupID { set; get; } // Add by chenzhi 2013-07-08 分组ID
    }

    /// <summary>TS节目信息</summary>
    public class TSPgmInfo
    {
        /// <summary>节目Id</summary>
        public int PgmId { set; get; }

        /// <summary>节目名称</summary>
        public string PgmName { set; get; }

        /// <summary>节目描述</summary>
        public string PgmDesc { set; get; }

        /// <summary>数据通道Id</summary>
        public int DataChannelId { set; get; }

        /// <summary>节目索引</summary>
        public int PgmIndex { set; get; }

        /// <summary>高标清，0：标清；1：高清</summary>
        public int PgmTypeId { set; get; }

        /// <summary>图像类型</summary>
        public ImageType emImageType { set; get; }

        /// <summary>TS信号Info</summary>
        public string TSSingalInfo { set; get; }

        /// <summary>多播IP</summary>
        public string MulticastIP { set; get; }

        /// <summary>多播Port</summary>
        public int MulticastPort { set; get; }

        /// <summary>扩展信息</summary>
        public string ExtendParams { set; get; }

        /// <summary></summary>
        public int nPureAudio { set; get; }
    }
    /// <summary>设备信息</summary>
    public class TSDeviceInfo
    {
        public int DeviceId { set; get; }
        public string DeviceName { set; get; }
        public string DeviceDesc { set; get; }
        public string IPAddress { set; get; }
        public int Port { set; get; }
        public List<TSVirtualChannelInfo> ChannelInfos { set; get; }
        public List<TSDataChannelInfo> DataChannelInfos { set; get; }
    }
    /// <summary>TS虚拟通道信息</summary>
    public class TSVirtualChannelInfo
    {
        public int ChannelId { set; get; }
        public string ChannelName { set; get; }
        public string ChannelDesc { set; get; }
        public int DeviceId { set; get; }
        public int DeviceIndex { set; get; }
        public string ChannelIPAddress { set; get; }
        public int CtrlPort { set; get; }
        public Channel_State emChannelStatus { set; get; }
        public Channel_Type emChannelType { set; get; }
        public int nCarrierID { set; get; }//运营商的ID
        public emBackupFlag emBackUpType { set; get; }//通道的备份类型
        public int nCPSignalType { set; get; }//可以采集的信号源类型，0：Auto，1：SD，2：HD

        // Add by chenzhi 2013-07-4
        public int nGroupID { set; get; } // 通道ID
    }
    /// <summary>TS数据通道信息</summary>
    public class TSDataChannelInfo
    {
        public int DataChannelId { set; get; }
        public string DataChannelName { set; get; }
        public int DataChannelIndex { set; get; }
        public int DeviceId { set; get; }
        public List<TSPgmInfo> PgmInfos { set; get; }
    }
    /// <summary>任务信息</summary>
    public class TaskContent
    {
        public int nTaskID = 0;
        public string strTaskName = string.Empty;
        public string strTaskDesc = string.Empty;
        public string strClassify = string.Empty;
        public int nChannelID = 0;
        public int nUnit = 0;
        public string strUserCode = string.Empty;
        public int nSignalID = 0;
        public string strBegin = DateTime.MinValue.ToString("yyyy-MM-dd HH:mm:ss");
        public string strEnd = DateTime.MinValue.ToString("yyyy-MM-dd HH:mm:ss");
        public TaskType emTaskType = TaskType.TT_NORMAL;
        public CooperantType emCooperantType = CooperantType.emPureTask;
        public taskState emState;
        public string strStampImage = string.Empty;
        public string strTaskGUID = string.Empty;
        public int nBackupVTRID = 0;
        public TaskPriority emPriority = TaskPriority.TP_Normal;
        public int nStampTitleIndex = 0;
        public int nStampImageType = 0;
        public int nSGroupColor = 0;
    }
    /// <summary>GPI设备信息</summary>
    public class GPIDeviceInfo
    {
        /// <summary>GPI编号</summary>
        public int nGPIID = -1;
        /// <summary>GPI名字</summary>
        public string strGPIName = string.Empty;
        /// <summary>GPI位于哪个Com端口上</summary>
        public int nComPort = -1;
        /// <summary>GPI总的出口数</summary>
        public int nOutputPortCount = -1;
        /// <summary>GPI的描述</summary>
        public string strDescription = string.Empty;
    }
    //zmj2009-10-22
    //增加通道与信号源对应的结构
    public class Channel2SignalSrcMap
    {
        public int nChannelID = -1;                                                     //通道ID
        public int nSignalSrcID = -1;                                                   //信号源ID
        public Channel2SignalSrc_State state = Channel2SignalSrc_State.emNotConnection; //通道到信号源的连接状态
        public DateTime lastOperTime = DateTime.MinValue;                               //上一次连接时的时间
    }

    public class ChannelScore
    {
        public ChannelScore()
        {
            Id = 0;
            Score = 0;
        }

        public ChannelScore(int id)
        {
            Id = id;
            Score = 0;
        }

        public int Id = 0;  //通道ID
        public double Score = 0;    //分数
    }
    #endregion DescModel

    public class GetChannelsByProgrammeId_out : Base_param
    {
        public List<CaptureChannelInfo> channelInfos;
        public int validCount;
    }

    #region OOU-IN-Model

    /// <summary> 输入端口信息 </summary>
    public class GetAllRouterInPortInfo_param : BaseVaildCount_param
    {
        public List<RoterInportDesc> inportDescs;
    }

    /// <summary> 输出端口信息 </summary>
    public class GetAllRouterOutPortInfo_param : BaseVaildCount_param
    {
        public List<RoterOutDesc> outportDescs;
    }

    /// <summary> 信号设备信息 </summary>
    public class GetAllSignalDeviceMap_param : BaseVaildCount_param
    {
        public List<SignalDeviceMap> arrSignalDeviceMap;
    }

    /// <summary>TS设备信息</summary>
    public class GetAllTSDeviceInfos_OUT : Base_param
    {
        public List<TSDeviceInfo> deviceInfos;
        public int nValidCount;//服
    }

    /// <summary> 信号源信息 </summary>
    public class GetAllSignalSrcs_param : BaseVaildCount_param
    {
        public List<SignalSrcInfo> signalInfo;
    }

    /// <summary> 采集通道信息 </summary>
    public class GetAllCaptureChannels_param : BaseVaildCount_param
    {
        public List<CaptureChannelInfo> captureChannelInfo;
    }

    /// <summary> 采集通道信息 </summary>
    public class GetCaptureChannelByID_OUT : BaseVaildCount_param
    {
        public CaptureChannelInfo captureChannelInfo;
    }

    /// <summary> 采集设备信息 </summary>
    public class GetAllCaptureDevices_param : BaseVaildCount_param
    {
        public List<CaptureDeviceInfo> arCaptureDeviceList;
    }

    /// <summary> 信号源扩展信息 </summary>
    public class GetAllSignalSrcExs_param : BaseVaildCount_param
    {
        public List<SignalSrcExInfo> signalInfo;
    }

    /// <summary> 信号设备信息 </summary>
    public class GetSignalDeviceMapBySignalID_param : Base_param
    {
        public int nDeviceID;
        public int nDeviceOutPortIdx;
        public emSignalSource SignalSource;
    }

    /// <summary> 高清Or标清 </summary>
    public class GetParamTypeByChannleID_param : Base_param
    {
        public int nType = -1;
    }

    /// <summary> MSV通道状态信息 </summary>
    public class GetMSVChannelState_param : Base_param
    {
        public MSVChannelState channelStata;
    }

    /// <summary>获得所有信号源分组</summary>
    public class GetAllSignalGroup_OUT : BaseVaildCount_param
    {
        public List<AllSignalGroup> arAllSignalGroup;
    }

    /// <summary> 所有信号源分组</summary>
    public class GetAllSignalGroupState_OUT : BaseVaildCount_param
    {
        public List<SignalGroupState> arAllSignalGroupState;
    }

    /// <summary>GPI所有的映射</summary>
    public class GetGPIMapInfoByGPIID_OUT : BaseVaildCount_param
    {
        public List<GPIDeviceMapInfo> arGPIDeviceMapInfo;
    }

    /// <summary> 所有节目信息 </summary>
    public class GetAllProgrammeInfos_OUT : BaseValidCount_param
    {
        public List<ProgrammeInfo> programmeInfos;
    }

    /// <summary>根据通道获取相应的节目</summary>
    public class GetProgrammeInfosByChannelId_OUT : Base_param
    {
        public List<ProgrammeInfo> programmeInfos;
        public int validCount;//一个字段三种形式我也是服
    }

    /// <summary>所有MSV通道状态信息</summary>
    public class GetAllChannelState_OUT : BaseVaildCount_param
    {
        public List<MSVChannelState> arMSVChannelState;
    }
    public class GetBackupSignalSrcInfo_OUT : Base_param
    {
        public bool bIsHavingBackupSglSrc;
        public int nBackupSignalSrcId;
    }
    public class GetParamTypeBySignalID_OUT : Base_param
    {
        public int nType;
    }

    /// <summary> 保存通道扩展信息返回 </summary>
    public class UpdateChnExtData_OUT : Base_param { }

    /// <summary>根据信号源,用户名,自动匹配最优通道</summary>
    public class GetBestChannelIDBySignalID_out : Base_param
    {
        public int nChannelID;
    }
    /// <summary>为信号源选择一个合适的预监通道</summary>
    public class GetBestPreviewChannelForSignal_out : Base_param
    {
        public int nChnID;
    }
    /// <summary>更改ModifySourceVTRIDAndUserCode</summary>
    public class ModifySourceVTRIDAndUserCode_out : Base_param { }

    /// <summary>更新所有的IP收录的设备</summary>
    public class UpdateAllTSDeviceInfos_IN
    {
        public TSDeviceInfo[] deviceInfos;
    }

    public class ModifySourceVTR_in
    {
        public int[] nIDArray;
        public int nSourceVTRID;
        public string userCode;
    }

    /// <summary> 保存通道扩展信息 </summary>
    public class UpdateChnExtData_IN
    {
        public int nChnID;
        public int type;
        public string strData;
    }

    /// <summary> 基础ValidCount模型 </summary>
    public class BaseValidCount_param : Base_param
    {
        /// <summary>数据量</summary>
        public int nValidDataCount = 0;
    }

    /// <summary> 基础VaildCount模型（我想吐血） </summary>
    public class BaseVaildCount_param : Base_param
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

    #endregion ParamModel
}
namespace IngestDevicePlugin.Dto.Response
{
    public class ResponseMessage
    {
        public int nCode { get; set; }
        public string message { get; set; }
        public ResponseMessage()
        {
            nCode = 1;          //1代表成功，0代表失败
            message = "OK";
        }
    }
    public class ResponseMessage<T> : IngestDevicePlugin.Dto.Response.ResponseMessage
    {
        public ResponseMessage() : base() { }

        public T extention;
    }
}