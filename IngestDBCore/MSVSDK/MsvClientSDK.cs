using Sobey.Core.Log;
using System;
using System.Collections;

namespace IngestDBCore.MSVSDK
{
    public class MsvClientCtrlSDK
    {

        public static readonly ILogger Logger = LoggerManager.GetLogger("MsvClientCtrlSDK");

        private Hashtable htMsvlst { get; set; }
        public MsvClientCtrlSDK()
        {
            htMsvlst = null;
        }

        public CClientTaskSDK GetMsvCtrlInstance(int nChPort, string strMsvIP)
        {
            CClientTaskSDK msvCtrlSDK = null;
            if (nChPort > 0 && strMsvIP != "")
            {
                if (htMsvlst == null)
                {
                    htMsvlst = new Hashtable();
                }
                string strkey = strMsvIP;
                strkey = strkey + "," + nChPort.ToString();
                if (htMsvlst.ContainsKey(strkey))
                {
                    msvCtrlSDK = (CClientTaskSDK)htMsvlst[strkey];
                }
                else
                {
                    msvCtrlSDK = new CClientTaskSDK(strMsvIP, nChPort, 5000);
                    htMsvlst.Add(strkey, msvCtrlSDK);
                }
            }
            return msvCtrlSDK;
        }
        //public void ClientParam2MSVTskParam(TaskParam tmptaskparam, ref TASK_PARAM pTaskparam)
        //{
        //    if (tmptaskparam != null)
        //    {
        //        pTaskparam.ulID = tmptaskparam.taskID;
        //        pTaskparam.nInOutCount = tmptaskparam.inOutCount;
        //        pTaskparam.strName = tmptaskparam.taskName;
        //        if (pTaskparam.nInOutCount > 0)
        //        {
        //            pTaskparam.dwInFrame = new ulong[100];
        //            pTaskparam.dwOutFrame = new ulong[100];
        //            pTaskparam.dwTotalFrame = new ulong[100];
        //            for (int i = 0; i < pTaskparam.nInOutCount; i++)
        //            {
        //                pTaskparam.dwInFrame[i] = Convert.ToUInt64(tmptaskparam.inFrame[i]);
        //                pTaskparam.dwOutFrame[i] = Convert.ToUInt64(tmptaskparam.outFrame[i]);
        //                pTaskparam.dwTotalFrame[i] = Convert.ToUInt64(tmptaskparam.totalFrame[i]);
        //            }

        //            //pTaskparam.bUseTime = 0;
        //            pTaskparam.bUseTime = false;
        //        }
        //        else
        //        {
        //            // Add by chenzhi 2012-01-14
        //            // TODO: 增加对计划任务采集的支持
        //            //if (tmptaskparam.isPlanMode)
        //            //{
        //            //    pTaskparam.bUseTime = 2;
        //            //}
        //            //else
        //            //{
        //            //    pTaskparam.bUseTime = 1;
        //            //}
        //            pTaskparam.bUseTime = true;
        //        }

        //    }

