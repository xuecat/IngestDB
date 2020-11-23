namespace IngestDevicePlugin.Dto.Enum
{
    /// <summary>通道与信号源连接的状态 没连接 = 0 连接上 = 1</summary>
    public enum Channel2SignalSrc_State
    {
        /// <summary>没连接</summary>
        emNotConnection = 0,
        /// <summary>连接上</summary>
        emConnection = 1
    }
    /// <summary>程序类型 空类型=-1 SID类型 = 0 IPTS类型= 2, 流媒体类型 = 3</summary>
    public enum ProgrammeType
    {
        PT_Null = -1,
        PT_SDI,
        PT_IPTS,
        PT_StreamMedia
    }

    /// <summary>图像类型 空类型 = -1 原类型 = 0 SD4:3 = 1 SD16:9 = 2 HD16:9=4</summary>
    public enum ImageType
    {
        IT_Null = -1,
        IT_Original = 0,
        IT_SD_4_3 = 1,
        IT_SD_16_9 = 2,
        IT_HD_16_9 = 4
    }

    /// <summary> 信号来源 卫星 = 0 总控矩阵=1 视频服务器=2 VTR = 3 MSV = 4 蓝光 = 5 IPTS流 = 6 流媒体 = 7</summary>
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
    /// <summary> 设备类型设备类型-由上面类型进行修改 MSV = 1 RTMP = 3 蓝光 = 4 VTR = 5</summary>
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

    /// <summary> 采集通道类型 默认通道时msv通道(为啥有这个是因为前端乱存数据msv存0，所以为了兼容msv和默认是一样) = 0 MSV 采集通道 = 1 IPTS 虚拟的通道 = 2 流媒体通道 = 3</summary>
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

    /// <summary>通道状态 空 = 0 空闲 = 1 准备 = 2 采集 = 3 错误 = 4</summary>
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

    /// <summary> 任务备份属性 不允许备份 = 0 允许备份 = 1 只允许作备份 = 2 </summary>
    public enum emBackupFlag
    {
        /// <summary> 不允许备份 </summary>
		emNoAllowBackUp = 0,

        /// <summary> 允许备份 </summary>
		emAllowBackUp = 1,

        /// <summary> 只允许作备份 </summary>
		emBackupOnly = 2
    }

    /// <summary>设备状态 没有连接 = 0 已经连接 = 1 正在采集 = 2</summary>
    public enum Device_State
    {
        /// <summary>没有连接</summary>
        DISCONNECTTED,

        /// <summary>已经连接</summary>
        CONNECTED,

        /// <summary>正在采集</summary>
        WORKING
    }

    /// <summary>MSV模式 本地 = 0 网络 = 1</summary>
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
    /// <summary>
    /// 任务类型 普通任务 = 0 周期任务 = 1 OpenEnd任务 = 2 循环任务 = 3 占位任务 = 4 手动任务 = 5 VTR上载任务 = 6 扩展的OpenEnd任务 = 7
    /// </summary>
    public enum TaskType
    {
        /// <summary>
        /// 普通任务 = 0
        /// </summary>
		TT_NORMAL = 0,
        /// <summary>
        /// 周期任务 = 1
        /// </summary>
		TT_PERIODIC = 1,
        /// <summary>
        /// OpenEnd任务 = 2
        /// </summary>
		TT_OPENEND = 2,
        /// <summary>
        /// 循环任务 = 3
        /// </summary>
		TT_LOOP = 3,
        /// <summary>
        /// 占位任务 = 4
        /// </summary>
		TT_TIEUP = 4,
        /// <summary>
        /// 手动任务 = 5
        /// </summary>
		TT_MANUTASK = 5,
        /// <summary>
        /// VTR上载任务 = 6
        /// </summary>
		TT_VTRUPLOAD = 6,
        /// <summary>
        /// 扩展的OpenEnd任务 = 7
        /// </summary>
        TT_OPENENDEX = 7
    }
    /// <summary>
    /// 任务附加类型 一般任务 = 0 同时收录到播出 = 1 vtr上载同时到播出 = 2 VTR备份 = 3 Kamataki完成标识 = 4 VTR备份完成标识 = 5 VTR备份失败标识 = 6
    /// </summary>
    public enum CooperantType
    {
        /// <summary>
        /// 一般任务 = 0
        /// </summary>
		emPureTask = 0,
        /// <summary>
        /// 同时收录到播出 = 1
        /// </summary>
		emKamataki = 1,
        /// <summary>
        /// vtr上载同时到播出 = 2
        /// </summary>
		emVTRKamataki = 2,
        /// <summary>
        /// VTR备份 = 3
        /// </summary>
		emVTRBackup = 3,
        /// <summary>
        /// Kamataki完成标识 = 4
        /// </summary>
		emKamatakiFinish = 4,
        /// <summary>
        /// VTR备份完成标识 = 5
        /// </summary>
		emVTRBackupFinish = 5,
        /// <summary>
        /// VTR备份失败标识 = 6
        /// </summary>
		emVTRBackupFailed = 6
    }
    /// <summary>
    /// 任务状态 就绪 = 0 正在执行任务 = 1 暂停标记 = 3 删除标记 = 4  冲突任务 = 5 无效任务 = 6 正在采集的手动任务 = 7 禁用的任务 = 8
    /// </summary>
    public enum taskState
    {
        /// 就绪 = 0
		tsReady = 0,
        /// 正在执行任务 = 1
		tsExecuting,
        /// 执行完成任务 = 2
		tsComplete,
        /// 暂停标记 = 3
        tsPause,
        /// 删除标记 = 4      
        tsDelete,
        /// <summary>
        /// 冲突任务 = 5
        /// </summary>
		tsConflict,
        /// <summary>
        /// 无效任务 = 6
        /// </summary>
		tsInvaild,
        /// <summary>
        /// 正在采集的手动任务 = 7
        /// </summary>
		tsManuexecuting,
        /// <summary>
        /// 禁用的任务 = 8
        /// </summary>
		tsDisabled
    }
    /// <summary>任务优先级 最低优先级= 0 较低优先级= 1 普通优先级= 2 较高优先级 = 3 最高优先级= 4</summary>
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
