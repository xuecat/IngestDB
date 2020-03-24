using System;
using System.Collections.Generic;

namespace IngestCustomerPlugin.Models
{
    public partial class DbpMaterialVideo
    {
        public int Materialid { get; set; }
        public string Videofilename { get; set; }
        public int Videotypeid { get; set; }
        public int Videosource { get; set; }
    }
}