        //}
        //public void MSVTskParam2ClientParam(TASK_PARAM pTaskparam, ref TaskParam tmptaskparam)
        //{
        //    tmptaskparam.taskID = Convert.ToInt32(pTaskparam.ulID);
        //    tmptaskparam.inOutCount = pTaskparam.nInOutCount;
        //    tmptaskparam.cutLen = 0;
        //    tmptaskparam.isRetro = pTaskparam.bRetrospect;
        //    //byte[] y = new byte[4096];
        //    //y = pTaskparam.TaskName.Cast<byte>().ToArray();
        //    tmptaskparam.taskName = pTaskparam.strName;
        //    int nlen = 0;
        //    nlen = pTaskparam.dwInFrame.Length;
        //    tmptaskparam.inOutCount = nlen;
        //    tmptaskparam.tmBeg = pTaskparam.tmBeg;
        //    tmptaskparam.inFrame = new int[100];
        //    tmptaskparam.outFrame = new int[100];
        //    tmptaskparam.totalFrame = new int[100];
        //    for (int i = 0; i < 100; i++)
        //    {
        //        tmptaskparam.inFrame[i] = Convert.ToInt32(pTaskparam.dwInFrame[i]);
        //        tmptaskparam.outFrame[i] = Convert.ToInt32(pTaskparam.dwOutFrame[i]);
        //        tmptaskparam.totalFrame[i] = Convert.ToInt32(pTaskparam.dwTotalFrame[i]);
        //    }
        //    tmptaskparam.retroFrame = 0;
        //    tmptaskparam.isPlanMode = true;
        //    tmptaskparam.tmBeg = pTaskparam.tmBeg;
        //}
        //public bool QuerySDIFormat(int nChPort, string strMsvIP, out int pnSingleType)
        //{
        //    SDISignalStatus singleType = new SDISignalStatus();
        //    bool bIsBack = false;
        //    pnSingleType = 1;
        //    MSV_RET ret;
        //    try
        //    {
        //        CClientTaskSDK SDK = GetMsvCtrlInstance(nChPort, strMsvIP);
        //        ret = SDK.MSVQuerySDIFormat(strMsvIP, ref singleType, ref bIsBack, nChPort);
        //        if (ret == MSV_RET.MSV_NETERROR)
        //        {
        //            LoggerService.Info(  "Cast Interface Function MSVQuerySDIFormat Error!(error = {0})...........MsvUdpClientCtrlSDK::QuerySDIFormat", SDK.MSVGetLastErrorString());
        //            return false;
        //        }
        //        if (ret != MSV_RET.MSV_SUCCESS)
        //        {
        //            LoggerService.Info(  "Cast Interface Function MSVQuerySDIFormat Error!(error = {0})...........MsvUdpClientCtrlSDK::QuerySDIFormat", SDK.MSVGetLastErrorString());
        //            return false;
        //        }

        //        LoggerService.Info(  "Cast Interface Function QuerySDIFormat!(vedioformat ={0} :width :{1})...........MsvUdpClientCtrlSDK::QuerySDIFormat", singleType.VideoFormat, singleType.nWidth);

        //        bool bValidVideo = false;
        //        switch (singleType.VideoFormat)
        //        {

        //            case SignalFormat._invalid_vid_format:
        //                pnSingleType = 254;
        //                break;
        //            //case 4095:
        //            //	{
        //            //		bValidVideo = TRUE;	
        //            //	}
        //            case SignalFormat._unknown_vid_format:
        //                pnSingleType = 255;
        //                break;
        //            default:
        //                bValidVideo = true;
        //                break;
        //        }
        //        if (bValidVideo)
        //        {
        //            if (singleType.nWidth > 0 && singleType.nWidth <= 720)
        //            {
        //                pnSingleType = 0;
        //            }
        //            else if (singleType.nWidth > 720)
        //            {
        //                pnSingleType = 1;
        //            }
        //            else
        //            {
        //                pnSingleType = 254;
        //                bValidVideo = false;
        //            }


        //        }
        //        return bValidVideo;
        //    }
        //    catch (System.Exception e)
        //    {
        //        LoggerService.Info(  "Cast Interface Function MSVQuerySDIFormat Exception!(exception = {0})...........MsvUdpClientCtrlSDK::QuerySDIFormat", e.Message);
        //        pnSingleType = 254;
        //        return false;
        //    }
        //}

        //public bool Record(int nChPort, string strMsvIP, out int nMsvRet)
        //{
        //    LoggerService.Info(  "MsvSDK Record Ready!, taskID:{0}...........Record", m_taskParam.ulID);
        //    MSV_RET ret;
        //    string strMutPath = "";
        //    string strPath = "";
        //    nMsvRet = 0;
        //    try
        //    {
        //        CClientTaskSDK SDK = GetMsvCtrlInstance(nChPort, strMsvIP);

        //        _xml.LoadXml(m_strCaptureParam);
        //        XmlElement _root = _xml.DocumentElement;
        //        XmlNode pathNode = _root.SelectSingleNode("multiDest");
        //        if (pathNode != null)
        //        {
        //            strPath = pathNode.InnerText;
        //            strMutPath = string.Format("<multiDest><taskid>{0}</taskid>{1}</multiDest>", m_taskParam.ulID, strPath);
        //            ret = SDK.MSVSetMulDestPath(strMsvIP, strMutPath);
        //            if (ret == MSV_RET.MSV_NETERROR)
        //            {
        //                LoggerService.Info(  "MSVSetMulDestPath::taskName={0};Error:{1}!...........MsvUdpClientCtrlSDK::Record", m_taskParam.strName, SDK.MSVGetLastErrorString());
        //            }
        //        }

