using System;
using System.Collections.Generic;

namespace IngestCustomerPlugin.Models
{
    public partial class DbpNewimportClip
    {
        public string ClipId { get; set; }
        public string ClipName { get; set; }
        public int? ClipDuration { get; set; }
        public int? ClipType { get; set; }
        public int? ClipState { get; set; }
        public int? ClipTctype { get; set; }
        public int? ClipTcvalue { get; set; }
        public int? ClipTrimin { get; set; }
        public int? ClipTrimout { get; set; }
        public int? ClipIn { get; set; }
        public int? ClipOut { get; set; }
        public string ClipMetadata { get; set; }
        public string ClipEvent { get; set; }
        public string DevId { get; set; }
        public string TaskId { get; set; }
    }
}
