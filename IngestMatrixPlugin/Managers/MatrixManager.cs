using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using IngestDBCore;
using IngestDBCore.Dto;
using IngestDBCore.Interface;
using IngestDBCore.Notify;
using IngestDBCore.Tool;
using IngestMatrixPlugin.Dto.Vo;
using IngestMatrixPlugin.Models.DB;
using IngestMatrixPlugin.Stores;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Sobey.Core.Log;
using MatrixOldResponseMessage = IngestMatrixPlugin.Dto.OldResponse.v1.MatrixOldResponseMessage;

namespace IngestMatrixPlugin.Managers
{
    public class MatrixManager
    {
        public MatrixManager(IMatrixStore store, IMapper mapper, RestClient client, IServiceProvider services, NotifyClock clock)
        {
            _restClient = client;
            Store = store;
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _deviceInterface = new Lazy<IIngestDeviceInterface>(() => services.GetRequiredService<IIngestDeviceInterface>()); ;
            _clock = clock;
        }

        private readonly ILogger Logger = LoggerManager.GetLogger("Matrixinfo");

        private RestClient _restClient { get; }

        /// <summary> 设备（仓储） </summary>
        protected IMatrixStore Store { get; }
        /// <summary> 数据映射器 </summary>
        protected IMapper _mapper { get; }

        private Lazy<IIngestDeviceInterface> _deviceInterface { get; }

        private readonly NotifyClock _clock;

        public async Task<bool> SwitchInOutAsync(long inPort, long outPort)
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
                // 得到矩阵信息
                var matrixinfo = await Store.QueryMatrixinfo(a => a.SingleOrDefaultAsync(x => x.Matrixid == info.lMatrixID), true);
                if (matrixinfo == null)
                {
                    Logger.Error("error module MatrixService!call CIVirtualMatrix::SwitchInOut(),Matrix  Baud,ComPort Is Not Found");

                    if (!await RecoverReleasedRoutAndPort(releasedVirtualPortList, releasedRoutList))
                    {
                        Logger.Error("call CIVirtualMatrix::SwitchInOut(),recover the released rout and port failed!");
                    }
                    return false;
                }
                //matrixinfo.Comportbaud pbaud  pserialport comport

                // 根据矩阵类型ID获得矩阵类型名称
                var strMatrixTypeName = await Store.QueryMatrixtypeinfo(a => a.Where(x => x.Matrixtypeid == matrixinfo.Matrixtypeid)
                                                                           .Select(x => x.Matrixtypename)
                                                                           .SingleOrDefaultAsync(), true);
                Logger.Info($"矩阵类型名称:{strMatrixTypeName}");
                if (string.IsNullOrEmpty(strMatrixTypeName))
                {
                    Logger.Error("In module MatrixService!call CIVirtualMatrix::SwitchInOut(),MatrixType Name %s Not Found");
                    if (!await RecoverReleasedRoutAndPort(releasedVirtualPortList, releasedRoutList))
                    {
                        Logger.Error("In module MatrixService!call CIVirtualMatrix::SwitchInOut(),recover the released rout and port failed!");
                    }
                    return false;
                }
                MatrixParam param = new MatrixParam
                {
                    commPort = matrixinfo.Comport,
                    lInPort = info.lInPort,
                    lOutPort = info.lOutPort,
                    matrixName = strMatrixTypeName,
                    matrixTypeID = matrixinfo.Matrixtypeid,
                };
                //把切换的日志全部打出来
                Logger.Info(JsonConvert.SerializeObject(param));

                //MatrixOldResponseMessage msg = await MatrixSwitch(param);
                var dbpRcdindesc = (await Store.QueryRcdindesc(a => a.Where(x => x.Recinidx == inPort))).FirstOrDefault();
                if (dbpRcdindesc == null)
                {
                    Logger.Error($"call DBAccessMatrixInfo::SwitchInOutByArea(), no inport {inPort} .");
                    if (!await RecoverReleasedRoutAndPort(releasedVirtualPortList, releasedRoutList))
                    {
                        Logger.Error("In module MatrixService!call CIVirtualMatrix::SwitchInOut(),recover the released rout and port failed!");
                    }
                    return false;
                }

                var dbpRcdoutdesc = (await Store.QueryRcdoutdesc(a => a.Where(x => x.Recoutidx == outPort))).FirstOrDefault();
                if (dbpRcdoutdesc == null)
                {
                    Logger.Error($"call DBAccessMatrixInfo::SwitchInOutByArea(), no outport {outPort} .");
                    if (!await RecoverReleasedRoutAndPort(releasedVirtualPortList, releasedRoutList))
                    {
                        Logger.Error("In module MatrixService!call CIVirtualMatrix::SwitchInOut(),recover the released rout and port failed!");
                    }
                    return false;
                }

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
                    msg = await MatrixSwitch(param);
                }