        //        m_taskAllParam.nCutLen = 10;
        //        LoggerService.Info(  "MsvSDK Record Prepare Cast MSVStartTaskNew Function ip={0} port={1} cutlen={2}!...........MsvUdpClientCtrlSDK::Record", strMsvIP, nChPort, m_taskAllParam.nCutLen);
        //        ret = SDK.MSVStartTaskNew(strMsvIP, m_taskAllParam, nChPort);
        //        LoggerService.Info(  "MsvSDK Record Cast MSVStartTaskNew End!...........MsvUdpClientCtrlSDK::Record");
        //        nMsvRet = Convert.ToInt32(ret);
        //        if (ret == MSV_RET.MSV_NETERROR)
        //        {
        //            LoggerService.Info(  "MsvSDK Record Failed(MSV_NETERROR)!...........MsvUdpClientCtrlSDK::Record");
        //            return false;
        //        }
        //        if (ret != MSV_RET.MSV_SUCCESS)
        //        {
        //            LoggerService.Info(  "MsvSDK Record Failed(ret = {0})...........MsvUdpClientCtrlSDK::Record", Convert.ToInt32(ret));
        //            LoggerService.Info(  "Cast Interface Function MSVStartTaskNew Error!(error = {0})...........CMsvCtrlCom::Record", SDK.MSVGetLastErrorString());
        //            return false;
        //        }
        //        else if (ret == MSV_RET.MSV_SUCCESS)
        //            LoggerService.Info(  "MsvSDK Record End!...........MsvUdpClientCtrlSDK::Record");
        //    }
        //    catch (Exception e)
        //    {
        //        LoggerService.Info(  "MsvUdpClientCtrlSDK::Record, Exception:{0}", e.Message);
        //        return false;
        //    }
        //    return true;
        //}

        //public bool RecordReady(int nChPort, string strMsvIP, ref TaskParam pTaskparam, string strTaskName, string pCaptureparam)
        //{
        //    if (pTaskparam == null)
        //    {
        //        LoggerService.Info(  "RecordReady: pTaskparam is null!");
        //        return false;
        //    }
        //    TASK_PARAM param = new TASK_PARAM();
        //    try
        //    {
        //        LoggerService.Info(  "MsvSDK Record Ready!,taskID:{0},pCaptureparam:{1} ", pTaskparam.taskID, pCaptureparam);
        //        XmlDocument doc = new XmlDocument();
        //        doc.LoadXml(pCaptureparam);
        //        XmlNode root = doc.DocumentElement;
        //        //string curDate = DateTime.Now.ToString("yyyy-MM-dd") + '\\';
        //        string curDate = DateTime.Now.ToString("yyyy-MM-dd");
        //        if (root != null)
        //        {
        //            //XmlNode _capTure = root.SelectSingleNode("CAPTUREPARAM"); 

        //            XmlNode _fileName0 = root.SelectSingleNode("path0FileName");
        //            if (_fileName0 != null)
        //            {
        //                string fileName0 = _fileName0.InnerText;
        //                //fileName0 += curDate;
        //                //                         int nIndex = fileName0.LastIndexOf('\\');
        //                //                         if (nIndex == fileName0.Length - 1)
        //                //                             fileName0 = fileName0.Substring(0, nIndex);
        //                //                         int nPos = fileName0.LastIndexOf('\\');
        //                //                         fileName0 = fileName0.Insert(nPos + 1, curDate);
        //                //fileName0 = fileName0.Substring(0, nPos + 1);
        //                //fileName0 = fileName0 + curDate;
        //                fileName0 = fileName0.Replace("yyyy-mm-dd", curDate);
        //                _fileName0.InnerText = fileName0;

