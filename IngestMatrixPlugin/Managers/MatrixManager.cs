using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using IngestMatrixPlugin.Dto.Response;
using IngestMatrixPlugin.Dto.Vo;
using IngestMatrixPlugin.Stores;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
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

        private readonly ILogger Logger = LoggerManager.GetLogger("MatrixStoreManager");

        List<MatrixRoutInfo> routListComPare = new List<MatrixRoutInfo>();
        List<MatrixRoutInfo> m_ListReleasedRout = new List<MatrixRoutInfo>();
        List<MatrixVirtualPortInfo> m_ListReleasedVirtualPort = new List<MatrixVirtualPortInfo>();

        /// <summary> 设备（仓储） </summary>
        protected IMatrixStore Store { get; }

        /// <summary> 数据映射器 </summary>
        protected IMapper _mapper { get; }

        public async Task<bool> SwitchInOutAsync(long lInPort, long lOutPort)
        {
            Logger.Info("**********************************************************************");
            string strlog = string.Format("Begin to switch in[{0}]-and -out[{1}] Port...", lInPort, lOutPort);
            Logger.Info(strlog);

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
            #endregion

            #region 获取路由表
            var routList = await TryRout(lInPort, lMatrixID, lRealOutPort, lOutPort);
            if (routList.Count <= 0)
            {
                Logger.Warn($"In module MatrixService!call CIVirtualMatrix::SwitchInOut(),can not find rout {lInPort} to {lOutPort}(tryrout)");

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
                // 根据矩阵类型ID获得矩阵类型名称
                Logger.Info("Get the matrix name from matrix type id!! ");
                var strMatrixTypeName = await Store.QueryMatrixtypeinfo(a => a.Where(x => x.Matrixtypeid == matrixinfo.Matrixtypeid)
                                                                           .Select(x => x.Matrixtypename)
                                                                           .SingleOrDefaultAsync(), true);
                Logger.Info($"矩阵类型名称：{strMatrixTypeName}");
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

                ResponseMessage msg = MatrixSwitch(param);

                Logger.Info($"matrix service return is :{msg.nCode}");
                Logger.Info($"matrix service return is :{msg.message}");
                if (msg.nCode == 0)
                {
                    Logger.Error("切换矩阵失败，直接return");
                    if (!await RecoverReleasedRoutAndPort(releasedVirtualPortList, releasedRoutList))//切换失败时，要恢复原来的链接状态
                    {
                        Logger.Error("In module MatrixService!call CIVirtualMatrix::SwitchInOut(),recover the released rout and port failed!");
                    }
                    return false;//切换失败，直接返回
                }
            }
            //离开循环之后，已完成矩阵切换，开始更新路由表
            Logger.Info("switching successfully,so updating relative database...");
            Logger.Info("begin to update rout infomation...");

            if (await Store.AddOrUpdateMatrixrout(_mapper.Map<List<Models.DB.DbpMatrixrout>>(routList)) <= 0)
            {
                Logger.Error("In module MatrixService!call CIVirtualMatrix::SwitchInOut(),update rout information failed");
                if (!await RecoverReleasedRoutAndPort(releasedVirtualPortList, releasedRoutList))
                {
                    Logger.Error("In module MatrixService!call CIVirtualMatrix::SwitchInOut(),recover the released rout and port failed!");
                }
                return false;
            }

            // 更新连接状态表
            if (await Store.UpdatePortInfo(lInPort, lOutPort, 1))
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

        private async Task<bool> ReleaseWhenSwitch(long lInPort, long lOutPort, List<MatrixRoutInfo> matrixRouts)
        {
            var matrixroutList = _mapper.Map<List<MatrixRoutInfo>>(await Store.QueryMatrixrout(a => a, true));
            matrixroutList.ForEach(a => a.lState = default);
            routListComPare = matrixroutList;

            var matrixList = _mapper.Map<List<MatrixRoutInfo>>(await Store.QueryMatrixrout(a => a, true));
            matrixRouts.AddRange(matrixList);

            await Store.DeleteMatrixrout(a => a.Where(x => !(x.Virtualinport == lInPort && x.Virtualoutport == lOutPort)));

            // 更新连接状态表
            await Store.UpdatePortInfo((int)lInPort, (int)lOutPort, 0);
            return true;
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

        public async Task<int> GetInPortFromOutPortAsync(long OutPort)
        {
            try
            {
                var matrixPortState = await Store.QueryVirtualmatrixportstate(a => a.SingleOrDefaultAsync(x => x.Virtualoutport == OutPort && x.State == 1));
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
        async Task<List<MatrixRoutInfo>> TryRout(long lVirtualInPort, long lMatrixID, long lRealOutPort, long lVirtualOutPort)
        {
            List<MatrixRoutInfo> routList = new List<MatrixRoutInfo>();
            List<LevelInfo> levelList = _mapper.Map<List<LevelInfo>>(await Store.QueryLevelrelation(a => a.Where(x => x.Matrixid == lMatrixID), true));
            if (levelList == null || levelList.Count == 0)
                Logger.Error("in TryCount：length is 0！");

            foreach (LevelInfo info in levelList)
            {
                //构造路由信息
                MatrixRoutInfo RoutInfo = new MatrixRoutInfo()
                {
                    lMatrixID = lMatrixID,
                    lInPort = info.lRealInPort,
                    lOutPort = lRealOutPort,
                    lVirtualOutPort = lVirtualOutPort,
                    lVirtualInPort = lVirtualInPort,
                    lState = 1
                };

                if (info.lParentMatrixID == -1)  //如果没有父矩阵
                {
                    //查找输入端口映射表,得到输入端口对应的虚拟输入端口
                    var matrixinport = await Store.QueryMapinport(a => a.SingleOrDefaultAsync(x => x.Matrixid == lMatrixID && x.Inport == info.lRealInPort), true);
                    if (matrixinport != null)
                    {
                        return routList;
                    }
                    if (matrixinport.Virtualinport == lVirtualInPort) //如果等于被请求的虚拟输入端口，则路由成功
                    {
                        routList.Add(RoutInfo);
                        return routList;
                    }
                }
                else//如果有父矩阵
                {
                    // 检查当前输入端口是否有信号通过
                    var matrixrout = await Store.QueryMatrixrout(a => a.SingleOrDefaultAsync(x => x.Matrixid == lMatrixID && x.Inport == info.lRealInPort), true);
                    if (matrixrout == null || lVirtualInPort == matrixrout.Virtualinport)//如果无信号通过 ,或者 与当前任务的信号相同
                    {
                        var childrenList = await TryRout(lVirtualInPort, info.lParentMatrixID, info.lRealParentOutPort, lVirtualOutPort);
                        if (childrenList != null && childrenList.Count > 0)
                        {
                            routList.Add(RoutInfo);
                            routList.AddRange(childrenList);
                            return routList;
                        }
                    }
                }
            }
            return routList;
        }

        /// <summary>
        /// 调用矩阵硬件接口，执行切换操作
        /// </summary>
        /// <returns>标准调用提示信息</returns>
        public ResponseMessage MatrixSwitch(MatrixParam param)
        {
            //try
            //{
            Logger.Info("~~~~~~~~~~~~~~开始调用郭文的接口~~~~~~~~~");
            string ip = DataConfig.IngestMatrixServerIP;
            string port = DataConfig.IngestMatrixServerPort;

            string uri = string.Format("http://{0}:{1}/api/G2MatrixWebCtrl/MatrixSwitch", ip, port);
            Logger.Info("matrix service url is:" + uri);

            StringContent content = new StringContent(JsonConvert.SerializeObject(param));
            content.Headers.ContentType.MediaType = "application/json";

            HttpClient client = new HttpClient();
            string objstr = client.PostAsync(uri, content).Result.Content.ReadAsStringAsync().Result;

            ResponseMessage msg = JsonConvert.DeserializeObject<ResponseMessage>(objstr);
            return msg;
            //}
            //catch (Exception ex)
            //{
            //    Logger.Error($"调用过问的矩阵切换接口发生异常： {ex.ToString()}" + );
            //    ResponseMessage msg = new ResponseMessage();
            //    msg.nCode = 0;
            //    msg.message = ex.ToString();
            //    return msg;
            //}
        }
    }
}
