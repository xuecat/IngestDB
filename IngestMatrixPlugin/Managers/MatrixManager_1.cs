using IngestDBCore;
using IngestDBCore.Notify;
using IngestMatrixPlugin.Dto.OldResponse.v1;
using IngestMatrixPlugin.Dto.Vo;
using IngestMatrixPlugin.Models.DB;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IngestMatrixPlugin.Managers
{
    public partial class MatrixManager
    {
        public async Task<bool> SwitchInOutByAreaAsync(long inPort, long outPort, DbpRcdindesc dbpRcdindesc, DbpRcdoutdesc dbpRcdoutdesc)
        {
            Logger.Info("**********************************************************************");
            string strlog = string.Format("Begin to switch in[{0}]-and -out[{1}] Port...", inPort, outPort);
            Logger.Info(strlog);

            var releasedRoutList = new List<DbpMatrixrout>();
            var releasedVirtualPortList = new List<DbpVirtualmatrixportstate>();

            var infoList = await Store.QueryVirtualmatrixportstate(a =>
            a.Where(x => x.Virtualoutport == outPort && x.State == 1), true);

            if (infoList.Count > 0)
            {
                foreach (var info in infoList)
                {
                    if (info.Virtualinport != inPort)
                    {
                        info.State = 1;
                        releasedVirtualPortList.Add(info);

                        await ReleaseWhenSwitch(info.Virtualinport, outPort, releasedRoutList);
                    }
                }
            }
            else
            {
                Logger.Warn("In module MatrixService!virtualOutport is non-existence with state = 1 in DB,so no need to release in and out ports!,lOutPort");
            }
            #region 得到虚拟输出端口对应的真实矩阵输入端口和矩阵ID
            long lMatrixID = -1;
            long lRealOutPort = -1;
            if (!Store.GetRealMatrixOutPort(outPort, ref lRealOutPort, ref lMatrixID))
            {
                if (!await RecoverReleasedRoutAndPort(releasedVirtualPortList, releasedRoutList))
                {
                    Logger.Error("In module MatrixService!call CIVirtualMatrix::SwitchInOut(),recover the released rout and port failed!");
                }
                Logger.Error("End to switch in-and-out Port...");
                return false;
            }
            #endregion

            #region 获取路由表
            var routList = await TrySimpleRout(inPort, lMatrixID, lRealOutPort, outPort);
            if (routList.Count <= 0)
            {
                Logger.Warn($"In module MatrixService!call CIVirtualMatrix::SwitchInOut(),can not find rout {inPort} to {outPort}(tryrout)");

                if (await RecoverReleasedRoutAndPort(releasedVirtualPortList, releasedRoutList))
                {
                    Logger.Warn("In module MatrixService!call CIVirtualMatrix::SwitchInOut(),recover the released rout and port failed!");
                }
                return false;
            }
            Logger.Info("TryRout count : " + routList.Count);
            #endregion

            MatrixRoutInfo RoutInfo = new MatrixRoutInfo();
            foreach (MatrixRoutInfo info in routList)
            {
                MatrixParam param = new MatrixParam();

                string deviceip = "", deviceport = "";
                
                if (dbpRcdindesc == null)
                {
                    dbpRcdindesc = (await Store.QueryRcdindesc(a => a.Where(x => x.Recinidx == inPort), true)).FirstOrDefault();
                    if (dbpRcdindesc == null)
                    {
                        Logger.Error($"call DBAccessMatrixInfo::SwitchInOutByArea(), no inport {inPort} .");
                        if (!await RecoverReleasedRoutAndPort(releasedVirtualPortList, releasedRoutList))
                        {
                            Logger.Error("In module MatrixService!call CIVirtualMatrix::SwitchInOut(),recover the released rout and port failed!");
                        }
                        return false;
                    }

                }

                if (dbpRcdoutdesc == null)
                {
                    dbpRcdoutdesc = (await Store.QueryRcdoutdesc(a => a.Where(x => x.Recoutidx == outPort), true)).FirstOrDefault();
                    if (dbpRcdoutdesc == null)
                    {
                        Logger.Error($"call DBAccessMatrixInfo::SwitchInOutByArea(), no outport {outPort} .");
                        if (!await RecoverReleasedRoutAndPort(releasedVirtualPortList, releasedRoutList))
                        {
                            Logger.Error("In module MatrixService!call CIVirtualMatrix::SwitchInOut(),recover the released rout and port failed!");
                        }
                        return false;
                    }
                }

                if (dbpRcdindesc.Area != dbpRcdoutdesc.Area) //先不修改赋值，直接判断不等的结果，感觉赋值先不用改 
                {
                    Logger.Error($"call DBAccessMatrixInfo::SwitchInOutByArea(), in area not equal out area.");
                    if (!await RecoverReleasedRoutAndPort(releasedVirtualPortList, releasedRoutList))
                    {
                        Logger.Error("In module MatrixService!call CIVirtualMatrix::SwitchInOut(),recover the released rout and port failed!");
                    }
                    return false;
                }
                else
                {
                    var dbpArea = await Store.QueryDbpArea(a => a.Where(x => x.Id == dbpRcdindesc.Area).FirstOrDefaultAsync(), true);
                    if (dbpArea == null)
                    {
                        Logger.Error($"call DBAccessMatrixInfo::SwitchInOutByArea(), no site {dbpRcdindesc.Area}");
                        if (!await RecoverReleasedRoutAndPort(releasedVirtualPortList, releasedRoutList))//切换失败时，要恢复原来的链接状态
                        {
                            Logger.Error("In module MatrixService!call CIVirtualMatrix::SwitchInOut(),recover the released rout and port failed!");
                        }
                        return false;
                    }

                    param.matrixTypeID = dbpArea.MatrixTypeId;
                    long type = param.matrixTypeID;
                    param.matrixName = dbpArea.MatrixName;
                    //param.matrixName = sitedata.Rows[0][1].ToString();
                    param.commPort = dbpArea.ComPort;
                    param.lInPort = info.lInPort;
                    param.lOutPort = info.lOutPort;
                    deviceip = dbpArea.DeviceCtrlIp;
                    deviceport = dbpArea.DeviceCtrlPort.ToString();
                }
                Logger.Info(JsonConvert.SerializeObject(param));

                MatrixOldResponseMessage msg = null;
                if (dbpRcdindesc.Signalsource == 7)
                {
                    string msvip = string.Empty;
                    int msvport = -1;
                    if (_deviceInterface != null)
                    {
                        var response = await _deviceInterface.Value.GetDeviceCallBack(new DeviceInternals()
                        {
                            funtype = IngestDBCore.DeviceInternals.FunctionType.DeviceInfoByID,
                            DeviceId = dbpRcdoutdesc.Rcdeviceid
                        });

                        var deviceInfos = response as ResponseMessage<DeviceInfoInterface>;

                        if (deviceInfos != null)
                        {
                            msvip = deviceInfos.Ext.Ip;
                            msvport = deviceInfos.Ext.ChannelIndex;
                        }
                    }

                    Logger.Error($"call SwitchInOutAsync, msvip: {msvip},msvport:{msvport}, dbpRcdindesc.Ipaddress:{dbpRcdindesc.Ipaddress}.");

                    Task.Run(() => { _clock.InvokeNotify(msvip, NotifyPlugin.Msv, NotifyAction.MSVRELOCATE, dbpRcdindesc.Ipaddress, msvport); });

                    //if (!ApplicationContext.Current.CtrlSDK.Relecate(msvip, msvport, dbpRcdindesc.Ipaddress))
                    //{
                    //    Logger.Error($"切换rtmp矩阵失败，直接return");
                    //    if (!await RecoverReleasedRoutAndPort(releasedVirtualPortList, releasedRoutList))
                    //    {
                    //        Logger.Error("In module MatrixService!call CIVirtualMatrix::SwitchInOut(),recover the released rout and port failed!");
                    //    }
                    //    //切换失败，直接返回
                    //    return false;
                    //}

                    msg = new MatrixOldResponseMessage();
                    msg.nCode = 1;
                    msg.message = "ok";
                }
                else
                {
                    if (param.matrixName != "NULLMATRIX" && dbpRcdindesc.Signalsource != 10) //NULLMATRIX和Never信号源不调矩阵接口
                    {
                        msg = await MatrixSwitch(param);
                    }
                    else
                    {
                        msg = new MatrixOldResponseMessage();
                        msg.nCode = 1;
                        msg.message = "ok";
                    }

                    //sdi的才会更换信息
                    if (msg == null || msg.nCode == 0)
                    {
                        Logger.Error("切换矩阵失败，直接return");
                        if (!await RecoverReleasedRoutAndPort(releasedVirtualPortList, releasedRoutList))//切换失败时，要恢复原来的链接状态
                        {
                            Logger.Error("In module MatrixService!call CIVirtualMatrix::SwitchInOut(),recover the released rout and port failed!");
                        }
                        return false;//切换失败，直接返回
                    }
                    else
                    {
                        if (ApplicationContext.Current.NotifyUdpInfomation)
                        {
                            var loginparam = (await Store.GetAllUserLoginInfos()).ToLookup(t => t.Ip, t => t.Port).ToDictionary(x => x.Key, y => y.First());
                            if (loginparam != null)
                            {
                                Task.Run(() =>
                                {
                                    _clock.InvokeNotify("udp", NotifyPlugin.Udp,
                                   $"<Notify><NotifyType>SwitchMatrix</NotifyType><TaskID>0</TaskID><Inport>{param.lInPort}</Inport><Outport>{param.lOutPort}</Outport></Notify>",
                                   loginparam);
                                });
                            }
                        }
                    }
                }

                Logger.Info($"matrix service return is :{msg.nCode} {msg.message}");
            }
            //离开循环之后，已完成矩阵切换，开始更新路由表
            Logger.Info("switching successfully,so updating relative database..begin to update rout infomation...");

            if (await Store.AddOrUpdateMatrixrout(_mapper.Map<List<Models.DB.DbpMatrixrout>>(routList), false) <= 0)
            {
                Logger.Error("In module MatrixService!call CIVirtualMatrix::SwitchInOut(),update rout information failed");
                if (!await RecoverReleasedRoutAndPort(releasedVirtualPortList, releasedRoutList))
                {
                    Logger.Error("In module MatrixService!call CIVirtualMatrix::SwitchInOut(),recover the released rout and port failed!");
                }
                return false;
            }

            // 更新连接状态表
            if (await Store.UpdatePortInfo(inPort, outPort, 1, true))
            {
                Logger.Info("in module MatrixService!call CIVirtualMatrix::SwitchInOut(),update Port state information successfully");
                return true;
            }
            else
            {
                Logger.Error("in module MatrixService!call CIVirtualMatrix::SwitchInOut(),fail to update Port state information");
                if (!await RecoverReleasedRoutAndPort(releasedVirtualPortList, releasedRoutList))
                {
                    Logger.Error("In module MatrixService!call CIVirtualMatrix::SwitchInOut(),recover the released rout and port failed!");
                }
                return false;
            }
        }


        public async Task<bool> SwitchSignalChannelByAreaAsync(int signalid, int channelid)
        {
            if (signalid < 0)//-1是自动通道，切换不切换已经没有意义了，直接采集
            {
                Logger.Info("SwitchSignalChannelAsync signalid less 0, -1");
                return true;
            }
            var dbpRcdindesc = (await Store.QueryRcdindesc(a => a.Where(b => b.Signalsrcid == signalid), true)).FirstOrDefault();
            var dbpRcdoutdesc = (await Store.QueryRcdoutdesc(a => a.Where(b => b.Channelid == channelid), true)).FirstOrDefault();
            if (dbpRcdindesc != null && dbpRcdoutdesc != null)
            {
                return await SwitchInOutByAreaAsync(dbpRcdindesc.Recinidx, dbpRcdoutdesc.Recoutidx, dbpRcdindesc, dbpRcdoutdesc);
            }

            Logger.Error($"SwitchSignalChannelAsync not find in/out {signalid} {channelid} ");
            return false;
        }

    }
}
