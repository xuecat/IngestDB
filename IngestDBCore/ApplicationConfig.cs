using System;
using System.Collections.Generic;
using System.Net;
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
        public string KafkaUrl { get; set; }
        public bool Limit24Hours { get; set; }
        public bool NotifyUdpInfomation { get; set; }

        public bool GlobalNotify { get; set; }
        public IPEndPoint[] IngestTask { get; set; }
    }
}
