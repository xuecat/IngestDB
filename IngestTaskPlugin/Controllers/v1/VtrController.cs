namespace IngestTaskPlugin.Controllers.v1
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Threading.Tasks;
    using IngestDBCore;
    using IngestDBCore.Interface;
    using IngestTaskPlugin.Dto;
    using IngestTaskPlugin.Dto.OldResponse;
    using IngestTaskPlugin.Dto.Request;
    using IngestTaskPlugin.Dto.Response;
    using IngestTaskPlugin.Dto.Response.OldVtr;
    using IngestTaskPlugin.Dto.OldResponse.OldVtr;
    using IngestTaskPlugin.Managers;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.DependencyInjection;
    using Sobey.Core.Log;
    using IngestTaskPlugin.Dto.OldVtr;

    /// <summary>
    /// VTR磁带.
    /// </summary>
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "v1")]
    public class VtrController : ControllerBase
    {
        /// <summary>
        /// Defines the Logger.
        /// </summary>
        private readonly ILogger Logger = LoggerManager.GetLogger("VtrInfo");

        /// <summary>
        /// Defines the _VtrManage.
        /// </summary>
        private readonly VtrManager _VtrManage;

        private readonly IIngestGlobalInterface _globalInterface;

        /// <summary>
        /// Initializes a new instance of the <see cref="VtrController"/> class.
        /// </summary>
        /// <param name="vtrManager">VTR.</param>
        /// <param name="global">VTR.</param>
        public VtrController(VtrManager vtrManager, IIngestGlobalInterface global)
        {
            _VtrManage = vtrManager;
            _globalInterface = global;
        }

        /// <summary>
        /// 增加或修改一盘VTR磁带.
        /// </summary>
        /// <param name="TapeName">磁带名.</param>
        /// <param name="TapeDesc">磁带描述信息.</param>
        /// <param name="nTapeID">磁带ID.</param>
        /// <returns>extension字段为 nOutTapeID.</returns>
        [HttpGet("SetTapeInfo")]
        public async Task<TaskOldResponseMessage<int>> SetTapeInfo([FromQuery]string TapeName, [FromQuery]string TapeDesc, [FromQuery]int nTapeID)
        {
            TaskOldResponseMessage<int> response = new TaskOldResponseMessage<int>();
            try
            {
                response.extention = await _VtrManage.SetTapeInfoAsync(nTapeID, TapeName, TapeDesc);
            }
            catch (Exception e)//其他未知的异常，写异常日志
            {
                response.nCode = 0;
                if (e is SobeyRecException se)//sobeyexcep会自动打印错误
                {
                    response.message = se.ErrorCode.ToString();
                }
                else
                {
                    response.message = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}：error info:{e.Message}";
                    Logger.Error(response.message);
                }
            }
            return response;
        }

        /// <summary>
        /// 设置VTR与磁带的对应序列.
        /// </summary>
        /// <param name="nVtrID">vtr id.</param>
        /// <param name="nTapeID">磁带id.</param>
        /// <returns>标准返回.</returns>
        [HttpGet("SetVtrTapeMap")]
        public async Task<TaskOldResponseMessage> SetVtrTapeMap([FromQuery]int nVtrID, [FromQuery]int nTapeID)
        {
            TaskOldResponseMessage response = new TaskOldResponseMessage();
            try
            {
                await _VtrManage.SetVtrTapeMapAsync(nVtrID, nTapeID);
            }
            catch (Exception e)//其他未知的异常，写异常日志
            {
                response.nCode = 0;
                if (e is SobeyRecException se)//sobeyexcep会自动打印错误
                {
                    response.message = se.ErrorCode.ToString();
                }
                else
                {
                    response.message = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}：error info:{e.Message}";
                    Logger.Error(response.message);
                }
            }
            return response;
        }

        /// <summary>
        /// 获得所有的磁带信息.
        /// </summary>
        /// <returns>extension 为 VTRTapeInfo[] TapeArray.</returns>
        [HttpGet("GetAllTapeInfo")]
        public async Task<TaskOldResponseMessage<List<VTRTapeInfo>>> GetAllTapeInfo()
        {
            TaskOldResponseMessage<List<VTRTapeInfo>> response = new TaskOldResponseMessage<List<VTRTapeInfo>>();
            try
            {
                response.extention = await _VtrManage.GetAllTapeInfoAsync<VTRTapeInfo>();
                if (response.extention == null || response.extention.Count == 0)
                {
                    response.extention = (new VTRTapeInfo[1]).ToList();
                }
            }
            catch (Exception e)//其他未知的异常，写异常日志
            {
                response.nCode = 0;
                if (e is SobeyRecException se)//sobeyexcep会自动打印错误
                {
                    response.message = se.ErrorCode.ToString();
                }
                else
                {
                    response.message = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}：error info:{e.Message}";
                    Logger.Error(response.message);
                }
            }
            return response;
        }

        /// <summary>
        /// 获得VTRID对应的默认磁带ID.
        /// </summary>
        /// <param name="nVTRID">id.</param>
        /// <returns>extension 为 nTapeID.</returns>
        [HttpGet("GetVtrTapeItem")]
        public async Task<TaskOldResponseMessage<int>> GetVtrTapeItem([FromQuery]int nVTRID)
        {
            TaskOldResponseMessage<int> response = new TaskOldResponseMessage<int>();
            try
            {
                response.extention = await _VtrManage.GetVtrTapeItemAsync(nVTRID);
            }
            catch (Exception e)//其他未知的异常，写异常日志
            {
                response.nCode = 0;
                if (e is SobeyRecException se)//sobeyexcep会自动打印错误
                {
                    response.message = se.ErrorCode.ToString();
                }
                else
                {
                    response.message = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}：error info:{e.Message}";
                    Logger.Error(response.message);
                }
            }
            return response;
        }

        /// <summary>
        /// 根据ID获得磁带信息.
        /// </summary>
        /// <param name="nTapeID">id.</param>
        /// <returns>extention 为 VTRTapeInfo tapeInfo.</returns>
        [HttpGet("GetTapeInfoByID")]
        public async Task<TaskOldResponseMessage<VTRTapeInfo>> GetTapeInfoByID([FromQuery]int nTapeID)
        {
            TaskOldResponseMessage<VTRTapeInfo> response = new TaskOldResponseMessage<VTRTapeInfo>();
            try
            {
                response.extention = await _VtrManage.GetTapeInfoByIDAsync<VTRTapeInfo>(nTapeID);
                if (response.extention == null)
                {
                    response.message = string.Format("Can not find the tape.");
                    response.nCode = 0;
                }
            }
            catch (Exception e)//其他未知的异常，写异常日志
            {
                response.nCode = 0;
                if (e is SobeyRecException se)//sobeyexcep会自动打印错误
                {
                    response.message = se.ErrorCode.ToString();
                }
                else
                {
                    response.message = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}：error info:{e.Message}";
                    Logger.Error(response.message);
                }
            }
            return response;
        }

        /// <summary>
        /// 设置VTR任务信息，VTRTaskID不存在则添加,否则则修改.
        /// </summary>
        /// <param name="UploadTask">上传任务信息.</param>
        /// <returns>.</returns>
        [HttpPost("SetVTRUploadTaskInfo")]
        public async Task<TaskOldResponseMessage<int>> SetVTRUploadTaskInfo([FromBody] VTRUploadTaskInfo UploadTask)
        {
            TaskOldResponseMessage<int> response = new TaskOldResponseMessage<int>();
            try
            {
                if (UploadTask.nVtrID <= 0)
                {
                    response.message = "[VTRUPLOAD]VtrID is Invaild ";
                    response.nCode = 0;
                    return response;
                }
                response.extention = await _VtrManage.SetVTRUploadTaskInfoAsync(UploadTask);
                
                if (_globalInterface != null)
                {
                    GlobalInternals re = new GlobalInternals() { Funtype = IngestDBCore.GlobalInternals.FunctionType.SetGlobalState, State = ClientOperLabelName.VTR_UPLOAD_ModifyTask };
                    var response1 = await _globalInterface.SubmitGlobalCallBack(re);
                    if (response1.Code != ResponseCodeDefines.SuccessCode)
                    {
                        Logger.Error("SetGlobalState modtask error");
                    }
                }
            }
            catch (Exception e)//其他未知的异常，写异常日志
            {
                response.nCode = 0;
                response.extention = -1;
                if (e is SobeyRecException se)//sobeyexcep会自动打印错误
                {
                    response.message = se.ErrorCode.ToString();
                }
                else
                {
                    response.message = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}：error info:{e.Message}";
                    Logger.Error(response.message);
                }
            }
            return response;
        }

        /// <summary>
        ///  设置VTR任务元数据
        /// </summary>
        [HttpPost("SetVtrTaskMetaData")]
        public async Task<TaskOldResponseMessage> SetVtrTaskMetaData([FromQuery] int lVtrTaskID, [FromQuery] int Type, [FromBody] string strMetaData)
        {
            TaskOldResponseMessage response = new TaskOldResponseMessage();
            if (lVtrTaskID <= 0)
            {
                response.message = "VtrTaskID is Invaild ";
                response.nCode = 0;
                return response;
            }
            if (strMetaData == null)
            {
                response.message = "VtrMetaData is Invaild ";
                response.nCode = 0;
                return response;
            }
            if (strMetaData.Length <= 0)
            {
                response.message = "VtrMetaData is Invaild ";
                response.nCode = 0;
                return response;
            }
            try
            {
                await _VtrManage.SetVBUTasksMetadatasAsync(lVtrTaskID, (MetaDataType)Type, strMetaData);
            }
            catch (Exception e)//其他未知的异常，写异常日志
            {
                response.nCode = 0;
                if (e is SobeyRecException se)//sobeyexcep会自动打印错误
                {
                    response.message = se.ErrorCode.ToString();
                }
                else
                {
                    response.message = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}：error info:{e.Message}";
                    Logger.Error(response.message);
                }
            }
            return response;
        }

        /// <summary>
        ///  查询TR上载任务
        /// </summary>
        /// <param name="Condition"></param>
        /// <returns></returns>
        [HttpPost("QueryVTRUploadTaskInfo")]
        public async Task<TaskOldResponseMessage<List<VTRUploadTaskInfo>>> QueryVTRUploadTaskInfo([FromBody] VTRUploadCondition Condition)
        {
            TaskOldResponseMessage<List<VTRUploadTaskInfo>> response = new TaskOldResponseMessage<List<VTRUploadTaskInfo>>();

            if (Condition == null)
            {
                return response;
            }
            try
            {
                Condition.state = Condition.state?.Where(a => a > (int)VTRUPLOADTASKSTATE.VTR_ALL_UPLOAD_STATE && a <= (int)VTRUPLOADTASKSTATE.VTR_UPLOAD_PRE_EXECUTE).ToList();
                response.extention = await _VtrManage.QueryVTRUploadTaskInfoAsync<VTRUploadTaskInfo, VTRUploadCondition>(Condition);
            }
            catch (Exception e)//其他未知的异常，写异常日志
            {
                response.nCode = 0;
                if (e is SobeyRecException se)//sobeyexcep会自动打印错误
                {
                    response.message = se.ErrorCode.ToString();
                }
                else
                {
                    response.message = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}：error info:{e.Message}";
                    Logger.Error(response.message);
                }
            }
            return response;
        }

        /// <summary>
        /// 通过TaskId查询VTR上载任务
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        [HttpGet("GetVTRUploadTaskById")]
        public async Task<TaskOldResponseMessage<VTRUploadTaskContent>> GetVTRUploadTaskById([FromQuery] int taskId)
        {
            TaskOldResponseMessage<VTRUploadTaskContent> response = new TaskOldResponseMessage<VTRUploadTaskContent>();
            try
            {
                if (taskId <= 0)
                {
                    response.message = "TaskId is Invalid.";
                    response.nCode = 0;
                    return response;
                }

                var task = await _VtrManage.GetVTRUploadTaskByIdAsync<VTRUploadTaskContent>(taskId);

                if (task == null)
                {
                    response.message = $"Can not find the task.TaskId = {taskId}";
                    response.nCode = 0;
                    return response;
                }
                response.extention = task;
            }
            catch (Exception e)//其他未知的异常，写异常日志
            {
                response.nCode = 0;
                if (e is SobeyRecException se)//sobeyexcep会自动打印错误
                {
                    response.message = se.ErrorCode.ToString();
                }
                else
                {
                    response.message = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}：error info:{e.Message}";
                    Logger.Error(response.message);
                }
            }
            return response;
        }

        /// <summary>
        /// 获取指定的VTR信息
        /// </summary>
        /// <param name="nVTRID"></param>
        /// <returns></returns>
        [HttpGet("GetVTRDetailInfoByID")]
        public async Task<TaskOldResponseMessage<VTRDetailInfo>> GetVTRDetailInfoByID([FromQuery] int nVTRID)
        {
            TaskOldResponseMessage<VTRDetailInfo> response = new TaskOldResponseMessage<VTRDetailInfo>();
            try
            {
                response.extention = await _VtrManage.GetVTRDetailInfoByIDAsync<VTRDetailInfo>(nVTRID);
            }
            catch (Exception e)//其他未知的异常，写异常日志
            {
                response.nCode = 0;
                if (e is SobeyRecException se)//sobeyexcep会自动打印错误
                {
                    response.message = se.ErrorCode.ToString();
                }
                else
                {
                    response.message = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}：error info:{e.Message}";
                    Logger.Error(response.message);
                }
            }
            return response;
        }

        /// <summary>
        /// 获取VTR状态
        /// </summary>
        /// <param name="nVtrID"></param>
        /// <returns></returns>
        [HttpGet("GetVtrState")]
        public async Task<TaskOldResponseMessage<VtrState>> GetVtrState([FromQuery] int nVtrID)
        {
            TaskOldResponseMessage<VtrState> response = new TaskOldResponseMessage<VtrState>();
            if (nVtrID <= 0)
            {
                response.message = "VtrID< is Invaild ";
                response.nCode = 0;
                return response;
            }
            try
            {
                response.extention = await _VtrManage.GetVtrStateAsync(nVtrID);
            }
            catch (Exception e)//其他未知的异常，写异常日志
            {
                response.nCode = 0;
                if (e is SobeyRecException se)//sobeyexcep会自动打印错误
                {
                    response.message = se.ErrorCode.ToString();
                }
                else
                {
                    response.message = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}：error info:{e.Message}";
                    Logger.Error(response.message);
                }
            }
            return response;
        }

        /// <summary>
        /// 根据任务ID更新任务状态
        /// </summary>
        /// <param name="lVtrTaskID"></param>
        /// <param name="lTaskState"></param>
        /// <returns></returns>
        [HttpGet("UpdateUploadTaskState")]
        public async Task<TaskOldResponseMessage> UpdateUploadTaskState([FromQuery] int lVtrTaskID, [FromQuery] int lTaskState)
        {
            TaskOldResponseMessage response = new TaskOldResponseMessage();
            if (lVtrTaskID <= 0)
            {
                response.message = "VtrTaskID is Invaild ";
                response.nCode = 0;
                return response;
            }
            if (lTaskState < 0)
            {
                response.message = "VtrTaskState is Invaild ";
                response.nCode = 0;
                return response;
            }
            try
            {
                await _VtrManage.UpdateUploadTaskStateAsync(lVtrTaskID, (VTRUPLOADTASKSTATE)lTaskState);
            }
            catch (Exception e)//其他未知的异常，写异常日志
            {
                response.nCode = 0;
                if (e is SobeyRecException se)//sobeyexcep会自动打印错误
                {
                    response.message = se.ErrorCode.ToString();
                }
                else
                {
                    response.message = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}：error info:{e.Message}";
                    Logger.Error(response.message);
                }
            }
            return response;
        }

        /// <summary>
        /// 获取所有vtr信息
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetUsableVtrList")]
        public async Task<TaskOldResponseMessage<List<VTRDetailInfo>>> GetUsableVtrList()
        {
            TaskOldResponseMessage<List<VTRDetailInfo>> response = new TaskOldResponseMessage<List<VTRDetailInfo>>();
            try
            {
                response.extention = await _VtrManage.GetUsableVtrListAsync<VTRDetailInfo>();
            }
            catch (Exception e)//其他未知的异常，写异常日志
            {
                response.nCode = 0;
                if (e is SobeyRecException se)//sobeyexcep会自动打印错误
                {
                    response.message = se.ErrorCode.ToString();
                }
                else
                {
                    response.message = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}：error info:{e.Message}";
                    Logger.Error(response.message);
                }
            }
            return response;
        }

        /// <summary>
        /// 获得需要即将要执行的VTR任务
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetNeedExecuteVTRUploadTasks")]
        public async Task<TaskOldResponseMessage<List<VTRUploadTaskContent>>> GetNeedExecuteVTRUploadTasks()
        {
            TaskOldResponseMessage<List<VTRUploadTaskContent>> response = new TaskOldResponseMessage<List<VTRUploadTaskContent>>();
            try
            {
                response.extention = await _VtrManage.GetNeedExecuteVTRUploadTasksAsync<VTRUploadTaskContent>();
            }
            catch (Exception e)//其他未知的异常，写异常日志
            {
                response.nCode = 0;
                if (e is SobeyRecException se)//sobeyexcep会自动打印错误
                {
                    response.message = se.ErrorCode.ToString();
                }
                else
                {
                    response.message = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}：error info:{e.Message}";
                    Logger.Error(response.message);
                }
            }
            return response;
        }

        /// <summary>
        /// 通过条件查询VTR上载任务
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        [HttpPost("GetVTRUploadTasks")]
        public async Task<TaskOldResponseMessage<List<VTRUploadTaskContent>>> GetVTRUploadTasks([FromBody] VTRUploadCondition condition)
        {
            TaskOldResponseMessage<List<VTRUploadTaskContent>> response = new TaskOldResponseMessage<List<VTRUploadTaskContent>>();
            if (condition == null)
            {
                response.message = "the param is null";
                response.nCode = 0;
                return response;
            }
            try
            {
                if (condition == null)
                {
                    response.message = "condition is null";
                    response.nCode = 0;
                    return response;
                }
                response.extention = await _VtrManage.GetVTRUploadTasksAsync<VTRUploadTaskContent, VTRUploadCondition>(condition);
            }
            catch (Exception e)//其他未知的异常，写异常日志
            {
                response.nCode = 0;
                if (e is SobeyRecException se)//sobeyexcep会自动打印错误
                {
                    response.message = se.ErrorCode.ToString();
                }
                else
                {
                    response.message = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}：error info:{e.Message}";
                    Logger.Error(response.message);
                }
            }
            return response;
        }

        /// <summary>
        /// 获得即将要执行的VTR任务，用来提示TapeID切换的
        /// </summary>
        /// <param name="minute">minute</param>
        /// <returns></returns>
        [HttpGet("GetVTRUploadTasks")]
        public async Task<TaskOldResponseMessage<List<VTRUploadTaskContent>>> GetWillExecuteVTRUploadTasks([FromQuery] int minute)
        {
            TaskOldResponseMessage<List<VTRUploadTaskContent>> response = new TaskOldResponseMessage<List<VTRUploadTaskContent>>();
            try
            {
                response.extention = await _VtrManage.GetWillExecuteVTRUploadTasksAsync<VTRUploadTaskContent>(minute);
            }
            catch (Exception e)//其他未知的异常，写异常日志
            {
                response.nCode = 0;
                if (e is SobeyRecException se)//sobeyexcep会自动打印错误
                {
                    response.message = se.ErrorCode.ToString();
                }
                else
                {
                    response.message = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}：error info:{e.Message}";
                    Logger.Error(response.message);
                }
            }
            return response;
        }

        /// <summary>
        /// 修改一个VTR任务的状态
        /// </summary>
        /// <param name="nTaskId"></param>
        /// <param name="vtrTaskState"></param>
        /// <param name="errorContent"></param>
        /// <returns></returns>
        [HttpGet("SetVTRUploadTaskState")]
        public async Task<TaskOldResponseMessage> SetVTRUploadTaskState([FromQuery] int nTaskId, [FromQuery] int vtrTaskState, [FromQuery] string errorContent)
        {
            TaskOldResponseMessage response = new TaskOldResponseMessage();
            if (nTaskId <= 0)
            {
                response.message = "TaskId Invalid.";
                response.nCode = 0;
                return response;
            }
            try
            {
                await _VtrManage.SetVTRUploadTaskStateAsync(nTaskId, (VTRUPLOADTASKSTATE)vtrTaskState, errorContent);
                
                if (_globalInterface != null)
                {
                    GlobalInternals re = new GlobalInternals() { Funtype = IngestDBCore.GlobalInternals.FunctionType.SetGlobalState, State = ClientOperLabelName.VTR_UPLOAD_ModifyTask };
                    var response1 = await _globalInterface.SubmitGlobalCallBack(re);
                    if (response1.Code != ResponseCodeDefines.SuccessCode)
                    {
                        Logger.Error("SetGlobalState modtask error");
                    }
                }
            }
            catch (Exception e)//其他未知的异常，写异常日志
            {
                response.nCode = 0;
                if (e is SobeyRecException se)//sobeyexcep会自动打印错误
                {
                    response.message = se.ErrorCode.ToString();
                }
                else
                {
                    response.message = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}：error info:{e.Message}";
                    Logger.Error(response.message);
                }
            }
            return response;
        }





        #region  Vtr set
        string no_err = "OK";
        /// <summary>
        /// 修改一个VTR上载任务
        /// </summary>
        /// 通知未写
        /// <param name="pIn">见定义</param>
        /// <returns></returns>
        [HttpPost("SetVTRUploadTask")]
        public async Task<TaskOldResponseMessage<VTRUploadTaskContent>> SetVTRUploadTask([FromBody] SetVTRUploadTask_in pIn)
        {
            TaskOldResponseMessage<VTRUploadTaskContent> res = new TaskOldResponseMessage<VTRUploadTaskContent>();
            res.message = no_err;
            try
            {
                res.message = no_err;
                if (pIn.vtrTask.nTaskId <= 0)
                {
                    res.message = "TaskId Invalid.";
                    res.nCode = 0;
                    return res;
                }
                //res.extention = await  _VtrManage.SetVTRUploadTask<VTRUploadTaskContent, SetVTRUploadTask_in>(pIn);
                
                res.extention = await  _VtrManage.SetVTRUploadTaskAsync<VTRUploadTaskContent, VTR_UPLOAD_MetadataPair>(pIn.vtrTask, pIn.metadatas, pIn.lMask);
                //res.extention = pIn.vtrTask;  //返回vtr任务，原本传递的引用，这里只能返回

                //GLOBALSERVICE.SetGlobalState2(ClientOperLabelName.VTR_UPLOAD_ModifyTask);
                
                if (_globalInterface != null)
                {
                    GlobalInternals re = new GlobalInternals() { Funtype = IngestDBCore.GlobalInternals.FunctionType.SetGlobalState, State = ClientOperLabelName.VTR_UPLOAD_ModifyTask };
                    var response1 = await _globalInterface.SubmitGlobalCallBack(re);
                    if (response1.Code != ResponseCodeDefines.SuccessCode)
                    {
                        Logger.Error("SetGlobalState modtask error");
                    }
                }

                //UdpNotify.SendToAll(UdpNotify.NotifyType_ModifyTask, pIn.vtrTask.nTaskId);

                res.nCode = (res.extention != null) ? 1 : 0;
            }
            catch (Exception ex)//其他未知的异常，写异常日志
            {
                Logger.Error("SetVTRUploadTask : " + ex.ToString());
                res.message = ex.Message;
                res.nCode = 0;
            }
            return res;

        }
        
        
        /// <summary>
        /// 通过ID获得上载任务信息
        /// </summary>
        /// <param name="nTaskID"></param>
        /// <returns>extention 为 VTRUploadTaskInfo</returns>
        [HttpGet("GetUploadTaskInfoByID")]
        public async Task<TaskOldResponseMessage<VTRUploadTaskInfo>> GetUploadTaskInfoByID([FromQuery, DefaultValue(313)] int nTaskID)
        {
            TaskOldResponseMessage<VTRUploadTaskInfo> res = new TaskOldResponseMessage<VTRUploadTaskInfo>();
            res.message = no_err;
            res.extention = null;
            try
            {
                res.extention = await _VtrManage.GetUploadTaskInfoByIDAsync<VTRUploadTaskInfo>(nTaskID);
                res.nCode = 1;
            }
            catch (Exception ex)//其他未知的异常，写异常日志
            {
                Logger.Error("GetUploadTaskInfoByID : " + ex.ToString());
                res.message = ex.Message;
                res.nCode = 0;
            }
            return res;
        }

        /// <summary>
        /// 提交一个vtr上载任务VTR_BUT_ErrorCode
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns> 扩展字段枚举VTR_BUT_ErrorCode的整数形式 </returns>
        [HttpGet("CommitVTRUploadTask")]
        public async Task<TaskOldResponseMessage<int>> CommitVTRUploadTask([FromQuery] int taskId)
        {
            TaskOldResponseMessage<int> res = new TaskOldResponseMessage<int>();
            res.message = no_err;
            res.extention = (int)VTR_BUT_ErrorCode.emNormal;

            try
            {
                res.message = no_err;

                if (taskId <= 0)
                {
                    res.message = "taskId is Invalid";
                    res.nCode = 0;
                    return res;
                }

                res.extention = await _VtrManage.CommitVTRBatchUploadTasksAsync(new List<int>() { taskId }, true);

                //GLOBALSERVICE.SetGlobalState2(ClientOperLabelName.VTR_UPLOAD_AddTask);
                if (_globalInterface != null)
                {
                    GlobalInternals re = new GlobalInternals() { Funtype = IngestDBCore.GlobalInternals.FunctionType.SetGlobalState, State = ClientOperLabelName.VTR_UPLOAD_AddTask };
                    var response1 = _globalInterface.SubmitGlobalCallBack(re).Result;
                    if (response1.Code != ResponseCodeDefines.SuccessCode)
                    {
                        Logger.Error("SetGlobalState modtask error");
                    }
                }
                //UdpNotify.SendToAll(UdpNotify.NotifyType_ModifyTask, taskId);
                if (res.extention == (int)VTR_BUT_ErrorCode.emNormal)
                    res.nCode = 1;
                else
                    res.nCode = 0;
            }

            catch (Exception ex)//其他未知的异常，写异常日志
            {
                Logger.Error("CommitVTRUploadTask : " + ex.ToString());
                res.message = ex.Message;
                res.nCode = 0;
            }
            return res;
        }

        /// <summary>
        /// 提交vtr批量上载任务
        /// </summary>
        /// <param name="param"></param>
        /// <returns>extention为整数，对应枚举 VTR_BUT_ErrorCode </returns>
        [HttpPost("api/vtr/CommitVTRBatchUploadTasks")]
        public async Task<TaskOldResponseMessage<int>> CommitVTRBatchUploadTasks([FromBody] CommitParam param)
        {
            TaskOldResponseMessage<int> res = new TaskOldResponseMessage<int>();
            res.message = no_err;
            res.extention = (int)VTR_BUT_ErrorCode.emNormal;
            if (param == null)
            {
                res.message = "ths param is null";
                return res;
            }

            try
            {
                res.message = no_err;
                if (param.taskIds == null || param.taskIds.Count <= 0)
                {
                    res.message = "taskIds is Invalid";
                    res.nCode = 0;
                    return res;
                }
                res.extention = await _VtrManage.CommitVTRBatchUploadTasksAsync(param.taskIds, param.ignoreWrong);
                //GLOBALSERVICE.SetGlobalState2(ClientOperLabelName.VTR_UPLOAD_AddTask);
                if (_globalInterface != null)
                {
                    GlobalInternals re = new GlobalInternals() { Funtype = IngestDBCore.GlobalInternals.FunctionType.SetGlobalState, State = ClientOperLabelName.VTR_UPLOAD_AddTask };
                    var response1 = _globalInterface.SubmitGlobalCallBack(re).Result;
                    if (response1.Code != ResponseCodeDefines.SuccessCode)
                    {
                        Logger.Error("SetGlobalState modtask error");
                    }
                }

                if (res.extention == (int)VTR_BUT_ErrorCode.emNormal)
                    res.nCode = 1;
                else
                    res.nCode = 0;
            }
            catch (Exception ex)//其他未知的异常，写异常日志
            {
                Logger.Error("CommitVTRBatchUploadTasks : " + ex.ToString());
                res.message = ex.Message;
                res.nCode = 0;
            }
            return res;
        }

        /// <summary>
        /// 删除VTR上载任务信息
        /// </summary>
        /// <param name="lVtrTaskID"></param>
        /// <returns></returns>
        [HttpGet("DelVTRUploadTaskInfo")]
        public async Task<TaskOldResponseMessage> DelVTRUploadTaskInfo(int lVtrTaskID)
        {
            TaskOldResponseMessage res = new TaskOldResponseMessage();
            if (lVtrTaskID <= 0)
            {
                res.message = "VtrTaskID is Invaild ";
                res.nCode = 0;
                return res;
            }
            try
            {
                res.message = no_err;
                //VTRACCESS.DelVTRUploadTaskInfo((int)lVtrTaskID);
                await _VtrManage.DeleteVtrUploadTaskAsync(lVtrTaskID);

                //GLOBALSERVICE.SetGlobalState2(ClientOperLabelName.VTR_UPLOAD_DeleteTask);
                if (_globalInterface != null)
                {
                    GlobalInternals re = new GlobalInternals() { Funtype = IngestDBCore.GlobalInternals.FunctionType.SetGlobalState, State = ClientOperLabelName.VTR_UPLOAD_DeleteTask };
                    var response1 = await _globalInterface.SubmitGlobalCallBack(re);
                    if (response1.Code != ResponseCodeDefines.SuccessCode)
                    {
                        Logger.Error("SetGlobalState modtask error");
                    }
                }
                res.nCode = 1;
            }
            catch (Exception ex)//其他未知的异常，写异常日志
            {
                Logger.Error("DelVTRUploadTaskInfo : " + ex.ToString());
                res.message = ex.Message;
                res.nCode = 0;
            }
            return res;
        }

        #endregion

        //增加一个VTR上载任务
        [HttpPost("AddVTRUploadTask")]
        public async Task<TaskOldResponseMessage<Dto.OldResponse.OldVtr.AddVTRUploadTask_out>> AddVTRUploadTask([FromBody]AddVTRUploadTask_in pIn)
        {
            TaskOldResponseMessage<AddVTRUploadTask_out> response = new TaskOldResponseMessage<AddVTRUploadTask_out>();
            response.extention = new AddVTRUploadTask_out();
            response.extention.errorCode = (int)VTR_BUT_ErrorCode.emNormal;
            if (pIn.vtrTask == null)
            {
                response.message = "vtrTask is null";
                response.nCode = 0;
                return response;
            }
            if (pIn.vtrTask.nTrimIn < 0 || pIn.vtrTask.nTrimOut < 0)
            {
                response.message = "Trimin or TrimOut is invalid ";
                response.nCode = 0;
                return response;
            }
            try
            {
                response.extention = await _VtrManage.AddVTRUploadTask<AddVTRUploadTask_out, VTRUploadTaskContent, VTR_UPLOAD_MetadataPair>(pIn.vtrTask, pIn.metadatas);
                if (_globalInterface != null)
                {
                    
                    var response1 = await _globalInterface.SubmitGlobalCallBack(new GlobalInternals() {
                        Funtype = IngestDBCore.GlobalInternals.FunctionType.SetGlobalState,
                        State = ClientOperLabelName.VTR_UPLOAD_ModifyTask
                    });
                    if (response1.Code != ResponseCodeDefines.SuccessCode)
                    {
                        Logger.Error("SetGlobalState modtask error");
                    }
                }

            }
            catch (Exception e)//其他未知的异常，写异常日志
            {
                response.nCode = 0;
                if (e is SobeyRecException se)//sobeyexcep会自动打印错误
                {
                    response.message = se.ErrorCode.ToString();
                }
                else
                {
                    response.message = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}：error info:{e.Message}";
                    Logger.Error(response.message);
                }
            }
            return response;
        }
        
        /// <summary>
        /// 获取VTR任务元数据
        /// </summary>
        /// <param name="lVtrTaskID">任务ID</param>
        /// <param name="Type">元数据类型</param>
        /// <returns> extention 为string元数据 </returns>
        [HttpGet("GetVtrTaskMetaData")]
        public async Task<TaskOldResponseMessage<string>> GetVtrTaskMetaData([FromQuery] int lVtrTaskID, [FromQuery] int Type)
        {
            TaskOldResponseMessage<string> response = new TaskOldResponseMessage<string>();
            response.extention = string.Empty;
            if (lVtrTaskID <= 0)
            {
                response.message = "VtrTaskID is Invaild ";
                response.nCode = 0;
                return response;
            }
            try
            {
                response.extention = await _VtrManage.GetVtrTaskMetaDataAsync(lVtrTaskID, Type);
            }
            catch (Exception e)//其他未知的异常，写异常日志
            {
                response.nCode = 0;
                if (e is SobeyRecException se)//sobeyexcep会自动打印错误
                {
                    response.message = se.ErrorCode.ToString();
                }
                else
                {
                    response.message = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}：error info:{e.Message}";
                    Logger.Error(response.message);
                }
            }
            return response;
        }

        /// <summary>
        /// 增加一系列VTR批量上载任务
        /// </summary>
        /// <param name="pIn">见声明</param>
        /// <returns>见声明</returns>
        [HttpPost]
        [Route("api/vtr/AddVTRBatchUploadTasks")]
        public async Task<TaskOldResponseMessage<VtrBatchUploadTask>> AddVTRBatchUploadTasks([FromBody] AddVTRBatchUploadTasks_in pIn)
        {
            TaskOldResponseMessage<VtrBatchUploadTask> res = new TaskOldResponseMessage<VtrBatchUploadTask>();
            res.extention = new VtrBatchUploadTask();
            res.message = no_err;
            res.extention.errorCode = VTR_BUT_ErrorCode.emNormal;
            res.extention.taskIds = null;
            if (pIn == null)
            {
                res.message = "ths param is null";
                return res;
            }
            try
            {
                res.message = no_err;
                if (pIn.vtrTasks == null || pIn.vtrTasks.Count <= 0)
                {
                    res.message = "vtrTasks is null";
                    res.nCode = 0;
                    return res;
                }

                res.extention = await _VtrManage.AddVTRBatchUploadTasksAsync<VtrBatchUploadTask,VTRUploadTaskContent, VTR_UPLOAD_MetadataPair>(pIn.vtrTasks, pIn.metadatas, pIn.ignoreWrong);
                res.nCode = res.extention.errorCode == VTR_BUT_ErrorCode.emNormal ? 1 : 0;
            }
            catch (Exception ex)//其他未知的异常，写异常日志
            {
                Logger.Error(ex.ToString());
                res.message = ex.Message;
                res.nCode = 0;
            }

            return res;
        }
    }
}
