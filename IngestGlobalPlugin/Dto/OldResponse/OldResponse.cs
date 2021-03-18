using IngestGlobalPlugin.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace IngestGlobalPlugin.Dto.OldResponse
{
    class OldResponse
    {
    }

    public enum TC_TYPE
    {
        emPresetTC = 0,
        emOriginalTC,
        emHouseTC
    }

    public enum TC_MODE
    {
        emForLine,
        emForOther
    }

    public enum OTID//ObjectTypeID
    {
        OTID_ALL = 0,//所有的zmj2008-9-3
        OTID_VTR = 1,//VTR
        OTID_CHANNEL,//通道
        OTID_OTHER,//其他
    }

    public class MetaDataPolicy
    {
        public int nID = 0;
        public string strName = "";
        public string strDesc = "";
        public int nDefaultPolicy = 0;
        public string strArchiveType = "";
    }

    public class PostLockObject_param_in
    {
        /// <summary>对象Id</summary>
        /// <example>3538</example>
        public int ObjectID;
        /// <summary>对象类型Id</summary>
        /// <example>3</example>
        public OTID ObjectTypeID;
        /// <summary>用户名</summary>
        /// <example>RecMQServer</example>
        public string userName;
        /// <summary>延时</summary>
        /// <example>30000</example>
        public int TimeOut;
    }

    /// <summary>
    /// 很多重复的类字段一样，进行整合:PostLockObject_param_out,SetUserSetting_OUT
    /// </summary>
    public class PostParam_Out
    {
        public string errStr;
        public bool bRet;
    }

    public class SetUserSetting_IN
    {
        /// <summary>用户Code</summary>
        /// <example>06c70a52172d4393beb1bb6743ca6944</example>
        public string strUserCode;
        /// <summary>settingType</summary>
        /// <example>UserMoudleData</example>
        public string strSettingtype;
        /// <summary>内容</summary>
        /// <example>Content...</example>
        public string strSettingText;
    }

    public class GetGlobalState_OUT
    {
        public List<GlobalState> arrGlobalState;
        public string strErr;
        public bool bRet;
    }
    
    public class OldResponseMessage
    {
        public int nCode { get; set; }
        public string message { get; set; }
        public OldResponseMessage()
        {
            nCode = 1;          //1代表成功，0代表失败
            message = "OK";
        }
    }

    public class OldResponseMessage<T> : OldResponseMessage
    {
        public T extention;
    }

    public class GetDefaultSTC_param
    {
        public TC_TYPE tcType;
        public int nTC;
        public string errStr;
        public bool bRet;
    }

    public class DeleteMqMsg_OUT
    {
        public string errStr;
        public bool bRet;
    }

    public class AddMaterialNew_OUT
    {
        public int nMaterialID;
        public string errStr;
        public bool bRet;
    }

    public class GlobalState
    {
        public string strLabel = string.Empty;
        public string dtLastTime = DateTime.MinValue.ToString("yyyy-MM-dd HH:mm:ss");
    }

    public class ObjectContent
    {
        public int ObjectID = -1;//对象ID
        public OTID ObjectType = OTID.OTID_ALL;//对象类型
        public string UserName = string.Empty;//用户名
        public DateTime BeginTime = DateTime.MinValue;//开始时间
        public int TimeOut = 0;//超时时间
    }

    public class GetParamTemplateByID_out
    {
        public string strCaptureParam;
        public string errStr;
        public bool bRet;
    }

    public class UserTemplate
    {
        /// <summary>模板ID</summary>
        /// <example>-1</example>
        public int nTemplateID = -1;                    //模板ID
        /// <summary>用户名</summary>
        /// <example>8de083d45c614628b99516740d628e91</example>
        public string strUserCode = string.Empty;       //用户名
        /// <summary>模板名</summary>
        /// <example>tName</example>
        public string strTemplateName = string.Empty;   //模板名
        /// <summary>模板内容</summary>
        /// <example><window_positions>...</window_positions></example>
        public string strTemplateContent = string.Empty;//模板内容   
    }

    public class ResponseMessageN
    {
        public string Code { get; set; }
        public string msg { get; set; }
        public ResponseMessageN()
        {
            Code = "0";
        }
    }
    public class ResponseMessageN<TEx> : ResponseMessageN
    {
        public TEx ext { get; set; }
    }


    public class etparam
    {
        public string paramname { get; set; }
        public string paramvalue { get; set; }
        public string paramvaluedef { get; set; }
        public string paramdescription { get; set; }
    }
    
    public class parameter : etparam
    {
        public string system { get; set; }
        public string tool { get; set; }
    }

    public class userparameter : parameter
    {
        public string loginname { get; set; }
    }

    public class OldCapParamTemplate
    {
        public int nID = 0;     //采集参数模板ID
        public string strTemplateName = "";//采集参数模板
        public string strParamTemplate = string.Empty;//参数模板内容采集
    }

    public class UserParmMap
    {
        public int nCapatureParamID = 0;
        public string szClassCode = "";
    }
    /// <summary>
    /// 消息的处理状态
    /// </summary>
    public enum MqmsgStatus
    {
        NotProcess,
        Processing,
        Processed,
        ProcessFailed
    }

    /// <summary>
    /// 消息的来源
    /// </summary>
    public enum MsgSourceType
    {
        MsgForSDICapture,
        MsgForFileImport
    }
    
    /// <summary>
    /// 消息处理方式：查询数据库或者直接创建默认
    /// </summary>
    public enum KafkaMode
    {
        CreateDefaultKafka = 1,
        GetKafkaFromDb
    }

    /// <summary>
    /// 消息结构
    /// </summary>
    public class MQmsgInfo
    {
        public MsgSourceType type { set; get; }    //消息的类型，是SDI的还是文件引入的
        public string MsgId { set; get; }//消息ID,唯一
        public string MsgContent { set; get; }//消息内容
        public string MsgSendTime { set; get; }//消息发送的时间
        public string MsgRevTime { set; get; }//消息到达的时间
        public MqmsgStatus MsgStatus { set; get; }//当前消息的状态
        public string MsgProcessTime { set; get; }//消息最后一次被处理的时间
        public int FailedCount { set; get; }//失败的次数
        public string sNextRetry { set; get; }//下次重试时间
        public string strLock;//加锁
        public int nActionID = -1;//对于没有处理过的消息，这个ID是0，失败重试的消息，这个ID是策略ID
        public int TaskId = 0;
    }
    /// <summary>
    /// 素材状态 开始采集了,可以边采边编了	= 0 采集完成 = 1
    /// </summary>
    public enum CLIP_STATE
    {
        STARTCAPUTER = 0,   //开始采集了,可以边采边编了	
        STOPCAPTURE,    //采集完成
    }

    public enum DELETED_STATE
    {
        NOTDELETED = 0,                 //没有被删除  
        DELETEDBYARCHIVESERVICE,        //被入库服务删除
        DELETEDBYMETADATASTATUSMANAGER, //被入库失败素材查询工具删除
        DELETEDBYOTHER,                 //被其他工具删除的
    }
    public class VideoInfo
    {
        public string strFilename = "";
        public int nVideoTypeID = -1;
        public int nVideoSource = 0;//视频来源
    }

    public class AudioInfo
    {
        public string strFilename = "";
        public int nAudioTypeID = -1;
        public int nAudioSource = 0;//音频来源
    }

    public class MaterialInfo
    {
        public int nID = 0;         //素材ID
        public string strName = "";     //素材名
        public string strRemark = "";   //素材描述
        public string strUserCode = ""; //用户编码
        public string strCreateTime = DateTime.MinValue.ToString("yyyy-MM-dd HH:mm:ss");    //创建时间
        public int nTaskID = 0;     //任务ID
        public int nSectionID = 0;  //分段索引
        public string strGUID = ""; //素材GUID
        public int nClipState = (int)CLIP_STATE.STARTCAPUTER;   //素材采集状态
        public List<VideoInfo> videos;  //视频文件列表
        public List<AudioInfo> audios;  //音频文件列表
        public List<int> ArchivePolicys;    //入库策略列表
        public int nDeleteState = (int)DELETED_STATE.NOTDELETED;
    };

    public class FileFormatInfo_out
    {
        public int nCode;
        public string errStr;
        public string key;
        public long nformatid;
        public long videostrandid;
        public string videostrandguid;
        public string extrainfo;
    }

    public class FileFormatInfo_in
    {
        public string key;
        public long nformatid;
        public long videostrandid;
        public string videostrandguid;
        public string extrainfo;
    }
    public class FailedMessageParam
    {
        public int TaskID { get; set; }
        public int SectionID { get; set; }
        public string MsgContent { get; set; }
    }

    public class MsgFailedRecord
    {
        public string MsgGuid { get; set; }
        public uint TaskID { get; set; }
        public uint SectionID { get; set; }
        public string DealTime { get; set; }
        public string DealMsg { get; set; }
    }

    //yangchuang20130220这个状态实际上表示的是入库过程中的状态，并不是表示入库结果,重新命名
    //0.1.2.3,分别是第一次入库准备,结束,第二次入库准备,结束
    /// <summary>
    /// 入库状态 第一次入库准备 = 0 第一次入库结束 = 1 第二次入库准备 = 2 第二次入库结束 = 3 手动入库模式 = 4
    /// </summary>
    public enum SAVE_IN_DB_STATE
    {
        FIRST_READY = 0,            //第一次入库准备
        FIRST_END,              //第一次入库结束
        SECOND_READY,           //第二次入库准备
        SECOND_END,             //第二次入库结束
        MANUALMODE,             //手动入库模式
    }
    public class UpdateSaveInDBStateForTask_IN
    {
        public int nTaskID;
        public int nPolicyID;
        public int nSectionID;
        public int state;
        public string strResult;
    }
    public class UpdateSaveInDBStateForTask_OUT
    {
        public string errStr;
        public bool bRet;
    }

}
