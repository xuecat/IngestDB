using IngestDBCore;
using IngestDBCore.Dto;
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
        protected CClientTaskSDKImp _sDKImp { get; }

        public MSVNotify(CClientTaskSDKImp sDKImp)
        {
            _sDKImp = sDKImp;
        }

        public override void ActionNotify(object theClock, NotifyArgs ti)
        {
            //发送通知
            if ((ti.Intent & NotifyPlugin.Msv) > 0)
            {
                _sDKImp.MSV_RelocateRTMP(ti.Action, ti.Port, ti.Type);

                //var imp = ApplicationContext.Current.ServiceProvider.GetRequiredService<CClientTaskSDKImp>();
                // imp.MSV_RelocateRTMP(ti.Intent, ti.Port, ti.Data);
                //var imp = new CClientTaskSDKImp();
                //if(ti.Param is MatrixMsvNotifyInfo)
                //{
                //    var notifyInfo = ti.Param as MatrixMsvNotifyInfo;
                //    imp.MSV_RelocateRTMP(notifyInfo.MsvIp, notifyInfo.Port, notifyInfo.LocalIP);
                //}

            }
        }
    }
}