        //            }
        //            else
        //            {
        //                LoggerService.Info(  "Not find fileName0:");
        //            }
        //            XmlNode _fileName1 = root.SelectSingleNode("path1FileName");
        //            if (_fileName1 != null)
        //            {
        //                string fileName1 = _fileName1.InnerText;
        //                //fileName1 += curDate;
        //                //                         int nIndex = fileName1.LastIndexOf('\\');
        //                //                         if (nIndex == fileName1.Length - 1)
        //                //                             fileName1 = fileName1.Substring(0, nIndex);
        //                //                         int nPos = fileName1.LastIndexOf('\\');
        //                //                         fileName1 = fileName1.Insert(nPos + 1, curDate);
        //                //fileName1 = fileName1.Substring(0, nPos + 1);
        //                //fileName1 = fileName1 + curDate;
        //                fileName1 = fileName1.Replace("yyyy-mm-dd", curDate);
        //                _fileName1.InnerText = fileName1;
        //            }
        //            else
        //            {
        //                LoggerService.Info(  "Not find fileName1:");
        //            }
        //        }
        //        else
        //        {
        //            LoggerService.Info(  "root is null");
        //        }
        //        pCaptureparam = doc.InnerXml;
        //        LoggerService.Info(  "MsvSDK Record Ready!, taskID:{0}, lastCapture:{1}...........RecordReady", pTaskparam.taskID, pCaptureparam);
        //        ClientParam2MSVTskParam(pTaskparam, ref param);
        //        m_taskParam.bRetrospect = param.bRetrospect;
        //        m_taskParam.bUseTime = param.bUseTime;
        //        m_taskParam.channel = param.channel;
        //        m_taskParam.dwCutClipFrame = param.dwCutClipFrame;
        //        m_taskParam.nInOutCount = param.nInOutCount;
        //        m_taskParam.strName = param.strName;
        //        for (int i = 0; i < m_taskParam.nInOutCount; i++)
        //        {
        //            m_taskParam.dwInFrame[i] = param.dwInFrame[i];
        //            m_taskParam.dwOutFrame[i] = param.dwOutFrame[i];
        //            m_taskParam.dwTotalFrame[i] = param.dwTotalFrame[i];
        //        }


        //        m_taskParam.nSignalID = param.nSignalID;
        //        m_taskParam.nTimeCode = param.nTimeCode;
        //        //m_taskparam.strDesc.Format(_T("%s"),pTaskparam->strDesc);
        //        //m_taskParam.strName = strTaskName;

        //        m_taskParam.tmBeg = DateTime.Now;
        //        m_taskParam.tmEnd = DateTime.Now;
        //        m_taskParam.ulID = param.ulID;
        //        m_taskParam.TaskMode = (TASK_MODE)param.TaskMode;
        //        m_taskParam.TaskState = (TASK_STATE)param.TaskState;


        //        m_taskAllParam.captureParam = pCaptureparam;
        //        m_taskAllParam.nCutLen = 10;
        //        m_taskAllParam.taskParam = m_taskParam;
        //        //m_taskAllParam.taskParam.strName = strTaskName;
        //        m_strCaptureParam = "";
        //        m_strCaptureParam = pCaptureparam;
        //        LoggerService.Info(  "curent strTaskName:" + strTaskName);
        //    }
        //    catch (Exception e)
        //    {
        //        LoggerService.Info(  "MsvUdpClientCtrlSDK::RecordReady, Exception:{0}", e.Message);
        //        return false;
        //    }
        //    return true;
        //}

        //public bool QueryTaskState(int nChPort, string strMsvIP, out TaskParam tskparam)
        //{
        //    MSV_RET ret;
        //    TASK_PARAM info = new TASK_PARAM();
        //    tskparam = new TaskParam();
        //    LoggerService.Info(  "MsvSDK prepare QueryTaskState(ip={0})!...........MsvUdpClientCtrlSDK::QueryTaskState", strMsvIP);
        //    try
        //    {
        //        CClientTaskSDK SDK = GetMsvCtrlInstance(nChPort, strMsvIP);
        //        if (SDK == null)
        //        {
        //            LoggerService.Info(  "GetMsvCtrlInstance(SDK==null).......MsvUdpClientCtrlSDK::QueryTaskState");
        //            return false;
        //        }
        //        ret = SDK.MSVQueryRuningTask(strMsvIP, ref info, nChPort);
        //        LoggerService.Info(  "MsvSDK End MSVQueryRuningTask!...........MsvUdpClientCtrlSDK::QueryTaskState");

