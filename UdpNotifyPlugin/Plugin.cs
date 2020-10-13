﻿using IngestDBCore;
using IngestDBCore.Plugin;
using System;
using System.Threading.Tasks;

namespace UdpNotifyPlugin
{
    public class Plugin : PluginBase
    {
        // {}


        public override string PluginID
        {
            get
            {
                return "8F37B188-C552-49D3-AB8F-5CE920AB5CB0";
            }
        }

        public override string PluginName
        {
            get
            {
                return "UDPNotify";
            }
        }

        public override string Description
        {
            get
            {
                return "UDPNotifyManager";
            }
        }


        public override Task<ResponseMessage> Init(ApplicationContext context)
        {

            //context.Services.AddMassTransit();
            //context.Services.configure


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