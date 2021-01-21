using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using G2CtrlRestFulSrv;
using Sobey.Core.Log;

namespace MsvClientSDK
{
    public class CClientTaskSDKImp
    {
        public int m_nTimeOut;
        //public int m_nComtype;
        public int m_iCtrlPort;
        public string m_error_desc;
        public string m_iCtrlIp;
        private XmlDocument _xml = null;
        private object _msvfunclock = null;
        private G2UdpMsvCtrl m_udpMsv = null;
        public static ILogger Logger = LoggerManager.GetLogger("MSvNotifyInfo");
        public CClientTaskSDKImp()
        {

            m_nTimeOut = 5000;
            m_error_desc = "";
            m_iCtrlPort = 3100;
            //m_nComtype = nComType;
            _msvfunclock = new object();
            m_udpMsv = new G2UdpMsvCtrl();
            _xml = new XmlDocument();
        }
        
        ~CClientTaskSDKImp()
        {
        }
        /************************************************************************/
        /* 返回错误描述                                                        */
        /************************************************************************/
        public string MSVGetLastErrorString()
        {
            return m_error_desc;
        }

        //     /************************************************************************/
        //     /*将时间字符串形式转化成COleDateTime                                                                   */
        //     /************************************************************************/
        //     BOOL CClientTaskSDKImp::StringToCTime(CString& strTime, CTime& time)
        //     {
        //         CString str;
        //         TCHAR ch = _T('-');
        //         int nYear, nMonth, nDay, nHour, nMinute, nSecond;
        //         int nStart = 0, nEnd = 0;
        // 
        //         nEnd = strTime.Find(ch);
        //         if (-1 == nEnd || (nEnd - nStart != 4))
        //             return FALSE;
        //         str = strTime.Left(nEnd);
        //         nYear = _ttoi(LPCTSTR(str));
        //         nStart = nEnd + 1;
        // 
        //         nEnd = strTime.Find(ch, nStart);
        //         if (-1 == nEnd || (nEnd - nStart > 2))
        //             return FALSE;
        //         str = strTime.Mid(nStart, nEnd - nStart);
        //         nMonth = _ttoi(LPCTSTR(str));
        //         nStart = nEnd + 1;
        // 
        //         ch = _T(' ');
        //         nEnd = strTime.Find(ch, nStart);
        //         if (-1 == nEnd || (nEnd - nStart > 2))
        //             return FALSE;
        //         str = strTime.Mid(nStart, nEnd - nStart);
        //         nDay = _ttoi(LPCTSTR(str));
        //         nStart = nEnd + 1;
        // 
        //         ch = _T(':');
        //         nEnd = strTime.Find(ch, nStart);
        //         if (-1 == nEnd || (nEnd - nStart > 2))
        //             return FALSE;
        //         str = strTime.Mid(nStart, nEnd - nStart);
        //         nHour = _ttoi(LPCTSTR(str));
        //         nStart = nEnd + 1;
        // 
        //         nEnd = strTime.Find(ch, nStart);
        //         if (-1 == nEnd || (nEnd - nStart > 2))
        //             return FALSE;
        //         str = strTime.Mid(nStart, nEnd - nStart);
        //         nMinute = _ttoi(LPCTSTR(str));
        //         nStart = nEnd + 1;
        // 
        //         nEnd = strTime.GetLength() - nStart;
        //         if (nEnd > 2)
        //             return FALSE;
        //         str = strTime.Right(nEnd);
        //         nSecond = _ttoi(LPCTSTR(str));
        // 
        // 
        //         CTime tmptime(nYear, nMonth, nDay, nHour, nMinute, nSecond);
        //         time = tmptime;
        // 
        //         return TRUE;
        //     }
        /************************************************************************/
        /* 从字符串中得到硬盘盘符                                               */
        /************************************************************************/
        //     void CClientTaskSDKImp::GetDriverListFromString(const CString string, CArray<CString, CString&>& driverList)
        //     {
        //         CString strTmp(string);
        //         strTmp.Remove(_T(' '));
        //         TCHAR ch = _T('+');
        // 
        // 
        //         int nFirst = 0;
        //         int nFind = strTmp.Find(ch, nFirst);
        //         if (nFind == -1 && strTmp.GetLength() != 0)
        //         {
        //             driverList.Add(strTmp);
        //             return;
        //         }
        //         CString strT;
        //         while (nFind != -1)
        //         {
        //             strT = strTmp.Mid(nFirst, nFind - nFirst);
        //             driverList.Add(strT);
        //             nFirst = nFind + 1;
        //             nFind = strTmp.Find(ch, nFirst);
        //             if (nFind == -1 && nFirst < string.GetLength())
        //             {
        //                 strT = strTmp.Mid(nFirst, string.GetLength() - nFirst);
        //                 driverList.Add(strT);
        //             }
        //         }
        // 
        //     }
        // 
        //     /************************************************************************/
        //     /* 从字符串中得到任务ID                                                 */
        //     /************************************************************************/
        //     void CClientTaskSDKImp::GetTaskListFromString(const CString string, CArray<long, long&>& taskList)
        //     {
        //         CString strTmp(string);
        //         strTmp.Remove(_T(' '));
        //         TCHAR ch = _T('+');
        // 
        // 
        //         int nFirst = 0;
        //         int nFind = strTmp.Find(ch, nFirst);
        //         if (nFind == -1 && strTmp.GetLength() != 0)
        //         {
        //             long id = _wtol(strTmp);
        //             taskList.Add(id);
        //             return;
        //         }
        //         while (nFind != -1)
        //         {
        //             CString strT = strTmp.Mid(nFirst, nFind - nFirst);
        //             long id = _wtol((LPCTSTR)strT);
        //             taskList.Add(id);
        //             nFirst = nFind + 1;
        //             nFind = strTmp.Find(ch, nFirst);
        //             if (nFind == -1 && nFirst < string.GetLength())
        //             {
        //                 strT = strTmp.Mid(nFirst, string.GetLength() - nFirst);
        //                 id = _wtol((LPCTSTR)strT);
        //                 taskList.Add(id);
        //             }
        //         }
        //     }
        // 
        /************************************************************************/
        /* 格式化任务参数到XML字符串                                            */
        /************************************************************************/
        protected string FromatParmaToString(TASK_ALL_PARAM param, string formatOrder, int nChannel)
        {
            if (param == null)
            {
                m_error_desc = "FromatParmaToString: param is null!";
                Logger.Info( "FromatParmaToString: param is null!");
                return "";
            }
            if ((param.captureParam.tsParam.nSingleType != 0) && (param.captureParam.tsParam.bNeedDecode == 0 || param.captureParam.tsParam.bNeedDecode == 2))
            {
                //高质量原码
                if (param.captureParam.tsParam.tsSignalInfo.dwVideoDataRate > 0)
                    param.captureParam.bit_rate0 = Convert.ToInt32(param.captureParam.tsParam.tsSignalInfo.dwVideoDataRate);
                //目前只支持mpeg2
            }

            string strSend = "<? xml version =\"1.0\"?>";
            string strTemp = "";
            strTemp = string.Format("<{0}>", formatOrder);
            strSend += strTemp;

            strTemp = string.Format("<nChannel>{0}</nChannel>", nChannel);
            strSend += strTemp;
            /*******常规采集参数段*******/
            strSend += "<NORMALPARAM>";
            /****************************/
            //任务ID
            strTemp = string.Format("<ulID>{0}</ulID>", param.taskParam.ulID);
            strSend += strTemp;
            //任务类型
            strTemp = string.Format("<TaskMode>{0}</TaskMode>", Convert.ToInt32(param.taskParam.TaskMode));
            strSend += strTemp;
            //任务名

            //保证用户输入的任务名称合法
            if (param.taskParam.strName.Length > 0)
            {
                //	param.taskParam.strName.Replace('\\', '_');
                param.taskParam.strName = param.taskParam.strName.Replace('/', '_');
                //	param.taskParam.strName.Replace(':', '_');
                param.taskParam.strName = param.taskParam.strName.Replace('*', '_');
                param.taskParam.strName = param.taskParam.strName.Replace('?', '_');
                param.taskParam.strName = param.taskParam.strName.Replace('|', '_');
                param.taskParam.strName = param.taskParam.strName.Replace('"', '_');
                param.taskParam.strName = param.taskParam.strName.Replace('>', '_');
                param.taskParam.strName = param.taskParam.strName.Replace('<', '_');
                param.taskParam.strName = param.taskParam.strName.Replace(' ', '_');
            }
            strTemp = string.Format("<strName>{0}</strName>", param.taskParam.strName);
            strSend += strTemp;
            //任务描述
            strTemp = string.Format("<strDesc>{0}</strDesc>", param.taskParam.strDesc);
            strSend += strTemp;
            //任务开始时间
            strTemp = param.taskParam.tmBeg.ToString("yyyy-MM-dd HH:mm:ss");
            strSend += "<tmBeg>";
            strSend += strTemp;
            strSend += "</tmBeg>";
            //任务结束时间
            strTemp = param.taskParam.tmEnd.ToString("yyyy-MM-dd HH:mm:ss");
            strSend += "<tmEnd>";
            strSend += strTemp;
            strSend += "</tmEnd>";

            strTemp = Convert.ToString(param.taskParam.channel);
            strSend += "<nProgramID>";
            strSend += strTemp;
            strSend += "</nProgramID>";
            //信号源nSignalID
            strTemp = Convert.ToString(param.taskParam.nSignalID);
            strSend += "<nSignalID>";
            strSend += strTemp;
            strSend += "</nSignalID>";


            /************************************************************************/
            //增加帧号的参数
            strTemp = Convert.ToString(Convert.ToInt32(param.taskParam.bUseTime));
            strSend += "<UseTime>";
            strSend += strTemp;
            strSend += "</UseTime>";
            if (param.taskParam.bUseTime)
            {
                //strTemp.Format(_T("<StartFrame>%d</StartFrame>"), 0);//开始帧 如果为0 ，立即开始. （该帧是需要采集的）		
                //strSend += strTemp;

                strTemp = string.Format("<InOutCount>{0}</InOutCount>", 1);//开始帧 如果为0 ，立即开始
                strSend += strTemp;

                strTemp = string.Format("<InFrame{0}>{1}</InFrame{2}>", 0, 0, 0);
                strSend += strTemp;
                strTemp = string.Format("<OutFrame{0}>{1}</OutFrame{2}>", 0, 0xfffffffe, 0);
                strSend += strTemp;
                strTemp = string.Format("<TotalFrame{0}>{1}</TotalFrame{2}>", 0, 0, 0);
                strSend += strTemp;
            }
            else
            {
                /*strTemp.Format(_T("<InOutCount>%d</InOutCount>"), 1);//开始帧 如果为0 ，立即开始
                strSend += strTemp;
                strTemp.Format(_T("<InFrame%d>%d</InFrame%d>"), 0,param.taskParam.dwStartFrame,0);
                strSend += strTemp;
                strTemp.Format(_T("<OutFrame%d>%d</OutFrame%d>"), 0,param.taskParam.dwStartFrame+param.taskParam.dwTaskTatolFrame,0);
                strSend += strTemp;/**/


                //全部转型成功后使用
                if (param.taskParam.nInOutCount > 100)
                    param.taskParam.nInOutCount = 100;

                strTemp = string.Format("<InOutCount>{0}</InOutCount>", param.taskParam.nInOutCount);//开始帧 如果为0 ，立即开始
                strSend += strTemp;
                for (int j = 0; j < param.taskParam.nInOutCount; ++j)
                {
                    strTemp = string.Format("<InFrame{0}>{1}</InFrame{2}>", j, param.taskParam.dwInFrame[j], j);
                    strSend += strTemp;
                    strTemp = string.Format("<OutFrame{0}>{1}</OutFrame{2}>", j, param.taskParam.dwOutFrame[j], j);
                    strSend += strTemp;
                    strTemp = string.Format("<TotalFrame{0}>{1}</TotalFrame{2}>", j, param.taskParam.dwTotalFrame[j], j);
                    strSend += strTemp;
                }/**/
            }

            strTemp = string.Format("<CutClipFrame>{0}</CutClipFrame>", param.taskParam.dwCutClipFrame);//如果不需要分段 默认为0xfffffffe	
            strSend += strTemp;
            //strTemp.Format(_T("<TaskTatolFrame>%d</TaskTatolFrame>"), param.taskParam.dwTaskTatolFrame);//任务总帧数 当任务采集的帧数到达此值 任务自动关闭，  如果不需要中层自动结束 设为0xfffffffe
            //strSend += strTemp;
            strTemp = string.Format("<Retrospect>{0}</Retrospect>", Convert.ToInt32(param.taskParam.bRetrospect));//是否回朔采集
            strSend += strTemp;
            /************************************************************************/

            /************************************/
            strSend += "</NORMALPARAM>";
            /************************************/

            /**********采集参数段********************/
            strSend += "<CAPTUREPARAM>";
            /****************************************/
            //任务分段长度
            strTemp = string.Format("<cutlen>{0}</cutlen>", param.nCutLen);
            strSend += strTemp;
            //入库分类ID
            strTemp = string.Format("<lCatalogID>{0}</lCatalogID>", param.lCatalogID);
            strSend += strTemp;
            //0
            strTemp = string.Format("<bPath0>{0}</bPath0>", Convert.ToInt32(param.captureParam.bPath0));
            strSend += strTemp;

            strTemp = string.Format("<bAudio0>{0}</bAudio0>", Convert.ToInt32(param.captureParam.bAudio0));
            strSend += strTemp;

            strTemp = string.Format("<bAlone0>{0}</bAlone0>", Convert.ToInt32(param.captureParam.bAlone0));
            strSend += strTemp;

            strTemp = string.Format("<bDetectKeyFrame>{0}</bDetectKeyFrame>", param.captureParam.bDetectKeyFrame);
            strSend += strTemp;

            //////////////////////////////////////////////////////////////////////////
            //将采集路径和任务名分开
            strTemp = string.Format("<path0FileName>{0}</path0FileName>", param.captureParam.path0FileName);
            strSend += strTemp;
            strTemp = string.Format("<taskName>{0}</taskName>", param.taskParam.strName);
            strSend += strTemp;
            //////////////////////////////////////////////////////////////////////////
            strTemp = string.Format("<dwStartTimeInfo>{0}</dwStartTimeInfo>", param.captureParam.startTimeInfo);
            strSend += strTemp;
            strTemp = string.Format("<dwStartFileIndex>{0}</dwStartFileIndex>", param.captureParam.startFileIndex);
            strSend += strTemp;
            /////////////////////////

            strTemp = string.Format("<nEncodeType0>{0}</nEncodeType0>", param.captureParam.nEncodeType0);
            strSend += strTemp;

            strTemp = string.Format("<subEncodeType0>{0}</subEncodeType0>", param.captureParam.subEncodeType0);
            strSend += strTemp;

            strTemp = string.Format("<bit_rate0>{0}</bit_rate0>", param.captureParam.bit_rate0);
            strSend += strTemp;

            strTemp = string.Format("<AVWriteTypeV0>{0}</AVWriteTypeV0>", param.captureParam.AVWriteTypeV0);
            strSend += strTemp;

            strTemp = string.Format("<dwSamplesOutA0>{0}</dwSamplesOutA0>", param.captureParam.dwSamplesOutA0);
            strSend += strTemp;

            strTemp = string.Format("<AudioWriteTypeA0>{0}</AudioWriteTypeA0>", param.captureParam.AudioWriteTypeA0);
            strSend += strTemp;

            strTemp = string.Format("<dwMp3RateA0>{0}</dwMp3RateA0>", param.captureParam.dwMp3RateA0);
            strSend += strTemp;

            strTemp = string.Format("<iBitDepth0>{0}</iBitDepth0>", param.captureParam.iBitDepth0);
            strSend += strTemp;

            //1
            strTemp = string.Format("<bPath1>{0}</bPath1>", Convert.ToInt32(param.captureParam.bPath1));
            strSend += strTemp;

            strTemp = string.Format("<bAudio1>{0}</bAudio1>", Convert.ToInt32(param.captureParam.bAudio1));
            strSend += strTemp;

            strTemp = string.Format("<bAlone1>{0}</bAlone1>", Convert.ToInt32(param.captureParam.bAlone1));
            strSend += strTemp;

            strTemp = string.Format("<path1FileName>{0}</path1FileName>", param.captureParam.path1FileName);
            strSend += strTemp;

            strTemp = string.Format("<nEncodeType1>{0}</nEncodeType1>", param.captureParam.nEncodeType1);
            strSend += strTemp;

            strTemp = string.Format("<subEncodeType1>{0}</subEncodeType1>", param.captureParam.subEncodeType1);
            strSend += strTemp;

            strTemp = string.Format("<bit_rate1>{0}</bit_rate1>", param.captureParam.bit_rate1);
            strSend += strTemp;

            strTemp = string.Format("<AVWriteTypeV1>{0}</AVWriteTypeV1>", param.captureParam.AVWriteTypeV1);
            strSend += strTemp;

            strTemp = string.Format("<dwMp3RateA1>{0}</dwMp3RateA1>", param.captureParam.dwMp3RateA1);
            strSend += strTemp;

            strTemp = string.Format("<iBitDepth1>{0}</iBitDepth1>", param.captureParam.iBitDepth1);
            strSend += strTemp;

            strTemp = string.Format("<dwSamplesOutA1>{0}</dwSamplesOutA1>", param.captureParam.dwSamplesOutA1);
            strSend += strTemp;
            strTemp = string.Format("<AudioWriteTypeA1>{0}</AudioWriteTypeA1>", param.captureParam.AudioWriteTypeA1);
            strSend += strTemp;
            strTemp = string.Format("<nGOPPFrameCount>{0}</nGOPPFrameCount>", param.captureParam.nGOPPCount);
            strSend += strTemp;
            strTemp = string.Format("<nGOPBFrameCount>{0}</nGOPBFrameCount>", param.captureParam.nGOPBCount);
            strSend += strTemp;
            strTemp = string.Format("<bUseTransfer>{0}</bUseTransfer>", Convert.ToInt32(param.captureParam.bUseTransfer));
            strSend += strTemp;


            /************************************************************************/
            //增加帧号的参数 by yj
            strTemp = Convert.ToString(Convert.ToInt32(param.taskParam.bUseTime));
            strSend += "<bUseTime>";
            strSend += strTemp;
            strSend += "</bUseTime>";
            if (param.taskParam.bUseTime)
            {
                //strTemp.Format(_T("<StartFrame>%d</StartFrame>"), 0);//开始帧 如果为0 ，立即开始. （该帧是需要采集的）		
                //strSend += strTemp;

                strTemp = string.Format("<nInOutCount>{0}</nInOutCount>", 1);//开始帧 如果为0 ，立即开始
                strSend += strTemp;

                strTemp = string.Format("<dwInFrame{0}>{1}</dwInFrame{2}>", 0, 0, 0);
                strSend += strTemp;
                strTemp = string.Format("<dwOutFrame{0}>{1}</dwOutFrame{2}>", 0, 0xfffffffe, 0);
                strSend += strTemp;
                strTemp = string.Format("<dwTotalFrame{0}>{1}</dwTotalFrame{2}>", 0, 0, 0);
                strSend += strTemp;
            }
            else
            {
                /*strTemp.Format(_T("<nInOutCount>%d</nInOutCount>"), 1);//开始帧 如果为0 ，立即开始
                strSend += strTemp;
                strTemp.Format(_T("<dwInFrame%d>%d</dwInFrame%d>"), 0,param.taskParam.dwStartFrame,0);
                strSend += strTemp;
                strTemp.Format(_T("<dwOutFrame%d>%d</dwOutFrame%d>"), 0,param.taskParam.dwStartFrame+param.taskParam.dwTaskTatolFrame,0);
                strSend += strTemp;/**/

                //全部转型成功后使用
                if (param.taskParam.nInOutCount > 100)
                    param.taskParam.nInOutCount = 100;

                strTemp = string.Format("<TimecodeType>%d</TimecodeType>", param.taskParam.nTimeCode);//开始帧 如果为0 ，立即开始
                strSend += strTemp;

                strTemp = string.Format("<nInOutCount>%d</nInOutCount>", param.taskParam.nInOutCount);//开始帧 如果为0 ，立即开始
                strSend += strTemp;
                for (int j = 0; j < param.taskParam.nInOutCount; ++j)
                {
                    strTemp = string.Format("<dwInFrame{0}>{1}</dwInFrame{2}>", j, param.taskParam.dwInFrame[j], j);
                    strSend += strTemp;
                    strTemp = string.Format("<dwOutFrame{0}>{1}</dwOutFrame{2}>", j, param.taskParam.dwOutFrame[j], j);
                    strSend += strTemp;
                    strTemp = string.Format("<dwTotalFrame{0}>{1}</dwTotalFrame{2}>", j, param.taskParam.dwTotalFrame[j], j);
                    strSend += strTemp;
                }/**/
            }

            strTemp = string.Format("<dwCutClipFrame>{0}</dwCutClipFrame>", param.taskParam.dwCutClipFrame);//如果不需要分段 默认为0xfffffffe	
            strSend += strTemp;
            //strTemp.Format(_T("<dwTaskTatolFrame>%d</dwTaskTatolFrame>"), param.taskParam.dwTaskTatolFrame);//任务总帧数 当任务采集的帧数到达此值 任务自动关闭，  如果不需要中层自动结束 设为0xfffffffe
            //strSend += strTemp;
            strTemp = string.Format("<bRetrospect>{0}</bRetrospect>", Convert.ToInt32(param.taskParam.bRetrospect));//是否回朔采集
            strSend += strTemp;
            /************************************************************************/

            strTemp = string.Format("<nAudioChannelAttribute>{0}</nAudioChannelAttribute>", param.captureParam.dwAudioChannelAttribute);//是否回朔采集
            strSend += strTemp;

            strTemp = string.Format("<FirPicNum>{0}</FirPicNum>", param.captureParam.dwFirPicNum); //刷新首帧缩略图的帧号
            strSend += strTemp;
            strTemp = string.Format("<video_width>{0}</video_width>", param.captureParam.dwVideo_width);
            strSend += strTemp;
            strTemp = string.Format("<video_height>{0}</video_height>", param.captureParam.dwVideo_height);
            strSend += strTemp;
            strTemp = string.Format("<CHROMA_FORMAT>{0}</CHROMA_FORMAT>", param.captureParam.wCHROMA_FORMAT);
            strSend += strTemp;

            strTemp = string.Format("<ASR_Dic_Name>{0}</ASR_Dic_Name>", param.captureParam.str_ASR_Dic_Name);
            strSend += strTemp;
            strTemp = string.Format("<ASR_mask>{0}</ASR_mask>", param.captureParam.dwASR_mask);
            strSend += strTemp;
            strTemp = string.Format("<CloseCaptionFilePath>{0}</CloseCaptionFilePath>", param.captureParam.strCloseCaptionFilePath);
            strSend += strTemp;

            //////////////////////////////////////////////////////////////////////////
            //ts
            strTemp = string.Format("<nSingleType>{0}</nSingleType>", param.captureParam.tsParam.nSingleType);
            strSend += strTemp;

            strTemp = string.Format("<IngestChannel>{0}</IngestChannel>", param.captureParam.tsParam.strDataChannel_Name);
            strSend += strTemp;

            strTemp = string.Format("<AnalyzeChannel>{0}</AnalyzeChannel>", param.captureParam.tsParam.strDataChannel_Name);
            strSend += strTemp;

            strTemp = string.Format("<AnalyzeChannelID>{0}</AnalyzeChannelID>", param.captureParam.tsParam.dwDataChannel_ID);
            strSend += strTemp;

            strTemp = string.Format("<PgmName>{0}</PgmName>", param.captureParam.tsParam.strPgmName);
            strSend += strTemp;

            strTemp = string.Format("<PgmID>{0}</PgmID>", param.captureParam.tsParam.dwPgmID);
            strSend += strTemp;

            strTemp = string.Format("<NeedDecode>{0}</NeedDecode>", param.captureParam.tsParam.bNeedDecode);
            strSend += strTemp;

            strTemp = "<TSSignalInfo>";
            strSend += strTemp;

            strTemp = string.Format("<nAudioType>{0}</nAudioType>", param.captureParam.tsParam.tsSignalInfo.nAudioType);
            strSend += strTemp;

            strTemp = string.Format("<nAudioCount>{0}</nAudioCount>", param.captureParam.tsParam.tsSignalInfo.nAudioCount);
            strSend += strTemp;

            strTemp = string.Format("<dwBitsPerSample>{0}</dwBitsPerSample>", param.captureParam.tsParam.tsSignalInfo.dwBitsPerSample);
            strSend += strTemp;

            strTemp = string.Format("<dwSamplePerSecond>{0}</dwSamplePerSecond>", param.captureParam.tsParam.tsSignalInfo.dwSamplePerSecond);
            strSend += strTemp;

            strTemp = string.Format("<nVideoType>{0}</nVideoType>", param.captureParam.tsParam.tsSignalInfo.nVideoType);
            strSend += strTemp;

            strTemp = string.Format("<nVideoWidth>{0}</nVideoWidth>", param.captureParam.tsParam.tsSignalInfo.nVideoWidth);
            strSend += strTemp;

            strTemp = string.Format("<nVideoHeight>{0}</nVideoHeight>", param.captureParam.tsParam.tsSignalInfo.nVideoHeight);
            strSend += strTemp;

            strTemp = string.Format("<fps>{0}</fps>", param.captureParam.tsParam.tsSignalInfo.fps);
            strSend += strTemp;

            strTemp = string.Format("<dwBitCount>{0}</dwBitCount>", param.captureParam.tsParam.tsSignalInfo.dwBitCount);
            strSend += strTemp;

            strTemp = string.Format("<nFrameType>{0}</nFrameType>", param.captureParam.tsParam.tsSignalInfo.nFrameType);
            strSend += strTemp;

            strTemp = "</TSSignalInfo>";
            strSend += strTemp;
            //////////////////////////////////////////////////////////////////////////
            /*******************************************************/
            strSend += "</CAPTUREPARAM>";
            /*******************************************************/
            strTemp = string.Format("</{0}>\0", formatOrder);
            strSend += strTemp;
            return strSend;
        }
        protected string FromatParmaToStringNew(TASK_ALL_PARAM_NEW param, string formatOrder, int nChannel)
        {
            if (param == null)
            {
                m_error_desc = "FromatParmaToStringNew: param is null";
                Logger.Info(m_error_desc);
                return "";
            }
            string strSend = "<?xml version=\"1.0\"?>";
            string strTemp = "";
            strTemp = string.Format("<{0}>", formatOrder);
            strSend += strTemp;

            strTemp = string.Format("<nChannel>{0}</nChannel>", nChannel);
            strSend += strTemp;
            /*******常规采集参数段*******/
            strSend += "<NORMALPARAM>";
            /****************************/
            //任务ID
            strTemp = string.Format("<ulID>{0}</ulID>", param.taskParam.ulID);
            strSend += strTemp;
            //任务类型
            strTemp = string.Format("<TaskMode>{0}</TaskMode>", Convert.ToInt32(param.taskParam.TaskMode));
            strSend += strTemp;
            //任务名

            //保证用户输入的任务名称合法
            if (param.taskParam.strName.Length > 0)
            {
                //	param.taskParam.strName.Replace(_T('\\'), _T('_'));
                param.taskParam.strName = param.taskParam.strName.Replace('/', '_');
                //	param.taskParam.strName.Replace(_T(':'), _T('_'));
                param.taskParam.strName = param.taskParam.strName.Replace('*', '_');
                param.taskParam.strName = param.taskParam.strName.Replace('?', '_');
                param.taskParam.strName = param.taskParam.strName.Replace('|', '_');
                param.taskParam.strName = param.taskParam.strName.Replace('"', '_');
                param.taskParam.strName = param.taskParam.strName.Replace('>', '_');
                param.taskParam.strName = param.taskParam.strName.Replace('<', '_');
                param.taskParam.strName = param.taskParam.strName.Replace(' ', '_');
            }
            strTemp = string.Format("<strName>{0}</strName>", param.taskParam.strName);
            strSend += strTemp;
            //任务描述
            strTemp = string.Format("<strDesc>{0}</strDesc>", param.taskParam.strDesc);
            strSend += strTemp;
            //任务开始时间
            strTemp = param.taskParam.tmBeg.ToString("yyyy-MM-dd HH:mm:ss");
            strSend += "<tmBeg>";
            strSend += strTemp;
            strSend += "</tmBeg>";
            //任务结束时间
            strTemp = param.taskParam.tmEnd.ToString("yyyy-MM-dd HH:mm:ss");
            strSend += "<tmEnd>";
            strSend += strTemp;
            strSend += "</tmEnd>";

            strTemp = Convert.ToString(param.taskParam.channel);
            strSend += "<nProgramID>";
            strSend += strTemp;
            strSend += "</nProgramID>";
            //信号源nSignalID
            strTemp = Convert.ToString(param.taskParam.nSignalID);
            strSend += "<nSignalID>";
            strSend += strTemp;
            strSend += "</nSignalID>";

            /************************************************************************/
            //增加帧号的参数
            strTemp = Convert.ToString(Convert.ToInt32(param.taskParam.bUseTime));
            strSend += "<UseTime>";
            strSend += strTemp;
            strSend += "</UseTime>";
            if (param.taskParam.bUseTime)
            {
                //strTemp.Format(_T("<StartFrame>%d</StartFrame>"), 0);//开始帧 如果为0 ，立即开始. （该帧是需要采集的）
                //strSend += strTemp;

                strTemp = string.Format("<InOutCount>{0}</InOutCount>", 1);//开始帧 如果为0 ，立即开始
                strSend += strTemp;
                strTemp = string.Format("<InFrame{0}>{1}</InFrame{2}>", 0, 0, 0);
                strSend += strTemp;
                strTemp = string.Format("<OutFrame{0}>{1}</OutFrame{2}>", 0, 0, 0);
                strSend += strTemp;
                strTemp = string.Format("<TotalFrame{0}>{1}</TotalFrame{2}>", 0, 0, 0);
                strSend += strTemp;/**/
            }
            else
            {
                /*strTemp.Format(_T("<InOutCount>%d</InOutCount>"), 1);//开始帧 如果为0 ，立即开始
                strSend += strTemp;
                strTemp.Format(_T("<InFrame%d>%d</InFrame%d>"), 0,param.taskParam.dwStartFrame,0);
                strSend += strTemp;
                strTemp.Format(_T("<OutFrame%d>%d</OutFrame%d>"), 0,param.taskParam.dwStartFrame+param.taskParam.dwTaskTatolFrame,0);
                strSend += strTemp;/***/

                //全部转型成功后使用
                if (param.taskParam.nInOutCount > 100)
                    param.taskParam.nInOutCount = 100;

                strTemp = string.Format("<InOutCount>{0}</InOutCount>", param.taskParam.nInOutCount);//开始帧 如果为0 ，立即开始
                strSend += strTemp;
                for (int j = 0; j < param.taskParam.nInOutCount; ++j)
                {
                    strTemp = string.Format("<InFrame{0}>{1}</InFrame{2}>", j, param.taskParam.dwInFrame[j], j);
                    strSend += strTemp;
                    strTemp = string.Format("<OutFrame{0}>{1}</OutFrame{2}>", j, param.taskParam.dwOutFrame[j], j);
                    strSend += strTemp;
                    strTemp = string.Format("<TotalFrame{0}>{1}</TotalFrame{2}>", j, param.taskParam.dwTotalFrame[j], j);
                    strSend += strTemp;
                }/**/
            }

            strTemp = string.Format("<CutClipFrame>{0}</CutClipFrame>", param.taskParam.dwCutClipFrame);//如果不需要分段 默认为0xfffffffe	
            strSend += strTemp;
            //strTemp.Format(_T("<TaskTatolFrame>%d</TaskTatolFrame>"), param.taskParam.dwTaskTatolFrame);//任务总帧数 当任务采集的帧数到达此值 任务自动关闭，  如果不需要中层自动结束 设为0xfffffffe
            //strSend += strTemp;
            strTemp = string.Format("<Retrospect>{0}</Retrospect>", Convert.ToInt32(param.taskParam.bRetrospect));//是否回朔采集
            strSend += strTemp;
            /************************************************************************/

            /************************************/
            strSend += "</NORMALPARAM>";
            /************************************/

            /************************************************************************/
            //客户端发送过来的采集字符串中没有帧信息，需要再加入一边
            //增加帧号的参数
            string strFrmInfo = "";
            int iPos = param.captureParam.IndexOf("<bUseTime>");
            if (iPos == -1)
            {
                strTemp = Convert.ToString(Convert.ToInt32(param.taskParam.bUseTime));
                strFrmInfo += "<bUseTime>";
                strFrmInfo += strTemp;
                strFrmInfo += "</bUseTime>";
                //strTemp.Format(_T("<dwStartFrame>%d</dwStartFrame>"), param.taskParam.dwStartFrame);//开始帧 如果为0 ，立即开始. （该帧是需要采集的）
                //strFrmInfo += strTemp;
                strTemp = string.Format("<dwCutClipFrame>{0}</dwCutClipFrame>", param.taskParam.dwCutClipFrame);//如果不需要分段 默认为0xfffffffe	
                strFrmInfo += strTemp;
                //strTemp.Format(_T("<dwTaskTatolFrame>%d</dwTaskTatolFrame>"), param.taskParam.dwTaskTatolFrame);//任务总帧数 当任务采集的帧数到达此值 任务自动关闭，  如果不需要中层自动结束 设为0xfffffffe
                //strFrmInfo += strTemp;
                strTemp = string.Format("<bRetrospect>{0}</bRetrospect>", Convert.ToUInt32(param.taskParam.bRetrospect));//是否回朔采集
                strFrmInfo += strTemp;

                if (param.taskParam.bUseTime)
                {
                    strTemp = string.Format("<nInOutCount>{0}</nInOutCount>", 1);//开始帧 如果为0 ，立即开始
                    strFrmInfo += strTemp;
                    strTemp = string.Format("<dwInFrame{0}>{1}</dwInFrame{2}>", 0, 0, 0);
                    strFrmInfo += strTemp;
                    strTemp = string.Format("<dwOutFrame{0}>{1}</dwOutFrame{2}>", 0, 0, 0);
                    strFrmInfo += strTemp;
                    strTemp = string.Format("<dwTotalFrame{0}>{1}</dwTotalFrame{2}>", 0, 0, 0);
                    strFrmInfo += strTemp;
                }
                else
                {
                    /*strTemp.Format(_T("<nInOutCount>%d</nInOutCount>"), 1);//开始帧 如果为0 ，立即开始
                    strFrmInfo += strTemp;			
                    strTemp.Format(_T("<dwInFrame%d>%d</dwInFrame%d>"), 0,param.taskParam.dwStartFrame,0);
                    strFrmInfo += strTemp;
                    strTemp.Format(_T("<dwOutFrame%d>%d</dwOutFrame%d>"), 0,param.taskParam.dwStartFrame+param.taskParam.dwTaskTatolFrame,0);
                    strFrmInfo += strTemp;/**/

                    //全部转型成功后使用
                    if (param.taskParam.nInOutCount > 100)
                        param.taskParam.nInOutCount = 100;

                    strTemp = string.Format("<nInOutCount>{0}</nInOutCount>", param.taskParam.nInOutCount);//开始帧 如果为0 ，立即开始
                    strFrmInfo += strTemp;
                    for (int j = 0; j < param.taskParam.nInOutCount; ++j)
                    {
                        strTemp = string.Format("<dwInFrame{0}>{1}</dwInFrame{2}>", j, param.taskParam.dwInFrame[j], j);
                        strFrmInfo += strTemp;
                        strTemp = string.Format("<dwOutFrame{0}>{1}</dwOutFrame{2}>", j, param.taskParam.dwOutFrame[j], j);
                        strFrmInfo += strTemp;
                        strTemp = string.Format("<dwTotalFrame{0}>{1}</dwTotalFrame{2}>", j, param.taskParam.dwTotalFrame[j], j);
                        strFrmInfo += strTemp;
                    }/**/
                }
            }
            /************************************************************************/

            strSend += param.captureParam;
            strSend = strSend.Insert(strSend.Length - 15, strFrmInfo);
            /////////////////////////
            strTemp = string.Format("<taskName>{0}</taskName>", param.taskParam.strName);
            strSend = strSend.Insert(strSend.Length - 15, strTemp);
            ///////////////////////////
            strTemp = string.Format("</{0}>\0", formatOrder);
            strSend += strTemp;
            return strSend;
        }
        protected string FormatParamToString(UploadInfo param, string formatOrder)
        {
            string strSend = "<?xml version=\"1.0\"?>";
            string strTemp = "";
            strTemp = string.Format("<{0}>", formatOrder);
            strSend += strTemp;
            /*******常规采集参数段*******/
            strSend += "<NORMALPARAM>";
            /****************************/
            //任务ID
            strTemp = string.Format("<ulID>{0}</ulID>", param.nTaskID);
            strSend += strTemp;

            //保证用户输入的任务名称合法
            if (param.strTaskName.Length > 0)
            {
                //	param.taskParam.strName.Replace(_T('\\'), _T('_'));
                param.strTaskName = param.strTaskName.Replace('/', '_');
                //	param.strTaskName.Replace(_T(':'), _T('_'));
                param.strTaskName = param.strTaskName.Replace('*', '_');
                param.strTaskName = param.strTaskName.Replace('?', '_');
                param.strTaskName = param.strTaskName.Replace('|', '_');
                param.strTaskName = param.strTaskName.Replace('"', '_');
                param.strTaskName = param.strTaskName.Replace('>', '_');
                param.strTaskName = param.strTaskName.Replace('<', '_');
                param.strTaskName = param.strTaskName.Replace(' ', '_');
            }
            strTemp = string.Format("<strName>{0}</strName>", param.strTaskName);
            strSend += strTemp;
            //任务开始时间
            strTemp = Convert.ToString(param.nTrimIn);
            strSend += "<tmBeg>";
            strSend += strTemp;
            strSend += "</tmBeg>";
            //任务结束时间
            strTemp = Convert.ToString(param.nTrimOut);
            strSend += "<tmEnd>";
            strSend += strTemp;
            strSend += "</tmEnd>";
            /************************************/
            strSend += "</NORMALPARAM>";
            /************************************/
            strSend += param.captureParam;
            /////////////////////////
            strTemp = string.Format("<taskName>{0}</taskName>", param.strTaskName);
            strSend.Insert(strSend.Length - 15, strTemp);
            ///////////////////////////
            strTemp = string.Format("</{0}>\0", formatOrder);
            strSend += strTemp;
            return strSend;
        }
        public MSV_RET MSVSetControlMode(string strMsvIp, bool bCtrl, int nChannel)
        {
            lock (_msvfunclock)
            {
                try
                {
                    string ip = (strMsvIp != "") ? strMsvIp : m_iCtrlIp;
                    string cmd = string.Format("<? xml version =\"1.0\"?><set_msv_ctrlmode><b_mode>{0}</b_mode><nChannel>{1}</nChannel><set_msv_ctrlmode>\0", Convert.ToInt32(bCtrl), nChannel);
                    string strRet = m_udpMsv.GetMsvUdpData("", nChannel, ip, nChannel, cmd, Logger);
                    if (strRet != "")
                    {
                        _xml.LoadXml(strRet);
                        XmlElement _root = _xml.DocumentElement;
                        if (_root != null && _root.Name == "std_reply")
                        {
                            XmlNode _retNode = _root.SelectSingleNode("s_result");
                            if (_retNode != null)
                            {
                                string _result = _retNode.InnerText;
                                if (_result == "succeed")
                                {
                                    m_error_desc = "Operation succeed";
                                    return MSV_RET.MSV_SUCCESS;
                                }
                                else
                                {
                                    XmlNode nError = _root.SelectSingleNode("s_error_string");
                                    if (nError != null)
                                    {
                                        m_error_desc = nError.InnerText;
                                    }
                                    return MSV_RET.MSV_FAILED;
                                }
                            }
                            else
                            {
                                return MSV_RET.MSV_XMLERROR;
                            }
                        }
                        else
                        {
                            return MSV_RET.MSV_XMLERROR;
                        }
                    }
                    else
                    {
                        return MSV_RET.MSV_NETERROR;
                    }
                }
                catch (System.Exception ex)
                {
                    m_error_desc = "MSVSetControlMode:" + ex.Message;
                    Logger.Info(m_error_desc);
                    return MSV_RET.MSV_FAILED;
                }
            }

        }
        /*********************************************************************************
        查询媒体服务器状态
        **********************************************************************************/
        public MSV_RET MSVQueryState(string strMsvIp, ref MSV_STATE state, int nChannel)
        {
            lock (_msvfunclock)
            {
                try
                {
                    string ip = (strMsvIp != "") ? strMsvIp : m_iCtrlIp;
                
                    string cmd = string.Format("<?xml version =\"1.0\"?><query_state><nChannel>{0}</nChannel></query_state>", nChannel);
                    string strRet = m_udpMsv.GetMsvUdpData("", nChannel, ip, nChannel, cmd, Logger);
                    if (strRet != "")
                    {
                        _xml.LoadXml(strRet);
                        XmlElement _root = _xml.DocumentElement;
                        if (_root != null && _root.Name == "std_reply")
                        {
                            XmlNode _retNode = _root.SelectSingleNode("s_result");
                            if (_retNode != null)
                            {
                                string _result = _retNode.InnerText;
                                if (_result == "succeed")
                                {
                                    //查询计数器值
                                    XmlNode nQueryCount = _root.SelectSingleNode("QueryCount");
                                    if (nQueryCount != null)
                                    {
                                        state.dwQueryCounter = Convert.ToUInt64(nQueryCount.InnerText);
                                    }
                                    //是否为回溯任务状态
                                    XmlNode nRetroSpect = _root.SelectSingleNode("RetroSpect");
                                    if (nRetroSpect != null)
                                    {
                                        state.bRetroSpect = Convert.ToInt32(nRetroSpect.InnerText);
                                    }
                                    //MSV启动模式
                                    XmlNode nMode = _root.SelectSingleNode("msv_mode");
                                    if (nMode != null)
                                    {
                                        state.msv_mode = (MSV_MODE)Convert.ToInt32(nMode.InnerText);
                                    }
                                    //MSV工作模式
                                    XmlNode nWorkMode = _root.SelectSingleNode("msv_work_mode");
                                    if (nWorkMode != null)
                                    {
                                        state.msv_work_mode = (WORK_MODE)Convert.ToInt32(nWorkMode.InnerText);
                                    }
                                    //MSV运行状态
                                    XmlNode nCaptureState = _root.SelectSingleNode("msv_capture_state");
                                    if (nCaptureState != null)
                                    {
                                        state.msv_capture_state = (CAPTURE_STATE)Convert.ToInt32(nCaptureState.InnerText);
                                    }
                                    //如果媒体服务器处于远程控制模式，将返回控方客户端IP
                                    XmlNode nClinetIp = _root.SelectSingleNode("msv_client_ip");
                                    if (nClinetIp != null)
                                    {
                                        state.msv_client_ip = nClinetIp.InnerText;
                                    }

                                    //m_s.Close();
                                    //m_error_desc = _T("操作成功");
                                    m_error_desc = "Operation succeed";
                                    return MSV_RET.MSV_SUCCESS;
                                }
                                else
                                {
                                    XmlNode nError = _root.SelectSingleNode("s_error_string");
                                    if (nError != null)
                                    {
                                        m_error_desc = nError.InnerText;
                                    }
                                    return MSV_RET.MSV_FAILED;
                                }
                            }
                            else
                            {
                                return MSV_RET.MSV_XMLERROR;
                            }
                        }
                        else
                        {
                            return MSV_RET.MSV_XMLERROR;
                        }
                    }
                    else
                    {
                        return MSV_RET.MSV_NETERROR;
                    }
                }
                    catch (System.Exception ex)
                {
                    m_error_desc = "MSVQueryState: " + ex.Message;
                    Logger.Info(m_error_desc);
                    return MSV_RET.MSV_FAILED;
                }
            }

        }

        /*********************************************************************************
        切换媒体服务器状态
        **********************************************************************************/
        public MSV_RET MSVSwitchState(string strMsvIp, WORK_MODE mode, int nChannel)
        {
            lock (_msvfunclock)
            {
                try
                {
                    string ip = (strMsvIp != "") ? strMsvIp : m_iCtrlIp;
                    string cmd = string.Format("<?xml version=\"1.0\"?><switch_msv_state><s_mode>{0}</s_mode><nChannel>{1}</nChannel></switch_msv_state>\0", Convert.ToInt32(mode), nChannel);
                    string strRet = m_udpMsv.GetMsvUdpData("", nChannel, ip, nChannel, cmd, Logger);
                    if (strRet != "")
                    {
                        _xml.LoadXml(strRet);
                        XmlElement _root = _xml.DocumentElement;
                        if (_root != null && _root.Name == "std_reply")
                        {
                            XmlNode _retNode = _root.SelectSingleNode("s_result");
                            if (_retNode != null)
                            {
                                string _result = _retNode.InnerText;
                                if (_result == "succeed")
                                {
                                    m_error_desc = "Operation succeed";
                                    return MSV_RET.MSV_SUCCESS;
                                }
                                else
                                {
                                    XmlNode nError = _root.SelectSingleNode("s_error_string");
                                    if (nError != null)
                                    {
                                        m_error_desc = nError.InnerText;
                                    }
                                    return MSV_RET.MSV_FAILED;
                                }
                            }
                            else
                            {
                                return MSV_RET.MSV_XMLERROR;
                            }
                        }
                        else
                        {
                            return MSV_RET.MSV_XMLERROR;
                        }
                    }
                    else
                    {
                        return MSV_RET.MSV_NETERROR;
                    }
                }
                catch (System.Exception ex)
                {
                    m_error_desc = "MSVSwitchState: " + ex.Message;
                    Logger.Info(m_error_desc);
                    return MSV_RET.MSV_FAILED;
                }
            }
        }
        /********************************************************************************
            描述 :获得任务队列状态
        ********************************************************************************/
        public MSV_RET MSVGetTaskListState(string strMsvIp, ref BATCH_STATE state, int nChannel)
        {
            lock (_msvfunclock)
            {
                try
                {
                    string ip = (strMsvIp != "") ? strMsvIp : m_iCtrlIp;
                    string cmd = string.Format("<?xml version=\"1.0\"?><get_tasklist_state><nChannel>{0}</nChannel></get_tasklist_state>\0", nChannel);
                    string strRet = m_udpMsv.GetMsvUdpData("", nChannel, ip, nChannel, cmd, Logger);
                    if (strRet != "")
                    {
                        _xml.LoadXml(strRet);
                        XmlElement _root = _xml.DocumentElement;
                        if (_root != null && _root.Name == "std_reply")
                        {
                            XmlNode _retNode = _root.SelectSingleNode("s_result");
                            if (_retNode != null)
                            {
                                string _result = _retNode.InnerText;
                                if (_result == "succeed")
                                {
                                    m_error_desc = "Operation succeed";
                                    return MSV_RET.MSV_SUCCESS;
                                }
                                else
                                {
                                    XmlNode nError = _root.SelectSingleNode("s_error_string");
                                    if (nError != null)
                                    {
                                        m_error_desc = nError.InnerText;
                                    }
                                    return MSV_RET.MSV_FAILED;
                                }
                            }
                            else
                            {
                                return MSV_RET.MSV_XMLERROR;
                            }
                        }
                        else
                        {
                            return MSV_RET.MSV_XMLERROR;
                        }
                    }
                    else
                    {
                        return MSV_RET.MSV_NETERROR;
                    }
                }
                catch (System.Exception ex)
                {
                    m_error_desc = "MSVGetTaskListState: " + ex.Message;
                    Logger.Info( m_error_desc);
                    return MSV_RET.MSV_FAILED;
                }
            }
        }

        /************************************************************************/
        /* 启动任务队列                                                         */
        /************************************************************************/
        public MSV_RET MSVStartTaskList(string strMsvIp, int nChannel)
        {
            lock (_msvfunclock)
            {
                try
                {
                    string ip = (strMsvIp != "") ? strMsvIp : m_iCtrlIp;
                    string cmd = string.Format("<?xml version=\"1.0\"?><set_tasklist_state><s_state>{0}</s_state><nChannel>{1}</nChannel></set_tasklist_state>\0", Convert.ToInt32(BATCH_STATE.BS_RUNNING), nChannel);
                    string strRet = m_udpMsv.GetMsvUdpData("", nChannel, ip, nChannel, cmd, Logger);
                    if (strRet != "")
                    {
                        _xml.LoadXml(strRet);
                        XmlElement _root = _xml.DocumentElement;
                        if (_root != null && _root.Name == "std_reply")
                        {
                            XmlNode _retNode = _root.SelectSingleNode("s_result");
                            if (_retNode != null)
                            {
                                string _result = _retNode.InnerText;
                                if (_result == "succeed")
                                {
                                    m_error_desc = "Operation succeed";
                                    return MSV_RET.MSV_SUCCESS;
                                }
                                else
                                {
                                    XmlNode nError = _root.SelectSingleNode("s_error_string");
                                    if (nError != null)
                                    {
                                        m_error_desc = nError.InnerText;
                                    }
                                    return MSV_RET.MSV_FAILED;
                                }
                            }
                            else
                            {
                                return MSV_RET.MSV_XMLERROR;
                            }
                        }
                        else
                        {
                            return MSV_RET.MSV_XMLERROR;
                        }
                    }
                    else
                    {
                        return MSV_RET.MSV_NETERROR;
                    }
                }
                catch (System.Exception ex)
                {
                    m_error_desc = "MSVStartTaskList: " + ex.Message;
                    Logger.Info(m_error_desc);
                    return MSV_RET.MSV_FAILED;
                }
            }
        }
        /************************************************************************/
        /* 停止媒体服务器任务队列                                               */
        /************************************************************************/
        public MSV_RET MSVStopTaskList(string strMsvIp, int nChannel)
        {
            lock (_msvfunclock)
            {
                try
                {
                    string ip = (strMsvIp != "") ? strMsvIp : m_iCtrlIp;
                    string cmd = string.Format("<?xml version=\"1.0\"?><set_tasklist_state><s_state>{0}</s_state><nChannel>{1}</nChannel></set_tasklist_state>\0", Convert.ToInt32(BATCH_STATE.BS_STOP), nChannel);
                    string strRet = m_udpMsv.GetMsvUdpData("", nChannel, ip, nChannel, cmd, Logger);
                    if (strRet != "")
                    {
                        _xml.LoadXml(strRet);
                        XmlElement _root = _xml.DocumentElement;
                        if (_root != null && _root.Name == "std_reply")
                        {
                            XmlNode _retNode = _root.SelectSingleNode("s_result");
                            if (_retNode != null)
                            {
                                string _result = _retNode.InnerText;
                                if (_result == "succeed")
                                {
                                    m_error_desc = "Operation succeed";
                                    return MSV_RET.MSV_SUCCESS;
                                }
                                else
                                {
                                    XmlNode nError = _root.SelectSingleNode("s_error_string");
                                    if (nError != null)
                                    {
                                        m_error_desc = nError.InnerText;
                                    }
                                    return MSV_RET.MSV_FAILED;
                                }
                            }
                            else
                            {
                                return MSV_RET.MSV_XMLERROR;
                            }
                        }
                        else
                        {
                            return MSV_RET.MSV_XMLERROR;
                        }
                    }
                    else
                    {
                        return MSV_RET.MSV_NETERROR;
                    }
                }
                catch (System.Exception ex)
                {
                    m_error_desc = "MSVStopTaskList: " + ex.Message;
                    Logger.Info(m_error_desc);
                    return MSV_RET.MSV_FAILED;
                }
            }
        }
        /*********************************************************************************
            描述: 向媒体服务器设置任务（编单）
        *********************************************************************************/
        public MSV_RET MSVSetTask(string strMsvIp, TASK_ALL_PARAM param, int nChannel)
        {
            if (param == null)
            {
                Logger.Info( "MSVSetTask: param is null!");
                return MSV_RET.MSV_FAILED;
            }
            if (/*param.nCutLen > 60 ||*/ param.nCutLen <= 0)
            {
                m_error_desc = "Segment time of task should be greater than 0 minutes";
                return MSV_RET.MSV_FAILED;
            }
            lock (_msvfunclock)
            {
                try
                {
                    string cmd = FromatParmaToString(param, "set_msv_task", nChannel);
                    string ip = (strMsvIp != "") ? strMsvIp : m_iCtrlIp;
                    string strRet = m_udpMsv.GetMsvUdpData("", nChannel, ip, nChannel, cmd, Logger);
                    if (strRet != "")
                    {
                        _xml.LoadXml(strRet);
                        XmlElement _root = _xml.DocumentElement;
                        if (_root != null && _root.Name == "std_reply")
                        {
                            XmlNode _retNode = _root.SelectSingleNode("s_result");
                            if (_retNode != null)
                            {
                                string _result = _retNode.InnerText;
                                if (_result == "succeed")
                                {
                                    m_error_desc = "Operation succeed";
                                    return MSV_RET.MSV_SUCCESS;
                                }
                                else
                                {
                                    XmlNode nError = _root.SelectSingleNode("s_error_string");
                                    if (nError != null)
                                    {
                                        m_error_desc = nError.InnerText;
                                    }
                                    return MSV_RET.MSV_FAILED;
                                }
                            }
                            else
                            {
                                return MSV_RET.MSV_XMLERROR;
                            }
                        }
                        else
                        {
                            return MSV_RET.MSV_XMLERROR;
                        }
                    }
                    else
                    {
                        return MSV_RET.MSV_NETERROR;
                    }
                }
                catch (System.Exception ex)
                {
                    m_error_desc = "MSVSetTask: " + ex.Message;
                    Logger.Info( m_error_desc);
                    return MSV_RET.MSV_FAILED;
                }
            }
        }

        public MSV_RET MSVSetTaskNew(string strMsvIp, TASK_ALL_PARAM_NEW param, int nChannel)
        {
            if (param == null)
            {
                m_error_desc = "MSVSetTaskNew: param is null";
                Logger.Info( m_error_desc);
                return MSV_RET.MSV_FAILED;
            }
            if (/*param.nCutLen > 60 ||*/ param.nCutLen <= 0)
            {
                m_error_desc = "Segment time of task should be greater than 0 minutes";
                return MSV_RET.MSV_FAILED;
            }
            lock (_msvfunclock)
            {
                try
                {
                    string cmd = FromatParmaToStringNew(param, "set_msv_task", nChannel);
                    string ip = (strMsvIp != "") ? strMsvIp : m_iCtrlIp;
                    string strRet = m_udpMsv.GetMsvUdpData("", nChannel, ip, nChannel, cmd, Logger);
                    if (strRet != "")
                    {
                        _xml.LoadXml(strRet);
                        XmlElement _root = _xml.DocumentElement;
                        if (_root != null && _root.Name == "std_reply")
                        {
                            XmlNode _retNode = _root.SelectSingleNode("s_result");
                            if (_retNode != null)
                            {
                                string _result = _retNode.InnerText;
                                if (_result == "succeed")
                                {
                                    m_error_desc = "Operation succeed";
                                    return MSV_RET.MSV_SUCCESS;
                                }
                                else
                                {
                                    XmlNode nError = _root.SelectSingleNode("s_error_string");
                                    if (nError != null)
                                    {
                                        m_error_desc = nError.InnerText;
                                    }
                                    return MSV_RET.MSV_FAILED;
                                }
                            }
                            else
                            {
                                return MSV_RET.MSV_XMLERROR;
                            }
                        }
                        else
                        {
                            return MSV_RET.MSV_XMLERROR;
                        }
                    }
                    else
                    {
                        return MSV_RET.MSV_NETERROR;
                    }
                }
                catch (System.Exception ex)
                {
                    m_error_desc = "MSVSetTaskNew: " + ex.Message;
                    Logger.Info( m_error_desc);
                    return MSV_RET.MSV_FAILED;
                }
            }
        }
        /**********************************************************************************
            描述: 删除任务
        ***********************************************************************************/
        public MSV_RET MSVDelTask(string strMsvIp, long taskID, int nChannel)
        {
            lock (_msvfunclock)
            {
                try
                {
                    string ip = (strMsvIp != "") ? strMsvIp : m_iCtrlIp;
                    string cmd = string.Format("<?xml version=\"1.0\"?><del_msv_task><i_taskID>{0}</i_taskID><nChannel>{1}</nChannel></del_msv_task>\0", taskID, nChannel);
                    string strRet = m_udpMsv.GetMsvUdpData("", nChannel, ip, nChannel, cmd, Logger);
                    if (strRet != "")
                    {
                        _xml.LoadXml(strRet);
                        XmlElement _root = _xml.DocumentElement;
                        if (_root != null && _root.Name == "std_reply")
                        {
                            XmlNode _retNode = _root.SelectSingleNode("s_result");
                            if (_retNode != null)
                            {
                                string _result = _retNode.InnerText;
                                if (_result == "succeed")
                                {
                                    m_error_desc = "Operation succeed";
                                    return MSV_RET.MSV_SUCCESS;
                                }
                                else
                                {
                                    XmlNode nError = _root.SelectSingleNode("s_error_string");
                                    if (nError != null)
                                    {
                                        m_error_desc = nError.InnerText;
                                    }
                                    return MSV_RET.MSV_FAILED;
                                }
                            }
                            else
                            {
                                return MSV_RET.MSV_XMLERROR;
                            }
                        }
                        else
                        {
                            return MSV_RET.MSV_XMLERROR;
                        }
                    }
                    else
                    {
                        return MSV_RET.MSV_NETERROR;
                    }
                }
                catch (System.Exception ex)
                {
                    m_error_desc = "MSVDelTask: " + ex.Message;
                    Logger.Info( m_error_desc);
                    return MSV_RET.MSV_FAILED;
                }
            }


        }



        /**********************************************************************************
            描述: 修改任务
        ***********************************************************************************/
        public MSV_RET MSVModifyTask(string strMsvIp, TASK_ALL_PARAM param, int nChannel)
        {
            if (param == null)
            {
                m_error_desc = "MSVModifyTask: param is null";
                Logger.Info( "MSVModifyTask: param is null");
                return MSV_RET.MSV_FAILED;
            }
            lock (_msvfunclock)
            {
                try
                {
                    string ip = (strMsvIp != "") ? strMsvIp : m_iCtrlIp;
                    string cmd = FromatParmaToString(param, "update_msv_task", nChannel);
                    string strRet = m_udpMsv.GetMsvUdpData("", nChannel, ip, nChannel, cmd, Logger);
                    if (strRet != "")
                    {
                        _xml.LoadXml(strRet);
                        XmlElement _root = _xml.DocumentElement;
                        if (_root != null && _root.Name == "std_reply")
                        {
                            XmlNode _retNode = _root.SelectSingleNode("s_result");
                            if (_retNode != null)
                            {
                                string _result = _retNode.InnerText;
                                if (_result == "succeed")
                                {
                                    m_error_desc = "Operation succeed";
                                    return MSV_RET.MSV_SUCCESS;
                                }
                                else
                                {
                                    XmlNode nError = _root.SelectSingleNode("s_error_string");
                                    if (nError != null)
                                    {
                                        m_error_desc = nError.InnerText;
                                    }
                                    return MSV_RET.MSV_FAILED;
                                }
                            }
                            else
                            {
                                return MSV_RET.MSV_XMLERROR;
                            }
                        }
                        else
                        {
                            return MSV_RET.MSV_XMLERROR;
                        }
                    }
                    else
                    {
                        return MSV_RET.MSV_NETERROR;
                    }
                }
                catch (System.Exception ex)
                {
                    m_error_desc = "MSVModifyTask: " + ex.Message;
                    Logger.Info( m_error_desc);
                    return MSV_RET.MSV_FAILED;
                }
            }
        }

        public MSV_RET MSVModifyTaskNew(string strMsvIp, TASK_ALL_PARAM_NEW param, int nChannel)
        {
            if (param == null)
            {
                m_error_desc = "MSVModifyTaskNew: param is null";
                Logger.Info( m_error_desc);
                return MSV_RET.MSV_FAILED;
            }
            lock (_msvfunclock)
            {
                try
                {
                    string ip = (strMsvIp != "") ? strMsvIp : m_iCtrlIp;
                    string cmd = FromatParmaToStringNew(param, "update_msv_task", nChannel);
                    string strRet = m_udpMsv.GetMsvUdpData("", nChannel, ip, nChannel, cmd, Logger);
                    if (strRet != "")
                    {
                        _xml.LoadXml(strRet);
                        XmlElement _root = _xml.DocumentElement;
                        if (_root != null && _root.Name == "std_reply")
                        {
                            XmlNode _retNode = _root.SelectSingleNode("s_result");
                            if (_retNode != null)
                            {
                                string _result = _retNode.InnerText;
                                if (_result == "succeed")
                                {
                                    m_error_desc = "Operation succeed";
                                    return MSV_RET.MSV_SUCCESS;
                                }
                                else
                                {
                                    XmlNode nError = _root.SelectSingleNode("s_error_string");
                                    if (nError != null)
                                    {
                                        m_error_desc = nError.InnerText;
                                    }
                                    return MSV_RET.MSV_FAILED;
                                }
                            }
                            else
                            {
                                return MSV_RET.MSV_XMLERROR;
                            }
                        }
                        else
                        {
                            return MSV_RET.MSV_XMLERROR;
                        }
                    }
                    else
                    {
                        return MSV_RET.MSV_NETERROR;
                    }
                }
                catch (System.Exception ex)
                {
                    m_error_desc = "MSVModifyTaskNew: " + ex.Message;
                    Logger.Info(  m_error_desc);
                    return MSV_RET.MSV_FAILED;
                }
            }
        }

        /**********************************************************************************
            描述: 启动一条手动任务
        ***********************************************************************************/
        public MSV_RET MSVStartTask(string strMsvIp, TASK_ALL_PARAM param, int nChannel)
        {
            if (param == null)
            {
                m_error_desc = "MSVStartTask: param is null!";
                Logger.Info(  m_error_desc);
                return MSV_RET.MSV_FAILED;
            }
            if (/*param.nCutLen > 60 || */param.nCutLen <= 0)
            {
                m_error_desc = "Segment time of task should be greater than 0 minutes";
                Logger.Info(  m_error_desc);
                return MSV_RET.MSV_FAILED;
            }
            lock (_msvfunclock)
            {
                try
                {
                    string ip = (strMsvIp != "") ? strMsvIp : m_iCtrlIp;
                    string cmd = FromatParmaToString(param, "start_msv_task", nChannel);
                    string strRet = m_udpMsv.GetMsvUdpData("", nChannel, ip, nChannel, cmd, Logger);
                    if (strRet != "")
                    {
                        _xml.LoadXml(strRet);
                        XmlElement _root = _xml.DocumentElement;
                        if (_root != null && _root.Name == "std_reply")
                        {
                            XmlNode _retNode = _root.SelectSingleNode("s_result");
                            if (_retNode != null)
                            {
                                string _result = _retNode.InnerText;
                                if (_result == "succeed")
                                {
                                    m_error_desc = "Operation succeed";
                                    return MSV_RET.MSV_SUCCESS;
                                }
                                else
                                {
                                    XmlNode nError = _root.SelectSingleNode("s_error_string");
                                    if (nError != null)
                                    {
                                        m_error_desc = nError.InnerText;
                                    }
                                    return MSV_RET.MSV_FAILED;
                                }
                            }
                            else
                            {
                                return MSV_RET.MSV_XMLERROR;
                            }
                        }
                        else
                        {
                            return MSV_RET.MSV_XMLERROR;
                        }
                    }
                    else
                    {
                        return MSV_RET.MSV_NETERROR;
                    }
                }
                catch (System.Exception ex)
                {
                    m_error_desc = "MSVStartTaskp: " + ex.Message;
                    Logger.Info(  m_error_desc);
                    return MSV_RET.MSV_FAILED;
                }
            }

        }

        public MSV_RET MSVStartTaskNew(string strMsvIp, TASK_ALL_PARAM_NEW param, int nChannel)
        {
            if (param == null)
            {
                m_error_desc = "MSVStartTaskNew: param is null";
                Logger.Info(  m_error_desc);
                return MSV_RET.MSV_FAILED;
            }
            //分段时间太长
            if (/*param.nCutLen > 60 || */param.nCutLen <= 0)
            {
                m_error_desc = "Segment time of task should be greater than 0 minutes";
                Logger.Info(  m_error_desc);
                return MSV_RET.MSV_FAILED;
            }
            lock (_msvfunclock)
            {
                try
                {
                    string ip = (strMsvIp != "") ? strMsvIp : m_iCtrlIp;
                    string cmd = FromatParmaToStringNew(param, "start_msv_task", nChannel);
                    string strRet = m_udpMsv.GetMsvUdpData("", nChannel, ip, nChannel, cmd, Logger,10);
                    
                    Logger.Info(  string.Format("taskID:{0}, MSVStartTaskNew strRet:{1}", param.taskParam.ulID, strRet));
                    if (strRet != "")
                    {
                        _xml.LoadXml(strRet);
                        XmlElement _root = _xml.DocumentElement;
                        if (_root != null && _root.Name == "std_reply")
                        {
                            XmlNode _retNode = _root.SelectSingleNode("s_result");
                            if (_retNode != null)
                            {
                                string _result = _retNode.InnerText;
                                if (_result == "succeed")
                                {
                                    m_error_desc = "Operation succeed";
                                    return MSV_RET.MSV_SUCCESS;
                                }
                                else
                                {
                                    XmlNode nError = _root.SelectSingleNode("s_error_string");
                                    if (nError != null)
                                    {
                                        m_error_desc = nError.InnerText;
                                    }
                                    return MSV_RET.MSV_FAILED;
                                }
                            }
                            else
                            {
                                return MSV_RET.MSV_XMLERROR;
                            }
                        }
                        else
                        {
                            return MSV_RET.MSV_XMLERROR;
                        }
                    }
                    else
                    {
                        return MSV_RET.MSV_NETERROR;
                    }
                }
                catch (System.Exception ex)
                {
                    m_error_desc = "MSVStartTaskNew: " + ex.Message;
                    Logger.Info(  m_error_desc);
                    return MSV_RET.MSV_FAILED;
                }
            }

        }

        /**********************************************************************************
            描述: 停止一条手动任务  taskID 返回停止的任务的ID
        ***********************************************************************************/
        public MSV_RET MSVStopTask(string strMsvIp, ref long taskID, long lSendTaskID, int nChannel)
        {
            lock (_msvfunclock)
            {
                try
                {
                    string ip = (strMsvIp != "") ? strMsvIp : m_iCtrlIp;
                    string cmd = string.Format("<?xml version=\"1.0\"?><stop_msv_task><nChannel>{0}</nChannel><nSendTaskID>{1}</nSendTaskID></stop_msv_task>\0", nChannel, lSendTaskID);
                    string strRet = m_udpMsv.GetMsvUdpData("", nChannel, ip, nChannel, cmd, Logger);
                    Logger.Info(  string.Format("taskID:{0}, MSVStopTask strRet:{1}", taskID, strRet));
                    if (strRet != "")
                    {
                        _xml.LoadXml(strRet);
                        XmlElement _root = _xml.DocumentElement;
                        if (_root != null && _root.Name == "std_reply")
                        {
                            XmlNode _retNode = _root.SelectSingleNode("s_result");
                            if (_retNode != null)
                            {
                                string _result = _retNode.InnerText;
                                if (_result == "succeed")
                                {
                                    XmlNode _taskIdNode = _root.SelectSingleNode("s_taskid");
                                    taskID = Convert.ToInt64(_taskIdNode.InnerText);
                                    m_error_desc = "Operation succeed";
                                    return MSV_RET.MSV_SUCCESS;
                                }
                                else
                                {
                                    XmlNode nError = _root.SelectSingleNode("s_error_string");
                                    if (nError != null)
                                    {
                                        m_error_desc = nError.InnerText;
                                    }
                                    return MSV_RET.MSV_FAILED;
                                }
                            }
                            else
                            {
                                return MSV_RET.MSV_XMLERROR;
                            }
                        }
                        else
                        {
                            return MSV_RET.MSV_XMLERROR;
                        }
                    }
                    else
                    {
                        return MSV_RET.MSV_NETERROR;
                    }
                }
                catch (System.Exception ex)
                {
                    m_error_desc = "MSVStopTask: " + ex.Message;
                    Logger.Info(  m_error_desc);
                    return MSV_RET.MSV_FAILED;
                }
            }

        }


        /**********************************************************************************
            描述: 查询手动任务  返回任务的常规参数
        ***********************************************************************************/
        public MSV_RET MSVQueryManauTask(string strMsvIp, ref TASK_PARAM taskParam, int nChannel)
        {
            lock (_msvfunclock)
            {
                try
                {
                    string ip = (strMsvIp != "") ? strMsvIp : m_iCtrlIp;
                    string cmd = string.Format("<?xml version=\"1.0\"?><query_manu_task><nChannel>{0}</nChannel></query_manu_task>\0", nChannel);
                    string strRet = m_udpMsv.GetMsvUdpData("", nChannel, ip, nChannel, cmd, Logger);
                    if (strRet != "")
                    {
                        _xml.LoadXml(strRet);
                        XmlElement _root = _xml.DocumentElement;
                        if (_root != null && _root.Name == "std_reply")
                        {
                            XmlNode _retNode = _root.SelectSingleNode("s_result");
                            if (_retNode != null)
                            {
                                string _result = _retNode.InnerText;
                                if (_result == "succeed")
                                {
                                    XmlNode _taskIdNode = _root.SelectSingleNode("s_taskid");
                                    if (_taskIdNode != null)
                                    {
                                        taskParam.ulID = Convert.ToInt64(_taskIdNode.InnerText);
                                    }
                                    XmlNode _taskNameNode = _root.SelectSingleNode("s_taskname");
                                    if (_taskNameNode != null)
                                    {
                                        taskParam.strName = _taskNameNode.InnerText;
                                    }
                                    XmlNode _taskDescNode = _root.SelectSingleNode("s_taskdesc");
                                    if (_taskDescNode != null)
                                    {
                                        taskParam.strDesc = _taskDescNode.InnerText;
                                    }
                                    XmlNode _beginNode = _root.SelectSingleNode("s_tmbegin");
                                    if (_beginNode != null)
                                    {
                                        taskParam.tmBeg = Convert.ToDateTime(_beginNode.InnerText);
                                    }
                                    XmlNode _EndNode = _root.SelectSingleNode("s_tmEnd");
                                    if (_EndNode != null)
                                    {
                                        taskParam.tmEnd = Convert.ToDateTime(_EndNode.InnerText);
                                    }
                                    XmlNode _bUseTimeNode = _root.SelectSingleNode("s_UseTime");
                                    if (_bUseTimeNode != null)
                                    {
                                        taskParam.bUseTime = Convert.ToBoolean(Convert.ToInt32(_bUseTimeNode.InnerText));
                                    }
                                    XmlNode _inOutCntNode = _root.SelectSingleNode("s_InOutCount");
                                    if (_inOutCntNode != null)
                                    {
                                        taskParam.nInOutCount = Convert.ToInt32(_inOutCntNode.InnerText);
                                    }
                                    if (taskParam.nInOutCount > 100)
                                        taskParam.nInOutCount = 100;
                                    XmlNode _inFrameNode, _outFrameNode;
                                    string strTemp = "";
                                    //返回一组出入点的帧数据信息
                                    for (int j = 0; j < taskParam.nInOutCount; ++j)
                                    {
                                        strTemp = string.Format("s_InFrame{0}", j);
                                        _inFrameNode = _root.SelectSingleNode(strTemp);
                                        if (_inFrameNode != null)
                                        {
                                            taskParam.dwInFrame[j] = Convert.ToUInt64(_inFrameNode.InnerText);
                                        }

                                        strTemp = string.Format("s_OutFrame{0}", j);
                                        _outFrameNode = _root.SelectSingleNode(strTemp);
                                        if (_outFrameNode != null)
                                        {
                                            taskParam.dwOutFrame[j] = Convert.ToUInt64(_outFrameNode.InnerText);
                                        }
                                    }
                                    XmlNode _retrospectNode = _root.SelectSingleNode("s_Retrospect");
                                    if (_retrospectNode != null)
                                    {
                                        taskParam.bRetrospect = Convert.ToBoolean(Convert.ToInt32(_retrospectNode.InnerText));

                                    }
                                    taskParam.TaskMode = TASK_MODE.TM_MANUAL;
                                    m_error_desc = "Operation succeed";
                                    return MSV_RET.MSV_SUCCESS;
                                }
                                else
                                {
                                    XmlNode nError = _root.SelectSingleNode("s_error_string");
                                    if (nError != null)
                                    {
                                        m_error_desc = nError.InnerText;
                                    }
                                    return MSV_RET.MSV_FAILED;
                                }
                            }
                            else
                            {
                                return MSV_RET.MSV_XMLERROR;
                            }
                        }
                        else
                        {
                            return MSV_RET.MSV_XMLERROR;
                        }
                    }
                    else
                    {
                        return MSV_RET.MSV_NETERROR;
                    }
                }
                catch (System.Exception ex)
                {
                    m_error_desc = "MSVQueryManauTask: " + ex.Message;
                    Logger.Info(  m_error_desc);
                    return MSV_RET.MSV_FAILED;
                }
            }

        }
        /************************************************************************/
        /* 暂停MSV采集  要取消暂停状态，请调用MSVPauseCapture                    */
        /************************************************************************/
        public MSV_RET MSVPauseCapture(long taskID, string strMsvIp, int nChannel)
        {
            lock (_msvfunclock)
            {
                try
                {
                    string ip = (strMsvIp != "") ? strMsvIp : m_iCtrlIp;
                    string cmd = string.Format("<?xml version=\"1.0\"?><msv_pause_capture><nChannel>{0}</nChannel><taskid>{1}</taskid></msv_pause_capture>\0", nChannel, taskID);
                    string strRet = m_udpMsv.GetMsvUdpData("", nChannel, ip, nChannel, cmd, Logger);
                    if (strRet != "")
                    {
                        _xml.LoadXml(strRet);
                        XmlElement _root = _xml.DocumentElement;
                        if (_root != null && _root.Name == "std_reply")
                        {
                            XmlNode _retNode = _root.SelectSingleNode("s_result");
                            if (_retNode != null)
                            {
                                string _result = _retNode.InnerText;
                                if (_result == "succeed")
                                {
                                    m_error_desc = "Operation succeed";
                                    return MSV_RET.MSV_SUCCESS;
                                }
                                else
                                {
                                    XmlNode nError = _root.SelectSingleNode("s_error_string");
                                    if (nError != null)
                                    {
                                        m_error_desc = nError.InnerText;
                                    }
                                    return MSV_RET.MSV_FAILED;
                                }
                            }
                            else
                            {
                                return MSV_RET.MSV_XMLERROR;
                            }
                        }
                        else
                        {
                            return MSV_RET.MSV_XMLERROR;
                        }
                    }
                    else
                    {
                        return MSV_RET.MSV_NETERROR;
                    }
                }
                catch (System.Exception ex)
                {
                    m_error_desc = "MSVPauseCapture: " + ex.Message;
                    Logger.Info(  m_error_desc);
                    return MSV_RET.MSV_FAILED;
                }
            }

        }
        /************************************************************************/
        /* 继续MSV采集， 取消暂停状态                                          */
        /************************************************************************/
        public MSV_RET MSVContinueCapture(long taskID, string strMsvIp, int nChannel)
        {
            lock (_msvfunclock)
            {
                try
                {
                    string ip = (strMsvIp != "") ? strMsvIp : m_iCtrlIp;
                    string cmd = string.Format("<?xml version=\"1.0\"?><msv_continue_capture><nChannel>{0}</nChannel><taskid>{1}</taskid></msv_continue_capture>\0",nChannel, taskID);
                    string strRet = m_udpMsv.GetMsvUdpData("", nChannel, ip, nChannel, cmd, Logger);
                    if (strRet != "")
                    {
                        _xml.LoadXml(strRet);
                        XmlElement _root = _xml.DocumentElement;
                        if (_root != null && _root.Name == "std_reply")
                        {
                            XmlNode _retNode = _root.SelectSingleNode("s_result");
                            if (_retNode != null)
                            {
                                string _result = _retNode.InnerText;
                                if (_result == "succeed")
                                {
                                    m_error_desc = "Operation succeed";
                                    return MSV_RET.MSV_SUCCESS;
                                }
                                else
                                {
                                    XmlNode nError = _root.SelectSingleNode("s_error_string");
                                    if (nError != null)
                                    {
                                        m_error_desc = nError.InnerText;
                                    }
                                    return MSV_RET.MSV_FAILED;
                                }
                            }
                            else
                            {
                                return MSV_RET.MSV_XMLERROR;
                            }
                        }
                        else
                        {
                            return MSV_RET.MSV_XMLERROR;
                        }
                    }
                    else
                    {
                        return MSV_RET.MSV_NETERROR;
                    }
                }
                catch (System.Exception ex)
                {
                    m_error_desc = "MSVContinueCapture: " + ex.Message;
                    Logger.Info(  m_error_desc);
                    return MSV_RET.MSV_FAILED;
                }
            }

        }
        /**********************************************************************************
            描述: 查询当前运行任务  返回任务的常规参数
        ***********************************************************************************/
        public MSV_RET MSVQueryRuningTask(string strMsvIp, ref TASK_PARAM taskParam, int nChannel)
        {
            lock (_msvfunclock)
            {
                try
                {
                    string ip = (strMsvIp != "") ? strMsvIp : m_iCtrlIp;
                    string cmd = string.Format("<?xml version=\"1.0\"?><query_runing_task><nChannel>{0}</nChannel></query_runing_task>\0", nChannel);
                    string strRet = m_udpMsv.GetMsvUdpData("", nChannel, ip, nChannel, cmd, Logger);
 
                    
                    if (strRet != "")
                    {
                        _xml.LoadXml(strRet);
                        XmlElement _root = _xml.DocumentElement;
                        if (_root != null && _root.Name == "std_reply")
                        {
                            XmlNode _retNode = _root.SelectSingleNode("s_result");
                            if (_retNode != null)
                            {
                                string _result = _retNode.InnerText;
                                if (_result == "succeed")
                                {
                                    XmlNode _taskIdNode = _root.SelectSingleNode("s_taskid");
                                    if (_taskIdNode != null)
                                    {
                                        taskParam.ulID = Convert.ToInt64(_taskIdNode.InnerText);
                                    }
                                    XmlNode _taskNameNode = _root.SelectSingleNode("s_taskname");
                                    if (_taskNameNode != null)
                                    {
                                        taskParam.strName = _taskNameNode.InnerText;
                                    }
                                    XmlNode _taskDescNode = _root.SelectSingleNode("s_taskdesc");
                                    if (_taskDescNode != null)
                                    {
                                        taskParam.strDesc = _taskDescNode.InnerText;
                                    }
                                    XmlNode _beginNode = _root.SelectSingleNode("s_tmbegin");
                                    if (_beginNode != null)
                                    {
                                        taskParam.tmBeg = Convert.ToDateTime(_beginNode.InnerText);
                                    }
                                    XmlNode _EndNode = _root.SelectSingleNode("s_tmEnd");
                                    if (_EndNode != null)
                                    {
                                        taskParam.tmEnd = Convert.ToDateTime(_EndNode.InnerText);
                                    }
                                    XmlNode _bUseTimeNode = _root.SelectSingleNode("s_UseTime");
                                    if (_bUseTimeNode != null)
                                    {
                                        taskParam.bUseTime = Convert.ToBoolean(Convert.ToInt32(_bUseTimeNode.InnerText));
                                    }
                                    XmlNode _inOutCntNode = _root.SelectSingleNode("s_InOutCount");
                                    if (_inOutCntNode != null)
                                    {
                                        taskParam.nInOutCount = Convert.ToInt32(_inOutCntNode.InnerText);
                                    }
                                    if (taskParam.nInOutCount > 100)
                                        taskParam.nInOutCount = 100;
                                    XmlNode _inFrameNode, _outFrameNode;
                                    string strTemp = "";
                                    //返回一组出入点的帧数据信息
                                    for (int j = 0; j < taskParam.nInOutCount; ++j)
                                    {
                                        strTemp = string.Format("s_InFrame{0}", j);
                                        _inFrameNode = _root.SelectSingleNode(strTemp);
                                        if (_inFrameNode != null)
                                        {
                                            taskParam.dwInFrame[j] = Convert.ToUInt64(_inFrameNode.InnerText);
                                        }

                                        strTemp = string.Format("s_OutFrame{0}", j);
                                        _outFrameNode = _root.SelectSingleNode(strTemp);
                                        if (_outFrameNode != null)
                                        {
                                            taskParam.dwOutFrame[j] = Convert.ToUInt64(_outFrameNode.InnerText);
                                        }
                                    }
                                    XmlNode _retrospectNode = _root.SelectSingleNode("s_Retrospect");
                                    if (_retrospectNode != null)
                                    {
                                        taskParam.bRetrospect = Convert.ToBoolean(Convert.ToInt32(_retrospectNode.InnerText));
                                    }
                                    XmlNode _stateNode = _root.SelectSingleNode("s_TaskState");
                                    if (_stateNode != null)
                                    {
                                        taskParam.TaskState = (TASK_STATE)Convert.ToInt32(_stateNode.InnerText);
                                    }
                                    taskParam.TaskMode = TASK_MODE.TM_MANUAL;
                                    m_error_desc = "Operation succeed";
                                    return MSV_RET.MSV_SUCCESS;
                                }
                                else
                                {
                                    XmlNode nError = _root.SelectSingleNode("s_error_string");
                                    if (nError != null)
                                    {
                                        m_error_desc = nError.InnerText;
                                    }
                                    return MSV_RET.MSV_FAILED;
                                }
                            }
                            else
                            {
                                return MSV_RET.MSV_XMLERROR;
                            }
                        }
                        else
                        {
                            return MSV_RET.MSV_XMLERROR;
                        }
                    }
                    else
                    {
                        return MSV_RET.MSV_NETERROR;
                    }
                    
                }
                catch (System.Exception ex)
                {
                    m_error_desc = "MSVQueryRuningTask: " + ex.Message;
                    Logger.Info(  m_error_desc);
                    return MSV_RET.MSV_FAILED;
                }
            }
           
        }

        /************************************************************************/
        /* 查询任务状态                                                         */
        /************************************************************************/
        public MSV_RET MSVQueryTaskState(string strMsvIp, long taskID, ref TASK_STATE state, int nChannel)
        {
            lock (_msvfunclock)
            {
                try
                {
                    string ip = (strMsvIp != "") ? strMsvIp : m_iCtrlIp;
                    string cmd = string.Format("<?xml version=\"1.0\"?><query_task_state><s_taskid>{0}</s_taskid><nChannel>{1}</nChannel></query_task_state>\0", taskID, nChannel);
                    string strRet = m_udpMsv.GetMsvUdpData("", nChannel, ip, nChannel, cmd, Logger);
                    if (strRet != "")
                    {
                        _xml.LoadXml(strRet);
                        XmlElement _root = _xml.DocumentElement;
                        if (_root != null && _root.Name == "std_reply")
                        {
                            XmlNode _retNode = _root.SelectSingleNode("s_result");
                            if (_retNode != null)
                            {
                                string _result = _retNode.InnerText;
                                if (_result == "succeed")
                                {
                                    XmlNode _stateNode = _root.SelectSingleNode("s_state");
                                    if (_stateNode != null)
                                    {
                                        state = (TASK_STATE)Convert.ToInt32(_stateNode.InnerText);
                                    }
                                    m_error_desc = "Operation succeed";
                                    return MSV_RET.MSV_SUCCESS;
                                }
                                else
                                {
                                    XmlNode nError = _root.SelectSingleNode("s_error_string");
                                    if (nError != null)
                                    {
                                        m_error_desc = nError.InnerText;
                                    }
                                    return MSV_RET.MSV_FAILED;
                                }
                            }
                            else
                            {
                                return MSV_RET.MSV_XMLERROR;
                            }
                        }
                        else
                        {
                            return MSV_RET.MSV_XMLERROR;
                        }
                    }
                    else
                    {
                        return MSV_RET.MSV_NETERROR;
                    }
                }
                catch (System.Exception ex)
                {
                    m_error_desc = "MSVQueryTaskState: " + ex.Message;
                    Logger.Info(  m_error_desc);
                    return MSV_RET.MSV_FAILED;
                }
            }

        }
        public MSV_RET MSVQuerySDIFormat(string strMsvIp, ref SDISignalStatus FormatStatus, ref bool bIsBlack, int nChannel = -1)
        {
            lock (_msvfunclock)
            {
                try
                {
                    string ip = (strMsvIp != "") ? strMsvIp : m_iCtrlIp;
                    string cmd = string.Format("<?xml version=\"1.0\"?><query_SDI_format><nChannel>{0}</nChannel></query_SDI_format>\0", nChannel);
                    string strRet = m_udpMsv.GetMsvUdpData("", nChannel, ip, nChannel, cmd, Logger);
                    if (strRet != "")
                    {
                        _xml.LoadXml(strRet);
                        XmlElement _root = _xml.DocumentElement;
                        if (_root != null && _root.Name == "std_reply")
                        {
                            XmlNode _retNode = _root.SelectSingleNode("s_result");
                            if (_retNode != null)
                            {
                                string _result = _retNode.InnerText;
                                if (_result == "succeed")
                                {
                                    //获取SDI制式
                                    XmlNode _sdiFormatNode = _root.SelectSingleNode("s_SDIFormat");
                                    if (_sdiFormatNode != null)
                                    {
                                        FormatStatus.VideoFormat = (SignalFormat)(Convert.ToInt32(_sdiFormatNode.InnerText));
                                    }
                                    //获取是否是黑场
                                    XmlNode _isBlackNode = _root.SelectSingleNode("s_IsBlack");
                                    if (_isBlackNode != null)
                                    {
                                        bIsBlack = Convert.ToBoolean(Convert.ToInt32(_isBlackNode.InnerText));
                                    }
                                    //获取是否是DropTC
                                    XmlNode _dropTCNode = _root.SelectSingleNode("s_DropTC");
                                    if (_dropTCNode != null)
                                    {
                                        int iDropTC = Convert.ToInt32(_dropTCNode.InnerText);
                                        if (iDropTC == 1)
                                        {
                                            FormatStatus.TCMode = TimeCodeMode.DF;   //是DropTC
                                        }
                                        else if (iDropTC == 0)
                                        {
                                            FormatStatus.TCMode = TimeCodeMode.NDF;  //是NoneDropTC
                                        }
                                        else if (iDropTC == -1)
                                        {
                                            FormatStatus.TCMode = TimeCodeMode.Unknow;
                                        }

                                    }
                                    //获取宽度
                                    XmlNode _widthNode = _root.SelectSingleNode("s_Width");
                                    if (_widthNode != null)
                                    {
                                        FormatStatus.nWidth = Convert.ToInt32(_widthNode.InnerText);
                                    }
                                    XmlNode _heigthNode = _root.SelectSingleNode("s_Height");
                                    if (_heigthNode != null)
                                    {
                                        FormatStatus.nHeight = Convert.ToInt32(_heigthNode.InnerText);
                                    }
                                    XmlNode _frameRateNode = _root.SelectSingleNode("s_FrameRate");
                                    if (_frameRateNode != null)
                                    {
                                        FormatStatus.fFrameRate = float.Parse(_frameRateNode.InnerText);
                                    }
                                    m_error_desc = "Operation succeed";
                                    return MSV_RET.MSV_SUCCESS;
                                }
                                else
                                {
                                    XmlNode nError = _root.SelectSingleNode("s_error_string");
                                    if (nError != null)
                                    {
                                        m_error_desc = nError.InnerText;
                                    }
                                    return MSV_RET.MSV_FAILED;
                                }
                            }
                            else
                            {
                                return MSV_RET.MSV_XMLERROR;
                            }
                        }
                        else
                        {
                            return MSV_RET.MSV_XMLERROR;
                        }
                    }
                    else
                    {
                        return MSV_RET.MSV_NETERROR;
                    }
                }
                catch (System.Exception ex)
                {
                    m_error_desc = "" + ex.Message;
                    Logger.Info(  m_error_desc);
                    return MSV_RET.MSV_FAILED;
                }
            }

        }
        public MSV_RET MSVStartRetrospectTask(string strMsvIp, TASK_ALL_PARAM_NEW param, int nChannel)
        {
            if(param == null)
            {
                m_error_desc = "MSVStartRetrospectTask: param is null";
                Logger.Info(  m_error_desc);
                return MSV_RET.MSV_FAILED;
            }
            if (/*param.nCutLen > 60 || */param.nCutLen <= 0)
            {
                m_error_desc = "Segment time of task should be greater than 0 minutes";
                string strLog = string.Format("MSVStartRetrospectTask(ID:{0},Name:{1}) MSV_FAILED.{2}", param.taskParam.ulID, param.taskParam.strName, m_error_desc);
                Logger.Info(  strLog);
                return MSV_RET.MSV_FAILED;
            }
            lock (_msvfunclock)
            {
                try
                {
                    string ip = (strMsvIp != "") ? strMsvIp : m_iCtrlIp;
                    string cmd = FromatParmaToStringNew(param, "start_msv_retrospecttask", nChannel);
                    string strRet = m_udpMsv.GetMsvUdpData("", nChannel, ip, nChannel, cmd, Logger);
                    if (strRet != "")
                    {
                        _xml.LoadXml(strRet);
                        XmlElement _root = _xml.DocumentElement;
                        if (_root != null && _root.Name == "std_reply")
                        {
                            XmlNode _retNode = _root.SelectSingleNode("s_result");
                            if (_retNode != null)
                            {
                                string _result = _retNode.InnerText;
                                if (_result == "succeed")
                                {
                                    m_error_desc = "Operation succeed";
                                    return MSV_RET.MSV_SUCCESS;
                                }
                                else
                                {
                                    XmlNode nError = _root.SelectSingleNode("s_error_string");
                                    if (nError != null)
                                    {
                                        m_error_desc = nError.InnerText;
                                    }
                                    return MSV_RET.MSV_FAILED;
                                }
                            }
                            else
                            {
                                return MSV_RET.MSV_XMLERROR;
                            }
                        }
                        else
                        {
                            return MSV_RET.MSV_XMLERROR;
                        }
                    }
                    else
                    {
                        return MSV_RET.MSV_NETERROR;
                    }
                }
                catch (System.Exception ex)
                {
                    m_error_desc = "MSVStartRetrospectTask: " + ex.Message;
                    Logger.Info(  m_error_desc);
                    return MSV_RET.MSV_FAILED;
                }
            }

        }
        //手动打点
        public MSV_RET SetEssenceMark(string strMSVIP, ref MANUALKEYFRAME KeyFrmParam)
        {
            lock (_msvfunclock)
            {
                try
                {
                    string ip = (strMSVIP != "") ? strMSVIP : m_iCtrlIp;
                    string cmd = "<Set_MSV_EssenceMark></Set_MSV_EssenceMark>\0";
                    string strRet = m_udpMsv.GetMsvUdpData("", m_iCtrlPort, ip, m_iCtrlPort, cmd, Logger);
                    if (strRet != "")
                    {
                        _xml.LoadXml(strRet);
                        XmlElement _root = _xml.DocumentElement;
                        if (_root != null && _root.Name == "std_reply")
                        {
                            XmlNode _retNode = _root.SelectSingleNode("s_result");
                            if (_retNode != null)
                            {
                                string _result = _retNode.InnerText;
                                if (_result == "succeed")
                                {
                                    XmlNode _frameNoNode = _root.SelectSingleNode("TaskFrmNo");
                                    if(_frameNoNode != null)
                                    {
                                        KeyFrmParam.dwTaskFrameNo = Convert.ToInt64(_frameNoNode.InnerText);
                                    }
                                    XmlNode _tcNode = _root.SelectSingleNode("TC");
                                    if(_tcNode != null)
                                    {
                                        KeyFrmParam.dwTimeCode = Convert.ToInt64(_tcNode.InnerText);
                                    }
                                    XmlNode _widthNode = _root.SelectSingleNode("Width");
                                    if(_tcNode != null)
                                    {
                                        KeyFrmParam.dwWidth = Convert.ToInt64(_widthNode.InnerText);
                                    }
                                    XmlNode _heigth = _root.SelectSingleNode("Height");
                                    if(_heigth != null)
                                    {
                                        KeyFrmParam.dwHeight = Convert.ToInt64(_heigth.InnerText);
                                    }
                                    XmlNode _bitDepthNode = _root.SelectSingleNode("BitDepth");
                                    if(_bitDepthNode != null)
                                    {
                                        KeyFrmParam.dwBitDepth = Convert.ToInt64(_bitDepthNode.InnerText);
                                    }
                                    m_error_desc = "Operation succeed";
                                    return MSV_RET.MSV_SUCCESS;
                                }
                                else
                                {
                                    XmlNode nError = _root.SelectSingleNode("s_error_string");
                                    if (nError != null)
                                    {
                                        m_error_desc = nError.InnerText;
                                    }
                                    return MSV_RET.MSV_FAILED;
                                }
                            }
                            else
                            {
                                return MSV_RET.MSV_XMLERROR;
                            }
                        }
                        else
                        {
                            return MSV_RET.MSV_XMLERROR;
                        }
                    }
                    else
                    {
                        return MSV_RET.MSV_NETERROR;
                    }
                }
                catch (System.Exception ex)
                {
                    m_error_desc = "SetEssenceMark: " + ex.Message;
                    Logger.Info(  m_error_desc);
                    return MSV_RET.MSV_FAILED;
                }
            }

        }
        //发送智能分段命令 by yj
        public MSV_RET MSVSetExceptCutClip(string strMSVIP, bool bEnable)
        {
            lock (_msvfunclock)
            {
                try
                {
                    string ip = (strMSVIP != "") ? strMSVIP : m_iCtrlIp;
                    string cmd = string.Format("<?xml version=\"1.0\"?><Set_MSV_ExceptCut><bEnable>{0}</bEnable></Set_MSV_ExceptCut>\0", Convert.ToInt32(bEnable));
                    string strRet = m_udpMsv.GetMsvUdpData("", m_iCtrlPort, ip, m_iCtrlPort, cmd, Logger);
                    if (strRet != "")
                    {
                        _xml.LoadXml(strRet);
                        XmlElement _root = _xml.DocumentElement;
                        if (_root != null && _root.Name == "std_reply")
                        {
                            XmlNode _retNode = _root.SelectSingleNode("s_result");
                            if (_retNode != null)
                            {
                                string _result = _retNode.InnerText;
                                if (_result == "succeed")
                                {
                                    m_error_desc = "Operation succeed";
                                    return MSV_RET.MSV_SUCCESS;
                                }
                                else
                                {
                                    XmlNode nError = _root.SelectSingleNode("s_error_string");
                                    if (nError != null)
                                    {
                                        m_error_desc = nError.InnerText;
                                    }
                                    return MSV_RET.MSV_FAILED;
                                }
                            }
                            else
                            {
                                return MSV_RET.MSV_XMLERROR;
                            }
                        }
                        else
                        {
                            return MSV_RET.MSV_XMLERROR;
                        }
                    }
                    else
                    {
                        return MSV_RET.MSV_NETERROR;
                    }
                }
                catch (System.Exception ex)
                {
                    m_error_desc = "MSVSetExceptCutClip: " + ex.Message;
                    Logger.Info(  m_error_desc);
                    return MSV_RET.MSV_FAILED;
                }
            }

        }
        //查询MSV本地缓存容量
        public MSV_RET MsvQueryLocalStorage(string strMsvIP, ref MSV_LocalStorage LocalStorage, int nChannel)
        {
            lock (_msvfunclock)
            {
                try
                {
                    string ip = (strMsvIP != "") ? strMsvIP : m_iCtrlIp;
                    string cmd = string.Format("<?xml version=\"1.0\"?><msv_query_LocalStorage><nChannel>{0}</nChannel></msv_query_LocalStorage>\0", nChannel);
                    string strRet = m_udpMsv.GetMsvUdpData("", nChannel, ip, nChannel, cmd, Logger);
                    if (strRet != "")
                    {
                        _xml.LoadXml(strRet);
                        XmlElement _root = _xml.DocumentElement;
                        if (_root != null && _root.Name == "std_reply")
                        {
                            XmlNode _retNode = _root.SelectSingleNode("s_result");
                            if (_retNode != null)
                            {
                                string _result = _retNode.InnerText;
                                if (_result == "succeed")
                                {
                                    XmlNode _diskCnNode = _root.SelectSingleNode("DiskCount");
                                    if(_diskCnNode != null)
                                    {
                                        LocalStorage.iDiskCount = Convert.ToInt32(_diskCnNode.InnerText);
                                    }

                                    string strNode = "";
                                    XmlNode _node;
                                    for (int i = 0; i < LocalStorage.iDiskCount; ++i)
                                    {
                                        strNode = string.Format("TotalSpace{0}", i);
                                        _node = _root.SelectSingleNode(strNode);
                                        if (_node != null)
                                        {
                                            LocalStorage.dwTotalSpace[i] = Convert.ToUInt32(_node.InnerText);
                                        }
                                        
                                        strNode = string.Format("FreeSpace{0}", i);
                                        _node = _root.SelectSingleNode(strNode);
                                        if (_node != null)
                                        {
                                            LocalStorage.dwFreeSpace[i] = Convert.ToUInt32(_node.InnerText);
                                        }

                                        strNode = string.Format("Driver{0}", i);
                                        _node = _root.SelectSingleNode(strNode);
                                        if (_node != null)
                                        {
                                            LocalStorage.strDisk[i] = _node.InnerText;
                                        }
                                    }

                                    m_error_desc = "Operation succeed";
                                    return MSV_RET.MSV_SUCCESS;
                                }
                                else
                                {
                                    XmlNode nError = _root.SelectSingleNode("s_error_string");
                                    if (nError != null)
                                    {
                                        m_error_desc = nError.InnerText;
                                    }
                                    return MSV_RET.MSV_FAILED;
                                }
                            }
                            else
                            {
                                return MSV_RET.MSV_XMLERROR;
                            }
                        }
                        else
                        {
                            return MSV_RET.MSV_XMLERROR;
                        }
                    }
                    else
                    {
                        return MSV_RET.MSV_NETERROR;
                    }
                }
                catch (System.Exception ex)
                {
                    m_error_desc = "MsvQueryLocalStorage:" + ex.Message;
                    Logger.Info(  m_error_desc);
                    return MSV_RET.MSV_FAILED;
                }
            }

        }
        //根据传入得盘符，获得网络状态
        public MSV_RET MsvQueryNetDriverStatus(string  strMsvIp, string  strDisk, ref bool bStatus, int nChannel)
        {
            lock (_msvfunclock)
            {
                try
                {
                    string ip = (strMsvIp != "") ? strMsvIp : m_iCtrlIp;
                    string cmd = string.Format("<?xml version=\"1.0\"?><msv_NetDriver_Status><disk>{0}</disk><nChannel>{1}</nChannel></msv_NetDriver_Status>\0", strDisk.Substring(0, 1), nChannel);
                    string strRet = m_udpMsv.GetMsvUdpData("", nChannel, ip, nChannel, cmd, Logger);
                    if (strRet != "")
                    {
                        _xml.LoadXml(strRet);
                        XmlElement _root = _xml.DocumentElement;
                        if (_root != null && _root.Name == "std_reply")
                        {
                            XmlNode _retNode = _root.SelectSingleNode("s_result");
                            if (_retNode != null)
                            {
                                string _result = _retNode.InnerText;
                                if (_result == "succeed")
                                {
                                    XmlNode _stateNode = _root.SelectSingleNode("Status");
                                    if(_stateNode != null)
                                    {
                                        bStatus = Convert.ToBoolean(Convert.ToInt32(_stateNode.InnerText));
                                    }
                                    m_error_desc = "Operation succeed";
                                    return MSV_RET.MSV_SUCCESS;
                                }
                                else
                                {
                                    XmlNode nError = _root.SelectSingleNode("s_error_string");
                                    if (nError != null)
                                    {
                                        m_error_desc = nError.InnerText;
                                    }
                                    return MSV_RET.MSV_FAILED;
                                }
                            }
                            else
                            {
                                return MSV_RET.MSV_XMLERROR;
                            }
                        }
                        else
                        {
                            return MSV_RET.MSV_XMLERROR;
                        }
                    }
                    else
                    {
                        return MSV_RET.MSV_NETERROR;
                    }
                }
                catch (System.Exception ex)
                {
                    m_error_desc = "MsvQueryNetDriverStatus: " + ex.Message;
                    Logger.Info(  m_error_desc);
                    return MSV_RET.MSV_FAILED;
                }
            }

        }
        public MSV_RET MSVQueryDBEChannel(string strMsvIP, ref ulong dwDBEChannel, int nChannel = -1)
        {
            lock (_msvfunclock)
            {
                try
                {
                    string ip = (strMsvIP != "") ? strMsvIP : m_iCtrlIp;
                    string cmd = string.Format("<?xml version=\"1.0\"?><msv_query_DBEChannel><nChannel>{0}</nChannel></msv_query_DBEChannel>\0", nChannel);
                    string strRet = m_udpMsv.GetMsvUdpData("", nChannel, ip, nChannel, cmd, Logger);
                    if (strRet != "")
                    {
                        _xml.LoadXml(strRet);
                        XmlElement _root = _xml.DocumentElement;
                        if (_root != null && _root.Name == "std_reply")
                        {
                            XmlNode _retNode = _root.SelectSingleNode("s_result");
                            if (_retNode != null)
                            {
                                string _result = _retNode.InnerText;
                                if (_result == "succeed")
                                {
                                    XmlNode DBEChannelNode = _root.SelectSingleNode("DBEChannel");
                                    if(DBEChannelNode != null)
                                    {
                                        dwDBEChannel = Convert.ToUInt64(DBEChannelNode.InnerText);
                                    }
                                    m_error_desc = "Operation succeed";
                                    return MSV_RET.MSV_SUCCESS;
                                }
                                else
                                {
                                    XmlNode nError = _root.SelectSingleNode("s_error_string");
                                    if (nError != null)
                                    {
                                        m_error_desc = nError.InnerText;
                                    }
                                    return MSV_RET.MSV_FAILED;
                                }
                            }
                            else
                            {
                                return MSV_RET.MSV_XMLERROR;
                            }
                        }
                        else
                        {
                            return MSV_RET.MSV_XMLERROR;
                        }
                    }
                    else
                    {
                        return MSV_RET.MSV_NETERROR;
                    }
                }
                catch (System.Exception ex)
                {
                    m_error_desc = "MSVQueryDBEChannel: " + ex.Message;
                    Logger.Info(  m_error_desc);
                    return MSV_RET.MSV_FAILED;
                }
            }

        }
        /*
        public MSV_RET MSV_GetMSV_RestartTime(LPCTSTR strTSIp,const int nTSPort, time_t & dwRestartTickCount, time_t & dwEndRestartTickCount )
        {
            //IPTS的已经不用该函数，直接返失败。
            return MSV_FAILED;

            if(MSV_SUCCESS!=MSV_GetMSV_RestartTime_fromMSV(strTSIp,dwRestartTickCount,dwEndRestartTickCount))
            {
                return MSV_GetMSV_RestartTime_fromCHS(strTSIp,nTSPort,dwRestartTickCount,dwEndRestartTickCount);
            }
            return MSV_SUCCESS;
        }
        */
        // // 函数名称：MSG_NetPreview,打开，关闭netpreview
        // //     
        // // 参数：
        // //    - strMSVIP 
        // //    - nOperaType: 0/1/2
        // //    = nPort[out]:一般为5555
        // //    = nState[out]:0/1
        // // 返回：
        // //     
        // // 说明：
        // //     
        // //-----------------------------------------------------------
        public MSV_RET MSG_NetPreview(string strMSVIP,int nOperaType, ref int nPort, ref int nState)
        {
            lock (_msvfunclock)
            {
                try
                {
                    string ip = (strMSVIP != "") ? strMSVIP : m_iCtrlIp;
                    string cmd = "<?xml version=\"1.0\"?>";
                    switch (nOperaType)
                    {
                        case 0:
                            {
                                cmd += "<stop_tcppreview>";
                                cmd += "</stop_tcppreview>";
                              
                            }
                            break;
                        case 1:
                            {
                                cmd += "<start_tcppreview>";
                                cmd += "</start_tcppreview>";

                            }
                            break;
                        case 2:
                            {
                                cmd += "<get_tcppreview_info>";
                                cmd += "</get_tcppreview_info>";
                            }
                            break;
                        default:
                            break;
                    }
                    string strRet = m_udpMsv.GetMsvUdpData("", m_iCtrlPort, ip, m_iCtrlPort, cmd, Logger);
                    if (strRet != "")
                    {
                        _xml.LoadXml(strRet);
                        XmlElement _root = _xml.DocumentElement;
                        if (_root != null && _root.Name == "std_reply")
                        {
                            XmlNode _retNode = _root.SelectSingleNode("s_result");
                            if (_retNode != null)
                            {
                                string _result = _retNode.InnerText;
                                if (_result == "succeed")
                                {
                                    XmlNode _portNode = _root.SelectSingleNode("Port");
                                    if(_portNode != null)
                                    {
                                        nPort = Convert.ToInt32(_portNode.InnerText);
                                    }
                                    XmlNode _stateNode = _root.SelectSingleNode("State");
                                    if(_stateNode != null)
                                    {
                                        nState = Convert.ToInt32(_stateNode.InnerText);
                                    }
                                    m_error_desc = "Operation succeed";
                                    return MSV_RET.MSV_SUCCESS;
                                }
                                else
                                {
                                    XmlNode nError = _root.SelectSingleNode("s_error_string");
                                    if (nError != null)
                                    {
                                        m_error_desc = nError.InnerText;
                                    }
                                    return MSV_RET.MSV_FAILED;
                                }
                            }
                            else
                            {
                                return MSV_RET.MSV_XMLERROR;
                            }
                        }
                        else
                        {
                            return MSV_RET.MSV_XMLERROR;
                        }
                    }
                    else
                    {
                        return MSV_RET.MSV_NETERROR;
                    }
                }
                catch (System.Exception ex)
                {
                    m_error_desc = "MSG_NetPreview: " + ex.Message;
                    Logger.Info(  m_error_desc);
                    return MSV_RET.MSV_FAILED;
                }
            }

        }
        
        public MSV_RET MSV_RelocateRTMP(string  strMsvIp, int nPort, string  lpStrLocalIP)
        {
            lock (_msvfunclock)
            {
                try
                {
                    string ip = (strMsvIp != "") ? strMsvIp : m_iCtrlIp;
                    string cmd = string.Format("<?xml version =\"1.0\"?><Relocate><Port>{0}</Port><TargetIP>{1}</TargetIP><LocalIP><![CDATA[{2}]]></LocalIP><PgmID>-1</PgmID><bUdp>0</bUdp></Relocate>", nPort, strMsvIp, lpStrLocalIP);
                    string strRet = m_udpMsv.GetMsvUdpData("", nPort, ip, nPort, cmd, Logger);
                    if (strRet != "")
                    {
                        _xml.LoadXml(strRet);
                        XmlElement _root = _xml.DocumentElement;
                        if (_root != null && _root.Name == "std_reply")
                        {
                            XmlNode _retNode = _root.SelectSingleNode("s_result");
                            if (_retNode != null)
                            {
                                string _result = _retNode.InnerText;
                                if (_result == "succeed")
                                {
                                    m_error_desc = "Operation succeed";
                                    return MSV_RET.MSV_SUCCESS;
                                }
                                else
                                {
                                    XmlNode nError = _root.SelectSingleNode("s_error_string");
                                    if (nError != null)
                                    {
                                        m_error_desc = nError.InnerText;
                                    }
                                    return MSV_RET.MSV_FAILED;
                                }
                            }
                            else
                            {
                                return MSV_RET.MSV_XMLERROR;
                            }
                        }
                        else
                        {
                            return MSV_RET.MSV_XMLERROR;
                        }
                    }
                    else
                    {
                        return MSV_RET.MSV_NETERROR;
                    }
                }
                catch (System.Exception ex)
                {
                    m_error_desc = "MSV_Relocate: " + ex.Message;
                    Logger.Info(  m_error_desc);
                    return MSV_RET.MSV_FAILED;
                }
            }

        }
        // //该接口暂时未用
        public MSV_RET MSV_Relocate(long taskID, string  strTSIp,int nTSPort, string lpStrTargetIP,int nPort, string lpStrLocalIP,ref int nAnalyzeID)
        {
            lock (_msvfunclock)
            {
                try
                {
                    string ip = (strTSIp != "") ? strTSIp : m_iCtrlIp;
                    string cmd = string.Format("<?xml version =\"1.0\"?><Relocate><Port>{0}</Port><TargetIP>{1}</TargetIP><LocalIP>{2}</LocalIP><PgmID>{3}</PgmID><taskID>{4}</taskID></Relocate>", nPort, lpStrTargetIP, lpStrLocalIP, nAnalyzeID, taskID);
                    string strRet = m_udpMsv.GetMsvUdpData("", nPort, ip, nPort, cmd, Logger);
                    if (strRet != "")
                    {
                        _xml.LoadXml(strRet);
                        XmlElement _root = _xml.DocumentElement;
                        if (_root != null && _root.Name == "std_reply")
                        {
                            XmlNode _retNode = _root.SelectSingleNode("s_result");
                            if (_retNode != null)
                            {
                                string _result = _retNode.InnerText;
                                if (_result == "succeed")
                                {
                                    XmlNode AnalyzeIDNode = _root.SelectSingleNode("AnalyzeID");
                                    if(AnalyzeIDNode != null)
                                    {
                                        nAnalyzeID = Convert.ToInt32(AnalyzeIDNode.InnerText);
                                    }
                                    m_error_desc = "Operation succeed";
                                    return MSV_RET.MSV_SUCCESS;
                                }
                                else
                                {
                                    XmlNode nError = _root.SelectSingleNode("s_error_string");
                                    if (nError != null)
                                    {
                                        m_error_desc = nError.InnerText;
                                    }
                                    return MSV_RET.MSV_FAILED;
                                }
                            }
                            else
                            {
                                return MSV_RET.MSV_XMLERROR;
                            }
                        }
                        else
                        {
                            return MSV_RET.MSV_XMLERROR;
                        }
                    }
                    else
                    {
                        return MSV_RET.MSV_NETERROR;
                    }
                }
                catch (System.Exception ex)
                {
                    m_error_desc = "MSV_Relocate: " + ex.Message;
                    Logger.Info(  m_error_desc);
                    return MSV_RET.MSV_FAILED;
                }
            }

        }
        public MSV_RET MSVStartRetrospectTask(string strMsvIp, TASK_ALL_PARAM param, int nChannel)
        {
            if (param == null)
            {
                m_error_desc = "MSVStartRetrospectTask: param is null";
                Logger.Info(  m_error_desc);
                return MSV_RET.MSV_FAILED;
            }
            if (/*param.nCutLen > 60 || */param.nCutLen <= 0)
            {
                m_error_desc = "Segment time of task should be greater than 0 minutes";
                string strLog = string.Format("MSVStartRetrospectTask(ID:{0},Name:{1}) MSV_FAILED.{2}", param.taskParam.ulID, param.taskParam.strName, m_error_desc);
                Logger.Info(  strLog);
                return MSV_RET.MSV_FAILED;
            }
            lock (_msvfunclock)
            {
                try
                {
                    string ip = (strMsvIp != "") ? strMsvIp : m_iCtrlIp;
                    string cmd = FromatParmaToString(param, "start_msv_retrospecttask", nChannel);
                    string strRet = m_udpMsv.GetMsvUdpData("", nChannel, ip, nChannel, cmd, Logger);
                    if (strRet != "")
                    {
                        _xml.LoadXml(strRet);
                        XmlElement _root = _xml.DocumentElement;
                        if (_root != null && _root.Name == "std_reply")
                        {
                            XmlNode _retNode = _root.SelectSingleNode("s_result");
                            if (_retNode != null)
                            {
                                string _result = _retNode.InnerText;
                                if (_result == "succeed")
                                {
                                    m_error_desc = "Operation succeed";
                                    return MSV_RET.MSV_SUCCESS;
                                }
                                else
                                {
                                    XmlNode nError = _root.SelectSingleNode("s_error_string");
                                    if (nError != null)
                                    {
                                        m_error_desc = nError.InnerText;
                                    }
                                    return MSV_RET.MSV_FAILED;
                                }
                            }
                            else
                            {
                                return MSV_RET.MSV_XMLERROR;
                            }
                        }
                        else
                        {
                            return MSV_RET.MSV_XMLERROR;
                        }
                    }
                    else
                    {
                        return MSV_RET.MSV_NETERROR;
                    }
                }
                catch (System.Exception ex)
                {
                    m_error_desc = "MSVStartRetrospectTask: " + ex.Message;
                    Logger.Info(  m_error_desc);
                    return MSV_RET.MSV_FAILED;
                }
            }
        }

        public MSV_RET MSVSetMulDestPath(string strMSVIP, string strMulDestPathXML)
        {
            lock (_msvfunclock)
            {
                try
                {
                    string ip = (strMSVIP != "") ? strMSVIP : m_iCtrlIp;
                    string cmd = string.Format("<?xml version =\"1.0\"?><set_muldest_path>{0}</set_muldest_path>\0", strMulDestPathXML);
                    string strRet = m_udpMsv.GetMsvUdpData("", m_iCtrlPort, ip, m_iCtrlPort, cmd, Logger);
                    if (strRet != "")
                    {
                        _xml.LoadXml(strRet);
                        XmlElement _root = _xml.DocumentElement;
                        if (_root != null && _root.Name == "std_reply")
                        {
                            XmlNode _retNode = _root.SelectSingleNode("s_result");
                            if (_retNode != null)
                            {
                                string _result = _retNode.InnerText;
                                if (_result == "succeed")
                                {
                                    m_error_desc = "Operation succeed";
                                    return MSV_RET.MSV_SUCCESS;
                                }
                                else
                                {
                                    XmlNode nError = _root.SelectSingleNode("s_error_string");
                                    if (nError != null)
                                    {
                                        m_error_desc = nError.InnerText;
                                    }
                                    return MSV_RET.MSV_FAILED;
                                }
                            }
                            else
                            {
                                return MSV_RET.MSV_XMLERROR;
                            }
                        }
                        else
                        {
                            return MSV_RET.MSV_XMLERROR;
                        }
                    }
                    else
                    {
                        return MSV_RET.MSV_NETERROR;
                    }
                }
                catch (System.Exception ex)
                {
                    m_error_desc = "MSVSetMulDestPath: " + ex.Message;
                    Logger.Info(  m_error_desc);
                    return MSV_RET.MSV_FAILED;
                }
            }

        }
        // 
        //     /********************************************************************************
        //         得到所有任务
        //     ********************************************************************************/
        //     MSV_RET CClientTaskSDKImp::MSVGetAllITask(const CString strMsvIp,
        //                                            CArray<long, long&>& taskList,
        //                                            int nChannel)
        //     {
        //         IRemoteSDK* m_s;
        //         m_s = CreateRemoteSDK();
        //         //	CRemoteSocket m_s(m_nTimeOut);
        // 
        //         if (FALSE == SocketConnect(strMsvIp, m_s))
        //         {
        //             CString strError = _T("Error while connecting MSV[%s]");
        //             m_error_desc.Format(strError, strMsvIp);
        //             //NMTrace(m_strLogName, logLevelWarring, m_error_desc);
        //             return MSV_FAILED;
        //         }
        // 
        //         CString strSend(MXL_HEADER);
        //         /************************************************************/
        //         strSend += _T("<get_all_task>");
        //         CString strChannel;
        //         strChannel.Format(_T("<nChannel>%d</nChannel>"), nChannel);
        //         strSend += strChannel;
        //         strSend += _T("</get_all_task>");
        //         /************************************************************/
        // 
        //         CXMLParser parser;
        //         if (FALSE == parser.Create())
        //         {
        //             CString strError = _T("Error occur while analyzing response message of MSV[%s]");
        //             m_error_desc.Format(strError, strMsvIp);
        //             //NMTrace(m_strLogName, logLevelWarring, m_error_desc);
        //             return MSV_FAILED;
        //         }
        //         InParam pIn;
        //         OutParam pOut;
        //         CString strRecv;
        //         pIn.m_s = m_s;
        //         //	pIn.m_s = &m_s;
        //         pIn.parser = &parser;
        //         pIn.strIp = strMsvIp;
        //         pIn.strOrigin = _T("get_all_task");
        //         pIn.strSend = strSend;
        //         pOut.strRecv = strRecv;
        // 
        //         int nRet = CommandOperation(pIn, pOut);
        //         if (nRet == -2)
        //             return MSV_NETERROR;
        //         else if (nRet == -1)
        //             return MSV_XMLERROR;
        //         else if (nRet == 0)
        //             return MSV_FAILED;
        // 
        // 
        //         CString strItem, strContent;
        // 
        // 
        //         strContent = parser.GetNodeText(_T("s_task"));
        //         GetTaskListFromString(strContent, taskList);
        // 
        //         //m_error_desc = _T("操作成功");
        //         m_error_desc = _T("Operation succeed");
        //         return MSV_SUCCESS;
        //     }
        // 
        //     /********************************************************************************
        //         删除所有编单任务
        //     ********************************************************************************/
        //     MSV_RET CClientTaskSDKImp::MSVDelAllTask(const CString strMsvIp,
        //                                           int nChannel)
        //     {
        //         IRemoteSDK* m_s;
        //         m_s = CreateRemoteSDK();
        //         //	CRemoteSocket m_s(m_nTimeOut);
        // 
        //         if (FALSE == SocketConnect(strMsvIp, m_s))
        //         {
        //             CString strError = _T("Error while connecting MSV[%s]");
        //             m_error_desc.Format(strError, strMsvIp);
        //             //NMTrace(m_strLogName, logLevelWarring, m_error_desc);
        //             return MSV_FAILED;
        //         }
        // 
        //         CString strSend(MXL_HEADER);
        //         /************************************************************/
        //         strSend += _T("<del_all_task>");
        //         CString strChannel;
        //         strChannel.Format(_T("<nChannel>%d</nChannel>"), nChannel);
        //         strSend += strChannel;
        //         strSend += _T("</del_all_task>");
        //         /************************************************************/
        // 
        //         CXMLParser parser;
        //         if (FALSE == parser.Create())
        //         {
        //             CString strError = _T("Error occur while analyzing response message of MSV[%s]");
        //             m_error_desc.Format(strError, strMsvIp);
        //             //NMTrace(m_strLogName, logLevelWarring, m_error_desc);
        //             return MSV_FAILED;
        //         }
        //         InParam pIn;
        //         OutParam pOut;
        //         CString strRecv;
        //         pIn.m_s = m_s;
        //         //	pIn.m_s = &m_s;
        //         pIn.parser = &parser;
        //         pIn.strIp = strMsvIp;
        //         pIn.strOrigin = _T("del_all_task");
        //         pIn.strSend = strSend;
        //         pOut.strRecv = strRecv;
        // 
        //         int nRet = CommandOperation(pIn, pOut);
        //         if (nRet == -2)
        //             return MSV_NETERROR;
        //         else if (nRet == -1)
        //             return MSV_XMLERROR;
        //         else if (nRet == 0)
        //             return MSV_FAILED;
        // 
        //         //m_error_desc = _T("操作成功");
        //         m_error_desc = _T("Operation succeed");
        //         return MSV_SUCCESS;
        //     }
        //     /**********************************************************************************
        //     描述: 查询任务的参数 
        //     ***********************************************************************************/
        //     MSV_RET CClientTaskSDKImp::MSVGetTaskParam(const CString strMsvIp,
        //     										const long taskID,
        //                                             TASK_ALL_PARAM& param,
        //                                             int nChannel)
        //     {
        //         IRemoteSDK* m_s;
        //         m_s = CreateRemoteSDK();
        //         //	CRemoteSocket m_s(m_nTimeOut);
        // 
        //         if (FALSE == SocketConnect(strMsvIp, m_s))
        //         {
        //             CString strError = _T("Error while connecting MSV[%s]");
        //             m_error_desc.Format(strError, strMsvIp);
        //             //NMTrace(m_strLogName, logLevelWarring, m_error_desc);
        //             return MSV_FAILED;
        //         }
        // 
        //         CString strSend(MXL_HEADER);
        //         /************************************************************/
        //         strSend += _T("<get_task_param>");
        //         CString strTemp;
        //         strTemp.Format(_T("<s_taskid>%d</s_taskid>"), taskID);
        //         strSend += strTemp;
        //         CString strChannel;
        //         strChannel.Format(_T("<nChannel>%d</nChannel>"), nChannel);
        //         strSend += strChannel;
        //         strSend += _T("</get_task_param>");
        //         /************************************************************/
        //         CXMLParser parser;
        //         if (FALSE == parser.Create())
        //         {
        //             CString strError = _T("Error occur while analyzing response message of MSV[%s]");
        //             m_error_desc.Format(strError, strMsvIp);
        //             //NMTrace(m_strLogName, logLevelWarring, m_error_desc);
        //             return MSV_FAILED;
        //         }
        //         InParam pIn;
        //         OutParam pOut;
        //         CString strRecv;
        //         pIn.m_s = m_s;
        //         //	pIn.m_s = &m_s;
        //         pIn.parser = &parser;
        //         pIn.strIp = strMsvIp;
        //         pIn.strOrigin = _T("get_task_param");
        //         pIn.strSend = strSend;
        //         pOut.strRecv = strRecv;
        // 
        //         int nRet = CommandOperation(pIn, pOut);
        //         if (nRet == -2)
        //             return MSV_NETERROR;
        //         else if (nRet == -1)
        //             return MSV_XMLERROR;
        //         else if (nRet == 0)
        //             return MSV_FAILED;
        // 
        // 
        //         CString strItem, strContent;
        // 
        // 
        // 
        //         strContent = parser.GetNodeText(_T("ulID"));//任务ID
        //         param.taskParam.ulID = _wtol((LPCTSTR)strContent);
        // 
        //         strContent = parser.GetNodeText(_T("TaskMode"));//任务类型
        //         param.taskParam.TaskMode = (TASK_MODE)(_wtoi((LPCTSTR)strContent));
        // 
        //         strContent = parser.GetNodeText(_T("strName"));//任务名
        //         param.taskParam.strName = strContent;
        // 
        //         strContent = parser.GetNodeText(_T("strDesc"));//任务描述
        //         param.taskParam.strDesc = strContent;
        // 
        //         strContent = parser.GetNodeText(_T("tmBeg")); //任务开始时间
        //         StringToCTime(strContent, param.taskParam.tmBeg);
        // 
        //         strContent = parser.GetNodeText(_T("tmEnd"));//任务结束时间
        //         StringToCTime(strContent, param.taskParam.tmEnd);
        // 
        //         //分段长度
        //         strContent = parser.GetNodeText(_T("cutlen"));
        //         param.nCutLen = _wtoi((LPCTSTR)strContent);
        //         //**************  以下是线路0 参数 ***************
        //         //是否采集视频
        //         strContent = parser.GetNodeText(_T("bPath0"));
        //         param.captureParam.bPath0 = (BOOL)_wtoi((LPCTSTR)strContent);
        //         //是否采集音频
        //         strContent = parser.GetNodeText(_T("bAudio0"));
        //         param.captureParam.bAudio0 = (BOOL)_wtoi((LPCTSTR)strContent);
        //         //是否单独生成音频文件
        //         strContent = parser.GetNodeText(_T("bAlone0"));
        //         param.captureParam.bAlone0 = (BOOL)_wtoi((LPCTSTR)strContent);
        //         //文件名
        //         strContent = parser.GetNodeText(_T("path0FileName"));
        //         param.captureParam.path0FileName = strContent;
        //         //编码线路 编码类型
        //         strContent = parser.GetNodeText(_T("nEncodeType0"));
        //         param.captureParam.nEncodeType0 = (MG_EncodeType)_wtoi((LPCTSTR)strContent);
        //         //编码 操作 类型
        //         strContent = parser.GetNodeText(_T("subEncodeType0"));
        //         param.captureParam.subEncodeType0 = _wtoi((LPCTSTR)strContent);
        //         //视频采集BIT率
        //         strContent = parser.GetNodeText(_T("bit_rate0"));
        //         param.captureParam.bit_rate0 = _wtoi((LPCTSTR)strContent);
        //         //音频格式
        //         strContent = parser.GetNodeText(_T("AVWriteTypeV0"));
        //         param.captureParam.AVWriteTypeV0 = (MG_AVWriteType)_wtoi((LPCTSTR)strContent);
        //         //音频采样率
        //         strContent = parser.GetNodeText(_T("dwSamplesOutA0"));
        //         param.captureParam.dwSamplesOutA0 = _wtoi((LPCTSTR)strContent);
        //         //音频采集格式
        //         strContent = parser.GetNodeText(_T("AudioWriteTypeA0"));
        //         param.captureParam.AudioWriteTypeA0 = (MG_AudioWriteType)_wtoi((LPCTSTR)strContent);
        //         //**************  以下是线路1 参数 ***************
        //         //是否采集视频
        //         strContent = parser.GetNodeText(_T("bPath1"));
        //         param.captureParam.bPath1 = (BOOL)_wtoi((LPCTSTR)strContent);
        //         //是否采集音频
        //         strContent = parser.GetNodeText(_T("bAudio1"));
        //         param.captureParam.bAudio1 = (BOOL)(_wtoi((LPCTSTR)strContent));
        //         //是否单独生成音频文件
        //         strContent = parser.GetNodeText(_T("bAlone1"));
        //         param.captureParam.bAlone1 = (BOOL)(_wtoi((LPCTSTR)strContent));
        //         //文件名
        //         strContent = parser.GetNodeText(_T("path1FileName"));
        //         param.captureParam.path1FileName = strContent;
        //         //编码线路 编码类型
        //         strContent = parser.GetNodeText(_T("nEncodeType1"));
        //         param.captureParam.nEncodeType1 = (MG_EncodeType)(_wtoi((LPCTSTR)strContent));
        //         //编码 操作 类型
        //         strContent = parser.GetNodeText(_T("subEncodeType1"));
        //         param.captureParam.subEncodeType1 = _wtoi((LPCTSTR)strContent);
        //         //视频采集BIT率
        //         strContent = parser.GetNodeText(_T("bit_rate1"));
        //         param.captureParam.bit_rate1 = _wtoi((LPCTSTR)strContent);
        //         //音频格式
        //         strContent = parser.GetNodeText(_T("AVWriteTypeV1"));
        //         param.captureParam.AVWriteTypeV1 = (MG_AVWriteType)_wtoi((LPCTSTR)strContent);
        //         //音频采样率
        //         strContent = parser.GetNodeText(_T("dwSamplesOutA1"));
        //         param.captureParam.dwSamplesOutA1 = _wtoi((LPCTSTR)strContent);
        //         //音频采集格式
        //         strContent = parser.GetNodeText(_T("AudioWriteTypeA1"));
        //         param.captureParam.AudioWriteTypeA1 = (MG_AudioWriteType)_wtoi((LPCTSTR)strContent);
        //         //GOP的P帧数
        //         strContent = parser.GetNodeText(_T("nGOPPFrameCount"));
        //         param.captureParam.nGOPPCount = _wtoi((LPCTSTR)strContent);
        //         //GOP的B帧数
        //         strContent = parser.GetNodeText(_T("nGOPBFrameCount"));
        //         param.captureParam.nGOPBCount = _wtoi((LPCTSTR)strContent);
        //         //迁移模式
        //         strContent = parser.GetNodeText(_T("bUseTransfer"));
        //         param.captureParam.bUseTransfer = (BOOL)_wtoi((LPCTSTR)strContent);
        // 
        //         //
        //         strContent = parser.GetNodeText(_T("bUseTime"));
        //         param.taskParam.bUseTime = (BOOL)_wtoi((LPCTSTR)strContent);
        //         //
        //         //strContent = parser.GetNodeText(_T("dwStartFrame"));
        //         //param.taskParam.dwStartFrame = (DWORD)_wtoi((LPCTSTR)strContent);
        // 
        //         //strContent = parser.GetNodeText(_T("dwTaskTatolFrame"));
        //         //param.taskParam.dwTaskTatolFrame = (DWORD)_wtoi((LPCTSTR)strContent);
        // 
        //         strContent = parser.GetNodeText(_T("nInOutCount"));
        //         param.taskParam.nInOutCount = _wtol((LPCTSTR)strContent);
        //         if (param.taskParam.nInOutCount > 100)
        //             param.taskParam.nInOutCount = 100;
        // 
        //         //返回一组出入点的帧数据信息
        //         for (int j = 0; j < param.taskParam.nInOutCount; ++j)
        //         {
        //             strContent.Format(_T("dwInFrame%d"), j);
        //             strContent = parser.GetNodeText(strContent);
        //             param.taskParam.dwInFrame[j] = _wtol((LPCTSTR)strContent);
        // 
        //             strContent.Format(_T("dwOutFrame%d"), j);
        //             strContent = parser.GetNodeText(strContent);
        //             param.taskParam.dwOutFrame[j] = _wtol((LPCTSTR)strContent);
        //         }
        // 
        // 
        //         //m_error_desc = _T("操作成功");
        //         m_error_desc = _T("Operation succeed");
        //         return MSV_SUCCESS;
        // 
        //     }
        // 
        //     //注册通知消息
        //     MSV_RET CClientTaskSDKImp::RegisterNotify(CString strMsvIp,
        //                                            UINT uiNotifyID,
        //                                            int nChannel)
        //     {
        //         //	CRemoteSocket m_sTrans(m_nTimeOut);
        //         //	CRemoteSocket m_sMSV(m_nTimeOut);
        //         IRemoteSDK* m_sTrans;
        //         IRemoteSDK* m_sMSV;
        //         m_sTrans = CreateRemoteSDK();
        //         m_sMSV = CreateRemoteSDK();
        //         /*
        //             switch(uiNotifyID) {
        //             case 7:
        //             case 8:
        //                 if(FALSE == SocketConnectTrans(strMsvIp,m_s))
        //                 {
        //                     CString strError = _T("Error while connecting MSV[%s]");
        //                 m_error_desc.Format(strError, strMsvIp);
        //                 //NMTrace(m_strLogName, logLevelWarring, m_error_desc);//(_T("连接媒体服务器[%s] 错误"),strMsvIp);
        //                     return MSV_FAILED;
        //                 }
        //                 break;
        //             default:
        //                 if(FALSE == SocketConnect(strMsvIp,m_s))
        //                 {		
        //                     CString strError = _T("Error while connecting MSV[%s]");
        //                 m_error_desc.Format(strError, strMsvIp);
        //                 //NMTrace(m_strLogName, logLevelWarring, m_error_desc);//(_T("连接媒体服务器[%s] 错误"),strMsvIp);
        //                     return MSV_FAILED;
        //                 }
        //                 break;
        //             }
        //         */
        //         if (FALSE == SocketConnectTrans(strMsvIp, m_sTrans))
        //         {
        //             CString strError = _T("Error while connecting MSV[%s]");
        //             m_error_desc.Format(strError, strMsvIp);
        //             //NMTrace(m_strLogName, logLevelWarring, m_error_desc);
        //             return MSV_FAILED;
        //         }
        //         if (FALSE == SocketConnect(strMsvIp, m_sMSV))
        //         {
        //             CString strError = _T("Error while connecting MSV[%s]");
        //             m_error_desc.Format(strError, strMsvIp);
        //             //NMTrace(m_strLogName, logLevelWarring, m_error_desc);
        //             return MSV_FAILED;
        //         }
        // 
        //         CString strSend(MXL_HEADER);
        //         /************************************************************/
        //         strSend += _T("<register_notify>");
        //         CString strTemp;
        //         strTemp.Format(_T("<s_id>%d</s_id>"), uiNotifyID); //任务ID
        //         strSend += strTemp;
        //         CString strChannel;
        //         strChannel.Format(_T("<nChannel>%d</nChannel>"), nChannel);
        //         strSend += strChannel;
        //         strSend += _T("</register_notify>");  //查询手动任务
        //                                               /************************************************************/
        // 
        //         CXMLParser parser;
        //         if (FALSE == parser.Create())
        //         {
        //             CString strError = _T("Error occur while analyzing response message of MSV[%s]");
        //             m_error_desc.Format(strError, strMsvIp);
        //             //NMTrace(m_strLogName, logLevelWarring, m_error_desc);
        //             return MSV_FAILED;
        //         }
        //         InParam pIn;
        //         OutParam pOut;
        //         CString strRecv;
        //         pIn.m_s = m_sMSV;
        //         pIn.parser = &parser;
        //         pIn.strIp = strMsvIp;
        //         pIn.strOrigin = _T("register_notify");
        //         pIn.strSend = strSend;
        //         pOut.strRecv = strRecv;
        // 
        //         int nRet = CommandOperation(pIn, pOut);
        //         if (nRet == -2)
        //             return MSV_NETERROR;
        //         else if (nRet == -1)
        //             return MSV_XMLERROR;
        //         else if (nRet == 0)
        //             return MSV_FAILED;
        // 
        //         strRecv = _T("");
        //         pIn.m_s = m_sTrans;
        //         pIn.parser = &parser;
        //         pIn.strIp = strMsvIp;
        //         pIn.strOrigin = _T("register_notify");
        //         pIn.strSend = strSend;
        //         pOut.strRecv = strRecv;
        // 
        //         nRet = CommandOperation(pIn, pOut);
        //         if (nRet == -2)
        //             return MSV_NETERROR;
        //         else if (nRet == -1)
        //             return MSV_XMLERROR;
        //         else if (nRet == 0)
        //             return MSV_FAILED;
        // 
        //         //m_error_desc = _T("操作成功");
        //         m_error_desc = _T("Operation succeed");
        //         return MSV_SUCCESS;
        //     }
        //     //反注册通知消息
        //     MSV_RET CClientTaskSDKImp::UnregisterNotify(CString strMsvIp,
        //                                              UINT uiNotifyID,
        //                                              CString strPreServerIp,
        //                                              int nChannel)
        //     {
        //         //	CRemoteSocket m_sTrans(m_nTimeOut);
        //         //	CRemoteSocket m_sMSV(m_nTimeOut);
        //         IRemoteSDK* m_sTrans;
        //         IRemoteSDK* m_sMSV;
        //         m_sTrans = CreateRemoteSDK();
        //         m_sMSV = CreateRemoteSDK();
        //         /*
        //         switch(uiNotifyID) {
        //         case 7:
        //         case 8:
        //             if(FALSE == SocketConnectTrans(strMsvIp,m_s))
        //             {
        //                 CString strError = _T("Error while connecting MSV[%s]");
        //             m_error_desc.Format(strError, strMsvIp);
        //             //NMTrace(m_strLogName, logLevelWarring, m_error_desc);//(_T("连接媒体服务器[%s] 错误"),strMsvIp);
        //                 return MSV_FAILED;
        //             }
        //             break;
        //         default:
        //             if(FALSE == SocketConnect(strMsvIp,m_s))
        //             {		
        //                 CString strError = _T("Error while connecting MSV[%s]");
        //             m_error_desc.Format(strError, strMsvIp);
        //             //NMTrace(m_strLogName, logLevelWarring, m_error_desc);//(_T("连接媒体服务器[%s] 错误"),strMsvIp);
        //                 return MSV_FAILED;
        //             }
        //             break;
        //         }
        //     */
        //         if (FALSE == SocketConnectTrans(strMsvIp, m_sTrans))
        //         {
        //             CString strError = _T("Error while connecting MSV[%s]");
        //             m_error_desc.Format(strError, strMsvIp);
        //             //NMTrace(m_strLogName, logLevelWarring, m_error_desc);
        //             return MSV_FAILED;
        //         }
        //         if (FALSE == SocketConnect(strMsvIp, m_sMSV))
        //         {
        //             CString strError = _T("Error while connecting MSV[%s]");
        //             m_error_desc.Format(strError, strMsvIp);
        //             //NMTrace(m_strLogName, logLevelWarring, m_error_desc);
        //             return MSV_FAILED;
        //         }
        // 
        //         CString strSend(MXL_HEADER);
        //         /************************************************************/
        //         strSend += _T("<unregister_notify>");
        //         CString strTemp;
        //         strTemp.Format(_T("<s_id>%d</s_id>"), uiNotifyID); //任务ID
        //         strSend += strTemp;
        //         strTemp.Format(_T("<s_ServerIp>%s</s_ServerIp>"), strPreServerIp);
        //         strSend += strTemp;
        //         CString strChannel;
        //         strChannel.Format(_T("<nChannel>%d</nChannel>"), nChannel);
        //         strSend += strChannel;
        //         strSend += _T("</unregister_notify>");  //查询手动任务
        //                                                 /************************************************************/
        // 
        //         CXMLParser parser;
        //         if (FALSE == parser.Create())
        //         {
        //             CString strError = _T("Error occur while analyzing response message of MSV[%s]");
        //             m_error_desc.Format(strError, strMsvIp);
        //             //NMTrace(m_strLogName, logLevelWarring, m_error_desc);
        //             return MSV_FAILED;
        //         }
        //         InParam pIn;
        //         OutParam pOut;
        //         CString strRecv;
        //         pIn.m_s = m_sMSV;
        //         pIn.parser = &parser;
        //         pIn.strIp = strMsvIp;
        //         pIn.strOrigin = _T("unregister_notify");
        //         pIn.strSend = strSend;
        //         pOut.strRecv = strRecv;
        // 
        //         int nRet = CommandOperation(pIn, pOut);
        //         if (nRet == -2)
        //             return MSV_NETERROR;
        //         else if (nRet == -1)
        //             return MSV_XMLERROR;
        //         else if (nRet == 0)
        //             return MSV_FAILED;
        // 
        //         strRecv = _T("");
        //         pIn.m_s = m_sTrans;
        //         pIn.parser = &parser;
        //         pIn.strIp = strMsvIp;
        //         pIn.strOrigin = _T("unregister_notify");
        //         pIn.strSend = strSend;
        //         pOut.strRecv = strRecv;
        // 
        //         nRet = CommandOperation(pIn, pOut);
        //         if (nRet == -2)
        //             return MSV_NETERROR;
        //         else if (nRet == -1)
        //             return MSV_XMLERROR;
        //         else if (nRet == 0)
        //             return MSV_FAILED;
        // 
        //         //m_error_desc = _T("操作成功");
        //         m_error_desc = _T("Operation succeed");
        //         return MSV_SUCCESS;
        //     }
        //     //反注册全部通知
        // 
        //     MSV_RET CClientTaskSDKImp::UnregisterAllNotify(CString strMsvIp,
        //                                                 CString strPreServerIp,
        //                                                 int nChannel)
        //     {
        //         int nCount = 9, nIndex;
        //         for (nIndex = 0; nIndex < nCount; nIndex++)
        //         {
        //             int nRe = UnregisterNotify(strMsvIp, nIndex, strPreServerIp, nChannel);
        //             if (MSV_SUCCESS != nRe)
        //             {
        //                 return (MSV_RET)nRe;
        //             }
        //         }
        //         /*CRemoteSocket m_s(m_nTimeOut);
        // 
        //         if(FALSE == SocketConnectTrans(strMsvIp,m_s))
        //         {
        //             CString strError = _T("Error while connecting MSV[%s]");
        //             m_error_desc.Format(strError, strMsvIp);
        //             //NMTrace(m_strLogName, logLevelWarring, m_error_desc);//(_T("连接媒体服务器[%s] 错误"),strMsvIp);
        //             return MSV_FAILED;
        //         }
        // 
        //         CString strSend(MXL_HEADER);
        //         CString strTemp;
        //         strTemp.Format(_T("<s_ServerIp>%s</s_ServerIp>"),strPreServerIp);
        //         strSend += _T("<unregister_all_notify>");
        //         strSend += strTemp;
        //         CString strChannel;
        //         strChannel.Format(_T("<nChannel>%d</nChannel>"),nChannel);
        //         strSend += strChannel;
        //         strSend += _T("</unregister_all_notify>");  //查询手动任务
        // 
        //         CXMLParser parser;
        //         if(FALSE == parser.Create())
        //         {
        //             CString strError = _T("Error occur while analyzing response message of MSV[%s]");
        //             m_error_desc.Format(strError, strMsvIp);
        //             //NMTrace(m_strLogName, logLevelWarring, m_error_desc);//(_T("(解析媒体服务器[%s] 回应消息错误)"),strMsvIp);
        //             return MSV_FAILED;
        //         }
        //         InParam pIn;
        //         OutParam pOut;
        //         CString  strRecv;
        //         pIn.m_s = m_s;
        //         //	pIn.m_s = &m_s;
        //         pIn.parser = &parser;
        //         pIn.strIp = strMsvIp;
        //         pIn.strOrigin = _T("unregister_all_notify");
        //         pIn.strSend   = strSend;
        //         pOut.strRecv =  strRecv;
        // 
        //         int nRet = CommandOperation(pIn,pOut);
        //         if(nRet == -2)
        //             return MSV_NETERROR;
        //         else if(nRet == -1)
        //             return MSV_XMLERROR;
        //         else if(nRet == 0)
        //             return MSV_FAILED;
        //         */
        // 
        //         //m_error_desc = _T("操作成功");
        //         m_error_desc = _T("Operation succeed");
        //         return MSV_SUCCESS;
        //     }
        // 
        //     /************************************************************************
        //     设置本地任务采集参数  
        //     ************************************************************************/
        //     MSV_RET CClientTaskSDKImp::MSVSetLocalCaptureParam(const CString strMsvIp,
        //     												const TASK_ALL_PARAM param,
        //                                                     int nChannel)
        //     {
        // 
        //         //分段时间太长
        //         if (param.nCutLen <= 0)
        //         {
        //             m_error_desc = _T("Segment time of task should be greater than 0 minutes");
        //             //NMTrace(m_strLogName, logLevelWarring, m_error_desc);
        //             return MSV_FAILED;
        //         }
        // 
        //         IRemoteSDK* m_s;
        //         m_s = CreateRemoteSDK();
        //         //	CRemoteSocket m_s(m_nTimeOut);
        // 
        //         if (FALSE == SocketConnect(strMsvIp, m_s))
        //         {
        //             CString strError = _T("Error while connecting MSV[%s]");
        //             m_error_desc.Format(strError, strMsvIp);
        //             //NMTrace(m_strLogName, logLevelWarring, m_error_desc);
        //             return MSV_FAILED;
        //         }
        //         /*格式化参数到XML文本*/
        //         CString strSend;
        // 
        //         strSend = FromatParmaToString(param, _T("set_local_param"), nChannel);
        // 
        //         /* 操作   */
        //         CXMLParser parser;
        //         if (FALSE == parser.Create())
        //         {
        //             CString strError = _T("Error occur while analyzing response message of MSV[%s]");
        //             m_error_desc.Format(strError, strMsvIp);
        //             //NMTrace(m_strLogName, logLevelWarring, m_error_desc);//(_T("(解析媒体服务器[%s] 回应消息错误)"),strMsvIp);
        //             return MSV_FAILED;
        //         }
        //         InParam pIn;
        //         OutParam pOut;
        //         CString strRecv;
        //         pIn.m_s = m_s;
        //         //	pIn.m_s = &m_s;
        //         pIn.parser = &parser;
        //         pIn.strIp = strMsvIp;
        //         pIn.strOrigin = _T("set_local_param");
        //         pIn.strSend = strSend;
        //         pOut.strRecv = strRecv;
        // 
        //         int nRet = CommandOperation(pIn, pOut);
        //         if (nRet == -2)
        //             return MSV_NETERROR;
        //         else if (nRet == -1)
        //             return MSV_XMLERROR;
        //         else if (nRet == 0)
        //             return MSV_FAILED;
        // 
        //         //m_error_desc = _T("操作成功");
        //         m_error_desc = _T("Operation succeed");
        //         return MSV_SUCCESS;
        //     }
        //     MSV_RET CClientTaskSDKImp::MSVSetLocalCaptureParamNew(const CString strMsvIp,
        //     		                            const TASK_ALL_PARAM_NEW param,
        //                                         int nChannel)
        //     {
        //         //分段时间太长
        //         if (param.nCutLen <= 0)
        //         {
        //             m_error_desc = _T("Segment time of task should be greater than 0 minutes");
        //             //m_error_desc = _T("任务分段时间不能小于0分钟");
        //             return MSV_FAILED;
        //         }
        // 
        //         IRemoteSDK* m_s;
        //         m_s = CreateRemoteSDK();
        //         //	CRemoteSocket m_s(m_nTimeOut);
        // 
        //         if (FALSE == SocketConnect(strMsvIp, m_s))
        //         {
        //             CString strError = _T("Error while connecting MSV[%s]");
        //             m_error_desc.Format(strError, strMsvIp);
        //             //NMTrace(m_strLogName, logLevelWarring, m_error_desc);//(_T("连接媒体服务器[%s] 错误"),strMsvIp);
        //             return MSV_FAILED;
        //         }
        //         /*格式化参数到XML文本*/
        //         CString strSend;
        // 
        //         strSend = FromatParmaToStringNew(param, _T("set_local_param"), nChannel);
        // 
        //         /* 操作   */
        //         CXMLParser parser;
        //         if (FALSE == parser.Create())
        //         {
        //             CString strError = _T("Error occur while analyzing response message of MSV[%s]");
        //             m_error_desc.Format(strError, strMsvIp);
        //             //NMTrace(m_strLogName, logLevelWarring, m_error_desc);//(_T("(解析媒体服务器[%s] 回应消息错误)"),strMsvIp);
        //             return MSV_FAILED;
        //         }
        //         InParam pIn;
        //         OutParam pOut;
        //         CString strRecv;
        //         pIn.m_s = m_s;
        //         //	pIn.m_s = &m_s;
        //         pIn.parser = &parser;
        //         pIn.strIp = strMsvIp;
        //         pIn.strOrigin = _T("set_local_param");
        //         pIn.strSend = strSend;
        //         pOut.strRecv = strRecv;
        // 
        //         int nRet = CommandOperation(pIn, pOut);
        //         if (nRet == -2)
        //             return MSV_NETERROR;
        //         else if (nRet == -1)
        //             return MSV_XMLERROR;
        //         else if (nRet == 0)
        //             return MSV_FAILED;
        // 
        //         //m_error_desc = _T("操作成功");
        //         m_error_desc = _T("Operation succeed");
        //         return MSV_SUCCESS;
        //     }
        //     MSV_RET CClientTaskSDKImp::MSVGetLocalCaptureParam(CString strMsvIp,
        //                                                     CString &strTaskName,
        //                                                     int& ncutLen,
        //                                                     int& lCatalogID,
        //                                                     CAPRTUR_PARAM &param,
        //                                                     int nChannel)
        //     {
        //         IRemoteSDK* m_s;
        //         m_s = CreateRemoteSDK();
        //         //	CRemoteSocket m_s(m_nTimeOut);
        // 
        //         if (FALSE == SocketConnect(strMsvIp, m_s))
        //         {
        //             CString strError = _T("Error while connecting MSV[%s]");
        //             m_error_desc.Format(strError, strMsvIp);
        //             //NMTrace(m_strLogName, logLevelWarring, m_error_desc);//(_T("连接媒体服务器[%s] 错误"),strMsvIp);
        //             return MSV_FAILED;
        //         }
        //         /*格式化参数到XML文本*/
        //         CString strSend(MXL_HEADER);
        //         /************************************************************/
        //         strSend += _T("<get_local_param>");
        //         CString strChannel;
        //         strChannel.Format(_T("<nChannel>%d</nChannel>"), nChannel);
        //         strSend += strChannel;
        //         strSend += _T("</get_local_param>");  //查询手动任务
        // 
        // 
        //         /* 操作   */
        //         CXMLParser parser;
        //         if (FALSE == parser.Create())
        //         {
        //             CString strError = _T("Error occur while analyzing response message of MSV[%s]");
        //             m_error_desc.Format(strError, strMsvIp);
        //             //NMTrace(m_strLogName, logLevelWarring, m_error_desc);//(_T("(解析媒体服务器[%s] 回应消息错误)"),strMsvIp);
        //             return MSV_FAILED;
        //         }
        //         InParam pIn;
        //         OutParam pOut;
        //         CString strRecv;
        //         pIn.m_s = m_s;
        //         //	pIn.m_s = &m_s;
        //         pIn.parser = &parser;
        //         pIn.strIp = strMsvIp;
        //         pIn.strOrigin = _T("get_local_param");
        //         pIn.strSend = strSend;
        //         pOut.strRecv = strRecv;
        // 
        //         int nRet = CommandOperation(pIn, pOut);
        //         if (nRet == -2)
        //             return MSV_NETERROR;
        //         else if (nRet == -1)
        //             return MSV_XMLERROR;
        //         else if (nRet == 0)
        //             return MSV_FAILED;
        // 
        //         CString strItem, strContent;
        //         //分段长度
        //         strContent = parser.GetNodeText(_T("cutlen"));
        //         ncutLen = _wtoi((LPCTSTR)strContent);
        //         //入库分类ID
        //         strContent = parser.GetNodeText(_T("lCatalogID"));
        //         lCatalogID = _wtoi((LPCTSTR)strContent);
        //         //**************  以下是线路0 参数 ***************
        //         //是否采集视频
        //         strContent = parser.GetNodeText(_T("bPath0"));
        //         param.bPath0 = (BOOL)_wtoi((LPCTSTR)strContent);
        //         //是否采集音频
        //         strContent = parser.GetNodeText(_T("bAudio0"));
        //         param.bAudio0 = (BOOL)_wtoi((LPCTSTR)strContent);
        //         //是否单独生成音频文件
        //         strContent = parser.GetNodeText(_T("bAlone0"));
        //         param.bAlone0 = (BOOL)_wtoi((LPCTSTR)strContent);
        //         //路径
        //         strContent = parser.GetNodeText(_T("strFileName0"));
        //         // param.path0FileName = strContent.Left(2);
        // 
        //         //取得任务的路径
        //         param.path0FileName = strContent;
        //         //取任务名
        //         strTaskName = parser.GetNodeText(_T("taskName"));
        // 
        //         //编码线路 编码类型
        //         strContent = parser.GetNodeText(_T("nEncodeType0"));
        //         param.nEncodeType0 = (MG_EncodeType)_wtoi((LPCTSTR)strContent);
        //         //编码 操作 类型
        //         strContent = parser.GetNodeText(_T("subEncodeType0"));
        //         param.subEncodeType0 = _wtoi((LPCTSTR)strContent);
        //         //视频采集BIT率
        //         strContent = parser.GetNodeText(_T("bit_rate0"));
        //         param.bit_rate0 = _wtoi((LPCTSTR)strContent);
        //         //视频格式
        //         strContent = parser.GetNodeText(_T("AVWriteTypeV0"));
        //         param.AVWriteTypeV0 = (MG_AVWriteType)_wtoi((LPCTSTR)strContent);
        //         //音频采样率
        //         strContent = parser.GetNodeText(_T("dwSamplesOutA0"));
        //         param.dwSamplesOutA0 = _wtoi((LPCTSTR)strContent);
        //         //音频采样格式
        //         strContent = parser.GetNodeText(_T("AudioWriteTypeA0"));
        //         param.AudioWriteTypeA0 = (MG_AudioWriteType)_wtoi((LPCTSTR)strContent);
        //         //**************  以下是线路1 参数 ***************
        //         //是否采集视频
        //         strContent = parser.GetNodeText(_T("bPath1"));
        //         param.bPath1 = (BOOL)_wtoi((LPCTSTR)strContent);
        //         //是否采集音频
        //         strContent = parser.GetNodeText(_T("bAudio1"));
        //         param.bAudio1 = (BOOL)(_wtoi((LPCTSTR)strContent));
        //         //是否单独生成音频文件
        //         strContent = parser.GetNodeText(_T("bAlone1"));
        //         param.bAlone1 = (BOOL)(_wtoi((LPCTSTR)strContent));
        //         //文件名
        //         strContent = parser.GetNodeText(_T("strFileName1"));
        //         //取得任务的路径
        //         param.path1FileName = strContent;
        // 
        //         //编码线路 编码类型
        //         strContent = parser.GetNodeText(_T("nEncodeType1"));
        //         param.nEncodeType1 = (MG_EncodeType)(_wtoi((LPCTSTR)strContent));
        //         //编码 操作 类型
        //         strContent = parser.GetNodeText(_T("subEncodeType1"));
        //         param.subEncodeType1 = _wtoi((LPCTSTR)strContent);
        //         //视频采集BIT率
        //         strContent = parser.GetNodeText(_T("bit_rate1"));
        //         param.bit_rate1 = _wtoi((LPCTSTR)strContent);
        //         //视频格式
        //         strContent = parser.GetNodeText(_T("AVWriteTypeV1"));
        //         param.AVWriteTypeV1 = (MG_AVWriteType)_wtoi((LPCTSTR)strContent);
        //         //音频采样率
        //         strContent = parser.GetNodeText(_T("dwSamplesOutA1"));
        //         param.dwSamplesOutA1 = _wtoi((LPCTSTR)strContent);
        //         //音频采样格式
        //         strContent = parser.GetNodeText(_T("AudioWriteTypeA1"));
        //         param.AudioWriteTypeA1 = (MG_AudioWriteType)_wtoi((LPCTSTR)strContent);
        //         //GOP的P帧数
        //         strContent = parser.GetNodeText(_T("nGOPPFrameCount"));
        //         param.nGOPPCount = _wtoi((LPCTSTR)strContent);
        //         //GOP的B帧数
        //         strContent = parser.GetNodeText(_T("nGOPBFrameCount"));
        //         param.nGOPBCount = _wtoi((LPCTSTR)strContent);
        //         //迁移模式
        //         strContent = parser.GetNodeText(_T("bUseTransfer"));
        //         param.bUseTransfer = (BOOL)_wtoi((LPCTSTR)strContent);
        // 
        // 
        //         return MSV_SUCCESS;
        //     }
        // 
        //     /************************************************************************/
        //     /* 得到MSV得硬盘表                                                      */
        //     /************************************************************************/
        //     MSV_RET CClientTaskSDKImp::MSVGetDriverString(const CString strMsvIp,
        //                                                CArray<CString, CString&>& driverList,
        //                                                int nChannel)
        //     {
        //         IRemoteSDK* m_s;
        //         m_s = CreateRemoteSDK();
        //         //	CRemoteSocket m_s(m_nTimeOut);
        // 
        //         if (FALSE == SocketConnect(strMsvIp, m_s))
        //         {
        //             CString strError = _T("Error while connecting MSV[%s],CClientTaskSDKImp::MSVGetDriverString()");
        //             m_error_desc.Format(strError, strMsvIp);
        //             //WriteLog(m_strLogName, logLevelWarring, m_error_desc);//(_T("连接媒体服务器[%s] 错误"),strMsvIp);
        //             return MSV_FAILED;
        //         }
        //         /*格式化参数到XML文本*/
        //         CString strSend(MXL_HEADER);
        //         /************************************************************/
        //         strSend += _T("<get_msv_dirver>");
        //         CString strChannel;
        //         strChannel.Format(_T("<nChannel>%d</nChannel>"), nChannel);
        //         strSend += strChannel;
        //         strSend += _T("</get_msv_dirver>");  //得到MSV硬盘串
        //                                              /************************************************************/
        //                                              /* 操作   */
        //         CXMLParser parser;
        //         if (FALSE == parser.Create())
        //         {
        //             CString strError = _T("Error occur while analyzing response message of MSV[%s]");
        //             m_error_desc.Format(strError, strMsvIp);
        //             //WriteLog(m_strLogName, logLevelWarring, m_error_desc);//(_T("(解析媒体服务器[%s] 回应消息错误)"),strMsvIp);
        //             return MSV_FAILED;
        //         }
        //         InParam pIn;
        //         OutParam pOut;
        //         CString strRecv;
        //         pIn.m_s = m_s;
        //         //	pIn.m_s = &m_s;
        //         pIn.parser = &parser;
        //         pIn.strIp = strMsvIp;
        //         pIn.strOrigin = _T("get_msv_dirver");
        //         pIn.strSend = strSend;
        //         pOut.strRecv = strRecv;
        // 
        //         int nRet = CommandOperation(pIn, pOut);
        // 
        //         //CString strinfo;
        //         //strinfo.Format(_T("CommandOperation ret %d"), nRet);
        //         //WriteLog(m_strLogName, logLevelWarring, strinfo);
        // 
        //         if (nRet == -2)
        //         {
        //             //AfxMessageBox(_T("MSV无响应，获取驱动器列表发生错误"));
        //             return MSV_NETERROR;
        //         }
        //         else if (nRet == -1)
        //         {
        //             return MSV_XMLERROR;
        //         }
        //         else if (nRet == 0)
        //             return MSV_FAILED;
        //         CString strItem, strContent;
        // 
        // 
        //         strContent = parser.GetNodeText(_T("s_driver"));
        //         GetDriverListFromString(strContent, driverList);
        // 
        // 
        //         //m_error_desc = _T("操作成功");
        //         m_error_desc = _T("Operation succeed");
        //         return MSV_SUCCESS;
        // 
        //     }
        // 
        //     //查询备份数据
        //     MSV_RET CClientTaskSDKImp::MSVQueryBackUpCount(const CString strMsvIp,
        //                                                 int& nCount,
        //                                                 int nChannel)
        //     {
        //         IRemoteSDK* m_s;
        //         m_s = CreateRemoteSDK();
        //         //	CRemoteSocket m_s(m_nTimeOut);
        // 
        //         if (FALSE == SocketConnectTrans(strMsvIp, m_s))
        //         {
        //             CString strError = _T("Error while connecting MSV[%s]");
        //             m_error_desc.Format(strError, strMsvIp);
        //             //NMTrace(m_strLogName, logLevelWarring, m_error_desc);//(_T("连接媒体服务器[%s] 错误"),strMsvIp);
        //             return MSV_FAILED;
        //         }
        //         /*格式化参数到XML文本*/
        //         CString strSend(MXL_HEADER);
        //         /************************************************************/
        //         strSend += _T("<query_backup_count>");
        //         CString strChannel;
        //         strChannel.Format(_T("<nChannel>%d</nChannel>"), nChannel);
        //         strSend += strChannel;
        //         strSend += _T("</query_backup_count>");  //得到MSV硬盘串
        //                                                  /************************************************************/
        //                                                  /* 操作   */
        // 
        // 
        //         CXMLParser parser;
        //         if (FALSE == parser.Create())
        //         {
        //             CString strError = _T("Error occur while analyzing response message of MSV[%s]");
        //             m_error_desc.Format(strError, strMsvIp);
        //             //NMTrace(m_strLogName, logLevelWarring, m_error_desc);//(_T("(解析媒体服务器[%s] 回应消息错误)"),strMsvIp);
        //             return MSV_FAILED;
        //         }
        //         InParam pIn;
        //         OutParam pOut;
        //         CString strRecv;
        //         pIn.m_s = m_s;
        //         //	pIn.m_s = &m_s;
        //         pIn.parser = &parser;
        //         pIn.strIp = strMsvIp;
        //         pIn.strOrigin = _T("query_backup_count");
        //         pIn.strSend = strSend;
        //         pOut.strRecv = strRecv;
        // 
        //         int nRet = CommandOperation(pIn, pOut);
        //         if (nRet == -2)
        //             return MSV_NETERROR;
        //         else if (nRet == -1)
        //             return MSV_XMLERROR;
        //         else if (nRet == 0)
        //             return MSV_FAILED;
        // 
        // 
        //         CString strItem, strContent;
        // 
        // 
        // 
        //         strContent = parser.GetNodeText(_T("s_count"));
        //         nCount = _wtoi(strContent);
        // 
        // 
        // 
        //         return MSV_SUCCESS;
        //     }
        //     MSV_RET CClientTaskSDKImp::MSVQueryBackUpDesc(const CString strMsvIp,
        //     										   const int nIndex,
        //                                                CString& strDesc,
        //                                                int nChannel)
        //     {
        //         IRemoteSDK* m_s;
        //         m_s = CreateRemoteSDK();
        //         //    CRemoteSocket m_s(m_nTimeOut);
        // 
        //         if (FALSE == SocketConnectTrans(strMsvIp, m_s))
        //         {
        //             CString strError = _T("Error while connecting MSV[%s]");
        //             m_error_desc.Format(strError, strMsvIp);
        //             //NMTrace(m_strLogName, logLevelWarring, m_error_desc);//(_T("连接媒体服务器[%s] 错误"),strMsvIp);
        //             return MSV_FAILED;
        //         }
        //         /*格式化参数到XML文本*/
        //         CString strSend(MXL_HEADER);
        //         CString strTemp;
        //         /************************************************************/
        //         strSend += _T("<query_backup_desc>");
        //         strTemp.Format(_T("<s_index>%d</s_index>"), nIndex);
        //         strSend += strTemp;
        //         CString strChannel;
        //         strChannel.Format(_T("<nChannel>%d</nChannel>"), nChannel);
        //         strSend += strChannel;
        //         strSend += _T("</query_backup_desc>");  //得到MSV硬盘串
        //                                                 /************************************************************/
        //                                                 /* 操作   */
        //         CXMLParser parser;
        //         if (FALSE == parser.Create())
        //         {
        //             CString strError = _T("Error occur while analyzing response message of MSV[%s]");
        //             m_error_desc.Format(strError, strMsvIp);
        //             //NMTrace(m_strLogName, logLevelWarring, m_error_desc);//(_T("(解析媒体服务器[%s] 回应消息错误)"),strMsvIp);
        //             return MSV_FAILED;
        //         }
        //         InParam pIn;
        //         OutParam pOut;
        //         CString strRecv;
        //         pIn.m_s = m_s;
        //         //	pIn.m_s = &m_s;
        //         pIn.parser = &parser;
        //         pIn.strIp = strMsvIp;
        //         pIn.strOrigin = _T("query_backup_desc");
        //         pIn.strSend = strSend;
        //         pOut.strRecv = strRecv;
        // 
        //         int nRet = CommandOperation(pIn, pOut);
        //         if (nRet == -2)
        //             return MSV_NETERROR;
        //         else if (nRet == -1)
        //             return MSV_XMLERROR;
        //         else if (nRet == 0)
        //             return MSV_FAILED;
        // 
        // 
        //         CString strItem, strContent;
        // 
        //         strDesc = parser.GetNodeText(_T("s_desc"));
        // 
        // 
        //         return MSV_SUCCESS;
        //     }
        // 
        //     //恢复备份
        //     MSV_RET CClientTaskSDKImp::MSVStartRecover(const CString strMsvIp,
        //                                             BOOL bDeleteBackup,
        //                                             int nChannel)
        //     {
        //         IRemoteSDK* m_s;
        //         m_s = CreateRemoteSDK();
        //         //	CRemoteSocket m_s(m_nTimeOut);
        // 
        //         if (FALSE == SocketConnectTrans(strMsvIp, m_s))
        //         {
        //             CString strError = _T("Error while connecting MSV[%s]");
        //             m_error_desc.Format(strError, strMsvIp);
        //             //NMTrace(m_strLogName, logLevelWarring, m_error_desc);//(_T("连接媒体服务器[%s] 错误"),strMsvIp);
        //             return MSV_FAILED;
        //         }
        //         /*格式化参数到XML文本*/
        //         CString strSend(MXL_HEADER);
        //         CString strTemp;
        //         /************************************************************/
        //         strSend += _T("<msv_recover_clip>");
        //         strTemp.Format(_T("<b_delete>%d</b_delete>"), bDeleteBackup);
        //         strSend += strTemp;
        //         CString strChannel;
        //         strChannel.Format(_T("<nChannel>%d</nChannel>"), nChannel);
        //         strSend += strChannel;
        //         strSend += _T("</msv_recover_clip>");
        //         /************************************************************/
        //         /* 操作   */
        //         CXMLParser parser;
        //         if (FALSE == parser.Create())
        //         {
        //             CString strError = _T("Error occur while analyzing response message of MSV[%s]");
        //             m_error_desc.Format(strError, strMsvIp);
        //             //NMTrace(m_strLogName, logLevelWarring, m_error_desc);//(_T("(解析媒体服务器[%s] 回应消息错误)"),strMsvIp);
        //             return MSV_FAILED;
        //         }
        //         InParam pIn;
        //         OutParam pOut;
        //         CString strRecv;
        //         pIn.m_s = m_s;
        //         //	pIn.m_s = &m_s;
        //         pIn.parser = &parser;
        //         pIn.strIp = strMsvIp;
        //         pIn.strOrigin = _T("msv_recover_clip");
        //         pIn.strSend = strSend;
        //         pOut.strRecv = strRecv;
        // 
        //         int nRet = CommandOperation(pIn, pOut);
        //         if (nRet == -2)
        //             return MSV_NETERROR;
        //         else if (nRet == -1)
        //             return MSV_XMLERROR;
        //         else if (nRet == 0)
        //             return MSV_FAILED;
        // 
        //         //m_error_desc = _T("操作成功");
        //         m_error_desc = _T("Operation succeed");
        //         return MSV_SUCCESS;
        //     }
        //     //停止恢复
        //     MSV_RET CClientTaskSDKImp::MSVStopRecover(const CString strMsvIp,
        //                                            int nChannel)
        //     {
        //         IRemoteSDK* m_s;
        //         m_s = CreateRemoteSDK();
        //         //	CRemoteSocket m_s(m_nTimeOut);
        // 
        //         if (FALSE == SocketConnectTrans(strMsvIp, m_s))
        //         {
        //             CString strError = _T("Error while connecting MSV[%s]");
        //             m_error_desc.Format(strError, strMsvIp);
        //             //NMTrace(m_strLogName, logLevelWarring, m_error_desc);//(_T("连接媒体服务器[%s] 错误"),strMsvIp);
        //             return MSV_FAILED;
        //         }
        //         /*格式化参数到XML文本*/
        //         CString strSend(MXL_HEADER);
        // 
        //         /************************************************************/
        //         strSend += _T("<msv_stop_recover>");
        //         CString strChannel;
        //         strChannel.Format(_T("<nChannel>%d</nChannel>"), nChannel);
        //         strSend += strChannel;
        //         strSend += _T("</msv_stop_recover>");
        //         /************************************************************/
        //         /* 操作   */
        //         CXMLParser parser;
        //         if (FALSE == parser.Create())
        //         {
        //             CString strError = _T("Error occur while analyzing response message of MSV[%s]");
        //             m_error_desc.Format(strError, strMsvIp);
        //             //NMTrace(m_strLogName, logLevelWarring, m_error_desc);//(_T("(解析媒体服务器[%s] 回应消息错误)"),strMsvIp);
        //             return MSV_FAILED;
        //         }
        //         InParam pIn;
        //         OutParam pOut;
        //         CString strRecv;
        //         pIn.m_s = m_s;
        //         //	pIn.m_s = &m_s;
        //         pIn.parser = &parser;
        //         pIn.strIp = strMsvIp;
        //         pIn.strOrigin = _T("msv_stop_recover");
        //         pIn.strSend = strSend;
        //         pOut.strRecv = strRecv;
        // 
        //         int nRet = CommandOperation(pIn, pOut);
        //         if (nRet == -2)
        //             return MSV_NETERROR;
        //         else if (nRet == -1)
        //             return MSV_XMLERROR;
        //         else if (nRet == 0)
        //             return MSV_FAILED;
        // 
        //         //m_error_desc = _T("操作成功");
        //         m_error_desc = _T("Operation succeed");
        //         return MSV_SUCCESS;
        //     }
        //     //查询已恢复数据
        //     MSV_RET CClientTaskSDKImp::MSVQueryRecoveredCount(const CString strMsvIp,
        //                                                    int& nCount,
        //                                                    int nChannel)
        //     {
        //         IRemoteSDK* m_s;
        //         m_s = CreateRemoteSDK();
        //         //	CRemoteSocket m_s(m_nTimeOut);
        // 
        //         if (FALSE == SocketConnectTrans(strMsvIp, m_s))
        //         {
        //             CString strError = _T("Error while connecting MSV[%s]");
        //             m_error_desc.Format(strError, strMsvIp);
        //             //NMTrace(m_strLogName, logLevelWarring, m_error_desc);//(_T("连接媒体服务器[%s] 错误"),strMsvIp);
        //             return MSV_FAILED;
        //         }
        //         /*格式化参数到XML文本*/
        //         CString strSend(MXL_HEADER);
        //         /************************************************************/
        //         strSend += _T("<get_recovered_count>");
        //         CString strChannel;
        //         strChannel.Format(_T("<nChannel>%d</nChannel>"), nChannel);
        //         strSend += strChannel;
        //         strSend += _T("</get_recovered_count>");  //得到MSV硬盘串
        //                                                   /************************************************************/
        //                                                   /* 操作   */
        //         CXMLParser parser;
        //         if (FALSE == parser.Create())
        //         {
        //             CString strError = _T("Error occur while analyzing response message of MSV[%s]");
        //             m_error_desc.Format(strError, strMsvIp);
        //             //NMTrace(m_strLogName, logLevelWarring, m_error_desc);//(_T("(解析媒体服务器[%s] 回应消息错误)"),strMsvIp);
        //             return MSV_FAILED;
        //         }
        //         InParam pIn;
        //         OutParam pOut;
        //         CString strRecv;
        //         pIn.m_s = m_s;
        //         //	pIn.m_s = &m_s;
        //         pIn.parser = &parser;
        //         pIn.strIp = strMsvIp;
        //         pIn.strOrigin = _T("get_recovered_count");
        //         pIn.strSend = strSend;
        //         pOut.strRecv = strRecv;
        // 
        //         int nRet = CommandOperation(pIn, pOut);
        //         if (nRet == -2)
        //             return MSV_NETERROR;
        //         else if (nRet == -1)
        //             return MSV_XMLERROR;
        //         else if (nRet == 0)
        //             return MSV_FAILED;
        // 
        // 
        //         CString strItem, strContent;
        // 
        //         strContent = parser.GetNodeText(_T("s_count"));
        //         nCount = _wtoi(strContent);
        // 
        //         //m_error_desc = _T("操作成功");
        //         m_error_desc = _T("Operation succeed");
        //         return MSV_SUCCESS;
        // 
        //     }
        //     MSV_RET CClientTaskSDKImp::MSVQueryRecoveredInfo(const CString strMsvIp,
        //     											  const int nIndex,
        //                                                   BACKUP_INFO& info,
        //                                                   int nChannel)
        //     {
        //         IRemoteSDK* m_s;
        //         m_s = CreateRemoteSDK();
        //         //	CRemoteSocket m_s(m_nTimeOut);
        // 
        //         if (FALSE == SocketConnectTrans(strMsvIp, m_s))
        //         {
        //             CString strError = _T("Error while connecting MSV[%s]");
        //             m_error_desc.Format(strError, strMsvIp);
        //             //NMTrace(m_strLogName, logLevelWarring, m_error_desc);//(_T("连接媒体服务器[%s] 错误"),strMsvIp);
        //             return MSV_FAILED;
        //         }
        //         /*格式化参数到XML文本*/
        //         CString strSend(MXL_HEADER);
        //         CString strIndex;
        //         /************************************************************/
        //         strSend += _T("<get_recovered_desc>");
        //         strIndex.Format(_T("<s_index>%d</s_index>"), nIndex);
        //         strSend += strIndex;
        //         CString strChannel;
        //         strChannel.Format(_T("<nChannel>%d</nChannel>"), nChannel);
        //         strSend += strChannel;
        //         strSend += _T("</get_recovered_desc>");  //得到MSV硬盘串
        //                                                  /************************************************************/
        //                                                  /* 操作   */
        //         CXMLParser parser;
        //         if (FALSE == parser.Create())
        //         {
        //             CString strError = _T("Error occur while analyzing response message of MSV[%s]");
        //             m_error_desc.Format(strError, strMsvIp);
        //             //NMTrace(m_strLogName, logLevelWarring, m_error_desc);//(_T("(解析媒体服务器[%s] 回应消息错误)"),strMsvIp);
        //             return MSV_FAILED;
        //         }
        //         InParam pIn;
        //         OutParam pOut;
        //         CString strRecv;
        //         pIn.m_s = m_s;
        //         //	pIn.m_s = &m_s;
        //         pIn.parser = &parser;
        //         pIn.strIp = strMsvIp;
        //         pIn.strOrigin = _T("get_recovered_desc");
        //         pIn.strSend = strSend;
        //         pOut.strRecv = strRecv;
        // 
        //         int nRet = CommandOperation(pIn, pOut);
        //         if (nRet == -2)
        //             return MSV_NETERROR;
        //         else if (nRet == -1)
        //             return MSV_XMLERROR;
        //         else if (nRet == 0)
        //             return MSV_FAILED;
        // 
        //         CString strItem, strContent;
        // 
        // 
        //         strContent = parser.GetNodeText(_T("s_hVFilename"));
        //         info.strVideoPathName0 = strContent;
        //         strContent = parser.GetNodeText(_T("s_hAFilename"));
        //         info.strVideoPathName1 = strContent;
        //         strContent = parser.GetNodeText(_T("s_lVFilename"));
        //         info.strAudioPathName0 = strContent;
        //         strContent = parser.GetNodeText(_T("s_lAFilename"));
        //         info.strAudioPathName1 = strContent;
        // 
        //         //m_error_desc = _T("操作成功");
        //         m_error_desc = _T("Operation succeed");
        //         return MSV_SUCCESS;
        //     }
        // 
        //     MSV_RET CClientTaskSDKImp::MSVSynchronizationTime(const CString strMsvIp,
        //     											   const CTime m_time,
        //                                                    int nChannel)
        //     {
        //         IRemoteSDK* m_s;
        //         m_s = CreateRemoteSDK();
        //         //	CRemoteSocket m_s(m_nTimeOut);
        // 
        //         if (FALSE == SocketConnect(strMsvIp, m_s))
        //         {
        //             CString strError = _T("Error while connecting MSV[%s]");
        //             m_error_desc.Format(strError, strMsvIp);
        //             //NMTrace(m_strLogName, logLevelWarring, m_error_desc);//(_T("连接媒体服务器[%s] 错误"),strMsvIp);
        //             return MSV_FAILED;
        //         }
        // 
        //         CString strSend(MXL_HEADER);
        //         /************************************************************/
        //         strSend += _T("<msv_synchron_time>");
        //         CString strState;
        //         strState = m_time.Format(_T("%Y-%m-%d %H:%M:%S"));
        //         strSend += _T("<s_time>");
        //         strSend += strState;
        //         strSend += _T("</s_time>");
        //         CString strChannel;
        //         strChannel.Format(_T("<nChannel>%d</nChannel>"), nChannel);
        //         strSend += strChannel;
        //         strSend += _T("</msv_synchron_time>");
        // 
        //         /************************************************************/
        //         CXMLParser parser;
        //         if (FALSE == parser.Create())
        //         {
        //             CString strError = _T("Error occur while analyzing response message of MSV[%s]");
        //             m_error_desc.Format(strError, strMsvIp);
        //             //NMTrace(m_strLogName, logLevelWarring, m_error_desc);//(_T("(解析媒体服务器[%s] 回应消息错误)"),strMsvIp);
        //             return MSV_FAILED;
        //         }
        //         InParam pIn;
        //         OutParam pOut;
        //         CString strRecv;
        //         pIn.m_s = m_s;
        //         //	pIn.m_s = &m_s;
        //         pIn.parser = &parser;
        //         pIn.strIp = strMsvIp;
        //         pIn.strOrigin = _T("msv_synchron_time");
        //         pIn.strSend = strSend;
        //         pOut.strRecv = strRecv;
        // 
        //         int nRet = CommandOperation(pIn, pOut);
        //         if (nRet == -2)
        //             return MSV_NETERROR;
        //         else if (nRet == -1)
        //             return MSV_XMLERROR;
        //         else if (nRet == 0)
        //             return MSV_FAILED;
        // 
        //         //m_error_desc = _T("操作成功");
        //         m_error_desc = _T("Operation succeed");
        //         return MSV_SUCCESS;
        //     }
        // 
        //     /************************************************************************/
        //     /* 获取MSV日志文件的数量                                                */
        //     /************************************************************************/
        //     MSV_RET CClientTaskSDKImp::MSVGetLogFileCount(const CString strMsvIp,
        //                                                int& nFileCount,
        //                                                int nChannel)
        //     {
        //         IRemoteSDK* m_s;
        //         m_s = CreateRemoteSDK();
        //         //	CRemoteSocket m_s(m_nTimeOut);
        // 
        //         if (FALSE == SocketConnect(strMsvIp, m_s))
        //         {
        //             CString strError = _T("Error while connecting MSV[%s]");
        //             m_error_desc.Format(strError, strMsvIp);
        //             //NMTrace(m_strLogName, logLevelWarring, m_error_desc);//(_T("连接媒体服务器[%s] 错误"),strMsvIp);
        //             return MSV_FAILED;
        //         }
        //         /*格式化参数到XML文本*/
        //         CString strSend(MXL_HEADER);
        //         /************************************************************/
        //         strSend += _T("<msv_get_logfile_count>");
        //         CString strChannel;
        //         strChannel.Format(_T("<nChannel>%d</nChannel>"), nChannel);
        //         strSend += strChannel;
        //         strSend += _T("</msv_get_logfile_count>");
        //         /************************************************************/
        //         /* 操作   */
        // 
        //         CXMLParser parser;
        //         if (FALSE == parser.Create())
        //         {
        //             CString strError = _T("Error occur while analyzing response message of MSV[%s]");
        //             m_error_desc.Format(strError, strMsvIp);
        //             //NMTrace(m_strLogName, logLevelWarring, m_error_desc);//(_T("(解析媒体服务器[%s] 回应消息错误)"),strMsvIp);
        //             return MSV_FAILED;
        //         }
        //         InParam pIn;
        //         OutParam pOut;
        //         CString strRecv;
        //         pIn.m_s = m_s;
        //         //	pIn.m_s = &m_s;
        //         pIn.parser = &parser;
        //         pIn.strIp = strMsvIp;
        //         pIn.strOrigin = _T("msv_get_logfile_count");
        //         pIn.strSend = strSend;
        //         pOut.strRecv = strRecv;
        //         int nRet = CommandOperation(pIn, pOut);
        //         if (nRet == -2)
        //             return MSV_NETERROR;
        //         else if (nRet == -1)
        //             return MSV_XMLERROR;
        //         else if (nRet == 0)
        //             return MSV_FAILED;
        // 
        // 
        //         CString strItem, strContent;
        // 
        // 
        //         strContent = parser.GetNodeText(_T("s_count"));
        //         nFileCount = _wtoi(strContent);
        //         return MSV_SUCCESS;
        // 
        //     }
        //     /************************************************************************/
        //     /* 获取MSV日志文件名                                                    */
        //     /************************************************************************/
        //     MSV_RET CClientTaskSDKImp::MSVGetLogFileName(const CString strMsvIp,
        //     										  const int nFileIndex,
        //                                               CString& strFileName,
        //                                               int nChannel)
        //     {
        //         IRemoteSDK* m_s;
        //         m_s = CreateRemoteSDK();
        //         //	CRemoteSocket m_s(m_nTimeOut);
        // 
        //         if (FALSE == SocketConnect(strMsvIp, m_s))
        //         {
        //             CString strError = _T("Error while connecting MSV[%s]");
        //             m_error_desc.Format(strError, strMsvIp);
        //             //NMTrace(m_strLogName, logLevelWarring, m_error_desc);//(_T("连接媒体服务器[%s] 错误"),strMsvIp);
        //             return MSV_FAILED;
        //         }
        //         /*格式化参数到XML文本*/
        //         CString strSend(MXL_HEADER);
        //         CString strIndex;
        //         strIndex.Format(_T("<s_index>%d</s_index>"), nFileIndex);
        //         /************************************************************/
        //         strSend += _T("<msv_get_logfile_desc>");
        //         strSend += strIndex;
        //         CString strChannel;
        //         strChannel.Format(_T("<nChannel>%d</nChannel>"), nChannel);
        //         strSend += strChannel;
        //         strSend += _T("</msv_get_logfile_desc>");
        //         /************************************************************/
        //         /* 操作   */
        // 
        //         CXMLParser parser;
        //         if (FALSE == parser.Create())
        //         {
        //             CString strError = _T("Error occur while analyzing response message of MSV[%s]");
        //             m_error_desc.Format(strError, strMsvIp);
        //             //NMTrace(m_strLogName, logLevelWarring, m_error_desc);//(_T("(解析媒体服务器[%s] 回应消息错误)"),strMsvIp);
        //             return MSV_FAILED;
        //         }
        //         InParam pIn;
        //         OutParam pOut;
        //         CString strRecv;
        //         pIn.m_s = m_s;
        //         //	pIn.m_s = &m_s;
        //         pIn.parser = &parser;
        //         pIn.strIp = strMsvIp;
        //         pIn.strOrigin = _T("msv_get_logfile_desc");
        //         pIn.strSend = strSend;
        //         pOut.strRecv = strRecv;
        // 
        //         int nRet = CommandOperation(pIn, pOut);
        //         if (nRet == -2)
        //             return MSV_NETERROR;
        //         else if (nRet == -1)
        //             return MSV_XMLERROR;
        //         else if (nRet == 0)
        //             return MSV_FAILED;
        // 
        // 
        //         CString strItem, strContent;
        // 
        // 
        //         strFileName = parser.GetNodeText(_T("s_desc"));
        // 
        //         return MSV_SUCCESS;
        // 
        //     }
        //     /************************************************************************/
        //     /* 按文件名获取MSV日志文件内容                                          */
        //     /************************************************************************/
        //     MSV_RET CClientTaskSDKImp::MSVGetLogFileContent(const CString strMsvIp,
        //     											 const CString strFileName,
        //                                                  BOOL bDelete,
        //                                                  int nChannel)
        //     {
        //         IRemoteSDK* m_s;
        //         m_s = CreateRemoteSDK();
        //         //  	CRemoteSocket m_s(m_nTimeOut);
        //         SocketConnect(strMsvIp, m_s);
        //         /*	m_s.Create();
        //             if(FALSE == m_s.Connect(strMsvIp))
        //             {
        //                  CString strError = _T("Error while connecting MSV[%s]");
        //                 m_error_desc.Format(strError, strMsvIp);
        //                 //NMTrace(m_strLogName, logLevelWarring, m_error_desc);//(_T("连接媒体服务器[%s] 错误"),strMsvIp);
        //                  return MSV_FAILED;
        //             }
        //         /**/
        // 
        //         CString strXML(MXL_HEADER);
        //         strXML += _T("<msv_get_logfile_content>");
        //         CString strTemp;
        //         strTemp.Format(_T("<s_filename>%s</s_filename>"), strFileName);
        //         strXML += strTemp;
        //         strTemp.Format(_T("<b_delete>%d</b_delete>"), bDelete);
        //         strXML += strTemp;
        //         CString strChannel;
        //         strChannel.Format(_T("<nChannel>%d</nChannel>"), nChannel);
        //         strXML += strChannel;
        //         strXML += _T("</msv_get_logfile_content>");
        //         /*******************************************************/
        //         CXMLParser parser;
        //         if (FALSE == parser.Create())
        //         {
        //             CString strError = _T("Error occur while analyzing response message of MSV[%s]");
        //             m_error_desc.Format(strError, strMsvIp);
        //             //NMTrace(m_strLogName, logLevelWarring, m_error_desc);//(_T("(解析媒体服务器[%s] 回应消息错误)"),strMsvIp);
        //             return MSV_FAILED;
        //         }
        //         InParam pIn;
        //         OutParam pOut;
        //         CString strRecv;
        //         pIn.m_s = m_s;
        //         //	pIn.m_s = &m_s;
        //         pIn.parser = &parser;
        //         pIn.strIp = strMsvIp;
        //         pIn.strOrigin = _T("msv_get_logfile_content");
        //         pIn.strSend = strXML;
        //         pOut.strRecv = strRecv;
        // 
        //         int nRet = CommandOperation(pIn, pOut);
        //         if (nRet == -2)
        //             return MSV_NETERROR;
        //         else if (nRet == -1)
        //             return MSV_XMLERROR;
        //         else if (nRet == 0)
        //             return MSV_FAILED;
        // 
        // 
        //         CString strItem, strContent;
        // 
        // 
        //         strContent = parser.GetNodeText(_T("s_fileLen"));
        // 
        //         int nFileLen = _wtoi(strContent);
        // 
        //         char* data = new char[1024];
        //         memset(data, 0x00, 1024);
        // 
        //         CString m_file_name;
        //         m_file_name = GetAppPath(strFileName);
        //         CFile destFile(m_file_name, CFile::modeCreate | CFile::modeWrite | CFile::typeBinary);
        // 
        //         /*
        //             for(int i=0;i<=nFileLen -1024;i+=1024)
        //             {
        //                 m_s.ReceiveFileString(data+i,1024);
        //             }
        //             if(nFileLen%1024!=0)
        //                 m_s.ReceiveFileString(data+nFileLen-nFileLen%1024,nFileLen%1024);
        // 
        //             destFile.Write(data, nFileLen); // Write it
        // 
        //             destFile.Close();*/
        //         int nReceiveLen;
        //         int nTotal = 0;
        //         /*
        //         for(;;)
        //             {
        //                 nReceiveLen = m_s.ReceiveFileString(data,1024);	//接受
        //                 nTotal += nReceiveLen;
        //                 destFile.Write(data,nReceiveLen);
        //                 if(nReceiveLen==0 || nTotal >nFileLen) 		//0表示结束
        //                     break;		//接受完毕
        // 
        //             }
        //             destFile.Close();
        //         */
        // 
        //         for (;;)
        //         {
        //             nReceiveLen = m_s.ReceiveFileString(data, 1024);   //接受
        //             nTotal += nReceiveLen;
        //             if (nReceiveLen <= 0)       //0表示结束
        //                 break;      //接受完毕
        //             destFile.Write(data, nReceiveLen);
        //             if (nTotal >= nFileLen)
        //                 break;
        //         }
        //         destFile.Close();
        // 
        // 
        // 
        // 
        // 
        // 
        //         delete[] data;
        //         data = NULL;
        // 
        //         return MSV_SUCCESS;
        //     }
        // 
        //     CString CClientTaskSDKImp::GetAppPath(const CString strFileName)
        //     {
        //         /*
        //           得到应用程序当前目录
        //         */
        //         CString m_file_dir;
        //         TCHAR szPath[MAX_PATH];
        //         memset(szPath, 0X00, MAX_PATH);
        //         GetModuleFileName(NULL, szPath, MAX_PATH);
        //         m_file_dir = szPath;
        //         int nIndex = m_file_dir.ReverseFind(_T('\\'));
        //         m_file_dir = m_file_dir.Left(nIndex + 1);
        //         m_file_dir += _T("LOG_TEMP\\");
        //         Afx_MakeDir(m_file_dir);
        //         m_file_dir += strFileName;
        //         return m_file_dir;
        //     }
        //     /************************************************************************/
        //     /*                                                                      */
        //     /************************************************************************/
        //     BOOL CClientTaskSDKImp::Afx_MakeDir(CString strDir)
        //     {
        //         //先检查一次全路径,节省时间
        //         if (Afx_FindDir(strDir))
        //             return TRUE;
        //         int nStart = 0;
        //         if (strDir[strDir.GetLength() - 1] != _T('\\'))
        //             strDir += _T("\\");
        //         while (nStart != -1)
        //         {
        //             if (nStart != 0)
        //             {
        //                 CString strDirTemp = strDir.Left(nStart);
        //                 if (!Afx_FindDir(strDirTemp))
        //                 {
        //                     if (!::CreateDirectory(strDirTemp, NULL))
        //                         return FALSE;
        //                 }
        //                 nStart++;
        //             }
        //             nStart = strDir.Find(_T("\\"), nStart);
        //         }
        //         return TRUE;
        //     }
        //     /************************************************************************/
        //     /*                                                                      */
        //     /************************************************************************/
        //     BOOL CClientTaskSDKImp::Afx_FindDir(CString strDir)
        //     {
        //         if (_waccess((LPCTSTR)strDir, 0) == 0)
        //         {
        //             return TRUE;
        //         }
        //         return FALSE;
        //     }
        // 


        // 
        //     /************************************************************************/
        //     /* MSV是否启用环出                                                       */
        //     /************************************************************************/
        //     MSV_RET CClientTaskSDKImp::MSVSetCycleoutState(CString strMsvIp,
        //                                                 BOOL bState,
        //                                                 int nChannel)
        //     {
        //         IRemoteSDK* m_s;
        //         m_s = CreateRemoteSDK();
        //         //    CRemoteSocket m_s(m_nTimeOut);
        //         if (FALSE == SocketConnect(strMsvIp, m_s))
        //         {
        //             CString strError = _T("Error while connecting MSV[%s]");
        //             m_error_desc.Format(strError, strMsvIp);
        //             //NMTrace(m_strLogName, logLevelWarring, m_error_desc);//(_T("连接媒体服务器[%s] 错误"),strMsvIp);
        //             return MSV_FAILED;
        //         }
        //         /*格式化参数到XML文本*/
        //         CString strSend(MXL_HEADER);
        //         /************************************************************/
        //         CString strTemp;
        //         strSend += _T("<msv_set_cycleout_state>");
        //         strTemp.Format(_T("<b_enable>%d</b_enable>"), bState);
        //         strSend += strTemp;
        //         CString strChannel;
        //         strChannel.Format(_T("<nChannel>%d</nChannel>"), nChannel);
        //         strSend += strChannel;
        //         strSend += _T("</msv_set_cycleout_state>");
        //         /************************************************************/
        //         /* 操作   */
        //         CXMLParser parser;
        //         if (FALSE == parser.Create())
        //         {
        //             CString strError = _T("Error occur while analyzing response message of MSV[%s]");
        //             m_error_desc.Format(strError, strMsvIp);
        //             //NMTrace(m_strLogName, logLevelWarring, m_error_desc);//(_T("(解析媒体服务器[%s] 回应消息错误)"),strMsvIp);
        //             return MSV_FAILED;
        //         }
        //         InParam pIn;
        //         OutParam pOut;
        //         CString strRecv;
        //         pIn.m_s = m_s;
        //         //	pIn.m_s = &m_s;
        //         pIn.parser = &parser;
        //         pIn.strIp = strMsvIp;
        //         pIn.strOrigin = _T("msv_set_cycleout_state");
        //         pIn.strSend = strSend;
        //         pOut.strRecv = strRecv;
        // 
        //         int nRet = CommandOperation(pIn, pOut);
        //         if (nRet == -2)
        //             return MSV_NETERROR;
        //         else if (nRet == -1)
        //             return MSV_XMLERROR;
        //         else if (nRet == 0)
        //             return MSV_FAILED;
        // 
        //         //m_error_desc = _T("操作成功");
        //         m_error_desc = _T("Operation succeed");
        //         return MSV_SUCCESS;
        //     }
        //     /************************************************************************/
        //     /* 查询目标机器注册的消息                                               */
        //     /************************************************************************/
        //     MSV_RET CClientTaskSDKImp::MSVQueryRegisterMsg(CString strMsvIp,
        //                                                 CArray<long, long&>& msgList,
        //                                                 int nChannel)
        //     {
        //         IRemoteSDK* m_s;
        //         m_s = CreateRemoteSDK();
        //         //	CRemoteSocket m_s(m_nTimeOut);
        //         if (FALSE == SocketConnectTrans(strMsvIp, m_s))
        //         {
        //             CString strError = _T("Error while connecting MSV[%s]");
        //             m_error_desc.Format(strError, strMsvIp);
        //             //NMTrace(m_strLogName, logLevelWarring, m_error_desc);//(_T("连接媒体服务器[%s] 错误"),strMsvIp);
        //             return MSV_FAILED;
        //         }
        //         /*格式化参数到XML文本*/
        //         CString strSend(MXL_HEADER);
        //         /************************************************************/
        //         strSend += _T("<msv_get_registed_msg>");
        //         CString strChannel;
        //         strChannel.Format(_T("<nChannel>%d</nChannel>"), nChannel);
        //         strSend += strChannel;
        //         strSend += _T("</msv_get_registed_msg>");
        //         /************************************************************/
        //         /* 操作   */
        //         CXMLParser parser;
        //         if (FALSE == parser.Create())
        //         {
        //             CString strError = _T("Error occur while analyzing response message of MSV[%s]");
        //             m_error_desc.Format(strError, strMsvIp);
        //             //NMTrace(m_strLogName, logLevelWarring, m_error_desc);//(_T("(解析媒体服务器[%s] 回应消息错误)"),strMsvIp);
        //             return MSV_FAILED;
        //         }
        //         InParam pIn;
        //         OutParam pOut;
        //         CString strRecv;
        //         pIn.m_s = m_s;
        //         //	pIn.m_s = &m_s;
        //         pIn.parser = &parser;
        //         pIn.strIp = strMsvIp;
        //         pIn.strOrigin = _T("msv_get_registed_msg");
        //         pIn.strSend = strSend;
        //         pOut.strRecv = strRecv;
        // 
        //         int nRet = CommandOperation(pIn, pOut);
        //         if (nRet == -2)
        //             return MSV_NETERROR;
        //         else if (nRet == -1)
        //             return MSV_XMLERROR;
        //         else if (nRet == 0)
        //             return MSV_FAILED;
        // 
        // 
        //         CString strItem, strContent;
        // 
        // 
        // 
        //         strContent = parser.GetNodeText(_T("s_msg"));
        //         GetTaskListFromString(strContent, msgList);
        // 
        //         //m_error_desc = _T("操作成功");
        //         m_error_desc = _T("Operation succeed");
        //         return MSV_SUCCESS;
        //     }
        //     /************************************************************************/
        //     /* 查询本地备份恢复进度                                                 */
        //     /************************************************************************/
        //     MSV_RET CClientTaskSDKImp::MSVQueryLocalRecoverState(CString strMsvIp,
        //                                                       int& nFileIndex,
        //                                                       int& nFrame,
        //                                                       int nChannel)
        //     {
        // 
        //         IRemoteSDK* m_s;
        //         m_s = CreateRemoteSDK();
        //         //	CRemoteSocket m_s(m_nTimeOut);
        //         if (FALSE == SocketConnectTrans(strMsvIp, m_s))
        //         {
        //             CString strError = _T("Error while connecting MSV[%s]");
        //             m_error_desc.Format(strError, strMsvIp);
        //             //NMTrace(m_strLogName, logLevelWarring, m_error_desc);//(_T("连接媒体服务器[%s] 错误"),strMsvIp);
        //             return MSV_FAILED;
        //         }
        //         /*格式化参数到XML文本*/
        //         CString strSend(MXL_HEADER);
        //         /************************************************************/
        //         strSend += _T("<msv_get_localrecover_state>");
        //         CString strChannel;
        //         strChannel.Format(_T("<nChannel>%d</nChannel>"), nChannel);
        //         strSend += strChannel;
        //         strSend += _T("</msv_get_localrecover_state>");
        //         /************************************************************/
        //         /* 操作   */
        // 
        //         CXMLParser parser;
        //         if (FALSE == parser.Create())
        //         {
        //             CString strError = _T("Error occur while analyzing response message of MSV[%s]");
        //             m_error_desc.Format(strError, strMsvIp);
        //             //NMTrace(m_strLogName, logLevelWarring, m_error_desc);//(_T("(解析媒体服务器[%s] 回应消息错误)"),strMsvIp);
        //             return MSV_FAILED;
        //         }
        //         InParam pIn;
        //         OutParam pOut;
        //         CString strRecv;
        //         pIn.m_s = m_s;
        //         //	pIn.m_s = &m_s;
        //         pIn.parser = &parser;
        //         pIn.strIp = strMsvIp;
        //         pIn.strOrigin = _T("msv_get_localrecover_state");
        //         pIn.strSend = strSend;
        //         pOut.strRecv = strRecv;
        // 
        //         int nRet = CommandOperation(pIn, pOut);
        //         if (nRet == -2)
        //             return MSV_NETERROR;
        //         else if (nRet == -1)
        //             return MSV_XMLERROR;
        //         else if (nRet == 0)
        //             return MSV_FAILED;
        // 
        // 
        //         CString strItem, strContent;
        // 
        // 
        // 
        //         strContent = parser.GetNodeText(_T("s_fileIndex"));
        //         nFileIndex = _wtoi(strContent);
        // 
        //         strContent = parser.GetNodeText(_T("s_fileFrame"));
        //         nFrame = _wtoi(strContent);
        // 
        //         //m_error_desc = _T("操作成功");
        //         m_error_desc = _T("Operation succeed");
        //         return MSV_SUCCESS;
        //     }
        // 
        //     /*MSV_FAILED = 0,   //执行失败
        //     MSV_SUCCESS = 1 , //执行成功
        //     MSV_XMLERROR = -1, //XML解析错误
        //     MSV_NETERROR = -2  // 网络连接错误/**/
        //     int CClientTaskSDKImp::CommandOperation(InParam pIn, OutParam& pOut)
        //     {
        //         //modefi by hongrui --try 2 times
        //         //int nRet=1;
        //         int nTry = 0;
        //         CString strItem, strContent;
        //         CString strLog;
        //         long lRet = pIn.m_s.SendCmdString(pIn.strSend);
        //         if (lRet == 0 || lRet == SOCKET_ERROR)
        //         {
        //             CString strError = _T("Failed to send data to MSV[%s] pIn.strSend=%s");
        //             m_error_desc.Format(strError, pIn.strIp, pIn.strSend);
        //             strLog.Format(_T("MSV_NETERROR.%s"), m_error_desc);
        //             NMTrace0(SDKLOGNAME, logLevelWarring, (_bstr_t)strLog);
        //             return MSV_NETERROR;
        //         }
        //         lRet = pIn.m_s.ReceiveCmdString(pOut.strRecv);
        //         if (lRet <= 0 || lRet == SOCKET_ERROR)
        //         {
        //             CString strError = _T("Error while receiving response from MSV[%s]. pIn.strSend=%s, pOut.strRecv=%s");
        //             m_error_desc.Format(strError, pIn.strIp, pIn.strSend, pOut.strRecv);
        //             strLog.Format(_T("MSV_NETERROR.%s"), m_error_desc);
        //             NMTrace0(SDKLOGNAME, logLevelWarring, (_bstr_t)strLog);
        //             return MSV_NETERROR;
        // 
        //         }
        // 
        //         if (FALSE == pIn.parser.Parse(pOut.strRecv))
        //         {
        //             CString strError = _T("Error occur while analyzing response message of MSV[%s] when Parse.pIn.strSend=%s,pOut.strRecv=%s");
        //             m_error_desc.Format(strError, pIn.strIp, pIn.strSend, pOut.strRecv);
        //             strLog.Format(_T("MSV_XMLERROR.%s"), m_error_desc);
        //             NMTrace0(SDKLOGNAME, logLevelWarring, (_bstr_t)strLog);
        //             return MSV_XMLERROR;
        //         }
        // 
        // 
        //         pIn.parser.GetRoot(strItem, strContent);
        // 
        //         if (_T("std_reply") != strItem)
        //         {
        //             CString strError = _T("Error answer info,send: %s,info: %s pIn.strSend=%s");
        //             m_error_desc.Format(strError, pIn.strIp, pOut.strRecv, pIn.strSend);
        //             strLog.Format(_T("MSV_XMLERROR.%s"), m_error_desc);
        //             NMTrace0(SDKLOGNAME, logLevelWarring, (_bstr_t)strLog);
        //             return MSV_XMLERROR;
        //         }
        //         strContent = pIn.parser.GetNodeText(_T("s_origin"));
        //         if (pIn.strOrigin != strContent)
        //         {
        //             CString strError = _T("pIn.strOrigin != strContent. MSV[%s];pIn.strSend=%s pOut.strRecv=%s");
        //             m_error_desc.Format(strError, pIn.strIp, pIn.strSend, pOut.strRecv);
        //             strLog.Format(_T("MSV_XMLERROR.%s"), m_error_desc);
        //             NMTrace0(SDKLOGNAME, logLevelWarring, (_bstr_t)strLog);
        //             return MSV_XMLERROR;
        //         }
        //         strContent = pIn.parser.GetNodeText(_T("s_result"));
        //         if (_T("succeed") != strContent)
        //         {
        //             CString strError;
        //             strError = pIn.parser.GetNodeText(_T("s_error_string"));
        //             m_error_desc = strError;
        //             strLog.Format(_T("MSV_FAILED.pIn.strSend=%s ,error desc=%s"), pIn.strSend, m_error_desc);
        //             NMTrace0(SDKLOGNAME, logLevelWarring, (_bstr_t)strLog);
        //             return MSV_FAILED;
        //         }
        //         //nRet = 1;
        //         return MSV_SUCCESS;
        //     }
        //     /************************************************************************/
        //     /* 修改任务名                                                           */
        //     /************************************************************************/
        //     MSV_RET CClientTaskSDKImp::MSVModifyManualTaskName(CString strMsvIp,
        //                                                     CString strTaskName,
        //                                                     int nChannel)
        //     {
        //         IRemoteSDK* m_s;
        //         m_s = CreateRemoteSDK();
        //         //	CRemoteSocket m_s(m_nTimeOut);
        //         if (FALSE == SocketConnect(strMsvIp, m_s))
        //         {
        //             CString strError = _T("Error while connecting MSV[%s]");
        //             m_error_desc.Format(strError, strMsvIp);
        //             //NMTrace(m_strLogName, logLevelWarring, m_error_desc);		
        //             return MSV_FAILED;
        //         }
        //         /*格式化参数到XML文本*/
        //         CString strSend(MXL_HEADER);
        //         CString strName;
        //         strName.Format(_T("<str_newName>%s</str_newName>"), strTaskName);
        //         /************************************************************/
        //         strSend += _T("<msv_modify_manual_name>");
        //         strSend += strName;
        //         CString strChannel;
        //         strChannel.Format(_T("<nChannel>%d</nChannel>"), nChannel);
        //         strSend += strChannel;
        //         strSend += _T("</msv_modify_manual_name>");
        //         /************************************************************/
        //         /* 操作   */
        // 
        //         CXMLParser parser;
        //         if (FALSE == parser.Create())
        //         {
        //             m_error_desc.Format(_T("Error occur while analyzing response message of MSV[%s]"), strMsvIp);
        //             //NMTrace(m_strLogName, logLevelWarring, m_error_desc);
        //             return MSV_FAILED;
        //         }
        //         InParam pIn;
        //         OutParam pOut;
        //         CString strRecv;
        //         pIn.m_s = m_s;
        //         //	pIn.m_s = &m_s;
        //         pIn.parser = &parser;
        //         pIn.strIp = strMsvIp;
        //         pIn.strOrigin = _T("msv_modify_manual_name");
        //         pIn.strSend = strSend;
        //         pOut.strRecv = strRecv;
        // 
        //         int nRet = CommandOperation(pIn, pOut);
        //         if (nRet == MSV_FAILED)
        //             return MSV_FAILED;
        // 
        //         /*	if(nRet == -2)
        //                 return MSV_NETERROR;
        //             else if(nRet == -1)
        //                 return MSV_XMLERROR;
        //             else if(nRet == 0)
        //                 return MSV_FAILED;
        //         */
        //         //m_error_desc = _T("操作成功");
        //         m_error_desc = _T("Operation succeed");
        //         return MSV_SUCCESS;
        //     }
        //     /************************************************************************/
        //     /* 修改任务采集时间                                                     */
        //     /************************************************************************/
        //     MSV_RET CClientTaskSDKImp::MSVModifyAutoTaskTime(CString strMsvIp,
        //     											  const long lTaskID,
        //                                                   CTime newTimeEnd,
        //                                                   int nChannel)
        //     {
        //         IRemoteSDK* m_s;
        //         m_s = CreateRemoteSDK();
        //         //	CRemoteSocket m_s(m_nTimeOut);
        //         if (FALSE == SocketConnect(strMsvIp, m_s))
        //         {
        //             CString strError = _T("Error while connecting MSV[%s]");
        //             m_error_desc.Format(strError, strMsvIp);
        //             //NMTrace(m_strLogName, logLevelWarring, m_error_desc);
        //             return MSV_FAILED;
        //         }
        //         /*格式化参数到XML文本*/
        //         CString strSend(MXL_HEADER);
        //         CString strName;
        //         CString strTime;
        //         strTime = newTimeEnd.Format(_T("%Y-%m-%d %H:%M:%S"));
        //         strName.Format(_T("<newTimeEnd>%s</newTimeEnd>"), strTime);
        //         CString strTaskID;
        //         strTaskID.Format(_T("<nTaskID>%d</nTaskID>"), lTaskID);
        //         /************************************************************/
        //         strSend += _T("<msv_modify_task_time>");
        //         strSend += strTaskID;
        //         strSend += strName;
        //         CString strChannel;
        //         strChannel.Format(_T("<nChannel>%d</nChannel>"), nChannel);
        //         strSend += strChannel;
        //         strSend += _T("</msv_modify_task_time>");
        //         /************************************************************/
        //         /* 操作   */
        // 
        //         CXMLParser parser;
        //         if (FALSE == parser.Create())
        //         {
        //             CString strError = _T("Error occur while analyzing response message of MSV[%s]");
        //             m_error_desc.Format(strError, strMsvIp);
        //             //NMTrace(m_strLogName, logLevelWarring, m_error_desc);
        //             return MSV_FAILED;
        //         }
        //         InParam pIn;
        //         OutParam pOut;
        //         CString strRecv;
        //         pIn.m_s = m_s;
        //         //	pIn.m_s = &m_s;
        //         pIn.parser = &parser;
        //         pIn.strIp = strMsvIp;
        //         pIn.strOrigin = _T("msv_modify_task_time");
        //         pIn.strSend = strSend;
        //         pOut.strRecv = strRecv;
        // 
        //         int nRet = CommandOperation(pIn, pOut);
        //         if (nRet == -2)
        //             return MSV_NETERROR;
        //         else if (nRet == -1)
        //             return MSV_XMLERROR;
        //         else if (nRet == 0)
        //             return MSV_FAILED;
        // 
        //         //m_error_desc = _T("操作成功");
        //         m_error_desc = _T("Operation succeed");
        //         return MSV_SUCCESS;
        // 
        // 
        //     }
        //     /************************************************************************/
        //     /*                                                                      */
        //     /************************************************************************/
        //     MSV_RET CClientTaskSDKImp::MSVSetSignedID(CString strMsvIP,
        //                                            CString strSignID,
        //                                            int nChannel)
        //     {
        //         //msv_set_sign_id
        //         IRemoteSDK* m_s;
        //         m_s = CreateRemoteSDK();
        //         //	CRemoteSocket m_s(m_nTimeOut);
        //         if (FALSE == SocketConnect(strMsvIP, m_s))
        //         {
        //             CString strError = _T("Error while connecting MSV[%s]");
        //             m_error_desc.Format(strError, strMsvIP);
        //             //NMTrace(m_strLogName, logLevelWarring, m_error_desc);
        //             return MSV_FAILED;
        //         }
        //         /*格式化参数到XML文本*/
        //         CString strSend(MXL_HEADER);
        //         CString strParam;
        //         strParam.Format(_T("<strSignID>%s</strSignID>"), strSignID);
        //         /************************************************************/
        //         strSend += _T("<msv_set_sign_id>");
        //         strSend += strParam;
        //         CString strChannel;
        //         strChannel.Format(_T("<nChannel>%d</nChannel>"), nChannel);
        //         strSend += strChannel;
        //         strSend += _T("</msv_set_sign_id>");
        //         /************************************************************/
        //         /* 操作   */
        // 
        //         CXMLParser parser;
        //         if (FALSE == parser.Create())
        //         {
        //             CString strError = _T("Error occur while analyzing response message of MSV[%s]");
        //             m_error_desc.Format(strError, strMsvIP);
        //             //NMTrace(m_strLogName, logLevelWarring, m_error_desc);
        //             return MSV_FAILED;
        //         }
        //         InParam pIn;
        //         OutParam pOut;
        //         CString strRecv;
        //         pIn.m_s = m_s;
        //         //	pIn.m_s = &m_s;
        //         pIn.parser = &parser;
        //         pIn.strIp = strMsvIP;
        //         pIn.strOrigin = _T("msv_set_sign_id");
        //         pIn.strSend = strSend;
        //         pOut.strRecv = strRecv;
        // 
        //         int nRet = CommandOperation(pIn, pOut);
        //         if (nRet == -2)
        //             return MSV_NETERROR;
        //         else if (nRet == -1)
        //             return MSV_XMLERROR;
        //         else if (nRet == 0)
        //             return MSV_FAILED;
        // 
        //         //m_error_desc = _T("操作成功");
        //         m_error_desc = _T("Operation succeed");
        //         return MSV_SUCCESS;
        //     }
        //     MSV_RET CClientTaskSDKImp::MSVReleaseSignedID(CString strMsvIP,
        //                                                CString releaseID,
        //                                                int nChannel)
        //     {
        //         //msv_release_sign_id
        //         IRemoteSDK* m_s;
        //         m_s = CreateRemoteSDK();
        //         //	CRemoteSocket m_s(m_nTimeOut);
        //         if (FALSE == SocketConnect(strMsvIP, m_s))
        //         {
        //             CString strError = _T("Error while connecting MSV[%s]");
        //             m_error_desc.Format(strError, strMsvIP);
        //             //NMTrace(m_strLogName, logLevelWarring, m_error_desc);
        //             return MSV_FAILED;
        //         }
        //         /*格式化参数到XML文本*/
        //         CString strSend(MXL_HEADER);
        //         CString strParam;
        //         strParam.Format(_T("<strSignID>%s</strSignID>"), releaseID);
        //         /************************************************************/
        //         strSend += _T("<msv_release_sign_id>");
        //         strSend += strParam;
        //         CString strChannel;
        //         strChannel.Format(_T("<nChannel>%d</nChannel>"), nChannel);
        //         strSend += strChannel;
        //         strSend += _T("</msv_release_sign_id>");
        //         /************************************************************/
        //         /* 操作   */
        // 
        //         CXMLParser parser;
        //         if (FALSE == parser.Create())
        //         {
        //             CString strError = _T("Error occur while analyzing response message of MSV[%s]");
        //             m_error_desc.Format(strError, strMsvIP);
        //             //NMTrace(m_strLogName, logLevelWarring, m_error_desc);
        //             return MSV_FAILED;
        //         }
        //         InParam pIn;
        //         OutParam pOut;
        //         CString strRecv;
        //         pIn.m_s = m_s;
        //         //	pIn.m_s = &m_s;
        //         pIn.parser = &parser;
        //         pIn.strIp = strMsvIP;
        //         pIn.strOrigin = _T("msv_release_sign_id");
        //         pIn.strSend = strSend;
        //         pOut.strRecv = strRecv;
        // 
        //         int nRet = CommandOperation(pIn, pOut);
        //         if (nRet == -2)
        //             return MSV_NETERROR;
        //         else if (nRet == -1)
        //             return MSV_XMLERROR;
        //         else if (nRet == 0)
        //             return MSV_FAILED;
        // 
        //         //m_error_desc = _T("操作成功");
        //         m_error_desc = _T("Operation succeed");
        //         return MSV_SUCCESS;
        // 
        //     }
        // 
        //     //设置是否启用存储均衡策略
        //     MSV_RET CClientTaskSDKImp::MSVSetStorageBalanceState(CString strMsvIP,
        //                                                       BOOL bUse,
        //                                                       int nChannel)
        //     {
        //         //msv_release_sign_id
        //         IRemoteSDK* m_s;
        //         m_s = CreateRemoteSDK();
        //         //	CRemoteSocket m_s(m_nTimeOut);
        //         if (FALSE == SocketConnect(strMsvIP, m_s))
        //         {
        //             CString strError = _T("Error while connecting MSV[%s]");
        //             m_error_desc.Format(strError, strMsvIP);
        //             //NMTrace(m_strLogName, logLevelWarring, m_error_desc);
        //             return MSV_FAILED;
        //         }
        //         /*格式化参数到XML文本*/
        //         CString strSend(MXL_HEADER);
        //         CString strParam;
        //         strParam.Format(_T("<strBEnable>%d</strBEnable>"), bUse);
        //         /************************************************************/
        //         strSend += _T("<msv_set_equipoise_state>");
        //         strSend += strParam;
        //         CString strChannel;
        //         strChannel.Format(_T("<nChannel>%d</nChannel>"), nChannel);
        //         strSend += strChannel;
        //         strSend += _T("</msv_set_equipoise_state>");
        //         /************************************************************/
        //         /* 操作   */
        // 
        //         CXMLParser parser;
        //         if (FALSE == parser.Create())
        //         {
        //             CString strError = _T("Error occur while analyzing response message of MSV[%s]");
        //             m_error_desc.Format(strError, strMsvIP);
        //             //NMTrace(m_strLogName, logLevelWarring, m_error_desc);
        //             return MSV_FAILED;
        //         }
        //         InParam pIn;
        //         OutParam pOut;
        //         CString strRecv;
        //         pIn.m_s = m_s;
        //         //	pIn.m_s = &m_s;
        //         pIn.parser = &parser;
        //         pIn.strIp = strMsvIP;
        //         pIn.strOrigin = _T("msv_set_equipoise_state");
        //         pIn.strSend = strSend;
        //         pOut.strRecv = strRecv;
        // 
        //         int nRet = CommandOperation(pIn, pOut);
        //         if (nRet == -2)
        //             return MSV_NETERROR;
        //         else if (nRet == -1)
        //             return MSV_XMLERROR;
        //         else if (nRet == 0)
        //             return MSV_FAILED;
        // 
        //         //m_error_desc = _T("操作成功");
        //         m_error_desc = _T("Operation succeed");
        //         return MSV_SUCCESS;
        //     }
        //     //设置存储均衡盘列表
        //     MSV_RET CClientTaskSDKImp::MSVSetStorageList(CString strMsvIP,
        //                                               CArray<CString, CString&>& storageList,
        //                                               int nChannel)
        //     {
        //         m_error_desc = _T("Target system can not support this function");
        //         //m_error_desc = _T("目前系统不支持此功能");
        //         return MSV_FAILED;
        // 
        //         int nSize = (int)storageList.GetSize();
        //         if (nSize == 0)
        //         {
        //             m_error_desc = _T("Invalid parameter list");
        //             //NMTrace(m_strLogName, logLevelWarring, m_error_desc);
        //             return MSV_FAILED;
        //         }
        //         //msv_release_sign_id
        //         IRemoteSDK* m_s;
        //         m_s = CreateRemoteSDK();
        //         //	CRemoteSocket m_s(m_nTimeOut);
        //         if (FALSE == SocketConnect(strMsvIP, m_s))
        //         {
        //             CString strError = _T("Error while connecting MSV[%s]");
        //             m_error_desc.Format(strError, strMsvIP);
        //             //NMTrace(m_strLogName, logLevelWarring, m_error_desc);
        //             return MSV_FAILED;
        //         }
        //         /*格式化参数到XML文本*/
        //         CString strSend(MXL_HEADER);
        // 
        //         CString strParam;
        //         CString strTemp;
        //         for (int i = 0; i < nSize; i++)
        //         {
        //             if (i < nSize - 1)
        //             {
        //                 strTemp.Format(_T("%s+"), storageList.GetAt(i));
        //             }
        //             else
        //                 strTemp.Format(_T("%s"), storageList.GetAt(i));
        //             strParam += strTemp;
        // 
        //         }
        //         strTemp.Format(_T("<strDiskList>%s</strDiskList>"), strParam);
        //         /************************************************************/
        //         strSend += _T("<msv_set_equipoise_content>");
        //         strSend += strTemp;
        //         CString strChannel;
        //         strChannel.Format(_T("<nChannel>%d</nChannel>"), nChannel);
        //         strSend += strChannel;
        //         strSend += _T("</msv_set_equipoise_content>");
        //         /************************************************************/
        //         /* 操作   */
        // 
        //         CXMLParser parser;
        //         if (FALSE == parser.Create())
        //         {
        //             CString strError = _T("Error occur while analyzing response message of MSV[%s]");
        //             m_error_desc.Format(strError, strMsvIP);
        //             //NMTrace(m_strLogName, logLevelWarring, m_error_desc);
        //             return MSV_FAILED;
        //         }
        //         InParam pIn;
        //         OutParam pOut;
        //         CString strRecv;
        //         pIn.m_s = m_s;
        //         //	pIn.m_s = &m_s;
        //         pIn.parser = &parser;
        //         pIn.strIp = strMsvIP;
        //         pIn.strOrigin = _T("msv_set_equipoise_content");
        //         pIn.strSend = strSend;
        //         pOut.strRecv = strRecv;
        // 
        //         int nRet = CommandOperation(pIn, pOut);
        //         if (nRet == -2)
        //             return MSV_NETERROR;
        //         else if (nRet == -1)
        //             return MSV_XMLERROR;
        //         else if (nRet == 0)
        //             return MSV_FAILED;
        // 
        //         //m_error_desc = _T("操作成功");
        //         m_error_desc = _T("Operation succeed");
        //         return MSV_SUCCESS;
        //     }
        //     //获取存储均衡策略信息
        //     MSV_RET CClientTaskSDKImp::MSVGetStorageBalanceInfo(CString strMsvIP,
        //                                                      BOOL& bUse,
        //                                                      CArray<CString, CString&>& storageList,
        //                                                      int nChannel)
        //     {
        //         m_error_desc = _T("目前系统不支持此功能");
        //         return MSV_FAILED;
        //         //msv_release_sign_id
        //         IRemoteSDK* m_s;
        //         m_s = CreateRemoteSDK();
        //         //	CRemoteSocket m_s(m_nTimeOut);
        //         if (FALSE == SocketConnect(strMsvIP, m_s))
        //         {
        //             CString strError = _T("Error while connecting MSV[%s]");
        //             m_error_desc.Format(strError, strMsvIP);
        //             //NMTrace(m_strLogName, logLevelWarring, m_error_desc);
        //             return MSV_FAILED;
        //         }
        //         /*格式化参数到XML文本*/
        //         CString strSend(MXL_HEADER);
        //         /************************************************************/
        //         strSend += _T("<msv_get_equipoise_state>");
        //         CString strChannel;
        //         strChannel.Format(_T("<nChannel>%d</nChannel>"), nChannel);
        //         strSend += strChannel;
        //         strSend += _T("</msv_get_equipoise_state>");
        //         /************************************************************/
        //         /* 操作   */
        //         CXMLParser parser;
        //         if (FALSE == parser.Create())
        //         {
        //             CString strError = _T("Error occur while analyzing response message of MSV[%s]");
        //             m_error_desc.Format(strError, strMsvIP);
        //             //NMTrace(m_strLogName, logLevelWarring, m_error_desc);
        //             return MSV_FAILED;
        //         }
        //         InParam pIn;
        //         OutParam pOut;
        //         CString strRecv;
        //         pIn.m_s = m_s;
        //         //	pIn.m_s = &m_s;
        //         pIn.parser = &parser;
        //         pIn.strIp = strMsvIP;
        //         pIn.strOrigin = _T("msv_get_equipoise_state");
        //         pIn.strSend = strSend;
        //         pOut.strRecv = strRecv;
        // 
        //         int nRet = CommandOperation(pIn, pOut);
        //         if (nRet == -2)
        //             return MSV_NETERROR;
        //         else if (nRet == -1)
        //             return MSV_XMLERROR;
        //         else if (nRet == 0)
        //             return MSV_FAILED;
        // 
        // 
        //         CString strContent;
        //         //
        //         strContent = parser.GetNodeText(_T("s_state"));
        //         bUse = (BOOL)(_wtoi(strContent));
        //         //
        //         strContent = parser.GetNodeText(_T("s_diskList"));
        //         GetDriverListFromString(strContent, storageList);
        // 
        //         //m_error_desc = _T("操作成功");
        //         m_error_desc = _T("Operation succeed");
        //         return MSV_SUCCESS;
        //     }
        // 
        //     /************************************************************************/
        //     /* 以手动采集参数开始采集                                               */
        //     /************************************************************************/
        //     MSV_RET CClientTaskSDKImp::MsvStartTaskWithLocalParam(const CString strMsvIP,
        //                                                        int nChannel)
        //     {
        //         //msv_release_sign_id
        //         IRemoteSDK* m_s;
        //         m_s = CreateRemoteSDK();
        //         //	CRemoteSocket m_s(m_nTimeOut);
        //         if (FALSE == SocketConnect(strMsvIP, m_s))
        //         {
        //             CString strError = _T("Error while connecting MSV[%s]");
        //             m_error_desc.Format(strError, strMsvIP);
        //             //NMTrace(m_strLogName, logLevelWarring, m_error_desc);
        //             return MSV_FAILED;
        //         }
        //         /*格式化参数到XML文本*/
        //         CString strSend(MXL_HEADER);
        //         /************************************************************/
        //         strSend += _T("<msv_start_task_local>");
        //         CString strChannel;
        //         strChannel.Format(_T("<nChannel>%d</nChannel>"), nChannel);
        //         strSend += strChannel;
        //         strSend += _T("</msv_start_task_local>");
        //         /************************************************************/
        //         /* 操作   */
        //         CXMLParser parser;
        //         if (FALSE == parser.Create())
        //         {
        //             CString strError = _T("Error occur while analyzing response message of MSV[%s]");
        //             m_error_desc.Format(strError, strMsvIP);
        //             //NMTrace(m_strLogName, logLevelWarring, m_error_desc);
        //             return MSV_FAILED;
        //         }
        //         InParam pIn;
        //         OutParam pOut;
        //         CString strRecv;
        //         pIn.m_s = m_s;
        //         //	pIn.m_s = &m_s;
        //         pIn.parser = &parser;
        //         pIn.strIp = strMsvIP;
        //         pIn.strOrigin = _T("msv_start_task_local");
        //         pIn.strSend = strSend;
        //         pOut.strRecv = strRecv;
        // 
        //         int nRet = CommandOperation(pIn, pOut);
        //         if (nRet == -2)
        //             return MSV_NETERROR;
        //         else if (nRet == -1)
        //             return MSV_XMLERROR;
        //         else if (nRet == 0)
        //             return MSV_FAILED;
        // 
        //         //m_error_desc = _T("操作成功");
        //         m_error_desc = _T("Operation succeed");
        //         return MSV_SUCCESS;
        //     }
        // 
        //     MSV_RET CClientTaskSDKImp::MsvQueryDiskInfo(CString strMsvIP,
        //     										 const CString strDisk,
        //                                              disk_info &info,
        //                                              int nChannel)
        //     {
        //         //msv_release_sign_id
        //         IRemoteSDK* m_s;
        //         m_s = CreateRemoteSDK();
        //         //	CRemoteSocket m_s(m_nTimeOut);
        //         if (FALSE == SocketConnect(strMsvIP, m_s))
        //         {
        //             CString strError = _T("Error while connecting MSV[%s]");
        //             m_error_desc.Format(strError, strMsvIP);
        //             //NMTrace(m_strLogName, logLevelWarring, m_error_desc);
        //             return MSV_FAILED;
        //         }
        //         /*格式化参数到XML文本*/
        //         CString strSend(MXL_HEADER);
        //         CString strParam(strDisk);
        //         strParam.TrimLeft();
        //         if (strParam == _T(""))
        //         {
        //             m_error_desc = _T("Please set the HD drive letter first");
        //             //NMTrace(m_strLogName, logLevelWarring, m_error_desc);
        //             return MSV_FAILED;
        //         }
        //         CString strT;
        //         strParam = strParam.Left(1);
        //         strT.Format(_T("<disk>%s</disk>"), strParam);
        //         /************************************************************/
        //         strSend += _T("<msv_query_disk_info>");
        //         strSend += strT;
        //         CString strChannel;
        //         strChannel.Format(_T("<nChannel>%d</nChannel>"), nChannel);
        //         strSend += strChannel;
        //         strSend += _T("</msv_query_disk_info>");
        //         /************************************************************/
        //         /* 操作   */
        //         CXMLParser parser;
        //         if (FALSE == parser.Create())
        //         {
        //             CString strError = _T("Error occur while analyzing response message of MSV[%s]");
        //             m_error_desc.Format(strError, strMsvIP);
        //             //NMTrace(m_strLogName, logLevelWarring, m_error_desc);
        //             return MSV_FAILED;
        //         }
        //         InParam pIn;
        //         OutParam pOut;
        //         CString strRecv;
        //         pIn.m_s = m_s;
        //         //	pIn.m_s = &m_s;
        //         pIn.parser = &parser;
        //         pIn.strIp = strMsvIP;
        //         pIn.strOrigin = _T("msv_query_disk_info");
        //         pIn.strSend = strSend;
        //         pOut.strRecv = strRecv;
        // 
        //         int nRet = CommandOperation(pIn, pOut);
        //         if (nRet == -2)
        //             return MSV_NETERROR;
        //         else if (nRet == -1)
        //             return MSV_XMLERROR;
        //         else if (nRet == 0)
        //             return MSV_FAILED;
        // 
        //         TCHAR* restring = NULL;
        //         info.diskName = parser.GetNodeText(_T("disk"));
        //         info.nTotolSize = (float)wcstod(parser.GetNodeText(_T("total")), &restring);
        //         info.nFreeSize = (float)wcstod(parser.GetNodeText(_T("free")), &restring);
        // 
        //         //m_error_desc = _T("操作成功");
        //         m_error_desc = _T("Operation succeed");
        //         return MSV_SUCCESS;
        //     }
        // 

        // 
        //     //按序号恢复素材
        //     MSV_RET CClientTaskSDKImp::MsvRecoverClipByIndex(const CString strMsvIP,
        //     											  const int nIndex,
        //                                                   BOOL bDelete,
        //                                                   int nChannel)
        //     {
        //         //msv_release_sign_id
        //         IRemoteSDK* m_s;
        //         m_s = CreateRemoteSDK();
        //         //	CRemoteSocket m_s(m_nTimeOut);
        //         if (FALSE == SocketConnectTrans(strMsvIP, m_s))
        //         {
        //             CString strError = _T("Error while connecting MSV[%s]");
        //             m_error_desc.Format(strError, strMsvIP);
        //             //NMTrace(m_strLogName, logLevelWarring, m_error_desc);
        //             return MSV_FAILED;
        //         }
        //         /*格式化参数到XML文本*/
        //         CString strSend(MXL_HEADER);
        //         CString strParam;
        //         strParam.Format(_T("<index>%d</index><bdelete>%d</bdelete>"), nIndex, bDelete);
        //         /************************************************************/
        //         strSend += _T("<msv_recoverclip_byindex>");
        //         strSend += strParam;
        //         CString strChannel;
        //         strChannel.Format(_T("<nChannel>%d</nChannel>"), nChannel);
        //         strSend += strChannel;
        //         strSend += _T("</msv_recoverclip_byindex>");
        //         /************************************************************/
        //         /* 操作   */
        //         CXMLParser parser;
        //         if (FALSE == parser.Create())
        //         {
        //             CString strError = _T("Error occur while analyzing response message of MSV[%s]");
        //             m_error_desc.Format(strError, strMsvIP);
        //             //NMTrace(m_strLogName, logLevelWarring, m_error_desc);
        //             return MSV_FAILED;
        //         }
        //         InParam pIn;
        //         OutParam pOut;
        //         CString strRecv;
        //         pIn.m_s = m_s;
        //         //	pIn.m_s = &m_s;
        //         pIn.parser = &parser;
        //         pIn.strIp = strMsvIP;
        //         pIn.strOrigin = _T("msv_recoverclip_byindex");
        //         pIn.strSend = strSend;
        //         pOut.strRecv = strRecv;
        // 
        //         int nRet = CommandOperation(pIn, pOut);
        //         if (nRet == -2)
        //             return MSV_NETERROR;
        //         else if (nRet == -1)
        //             return MSV_XMLERROR;
        //         else if (nRet == 0)
        //             return MSV_FAILED;
        // 
        //         //m_error_desc = _T("操作成功");
        //         m_error_desc = _T("Operation succeed");
        //         return MSV_SUCCESS;
        //     }
        // 
        //     //删除指定备份
        //     MSV_RET CClientTaskSDKImp::MsvDeleteBackUpClipByIndex(const CString strMsvIP,
        //     												   const int nIndex,
        //                                                        int nChannel)
        //     {
        //         //msv_release_sign_id
        //         IRemoteSDK* m_s;
        //         m_s = CreateRemoteSDK();
        //         //	CRemoteSocket m_s(m_nTimeOut);
        //         if (FALSE == SocketConnectTrans(strMsvIP, m_s))
        //         {
        //             CString strError = _T("Error while connecting MSV[%s]");
        //             m_error_desc.Format(strError, strMsvIP);
        //             //NMTrace(m_strLogName, logLevelWarring, m_error_desc);
        //             return MSV_FAILED;
        //         }
        //         /*格式化参数到XML文本*/
        //         CString strSend(MXL_HEADER);
        //         CString strParam;
        //         strParam.Format(_T("<index>%d</index>"), nIndex);
        //         /************************************************************/
        //         strSend += _T("<msv_deletebackup_byindex>");
        //         strSend += strParam;
        //         CString strChannel;
        //         strChannel.Format(_T("<nChannel>%d</nChannel>"), nChannel);
        //         strSend += strChannel;
        //         strSend += _T("</msv_deletebackup_byindex>");
        //         /************************************************************/
        //         /* 操作   */
        //         CXMLParser parser;
        //         if (FALSE == parser.Create())
        //         {
        //             CString strError = _T("Error occur while analyzing response message of MSV[%s]");
        //             m_error_desc.Format(strError, strMsvIP);
        //             //NMTrace(m_strLogName, logLevelWarring, m_error_desc);
        //             return MSV_FAILED;
        //         }
        //         InParam pIn;
        //         OutParam pOut;
        //         CString strRecv;
        //         pIn.m_s = m_s;
        //         //	pIn.m_s = &m_s;
        //         pIn.parser = &parser;
        //         pIn.strIp = strMsvIP;
        //         pIn.strOrigin = _T("msv_deletebackup_byindex");
        //         pIn.strSend = strSend;
        //         pOut.strRecv = strRecv;
        // 
        //         int nRet = CommandOperation(pIn, pOut);
        //         if (nRet == -2)
        //             return MSV_NETERROR;
        //         else if (nRet == -1)
        //             return MSV_XMLERROR;
        //         else if (nRet == 0)
        //             return MSV_FAILED;
        // 
        //         //m_error_desc = _T("操作成功");
        //         m_error_desc = _T("Operation succeed");
        //         return MSV_SUCCESS;
        //     }
        //     //为指定的服务端在MSV上注册消息
        //     MSV_RET CClientTaskSDKImp::RegisterNotifyEx(CString strMsvIp,
        //                                              CString strServerIP,
        //                                              UINT uiNotifyID,
        //                                              int nChannel)
        //     {
        //         //	CRemoteSocket m_sTrans(m_nTimeOut);
        //         //	CRemoteSocket m_sMSV(m_nTimeOut);
        //         IRemoteSDK* m_sTrans;
        //         IRemoteSDK* m_sMSV;
        //         m_sTrans = CreateRemoteSDK();
        //         m_sMSV = CreateRemoteSDK();
        //         if (strServerIP.GetLength() <= 0)
        //         {
        //             m_error_desc = _T("Invalid parameter");
        //             //NMTrace(m_strLogName, logLevelWarring, m_error_desc);
        //             return MSV_FAILED;
        //         }
        //         /*
        //             switch(uiNotifyID) {
        //             case 7:
        //             case 8:
        //                 if(FALSE == SocketConnectTrans(strMsvIp,m_s))
        //                 {
        //                     CString strError = _T("Error while connecting MSV[%s]");
        //                 m_error_desc.Format(strError, strMsvIp);
        //                 //NMTrace(m_strLogName, logLevelWarring, m_error_desc);//(_T("连接媒体服务器[%s] 错误"),strMsvIp);
        //                     return MSV_FAILED;
        //                 }
        //                 break;
        //             default:
        //                 if(FALSE == SocketConnect(strMsvIp,m_s))
        //                 {		
        //                     CString strError = _T("Error while connecting MSV[%s]");
        //                 m_error_desc.Format(strError, strMsvIp);
        //                 //NMTrace(m_strLogName, logLevelWarring, m_error_desc);//(_T("连接媒体服务器[%s] 错误"),strMsvIp);
        //                     return MSV_FAILED;
        //                 }
        //                 break;
        //             }
        //         */
        //         if (FALSE == SocketConnectTrans(strMsvIp, m_sTrans))
        //         {
        //             CString strError = _T("Error while connecting MSV[%s]");
        //             m_error_desc.Format(strError, strMsvIp);
        //             //NMTrace(m_strLogName, logLevelWarring, m_error_desc);
        //             return MSV_FAILED;
        //         }
        //         if (FALSE == SocketConnect(strMsvIp, m_sMSV))
        //         {
        //             CString strError = _T("Error while connecting MSV[%s]");
        //             m_error_desc.Format(strError, strMsvIp);
        //             //NMTrace(m_strLogName, logLevelWarring, m_error_desc);
        //             return MSV_FAILED;
        //         }
        //         CString strSend(MXL_HEADER);
        //         /************************************************************/
        //         strSend += _T("<register_notify_ex>");
        //         CString strTemp;
        //         strTemp.Format(_T("<s_id>%d</s_id>"), uiNotifyID); //任务ID
        //         strSend += strTemp;
        //         strTemp.Format(_T("<s_ServerIP>%s</s_ServerIP>"), strServerIP);
        //         strSend += strTemp;
        //         CString strChannel;
        //         strChannel.Format(_T("<nChannel>%d</nChannel>"), nChannel);
        //         strSend += strChannel;
        //         strSend += _T("</register_notify_ex>");  //查询手动任务
        //                                                  /************************************************************/
        // 
        //         CXMLParser parser;
        //         if (FALSE == parser.Create())
        //         {
        //             CString strError = _T("Error occur while analyzing response message of MSV[%s]");
        //             m_error_desc.Format(strError, strMsvIp);
        //             //NMTrace(m_strLogName, logLevelWarring, m_error_desc);
        //             return MSV_FAILED;
        //         }
        //         InParam pIn;
        //         OutParam pOut;
        //         CString strRecv;
        //         pIn.m_s = m_sMSV;
        //         pIn.parser = &parser;
        //         pIn.strIp = strMsvIp;
        //         pIn.strOrigin = _T("register_notify_ex");
        //         pIn.strSend = strSend;
        //         pOut.strRecv = strRecv;
        // 
        //         int nRet = CommandOperation(pIn, pOut);
        //         if (nRet == -2)
        //             return MSV_NETERROR;
        //         else if (nRet == -1)
        //             return MSV_XMLERROR;
        //         else if (nRet == 0)
        //             return MSV_FAILED;
        // 
        //         strRecv = _T("");
        //         pIn.m_s = m_sTrans;
        //         pIn.parser = &parser;
        //         pIn.strIp = strMsvIp;
        //         pIn.strOrigin = _T("register_notify_ex");
        //         pIn.strSend = strSend;
        //         pOut.strRecv = strRecv;
        // 
        //         nRet = CommandOperation(pIn, pOut);
        //         if (nRet == -2)
        //             return MSV_NETERROR;
        //         else if (nRet == -1)
        //             return MSV_XMLERROR;
        //         else if (nRet == 0)
        //             return MSV_FAILED;
        // 
        //         //m_error_desc = _T("操作成功");
        //         m_error_desc = _T("Operation succeed");
        //         return MSV_SUCCESS;
        //     }
        // 
        // 
        //     // 按文件名获取MSV日志文件内容                                          
        //     MSV_RET CClientTaskSDKImp::MSVGetRecentLogFileContent(const CString strMsvIP, CString& strLog , int nChannel)
        //     {
        // 
        //         //msv_release_sign_id
        //         IRemoteSDK* m_s;
        //         m_s = CreateRemoteSDK();
        //         //	CRemoteSocket m_s(m_nTimeOut);
        //         if (FALSE == SocketConnect(strMsvIP, m_s))
        //         {
        //             CString strError = _T("Error while connecting MSV[%s]");
        //             m_error_desc.Format(strError, strMsvIP);
        //             //NMTrace(m_strLogName, logLevelWarring, m_error_desc);
        //             return MSV_FAILED;
        //         }
        //         /*格式化参数到XML文本*/
        //         CString strSend(MXL_HEADER);
        //         /************************************************************/
        //         strSend += _T("<msv_getrecent_detail>");
        //         CString strChannel;
        //         strChannel.Format(_T("<nChannel>%d</nChannel>"), nChannel);
        //         strSend += strChannel;
        //         strSend += _T("</msv_getrecent_detail>");
        //         /************************************************************/
        //         /* 操作   */
        //         CXMLParser parser;
        //         if (FALSE == parser.Create())
        //         {
        //             CString strError = _T("Error occur while analyzing response message of MSV[%s]");
        //             m_error_desc.Format(strError, strMsvIP);
        //             //NMTrace(m_strLogName, logLevelWarring, m_error_desc);
        //             return MSV_FAILED;
        //         }
        //         InParam pIn;
        //         OutParam pOut;
        //         CString strRecv;
        //         pIn.m_s = m_s;
        //         //	pIn.m_s = &m_s;
        //         pIn.parser = &parser;
        //         pIn.strIp = strMsvIP;
        //         pIn.strOrigin = _T("msv_getrecent_detail");
        //         pIn.strSend = strSend;
        //         pOut.strRecv = strRecv;
        // 
        //         int nRet = CommandOperation(pIn, pOut);
        //         if (nRet == -2)
        //             return MSV_NETERROR;
        //         else if (nRet == -1)
        //             return MSV_XMLERROR;
        //         else if (nRet == 0)
        //             return MSV_FAILED;
        //         strLog = parser.GetNodeText(_T("log_content"));
        // 
        //         //m_error_desc = _T("操作成功");
        //         m_error_desc = _T("Operation succeed");
        //         return MSV_SUCCESS;
        //     }
        //     // 取MSV最近一条日志文件，和文件名                                         
        //     MSV_RET CClientTaskSDKImp::MSVGetLogFileDetail(const CString strMsvIP,
        //                                                  CString& strFileName,
        //                                                  int nChannel)
        //     {
        //         //m_error_desc = _T("操作成功");
        //         m_error_desc = _T("Operation succeed");
        //         return MSV_SUCCESS;
        //     }
        // 
        // 
        //     MSV_RET CClientTaskSDKImp::MSVSetNetPreview(const CString strMsvIp,
        //                                              BOOL bPreview,
        //                                              int nChannel)
        //     {
        //         IRemoteSDK* m_s;
        //         m_s = CreateRemoteSDK();
        //         //	CRemoteSocket m_s(m_nTimeOut);
        //         if (FALSE == SocketConnect(strMsvIp, m_s))
        //         {
        //             CString strError = _T("Error while connecting MSV[%s]");
        //             m_error_desc.Format(strError, strMsvIp);
        //             //NMTrace(m_strLogName, logLevelWarring, m_error_desc);
        //             return MSV_FAILED;
        //         }
        //         /*格式化参数到XML文本*/
        //         CString strSend(MXL_HEADER);
        //         CString strParam;
        //         strParam.Format(_T("<b_enable>%d</b_enable>"), bPreview);
        //         /************************************************************/
        //         strSend += _T("<msv_set_net_preview>");
        //         strSend += strParam;
        //         CString strChannel;
        //         strChannel.Format(_T("<nChannel>%d</nChannel>"), nChannel);
        //         strSend += strChannel;
        //         strSend += _T("</msv_set_net_preview>");
        //         /************************************************************/
        //         /* 操作   */
        // 
        //         CXMLParser parser;
        //         if (FALSE == parser.Create())
        //         {
        //             CString strError = _T("Error occur while analyzing response message of MSV[%s]");
        //             m_error_desc.Format(strError, strMsvIp);
        //             //NMTrace(m_strLogName, logLevelWarring, m_error_desc);
        //             return MSV_FAILED;
        //         }
        //         InParam pIn;
        //         OutParam pOut;
        //         CString strRecv;
        //         pIn.m_s = m_s;
        //         //	pIn.m_s = &m_s;
        //         pIn.parser = &parser;
        //         pIn.strIp = strMsvIp;
        //         pIn.strOrigin = _T("msv_set_net_preview");
        //         pIn.strSend = strSend;
        //         pOut.strRecv = strRecv;
        // 
        //         int nRet = CommandOperation(pIn, pOut);
        //         if (nRet == -2)
        //             return MSV_NETERROR;
        //         else if (nRet == -1)
        //             return MSV_XMLERROR;
        //         else if (nRet == 0)
        //             return MSV_FAILED;
        // 
        //         //m_error_desc = _T("操作成功");
        //         m_error_desc = _T("Operation succeed");
        //         return MSV_SUCCESS;
        //     }
        // 
        //     MSV_RET CClientTaskSDKImp::MSVMapNetDriver(const CString strMsvIp,
        //                                             CString strDriverName,
        //                                             CString strNetPath,
        //                                             CString strUserName,
        //                                             CString strPassWord,
        //                                             int nChannel)
        //     {
        //         IRemoteSDK* m_s;
        //         m_s = CreateRemoteSDK();
        //         //	CRemoteSocket m_s(m_nTimeOut);
        //         if (FALSE == SocketConnect(strMsvIp, m_s))
        //         {
        //             CString strError = _T("Error while connecting MSV[%s]");
        //             m_error_desc.Format(strError, strMsvIp);
        //             //NMTrace(m_strLogName, logLevelWarring, m_error_desc);
        //             return MSV_FAILED;
        //         }
        //         /*格式化参数到XML文本*/
        //         CString strSend(MXL_HEADER);
        //         CString strParam, strParam1, strParam2, strParam3, strParam4;
        // 
        //         strParam1.Format(_T("<strDriverName>%s</strDriverName>"), strDriverName);
        //         strParam2.Format(_T("<strNetPath>%s</strNetPath>"), strNetPath);
        //         strParam3.Format(_T("<strUserName>%s</strUserName>"), strUserName);
        //         strParam4.Format(_T("<strPassWord>%s</strPassWord>"), strPassWord);
        //         strParam = strParam1 + strParam2 + strParam3 + strParam4;
        // 
        //         /************************************************************/
        //         strSend += _T("<msv_map_net_driver>");
        //         strSend += strParam;
        //         CString strChannel;
        //         strChannel.Format(_T("<nChannel>%d</nChannel>"), nChannel);
        //         strSend += strChannel;
        //         strSend += _T("</msv_map_net_driver>");
        //         /************************************************************/
        //         /* 操作   */
        // 
        //         CXMLParser parser;
        //         if (FALSE == parser.Create())
        //         {
        //             CString strError = _T("Error occur while analyzing response message of MSV[%s]");
        //             m_error_desc.Format(strError, strMsvIp);
        //             //NMTrace(m_strLogName, logLevelWarring, m_error_desc);
        //             return MSV_FAILED;
        //         }
        //         InParam pIn;
        //         OutParam pOut;
        //         CString strRecv;
        //         pIn.m_s = m_s;
        //         //	pIn.m_s = &m_s;
        //         pIn.parser = &parser;
        //         pIn.strIp = strMsvIp;
        //         pIn.strOrigin = _T("msv_map_net_driver");
        //         pIn.strSend = strSend;
        //         pOut.strRecv = strRecv;
        // 
        //         int nRet = CommandOperation(pIn, pOut);
        //         if (nRet == -2)
        //             return MSV_NETERROR;
        //         else if (nRet == -1)
        //             return MSV_XMLERROR;
        //         else if (nRet == 0)
        //             return MSV_FAILED;
        // 
        //         //m_error_desc = _T("操作成功");
        //         m_error_desc = _T("Operation succeed");
        //         return MSV_SUCCESS;
        //     }
        // 
        //     MSV_RET CClientTaskSDKImp::MSVSetAudioChannel(const CString strMsvIp, int iAudioMask)
        //     {
        //         IRemoteSDK* m_s;
        //         m_s = CreateRemoteSDK();
        //         //	CRemoteSocket m_s(m_nTimeOut);
        // 
        //         if (FALSE == SocketConnect(strMsvIp, m_s))
        //         {
        //             CString strError = _T("Error while connecting MSV[%s]");
        //             m_error_desc.Format(strError, strMsvIp);
        //             //NMTrace(m_strLogName, logLevelWarring, m_error_desc);
        //             return MSV_FAILED;
        //         }
        // 
        //         CString strSend(MXL_HEADER);
        //         CString strParam = _T("");
        //         CString tempstr;
        //         tempstr.Format(_T("<ChannelMask>%d</ChannelMask>"), iAudioMask);
        //         strParam = tempstr;
        // 
        //         strSend += _T("<set_audio_state>");
        //         strSend += strParam;
        //         strSend += _T("</set_audio_state>");//
        // 
        //         CXMLParser parser;
        //         if (FALSE == parser.Create())
        //         {
        //             CString strError = _T("Error occur while analyzing response message of MSV[%s]");
        //             m_error_desc.Format(strError, strMsvIp);
        //             //NMTrace(m_strLogName, logLevelWarring, m_error_desc);
        //             return MSV_FAILED;
        //         }
        //         InParam pIn;
        //         OutParam pOut;
        //         CString strRecv;
        //         pIn.m_s = m_s;
        //         //	pIn.m_s = &m_s;
        //         pIn.parser = &parser;
        //         pIn.strIp = strMsvIp;
        //         pIn.strOrigin = _T("set_audio_state");
        //         pIn.strSend = strSend;
        //         pOut.strRecv = strRecv;
        // 
        //         int nRet = CommandOperation(pIn, pOut);
        //         if (nRet == -2)
        //             return MSV_NETERROR;
        //         else if (nRet == -1)
        //             return MSV_XMLERROR;
        //         else if (nRet == 0)
        //             return MSV_FAILED;
        // 
        // 
        //         //m_error_desc = _T("操作成功");
        //         m_error_desc = _T("Operation succeed");
        //         return MSV_SUCCESS;
        //     }
        // 
        //     MSV_RET CClientTaskSDKImp::MSVGetAudioChannel(const CString strMsvIp, int& iAudioMask)
        //     {
        //         IRemoteSDK* m_s;
        //         m_s = CreateRemoteSDK();
        //         //	CRemoteSocket m_s(m_nTimeOut);
        // 
        //         if (FALSE == SocketConnect(strMsvIp, m_s))
        //         {
        //             CString strError = _T("Error while connecting MSV[%s]");
        //             m_error_desc.Format(strError, strMsvIp);
        //             //NMTrace(m_strLogName, logLevelWarring, m_error_desc);
        //             return MSV_FAILED;
        //         }
        // 
        //         CString strSend(MXL_HEADER);
        //         /************************************************************/
        //         strSend += _T("<query_audio_state>");
        //         strSend += _T("</query_audio_state>");//查询状态
        //                                               /************************************************************/
        // 
        //         CXMLParser parser;
        //         if (FALSE == parser.Create())
        //         {
        //             CString strError = _T("Error occur while analyzing response message of MSV[%s]");
        //             m_error_desc.Format(strError, strMsvIp);
        //             //NMTrace(m_strLogName, logLevelWarring, m_error_desc);
        //             return MSV_FAILED;
        //         }
        //         InParam pIn;
        //         OutParam pOut;
        //         CString strRecv;
        //         pIn.m_s = m_s;
        //         //	pIn.m_s = &m_s;
        //         pIn.parser = &parser;
        //         pIn.strIp = strMsvIp;
        //         pIn.strOrigin = _T("query_audio_state");
        //         pIn.strSend = strSend;
        //         pOut.strRecv = strRecv;
        // 
        //         int nRet = CommandOperation(pIn, pOut);
        //         if (nRet == -2)
        //             return MSV_NETERROR;
        //         else if (nRet == -1)
        //             return MSV_XMLERROR;
        //         else if (nRet == 0)
        //             return MSV_FAILED;
        // 
        // 
        //         CString strParam;
        //         int iTemp = 0;
        //         strParam = parser.GetNodeText(_T("ChannelMask"));
        //         iTemp = _wtoi((LPCTSTR)strParam);
        //         iAudioMask = iTemp;
        // 
        // 
        //         //    m_s.Close();
        // 
        // 
        //         //m_error_desc = _T("操作成功");
        //         m_error_desc = _T("Operation succeed");
        //         return MSV_SUCCESS;
        //     }
        //     /*
        //     MSV_RET CClientTaskSDKImp::MSVSetAudioChannel(const CString strMsvIp,CArray<AudioChannel,AudioChannel>& channelArray)
        //     {
        //     IRemoteSDK *m_s;
        //     m_s = CreateRemoteSDK();
        //         //	CRemoteSocket m_s(m_nTimeOut);
        // 
        //         if(FALSE == SocketConnect(strMsvIp,m_s))
        //         {
        //             CString strError = _T("Error while connecting MSV[%s]");
        //             m_error_desc.Format(strError, strMsvIp);
        //             //NMTrace(m_strLogName, logLevelWarring, m_error_desc);//(_T("连接媒体服务器[%s] 错误"),strMsvIp);
        //             return MSV_FAILED;
        //         }
        // 
        //         CString strSend(MXL_HEADER);
        //         CString strParam;
        //         CString tempstr;
        //         tempstr.Format(_T("<Achannelcount>%d</Achannelcount>"),channelArray.GetSize());
        //         strParam+=tempstr;
        // 
        //     //	int itemcount=0;
        //     //	int index=0;
        //     //	for(int i=0;i<channelArray.GetSize();i++)
        //     //	{
        //     //		itemcount=0;
        //     //		index=0;
        //     //		for(int j=0;j<MAX_AUDIO_COUNT;j++)
        //     //		{
        //     //			itemcount+=channelArray.GetAt(i).audioMember[j];
        //     //		}
        //     //		tempstr.Format(_T("<Achannel%d>%d</Achannel%d>"),i,channelArray.GetAt(i).AudioIdx,i);
        //     //		strParam+=tempstr;
        //     //		tempstr.Format(_T("<Achannelcount%d>%d</Achannelcount%d>"),i,itemcount,i);
        //     //		strParam+=tempstr;
        //     //		if(itemcount>0)
        //     //		{
        //     //			for(int j=0;j<MAX_AUDIO_COUNT;j++)
        //     //			{
        //     //				if(channelArray.GetAt(i).audioMember[j])
        //     //				{
        //     //					tempstr.Format(_T("<Achannel_%d_%d>%d</Achannel_%d_%d>"),i,index,j,i,index);
        //     //					strParam+=tempstr;
        //     //					index++;
        //     //				}
        //     //			}
        //     //		}	
        //     //	}
        // 
        //         for(int i=0;i<channelArray.GetSize();i++)
        //         {
        //             tempstr.Format(_T("<Achannel%d>%d</Achannel%d>"),i,channelArray.GetAt(i).AudioIdx,i);
        //             strParam+=tempstr;
        //             DWORD tempdata=0;
        //             for(int j=MAX_AUDIO_COUNT-1;j>-1;j--)
        //             {
        //                 tempdata<<=1;
        //                 tempdata+=channelArray.GetAt(i).audioMember[j];
        //             }
        //             tempstr.Format(_T("<Achannelcount%d>%d</Achannelcount%d>"),i,tempdata,i);
        //             strParam+=tempstr;
        //         }
        // 
        //         strSend += _T("<set_audio_state>");
        //         strSend+=strParam;
        //         strSend += _T("</set_audio_state>");//查询状态
        // 
        // 
        //         CXMLParser parser;
        //         if(FALSE == parser.Create())
        //         {
        //             CString strError = _T("Error occur while analyzing response message of MSV[%s]");
        //             m_error_desc.Format(strError, strMsvIp);
        //             //NMTrace(m_strLogName, logLevelWarring, m_error_desc);//(_T("(解析媒体服务器[%s] 回应消息错误)"),strMsvIp);
        //             return MSV_FAILED;
        //         }
        //         InParam pIn;
        //         OutParam pOut;
        //         CString  strRecv;
        //         pIn.m_s = m_s;
        //         //	pIn.m_s = &m_s;
        //         pIn.parser = &parser;
        //         pIn.strIp = strMsvIp;
        //         pIn.strOrigin = _T("set_audio_state");
        //         pIn.strSend   = strSend;
        //         pOut.strRecv =  strRecv;
        // 
        //         int nRet = CommandOperation(pIn,pOut);
        //         if(nRet == -2)
        //             return MSV_NETERROR;
        //         else if(nRet == -1)
        //             return MSV_XMLERROR;
        //         else if(nRet == 0)
        //             return MSV_FAILED;
        // 
        // 
        //         //m_error_desc = _T("操作成功");
        //         m_error_desc = _T("Operation succeed");
        //         return MSV_SUCCESS;
        //     }
        //     MSV_RET CClientTaskSDKImp::MSVGetAudioChannel(const CString strMsvIp,CArray<AudioChannel,AudioChannel>& channelArray)
        //     {
        //     IRemoteSDK *m_s;
        //     m_s = CreateRemoteSDK();
        //         //	CRemoteSocket m_s(m_nTimeOut);
        // 
        //         if(FALSE == SocketConnect(strMsvIp,m_s))
        //         {
        //             CString strError = _T("Error while connecting MSV[%s]");
        //             m_error_desc.Format(strError, strMsvIp);
        //             //NMTrace(m_strLogName, logLevelWarring, m_error_desc);//(_T("连接媒体服务器[%s] 错误"),strMsvIp);
        //             return MSV_FAILED;
        //         }
        // 
        //         CString strSend(MXL_HEADER);
        //         strSend += _T("<query_audio_state>");
        //         strSend += _T("</query_audio_state>");//查询状态
        // 
        //         CXMLParser parser;
        //         if(FALSE == parser.Create())
        //         {
        //             CString strError = _T("Error occur while analyzing response message of MSV[%s]");
        //             m_error_desc.Format(strError, strMsvIp);
        //             //NMTrace(m_strLogName, logLevelWarring, m_error_desc);//(_T("(解析媒体服务器[%s] 回应消息错误)"),strMsvIp);
        //             return MSV_FAILED;
        //         }
        //         InParam pIn;
        //         OutParam pOut;
        //         CString  strRecv;
        //         pIn.m_s = m_s;
        //         //	pIn.m_s = &m_s;
        //         pIn.parser = &parser;
        //         pIn.strIp = strMsvIp;
        //         pIn.strOrigin = _T("query_audio_state");
        //         pIn.strSend   = strSend;
        //         pOut.strRecv =  strRecv;
        // 
        //         int nRet = CommandOperation(pIn,pOut);
        //         if(nRet == -2)
        //             return MSV_NETERROR;
        //         else if(nRet == -1)
        //             return MSV_XMLERROR;
        //         else if(nRet == 0)
        //             return MSV_FAILED;
        // 
        // 
        //         CString strParam;
        // 
        //         channelArray.RemoveAll();
        //         DWORD dwTemp=0;
        //         strParam = parser.GetNodeText(_T("Achannelcount"));
        //         dwTemp=_wtoi((LPCTSTR)strParam);
        // 
        //     //	int itemcount=0;
        //     //	int index=0;
        //     //	CString keystring;
        //     //	for(int i=0;i<dwTemp;i++)
        //     //	{
        //     //		AudioChannel auChannel;
        //     //		keystring.Format(_T("Achannel%d"),i);
        //     //		strParam = parser.GetNodeText(keystring);
        //     //		auChannel.AudioIdx=_wtoi((LPCTSTR)strParam);
        //     //		keystring.Format(_T("Achannelcount%d"),i);
        //     //		strParam = parser.GetNodeText(keystring);
        //     //		itemcount=_wtoi((LPCTSTR)strParam);
        //     //		int index=0;
        //     //		for(int j=0;j<itemcount;j++)
        //     //		{
        //     //			keystring.Format(_T("Achannel_%d_%d"),i,j);
        //     //			strParam = parser.GetNodeText(keystring);
        //     //			index=_wtoi((LPCTSTR)strParam);
        //     //			if(index>-1&&index<MAX_AUDIO_COUNT)
        //     //			{
        //     //				auChannel.audioMember[index]=TRUE;
        //     //			}
        //     //			
        //     //		}
        //     //		channelArray.Add(auChannel);
        //     //	}
        // 
        //         CString keystring;
        //         for(int i=0;i<dwTemp;i++)
        //         {
        //             AudioChannel auChannel;
        //             DWORD dwTempdata=0;
        //             keystring.Format(_T("Achannel%d"),i);
        //             strParam = parser.GetNodeText(keystring);
        //             auChannel.AudioIdx=_wtoi((LPCTSTR)strParam);
        // 
        //             keystring.Format(_T("Achannelcount%d"),i);
        //             strParam = parser.GetNodeText(keystring);
        //             dwTempdata=_wtoi((LPCTSTR)strParam);
        // 
        //             for(int j=0;j<MAX_AUDIO_COUNT;j++)
        //             {
        //                 auChannel.audioMember[j]=dwTempdata%2;
        //                 dwTempdata>>=1;
        //             }
        //             channelArray.Add(auChannel);
        //         }
        //         m_s.Close();
        //         //m_error_desc = _T("操作成功");
        //         m_error_desc = _T("Operation succeed");
        //         return MSV_SUCCESS;
        //     }
        //     /**/
        // 
        //     MSV_RET CClientTaskSDKImp::MSVGetNetPreview(const CString strMsvIp, BOOL& bPreview, int channel)
        //     {
        //         IRemoteSDK* m_s;
        //         m_s = CreateRemoteSDK();
        //         //	CRemoteSocket m_s(m_nTimeOut);
        // 
        //         if (FALSE == SocketConnect(strMsvIp, m_s))
        //         {
        //             CString strError = _T("Error while connecting MSV[%s]");
        //             m_error_desc.Format(strError, strMsvIp);
        //             //NMTrace(m_strLogName, logLevelWarring, m_error_desc);
        //             return MSV_FAILED;
        //         }
        // 
        //         CString strSend(MXL_HEADER);
        //         /************************************************************/
        //         strSend += _T("<get_netpreview>");
        //         strSend += _T("</get_netpreview>");//查询状态
        //                                            /************************************************************/
        // 
        //         CXMLParser parser;
        //         if (FALSE == parser.Create())
        //         {
        //             CString strError = _T("Error occur while analyzing response message of MSV[%s]");
        //             m_error_desc.Format(strError, strMsvIp);
        //             //NMTrace(m_strLogName, logLevelWarring, m_error_desc);
        //             return MSV_FAILED;
        //         }
        //         InParam pIn;
        //         OutParam pOut;
        //         CString strRecv;
        //         pIn.m_s = m_s;
        //         //	pIn.m_s = &m_s;
        //         pIn.parser = &parser;
        //         pIn.strIp = strMsvIp;
        //         pIn.strOrigin = _T("get_netpreview");
        //         pIn.strSend = strSend;
        //         pOut.strRecv = strRecv;
        // 
        //         int nRet = CommandOperation(pIn, pOut);
        //         if (nRet == -2)
        //             return MSV_NETERROR;
        //         else if (nRet == -1)
        //             return MSV_XMLERROR;
        //         else if (nRet == 0)
        //             return MSV_FAILED;
        // 
        // 
        //         CString strParam;
        // 
        //         strParam = parser.GetNodeText(_T("netpreview"));
        //         bPreview = (BOOL)(_wtoi((LPCTSTR)strParam));
        // 
        //         //    m_s.Close();
        // 
        //         //m_error_desc = _T("操作成功");
        //         m_error_desc = _T("Operation succeed");
        //         return MSV_SUCCESS;
        //     }
        // 
        //     /************************************************************************/
        //     /* 设置GOP结构                                                          */
        //     /************************************************************************/
        //     MSV_RET CClientTaskSDKImp::MSVSetGopStruct(const CString strMsvIP, int nPcount, int nBcontinuecount)
        //     {
        //         IRemoteSDK* m_s;
        //         m_s = CreateRemoteSDK();
        //         //	CRemoteSocket m_s(m_nTimeOut);
        // 
        //         if (FALSE == SocketConnect(strMsvIP, m_s))
        //         {
        //             CString strError = _T("Error while connecting MSV[%s]");
        //             m_error_desc.Format(strError, strMsvIP);
        //             //NMTrace(m_strLogName, logLevelWarring, m_error_desc);
        //             return MSV_FAILED;
        //         }
        // 
        //         CString strSend(MXL_HEADER);
        //         CString strParam;
        //         strParam.Format(_T("<nPcount>%d</nPcount><nBcontinuecount>%d</nBcontinuecount>"), nPcount, nBcontinuecount);
        //         /************************************************************/
        //         strSend += _T("<set_gopstruct>");
        //         strSend += strParam;
        //         strSend += _T("</set_gopstruct>");//查询状态
        //                                           /************************************************************/
        // 
        //         CXMLParser parser;
        //         if (FALSE == parser.Create())
        //         {
        //             CString strError = _T("Error occur while analyzing response message of MSV[%s]");
        //             m_error_desc.Format(strError, strMsvIP);
        //             //NMTrace(m_strLogName, logLevelWarring, m_error_desc);
        //             return MSV_FAILED;
        //         }
        //         InParam pIn;
        //         OutParam pOut;
        //         CString strRecv;
        //         pIn.m_s = m_s;
        //         //	pIn.m_s = &m_s;
        //         pIn.parser = &parser;
        //         pIn.strIp = strMsvIP;
        //         pIn.strOrigin = _T("set_gopstruct");
        //         pIn.strSend = strSend;
        //         pOut.strRecv = strRecv;
        // 
        //         int nRet = CommandOperation(pIn, pOut);
        //         if (nRet == -2)
        //             return MSV_NETERROR;
        //         else if (nRet == -1)
        //             return MSV_XMLERROR;
        //         else if (nRet == 0)
        //             return MSV_FAILED;
        // 
        // 
        //         //m_error_desc = _T("操作成功");
        //         m_error_desc = _T("Operation succeed");
        //         return MSV_SUCCESS;
        //     }
        //     MSV_RET CClientTaskSDKImp::MSVGetGopStruct(const CString strMsvIP, int& nPcount, int& nBcontinuecount)
        //     {
        //         IRemoteSDK* m_s;
        //         m_s = CreateRemoteSDK();
        //         //	CRemoteSocket m_s(m_nTimeOut);
        // 
        //         if (FALSE == SocketConnect(strMsvIP, m_s))
        //         {
        //             CString strError = _T("Error while connecting MSV[%s]");
        //             m_error_desc.Format(strError, strMsvIP);
        //             //NMTrace(m_strLogName, logLevelWarring, m_error_desc);
        //             return MSV_FAILED;
        //         }
        // 
        //         CString strSend(MXL_HEADER);
        //         /************************************************************/
        //         strSend += _T("<get_gopstruct>");
        //         strSend += _T("</get_gopstruct>");//查询状态
        //                                           /************************************************************/
        // 
        //         CXMLParser parser;
        //         if (FALSE == parser.Create())
        //         {
        //             CString strError = _T("Error occur while analyzing response message of MSV[%s]");
        //             m_error_desc.Format(strError, strMsvIP);
        //             //NMTrace(m_strLogName, logLevelWarring, m_error_desc);
        //             return MSV_FAILED;
        //         }
        //         InParam pIn;
        //         OutParam pOut;
        //         CString strRecv;
        //         pIn.m_s = m_s;
        //         //	pIn.m_s = &m_s;
        //         pIn.parser = &parser;
        //         pIn.strIp = strMsvIP;
        //         pIn.strOrigin = _T("get_gopstruct");
        //         pIn.strSend = strSend;
        //         pOut.strRecv = strRecv;
        // 
        //         int nRet = CommandOperation(pIn, pOut);
        // 
        //         //	m_s.Close();
        // 
        //         if (nRet == -2)
        //             return MSV_NETERROR;
        //         else if (nRet == -1)
        //             return MSV_XMLERROR;
        //         else if (nRet == 0)
        //             return MSV_FAILED;
        // 
        // 
        //         CString strParam;
        //         strParam = parser.GetNodeText(_T("nPcount"));
        //         nPcount = (int)(_wtoi((LPCTSTR)strParam));
        //         strParam = parser.GetNodeText(_T("nBcontinuecount"));
        //         nBcontinuecount = (int)(_wtoi((LPCTSTR)strParam));
        // 
        //         //m_error_desc = _T("操作成功");
        //         m_error_desc = _T("Operation succeed");
        //         return MSV_SUCCESS;
        //     }
        //     /************************************************************************/
        //     /* 设置迁移模式                                                          */
        //     /************************************************************************/
        //     MSV_RET CClientTaskSDKImp::MSVSetMoveFileMode(const CString strMsvIP, BOOL moveflag)
        //     {
        //         IRemoteSDK* m_s;
        //         m_s = CreateRemoteSDK();
        //         //	CRemoteSocket m_s(m_nTimeOut);
        // 
        //         if (FALSE == SocketConnect(strMsvIP, m_s))
        //         {
        //             CString strError = _T("Error while connecting MSV[%s]");
        //             m_error_desc.Format(strError, strMsvIP);
        //             //NMTrace(m_strLogName, logLevelWarring, m_error_desc);
        //             return MSV_FAILED;
        //         }
        // 
        //         CString strSend(MXL_HEADER);
        //         CString strParam;
        //         strParam.Format(_T("<moveflag>%d</moveflag>"), moveflag);
        //         /************************************************************/
        //         strSend += _T("<set_movefilemode>");
        //         strSend += strParam;
        //         strSend += _T("</set_movefilemode>");//查询状态
        //                                              /************************************************************/
        // 
        //         CXMLParser parser;
        //         if (FALSE == parser.Create())
        //         {
        //             CString strError = _T("Error occur while analyzing response message of MSV[%s]");
        //             m_error_desc.Format(strError, strMsvIP);
        //             //NMTrace(m_strLogName, logLevelWarring, m_error_desc);
        //             return MSV_FAILED;
        //         }
        //         InParam pIn;
        //         OutParam pOut;
        //         CString strRecv;
        //         pIn.m_s = m_s;
        //         //	pIn.m_s = &m_s;
        //         pIn.parser = &parser;
        //         pIn.strIp = strMsvIP;
        //         pIn.strOrigin = _T("set_movefilemode");
        //         pIn.strSend = strSend;
        //         pOut.strRecv = strRecv;
        // 
        //         int nRet = CommandOperation(pIn, pOut);
        //         if (nRet == -2)
        //             return MSV_NETERROR;
        //         else if (nRet == -1)
        //             return MSV_XMLERROR;
        //         else if (nRet == 0)
        //             return MSV_FAILED;
        // 
        // 
        //         //m_error_desc = _T("操作成功");
        //         m_error_desc = _T("Operation succeed");
        //         return MSV_SUCCESS;
        //     }
        //     MSV_RET CClientTaskSDKImp::MSVGetMoveFileMode(const CString strMsvIP, BOOL& moveflag)
        //     {
        //         IRemoteSDK* m_s;
        //         m_s = CreateRemoteSDK();
        //         //	CRemoteSocket m_s(m_nTimeOut);
        // 
        //         if (FALSE == SocketConnect(strMsvIP, m_s))
        //         {
        //             CString strError = _T("Error while connecting MSV[%s]");
        //             m_error_desc.Format(strError, strMsvIP);
        //             //NMTrace(m_strLogName, logLevelWarring, m_error_desc);
        //             return MSV_FAILED;
        //         }
        // 
        //         CString strSend(MXL_HEADER);
        //         /************************************************************/
        //         strSend += _T("<get_movefilemode>");
        //         strSend += _T("</get_movefilemode>");//查询状态
        //                                              /************************************************************/
        // 
        //         CXMLParser parser;
        //         if (FALSE == parser.Create())
        //         {
        //             CString strError = _T("Error occur while analyzing response message of MSV[%s]");
        //             m_error_desc.Format(strError, strMsvIP);
        //             //NMTrace(m_strLogName, logLevelWarring, m_error_desc);
        //             return MSV_FAILED;
        //         }
        //         InParam pIn;
        //         OutParam pOut;
        //         CString strRecv;
        //         pIn.m_s = m_s;
        //         //	pIn.m_s = &m_s;
        //         pIn.parser = &parser;
        //         pIn.strIp = strMsvIP;
        //         pIn.strOrigin = _T("get_movefilemode");
        //         pIn.strSend = strSend;
        //         pOut.strRecv = strRecv;
        // 
        //         int nRet = CommandOperation(pIn, pOut);
        // 
        //         //	m_s.Close();
        // 
        //         if (nRet == -2)
        //             return MSV_NETERROR;
        //         else if (nRet == -1)
        //             return MSV_XMLERROR;
        //         else if (nRet == 0)
        //             return MSV_FAILED;
        // 
        // 
        //         CString strParam;
        //         strParam = parser.GetNodeText(_T("moveflag"));
        //         moveflag = (BOOL)(_wtoi((LPCTSTR)strParam));
        // 
        //         //m_error_desc = _T("操作成功");
        //         m_error_desc = _T("Operation succeed");
        //         return MSV_SUCCESS;
        //     }
        //     /************************************************************************/
        //     /* 设置信号源格式结构                                                          */
        //     /************************************************************************/
        //     MSV_RET CClientTaskSDKImp::MSVSetSignalFomat(const CString strMsvIP, int width, int height, int framerate, int nChannel)
        //     {
        //         IRemoteSDK* m_s;
        //         m_s = CreateRemoteSDK();
        //         //	CRemoteSocket m_s(m_nTimeOut);
        // 
        //         if (FALSE == SocketConnect(strMsvIP, m_s))
        //         {
        //             CString strError = _T("Error while connecting MSV[%s]");
        //             m_error_desc.Format(strError, strMsvIP);
        //             //NMTrace(m_strLogName, logLevelWarring, m_error_desc);
        //             return MSV_FAILED;
        //         }
        // 
        //         CString strSend(MXL_HEADER);
        //         CString strParam;
        //         strParam.Format(_T("<width>%d</width><height>%d</height><framerate>%d</framerate>"), width, height, framerate);
        //         /************************************************************/
        //         strSend += _T("<set_signalfomat>");
        //         strSend += strParam;
        //         strSend += _T("</set_signalfomat>");//查询状态
        //                                             /************************************************************/
        // 
        //         CXMLParser parser;
        //         if (FALSE == parser.Create())
        //         {
        //             CString strError = _T("Error occur while analyzing response message of MSV[%s]");
        //             m_error_desc.Format(strError, strMsvIP);
        //             //NMTrace(m_strLogName, logLevelWarring, m_error_desc);
        //             return MSV_FAILED;
        //         }
        //         InParam pIn;
        //         OutParam pOut;
        //         CString strRecv;
        //         pIn.m_s = m_s;
        //         //	pIn.m_s = &m_s;
        //         pIn.parser = &parser;
        //         pIn.strIp = strMsvIP;
        //         pIn.strOrigin = _T("set_signalfomat");
        //         pIn.strSend = strSend;
        //         pOut.strRecv = strRecv;
        // 
        //         int nRet = CommandOperation(pIn, pOut);
        //         if (nRet == -2)
        //             return MSV_NETERROR;
        //         else if (nRet == -1)
        //             return MSV_XMLERROR;
        //         else if (nRet == 0)
        //             return MSV_FAILED;
        // 
        // 
        //         //m_error_desc = _T("操作成功");
        //         m_error_desc = _T("Operation succeed");
        //         return MSV_SUCCESS;
        //     }
        //     MSV_RET CClientTaskSDKImp::MSVGetSignalFomat(const CString strMsvIP, CArray<SignalSourceFormat, SignalSourceFormat>& arrayformat, int nChannel)
        //     {
        //         IRemoteSDK* m_s;
        //         m_s = CreateRemoteSDK();
        //         //	CRemoteSocket m_s(m_nTimeOut);
        // 
        //         if (FALSE == SocketConnect(strMsvIP, m_s))
        //         {
        //             CString strError = _T("Error while connecting MSV[%s]");
        //             m_error_desc.Format(strError, strMsvIP);
        //             //NMTrace(m_strLogName, logLevelWarring, m_error_desc);
        //             return MSV_FAILED;
        //         }
        // 
        //         CString strSend(MXL_HEADER);
        //         /************************************************************/
        //         strSend += _T("<get_signalfomat>");
        //         strSend += _T("</get_signalfomat>");//查询状态
        //                                             /************************************************************/
        // 
        //         CXMLParser parser;
        //         if (FALSE == parser.Create())
        //         {
        //             CString strError = _T("Error occur while analyzing response message of MSV[%s]");
        //             m_error_desc.Format(strError, strMsvIP);
        //             //NMTrace(m_strLogName, logLevelWarring, m_error_desc);
        //             return MSV_FAILED;
        //         }
        //         InParam pIn;
        //         OutParam pOut;
        //         CString strRecv;
        //         pIn.m_s = m_s;
        //         //	pIn.m_s = &m_s;
        //         pIn.parser = &parser;
        //         pIn.strIp = strMsvIP;
        //         pIn.strOrigin = _T("get_signalfomat");
        //         pIn.strSend = strSend;
        //         pOut.strRecv = strRecv;
        // 
        //         int nRet = CommandOperation(pIn, pOut);
        // 
        //         //	m_s.Close();
        // 
        //         if (nRet == -2)
        //             return MSV_NETERROR;
        //         else if (nRet == -1)
        //             return MSV_XMLERROR;
        //         else if (nRet == 0)
        //             return MSV_FAILED;
        // 
        // 
        //         CString strParam;
        //         std::vector<CString> vXML;
        //         arrayformat.RemoveAll();
        //         parser.GetNodesXML(_T("format"), vXML);
        //         //	strParam = parser.GetNodeText(_T("moveflag"));
        //         //	moveflag = (BOOL)(_wtoi((LPCTSTR)strParam));
        //         CXMLParser parserForamt;
        //         if (FALSE == parserForamt.Create())
        //         {
        //             CString strError = _T("Error occur while analyzing response message of MSV[%s]");
        //             m_error_desc.Format(strError, strMsvIP);
        //             //NMTrace(m_strLogName, logLevelWarring, m_error_desc);
        //             return MSV_FAILED;
        //         }
        //         CString temp;
        //         for (int i = 0; i < vXML.size(); i++)
        //         {
        //             temp.Format(_T("%s%s"), MXL_HEADER, vXML.at(i));
        //             if (FALSE == parserForamt.Parse(temp))
        //             {
        //                 CString strError = _T("Error occur while analyzing response message of MSV[%s]");
        //                 m_error_desc.Format(strError, strMsvIP);
        //                 //NMTrace(m_strLogName, logLevelWarring, m_error_desc);			
        //                 return MSV_FAILED;
        //             }
        //             SignalSourceFormat foramt;
        //             strParam = parserForamt.GetNodeText(_T("width"));
        //             foramt.width = (int)(_wtoi((LPCTSTR)strParam));
        //             strParam = parserForamt.GetNodeText(_T("height"));
        //             foramt.height = (int)(_wtoi((LPCTSTR)strParam));
        //             strParam = parserForamt.GetNodeText(_T("framerate"));
        //             foramt.framerate = (int)(_wtoi((LPCTSTR)strParam));
        //             arrayformat.Add(foramt);
        //         }
        //         //m_error_desc = _T("操作成功");
        //         m_error_desc = _T("Operation succeed");
        //         return MSV_SUCCESS;
        //     }
        // 
        //     MSV_RET CClientTaskSDKImp::ASIChangeProgram(const CString strMsvIP, int nChannel, int nProgramID)
        //     {
        //         IRemoteSDK* m_s;
        //         m_s = CreateRemoteSDK();
        //         //	CRemoteSocket m_s(m_nTimeOut);
        // 
        //         if (FALSE == SocketConnect(strMsvIP, m_s))
        //         {
        //             CString strError = _T("Error while connecting MSV[%s]");
        //             m_error_desc.Format(strError, strMsvIP);
        //             //NMTrace(m_strLogName, logLevelWarring, m_error_desc);
        //             return MSV_FAILED;
        //         }
        // 
        //         CString strSend(MXL_HEADER);
        //         /************************************************************/
        //         strSend += _T("<change_program>");
        //         CString strParam;
        //         strParam.Format(_T("<program_id>%d</program_id>"), nProgramID);
        //         strSend += strParam;
        //         CString strChannel;
        //         strChannel.Format(_T("<nChannel>%d</nChannel>"), nChannel);
        //         strSend += strChannel;
        //         strSend += _T("</change_program>");//查询状态
        //                                            /************************************************************/
        // 
        //         CXMLParser parser;
        //         if (FALSE == parser.Create())
        //         {
        //             CString strError = _T("Error occur while analyzing response message of MSV[%s]");
        //             m_error_desc.Format(strError, strMsvIP);
        //             //NMTrace(m_strLogName, logLevelWarring, m_error_desc);
        //             return MSV_FAILED;
        //         }
        //         InParam pIn;
        //         OutParam pOut;
        //         CString strRecv;
        //         pIn.m_s = m_s;
        //         //	pIn.m_s = &m_s;
        //         pIn.parser = &parser;
        //         pIn.strIp = strMsvIP;
        //         pIn.strOrigin = _T("change_program");
        //         pIn.strSend = strSend;
        //         pOut.strRecv = strRecv;
        // 
        //         int nRet = CommandOperation(pIn, pOut);
        //         if (nRet == -2)
        //             return MSV_NETERROR;
        //         else if (nRet == -1)
        //             return MSV_XMLERROR;
        //         else if (nRet == 0)
        //             return MSV_FAILED;
        // 
        // 
        //         //m_error_desc = _T("操作成功");
        //         m_error_desc = _T("Operation succeed");
        //         return MSV_SUCCESS;
        //     }
        //     MSV_RET CClientTaskSDKImp::MSVQueryAllState(const CString strMsvIP, MSV_STATE& state, BATCH_STATE& bachstate,
        //             disk_info &infoA, disk_info &infoB, CString& curtaskname, CString& LastLog)
        //     {
        //         IRemoteSDK* m_s;
        //         m_s = CreateRemoteSDK();
        //         //	CRemoteSocket m_s(m_nTimeOut);
        // 
        //         if (FALSE == SocketConnect(strMsvIP, m_s))
        //         {
        //             CString strError = _T("Error while connecting MSV[%s]");
        //             m_error_desc.Format(strError, strMsvIP);
        //             //NMTrace(m_strLogName, logLevelWarring, m_error_desc);
        //             return MSV_FAILED;
        //         }
        // 
        //         CString strSend(MXL_HEADER);
        //         /************************************************************/
        //         strSend += _T("<query_allstate>");
        //         strSend += _T("</query_allstate>");//查询状态
        //                                            /************************************************************/
        // 
        //         CXMLParser parser;
        //         if (FALSE == parser.Create())
        //         {
        //             CString strError = _T("Error occur while analyzing response message of MSV[%s]");
        //             m_error_desc.Format(strError, strMsvIP);
        //             //NMTrace(m_strLogName, logLevelWarring, m_error_desc);
        //             return MSV_FAILED;
        //         }
        //         InParam pIn;
        //         OutParam pOut;
        //         CString strRecv;
        //         pIn.m_s = m_s;
        //         //	pIn.m_s = &m_s;
        //         pIn.parser = &parser;
        //         pIn.strIp = strMsvIP;
        //         pIn.strOrigin = _T("query_allstate");
        //         pIn.strSend = strSend;
        //         pOut.strRecv = strRecv;
        // 
        //         int nRet = CommandOperation(pIn, pOut);
        // 
        //         //	m_s.Close();
        // 
        //         if (nRet == -2)
        //             return MSV_NETERROR;
        //         else if (nRet == -1)
        //             return MSV_XMLERROR;
        //         else if (nRet == 0)
        //             return MSV_FAILED;
        // 
        // 
        //         CString strParam;
        //         ///////////////////////////
        //         //MSV启动模式
        //         strParam = parser.GetNodeText(_T("msv_mode"));
        //         state.msv_mode = (MSV_MODE)(_wtoi((LPCTSTR)strParam));
        //         //MSV工作模式
        //         strParam = parser.GetNodeText(_T("msv_work_mode"));
        //         state.msv_work_mode = (WORK_MODE)(_wtoi((LPCTSTR)strParam));
        //         //MSV运行状态
        //         strParam = parser.GetNodeText(_T("msv_capture_state"));
        //         state.msv_capture_state = (CAPTURE_STATE)(_wtoi((LPCTSTR)strParam));
        //         //如果媒体服务器处于远程控制模式，将返回控方客户端IP
        //         strParam = parser.GetNodeText(_T("msv_client_ip"));
        //         state.msv_client_ip = strParam;
        // 
        //         strParam = parser.GetNodeText(_T("s_state"));
        //         bachstate = (BATCH_STATE)(_wtoi((LPCTSTR)strParam));
        // 
        //         TCHAR* restring = NULL;
        //         infoA.diskName = parser.GetNodeText(_T("diskA"));
        //         infoA.nTotolSize = (float)wcstod(parser.GetNodeText(_T("totalA")), &restring);
        //         infoA.nFreeSize = (float)wcstod(parser.GetNodeText(_T("freeA")), &restring);
        //         infoB.diskName = parser.GetNodeText(_T("diskB"));
        //         infoB.nTotolSize = (float)wcstod(parser.GetNodeText(_T("totalB")), &restring);
        //         infoB.nFreeSize = (float)wcstod(parser.GetNodeText(_T("freeB")), &restring);
        // 
        //         curtaskname = parser.GetNodeText(_T("taskname"));
        // 
        //         LastLog = parser.GetNodeText(_T("lastlog"));
        //         /////////////////////////////////////
        //         //m_error_desc = _T("操作成功");
        //         m_error_desc = _T("Operation succeed");
        //         return MSV_SUCCESS;
        //     }
        // 
        //     MSV_RET CClientTaskSDKImp::MSVEnumAudioChannel(const CString strMsvIP, CArray<int, int>& dataArray)
        //     {
        //         IRemoteSDK* m_s;
        //         m_s = CreateRemoteSDK();
        //         //	CRemoteSocket m_s(m_nTimeOut);
        // 
        //         if (FALSE == SocketConnect(strMsvIP, m_s))
        //         {
        //             CString strError = _T("Error while connecting MSV[%s]");
        //             m_error_desc.Format(strError, strMsvIP);
        //             //NMTrace(m_strLogName, logLevelWarring, m_error_desc);
        //             return MSV_FAILED;
        //         }
        // 
        //         CString strSend(MXL_HEADER);
        //         /************************************************************/
        //         strSend += _T("<enum_audio>");
        //         strSend += _T("</enum_audio>");//查询状态
        //                                        /************************************************************/
        // 
        //         CXMLParser parser;
        //         if (FALSE == parser.Create())
        //         {
        //             CString strError = _T("Error occur while analyzing response message of MSV[%s]");
        //             m_error_desc.Format(strError, strMsvIP);
        //             //NMTrace(m_strLogName, logLevelWarring, m_error_desc);
        //             return MSV_FAILED;
        //         }
        //         InParam pIn;
        //         OutParam pOut;
        //         CString strRecv;
        //         pIn.m_s = m_s;
        //         //	pIn.m_s = &m_s;
        //         pIn.parser = &parser;
        //         pIn.strIp = strMsvIP;
        //         pIn.strOrigin = _T("enum_audio");
        //         pIn.strSend = strSend;
        //         pOut.strRecv = strRecv;
        // 
        //         int nRet = CommandOperation(pIn, pOut);
        // 
        //         //	m_s.Close();
        // 
        //         if (nRet == -2)
        //             return MSV_NETERROR;
        //         else if (nRet == -1)
        //             return MSV_XMLERROR;
        //         else if (nRet == 0)
        //             return MSV_FAILED;
        // 
        // 
        //         CString strParam;
        //         ///////////////////////////
        //         //MSV启动模式
        //         strParam = parser.GetNodeText(_T("audiousedata"));
        //         int datatemp = _wtoi((LPCTSTR)strParam);
        //         dataArray.RemoveAll();
        //         for (int i = 0; i < MAX_AUDIO_COUNT; i++)
        //         {
        //             if (datatemp % 2)
        //             {
        //                 dataArray.Add(i);
        //             }
        //             datatemp >>= 1;
        //             if (datatemp <= 0)
        //             {
        //                 break;
        //             }
        //         }
        // 
        //         //m_error_desc = _T("操作成功");
        //         m_error_desc = _T("Operation succeed");
        //         return MSV_SUCCESS;
        //     }
        // 

        //     MSV_RET CClientTaskSDKImp::MSVStartRetrospectTask(const CString strMsvIp,
        //     		               const TASK_ALL_PARAM param,
        //                            int nChannel)
        //     {
        //         CString strLog;
        //         strLog.Format(_T("##MSVStartRetrospectTask(ID:%d,Name:%s) from %s"), param.taskParam.ulID, param.taskParam.strName, strMsvIp);
        //         NMTrace0(SDKLOGNAME, logLevelWarring, (_bstr_t)strLog);
        //         //分段时间太长
        //         if (/*param.nCutLen > 60 || */param.nCutLen <= 0)
        //         {
        //             m_error_desc = _T("Segment time of task should be greater than 0 minutes");
        //             strLog.Format(_T("MSVStartRetrospectTask(ID:%d,Name:%s) MSV_FAILED.%s"), param.taskParam.ulID, param.taskParam.strName, m_error_desc);
        //             NMTrace0(SDKLOGNAME, logLevelWarring, (_bstr_t)strLog);
        //             return MSV_FAILED;
        //         }
        //         IRemoteSDK* m_s;
        //         m_s = CreateRemoteSDK();
        //         //	CRemoteSocket m_s(m_nTimeOut);
        // 
        //         if (FALSE == SocketConnect(strMsvIp, m_s))
        //         {
        //             CString strError = _T("Error while connecting MSV[%s]");
        //             m_error_desc.Format(strError, strMsvIp);
        //             strLog.Format(_T("MSVStartRetrospectTask(ID:%d,Name:%s) MSV_FAILED.%s"), param.taskParam.ulID, param.taskParam.strName, m_error_desc);
        //             NMTrace0(SDKLOGNAME, logLevelWarring, (_bstr_t)strLog);
        //             return MSV_FAILED;
        //         }
        //         /*格式化参数到XML文本*/
        //         CString strSend;
        // 
        //         strSend = FromatParmaToString(param, _T("start_msv_retrospecttask"), nChannel);
        // 
        //         /* 操作   */
        // 
        //         CXMLParser parser;
        //         if (FALSE == parser.Create())
        //         {
        //             CString strError = _T("Error occur while analyzing response message of MSV[%s]");
        //             m_error_desc.Format(strError, strMsvIp);
        //             strLog.Format(_T("MSVStartRetrospectTask(ID:%d,Name:%s) MSV_FAILED.%s"), param.taskParam.ulID, param.taskParam.strName, m_error_desc);
        //             NMTrace0(SDKLOGNAME, logLevelWarring, (_bstr_t)strLog);
        //             return MSV_FAILED;
        //         }
        //         InParam pIn;
        //         OutParam pOut;
        //         CString strRecv;
        //         pIn.m_s = m_s;
        //         //	pIn.m_s = &m_s;
        //         pIn.parser = &parser;
        //         pIn.strIp = strMsvIp;
        //         pIn.strOrigin = _T("start_msv_retrospecttask");
        //         pIn.strSend = strSend;
        //         pOut.strRecv = strRecv;
        // 
        //         int nRet = CommandOperation(pIn, pOut);
        //         if (nRet == -2)
        //             return MSV_NETERROR;
        //         else if (nRet == -1)
        //             return MSV_XMLERROR;
        //         else if (nRet == 0)
        //             return MSV_FAILED;
        // 
        //         //m_error_desc = _T("操作成功");
        //         m_error_desc = _T("Operation succeed");
        //         strLog.Format(_T("##MSVStartRetrospectTask(ID:%d,Name:%s) MSV_SUCCESS."), param.taskParam.ulID, param.taskParam.strName);
        //         NMTrace0(SDKLOGNAME, logLevelWarring, (_bstr_t)strLog);
        //         return MSV_SUCCESS;
        //     }
        //     MSV_RET CClientTaskSDKImp::MSVRunRetrospectTask(const CString strMsvIP,
        //                            int taskID, int retrospectcount, int taskpluslength,
        //                            int nChannel)
        //     {
        //         CString strLog;
        //         strLog.Format(_T("##MSVRunRetrospectTask(ID:%d) from %s,retrospectcount=%d,taskpluslength=%d"), taskID, strMsvIP, retrospectcount, taskpluslength);
        //         NMTrace0(SDKLOGNAME, logLevelWarring, (_bstr_t)strLog);
        //         //	CRemoteSocket m_s(m_nTimeOut);
        // 
        //         IRemoteSDK* m_s;
        //         m_s = CreateRemoteSDK();
        //         if (FALSE == SocketConnect(strMsvIP, m_s))
        //         {
        //             CString strError = _T("Error while connecting MSV[%s]");
        //             m_error_desc.Format(strError, strMsvIP);
        //             strLog.Format(_T("MSVRunRetrospectTask(ID:%d) MSV_FAILED.%s"), taskID, m_error_desc);
        //             NMTrace0(SDKLOGNAME, logLevelWarring, (_bstr_t)strLog);
        //             return MSV_FAILED;
        //         }
        // 
        //         CString strSend(MXL_HEADER);
        //         CString strParam;
        //         strParam.Format(_T("<taskid>%d</taskid><retrospectcount>%d</retrospectcount><pluslength>%d</pluslength>"), taskID, retrospectcount, taskpluslength);
        //         /************************************************************/
        //         strSend += _T("<run_retrospectTask>");
        //         strSend += strParam;
        //         strSend += _T("</run_retrospectTask>");//查询状态
        //                                                /************************************************************/
        // 
        //         CXMLParser parser;
        //         if (FALSE == parser.Create())
        //         {
        //             CString strError = _T("Error occur while analyzing response message of MSV[%s]");
        //             m_error_desc.Format(strError, strMsvIP);
        //             strLog.Format(_T("MSVRunRetrospectTask(ID:%d) MSV_FAILED.%s"), taskID, m_error_desc);
        //             NMTrace0(SDKLOGNAME, logLevelWarring, (_bstr_t)strLog);
        //             return MSV_FAILED;
        //         }
        //         InParam pIn;
        //         OutParam pOut;
        //         CString strRecv;
        //         pIn.m_s = m_s;
        //         //	pIn.m_s = &m_s;
        //         pIn.parser = &parser;
        //         pIn.strIp = strMsvIP;
        //         pIn.strOrigin = _T("run_retrospectTask");
        //         pIn.strSend = strSend;
        //         pOut.strRecv = strRecv;
        // 
        //         int nRet = CommandOperation(pIn, pOut);
        //         if (nRet == -2)
        //             return MSV_NETERROR;
        //         else if (nRet == -1)
        //             return MSV_XMLERROR;
        //         else if (nRet == 0)
        //             return MSV_FAILED;
        // 
        // 
        //         //m_error_desc = _T("操作成功");
        //         m_error_desc = _T("Operation succeed");
        //         strLog.Format(_T("##MSVRunRetrospectTask(ID:%d) MSV_SUCCESS."), taskID);
        //         NMTrace0(SDKLOGNAME, logLevelWarring, (_bstr_t)strLog);
        //         return MSV_SUCCESS;
        //     }
        //     MSV_RET CClientTaskSDKImp::MSVSetManuMode(const CString strMsvIP, BOOL bRigor)
        //     {
        //         IRemoteSDK* m_s;
        //         m_s = CreateRemoteSDK();
        //         //	CRemoteSocket m_s(m_nTimeOut);
        // 
        //         if (FALSE == SocketConnect(strMsvIP, m_s))
        //         {
        //             CString strError = _T("Error while connecting MSV[%s]");
        //             m_error_desc.Format(strError, strMsvIP);
        //             //NMTrace(m_strLogName, logLevelWarring, m_error_desc);
        //             return MSV_FAILED;
        //         }
        // 
        //         CString strSend(MXL_HEADER);
        //         CString strParam;
        //         strParam.Format(_T("<mode>%d</mode>"), bRigor);
        //         /************************************************************/
        //         strSend += _T("<switch_manuMode>");
        //         strSend += strParam;
        //         strSend += _T("</switch_manuMode>");//查询状态
        //                                             /************************************************************/
        // 
        //         CXMLParser parser;
        //         if (FALSE == parser.Create())
        //         {
        //             CString strError = _T("Error occur while analyzing response message of MSV[%s]");
        //             m_error_desc.Format(strError, strMsvIP);
        //             //NMTrace(m_strLogName, logLevelWarring, m_error_desc);
        //             return MSV_FAILED;
        //         }
        //         InParam pIn;
        //         OutParam pOut;
        //         CString strRecv;
        //         pIn.m_s = m_s;
        //         //	pIn.m_s = &m_s;
        //         pIn.parser = &parser;
        //         pIn.strIp = strMsvIP;
        //         pIn.strOrigin = _T("switch_manuMode");
        //         pIn.strSend = strSend;
        //         pOut.strRecv = strRecv;
        // 
        //         int nRet = CommandOperation(pIn, pOut);
        //         if (nRet == -2)
        //             return MSV_NETERROR;
        //         else if (nRet == -1)
        //             return MSV_XMLERROR;
        //         else if (nRet == 0)
        //             return MSV_FAILED;
        // 
        //         //m_error_desc = _T("操作成功");
        //         m_error_desc = _T("Operation succeed");
        //         return MSV_SUCCESS;
        //     }
        // 
        //     MSV_RET CClientTaskSDKImp::MSVGetVtrTimeCode(CString strMsvIP, int& lTimeCode)
        //     {
        //         IRemoteSDK* m_s;
        //         m_s = CreateRemoteSDK();
        //         //	CRemoteSocket m_s(m_nTimeOut);
        // 
        //         if (FALSE == SocketConnect(strMsvIP, m_s))
        //         {
        //             CString strError = _T("Error while connecting MSV[%s]");
        //             m_error_desc.Format(strError, strMsvIP);
        //             //NMTrace(m_strLogName, logLevelWarring, m_error_desc);
        //             return MSV_FAILED;
        //         }
        // 
        //         CString strSend(MXL_HEADER);
        //         /************************************************************/
        //         strSend += _T("<get_vtr_timecode>");
        //         strSend += _T("</get_vtr_timecode>");//查询状态
        //                                              /************************************************************/
        // 
        //         CXMLParser parser;
        //         if (FALSE == parser.Create())
        //         {
        //             CString strError = _T("Error occur while analyzing response message of MSV[%s]");
        //             m_error_desc.Format(strError, strMsvIP);
        //             //NMTrace(m_strLogName, logLevelWarring, m_error_desc);
        //             return MSV_FAILED;
        //         }
        //         InParam pIn;
        //         OutParam pOut;
        //         CString strRecv;
        //         pIn.m_s = m_s;
        //         //	pIn.m_s = &m_s;
        //         pIn.parser = &parser;
        //         pIn.strIp = strMsvIP;
        //         pIn.strOrigin = _T("get_vtr_timecode");
        //         pIn.strSend = strSend;
        //         pOut.strRecv = strRecv;
        // 
        //         int nRet = CommandOperation(pIn, pOut);
        //         if (nRet == -2)
        //             return MSV_NETERROR;
        //         else if (nRet == -1)
        //             return MSV_XMLERROR;
        //         else if (nRet == 0)
        //             return MSV_FAILED;
        // 
        //         CString strParam;
        //         ///////////////////////////
        //         //MSV启动模式
        //         strParam = parser.GetNodeText(_T("timecode"));
        //         lTimeCode = _wtoi((LPCTSTR)strParam);
        // 
        //         //m_error_desc = _T("操作成功");
        //         m_error_desc = _T("Operation succeed");
        //         return MSV_SUCCESS;
        //     }
        // 
        //     MSV_RET CClientTaskSDKImp::MSVSetUploadTask(CString strMsvIp, UploadInfo TaskInfo)
        //     {
        //         IRemoteSDK* m_s;
        //         m_s = CreateRemoteSDK();
        //         //	CRemoteSocket m_s(m_nTimeOut);
        // 
        // 
        //         if (FALSE == SocketConnect(strMsvIp, m_s))
        //         {
        //             CString strError = _T("Error while connecting MSV[%s]");
        //             m_error_desc.Format(strError, strMsvIp);
        //             //NMTrace(m_strLogName, logLevelWarring, m_error_desc);
        //             return MSV_FAILED;
        //         }
        //         /*格式化参数到XML文本*/
        //         CString strSend;
        // 
        //         strSend = FormatParamToString(TaskInfo, _T("set_upload_task"));
        // 
        //         /* 操作 1  */
        // 
        //         CXMLParser parser;
        //         if (FALSE == parser.Create())
        //         {
        //             CString strError = _T("Error occur while analyzing response message of MSV[%s]");
        //             m_error_desc.Format(strError, strMsvIp);
        //             //NMTrace(m_strLogName, logLevelWarring, m_error_desc);
        //             return MSV_FAILED;
        //         }
        //         InParam pIn;
        //         OutParam pOut;
        //         CString strRecv;
        //         pIn.m_s = m_s;
        //         //	pIn.m_s = &m_s;
        //         pIn.parser = &parser;
        //         pIn.strIp = strMsvIp;
        //         pIn.strOrigin = _T("set_upload_task");
        //         pIn.strSend = strSend;
        //         pOut.strRecv = strRecv;
        // 
        //         int nRet = CommandOperation(pIn, pOut);
        //         if (nRet == -2)
        //             return MSV_NETERROR;
        //         else if (nRet == -1)
        //             return MSV_XMLERROR;
        //         else if (nRet == 0)
        //             return MSV_FAILED;
        // 
        //         //m_error_desc = _T("操作成功");
        //         m_error_desc = _T("Operation succeed");
        //         return MSV_SUCCESS;
        //     }
        //     MSV_RET CClientTaskSDKImp::MSVDelAllUploadTask(CString strMsvIp)
        //     {
        //         IRemoteSDK* m_s;
        //         m_s = CreateRemoteSDK();
        //         //	CRemoteSocket m_s(m_nTimeOut);
        // 
        // 
        //         if (FALSE == SocketConnect(strMsvIp, m_s))
        //         {
        //             CString strError = _T("Error while connecting MSV[%s]");
        //             m_error_desc.Format(strError, strMsvIp);
        //             //NMTrace(m_strLogName, logLevelWarring, m_error_desc);
        //             return MSV_FAILED;
        //         }
        //         CString strSend(MXL_HEADER);
        //         /************************************************************/
        //         strSend += _T("<del_all_uploadtask>");
        //         strSend += _T("</del_all_uploadtask>");
        //         /************************************************************/
        // 
        //         CXMLParser parser;
        //         if (FALSE == parser.Create())
        //         {
        //             CString strError = _T("Error occur while analyzing response message of MSV[%s]");
        //             m_error_desc.Format(strError, strMsvIp);
        //             //NMTrace(m_strLogName, logLevelWarring, m_error_desc);
        //             return MSV_FAILED;
        //         }
        //         InParam pIn;
        //         OutParam pOut;
        //         CString strRecv;
        //         pIn.m_s = m_s;
        //         //	pIn.m_s = &m_s;
        //         pIn.parser = &parser;
        //         pIn.strIp = strMsvIp;
        //         pIn.strOrigin = _T("del_all_uploadtask");
        //         pIn.strSend = strSend;
        //         pOut.strRecv = strRecv;
        // 
        //         int nRet = CommandOperation(pIn, pOut);
        //         if (nRet == -2)
        //             return MSV_NETERROR;
        //         else if (nRet == -1)
        //             return MSV_XMLERROR;
        //         else if (nRet == 0)
        //             return MSV_FAILED;
        // 
        //         //m_error_desc = _T("操作成功");
        //         m_error_desc = _T("Operation succeed");
        //         return MSV_SUCCESS;
        //     }
        // 
        //     MSV_RET CClientTaskSDKImp::MSVGetUploadTaskCount(CString strMsvIp, int& taskCount)
        //     {
        //         IRemoteSDK* m_s;
        //         m_s = CreateRemoteSDK();
        //         //	CRemoteSocket m_s(m_nTimeOut);
        // 
        // 
        //         if (FALSE == SocketConnect(strMsvIp, m_s))
        //         {
        //             CString strError = _T("Error while connecting MSV[%s]");
        //             m_error_desc.Format(strError, strMsvIp);
        //             //NMTrace(m_strLogName, logLevelWarring, m_error_desc);
        //             return MSV_FAILED;
        //         }
        //         CString strSend(MXL_HEADER);
        //         /************************************************************/
        //         strSend += _T("<get_uploadtask_count>");
        //         strSend += _T("</get_uploadtask_count>");
        //         /************************************************************/
        // 
        //         CXMLParser parser;
        //         if (FALSE == parser.Create())
        //         {
        //             CString strError = _T("Error occur while analyzing response message of MSV[%s]");
        //             m_error_desc.Format(strError, strMsvIp);
        //             //NMTrace(m_strLogName, logLevelWarring, m_error_desc);
        //             return MSV_FAILED;
        //         }
        //         InParam pIn;
        //         OutParam pOut;
        //         CString strRecv;
        //         pIn.m_s = m_s;
        //         //	pIn.m_s = &m_s;
        //         pIn.parser = &parser;
        //         pIn.strIp = strMsvIp;
        //         pIn.strOrigin = _T("get_uploadtask_count");
        //         pIn.strSend = strSend;
        //         pOut.strRecv = strRecv;
        // 
        //         int nRet = CommandOperation(pIn, pOut);
        //         if (nRet == -2)
        //             return MSV_NETERROR;
        //         else if (nRet == -1)
        //             return MSV_XMLERROR;
        //         else if (nRet == 0)
        //             return MSV_FAILED;
        // 
        //         CString strParam;
        //         ///////////////////////////111
        //         //MSV启动模式 ///
        //         strParam = parser.GetNodeText(_T("taskcount"));
        //         taskCount = _wtoi((LPCTSTR)strParam);
        // 
        //         //m_error_desc = _T("操作成功");
        //         m_error_desc = _T("Operation succeed");
        //         return MSV_SUCCESS;
        //     }
        // 
        // 
        //     // 发送得到帧索引的命令 by yj
        //     MSV_RET CClientTaskSDKImp::MSVGetFrameIndex(CString strMSVIP, DWORD& dwFrameIndex)
        //     {
        //         IRemoteSDK* m_s;
        //         m_s = CreateRemoteSDK();
        //         //	CRemoteSocket m_s(m_nTimeOut);
        //         if (FALSE == SocketConnect(strMSVIP, m_s))
        //         {
        //             CString strError = _T("Error while connecting MSV[%s]");
        //             m_error_desc.Format(strError, strMSVIP);
        //             //NMTrace(m_strLogName, logLevelWarring, m_error_desc);
        //             return MSV_FAILED;
        //         }
        // 
        //         CString strSend(MXL_HEADER);
        //         /************************************************************/
        //         strSend += _T("<Get_MSV_FrameIndex>");
        //         strSend += _T("</Get_MSV_FrameIndex>");
        //         /************************************************************/
        // 
        //         CXMLParser parser;
        //         if (FALSE == parser.Create())
        //         {
        //             CString strError = _T("Error occur while analyzing response message of MSV[%s]");
        //             m_error_desc.Format(strError, strMSVIP);
        //             //NMTrace(m_strLogName, logLevelWarring, m_error_desc);
        //             return MSV_FAILED;
        //         }
        //         InParam pIn;
        //         OutParam pOut;
        //         CString strRecv;
        //         pIn.m_s = m_s;
        //         //	pIn.m_s = &m_s;
        //         pIn.parser = &parser;
        //         pIn.strIp = strMSVIP;
        //         pIn.strOrigin = _T("Get_MSV_FrameIndex");
        //         pIn.strSend = strSend;
        //         pOut.strRecv = strRecv;
        // 
        //         int nRet = CommandOperation(pIn, pOut);
        //         if (nRet == -2)
        //             return MSV_NETERROR;
        //         else if (nRet == -1)
        //             return MSV_XMLERROR;
        //         else if (nRet == 0)
        //             return MSV_FAILED;
        // 
        //         CString strParam;
        //         ///////////////////////////
        //         //MSV启动模式
        //         strParam = parser.GetNodeText(_T("dwFrameIndex"));
        //         dwFrameIndex = _wtoi((LPCTSTR)strParam);
        // 
        //         //m_error_desc = _T("操作成功");
        //         m_error_desc = _T("Operation succeed");
        //         return MSV_SUCCESS; //
        //     }
        // 
        //     //返回当前任务采集的帧数
        //     MSV_RET CClientTaskSDKImp::MSVGetFramesInTask(CString strMSVIP, DWORD& dwFrames)
        //     {
        // 
        //         return MSV_FAILED;
        //     }
        //     //返回当前任务采集的帧数
        //     MSV_RET CClientTaskSDKImp::MSVGetCurCaptureFrame(CString strMSVIP, DWORD& dwFrames)
        //     {
        //         return MSV_SUCCESS;
        //     }


        //     //查询MSV本地缓存容量
        //     MSV_RET CClientTaskSDKImp::MsvQueryLocalStorage(CString strMsvIP, MSV_LocalStorage& LocalStorage, int nChannel)
        //     {
        //         CString strLog;
        //         IRemoteSDK* m_s;
        //         m_s = CreateRemoteSDK();
        //         if (FALSE == SocketConnect(strMsvIP, m_s))
        //         {
        //             CString strError = _T("Error while connecting MSV[%s]");
        //             m_error_desc.Format(strError, strMsvIP);
        //             strLog.Format(_T("MsvQueryLocalStorage MSV_FAILED.%s"), m_error_desc);
        //             NMTrace0(SDKLOGNAME, logLevelWarring, (_bstr_t)strLog);
        //             return MSV_FAILED;
        //         }
        //         /*格式化参数到XML文本*/
        //         CString strSend(MXL_HEADER);
        //         /************************************************************/
        //         strSend += _T("<msv_query_LocalStorage>");
        //         CString strChannel;
        //         strChannel.Format(_T("<nChannel>%d</nChannel>"), nChannel);
        //         strSend += strChannel;
        //         strSend += _T("</msv_query_LocalStorage>");
        //         /************************************************************/
        //         /* 操作   */
        //         CXMLParser parser;
        //         if (FALSE == parser.Create())
        //         {
        //             CString strError = _T("Error occur while analyzing response message of MSV[%s]");
        //             m_error_desc.Format(strError, strMsvIP);
        //             strLog.Format(_T("MsvQueryLocalStorage MSV_FAILED.%s"), m_error_desc);
        //             NMTrace0(SDKLOGNAME, logLevelWarring, (_bstr_t)strLog);
        //             return MSV_FAILED;
        //         }
        //         InParam pIn;
        //         OutParam pOut;
        //         CString strRecv;
        //         pIn.m_s = m_s;
        //         //	pIn.m_s = &m_s;
        //         pIn.parser = &parser;
        //         pIn.strIp = strMsvIP;
        //         pIn.strOrigin = _T("msv_query_LocalStorage");
        //         pIn.strSend = strSend;
        //         pOut.strRecv = strRecv;
        // 
        //         int nRet = CommandOperation(pIn, pOut);
        //         if (nRet == -2)
        //             return MSV_NETERROR;
        //         else if (nRet == -1)
        //             return MSV_XMLERROR;
        //         else if (nRet == 0)
        //             return MSV_FAILED;
        // 
        // 
        //         CString strParam;
        //         strParam = parser.GetNodeText(_T("DiskCount"));
        //         LocalStorage.iDiskCount = _wtoi((LPCTSTR)strParam);
        // 
        //         CString strNode;
        //         for (int i = 0; i < LocalStorage.iDiskCount; ++i)
        //         {
        //             strNode.Format(_T("TotalSpace%d"), i);
        //             strParam = parser.GetNodeText(strNode);
        //             LocalStorage.dwTotalSpace[i] = _wtoi((LPCTSTR)strParam);
        // 
        //             strNode.Format(_T("FreeSpace%d"), i);
        //             strParam = parser.GetNodeText(strNode);
        //             LocalStorage.dwFreeSpace[i] = _wtoi((LPCTSTR)strParam);
        // 
        //             strNode.Format(_T("Driver%d"), i);
        //             LocalStorage.strDisk[i] = parser.GetNodeText(strNode);
        //         }
        // 
        //         //m_error_desc = _T("操作成功");
        //         m_error_desc = _T("Operation succeed");
        //         return MSV_SUCCESS;
        //     }
        //     //add by kdb on 2007-12-24(all next)
        // 
        //     MSV_RET CClientTaskSDKImp::MSVParamLoad(const CString strMsvIp)
        //     {
        //         return MSV_SUCCESS;
        //     }
        //     /************************************************************************/
        //     /*  获取MSV支持得编码格式                                              */
        //     /************************************************************************/
        //     MSV_RET CClientTaskSDKImp::MSVGetSupportEncodeType(const CString strMsvIp, CArray<EncodeType_st, EncodeType_st&>& typeList)
        //     {
        //         return MSV_SUCCESS;
        //     }
        //     /************************************************************************/
        //     /* 获得子编码格式                                                       */
        //     /************************************************************************/
        //     MSV_RET CClientTaskSDKImp::MSVGetSupportSubEncodeType(const CString strMsvIp, EncodeType_st encodeType, CArray<EncodeType_st, EncodeType_st&>& typeList)
        //     {
        //         return MSV_SUCCESS;
        //     }
        //     /************************************************************************/
        //     /* 获得文件格式                                                         */
        //     /************************************************************************/
        //     MSV_RET CClientTaskSDKImp::MSVGetSupportFileType(const CString strMsvIp,
        //                                                    EncodeType_st encodeType,
        //                                                    EncodeType_st subEncodeType,
        //                                                    CArray<File_st, File_st&>& fileType)
        //     {
        //         return MSV_SUCCESS;
        //     }
        //     MSV_RET CClientTaskSDKImp::MSVGetSupporAudiotFileType(CArray<EncodeType_st, EncodeType_st&>& typeList,
        //     													const CString strMsvIp,
        //                                                         EncodeType_st encodeType,
        //                                                         EncodeType_st subEncodeType,
        //                                                         File_st fileType)
        //     {
        //         return MSV_SUCCESS;
        //     }
        // 
        //     IRemoteSDK* CClientTaskSDKImp::CreateRemoteSDK()
        //     {
        //         if (m_pRemoteSDK != NULL)
        //             return m_pRemoteSDK;
        //         //IRemoteSDK *pSDK;
        //         IRemoteSDK::Initstance(m_nComtype, &m_pRemoteSDK);
        // 
        //         m_pRemoteSDK.Init(m_nTimeOut);
        //         return m_pRemoteSDK;
        //     }
        //     BOOL CClientTaskSDKImp::CloseRemoteSDK(IRemoteSDK* pSDK)
        //     {
        //         if (pSDK == NULL)
        //             return true;
        //         pSDK.Close();
        //         delete pSDK;
        //         pSDK = NULL;
        //         return TRUE;
        //     }
        // 
        //     MSV_RET CClientTaskSDKImp::MSVGetFieldFrame( const CString strMSVIP, LONGLONG &llFrame )
        //     {
        //         IRemoteSDK* m_s;
        //         m_s = CreateRemoteSDK();
        //         //	CRemoteSocket m_s(m_nTimeOut);
        //         if (FALSE == SocketConnect(strMSVIP, m_s))
        //         {
        //             CString strError = _T("Error while connecting MSV[%s]");
        //             m_error_desc.Format(strError, strMSVIP);
        //             //NMTrace(m_strLogName, logLevelWarring, m_error_desc);
        //             return MSV_FAILED;
        //         }
        // 
        //         CString strSend(MXL_HEADER);
        //         /************************************************************/
        //         strSend += _T("<msv_get_fieldFrame>");
        //         strSend += _T("</msv_get_fieldFrame>");
        //         /************************************************************/
        // 
        //         CXMLParser parser;
        //         if (FALSE == parser.Create())
        //         {
        //             CString strError = _T("Error occur while analyzing response message of MSV[%s]");
        //             m_error_desc.Format(strError, strMSVIP);
        //             //NMTrace(m_strLogName, logLevelWarring, m_error_desc);
        //             return MSV_FAILED;
        //         }
        //         InParam pIn;
        //         OutParam pOut;
        //         CString strRecv;
        //         pIn.m_s = m_s;
        //         //	pIn.m_s = &m_s;
        //         pIn.parser = &parser;
        //         pIn.strIp = strMSVIP;
        //         pIn.strOrigin = _T("msv_get_fieldFrame");
        //         pIn.strSend = strSend;
        //         pOut.strRecv = strRecv;
        // 
        //         int nRet = CommandOperation(pIn, pOut);
        //         if (nRet == -2)
        //             return MSV_NETERROR;
        //         else if (nRet == -1)
        //             return MSV_XMLERROR;
        //         else if (nRet == 0)
        //             return MSV_FAILED;
        // 
        //         CString strParam;
        //         ///////////////////////////
        //         //MSV启动模式
        //         strParam = parser.GetNodeText(_T("dwFrames"));
        //         llFrame = _wtoi64((LPCTSTR)strParam);
        // 
        //         //m_error_desc = _T("操作成功");
        //         m_error_desc = _T("Operation succeed");
        //         return MSV_SUCCESS; //
        //     }
        // 
        //     MSV_RET CClientTaskSDKImp::MSVModifyTaskLength(const CString strMsvIp, DWORD dwTaskID, DWORD dwLength, int nChannel /* = -1 */)
        //     {
        //         //	CRemoteSocket m_s(m_nTimeOut);
        //         IRemoteSDK* m_s;
        //         m_s = CreateRemoteSDK();
        //         if (FALSE == SocketConnect(strMsvIp, m_s))
        //         {
        //             CString strError = _T("Error while connecting MSV[%s]");
        //             m_error_desc.Format(strError, strMsvIp);
        //             //NMTrace(m_strLogName, logLevelWarring, m_error_desc);		
        //             return MSV_FAILED;
        //         }
        //         /*格式化参数到XML文本*/
        //         CString strSend(MXL_HEADER);
        //         CString strName;
        //         strName.Format(_T("<TaskID>%d</TaskID>"), dwTaskID);
        //         strSend += _T("<modify_msv_task_length>");
        //         strSend += strName;
        //         CString length;
        //         length.Format(_T("<new_Length>%d</new_Length>"), dwLength); //新的采集长度
        //         strSend += length;
        //         strSend += _T("</modify_msv_task_length>");
        // 
        //         /* 操作   */
        //         CXMLParser parser;
        //         if (FALSE == parser.Create())
        //         {
        //             CString strError = _T("Error occur while analyzing response message of MSV[%s]");
        //             m_error_desc.Format(strError, strMsvIp);
        //             //NMTrace(m_strLogName, logLevelWarring, m_error_desc);
        //             return MSV_FAILED;
        //         }
        //         InParam pIn;
        //         OutParam pOut;
        //         CString strRecv;
        //         pIn.m_s = m_s;
        //         pIn.parser = &parser;
        //         pIn.strIp = strMsvIp;
        //         pIn.strOrigin = _T("modify_msv_task_length");
        //         pIn.strSend = strSend;
        //         pOut.strRecv = strRecv;
        // 
        //         int nRet = CommandOperation(pIn, pOut);
        //         if (nRet == -2)
        //             return MSV_NETERROR;
        //         else if (nRet == -1)
        //             return MSV_XMLERROR;
        //         else if (nRet == 0)
        //             return MSV_FAILED;
        // 
        //         //m_error_desc = _T("操作成功");
        //         m_error_desc = _T("Operation succeed");
        // 
        //         return MSV_SUCCESS;
        //     }
        // 
        //     MSV_RET CClientTaskSDKImp::MSVGetFieldIntEvt( const CString strMSVIP, HANDLE & nHandle )
        //     {
        //         IRemoteSDK* m_s;
        //         m_s = CreateRemoteSDK();
        //         //	CRemoteSocket m_s(m_nTimeOut);
        //         if (FALSE == SocketConnect(strMSVIP, m_s))
        //         {
        //             CString strError = _T("Error while connecting MSV[%s]");
        //             m_error_desc.Format(strError, strMSVIP);
        //             //NMTrace(m_strLogName, logLevelWarring, m_error_desc);
        //             return MSV_FAILED;
        //         }
        // 
        //         CString strSend(MXL_HEADER);
        //         /************************************************************/
        //         strSend += _T("<msv_get_FieldIntEvt>");
        //         strSend += _T("</msv_get_FieldIntEvt>");
        //         /************************************************************/
        // 
        //         CXMLParser parser;
        //         if (FALSE == parser.Create())
        //         {
        //             CString strError = _T("Error occur while analyzing response message of MSV[%s]");
        //             m_error_desc.Format(strError, strMSVIP);
        //             //NMTrace(m_strLogName, logLevelWarring, m_error_desc);
        //             return MSV_FAILED;
        //         }
        //         InParam pIn;
        //         OutParam pOut;
        //         CString strRecv;
        //         pIn.m_s = m_s;
        //         //	pIn.m_s = &m_s;
        //         pIn.parser = &parser;
        //         pIn.strIp = strMSVIP;
        //         pIn.strOrigin = _T("msv_get_FieldIntEvt");
        //         pIn.strSend = strSend;
        //         pOut.strRecv = strRecv;
        // 
        //         int nRet = CommandOperation(pIn, pOut);
        //         if (nRet == -2)
        //             return MSV_NETERROR;
        //         else if (nRet == -1)
        //             return MSV_XMLERROR;
        //         else if (nRet == 0)
        //             return MSV_FAILED;
        // 
        //         CString strParam;
        //         ///////////////////////////
        //         //MSV启动模式
        //         strParam = parser.GetNodeText(_T("fieldIntEvt"));
        //         DWORD_PTR dwHandle = _wtoi((LPCTSTR)strParam);
        // 
        //         strParam = parser.GetNodeText(_T("processID"));
        //         DWORD dwProcessID = _wtoi((LPCTSTR)strParam);
        //         //执行duplicate
        // 
        //         HANDLE hProcess = OpenProcess(PROCESS_ALL_ACCESS,
        //                                     1, dwProcessID);
        //         DuplicateHandle(hProcess,
        //                         (HANDLE)dwHandle,
        //                         GetCurrentProcess(),
        //                         &nHandle,
        //                         0, TRUE,
        //                         DUPLICATE_SAME_ACCESS);
        //         //m_error_desc = _T("操作成功");
        //         m_error_desc = _T("Operation succeed");
        //         return MSV_SUCCESS; //
        //     }

        // 

        // 
        //     MSV_RET CClientTaskSDKImp::MSV_ReIngest_ForDR( const CString strMsvIp, DWORD dwDRFileIn, DWORD dwDRFileOut, DWORD dwRecordFileIn, DWORD dwRecordFileOut, DWORD dwSPCTag, CString pcDRFileName)
        //     {
        //         IRemoteSDK* m_s;
        //         m_s = CreateRemoteSDK();
        // 
        //         if (FALSE == SocketConnect(strMsvIp, m_s))
        //         {
        //             CString strError = _T("Error while connecting MSV[%s]");
        //             m_error_desc.Format(strError, strMsvIp);
        //             //NMTrace(m_strLogName, logLevelWarring, m_error_desc);
        //             return MSV_FAILED;
        //         }
        //         CString strSend(MXL_HEADER);
        //         CString strTemp;
        //         /************************************************************/
        //         strSend += _T("<ReIngest_for_DR>");
        //         strSend += _T("<DRFileIn>");
        //         strTemp.Format(_T("%d"), dwDRFileIn);
        //         strSend += strTemp;
        //         strSend += _T("</DRFileIn>");
        // 
        //         strSend += _T("<DRFileOut>");
        //         strTemp.Format(_T("%d"), dwDRFileOut);
        //         strSend += strTemp;
        //         strSend += _T("</DRFileOut>");
        // 
        //         strSend += _T("<RecordFileIn>");
        //         strTemp.Format(_T("%d"), dwRecordFileIn);
        //         strSend += strTemp;
        //         strSend += _T("</RecordFileIn>");
        // 
        //         strSend += _T("<RecordFileOut>");
        //         strTemp.Format(_T("%d"), dwRecordFileOut);
        //         strSend += strTemp;
        //         strSend += _T("</RecordFileOut>");
        // 
        //         strSend += _T("<SPCTag>");
        //         strTemp.Format(_T("%d"), dwSPCTag);
        //         strSend += strTemp;
        //         strSend += _T("</SPCTag>");
        // 
        //         strSend += _T("<DRFileName>");
        //         strTemp.Format(_T("%s"), pcDRFileName);
        //         strSend += strTemp;
        //         strSend += _T("</DRFileName>");
        // 
        //         /*
        //         <ReIngest_for_DR>
        //         <DRFileIn></DRFileIn>
        //         <DRFileOut></DRFileOut>
        //         <RecordFileIn></RecordFileIn>
        //         <RecordFileOut></RecordFileOut>
        //         <SPCTag></SPCTag>
        //         <DRFileName></DRFileName>
        //         </ReIngest_for_DR>
        //       /**/
        //         strSend += _T("</ReIngest_for_DR>");
        //         /************************************************************/
        // 
        //         CXMLParser parser;
        //         if (FALSE == parser.Create())
        //         {
        //             CString strError = _T("Error occur while analyzing response message of MSV[%s]");
        //             m_error_desc.Format(strError, strMsvIp);
        //             //NMTrace(m_strLogName, logLevelWarring, m_error_desc);
        //             return MSV_FAILED;
        //         }
        //         InParam pIn;
        //         OutParam pOut;
        //         CString strRecv;
        //         pIn.m_s = m_s;
        //         //	pIn.m_s = &m_s;
        //         pIn.parser = &parser;
        //         pIn.strIp = strMsvIp;
        //         pIn.strOrigin = _T("msv_get_FieldIntEvt");
        //         pIn.strSend = strSend;
        //         pOut.strRecv = strRecv;
        //         int nRet = CommandOperation(pIn, pOut);
        //         return MSV_SUCCESS;
        //     }
        // 
        //     MSV_RET CClientTaskSDKImp::MSV_BindTsPgm( const CString &strMsvIp, int nAnalyzeChannelID, int nPgmID, bool bStopBind)
        //     {
        //         CString strLog;
        //         strLog.Format(_T("INFO: MSV_BindTsPgm, NOT USE"));
        //         NMTrace0(SDKLOGNAME, logLevelWarring, (_bstr_t)strLog);
        //         return MSV_SUCCESS;
        // 
        //         /*IRemoteSDK *m_s;
        //         m_s = CreateRemoteSDK();
        // 
        //         if(FALSE == SocketConnect(strMsvIp,m_s))
        //         {
        //             CString strError = _T("Error while connecting MSV[%s]");
        //             m_error_desc.Format(strError, strMsvIp);
        //             //NMTrace(m_strLogName, logLevelWarring, m_error_desc);
        //             return MSV_FAILED;
        //         }
        // 
        //         CString strSend(MXL_HEADER);
        //         CString strTemp;
        // 
        //         strSend+=_T("<BindPgm>");
        //         strTemp.Format(_T("<analyzeID>%d</analyzeID>"),nAnalyzeChannelID);
        //         strSend +=strTemp;
        // 
        //         strTemp.Format(_T("<pgmID>%d</pgmID>"),nPgmID);
        //         strSend += strTemp;
        // 
        //         if(bStopBind)
        //         {
        //             strTemp.Format(_T("<StopBind>1</StopBind>"));
        //             strSend += strTemp;
        //         }
        //         strSend+=_T("</BindPgm>");
        // 
        //         CXMLParser parser;
        //         if(FALSE== parser.Create())
        //         {
        //             CString strError = _T("Error occur while analyzing response message of MSV[%s]");
        //             m_error_desc.Format(strError, strMsvIp);
        //             //NMTrace(m_strLogName, logLevelWarring, m_error_desc);
        //             return MSV_FAILED;
        //         }
        //         InParam pIn;
        //         OutParam pOut;
        //         CString  strRecv;
        //         pIn.m_s = m_s;
        //         //	pIn.m_s = &m_s;
        //         pIn.parser = &parser;
        //         pIn.strIp = strMsvIp;
        //         pIn.strOrigin = _T("BindPgm");
        //         pIn.strSend   = strSend;
        //         pOut.strRecv =  strRecv;	
        //         int nRet = CommandOperation(pIn,pOut);
        //         if(nRet == -2)
        //             return MSV_NETERROR;
        //         else if(nRet == -1)
        //             return MSV_XMLERROR;
        //         else if(nRet == 0)
        //             return MSV_FAILED;
        //         return MSV_SUCCESS;/**/
        //     }
        // 
        //     //该接口未用
        //     MSV_RET CClientTaskSDKImp::MSV_GetTsInfo(const CString &strTSIp,const int nTSPort, CString &strTSInfo, int AnalyzeChannelID/*=-1*/)
        //     {
        //         CString strLog;
        //         strLog.Format(_T("INFO: MSV_GetTsInfo, NOT USE"));
        //         NMTrace0(SDKLOGNAME, logLevelWarring, (_bstr_t)strLog);
        //         return MSV_SUCCESS;
        // 
        //         // 	IRemoteSDK *m_s;
        //         // 	m_s = CreateRemoteSDK();
        //         // 	if(FALSE == SocketConnect(strTSIp,nTSPort,m_s))
        //         // 	{
        //         // 		m_error_desc.Format(_T("Error while conneting TS[%s][%d]"),strTSIp,nTSPort);
        //         // 		return MSV_FAILED;
        //         // 	}
        //         // 
        //         // 	CString strSend(MXL_HEADER);
        //         // 	CString strTemp;
        //         // 	strSend +=_T("<GetAllProgramInfoList>");
        //         // 	strTemp.Format(_T("<AnalyzeChannelID>%d</AnalyzeChannelID>"),AnalyzeChannelID);
        //         // 	strSend += strTemp;
        //         // 	strSend +=_T("</GetAllProgramInfoList>");
        //         // 	long lRet = m_s.SendCmdString(strSend);
        //         // 	if(lRet ==0|| lRet == SOCKET_ERROR)
        //         // 	{
        //         // 		m_error_desc.Format(_T("SendCmd Faild."));
        //         // 		return MSV_FAILED;
        //         // 	}
        //         /************************************************************************
        //         <RefreshDataSource>
        //         <AppType>
        //             <Type>2</Type><InstanceIndex>0</InstanceIndex><CardIndex>0</CardIndex><ChannelIndex>0</ChannelIndex><ChannelName>TS_Ingest_V2_0_0</ChannelName>
        //             <ChannelDisplayName>TS_Ingest_V2_0_0</ChannelDisplayName>
        //         </AppType>
        //         <AppType>
        //             <Type>2</Type><InstanceIndex>0</InstanceIndex><CardIndex>0</CardIndex><ChannelIndex>1</ChannelIndex><ChannelName>TS_Ingest_V2_0_1</ChannelName>
        //             <ChannelDisplayName>TS_Ingest_V2_0_1</ChannelDisplayName>
        //         </AppType>
        //         <GetAllProgramInfoList>
        //             <ProgramInfoList ChannelName="TS_Ingest_V2_0_0" ChannelID="0">
        //                 <OneProgramInfo><PgmName>CCTV-1</PgmName><PgmID>512</PgmID><VideoID>512</VideoID><AudioID>650</AudioID><VideoDataRate>10000</VideoDataRate></OneProgramInfo>
        //                 <OneProgramInfo><PgmName>CCTV-2</PgmName><PgmID>513</PgmID><VideoID>513</VideoID><AudioID>660</AudioID><VideoDataRate>10000</VideoDataRate></OneProgramInfo>
        //                 <OneProgramInfo><PgmName>CCTV-7</PgmName><PgmID>514</PgmID><VideoID>514</VideoID><AudioID>670</AudioID><VideoDataRate>10000</VideoDataRate></OneProgramInfo>
        //                 <OneProgramInfo><PgmName>CCTV-10</PgmName><PgmID>515</PgmID><VideoID>515</VideoID><AudioID>680</AudioID><VideoDataRate>10000</VideoDataRate></OneProgramInfo>
        //                 <OneProgramInfo><PgmName>CCTV-11</PgmName><PgmID>516</PgmID><VideoID>516</VideoID><AudioID>690</AudioID><VideoDataRate>10000</VideoDataRate></OneProgramInfo>
        //                 <OneProgramInfo><PgmName>CCTV-12</PgmName><PgmID>517</PgmID><VideoID>517</VideoID><AudioID>700</AudioID><VideoDataRate>10000</VideoDataRate></OneProgramInfo>
        //                 <OneProgramInfo><PgmName>CCTV-MUSIC</PgmName><PgmID>518</PgmID><VideoID>518</VideoID><AudioID>710</AudioID><VideoDataRate>10000</VideoDataRate></OneProgramInfo>
        //             </ProgramInfoList>
        //             <ProgramInfoList ChannelName="TS_Ingest_V2_0_1" ChannelID="1"/>
        //         </GetAllProgramInfoList>
        // 
        //         <MediaDataInfoList>
        //             <PgmID>512</PgmID>
        //             <ChannelID>0</ChannelID>
        //             <ChannelName>TS_Ingest_V2_0_0</ChannelName>
        //             <OneMediaDataInfo>
        //                 <AudioMediaType>256</AudioMediaType><SamplesPerSecond>48000</SamplesPerSecond><AudioChannelCount>1</AudioChannelCount><BitsPerSample>16</BitsPerSample><VideoMediaType>2</VideoMediaType><VideoWidth>720</VideoWidth><VideoHeight>576</VideoHeight><FrameRate>25.000000</FrameRate><VideoBitCount>16</VideoBitCount><ProgressiveSequence>0</ProgressiveSequence><ChromaFormat>1</ChromaFormat><FrameType>50</FrameType><PictureStructure>3</PictureStructure><BitFlags>0</BitFlags><Fields>0</Fields>
        //             </OneMediaDataInfo>
        //             <OneMediaDataInfo>
        //                 <AudioMediaType>256</AudioMediaType><SamplesPerSecond>48000</SamplesPerSecond><AudioChannelCount>1</AudioChannelCount><BitsPerSample>16</BitsPerSample><VideoMediaType>1024</VideoMediaType><VideoWidth>720</VideoWidth><VideoHeight>576</VideoHeight><FrameRate>25.000000</FrameRate><VideoBitCount>16</VideoBitCount><ProgressiveSequence>0</ProgressiveSequence><ChromaFormat>1</ChromaFormat><FrameType>50</FrameType><PictureStructure>3</PictureStructure><BitFlags>0</BitFlags><Fields>0</Fields>
        //             </OneMediaDataInfo>
        //         </MediaDataInfoList>
        // 
        //         <ProgramInfoNotify><Type>0</Type><NotifyString/></ProgramInfoNotify>
        //         </RefreshDataSource>
        // 
        //         ------------------------------------------------------------------------------------
        //         ProgramInfoList  一个
        //         ------------------------------------------------------------------------------------
        //         GetAllProgramInfoList 所有
        //         ------------------------------------------------------------------------------------
        // 
        //         ************************************************************************/
        //         // 	Sleep(100);
        //         // 	CString strRecv;
        //         // 	CTime t;
        //         // 	t = CTime::GetCurrentTime();
        //         // 	CString xx = t.Format(_T("hours: %H, mins: %M, secs: %S"));
        //         // 	//AfxMessageBox(xx);
        //         // 	lRet = m_s.ReceiveCmdString2(strTSInfo);
        //         // 	if(lRet ==0 || lRet == SOCKET_ERROR)
        //         // 	{
        //         // 		m_error_desc.Format(_T("receive Data Error from [%s]"), strTSIp);
        //         // 		return MSV_FAILED;
        //         // 	}
        //         /*
        //         CXMLParser parser;
        //         if(FALSE == parser.Create())
        //         {
        //             CString strError = _T("Error occur while analyzing response message of TS[%s]");
        //             m_error_desc.Format(strError, strTSIp);
        //             //NMTrace(m_strLogName, logLevelWarring, m_error_desc);
        //             return MSV_FAILED;
        //         }
        // 
        //         parser.GetRoot();
        //         InParam pIn;
        //         OutParam pOut;
        //         CString strRecv;
        //         pIn.m_s = m_s;
        //         pIn.strIp = strTSIp;
        //         pIn.strOrigin = _T("");
        // 
        //         return MSV_SUCCESS;/**/
        //     }
        // 
        //     MSV_RET CClientTaskSDKImp::MSV_GetTsInfo( const CString &strTSIp,const int nTSPort, TS_DataChannelInfo** pTSDataChannelInfo, int &nChannelCount , int AnalyzeChannelID/*=-1*/ )
        //     {
        //         CString strLog;
        //         strLog.Format(_T("INFO: MSV_GetTsInfo(const CString,const int,TS_DataChannelInfo,int,int), NOT USE"));
        //         NMTrace0(SDKLOGNAME, logLevelWarring, (_bstr_t)strLog);
        //         return MSV_SUCCESS;
        // 
        //         /*CString strRecive;
        //         if(MSV_SUCCESS!=MSV_GetTsInfo(strTSIp,nTSPort,strRecive,AnalyzeChannelID))
        //             return MSV_FAILED;
        //         //解析xml	
        //         CXMLParser parser;
        //         if(FALSE == parser.Create())
        //         {
        //             CString strError = _T("Error occur while analyzing response message of TS[%s]");
        //             m_error_desc.Format(strError, strTSIp);
        //             //NMTrace(m_strLogName, logLevelWarring, m_error_desc);
        //             return MSV_FAILED;
        //         }
        // 
        //         if(FALSE == parser.Parse(strRecive))
        //             return MSV_FAILED;
        //         CString strName,strPara;
        //         parser.GetRoot(strName,strPara);
        //         int iAllCount = parser.GetNodeCount();
        //         CXMLParser parser2;
        //         if(FALSE == parser2.Create())
        //         {
        //             return MSV_FAILED;
        //         }
        //         //TODO: 需要解析xml 
        //         CString strValue;
        //         TS_DataChannelInfo *pDataInfoTemp;
        //         TS_PgmInfo *pPgmInfoTemp;
        //         //*pTSDataChannelInfo =new TS_DataChannelInfo[iAllCount];
        //         if(iAllCount>0)
        //         {
        //             TS_DataChannelInfo *pArr = new TS_DataChannelInfo[iAllCount];
        // 
        //             for(int i=0;i<iAllCount;i++)
        //             {
        //                 CString strXml = parser.GetNodeXML(_T("ProgramInfoList"),i);
        //                 if(strXml==_T(""))
        //                 {
        //                     strXml = parser.GetNodeXML(_T("msvInfos"));
        //                     if(strXml==_T(""))
        //                         continue;
        //                     if(FALSE == parser2.Parse(strXml))
        //                         continue;
        //                     //parser msvInfo
        //                     MSV_ChannelInfo pMsvInfo;
        //                     int iCount = parser2.GetNodeCount();
        //                     for(int j=0;j<iCount;j++)
        //                     {
        //                         strValue = parser2.GetNodeText(_T("msvId"));
        //                         pMsvInfo.nChannelID =_wtol((LPCTSTR)strValue);
        //                         strValue = parser2.GetNodeText(_T("port"));
        //                         pMsvInfo.dwCtrlPort =_wtol((LPCTSTR)strValue);
        //                     }
        //                     continue;
        //                 }
        //                 if(FALSE==parser2.Parse(strXml))
        //                 {
        //                     continue;
        //                 }
        //                 //pDataInfoTemp = pTSDataChannelInfo[i];
        //                 pDataInfoTemp = &pArr[i];
        //                 strValue = parser2.GetNodeAttribute(_T("ChannelName"));
        //                 _tcscpy_s(pDataInfoTemp.strDataChannel_Name,_countof(pDataInfoTemp.strDataChannel_Name),strValue);
        //                 strValue = parser2.GetNodeAttribute(_T("ChannelID"));
        //                 pDataInfoTemp.dwDataChannel_ID= _wtol((LPCTSTR)strValue);
        // 
        // 
        //                 int iCount = parser2.GetNodeCount();
        //                 pDataInfoTemp.nPgmCount =iCount;
        //                 if(iCount>0)
        //                 {
        //                     TS_PgmInfo *pPgmArr = new TS_PgmInfo[iCount];
        //                     for(int j=0;j<iCount;j++)
        //                     {
        //                         //pPgmInfoTemp = &(pDataInfoTemp.ts_PgmInfo[j]);
        //                         pPgmInfoTemp =&(pPgmArr[j]);
        // 
        //                         strValue = parser2.GetNodeText(_T("PgmName"),j);
        //                         _tcscpy_s(pPgmInfoTemp.strPgmName,_countof(pPgmInfoTemp.strPgmName),strValue);
        // 
        //                         strValue = parser2.GetNodeText(_T("PgmID"),j);
        //                         pPgmInfoTemp.dwPgmID = _wtol((LPCTSTR)strValue);
        // 
        //                         strValue = parser2.GetNodeText(_T("VideoID"),j);
        // 
        //                         strValue = parser2.GetNodeText(_T("AudioID"),j);
        // 
        //                         strValue = parser2.GetNodeText(_T("VideoDataRate"),j);
        //                         pPgmInfoTemp.tsSingalInfo.dwVideoDataRate = _wtol((LPCTSTR)strValue);
        //                         {
        //                             //ts源信息	
        //                             strValue = parser2.GetNodeText(_T("AudioMediaType"),j);
        //                             pPgmInfoTemp.tsSingalInfo.nAudioType =_wtol((LPCTSTR)strValue);
        // 
        //                             strValue = parser2.GetNodeText(_T("AudioChannelCount"),j);
        //                             pPgmInfoTemp.tsSingalInfo.nAudioCount =_wtol((LPCTSTR)strValue);
        // 
        //                             strValue = parser2.GetNodeText(_T("BitsPerSample"),j);
        //                             pPgmInfoTemp.tsSingalInfo.dwBitsPerSample =_wtol((LPCTSTR)strValue);
        // 
        //                             strValue = parser2.GetNodeText(_T("SamplesPerSecond"),j);
        //                             pPgmInfoTemp.tsSingalInfo.dwSamplePerSecond =_wtol((LPCTSTR)strValue);
        // 
        //                             strValue = parser2.GetNodeText(_T("VideoMediaType"),j);
        //                             pPgmInfoTemp.tsSingalInfo.nVideoType =_wtol((LPCTSTR)strValue);
        // 
        //                             strValue = parser2.GetNodeText(_T("VideoWidth"),j);
        //                             pPgmInfoTemp.tsSingalInfo.nVideoWidth =_wtol((LPCTSTR)strValue);
        // 
        //                             strValue = parser2.GetNodeText(_T("VideoHeight"),j);
        //                             pPgmInfoTemp.tsSingalInfo.nVideoHeight =_wtol((LPCTSTR)strValue);
        // 
        //                             strValue = parser2.GetNodeText(_T("FrameRate"),j);
        //                             pPgmInfoTemp.tsSingalInfo.fps =_wtol((LPCTSTR)strValue);
        // 
        //                             strValue = parser2.GetNodeText(_T("VideoBitCount"),j);
        //                             pPgmInfoTemp.tsSingalInfo.dwBitCount =_wtol((LPCTSTR)strValue);
        // 
        //                             strValue = parser2.GetNodeText(_T("FrameType"),j);
        //                             pPgmInfoTemp.tsSingalInfo.nFrameType =_wtol((LPCTSTR)strValue);
        //                         }
        //                     }
        // 
        //                     pDataInfoTemp.pTS_PgmInfo = pPgmArr;
        //                 }
        //             }
        //             *pTSDataChannelInfo = pArr;
        //         }
        //         nChannelCount = iAllCount;
        // 
        //         return MSV_SUCCESS;/**/
        //     }
        // 
        //     MSV_RET CClientTaskSDKImp::MSV_GetTsInfo( const CString &strTSIp,const int nTSPort, std::map<int, TS_DataChannelInfoEx> &pTSDataChannelInfo, int AnalyzeChannelID/*=-1*/ )
        //     {
        //         CString strLog;
        //         strLog.Format(_T("INFO: MSV_GetTsInfo(const CString,const int,std::map<int,TS_DataChannelInfoEx>,int), NOT USE"));
        //         NMTrace0(SDKLOGNAME, logLevelWarring, (_bstr_t)strLog);
        //         return MSV_SUCCESS;
        // 
        //         /*CString strRecive;
        //         if(MSV_SUCCESS!=MSV_GetTsInfo(strTSIp,nTSPort,strRecive,AnalyzeChannelID))
        //             return MSV_FAILED;
        //         //解析xml	
        //         CXMLParser parser;
        //         if(FALSE == parser.Create())
        //         {
        //             CString strError = _T("Error occur while analyzing response message of TS[%s]");
        //             m_error_desc.Format(strError, strTSIp);
        //             //NMTrace(m_strLogName, logLevelWarring, m_error_desc);
        //             return MSV_FAILED;
        //         }
        // 
        //         if(FALSE == parser.Parse(strRecive))
        //             return MSV_FAILED;
        //         CString strName,strPara;
        //         parser.GetRoot(strName,strPara);
        //         int iAllCount = parser.GetNodeCount();
        //         CXMLParser parser2;
        //         if(FALSE == parser2.Create())
        //         {
        //             return MSV_FAILED;
        //         }
        //         //TODO: 需要解析xml 
        //         CString strValue;
        //         TS_DataChannelInfoEx *pDataInfoTemp;
        //         TS_PgmInfo *pPgmInfoTemp;
        // 
        //         if(iAllCount>0)
        //         {
        //             for(int i=0;i<iAllCount;i++)
        //             {
        //                 CString strXml = parser.GetNodeXML(_T("ProgramInfoList"),i);
        // 
        //                 if(FALSE==parser2.Parse(strXml))
        //                 {
        //                     continue;
        //                 }
        //                 //pDataInfoTemp = pTSDataChannelInfo[i];
        //                 TS_DataChannelInfoEx dataInfo;
        //                 pDataInfoTemp = &dataInfo;
        // 
        //                 strValue = parser2.GetNodeAttribute(_T("ChannelName"));
        //                 _tcscpy_s(pDataInfoTemp.strDataChannel_Name,_countof(pDataInfoTemp.strDataChannel_Name),strValue);
        //                 strValue = parser2.GetNodeAttribute(_T("ChannelID"));
        //                 pDataInfoTemp.dwDataChannel_ID= _wtol((LPCTSTR)strValue);
        // 
        //                 int iCount = parser2.GetNodeCount();
        //                 pDataInfoTemp.nPgmCount =iCount;
        // 
        //                 for(int j=0;j<iCount;j++)
        //                 {
        //                     //pPgmInfoTemp = &(pDataInfoTemp.ts_PgmInfo[j]);
        //                     TS_PgmInfo pgmInfo;
        //                     pPgmInfoTemp =&pgmInfo;
        // 
        //                     strValue = parser2.GetNodeText(_T("PgmName"),j);
        //                     _tcscpy_s(pPgmInfoTemp.strPgmName,_countof(pPgmInfoTemp.strPgmName),strValue);
        // 
        //                     strValue = parser2.GetNodeText(_T("PgmID"),j);
        //                     pPgmInfoTemp.dwPgmID = _wtol((LPCTSTR)strValue);
        // 
        //                     strValue = parser2.GetNodeText(_T("VideoID"),j);
        // 
        //                     strValue = parser2.GetNodeText(_T("AudioID"),j);
        // 
        //                     strValue = parser2.GetNodeText(_T("VideoDataRate"),j);
        //                     pPgmInfoTemp.tsSingalInfo.dwVideoDataRate = _wtol((LPCTSTR)strValue);
        //                     {
        //                         //ts源信息	
        //                         strValue = parser2.GetNodeText(_T("AudioMediaType"),j);
        //                         pPgmInfoTemp.tsSingalInfo.nAudioType =_wtol((LPCTSTR)strValue);
        // 
        //                         strValue = parser2.GetNodeText(_T("AudioChannelCount"),j);
        //                         pPgmInfoTemp.tsSingalInfo.nAudioCount =_wtol((LPCTSTR)strValue);
        // 
        //                         strValue = parser2.GetNodeText(_T("BitsPerSample"),j);
        //                         pPgmInfoTemp.tsSingalInfo.dwBitsPerSample =_wtol((LPCTSTR)strValue);
        // 
        //                         strValue = parser2.GetNodeText(_T("SamplesPerSecond"),j);
        //                         pPgmInfoTemp.tsSingalInfo.dwSamplePerSecond =_wtol((LPCTSTR)strValue);
        // 
        //                         strValue = parser2.GetNodeText(_T("VideoMediaType"),j);
        //                         pPgmInfoTemp.tsSingalInfo.nVideoType =_wtol((LPCTSTR)strValue);
        // 
        //                         strValue = parser2.GetNodeText(_T("VideoWidth"),j);
        //                         pPgmInfoTemp.tsSingalInfo.nVideoWidth =_wtol((LPCTSTR)strValue);
        // 
        //                         strValue = parser2.GetNodeText(_T("VideoHeight"),j);
        //                         pPgmInfoTemp.tsSingalInfo.nVideoHeight =_wtol((LPCTSTR)strValue);
        // 
        //                         strValue = parser2.GetNodeText(_T("FrameRate"),j);
        //                         //pPgmInfoTemp.tsSingalInfo.fps =_wtol((LPCTSTR)strValue);
        //                         pPgmInfoTemp.tsSingalInfo.fps =_wtof((LPCTSTR)strValue);//_wtol((LPCTSTR)strValue);
        // 
        //                         strValue = parser2.GetNodeText(_T("VideoBitCount"),j);
        //                         pPgmInfoTemp.tsSingalInfo.dwBitCount =_wtol((LPCTSTR)strValue);
        // 
        //                         strValue = parser2.GetNodeText(_T("FrameType"),j);
        //                         pPgmInfoTemp.tsSingalInfo.nFrameType =_wtol((LPCTSTR)strValue);
        //                     }
        //                     dataInfo.pTS_PgmInfo[pgmInfo.dwPgmID] = (pgmInfo);
        // 
        // 
        // 
        //                 }
        //                 pTSDataChannelInfo[dataInfo.dwDataChannel_ID] = (dataInfo);
        //             }		
        //         }	
        //         return MSV_SUCCESS;/**/
        //     }
        // 
        //     MSV_RET CClientTaskSDKImp::MSVStartTask_For_Stream( const CString strMsvIP, const TASK_ALL_PARAM param, const CString strUrl, const CString StreamType, int nChannel /*= -1 */ )
        //     {
        //         return MSVStartTask(strMsvIP, param, nChannel);
        //         return MSV_FAILED;
        //     }
        // 
        //     MSV_RET CClientTaskSDKImp::MSV_GetMSVInfo( const CString &strServerIp,const int nServerPort, MSV_ChannelInfo** pMSVInfo, int &nChannelCount)
        //     {
        //         CString strLog;
        //         strLog.Format(_T("INFO: MSV_GetMSVInfo( const CString,const int,MSV_ChannelInfo,int), NOT USE"));
        //         NMTrace0(SDKLOGNAME, logLevelWarring, (_bstr_t)strLog);
        //         return MSV_SUCCESS;
        // 
        //         /*IRemoteSDK *m_s;
        //         m_s = CreateRemoteSDK();
        //         if(FALSE == SocketConnect(strServerIp,nServerPort,m_s))
        //         {
        //             m_error_desc.Format(_T("Error while conneting TS[%s][%d]"),strServerIp,nServerPort);
        //             return MSV_FAILED;
        //         }
        // 
        //         CString strSend(MXL_HEADER);
        // 
        //         strSend +=_T("<GetMSVInfo>");
        //         strSend +=_T("</GetMSVInfo>");
        //         long lRet = m_s.SendCmdString(strSend);
        //         if(lRet ==0|| lRet == SOCKET_ERROR)
        //         {
        //             m_error_desc.Format(_T("SendCmd Faild."));
        //             return MSV_FAILED;
        //         }
        //         Sleep(100);
        //         CString strRecv;
        //         CTime t;
        //         t = CTime::GetCurrentTime();
        //         CString xx = t.Format(_T("hours: %H, mins: %M, secs: %S"));
        //         //AfxMessageBox(xx);
        //         lRet = m_s.ReceiveCmdString2(strRecv);
        //         if(lRet ==0 || lRet == SOCKET_ERROR)
        //         {
        //             m_error_desc.Format(_T("receive Data Error from [%s]"), strServerIp);
        //             return MSV_FAILED;
        //         }
        // 
        //     //解析
        //         CXMLParser parser;
        //         if(FALSE == parser.Create())
        //         {
        //             CString strError = _T("Error occur while analyzing response message of TS[%s]");
        //             m_error_desc.Format(strError, strServerIp);
        //             //NMTrace(m_strLogName, logLevelWarring, m_error_desc);
        //             return MSV_FAILED;
        //         }
        // 
        //         if(FALSE == parser.Parse(strRecv))
        //             return MSV_FAILED;
        //         CString strName,strPara;
        //         parser.GetRoot(strName,strPara);
        //         int iCount = parser.GetNodeCount();
        //         CString strValue;
        //         int nChannelID=-1;
        //         CXMLParser parser2;
        //         if(FALSE == parser2.Create())
        //         {
        //             return MSV_FAILED;
        //         }
        //         MSV_ChannelInfo *pMsvInfoTemp;
        //         if(iCount>0)
        //         {
        //             MSV_ChannelInfo *pArr = new MSV_ChannelInfo[iCount];
        //             for(int j=0;j<iCount;j++)
        //             {
        //                 CString strXml =parser.GetNodeXML(_T("msvInfo"),j);
        //                 if(FALSE==parser2.Parse(strXml))
        //                     continue;
        //                 strValue = parser2.GetNodeText(_T("msvId"));
        //                 nChannelID = _wtoi((LPCTSTR)strValue);
        //                 if(nChannelID<0)
        //                     continue;
        //                 pMsvInfoTemp = &pArr[j];
        // 
        //                 pMsvInfoTemp.nChannelID =nChannelID;
        //                 strValue = parser2.GetNodeText(_T("port"));
        //                 pMsvInfoTemp.dwCtrlPort =_wtoi((LPCTSTR)strValue);
        //                 _tcscpy_s(pMsvInfoTemp.strChannel_IP,_countof(pMsvInfoTemp.strChannel_IP),strServerIp);
        //             }
        //             *pMSVInfo = pArr;
        //         }
        //         nChannelCount = iCount;
        // 
        //         return MSV_SUCCESS;/**/
        //     }
        // 
        //     MSV_RET CClientTaskSDKImp::MSV_GetMSVInfo( const CString &strServerIp,const int nServerPort, std::map<int, MSV_ChannelInfo> & pMSVInfo )
        //     {
        //         CString strLog;
        //         strLog.Format(_T("INFO: MSV_GetMSVInfo( const CString,const int,std::map<int,MSV_ChannelInfo>), NOT USE"));
        //         NMTrace0(SDKLOGNAME, logLevelWarring, (_bstr_t)strLog);
        //         return MSV_SUCCESS;
        // 
        //         /*IRemoteSDK *m_s;
        //         m_s = CreateRemoteSDK();
        //         if(FALSE == SocketConnect(strServerIp,nServerPort,m_s))
        //         {
        //             m_error_desc.Format(_T("Error while conneting TS[%s][%d]"),strServerIp,nServerPort);
        //             return MSV_FAILED;
        //         }
        // 
        //         CString strSend(MXL_HEADER);
        // 
        //         strSend +=_T("<GetMSVInfo>");
        //         strSend +=_T("</GetMSVInfo>");
        //         long lRet = m_s.SendCmdString(strSend);
        //         if(lRet ==0|| lRet == SOCKET_ERROR)
        //         {
        //             m_error_desc.Format(_T("SendCmd Faild."));
        //             return MSV_FAILED;
        //         }
        //         Sleep(100);
        //         CString strRecv;
        //         CTime t;
        //         t = CTime::GetCurrentTime();
        //         CString xx = t.Format(_T("hours: %H, mins: %M, secs: %S"));
        //         //AfxMessageBox(xx);
        //         lRet = m_s.ReceiveCmdString2(strRecv);
        //         if(lRet ==0 || lRet == SOCKET_ERROR)
        //         {
        //             m_error_desc.Format(_T("receive Data Error from [%s]"), strServerIp);
        //             return MSV_FAILED;
        //         }
        // 
        //         //解析
        //         CXMLParser parser;
        //         if(FALSE == parser.Create())
        //         {
        //             CString strError = _T("Error occur while analyzing response message of TS[%s]");
        //             m_error_desc.Format(strError, strServerIp);
        //             //NMTrace(m_strLogName, logLevelWarring, m_error_desc);
        //             return MSV_FAILED;
        //         }
        // 
        //         if(FALSE == parser.Parse(strRecv))
        //             return MSV_FAILED;
        //         CString strName,strPara;
        //         parser.GetRoot(strName,strPara);
        //         int iCount = parser.GetNodeCount();
        //         CString strValue;
        //         int nChannelID=-1;
        //         CXMLParser parser2;
        //         if(FALSE == parser2.Create())
        //         {
        //             return MSV_FAILED;
        //         }
        // 
        //         if(iCount>0)
        //         {
        //             //MSV_ChannelInfo *pArr = new MSV_ChannelInfo[iCount];
        //             for(int j=0;j<iCount;j++)
        //             {
        //                 CString strXml =parser.GetNodeXML(_T("msvInfo"),j);
        //                 if(FALSE==parser2.Parse(strXml))
        //                     continue;
        //                 strValue = parser2.GetNodeText(_T("msvId"));
        //                 nChannelID = _wtoi((LPCTSTR)strValue);
        //                 if(nChannelID<0)
        //                     continue;
        //                 MSV_ChannelInfo msvInfo;
        //                 MSV_ChannelInfo *pMsvInfoTemp;
        //                 pMsvInfoTemp = &msvInfo;
        // 
        //             //	pMsvInfoTemp = &pArr[j];
        // 
        //                 pMsvInfoTemp.nChannelID =nChannelID;
        //                 strValue = parser2.GetNodeText(_T("port"));
        //                 pMsvInfoTemp.dwCtrlPort =_wtoi((LPCTSTR)strValue);
        //                 _tcscpy_s(pMsvInfoTemp.strChannel_IP,_countof(pMsvInfoTemp.strChannel_IP),strServerIp);
        //                 pMsvInfoTemp.nChannelType =1;
        //                 pMSVInfo[nChannelID] = msvInfo;
        //             }
        //             //*pMSVInfo = pArr;
        //         }
        // 
        // 
        //         return MSV_SUCCESS;/**/
        //     }
        // 
        //     int CClientTaskSDKImp::CommandOperation_KeyFrm(InParam pIn, OutParam& pOut, char* pBuf)
        //     {
        //         int nTry = 0;
        //         CString strItem, strContent;
        //         CString strLog;
        //         long lRet = pIn.m_s.SendCmdString(pIn.strSend);
        //         if (lRet == 0 || lRet == SOCKET_ERROR)
        //         {
        //             CString strError = _T("Failed to send data to MSV[%s], pIn.strSend=%s");
        //             m_error_desc.Format(strError, pIn.strIp, pIn.strSend);
        //             strLog.Format(_T("MSV_NETERROR.%s"), m_error_desc);
        //             NMTrace0(SDKLOGNAME, logLevelWarring, (_bstr_t)strLog);
        //             return MSV_NETERROR;
        //         }
        //         lRet = pIn.m_s.ReceiveCmdString_KeyFrm(pOut.strRecv, pBuf);
        //         if (lRet <= 0 || lRet == SOCKET_ERROR)
        //         {
        //             CString strError = _T("Error while receiving response from MSV[%s], pIn.strSend=%s, pOut.strRecv=%s");
        //             m_error_desc.Format(strError, pIn.strIp, pIn.strSend, pOut.strRecv);
        //             strLog.Format(_T("MSV_NETERROR.%s"), m_error_desc);
        //             NMTrace0(SDKLOGNAME, logLevelWarring, (_bstr_t)strLog);
        //             return MSV_NETERROR;
        //         }
        //         if (FALSE == pIn.parser.Parse(pOut.strRecv))
        //         {
        //             CString strError = _T("Error occur while analyzing response message of MSV[%s], pIn.strSend=%s, pOut.strRecv=%s");
        //             m_error_desc.Format(strError, pIn.strIp, pIn.strSend, pOut.strRecv);
        //             strLog.Format(_T("MSV_XMLERROR.%s"), m_error_desc);
        //             NMTrace0(SDKLOGNAME, logLevelWarring, (_bstr_t)strLog);
        //             return MSV_XMLERROR;
        //         }
        //         pIn.parser.GetRoot(strItem, strContent);
        //         if (_T("std_reply") != strItem)
        //         {
        //             CString strError = _T("Error answer info,send: %s, pIn.strSend=%s, pOut.strRecv=%s");
        //             m_error_desc.Format(strError, pIn.strIp, pIn.strSend, pOut.strRecv);
        //             strLog.Format(_T("MSV_XMLERROR.%s"), m_error_desc);
        //             NMTrace0(SDKLOGNAME, logLevelWarring, (_bstr_t)strLog);
        //             return MSV_XMLERROR;
        //         }
        //         strContent = pIn.parser.GetNodeText(_T("s_origin"));
        //         if (pIn.strOrigin != strContent)
        //         {
        //             CString strError = _T("Error occur while analyzing response message of MSV[%s], pIn.strSend=%s, pOut.strRecv=%s");
        //             m_error_desc.Format(strError, pIn.strIp, pIn.strSend, pOut.strRecv);
        //             strLog.Format(_T("MSV_XMLERROR.%s"), m_error_desc);
        //             NMTrace0(SDKLOGNAME, logLevelWarring, (_bstr_t)strLog);
        //             return MSV_XMLERROR;
        //         }
        //         strContent = pIn.parser.GetNodeText(_T("s_result"));
        //         if (_T("succeed") != strContent)
        //         {
        //             CString strError;
        //             strError = pIn.parser.GetNodeText(_T("s_error_string"));
        //             m_error_desc = strError;
        //             strLog.Format(_T("MSV_FAILED.%s, pIn.strSend=%s, pOut.strRecv=%s"), m_error_desc, pIn.strSend, pOut.strRecv);
        //             NMTrace0(SDKLOGNAME, logLevelWarring, (_bstr_t)strLog);
        //             return MSV_FAILED;
        //         }
        //         return MSV_SUCCESS;
        //     }
        //     MSV_RET CClientTaskSDKImp::MSV_GetMSV_RestartTime_fromMSV(LPCTSTR strMsvIp, time_t & dwRestartTickCount, time_t & dwEndRestartTickCount )
        //     {
        //         CString strLog;
        //         strLog.Format(_T("INFO: MSV_GetMSV_RestartTime_fromMSV(), NOT USE"));
        //         NMTrace0(SDKLOGNAME, logLevelWarring, (_bstr_t)strLog);
        // 
        //         //IPTS的已经不用该函数，直接返失败。
        //         return MSV_FAILED;
        // 
        //         /*IRemoteSDK * m_s = CreateRemoteSDK();
        // 
        //         CString strLog;	
        //         strLog.Format(_T("Begin MSV_GetMSV_RestartTime2(%s:%d)."),strMsvIp,m_iCtrlPort);
        //         NMTrace0(SDKLOGNAME,logLevelInfo,strLog);
        // 
        //         if(FALSE == SocketConnect(strMsvIp,m_s))
        //         {
        //             m_error_desc.Format(_T("Error while conneting TS[%s][%d]"),strMsvIp,m_iCtrlPort);
        //             strLog.Format(_T("MSV_FAILED.%s"),m_error_desc);
        //             NMTrace0(SDKLOGNAME,logLevelWarring,(_bstr_t)strLog);
        //             return MSV_FAILED;
        //         }
        //         CString strSend(MXL_HEADER);
        //         strSend.Format(_T("<get_msv_restartTime></get_msv_restartTime>"));
        //         CXMLParser parser;
        //         if(FALSE== parser.Create())
        //         {
        //             CString strError = _T("Error occur while analyzing response message of MSV[%s]");
        //             m_error_desc.Format(strError, strMsvIp);
        // 
        //             strLog.Format(_T("MSV_FAILED.%s"),m_error_desc);
        //             NMTrace0(SDKLOGNAME,logLevelWarring,(_bstr_t)strLog);
        //             return MSV_FAILED;
        //         }
        //         long lRet = m_s.SendCmdString(strSend);
        //         if(lRet==0 || lRet == SOCKET_ERROR)
        //         {
        //             m_error_desc.Format(_T("SendCmd faild"));
        //             strLog.Format(_T("MSV_FAILED.%s"),m_error_desc);
        //             NMTrace0(SDKLOGNAME,logLevelWarring,(_bstr_t)strLog);
        //             return MSV_FAILED;
        //         }
        //         Sleep(100);
        //         CString strRecv;
        //         lRet = m_s.ReceiveCmdString(strRecv);
        //         if(lRet==0 || lRet == SOCKET_ERROR)
        //         {
        //             m_error_desc.Format(_T("lRet==0 || lRet == SOCKET_ERROR receive Data Error from [%s]"), strMsvIp);
        //             strLog.Format(_T("MSV_FAILED.%s"),m_error_desc);
        //             NMTrace0(SDKLOGNAME,logLevelWarring,(_bstr_t)strLog);
        //             return MSV_FAILED;
        //         }
        // 
        //         if(FALSE==parser.Parse(strRecv)){
        //             m_error_desc.Format(_T("MSV_FAILED:FALSE==parser.Parse(strRecv).%s"),strRecv);
        //             strLog.Format(_T("MSV_FAILED.%s"),m_error_desc);
        //             NMTrace0(SDKLOGNAME,logLevelWarring,(_bstr_t)strLog);
        //             return MSV_FAILED;
        //         }
        // 
        //         CString strItem,strContent;
        //         parser.GetRoot(strItem,strContent);
        //         if( _T("std_reply") != strItem)
        //         {
        //             m_error_desc.Format(_T("MSV_FAILED. _T(std_reply) != strItem"));
        //             strLog.Format(_T("MSV_FAILED.%s"),m_error_desc);
        //             NMTrace0(SDKLOGNAME,logLevelWarring,(_bstr_t)strLog);
        //             return MSV_XMLERROR;
        //         }
        //         strContent = parser.GetNodeText(_T("s_origin"));
        //         if(_T("get_msv_restartTime")!=strContent){
        //             m_error_desc.Format(_T("MSV_FAILED._T(Relocate)!=strContent"));
        //             strLog.Format(_T("MSV_FAILED.%s"),m_error_desc);
        //             NMTrace0(SDKLOGNAME,logLevelWarring,(_bstr_t)strLog);
        //             return MSV_XMLERROR;
        //         }
        //         strContent = parser.GetNodeText(_T("s_result"));
        //         if(_T("succeed") != strContent)
        //         {
        //             m_error_desc = parser.GetNodeText(_T("s_error_string"));
        //             strLog.Format(_T("MSV_FAILED.%s"),m_error_desc);
        //             NMTrace0(SDKLOGNAME,logLevelWarring,(_bstr_t)strLog);
        //             return MSV_XMLERROR;
        //         }
        // 
        //         strContent = parser.GetNodeText(_T("RestartTickCount"));
        //         dwRestartTickCount = _wtoi64((LPCTSTR)strContent);
        //         dwEndRestartTickCount = dwRestartTickCount + 60*10;
        // 
        //         struct tm Begintime;
        //         _localtime64_s(&Begintime, &dwRestartTickCount ); 
        //         struct tm Endtime;
        //         _localtime64_s(&Endtime, &dwEndRestartTickCount );
        //         TCHAR buff[50];
        //         _wasctime_s(buff, 50, &Begintime);
        //         TCHAR buff2[50];
        //         _wasctime_s(buff2, 50, &Endtime);
        // 
        //         strLog.Format(_T("MSV_GetMSV_RestartTime_fromMSV(%s:%d) success.dwRestartTickCount=%I64d (%s),dwEndRestartTickCount=%I64d (%s)"),
        //             strMsvIp,m_iCtrlPort,
        //             dwRestartTickCount,
        //             buff,
        //             dwEndRestartTickCount,
        //             buff2);
        //         NMTrace0(SDKLOGNAME,logLevelInfo,strLog);	
        //         return MSV_SUCCESS;/**/
        //     }
        // 
        //     MSV_RET CClientTaskSDKImp::MSV_GetMSV_RestartTime(LPCTSTR strTSIp,const int nTSPort, time_t & dwRestartTickCount, time_t & dwEndRestartTickCount )
        //     {
        //         //IPTS的已经不用该函数，直接返失败。
        //         return MSV_FAILED;
        // 
        //         if (MSV_SUCCESS != MSV_GetMSV_RestartTime_fromMSV(strTSIp, dwRestartTickCount, dwEndRestartTickCount))
        //         {
        //             return MSV_GetMSV_RestartTime_fromCHS(strTSIp, nTSPort, dwRestartTickCount, dwEndRestartTickCount);
        //         }
        //         return MSV_SUCCESS;
        //     }
        //     MSV_RET CClientTaskSDKImp::MSV_GetMSV_RestartTime_fromCHS(LPCTSTR strTSIp,const int nTSPort, time_t & dwRestartTickCount, time_t & dwEndRestartTickCount )
        //     {
        //         //IPTS的已经不用该函数，直接返失败。
        //         return MSV_FAILED;
        // 
        //         IRemoteSDK* m_s = CreateRemoteSDK();
        // 
        //         CString strLog;
        //         strLog.Format(_T("Begin MSV_GetMSV_RestartTime(%s:%d)."), strTSIp, nTSPort);
        //         NMTrace0(SDKLOGNAME, logLevelInfo, strLog);
        // 
        //         if (FALSE == SocketConnect(strTSIp, nTSPort, m_s))
        //         {
        //             m_error_desc.Format(_T("Error while conneting TS[%s][%d]"), strTSIp, nTSPort);
        //             strLog.Format(_T("MSV_FAILED.%s"), m_error_desc);
        //             NMTrace0(SDKLOGNAME, logLevelWarring, (_bstr_t)strLog);
        //             return MSV_FAILED;
        //         }
        //         CString strSend(MXL_HEADER);
        //         strSend.Format(_T("<get_msv_restartTime></get_msv_restartTime>"));
        //         CXMLParser parser;
        //         if (FALSE == parser.Create())
        //         {
        //             CString strError = _T("Error occur while analyzing response message of MSV[%s]");
        //             m_error_desc.Format(strError, strTSIp);
        // 
        //             strLog.Format(_T("MSV_FAILED.%s"), m_error_desc);
        //             NMTrace0(SDKLOGNAME, logLevelWarring, (_bstr_t)strLog);
        //             return MSV_FAILED;
        //         }
        //         long lRet = m_s.SendCmdString(strSend);
        //         if (lRet == 0 || lRet == SOCKET_ERROR)
        //         {
        //             m_error_desc.Format(_T("SendCmd faild"));
        //             strLog.Format(_T("MSV_FAILED.%s"), m_error_desc);
        //             NMTrace0(SDKLOGNAME, logLevelWarring, (_bstr_t)strLog);
        //             return MSV_FAILED;
        //         }
        //         //Sleep(100);
        //         CString strRecv;
        //         lRet = m_s.ReceiveCmdString2(strRecv);
        //         if (lRet == 0 || lRet == SOCKET_ERROR)
        //         {
        //             m_error_desc.Format(_T("lRet==0 || lRet == SOCKET_ERROR receive Data Error from [%s]"), strTSIp);
        //             strLog.Format(_T("MSV_FAILED.%s"), m_error_desc);
        //             NMTrace0(SDKLOGNAME, logLevelWarring, (_bstr_t)strLog);
        //             return MSV_FAILED;
        //         }
        // 
        //         if (FALSE == parser.Parse(strRecv))
        //         {
        //             m_error_desc.Format(_T("MSV_FAILED:FALSE==parser.Parse(strRecv).%s"), strRecv);
        //             strLog.Format(_T("MSV_FAILED.%s"), m_error_desc);
        //             NMTrace0(SDKLOGNAME, logLevelWarring, (_bstr_t)strLog);
        //             return MSV_FAILED;
        //         }
        // 
        //         CString strItem, strContent;
        //         parser.GetRoot(strItem, strContent);
        //         if (_T("std_reply") != strItem)
        //         {
        //             m_error_desc.Format(_T("MSV_FAILED. _T(std_reply) != strItem"));
        //             strLog.Format(_T("MSV_FAILED.%s"), m_error_desc);
        //             NMTrace0(SDKLOGNAME, logLevelWarring, (_bstr_t)strLog);
        //             return MSV_XMLERROR;
        //         }
        //         strContent = parser.GetNodeText(_T("s_origin"));
        //         if (_T("get_msv_restartTime") != strContent)
        //         {
        //             m_error_desc.Format(_T("MSV_FAILED._T(Relocate)!=strContent"));
        //             strLog.Format(_T("MSV_FAILED.%s"), m_error_desc);
        //             NMTrace0(SDKLOGNAME, logLevelWarring, (_bstr_t)strLog);
        //             return MSV_XMLERROR;
        //         }
        //         strContent = parser.GetNodeText(_T("s_result"));
        //         if (_T("succeed") != strContent)
        //         {
        //             m_error_desc = parser.GetNodeText(_T("s_error_string"));
        //             strLog.Format(_T("MSV_FAILED.%s"), m_error_desc);
        //             NMTrace0(SDKLOGNAME, logLevelWarring, (_bstr_t)strLog);
        //             return MSV_XMLERROR;
        //         }
        // 
        //         strContent = parser.GetNodeText(_T("RestartTickCount"));
        //         dwRestartTickCount = _wtoi64((LPCTSTR)strContent);
        //         dwEndRestartTickCount = dwRestartTickCount + 60 * 10;
        // 
        // 
        //     struct tm Begintime;
        // 
        //     _localtime64_s(&Begintime, &dwRestartTickCount );
        //     struct tm Endtime;
        // 
        //     _localtime64_s(&Endtime, &dwEndRestartTickCount );
        //     TCHAR buff[50];
        // 
        //     _wasctime_s(buff, 50, &Begintime);
        //     TCHAR buff2[50];
        // 
        //     _wasctime_s(buff2, 50, &Endtime);
        // 
        //     strLog.Format(_T("MSV_GetMSV_RestartTime_fromCHS(%s:%d) success.dwRestartTickCount=%I64d (%s),dwEndRestartTickCount=%I64d (%s)"),
        // 		strTSIp,nTSPort,
        // 		dwRestartTickCount,
        // 		buff,
        // 		dwEndRestartTickCount,
        // 		buff2);
        // 
        //     NMTrace0(SDKLOGNAME, logLevelInfo, strLog);	
        // 	return MSV_SUCCESS;
        // }
        // 
        // 
        // //该接口暂时未用
        // MSV_RET CClientTaskSDKImp::MSV_Relocate(long taskID, LPCTSTR strTSIp,const int nTSPort, LPCTSTR lpStrTargetIP, int nPort, LPCTSTR lpStrLocalIP, int &nAnalyzeID )
        // {
        //     IRemoteSDK* m_s;
        //     m_s = CreateRemoteSDK();
        //     CString strLog;
        //     strLog.Format(_T("MSV_Relocate  (taskid=%d LPCTSTR strTSIp=%s,const int nTSPort=%d, LPCTSTR lpStrTargetIP=%s,int nPort=%d,LPCTSTR lpStrLocalIP=%s,int &nAnalyzeID =%d)"),
        //         taskID, strTSIp, nTSPort, lpStrTargetIP, nPort, lpStrLocalIP, nAnalyzeID);
        //     NMTrace0(SDKLOGNAME, logLevelWarring, strLog);
        //     if (FALSE == SocketConnect(strTSIp, nTSPort, m_s))
        //     {
        //         m_error_desc.Format(_T("Error while conneting TS[%s][%d]"), strTSIp, nTSPort);
        //         strLog.Format(_T("MSV_FAILED.%s"), m_error_desc);
        //         NMTrace0(SDKLOGNAME, logLevelWarring, (_bstr_t)strLog);
        //         return MSV_FAILED;
        //     }
        //     CString strSend(MXL_HEADER);
        //     strSend.Format(_T("<Relocate><Port>%d</Port><TargetIP>%s</TargetIP><LocalIP>%s</LocalIP><PgmID>%d</PgmID><taskID>%d</taskID></Relocate>"), nPort, lpStrTargetIP, lpStrLocalIP, nAnalyzeID, taskID);
        //     CXMLParser parser;
        //     if (FALSE == parser.Create())
        //     {
        //         CString strError = _T("Error occur while analyzing response message of MSV[%s]");
        //         m_error_desc.Format(strError, strTSIp);
        //         //NMTrace(m_strLogName, logLevelWarring, m_error_desc);
        //         strLog.Format(_T("MSV_FAILED.%s"), m_error_desc);
        //         NMTrace0(SDKLOGNAME, logLevelWarring, (_bstr_t)strLog);
        //         return MSV_FAILED;
        //     }
        //     long lRet = m_s.SendCmdString(strSend);
        //     if (lRet == 0 || lRet == SOCKET_ERROR)
        //     {
        //         m_error_desc.Format(_T("SendCmd faild"));
        //         strLog.Format(_T("MSV_FAILED.%s"), m_error_desc);
        //         NMTrace0(SDKLOGNAME, logLevelWarring, (_bstr_t)strLog);
        //         return MSV_FAILED;
        //     }
        //     //Sleep(100);
        //     CString strRecv;
        // 
        //     lRet = m_s.ReceiveCmdString2(strRecv);
        //     if (lRet == 0 || lRet == SOCKET_ERROR)
        //     {
        //         m_error_desc.Format(_T("lRet==0 || lRet == SOCKET_ERROR receive Data Error from [%s]"), strTSIp);
        //         strLog.Format(_T("MSV_FAILED.%s"), m_error_desc);
        //         NMTrace0(SDKLOGNAME, logLevelWarring, (_bstr_t)strLog);
        //         return MSV_FAILED;
        //     }
        // 
        //     if (FALSE == parser.Parse(strRecv))
        //     {
        //         m_error_desc.Format(_T("MSV_FAILED:FALSE==parser.Parse(strRecv).%s"), strRecv);
        //         strLog.Format(_T("MSV_FAILED.%s"), m_error_desc);
        //         NMTrace0(SDKLOGNAME, logLevelWarring, (_bstr_t)strLog);
        //         return MSV_FAILED;
        //     }
        // 
        //     CString strItem, strContent;
        //     parser.GetRoot(strItem, strContent);
        //     if (_T("std_reply") != strItem)
        //     {
        //         m_error_desc.Format(_T("MSV_FAILED. _T(std_reply) != strItem"));
        //         strLog.Format(_T("MSV_FAILED.%s"), m_error_desc);
        //         NMTrace0(SDKLOGNAME, logLevelWarring, (_bstr_t)strLog);
        //         return MSV_XMLERROR;
        //     }
        //     strContent = parser.GetNodeText(_T("s_origin"));
        //     if (_T("Relocate") != strContent)
        //     {
        //         m_error_desc.Format(_T("MSV_FAILED._T(Relocate)!=strContent"));
        //         strLog.Format(_T("MSV_FAILED.%s"), m_error_desc);
        //         NMTrace0(SDKLOGNAME, logLevelWarring, (_bstr_t)strLog);
        //         return MSV_XMLERROR;
        //     }
        //     strContent = parser.GetNodeText(_T("s_result"));
        //     if (_T("succeed") != strContent)
        //     {
        //         m_error_desc = parser.GetNodeText(_T("s_error_string"));
        //         strLog.Format(_T("MSV_FAILED.%s"), m_error_desc);
        //         NMTrace0(SDKLOGNAME, logLevelWarring, (_bstr_t)strLog);
        //         return MSV_XMLERROR;
        //     }
        // 
        //     strContent = parser.GetNodeText(_T("AnalyzeID"));
        //     nAnalyzeID = _wtoi((LPCTSTR)strContent);
        //     return MSV_SUCCESS;
        // }
        // 
        // MSV_RET CClientTaskSDKImp::MSV_Relocate(long taskID, LPCTSTR strMsvIp, LPCTSTR lpStrTargetIP, int nPort, LPCTSTR lpStrLocalIP, int bUDP, int nPgmID)
        // {
        //     IRemoteSDK* m_s;
        //     m_s = CreateRemoteSDK();
        //     CString strLog;
        //     strLog.Format(_T("MSV_Relocate  (taskID=%d,LPCTSTR strTSIp=%s, LPCTSTR lpStrTargetIP=%s,int nPort=%d,LPCTSTR lpStrLocalIP=%s,bUDP=%d int nPgmID =%d)"),
        //         taskID, strMsvIp, lpStrTargetIP, nPort, lpStrLocalIP, bUDP, nPgmID);
        //     NMTrace0(SDKLOGNAME, logLevelWarring, strLog);
        //     if (FALSE == SocketConnect(strMsvIp, m_s))
        //     {
        //         m_error_desc.Format(_T("Error while conneting TS[%s]"), strMsvIp);
        //         strLog.Format(_T("MSV_FAILED.%s"), m_error_desc);
        //         NMTrace0(SDKLOGNAME, logLevelWarring, (_bstr_t)strLog);
        //         return MSV_FAILED;
        //     }
        //     CString strSend(MXL_HEADER);
        //     strSend.Format(_T("<Relocate><Port>%d</Port><TargetIP>%s</TargetIP><LocalIP>%s</LocalIP><PgmID>%d</PgmID><bUdp>%d</bUdp><taskID>%d</taskID></Relocate>"), nPort, lpStrTargetIP, lpStrLocalIP, nPgmID, bUDP, taskID);
        //     CXMLParser parser;
        //     if (FALSE == parser.Create())
        //     {
        //         CString strError = _T("Error occur while analyzing response message of MSV[%s]");
        //         m_error_desc.Format(strError, strMsvIp);
        //         //NMTrace(m_strLogName, logLevelWarring, m_error_desc);
        //         strLog.Format(_T("MSV_FAILED.%s"), m_error_desc);
        //         NMTrace0(SDKLOGNAME, logLevelWarring, (_bstr_t)strLog);
        //         return MSV_FAILED;
        //     }
        //     long lRet = m_s.SendCmdString(strSend);
        //     if (lRet == 0 || lRet == SOCKET_ERROR)
        //     {
        //         m_error_desc.Format(_T("SendCmd faild"));
        //         strLog.Format(_T("MSV_FAILED.%s"), m_error_desc);
        //         NMTrace0(SDKLOGNAME, logLevelWarring, (_bstr_t)strLog);
        //         return MSV_FAILED;
        //     }
        //     //Sleep(100);
        //     CString strRecv;
        // 
        //     lRet = m_s.ReceiveCmdString(strRecv);
        //     if (lRet == 0 || lRet == SOCKET_ERROR)
        //     {
        //         m_error_desc.Format(_T("lRet==0 || lRet == SOCKET_ERROR receive Data Error from [%s]"), strMsvIp);
        //         strLog.Format(_T("MSV_FAILED.%s"), m_error_desc);
        //         NMTrace0(SDKLOGNAME, logLevelWarring, (_bstr_t)strLog);
        //         return MSV_FAILED;
        //     }
        // 
        //     if (FALSE == parser.Parse(strRecv))
        //     {
        //         m_error_desc.Format(_T("MSV_FAILED:FALSE==parser.Parse(strRecv).[%d][%s]"), strRecv.GetLength(), strRecv);
        //         strLog.Format(_T("MSV_FAILED.%s"), m_error_desc);
        //         NMTrace0(SDKLOGNAME, logLevelWarring, (_bstr_t)strLog);
        //         return MSV_FAILED;
        //     }
        // 
        //     CString strItem, strContent;
        //     parser.GetRoot(strItem, strContent);
        //     if (_T("std_reply") != strItem)
        //     {
        //         m_error_desc.Format(_T("MSV_FAILED. _T(std_reply) != strItem"));
        //         strLog.Format(_T("MSV_FAILED.%s"), m_error_desc);
        //         NMTrace0(SDKLOGNAME, logLevelWarring, (_bstr_t)strLog);
        //         return MSV_XMLERROR;
        //     }
        //     strContent = parser.GetNodeText(_T("s_origin"));
        //     if (_T("Relocate") != strContent)
        //     {
        //         m_error_desc.Format(_T("MSV_FAILED._T(Relocate)!=strContent"));
        //         strLog.Format(_T("MSV_FAILED.%s"), m_error_desc);
        //         NMTrace0(SDKLOGNAME, logLevelWarring, (_bstr_t)strLog);
        //         return MSV_XMLERROR;
        //     }
        //     strContent = parser.GetNodeText(_T("s_result"));
        //     if (_T("succeed") != strContent)
        //     {
        //         m_error_desc = parser.GetNodeText(_T("s_error_string"));
        //         strLog.Format(_T("MSV_FAILED.%s"), m_error_desc);
        //         NMTrace0(SDKLOGNAME, logLevelWarring, (_bstr_t)strLog);
        //         return MSV_XMLERROR;
        //     }
        // 
        //     return MSV_SUCCESS;
        // }


    }
}
