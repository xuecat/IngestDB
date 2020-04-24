using IngestDBCore;
using IngestDBCore.Notify;
using Microsoft.Extensions.DependencyInjection;
using MsvClientSDK;
using System;
using System.Collections.Generic;
using System.Text;

namespace MSVNotifyPlugin
{
    public class MSVNotify : SubNotify
    {
        public override void ActionNotify(object theClock, NotifyArgs ti)
        {
            //发送通知
            if (ti.Type == "relocate")
            {
                var imp = ApplicationContext.Current.ServiceProvider.GetRequiredService<CClientTaskSDKImp>();
                // imp.MSV_RelocateRTMP(ti.Intent, ti.Port, ti.Data);
            }
        }
    }
}
