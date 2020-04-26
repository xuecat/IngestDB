using System;
using System.Collections.Generic;
using System.Text;

namespace IngestGlobalPlugin.Dto
{
    public class VideoInfoResponse
    {
        public string Filename { get; set; }
        public int VideoTypeID { get; set; } = -1;
        public int VideoSource { get; set; }//视频来源
    }

    public class AudioInfoResponse
    {
        public string Filename { get; set; }
        public int AudioTypeID { get; set; } = -1;
        public int AudioSource { get; set; }//音频来源
    }
    public class MaterialInfoResponse
    {
        public int ID { get; set; }         //素材ID
        public string Name { get; set; }     //素材名
        public string Remark { get; set; }   //素材描述
        public string UserCode { get; set; } //用户编码
        public string CreateTime { get; set; } = DateTime.MinValue.ToString("yyyy-MM-dd HH:mm:ss");    //创建时间
        public int TaskID { get; set; }     //任务ID
        public int SectionID { get; set; }  //分段索引
        public string GUID { get; set; } //素材GUID
        public int ClipState { get; set; } = (int)CLIP_STATE.STARTCAPUTER;   //素材采集状态
        public List<VideoInfoResponse> Videos { get; set; }  //视频文件列表
        public List<AudioInfoResponse> Audios { get; set; }  //音频文件列表
        public List<int> ArchivePolicys { get; set; }    //入库策略列表
        public int DeleteState { get; set; } = (int)DELETED_STATE.NOTDELETED;
    }
}
