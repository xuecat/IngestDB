using System;
using System.Collections.Generic;
using System.Text;

namespace IngestTaskPlugin.Dto
{
    public class CompleteSyncTaskRequest
    {
        public bool IsFinish { get; set; }
        public bool Perodic2Next { get; set; }
        public int TaskID { get; set; }
        public int TaskState { get; set; }
        public int DispatchState { get; set; }
        public int SynState { get; set; }
    }
}
