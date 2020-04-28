namespace IngestTaskPlugin.Controllers.v1
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using IngestDBCore;
    using IngestDBCore.Interface;
    using IngestTaskPlugin.Dto;
    using IngestTaskPlugin.Dto.Request;
    using IngestTaskPlugin.Dto.Response;
    using IngestTaskPlugin.Dto.Response.OldVtr;
    using IngestTaskPlugin.Managers;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.DependencyInjection;
    using Sobey.Core.Log;

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

        /// <summary>
        /// Initializes a new instance of the <see cref="VtrController"/> class.
        /// </summary>
        /// <param name="vtrManager">VTR.</param>
        public VtrController(VtrManager vtrManager)
        {
            _VtrManage = vtrManager;
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
                if (e.GetType() == typeof(SobeyRecException))//sobeyexcep会自动打印错误
                {
                    SobeyRecException se = e as SobeyRecException;
                    response.message = se.ErrorCode.ToString();
                }
                else
                {
                    response.message = "OldGetTaskMetaData error info：" + e.ToString();
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
                if (e.GetType() == typeof(SobeyRecException))//sobeyexcep会自动打印错误
                {
                    SobeyRecException se = e as SobeyRecException;
                    response.message = se.ErrorCode.ToString();
                }
                else
                {
                    response.message = "OldGetTaskMetaData error info：" + e.ToString();
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
                response.extention = await _VtrManage.GetAllTapeInfoAsync();
                if (response.extention == null || response.extention.Count == 0)
                {
                    response.extention = (new VTRTapeInfo[1]).ToList();
                }
            }
            catch (Exception e)//其他未知的异常，写异常日志
            {
                response.nCode = 0;
                if (e.GetType() == typeof(SobeyRecException))//sobeyexcep会自动打印错误
                {
                    SobeyRecException se = e as SobeyRecException;
                    response.message = se.ErrorCode.ToString();
                }
                else
                {
                    response.message = "OldGetTaskMetaData error info：" + e.ToString();
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
                if (e.GetType() == typeof(SobeyRecException))//sobeyexcep会自动打印错误
                {
                    SobeyRecException se = e as SobeyRecException;
                    response.message = se.ErrorCode.ToString();
                }
                else
                {
                    response.message = "OldGetTaskMetaData error info：" + e.ToString();
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
                response.extention = await _VtrManage.GetTapeInfoByIDAsync(nTapeID);
                if (response.extention == null)
                {
                    response.message = string.Format("Can not find the tape.");
                    response.nCode = 0;
                }
            }
            catch (Exception e)//其他未知的异常，写异常日志
            {
                response.nCode = 0;
                if (e.GetType() == typeof(SobeyRecException))//sobeyexcep会自动打印错误
                {
                    SobeyRecException se = e as SobeyRecException;
                    response.message = se.ErrorCode.ToString();
                }
                else
                {
                    response.message = "OldGetTaskMetaData error info：" + e.ToString();
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
                var _globalinterface = ApplicationContext.Current.ServiceProvider.GetRequiredService<IIngestGlobalInterface>();
                if (_globalinterface != null)
                {
                    GlobalInternals re = new GlobalInternals() { funtype = IngestDBCore.GlobalInternals.FunctionType.SetGlobalState, State = ClientOperLabelName.VTR_UPLOAD_ModifyTask };
                    var response1 = await _globalinterface.SubmitGlobalCallBack(re);
                    if (response1.Code != ResponseCodeDefines.SuccessCode)
                    {
                        Logger.Error("SetGlobalState modtask error");
                    }
                }
            }
            catch (Exception e)//其他未知的异常，写异常日志
            {
                response.nCode = 0;
                if (e.GetType() == typeof(SobeyRecException))//sobeyexcep会自动打印错误
                {
                    SobeyRecException se = e as SobeyRecException;
                    response.message = se.ErrorCode.ToString();
                }
                else
                {
                    response.message = "OldGetTaskMetaData error info：" + e.ToString();
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
                if (e.GetType() == typeof(SobeyRecException))//sobeyexcep会自动打印错误
                {
                    SobeyRecException se = e as SobeyRecException;
                    response.message = se.ErrorCode.ToString();
                }
                else
                {
                    response.message = "OldGetTaskMetaData error info：" + e.ToString();
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
                response.extention = await _VtrManage.QueryVTRUploadTaskInfoAsync(Condition);
            }
            catch (Exception e)//其他未知的异常，写异常日志
            {
                response.nCode = 0;
                if (e.GetType() == typeof(SobeyRecException))//sobeyexcep会自动打印错误
                {
                    SobeyRecException se = e as SobeyRecException;
                    response.message = se.ErrorCode.ToString();
                }
                else
                {
                    response.message = "OldGetTaskMetaData error info：" + e.ToString();
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

                List<VTRUploadTaskContent> vtrTasks = null;
                VTRUploadCondition condition = new VTRUploadCondition();
                condition.lTaskID = taskId;

                vtrTasks = await _VtrManage.GetVTRUploadTasksAsync(condition);

                if (vtrTasks == null || vtrTasks.Count <= 0 || vtrTasks[0] == null)
                {
                    response.message = "Can not find the task.TaskId = " + taskId.ToString();
                    response.nCode = 0;
                    return response;
                }
                response.extention = vtrTasks[0];
            }
            catch (Exception e)//其他未知的异常，写异常日志
            {
                response.nCode = 0;
                if (e.GetType() == typeof(SobeyRecException))//sobeyexcep会自动打印错误
                {
                    SobeyRecException se = e as SobeyRecException;
                    response.message = se.ErrorCode.ToString();
                }
                else
                {
                    response.message = "OldGetTaskMetaData error info：" + e.ToString();
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
                response.extention = await _VtrManage.GetVTRDetailInfoByID(nVTRID);
            }
            catch (Exception e)//其他未知的异常，写异常日志
            {
                response.nCode = 0;
                if (e.GetType() == typeof(SobeyRecException))//sobeyexcep会自动打印错误
                {
                    SobeyRecException se = e as SobeyRecException;
                    response.message = se.ErrorCode.ToString();
                }
                else
                {
                    response.message = "OldGetTaskMetaData error info：" + e.ToString();
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
                if (e.GetType() == typeof(SobeyRecException))//sobeyexcep会自动打印错误
                {
                    SobeyRecException se = e as SobeyRecException;
                    response.message = se.ErrorCode.ToString();
                }
                else
                {
                    response.message = "OldGetTaskMetaData error info：" + e.ToString();
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
        [HttpGet("vtr/UpdateUploadTaskState")]
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
                await _VtrManage.UpdateUploadTaskStateAsync(lVtrTaskID, lTaskState);
            }
            catch (Exception e)//其他未知的异常，写异常日志
            {
                response.nCode = 0;
                if (e.GetType() == typeof(SobeyRecException))//sobeyexcep会自动打印错误
                {
                    SobeyRecException se = e as SobeyRecException;
                    response.message = se.ErrorCode.ToString();
                }
                else
                {
                    response.message = "OldGetTaskMetaData error info：" + e.ToString();
                    Logger.Error(response.message);
                }
            }
            return response;
        }
    }
}
