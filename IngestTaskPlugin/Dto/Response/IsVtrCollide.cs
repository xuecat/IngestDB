using System;
using System.Collections.Generic;
using System.Text;

namespace IngestTaskPlugin.Dto
{
    public class IsVtrCollide
    {
        public VTRCollideResult Result { get; set; }
        public TaskContentResponse CollideTaskContent { get; set; }
    }
}
