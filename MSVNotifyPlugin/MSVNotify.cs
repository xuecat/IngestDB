using IngestDBCore;
using IngestDBCore.Notify;
using Microsoft.Extensions.DependencyInjection;
using MsvClientSDK;
using Sobey.Core.Log;
using System;
using System.Collections.Generic;
using System.Text;

namespace MSVNotifyPlugin
{
    public class MSVNotify : SubNotify
    {
        private readonly ILogger Logger = LoggerManager.GetLogger("MsvNotify");
        public override void ActionNotify(object theClock, NotifyArgs ti)
        {
            //发送通知
            if ((ti.Intent & NotifyPlugin.Msv) > 0)
            {
                //var imp = ApplicationContext.Current.ServiceProvider.GetRequiredService<CClientTaskSDKImp>();
                // imp.MSV_RelocateRTMP(ti.Intent, ti.Port, ti.Data);
            }
        }
    }
}