        //        if (ret == MSV_RET.MSV_NETERROR)
        //        {
        //            LoggerService.Info(  "Cast Interface Function MSVQueryRuningTask Error!(error = {0})...........MsvUdpClientCtrlSDK::QueryTaskState", SDK.MSVGetLastErrorString());
        //            return false;
        //        }
        //        if (ret != MSV_RET.MSV_SUCCESS)
        //        {
        //            LoggerService.Info(  "Cast Interface Function MSVQueryRuningTask Error!(error = {0})...........MsvUdpClientCtrlSDK::QueryTaskState", SDK.MSVGetLastErrorString());
        //            return false;
        //        }
        //        //string strbtime = info.tmBeg.ToString("yy-MM-dd hh:mm:ss");
        //        //string stretime = info.tmBeg.ToString("yy-MM-dd hh:mm:ss");
        //        //info.tmBeg = Convert.ToDateTime(strbtime);
        //        //info.tmEnd = Convert.ToDateTime(stretime);
        //        MSVTskParam2ClientParam(info, ref tskparam);
        //    }
        //    catch (Exception e)
        //    {
        //        LoggerService.Info(  "MsvUdpClientCtrlSDK::QueryTaskState, Exception:{0}", e.Message);
        //        return false;
        //    }
        //    return true;
        //}
        public bool QueryState(int nChPort, string strMsvIP, out int ds)
        {
            MSV_STATE state = new MSV_STATE();
            MSV_RET ret;
            ds = 1;
            try
            {
                CClientTaskSDK SDK = GetMsvCtrlInstance(nChPort, strMsvIP);
                if (SDK == null)
                {
                    Logger.Info("GetMsvCtrlInstance(SDK == null) .....MsvUdpClientCtrlSDK::QueryState");
                }
                Logger.Info($"prepare to call MSVQueryState.....MsvUdpClientCtrlSDK::QueryState nChPort:{nChPort},strMsvIP:{strMsvIP}");
                ret = SDK.MSVQueryState(strMsvIP, ref state, nChPort);
                if (ret == MSV_RET.MSV_NETERROR)
                {
                    Logger.Info("MSVQueryState MSV_NETERROR, {0}:{1}, error:{2}......MsvUdpClientCtrlSDK::QueryState", strMsvIP, nChPort, SDK.MSVGetLastErrorString());
                    return false;
                }
                if (ret != MSV_RET.MSV_SUCCESS)
                {
                    Logger.Info("MSVQueryState failed, {0}:{1}, error:{2}......MsvUdpClientCtrlSDK::QueryState", strMsvIP, nChPort, SDK.MSVGetLastErrorString());
                    return false;
                }
                if (state.msv_capture_state == CAPTURE_STATE.CS_PAUSE)
                    ds = 2;
                else if (state.msv_capture_state == CAPTURE_STATE.CS_CAPTURE)
                    ds = 0;
                else
                    ds = 1;

                Logger.Info("MSVQueryState End, state:{0}......MsvUdpClientCtrlSDK::QueryState", ds);
            }
            catch (Exception e)
            {
                Logger.Info("MsvUdpClientCtrlSDK::QueryState, Exception:{0}", e.Message);
                return false;
            }
            return true;
        }

        //public bool GetLastErrorInfo(int nChPort, string strMsvIP, out _ErrorInfo pErrorInfo)
        //{
        //    pErrorInfo = new _ErrorInfo();
        //    try
        //    {
        //        CClientTaskSDK SDK = GetMsvCtrlInstance(nChPort, strMsvIP);
        //        if (SDK == null)
        //        {
        //            LoggerService.Info(  "GetMsvCtrlInstance(SDK=NULL)......MsvUdpClientCtrlSDK::GetLastErrorInfo");
        //            return false;
        //        }
        //        pErrorInfo.errStr = SDK.MSVGetLastErrorString();
        //    }
        //    catch (Exception e)
        //    {
        //        LoggerService.Info(  "MsvUdpClientCtrlSDK::GetLastErrorInfo, Exception:{0}", e.Message);
        //        return false;
        //    }
        //    return true;
        //}

