using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace IngestDBCore
{
    public class GlobalStateName
    {
        public const string ADDTASK = "TASK_ADD";
        public const string MODTASK = "TASK_MOD";
        public const string DELTASK = "TASK_DEL";
        public const string INCHANGE = "INCHANGE";
        public const string INDELETE = "INDELETE";
        public const string OUTCHANGE = "OUTCHANGE";
        public const string OUTDELETE = "OUTDELETE";
        public const string DEVICECHANGE = "DEVICECHANGE";
        public const string DEVICEDELETE = "DEVICEDELETE";
        public const string CHANNELCHANGE = "CHANNELCHANGE";
        public const string CHANNELDELETE = "CHANNELDELETE";
        public const string MLCRT = "ML_CRT";
        public const string MLCPT = "ML_CPT";
        public const string BACKUP = "BACKUP";
    }
    public static class ClientOperLabelName
    {
        public const string SDI_AddTask = "SDI_AddTask";
        public const string SDI_DeleteTask = "SDI_DeleteTask";
        public const string SDI_ModifyTask = "SDI_ModifyTask";
        public const string VTR_UPLOAD_AddTask = "VTR_UPLOAD_AddTask";
        public const string VTR_UPLOAD_DeleteTask = "VTR_UPLOAD_DeleteTask";
        public const string VTR_UPLOAD_ModifyTask = "VTR_UPLOAD_ModifyTask";
        
    }

    public class GlobalInternals
    {
        public enum FunctionType
        {
            SetGlobalState,
            /// <summary>
            /// /////////////////////////////////////////////////////////
            /// </summary>
            UserParamTemplateByID,
            MaterialInfo
        }

        public FunctionType Funtype { get; set; }
        public int TemplateID { get; set; }

        public string State { get; set; }
        public int TaskID { get; set; }
    }


    public enum CLIP_STATEINTERFACE
    {
        STARTCAPUTER = 0,   //开始采集了,可以边采边编了	
        STOPCAPTURE,    //采集完成
    }

    public enum DELETED_STATEINTERFACE
    {
        NOTDELETED = 0,                 //没有被删除  
        DELETEDBYARCHIVESERVICE,        //被入库服务删除
        DELETEDBYMETADATASTATUSMANAGER, //被入库失败素材查询工具删除
        DELETEDBYOTHER,                 //被其他工具删除的
    }

    public class VideoInfoInterface
    {
        public string Filename { get; set; }
        public int VideoTypeID { get; set; } = -1;
        public int VideoSource { get; set; }//视频来源
    }

    public class AudioInfoInterface
    {
        public string Filename { get; set; }
        public int AudioTypeID { get; set; } = -1;
        public int AudioSource { get; set; }//音频来源
    }
    public class MaterialInfoInterface
    {
        public int ID { get; set; }         //素材ID
        public string Name { get; set; }     //素材名
        public string Remark { get; set; }   //素材描述
        public string UserCode { get; set; } //用户编码
        public string CreateTime { get; set; } = DateTime.MinValue.ToString("yyyy-MM-dd HH:mm:ss");    //创建时间
        public int TaskID { get; set; }     //任务ID
        public int SectionID { get; set; }  //分段索引
        public string GUID { get; set; } //素材GUID
        public int ClipState { get; set; } = (int)CLIP_STATEINTERFACE.STARTCAPUTER;   //素材采集状态
        public List<VideoInfoInterface> Videos { get; set; }  //视频文件列表
        public List<AudioInfoInterface> Audios { get; set; }  //音频文件列表
        public List<int> ArchivePolicys { get; set; }    //入库策略列表
        public int DeleteState { get; set; } = (int)DELETED_STATEINTERFACE.NOTDELETED;
    }
}
