using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IngestDBCore.MSVSDK
{


        //public const int MAX_PATH = 260;
        public enum MSV_RET
        {
            MSV_FAILED = 0,   //执行失败
            MSV_SUCCESS = 1, //执行成功
            MSV_XMLERROR = -1, //XML解析错误
            MSV_NETERROR = -2  // 网络连接错误
        }
        /// <summary>
        /// MSV启动模式
        /// </summary>
        public enum MSV_MODE
        {
            CM_LOCAL = 0,           //本地
            CM_REMOTE = 1,          //远程
            CM_SERIALPORT = 2       //串口
        }

        /// <summary>
        /// MSV控制模式
        /// </summary>

        public enum WORK_MODE
        {
            WM_AUTO = 0,            //编单
            WM_MANUAL = 1           //手动
        }

        /// <summary>
        /// 任务队列运行状态
        /// </summary>
        public enum BATCH_STATE
        {
            BS_STOP = 0,            //任务队列停止
            BS_RUNNING = 1          //任务队列运行
        }
        /// <summary>
        /// 采集状态
        /// </summary>
        public enum CAPTURE_STATE
        {
            CS_STANDBY = 0,         //待机
            CS_CAPTURE = 1,         //运行
            CS_PAUSE = 2,           //暂停
            CS_CAPTURE_ACTIVE = 3           //任务激活
        }
        //**********************************************************************
        /// <summary>
        /// 
        /// </summary>
        public struct MSV_STATE
        {
            public ulong dwQueryCounter;   //查询计数器
            public int bRetroSpect;        //是否为回溯状态//0：无回溯 1：回溯任务  2：已经开始回溯
            public MSV_MODE msv_mode;          //MSV启动模式
            public WORK_MODE msv_work_mode;     //MSV工作模式
            public CAPTURE_STATE msv_capture_state; //MSV运行状态
            public string msv_client_ip;     //如果媒体服务器处于远程控制模式，将返回控方客户端IP
        }

        public class MSV_LocalStorage
        {
            public int iDiskCount;
            public ulong[] dwTotalSpace = new ulong[6];
            public ulong[] dwFreeSpace = new ulong[6];
            public string[] strDisk = new string[6];
        }

        /// <summary>
        /// 任务状态
        /// </summary>
        public enum TASK_STATE
        {
            TS_READY = 0,
            TS_ACTIVE,
            TS_RUNNING,
            TS_PAUSE,
            TS_STOP,
            TS_FINISH
        }

        /// <summary>
        /// 任务启动模式
        /// </summary>
        public enum TASK_MODE
        {
            TM_AUTO = 0,     //自动任务
            TM_MANUAL = 1,    //手动任务
            TM_RETROSPECT = 2
        }

        /// <summary>
        /// 二级编码类型
        /// </summary>
//         public const string MG_MPEG4_MSV2 = "24pm";
//         public const string MG_MPEG4_MSV3 = "v4pm";
// 
//         public const int MG_MPEG2TYPE_UNKNOWN = 0;
//         public const int MG_MPEG2_SONY_IMX = 1;			
//         public const int MG_MPEG2_MATROX_I = 2;			
//         public const int MG_MPEG2_MATROX_IBP = 3;		
// 
//         public const int MG_DVCAM_DVSD = 0 ; 
//         public const int MG_DVCPRO_DV25 = 1 ;
//         public const int MG_DVCPRO50_DV50 =  2;
// 
//         public const int MG_WMV_9 = 0;
//         public const int MG_WMV_8 = 1;

        /// <summary>
        /// 文件格式
        /// </summary>
//         public const string MG_VFILE_MPEG2 = "SEMM";   //     MPEG2 I or IBP: bih.biCompression = 'SEMM'
//         public const string MG_MPEG4_FILE_MSV2 = "24pm";
//         public const string MG_MPEG4_FILE_MSV3 = "v4pm";
//         public const string MG_FILE_DVCAM_DVSD = "dsvd";
//         public const string MG_FILE_DVCPRO_DV25 = "52vd";
//         public const string MG_FILE_DVCPRO50_DV50 = "05vd";
//         public const string MG_FILE_MJEP = "GPJM";

        /// <summary>
        /// 编码类型
        /// </summary>
        public enum MG_EncodeType
        {
            MG_ENCTYPE_UNKNOWN = 0,
            MG_ENCTYPE_MPEG2 = 1,
            MG_ENCTYPE_MPEG4 = 2,
            MG_ENCTYPE_MJPG = 3,
            MG_ENCTYPE_DV = 4,
            MG_ENCTYPE_ASF = 5,
            MG_ENCTYPE_REAL = 7,
            MG_ENCTYPE_WMV = 21,
            MG_ENCTYPE_DNxHD = 55,
            MG_ENCTYPE_AVCIntra = 56,
            MG_ENCTYPE_H264 = 57,
        }

        /// <summary>
        /// 写视频文件类型 
        /// </summary>
        public enum MG_AVWriteType
        {
            MG_WRITETYPE_UNKNOWN = 0,
            MG_WRITETYPE_RIFF = 1,
            MG_WRITETYPE_MATROX = 2,
            MG_WRITETYPE_MAV = 3,
            MG_WRITETYPE_GXF = 4,
            MG_WRITETYPE_MXF = 5,
            MG_WRITETYPE_PS = 6,
            MG_WRITETYPE_WMV = 7,
            MG_WRITETYPE_AVI2 = 8,
            MG_WRITETYPE_MP4 = 70,
            MG_WRITETYPE_TS = 189,
            MG_WRITETYPE_ISMV = 191,
            MG_WRITETYPE_DONOTWRITEFILE = 0XFF
        }
        /// <summary>
        /// 写音频文件类型
        /// </summary>
        public enum MG_AudioWriteType
        {
            MG_AUDIOWRITETYPE_UNKNOWN = 0,
            MG_AUDIOWRITETYPE_WAV = 21,
            MG_AUDIOWRITETYPE_MP2 = 22,
            MG_AUDIOWRITETYPE_MP3 = 23,
            MG_AUDIOWRITETYPE_MXF = 24,
            MG_AUDIOWRITETYPE_AAC = 25,
            MG_AUDIOWRITETYPE_AC3 = 26,
            MG_AUDIOWRITETYPE_DBE = 27,
            MG_AUDIOWRITETYPE_HEAAC = 28
        }

        //ts signal 
        public struct TS_Signal_Info
        {
            public int nAudioType;
            public int nAudioCount;
            public ulong dwBitsPerSample;//
            public ulong  dwSamplePerSecond;//

            public int nVideoType;
            public int nVideoWidth;
            public int nVideoHeight;
            public double fps;
            public ulong  dwBitCount;
            public int nFrameType;
            public ulong dwVideoDataRate;

//             void Serialize(CArchive & ar)
//             {
//                 if (ar.IsStoring())
//                 {
//                     ar << nAudioType << nAudioCount << dwBitsPerSample << dwSamplePerSecond << nVideoType << nVideoWidth << nVideoHeight << fps << dwBitCount << nFrameType << dwVideoDataRate;
//                 }
//                 else
//                 {
//                     ar >> nAudioType >> nAudioCount >> dwBitsPerSample >> dwSamplePerSecond >> nVideoType >> nVideoWidth >> nVideoHeight >> fps >> dwBitCount >> nFrameType >> dwVideoDataRate;
//                 }
//             }
        }
        public class TS_PgmInfo
        {
            public int dwPgmID;  //<0: 没有节目信息
            public TS_Signal_Info tsSingalInfo;
            //char[] strPgmName = new char[255];
            public string strPgmName;
            public TS_PgmInfo()
            {
                dwPgmID = -1;
                strPgmName = "";
                //Array.Clear(strPgmName, 0, strPgmName.Length);
            }

        }
        public class TS_DataChannelInfo
        {
            public int dwDataChannel_ID;
            //public char[] strDataChannel_Name = new char[255];
            public string strDataChannel_Name;
            public int nPgmCount;          //此信号源的 节目个数,表示 pTS_PgmInfo的个数
            public TS_PgmInfo pTS_PgmInfo;
            public TS_DataChannelInfo()
            {
                dwDataChannel_ID = -1;
                pTS_PgmInfo = null;
                nPgmCount = 0;
                strDataChannel_Name = "";
                //Array.Clear(strDataChannel_Name, 0, strDataChannel_Name.Length);
            }
            ~TS_DataChannelInfo()
            {
                nPgmCount = 0;
            }
        }

        public class TS_DataChannelInfoEx
        {
            public int dwDataChannel_ID;
            public string strDataChannel_Name;
            public int nPgmCount;          //此信号源的 节目个数,表示 pTS_PgmInfo的个数
            public Dictionary<int, TS_PgmInfo> pTS_PgmInfo;
            public TS_DataChannelInfoEx()
            {
                dwDataChannel_ID = -1;
                pTS_PgmInfo = null;
                nPgmCount = 0;
                strDataChannel_Name = "";
            }
            ~TS_DataChannelInfoEx()
            {
                nPgmCount = 0;
            }
        }
        //msv采集通道的信息
        public class MSV_ChannelInfo
        {
            public int nChannelID;
            public int nChannelType;//通道类型 0:sdi 1:ts
            public ulong dwCtrlPort;//控制端口
            public string strChannel_Name;
            public string strChannel_IP;
            public MSV_ChannelInfo()
            {
                nChannelID = -1;
                dwCtrlPort = 0;
                nChannelType = 0;
                strChannel_Name = "";
                strChannel_IP = "";
            }
        }
        //TS 流专门的采集参数
        public class TSParam
        {
            public int nSingleType;//标志是否采集的信号类型 0:sdi  1:TS 2:流媒体
            public string strDataChannel_Name; //nSingleType:True 分析通道名字 2: 流媒体url
            public string strPgmName;//nSingleType 1: 节目名字  2: 流媒体类型
            public ulong dwDataChannel_ID;//nSingleType 1: 有效
            public ulong dwPgmID;                //nSingleType 1: 有效
            public int bNeedDecode; //标示是否需要转码操作. 0:不转码 1:转码高质量 2:高质量原码+低质量
            public TS_Signal_Info tsSignalInfo;    //nSingleType 1: 有效
            public ulong[] dwRev = new ulong[8];
        }


        //采集控制参数,接口参数
        public class CAPRTUR_PARAM
        {
            public bool bPath0;  //线路0 是否写文件 （视频）
            public bool bAudio0; //线路0 是否产生音频
            public bool bAlone0; //线路0 是否产生独立音频 。 TRUE : 产生独立音频文件 ， FALSE : 音视频文件共用一个文件
            public string path0FileName; // 路径+名字
            public MG_EncodeType nEncodeType0; //编码线路 0 编码类型
            public int subEncodeType0;  //编码 操作 类型
            public int bit_rate0;      // For MPEG-1 and MPEG-2:
                                       // Bit rate in bits/sec.  130000 < bit_rate <100000000
            public bool bDetectKeyFrame;//是否启动转场帧识别

            public MG_AVWriteType AVWriteTypeV0; //V0写文件对象
            public ulong dwSamplesOutA0;//输出采样率
            public MG_AudioWriteType AudioWriteTypeA0; // A0写文件对象
            public ulong dwMp3RateA0; //输出的MP3的码率
            public int iBitDepth0;


            public bool bPath1;  //线路1 是否写文件 （视频）
            public bool bAudio1; //线路1 是否产生音频
            public bool bAlone1; //线路1 是否产生独立音频 。 TRUE : 产生独立音频文件 ， FALSE : 音视频文件共用一个文件
            public string path1FileName; // 路径+名字
            public MG_EncodeType nEncodeType1; //编码线路 1 编码类型
            public int subEncodeType1;  //编码 操作 类型
            public int bit_rate1;     // For MPEG-1 and MPEG-2:
                                      // Bit rate in bits/sec.  130000 < bit_rate <100000000
            public MG_AVWriteType AVWriteTypeV1; //V1写文件对象
            public ulong dwSamplesOutA1;//输出采样率
            public MG_AudioWriteType AudioWriteTypeA1; // A1写文件对象
            public ulong dwMp3RateA1; //输出的MP3的码率
            public int iBitDepth1;

            public int nGOPPCount; //1个GOP里面的P帧数量
            public int nGOPBCount; //1个GOP里面的B帧数量
            public bool bUseTransfer;

            public ulong dwAudioChannelAttribute;

            public ulong dwFirPicNum;  //刷新首帧缩略图的帧号
            public ulong dwVideo_width;
            public ulong dwVideo_height;
            public ushort wCHROMA_FORMAT;    //_chroma_420 = 0,_chroma_422 = 1,_chroma_444 = 2,

            public int AudioEncodeTypeA0;//A1编码格式
            public int AudioEncodeTypeA1;//A1编码格式

            public ulong startTimeInfo;  //开始写文件TimeInfo			
            public ulong startFileIndex; //开始写文件序号（mfs中适用）
            public string strStream_URL = ""; //流媒体url
            public string strStream_Type = "";//流媒体类型
            public string str_ASR_Dic_Name = "";//语音识别字典文件
            public ulong dwASR_mask;//语音识别通道掩码
            public string strCloseCaptionFilePath = "";//存储CloseCaption文件的路径
            public TSParam tsParam = new TSParam();
            public CAPRTUR_PARAM()
            {
                startTimeInfo = 0;
                startFileIndex = 0;
                bPath0 = true;
                bAudio0 = true;
                bAlone0 = true;
                path0FileName = "";
                nEncodeType0 = MG_EncodeType.MG_ENCTYPE_UNKNOWN;
                subEncodeType0 = -1;  //编码 操作 类型
                bit_rate0 = -1;     // For MPEG-1 and MPEG-2:
                AVWriteTypeV0 = MG_AVWriteType.MG_WRITETYPE_UNKNOWN;
                dwSamplesOutA0 = 0;
                AudioWriteTypeA0 = MG_AudioWriteType.MG_AUDIOWRITETYPE_UNKNOWN;
                dwMp3RateA0 = 0;
                iBitDepth0 = 16;

                bDetectKeyFrame = true;
                bPath1 = true;
                bAudio1 = true;
                bAlone1 = true;
                path1FileName = "";
                nEncodeType1 = MG_EncodeType.MG_ENCTYPE_UNKNOWN; //编码线路 1 编码类型
                subEncodeType1 = -1;  //编码 操作 类型
                bit_rate1 = -1;     // For MPEG-1 and MPEG-2:
                AVWriteTypeV1 = MG_AVWriteType.MG_WRITETYPE_UNKNOWN;
                dwSamplesOutA1 = 0;
                AudioWriteTypeA1 = MG_AudioWriteType.MG_AUDIOWRITETYPE_UNKNOWN;
                dwMp3RateA1 = 0;
                iBitDepth1 = 16;

                nGOPPCount = -1;
                nGOPBCount = -1;
                bUseTransfer = false;

                dwAudioChannelAttribute = 3;
                dwFirPicNum = 0;
                dwVideo_width = 1920;
                dwVideo_height = 1080;
                wCHROMA_FORMAT = 1;

                //Array.Clear(strCloseCaptionFilePath, 0, strCloseCaptionFilePath.Length);
                //str_ASR_Dic_Name[0] = '0';
                dwASR_mask = 0;

                tsParam = null;
            }
        }
        //任务常规参数
        public class TASK_PARAM
        {
            public long ulID;     //任务ID
            public TASK_MODE TaskMode; //任务模式
            public TASK_STATE TaskState;  //任务状态
            public string strName;    //任务名称
            public string strDesc;    //任务描述
            public DateTime tmBeg;        //开始时间
            public DateTime tmEnd;        //结束时间
            public uint channel;    //节目号 ASI用
            public uint nSignalID;   //信号源ID

            public bool bUseTime;  //是否使用时间参数
            public int nTimeCode;  //采集模式  0:timecode 采集,1: frame采集
                                   //DWORD		dwStartFrame;//开始帧 如果为0 ，立即开始. （该帧是需要采集的）
            public ulong dwCutClipFrame; //如果不需要分段 默认为0xfffffffe	
                                         //DWORD		dwTaskTatolFrame;//任务总帧数 当任务采集的帧数到达此值 任务自动关闭，  如果不需要中层自动结束 设为0xfffffffe
            public bool bRetrospect; //是否回朔采集  

            public int nInOutCount;//任务中入点出点的对数。最多支持100对
            public ulong[] dwInFrame = new ulong[100];//开始帧 如果为0 ，立即开始. （该帧是需要采集的）
            public ulong[] dwOutFrame = new ulong[100];//任务总帧数 当任务采集的帧数到达此值 任务自动关闭，  如果不需要中层自动结束 设为0xfffffffe
            public ulong[] dwTotalFrame = new ulong[100];

            public TASK_PARAM()
            {
                ulID = -1;
                TaskMode = TASK_MODE.TM_MANUAL;
                TaskState = TASK_STATE.TS_READY;
                strName = "";
                strDesc = "";
                tmBeg = DateTime.Now;
                tmEnd = DateTime.Now;
                channel = 0;

                bUseTime = true;    //是否使用时间参数
                                    //dwStartFrame = 0;//开始帧 如果为0 ，立即开始. （该帧是需要采集的）
                dwCutClipFrame = 1500; //如果不需要分段 默认为0xfffffffe	
                                       //dwTaskTatolFrame = 0xfffffffe;//任务总帧数 当任务采集的帧数到达此值 任务自动关闭，  如果不需要中层自动结束 设为0xfffffffe
                bRetrospect = false; //是否回朔采集

                nInOutCount = 0;
                dwInFrame[0] = 0;
                dwOutFrame[0] = 0xfffffffe;
                dwTotalFrame[0] = 0;
                nTimeCode = 0;
            }
        }

        public class TASK_ALL_PARAM
        {
            public TASK_PARAM taskParam;
            public CAPRTUR_PARAM captureParam;
            public int nCutLen;      //分段长度
            public int lCatalogID;   //入库分类ID 	
            public TASK_ALL_PARAM()
            {
                nCutLen = 0;
                lCatalogID = 0;
                taskParam = new TASK_PARAM();
                captureParam = new CAPRTUR_PARAM();
            }
        }
        public class TASK_ALL_PARAM_NEW
        {
            public TASK_PARAM taskParam = new TASK_PARAM();
            public string captureParam;
            public int nCutLen;      //分段长度
            public int lCatalogID;   //入库分类ID   
            public TASK_ALL_PARAM_NEW()
            {
                nCutLen = 0;
                lCatalogID = 0;
                taskParam = new TASK_PARAM();
                captureParam = "";
            }
    }

        public struct BACKUP_INFO
        {
            public string strVideoPathName0;
            public string strAudioPathName0;
            public string strVideoPathName1;
            public string strAudioPathName1;
        }



        //////////////////////////////////////////////////////////////////////////
        /*                     采集参数相关                          */
        //////////////////////////////////////////////////////////////////////////
        public struct EncodeType_st
        {
            public int nParam;      //类型值
            public string strDesc; //类型描述
        }
        public struct File_st
        {
            public int nParam;
            public string strDesc;
            public int MaxCodeRate;
            public int MinCodeRate;
        }
        public struct check_st
        {
            public int nEncodeParam;
            public int nSubEncodeParam;
            public int nFileParam;
        }
        public struct check_rest
        {
            public string strEncodeDesc;
            public string strSubEncodeDesc;
            public string strFileDesc;
        }

        public struct disk_info
        {
            public string diskName;    //磁盘符
            public float nTotolSize;//(GB) 总容量
            public float nFreeSize; //(GB) 剩余大小
        }

        public struct SignalSourceFormat
        {
            public int width;
            public int height;
            public int framerate;
        }


        public class AudioChannel
        {
            public const int MAX_AUDIO_COUNT = 8;
            public int AudioIdx;
            bool[] audioMember = new bool[MAX_AUDIO_COUNT];
            AudioChannel()
	        {
		        for(int i = 0; i<MAX_AUDIO_COUNT;i++)
		        {
			        audioMember[i]=false;
		        }
            }
        }

        public class MANUALKEYFRAME
        {
            public long dwTaskFrameNo;
            public long dwTimeCode;
            public long dwWidth;
            public long dwHeight;
            public long dwBitDepth;
            //LPBYTE pPicData;
            public string pPicData;

            public MANUALKEYFRAME()
            {
                dwTaskFrameNo = -1;
                dwTimeCode = -1;
                dwWidth = 0;
                dwHeight = 0;
                dwBitDepth = 24;
                pPicData = "";
            }
        }
        public struct UploadInfo
        {
            public string strTaskName;    //上载任务名称
            public int nTaskID;            //上载任务ID	
            public int nTrimIn;            //任务入点	
            public int nTrimOut;           //任务出点	
            public string captureParam;   //采集参数
        }

        public enum SignalFormat
        {
            _720_480_5994I = 0,
            _720_576_50I = 1,
            _1920_1080_5994I = 2,       //HD NTSC
            _1920_1080_2997PSF = 3,
            _1920_1080_50I = 4,     //HD PAL
            _1920_1080_25PSF = 5,
            _1920_1080_24PSF = 6,
            _1920_1080_2398PSF = 7,
            _1920_1080_48I = 8,
            _1920_1080_2398P = 9,
            _1280_720_5994P = 10,
            _1280_720_50P = 11,
            _1280_720_24P = 12,
            _1280_720_2398P = 13,
            _1920_1035_5994I = 14,
            _1440_1080_5994I = 15,  //HD
            _1440_1080_50I = 16,    //HD 
            _1280_720_2997P = 20,
            _1280_720_25P = 21,
            _720_525_5994P = 22,
            _720_625_50P = 23,
            _1920_1080_2997P = 24,
            _1920_1080_25P = 25,
            _1920_1080_24P = 26,
            _1440_1080_24PSF = 31, // 0x1F
            _1440_1080_2398PSF = 32, // 0x20
            _1440_1080_2997PSF = 35, // 0x23
            _1440_1080_25PSF = 36, // 0x24
            _720_512_5994I = 64, // 0x40	//SD NTSC
            _720_608_50I = 65, // 0x41	//SD PAL
            _invalid_vid_format = 0xfe, // for internal lock mode
            _unknown_vid_format = 0xff, // for input lock mode
        }
        /**/
        public enum TimeCodeMode
        {
            DF = 0,
            NDF = 1,
            Unknow = 2
        }
        public struct SDISignalStatus
        {
            public SignalFormat VideoFormat;
            public TimeCodeMode TCMode;
            public int nWidth;
            public int nHeight;
            public float fFrameRate;
        }
}
