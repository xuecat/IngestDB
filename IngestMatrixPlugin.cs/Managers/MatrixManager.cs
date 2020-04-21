using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using IngestMatrixPlugin.Dto.Vo;
using IngestMatrixPlugin.Stores;
using Sobey.Core.Log;

namespace IngestMatrixPlugin.Managers
{
    public class MatrixManager
    {
        public MatrixManager(IMatrixStore store, IMapper mapper)
        {
            Store = store;
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        private readonly ILogger Logger = LoggerManager.GetLogger("DeviceInfo");

        List<MatrixRoutInfo> routListComPare = new List<MatrixRoutInfo>();
        List<MatrixRoutInfo> m_ListReleasedRout = new List<MatrixRoutInfo>();
        List<MatrixVirtualPortInfo> m_ListReleasedVirtualPort = new List<MatrixVirtualPortInfo>();

        /// <summary> 设备（仓储） </summary>
        protected IMatrixStore Store { get; }

        /// <summary> 数据映射器 </summary>
        protected IMapper _mapper { get; }




        public async bool SwitchInOutByArea(Int64 lInPort, Int64 lOutPort, Int64 lTimeOut)
        {
            Logger.Debug("***********************SwitchInOutByArea**********************************");
            string strlog = string.Format("Begin to switch in[{0}]-and -out[{1}] Port...", lInPort, lOutPort);
            Logger.Debug(strlog);

            //m_MatrixPortState.InitPortState();
            //string sql = string.Format("SELECT * FROM DBP_VIRTUALMATRIXPORTSTATE WHERE VIRTUALOUTPORT={0} AND STATE=1", lOutPort);
            //DataTable tableTemp = MysqlAccess.GetDataTable(sql);
            //m_ListReleasedVirtualPort.Clear();
            //m_ListReleasedRout.Clear();
            var releasedRoutList = new List<MatrixRoutInfo>();
            var releasedVirtualPortList = new List<MatrixVirtualPortInfo>();

            var infoList = _mapper.Map<List<MatrixVirtualPortInfo>>(await Store.QueryVirtualmatrixportstate(a => a.Where(x => x.Virtualinport == lInPort && x.Virtualoutport == lOutPort && x.State == 1)));
            if (infoList.Count > 0)
            {
                m_ListReleasedVirtualPort.AddRange(infoList);
                foreach (var info in infoList)
                {
                    await ReleaseWhenSwitch(info.lVirtualInPort, lOutPort, releasedRoutList);
                }
            }
            //if (null != tableTemp && tableTemp.Rows.Count != 0)
            //{
            //    foreach (DataRow row in tableTemp.Rows)
            //    {
            //        if (lInPort == Convert.ToInt64(row["VIRTUALINPORT"]) && lOutPort == Convert.ToInt64(row["VIRTUALOUTPORT"]))
            //        {

            //        }
            //        else
            //        {
            //            MatrixVirtualPortInfo info = new MatrixVirtualPortInfo();
            //            info.lVirtualInPort = Convert.ToInt64(row["VIRTUALINPORT"]);
            //            info.lVirtualOutPort = lOutPort;
            //            info.lState = 1;
            //            m_ListReleasedVirtualPort.Add(info);

            //            if (!ReleaseWhenSwitch(info.lVirtualInPort, lOutPort, lTimeOut))
            //            {
            //                LoggerService.WriteMatrixLog(string.Format("call CIVirtualMatrix::SwitchInOut(),fail to release the connection inPort {0} to outPort{1}", info.lVirtualInPort, lOutPort));
            //                LoggerService.WriteMatrixLog("End to switch in-and-out Port...");
            //                return false;
            //            }
            //        }
            //    }
            //}
            else
            {
                Logger.Warn("In module MatrixService!virtualOutport is non-existence with state = 1 in DB,so no need to release in and out ports!,lOutPort");
            }
            #region 得到虚拟输出端口对应的真实矩阵输入端口和矩阵ID
            long lMatrixID = -1;
            long lRealOutPort = -1;
            if (!Store.GetRealMatrixOutPort(lOutPort, ref lRealOutPort, ref lMatrixID))
            {
                if (!await RecoverReleasedRoutAndPort(releasedVirtualPortList, releasedRoutList))
                {
                    Logger.Error("In module MatrixService!call CIVirtualMatrix::SwitchInOut(),recover the released rout and port failed!");
                }
                Logger.Error("End to switch in-and-out Port...");
                return false;
            }
            //得到虚拟输出端口对应的真实矩阵输入端口和矩阵ID

            //LoggerService.WriteMatrixLog("Get the real port from virtual port!! ");
            //if (!MapVtOutPortToReal(lOutPort, ref lRealOutPort, ref lMatrixID))
            //{
            //    if (!RecoverReleasedRoutAndPort())
            //    {
            //        LoggerService.WriteMatrixLog("In module MatrixService!call CIVirtualMatrix::SwitchInOut(),recover the released rout and port failed!");
            //    }
            //    LoggerService.WriteMatrixLog("End to switch in-and-out Port...");
            //    return false;
            //}
            #endregion

            //获取路由表
            m_RouteAndLevelInfo.InitRoutInfo();

            // 清空存储路由信息的链表
            if (!(routList.Count == 0))
                routList.Clear();

            LoggerService.WriteMatrixLog("MapVtOutPortToReal return value is : " + "lRealOutPort " + lRealOutPort + "lMatrixID " + lMatrixID);

            //寻找路由
            if (!TryRout(lInPort, lMatrixID, lRealOutPort, lOutPort))
            {

                LoggerService.WriteMatrixLog(string.Format("In module MatrixService!call CIVirtualMatrix::SwitchInOut(),can not find rout {0} to {1}(tryrout)", lInPort, lOutPort));

                if (RecoverReleasedRoutAndPort())
                {
                    LoggerService.WriteMatrixLog("In module MatrixService!call CIVirtualMatrix::SwitchInOut(),recover the released rout and port failed!");
                }
                return false;
            }
            LoggerService.WriteMatrixLog("TryRout count : " + routList.Count);

            WriteRoutInfo(lInPort, lOutPort);

            //MatrixRoutInfo RoutInfo = new MatrixRoutInfo();
            foreach (MatrixRoutInfo info in routList)
            {
                /*
                 *这中间省略了一大段代码，是串口控制矩阵设备的，将用web Api替换
                */
                MatrixParam param = new MatrixParam();


                string deviceip = "", deviceport = "";

                string insql = string.Format("SELECT SITE,SIGNALSOURCE,IPADDRESS FROM DBP_RCDINDESC WHERE RECINIDX={0}", lInPort);
                DataTable indata = MysqlAccess.GetDataTable(insql);
                if (indata.Rows.Count == 0)
                {
                    if (!RecoverReleasedRoutAndPort())//切换失败时，要恢复原来的链接状态
                    {
                        LoggerService.WriteMatrixLog("In module MatrixService!call CIVirtualMatrix::SwitchInOut(),recover the released rout and port failed!");
                    }
                    LoggerService.WriteMatrixLog(string.Format("call DBAccessMatrixInfo::SwitchInOutByArea(), no inport {0}", lInPort));
                    return false;
                }
                int insite = Convert.ToInt32(indata.Rows[0][0]);

                string outsql = string.Format("SELECT SITE,RCDEVICEID FROM DBP_RCDOUTDESC WHERE RECOUTIDX={0}", lOutPort);
                DataTable outdata = MysqlAccess.GetDataTable(outsql);
                if (outdata.Rows.Count == 0)
                {
                    if (!RecoverReleasedRoutAndPort())//切换失败时，要恢复原来的链接状态
                    {
                        LoggerService.WriteMatrixLog("In module MatrixService!call CIVirtualMatrix::SwitchInOut(),recover the released rout and port failed!");
                    }
                    LoggerService.WriteMatrixLog(string.Format("call DBAccessMatrixInfo::SwitchInOutByArea(), no outport {0}", lOutPort));
                    return false;
                }
                int outsite = Convert.ToInt32(outdata.Rows[0][0]);

                if (outsite == insite)
                {
                    string sitesql = string.Format("SELECT MATRIXTYPEID,MATRIXNAME,COMPORT,DEVICECTRLIP,DEVICECTRLPORT FROM DBP_AREA WHERE ID={0}", insite);
                    DataTable sitedata = MysqlAccess.GetDataTable(sitesql);
                    if (outdata.Rows.Count == 0)
                    {
                        if (!RecoverReleasedRoutAndPort())//切换失败时，要恢复原来的链接状态
                        {
                            LoggerService.WriteMatrixLog("In module MatrixService!call CIVirtualMatrix::SwitchInOut(),recover the released rout and port failed!");
                        }
                        LoggerService.WriteMatrixLog(string.Format("call DBAccessMatrixInfo::SwitchInOutByArea(), no site {0}", insite));
                        return false;
                    }
                    param.matrixTypeID = Convert.ToInt32(sitedata.Rows[0][0]);
                    long type = param.matrixTypeID;
                    param.matrixName = GetMatrixTypeNameFromTypeID(ref type);
                    //param.matrixName = sitedata.Rows[0][1].ToString();
                    param.commPort = Convert.ToInt32(sitedata.Rows[0][2]);
                    param.lInPort = info.lInPort;
                    param.lOutPort = info.lOutPort;
                    deviceip = sitedata.Rows[0][3].ToString();
                    deviceport = sitedata.Rows[0][4].ToString();
                }
                else
                {
                    if (!RecoverReleasedRoutAndPort())//切换失败时，要恢复原来的链接状态
                    {
                        LoggerService.WriteMatrixLog("In module MatrixService!call CIVirtualMatrix::SwitchInOut(),recover the released rout and port failed!");
                    }
                    LoggerService.WriteMatrixLog(string.Format("call DBAccessMatrixInfo::SwitchInOutByArea(), site not equal {0} {1}", insite, outsite));
                    return false;
                }

                //把切换的日志全部打出来
                LoggerService.WriteMatrixLog("begin switch" + JsonConvert.SerializeObject(param));

                ResponseMessage msg = null;
                if (Convert.ToInt32(indata.Rows[0][1]) == 7)
                {
                    insql = string.Format("SELECT IPADDRESS FROM DBP_CAPTUREDEVICE WHERE CPDEVICEID={0}", outdata.Rows[0][1]);
                    DataTable deviceinfo = MysqlAccess.GetDataTable(insql);
                    string msvip = deviceinfo.Rows[0][0].ToString();

                    insql = string.Format("SELECT CHANNELINDEX FROM DBP_CAPTURECHANNELS WHERE CPDEVICEID={0}", outdata.Rows[0][1]);
                    deviceinfo = MysqlAccess.GetDataTable(insql);
                    int msvport = Convert.ToInt32(deviceinfo.Rows[0][0]);


                    if (!DataConfig.CtrlSDK.Relecate(msvip, msvport, indata.Rows[0][2].ToString()))
                    {
                        LoggerService.Error("切换rtmp矩阵失败，直接return");
                        if (!RecoverReleasedRoutAndPort())//切换失败时，要恢复原来的链接状态
                        {
                            LoggerService.WriteMatrixLog("In module MatrixService!call CIVirtualMatrix::SwitchInOut(),recover the released rout and port failed!");
                        }
                        //切换失败，直接返回
                        return false;
                    }

                    msg = new ResponseMessage();
                    msg.nCode = 1;
                    msg.message = "ok";
                }
                else
                {
                    msg = MatrixSwitch(param, deviceip, deviceport);
                }

                if (msg == null)
                {
                    LoggerService.Error("切换矩阵失败，null");
                    if (!RecoverReleasedRoutAndPort())//切换失败时，要恢复原来的链接状态
                    {
                        LoggerService.WriteMatrixLog("In module MatrixService!call CIVirtualMatrix::SwitchInOut(),recover the released rout and port failed!");
                    }
                    return false;
                }

                LoggerService.WriteMatrixLog("matrix service return is :" + msg.nCode.ToString());
                LoggerService.WriteMatrixLog("matrix service return is:" + msg.message);
                if (msg.nCode == 0)
                {
                    LoggerService.Error("切换矩阵失败，直接return");
                    if (!RecoverReleasedRoutAndPort())//切换失败时，要恢复原来的链接状态
                    {
                        LoggerService.WriteMatrixLog("In module MatrixService!call CIVirtualMatrix::SwitchInOut(),recover the released rout and port failed!");
                    }
                    //切换失败，直接返回
                    return false;
                }

            }
            //离开循环之后，已完成矩阵切换，开始更新路由表
            LoggerService.WriteMatrixLog("switching successfully,so updating relative database...");
            LoggerService.WriteMatrixLog("begin to update rout infomation...");
            if (!UpdateDBRout())
            {
                LoggerService.WriteMatrixLog("In module MatrixService!call CIVirtualMatrix::SwitchInOut(),update rout information failed");
                if (!RecoverReleasedRoutAndPort())
                {
                    LoggerService.WriteMatrixLog("In module MatrixService!call CIVirtualMatrix::SwitchInOut(),recover the released rout and port failed!");
                }
                return false;
            }

            // 更新连接状态表
            if (m_MatrixPortState.UpdatePortInfo(lInPort, lOutPort, true, System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")))
            {
                LoggerService.WriteMatrixLog("in module MatrixService!call CIVirtualMatrix::SwitchInOut(),update Port state information successfully");
                return true;
            }
            else
            {
                LoggerService.WriteMatrixLog("in module MatrixService!call CIVirtualMatrix::SwitchInOut(),fail to update Port state information");
                if (!RecoverReleasedRoutAndPort())
                {
                    LoggerService.WriteMatrixLog("In module MatrixService!call CIVirtualMatrix::SwitchInOut(),recover the released rout and port failed!");
                }
                return false;
            }
        }

        /// <summary>得到所有的连接状态记录</summary>
        public bool InitPortState()
        {
            try
            {
                string sql = "SELECT * FROM DBP_VIRTUALMATRIXPORTSTATE";
                m_pPortTable = MysqlAccess.GetDataTable(sql);
                m_bInit = true;

            }
            catch (System.Exception ex)
            {
                LoggerService.WriteMatrixLog("Error call DBVtMatrixPortState.InitPortState : " + ex.Message);
                return false;
            }
            return true;
        }

        private async Task<bool> ReleaseWhenSwitch(long lInPort, long lOutPort, List<MatrixRoutInfo> matrixRouts)
        {

            //try
            //{
            //if (!m_RouteAndLevelInfo.InitRoutInfo())
            //{
            //    LoggerService.WriteMatrixLog("error module MatrixService!call CIVirtualMatrix::ReleaseWhenSwitch(),fail to get rout information");
            //    return false;
            //}

            //DataTable table = m_RouteAndLevelInfo.m_pRoutInfoTable;
            //if (!StructRoutInfoCompare(table))
            //{
            //    return false;
            //}
            var matrixroutList = _mapper.Map<List<MatrixRoutInfo>>(await Store.QueryMatrixroutList(a => a, true));
            matrixroutList.ForEach(a => a.lState = default);
            routListComPare = matrixroutList;

            var matrixList = _mapper.Map<List<MatrixRoutInfo>>(await Store.QueryMatrixroutList(a => a, true));
            //m_ListReleasedRout.AddRange(matrixList);
            matrixRouts.AddRange(matrixList);
            //string sql = string.Format("SELECT * FROM DBP_MATRIXROUT WHERE VIRTUALINPORT = {0} AND VIRTUALOUTPORT = {1}", lInPort, lOutPort);
            //table = MysqlAccess.GetDataTable(sql);
            //if (table.Rows.Count == 0)
            //{
            //    LoggerService.WriteMatrixLog("attention in module MatrixService!call CIVirtualMatrix::ReleaseWhenSwitch() no rout could be released currently");
            //    //return true;
            //}
            ////save the released rout information and virtualport pairs to recover to the old state if switch failed!

            //foreach (DataRow row in table.Rows)
            //{
            //    MatrixRoutInfo info = new MatrixRoutInfo();
            //    info.lInPort = Convert.ToInt64(row["INPORT"]);
            //    info.lOutPort = Convert.ToInt64(row["OUTPORT"]);
            //    info.lMatrixID = Convert.ToInt64(row["MATRIXID"]);
            //    info.lState = Convert.ToInt64(row["STATE"]);
            //    info.lVirtualInPort = lInPort;
            //    info.lVirtualOutPort = lOutPort;

            //    m_ListReleasedRout.Add(info);
            //}

            #region 这段话没有意义啊，折腾半天给nCount这个局部变量赋值，最后又不用
            //foreach (DataRow row in table.Rows)
            //{
            //    long lMatrixID, lRealInPort, lRealOutPort, lMatrixTypeID;

            //    lMatrixID = Convert.ToInt64(row["MATRIXID"]);
            //    lRealInPort = Convert.ToInt64(row["INPORT"]);
            //    lRealOutPort = Convert.ToInt64(row["OUTPORT"]);
            //    int nCount = -1;
            //    if (!CompareRoutInfo(lMatrixID, lRealInPort, lRealOutPort, ref nCount))
            //    {
            //        return false;
            //    }

            //}
            #endregion


            //pRoutInfoRs->Delete(adAffectCurrent); 这句话什么意思

            //更新路由表
            //if (!m_RouteAndLevelInfo.UpdateRoutDB())
            //{
            //    LoggerService.WriteMatrixLog("error module MatrixService!call CIVirtualMatrix::ReleaseWhenSwitch(),fail to update rout information");
            //    return false;
            //}

            await Store.DeleteMatrixrout(a => a.Where(x => !(x.Virtualinport == lInPort && x.Virtualoutport == lOutPort)));

            // 更新连接状态表
            await Store.UpdatePortInfo((int)lInPort, (int)lOutPort, 0);
            //    if (!m_MatrixPortState.UpdatePortInfo(lInPort, lOutPort, false, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")))
            //    {

            //        LoggerService.WriteMatrixLog("error module MatrixService!call CIVirtualMatrix::ReleaseWhenSwitch(),fail to update Port state information ");
            //        return false;
            //    }
            //    else
            //    {
            //        LoggerService.WriteMatrixLog("in module MatrixService!call CIVirtualMatrix::ReleaseWhenSwitch(),update Port state information successfully");
            return true;
            //    }

            //}
            //catch (Exception ex)
            //{

            //    LoggerService.WriteMatrixLog("catch!error module MatrixService!call CIVirtualMatrix::ReleaseWhenSwitch : " + ex.Message);

            //    return false;
            //}
        }

        public async Task<bool> RecoverReleasedRoutAndPort(List<MatrixVirtualPortInfo> matrixVirtuals, List<MatrixRoutInfo> matrixRouts)
        {
            if (matrixVirtuals.Count == 0 || matrixRouts.Count == 0)
            //if (m_ListReleasedVirtualPort.Count == 0 || m_ListReleasedRout.Count == 0)
            {
                return true;
            }
            if (await Store.AddRangeMatrixrout(_mapper.Map<List<Models.DB.DbpMatrixrout>>(matrixVirtuals)) <= 0)
            {
                return false;
            }
            if (matrixRouts.Count != 1)
            {
                return false;
            }
            if (await Store.AddVirtualmatrixportstate(_mapper.Map<Models.DB.DbpVirtualmatrixportstate>(matrixRouts.Single())) <= 0)
            {
                return false;
            }
            return true;
        }

        // 寻找路由
        async Task<bool> TryRout(long lVirtualInPort, long lMatrixID, long lRealOutPort, long lVirtualOutPort)
        {
            //LoggerService.WriteMatrixLog("ehter TryRout");
            try
            {
                List<LevelInfo> levelList = _mapper.Map<List<LevelInfo>>(await Store.QueryLevelrelation(a => a.Where(x => x.Matrixid == lMatrixID),true));
                //StructLevelInfo(levelList, lMatrixID);
                if (levelList == null || levelList.Count == 0)
                    Logger.Error("in TryCount：length is 0！");


                foreach (LevelInfo info in levelList)
                {
                    long _lParentID, _lRealInPort, _lParentOutPort;
                    _lParentID = info.lParentMatrixID;
                    _lRealInPort = info.lRealInPort;
                    _lParentOutPort = info.lRealParentOutPort;

                    //构造路由信息
                    //MATRIXROUTINFO RoutInfo;
                    MatrixRoutInfo RoutInfo = new MatrixRoutInfo();
                    RoutInfo.lMatrixID = lMatrixID;
                    RoutInfo.lInPort = _lRealInPort;
                    RoutInfo.lOutPort = lRealOutPort;
                    RoutInfo.lVirtualOutPort = lVirtualOutPort;
                    RoutInfo.lVirtualInPort = lVirtualInPort;
                    RoutInfo.lState = 1;

                    if (_lParentID == -1)  //如果没有父矩阵
                    {
                        //查找输入端口映射表,得到输入端口对应的虚拟输入端口
                        long _lVirtualInPort = -1;
                        if (!m_PortMap.GetVirtualInPort(lMatrixID, _lRealInPort, ref _lVirtualInPort))
                        {
                            return false;
                        }
                        if (_lVirtualInPort == lVirtualInPort) //如果等于被请求的虚拟输入端口，则路由成功
                        {
                            routList.Add(RoutInfo);
                            return true;
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else//如果有父矩阵
                    {
                        // 检查当前输入端口是否有信号通过
                        bool bpTag = false;
                        if (!m_RouteAndLevelInfo.IsHasSignal(lMatrixID, _lRealInPort, ref bpTag))
                        {
                            return false;
                        }
                        if (bpTag == false) //如果无信号通过
                        {
                            routList.Add(RoutInfo);
                            m_RouteAndLevelInfo.InitRoutInfo(); //还原table信息
                            if (TryRout(lVirtualInPort, _lParentID, _lParentOutPort, lVirtualOutPort) == true)
                                return true;
                            else
                            {
                                routList.RemoveAt(routList.Count - 1);
                                continue;
                            }
                        }
                        else
                        {                       //注意：这里的table中难道只有一条数据么?就直接取了，没有任何的筛选条件，
                            long _lVirtualOutPort = Convert.ToInt64(m_RouteAndLevelInfo.m_pRoutInfoTable.Rows[0]["VIRTUALINPORT"]); //(m_RouteAndLevelInfo.m_pRoutInfoRs)->GetCollect(L"VIRTUALINPORT");
                            if (lVirtualInPort == _lVirtualOutPort)
                            {
                                routList.Add(RoutInfo);
                                m_RouteAndLevelInfo.InitRoutInfo();
                                if (TryRout(lVirtualInPort, _lParentID, _lParentOutPort, lVirtualOutPort) == true)
                                    return true;
                                else
                                {
                                    routList.RemoveAt(routList.Count - 1);
                                    continue;
                                }
                            }
                            else  //如果与当前任务的信号不同，则此输入端口被占用
                            {
                                m_RouteAndLevelInfo.InitRoutInfo();
                                continue;
                            }
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                LoggerService.WriteMatrixLog("catch!error module MatrixService!call CIVirtualMatrix::TryRout : " + ex.Message);
                return false;
            }
        }
    }
}