        //public bool Stop(int nChPort, string strMsvIP, int taskID, out int nMsvStopRet)
        //{
        //    MSV_RET ret;
        //    nMsvStopRet = 0;
        //    try
        //    {
        //        CClientTaskSDK SDK = GetMsvCtrlInstance(nChPort, strMsvIP);
        //        if (SDK == null)
        //        {
        //            LoggerService.Info(  "GetMsvCtrlInstance(SDK=NULL)......MsvUdpClientCtrlSDK::Stop");
        //            return false;
        //        }
        //        LoggerService.Info(  "prepare to MSVStopTask......MsvUdpClientCtrlSDK::Stop");
        //        long nRetTaskId = -1;
        //        ret = SDK.MSVStopTask(strMsvIP, ref nRetTaskId, taskID, nChPort);
        //        nMsvStopRet = Convert.ToInt32(ret);
        //        if (ret != MSV_RET.MSV_SUCCESS)
        //        {
        //            LoggerService.Info(  "MSVStopTask failed, error:{0} .......MsvUdpClientCtrlSDK::Stop", SDK.MSVGetLastErrorString());
        //            return false;
        //        }
        //        LoggerService.Info(  "MSVStopTask end, nRetTaskId:{0} .......MsvUdpClientCtrlSDK::Stop", nRetTaskId);
        //    }
        //    catch (Exception e)
        //    {
        //        LoggerService.Info(  "MsvUdpClientCtrlSDK::Stop, Exception:{0}", e.Message);
        //        return false;
        //    }
        //    return true;
        //}

        //public bool Trace(int nChPort, string strMsvIP, ref TaskParam pTaskparam, string strTaskName, string pCaptureparam)
        //{
        //    MSV_RET ret;
        //    try
        //    {
        //        CClientTaskSDK SDK = GetMsvCtrlInstance(nChPort, strMsvIP);

        //        if (SDK == null)
        //        {
        //            LoggerService.Info(  "Tace(SDK=null) .....MsvUdpClientCtrlSDK::Trace");
        //            return false;
        //        }
        //        LoggerService.Info(  "prepare to MSVStartRetrospectTask......MsvUdpClientCtrlSDK::Trace");
        //        TASK_PARAM tmptaskparam = new TASK_PARAM();
        //        ClientParam2MSVTskParam(pTaskparam, ref tmptaskparam);
        //        TASK_ALL_PARAM_NEW task = new TASK_ALL_PARAM_NEW();
        //        task.taskParam.ulID = tmptaskparam.ulID;
        //        task.taskParam.strName = strTaskName;
        //        task.nCutLen = 10;
        //        task.captureParam = pCaptureparam;
        //        ret = SDK.MSVStartRetrospectTask(strMsvIP, task, nChPort);
        //        if (ret != MSV_RET.MSV_SUCCESS)
        //        {
        //            LoggerService.Info(  "MSVStartRetrospectTask falied, error:{0}", SDK.MSVGetLastErrorString());
        //            return false;
        //        }
        //        LoggerService.Info(  "MSVStartRetrospectTask end......MsvUdpClientCtrlSDK::Trace");
        //    }
        //    catch (Exception e)
        //    {
        //        LoggerService.Info(  "MsvUdpClientCtrlSDK::Trace, Exception:{0}", e.Message);
        //        return false;
        //    }
        //    return true;
        //}

        public bool Relecate(string strMsvIp, int nPort, string rtmpaddress)
        {
            //long taskID, string strMsvIp, string lpStrTargetIP, int nPort, string lpStrLocalIP, int bUDP, int nPgmID
            MSV_RET ret;
            try
            {
                Logger.Info($"************MsvUdpClientCtrlSDK::Relecate strMsvIp: {strMsvIp}, nPort:{nPort}.");
                CClientTaskSDK SDK = GetMsvCtrlInstance(nPort, strMsvIp);
                if (SDK == null)
                {
                    Logger.Info("GetMsvCtrlInstance(SDK=null), MsvUdpClientCtrlSDK::QuerySignalStatus");
                    return false;
                }
                ret = SDK.MSV_RelocateRTMP(strMsvIp, nPort, rtmpaddress);

                if (ret != MSV_RET.MSV_SUCCESS)
                {
                    Logger.Info("Cast Interface Function QuerySignalStatus Error!(error = {0})...........MsvUdpClientCtrlSDK::QuerySignalStatus", SDK.MSVGetLastErrorString());
                    return false;
                }
            }
            catch (System.Exception e)
            {
                Logger.Info("MsvUdpClientCtrlSDK::QuerySignalStatus Exception:{0}", e.Message);
                return false;
            }
            return true;
        }
        
    }
}
