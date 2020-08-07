using IngestDBCore;
using IngestDBCore.Basic;
using IngestDBCore.Interface;
using IngestDBCore.Notify;
using IngestDBCore.Tool;
using IngestTaskPlugin.Dto.OldResponse;
using IngestTaskPlugin.Managers;
using IngestTaskPlugin.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Sobey.Core.Log;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IngestTaskPlugin.Controllers.v1
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly ILogger Logger = LoggerManager.GetLogger("TaskInfo");
        private readonly TaskManager _taskManage;
        private readonly NotifyClock _clock;
        private readonly Lazy<IIngestGlobalInterface> _globalInterface;
        //private readonly IMapper _mapper;

        public TaskController(TaskManager task, IServiceProvider services, NotifyClock notify/*, IMapper mapper*/)
        {
            _clock = notify;
            _taskManage = task;
            _globalInterface = new Lazy<IIngestGlobalInterface>(() => services.GetRequiredService<IIngestGlobalInterface>());
            //_mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        //private readonly ILogger Logger = LoggerManager.GetLogger("TaskInfo");
        //private readonly TaskManager _monthManage;
        //private readonly RestClient _restClient;
        [HttpGet("GetQueryTaskMetaData"), MapToApiVersion("1.0")]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<GetQueryTaskMetaData_param> OldGetTaskMetaData([FromQuery]int nTaskID, [FromQuery]int Type)
        {
            if (nTaskID < 1)
            {
                var Response = new GetQueryTaskMetaData_param
                {
                    bRet = false,
                    errStr = "OK"
                };
                return Response;
            }
            try
            {
                var metadata = await _taskManage.GetTaskMetadataAsync<GetQueryTaskMetaData_param>(nTaskID, Type);
                return metadata != null ? metadata : new GetQueryTaskMetaData_param() { bRet = true, errStr = "OK" };
            }
            catch (Exception e)
            {
                var Response = new GetQueryTaskMetaData_param()
                {
                    bRet = false
                };

                Response.errStr = "OldGetTaskMetaData error info:" + e.Message;
                Logger.Error(Response.errStr);
                return Response;
            }
            
        }

        [HttpPost("PostAddTaskMetaDataPropety"), MapToApiVersion("1.0")]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<PostSetTaskMetaData_OUT> PostAddTaskMetaDataPropety([FromBody]PostSetTaskMetaData_IN pIn)
        {
            if (pIn == null)
            {
                var Response = new PostSetTaskMetaData_OUT
                {
                    bRet = false,
                    errStr = "OK"
                };
                return Response;
            }
            try
            {
                PostSetTaskMetaData_OUT pOut = new PostSetTaskMetaData_OUT();
                if (pIn.MateData == null)
                {
                    pOut.errStr = "MetaData";
                    pOut.bRet = false;
                }

                if (pIn.MateData.Length <= 0 && pIn.Type != MetaDataType.emAmfsData)
                {
                    pOut.errStr = "MetaData";
                    pOut.bRet = false;
                }

                if (pIn.Type == MetaDataType.emAmfsData)
                    pIn.MateData = System.Guid.NewGuid().ToString("N");

                await _taskManage.UpdateMetadataPropertyAsync(pIn.nTaskID, pIn.Type, 
                    new List<Dto.Response.PropertyResponse>() { new Dto.Response.PropertyResponse() { Property = pIn.TypeID, Value = pIn.MateData} });

                pOut.bRet = true;
                return pOut;
            }
            catch (Exception e)
            {
                var Response = new PostSetTaskMetaData_OUT()
                {
                    bRet = false
                };

                Response.errStr = "PostAddTaskMetaDataPropety error info:" + e.Message;
                Logger.Error(Response.errStr);
                return Response;
            }

        }

        [HttpPost("PostSetTaskMetaData"), MapToApiVersion("1.0")]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<PostSetTaskMetaData_OUT> PostSetTaskMetaData([FromBody]PostSetTaskMetaData_IN pIn)
        {
            if (pIn == null)
            {
                var Response = new PostSetTaskMetaData_OUT
                {
                    bRet = false,
                    errStr = "OK"
                };
                return Response;
            }
            try
            {
                PostSetTaskMetaData_OUT pOut = new PostSetTaskMetaData_OUT();
                if (pIn.MateData == null)
                {
                    pOut.errStr = "MetaData";
                    pOut.bRet = false;
                }
               
                if (pIn.MateData.Length <= 0 && pIn.Type != MetaDataType.emAmfsData)
                {
                    pOut.errStr = "MetaData";
                    pOut.bRet = false;
                }

                if (pIn.Type == MetaDataType.emAmfsData)
                    pIn.MateData = System.Guid.NewGuid().ToString("N");

                await _taskManage.UpdateTaskMetaDataAsync(pIn.nTaskID, pIn.Type, pIn.MateData);

                pOut.bRet = true;
                return pOut;
            }
            catch (Exception e)
            {
                var Response = new PostSetTaskMetaData_OUT()
                {
                    bRet = false
                };

                Response.errStr = "PostSetTaskMetaData error info:" + e.Message;
                Logger.Error(Response.errStr);
                return Response;
            }

        }

        [HttpGet("GetTaskCustomMetadata"), MapToApiVersion("1.0")]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<GetTaskCustomMetadata_OUT> GetTaskCustomMetadata([FromQuery]int nTaskID)
        {
            if (nTaskID < 1)
            {
                var Response = new GetTaskCustomMetadata_OUT
                {
                    bRet = false,
                    errStr = "OK"
                };
                return Response;
            }
            try
            {
                var ret = await _taskManage.GetCustomMetadataAsync<GetTaskCustomMetadata_OUT>(nTaskID);
                if (ret != null)
                {
                    ret.bRet = true;
                    ret.errStr = "OK";
                }
                
                return ret;
            }
            catch (Exception e)
            {
                var Response = new GetTaskCustomMetadata_OUT()
                {
                    bRet = false
                };

                Response.errStr = "GetTaskCustomMetadata error info:" + e.Message;
                Logger.Error(Response.errStr);
                return Response;
            }

        }

        [HttpPost("SetTaskCustomMetadata"), MapToApiVersion("1.0")]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<SetTaskCustomMetadata_OUT> PostSetTaskCustomMetadata([FromBody]SetTaskCustomMetadata_IN pIn)
        {
            if (pIn == null)
            {
                var Response = new SetTaskCustomMetadata_OUT
                {
                    bRet = false,
                    errStr = "OK"
                };
                return Response;
            }
            try
            {
                SetTaskCustomMetadata_OUT pOut = new SetTaskCustomMetadata_OUT() {
                    bRet = true,
                    errStr = "OK"
                };

                await _taskManage.UpdateCustomMetadataAsync(pIn.nTaskID, pIn.Metadata);

                return pOut;
            }
            catch (Exception e)
            {
                var Response = new SetTaskCustomMetadata_OUT()
                {
                    bRet = false
                };
               
                Response.errStr = "PostSetTaskCustomMetadata error info:" + e.Message;
                Logger.Error(Response.errStr);
                return Response;
            }

        }

        [HttpGet("StopGroupTaskById"), MapToApiVersion("1.0")]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<GroupTaskParam_OUT> StopGroupTaskById([FromQuery]int nTaskID)
        {
            var Response = new GroupTaskParam_OUT
            {
                bRet = true,
                errStr = "OK"
            };

            if (nTaskID < 1)
            {
                Response.bRet = false;
                return Response;
            }
            try
            {
                Response.taskResults = await _taskManage.StopGroupTaskAsync(nTaskID);

                if (_globalInterface != null && _globalInterface.Value != null)
                {
                    GlobalInternals re = new GlobalInternals() { Funtype = IngestDBCore.GlobalInternals.FunctionType.SetGlobalState, State = GlobalStateName.MODTASK };
                    var response1 = await _globalInterface.Value.SubmitGlobalCallBack(re);
                    if (response1.Code != ResponseCodeDefines.SuccessCode)
                    {
                        Logger.Error("SetGlobalState modtask error");
                    }

                    foreach (var item in Response.taskResults)
                    {
                        Task.Run(() => { _clock.InvokeNotify(GlobalStateName.MODTASK, NotifyPlugin.Kafka, NotifyAction.STOPGROUPTASK, new DbpTask { Taskid = item } ); });
                    }
                    
                }

                return Response;
            }
            catch (Exception e)
            {
                Response.bRet = false;
                
                Response.errStr = "StopGroupTaskById error info:" + e.Message;
                Logger.Error(Response.errStr);
                return Response;
            }

        }


        [HttpGet("DeleteGroupTaskById"), MapToApiVersion("1.0")]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<GroupTaskParam_OUT> DeleteGroupTaskById([FromQuery]int nTaskID)
        {
            var Response = new GroupTaskParam_OUT
            {
                bRet = true,
                errStr = "OK"
            };

            if (nTaskID < 1)
            {
                Response.bRet = false;
                return Response;
            }
            try
            {
                Response.taskResults = await _taskManage.DeleteGroupTaskAsync(nTaskID);

                
                if (_globalInterface != null)
                {
                    GlobalInternals re = new GlobalInternals() { Funtype = IngestDBCore.GlobalInternals.FunctionType.SetGlobalState, State = GlobalStateName.MODTASK };
                    var response1 = await _globalInterface.Value.SubmitGlobalCallBack(re);
                    if (response1.Code != ResponseCodeDefines.SuccessCode)
                    {
                        Logger.Error("SetGlobalState modtask error");
                    }

                    foreach (var item in Response.taskResults)
                    {
                        Task.Run(() => { _clock.InvokeNotify(GlobalStateName.MODTASK, NotifyPlugin.Kafka, NotifyAction.DELETEGROUPTASK, new DbpTask() { Taskid = item }); });
                    }
                }

                return Response;
            }
            catch (Exception e)
            {
                Response.bRet = false;

                Response.errStr = "DeleteGroupTaskById error info:" + e.Message;
                Logger.Error(Response.errStr);
                return Response;
            }

        }

        [HttpPost("AddTaskExDb"), MapToApiVersion("1.0")]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<AddTaskExDb_out> AddTaskExDb([FromBody] AddTaskExDb_in pIn)
        {
            var Response = new AddTaskExDb_out
            {
                bRet = true,
                errStr = "OK"
            };

            try
            {
                string CaptureMeta = pIn.strCaptureMetaData;
                string ContentMeta = pIn.strContentMetaData;
                string MatiralMeta = string.Empty;
                string PlanningMeta = string.Empty;

                if (string.IsNullOrEmpty(pIn.taskAdd.strBegin) || pIn.taskAdd.strBegin == "0000-00-00 00:00:00")
                {
                    Response.bRet = false;
                    Response.errStr = "request begin of param error";
                    return Response;
                }
                if (string.IsNullOrEmpty(pIn.taskAdd.strEnd) || pIn.taskAdd.strEnd == "0000-00-00 00:00:00")
                {
                    Response.bRet = false;
                    Response.errStr = "request end of param error";
                    return Response;
                }

                var f = await _taskManage.AddTaskWithoutPolicy<AddTaskExDb_in>(pIn, CaptureMeta, ContentMeta, MatiralMeta, PlanningMeta);

                if (f == null)
                {
                    Response.bRet = false;
                    Response.errStr = "error";
                    return Response;
                }

                Response.taskID = f.Taskid;
                //添加后如果开始时间在2分钟以内，需要调度一次
                if ((DateTimeFormat.DateTimeFromString(pIn.taskAdd.strBegin) - DateTime.Now).TotalSeconds < 120)
                    await _taskManage.UpdateComingTasks();

                
                if (_globalInterface != null)
                {
                    GlobalInternals re = new GlobalInternals() { Funtype = IngestDBCore.GlobalInternals.FunctionType.SetGlobalState, State = GlobalStateName.ADDTASK, TaskID = f.Channelid.GetValueOrDefault() };
                    var response1 = await _globalInterface.Value.SubmitGlobalCallBack(re);
                    if (response1.Code != ResponseCodeDefines.SuccessCode)
                    {
                        Logger.Error("SetGlobalState modtask error");
                    }

                    Task.Run(() => { _clock.InvokeNotify(GlobalStateName.ADDTASK, NotifyPlugin.Kafka, NotifyAction.ADDTASK, f); });
                }

                return Response;
            }
            catch (Exception e)
            {
                Response.bRet = false;
                Response.taskID = -1;
                Response.errStr = "PostAddTaskSvr error info:" + e.Message;
                Logger.Error(Response.errStr);
                return Response;
            }

        }

        [HttpPost("PostAddTaskSvr"), MapToApiVersion("1.0")]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<AddTaskSvr_OUT> PostAddTaskSvr([FromBody] AddTaskSvr_IN pIn)
        {
            var Response = new AddTaskSvr_OUT
            {
                bRet = true,
                errStr = "OK",
                newTaskId = -1
            };

            try
            {
                string CaptureMeta = string.Empty;
                string ContentMeta = string.Empty;
                string MatiralMeta = string.Empty;
                string PlanningMeta = string.Empty;
                foreach (var item in pIn.metadatas)
                {
                    if (item.emtype == MetaDataType.emCapatureMetaData)
                    {
                        CaptureMeta = item.strMetadata;
                    }
                    else if (item.emtype == MetaDataType.emStoreMetaData)
                    {
                        MatiralMeta = item.strMetadata;
                    }
                    else if (item.emtype == MetaDataType.emContentMetaData)
                    {
                        ContentMeta = item.strMetadata;
                    }
                    else if (item.emtype == MetaDataType.emPlanMetaData)
                    {
                        PlanningMeta = item.strMetadata;
                    }
                }
                if (string.IsNullOrEmpty(pIn.taskAdd.strBegin) || pIn.taskAdd.strBegin == "0000-00-00 00:00:00")
                {
                    Response.bRet = false;
                    Response.errStr = "request begin of param error";
                    return Response;
                }
                if (string.IsNullOrEmpty(pIn.taskAdd.strEnd) || pIn.taskAdd.strEnd == "0000-00-00 00:00:00")
                {
                    Response.bRet = false;
                    Response.errStr = "request end of param error";
                    return Response;
                }

                var f = await _taskManage.AddTaskWithoutPolicy<AddTaskSvr_IN>(pIn, CaptureMeta, ContentMeta, MatiralMeta, PlanningMeta);

                if (f == null)
                {
                    Response.bRet = false;
                    Response.errStr = "error";
                    return Response;
                }

                Response.newTaskId = f.Taskid;
                //添加后如果开始时间在2分钟以内，需要调度一次
                if ((DateTimeFormat.DateTimeFromString(pIn.taskAdd.strBegin) - DateTime.Now).TotalSeconds < 120)
                    await _taskManage.UpdateComingTasks();

                
                if (_globalInterface != null)
                {
                    GlobalInternals re = new GlobalInternals() { Funtype = IngestDBCore.GlobalInternals.FunctionType.SetGlobalState, State = GlobalStateName.ADDTASK, TaskID = f.Channelid.GetValueOrDefault() };
                    var response1 = await _globalInterface.Value.SubmitGlobalCallBack(re);
                    if (response1.Code != ResponseCodeDefines.SuccessCode)
                    {
                        Logger.Error("SetGlobalState modtask error");
                    }

                    Task.Run(() => { _clock.InvokeNotify(GlobalStateName.ADDTASK, NotifyPlugin.Kafka, NotifyAction.ADDTASK, f); });
                }

                return Response;
            }
            catch (Exception e)
            {
                Response.bRet = false;
               
                Response.errStr = "PostAddTaskSvr error info:" + e.Message;
                Logger.Error(Response.errStr);
                return Response;
            }

        }

        [HttpPost("PostAddTaskSvrPolicysAndBackupFlag"), MapToApiVersion("1.0")]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<AddTaskSvrPolicysAndBackupFlag_OUT> PostAddTaskSvrPolicysAndBackupFlag([FromBody] AddTaskSvrPolicysAndBackupFlag_IN pIn)
        {
            var Response = new AddTaskSvrPolicysAndBackupFlag_OUT
            {
                bRet = true,
                errStr = "OK",
                newTaskId = -1,
                backupTaskId = -1
            };

            try
            {
                string CaptureMeta = string.Empty;
                string ContentMeta = string.Empty;
                string MatiralMeta = string.Empty;
                string PlanningMeta = string.Empty;
                foreach (var item in pIn.metadatas)
                {
                    if (item.emtype == MetaDataType.emCapatureMetaData)
                    {
                        CaptureMeta = item.strMetadata;
                    }
                    else if (item.emtype == MetaDataType.emStoreMetaData)
                    {
                        MatiralMeta = item.strMetadata;
                    }
                    else if (item.emtype == MetaDataType.emContentMetaData)
                    {
                        ContentMeta = item.strMetadata;
                    }
                    else if (item.emtype == MetaDataType.emPlanMetaData)
                    {
                        PlanningMeta = item.strMetadata;
                    }
                }
                if (string.IsNullOrEmpty(pIn.taskAdd.strBegin) || pIn.taskAdd.strBegin == "0000-00-00 00:00:00")
                {
                    Response.bRet = false;
                    Response.errStr = "request begin of param error";
                    return Response;
                }
                if (string.IsNullOrEmpty(pIn.taskAdd.strEnd) || pIn.taskAdd.strEnd == "0000-00-00 00:00:00")
                {
                    Response.bRet = false;
                    Response.errStr = "request end of param error";
                    return Response;
                }

                var f = await _taskManage.AddTaskWithPolicy<AddTaskSvrPolicysAndBackupFlag_IN>(pIn, false,CaptureMeta, ContentMeta, MatiralMeta, PlanningMeta);
                if (f == null)
                {
                    Response.bRet = false;
                    Response.errStr = "error";
                    return Response;
                }

                Response.taskBack = _taskManage.ConvertTaskResponse(f);
                Response.newTaskId = Response.taskBack.nTaskID;

                if (pIn.isCreateBackupTask)
                {
                    pIn.taskAdd.nTaskID = Response.newTaskId;
                    Response.backupTaskId = (await _taskManage.AddTaskWithPolicy<AddTaskSvrPolicysAndBackupFlag_IN>(pIn, true, CaptureMeta, ContentMeta, MatiralMeta, PlanningMeta)).Taskid;

                }

                //添加后如果开始时间在2分钟以内，需要调度一次
                if ((DateTimeFormat.DateTimeFromString(pIn.taskAdd.strBegin) - DateTime.Now).TotalSeconds < 120)
                    await _taskManage.UpdateComingTasks();

                
                if (_globalInterface != null)
                {
                    GlobalInternals re = new GlobalInternals() { Funtype = IngestDBCore.GlobalInternals.FunctionType.SetGlobalState, State = GlobalStateName.ADDTASK, TaskID = f.Channelid.GetValueOrDefault() };
                    var response1 = await _globalInterface.Value.SubmitGlobalCallBack(re);
                    if (response1.Code != ResponseCodeDefines.SuccessCode)
                    {
                        Logger.Error("SetGlobalState modtask error");
                    }

                    Task.Run(() => { _clock.InvokeNotify(GlobalStateName.ADDTASK, NotifyPlugin.Kafka, NotifyAction.ADDTASK, f); });
                }


                return Response;
            }
            catch (Exception e)
            {
                Response.bRet = false;
                Response.backupTaskId = -1;

                Response.errStr = "PostAddTaskSvr error info:" + e.Message;
                Logger.Error(Response.errStr);
                return Response;
            }

        }

        [HttpGet("GetTaskIDByTaskGUID"), MapToApiVersion("1.0")]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<GetTaskIDByTaskGUID_OUT> GetTaskIDByTaskGUID([FromQuery]string strTaskGUID)
        {
            var Response = new GetTaskIDByTaskGUID_OUT
            {
                bRet = true,
                errStr = "OK",
                nTaskID = -1
            };

            try
            {
                Response.nTaskID = await _taskManage.GetTaskIDByTaskGUID(strTaskGUID);
                return Response;
            }
            catch (Exception e)
            {
                Response.bRet = false;
                Response.errStr = "error info:" + e.Message;
                Logger.Error("GetTaskIDByTaskGUID" + e.Message);
                return Response;
            }

        }


        [HttpGet("GetAllChannelCapturingTask"), MapToApiVersion("1.0")]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<GetAllChannelCapturingTask_OUT> GetAllChannelCapturingTask()
        {
            var Response = new GetAllChannelCapturingTask_OUT
            {
                bRet = true,
                errStr = "OK"
            };

            try
            {
                Response.content = await _taskManage.GetAllChannelCapturingTask<TaskContent>();
                Response.nVaildDataCount = Response.content.Count;
                return Response;
            }
            catch (Exception e)
            {
                Response.bRet = false;

                Response.errStr = "error info:" + e.Message;
                Logger.Error("GetAllChannelCapturingTask" + e.Message);
                return Response;
            }

        }

        [HttpGet("GetChannelCapturingTask"), MapToApiVersion("1.0")]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<GetChannelCapturingTask_out> GetChannelCapturingTask([FromQuery]int nChannelID, [FromQuery]int newest)
        {
            var Response = new GetChannelCapturingTask_out
            {
                bRet = true,
                errStr = "OK",
                nChannelID = nChannelID
            };

            try
            {
                Response.content = await _taskManage.GetChannelCapturingTask<TaskContent>(nChannelID, newest);
                return Response;
            }
            catch (Exception e)
            {
                Response.bRet = false;
                Response.errStr = "error info:" + e.Message;
                Logger.Error("GetAllChannelCapturingTask" + e.Message);
                return Response;
            }
        }

        [HttpPost("PostModifyTaskDb"), MapToApiVersion("1.0")]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<PostModifyTaskDb_OUT> PostModifyTaskDb([FromBody]PostModifyTaskDb_IN pIn)
        {
            var Response = new PostModifyTaskDb_OUT
            {
                bRet = true,
                errStr = "OK"
            };

            try
            {
                var modifyTask = await _taskManage.ModifyTask<TaskContent>(pIn.taskModify, string.Empty, pIn.TaskMetaData, pIn.MaterialMetaData, string.Empty);
                //添加后如果开始时间在2分钟以内，需要调度一次
                if ((DateTimeFormat.DateTimeFromString(pIn.taskModify.strBegin) - DateTime.Now).TotalSeconds < 120)
                    await _taskManage.UpdateComingTasks();

                
                if (_globalInterface != null)
                {
                    GlobalInternals re = new GlobalInternals() { Funtype = IngestDBCore.GlobalInternals.FunctionType.SetGlobalState, State = GlobalStateName.MODTASK };
                    var response1 = await _globalInterface.Value.SubmitGlobalCallBack(re);
                    if (response1.Code != ResponseCodeDefines.SuccessCode)
                    {
                        Logger.Error("SetGlobalState modtask error");
                    }

                    Task.Run(() => { _clock.InvokeNotify(GlobalStateName.MODTASK, NotifyPlugin.Kafka, NotifyAction.MODIFYTASK, modifyTask); });
                }

                return Response;
            }
            catch (Exception e)
            {
                Response.bRet = false;
                Response.errStr = "error info:" + e.Message;
                Logger.Error("PostModifyTaskDb" + e.Message);
                return Response;
            }
            return Response;
        }

        [HttpPost("ModifyTask"), MapToApiVersion("1.0")]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<ModifyTask_out> ModifyTask([FromBody] ModifyTask_in pIn)
        {
            var Response = new ModifyTask_out
            {
                bRet = true,
                errStr = "OK"
            };

            try
            {
                string CaptureMeta = string.Empty;
                string ContentMeta = string.Empty;
                string MatiralMeta = string.Empty;
                string PlanningMeta = string.Empty;
                foreach (var item in pIn.metadatas)
                {
                    if (item.emtype == MetaDataType.emCapatureMetaData)
                    {
                        CaptureMeta = item.strMetadata;
                    }
                    else if (item.emtype == MetaDataType.emStoreMetaData)
                    {
                        MatiralMeta = item.strMetadata;
                    }
                    else if (item.emtype == MetaDataType.emContentMetaData)
                    {
                        ContentMeta = item.strMetadata;
                    }
                    else if (item.emtype == MetaDataType.emPlanMetaData)
                    {
                        PlanningMeta = item.strMetadata;
                    }
                }

                var modifyTask = await _taskManage.ModifyTask<TaskContent>(pIn.taskModify, CaptureMeta, ContentMeta, MatiralMeta, PlanningMeta);
                //添加后如果开始时间在2分钟以内，需要调度一次
                if ((DateTimeFormat.DateTimeFromString(pIn.taskModify.strBegin) - DateTime.Now).TotalSeconds < 120)
                    await _taskManage.UpdateComingTasks();

                
                if (_globalInterface != null)
                {
                    GlobalInternals re = new GlobalInternals() { Funtype = IngestDBCore.GlobalInternals.FunctionType.SetGlobalState, State = GlobalStateName.MODTASK };
                    var response1 = await _globalInterface.Value.SubmitGlobalCallBack(re);
                    if (response1.Code != ResponseCodeDefines.SuccessCode)
                    {
                        Logger.Error("SetGlobalState modtask error");
                    }

                    Task.Run(() => { _clock.InvokeNotify(GlobalStateName.MODTASK, NotifyPlugin.Kafka, NotifyAction.MODIFYTASK,modifyTask); });
                }

                return Response;
            }
            catch (Exception e)
            {
                Response.bRet = false;
                Response.errStr = "error info:" + e.Message;
                Logger.Error("ModifyTask" + e.Message);
                return Response;
            }
            return Response;
        }

        [HttpGet("GetTaskByID"), MapToApiVersion("1.0")]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<GetTaskByID_OUT> GetTaskByID([FromQuery]int nTaskID)
        {
            var Response = new GetTaskByID_OUT
            {
                bRet = true,
                errStr = "OK",
            };

            try
            {
                Response.taskConten = await _taskManage.GetTaskInfoByID<TaskContent>(nTaskID, 0);
                return Response;
            }
            catch (Exception e)
            {
                Response.bRet = false;
                Response.errStr = "error info:" + e.Message;
                Logger.Error("GetTaskByID" + e.Message);
                return Response;
            }
        }

        [HttpGet("GetTaskByIDForFSW"), MapToApiVersion("1.0")]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<GetTaskByIDForFSW_OUT> GetTaskByIDForFSW([FromQuery]int nTaskID)
        {
            var Response = new GetTaskByIDForFSW_OUT
            {
                bRet = true,
                errStr = "OK",
            };

            try
            {
                Response.taskConten = await _taskManage.GetTaskInfoByID<TaskContent>(nTaskID, 1);
                return Response;
            }
            catch (Exception e)
            {
                Response.bRet = false;
                Response.errStr = "error info:" + e.Message;
                Logger.Error("GetTaskByIDForFSW" + e.Message);
                return Response;
            }
        }

        [HttpGet("GetTieUpTaskByChannelID"), MapToApiVersion("1.0")]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<GetTieUpTaskByChannelID_OUT> GetTieUpTaskByChannelID([FromQuery]int nChannelID)
        {
            var Response = new GetTieUpTaskByChannelID_OUT
            {
                bRet = true,
                errStr = "OK",
                nTaskID = -1
            };

            try
            {
                Response.nTaskID = await _taskManage.GetTieUpTaskIDByChannelId(nChannelID);
                return Response;
            }
            catch (Exception e)
            {
                Response.bRet = false;
                Response.errStr = "error info:" + e.Message;
                Logger.Error("GetTieUpTaskByChannelID" + e.Message);
                return Response;
            }
        }

        [HttpGet("GetStopTaskFromFSW"), MapToApiVersion("1.0")]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<StopTaskFromFSW_OUT> GetStopTaskFromFSW([FromQuery]int nTaskID)
        {
            var Response = new StopTaskFromFSW_OUT
            {
                bRet = true,
                errStr = "OK",
            };

            try
            {
                var task = await _taskManage.StopTask(nTaskID, DateTime.MinValue);

                if (task == null || task.Taskid < 1)
                {
                    Response.bRet = false;
                    Response.errStr = "not found task";
                    return Response;
                }

                if (_globalInterface != null)
                {
                    GlobalInternals re = new GlobalInternals() { Funtype = IngestDBCore.GlobalInternals.FunctionType.SetGlobalState, State = GlobalStateName.MODTASK };
                    var response1 = await _globalInterface.Value.SubmitGlobalCallBack(re);
                    if (response1.Code != ResponseCodeDefines.SuccessCode)
                    {
                        Logger.Error("SetGlobalState modtask error");
                    }

                    Task.Run(() => { _clock.InvokeNotify(GlobalStateName.MODTASK, NotifyPlugin.Kafka, NotifyAction.STOPTASK, task); });
                }
                return Response;
            }
            catch (Exception e)
            {
                Response.bRet = false;
                Response.errStr = "error info:" + e.Message;
                Logger.Error("GetStopTaskFromFSW" + e.Message);
                return Response;
            }
        }

        [HttpGet("GetStopCapture"), MapToApiVersion("1.0")]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<StopCapture_OUT> GetStopCapture([FromQuery]int nTaskID, [FromQuery]string strEndTime)
        {
            var Response = new StopCapture_OUT
            {
                bRet = true,
                errStr = "OK",
            };

            try
            {
                DbpTask task = null;
                if (string.IsNullOrEmpty(strEndTime))
                {
                    task = await _taskManage.StopTask(nTaskID, DateTime.Now);
                }
                else
                    task = await _taskManage.StopTask(nTaskID, DateTimeFormat.DateTimeFromString(strEndTime));

                if (task == null || task.Taskid < 1)
                {
                    Response.bRet = false;
                    Response.errStr = "not found task";
                    return Response;
                }

                if (_globalInterface != null)
                {
                    GlobalInternals re = new GlobalInternals() { Funtype = IngestDBCore.GlobalInternals.FunctionType.SetGlobalState, State = GlobalStateName.MODTASK };
                    var response1 = await _globalInterface.Value.SubmitGlobalCallBack(re);
                    if (response1.Code != ResponseCodeDefines.SuccessCode)
                    {
                        Logger.Error("SetGlobalState modtask error");
                    }

                    Task.Run(() => { _clock.InvokeNotify(GlobalStateName.MODTASK, NotifyPlugin.Kafka, NotifyAction.STOPTASK, task ); });
                }
                return Response;
            }
            catch (Exception e)
            {
                Response.bRet = false;
                Response.errStr = "error info:" + e.Message;
                Logger.Error("GetStopTaskFromFSW" + e.Message);
                return Response;
            }
        }

        [HttpGet("GetSetTaskState"), MapToApiVersion("1.0")]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<GetSetTaskState_OUT> GetSetTaskState([FromQuery]int nTaskID, [FromQuery]int nState)
        {
            var Response = new GetSetTaskState_OUT
            {
                bRet = true,
                errStr = "OK",
            };

            try
            {
                int result = await _taskManage.SetTaskState(nTaskID, nState);
                if(result > 0)
                {
                    Task.Run(() => { _clock.InvokeNotify(GlobalStateName.MODTASK, NotifyPlugin.Kafka, NotifyAction.MODIFYTASKSTATE, new DbpTask() { Taskid = nTaskID, State = nState } ); });

                }
                return Response;
            }
            catch (Exception e)
            {
                Response.bRet = false;
                Response.errStr = "error info:" + e.Message;
                Logger.Error("GetStopTaskFromFSW" + e.Message);
                return Response;
            }
        }

        [HttpGet("GetQueryTaskContent"), MapToApiVersion("1.0")]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<GetQueryTaskContent_OUT> GetQueryTaskContent(int nUnitID, string strDay, int timeMode)
        {
            var Response = new GetQueryTaskContent_OUT
            {
                bRet = true,
                errStr = "OK",
                nVaildDataCount = 0,
            };

            try
            {
                Response.taskCon = await _taskManage.QueryTaskContent<TaskContent>(nUnitID, DateTimeFormat.DateTimeFromString(strDay), (TimeLineType)timeMode);

                if (Response.taskCon != null)
                {
                    Response.nVaildDataCount = Response.taskCon.Count;
                }

                return Response;
            }
            catch (Exception e)
            {

                Response.errStr = "error info:" + e.Message;
                Logger.Error("GetStopTaskFromFSW" + e.Message);
                Response.bRet = false;
                return Response;
            }
        }

        [HttpGet("GetTaskSource"), MapToApiVersion("1.0")]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<GetTaskSource_OUT> GetTaskSource([FromQuery]int nTaskID)
        {
            var Response = new GetTaskSource_OUT
            {
                bRet = true,
                errStr = "OK",
                Source = TaskSource.emUnknowTask,
            };
            try
            {
                if (nTaskID < 0)
                {
                    Response.errStr = "TaskID is invaild";
                    Response.bRet = false;
                }
                Response.Source = await _taskManage.GetTaskSource(nTaskID);
            }
            catch (Exception e)//其他未知的异常，写异常日志
            {
                Response.bRet = false;
                Response.errStr = "error info:" + e.Message;
                Logger.Error("GetTaskSource" + e.Message);
                return Response;
            }
            return Response;

        }

        [HttpGet("GetTrimTaskBeginTime"), MapToApiVersion("1.0")]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<bool> GetTrimTaskBeginTime([FromQuery]int nTaskID, [FromQuery]string strStartTime)
        {
            try
            {
                if (nTaskID < 0)
                {
                    return false;
                }
                int result = await _taskManage.TrimTaskBeginTime(nTaskID, strStartTime);
                if(result > 0)
                {
                    Task.Run(() => { _clock.InvokeNotify(GlobalStateName.MODTASK, NotifyPlugin.Kafka, NotifyAction.MODIFYTASKSTARTTIME, new DbpTask() { Taskid = nTaskID, Starttime = DateTimeFormat.DateTimeFromString(strStartTime) } ); });
                }
            }
            catch (Exception e)//其他未知的异常，写异常日志
            {
                
                {
                    //Response.errStr = "error info:" + e.ToString();
                    Logger.Error("GetTrimTaskBeginTime" + e.Message);
                }
                return false;
            }
            return true;

        }

        [HttpPost("PostQueryTaskMetadataGroup"), MapToApiVersion("1.0")]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<QueryTaskMetadataGroup_OUT> PostQueryTaskMetadataGroup([FromBody]List<int> nTaskID)
        {
            var Response = new QueryTaskMetadataGroup_OUT
            {
                bRet = true,
                errStr = "OK",
            };
            try
            {

                if (nTaskID.Count <= 0)
                {
                    Response.bRet = false;
                    return Response;
                }
                 Response.metaDataPair = await _taskManage.GetTaskMetadataListAsync<MetadataPair>(nTaskID);
            }
            catch (Exception e)//其他未知的异常，写异常日志
            {
                Response.bRet = false;
                Response.errStr = "error info:" + e.Message;
                Logger.Error("PostQueryTaskMetadataGroup" + e.Message);
                return Response;
            }
            return Response;

        }

        [HttpGet("GetDelTaskDb"), MapToApiVersion("1.0")]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<DelTaskDb_OUT> GetDelTaskDb([FromQuery]int nTaskID)
        {
            var Response = new DelTaskDb_OUT
            {
                bRet = true,
                errStr = "OK",
            };
            try
            {
                if (nTaskID <= 0)
                {
                    Response.bRet = false;
                    return Response;
                }
                var task = await _taskManage.DeleteTask(nTaskID);
                if (task == null)
                {
                    Task.Run(() => { _clock.InvokeNotify(GlobalStateName.DELTASK, NotifyPlugin.Kafka, NotifyAction.DELETETASK, new DbpTask() { Taskid = nTaskID }); });
                }
                else
                {
                    Task.Run(() => { _clock.InvokeNotify(GlobalStateName.MODTASK, NotifyPlugin.Kafka, NotifyAction.DELETETASK, task); });
                }}
            catch (Exception e)//其他未知的异常，写异常日志
            {
                Response.bRet = false;
                Response.errStr = "error info:" + e.Message;
                Logger.Error("GetDelTaskDb" + e.Message);
                return Response;
            }
            return Response;

        }

        [HttpPost("SetTaskClassify"), MapToApiVersion("1.0")]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<SetTaskClassify_OUT> SetTaskClassify([FromQuery]int nTaskID, [FromBody]string strClassify)
        {
            var Response = new SetTaskClassify_OUT
            {
                bRet = true,
                errStr = "OK",
            };
            try
            {
                if (nTaskID <= 0)
                {
                    Response.bRet = false;
                    return Response;
                }
                await _taskManage.SetTaskClassify(nTaskID, strClassify);
            }
            catch (Exception e)//其他未知的异常，写异常日志
            {
                Response.bRet = false;
                Response.errStr = "error info:" + e.Message;
                Logger.Error("SetTaskClassify" + e.Message);
                return Response;
            }
            return Response;

        }

        [HttpGet("GetUnlockAllTasks"), MapToApiVersion("1.0")]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<bool> GetUnlockAllTasks()
        {
            var Response = false;
            
            try
            {
                return await _taskManage.UnlockAllTasks();
            }
            catch (Exception e)//其他未知的异常，写异常日志
            {
                
                //Response.errStr = "error info:" + e.ToString();
                Logger.Error("GetUnlockAllTasks" + e.Message);
                return Response;
            }
            return Response;

        }

        [HttpPost("SetPeriodTaskToNextTime"), MapToApiVersion("1.0")]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<bool> SetPeriodTaskToNextTime()
        {
            var Response = false;
            try
            {
                return await _taskManage.SetPeriodTaskToNextTime();
            }
            catch (Exception e)//其他未知的异常，写异常日志
            {
                if (e.GetType() == typeof(SobeyRecException))//sobeyexcep会自动打印错误
                {
                    SobeyRecException se = e as SobeyRecException;
                    //Response.errStr = se.ErrorCode.ToString();
                }
                else
                {
                    //Response.errStr = "error info:" + e.ToString();
                    Logger.Error("SetPeriodTaskToNextTime" + e.ToString());
                }
                return Response;
            }
            return Response;

        }

        [HttpGet("GetNeedSynTasks2"), MapToApiVersion("1.0")]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<GetNeedSynTasks2_OUT> GetNeedSynTasks2()
        {
            var Response = new GetNeedSynTasks2_OUT
            {
                bRet = true,
                errStr = "OK",
            };

            try
            {
                await _taskManage.UpdateComingTasks();

                Response.synTasks = await _taskManage.GetNeedSynTasksNew<TaskFullInfo>();
                if (Response.synTasks != null)
                {
                    Response.nValidDataCount = Response.synTasks.Count;
                }
                
            }
            catch (Exception e)//其他未知的异常，写异常日志
            {
                Response.bRet = false;

                Response.errStr = "error info:" + e.Message;
                Logger.Error("GetNeedSynTasks2" + e.Message);
                return Response;
            }
            return Response;

        }

        [HttpPost("PostCompleteSynTasks"), MapToApiVersion("1.0")]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<bool> PostCompleteSynTasks([FromBody]CompleteSynTasks_IN pIn)
        {
            //var Response = new GetNeedSynTasks2_OUT
            //{
            //    bRet = true,
            //    errStr = "OK",
            //};

            try
            {
                await _taskManage.CompleteSynTasks(pIn);
                //Response.nValidDataCount = Response.synTasks.Count;
                return true;
            }
            catch (Exception e)//其他未知的异常，写异常日志
            {
                
                    //Response.errStr = "error info:" + e.ToString();
                Logger.Error("GetTrimTaskBeginTime" + e.Message);
                //return Response;
            }
            return false;
            //return Response;

        }

        [HttpPost("Update24HoursTask"), MapToApiVersion("1.0")]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<string> Update24HoursTask([FromQuery]int ntaskid, [FromQuery]long oldlen, [FromQuery]int oldclipnum, [FromQuery]string newname, [FromQuery]string newguid, [FromQuery]int index)
        {
            //var Response = new GetNeedSynTasks2_OUT
            //{
            //    bRet = true,
            //    errStr = "OK",
            //};
            try
            {
                return await _taskManage.Update24HoursTask(ntaskid, oldlen, oldclipnum, newname, newguid, index);
            }
            catch (Exception e)//其他未知的异常，写异常日志
            {
                if (e.GetType() == typeof(SobeyRecException))//sobeyexcep会自动打印错误
               
                //Response.errStr = "error info:" + e.ToString();
                Logger.Error("GetTrimTaskBeginTime" + e.Message);
                //return Response;
            }
            return "";

        }

        [HttpGet("GetNeedRescheduleTasks"), MapToApiVersion("1.0")]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<GetNeedRescheduleTasks_OUT> GetNeedRescheduleTasks()
        {
            var Response = new GetNeedRescheduleTasks_OUT
            {
                bRet = true,
                errStr = "OK",
            };

            try
            {
                Response.rescheduleTasks = await _taskManage.GetScheduleFailedTasks<TaskFullInfo>();
                if (Response.rescheduleTasks != null)
                {
                    Response.nValidDataCount = Response.rescheduleTasks.Count;
                }
            }
            catch (Exception e)//其他未知的异常，写异常日志
            {
                Response.bRet = false;
                //Response.errStr = "error info:" + e.ToString();
                Logger.Error("GetNeedRescheduleTasks" + e.Message);
                return Response;
            }
            return Response;

        }

        [HttpPost("PostCompleteRescheduleTasks"), MapToApiVersion("1.0")]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<bool> PostCompleteRescheduleTasks([FromBody]TaskFullInfo rescheduleTask)
        {
            var Response = false;

            try
            {
                return await _taskManage.CompleteRescheduleTasks<TaskContent>(rescheduleTask.taskContent);
            }
            catch (Exception e)//其他未知的异常，写异常日志
            {
               
                //Response.errStr = "error info:" + e.ToString();
                Logger.Error("PostCompleteRescheduleTasks" + e.Message);
                return Response;
            }
            return Response;

        }

        [HttpGet("GetLockTaskByID"), MapToApiVersion("1.0")]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<bool> GetLockTaskByID([FromQuery]int taskID)
        {
            var Response = false;

            try
            {
                return await _taskManage.LockTask(taskID);
            }
            catch (Exception e)//其他未知的异常，写异常日志
            {
                
                    //Response.errStr = "error info:" + e.ToString();
                    Logger.Error("GetLockTaskByID" + e.Message);
                return Response;
            }
            return Response;

        }

        [HttpGet("GetModifyCooperTask"), MapToApiVersion("1.0")]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<bool> GetModifyCooperTask([FromQuery]int nTaskID, [FromQuery]int emCoopType)
        {
            var Response = false;

            try
            {
                var back = await _taskManage.SetTaskCooperType(nTaskID, (CooperantType)emCoopType);
                
                if (_globalInterface != null)
                {
                    GlobalInternals re = new GlobalInternals() { Funtype = IngestDBCore.GlobalInternals.FunctionType.SetGlobalState, State = GlobalStateName.MODTASK };
                    var response1 = await _globalInterface.Value.SubmitGlobalCallBack(re);
                    if (response1.Code != ResponseCodeDefines.SuccessCode)
                    {
                        Logger.Error("SetGlobalState modtask error");
                    }

                    Task.Run(() => { _clock.InvokeNotify(GlobalStateName.MODTASK, NotifyPlugin.Kafka, NotifyAction.MODIFYTASKCOOPTYPE, new DbpTask() { Taskid = nTaskID, Backtype = emCoopType } ); });
                }
                return back;
            }
            catch (Exception e)//其他未知的异常，写异常日志
            {
               
                //Response.errStr = "error info:" + e.ToString();
                Logger.Error("GetLockTaskByID" + e.Message);
                return Response;
            }
            return Response;

        }

        [HttpGet("GetRescheduleTasks"), MapToApiVersion("1.0")]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<RescheduleTasks_OUT> GetRescheduleTasks()
        {
            var Response = new RescheduleTasks_OUT()
            {
                errStr = "OK",
                bRet = true
            };

            try
            {
                Response.taskInfoRescheduled = await _taskManage.RescheduleTasks<TaskInfoRescheduled>();
                if (Response.taskInfoRescheduled != null)
                {
                    Response.nValidDataCount = Response.taskInfoRescheduled.Count;
                }

                return Response;
            }
            catch (Exception e)//其他未知的异常，写异常日志
            {
                Response.bRet = false;
                Response.errStr = "error info:" + e.Message;
                Logger.Error("GetRescheduleTasks" + e.Message);
                return Response;
            }
            return Response;

        }

        [HttpGet("GetWarningInfos"), MapToApiVersion("1.0")]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<GetWarningInfos_OUT> GetWarningInfos([FromQuery]int nChannel, [FromQuery]int bChannelAlive)
        {
            var Response = new GetWarningInfos_OUT()
            {
                bRet = true
            };

            try
            {
                if (bChannelAlive == 1)
                    Response.arrErrInfo = await _taskManage.GetAutoManuConflict<WarningInfo>(nChannel);
                else
                    Response.arrErrInfo = await _taskManage.GetBadChannelTask<WarningInfo>(nChannel);
                return Response;
            }
            catch (Exception e)//其他未知的异常，写异常日志
            {
                Response.bRet = false;
                //Response.errStr = "error info:" + e.ToString();
                Logger.Error("GetWarningInfos" + e.Message);
                return Response;
            }
            return Response;

        }

        [HttpGet("GetChannelCapturingLowMaterial"), MapToApiVersion("1.0")]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<GetChannelCapturingLowMaterial_OUT> GetChannelCapturingLowMaterial([FromQuery]int channelID)
        {
            var Response = new GetChannelCapturingLowMaterial_OUT()
            {
                errStr = "OK",
                bRet = true
            };

            try
            {
                Response.strLowFileName = await _taskManage.GetChannelCapturingLowMaterial(channelID, _globalInterface.Value);
                return Response;
            }
            catch (Exception e)//其他未知的异常，写异常日志
            {
                if (e.GetType() == typeof(SobeyRecException))//sobeyexcep会自动打印错误
                
                Response.bRet = false;
                //Response.errStr = "error info:" + e.ToString();
                Logger.Error("GetChannelCapturingLowMaterial" + e.Message);
                return Response;
            }
            return Response;

        }

        [HttpPost("SplitTask"), MapToApiVersion("1.0")]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<SplitTask_OUT> SplitTask([FromBody]SplitTask_IN pIn)
        {
            var Response = new SplitTask_OUT()
            {
                errStr = "OK",
                bRet = true
            };

            try
            {
                var ret = await _taskManage.SplitTask<SplitTask_OUT>(pIn.nTaskID, pIn.strNewGUID, pIn.strNewName);
                if (ret != null)
                {
                    ret.bRet = true;
                    ret.errStr = "ok";
                }
                else
                {
                    Response.bRet = false;
                    Response.errStr = "not find";
                    return Response;
                }

                
                
                return ret;
            }
            catch (Exception e)//其他未知的异常，写异常日志
            {
                
                Response.bRet = false;
                Response.errStr = "error info:" + e.Message;
                Logger.Error("SplitTask" + e.Message);
                return Response;
            }
            return Response;

        }
        
        [HttpGet("CreateNewTaskFromPeriodicTask"), MapToApiVersion("1.0")]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<CreateNewTaskFromPeriodicTask_OUT> CreateNewTaskFromPeriodicTask([FromQuery]int periodicTaskId)
        {
            var Response = new CreateNewTaskFromPeriodicTask_OUT()
            {
                errStr = "OK",
                bRet = true
            };

            try
            {
                Response.newTask = await _taskManage.CreateNewTaskFromPeriodicTask<TaskFullInfo>(periodicTaskId);

                var custom = await _taskManage.GetCustomMetadataAsync<GetTaskCustomMetadata_OUT>(periodicTaskId);
                if (custom != null)
                {
                    await _taskManage.UpdateCustomMetadataAsync(Response.newTask.taskContent.nTaskID, custom.Metadata);
                }
                
                return Response;
            }
            catch (Exception e)//其他未知的异常，写异常日志
            {

                Response.bRet = false;
                Response.errStr = "error info:" + e.Message;
                Logger.Error("CreateNewTaskFromPeriodicTask" + e.Message);
                return Response;
            }
            return Response;

        }

        [HttpGet("GetStartTieUpTask"), MapToApiVersion("1.0")]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<StartTieUpTask_out> GetStartTieUpTask([FromQuery]int iTaskID)
        {
            var Response = new StartTieUpTask_out()
            {
                errStr = "OK",
                bRet = true
            };

            try
            {
                //Response.bRet = await _taskManage.StartTieupTask(iTaskID);
                var tieupTask = await _taskManage.StartTieupTask(iTaskID);
                Response.bRet = (tieupTask != null && tieupTask.Tasktype == (int)TaskType.TT_TIEUP) ? true : false;

                if (_globalInterface != null)
                {
                    GlobalInternals re = new GlobalInternals() { Funtype = IngestDBCore.GlobalInternals.FunctionType.SetGlobalState, State = GlobalStateName.MODTASK };
                    var response1 = await _globalInterface.Value.SubmitGlobalCallBack(re);
                    if (response1.Code != ResponseCodeDefines.SuccessCode)
                    {
                        Logger.Error("SetGlobalState modtask error");
                    }

                    Task.Run(() => { _clock.InvokeNotify(GlobalStateName.MODTASK, NotifyPlugin.Kafka, NotifyAction.MODIFYTASK, tieupTask); });
                }
                return Response;
            }
            catch (Exception e)//其他未知的异常，写异常日志
            {

                Response.bRet = false;
                Response.errStr = "error info:" + e.Message;
                Logger.Error("GetStartTieUpTask" + e.Message);
                return Response;
            }
            return Response;

        }

        [HttpGet("ChooseUseableChannel"), MapToApiVersion("1.0")]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<ChooseUseableChannel_out> ChooseUseableChannel([FromBody]ChooseUseableChannel_in pIn)
        {
            var Response = new ChooseUseableChannel_out()
            {
                errStr = "OK",
                bRet = true
            };

            try
            {
                Response.channelId = await _taskManage.ChooseUsealbeChannel(pIn.channelIds, DateTimeFormat.DateTimeFromString(pIn.strBein), DateTimeFormat.DateTimeFromString(pIn.strEnd));
                return Response;
            }
            catch (Exception e)//其他未知的异常，写异常日志
            {

                Response.bRet = false;
                Response.errStr = "error info:" + e.Message;
                Logger.Error("GetStartTieUpTask" + e.Message);
                return Response;
            }
            return Response;

        }

        [HttpGet("ModifyTaskName"), MapToApiVersion("1.0")]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<ModifyTaskName_out> ModifyTaskName([FromQuery]int nTaskID, [FromQuery]string strNewName)
        {
            var Response = new ModifyTaskName_out()
            {
                errStr = "OK",
                bRet = true
            };

            try
            {
                await _taskManage.ModifyTaskName(nTaskID, strNewName);
                return Response;
            }
            catch (Exception e)//其他未知的异常，写异常日志
            {

                Response.bRet = false;
                Response.errStr = "error info:" + e.Message;
                Logger.Error("ModifyTaskName" + e.Message);
                return Response;
            }
            return Response;

        }

        [HttpPost("ModifyPeriodTask"), MapToApiVersion("1.0")]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<ModifyPeriodTask_out> ModifyPeriodTask([FromQuery]int IsAll, [FromBody]TaskContent taskModify)
        {
            var Response = new ModifyPeriodTask_out()
            {
                errStr = "OK",
                bRet = true,
                newTaskId = -1
            };

            try
            {
                //Response.newTaskId = await _taskManage.ModifyPeriodTask<TaskContent>(taskModify, IsAll == 1 ? true : false);
                var newTask = await _taskManage.ModifyPeriodTask<TaskContent>(taskModify, IsAll == 1 ? true : false);
                Response.newTaskId = newTask != null ? newTask.Taskid : 0;

                //添加后如果开始时间在2分钟以内，需要调度一次
                if ((DateTimeFormat.DateTimeFromString(taskModify.strBegin) - DateTime.Now).TotalSeconds < 120)
                    await _taskManage.UpdateComingTasks();

                
                if (_globalInterface != null)
                {
                    GlobalInternals re = new GlobalInternals() { Funtype = IngestDBCore.GlobalInternals.FunctionType.SetGlobalState, State = GlobalStateName.MODTASK };
                    var response1 = await _globalInterface.Value.SubmitGlobalCallBack(re);
                    if (response1.Code != ResponseCodeDefines.SuccessCode)
                    {
                        Logger.Error("SetGlobalState modtask error");
                    }

                    Task.Run(() => { _clock.InvokeNotify(GlobalStateName.MODTASK, NotifyPlugin.Kafka, NotifyAction.MODIFYPERIODCTASK,newTask); });
                }
                return Response;
            }
            catch (Exception e)//其他未知的异常，写异常日志
            {
               
                    Response.bRet = false;
                    Response.errStr = "error info:" + e.Message;
                    Logger.Error("ModifyPeriodTask" + e.Message);
                return Response;
            }
            return Response;

        }

        [HttpGet("IsVTRCollide"), MapToApiVersion("1.0")]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<IsVTRCollide_out> IsVTRCollide(
            [FromQuery]int VTR_ID, 
            [FromQuery]string strBeginTime, 
            [FromQuery]string strEndTime, 
            [FromQuery] int TaskID)
        {
            var Response = new IsVTRCollide_out()
            {
                errStr = "OK",
                bRet = true
            };

            try
            {
                if (string.IsNullOrEmpty(strBeginTime))
                {
                    Response.bRet = false;
                    Response.errStr = $"检测VTR是否冲突过程中，任务开始时间不合法";
                    return Response;
                }
                if (string.IsNullOrEmpty(strEndTime))
                {
                    Response.bRet = false;
                    Response.errStr = $"检测VTR是否冲突过程中，任务结束时间不合法";
                    return Response;
                }

                if (VTR_ID <= 0)
                {
                    Response.bRet = false;
                    Response.errStr = $"检测VTR是否冲突过程中，VTR的ID不合法";
                    return Response;
                }
                if (TaskID < -1)
                {
                    Response.bRet = false;
                    Response.errStr = $"检测VTR是否冲突过程中，任务的ID不合法";
                    return Response;
                }

                Response.emResult = VTRCollideResult.emVTRNotDefine;
                Response.CollideTaskContent= await _taskManage.IsVTRCollide<TaskContent>(VTR_ID, strBeginTime, strEndTime, TaskID);
                if (Response.CollideTaskContent != null)
                {
                    Response.emResult = VTRCollideResult.emHaveVTRCollide;
                }
                else
                    Response.emResult = VTRCollideResult.emNotVTRCollide;

                return Response;
            }
            catch (Exception e)//其他未知的异常，写异常日志
            {
                
                Response.bRet = false;
                Response.errStr = "error info:" + e.Message;
                Logger.Error("IsVTRCollide" + e.Message);
                return Response;
            }
            return Response;

        }

        [HttpPost("WriteVTRUploadTaskDB"), MapToApiVersion("1.0")]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<TaskOldResponseMessage> WriteVTRUploadTaskDB([FromBody] TaskContent taskAdd)
        {
            var Response = new TaskOldResponseMessage();

            try
            {
                await _taskManage.WriteVTRUploadTaskDB<TaskContent>(taskAdd);
                return Response;
            }
            catch (Exception e)//其他未知的异常，写异常日志
            {
                if (e.GetType() == typeof(SobeyRecException))//sobeyexcep会自动打印错误
                {
                    SobeyRecException se = e as SobeyRecException;
                    Response.nCode = se.ErrorCode;
                    //Response.message = false;
                }
                else
                {
                    Response.nCode = 500;
                    //Response.errStr = "error info:" + e.ToString();
                    Logger.Error("ModifyTaskName" + e.Message);
                }
                return Response;
            }
            return Response;

        }

        [HttpPost("GetKamakatiFailTasks"), MapToApiVersion("1.0")]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<List<TaskContent>> GetKamakatiFailTasks()
        {
            var Response = new List<TaskContent>();

            try
            {
                Response = await _taskManage.GetKamakatiFailTasks<TaskContent>();
                return Response;
            }
            catch (Exception e)//其他未知的异常，写异常日志
            {
                if (e.GetType() == typeof(SobeyRecException))//sobeyexcep会自动打印错误
                {
                    SobeyRecException se = e as SobeyRecException;
                    //Response.nCode = se.ErrorCode;
                    //Response.message = false;
                }
                else
                {
                    //Response.nCode = 500;
                    //Response.errStr = "error info:" + e.ToString();
                    Logger.Error("GetKamakatiFailTasks" + e.ToString());
                }
                return Response;
            }
            return Response;

        }

        [HttpPost("SetTaskStampBmp"), MapToApiVersion("1.0")]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<TaskOldResponseMessage> SetTaskStampBmp([FromQuery] int nTaskID, [FromBody] string strBmp)
        {
            var Response = new TaskOldResponseMessage() { nCode = 1, message = "OK"};

            try
            {
                strBmp = strBmp.Replace("\\", "\\\\");
                strBmp = strBmp.Replace("'", "''");
                await _taskManage.SetTaskBmp(nTaskID, strBmp);
                return Response;
            }
            catch (Exception e)//其他未知的异常，写异常日志
            {
                if (e.GetType() == typeof(SobeyRecException))//sobeyexcep会自动打印错误
                {
                    SobeyRecException se = e as SobeyRecException;
                   Response.nCode = se.ErrorCode;
                    //Response.message = false;
                }
                else
                {
                    Response.nCode = 500;
                    //Response.errStr = "error info:" + e.ToString();
                    Logger.Error("SetTaskStampBmp" + e.ToString());
                }
                return Response;
            }
            return Response;

        }

        [HttpGet("AutoAddTaskByOldTask"), MapToApiVersion("1.0")]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<TaskOldResponseMessage<int>> AutoAddTaskByOldTask([FromQuery]int nOldTaskID, [FromQuery]string strStartTime)
        {
            var Response = new TaskOldResponseMessage<int>();
            Response.extention = -1;

            try
            {
                var task = await _taskManage.AutoAddTaskByOldTask(nOldTaskID, DateTimeFormat.DateTimeFromString(strStartTime),_globalInterface.Value);
                Response.extention = task.Taskid;
                Response.nCode = 1;
                
                if (_globalInterface != null)
                {
                    GlobalInternals re = new GlobalInternals() { Funtype = IngestDBCore.GlobalInternals.FunctionType.SetGlobalState, State = GlobalStateName.ADDTASK };
                    var response1 = await _globalInterface.Value.SubmitGlobalCallBack(re);
                    if (response1.Code != ResponseCodeDefines.SuccessCode)
                    {
                        Logger.Error("SetGlobalState modtask error");
                    }

                    Task.Run(() => { _clock.InvokeNotify(GlobalStateName.MODTASK, NotifyPlugin.Kafka, NotifyAction.STOPTASK, new DbpTask() { Taskid = nOldTaskID, Endtime = DateTimeFormat.DateTimeFromString(strStartTime) } ); });
                    Task.Run(() => { _clock.InvokeNotify(GlobalStateName.ADDTASK, NotifyPlugin.Kafka, NotifyAction.ADDTASK, task); });
                }
                return Response;
            }
            catch (Exception e)//其他未知的异常，写异常日志
            {

                //Response.nCode = 500;
                //Response.errStr = "error info:" + e.ToString();
                Response.nCode = 0;
                Logger.Error("AutoAddTaskByOldTask" + e.ToString());
                return Response;
            }
            return Response;

        }

        [HttpPost("AddReScheduleTaskSvr"), MapToApiVersion("1.0")]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<TaskOldResponseMessage<int>> AddReScheduleTaskSvr([FromBody] AddReScheduleTaskSvr_in pIn)
        {
            var Response = new TaskOldResponseMessage<int>();

            try
            {
                string CaptureMeta = string.Empty;
                string ContentMeta = string.Empty;
                string MatiralMeta = string.Empty;
                string PlanningMeta = string.Empty;
                foreach (var item in pIn.metadatas)
                {
                    if (item.emtype == MetaDataType.emCapatureMetaData)
                    {
                        CaptureMeta = item.strMetadata;
                    }
                    else if (item.emtype == MetaDataType.emStoreMetaData)
                    {
                        MatiralMeta = item.strMetadata;
                    }
                    else if (item.emtype == MetaDataType.emContentMetaData)
                    {
                        ContentMeta = item.strMetadata;
                    }
                    else if (item.emtype == MetaDataType.emPlanMetaData)
                    {
                        PlanningMeta = item.strMetadata;
                    }
                }
                /*
                 * @brief 老代码会通过老任务查询一遍policy，再保存入库，由于现在入库策略就一种，所以去掉那部分逻辑
                 */
                var f = await _taskManage.AddTaskWithPolicy<AddReScheduleTaskSvr_in>(pIn, true, CaptureMeta, ContentMeta, MatiralMeta, PlanningMeta);

                if (f == null)
                {
                    Response.nCode = 0;
                    Response.message = "error";
                    return Response;
                }

                //添加后如果开始时间在2分钟以内，需要调度一次
                if ((DateTimeFormat.DateTimeFromString(pIn.taskAdd.strBegin) - DateTime.Now).TotalSeconds < 120)
                    await _taskManage.UpdateComingTasks();

                
                if (_globalInterface != null)
                {
                    GlobalInternals re = new GlobalInternals() { Funtype = IngestDBCore.GlobalInternals.FunctionType.SetGlobalState, State = GlobalStateName.ADDTASK, TaskID = f.Channelid.GetValueOrDefault() };
                    var response1 = await _globalInterface.Value.SubmitGlobalCallBack(re);
                    if (response1.Code != ResponseCodeDefines.SuccessCode)
                    {
                        Logger.Error("SetGlobalState modtask error");
                    }

                    Task.Run(() => { _clock.InvokeNotify(GlobalStateName.ADDTASK, NotifyPlugin.Kafka, NotifyAction.ADDTASK, f); });
                }
                Response.extention = f.Taskid;
                return Response;
            }
            catch (Exception e)//其他未知的异常，写异常日志
            {
                Response.nCode = 0;
                Response.extention = -1;
                    Logger.Error("AddReScheduleTaskSvr" + e.Message);
                return Response;
            }
            return Response;

        }
        ////////////////////////////
    }
}
