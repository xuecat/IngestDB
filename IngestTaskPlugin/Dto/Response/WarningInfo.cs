using System;
using System.Collections.Generic;
using System.Text;

namespace IngestTaskPlugin.Dto
{
    public class WarningInfoResponse
    {
        public int TaskID { get; set; }
        public int RelatedID { get; set; }
        public int WarningLevel { get; set; }
        public string WarningMessage { get; set; }
    }
}
