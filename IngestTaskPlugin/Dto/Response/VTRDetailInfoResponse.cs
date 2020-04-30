using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IngestTaskPlugin.Dto.Response.OldVtr;

namespace IngestTaskPlugin.Dto.Response
{
    /// <summary>
    /// Vtr详细信息 <see cref="VTRDetailInfo" />.
    /// </summary>
    public class VTRDetailInfoResponse
    {
        /// <summary>
        /// VtrId
        /// </summary>
        public int VtrId { get; set; } = 0;

        /// <summary>
        /// VTR类型ID
        /// </summary>
        public int VtrTypeId { get; set; } = 0;

        /// <summary>
        /// VTR子类型ID
        /// </summary>
        public int VtrSubTypeId { get; set; } = 0;

        /// <summary>
        /// 录像机名称
        /// </summary>
        public string VtrDetailName { get; set; } = "";

        /// <summary>
        /// VTR描述
        /// </summary>
        public string VtrDetailDesc { get; set; } = "";

        /// <summary>
        /// VTR控制串口号
        /// </summary>
        public int VtrVComPortIdx { get; set; } = -1;

        /// <summary>
        /// 循环录制标志
        /// </summary>
        public int LoopFlag { get; set; } = -1;

        /// <summary>
        /// VTR服务IP
        /// </summary>
        public string ServerIP { get; set; } = "";

        /// <summary>
        /// 上载预卷帧数
        /// </summary>
        public int PreRolFrame { get; set; } = 0;

        /// <summary>
        /// 波特率
        /// </summary>
        public int BaudRate { get; set; } = 38400;

        /// <summary>
        /// 工作帧率
        /// </summary>
        public double FrameRate { get; set; } = 25.00;

        /// <summary>
        /// VTR备份标识
        /// </summary>
        public VtrBackupFlag BackUpType { get; set; } = VtrBackupFlag.emAllowBackUp;

        /// <summary>
        /// VTR当前状态
        /// </summary>
        public VtrState VtrState { get; set; } = VtrState.emVtrNormal;

        /// <summary>
        /// VTR工作模式
        /// </summary>
        public VtrWorkMode WorkMode { get; set; } = VtrWorkMode.emVtrCollectUpload;

        /// <summary>
        /// 默认是标清
        /// </summary>
        public VtrSignalType VtrSignalType { get; set; } = VtrSignalType.emSD;
    }
}
