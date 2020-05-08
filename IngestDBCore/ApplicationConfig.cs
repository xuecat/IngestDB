using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace IngestDBCore
{
    public class ApplicationConfig
    {
        public bool UseSwagger { get; set; }
        public string ConnectionString { get; set; }
        public string IngestDBUrl { get; set; }
        public string IngestMatrixUrl { get; set; }
        public string IngestVtrUrl { get; set; }
        public string VIP { get; set; }
        public string CMServerUrl { get; set; }
        public string CMServerWindowsUrl { get; set; }
    }
}
