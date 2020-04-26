using System;
using System.Collections.Generic;
using System.Text;

namespace IngestDBCore.Notify
{
    public class NotifyArgs: EventArgs
    {
        public NotifyArgs(string type, string intent, int port, string data)
        {
            Type = type;
            Intent = intent;
            Port = port;
            Data = data;
        }
        public string Type { get; set; }
        public string Intent { get; set; }
        public int Port { get; set; }
        public string Data { get; set; }
    }
}
