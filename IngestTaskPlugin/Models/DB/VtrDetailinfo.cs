using System;
using System.Collections.Generic;

namespace IngestTaskPlugin.Models
{
    public partial class VtrDetailinfo
    {
        public int Vtrid { get; set; }
        public int Vtrtypeid { get; set; }
        public int Vtrsubtype { get; set; }
        public string Vtrname { get; set; }
        public string Vtrddescribe { get; set; }
        public int Vtrcomport { get; set; }
        public int? Looprecord { get; set; }
        public string Vtrserverip { get; set; }
        public int? Prerolframenum { get; set; }
        public int? Baudrate { get; set; }
        public int? Backuptype { get; set; }
        public float? Framerate { get; set; }
        public int? Vtrstate { get; set; }
        public int? Workmode { get; set; }
        public int? Vtrsignaltype { get; set; }
    }
}
