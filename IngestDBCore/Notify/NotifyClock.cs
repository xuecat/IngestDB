using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace IngestDBCore.Notify
{
    public class NotifyClock
    {
        public delegate void NotifyChangeHandler(object clock, NotifyArgs arg);
        public event NotifyChangeHandler NotifyChange;
        public NotifyClock()
        {
            
        }

        public NotifyClock Register(IServiceProvider services)
        {
            if (NotifyChange == null|| NotifyChange.GetInvocationList().Length < 0)
            {
                var serviceinfo = services.GetServices<ISubNotify>();
                if (serviceinfo != null)
                {
                    foreach (var item in serviceinfo)
                    {
                        NotifyChange += item.ActionNotify;
                    }
                }
            }
            return this;
        }

        public void InvokeNotify(string type, int intent, string action, object param, int port = -1)
        {
            NotifyChange(this, new NotifyArgs(type, intent, action, param, port));
        }
    }
}
