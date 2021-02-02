using IngestDBCore.Notify;
using Sobey.Core.Log;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace UdpNotifyPlugin
{
    public class UdpNotify : ISubNotify
    {
        private readonly ILogger Logger = LoggerManager.GetLogger("UdpNotify");
        public UdpNotify()
        {

        }
        public void ActionNotify(object theClock, NotifyArgs ti)
        {
            //发送通知
            if ((ti.Intent & NotifyPlugin.Udp) > 0)
            {
                var iplist = ti.Param as Dictionary<string, int>;
                if (iplist != null && iplist.Count>0)
                {
                    try
                    {
                        using (Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
                        {
                            Byte[] bySend = Encoding.Unicode.GetBytes(ti.Action);

                            foreach (var item in iplist)
                            {
                                IPAddress ip;
                                if (item.Key != "0.0.0.0" && IPAddress.TryParse(item.Key, out ip))
                                {
                                    Logger.Info($"udpnotify {item.Key} {item.Value}");
                                    IPEndPoint ipe = new IPEndPoint(ip, item.Value);
                                    sock.SendTo(bySend, ipe);
                                }

                            }

                            sock.Close();
                        }
                        
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
