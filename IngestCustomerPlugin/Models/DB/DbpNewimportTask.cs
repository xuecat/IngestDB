using System;
using System.Collections.Generic;

namespace IngestCustomerPlugin.Models
{
    public partial class DbpNewimportTask
    {
        public string TaskId { get; set; }
        public string TaskName { get; set; }
        public int? TaskType { get; set; }
        public int? TaskState { get; set; }
        public int? TaskDuration { get; set; }
        public int? TaskLProgress { get; set; }
        public int? TaskHProgress { get; set; }
        public DateTime TaskBegintime { get; set; }
        public DateTime TaskEndtime { get; set; }
        public string TaskUser { get; set; }
        public string TaskErr { get; set; }
        public string TaskServiceid { get; set; }
        public string TaskCommand { get; set; }
        public string TaskMetadata { get; set; }
        public string TaskFileinfo { get; set; }
    }
}
