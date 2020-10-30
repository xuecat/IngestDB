using System;
using System.Collections.Generic;
using System.Text;


namespace IngestTaskPlugin.Dto.Response
{
    public class TaskMetadataResponse
    {
        /// <summary>任务id</summary>
        /// <example>1</example>
        public int TaskId { get; set; }
        /// <summary>任务来源</summary>
        /// <example>任务源数据</example>
        public string Metadata { get; set; }
    }

    public class TaskMaterialMetaResponse
    {
        /// <summary>生成素材title</summary>
        /// <example>string</example>
        public string Title { get; set; }
        /// <summary>生成素材materialid</summary>
        /// <example>string</example>
        public string MaterialId { get; set; }
        /// <summary>权限</summary>
        /// <example>string</example>
        public string Rights { get; set; }
        /// <summary>评论</summary>
        /// <example>string</example>
        public string Comments { get; set; }
        /// <summary>生成素材路径目录</summary>
        /// <example>string</example>
        public string Destination { get; set; }
        /// <summary>生成素材目录类型</summary>
        /// <example>0</example>
        public int FolderId { get; set; }
        /// <summary>暂时无</summary>
        /// <example>string</example>
        public string ItemName { get; set; }
        /// <summary>暂时无</summary>
        /// <example>string</example>
        public string JournaList { get; set; }
        /// <summary>暂时无</summary>
        /// <example>string</example>
        public string CateGory { get; set; }
        /// <summary>暂时无</summary>
        /// <example>string</example>
        public string ProgramName {get;set;}
        /// <summary>暂时无</summary>
        /// <example>0</example>
        public int Datefolder { get; set; }
    }

    public class TaskContentMetaResponse
    {
        /// <summary>暂时无</summary>
        /// <example>0</example>
        public int HouseTc { get; set; }
        /// <summary>暂时无</summary>
        /// <example>0</example>
        public int PresetStamp { get; set; }
        /// <summary>暂时无</summary>
        public PeriodParamResponse PeriodParam { get; set; }
        /// <summary>暂时无</summary>
        /// <example>0</example>
        public int SixteenToNine { get; set; }
        /// <summary>暂时无</summary>
        /// <example>0</example>
        public int SourceTapeID { get; set; }
        /// <summary>暂时无</summary>
        /// <example>0</example>
        public int DeleteFlag { get; set; }
        /// <summary>暂时无</summary>
        /// <example>0</example>
        public int SourceTapeBarcode {get;set;}
        /// <summary>暂时无</summary>
        /// <example>0</example>
        public int BackTapeId { get; set; }
        /// <summary>暂时无</summary>
        /// <example>0</example>
        public int UserMediaId { get; set; }
        /// <summary>暂时无</summary>
        /// <example>string</example>
        public string UserToken { get; set; }
        /// <summary>暂时无</summary>
        /// <example>string</example>
        public string VtrStart { get; set; }
        /// <summary>暂时无</summary>
        /// <example>0</example>
        public int TcMode { get; set; }
        /// <summary>暂时无</summary>
        /// <example>0</example>
        public int ClipSum { get; set; } = -1;
        /// <summary>暂时无</summary>
        /// <example>0</example>
        public int TransState { get; set; }

        /// <summary>音频轨道配置</summary>
        /// <example>-1</example>
        public int AudioChannels { get; set; } = -1;
        /// <summary>纯音频配置</summary>
        /// <example>0</example>
        public int AudioChannelAttribute { get; set; }
        /// <summary>音频掩码</summary>
        /// <example>-1</example>
        public int ASRmask { get; set; } = -1;
        /// <summary>指定信号rtmp切换url</summary>
        /// <example>rtmp</example>
        public string SignalRtmpUrl { get; set; }
    }

    public class TaskPlanningResponse
    {
        /// <summary>暂时无</summary>
        /// <example>string</example>
        public string PlanGuid { get; set; }
        /// <summary>暂时无</summary>
        /// <example>string</example>
        public string PlanName { get; set; }
        /// <summary>暂时无</summary>
        /// <example>string</example>
        public string CreaToRName { get; set; }//这个很坑，但是要改客户端算了
        /// <summary>暂时无</summary>
        /// <example>string</example>
        public string CreateDate { get; set; }
        /// <summary>暂时无</summary>
        /// <example>string</example>
        public string ModifyName { get; set; }
        /// <summary>暂时无</summary>
        /// <example>string</example>
        public string ModifyDate { get; set; }
        /// <summary>暂时无</summary>
        /// <example>0</example>
        public int Version { get; set; }
        /// <summary>暂时无</summary>
        /// <example>string</example>
        public string Place { get; set; }
        /// <summary>暂时无</summary>
        /// <example>string</example>
        public string PlanningDate { get; set; }
        /// <summary>暂时无</summary>
        /// <example>string</example>
        public string Director { get; set; }
        /// <summary>暂时无</summary>
        /// <example>string</example>
        public string Photographer { get; set; }
        /// <summary>暂时无</summary>
        /// <example>string</example>
        public string Reporter { get; set; }
        /// <summary>暂时无</summary>
        /// <example>string</example>
        public string Other { get; set; }
        /// <summary>暂时无</summary>
        /// <example>string</example>
        public string Equipment { get; set; }
        /// <summary>暂时无</summary>
        /// <example>string</example>
        public string ContactInfo { get; set; }
        /// <summary>暂时无</summary>
        /// <example>string</example>
        public string PlanningXml { get; set; }
    }

    public class TaskSplitResponse
    {
        public string VtrStart { get; set; }
    }

    public class PeriodParamResponse
    {
        public string BeginDate { get; set; }
        public string EndDate { get; set; }
        public int AppDate { get; set; }
        public string AppDateFormat { get; set; }
        public int Mode { get; set; }
        public List<int> Params { get; set; }//DAY
    }

    public class PropertyResponse
    {
        public string Property { get; set; }
        public string Value { get; set; }
    }
}
