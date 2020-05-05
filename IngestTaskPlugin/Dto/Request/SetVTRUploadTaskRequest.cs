using IngestTaskPlugin.Dto.Response.OldVtr;
using System;
using System.Collections.Generic;
using System.Text;

namespace IngestTaskPlugin.Dto.Request
{
    public enum VTRUploadTaskMask
    {
        VTR_Mask_TaskId = 0x00000001,
        VTR_Mask_VtrId = 0x00000002,
        VTR_Mask_BlankTaskId = 0x00000004,
        VTR_Mask_VtrTaskType = 0x00000008,

        VTR_Mask_ChannelId = 0x00000010,
        VTR_Mask_SignalId = 0x00000020,
        VTR_Mask_TrimIn = 0x00000040,
        VTR_Mask_TrimOut = 0x00000080,

        VTR_Mask_UserCode = 0x00000100,
        VTR_Mask_CommitTime = 0x00000200,
        VTR_Mask_Order = 0x00000400,
        VTR_Mask_TaskGUID = 0x00000800,

        VTR_Mask_TapeId = 0x00001000,
        VTR_Mask_TaskName = 0x00002000,
        VTR_Mask_UserToken = 0x00004000,
        VTR_Mask_TrimInCTL = 0x00008000,

        VTR_Mask_TrimOutCTL = 0x00010000,
        VTR_Mask_TaskState = 0x00020000,
        VTR_Mask_TaskDesc = 0x00040000,
        VTR_Mask_Classify = 0x00080000,

        VTR_Mask_Unit = 0x00100000,
        VTR_Mask_BeginTime = 0x00200000,
        VTR_Mask_EndTime = 0x00400000,
        VTR_Mask_TaskType = 0x00800000,

        VTR_Mask_CooperantType = 0x01000000,
        VTR_Mask_State = 0x02000000,
        VTR_Mask_StampImage = 0x04000000,

        //任务元数据
        VTR_Mask_CapatureMetaData = 0x08000000,
        VTR_Mask_StoreMetaData = 0x10000000,
        VTR_Mask_ContentMetaData = 0x20000000,
        VTR_Mask_PlanMetaData = 0x40000000
    }

    public enum VTR_BUT_ErrorCode
    {
        emNormal = 0,       //成功
        emVTRCollide,       //VTR冲突了，没有合适的VTR
        emNoChannel,        //没有可用的通道
        emSomeSuccessful    //某些任务可以提交成功
    };

    public class VTR_UPLOAD_MetadataPair
    {
        public int nTaskID;
        public string strMetadata;
        public int emType;
    }

    public class CommitParam
    {
        public List<int> taskIds;
        public bool ignoreWrong;
    }


    public class CommitParamRequest
    {
        public List<int> taskids { get; set; }
        public bool ignorewrong { get; set; }
    }
 
}
