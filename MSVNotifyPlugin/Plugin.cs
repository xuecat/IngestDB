using IngestDBCore;
using IngestDBCore.Notify;
using IngestDBCore.Plugin;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace MSVNotifyPlugin
{
    public class Plugin : PluginBase
    {
        // {B8AD5930-E923-49D1-AC54-AF77EA755418}
       
        public override string PluginID
        {
            get
            {
                return "B8AD5930-E923-49D1-AC54-AF77EA755418";
            }
        }

        public override string PluginName
        {
            get
            {
                return "MsvNotify";
            }
        }

        public override string Description
        {
            get
            {
                return "MsvNotifyManager";
            }
        }


        public override Task<ResponseMessage> Init(ApplicationContext context)
        {
            var notify = new MSVNotify();
            context.Services.AddSingleton<MSVNotify>(notify);
            context.Services.AddSingleton<MsvClientSDK.CClientTaskSDKImp>();
            notify.Subscribe(context.NotifyClock);

            return base.Init(context);
        }


        public override Task<ResponseMessage> Start(ApplicationContext context)
        {
            return base.Start(context);
        }

        public override Task<ResponseMessage> Stop(ApplicationContext context)
        {
            return base.Stop(context);
        }
    }
}
