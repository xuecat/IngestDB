using System;
using System.Collections.Generic;
using System.Text;

namespace IngestDBCore
{
    public class GlobalStateName
    {
        static public string ADDTASK { get { return "TASK_ADD"; } }
        static public string MODTASK { get { return "TASK_MOD"; } }
        static public string DELTASK { get { return "TASK_DEL"; } }
        static public string MLCRT { get { return "ML_CRT"; } }
        static public string MLCPT { get { return "ML_CPT"; } }
        static public string BACKUP { get { return "BACKUP"; } }
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

        public FunctionType funtype { get; set; }
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
