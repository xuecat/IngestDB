using System;
using System.Collections.Generic;
using System.Text;

namespace IngestDBCore.Notify
{
    public class NotifyClock
    {
        public delegate void NotifyChangeHandler(object clock, NotifyArgs arg);
        public event NotifyChangeHandler NotifyChange;

        public void InvokeNotify(string type, string intent, int port, string data)
        {
            NotifyChange(this, new NotifyArgs(type, intent, port, data));
        }
    }
}
