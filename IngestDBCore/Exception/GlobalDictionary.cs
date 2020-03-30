using System;
using System.Collections.Generic;
using System.Text;

namespace IngestDBCore
{
    /// <summary>
    /// 公共错误信息字典类
    /// </summary>
    /// <remarks>
    /// Add by chenzhi 
    /// 2013-05-15
    /// </remarks>
    public class GlobalDictionary
    {

        #region 内部成员

        /// <summary>
        /// 默认的构造函数
        /// </summary>
        public GlobalDictionary()
        {
            InitDictionary();
        }

        private static Dictionary<int, string> _globalDict = new Dictionary<int, string>();

        private static object _globalDicLock = new object();

        private static GlobalDictionary _instance;

        /// <summary>
        /// 单件实体
        /// </summary>
        public static GlobalDictionary Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_globalDicLock)
                    {
                        if (_instance == null)
                        {
                            _instance = new GlobalDictionary();
                        }
                    }
                }

                return _instance;
            }
        }

        #endregion

        #region 公共方法

        /// <summary>
        /// 通过错误编码获得错误信息
        /// </summary>
        /// <param name="nCode">错误编码</param>
        /// <returns>错误信息</returns>
        public string GetMessageByCode(int nCode)
        {
            string strMessage = "";

            if (_globalDict.TryGetValue(nCode, out strMessage))
            {
                var f= strMessage.Split("@");
                if (f != null && f.Length > 2)
                {
                    return f[1];
                }
            }

            return strMessage;
        }

        #endregion

        #region 请参考已有样式，填入字典文件项目

        /// <summary>
        /// 初始化错误信息字典
        /// </summary>
        /// <remarks>
        /// 需要手动将错误代码和对应的错误信息添加到此处
        /// </remarks>
        private void InitDictionary()
        {
            // 请参考如下样式，添加自定义的异常信息
            try
            {
                _globalDict.Add(GLOBALDICT_CODE_UNKNOWN_ERROR, "一般性未知错误@Unknown error@一般性未知錯誤@");
                _globalDict.Add(GLOBALDICT_CODE_DATABASE_FATAL_ERROR, "致命的数据库错误@Database fatal error@致命的數據庫錯誤@");
                _globalDict.Add(GLOBALDICT_CODE_NO_CAPTURE_PARAM, "没有配置采集参数@No Capture Param@沒有配置採集參數@");
                _globalDict.Add(GLOBALDICT_CODE_THE_NCHANNEL_CANNOT_NEGATIVE, "通道ID不合法@The Channel cannot negative!@通道ID不合法@");
                _globalDict.Add(GLOBALDICT_CODE_NO_USEABLE_CHANNEL_CONFLICT_TASKS_ONEPARAM, "没有可用的通道，相冲突的任务是{0}@No Useable Channel, Conflict Tasks:{0}@沒有可用的通道，相衝突的任務是{0}@");
                _globalDict.Add(GLOBALDICT_CODE_SELECTED_CHANNEL_IS_BUSY, "选择的通道正在执行任务@Selected Channel is busy!@選擇的通道正在執行任務@");
                _globalDict.Add(GLOBALDICT_CODE_ALL_USEABLE_CHANNELS_ARE_BUSY, " 所有可用通道都正在执行任务@ All useable channels are busy!@ 所有可用通道都正在執行任務@");
                _globalDict.Add(GLOBALDICT_CODE_CHANNEL_IS_BUSY_AND_CLASH_TASK_INFO_TWOPARAM, "通道正在执行任务，冲突任务信息：任务名{0}，任务开始时间{1}@This channel is busy! Conflict task information is:[TASKNAME:{0}**STARTTIME:{1}]@通道正在執行任務，衝突任務信息：任務名{0}，任務開始時間{1}@");
                _globalDict.Add(GLOBALDICT_CODE_CAN_NOT_FIND_SIGNAL_SRC_FOR_CHANNEL, "不能为信号源找到可用通道@Can not find signal src for channel@不能為信號源找到可用通道@");
                _globalDict.Add(GLOBALDICT_CODE_CAN_NOT_FIND_THE_END_DATE_IN_CAPTURE_PARAM, "在采集参数中未能找到采集截止时间@Can not find the end date in capture param!@在採集參數中未能找到採集截止時間@");
                _globalDict.Add(GLOBALDICT_CODE_NO_USEABLE_CHANNEL, "没有可用的通道@No Useable Channel.@沒有可用的通道@");
                _globalDict.Add(GLOBALDICT_CODE_SELECTED_CHANNEL_IS_BUSY_OR_CAN_NOT_BE_SUITED_TO_PROGRAMME, "选择的通道不能创建任务@Selected channel is busy@選擇的通道不能創建任務@");
                _globalDict.Add(GLOBALDICT_CODE_NO_BACKUP_SIGNALSRC, "没有设置备份信号源@No Backup Signal Src@沒有設置備份信號源@");
                _globalDict.Add(GLOBALDICT_CODE_NO_MAIN_TASK, "主备信号源任务中，没有主信号源任务信息@No Main Task Information@主備信號源任務中，沒有主信號源任務信息@");
                _globalDict.Add(GLOBALDICT_CODE_NO_SINGAL_SRC, "主备信号源任务中，没有信号源信息@No Singal Src Information@主備信號源任務中，沒有信號源信息@");
                _globalDict.Add(GLOBALDICT_CODE_SINGAL_SRC_IS_WRONG, "主备信号源任务中，信号源信息是错误的@Singal Src is wrong@主備信號源任務中，信號源信息是錯誤的@");
                _globalDict.Add(GLOBALDICT_CODE_SIGNAL_ID_ERROR_NOT_IN_A_UNIT, "信号源不在同一单元中@Signal ID Error(not in a unit)@信號源不在同一單元中@");
                _globalDict.Add(GLOBALDICT_CODE_TASK_END_TIME_IS_SMALLER_THAN_BEING_TIME, "任务结束时间早于开始时间@The end time is earlier than starting time.@任務結束時間早於開始時間@");
                _globalDict.Add(GLOBALDICT_CODE_TASK_TIME_IS_OVER_24_HOURS, "任务信息错误，非周期任务的时长超过24小时@The task's time is over 24 hours@任務信息錯誤，非週期任務的時長超過24小時@");
                _globalDict.Add(GLOBALDICT_CODE_TASK_IS_LOCKED, "任务被锁定@Task Is Locked!@任務被鎖定@");
                _globalDict.Add(GLOBALDICT_CODE_SIGNAL_AND_CHANNEL_IS_MISMATCHED, "信号源和通道不匹配@The signal and channel is mismatched@信號源和通道不匹配@");
                _globalDict.Add(GLOBALDICT_CODE_CAN_NOT_FIND_THE_TASK_ONEPARAM, "找不到ID为{0}的任务@Can not find the task.TaskId = {0}@找不到ID為{0}的任務@");
                _globalDict.Add(GLOBALDICT_CODE_VTR_HAS_BEEN_USED_BY_OTHER_TASKS_ONEPARAM, "{0}的VTR设备已经被其它任务所占用@{0} has been used by other tasks@{0}的VTR設備已經被其它任務所佔用@");
                _globalDict.Add(GLOBALDICT_CODE_CAN_NOT_MODIFY_TIME_CONFLICT_TASKS_ONEPARAM, "不能更改任务时间，冲突任务包括{0}@Can Not Modify Time, Conflict Tasks:{0}@不能更改任務時間，衝突任務包括{0}@");
                _globalDict.Add(GLOBALDICT_CODE_SET_TASKMETADATA_FAIL, "更新任务元数据失败@Set task metadata Fail.@更新任務元數據失敗@");
                _globalDict.Add(GLOBALDICT_CODE_TASK_IS_NOT_A_PERIODIC_TASK, "任务不是一个周期任务@The Task is not a Periodic Task.@任務不是一個週期任務@");
                _globalDict.Add(GLOBALDICT_CODE_NOT_DISPOSE_SEPARATE_PERIODIC_TASK, "不能单独处理已拆分出来的周期任务@Can not process separate periodic task.@不能單獨處理已拆分出來的週期任務@");
                _globalDict.Add(GLOBALDICT_CODE_MODIFY_THE_PERIODIC_TASK_FAIL, "修改周期任务失败@Modify the periodic task fail.@修改週期任務失敗@");
                _globalDict.Add(GLOBALDICT_CODE_MODIFY_TASK_FAIL, "修改任务数据失败@Modify task fail@修改任務數據失敗@");
                _globalDict.Add(GLOBALDICT_CODE_TASK_IS_NOT_MANUAL_TASK, "任务不是一个手动任务@The task is not Manual Task@任務不是一個手動任務@");
                _globalDict.Add(GLOBALDICT_CODE_CAN_NOT_SPLIT_LOOP_PERIODIC_TASK, "不能拆分循环任务、周期任务@Can not split loop or periodic task.@不能拆分循環任務、週期任務@");
                _globalDict.Add(GLOBALDICT_CODE_DO_NOT_ALLOW_TO_SPLIT_THE_TASK, "不能拆分任务：因为任务在5秒内将结束@Do not allow to split the task: The task will end within 5 seconds.@不能拆分任務：因為任務在5秒內將結束@");
                _globalDict.Add(GLOBALDICT_CODE_CAN_NOT_CREATE_NEW_TASK_END_TIME_MUST_LARGER_THAN_START_TIME, "不能创建任务：任务开始时间必须早于结束时间@Can not create new task：Start time must earlier than the end time.@不能創建任務：任務開始時間必須早於結束時間@");
                _globalDict.Add(GLOBALDICT_CODE_MATERIAL_DOES_NOT_EXIST, "素材不存在@Material does not exist.@素材不存在@");
                _globalDict.Add(GLOBALDICT_CODE_FILL_PLANNING_FAILED, "保存计划任务数据失败@Fail to save scheduled task metadata.@保存計劃任務數據失敗@");
                _globalDict.Add(GLOBALDICT_CODE_TASK_ID_DOES_NOT_EXIST, "任务ID不存在@Task ID Does Not Exist@任務ID不存在@");
                _globalDict.Add(GLOBALDICT_CODE_TASK_IS_A_STENCIL_PLATE_TASK_OF_PERIODIC_TASKS_ONEPARAM, "修改任务状态时，该任务是一个周期任务的模板而不能修改，任务ID：{0}@In SetTaskState, Task is a stencil-plate task of periodic tasks, ID:{0}@修改任務狀態時，該任務是一個週期任務的模板而不能修改，任務ID：{0}@");
                _globalDict.Add(GLOBALDICT_CODE_TASK_ID_IS_NOT_AVAILABLE_OR_TASK_IS_LOCKED_ONEPARAM, "任务信息不可用或任务正处理锁定状态不能操作，任务ID：{0}@Task ID is not available Or Task Is Locked! ID:{0}@任務信息不可用或任務正處理鎖定狀態不能操作，任務ID：{0}@");
                _globalDict.Add(GLOBALDICT_CODE_TASK_ID_IS_NOT_AVAILABLE, "任务ID是不可用的@Task ID is not available@任務ID是不可用的@");
                _globalDict.Add(GLOBALDICT_CODE_TASK_IS_NOT_EXSIT_ONEPARAM, "任务ID为{0}的任务并不存在@Task is not exsit: Task ID:{0}@任務ID為{0}的任務並不存在@");
                _globalDict.Add(GLOBALDICT_CODE_IN_STOPMANUTASK_TASK_IS_A_STENCIL_PLATE_TASK_ONEPARAM, "在停止手动任务操作中，任务ID为{0}的任务是周期任务的模板任务@In StopManuTask, Task is a stencil-plate task of periodic tasks, ID:{0}@在停止手動任務操作中，任務ID為{0}的任務是週期任務的模板任務@");
                _globalDict.Add(GLOBALDICT_CODE_IN_STOPCAPTURE_TASK_IS_A_STENCIL_PLATE_TASK_ONEPARAM, "在停止采集操作中，任务ID为{0}的任务是周期任务的模板任务@In StopCapture, Task is a stencil-plate task of periodic tasks, ID:{0}@在停止採集操作中，任務ID為{0}的任務是週期任務的模板任務@");
                _globalDict.Add(GLOBALDICT_CODE_TASK_GUID_IS_NULL, "任务信息中的GUID为空@Task Guid is null@任務信息中的GUID為空@");
                _globalDict.Add(GLOBALDICT_CODE_TASKSET_IS_NULL, "任务数据记录为空@TaskSet is null@任務數據記錄為空@");
                _globalDict.Add(GLOBALDICT_CODE_UPDATE_TASKSOURCE_FAILED, "更新任务信息中的源信息时失败@Update TaskSource Failed@更新任務信息中的源信息時失敗@");
                _globalDict.Add(GLOBALDICT_CODE_UPDATE_TASK_INFORMATION_FAILED, "更新任务信息时失败@Fail to update task information.@更新任務信息時失敗@");
                _globalDict.Add(GLOBALDICT_CODE_SET_TASKSTATE_ERROR, "设置任务状态失败@Fail to set task status.@設置任務狀態失敗@");
                _globalDict.Add(GLOBALDICT_CODE_GET_TASK_METADATA_ERROR, "获取任务元数据时失败@Fail to get task metadata.@獲取任務元數據時失敗@");
                _globalDict.Add(GLOBALDICT_CODE_GET_TASK_METADATA_IN_BACKUP_TABLE_ERROR, "从备份数据表中获取任务元数据失败@Fail to get task metadata from Backup Table.@從備份數據表中獲取任務元數據失敗@");
                _globalDict.Add(GLOBALDICT_CODE_SET_TASK_METADATA_ERROR, "设置任务元数据失败@Fail to set task metadata.@設置任務元數據失敗@");
                _globalDict.Add(GLOBALDICT_CODE_SET_TASK_METADATA_IN_BACKUP_TABLE_ERROR, "在备份数据表中设置任务元数据失败@Fail to set task metadata from Backup Table.@在備份數據表中設置任務元數據失敗@");
                _globalDict.Add(GLOBALDICT_CODE_FILL_GETTASKSBYSQL_EXCEPTION, "在GetTasksBySQL操作中发生异常@Exception: Fill GetTaskBySQL@在GetTasksBySQL操作中發生異常@");
                _globalDict.Add(GLOBALDICT_CODE_FILL_GETCONFLICTTASKS_EXCEPTION, "在GetConflictTasks操作中发生异常@Exception: Fill GetConflictTasks@在GetConflictTasks操作中發生異常@");
                _globalDict.Add(GLOBALDICT_CODE_EXECUTE_COMMAND_ERROR, "执行数据库命令时失败@Execute Command Error@執行數據庫命令時失敗@");
                _globalDict.Add(GLOBALDICT_CODE_GET_TASK_SOURCEMAP_BY_TASKID_ERROR, "在GetTaskSourceMapByTaskID操作中失败@GetTaskSourceMapByTaskID Error@在GetTaskSourceMapByTaskID操作中失敗@");
                _globalDict.Add(GLOBALDICT_CODE_DELETE_TASK_FROM_DB_ERROR, "在数据库中删除任务数据时失败@Fail to delete task data in DataBase.@在數據庫中刪除任務數據時失敗@");
                _globalDict.Add(GLOBALDICT_CODE_STOPTASKFROMFSW_ERROR, "在StopTaskFromFSW操作中发生异常@Exception: Stop TaskFromFSW@在StopTaskFromFSW操作中發生異常@");
                _globalDict.Add(GLOBALDICT_CODE_GETTIEUPTASKBYCHANNELID_ERROR, "在GetTieUpTaskByChannelID操作中发生异常@GetTieUpTaskByChannelID Error@在GetTieUpTaskByChannelID操作中發生異常@");
                _globalDict.Add(GLOBALDICT_CODE_FILL_KAMATAKIFAILSET_EXCEPTION, "在kamatakiFailSet操作中发生异常@Exception: Fill kamatakiFailSet@在kamatakiFailSet操作中發生異常@");
                _globalDict.Add(GLOBALDICT_CODE_FILL_TASKIDSET_EXCEPTION, "从数据集中获取任务信息时失败@Fill taskIDSet Exception@從數據集中獲取任務信息時失敗@");
                _globalDict.Add(GLOBALDICT_CODE_FILL_GETTASKIDBYTASKGUID2_EXCEPTION, "在GetTaskIDByTaskGUID2操作中发生异常@Exception: Fill GetTaskIDByTaskGUID2@在GetTaskIDByTaskGUID2操作中發生異常@");
                _globalDict.Add(GLOBALDICT_CODE_SELECT_COMMAND_FAILD, "执行数据库查询命令失败@Fail to execute SELECT command.@執行數據庫查詢命令失敗@");
                _globalDict.Add(GLOBALDICT_CODE_XDCAM_DEVICE_ID_DOES_NOT_EXIST, "XDCAM 设备ID不存在@XDCAM Device ID does not exist.@XDCAM 設備ID不存在@");
                _globalDict.Add(GLOBALDICT_CODE_XDCAM_TASKID_DOES_NOT_EXIST, "XDCAM 任务ID不存在@XDCAM TaskID does not exist.@XDCAM 任務ID不存在@");
                _globalDict.Add(GLOBALDICT_CODE_XDCAM_TASK_DOES_NOT_EXIST, "XDCAM任务并不存在@XDCAM Task does not exist.@XDCAM任務並不存在@");
                _globalDict.Add(GLOBALDICT_CODE_XDCAM_DISK_DOES_NOT_EXIST, "XDCAM磁盘并不存在@XDCAM Disc does not exist.@XDCAM磁盤並不存在@");
                _globalDict.Add(GLOBALDICT_CODE_XDCAM_DISK_ID_DOES_NOT_EXIST, "传入的XDCAM磁盘ID并不存在@XDCAM Disc ID does not exist.@傳入的XDCAM磁盤ID並不存在@");
                _globalDict.Add(GLOBALDICT_CODE_PROGRESS_IS_INVALID, "传入的进度值是无效的@The progress is invalid@傳入的進度值是無效的@");
                _globalDict.Add(GLOBALDICT_CODE_XDCAM_DISK_DO_NOT_BE_UPLOADING, "XDCAM磁盘不能被上载@Can not upload XDCAM Disc.@XDCAM磁盤不能被上載@");
                _globalDict.Add(GLOBALDICT_CODE_XDCAM_TASK_FILE_ID_DOES_NOT_EXIST, "传入的XDCAM任务文件ID并不存在@XDCAM Task File ID does not exist.@傳入的XDCAM任務文件ID並不存在@");
                _globalDict.Add(GLOBALDICT_CODE_XDCAM_TASK_FILE_DOES_NOT_EXIST, "指定的XDCAM任务文件并不存在@XDCAM Task File does not exist.@指定的XDCAM任務文件並不存在@");
                _globalDict.Add(GLOBALDICT_CODE_USABLE_XDCAM_DISK_DOES_NOT_EXIST, "可以使用的XDCAM磁盘并不存在@No usable XDCAM Disc.@可以使用的XDCAM磁盤並不存在@");
                _globalDict.Add(GLOBALDICT_CODE_DISKES_IS_MORE_THAN_THE_DEVICES, "XDCAM磁盘数超过XDCAM设备数@The count of XDCAM Disc is more than the count of XDCAM Device.@XDCAM磁盤數超過XDCAM設備數@");
                _globalDict.Add(GLOBALDICT_CODE_VTRID_DOES_NOT_EXIST_THE_NVTRID_IS_ONEPARAM, "ID为{0}的VTR设备并不存在！@VTR ID does not exist: VTR ID is {0}.@ID為{0}的VTR設備並不存在！@");
                _globalDict.Add(GLOBALDICT_CODE_INSET_AND_OUTSET_SEQUENCE_OF_THE_NUMBER_DOES_NOT_MATCH, "入出点序列个数不匹配@The number between IN point and OUT point is mismatched.@入出點序列個數不匹配@");
                _globalDict.Add(GLOBALDICT_CODE_IN_SETVTRTAPEMAP_TAPEID_IS_NOT_EXIST_ONEPARAM, "在SetVtrTapeMap操作中，传入的磁带ID{0}并不存在@In SetVtrTapeMap,TapeID is not exist,TapeID = {0}@在SetVtrTapeMap操作中，傳入的磁帶ID{0}並不存在@");
                _globalDict.Add(GLOBALDICT_CODE_IN_SETVTRTAPEMAP2_TAPEID_IS_NOT_EXIST_ONEPARAM, "在SetVtrTapeMap2操作中，传入的磁带ID{0}并不存在@In SetVtrTapeMap2,TapeID is not exist,TapeID = {0}@在SetVtrTapeMap2操作中，傳入的磁帶ID{0}並不存在@");
                _globalDict.Add(GLOBALDICT_CODE_USER_ID_DOES_NOT_EXIST, "传入的用户ID并不存在@User ID does not exist.@傳入的用戶ID並不存在@");
                _globalDict.Add(GLOBALDICT_CODE_USER_PARAM_ID_DOES_NOT_EXIST, "传入的用户参数ID并不存在@User Parameter ID does not exist.@傳入的用戶參數ID並不存在@");
                _globalDict.Add(GLOBALDICT_CODE_USER_PARAM_DOES_NOT_EXIST, "用户参数信息并不存在@User Parameter does not exist.@用戶參數信息並不存在@");
                _globalDict.Add(GLOBALDICT_CODE_THE_USER_TEMPLATE_HAS_EXISTS_ONEPARAM, "名为{0}的用户模板配置信息已经存在@User tempalte is already existed: Title {0}@名為{0}的用戶模板配置信息已經存在@");
                _globalDict.Add(GLOBALDICT_CODE_CAN_NOT_FIND_THE_TEMPLATE_ID_IS_ONEPARAM, "找不到ID为{0}的用户模板配置信息@Can not find the template. ID = {0}@找不到ID為{0}的用戶模板配置信息@");
                _globalDict.Add(GLOBALDICT_CODE_FAIL_TO_GET_NEXT_VALUE_FOR_DBP_SQ_TRANSCODEID, "从DBP_SQ_TRANSCODEID中取下一序列值失败@Faield to get the next value for DBP_SQ_TRANSCODEID.@從DBP_SQ_TRANSCODEID中取下一序列值失敗@");
                _globalDict.Add(GLOBALDICT_CODE_FAIL_TO_FIND_THE_APPOINTED_TRANSCODE, "无法找到指定的转码模板@Failed to find the transcode template.@無法找到指定的轉碼模板@");
                _globalDict.Add(GLOBALDICT_CODE_FAIL_TO_FIND_THE_APPOINTED_ARCHIVETYPE, "无法找到指定的存档类型@Failed to find the Archive type.@無法找到指定的存檔類型@");
                _globalDict.Add(GLOBALDICT_CODE_FAIL_TO_FIND_THE_APPOINTED_TRANSCODEPOLICY, "无法找到指定的转码策略@Failed to find the TransCode Policy.@無法找到指定的轉碼策略@");
                _globalDict.Add(GLOBALDICT_CODE_FAIL_TO_GET_NEXT_VALUE_FOR_DBP_SQ_TRANSFERID, "从DBP_SQ_TRANSFERID中取下一序列值失败@Faield to get the next value for DBP_SQ_TRANSFERID.@從DBP_SQ_TRANSFERID中取下一序列值失敗@");
                _globalDict.Add(GLOBALDICT_CODE_FAIL_TO_FIND_THE_APPOINTED_TRANSFER, "无法找到指定的转发模板@Failed to find the Transfer template.@無法找到指定的轉發模板@");
                _globalDict.Add(GLOBALDICT_CODE_POLICY_DOES_NOT_EXIST, "指定的策略并不存在@Policy is not existed.@指定的策略並不存在@");
                _globalDict.Add(GLOBALDICT_CODE_POLICY_USER_CLASS_ID_DOES_NOT_EXIST, "指定的用户策略级别ID并不存在@Policy ID is not existed.@指定的用戶策略級別ID並不存在@");
                _globalDict.Add(GLOBALDICT_CODE_TASK_POLICY_DOES_NOT_EXIST, "任务的策略信息并不存在@Information of task policy is not existed.@任務的策略信息並不存在@");
                _globalDict.Add(GLOBALDICT_CODE_CONVERT_PARAMS_ERROR_IN_MEDIAROW2MEDIASTRUCT, "在MediaRow2MediaStruct过程中，转换参数失败@Convert Params Error in MediaRow2MediaStruct@在MediaRow2MediaStruct過程中，轉換參數失敗@");
                _globalDict.Add(GLOBALDICT_CODE_FAIL_TO_FIND_THE_APPOINTED_MATERIAL, "无法找到指定的素材@Failed to find the material.@無法找到指定的素材@");
                _globalDict.Add(GLOBALDICT_CODE_MATERIAL_ID_DOES_NOT_EXIST_ONEPARAM, "ID为{0}的素材并不存在@Material ID {0} is not existed.@ID為{0}的素材並不存在@");
                _globalDict.Add(GLOBALDICT_CODE_MATERIAL_POLICY_DOES_NOT_EXIST, "素材的策略信息并不存在@Material policy is not existed.@素材的策略信息並不存在@");
                _globalDict.Add(GLOBALDICT_CODE_FAILED_TO_GET_MATERIAL_BY_TASKID_FOR_POLICYID_TWOPARAM, "通过任务ID{0}和策略ID{1}获取素材失败@Failed to get task ID {0} and policy ID {1}.@通過任務ID{0}和策略ID{1}獲取素材失敗@");
                _globalDict.Add(GLOBALDICT_CODE_IMPORT_TASK_ID_DOES_NOT_EXIST, "引入任务的ID并不存在@Importing task ID is not existed.@引入任務的ID並不存在@");
                _globalDict.Add(GLOBALDICT_CODE_ROUTER_SERVER_ID_DOES_NOT_EXIST, "Router服务器的ID并不存在@Router Server ID is not existed.@Router服務器的ID並不存在@");
                _globalDict.Add(GLOBALDICT_CODE_MESSAGE_ID_DOES_NOT_EXIST, "消息ID并不存在@Message ID is not existed.@消息ID並不存在@");
                _globalDict.Add(GLOBALDICT_CODE_MESSAGE_CTRL_ID_DOES_NOT_EXIST, "消息服务器ID并不存在@Message Ctrl ID is not existed.@消息服務器ID並不存在@");
                _globalDict.Add(GLOBALDICT_CODE_CAPTURE_DEVICE_ID_DOES_NOT_EXIST, "采集设备ID并不存在@Ingest Device ID is not existed.@採集設備ID並不存在@");
                _globalDict.Add(GLOBALDICT_CODE_SIGNAL_RESOURCE_ID_DOES_NOT_EXIST, "信号源ID并不存在@Signal resource ID is not existed.@信號源ID並不存在@");
                _globalDict.Add(GLOBALDICT_CODE_PROGRAM_ID_DOES_NOT_EXIST, "节目ID并不存在@Program ID is not existed.@節目ID並不存在@");
                _globalDict.Add(GLOBALDICT_CODE_SIGNAL_TYPE_ID_DOES_NOT_EXIST, "信号类型ID并不存在@Signal Type ID is not existed.@信號類型ID並不存在@");
                _globalDict.Add(GLOBALDICT_CODE_SIGNAL_TYPE_MAP_DOES_NOT_EXIST, "信号类型与设备类型对应关系并不存在@The mapping between Signal Type and Device Type is not existed.@信號類型與設備類型對應關係並不存在@");
                _globalDict.Add(GLOBALDICT_CODE_SCHEDULER_ID_DOES_NOT_EXIST, "调度ID并不存在@Scheduler ID is not existed.@調度ID並不存在@");
                _globalDict.Add(GLOBALDICT_CODE_UNIT_ID_DOES_NOT_EXIST, "单元ID并不存在@Unit ID is not existed.@單元ID並不存在@");
                _globalDict.Add(GLOBALDICT_CODE_CHANNEL_ID_DOES_NOT_EXIST, "通道ID并不存在@Channel ID is not existed.@通道ID並不存在@");
                _globalDict.Add(GLOBALDICT_CODE_REC_DEVICE_TYPE_ID_DOES_NOT_EXIST, "收录设备类型ID并不存在@Ingest Device Type ID is not existed.@收錄設備類型ID並不存在@");
                _globalDict.Add(GLOBALDICT_CODE_ROUTER_ID_DOES_NOT_EXIST, "路由ID并不存在@Router ID is not existed.@路由ID並不存在@");
                _globalDict.Add(GLOBALDICT_CODE_ROUTER_OUT_MAP_DOES_NOT_EXIST, "路由输出对应关系不存在@The mapping of Router output is not existed.@路由輸出對應關係不存在@");
                _globalDict.Add(GLOBALDICT_CODE_SIGNAL_UNIT_MAP_DOES_NOT_EXIST, "信号单元对应关系不存在@The mapping of Signal unit is not existed.@信號單元對應關係不存在@");
                _globalDict.Add(GLOBALDICT_CODE_CHANNEL_UNIT_MAP_DOES_NOT_EXIST, "通道单元对应关系不存在@The mapping of Channel unit is not existed.@通道單元對應關係不存在@");
                _globalDict.Add(GLOBALDICT_CODE_DEVICE_IP_DOES_NOT_EXIST, "通过IP找不到设备@Can not find Ingest Device through IP address.@通過IP找不到設備@");
                _globalDict.Add(GLOBALDICT_CODE_LOCK_OBJECTID_WRONG, "锁定对象的ID不合法@Locked ObjectID is invalid.@鎖定對象的ID不合法@");
                _globalDict.Add(GLOBALDICT_CODE_LOCK_OBJECT_TPYEID_IS_NOT_EXIST, "锁定对象的类型不正确@Locked Object TypeID is not existed.@鎖定對象的類型不正確@");
                _globalDict.Add(GLOBALDICT_CODE_LOCK_OBJECT_TIMEOUT_IS_WRONG, "锁定对象的时间长度不合法@Locked Object TimeOut is invalid.@鎖定對象的時間長度不合法@");
                _globalDict.Add(GLOBALDICT_CODE_UNLOCK_OBJECTID_IS_WRONG, "解锁对象的ID不合法@Unlocked ObjectID is invalid.@解鎖對象的ID不合法@");
                _globalDict.Add(GLOBALDICT_CODE_UNLOCK_OBJECT_TYPEID_IS_NOT_EXIST, "解锁对象的类型不正确@Unlocked Object TypeID is not existed.@解鎖對象的類型不正確@");
                _globalDict.Add(GLOBALDICT_CODE_IN_ISVTRCOLLIDE_BEGINTIME_IS_WRONG, "检测VTR是否冲突过程中，任务开始时间不合法@Task Start time is invalid while checking VTR conflic.@檢測VTR是否衝突過程中，任務開始時間不合法@");
                _globalDict.Add(GLOBALDICT_CODE_IN_ISVTRCOLLIDE_ENDTIME_IS_WRONG, "检测VTR是否冲突过程中，任务结束时间不合法@Task End time is invalid while checking VTR conflict.@檢測VTR是否衝突過程中，任務結束時間不合法@");
                _globalDict.Add(GLOBALDICT_CODE_IN_ISVTRCOLLIDE_VTRID_IS_WRONG, "检测VTR是否冲突过程中，VTR的ID不合法@VTR ID is invalid while checking VTR conflict.@檢測VTR是否衝突過程中，VTR的ID不合法@");
                _globalDict.Add(GLOBALDICT_CODE_IN_ISVTRCOLLIDE_TASKID_IS_WRONG, "检测VTR是否冲突过程中，任务的ID不合法@Task ID is invalid while checking VTR conflict.@檢測VTR是否衝突過程中，任務的ID不合法@");
                _globalDict.Add(GLOBALDICT_CODE_XDCAM_LOADALLDEVICES_FILL_FAILED, "在获取所有XDCAM设备数据时出错@Failed to get data of all XDCAM Devices.@在獲取所有XDCAM設備數據時出錯@");
                _globalDict.Add(GLOBALDICT_CODE_FILL_XDCAM_DBP_XDCAM_TASK_METADATA_EXCEPTION, "在获取XDCAM任务元数据时出错@Failed to get metadata of XDCAM task.@在獲取XDCAM任務元數據時出錯@");
                _globalDict.Add(GLOBALDICT_CODE_FILL_XDCAM_SETTASKMETADATA_EXCEPTION, "在设置XDCAM任务元数据时出错@Failed to set metadata of XDCAM task.@在設置XDCAM任務元數據時出錯@");
                _globalDict.Add(GLOBALDICT_CODE_FILL_XDCAM_GETXDCAMDISKBYUMID_EXCEPTION, "在GetXDCAMDiskByUMID过程中发生错误@Error in XDCAM GetXDCAMDiskByUMID.@在GetXDCAMDiskByUMID過程中發生錯誤@");
                _globalDict.Add(GLOBALDICT_CODE_FILL_XDCAM_GETALLUSABLEDISK_EXCEPTION, "在GetALLUsableDisk过程中发生错误@Error in XDCAM GetALLUsableDisk.@在GetALLUsableDisk過程中發生錯誤@");
                _globalDict.Add(GLOBALDICT_CODE_FILL_XDCAM_GETXDCAMTASKBYUMID_EXCEPTION, "在GetXDCAMTaskByUMID过程中发生错误@Error in XDCAM GetXDCAMTaskByUMID.@在GetXDCAMTaskByUMID過程中發生錯誤@");
                _globalDict.Add(GLOBALDICT_CODE_FILL_XDCAM_GETXDCAMTASKFILEBYUMID_EXCEPTION, "在GetXDCAMTaskFileByUMID过程中发生错误@Error in XDCAM GetXDCAMTaskFileByUMID.@在GetXDCAMTaskFileByUMID過程中發生錯誤@");
                _globalDict.Add(GLOBALDICT_CODE_FILL_XDCAM_GETXDCAMTASKFILEBYTASKID_EXCEPTION, "在GetXDCAMTaskFileByTaskID过程中发生错误@Error in XDCAM GetXDCAMTaskFileByTaskID.@在GetXDCAMTaskFileByTaskID過程中發生錯誤@");
                _globalDict.Add(GLOBALDICT_CODE_FILL_XDCAM_QUERYTASKFILEINFOBYSTRING_EXCEPTION, "在QueryTaskFileInfoByString过程中发生错误@Error in XDCAM QueryTaskFileInfoByString.@QueryTaskFileInfoByString過程中發生錯誤@");
                _globalDict.Add(GLOBALDICT_CODE_FILL_XDCAM_GETPRIORITYTASKID_EXCEPTION, "在GetpriorityTaskID过程中发生错误@Error in XDCAM GetpriorityTaskID.@在GetpriorityTaskID過程中發生錯誤@");
                _globalDict.Add(GLOBALDICT_CODE_XDCAMSET_IS_NULL, "XDCAM设备信息为空@XDCAM Device information is empty.@XDCAM設備信息為空@");
                _globalDict.Add(GLOBALDICT_CODE_XDCAM_UPDATEDEVICES_CONCURRENCY_EXCEPTION, "更新XDCAM设备信息时，发生数据库并发冲突@Database concurrency conflict happened while updating XDCAM Device information.@更新XDCAM設備信息時，發生數據庫並發衝突@");
                _globalDict.Add(GLOBALDICT_CODE_XDCAM_UPDATEDEVICES_FAILED, "更新XDCAM设备信息失败@Failed to update XDCAM Device information.@更新XDCAM設備信息失敗@");
                _globalDict.Add(GLOBALDICT_CODE_FILL_XDCAM_ADDTASKSOFWHOLEDISC_EXCEPTION, "在AddTasksOfWholeDisc过程中发生错误@Error in XDCAM AddTasksOfWholeDisc.@在AddTasksOfWholeDisc過程中發生錯誤@");
                _globalDict.Add(GLOBALDICT_CODE_FILL_XDCAM_GETCANCELTASKFILENAME_EXCEPTION, "在GetCancelTaskFileName过程中发生错误@Error in XDCAM GetCancelTaskFileName.@在GetCancelTaskFileName過程中發生錯誤@");
                _globalDict.Add(GLOBALDICT_CODE_IN_UPDATEVTRSET_DATASET_IS_NULL, "更新VTR信息时，VTR信息为空@VTR information is empty while updating VTR information.@更新VTR信息時，VTR信息為空@");
                _globalDict.Add(GLOBALDICT_CODE_IN_UPDATEVTRSET_UPDATE_FAILED, "更新VTR信息失败@Failed to update VTR information.@更新VTR信息失敗@");
                _globalDict.Add(GLOBALDICT_CODE_FILL_VTR_LOADVTRINFO_EXCEPTION, "在LoadVtrInfo过程中发生错误@Error in VTR LoadVtrInfo.@在LoadVtrInfo過程中發生錯誤@");
                _globalDict.Add(GLOBALDICT_CODE_FILL_VTR_LOADVTRTASKINOUT_EXCEPTION, "在LoadVtrTaskInOut过程中发生错误@Error in VTR LoadVtrTaskInOut.@在LoadVtrTaskInOut過程中發生錯誤@");
                _globalDict.Add(GLOBALDICT_CODE_FILL_VTR_LOADVTRTAPELIST_EXCEPTION, "在LoadVTRTapeList过程中发生错误@Error in VTR LoadVTRTapeList.@在LoadVTRTapeList過程中發生錯誤@");
                _globalDict.Add(GLOBALDICT_CODE_IN_GETVTRUPLOADTASKS_READ_DATA_FAILED, "在GetVTRUploadTasks过程中，从数据库读取数据失败@Failed to read data from Database while doing GetVTRUploadTasks.@在GetVTRUploadTasks過程中，從數據庫讀取數據失敗@");
                _globalDict.Add(GLOBALDICT_CODE_FILL_VTR_GETVTRUPLOADTASKINFO_EXCEPTION, "在GetVtrUploadTaskInfo过程中发生错误@Error in VTR GetVtrUploadTaskInfo.@在GetVtrUploadTaskInfo過程中發生錯誤@");
                _globalDict.Add(GLOBALDICT_CODE_IN_GETTIMEPERIODSBYSCHEDULEVBUTASKS_READ_DATA_FAILED, "在GetTimePeriodsByScheduleVBUTasks过程中，从数据库读取数据失败@Failed to read data from Database wihle doing GetTimePeriodsByScheduleVBUTasks.@在GetTimePeriodsByScheduleVBUTasks過程中，從數據庫讀取數據失敗@");
                _globalDict.Add(GLOBALDICT_CODE_FILL_VTR_GETVBUTASKSBYSQL_EXCEPTION, "在GetVBUTasksBySQL过程中发生错误@Error in VTR GetVBUTasksBySQL.@在GetVBUTasksBySQL過程中發生錯誤@");
                _globalDict.Add(GLOBALDICT_CODE_FILL_VTR_GETNEEDMANUALEXECUTEVTRUPLOADTASKS_EXCEPTION, "在GetNeedManualExecuteVTRUploadTasks过程中发生错误@Error in VTR GetNeedManualExecuteVTRUploadTasks.@在GetNeedManualExecuteVTRUploadTasks過程中發生錯誤@");
                _globalDict.Add(GLOBALDICT_CODE_FILL_VTR_GETVTRUPLOADTASKINFOFORUPLOAD_EXCEPTION, "在GetVtrUploadTaskInfoForUpload过程中发生错误@Error in VTR GetVtrUploadTaskInfoForUpload.@在GetVtrUploadTaskInfoForUpload過程中發生錯誤@");
                _globalDict.Add(GLOBALDICT_CODE_FILL_VTR_GETVTRTASKSETFORIMPORTCLIP_EXCEPTION, "在GetVtrTaskSetForImportClip过程中发生错误@Error in VTR GetVtrTaskSetForImportClip.@在GetVtrTaskSetForImportClip過程中發生錯誤@");
                _globalDict.Add(GLOBALDICT_CODE_IN_DELETEVTRUPLOADTASK_READ_DATA_FAILED, "在DeleteVtrUploadTask过程中，执行数据库操作失败@Failed to execute Database operation while doing DeleteVtrUploadTask Read Data.@在DeleteVtrUploadTask過程中，執行數據庫操作失敗@");
                _globalDict.Add(GLOBALDICT_CODE_FILL_VTR_DELETEVTRTASKMETADATA_EXCEPTION, "在DeleteVtrTaskMetaData过程中发生错误@Error in  VTR DeleteVtrTaskMetaData.@在DeleteVtrTaskMetaData過程中發生錯誤@");
                _globalDict.Add(GLOBALDICT_CODE_FILL_VTR_GETVTRTASKMETADATA_EXCEPTION, "在GetVtrTaskMetaData过程中发生错误@Error in VTR GetVtrTaskMetaData.@在GetVtrTaskMetaData過程中發生錯誤@");
                _globalDict.Add(GLOBALDICT_CODE_FILL_VTR_GETVTRTASKMETADATABYGUID_EXCEPTION, "在GetVtrTaskMetaDataByGUID过程中发生错误@Error in VTR GetVtrTaskMetaDataByGUID.@在GetVtrTaskMetaDataByGUID過程中發生錯誤@");
                _globalDict.Add(GLOBALDICT_CODE_FILL_VTR_QUERYVTRDOWNLOADTASKBYCONDITION_EXCEPTION, "在QueryVtrDownloadTaskByCondition过程中发生错误@Error in VTR QueryVtrDownloadTaskByCondition.@在QueryVtrDownloadTaskByCondition過程中發生錯誤@");
                _globalDict.Add(GLOBALDICT_CODE_FILL_VTR_LOADVTRDOWNLOADMATERIALLIST_EXCEPTION, "在LoadVTRDownloadMaterialList过程中发生错误@Error in VTR LoadVTRDownloadMaterialList.@在LoadVTRDownloadMaterialList過程中發生錯誤@");
                _globalDict.Add(GLOBALDICT_CODE_FILL_VTR_QUERYVTRDOWNLOADMATERIALBYCONDITION_EXCEPTION, "在QueryVtrDownloadMaterialByCondition过程中发生错误@Error in  VTR QueryVtrDownloadMaterialByCondition.@在QueryVtrDownloadMaterialByCondition過程中發生錯誤@");
                _globalDict.Add(GLOBALDICT_CODE_FILL_VTR_QUERYVTRTASKINFOBYCONDITION_EXCEPTION, "在QueryVtrTaskInfoByCondition过程中发生错误@Error in VTR QueryVtrTaskInfoByCondition.@在QueryVtrTaskInfoByCondition過程中發生錯誤@");
                _globalDict.Add(GLOBALDICT_CODE_FILL_VTR_GETVTRTASKINOUTPOINT_EXCEPTION, "在GetVtrTaskInOutPoint过程中发生错误@Error in VTR GetVtrTaskInOutPoint.@在GetVtrTaskInOutPoint過程中發生錯誤@");
                _globalDict.Add(GLOBALDICT_CODE_FILL_VTR_GETVTRTASKINFOBYID_EXCEPTION, "在GetVtrTaskInfoByID过程中发生错误@Error in  VTR GetVtrTaskInfoByID.@在GetVtrTaskInfoByID過程中發生錯誤@");
                _globalDict.Add(GLOBALDICT_CODE_FILL_VTR_GETVTRUPLOADTASKINFOBYGUID_EXCEPTION, "在GetVtrUploadTaskInfoByGUID过程中发生错误@Error in VTR GetVtrUploadTaskInfoByGUID.@在GetVtrUploadTaskInfoByGUID過程中發生錯誤@");
                _globalDict.Add(GLOBALDICT_CODE_FILL_VTR_LOADVTRTAPEMAP_EXCEPTION, "在LoadVTRTapeMap过程中发生错误@Error in VTR LoadVTRTapeMap.@在LoadVTRTapeMap過程中發生錯誤@");
                _globalDict.Add(GLOBALDICT_CODE_FILL_VTR_GETTAPEINFOBYNAME_EXCEPTION, "在GetTapeInfoByName过程中发生错误@Error in VTR GetTapeInfoByName.@在GetTapeInfoByName過程中發生錯誤@");
                _globalDict.Add(GLOBALDICT_CODE_FILL_VTR_GETALLTAPEINFO_EXCEPTION, "在GetAllTapeInfo过程中发生错误@Error in VTR GetAllTapeInfo.@在GetAllTapeInfo過程中發生錯誤@");
                _globalDict.Add(GLOBALDICT_CODE_IN_UPDATEUSERS_DATASET_IS_NULL, "在更新用户信息时，用户信息为空@User information is empty while updating user information.@在更新用戶信息時，用戶信息為空@");
                _globalDict.Add(GLOBALDICT_CODE_IN_UPDATEUSERS_UPDATE_FAILED, "在更新用户信息时发生错误@Error in updating user information.@在更新用戶信息時發生錯誤@");
                _globalDict.Add(GLOBALDICT_CODE_FILL_USER_GETALLCPPARAMTEMPLATE_EXCEPTION, "在GetAllCPParamTemplate过程中发生错误@Error in User GetAllCPParamTemplate.@在GetAllCPParamTemplate過程中發生錯誤@");
                _globalDict.Add(GLOBALDICT_CODE_FILL_USER_GETDEFAULTPARAMTEMPLATE_EXCEPTION, "在GetDefaultParamTemplate过程中发生错误@Error in  User GetDefaultParamTemplate.@在GetDefaultParamTemplate過程中發生錯誤@");
                _globalDict.Add(GLOBALDICT_CODE_FILL_USER_GETALLUSERPARAMMAP_EXCEPTION, "在GetAllUserParamMap过程中发生错误@Error in User GetAllUserParamMap.@在GetAllUserParamMap過程中發生錯誤@");
                _globalDict.Add(GLOBALDICT_CODE_FILL_USER_GETCPPARAMTEMPLATEIDBYUSERID_EXCEPTION, "在GetCPParamTemplateIDByUserID过程中发生错误@Error in  User GetCPParamTemplateIDByUserID.@在GetCPParamTemplateIDByUserID過程中發生錯誤@");
                _globalDict.Add(GLOBALDICT_CODE_FILL_USER_GETUSERSETTING_EXCEPTION, "在GetUserSetting过程中发生错误@Error in User GetUserSetting.@在GetUserSetting過程中發生錯誤@");
                _globalDict.Add(GLOBALDICT_CODE_FILL_USER_SETUSERSETTING_EXCEPTION, "在SetUserSetting过程中发生错误@Error in User SetUserSetting.@在SetUserSetting過程中發生錯誤@");
                _globalDict.Add(GLOBALDICT_CODE_IN_UPDATEUSERSETTING_DATASET_IS_NULL, "在UpdateUserSetting过程中，更新数据时数据集为空@Database is empty while doing UpdateUserSetting.@在UpdateUserSetting過程中，更新數據時數據集為空@");
                _globalDict.Add(GLOBALDICT_CODE_IN_UPDATEUSERSETTING_UPDATE_FAILED, "在UpdateUserSetting过程中，更新数据时失败@Failed to update data while doing UpdateUserSetting.@在UpdateUserSetting過程中，更新數據時失敗@");
                _globalDict.Add(GLOBALDICT_CODE_FILL_USER_GETUSERPARAMFORDB2_EXCEPTION, "在GetUserParamForDB2过程中发生错误@Error in User GetUserParamForDB2.@在GetUserParamForDB2過程中發生錯誤@");
                _globalDict.Add(GLOBALDICT_CODE_FILL_USER_GETUSERHIDDENCHANNELS_EXCEPTION, "在GetUserHiddenChannels过程中发生错误@Error in User GetUserHiddenChannels.@在GetUserHiddenChannels過程中發生錯誤@");
                _globalDict.Add(GLOBALDICT_CODE_FILL_USER_ADDUSERTEMPLATE_EXCEPTION, "在AddUserTemplate过程中发生错误@Error in User AddUserTemplate.@在AddUserTemplate過程中發生錯誤@");
                _globalDict.Add(GLOBALDICT_CODE_FILL_USER_MODIFYUSERTEMPLATENAME_EXCEPTION, "在ModifyUserTemplateName过程中发生错误@Error in User ModifyUserTemplateName.@在ModifyUserTemplateName過程中發生錯誤@");
                _globalDict.Add(GLOBALDICT_CODE_FILL_USER_CHECKUSERTEMPLATENAME_EXCEPTION, "在CheckUserTemplateName过程中发生错误@Error in User CheckUserTemplateName.@在CheckUserTemplateName過程中發生錯誤@");
                _globalDict.Add(GLOBALDICT_CODE_FILL_USER_GETUSERTEMPLATEBYID_EXCEPTION, "在GetUserTemplateByID过程中发生错误@Error in User GetUserTemplateByID.@在GetUserTemplateByID過程中發生錯誤@");
                _globalDict.Add(GLOBALDICT_CODE_FILL_USER_MODIFYUSERTEMPALTECONTENT_EXCEPTION, "在ModifyUserTempalteContent过程中发生错误@Error in User ModifyUserTempalteContent.@在ModifyUserTempalteContent過程中發生錯誤@");
                _globalDict.Add(GLOBALDICT_CODE_FILL_USER_DELETEUSERTEMPLATE_EXCEPTION, "在DeleteUserTemplate过程中发生错误@Error in User DeleteUserTemplate.@在DeleteUserTemplate過程中發生錯誤@");
                _globalDict.Add(GLOBALDICT_CODE_FILL_USER_GETUSERTEMPLATE_EXCEPTION, "在GetUserTemplate过程中发生错误@Error in User GetUserTemplate.@在GetUserTemplate過程中發生錯誤@");
                _globalDict.Add(GLOBALDICT_CODE_FILL_USER_GETUSERALLTEMPLATES_EXCEPTION, "在GetUserAllTemplates过程中发生错误@Error in User GetUserAllTemplates.@在GetUserAllTemplates過程中發生錯誤@");
                _globalDict.Add(GLOBALDICT_CODE_FILL_UPVTR_GETVTRUPLOADTASKINFOFORUPLOAD_EXCEPTION, "在GetVtrUploadTaskInfoForUpload过程中发生错误@Error in UpVTR GetVtrUploadTaskInfoForUpload.@在GetVtrUploadTaskInfoForUpload過程中發生錯誤@");
                _globalDict.Add(GLOBALDICT_CODE_FILL_UPVTR_GETVTRTASKSETFORIMPORTCLIP_EXCEPTION, "在GetVtrTaskSetForImportClip过程中发生错误@Error in UpVTR GetVtrTaskSetForImportClip.@在GetVtrTaskSetForImportClip過程中發生錯誤@");
                _globalDict.Add(GLOBALDICT_CODE_FILL_UPVTR_LOADUPVTRTAPEMAP_EXCEPTION, "在LoadUPVTRTapeMap过程中发生错误@Error in UpVTR LoadUPVTRTapeMap.@在LoadUPVTRTapeMap過程中發生錯誤@");
                _globalDict.Add(GLOBALDICT_CODE_IN_UPDATETASKS_UPDATE_FAILED, "更新任务信息时失败@Failed to update task information.@更新任務信息時失敗@");
                _globalDict.Add(GLOBALDICT_CODE_GET_ORACLE_SEQUENCE_FAILED, "从Oracle数据库中获取序列值失败@Failed to get sequence from Oracle Database.@從Oracle數據庫中獲取序列值失敗@");
                _globalDict.Add(GLOBALDICT_CODE_GET_SQLSERVER_SEQUENCE_FAILED, "从SQLServer数据库中获取序列值失败@Failed to get sequence from SQL Server Database.@從SQLServer數據庫中獲取序列值失敗@");
                _globalDict.Add(GLOBALDICT_CODE_GET_DB2_SEQUENCE_FAILED, "从DB2数据库中获取序列值失败@Failed to get sequence from DB2 Database.@從DB2數據庫中獲取序列值失敗@");
                _globalDict.Add(GLOBALDICT_CODE_FILL_MATERIAL_SELECTMATERIAL_EXCEPTION, "在SelectMaterial过程中发生错误@Error in Material SelectMaterial.@在SelectMaterial過程中發生錯誤@");
                _globalDict.Add(GLOBALDICT_CODE_FILL_MATERIAL_SELECTMATERIALVIDEOANDAUDIO_EXCEPTION, "在SelectMaterialVideoAndAudio过程中发生错误@Error in Material SelectMaterialVideoAndAudio.@在SelectMaterialVideoAndAudio過程中發生錯誤@");
                _globalDict.Add(GLOBALDICT_CODE_FILL_POLICY_LOADPOLICIES_EXCEPTION, "在LoadPolicies过程中发生错误@Error in Policy LoadPolicies.@在LoadPolicies過程中發生錯誤@");
                _globalDict.Add(GLOBALDICT_CODE_IN_UPDATEPOLICIES_DATASET_IS_NULL, "更新策略信息时，策略信息为空@Policy is empty while updating policy.@更新策略信息時，策略信息為空@");
                _globalDict.Add(GLOBALDICT_CODE_IN_UPDATEPOLICIES_CONCURRENCY_EXCEPTION, "更新策略信息时，发生数据库并发冲突@Database concurrency while updating policy.@更新策略信息時，發生數據庫並發衝突@");
                _globalDict.Add(GLOBALDICT_CODE_IN_UPDATEPOLICIES_UPDATE_FAILED, "更新策略信息时，操作数据库失败@Failed to operate Database while updating policy.@更新策略信息時，操作數據庫失敗@");
                _globalDict.Add(GLOBALDICT_CODE_FILL_POLICY_FINDINPOLICYTASKBYID_EXCEPTION, "在FindInPolicyTaskByID过程中发生错误@Error in Policy FindInPolicyTaskByID.@在FindInPolicyTaskByID過程中發生錯誤@");
                _globalDict.Add(GLOBALDICT_CODE_FILL_POLICY_FINDINPOLICYUSERBYID_EXCEPTION, "在FindInPolicyUserByID过程中发生错误@Error in Policy FindInPolicyUserByID.@在FindInPolicyUserByID過程中發生錯誤@");
                _globalDict.Add(GLOBALDICT_CODE_FILL_POLICY_FINDINPOLICYCLASSBYID_EXCEPTION, "在FindInPolicyClassByID过程中发生错误@Error in Policy FindInPolicyClassByID.@在FindInPolicyClassByID過程中發生錯誤@");
                _globalDict.Add(GLOBALDICT_CODE_FILL_MATERIAL_LOADMATERIALTYPES_EXCEPTION, "在LoadMaterialTypes过程中发生错误@Error in  Material LoadMaterialTypes.@在LoadMaterialTypes過程中發生錯誤@");
                _globalDict.Add(GLOBALDICT_CODE_FILL_MATERIAL_GETFAILMETADATA_EXCEPTION, "在GetFailMetadata过程中发生错误@Error in Material GetFailMetadata.@在GetFailMetadata過程中發生錯誤@");
                _globalDict.Add(GLOBALDICT_CODE_FILL_MATERIAL_CHANGEARCHIVEANDCLIPSTATE_EXCEPTION, "在ChangeArchiveAndClipState过程中发生错误@Error in Material ChangeArchiveAndClipState.@在ChangeArchiveAndClipState過程中發生錯誤@");
                _globalDict.Add(GLOBALDICT_CODE_FILL_MATERIAL_GETMATERIALNEEDARCHIVE_EXCEPTION, "在GetMaterialNeedArchive过程中发生错误@Error in Material GetMaterialNeedArchive.@在GetMaterialNeedArchive過程中發生錯誤@");
                _globalDict.Add(GLOBALDICT_CODE_FILL_MATERIAL_GETXDCAMMATERIALNEEDARCHIVE_EXCEPTION, "在GetXDCAMMaterialNeedArchive过程中发生错误@Error in Material GetXDCAMMaterialNeedArchive.@在GetXDCAMMaterialNeedArchive過程中發生錯誤@");
                _globalDict.Add(GLOBALDICT_CODE_FILL_MATERIAL_GETMATERIALARCHIVEINFOFORUPLOAD_EXCEPTION, "在GetMaterialArchiveInfoForUpload过程中发生错误@Error in Material GetMaterialArchiveInfoForUpload.@在GetMaterialArchiveInfoForUpload過程中發生錯誤@");
                _globalDict.Add(GLOBALDICT_CODE_FILL_MATERIAL_GETMATERIALARCHIVEINFO_EXCEPTION, "在GetMaterialArchiveInfo过程中发生错误@Error in Material GetMaterialArchiveInfo.@在GetMaterialArchiveInfo過程中發生錯誤@");
                _globalDict.Add(GLOBALDICT_CODE_IN_UPDATEMATERIALS_DATASET_IS_NULL, "更新素材信息时，素材信息为空@Material information is empty while updating material information.@更新素材信息時，素材信息為空@");
                _globalDict.Add(GLOBALDICT_CODE_IN_UPDATEMATERIALS_UPDATE_FAILED, "更新素材信息时，操作数据库失败@Failed to operate Database while updating material information.@更新素材信息時，操作數據庫失敗@");
                _globalDict.Add(GLOBALDICT_CODE_IN_UPDATEMATERIALSINDB2_UPDATE_FAILED, "在UpdateMaterialsInDB2过程中，操作数据库失败@Failed to operate Database while doing  UpdateMaterialsInDB2.@在UpdateMaterialsInDB2過程中，操作數據庫失敗@");
                _globalDict.Add(GLOBALDICT_CODE_FILL_MATERIAL_UPDATEMATERIALSINDB2_EXCEPTION, "在UpdateMaterialsInDB2过程中发生错误@Error in Material UpdateMaterialsInDB2.@在UpdateMaterialsInDB2過程中發生錯誤@");
                _globalDict.Add(GLOBALDICT_CODE_FILL_MATERIAL_GETMATERIALARCHIVE_EXCEPTION, "在GetMaterialArchive过程中发生错误@Error in Material GetMaterialArchive.@在GetMaterialArchive過程中發生錯誤@");
                _globalDict.Add(GLOBALDICT_CODE_FILL_MATERIAL_GETMATERIALARCHIVEBYTASKID_EXCEPTION, "在GetMaterialArchiveByTaskID过程中发生错误@Error in Material GetMaterialArchiveByTaskID.@在GetMaterialArchiveByTaskID過程中發生錯誤@");
                _globalDict.Add(GLOBALDICT_CODE_FILL_MATERIAL_GETMATERIALVIDEO_EXCEPTION, "在GetMaterialVideo过程中发生错误@Erroin in Material GetMaterialVideo.@在GetMaterialVideo過程中發生錯誤@");
                _globalDict.Add(GLOBALDICT_CODE_FILL_MATERIAL_GETMATERIALAUDIO_EXCEPTION, "在GetMaterialAudio过程中发生错误@Error in Material GetMaterialAudio.@在GetMaterialAudio過程中發生錯誤@");
                _globalDict.Add(GLOBALDICT_CODE_FILL_MATERIAL_GETMATERIALVIDEOINBACKUP_EXCEPTION, "在GetMaterialVideoInBackup过程中发生错误@Error in Material GetMaterialVideoInBackup.@在GetMaterialVideoInBackup過程中發生錯誤@");
                _globalDict.Add(GLOBALDICT_CODE_FILL_MATERIAL_GETMATERIALAUDIOINBACKUP_EXCEPTION, "在GetMaterialAudioInBackup过程中发生错误@Error in Material GetMaterialAudioInBackup.@在GetMaterialAudioInBackup過程中發生錯誤@");
                _globalDict.Add(GLOBALDICT_CODE_FILL_MATERIAL_GETMATERIAL_EXCEPTION, "在GetMaterial过程中发生错误@Error in Material GetMaterial.@在GetMaterial過程中發生錯誤@");
                _globalDict.Add(GLOBALDICT_CODE_FILL_MATERIAL_GETMATERIALBYTASKID_EXCEPTION, "在GetMaterialByTaskID过程中发生错误@Error in Material GetMaterialByTaskID.@在GetMaterialByTaskID過程中發生錯誤@");
                _globalDict.Add(GLOBALDICT_CODE_FILL_MATERIAL_GETMATERIALIDBYTASK_EXCEPTION, "在GetMaterialIDByTask过程中发生错误@Error in Material GetMaterialIDByTask.@在GetMaterialIDByTask過程中發生錯誤@");
                _globalDict.Add(GLOBALDICT_CODE_FILL_MATERIAL_GETMATERIALINBACKUPBYTASKID_EXCEPTION, "在GetMaterialInBackupByTaskID过程中发生错误@Error in Material GetMaterialInBackupByTaskID.@在GetMaterialInBackupByTaskID過程中發生錯誤@");
                _globalDict.Add(GLOBALDICT_CODE_FILL_MATERIAL_GETMATERIALDURATION_EXCEPTION, "在GetMaterialDuration过程中发生错误@Error in Material GetMaterialDuration.@在GetMaterialDuration過程中發生錯誤@");
                _globalDict.Add(GLOBALDICT_CODE_FILL_MATERIAL_GETMATERIALDURATIONBYID_EXCEPTION, "在GetMaterialDurationByID过程中发生错误@Error in Material GetMaterialDurationByID.@在GetMaterialDurationByID過程中發生錯誤@");
                _globalDict.Add(GLOBALDICT_CODE_FILL_MATERIAL_GETTASKDURATION_EXCEPTION, "在GetTaskDuration过程中发生错误@Error in Material GetTaskDuration.@在GetTaskDuration過程中發生錯誤@");
                _globalDict.Add(GLOBALDICT_CODE_FILL_MATERIAL_MODIFYMATERIALARCHIVESTATEINDB2_EXCEPTION, "在ModifyMaterialArchiveStateInDb2过程中发生错误@Error in Material ModifyMaterialArchiveStateInDb2.@在ModifyMaterialArchiveStateInDb2過程中發生錯誤@");
                _globalDict.Add(GLOBALDICT_CODE_FILL_MATERIAL_MODIFYMATERIALCLIPSTATEINDB2_EXCEPTION, "在ModifyMaterialClipStateInDb2过程中发生错误@Error in Material ModifyMaterialClipStateInDb2.@在ModifyMaterialClipStateInDb2過程中發生錯誤@");
                _globalDict.Add(GLOBALDICT_CODE_FILL_MATERIAL_UPDATESAVEINDBSTATEINDB2_EXCEPTION, "在UpdateSaveInDBStateInDb2过程中发生错误@Error in Material UpdateSaveInDBStateInDb2.@在UpdateSaveInDBStateInDb2過程中發生錯誤@");
                _globalDict.Add(GLOBALDICT_CODE_FILL_MATERIAL_GETLASTSECTION_EXCEPTION, "在GetLastSection过程中发生错误@Error in Material GetLastSection.@在GetLastSection過程中發生錯誤@");
                _globalDict.Add(GLOBALDICT_CODE_FILL_MATERIAL_MODIFYARCHIVESTATE_EXCEPTION, "在ModifyArchiveState过程中发生错误@Error in Material ModifyArchiveState.@在ModifyArchiveState過程中發生錯誤@");
                _globalDict.Add(GLOBALDICT_CODE_FILL_MATERIAL_DELETEAUDIOVIDEOBYMATERIALID_EXCEPTION, "在DeleteAudioVideoByMaterialID过程中发生错误@Error in Material DeleteAudioVideoByMaterialID.@在DeleteAudioVideoByMaterialID過程中發生錯誤@");
                _globalDict.Add(GLOBALDICT_CODE_FILL_MATERIAL_QUERYARCHIVEANDCLIPSTATEBYMATERIALID_EXCEPTION, "在QueryArchiveAndClipStateByMaterialID过程中发生错误@Error in Material QueryArchiveAndClipStateByMaterialID.@在QueryArchiveAndClipStateByMaterialID過程中發生錯誤@");
                _globalDict.Add(GLOBALDICT_CODE_FILL_MATERIAL_GETMATERIALLISTFOLDERS_EXCEPTION, "在GetMaterialListFolders过程中发生错误@Error in Material GetMaterialListFolders.@在GetMaterialListFolders過程中發生錯誤@");
                _globalDict.Add(GLOBALDICT_CODE_FILL_MATERIAL_GETMLFOLDERBYID_EXCEPTION, "在GetMLFolderByID过程中发生错误@Error in Material GetMLFolderByID.@在GetMLFolderByID過程中發生錯誤@");
                _globalDict.Add(GLOBALDICT_CODE_FILL_MATERIAL_GETNOTPROCESSMSGS_EXCEPTION, "在GetNotProcessMsgs过程中发生错误@Error in Material GetNotProcessMsgs.@在GetNotProcessMsgs過程中發生錯誤@");
                _globalDict.Add(GLOBALDICT_CODE_FILL_MATERIAL_GETNEEDRETRYFAILEDMSG_EXCEPTION, "在GetNeedRetryFailedMsg过程中发生错误@Error in Material GetNeedRetryFailedMsg.@在GetNeedRetryFailedMsg過程中發生錯誤@");
                _globalDict.Add(GLOBALDICT_CODE_FILL_DEVICE_LOADALLDEVICES_EXCEPTION, "在LoadAllDevices过程中发生错误@Error in Device LoadAllDevices.@在LoadAllDevices過程中發生錯誤@");
                _globalDict.Add(GLOBALDICT_CODE_IN_UPDATEDEVICES_DATASET_IS_NULL, "更新设备信息时，设备信息为空@Device information is empty while updating Device information.@更新設備信息時，設備信息為空@");
                _globalDict.Add(GLOBALDICT_CODE_IN_UPDATEDEVICES_CONCURRENCY_EXCEPTION, "更新设备信息时，发生数据库并发冲突@Database concurrency while updating Device information.@更新設備信息時，發生數據庫並發衝突@");
                _globalDict.Add(GLOBALDICT_CODE_IN_UPDATEDEVICES_UPDATE_FAILED, "更新设备信息时，操作数据库失败@Failed to operate Database while updating Device information.@更新設備信息時，操作數據庫失敗@");
                _globalDict.Add(GLOBALDICT_CODE_EXECUTE_READER_FAILED, "执行数据库Reader失败@Failed to execute Reader in Database.@執行數據庫Reader失敗@");
                _globalDict.Add(GLOBALDICT_CODE_IN_GETMSVCHANNELSTATEBYIDARRY_READ_DATA_FAILED, "在GetMSVChannelStateByIDArry过程中，读取数据失败@Failed to read Data while doing GetMSVChannelStateByIDArry.@在GetMSVChannelStateByIDArry過程中，讀取數據失敗@");
                _globalDict.Add(GLOBALDICT_CODE_IN_GETMSVCHANNELSTATE_READ_DATA_FAILED, "在GetMSVChannelState过程中，读取数据失败@Failed to read Data while doing GetMSVChannelState.@在GetMSVChannelState過程中，讀取數據失敗@");
                _globalDict.Add(GLOBALDICT_CODE_FILL_DEVICE_GETUSABLEUPLOADCHANNELLIST_EXCEPTION, "在GetUsableUploadChannelList过程中发生错误@Error in Device GetUsableUploadChannelList.@在GetUsableUploadChannelList過程中發生錯誤@");
                _globalDict.Add(GLOBALDICT_CODE_IN_GETUSABLEUPLOADCHANNELLIST_READ_DATA_FAILED, "在GetUsableUploadChannelList过程中，读取数据失败@Failed to read Data while doing GetUsableUploadChannelList Read.@在GetUsableUploadChannelList過程中，讀取數據失敗@");
                _globalDict.Add(GLOBALDICT_CODE_IN_GETUPLOADMODE_READ_DATA_FAILED, "在GetUploadMode过程中，读取数据失败@Failed to read Data while doing GetUploadMode.@在GetUploadMode過程中，讀取數據失敗@");
                _globalDict.Add(GLOBALDICT_CODE_IN_GETUPLOADCHANNNELLIST_FILL_DATASET_EXCEPTION, "在GetUploadChannnelList过程中，填充数据集失败@Failed to fill Data while doing GetUploadChannnelList.@在GetUploadChannnelList過程中，填充數據集失敗@");
                _globalDict.Add(GLOBALDICT_CODE_IN_GETCHANNELSIGNALSRC_EXECUTE_SCALAR_FAILED, "在GetChannelSignalSrc过程中，执行数据库操作失败@Failed to operate Database while doing GetChannelSignalSrc.@在GetChannelSignalSrc過程中，執行數據庫操作失敗@");
                _globalDict.Add(GLOBALDICT_CODE_IN_GETALLCHANNELSTATE_READ_DATA_FAILED, "在GetAllChannelState过程中，读取数据失败@Failed to read Data while doing GetAllChannelState.@在GetAllChannelState過程中，讀取數據失敗@");
                _globalDict.Add(GLOBALDICT_CODE_IN_GETPARAMTYPEBYCHANNLEID_READ_DATA_FAILED, "在GetParamTypeByChannleID过程中，读取数据失败@Failed to read Data while doing GetParamTypeByChannleID.@在GetParamTypeByChannleID過程中，讀取數據失敗@");
                _globalDict.Add(GLOBALDICT_CODE_IN_GETPARAMTYPEBYINPORT_READ_DATA_FAILED, "在GetParamTypeByInport过程中，读取数据失败@Failed to read Data while doing GetParamTypeByInport.@在GetParamTypeByInport過程中，讀取數據失敗@");
                _globalDict.Add(GLOBALDICT_CODE_IN_GETALLSIGNALGROUPINFO_READ_DATA_FAILED, "在GetAllSignalGroupInfo过程中，读取数据失败@Failed to read Data while doing GetAllSignalGroupInfo.@在GetAllSignalGroupInfo過程中，讀取數據失敗@");
                _globalDict.Add(GLOBALDICT_CODE_IN_GETALLSIGNALGROUP_READ_DATA_FAILED, "在GetAllSignalGroup过程中，读取数据失败@Failed to read Data while doing GetAllSignalGroup.@在GetAllSignalGroup過程中，讀取數據失敗@");
                _globalDict.Add(GLOBALDICT_CODE_IN_GETALLGPIDEVICES_READ_DATA_FAILED, "在GetAllGPIDevices过程中，读取数据失败@Failed to read Data while doing GetAllGPIDevices.@在GetAllGPIDevices過程中，讀取數據失敗@");
                _globalDict.Add(GLOBALDICT_CODE_IN_GETGPIMAPINFOBYGPIID_READ_DATA_FAILED, "在GetGPIMapInfoByGPIID过程中，读取数据失败@Failed to read Data while doing GetGPIMapInfoByGPIID.@在GetGPIMapInfoByGPIID過程中，讀取數據失敗@");
                _globalDict.Add(GLOBALDICT_CODE_IN_GETALLCHANNEL2SIGNALSRCMAP_READ_DATA_FAILED, "在GetAllChannel2SignalSrcMap过程中，读取数据失败@Failed to read Data while doing GetAllChannel2SignalSrcMap.@在GetAllChannel2SignalSrcMap過程中，讀取數據失敗@");
                _globalDict.Add(GLOBALDICT_CODE_GETVALUESTRING_EXCEPTION, "从数据库中取值时失败@Executing GetValueString failed.@從數據庫中取值時失敗@");
                _globalDict.Add(GLOBALDICT_CODE_SETVALUE_EXCEPTION, "在数据库中更新数值时失败@Failed to update value in DataBase.@在數據庫中更新數值時失敗@");
                _globalDict.Add(GLOBALDICT_CODE_ADDROW_EXCEPTION, "在数据库中增加一条记录时失败@Failed to add a new record in DataBase.@在數據庫中增加一條記錄時失敗@");
                _globalDict.Add(GLOBALDICT_CODE_ERASEROW_EXCEPTION, "在数据库中移除一条记录时失败@Failed to delete a record in DataBase.@在數據庫中移除一條記錄時失敗@");
                _globalDict.Add(GLOBALDICT_CODE_GETALL_EXCEPTION, "在数据库中提取公用数据时失败@Executing Getall failed.@在數據庫中提取公用數據時失敗@");
                _globalDict.Add(GLOBALDICT_CODE_IN_GETDEFAULTSTC_READ_DATA_FAILED, "在获取默认制式时读取数据失败@Failed to read Data while doing GetDefaultSTC.@在獲取默認制式時讀取數據失敗@");
                _globalDict.Add(GLOBALDICT_CODE_ADD_LOCK_ERROR, "添加一个对象锁失败@Failed to add LOCK object.@添加一個對像鎖失敗@");
                _globalDict.Add(GLOBALDICT_CODE_IN_SETUNLOCKOBJECT_READ_DATA_FAILED, "解锁对象时，发生数据库读写错误@Read Data failed while doing SetUnlockObject.@解鎖對像時，發生數據庫讀寫錯誤@");
                _globalDict.Add(GLOBALDICT_CODE_IN_SETUNLOCKOBJECT_EXCEPTION, "解锁对象时，发生一般性错误@Error happened while doing SetUnlockObject.@解鎖對像時，發生一般性錯誤@");
                _globalDict.Add(GLOBALDICT_CODE_IN_UNLOCKOBJECTTIMEOUT_READ_DATA_FAILED, "超时解锁对象时，发生数据库读写错误@Read Data failed while doing UnlockObjectTimeout.@超時解鎖對像時，發生數據庫讀寫錯誤@");
                _globalDict.Add(GLOBALDICT_CODE_IN_UNLOCKOBJECTTIMEOUT_EXCEPTION, "超时解锁对象时，发生一般性错误@Error happened while doing UnlockObjectTimeout.@超時解鎖對像時，發生一般性錯誤@");
                _globalDict.Add(GLOBALDICT_CODE_IN_ISCHANNELLOCK_READ_DATA_FAILED, "判断通道是否被锁定时，发生数据库读写错误@Read Data failed while doing IsChannelLock.@判斷通道是否被鎖定時，發生數據庫讀寫錯誤@");
                _globalDict.Add(GLOBALDICT_CODE_LOCK_CHANNEL_FAILED, "锁定通道失败@Failed to lock channel.@鎖定通道失敗@");
                _globalDict.Add(GLOBALDICT_CODE_NEXT_TIME_IS_MORE_THAN_1_MINUTE, "周期任务下次执行的时间在1分钟之后@The next time is more than 1 minute.@週期任務下次執行的時間在1分鐘之後@");
                _globalDict.Add(GLOBALDICT_CODE_TASKID_IS_NULL, "任务ID为空值@The TaskID is null.@任務ID為空值@");
                _globalDict.Add(GLOBALDICT_CODE_CAN_NOT_DELETE_THE_COMPLETE_TASK, "不能删除已完成的任务@Can not delete the complete task.@不能刪除已完成的任務@");
                _globalDict.Add(GLOBALDICT_CODE_CHANNEL_IS_NOT_AVAILABLE_TWOPARAM, "通道不可用！任务{0}会在{1}调度执行。@Channel is not available! Scheduled task {0} will begin at {1}.@通道不可用！任務{0}會在{1}調度執行。@");
                _globalDict.Add(GLOBALDICT_CODE_SCHEDULED_TASK_WILL_BEGIN_AT_TWOPARAM, "任务{0}会在{1}的时刻调度执行。@Scheduled task {0} will begin at {1}. @任務{0}會在{1}的時刻調度執行。@");
                _globalDict.Add(GLOBALDICT_CODE_CANNOTMODIFYTASK_WHERE_FILING, "任务正在采集，不能修改@Can not modify where the task is running@任務正在採集，不能修改@");
                _globalDict.Add(GLOBALDICT_CODE_PROGRAM_NAME_REPEATED, "信号源名字不能重复@Signal name can not repeat@信號源名字不能重複@");

                _globalDict.Add(GLOBALDICT_CODE_FILL_GETTASKMETADATA_EXCEPTION, "在GetTaskMetaData过程中发生错误@Error in GetTaskMetaData.@在GetTaskMetaData過程中發生錯誤@");
                 
                // 调试日志
                //ApplicationLog.WriteDebugTraceLog0(string.Format("### GlobalDictionary having {0} count Dict record. ###", _globalDict.Count));
            }
            catch (Exception ae)
            {
                // 错误日志
                //ApplicationLog.WriteInfo(string.Format("In InitDictionary function, Add a duplicate key values! The exception message is: {0}", ae.Message));
            }
        }

        #endregion

        #region 自定义异常代码

        //////////////////////////////////////////////////////////////////////////
        /*           自定义的异常代码范围，从18001开始,至19999至                */
        /*                         请参考下表进行定义                           */
        //////////////////////////////////////////////////////////////////////////


        /// <summary>
        /// 一般性未知错误: 19999
        /// </summary>
        public const int GLOBALDICT_CODE_UNKNOWN_ERROR = 19999;

        /// <summary>
        /// 致命的数据库错误: 19998
        /// </summary>
        public const int GLOBALDICT_CODE_DATABASE_FATAL_ERROR = 19998;

        #region 已定义

        /// <summary>
        /// 没有配置采集参数: 18001
        /// </summary>
        public const int GLOBALDICT_CODE_NO_CAPTURE_PARAM = 18001;

        /// <summary>
        /// 通道ID不合法: 18002
        /// </summary>
        public const int GLOBALDICT_CODE_THE_NCHANNEL_CANNOT_NEGATIVE = 18002;

        /// <summary>
        /// 没有可用的通道，相冲突的任务是: 18003
        /// </summary>
        public const int GLOBALDICT_CODE_NO_USEABLE_CHANNEL_CONFLICT_TASKS_ONEPARAM = 18003;

        /// <summary>
        /// 选择的通道正忙: 18004
        /// </summary>
        public const int GLOBALDICT_CODE_SELECTED_CHANNEL_IS_BUSY = 18004;

        /// <summary>
        /// 所有可用通道都正忙: 18005
        /// </summary>
        public const int GLOBALDICT_CODE_ALL_USEABLE_CHANNELS_ARE_BUSY = 18005;

        /// <summary>
        /// 通道正忙，冲突的任务信息是: 18006
        /// </summary>
        public const int GLOBALDICT_CODE_CHANNEL_IS_BUSY_AND_CLASH_TASK_INFO_TWOPARAM = 18006;

        /// <summary>
        /// 不能为信号源找到可用通道: 18007
        /// </summary>
        public const int GLOBALDICT_CODE_CAN_NOT_FIND_SIGNAL_SRC_FOR_CHANNEL = 18007;

        /// <summary>
        /// 在采集参数中未能找到采集截止时间: 18008
        /// </summary>
        public const int GLOBALDICT_CODE_CAN_NOT_FIND_THE_END_DATE_IN_CAPTURE_PARAM = 18008;

        /// <summary>
        /// 没有可用的通道: 18009
        /// </summary>
        public const int GLOBALDICT_CODE_NO_USEABLE_CHANNEL = 18009;

        /// <summary>
        /// 选择的通道正忙或不适合: 18010
        /// </summary>
        public const int GLOBALDICT_CODE_SELECTED_CHANNEL_IS_BUSY_OR_CAN_NOT_BE_SUITED_TO_PROGRAMME = 18010;

        /// <summary>
        /// 没有设置备份信号源: 18011
        /// </summary>
        public const int GLOBALDICT_CODE_NO_BACKUP_SIGNALSRC = 18011;

        /// <summary>
        /// 主备信号源任务中，没有主信号源任务信息: 18012
        /// </summary>
        public const int GLOBALDICT_CODE_NO_MAIN_TASK = 18012;

        /// <summary>
        /// 主备信号源任务中，没有信号源信息: 18013
        /// </summary>
        public const int GLOBALDICT_CODE_NO_SINGAL_SRC = 18013;

        /// <summary>
        /// 主备信号源任务中，信号源信息是错误的: 18014
        /// </summary>
        public const int GLOBALDICT_CODE_SINGAL_SRC_IS_WRONG = 18014;

        /// <summary>
        /// 信号源不在同一单元中: 18015
        /// </summary>
        public const int GLOBALDICT_CODE_SIGNAL_ID_ERROR_NOT_IN_A_UNIT = 18015;

        /// <summary>
        /// 任务信息错误，截止时间小于开始时间: 18016
        /// </summary>
        public const int GLOBALDICT_CODE_TASK_END_TIME_IS_SMALLER_THAN_BEING_TIME = 18016;

        /// <summary>
        /// 任务信息错误，非周期任务的时长超过24小时: 18017
        /// </summary>
        public const int GLOBALDICT_CODE_TASK_TIME_IS_OVER_24_HOURS = 18017;

        /// <summary>
        /// 任务已经被锁定，不能操作: 18018
        /// </summary>
        public const int GLOBALDICT_CODE_TASK_IS_LOCKED = 18018;

        /// <summary>
        /// 信号源和通道不匹配: 18019
        /// </summary>
        public const int GLOBALDICT_CODE_SIGNAL_AND_CHANNEL_IS_MISMATCHED = 18019;

        /// <summary>
        /// 找不到ID为*的任务: 18020
        /// </summary>
        public const int GLOBALDICT_CODE_CAN_NOT_FIND_THE_TASK_ONEPARAM = 18020;

        /// <summary>
        /// VTR设备已经被其它任务所占用: 18021
        /// </summary>
        public const int GLOBALDICT_CODE_VTR_HAS_BEEN_USED_BY_OTHER_TASKS_ONEPARAM = 18021;

        /// <summary>
        /// 不能更改任务时间，冲突任务包括: 18022
        /// </summary>
        public const int GLOBALDICT_CODE_CAN_NOT_MODIFY_TIME_CONFLICT_TASKS_ONEPARAM = 18022;

        /// <summary>
        /// 更新任务元数据失败: 18023
        /// </summary>
        public const int GLOBALDICT_CODE_SET_TASKMETADATA_FAIL = 18023;

        /// <summary>
        /// 任务不是一个周期任务: 18024
        /// </summary>
        public const int GLOBALDICT_CODE_TASK_IS_NOT_A_PERIODIC_TASK = 18024;

        /// <summary>
        /// 不能单独处理已拆分出来的周期任务: 18025
        /// </summary>
        public const int GLOBALDICT_CODE_NOT_DISPOSE_SEPARATE_PERIODIC_TASK = 18025;

        /// <summary>
        /// 修改周期任务失败: 18026
        /// </summary>
        public const int GLOBALDICT_CODE_MODIFY_THE_PERIODIC_TASK_FAIL = 18026;

        /// <summary>
        /// 修改任务数据失败: 18027
        /// </summary>
        public const int GLOBALDICT_CODE_MODIFY_TASK_FAIL = 18027;

        /// <summary>
        /// 任务不是一个手动任务: 18028
        /// </summary>
        public const int GLOBALDICT_CODE_TASK_IS_NOT_MANUAL_TASK = 18028;

        /// <summary>
        /// 不能拆分循环任务，周期任务: 18029
        /// </summary>
        public const int GLOBALDICT_CODE_CAN_NOT_SPLIT_LOOP_PERIODIC_TASK = 18029;

        /// <summary>
        /// 不能拆分任务，因为任务在5秒内将结束: 18030
        /// </summary>
        public const int GLOBALDICT_CODE_DO_NOT_ALLOW_TO_SPLIT_THE_TASK = 18030;

        /// <summary>
        /// 不能创建任务，因为任务开始时间必须小于截止时间: 18031
        /// </summary>
        public const int GLOBALDICT_CODE_CAN_NOT_CREATE_NEW_TASK_END_TIME_MUST_LARGER_THAN_START_TIME = 18031;

        /// <summary>
        /// 素材并不存在: 18032
        /// </summary>
        public const int GLOBALDICT_CODE_MATERIAL_DOES_NOT_EXIST = 18032;

        /// <summary>
        /// 存取计划任务数据失败: 18033
        /// </summary>
        public const int GLOBALDICT_CODE_FILL_PLANNING_FAILED = 18033;

        /// <summary>
        /// 传入的任务ID并不存在: 18034
        /// </summary>
        public const int GLOBALDICT_CODE_TASK_ID_DOES_NOT_EXIST = 18034;

        /// <summary>
        /// 修改任务状态时，该任务是一个周期任务的模板而不能修改: 18035
        /// </summary>
        public const int GLOBALDICT_CODE_TASK_IS_A_STENCIL_PLATE_TASK_OF_PERIODIC_TASKS_ONEPARAM = 18035;

        /// <summary>
        /// 任务信息不可用或任务正处理锁定状态不能操作: 18036
        /// </summary>
        public const int GLOBALDICT_CODE_TASK_ID_IS_NOT_AVAILABLE_OR_TASK_IS_LOCKED_ONEPARAM = 18036;

        /// <summary>
        /// 任务ID是不可用的: 18037
        /// </summary>
        public const int GLOBALDICT_CODE_TASK_ID_IS_NOT_AVAILABLE = 18037;

        /// <summary>
        /// 任务ID为{0}的任务并不存在: 18038
        /// </summary>
        public const int GLOBALDICT_CODE_TASK_IS_NOT_EXSIT_ONEPARAM = 18038;

        /// <summary>
        /// 在停止手动任务操作中，任务ID为{0}的任务是周期任务的模板任务: 18039
        /// </summary>
        public const int GLOBALDICT_CODE_IN_STOPMANUTASK_TASK_IS_A_STENCIL_PLATE_TASK_ONEPARAM = 18039;

        /// <summary>
        /// 在停止采集操作中，任务ID为{0}的任务是周期任务的模板任务: 18040
        /// </summary>
        public const int GLOBALDICT_CODE_IN_STOPCAPTURE_TASK_IS_A_STENCIL_PLATE_TASK_ONEPARAM = 18040;

        /// <summary>
        /// 任务信息中的唯一标识为空: 18041
        /// </summary>
        public const int GLOBALDICT_CODE_TASK_GUID_IS_NULL = 18041;

        /// <summary>
        /// 任务数据记录为空: 18042;
        /// </summary>
        public const int GLOBALDICT_CODE_TASKSET_IS_NULL = 18042;

        /// <summary>
        /// 更新任务信息中的源信息时失败: 18043
        /// </summary>
        public const int GLOBALDICT_CODE_UPDATE_TASKSOURCE_FAILED = 18043;

        /// <summary>
        /// 更新任务信息时失败: 18044
        /// </summary>
        public const int GLOBALDICT_CODE_UPDATE_TASK_INFORMATION_FAILED = 18044;

        /// <summary>
        /// 设置任务状态失败: 18045
        /// </summary>
        public const int GLOBALDICT_CODE_SET_TASKSTATE_ERROR = 18045;

        /// <summary>
        /// 获取任务元数据时失败: 18046
        /// </summary>
        public const int GLOBALDICT_CODE_GET_TASK_METADATA_ERROR = 18046;

        /// <summary>
        /// 从备份数据表中获取任务元数据失败: 18047
        /// </summary>
        public const int GLOBALDICT_CODE_GET_TASK_METADATA_IN_BACKUP_TABLE_ERROR = 18047;

        /// <summary>
        /// 设置任务元数据失败: 18048
        /// </summary>
        public const int GLOBALDICT_CODE_SET_TASK_METADATA_ERROR = 18048;

        /// <summary>
        /// 在备份数据表中设置任务元数据失败: 18049
        /// </summary>
        public const int GLOBALDICT_CODE_SET_TASK_METADATA_IN_BACKUP_TABLE_ERROR = 18049;

        /// <summary>
        /// 在GetTasksBySQL操作中发生异常: 18050
        /// </summary>
        public const int GLOBALDICT_CODE_FILL_GETTASKSBYSQL_EXCEPTION = 18050;

        /// <summary>
        /// 在GetConflictTasks操作中发生异常: 18051
        /// </summary>
        public const int GLOBALDICT_CODE_FILL_GETCONFLICTTASKS_EXCEPTION = 18051;

        /// <summary>
        /// 执行数据库命令时失败: 18052
        /// </summary>
        public const int GLOBALDICT_CODE_EXECUTE_COMMAND_ERROR = 18052;

        /// <summary>
        /// 在GetTaskSourceMapByTaskID操作中失败: 18053
        /// </summary>
        public const int GLOBALDICT_CODE_GET_TASK_SOURCEMAP_BY_TASKID_ERROR = 18053;

        /// <summary>
        /// 在数据库中删除任务数据时失败: 18054
        /// </summary>
        public const int GLOBALDICT_CODE_DELETE_TASK_FROM_DB_ERROR = 18054;

        /// <summary>
        /// 在StopTaskFromFSW操作中发生异常: 18055
        /// </summary>
        public const int GLOBALDICT_CODE_STOPTASKFROMFSW_ERROR = 18055;

        /// <summary>
        /// 在GetTieUpTaskByChannelID操作中发生异常: 18056
        /// </summary>
        public const int GLOBALDICT_CODE_GETTIEUPTASKBYCHANNELID_ERROR = 18056;

        /// <summary>
        /// 在kamatakiFailSet操作中发生异常: 18057
        /// </summary>
        public const int GLOBALDICT_CODE_FILL_KAMATAKIFAILSET_EXCEPTION = 18057;

        /// <summary>
        /// 从数据集中获取任务信息时失败: 18058
        /// </summary>
        public const int GLOBALDICT_CODE_FILL_TASKIDSET_EXCEPTION = 18058;

        /// <summary>
        /// 在GetTaskIDByTaskGUID2操作中发生异常: 18059
        /// </summary>
        public const int GLOBALDICT_CODE_FILL_GETTASKIDBYTASKGUID2_EXCEPTION = 18059;

        /// <summary>
        /// 执行数据库查询命令时失败: 18060
        /// </summary>
        public const int GLOBALDICT_CODE_SELECT_COMMAND_FAILD = 18060;

        /// <summary>
        /// 传入的XDCAM设备ID并不存在: 18061
        /// </summary>
        public const int GLOBALDICT_CODE_XDCAM_DEVICE_ID_DOES_NOT_EXIST = 18061;

        /// <summary>
        /// 传入的XDCAM任务ID并不存在: 18062
        /// </summary>
        public const int GLOBALDICT_CODE_XDCAM_TASKID_DOES_NOT_EXIST = 18062;

        /// <summary>
        /// XDCAM任务并不存在: 18063
        /// </summary>
        public const int GLOBALDICT_CODE_XDCAM_TASK_DOES_NOT_EXIST = 18063;

        /// <summary>
        /// XDCAM磁盘并不存在: 18064
        /// </summary>
        public const int GLOBALDICT_CODE_XDCAM_DISK_DOES_NOT_EXIST = 18064;

        /// <summary>
        /// 传入的XDCAM磁盘ID并不存在: 18065
        /// </summary>
        public const int GLOBALDICT_CODE_XDCAM_DISK_ID_DOES_NOT_EXIST = 18065;

        /// <summary>
        /// 传入的进度值是无效的，大于100%: 18066
        /// </summary>
        public const int GLOBALDICT_CODE_PROGRESS_IS_INVALID = 18066;

        /// <summary>
        /// XDCAM磁盘不能被上载: 18067
        /// </summary>
        public const int GLOBALDICT_CODE_XDCAM_DISK_DO_NOT_BE_UPLOADING = 18067;

        /// <summary>
        /// 传入的XDCAM任务文件ID并不存在: 18068
        /// </summary>
        public const int GLOBALDICT_CODE_XDCAM_TASK_FILE_ID_DOES_NOT_EXIST = 18068;

        /// <summary>
        /// 指定的XDCAM任务文件并不存在: 18069
        /// </summary>
        public const int GLOBALDICT_CODE_XDCAM_TASK_FILE_DOES_NOT_EXIST = 18069;

        /// <summary>
        /// 可以使用的XDCAM磁盘并不存在: 18070
        /// </summary>
        public const int GLOBALDICT_CODE_USABLE_XDCAM_DISK_DOES_NOT_EXIST = 18070;

        /// <summary>
        /// XDCAM磁盘数超过XDCAM设备数: 18071
        /// </summary>
        public const int GLOBALDICT_CODE_DISKES_IS_MORE_THAN_THE_DEVICES = 18071;

        /// <summary>
        /// ID为{0}的VTR设备并不存在: 18072
        /// </summary>
        public const int GLOBALDICT_CODE_VTRID_DOES_NOT_EXIST_THE_NVTRID_IS_ONEPARAM = 18072;

        /// <summary>
        /// 入出点序列个数不匹配: 18073
        /// </summary>
        public const int GLOBALDICT_CODE_INSET_AND_OUTSET_SEQUENCE_OF_THE_NUMBER_DOES_NOT_MATCH = 18073;

        /// <summary>
        /// 在SetVtrTapeMap操作中，传入的磁带ID{0}并不存在: 18074
        /// </summary>
        public const int GLOBALDICT_CODE_IN_SETVTRTAPEMAP_TAPEID_IS_NOT_EXIST_ONEPARAM = 18074;

        /// <summary>
        /// 在SetVtrTapeMap2操作中，传入的磁带ID{0}并不存在: 18075
        /// </summary>
        public const int GLOBALDICT_CODE_IN_SETVTRTAPEMAP2_TAPEID_IS_NOT_EXIST_ONEPARAM = 18075;

        /// <summary>
        /// 传入的用户ID并不存在: 18076
        /// </summary>
        public const int GLOBALDICT_CODE_USER_ID_DOES_NOT_EXIST = 18076;

        /// <summary>
        /// 传入的用户参数ID并不存在: 18077
        /// </summary>
        public const int GLOBALDICT_CODE_USER_PARAM_ID_DOES_NOT_EXIST = 18077;

        /// <summary>
        /// 用户参数信息并不存在: 18078
        /// </summary>
        public const int GLOBALDICT_CODE_USER_PARAM_DOES_NOT_EXIST = 18078;

        /// <summary>
        /// 名为{0}的用户模板配置信息已经存在
        /// </summary>
        public const int GLOBALDICT_CODE_THE_USER_TEMPLATE_HAS_EXISTS_ONEPARAM = 18079;

        /// <summary>
        /// 找不到ID为{0}的用户模板配置信息: 18080
        /// </summary>
        public const int GLOBALDICT_CODE_CAN_NOT_FIND_THE_TEMPLATE_ID_IS_ONEPARAM = 18080;

        /// <summary>
        /// 从DBP_SQ_TRANSCODEID中取下一序列值失败: 18081
        /// </summary>
        public const int GLOBALDICT_CODE_FAIL_TO_GET_NEXT_VALUE_FOR_DBP_SQ_TRANSCODEID = 18081;

        /// <summary>
        /// 无法找到指定的转码模板: 18082
        /// </summary>
        public const int GLOBALDICT_CODE_FAIL_TO_FIND_THE_APPOINTED_TRANSCODE = 18082;

        /// <summary>
        /// 无法找到指定的存档类型: 18083
        /// </summary>
        public const int GLOBALDICT_CODE_FAIL_TO_FIND_THE_APPOINTED_ARCHIVETYPE = 18083;

        /// <summary>
        /// 无法找到指定的转码策略: 18084
        /// </summary>
        public const int GLOBALDICT_CODE_FAIL_TO_FIND_THE_APPOINTED_TRANSCODEPOLICY = 18084;

        /// <summary>
        /// 从DBP_SQ_TRANSFERID中取下一序列值失败: 18085
        /// </summary>
        public const int GLOBALDICT_CODE_FAIL_TO_GET_NEXT_VALUE_FOR_DBP_SQ_TRANSFERID = 18085;

        /// <summary>
        /// 无法找到指定的转发模板: 18086
        /// </summary>
        public const int GLOBALDICT_CODE_FAIL_TO_FIND_THE_APPOINTED_TRANSFER = 18086;

        /// <summary>
        /// 指定的策略并不存在: 18087
        /// </summary>
        public const int GLOBALDICT_CODE_POLICY_DOES_NOT_EXIST = 18087;

        /// <summary>
        /// 指定的用户策略级别ID并不存在: 18088
        /// </summary>
        public const int GLOBALDICT_CODE_POLICY_USER_CLASS_ID_DOES_NOT_EXIST = 18088;

        /// <summary>
        /// 任务的策略信息并不存在: 18089
        /// </summary>
        public const int GLOBALDICT_CODE_TASK_POLICY_DOES_NOT_EXIST = 18089;

        /// <summary>
        /// 在MediaRow2MediaStruct过程中，转换参数失败: 18090
        /// </summary>
        public const int GLOBALDICT_CODE_CONVERT_PARAMS_ERROR_IN_MEDIAROW2MEDIASTRUCT = 18090;

        /// <summary>
        /// 无法找到指定的素材: 18091
        /// </summary>
        public const int GLOBALDICT_CODE_FAIL_TO_FIND_THE_APPOINTED_MATERIAL = 18091;

        /// <summary>
        /// ID为{0}的素材并不存在: 18092
        /// </summary>
        public const int GLOBALDICT_CODE_MATERIAL_ID_DOES_NOT_EXIST_ONEPARAM = 18092;

        /// <summary>
        /// 素材的策略信息并不存在: 18093
        /// </summary>
        public const int GLOBALDICT_CODE_MATERIAL_POLICY_DOES_NOT_EXIST = 18093;

        /// <summary>
        /// 通过任务ID{0}和策略ID{1}获取素材失败: 18094
        /// </summary>
        public const int GLOBALDICT_CODE_FAILED_TO_GET_MATERIAL_BY_TASKID_FOR_POLICYID_TWOPARAM = 18094;

        /// <summary>
        /// 引入任务的ID并不存在: 18095
        /// </summary>
        public const int GLOBALDICT_CODE_IMPORT_TASK_ID_DOES_NOT_EXIST = 18095;

        /// <summary>
        /// Router服务器的ID并不存在: 18096
        /// </summary>
        public const int GLOBALDICT_CODE_ROUTER_SERVER_ID_DOES_NOT_EXIST = 18096;

        /// <summary>
        /// 消息ID并不存在: 18097
        /// </summary>
        public const int GLOBALDICT_CODE_MESSAGE_ID_DOES_NOT_EXIST = 18097;

        /// <summary>
        /// 消息服务器ID并不存在: 18098
        /// </summary>
        public const int GLOBALDICT_CODE_MESSAGE_CTRL_ID_DOES_NOT_EXIST = 18098;

        /// <summary>
        /// 采集设备ID并不存在: 18099
        /// </summary>
        public const int GLOBALDICT_CODE_CAPTURE_DEVICE_ID_DOES_NOT_EXIST = 18099;

        /// <summary>
        /// 信号源ID并不存在: 18100
        /// </summary>
        public const int GLOBALDICT_CODE_SIGNAL_RESOURCE_ID_DOES_NOT_EXIST = 18100;

        /// <summary>
        /// 节目ID并不存在: 18101
        /// </summary>
        public const int GLOBALDICT_CODE_PROGRAM_ID_DOES_NOT_EXIST = 18101;

        /// <summary>
        /// 信号类型ID并不存在: 18102
        /// </summary>
        public const int GLOBALDICT_CODE_SIGNAL_TYPE_ID_DOES_NOT_EXIST = 18102;

        /// <summary>
        /// 信号类型与设备类型对应关系并不存在: 18103
        /// </summary>
        public const int GLOBALDICT_CODE_SIGNAL_TYPE_MAP_DOES_NOT_EXIST = 18103;

        /// <summary>
        /// 调度ID并不存在: 18104
        /// </summary>
        public const int GLOBALDICT_CODE_SCHEDULER_ID_DOES_NOT_EXIST = 18104;

        /// <summary>
        /// 单元ID并不存在: 18105
        /// </summary>
        public const int GLOBALDICT_CODE_UNIT_ID_DOES_NOT_EXIST = 18105;

        /// <summary>
        /// 通道ID并不存在: 18106
        /// </summary>
        public const int GLOBALDICT_CODE_CHANNEL_ID_DOES_NOT_EXIST = 18106;

        /// <summary>
        /// 收录设备类型ID并不存在: 18107
        /// </summary>
        public const int GLOBALDICT_CODE_REC_DEVICE_TYPE_ID_DOES_NOT_EXIST = 18107;

        /// <summary>
        /// 路由ID并不存在: 18108
        /// </summary>
        public const int GLOBALDICT_CODE_ROUTER_ID_DOES_NOT_EXIST = 18108;

        /// <summary>
        /// 路由输出对应关系不存在: 18109
        /// </summary>
        public const int GLOBALDICT_CODE_ROUTER_OUT_MAP_DOES_NOT_EXIST = 18109;

        /// <summary>
        /// 信号单元对应关系不存在: 18110
        /// </summary>
        public const int GLOBALDICT_CODE_SIGNAL_UNIT_MAP_DOES_NOT_EXIST = 18110;

        /// <summary>
        /// 通道单元对应关系不存在: 18111
        /// </summary>
        public const int GLOBALDICT_CODE_CHANNEL_UNIT_MAP_DOES_NOT_EXIST = 18111;

        /// <summary>
        /// 通过IP找不到设备: 18112
        /// </summary>
        public const int GLOBALDICT_CODE_DEVICE_IP_DOES_NOT_EXIST = 18112;

        /// <summary>
        /// 锁定对象的ID不合法: 18113
        /// </summary>
        public const int GLOBALDICT_CODE_LOCK_OBJECTID_WRONG = 18113;

        /// <summary>
        /// 锁定对象的类型不正确: 18114
        /// </summary>
        public const int GLOBALDICT_CODE_LOCK_OBJECT_TPYEID_IS_NOT_EXIST = 18114;

        /// <summary>
        /// 锁定对象的时间长度不合法: 18115
        /// </summary>
        public const int GLOBALDICT_CODE_LOCK_OBJECT_TIMEOUT_IS_WRONG = 18115;

        /// <summary>
        /// 解锁对象的ID不合法: 18116
        /// </summary>
        public const int GLOBALDICT_CODE_UNLOCK_OBJECTID_IS_WRONG = 18116;

        /// <summary>
        /// 解锁对象的类型不正确: 18117
        /// </summary>
        public const int GLOBALDICT_CODE_UNLOCK_OBJECT_TYPEID_IS_NOT_EXIST = 18117;

        /// <summary>
        /// 检测VTR是否冲突过程中，任务开始时间不合法: 18118
        /// </summary>
        public const int GLOBALDICT_CODE_IN_ISVTRCOLLIDE_BEGINTIME_IS_WRONG = 18118;

        /// <summary>
        /// 检测VTR是否冲突过程中，任务结束时间不合法: 18119
        /// </summary>
        public const int GLOBALDICT_CODE_IN_ISVTRCOLLIDE_ENDTIME_IS_WRONG = 18119;

        /// <summary>
        /// 检测VTR是否冲突过程中，VTR的ID不合法: 18120
        /// </summary>
        public const int GLOBALDICT_CODE_IN_ISVTRCOLLIDE_VTRID_IS_WRONG = 18120;

        /// <summary>
        /// 检测VTR是否冲突过程中，任务的ID不合法: 18121
        /// </summary>
        public const int GLOBALDICT_CODE_IN_ISVTRCOLLIDE_TASKID_IS_WRONG = 18121;

        /// <summary>
        /// 在获取所有XDCAM设备数据时出错: 18122
        /// </summary>
        public const int GLOBALDICT_CODE_XDCAM_LOADALLDEVICES_FILL_FAILED = 18122;

        /// <summary>
        /// 在获取XDCAM任务元数据时出错: 18123
        /// </summary>
        public const int GLOBALDICT_CODE_FILL_XDCAM_DBP_XDCAM_TASK_METADATA_EXCEPTION = 18123;

        /// <summary>
        /// 在设置XDCAM任务元数据时出错: 18124
        /// </summary>
        public const int GLOBALDICT_CODE_FILL_XDCAM_SETTASKMETADATA_EXCEPTION = 18124;

        /// <summary>
        /// 在GetXDCAMDiskByUMID过程中发生错误: 18125
        /// </summary>
        public const int GLOBALDICT_CODE_FILL_XDCAM_GETXDCAMDISKBYUMID_EXCEPTION = 18125;

        /// <summary>
        /// 在GetALLUsableDisk过程中发生错误: 18126
        /// </summary>
        public const int GLOBALDICT_CODE_FILL_XDCAM_GETALLUSABLEDISK_EXCEPTION = 18126;

        /// <summary>
        /// 在GetXDCAMTaskByUMID过程中发生错误: 18127
        /// </summary>
        public const int GLOBALDICT_CODE_FILL_XDCAM_GETXDCAMTASKBYUMID_EXCEPTION = 18127;

        /// <summary>
        /// 在GetXDCAMTaskFileByUMID过程中发生错误: 18128
        /// </summary>
        public const int GLOBALDICT_CODE_FILL_XDCAM_GETXDCAMTASKFILEBYUMID_EXCEPTION = 18128;

        /// <summary>
        /// 在GetXDCAMTaskFileByTaskID过程中发生错误: 18129
        /// </summary>
        public const int GLOBALDICT_CODE_FILL_XDCAM_GETXDCAMTASKFILEBYTASKID_EXCEPTION = 18129;

        /// <summary>
        /// 在QueryTaskFileInfoByString过程中发生错误: 18130
        /// </summary>
        public const int GLOBALDICT_CODE_FILL_XDCAM_QUERYTASKFILEINFOBYSTRING_EXCEPTION = 18130;

        /// <summary>
        /// 在GetpriorityTaskID过程中发生错误: 18131
        /// </summary>
        public const int GLOBALDICT_CODE_FILL_XDCAM_GETPRIORITYTASKID_EXCEPTION = 18131;

        /// <summary>
        /// XDCAM设备信息为空: 18132
        /// </summary>
        public const int GLOBALDICT_CODE_XDCAMSET_IS_NULL = 18132;

        /// <summary>
        /// 更新XDCAM设备信息时，发生数据库并发冲突: 18133
        /// </summary>
        public const int GLOBALDICT_CODE_XDCAM_UPDATEDEVICES_CONCURRENCY_EXCEPTION = 18133;

        /// <summary>
        /// 更新XDCAM设备信息失败: 18134
        /// </summary>
        public const int GLOBALDICT_CODE_XDCAM_UPDATEDEVICES_FAILED = 18134;

        /// <summary>
        /// 在AddTasksOfWholeDisc过程中发生错误: 18135
        /// </summary>
        public const int GLOBALDICT_CODE_FILL_XDCAM_ADDTASKSOFWHOLEDISC_EXCEPTION = 18135;

        /// <summary>
        /// 在GetCancelTaskFileName过程中发生错误: 18136
        /// </summary>
        public const int GLOBALDICT_CODE_FILL_XDCAM_GETCANCELTASKFILENAME_EXCEPTION = 18136;

        /// <summary>
        /// 更新VTR信息时，VTR信息为空: 18137
        /// </summary>
        public const int GLOBALDICT_CODE_IN_UPDATEVTRSET_DATASET_IS_NULL = 18137;

        /// <summary>
        /// 更新VTR信息失败: 18138
        /// </summary>
        public const int GLOBALDICT_CODE_IN_UPDATEVTRSET_UPDATE_FAILED = 18138;

        /// <summary>
        /// 在LoadVtrInfo过程中发生错误: 18139
        /// </summary>
        public const int GLOBALDICT_CODE_FILL_VTR_LOADVTRINFO_EXCEPTION = 18139;

        /// <summary>
        /// 在LoadVtrTaskInOut过程中发生错误: 18140
        /// </summary>
        public const int GLOBALDICT_CODE_FILL_VTR_LOADVTRTASKINOUT_EXCEPTION = 18140;

        /// <summary>
        /// 在LoadVTRTapeList过程中发生错误: 18141
        /// </summary>
        public const int GLOBALDICT_CODE_FILL_VTR_LOADVTRTAPELIST_EXCEPTION = 18141;

        /// <summary>
        /// 在GetVTRUploadTasks过程中，从数据库读取数据失败: 18142
        /// </summary>
        public const int GLOBALDICT_CODE_IN_GETVTRUPLOADTASKS_READ_DATA_FAILED = 18142;

        /// <summary>
        /// 在GetVtrUploadTaskInfo过程中发生错误: 18143
        /// </summary>
        public const int GLOBALDICT_CODE_FILL_VTR_GETVTRUPLOADTASKINFO_EXCEPTION = 18143;

        /// <summary>
        /// 在GetTimePeriodsByScheduleVBUTasks过程中，从数据库读取数据失败: 18144
        /// </summary>
        public const int GLOBALDICT_CODE_IN_GETTIMEPERIODSBYSCHEDULEVBUTASKS_READ_DATA_FAILED = 18144;

        /// <summary>
        /// 在GetVBUTasksBySQL过程中发生错误: 18145
        /// </summary>
        public const int GLOBALDICT_CODE_FILL_VTR_GETVBUTASKSBYSQL_EXCEPTION = 18145;

        /// <summary>
        /// 在GetNeedManualExecuteVTRUploadTasks过程中发生错误: 18146
        /// </summary>
        public const int GLOBALDICT_CODE_FILL_VTR_GETNEEDMANUALEXECUTEVTRUPLOADTASKS_EXCEPTION = 18146;

        /// <summary>
        /// 在GetVtrUploadTaskInfoForUpload过程中发生错误: 18147
        /// </summary>
        public const int GLOBALDICT_CODE_FILL_VTR_GETVTRUPLOADTASKINFOFORUPLOAD_EXCEPTION = 18147;

        /// <summary>
        /// 在GetVtrTaskSetForImportClip过程中发生错误: 18148
        /// </summary>
        public const int GLOBALDICT_CODE_FILL_VTR_GETVTRTASKSETFORIMPORTCLIP_EXCEPTION = 18148;

        /// <summary>
        /// 在DeleteVtrUploadTask过程中，执行数据库操作失败: 18149
        /// </summary>
        public const int GLOBALDICT_CODE_IN_DELETEVTRUPLOADTASK_READ_DATA_FAILED = 18149;

        /// <summary>
        /// 在DeleteVtrTaskMetaData过程中发生错误: 18150
        /// </summary>
        public const int GLOBALDICT_CODE_FILL_VTR_DELETEVTRTASKMETADATA_EXCEPTION = 18150;

        /// <summary>
        /// 在GetVtrTaskMetaData过程中发生错误: 18151
        /// </summary>
        public const int GLOBALDICT_CODE_FILL_VTR_GETVTRTASKMETADATA_EXCEPTION = 18151;

        /// <summary>
        /// 在GetVtrTaskMetaDataByGUID过程中发生错误: 18152
        /// </summary>
        public const int GLOBALDICT_CODE_FILL_VTR_GETVTRTASKMETADATABYGUID_EXCEPTION = 18152;

        /// <summary>
        /// 在QueryVtrDownloadTaskByCondition过程中发生错误: 18153
        /// </summary>
        public const int GLOBALDICT_CODE_FILL_VTR_QUERYVTRDOWNLOADTASKBYCONDITION_EXCEPTION = 18153;

        /// <summary>
        /// 在LoadVTRDownloadMaterialList过程中发生错误: 18154
        /// </summary>
        public const int GLOBALDICT_CODE_FILL_VTR_LOADVTRDOWNLOADMATERIALLIST_EXCEPTION = 18154;

        /// <summary>
        /// 在QueryVtrDownloadMaterialByCondition过程中发生错误: 18155
        /// </summary>
        public const int GLOBALDICT_CODE_FILL_VTR_QUERYVTRDOWNLOADMATERIALBYCONDITION_EXCEPTION = 18155;

        /// <summary>
        /// 在QueryVtrTaskInfoByCondition过程中发生错误: 18156
        /// </summary>
        public const int GLOBALDICT_CODE_FILL_VTR_QUERYVTRTASKINFOBYCONDITION_EXCEPTION = 18156;

        /// <summary>
        /// 在GetVtrTaskInOutPoint过程中发生错误: 18157
        /// </summary>
        public const int GLOBALDICT_CODE_FILL_VTR_GETVTRTASKINOUTPOINT_EXCEPTION = 18157;

        /// <summary>
        /// 在GetVtrTaskInfoByID过程中发生错误: 18158
        /// </summary>
        public const int GLOBALDICT_CODE_FILL_VTR_GETVTRTASKINFOBYID_EXCEPTION = 18158;

        /// <summary>
        /// 在GetVtrUploadTaskInfoByGUID过程中发生错误: 18159
        /// </summary>
        public const int GLOBALDICT_CODE_FILL_VTR_GETVTRUPLOADTASKINFOBYGUID_EXCEPTION = 18159;

        /// <summary>
        /// 在LoadVTRTapeMap过程中发生错误: 18160
        /// </summary>
        public const int GLOBALDICT_CODE_FILL_VTR_LOADVTRTAPEMAP_EXCEPTION = 18160;

        /// <summary>
        /// 在GetTapeInfoByName过程中发生错误: 18161
        /// </summary>
        public const int GLOBALDICT_CODE_FILL_VTR_GETTAPEINFOBYNAME_EXCEPTION = 18161;

        /// <summary>
        /// 在GetAllTapeInfo过程中发生错误: 18162
        /// </summary>
        public const int GLOBALDICT_CODE_FILL_VTR_GETALLTAPEINFO_EXCEPTION = 18162;

        /// <summary>
        /// 在更新用户信息时，用户信息为空: 18163
        /// </summary>
        public const int GLOBALDICT_CODE_IN_UPDATEUSERS_DATASET_IS_NULL = 18163;

        /// <summary>
        /// 在更新用户信息时发生错误: 18164
        /// </summary>
        public const int GLOBALDICT_CODE_IN_UPDATEUSERS_UPDATE_FAILED = 18164;

        /// <summary>
        /// 在GetAllCPParamTemplate过程中发生错误: 18165
        /// </summary>
        public const int GLOBALDICT_CODE_FILL_USER_GETALLCPPARAMTEMPLATE_EXCEPTION = 18165;

        /// <summary>
        /// 在GetDefaultParamTemplate过程中发生错误: 18166
        /// </summary>
        public const int GLOBALDICT_CODE_FILL_USER_GETDEFAULTPARAMTEMPLATE_EXCEPTION = 18166;

        /// <summary>
        /// 在GetAllUserParamMap过程中发生错误: 18167
        /// </summary>
        public const int GLOBALDICT_CODE_FILL_USER_GETALLUSERPARAMMAP_EXCEPTION = 18167;

        /// <summary>
        /// 在GetCPParamTemplateIDByUserID过程中发生错误: 18168
        /// </summary>
        public const int GLOBALDICT_CODE_FILL_USER_GETCPPARAMTEMPLATEIDBYUSERID_EXCEPTION = 18168;

        /// <summary>
        /// 在GetUserSetting过程中发生错误: 18169
        /// </summary>
        public const int GLOBALDICT_CODE_FILL_USER_GETUSERSETTING_EXCEPTION = 18169;

        /// <summary>
        /// 在SetUserSetting过程中发生错误: 18170
        /// </summary>
        public const int GLOBALDICT_CODE_FILL_USER_SETUSERSETTING_EXCEPTION = 18170;

        /// <summary>
        /// 在UpdateUserSetting过程中，更新数据时数据集为空: 18171
        /// </summary>
        public const int GLOBALDICT_CODE_IN_UPDATEUSERSETTING_DATASET_IS_NULL = 18171;

        /// <summary>
        /// 在UpdateUserSetting过程中，更新数据时失败: 18172
        /// </summary>
        public const int GLOBALDICT_CODE_IN_UPDATEUSERSETTING_UPDATE_FAILED = 18172;

        /// <summary>
        /// 在GetUserParamForDB2过程中发生错误: 18173
        /// </summary>
        public const int GLOBALDICT_CODE_FILL_USER_GETUSERPARAMFORDB2_EXCEPTION = 18173;

        /// <summary>
        /// 在GetUserHiddenChannels过程中发生错误: 18174
        /// </summary>
        public const int GLOBALDICT_CODE_FILL_USER_GETUSERHIDDENCHANNELS_EXCEPTION = 18174;

        /// <summary>
        /// 在AddUserTemplate过程中发生错误: 18175
        /// </summary>
        public const int GLOBALDICT_CODE_FILL_USER_ADDUSERTEMPLATE_EXCEPTION = 18175;

        /// <summary>
        /// 在ModifyUserTemplateName过程中发生错误: 18176
        /// </summary>
        public const int GLOBALDICT_CODE_FILL_USER_MODIFYUSERTEMPLATENAME_EXCEPTION = 18176;

        /// <summary>
        /// 在CheckUserTemplateName过程中发生错误: 18177
        /// </summary>
        public const int GLOBALDICT_CODE_FILL_USER_CHECKUSERTEMPLATENAME_EXCEPTION = 18177;

        /// <summary>
        /// 在GetUserTemplateByID过程中发生错误: 18178
        /// </summary>
        public const int GLOBALDICT_CODE_FILL_USER_GETUSERTEMPLATEBYID_EXCEPTION = 18178;

        /// <summary>
        /// 在ModifyUserTempalteContent过程中发生错误: 18179
        /// </summary>
        public const int GLOBALDICT_CODE_FILL_USER_MODIFYUSERTEMPALTECONTENT_EXCEPTION = 18179;

        /// <summary>
        /// 在DeleteUserTemplate过程中发生错误: 18180
        /// </summary>
        public const int GLOBALDICT_CODE_FILL_USER_DELETEUSERTEMPLATE_EXCEPTION = 18180;

        /// <summary>
        /// 在GetUserTemplate过程中发生错误: 18181
        /// </summary>
        public const int GLOBALDICT_CODE_FILL_USER_GETUSERTEMPLATE_EXCEPTION = 18181;

        /// <summary>
        /// 在GetUserAllTemplates过程中发生错误: 18182
        /// </summary>
        public const int GLOBALDICT_CODE_FILL_USER_GETUSERALLTEMPLATES_EXCEPTION = 18182;

        /// <summary>
        /// 在GetVtrUploadTaskInfoForUpload过程中发生错误: 18183
        /// </summary>
        public const int GLOBALDICT_CODE_FILL_UPVTR_GETVTRUPLOADTASKINFOFORUPLOAD_EXCEPTION = 18183;

        /// <summary>
        /// 在GetVtrTaskSetForImportClip过程中发生错误: 18184
        /// </summary>
        public const int GLOBALDICT_CODE_FILL_UPVTR_GETVTRTASKSETFORIMPORTCLIP_EXCEPTION = 18184;

        /// <summary>
        /// 在LoadUPVTRTapeMap过程中发生错误: 18185
        /// </summary>
        public const int GLOBALDICT_CODE_FILL_UPVTR_LOADUPVTRTAPEMAP_EXCEPTION = 18185;

        /// <summary>
        /// 更新任务信息时失败: 18186
        /// </summary>
        public const int GLOBALDICT_CODE_IN_UPDATETASKS_UPDATE_FAILED = 18186;

        /// <summary>
        /// 从Oracle数据库中获取序列值失败: 18187
        /// </summary>
        public const int GLOBALDICT_CODE_GET_ORACLE_SEQUENCE_FAILED = 18187;

        /// <summary>
        /// 从SQLServer数据库中获取序列值失败: 18188
        /// </summary>
        public const int GLOBALDICT_CODE_GET_SQLSERVER_SEQUENCE_FAILED = 18188;

        /// <summary>
        /// 从DB2数据库中获取序列值失败: 18189
        /// </summary>
        public const int GLOBALDICT_CODE_GET_DB2_SEQUENCE_FAILED = 18189;

        /// <summary>
        /// 在SelectMaterial过程中发生错误: 18190
        /// </summary>
        public const int GLOBALDICT_CODE_FILL_MATERIAL_SELECTMATERIAL_EXCEPTION = 18190;

        /// <summary>
        /// 在SelectMaterialVideoAndAudio过程中发生错误: 18191
        /// </summary>
        public const int GLOBALDICT_CODE_FILL_MATERIAL_SELECTMATERIALVIDEOANDAUDIO_EXCEPTION = 18191;

        /// <summary>
        /// 在LoadPolicies过程中发生错误: 18192
        /// </summary>
        public const int GLOBALDICT_CODE_FILL_POLICY_LOADPOLICIES_EXCEPTION = 18192;

        /// <summary>
        /// 更新策略信息时，策略信息为空: 18193
        /// </summary>
        public const int GLOBALDICT_CODE_IN_UPDATEPOLICIES_DATASET_IS_NULL = 18193;

        /// <summary>
        /// 更新策略信息时，发生数据库并发冲突: 18194
        /// </summary>
        public const int GLOBALDICT_CODE_IN_UPDATEPOLICIES_CONCURRENCY_EXCEPTION = 18194;

        /// <summary>
        /// 更新策略信息时，操作数据库失败: 18195
        /// </summary>
        public const int GLOBALDICT_CODE_IN_UPDATEPOLICIES_UPDATE_FAILED = 18195;

        /// <summary>
        /// 在FindInPolicyTaskByID过程中发生错误: 18196
        /// </summary>
        public const int GLOBALDICT_CODE_FILL_POLICY_FINDINPOLICYTASKBYID_EXCEPTION = 18196;

        /// <summary>
        /// 在FindInPolicyUserByID过程中发生错误: 18197
        /// </summary>
        public const int GLOBALDICT_CODE_FILL_POLICY_FINDINPOLICYUSERBYID_EXCEPTION = 18197;

        /// <summary>
        /// 在FindInPolicyClassByID过程中发生错误: 18198
        /// </summary>
        public const int GLOBALDICT_CODE_FILL_POLICY_FINDINPOLICYCLASSBYID_EXCEPTION = 18198;

        /// <summary>
        /// 在LoadMaterialTypes过程中发生错误: 18199
        /// </summary>
        public const int GLOBALDICT_CODE_FILL_MATERIAL_LOADMATERIALTYPES_EXCEPTION = 18199;

        /// <summary>
        /// 在GetFailMetadata过程中发生错误: 18200
        /// </summary>
        public const int GLOBALDICT_CODE_FILL_MATERIAL_GETFAILMETADATA_EXCEPTION = 18200;

        /// <summary>
        /// 在ChangeArchiveAndClipState过程中发生错误: 18201
        /// </summary>
        public const int GLOBALDICT_CODE_FILL_MATERIAL_CHANGEARCHIVEANDCLIPSTATE_EXCEPTION = 18201;

        /// <summary>
        /// 在GetMaterialNeedArchive过程中发生错误: 18202
        /// </summary>
        public const int GLOBALDICT_CODE_FILL_MATERIAL_GETMATERIALNEEDARCHIVE_EXCEPTION = 18202;

        /// <summary>
        /// 在GetXDCAMMaterialNeedArchive过程中发生错误: 18203
        /// </summary>
        public const int GLOBALDICT_CODE_FILL_MATERIAL_GETXDCAMMATERIALNEEDARCHIVE_EXCEPTION = 18203;

        /// <summary>
        /// 在GetMaterialArchiveInfoForUpload过程中发生错误: 18204
        /// </summary>
        public const int GLOBALDICT_CODE_FILL_MATERIAL_GETMATERIALARCHIVEINFOFORUPLOAD_EXCEPTION = 18204;

        /// <summary>
        /// 在GetMaterialArchiveInfo过程中发生错误: 18205
        /// </summary>
        public const int GLOBALDICT_CODE_FILL_MATERIAL_GETMATERIALARCHIVEINFO_EXCEPTION = 18205;

        /// <summary>
        /// 更新素材信息时，素材信息为空: 18206
        /// </summary>
        public const int GLOBALDICT_CODE_IN_UPDATEMATERIALS_DATASET_IS_NULL = 18206;

        /// <summary>
        /// 更新素材信息时，操作数据库失败: 18207
        /// </summary>
        public const int GLOBALDICT_CODE_IN_UPDATEMATERIALS_UPDATE_FAILED = 18207;

        /// <summary>
        /// 在UpdateMaterialsInDB2过程中，操作数据库失败: 18208
        /// </summary>
        public const int GLOBALDICT_CODE_IN_UPDATEMATERIALSINDB2_UPDATE_FAILED = 18208;

        /// <summary>
        /// 在UpdateMaterialsInDB2过程中发生错误: 18209
        /// </summary>
        public const int GLOBALDICT_CODE_FILL_MATERIAL_UPDATEMATERIALSINDB2_EXCEPTION = 18209;

        /// <summary>
        /// 在GetMaterialArchive过程中发生错误: 18210
        /// </summary>
        public const int GLOBALDICT_CODE_FILL_MATERIAL_GETMATERIALARCHIVE_EXCEPTION = 18210;

        /// <summary>
        /// 在GetMaterialArchiveByTaskID过程中发生错误: 18211
        /// </summary>
        public const int GLOBALDICT_CODE_FILL_MATERIAL_GETMATERIALARCHIVEBYTASKID_EXCEPTION = 18211;

        /// <summary>
        /// 在GetMaterialVideo过程中发生错误: 18212
        /// </summary>
        public const int GLOBALDICT_CODE_FILL_MATERIAL_GETMATERIALVIDEO_EXCEPTION = 18212;

        /// <summary>
        /// 在GetMaterialAudio过程中发生错误: 18213
        /// </summary>
        public const int GLOBALDICT_CODE_FILL_MATERIAL_GETMATERIALAUDIO_EXCEPTION = 18213;

        /// <summary>
        /// 在GetMaterialVideoInBackup过程中发生错误: 18214
        /// </summary>
        public const int GLOBALDICT_CODE_FILL_MATERIAL_GETMATERIALVIDEOINBACKUP_EXCEPTION = 18214;

        /// <summary>
        /// 在GetMaterialAudioInBackup过程中发生错误: 18215
        /// </summary>
        public const int GLOBALDICT_CODE_FILL_MATERIAL_GETMATERIALAUDIOINBACKUP_EXCEPTION = 18215;

        /// <summary>
        /// 在GetMaterial过程中发生错误: 18216
        /// </summary>
        public const int GLOBALDICT_CODE_FILL_MATERIAL_GETMATERIAL_EXCEPTION = 18216;

        /// <summary>
        /// 在GetMaterialByTaskID过程中发生错误: 18217
        /// </summary>
        public const int GLOBALDICT_CODE_FILL_MATERIAL_GETMATERIALBYTASKID_EXCEPTION = 18217;

        /// <summary>
        /// 在GetMaterialIDByTask过程中发生错误: 18218
        /// </summary>
        public const int GLOBALDICT_CODE_FILL_MATERIAL_GETMATERIALIDBYTASK_EXCEPTION = 18218;

        /// <summary>
        /// 在GetMaterialInBackupByTaskID过程中发生错误: 18219
        /// </summary>
        public const int GLOBALDICT_CODE_FILL_MATERIAL_GETMATERIALINBACKUPBYTASKID_EXCEPTION = 18219;

        /// <summary>
        /// 在GetMaterialDuration过程中发生错误: 18220
        /// </summary>
        public const int GLOBALDICT_CODE_FILL_MATERIAL_GETMATERIALDURATION_EXCEPTION = 18220;

        /// <summary>
        /// 在GetMaterialDurationByID过程中发生错误: 18221
        /// </summary>
        public const int GLOBALDICT_CODE_FILL_MATERIAL_GETMATERIALDURATIONBYID_EXCEPTION = 18221;

        /// <summary>
        /// 在GetTaskDuration过程中发生错误: 18222
        /// </summary>
        public const int GLOBALDICT_CODE_FILL_MATERIAL_GETTASKDURATION_EXCEPTION = 18222;

        /// <summary>
        /// 在ModifyMaterialArchiveStateInDb2过程中发生错误: 18223
        /// </summary>
        public const int GLOBALDICT_CODE_FILL_MATERIAL_MODIFYMATERIALARCHIVESTATEINDB2_EXCEPTION = 18223;

        /// <summary>
        /// 在ModifyMaterialClipStateInDb2过程中发生错误: 18224
        /// </summary>
        public const int GLOBALDICT_CODE_FILL_MATERIAL_MODIFYMATERIALCLIPSTATEINDB2_EXCEPTION = 18224;

        /// <summary>
        /// 在UpdateSaveInDBStateInDb2过程中发生错误: 18225
        /// </summary>
        public const int GLOBALDICT_CODE_FILL_MATERIAL_UPDATESAVEINDBSTATEINDB2_EXCEPTION = 18225;

        /// <summary>
        /// 在GetLastSection过程中发生错误: 18226
        /// </summary>
        public const int GLOBALDICT_CODE_FILL_MATERIAL_GETLASTSECTION_EXCEPTION = 18226;

        /// <summary>
        /// 在ModifyArchiveState过程中发生错误: 18227
        /// </summary>
        public const int GLOBALDICT_CODE_FILL_MATERIAL_MODIFYARCHIVESTATE_EXCEPTION = 18227;

        /// <summary>
        /// 在DeleteAudioVideoByMaterialID过程中发生错误: 18228
        /// </summary>
        public const int GLOBALDICT_CODE_FILL_MATERIAL_DELETEAUDIOVIDEOBYMATERIALID_EXCEPTION = 18228;

        /// <summary>
        /// 在QueryArchiveAndClipStateByMaterialID过程中发生错误: 18229
        /// </summary>
        public const int GLOBALDICT_CODE_FILL_MATERIAL_QUERYARCHIVEANDCLIPSTATEBYMATERIALID_EXCEPTION = 18229;

        /// <summary>
        /// 在GetMaterialListFolders过程中发生错误: 18230
        /// </summary>
        public const int GLOBALDICT_CODE_FILL_MATERIAL_GETMATERIALLISTFOLDERS_EXCEPTION = 18230;

        /// <summary>
        /// 在GetMLFolderByID过程中发生错误: 18231
        /// </summary>
        public const int GLOBALDICT_CODE_FILL_MATERIAL_GETMLFOLDERBYID_EXCEPTION = 18231;

        /// <summary>
        /// 在GetNotProcessMsgs过程中发生错误: 18232
        /// </summary>
        public const int GLOBALDICT_CODE_FILL_MATERIAL_GETNOTPROCESSMSGS_EXCEPTION = 18232;

        /// <summary>
        /// 在GetNeedRetryFailedMsg过程中发生错误: 18233
        /// </summary>
        public const int GLOBALDICT_CODE_FILL_MATERIAL_GETNEEDRETRYFAILEDMSG_EXCEPTION = 18233;

        /// <summary>
        /// 在LoadAllDevices过程中发生错误: 18234
        /// </summary>
        public const int GLOBALDICT_CODE_FILL_DEVICE_LOADALLDEVICES_EXCEPTION = 18234;

        /// <summary>
        /// 更新设备信息时，设备信息为空: 18235
        /// </summary>
        public const int GLOBALDICT_CODE_IN_UPDATEDEVICES_DATASET_IS_NULL = 18235;

        /// <summary>
        /// 更新设备信息时，发生数据库并发冲突: 18236
        /// </summary>
        public const int GLOBALDICT_CODE_IN_UPDATEDEVICES_CONCURRENCY_EXCEPTION = 18236;

        /// <summary>
        /// 更新设备信息时，操作数据库失败: 18237
        /// </summary>
        public const int GLOBALDICT_CODE_IN_UPDATEDEVICES_UPDATE_FAILED = 18237;

        /// <summary>
        /// 执行数据库Reader失败: 18238
        /// </summary>
        public const int GLOBALDICT_CODE_EXECUTE_READER_FAILED = 18238;

        /// <summary>
        /// 在GetMSVChannelStateByIDArry过程中，读取数据失败: 18239
        /// </summary>
        public const int GLOBALDICT_CODE_IN_GETMSVCHANNELSTATEBYIDARRY_READ_DATA_FAILED = 18239;

        /// <summary>
        /// 在GetMSVChannelState过程中，读取数据失败: 18240
        /// </summary>
        public const int GLOBALDICT_CODE_IN_GETMSVCHANNELSTATE_READ_DATA_FAILED = 18240;

        /// <summary>
        /// 在GetUsableUploadChannelList过程中发生错误: 18241
        /// </summary>
        public const int GLOBALDICT_CODE_FILL_DEVICE_GETUSABLEUPLOADCHANNELLIST_EXCEPTION = 18241;

        /// <summary>
        /// 在GetUsableUploadChannelList过程中，读取数据失败: 18242
        /// </summary>
        public const int GLOBALDICT_CODE_IN_GETUSABLEUPLOADCHANNELLIST_READ_DATA_FAILED = 18242;

        /// <summary>
        /// 在GetUploadMode过程中，读取数据失败: 18243
        /// </summary>
        public const int GLOBALDICT_CODE_IN_GETUPLOADMODE_READ_DATA_FAILED = 18243;

        /// <summary>
        /// 在GetUploadChannnelList中，填充数据集失败: 18244
        /// </summary>
        public const int GLOBALDICT_CODE_IN_GETUPLOADCHANNNELLIST_FILL_DATASET_EXCEPTION = 18244;

        /// <summary>
        /// 在GetChannelSignalSrc过程中，执行数据库操作失败: 18245
        /// </summary>
        public const int GLOBALDICT_CODE_IN_GETCHANNELSIGNALSRC_EXECUTE_SCALAR_FAILED = 18245;

        /// <summary>
        /// 在GetAllChannelState过程中，读取数据失败: 18246
        /// </summary>
        public const int GLOBALDICT_CODE_IN_GETALLCHANNELSTATE_READ_DATA_FAILED = 18246;

        /// <summary>
        /// 在GetParamTypeByChannleID过程中，读取数据失败: 18247
        /// </summary>
        public const int GLOBALDICT_CODE_IN_GETPARAMTYPEBYCHANNLEID_READ_DATA_FAILED = 18247;

        /// <summary>
        /// 在GetParamTypeByInport过程中，读取数据失败: 18248
        /// </summary>
        public const int GLOBALDICT_CODE_IN_GETPARAMTYPEBYINPORT_READ_DATA_FAILED = 18248;

        /// <summary>
        /// 在GetAllSignalGroupInfo过程中，读取数据失败: 18249
        /// </summary>
        public const int GLOBALDICT_CODE_IN_GETALLSIGNALGROUPINFO_READ_DATA_FAILED = 18249;

        /// <summary>
        /// 在GetAllSignalGroup过程中，读取数据失败: 18250
        /// </summary>
        public const int GLOBALDICT_CODE_IN_GETALLSIGNALGROUP_READ_DATA_FAILED = 18250;

        /// <summary>
        /// 在GetAllGPIDevices过程中，读取数据失败: 18251
        /// </summary>
        public const int GLOBALDICT_CODE_IN_GETALLGPIDEVICES_READ_DATA_FAILED = 18251;

        /// <summary>
        /// 在GetGPIMapInfoByGPIID过程中，读取数据失败: 18252
        /// </summary>
        public const int GLOBALDICT_CODE_IN_GETGPIMAPINFOBYGPIID_READ_DATA_FAILED = 18252;

        /// <summary>
        /// 在GetAllChannel2SignalSrcMap过程中，读取数据失败: 18253
        /// </summary>
        public const int GLOBALDICT_CODE_IN_GETALLCHANNEL2SIGNALSRCMAP_READ_DATA_FAILED = 18253;

        /// <summary>
        /// 从数据库中取值时失败: 18254
        /// </summary>
        public const int GLOBALDICT_CODE_GETVALUESTRING_EXCEPTION = 18254;

        /// <summary>
        /// 在数据库中更新数值时失败: 18255
        /// </summary>
        public const int GLOBALDICT_CODE_SETVALUE_EXCEPTION = 18255;

        /// <summary>
        /// 在数据库中增加一条记录时失败: 18256
        /// </summary>
        public const int GLOBALDICT_CODE_ADDROW_EXCEPTION = 18256;

        /// <summary>
        /// 在数据库中移除一条记录时失败: 18257
        /// </summary>
        public const int GLOBALDICT_CODE_ERASEROW_EXCEPTION = 18257;

        /// <summary>
        /// 在数据库中提取公用数据时失败: 18258
        /// </summary>
        public const int GLOBALDICT_CODE_GETALL_EXCEPTION = 18258;

        /// <summary>
        /// 在获取默认制式时读取数据失败: 18259
        /// </summary>
        public const int GLOBALDICT_CODE_IN_GETDEFAULTSTC_READ_DATA_FAILED = 18259;

        /// <summary>
        /// 添加一个对象锁失败: 18260
        /// </summary>
        public const int GLOBALDICT_CODE_ADD_LOCK_ERROR = 18260;

        /// <summary>
        /// 解锁对象时，发生数据库读写错误: 18261
        /// </summary>
        public const int GLOBALDICT_CODE_IN_SETUNLOCKOBJECT_READ_DATA_FAILED = 18261;

        /// <summary>
        /// 解锁对象时，发生一般性错误: 18262
        /// </summary>
        public const int GLOBALDICT_CODE_IN_SETUNLOCKOBJECT_EXCEPTION = 18262;

        /// <summary>
        /// 超时解锁对象时，发生数据库读写错误: 18263
        /// </summary>
        public const int GLOBALDICT_CODE_IN_UNLOCKOBJECTTIMEOUT_READ_DATA_FAILED = 18263;

        /// <summary>
        /// 超时解锁对象时，发生一般性错误: 18264
        /// </summary>
        public const int GLOBALDICT_CODE_IN_UNLOCKOBJECTTIMEOUT_EXCEPTION = 18264;

        /// <summary>
        /// 判断通道是否被锁定时，发生数据库读写错误: 18265
        /// </summary>
        public const int GLOBALDICT_CODE_IN_ISCHANNELLOCK_READ_DATA_FAILED = 18265;

        /// <summary>
        /// 锁定通道失败: 18266
        /// </summary>
        public const int GLOBALDICT_CODE_LOCK_CHANNEL_FAILED = 18266;

        /// <summary>
        /// 周期任务下次执行的时间在1分钟之后: 18267
        /// </summary>
        public const int GLOBALDICT_CODE_NEXT_TIME_IS_MORE_THAN_1_MINUTE = 18267;

        /// <summary>
        /// 任务ID为空值: 18268
        /// </summary>
        public const int GLOBALDICT_CODE_TASKID_IS_NULL = 18268;

        /// <summary>
        /// 不能删除已完成的任务: 18269
        /// </summary>
        public const int GLOBALDICT_CODE_CAN_NOT_DELETE_THE_COMPLETE_TASK = 18269;

        /// <summary>
        /// 通道不可用！ \r\n 任务{0}会在{1}调度执行。: 18270
        /// </summary>
        public const int GLOBALDICT_CODE_CHANNEL_IS_NOT_AVAILABLE_TWOPARAM = 18270;


        /// <summary>
        /// 任务{0}会在{1}的时刻调度执行。: 18271
        /// </summary>
        public const int GLOBALDICT_CODE_SCHEDULED_TASK_WILL_BEGIN_AT_TWOPARAM = 18271;

        public const int GLOBALDICT_CODE_CANNOTMODIFYTASK_WHERE_FILING = 18272;
        public const int GLOBALDICT_CODE_PROGRAM_NAME_REPEATED = 18273;
        public const int GLOBALDICT_CODE_FILL_GETTASKMETADATA_EXCEPTION = 18274;
        #endregion

        #endregion
    }
}
