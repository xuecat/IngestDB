using System;
using System.Collections.Generic;
using System.Text;

namespace IngestDBCore.Notify
{
    public class NotifyArgs: EventArgs
    {
        public NotifyArgs(string type, int intent, string action, object param, int port)
        {
            Type = type;
            Intent = intent;
            Action = action;
            Param = param;
            Port = port;
        }
        public string Type { get; set; }
        public int Intent { get; set; }
        public string Action { get; set; }
        //public int ID { get; set; }
        public object Param { get; set; }
        public int Port { get; set; }
    }
}
