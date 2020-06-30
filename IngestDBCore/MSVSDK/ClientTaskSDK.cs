using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IngestDBCore.MSVSDK
{
    public class CClientTaskSDK
    {
        public CClientTaskSDKImp m_pClientTaskSDKImp;
        public CClientTaskSDK(int nTimeOut, int iPort,int nComType)
        {
           m_pClientTaskSDKImp = new CClientTaskSDKImp(nTimeOut, iPort, nComType);
        }
        public CClientTaskSDK(string ip, int iPort = 3000, int nTimeOut = 5000, int nComType = 0)
        {
            m_pClientTaskSDKImp = new CClientTaskSDKImp(ip, iPort, nTimeOut, nComType);
        }
        ~CClientTaskSDK()
        {

        }

        /************************************************************************/
        /* 返回错误描述                                                        */
        /************************************************************************/
        public string MSVGetLastErrorString()
        {
            return m_pClientTaskSDKImp.MSVGetLastErrorString();
        }

        public MSV_RET MSVSetControlMode(string strMsvIp, bool bCtrl, int nChannel)
        {
            return m_pClientTaskSDKImp.MSVSetControlMode(strMsvIp, bCtrl, nChannel);
        }
        /*********************************************************************************
        查询媒体服务器状态
        **********************************************************************************/
        public MSV_RET MSVQueryState(string strMsvIp, ref MSV_STATE state, int nChannel)
        {
            return m_pClientTaskSDKImp.MSVQueryState(strMsvIp, ref state, nChannel);
        }

        /*********************************************************************************
        切换媒体服务器状态
        **********************************************************************************/
        public MSV_RET MSVSwitchState(string strMsvIp, WORK_MODE mode, int nChannel)
        {
            return m_pClientTaskSDKImp.MSVSwitchState(strMsvIp, mode, nChannel);
        }
        /********************************************************************************
            描述 :获得任务队列状态
        ********************************************************************************/
        public MSV_RET MSVGetTaskListState(string strMsvIp, ref BATCH_STATE state, int nChannel)
        {
            return m_pClientTaskSDKImp.MSVGetTaskListState(strMsvIp, ref state, nChannel);
        }
        /************************************************************************/
        /* 停止媒体服务器任务队列                                               */
        /************************************************************************/
        public MSV_RET MSVStopTaskList(string strMsvIp, int nChannel)
        {
            return m_pClientTaskSDKImp.MSVStopTaskList(strMsvIp, nChannel);
        }
        /*********************************************************************************
            描述: 向媒体服务器设置任务（编单）
        *********************************************************************************/
        public MSV_RET MSVSetTask(string strMsvIp, TASK_ALL_PARAM param, int nChannel)
        {
            return m_pClientTaskSDKImp.MSVSetTask(strMsvIp, param, nChannel);
        }

        public MSV_RET MSVSetTaskNew(string strMsvIp, TASK_ALL_PARAM_NEW param, int nChannel)
        {
            return m_pClientTaskSDKImp.MSVSetTaskNew(strMsvIp, param, nChannel);
        }
        /**********************************************************************************
            描述: 删除任务
        ***********************************************************************************/
        public MSV_RET MSVDelTask(string strMsvIp,long taskID,int nChannel)
        {
            return m_pClientTaskSDKImp.MSVDelTask(strMsvIp, taskID, nChannel);
        }
        /**********************************************************************************
            描述: 修改任务
        ***********************************************************************************/
        public MSV_RET MSVModifyTask(string strMsvIp,TASK_ALL_PARAM param,int nChannel)
        {
            return m_pClientTaskSDKImp.MSVModifyTask(strMsvIp, param, nChannel);
        }
        public MSV_RET MSVModifyTaskNew(string strMsvIp, TASK_ALL_PARAM_NEW param, int nChannel)
        {
            return m_pClientTaskSDKImp.MSVModifyTaskNew(strMsvIp, param, nChannel);
        }
        /**********************************************************************************
            描述: 启动一条手动任务
        ***********************************************************************************/
        public MSV_RET MSVStartTask(string strMsvIp, TASK_ALL_PARAM param, int nChannel)
        {
            return m_pClientTaskSDKImp.MSVStartTask(strMsvIp, param, nChannel);
        }
        public MSV_RET MSVStartTaskNew(string strMsvIp, TASK_ALL_PARAM_NEW param,int nChannel)
        {
            return m_pClientTaskSDKImp.MSVStartTaskNew(strMsvIp, param, nChannel);
        }
        /**********************************************************************************
            描述: 停止一条手动任务  taskID 返回停止的任务的ID
        ***********************************************************************************/
        public MSV_RET MSVStopTask(string strMsvIp, ref long taskID, long lSendTaskID, int nChannel)
        {
            return m_pClientTaskSDKImp.MSVStopTask(strMsvIp, ref taskID, lSendTaskID, nChannel);
        }

        /**********************************************************************************
            描述: 查询手动任务  返回任务的常规参数
        ***********************************************************************************/
        public MSV_RET MSVQueryManauTask(string strMsvIp, ref TASK_PARAM taskParam,int nChannel)
        {
            return m_pClientTaskSDKImp.MSVQueryManauTask(strMsvIp, ref taskParam, nChannel);
        }

        /**********************************************************************************
            描述: 查询当前运行任务  返回任务的常规参数
        ***********************************************************************************/
        public MSV_RET MSVQueryRuningTask(string strMsvIp,ref TASK_PARAM taskParam, int nChannel)
        {
            return m_pClientTaskSDKImp.MSVQueryRuningTask(strMsvIp, ref taskParam, nChannel);
        }

        /************************************************************************/
        /* 查询任务状态                                                         */
        /************************************************************************/
        public MSV_RET MSVQueryTaskState(string strMsvIp,long taskID, ref TASK_STATE state, int nChannel)
        {
            return m_pClientTaskSDKImp.MSVQueryTaskState(strMsvIp, taskID, ref state, nChannel);
        }

        /********************************************************************************
            得到所有任务
        ********************************************************************************/
        //         MSV_RET MSVGetAllITask(string strMsvIp,ref List<long> taskList,int nChannel)
        //         {
        //             return m_pClientTaskSDKImp.MSVGetAllITask(strMsvIp, taskList, nChannel);
        //         }

        /********************************************************************************
            删除所有编单任务
        ********************************************************************************/
        //         MSV_RET MSVDelAllTask(string strMsvIp, int nChannel)
        //         {
        //             return m_pClientTaskSDKImp.MSVDelAllTask(strMsvIp, nChannel);
        //         }
        /**********************************************************************************
        描述: 查询任务的参数 
        ***********************************************************************************/
        //         MSV_RET MSVGetTaskParam(string strMsvIp,long taskID, ref TASK_ALL_PARAM& param, int nChannel)
        //         {
        //             return m_pClientTaskSDKImp.MSVGetTaskParam(strMsvIp, taskID, param, nChannel);
        // 
        //         }

        //注册通知消息
        //         MSV_RET RegisterNotify(string strMsvIp, uint uiNotifyID, int nChannel)
        //         {
        //             return m_pClientTaskSDKImp.RegisterNotify(strMsvIp, uiNotifyID, nChannel);
        //         }
        //反注册通知消息
        //         MSV_RET UnregisterNotify(string strMsvIp, uint uiNotifyID, string strPreServerIp, int nChannel)
        //         {
        //             return m_pClientTaskSDKImp.UnregisterNotify(strMsvIp, uiNotifyID, strPreServerIp, nChannel);
        //         }
        //反注册全部通知

        //         MSV_RET UnregisterAllNotify(string strMsvIp, string strPreServerIp, int nChannel)
        //         {
        //             return m_pClientTaskSDKImp.UnregisterAllNotify(strMsvIp, strPreServerIp, nChannel);
        //         }

        /************************************************************************
        设置本地任务采集参数  
        ************************************************************************/
        //         MSV_RET MSVSetLocalCaptureParam(string strMsvIp, TASK_ALL_PARAM param, int nChannel)
        //         {
        //             return m_pClientTaskSDKImp.MSVSetLocalCaptureParam(strMsvIp, param, nChannel);
        //         }
        //         MSV_RET MSVSetLocalCaptureParamNew(string strMsvIp,TASK_ALL_PARAM_NEW param, int nChannel)
        //         {
        //             return m_pClientTaskSDKImp.MSVSetLocalCaptureParamNew(strMsvIp, param, nChannel);
        //         }
        //         MSV_RET MSVGetLocalCaptureParam(string strMsvIp, ref string strTaskName,ref int ncutLen, ref int lCatalogID, ref CAPRTUR_PARAM param, int nChannel)
        //         {
        //             return m_pClientTaskSDKImp.MSVGetLocalCaptureParam(strMsvIp, strTaskName, ncutLen,lCatalogID, param, nChannel);
        //         }

        /************************************************************************/
        /* 得到MSV得硬盘表                                                      */
        /************************************************************************/
        //         MSV_RET MSVGetDriverString(string strMsvIp, ref List<string> driverList, int nChannel)
        //         {
        //             return m_pClientTaskSDKImp.MSVGetDriverString(strMsvIp, driverList, nChannel);
        //         }

        //查询备份数据
        //         MSV_RET MSVQueryBackUpCount(string  strMsvIp, ref int nCount, int nChannel)
        //         {
        //             return m_pClientTaskSDKImp.MSVQueryBackUpCount(strMsvIp, nCount, nChannel);
        //         }
        //         MSV_RET MSVQueryBackUpDesc(string strMsvIp, int nIndex, ref string strDesc, int nChannel)
        //         {
        //             return m_pClientTaskSDKImp.MSVQueryBackUpDesc(strMsvIp, nIndex, strDesc, nChannel);
        //         }

        //恢复备份
        //         MSV_RET MSVStartRecover(string strMsvIp, bool bDeleteBackup, int nChannel)
        //         {
        //             return m_pClientTaskSDKImp.MSVStartRecover(strMsvIp, bDeleteBackup, nChannel);
        //         }
        //停止恢复
        //         MSV_RET MSVStopRecover(string strMsvIp, int nChannel)
        //         {
        //             return m_pClientTaskSDKImp.MSVStopRecover(strMsvIp, nChannel);
        //         }
        //查询已恢复数据
        //         MSV_RET MSVQueryRecoveredCount(string  strMsvIp, ref int nCount, int nChannel)
        //         {
        //             return m_pClientTaskSDKImp.MSVQueryRecoveredCount(strMsvIp, nCount, nChannel);
        // 
        //         }
        //         MSV_RET MSVQueryRecoveredInfo(string  strMsvIp, int nIndex, ref BACKUP_INFO info, int nChannel)
        //         {
        //             return m_pClientTaskSDKImp.MSVQueryRecoveredInfo(strMsvIp, nIndex, info, nChannel);
        //         }

        //         MSV_RET MSVSynchronizationTime(string  strMsvIp, DateTime m_time, int nChannel)
        //         {
        //             return m_pClientTaskSDKImp.MSVSynchronizationTime(strMsvIp, m_time, nChannel);
        //         }

        /************************************************************************/
        /* 获取MSV日志文件的数量                                                */
        /************************************************************************/
        //         MSV_RET MSVGetLogFileCount(string strMsvIp, ref int nFileCount, int nChannel)
        //         {
        //             return m_pClientTaskSDKImp.MSVGetLogFileCount(strMsvIp, nFileCount, nChannel);
        //         }
        /************************************************************************/
        /* 获取MSV日志文件名                                                    */
        /************************************************************************/
        //         MSV_RET MSVGetLogFileName(string strMsvIp, int nFileIndex, ref string strFileName,int nChannel)
        //         {
        //             return m_pClientTaskSDKImp.MSVGetLogFileName(strMsvIp, nFileIndex, strFileName, nChannel);
        //         }
        /************************************************************************/
        /* 按文件名获取MSV日志文件内容                                          */
        /************************************************************************/
        //         MSV_RET MSVGetLogFileContent(string  strMsvIp, string  strFileName,bool  bDelete,int nChannel)
        //         {
        //             return m_pClientTaskSDKImp.MSVGetLogFileContent(strMsvIp, strFileName, bDelete, nChannel);
        //         }

        /************************************************************************/
        /* 暂停MSV采集  要取消暂停状态，请调用MSVPauseCapture                    */
        /************************************************************************/
        public MSV_RET MSVPauseCapture(long taskID, string strMsvIp, int nChannel)
        {
            return m_pClientTaskSDKImp.MSVPauseCapture(taskID, strMsvIp, nChannel);
        }
        /************************************************************************/
        /* 继续MSV采集， 取消暂停状态                                          */
        /************************************************************************/
        public MSV_RET MSVContinueCapture(long taskID, string  strMsvIp, int nChannel)
        {
            return m_pClientTaskSDKImp.MSVContinueCapture(taskID, strMsvIp, nChannel);
        }

        /************************************************************************/
        /* MSV是否启用环出                                                       */
        /************************************************************************/
//         MSV_RET MSVSetCycleoutState(string  strMsvIp, bool bState, int nChannel)
//         {
//              return m_pClientTaskSDKImp.MSVSetCycleoutState(strMsvIp, bState, nChannel);
//         }
        /************************************************************************/
        /* 查询目标机器注册的消息                                               */
        /************************************************************************/
//         MSV_RET MSVQueryRegisterMsg(string  strMsvIp, ref List<long> msgList, int nChannel)
//         {
//             return m_pClientTaskSDKImp.MSVQueryRegisterMsg(strMsvIp, msgList, nChannel);
//         }
        /************************************************************************/
        /* 查询本地备份恢复进度                                                 */
        /************************************************************************/
//         MSV_RET MSVQueryLocalRecoverState(string  strMsvIp, ref int nFileIndex, ref int nFrame, int nChannel)
//         {
//              return m_pClientTaskSDKImp.MSVQueryLocalRecoverState(strMsvIp, nFileIndex, nFrame, nChannel);
//         }



        /************************************************************************/
        /* 修改任务名                                                           */
        /************************************************************************/
//         MSV_RET MSVModifyManualTaskName(string  strMsvIp, string strTaskName, int nChannel)
//         {
// 
//             return m_pClientTaskSDKImp.MSVModifyManualTaskName(strMsvIp, strTaskName, nChannel);
//         }
        /************************************************************************/
        /* 修改任务采集时间                                                     */
        /************************************************************************/
//         MSV_RET MSVModifyAutoTaskTime(string strMsvIp, long lTaskID, DateTime newTimeEnd,int nChannel)
//         {
//             return m_pClientTaskSDKImp.MSVModifyAutoTaskTime(strMsvIp, lTaskID, newTimeEnd, nChannel);
//         }
        /************************************************************************/
        /*                                                                      */
        /************************************************************************/
//         MSV_RET MSVSetSignedID(string  strMsvIp, string  strSignID, int nChannel)
//         {
//             return m_pClientTaskSDKImp.MSVSetSignedID(strMsvIp, strSignID, nChannel);
//         }
//         MSV_RET MSVReleaseSignedID(string  strMsvIp, string  releaseID,int nChannel)
//         {
//             return m_pClientTaskSDKImp.MSVReleaseSignedID(strMsvIp, releaseID, nChannel);
//         }

        //设置是否启用存储均衡策略
//         MSV_RET MSVSetStorageBalanceState(string  strMsvIp, bool  bUse, int nChannel)
//         {
//             return m_pClientTaskSDKImp.MSVSetStorageBalanceState(strMsvIp, bUse, nChannel);
//         }
        //设置存储均衡盘列表
//         MSV_RET MSVSetStorageList(string  strMsvIp, ref List<string> storageList, int nChannel)
//         {
//             return m_pClientTaskSDKImp.MSVSetStorageList(strMsvIp, storageList, nChannel);
//         }
        //获取存储均衡策略信息
//         MSV_RET MSVGetStorageBalanceInfo(string  strMsvIp, ref bool bUse, ref List<string> storageList, int nChannel)
//         {
//             return m_pClientTaskSDKImp.MSVGetStorageBalanceInfo(strMsvIp, bUse, storageList, nChannel);
//         }

        /************************************************************************/
        /* 以手动采集参数开始采集                                               */
        /************************************************************************/
//         MSV_RET MsvStartTaskWithLocalParam(string strMsvIp, int nChannel)
//         {
//             return m_pClientTaskSDKImp.MsvStartTaskWithLocalParam(strMsvIp, nChannel);
//         }

//         MSV_RET MsvQueryDiskInfo(string  strMsvIp, string strDisk, ref disk_info info, int nChannel)
//         {
//             return m_pClientTaskSDKImp.MsvQueryDiskInfo(strMsvIp, strDisk, ref info, nChannel);
//         }

        //根据传入得盘符，获得网络状态
        MSV_RET MsvQueryNetDriverStatus(string  strMsvIp,string strDisk, ref bool bStatus, int nChannel)
        {
        return m_pClientTaskSDKImp.MsvQueryNetDriverStatus(strMsvIp, strDisk, ref bStatus, nChannel);
        }

        //按序号恢复素材
        //         MSV_RET MsvRecoverClipByIndex(string  strMsvIp,int nIndex,bool bDelete, int nChannel)
        //         {
        //         return m_pClientTaskSDKImp.MsvRecoverClipByIndex(strMsvIp, nIndex, bDelete, nChannel);
        //         }

        //删除指定备份
        //         MSV_RET MsvDeleteBackUpClipByIndex(string strMsvIp,int nIndex, int nChannel)
        //         {
        //             return m_pClientTaskSDKImp.MsvDeleteBackUpClipByIndex(strMsvIp, nIndex, nChannel);
        //         }
        //为指定的服务端在MSV上注册消息
        //         MSV_RET RegisterNotifyEx(string  strMsvIp, string  strServerIP, uint uiNotifyID, int nChannel)
        //         {
        //              return m_pClientTaskSDKImp.RegisterNotifyEx(strMsvIp, strServerIP, uiNotifyID, nChannel);
        //         }
        //         MSV_RET MSVGetVtrTimeCode(string  strMsvIp, ref int lTimeCode)
        //         {
        //              return m_pClientTaskSDKImp.MSVGetVtrTimeCode(strMsvIp, lTimeCode);
        //         }


        // 发送得到帧索引的命令 by yj
        //         MSV_RET MSVGetFrameIndex(string  strMsvIp, ref ulong dwFrameIndex)
        //         {
        //             return m_pClientTaskSDKImp.MSVGetFrameIndex(strMsvIp, dwFrameIndex);
        //         }

        //返回当前任务采集的帧数
        //         MSV_RET MSVGetFramesInTask(string  strMsvIp, ref ulong dwFrames)
        //         {
        //             return MSV_RET.MSV_FAILED;
        //         }
        //返回当前任务采集的帧数
        //         MSV_RET MSVGetCurCaptureFrame(string  strMsvIp, ref ulong dwFrames)
        //         {
        //             return MSV_RET.MSV_SUCCESS;
        //         }
        //add by kdb on 2007-12-24(all next)

        //         MSV_RET MSVParamLoad(string strMsvIp)
        //         {
        //             return MSV_RET.MSV_SUCCESS;
        //         }
        /************************************************************************/
        /*  获取MSV支持得编码格式                                              */
        /************************************************************************/
        //         MSV_RET MSVGetSupportEncodeType(string  strMsvIp, ref List<EncodeType_st> typeList)
        //         {
        //             return MSV_RET.MSV_SUCCESS;
        //         }
        /************************************************************************/
        /* 获得子编码格式                                                       */
        /************************************************************************/
        //         MSV_RET MSVGetSupportSubEncodeType(string strMsvIp, EncodeType_st encodeType, ref List<EncodeType_st> typeList)
        //         {
        //             return MSV_RET.MSV_SUCCESS;
        //         }
        /************************************************************************/
        /* 获得文件格式                                                         */
        /************************************************************************/
        //         MSV_RET MSVGetSupportFileType(string strMsvIp, EncodeType_st encodeType, EncodeType_st subEncodeType, ref List<File_st> fileType)
        //         {
        //             return MSV_RET.MSV_SUCCESS;
        //         }
        //         MSV_RET MSVGetSupporAudiotFileType(ref List<EncodeType_st> typeList,string strMsvIp, EncodeType_st encodeType,EncodeType_st subEncodeType, File_st fileType)
        //         {
        //             return MSV_RET.MSV_SUCCESS;
        //         }

        //         MSV_RET ManualCutClip(string strMSVIP)
        //         {
        //             return m_pClientTaskSDKImp.ManualCutClip(strMSVIP);
        //         }

        //         MSV_RET SetEssenceMark(string strMSVIP, ref MANUALKEYFRAME KeyFrmParam )
        //         {
        //             return m_pClientTaskSDKImp.SetEssenceMark(strMSVIP, KeyFrmParam);
        //         }

        //         MSV_RET MSVSetAudioChannel(string strMsvIp, int iAudioMask)
        //         {
        //             return m_pClientTaskSDKImp.MSVSetAudioChannel(strMsvIp, iAudioMask);
        //         }

        public MSV_RET MSVMapNetDriver(string  strMsvIp, string  strDriverName, string  strNetPath, string  strUserName, string  strPassWord, int nChannel /*= -1*/ )
        {
            return MSV_RET.MSV_SUCCESS;
        }

        public MSV_RET MSVSetNetPreview(string  strMsvIp, bool bPreview = true , int nChannel = -1 )
        {
            return MSV_RET.MSV_SUCCESS;
        }
        public MSV_RET MSVGetNetPreview(string strMsvIp,int nOperaType, ref int nPort, ref int nState )
        {
            return m_pClientTaskSDKImp.MSG_NetPreview(strMsvIp, nOperaType, ref nPort, ref nState);
        }


        public MSV_RET MSVSetExceptCutClip(string  strMSVIP, bool bEnable)
        {
            return m_pClientTaskSDKImp.MSVSetExceptCutClip(strMSVIP, bEnable);
        }

        public MSV_RET MSVStartTaskList(string  strMsvIp, int nChannel /*= -1*/ )
        {
            return MSV_RET.MSV_SUCCESS;
        }

        public MSV_RET MSVRunRetrospectTask(string  strMsvIP, int taskID, int retrospectcount, int taskpluslength, int nChannel = -1 )
        {
            return MSV_RET.MSV_SUCCESS;
        }

        //         MSV_RET MSVGetFieldFrame(string  strMsvIp, ref long llFrame )
        //         {
        //            return m_pClientTaskSDKImp.MSVGetFieldFrame(strMsvIp, llFrame);
        //         }

        //         MSV_RET MSVGetFieldIntEvt( string strMsvIp, HANDLE & nHandle )
        //         {
        //             return m_pClientTaskSDKImp.MSVGetFieldIntEvt(strMsvIp, nHandle);
        //         }

        //         MSV_RET MSV_ReIngest_ForDR(string strMsvIp, ulong dwDRFileIn, ulong dwDRFileOut, ulong dwRecordFileIn, long dwRecordFileOut, long dwSPCTag, string  pcDRFileName)
        //         {
        //              return m_pClientTaskSDKImp.MSV_ReIngest_ForDR(strMsvIp, dwDRFileIn, dwDRFileOut, dwRecordFileIn, dwRecordFileOut, dwSPCTag, pcDRFileName);
        //         }
        public MSV_RET MSVQueryDBEChannel(string  strMsvIP, ref ulong dwDBEChannel, int nChannel = -1 )
        {
             return m_pClientTaskSDKImp.MSVQueryDBEChannel(strMsvIP, ref dwDBEChannel, nChannel);
        }

        public MSV_RET MSVQuerySDIFormat(string  strMsvIp, ref SDISignalStatus FormatStatus, ref bool bIsBlack, int nChannel /* = -1 */)
        {
             return m_pClientTaskSDKImp.MSVQuerySDIFormat(strMsvIp, ref FormatStatus, ref bIsBlack, nChannel);
        }

        //         MSV_RET MSVModifyTaskLength(string  strMsvIp, ulong  dwTaskID, ulong  dwLength, int nChannel  = -1 )
        //         {
        //              return m_pClientTaskSDKImp.MSVModifyTaskLength(strMsvIp, dwTaskID, dwLength, nChannel);
        //         }

        //         MSV_RET MsvQueryLocalStorage(string  strMsvIP, ref MSV_LocalStorage LocalStorage, int nChannel  = -1 )
        //         {
        //             return m_pClientTaskSDKImp.MsvQueryLocalStorage(strMsvIP, LocalStorage, nChannel);
        //         }

        //         MSV_RET MSV_GetTsInfo(ref string strTSIp,int nTSPort, ref string strTSInfo, int AnalyzeChannelID = -1)
        //         {
        //             return m_pClientTaskSDKImp.MSV_GetTsInfo(strTSIp, nTSPort, strTSInfo, AnalyzeChannelID);
        //         }

        //         MSV_RET MSV_GetTsInfo(ref string strTSIp,int nTSPort, ref TS_DataChannelInfo/*TS_DataChannelInfo***/ pTSDataChannelInfo, ref int nChannelCount, int AnalyzeChannelID = -1 )
        //         {
        //             return m_pClientTaskSDKImp.MSV_GetTsInfo(strTSIp, nTSPort, pTSDataChannelInfo, nChannelCount, AnalyzeChannelID);
        //         }

        //         MSV_RET MSV_GetTsInfo(ref string strTSIp,int nTSPort, ref Dictionary<int, TS_DataChannelInfoEx> pTSDataChannelInfo, int AnalyzeChannelID = -1 )
        //         {
        //         return m_pClientTaskSDKImp.MSV_GetTsInfo(strTSIp, nTSPort, pTSDataChannelInfo, AnalyzeChannelID);
        //         }

        //         MSV_RET MSV_BindTsPgm(string strMsvIp, int nAnalyzeChannelID, int nPgmID, bool bStopBind)
        //         {
        //             return m_pClientTaskSDKImp.MSV_BindTsPgm(strMsvIp, nAnalyzeChannelID, nPgmID, bStopBind);
        //         }

        //         MSV_RET MSV_GetMSVInfo(string strServerIp,int nServerPort, ref MSV_ChannelInfo/*MSV_ChannelInfo***/ pMSVInfo, ref int nChannelCount)
        //         {
        //             return m_pClientTaskSDKImp.MSV_GetMSVInfo(strServerIp, nServerPort, pMSVInfo, nChannelCount);
        //         }

        //         MSV_RET MSV_GetMSVInfo(string strServerIp,int nServerPort, ref Dictionary<int, MSV_ChannelInfo> pMSVInfo )
        //         {
        //             return m_pClientTaskSDKImp.MSV_GetMSVInfo(strServerIp, nServerPort, pMSVInfo);
        //         }

        public MSV_RET MSVStartRetrospectTask(string  strMsvIP, TASK_ALL_PARAM param, int nChannel  = -1 )
        {
           return m_pClientTaskSDKImp.MSVStartRetrospectTask(strMsvIP, param, nChannel);
        }

        public MSV_RET MSVStartRetrospectTask(string  strMsvIP, TASK_ALL_PARAM_NEW param, int nChannel  = -1 )
        {
            return m_pClientTaskSDKImp.MSVStartRetrospectTask(strMsvIP, param, nChannel);
        }

        public MSV_RET MSV_Relocate(long taskID, string  strTSIp,int nTSPort, string lpStrTargetIP, int nPort, string  lpStrLocalIP, ref int nAnalyzeID)
        {
            return m_pClientTaskSDKImp.MSV_Relocate(taskID, strTSIp, nTSPort, lpStrTargetIP, nPort, lpStrLocalIP, ref nAnalyzeID);
        }

        public MSV_RET MSV_RelocateRTMP(string  strMsvIp, int nPort, string  lpStrLocalIP)
        {
             return m_pClientTaskSDKImp.MSV_RelocateRTMP(strMsvIp, nPort, lpStrLocalIP);
        }

        public MSV_RET MSVSetMulDestPath(string  strMSVIP, string  strMulDestPathXML)
        {
            return m_pClientTaskSDKImp.MSVSetMulDestPath(strMSVIP, strMulDestPathXML);
        }

        //         MSV_RET MSV_GetMSV_RestartTime(string  strMSVIP,int nTSPort, time_t & dwRestartTickCount, time_t & dwEndRestartTickCount)
        //         {
        //             return m_pClientTaskSDKImp.MSV_GetMSV_RestartTime(strMSVIP, nTSPort, dwRestartTickCount, dwEndRestartTickCount);
        //         }
        public MSV_RET MSG_NetPreview(string  strMSVIP,int nOperaType, ref int nPort, ref int nState)
        {
             return m_pClientTaskSDKImp.MSG_NetPreview(strMSVIP, nOperaType, ref nPort, ref nState);
        }
   }
}