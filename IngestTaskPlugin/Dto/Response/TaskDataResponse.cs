﻿using System;
using System.Collections.Generic;
using System.Text;

namespace IngestTaskPlugin.Dto
{
    public class TaskContentResponse
    {
        public int TaskID { get; set; }
        public string TaskName { get; set; }
        public string TaskDesc { get; set; }
        public string Classify { get; set; }
        public int ChannelID { get; set; }
        public int Unit { get; set; }
        public string UserCode { get; set; }
        public int SignalID { get; set; }
        public string Begin { get; set; } = DateTime.MinValue.ToString("yyyy-MM-dd HH:mm:ss");
        public string End { get; set; } = DateTime.MinValue.ToString("yyyy-MM-dd HH:mm:ss");
        public TaskType TaskType { get; set; } = TaskType.TT_NORMAL;
        public CooperantType CooperantType { get; set; } = CooperantType.emPureTask;
        public taskState State { get; set; }
        public string StampImage { get; set; }
        public string TaskGUID { get; set; }
        public int BackupVTRID { get; set; }
        public TaskPriority Priority { get; set; } = TaskPriority.TP_Normal;
        public int StampTitleIndex { get; set; }
        public int StampImageType { get; set; }
        public int GroupColor { get; set; }//注意没有S
    }

    public class TaskInfoResponse
    {
        public TaskContentResponse TaskContent { get; set; }
        public TaskMaterialMetaResponse MaterialMeta { get; set; }
        public TaskContentMetaResponse ContentMeta { get; set; }
        public TaskPlanningResponse PlanningMeta { get; set; }
        public TaskSplitResponse SplitMeta { get; set; }
        public string CaptureMeta { get; set; }
    }
}
