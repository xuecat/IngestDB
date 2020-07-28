namespace IngestDevicePlugin.Dto.Enum
{
    /// <summary>通道与信号源连接的状态</summary>
    public enum Channel2SignalSrc_State
    {
        /// <summary>没连接</summary>
        emNotConnection = 0,
        /// <summary>连接上</summary>
        emConnection = 1
    }
    /// <summary>程序类型</summary>
    public enum ProgrammeType
    {
        PT_Null = -1,
        PT_SDI,
        PT_IPTS,
        PT_StreamMedia
    }

    /// <summary>图像类型</summary>
    public enum ImageType
    {
        IT_Null = -1,
        IT_Original = 0,
        IT_SD_4_3 = 1,
        IT_SD_16_9 = 2,
        IT_HD_16_9 = 4
    }

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

    // <summary> 设备类型 </summary>
    //public enum emDeviceType
    //{
    //    /// <summary>MSV</summary>
    //    emDeviceMSV = 0,

    //    /// <summary>VTR</summary>
    //    emDeviceVTR = 1,

    //    /// <summary>蓝光</summary>
    //    emDeviceXDCAM = 2
    //}
    /// <summary> 设备类型设备类型-由上面类型进行修改</summary>
    public enum emDeviceType
    {
        /// <summary>MSV</summary>
        emDeviceMSV = 1,
        //将2预留出来 和通道统一
        /// <summary>RTMP</summary>
        emDeviceRTMP = 3,
        /// <summary>蓝光</summary>
        emDeviceXDCAM = 4,
        /// <summary>VTR</summary>
        emDeviceVTR = 5
    }

    /// <summary> 采集通道类型 </summary>
    public enum CaptureChannelType
    {
        /// <summary> 默认通道时msv通道，为啥有这个是因为前端乱存数据msv存0，所以为了兼容msv和默认是一样 </summary>
        emDefualtChannel = 0,
        /// <summary> MSV 采集通道 </summary>
        emMsvChannel = 1,

        /// <summary> IPTS 虚拟的通道 </summary>
        emIPTSChannel,

        /// <summary> 流媒体通道 </summary>
        emStreamChannel
        //emIPSChannel                //IPS2110 
    }

    /// <summary>通道状态</summary>
    public enum Channel_State
    {
        CS_Null = 0,
        CS_Idle,
        CS_Ready,
        CS_Capturing,
        CS_Error,
    }

    /// <summary>通道类型</summary>
    public enum Channel_Type
    {
        CT_SDI = 0,
        CT_TS,
        CT_Stream,
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

    /// <summary>设备状态</summary>
    public enum Device_State
    {
        /// <summary>没有连接</summary>
        DISCONNECTTED,

        /// <summary>已经连接</summary>
        CONNECTED,

        /// <summary>正在采集</summary>
        WORKING
    }

    /// <summary>MSV模式</summary>
    public enum MSV_Mode
    {
        /// <summary>本地</summary>
        LOCAL,

        /// <summary>网络</summary>
        NETWORK
    }

    /// <summary>上载模式</summary>
    public enum Upload_Mode
    {
        /// <summary></summary>
        NOUPLOAD,

        /// <summary></summary>
        CANUPLOAD,

        /// <summary>上载独占</summary>
        ONLYUPLOAD
    }
    /// <summary> 任务类型 </summary>
    public enum TaskType
    {
        /// <summary>普通任务</summary>
		TT_NORMAL = 0,
        /// <summary>周期任务</summary>
		TT_PERIODIC = 1,
        /// <summary>OpenEnd任务</summary>
		TT_OPENEND = 2,
        /// <summary>循环任务</summary>
		TT_LOOP = 3,
        /// <summary>占位任务</summary>
		TT_TIEUP = 4,
        /// <summary>手动任务</summary>
		TT_MANUTASK = 5,
        /// <summary>VTR上载任务</summary>
		TT_VTRUPLOAD = 6,
        /// <summary>扩展的OpenEnd任务</summary>
        TT_OPENENDEX = 7
    }
    /// <summary>任务附加类型</summary>
	public enum CooperantType
    {
        /// <summary>一般任务</summary>
		emPureTask = 0,
        /// <summary>同时收录到播出</summary>
		emKamataki = 1,
        /// <summary>vtr上载同时到播出</summary>
		emVTRKamataki = 2,
        /// <summary>VTR备份</summary>
		emVTRBackup = 3,
        /// <summary>Kamataki完成标识</summary>
		emKamatakiFinish = 4,
        /// <summary>VTR备份完成标识</summary>
		emVTRBackupFinish = 5,
        /// <summary>VTR备份失败标识</summary>
		emVTRBackupFailed = 6
    }
    /// <summary>任务状态</summary>
    public enum taskState
    {
        /// <summary>就绪</summary>
		tsReady = 0,
        /// <summary>正在执行任务</summary>
		tsExecuting,
        /// <summary>执行完成任务</summary>
		tsComplete,
        /// <summary>暂停标记</summary>
        tsPause,
        /// <summary>删除标记</summary>
        tsDelete,
        /// <summary>冲突任务</summary>
		tsConflict,
        /// <summary>无效任务</summary>
		tsInvaild,
        /// <summary>正在采集的手动任务</summary>
		tsManuexecuting,
        /// <summary>禁用的任务</summary>
		tsDisabled
    }
    /// <summary>任务优先级</summary>
    public enum TaskPriority
    {
        /// <summary>最低优先级</summary>
        TP_Lowest,
        /// <summary>较低优先级</summary>
        TP_BelowNormal,
        /// <summary>普通优先级</summary>
        TP_Normal,
        /// <summary>较高优先级</summary>
        TP_AboveNormal,
        /// <summary>最高优先级</summary>
        TP_Highest
    }
}
