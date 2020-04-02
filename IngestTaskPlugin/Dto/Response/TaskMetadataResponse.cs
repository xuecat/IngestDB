using System;
using System.Collections.Generic;
using System.Text;


namespace IngestTaskPlugin.Dto
{
    public class TaskMetadataResponse
    {
        public int TaskID { get; set; }
        public string Metadata { get; set; }
    }

    public class TaskMaterialMetaResponse
    {
        public string Title { get; set; }
        public string MaterialID { get; set; }
        public string Rights { get; set; }
        public string Comments { get; set; }
        public string Destination { get; set; }
        public int FolderID { get; set; }
        public string ItemName { get; set; }
        public string JournaList { get; set; }
        public string CateGory { get; set; }
        public string ProgramName {get;set;}
    }

    public class TaskContentMetaResponse
    {
        public int HouseTC { get; set; }
        public int Presetstamp { get; set; }
        public int SixteenToNine { get; set; }
        public int SourceTapeID { get; set; }
        public int DeleteFlag { get; set; }
        public int SourceTapeBarcode {get;set;}
        public int BackTapeID { get; set; }
        public int UserMediaID { get; set; }
        public string UserToken { get; set; }
        public string VtrStart { get; set; }
        public int TcMode { get; set; }
        public int ClipSum { get; set; }
        public int TransState { get; set; }
    }

    public class TaskPlanningResponse
    {
        public string PlanGuid { get; set; }
        public string PlanName { get; set; }
        public string CreaToRName { get; set; }//这个很坑，但是要改客户端算了
        public string CreateDate { get; set; }
        public string ModifyName { get; set; }
        public string ModifyDate { get; set; }
        public int Version { get; set; }
        public string Place { get; set; }
        public string PlanningDate { get; set; }
        public string Director { get; set; }
        public string Photographer { get; set; }
        public string Reporter { get; set; }
        public string Other { get; set; }
        public string Equipment { get; set; }
        public string ContactInfo { get; set; }
        public string PlanningXml { get; set; }
    }

    public class TaskSplitResponse
    {
        public string VtrStart { get; set; }
    }

    public class PropertyResponse
    {
        public string Property { get; set; }
        public string Value { get; set; }
    }
}
