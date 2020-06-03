using System;
using System.Collections.Generic;
using System.Text;

namespace IngestDBCore.Notify
{
    public class NotifyArgs: EventArgs
    {
        public NotifyArgs(string type, int intent, string action, int id, object param)
        {
            Type = type;
            Intent = intent;
            Action = action;
            ID = id;
            Param = param;
        }
        public string Type { get; set; }
        public int Intent { get; set; }
        public string Action { get; set; }
        public int ID { get; set; }
        public object Param { get; set; }
    }
}