                if (msg == null|| msg.nCode == 0)
                {
                    Logger.Error("切换矩阵失败，直接return");
                    if (!await RecoverReleasedRoutAndPort(releasedVirtualPortList, releasedRoutList))//切换失败时，要恢复原来的链接状态
                    {
                        Logger.Error("In module MatrixService!call CIVirtualMatrix::SwitchInOut(),recover the released rout and port failed!");
                    }
                    return false;//切换失败，直接返回
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

        private async Task<bool> ReleaseWhenSwitch(long inPort, long outPort, List<DbpMatrixrout> matrixRouts)
        {
            var matrixroutList = await Store.QueryMatrixrout(a => a.Where(b => b.Virtualinport == inPort && b.Virtualoutport == outPort), true);

            if (matrixroutList == null || matrixroutList.Count < 1)
            {
                Logger.Error("ReleaseWhenSwitch matrixroutList empty");
            }
            else
                matrixRouts.AddRange(matrixroutList);

            //var matrixList = _mapper.Map<List<MatrixRoutInfo>>(await Store.QueryMatrixrout(a => a, true));
            //matrixRouts.AddRange(matrixList);

            //await Store.DeleteMatrixrout(a => a.Where(x => !(x.Virtualinport == inPort && x.Virtualoutport == outPort)));

            // 更新连接状态表
            await Store.UpdatePortInfo((int)inPort, (int)outPort, 0, true);
            return true;
        }

        public async Task<bool> RecoverReleasedRoutAndPort(List<DbpVirtualmatrixportstate> matrixVirtuals, List<DbpMatrixrout> matrixRouts)
        {
            if (matrixVirtuals.Count == 0 || matrixRouts.Count == 0)
            {
                return true;
            }
            if (await Store.UpdateRangeMatrixrout(matrixRouts, false) <= 0)
            {
                return false;
            }
            if (matrixRouts.Count != 1)
            {
                return false;
            }
            if (await Store.UpdateVirtualmatrixportstate(matrixVirtuals.FirstOrDefault(), true) <= 0)
            {
                return false;
            }
            return true;
        }

        public async Task<int> GetInPortFromOutPortAsync(long outPort)
        {
            try
            {
                var matrixPortState = await Store.QueryVirtualmatrixportstate(a => a.SingleOrDefaultAsync(x => x.Virtualoutport == outPort && x.State == 1));
                if (matrixPortState != null)
                    return matrixPortState.Virtualinport;
                Logger.Error("error module MatrixService!call CIVirtualMatrix::GetInPortFromOutPort(),fail to get inPort connected");
                return -1;
            }
            catch (Exception ex)
            {
                Logger.Error($"catch!error module MatrixService!call CIVirtualMatrix::GetInPortFromOutPort : { ex.Message}");
                return -1;
            }
        }

        public async Task<string> QueryLinkStateAsync()
        {
            try
            {
                var strPortState = "<?xml version=\"1.0\" encoding=\"utf-8\"?>";
                strPortState += "<matrixlinkstate>";
                var stateList = await Store.QueryVirtualmatrixportstate(a => a.Where(x => x.State == 1).OrderBy(x => x.Virtualinport), true);
                int nPreInPort = -1;
                bool bFirst = true;
                foreach (var state in stateList)
                {
                    var Vt_in = Convert.ToInt32(state.Virtualinport);
                    var Vt_out = Convert.ToInt32(state.Virtualoutport);
                    if (nPreInPort != state.Virtualinport)
                    {
                        if (bFirst)
                        {
                            strPortState += "<state>";
                            strPortState += string.Format("<inport>{0}</inport><outport>{1}</outport>", state.Virtualinport, state.Virtualoutport);
                            bFirst = false;
                            nPreInPort = state.Virtualinport;
                            continue;
                        }
                        else
                        {
                            strPortState += "</state><state>";
                            strPortState += string.Format("<inport>{0}</inport><outport>{1}</outport>", state.Virtualinport, state.Virtualoutport);
                            nPreInPort = state.Virtualinport;
                            continue;
                        }
                    }
                    else
                    {
                        strPortState += string.Format("<outport>{0}</outport>", state.Virtualoutport);
                        continue;
                    }
                }
                strPortState += "</state>";
                strPortState += "</matrixlinkstate>";
                Logger.Info("attention in module MatrixService!call CIVirtualMatrix::QueryLinkState(),get matrix link state information successfully");
                return strPortState;
            }
            catch (Exception ex)
            {
                Logger.Error("catch!In IDBAcess!call DBVtMatrixPortState::xmlGetPortState : " + ex.Message);
                return "";
            }
        }

        // 寻找路由
        async Task<List<MatrixRoutInfo>> TryRout(long virtualInPort, long matrixID, long realOutPort, long virtualOutPort)
        {
            List<MatrixRoutInfo> routList = new List<MatrixRoutInfo>();
            var levelList = await Store.QueryLevelrelation(a => a.Where(x => x.Matrixid == matrixID), true);

            if (levelList == null || levelList.Count == 0)
            {

                Logger.Error("in TryCount:length is 0！");
                return routList;
            }
                

            foreach (var info in levelList)
            {
                //构造路由信息
                MatrixRoutInfo RoutInfo = new MatrixRoutInfo()
                {
                    lMatrixID = matrixID,
                    lInPort = info.Inport,
                    lOutPort = realOutPort,
                    lVirtualOutPort = virtualOutPort,
                    lVirtualInPort = virtualInPort,
                    lState = 1
                };

                if (info.Parentmatrixid == -1)  //如果没有父矩阵
                {
                    //查找输入端口映射表,得到输入端口对应的虚拟输入端口
                    var matrixinport = await Store.QueryMapinport(a => a.SingleOrDefaultAsync(x => x.Matrixid == matrixID && x.Inport == info.Inport), true);
                    if (matrixinport != null)
                    {
                        if (matrixinport.Virtualinport == virtualInPort) //如果等于被请求的虚拟输入端口，则路由成功
                        {
                            routList.Add(RoutInfo);
                            return routList;
                        }
                    }
                    else
                        return routList;
                    
                }
                else//如果有父矩阵
                {
                    // 检查当前输入端口是否有信号通过
                    var matrixrout = await Store.QueryMatrixrout(a => a.SingleOrDefaultAsync(x => x.Matrixid == matrixID && x.Inport == info.Inport), true);


                    if (matrixrout == null || virtualInPort == matrixrout.Virtualinport)//如果无信号通过 ,或者 与当前任务的信号相同
                    {
                        routList.Add(RoutInfo);

                        var childrenList = await TryRout(virtualInPort, info.Parentmatrixid, info.Parentoutport, virtualOutPort);
                        if (childrenList != null && childrenList.Count > 0)
                        {
                            routList.AddRange(childrenList);
                            return routList;
                        }
                        else
                        {
                            routList.RemoveAt(routList.Count -1);
                            continue;
                        }
                    }
                }
            }
            return routList;
        }

        async Task<List<MatrixRoutInfo>> TrySimpleRout(long virtualInPort, long matrixID, long realOutPort, long virtualOutPort)
        {
            Logger.Info("enter TrySimpleRout");
            List<MatrixRoutInfo> routList = new List<MatrixRoutInfo>();
            try
            {
                long _lRealInPort = -1, _lCurMatrixID = -1;
                var mapInport = (await Store.QueryMapinport(a => a.Where(x => x.Virtualinport == virtualInPort), true)).FirstOrDefault();
                if(mapInport == null)
                {
                    return routList;
                }

                _lRealInPort = mapInport.Inport;
                _lCurMatrixID = mapInport.Matrixid;

                if (_lCurMatrixID != matrixID)
                {
                    Logger.Error("error MatrixService! MultiRouter is not supported.");
                    return routList;
                }

                MatrixRoutInfo RoutInfo = new MatrixRoutInfo();
                RoutInfo.lMatrixID = matrixID;
                RoutInfo.lInPort = _lRealInPort;
                RoutInfo.lOutPort = realOutPort;
                RoutInfo.lVirtualOutPort = virtualOutPort;
                RoutInfo.lVirtualInPort = virtualInPort;
                RoutInfo.lState = 1;

                routList.Add(RoutInfo);
            }
            catch (Exception ex)
            {
                Logger.Error("catch!error module MatrixService!call CIVirtualMatrix::TrySimpleRout : " + ex.Message);
            }
            return routList;
        }

        /// <summary>
        /// 调用矩阵硬件接口，执行切换操作
        /// </summary>
        /// <returns>标准调用提示信息</returns>
        public async Task<MatrixOldResponseMessage> MatrixSwitch(MatrixParam param)
        {
            string uri = $"{ApplicationContext.Current.IngestMatrixUrl}/api/G2MatrixWebCtrl/MatrixSwitch";
            Logger.Info("matrix service url is:" + uri);

            return await _restClient.Post<MatrixOldResponseMessage>(uri, param);
        }
    }
}
