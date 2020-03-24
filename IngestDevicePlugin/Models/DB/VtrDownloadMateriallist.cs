using System;
using System.Collections.Generic;

namespace IngestDevicePlugin.Models
{
    public partial class VtrDownloadMateriallist
    {
        public int Materialid { get; set; }
        public string Contentid { get; set; }
        public string Fileguid { get; set; }
        public string Filetypeid { get; set; }
        public string Filename { get; set; }
        public int? Fileinpoint { get; set; }
        public int? Fileoutpoint { get; set; }
        public int? Refinpoint { get; set; }
        public int? Refoutpoint { get; set; }
        public string Groupname { get; set; }
        public string Creator { get; set; }
        public DateTime Createtime { get; set; }
    }
}
