using IngestDBCore.Notify;
using Sobey.Core.Log;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace UdpNotifyPlugin
{
    public class UdpNotify : SubNotify
    {
        private readonly ILogger Logger = LoggerManager.GetLogger("UdpNotify");
        public UdpNotify()
        {

        }
        public override void ActionNotify(object theClock, NotifyArgs ti)
        {
            //发送通知
            if ((ti.Intent & NotifyPlugin.Udp) > 0)
            {
                var iplist = ti.Param as Dictionary<string, int>;
                if (iplist != null && iplist.Count>0)
                {
                    try
                    {
                        Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

                        Byte[] bySend = Encoding.Unicode.GetBytes(ti.Action);

                        foreach (var item in iplist)
                        {
                            Logger.Info($"udpnotify {item.Key} {item.Value}");
                            IPEndPoint ipe = new IPEndPoint(IPAddress.Parse(item.Key), item.Value);
                            sock.SendTo(bySend, ipe);
                        }

                        sock.Close();
                        sock = null;
                    }
                    catch (Exception e)
                    {
                        Logger.Error($"ActionNotify exception {e.Message}");
                    }
                    
                }
            }
        }
    }
}
