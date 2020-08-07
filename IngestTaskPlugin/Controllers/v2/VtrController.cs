namespace IngestTaskPlugin.Controllers.v2
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using IngestDBCore;
    using IngestDBCore.Basic;
    using IngestDBCore.Interface;
    using IngestTaskPlugin.Dto;
    using IngestTaskPlugin.Dto.OldResponse;
    using IngestTaskPlugin.Dto.Request;
    using IngestTaskPlugin.Dto.Response;
    using IngestTaskPlugin.Dto.Response.OldVtr;
    using IngestTaskPlugin.Managers;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.ModelBinding;
    using Microsoft.Extensions.DependencyInjection;
    using Sobey.Core.Log;
    using VTRUploadTaskInfoRequest = IngestTaskPlugin.Dto.Response.VTRUploadTaskInfoResponse;

    /// <summary>
    /// VTR磁带.
    /// </summary>
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("2.0")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "v2")]
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

        [HttpGet("test")]
        public ResponseMessage TestMethod()
        {
            return new ResponseMessage();
        }

        /// <summary>
        /// 增加或修改一盘VTR磁带.
        /// </summary>
        /// <param name="tapeid">磁带ID<see cref="int"/>.</param>
        /// <param name="tapename">磁带名<see cref="string"/>.</param>
        /// <param name="tapedesc">磁带描述信息<see cref="string"/>.</param>
        /// <returns>磁带ID<see cref="Task{ResponseMessage{int}}"/>.</returns>
        [HttpPost("tapeinfo/{tapeid}")]
        public async Task<ResponseMessage<int>> SetTapeInfo([FromRoute, BindRequired, DefaultValue(1)]int tapeid,
                                                            [FromQuery, BindRequired, DefaultValue("tapeName")]string tapename,
                                                            [FromQuery, BindRequired, DefaultValue("tapeDesc")]string tapedesc)
        {
            ResponseMessage<int> response = new ResponseMessage<int>();
            try
            {
                response.Ext = await _VtrManage.SetTapeInfoAsync(tapeid, tapename, tapedesc);
                if (response.Ext <= 0)
                {
                    response.Code = ResponseCodeDefines.NotFound;
                    response.Msg = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}:error info: not find data!";
                }
            }
            catch (Exception e)//其他未知的异常，写异常日志
            {
                if (e is SobeyRecException se)//sobeyexcep会自动打印错误
                {
                    response.Code = se.ErrorCode.ToString();
                    response.Msg = se.Message;
                }
                else
                {
                    response.Code = ResponseCodeDefines.ServiceError;
                    response.Msg = "OldGetTaskMetaData error info:" + e.Message;
                    Logger.Error(response.Msg);
                }
            }
            return response;
        }

        /// <summary>
        /// 设置VTR与磁带的对应序列.
        /// </summary>
        /// <param name="vtrid">vtrId<see cref="int"/>.</param>
        /// <param name="tapeid">磁带id<see cref="int"/>.</param>
        /// <returns>是否设置成功<see cref="Task{ResponseMessage{bool}}"/>.</returns>
        [HttpPost("vtrtapemap")]
        public async Task<ResponseMessage<bool>> SetVtrTapeMap([FromQuery, BindRequired, DefaultValue(1)]int vtrid,
                                                               [FromQuery, BindRequired, DefaultValue(9)]int tapeid)
        {
            ResponseMessage<bool> response = new ResponseMessage<bool>();
            try
            {
                response.Ext = await _VtrManage.SetVtrTapeMapAsync(vtrid, tapeid);
            }
            catch (Exception e)//其他未知的异常，写异常日志
            {
                if (e is SobeyRecException se)//sobeyexcep会自动打印错误
                {
                    response.Code = se.ErrorCode.ToString();
                    response.Msg = se.Message;
                }
                else
                {
                    response.Code = ResponseCodeDefines.ServiceError;
                    response.Msg = "OldGetTaskMetaData error info:" + e.Message;
                    Logger.Error(response.Msg);
                }
            }
            return response;
        }

        /// <summary>
        /// 获得所有的磁带信息.
        /// </summary>
        /// <returns>所有的磁带信息<see cref="Task{ResponseMessage{List{VTRTapeInfoResponse}}}"/>.</returns>
        [HttpGet("vtrtapeinfo/all")]
        public async Task<ResponseMessage<List<VTRTapeInfoResponse>>> GetAllTapeInfo()
        {
            ResponseMessage<List<VTRTapeInfoResponse>> response = new ResponseMessage<List<VTRTapeInfoResponse>>();
            try
            {
                response.Ext = await _VtrManage.GetAllTapeInfoAsync<VTRTapeInfoResponse>();
                if (response.Ext == null || response.Ext.Count == 0)
                {
                    response.Ext = (new VTRTapeInfoResponse[1]).ToList();
                }
            }
            catch (Exception e)//其他未知的异常，写异常日志
            {
                if (e is SobeyRecException se)//sobeyexcep会自动打印错误
                {
                    response.Code = se.ErrorCode.ToString();
                    response.Msg = se.Message;
                }
                else
                {
                    response.Code = ResponseCodeDefines.ServiceError;
                    response.Msg = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}:error info:{e.Message}";
                    Logger.Error(response.Msg);
                }
            }
            return response;
        }

        /// <summary>
        /// 获得VTRID对应的默认磁带ID.
        /// </summary>
        /// <param name="vtrid">vtrId<see cref="int"/>.</param>
        /// <returns>默认磁带ID<see cref="Task{ResponseMessage{int}}"/>.</returns>
        [HttpGet("vtrtapeinfo/tapeid/{vtrid}")]
        public async Task<ResponseMessage<int>> GetVtrTapeItem([FromRoute, BindRequired, DefaultValue(1)]int vtrid)
        {
            ResponseMessage<int> response = new ResponseMessage<int>();
            try
            {
                response.Ext = await _VtrManage.GetVtrTapeItemAsync(vtrid);
                if (response.Ext <= 0)
                {
                    response.Code = ResponseCodeDefines.NotFound;
                    response.Msg = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}:error info: not find data!";
                }
            }
            catch (Exception e)//其他未知的异常，写异常日志
            {
                if (e is SobeyRecException se)//sobeyexcep会自动打印错误
                {
                    response.Code = se.ErrorCode.ToString();
                    response.Msg = se.Message;
                }
                else
                {
                    response.Code = ResponseCodeDefines.ServiceError;
                    response.Msg = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}:error info:{e.Message}";
                    Logger.Error(response.Msg);
                }
            }
            return response;
        }

        /// <summary>
        /// 根据ID获得磁带信息.
        /// </summary>
        /// <param name="tapeid">磁带Id<see cref="int"/>.</param>
        /// <returns>The 磁带信息<see cref="Task{ResponseMessage{VTRTapeInfo}}"/>.</returns>
        [HttpGet("vtrtapeinfo/{tapeid}")]
        public async Task<ResponseMessage<VTRTapeInfoResponse>> GetTapeInfoByID([FromRoute, BindRequired, DefaultValue(1)]int tapeid)
        {
            ResponseMessage<VTRTapeInfoResponse> response = new ResponseMessage<VTRTapeInfoResponse>();
            try
            {
                response.Ext = await _VtrManage.GetTapeInfoByIDAsync<VTRTapeInfoResponse>(tapeid);
                if (response.Ext == null)
                {
                    response.Msg = string.Format("Can not find the tape.");
                    response.Code = ResponseCodeDefines.ServiceError;
                }
            }
            catch (Exception e)//其他未知的异常，写异常日志
            {
                if (e is SobeyRecException se)//sobeyexcep会自动打印错误
                {
                    response.Code = se.ErrorCode.ToString();
                    response.Msg = se.Message;
                }
                else
                {
                    response.Code = ResponseCodeDefines.ServiceError;
                    response.Msg = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}:error info:{e.Message}";
                    Logger.Error(response.Msg);
                }
            }
            return response;
        }

        /// <summary>
        /// The 设置VTR任务信息，VTRTaskID不存在则添加,否则则修改.和add冲突，暂时改为put，但是该方法不仅有添加，也有修改
        /// </summary>
        /// <param name="request">上传任务信息. <see cref="VTRUploadTaskInfoRequest"/></param>
        /// <returns>The 影响的Vtr任务Id<see cref="Task{ResponseMessage{int}}"/>.</returns>
        [HttpPut("vtruploadtask")]
        public async Task<ResponseMessage<int>> SetVTRUploadTaskInfo([FromBody, BindRequired] VTRUploadTaskInfoRequest request)
        {
            ResponseMessage<int> response = new ResponseMessage<int>();
            try
            {
                if (request.VtrId <= 0)
                {
                    response.Msg = "[VTRUPLOAD]VtrID is Invaild ";
                    response.Code = ResponseCodeDefines.ServiceError;
                    return response;
                }
                response.Ext = await _VtrManage.SetVTRUploadTaskInfoAsync(request);
                if (response.Ext <= 0)
                {
                    response.Code = ResponseCodeDefines.NotFound;
                    response.Msg = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}:error info: not find data!";
                }
                if (_globalInterface != null)
                {
                    GlobalInternals re = new GlobalInternals()
                    {
                        Funtype = IngestDBCore.GlobalInternals.FunctionType.SetGlobalState,
                        State = ClientOperLabelName.VTR_UPLOAD_ModifyTask
                    };
                    var response1 = await _globalInterface.SubmitGlobalCallBack(re);
                    if (response1.Code != ResponseCodeDefines.SuccessCode)
                    {
                        Logger.Error("SetGlobalState modtask error");
                    }
                }
            }
            catch (Exception e)//其他未知的异常，写异常日志
            {
                if (e is SobeyRecException se)//sobeyexcep会自动打印错误
                {
                    response.Code = se.ErrorCode.ToString();
                    response.Msg = se.Message;
                }
                else
                {
                    response.Code = ResponseCodeDefines.ServiceError;
                    response.Msg = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}:error info:{e.Message}";
                    Logger.Error(response.Msg);
                }
            }
            return response;
        }

        /// <summary>
        /// The 设置VTR任务元数据.
        /// </summary>
        /// <param name="vtrtaskid">The 任务Id<see cref="int"/>.</param>
        /// <param name="request">The 元数据<see cref="VTRTaskMetaDataRequest"/>.</param>
        /// <returns>The <see cref="Task{ResponseMessage}"/>.</returns>
        [HttpPost("vtrtaskmetadata/{vtrtaskid}")]
        public async Task<ResponseMessage> SetVtrTaskMetaData([FromRoute, BindRequired, DefaultValue(1)] int vtrtaskid,
                                                              [FromBody, BindRequired] VTRTaskMetaDataRequest request)
        {
            ResponseMessage response = new ResponseMessage();
            if (vtrtaskid <= 0)
            {
                response.Msg = "VtrTaskID is Invaild ";
                response.Code = ResponseCodeDefines.ServiceError;
                return response;
            }
            if (request.MetaData == null)
            {
                response.Msg = "VtrMetaData is Invaild ";
                response.Code = ResponseCodeDefines.ServiceError;
                return response;
            }
            if (request.MetaData.Length <= 0)
            {
                response.Msg = "VtrMetaData is Invaild ";
                response.Code = ResponseCodeDefines.ServiceError;
                return response;
            }
            try
            {
                await _VtrManage.SetVBUTasksMetadatasAsync(vtrtaskid, request.Type, request.MetaData);
            }
            catch (Exception e)//其他未知的异常，写异常日志
            {
                if (e is SobeyRecException se)//sobeyexcep会自动打印错误
                {
                    response.Code = se.ErrorCode.ToString();
                    response.Msg = se.Message;
                }
                else
                {
                    response.Code = ResponseCodeDefines.ServiceError;
                    response.Msg = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}:error info:{e.Message}";
                    Logger.Error(response.Msg);
                }
            }
            return response;
        }

        /// <summary>
        /// The 查询VTR上载任务.
        /// </summary>
        /// <param name="conditionrequest">The 查询条件<see cref="VTRUploadConditionRequest"/>.</param>
        /// <returns>The VTR上载任务<see cref="Task{ResponseMessage{List{VTRUploadTaskInfoResponse}}}"/>.</returns>
        [HttpGet("vtruploadtask")]
        public async Task<ResponseMessage<List<VTRUploadTaskInfoResponse>>> QueryVTRUploadTaskInfo([FromQuery, BindRequired] VTRUploadConditionRequest conditionrequest)
        {
            ResponseMessage<List<VTRUploadTaskInfoResponse>> response = new ResponseMessage<List<VTRUploadTaskInfoResponse>>();
            if (conditionrequest == null)
            {
                return response;
            }
            try
            {
                conditionrequest.TaskState = conditionrequest.TaskState?.Where(a => a >
                    (int)VTRUPLOADTASKSTATE.VTR_ALL_UPLOAD_STATE &&
                    a <= (int)VTRUPLOADTASKSTATE.VTR_UPLOAD_PRE_EXECUTE)
                    .ToList();
                response.Ext = await _VtrManage.QueryVTRUploadTaskInfoAsync<VTRUploadTaskInfoResponse, VTRUploadConditionRequest>(conditionrequest);
                if (response.Ext != null || response?.Ext.Count <= 0)
                {
                    response.Code = ResponseCodeDefines.NotFound;
                    response.Msg = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}:error info: not find data!";
                }
            }
            catch (Exception e)//其他未知的异常，写异常日志
            {
                if (e is SobeyRecException se)//sobeyexcep会自动打印错误
                {
                    response.Code = se.ErrorCode.ToString();
                    response.Msg = se.Message;
                }
                else
                {
                    response.Code = ResponseCodeDefines.ServiceError;
                    response.Msg = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}:error info:{e.Message}";
                    Logger.Error(response.Msg);
                }
            }
            return response;
        }

        /// <summary>
        /// The 通过TaskId查询VTR上载任务 及 内容.
        /// </summary>
        /// <param name="taskid">The 任务Id<see cref="int"/>.</param>
        /// <returns>The VTR上载任务 及 内容<see cref="Task{ResponseMessage{VTRUploadTaskContentResponse}}"/>.</returns>
        [HttpGet("vtruploadtaskcontent/{taskid}")]
        public async Task<ResponseMessage<VTRUploadTaskContentResponse>> GetVTRUploadTaskById([FromRoute, BindRequired, DefaultValue(312)] int taskid)
        {
            ResponseMessage<VTRUploadTaskContentResponse> response = new ResponseMessage<VTRUploadTaskContentResponse>();
            try
            {
                if (taskid <= 0)
                {
                    response.Msg = "TaskId is Invalid.";
                    response.Code = ResponseCodeDefines.ServiceError;
                    return response;
                }

                var task = await _VtrManage.GetVTRUploadTaskByIdAsync<VTRUploadTaskContentResponse>(taskid);
                if (task == null)
                {
                    response.Msg = $"Can not find the task.TaskId = {taskid}";
                    response.Code = ResponseCodeDefines.ServiceError;
                    return response;
                }
                response.Ext = task;
            }
            catch (Exception e)//其他未知的异常，写异常日志
            {
                if (e is SobeyRecException se)//sobeyexcep会自动打印错误
                {
                    response.Code = se.ErrorCode.ToString();
                    response.Msg = se.Message;
                }
                else
                {
                    response.Code = ResponseCodeDefines.ServiceError;
                    response.Msg = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}:error info:{e.Message}";
                    Logger.Error(response.Msg);
                }
            }
            return response;
        }

        /// <summary>
        /// The 获取指定的VTR信息.
        /// </summary>
        /// <param name="vtrid">The vtrId<see cref="int"/>.</param>
        /// <returns>The VTR信息<see cref="Task{ResponseMessage{VTRDetailInfoResponse}}"/>.</returns>
        [HttpGet("vtrdetailinfo/{vtrid}")]
        public async Task<ResponseMessage<VTRDetailInfoResponse>> GetVTRDetailInfoByID([FromQuery, BindRequired, DefaultValue(313)] int vtrid)
        {
            ResponseMessage<VTRDetailInfoResponse> response = new ResponseMessage<VTRDetailInfoResponse>();
            try
            {
                response.Ext = await _VtrManage.GetVTRDetailInfoByIDAsync<VTRDetailInfoResponse>(vtrid);
            }
            catch (Exception e)//其他未知的异常，写异常日志
            {
                if (e is SobeyRecException se)//sobeyexcep会自动打印错误
                {
                    response.Code = se.ErrorCode.ToString();
                    response.Msg = se.Message;
                }
                else
                {
                    response.Code = ResponseCodeDefines.ServiceError;
                    response.Msg = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}:error info:{e.Message}";
                    Logger.Error(response.Msg);
                }
            }
            return response;
        }

        /// <summary>
        /// The 获取VTR状态.
        /// </summary>
        /// <param name="vtrid">The vtrId<see cref="int"/>.</param>
        /// <returns>The vtr状态<see cref="Task{ResponseMessage{VtrState}}"/>.</returns>
        [HttpGet("vtrdetailinfo/state/{vtrid}")]
        public async Task<ResponseMessage<VtrState>> GetVtrState([FromQuery, BindRequired, DefaultValue(313)] int vtrid)
        {
            ResponseMessage<VtrState> response = new ResponseMessage<VtrState>();
            if (vtrid <= 0)
            {
                response.Msg = "vtrId < is Invaild ";
                response.Code = ResponseCodeDefines.ServiceError;
                return response;
            }
            try
            {
                response.Ext = await _VtrManage.GetVtrStateAsync(vtrid);
            }
            catch (Exception e)//其他未知的异常，写异常日志
            {
                if (e is SobeyRecException se)//sobeyexcep会自动打印错误
                {
                    response.Code = se.ErrorCode.ToString();
                    response.Msg = se.Message;
                }
                else
                {
                    response.Code = ResponseCodeDefines.ServiceError;
                    response.Msg = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}:error info:{e.Message}";
                    Logger.Error(response.Msg);
                }
            }
            return response;
        }

        /// <summary>
        /// The 根据任务ID更新任务状态.
        /// </summary>
        /// <param name="vtrtaskid">The 任务Id<see cref="int"/>.</param>
        /// <param name="taskstate">The 任务状态<see cref="int"/>.</param>
        /// <returns>The 是否成功<see cref="Task{ResponseMessage{bool}}"/>.</returns>
        [HttpPost("uploadtaskstate/{vtrtaskid}")]
        public async Task<ResponseMessage<bool>> UpdateUploadTaskState([FromRoute, BindRequired, DefaultValue(313)] int vtrtaskid,
                                                                       [FromQuery, BindRequired, DefaultValue(0)] VTRUPLOADTASKSTATE taskstate)
        {
            ResponseMessage<bool> response = new ResponseMessage<bool>();
            if (vtrtaskid <= 0)
            {
                response.Msg = "VtrTaskID is Invaild ";
                response.Code = ResponseCodeDefines.ServiceError;
                return response;
            }
            if (taskstate < 0)
            {
                response.Msg = "VtrTaskState is Invaild ";
                response.Code = ResponseCodeDefines.ServiceError;
                return response;
            }
            try
            {
                response.Ext = await _VtrManage.UpdateUploadTaskStateAsync(vtrtaskid, taskstate);
            }
            catch (Exception e)//其他未知的异常，写异常日志
            {
                if (e is SobeyRecException se)//sobeyexcep会自动打印错误
                {
                    response.Code = se.ErrorCode.ToString();
                    response.Msg = se.Message;
                }
                else
                {
                    response.Code = ResponseCodeDefines.ServiceError;
                    response.Msg = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}:error info:{e.Message}";
                    Logger.Error(response.Msg);
                }
            }
            return response;
        }

        /// <summary>
        /// The 获取所有vtr信息.
        /// </summary>
        /// <returns>The 所有vtr信息<see cref="Task{ResponseMessage{List{VTRDetailInfoResponse}}}"/>.</returns>
        [HttpGet("vtrdetailinfo/usable")]
        public async Task<ResponseMessage<List<VTRDetailInfoResponse>>> GetUsableVtrList()
        {
            ResponseMessage<List<VTRDetailInfoResponse>> response = new ResponseMessage<List<VTRDetailInfoResponse>>();
            try
            {
                response.Ext = await _VtrManage.GetUsableVtrListAsync<VTRDetailInfoResponse>();
                if (response.Ext != null || response?.Ext.Count <= 0)
                {
                    response.Code = ResponseCodeDefines.NotFound;
                    response.Msg = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}:error info: not find data!";
                }
            }
            catch (Exception e)//其他未知的异常，写异常日志
            {
                if (e is SobeyRecException se)//sobeyexcep会自动打印错误
                {
                    response.Code = se.ErrorCode.ToString();
                    response.Msg = se.Message;
                }
                else
                {
                    response.Code = ResponseCodeDefines.ServiceError;
                    response.Msg = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}:error info:{e.Message}";
                    Logger.Error(response.Msg);
                }
            }
            return response;
        }

        /// <summary>
        /// The 获得需要即将要执行的VTR上载任务.
        /// </summary>
        /// <returns>The VTR上载任务<see cref="Task{ResponseMessage{List{VTRUploadTaskContentResponse}}}"/>.</returns>
        [HttpGet("vtruploadtaskcontent/needexecute")]
        public async Task<ResponseMessage<List<VTRUploadTaskContentResponse>>> GetNeedExecuteVTRUploadTasks()
        {
            ResponseMessage<List<VTRUploadTaskContentResponse>> response = new ResponseMessage<List<VTRUploadTaskContentResponse>>();
            try
            {
                response.Ext = await _VtrManage.GetNeedExecuteVTRUploadTasksAsync<VTRUploadTaskContentResponse>();
                if (response.Ext != null || response?.Ext.Count <= 0)
                {
                    response.Code = ResponseCodeDefines.NotFound;
                    response.Msg = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}:error info: not find data!";
                }
            }
            catch (Exception e)//其他未知的异常，写异常日志
            {
                if (e is SobeyRecException se)//sobeyexcep会自动打印错误
                {
                    response.Code = se.ErrorCode.ToString();
                    response.Msg = se.Message;
                }
                else
                {
                    response.Code = ResponseCodeDefines.ServiceError;
                    response.Msg = "OldGetTaskMetaData error info:" + e.Message;
                    Logger.Error(response.Msg);
                }
            }
            return response;
        }

        /// <summary>
        /// The 通过条件查询VTR上载任务.
        /// </summary>
        /// <param name="conditionRequest">The 查询条件<see cref="VTRUploadConditionRequest"/>.</param>
        /// <returns>The VTR上载任务<see cref="Task{ResponseMessage{List{VTRUploadTaskContentResponse}}}"/>.</returns>
        [HttpGet("vtruploadtaskcontent")]
        public async Task<ResponseMessage<List<VTRUploadTaskContentResponse>>> GetVTRUploadTasks([FromQuery, BindRequired] VTRUploadConditionRequest conditionRequest)
        {
            ResponseMessage<List<VTRUploadTaskContentResponse>> response = new ResponseMessage<List<VTRUploadTaskContentResponse>>();
            if (conditionRequest == null)
            {
                response.Msg = "the param is null";
                response.Code = ResponseCodeDefines.ServiceError;
                return response;
            }
            try
            {
                response.Ext = await _VtrManage.GetVTRUploadTasksAsync<VTRUploadTaskContentResponse, VTRUploadConditionRequest>(conditionRequest);
                if (response.Ext != null || response?.Ext.Count <= 0)
                {
                    response.Code = ResponseCodeDefines.NotFound;
                    response.Msg = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}:error info: not find data!";
                }
            }
            catch (Exception e)//其他未知的异常，写异常日志
            {
                if (e is SobeyRecException se)//sobeyexcep会自动打印错误
                {
                    response.Code = se.ErrorCode.ToString();
                    response.Msg = se.Message;
                }
                else
                {
                    response.Code = ResponseCodeDefines.ServiceError;
                    response.Msg = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}:error info:{e.Message}";
                    Logger.Error(response.Msg);
                }
            }
            return response;
        }

        /// <summary>
        /// The 获得即将要执行的VTR任务，用来提示TapeID切换的.
        /// </summary>
        /// <param name="minute">The minute<see cref="int"/>.</param>
        /// <returns>The 即将要执行的VTR任务<see cref="Task{ResponseMessage{List{VTRUploadTaskContentResponse}}}"/>.</returns>
        [HttpGet("vtruploadtaskcontent/willexecute/{minute}")]
        public async Task<ResponseMessage<List<VTRUploadTaskContentResponse>>> GetWillExecuteVTRUploadTasks([FromRoute, BindRequired] int minute)
        {
            ResponseMessage<List<VTRUploadTaskContentResponse>> response = new ResponseMessage<List<VTRUploadTaskContentResponse>>();
            try
            {
                response.Ext = await _VtrManage.GetWillExecuteVTRUploadTasksAsync<VTRUploadTaskContentResponse>(minute);
                if (response.Ext != null || response?.Ext.Count <= 0)
                {
                    response.Code = ResponseCodeDefines.NotFound;
                    response.Msg = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}:error info: not find data!";
                }
            }
            catch (Exception e)//其他未知的异常，写异常日志
            {
                if (e is SobeyRecException se)//sobeyexcep会自动打印错误
                {
                    response.Code = se.ErrorCode.ToString();
                    response.Msg = se.Message;
                }
                else
                {
                    response.Code = ResponseCodeDefines.ServiceError;
                    response.Msg = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}:error info:{e.Message}";
                    Logger.Error(response.Msg);
                }
            }
            return response;
        }

        /// <summary>
        /// The 根据id修改一个VTR任务的状态.
        /// </summary>
        /// <param name="taskid">The taskId<see cref="int"/>.</param>
        /// <param name="request">The request<see cref="VTRUploadStateRequest"/>.</param>
        /// <returns>The 是否修改成功<see cref="Task{ResponseMessage{bool}}"/>.</returns>
        [HttpPost("vtruploadtask/state/{taskid}")]
        public async Task<ResponseMessage<bool>> SetVTRUploadTaskState([FromRoute, BindRequired, DefaultValue(312)] int taskid,
                                                                       [FromBody, BindRequired] VTRUploadStateRequest request)
        {
            ResponseMessage<bool> response = new ResponseMessage<bool>();
            if (taskid <= 0)
            {
                response.Msg = "TaskId Invalid.";
                response.Code = ResponseCodeDefines.ServiceError;
                return response;
            }
            try
            {
                response.Ext = await _VtrManage.SetVTRUploadTaskStateAsync(taskid,
                                                                           request.TaskState,
                                                                           request.ErrorContent);
                if (response.Ext)
                {
                    if (_globalInterface != null)
                    {
                        GlobalInternals re = new GlobalInternals()
                        {
                            Funtype = IngestDBCore.GlobalInternals.FunctionType.SetGlobalState,
                            State = ClientOperLabelName.VTR_UPLOAD_ModifyTask
                        };
                        var response1 = await _globalInterface.SubmitGlobalCallBack(re);
                        if (response1.Code != ResponseCodeDefines.SuccessCode)
                        {
                            Logger.Error("SetGlobalState modtask error");
                        }
                    }
                }
            }
            catch (Exception e)//其他未知的异常，写异常日志
            {
                if (e is SobeyRecException se)//sobeyexcep会自动打印错误
                {
                    response.Code = se.ErrorCode.ToString();
                    response.Msg = se.Message;
                }
                else
                {
                    response.Code = ResponseCodeDefines.ServiceError;
                    response.Msg = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}:error info:{e.Message}";
                    Logger.Error(response.Msg);
                }
            }
            return response;
        }


        #region vtr set
        /// <summary>
        /// 通过ID获得上载任务信息
        /// </summary>
        /// <remarks>
        /// example:
        /// Get api/v2/vtr/uploadtaskinfo/{taskid}
        /// </remarks>
        /// <param name="taskid">任务Id</param>
        /// <returns>extention 为 VTRUploadTaskInfo</returns>
        [HttpGet("vtruploadtask/{taskid}")]
        public async Task<ResponseMessage<VTRUploadTaskInfoResponse>> GetUploadTaskInfoByID([FromRoute, BindRequired, DefaultValue(313)] int taskid)
        {
            ResponseMessage<VTRUploadTaskInfoResponse> response = new ResponseMessage<VTRUploadTaskInfoResponse>();
            try
            {
                response.Ext = await _VtrManage.GetUploadTaskInfoByIDAsync<VTRUploadTaskInfoResponse>(taskid);
                if (response.Ext != null)
                {
                    response.Code = ResponseCodeDefines.NotFound;
                    response.Msg = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}:error info: not find data!";
                }
            }
            catch (Exception e)//其他未知的异常，写异常日志
            {
                if (e is SobeyRecException se)//sobeyexcep会自动打印错误
                {
                    response.Code = se.ErrorCode.ToString();
                    response.Msg = se.Message;
                }
                else
                {
                    response.Code = ResponseCodeDefines.ServiceError;
                    response.Msg = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}:error info:{e.Message}";
                    Logger.Error(response.Msg);
                }
            }
            return response;
        }


        /// <summary>
        /// 修改一个VTR上载任务
        /// </summary>
        /// <remarks>
        /// example:
        /// Get api/v2/vtr/vtruploadtask/modify
        /// </remarks> 
        /// <param name="vtrtaskrequest">见定义<see cref="VTRUploadTaskRequest"/>.</param>
        /// <returns>The 是否修改成功<see cref="Task{ResponseMessage{VTRUploadTaskContentResponse}}"/>.</returns>
        [HttpPost("vtruploadtask/modify")]
        public async Task<ResponseMessage<VTRUploadTaskContentResponse>> SetVTRUploadTaskAsync([FromBody, BindRequired] VTRUploadTaskRequest vtrtaskrequest)
        {
            ResponseMessage<VTRUploadTaskContentResponse> response = new ResponseMessage<VTRUploadTaskContentResponse>();
            try
            {
                if (vtrtaskrequest.VtrTask.TaskId <= 0)
                {
                    response.Msg = "TaskId Invalid.";
                    response.Code = ResponseCodeDefines.ArgumentNullError;
                }
                //bool ret = VTRACCESS.SetVTRUploadTask(ref pIn.vtrTask, pIn.metadatas, pIn.lMask);
                //res.extention = pIn.vtrTask;  //返回vtr任务，原本传递的引用，这里只能返回

                response.Ext = await _VtrManage.SetVTRUploadTaskAsync<VTRUploadTaskContentResponse, VTRUploadMetadataPair>(vtrtaskrequest.VtrTask, vtrtaskrequest.Metadatas, vtrtaskrequest.Mask);
                if (response.Ext != null)
                {
                    response.Code = ResponseCodeDefines.NotFound;
                    response.Msg = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}:error info: not find data!";
                }
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
                //res.nCode = (ret == true) ? 1 : 0;
                response.Code = response.Ext != null ? ResponseCodeDefines.SuccessCode : ResponseCodeDefines.ServiceError;
            }
            catch (Exception e)//其他未知的异常，写异常日志
            {
                if (e.GetType() == typeof(SobeyRecException))//sobeyexcep会自动打印错误
                {
                    SobeyRecException se = e as SobeyRecException;
                    response.Code = se.ErrorCode.ToString();
                    response.Msg = se.Message;
                }
                else
                {
                    response.Code = ResponseCodeDefines.ServiceError;
                    response.Msg = "error info:" + e.Message;
                    Logger.Error("GetUserInfoByCode : " + response.Msg);
                }
            }
            return response;

        }


        /// <summary>
        /// 提交一个vtr上载任务VTR_BUT_ErrorCode
        /// </summary>
        /// <param name="taskid"></param>
        /// <returns> 扩展字段枚举VTR_BUT_ErrorCode的整数形式 </returns>
        [HttpPost("vtruploadtask/commit/{taskid}")]
        public async Task<ResponseMessage<int>> CommitVTRUploadTask([FromRoute, BindRequired, DefaultValue(1)] int taskid)
        {
            ResponseMessage<int> response = new ResponseMessage<int>();
            response.Ext = (int)VTR_BUT_ErrorCode.emNormal;
            if (response.Ext <= 0)
            {
                response.Code = ResponseCodeDefines.NotFound;
                response.Msg = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}:error info: not find data!";
            }
            try
            {
                if (taskid <= 0)
                {
                    response.Msg = "taskId is Invalid";
                    response.Code = ResponseCodeDefines.ArgumentNullError;
                }

                response.Ext = await _VtrManage.CommitVTRBatchUploadTasksAsync(new List<int>() { taskid }, true);//.CommitVTRUploadTask(taskId);

                //GLOBALSERVICE.SetGlobalState2(ClientOperLabelName.VTR_UPLOAD_AddTask);
                if (_globalInterface != null)
                {
                    GlobalInternals re = new GlobalInternals() { Funtype = IngestDBCore.GlobalInternals.FunctionType.SetGlobalState, State = ClientOperLabelName.VTR_UPLOAD_AddTask };
                    var response1 = await _globalInterface.SubmitGlobalCallBack(re);
                    if (response1.Code != ResponseCodeDefines.SuccessCode)
                    {
                        Logger.Error("SetGlobalState modtask error");
                    }
                }
                //UdpNotify.SendToAll(UdpNotify.NotifyType_ModifyTask, taskId);

                response.Code = response.Ext == (int)VTR_BUT_ErrorCode.emNormal ? ResponseCodeDefines.SuccessCode : ResponseCodeDefines.ServiceError;
            }
            catch (Exception e)//其他未知的异常，写异常日志
            {
                if (e.GetType() == typeof(SobeyRecException))//sobeyexcep会自动打印错误
                {
                    SobeyRecException se = e as SobeyRecException;
                    response.Code = se.ErrorCode.ToString();
                    response.Msg = se.Message;
                }
                else
                {
                    response.Code = ResponseCodeDefines.ServiceError;
                    response.Msg = "error info:" + e.Message;
                    Logger.Error("GetUserInfoByCode : " + response.Msg);
                }
            }
            return response;
        }


        /// <summary>
        /// 提交vtr批量上载任务
        /// </summary>
        /// <param name="param"></param>
        /// <returns>extention为整数，对应枚举 VTR_BUT_ErrorCode </returns>
        [HttpPost("vtruploadtask/commit")]
        public async Task<ResponseMessage<int>> CommitVTRBatchUploadTasks([FromBody] CommitParamRequest param)
        {
            ResponseMessage<int> response = new ResponseMessage<int>();

            response.Ext = (int)VTR_BUT_ErrorCode.emNormal;
            if (response.Ext <= 0)
            {
                response.Code = ResponseCodeDefines.NotFound;
                response.Msg = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}:error info: not find data!";
            }
            if (param == null)
            {
                response.Msg = "ths param is null";
                return response;
            }

            try
            {
                if (param.taskids == null || param.taskids.Count <= 0)
                {
                    response.Msg = "taskIds is Invalid";
                    response.Code = ResponseCodeDefines.ArgumentNullError;
                    return response;
                }

                response.Ext = await _VtrManage.CommitVTRBatchUploadTasksAsync(param.taskids, param.ignorewrong);
                //GLOBALSERVICE.SetGlobalState2(ClientOperLabelName.VTR_UPLOAD_AddTask);
                if (_globalInterface != null)
                {
                    GlobalInternals re = new GlobalInternals() { Funtype = IngestDBCore.GlobalInternals.FunctionType.SetGlobalState, State = ClientOperLabelName.VTR_UPLOAD_AddTask };
                    var response1 = await _globalInterface.SubmitGlobalCallBack(re);
                    if (response1.Code != ResponseCodeDefines.SuccessCode)
                    {
                        Logger.Error("SetGlobalState modtask error");
                    }
                }
                //UdpNotify.SendToAll(UdpNotify.NotifyType_ModifyTask, taskId);

                response.Code = response.Ext == (int)VTR_BUT_ErrorCode.emNormal ? ResponseCodeDefines.SuccessCode : ResponseCodeDefines.ServiceError;
            }
            catch (Exception e)//其他未知的异常，写异常日志
            {
                if (e.GetType() == typeof(SobeyRecException))//sobeyexcep会自动打印错误
                {
                    SobeyRecException se = e as SobeyRecException;
                    response.Code = se.ErrorCode.ToString();
                    response.Msg = se.Message;
                }
                else
                {
                    response.Code = ResponseCodeDefines.ServiceError;
                    response.Msg = "error info:" + e.Message;
                    Logger.Error("GetUserInfoByCode : " + response.Msg);
                }
            }
            return response;
        }


        /// <summary>
        /// 增加一系列VTR批量上载任务
        /// </summary>
        /// <param name="pin">见声明</param>
        /// <returns>见声明</returns>
        [HttpPost("vtrbatchuploadtasks/add")]
        public async Task<ResponseMessage<VtrBatchUploadTaskResponse>> AddVTRBatchUploadTasks([FromBody]VTRBatchUploadTaskRequest pin)
        {
            ResponseMessage<VtrBatchUploadTaskResponse> response = new ResponseMessage<VtrBatchUploadTaskResponse>();
            response.Ext = new VtrBatchUploadTaskResponse();

            response.Ext.errorCode = VTR_BUT_ErrorCode.emNormal;
            response.Ext.taskIds = null;
            if (pin == null)
            {
                response.Msg = "ths param is null";
                return response;
            }
            try
            {
                if (pin.VtrTasks == null || pin.VtrTasks.Count <= 0)
                {
                    response.Msg = "vtrTasks is null";
                    response.Code = ResponseCodeDefines.ArgumentNullError;
                }

                response.Ext = await _VtrManage.AddVTRBatchUploadTasksAsync<VtrBatchUploadTaskResponse, VTRUploadTaskContentResponse, VTRUploadMetadataPair>(pin.VtrTasks, pin.Metadatas, pin.IgnoreWrong);
                if (response.Ext == null)
                {
                    response.Code = ResponseCodeDefines.NotFound;
                    response.Msg = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}:error info: not find data!";
                }
                response.Code = response.Ext.errorCode == VTR_BUT_ErrorCode.emNormal ? ResponseCodeDefines.SuccessCode : ResponseCodeDefines.ServiceError;
            }
            catch (Exception e)//其他未知的异常，写异常日志
            {
                if (e.GetType() == typeof(SobeyRecException))//sobeyexcep会自动打印错误
                {
                    SobeyRecException se = e as SobeyRecException;
                    response.Code = se.ErrorCode.ToString();
                    response.Msg = se.Message;
                }
                else
                {
                    response.Code = ResponseCodeDefines.ServiceError;
                    response.Msg = "error info:" + e.Message;
                    Logger.Error("GetUserInfoByCode : " + response.Msg);
                }
            }

            return response;
        }

        /// <summary>
        /// 删除VTR上载任务信息
        /// </summary>
        /// <param name="vtrtaskid"></param>
        /// <returns></returns>
        [HttpDelete("vtruploadtask/delete/{vtrtaskid}")]
        public async Task<ResponseMessage> DelVTRUploadTaskInfo(int vtrtaskid)
        {
            ResponseMessage response = new ResponseMessage();
            if (vtrtaskid <= 0)
            {
                response.Msg = "VtrTaskID is Invaild ";
                response.Code = ResponseCodeDefines.ArgumentNullError;
                return response;
            }
            try
            {
                await _VtrManage.DeleteVtrUploadTaskAsync(vtrtaskid);
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
                //res.nCode = 1;
                response.Code = ResponseCodeDefines.SuccessCode;
            }
            catch (Exception e)//其他未知的异常，写异常日志
            {
                if (e.GetType() == typeof(SobeyRecException))//sobeyexcep会自动打印错误
                {
                    SobeyRecException se = e as SobeyRecException;
                    response.Code = se.ErrorCode.ToString();
                    response.Msg = se.Message;
                }
                else
                {
                    response.Code = ResponseCodeDefines.ServiceError;
                    response.Msg = "error info:" + e.Message;
                    Logger.Error("GetUserInfoByCode : " + response.Msg);
                }
            }
            return response;
        }

        /// <summary>
        /// 获取VTR任务元数据
        /// </summary>
        /// <param name="vtrtaskid">任务ID</param>
        /// <param name="type">元数据类型</param>
        /// <returns> extention 为string元数据 </returns>
        [HttpGet("vtrtaskmetadata/{vtrtaskid}")]
        public async Task<ResponseMessage<string>> GetVtrTaskMetaData([FromRoute] int vtrtaskid, [FromQuery] int type)
        {
            ResponseMessage<string> response = new ResponseMessage<string>();
            response.Ext = string.Empty;
            if (vtrtaskid <= 0)
            {
                response.Msg = "VtrTaskID is Invaild ";
                response.Code = ResponseCodeDefines.ArgumentNullError;
                return response;
            }
            try
            {
                response.Msg = "OK";
                response.Ext = await _VtrManage.GetVtrTaskMetaDataAsync(vtrtaskid, type);
                if (string.IsNullOrEmpty(response.Ext))
                {
                    response.Code = ResponseCodeDefines.NotFound;
                    response.Msg = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}:error info: not find data!";
                }
                else
                    response.Code = ResponseCodeDefines.SuccessCode;
            }
            catch (Exception e)//其他未知的异常，写异常日志
            {
                if (e.GetType() == typeof(SobeyRecException))//sobeyexcep会自动打印错误
                {
                    SobeyRecException se = e as SobeyRecException;
                    response.Code = se.ErrorCode.ToString();
                    response.Msg = se.Message;
                }
                else
                {
                    response.Code = ResponseCodeDefines.ServiceError;
                    response.Msg = "error info:" + e.Message;
                    Logger.Error("GetVtrTaskMetaData : " + response.Msg);
                }
            }
            return response;
        }

        /// <summary>
        /// 添加一个vtrupload任务
        /// </summary>
        /// <param name="vtrtask">任务结构体</param>
        /// <returns> 任务新的结构体 </returns>
        [HttpPost("vtruploadtask")]
        public async Task<ResponseMessage<AddVTRUploadTaskResponse>> AddVTRUploadTask(VTRUploadTaskRequest vtrtask)
        {
            ResponseMessage<AddVTRUploadTaskResponse> response = new ResponseMessage<AddVTRUploadTaskResponse>();
            response.Ext = new AddVTRUploadTaskResponse();
            response.Msg = "OK";
            response.Ext.ErrorCode = (int)VTR_BUT_ErrorCode.emNormal;

            try
            {
                if (vtrtask.VtrTask == null)
                {
                    response.Msg = "vtrTask is null";
                    response.Code = ResponseCodeDefines.ArgumentNullError;
                    return response;
                }

                if (vtrtask.VtrTask.TrimIn < 0 || vtrtask.VtrTask.TrimOut < 0)
                {
                    response.Msg = "Trimin or TrimOut is invalid ";
                    response.Code = ResponseCodeDefines.ArgumentNullError;
                }

                response.Ext = await _VtrManage.AddVTRUploadTask<AddVTRUploadTaskResponse, VTRUploadTaskContentResponse, VTRUploadMetadataPair>(vtrtask.VtrTask, vtrtask.Metadatas);
                if (response.Ext==null)
                {
                    response.Code = ResponseCodeDefines.NotFound;
                    response.Msg = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}:error info: not find data!";
                }
                //GLOBALSERVICE.SetGlobalState2(ClientOperLabelName.VTR_UPLOAD_AddTask);
                if (_globalInterface != null)
                {
                    GlobalInternals re = new GlobalInternals() { Funtype = IngestDBCore.GlobalInternals.FunctionType.SetGlobalState, State = ClientOperLabelName.VTR_UPLOAD_AddTask };
                    var response1 = await _globalInterface.SubmitGlobalCallBack(re);
                    if (response1.Code != ResponseCodeDefines.SuccessCode)
                    {
                        Logger.Error("SetGlobalState modtask error");
                    }
                }

            }
            catch (Exception e)//其他未知的异常，写异常日志
            {
                if (e.GetType() == typeof(SobeyRecException))//sobeyexcep会自动打印错误
                {
                    SobeyRecException se = e as SobeyRecException;
                    response.Code = se.ErrorCode.ToString();
                    response.Msg = se.Message;
                }
                else
                {
                    response.Code = ResponseCodeDefines.ServiceError;
                    response.Msg = "error info:" + e.Message;
                    Logger.Error("AddVTRUploadTask : " + response.Msg);
                }
            }
            return response;
        }

        #endregion

    }
}
