namespace IngestTaskPlugin.Dto.Response.OldVtr
{
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

    /// <summary>
    /// VTR状态信息
    /// </summary>
    public enum VtrState
    {
        /// <summary>
        /// 普通VTR
        /// </summary>
        emVtrNormal = 0,
        /// <summary>
        /// 上载VTR
        /// </summary>
        emVtrUpload = 1,
        /// <summary>
        /// 下载VTR
        /// </summary>
        emVtrDownload = 2,
        /// <summary>
        /// 备份VTR
        /// </summary>
        emVtrBackup = 3
    }

    /// <summary>
    /// Defines the VtrSignalType.
    /// </summary>
    public enum VtrSignalType
    {
        /// <summary>
        /// 标清
        /// </summary>
        emSD = 0,
        /// <summary>
        /// 高清
        /// </summary>
        emHD = 1
    }

    /// <summary>
    /// VTR工作模式
    /// </summary>
    public enum VtrWorkMode
    {
        /// <summary>
        /// 集中上载
        /// </summary>
        emVtrCollectUpload = 0,

        /// <summary>
        /// 独立上载
        /// </summary>
        emVtrAloneUpload = 1
    }
}
