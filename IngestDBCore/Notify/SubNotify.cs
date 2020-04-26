using System;
using System.Collections.Generic;
using System.Text;

namespace IngestDBCore.Notify
{
    public class SubNotify
    {
        public void Subscribe(NotifyClock theClock)
        {
            theClock.NotifyChange +=
               new NotifyClock.NotifyChangeHandler(ActionNotify);
        }

        public virtual void ActionNotify(
         object theClock, NotifyArgs ti)
        {
            
        }
    }
}
