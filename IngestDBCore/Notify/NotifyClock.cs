﻿using System;
using System.Collections.Generic;
using System.Text;

namespace IngestDBCore.Notify
{
    public class NotifyClock
    {
        public delegate void NotifyChangeHandler(object clock, NotifyArgs arg);
        public event NotifyChangeHandler NotifyChange;

        public void InvokeNotify(string type, int intent, string action, object param, int port = -1)
        {
            NotifyChange(this, new NotifyArgs(type, intent, action, param, port));
        }
    }
}
