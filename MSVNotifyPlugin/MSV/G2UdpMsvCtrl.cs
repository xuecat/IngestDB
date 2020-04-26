using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using Sobey.Core.Log;

namespace G2CtrlRestFulSrv
{ 
    public class G2UdpMsvCtrl
    {
        //private UdpClient udpClient { get; set; }//new UdpClient();
        public G2UdpMsvCtrl()
        {
            //udpClient = new UdpClient(0);
        }
        public string GetMsvUdpData(string strlocip, int nlocport, string strmsvip, int nmsvchport, string strcmd, ILogger Logger, int nTimeOut = 4)
        {
            string strret = "";
            //IPEndPoint localIpep = new IPEndPoint(IPAddress.Parse(strlocip), nlocport); // 本机IP，指定的端口号
            //             if(udpClient == null)
            //             {
            //                 udpClient = new UdpClient();
            //             }
            string strtmplog = string.Format("{0}:{1}", strmsvip, nmsvchport);
            Logger.Info(  strtmplog +", strcmd:" + strcmd);
            UdpClient udpClient = new UdpClient(0);

            try
            {
                
                IPEndPoint remoteIpep = new IPEndPoint(IPAddress.Parse(strmsvip), nmsvchport); // 发送到的IP地址和端口号
                string strContent = strcmd;//string.Format("<?xml version=\"1.0\"?><query_state><nChannel>{0}</nChannel></query_state>\0", 3100);
                byte[] bytes = System.Text.Encoding.Unicode.GetBytes(strContent);
                int u = bytes.Length + 8;
                byte[] byteL = new byte[u];
                byteL[3] = (byte)(u);
                byteL[2] = (byte)(u >> 8);
                byteL[1] = (byte)(u >> 16);
                byteL[0] = (byte)(u >> 24);
                bytes.CopyTo(byteL, 4);
                int a = udpClient.Send(byteL, byteL.Length, remoteIpep);

                udpClient.Client.Blocking = false;
                int buffSizeCurrent = 0;
                buffSizeCurrent = udpClient.Available;//取得缓冲区当前的数据的个数            
                int i = 0;
                int ntimes = 4;
                uint nNeedRecvLen = 0;
                uint nRecvedLen = (uint)0;
                byte[] bytedata = new byte[1024];
                DateTime dtbegin = DateTime.Now;
                while (true)
                {
                    i++;  
                    DateTime dtend = DateTime.Now;
                    TimeSpan ts = dtend - dtbegin;
                    if (ts.TotalSeconds > nTimeOut)
                    {
                        strtmplog = string.Format("{0}:{1}", strmsvip, nmsvchport);
                        Logger.Info(  strtmplog + ",Recv Time out" );
                        break;
                    }
                    if (buffSizeCurrent == 1)
                    {
                        udpClient.Send(byteL, byteL.Length, remoteIpep);
                        System.Threading.Thread.Sleep(100);
                        buffSizeCurrent = udpClient.Available;//取得缓冲区当前的数据的个数  
                        continue;
                    }

                    if (buffSizeCurrent > 0)     //有数据时候才读，不然会出异常哦
                    {
                        byte[] data = udpClient.Receive(ref remoteIpep);  
                        if (nNeedRecvLen == 0)
                        {
                            data.CopyTo(bytedata, nRecvedLen);
                            nRecvedLen += (uint)data.Length;
                            if (nRecvedLen < 4)
                            {
                                System.Threading.Thread.Sleep(100);
                                buffSizeCurrent = udpClient.Available;//取得缓冲区当前的数据的个数   
                                continue;
                            }
                            else
                            {
                                uint nlen = (uint)(bytedata[3] | bytedata[2] << 8 | bytedata[1] << 16 | bytedata[0] << 24);
                                nNeedRecvLen = nlen;  
                                if (nRecvedLen == nNeedRecvLen)
                                {
                                    break;
                                }
                            }

                        }
                        else
                        {
                            data.CopyTo(bytedata, nRecvedLen);
                            nRecvedLen += (uint)data.Length;
                            if (nRecvedLen == nNeedRecvLen)
                            {
                                break;
                            }
                        }

                    }
                    System.Threading.Thread.Sleep(50);
                    buffSizeCurrent = udpClient.Available;//取得缓冲区当前的数据的个数 

                    if (buffSizeCurrent <= 0)
                    {
                        ntimes--;
                    }
                    else
                        ntimes = 4;

                    if (ntimes <=0)//连续四次recevice还出错说明有问题网络
                    {
                        break;
                    }

                }
                //udpClient.Close();
                if (nRecvedLen > 0)
                {
                    byte[] datatmp = new byte[nRecvedLen - 4 - 1];
                    for (int j = 4; j < nRecvedLen - 1; j++)
                    {
                        datatmp[j - 4] = bytedata[j];
                    }
                    //bytedata[nRecvedLen - 1] = Convert.ToByte('\0');
                    strret = Encoding.Unicode.GetString(datatmp).TrimEnd('\0');
                    strtmplog = string.Format("{0}:{1}", strmsvip, nmsvchport);
                    Logger.Info(  strtmplog  + ",Recv:" +strret);
                }
                else
                {
                    Logger.Info("Recv: len error");
                }
                udpClient.Close();
                udpClient = null;
                bytedata = null;
            }
            catch (System.ObjectDisposedException ex)
            {
                udpClient.Close();
                udpClient = null;
                Logger.Info(  strtmplog + "GetMsvUdpData Error!errorinfo=" + ex.ToString());
            }
            catch (System.ArgumentException ex)
            {
                udpClient.Close();
                udpClient = null;
                Logger.Info(  strtmplog + "GetMsvUdpData Error!errorinfo=" + ex.ToString());
            }
            catch (System.InvalidOperationException ex)
            {
                udpClient.Close();
                udpClient = null;
                Logger.Info(  strtmplog + "GetMsvUdpData Error!errorinfo=" + ex.ToString());
            }
            catch (SocketException ex)
            {
                udpClient.Close();
                udpClient = null;
                Logger.Info(  strtmplog + "GetMsvUdpData Error!errorinfo=" + ex.ToString());
            }
            catch (System.Exception ex)
            {
                udpClient.Close();
                udpClient = null;
                Logger.Info( strtmplog + "GetMsvUdpData Error!errorinfo="+ex.ToString());

            }
            return strret;
        }

    }
}
