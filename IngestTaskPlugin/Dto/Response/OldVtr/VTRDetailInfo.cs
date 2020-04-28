namespace IngestTaskPlugin.Dto.Response.OldVtr
{
    /// <summary>
    /// 录像机详细信息 <see cref="VTRDetailInfo" />.
    /// </summary>
    public class VTRDetailInfo      
    {
        /// <summary>
        /// 录像机ID
        /// </summary>
        public int nVTRID = 0;

        /// <summary>
        /// VTR类型ID
        /// </summary>
        public int lVTRTypeID = 0;

        /// <summary>
        /// VTR子类型ID
        /// </summary>
        public int lVTRSubTypeID = 0;

        /// <summary>
        /// 录像机名称
        /// </summary>
        public string szVTRDetailName = "";

        /// <summary>
        /// VTR描述
        /// </summary>
        public string szVTRDetailDesc = "";

        /// <summary>
        /// VTR控制串口号
        /// </summary>
        public int nVTRVComPortIdx = -1;

        /// <summary>
        /// 循环录制标志
        /// </summary>
        public int nLoopFlag = -1;

        /// <summary>
        /// VTR服务IP
        /// </summary>
        public string szServerIP = "";

        /// <summary>
        /// 上载预卷帧数
        /// </summary>
        public int nPreRolFrame = 0;

        /// <summary>
        /// 波特率
        /// </summary>
        public int nBaudRate = 38400;

        /// <summary>
        /// 工作帧率
        /// </summary>
        public double dbFrameRate = 25.00;

        /// <summary>
        /// VTR备份标识
        /// </summary>
        public emBackupFlag emBackUpType = emBackupFlag.emAllowBackUp;

        /// <summary>
        /// VTR当前状态
        /// </summary>
        public VtrState emVtrState = VtrState.emVtrNormal;

        /// <summary>
        /// VTR工作模式
        /// </summary>
        public VtrWorkMode emWorkMode = VtrWorkMode.emVtrCollectUpload;

        /// <summary>
        /// 默认是标清
        /// </summary>
        public VtrSignalType emVtrSignalType = VtrSignalType.emSD;
    }

;
}
