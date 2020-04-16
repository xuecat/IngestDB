using IngestDBCore;
using IngestDBCore.Basic;
using IngestDBCore.Interface;
using IngestDBCore.Tool;
using IngestTaskPlugin.Dto;
using IngestTaskPlugin.Managers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Sobey.Core.Log;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IngestTaskPlugin.Controllers
{
    //[IngestAuthentication]
    //[Produces("application/json")]
    //[ApiVersion("1.0")]
    //[Route("api/v0/task")]
    //[ApiController]
    public partial class TaskController : ControllerBase
    {
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
                return await _taskManage.GetTaskMetadataAsync<GetQueryTaskMetaData_param>(nTaskID, Type);
            }
            catch (Exception e)
            {
                var Response = new GetQueryTaskMetaData_param()
                {
                    bRet = false
                };
                if (e.GetType() == typeof(SobeyRecException))//sobeyexcep会自动打印错误
                {
                    SobeyRecException se = e as SobeyRecException;
                    Response.errStr = se.ErrorCode.ToString();
                }
                else
                {
                    Response.errStr = "OldGetTaskMetaData error info：" + e.ToString();
                    Logger.Error(Response.errStr);
                }
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
                    pIn.MateData = System.Guid.NewGuid().ToString();

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
                if (e.GetType() == typeof(SobeyRecException))//sobeyexcep会自动打印错误
                {
                    SobeyRecException se = e as SobeyRecException;
                    Response.errStr = se.ErrorCode.ToString();
                }
                else
                {
                    Response.errStr = "PostSetTaskMetaData error info：" + e.ToString();
                    Logger.Error(Response.errStr);
                }
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
                ret.bRet = true;
                ret.errStr = "OK";
                return ret;
            }
            catch (Exception e)
            {
                var Response = new GetTaskCustomMetadata_OUT()
                {
                    bRet = false
                };
                if (e.GetType() == typeof(SobeyRecException))//sobeyexcep会自动打印错误
                {
                    SobeyRecException se = e as SobeyRecException;
                    Response.errStr = se.ErrorCode.ToString();
                }
                else
                {
                    Response.errStr = "GetTaskCustomMetadata error info：" + e.ToString();
                    Logger.Error(Response.errStr);
                }
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
                if (e.GetType() == typeof(SobeyRecException))//sobeyexcep会自动打印错误
                {
                    SobeyRecException se = e as SobeyRecException;
                    Response.errStr = se.ErrorCode.ToString();
                }
                else
                {
                    Response.errStr = "PostSetTaskCustomMetadata error info：" + e.ToString();
                    Logger.Error(Response.errStr);
                }
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

                var _globalinterface = ApplicationContext.Current.ServiceProvider.GetRequiredService<IIngestGlobalInterface>();
                if (_globalinterface != null)
                {
                    GlobalInternals re = new GlobalInternals() { funtype = IngestDBCore.GlobalInternals.FunctionType.SetGlobalState, State = GlobalStateName.MODTASK };
                    var response1 = await _globalinterface.SubmitGlobalCallBack(re);
                    if (response1.Code != ResponseCodeDefines.SuccessCode)
                    {
                        Logger.Error("SetGlobalState modtask error");
                    }
                }

                return Response;
            }
            catch (Exception e)
            {
                Response.bRet = false;
                if (e.GetType() == typeof(SobeyRecException))//sobeyexcep会自动打印错误
                {
                    SobeyRecException se = e as SobeyRecException;
                    Response.errStr = se.ErrorCode.ToString();
                }
                else
                {
                    Response.errStr = "StopGroupTaskById error info：" + e.ToString();
                    Logger.Error(Response.errStr);
                }
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

                var _globalinterface = ApplicationContext.Current.ServiceProvider.GetRequiredService<IIngestGlobalInterface>();
                if (_globalinterface != null)
                {
                    GlobalInternals re = new GlobalInternals() { funtype = IngestDBCore.GlobalInternals.FunctionType.SetGlobalState, State = GlobalStateName.MODTASK };
                    var response1 = await _globalinterface.SubmitGlobalCallBack(re);
                    if (response1.Code != ResponseCodeDefines.SuccessCode)
                    {
                        Logger.Error("SetGlobalState modtask error");
                    }
                }

                return Response;
            }
            catch (Exception e)
            {
                Response.bRet = false;
                if (e.GetType() == typeof(SobeyRecException))//sobeyexcep会自动打印错误
                {
                    SobeyRecException se = e as SobeyRecException;
                    Response.errStr = se.ErrorCode.ToString();
                }
                else
                {
                    Response.errStr = "DeleteGroupTaskById error info：" + e.ToString();
                    Logger.Error(Response.errStr);
                }
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
                var f = await _taskManage.AddTaskWithoutPolicy<AddTaskSvr_IN>(pIn, CaptureMeta, ContentMeta, MatiralMeta, PlanningMeta);
               
                Response.newTaskId = f.TaskID;
                //添加后如果开始时间在2分钟以内，需要调度一次
                if ((DateTimeFormat.DateTimeFromString(pIn.taskAdd.strBegin) - DateTime.Now).TotalSeconds < 120)
                    await _taskManage.UpdateComingTasks();

                var _globalinterface = ApplicationContext.Current.ServiceProvider.GetRequiredService<IIngestGlobalInterface>();
                if (_globalinterface != null)
                {
                    GlobalInternals re = new GlobalInternals() { funtype = IngestDBCore.GlobalInternals.FunctionType.SetGlobalState, State = GlobalStateName.ADDTASK };
                    var response1 = await _globalinterface.SubmitGlobalCallBack(re);
                    if (response1.Code != ResponseCodeDefines.SuccessCode)
                    {
                        Logger.Error("SetGlobalState modtask error");
                    }
                }

                return Response;
            }
            catch (Exception e)
            {
                Response.bRet = false;
                if (e.GetType() == typeof(SobeyRecException))//sobeyexcep会自动打印错误
                {
                    SobeyRecException se = e as SobeyRecException;
                    Response.errStr = se.ErrorCode.ToString();
                }
                else
                {
                    Response.errStr = "PostAddTaskSvr error info：" + e.ToString();
                    Logger.Error(Response.errStr);
                }
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
                var f = await _taskManage.AddTaskWithPolicy<AddTaskSvrPolicysAndBackupFlag_IN>(pIn, false,CaptureMeta, ContentMeta, MatiralMeta, PlanningMeta);
                Response.taskBack = _taskManage.ConvertTaskResponse(f);
                Response.newTaskId = Response.taskBack.nTaskID;

                if (pIn.isCreateBackupTask)
                {
                    pIn.taskAdd.nTaskID = Response.newTaskId;
                    Response.backupTaskId = (await _taskManage.AddTaskWithPolicy<AddTaskSvrPolicysAndBackupFlag_IN>(pIn, true, CaptureMeta, ContentMeta, MatiralMeta, PlanningMeta)).TaskID;

                }

                //添加后如果开始时间在2分钟以内，需要调度一次
                if ((DateTimeFormat.DateTimeFromString(pIn.taskAdd.strBegin) - DateTime.Now).TotalSeconds < 120)
                    await _taskManage.UpdateComingTasks();

                var _globalinterface = ApplicationContext.Current.ServiceProvider.GetRequiredService<IIngestGlobalInterface>();
                if (_globalinterface != null)
                {
                    GlobalInternals re = new GlobalInternals() { funtype = IngestDBCore.GlobalInternals.FunctionType.SetGlobalState, State = GlobalStateName.ADDTASK };
                    var response1 = await _globalinterface.SubmitGlobalCallBack(re);
                    if (response1.Code != ResponseCodeDefines.SuccessCode)
                    {
                        Logger.Error("SetGlobalState modtask error");
                    }
                }


                return Response;
            }
            catch (Exception e)
            {
                Response.bRet = false;
                if (e.GetType() == typeof(SobeyRecException))//sobeyexcep会自动打印错误
                {
                    SobeyRecException se = e as SobeyRecException;
                    Response.errStr = se.ErrorCode.ToString();
                }
                else
                {
                    Response.errStr = "PostAddTaskSvr error info：" + e.ToString();
                    Logger.Error(Response.errStr);
                }
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
                errStr = "OK"
            };

            try
            {
                Response.nTaskID = await _taskManage.GetTaskIDByTaskGUID(strTaskGUID);
                return Response;
            }
            catch (Exception e)
            {
                if (e.GetType() == typeof(SobeyRecException))//sobeyexcep会自动打印错误
                {
                    SobeyRecException se = e as SobeyRecException;
                    Response.errStr = se.ErrorCode.ToString();
                }
                else
                {
                    Response.errStr = "error info：" + e.ToString();
                    Logger.Error("GetTaskIDByTaskGUID" + e.ToString());
                }
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
                if (e.GetType() == typeof(SobeyRecException))//sobeyexcep会自动打印错误
                {
                    SobeyRecException se = e as SobeyRecException;
                    Response.errStr = se.ErrorCode.ToString();
                }
                else
                {
                    Response.errStr = "error info：" + e.ToString();
                    Logger.Error("GetAllChannelCapturingTask" + e.ToString());
                }
                return Response;
            }

        }

        [HttpGet("GetChannelCapturingTask"), MapToApiVersion("1.0")]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<GetChannelCapturingTask_out> GetChannelCapturingTask(int nChannelID)
        {
            var Response = new GetChannelCapturingTask_out
            {
                bRet = true,
                errStr = "OK",
                nChannelID = nChannelID
            };

            try
            {
                Response.content = await _taskManage.GetChannelCapturingTask<TaskContent>(nChannelID);
                return Response;
            }
            catch (Exception e)
            {
                if (e.GetType() == typeof(SobeyRecException))//sobeyexcep会自动打印错误
                {
                    SobeyRecException se = e as SobeyRecException;
                    Response.errStr = se.ErrorCode.ToString();
                }
                else
                {
                    Response.errStr = "error info：" + e.ToString();
                    Logger.Error("GetAllChannelCapturingTask" + e.ToString());
                }
                return Response;
            }
        }

        [HttpPost("GetChannelCapturingTask"), MapToApiVersion("1.0")]
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
                await _taskManage.ModifyTask<TaskContent>(pIn.taskModify, string.Empty, pIn.TaskMetaData, pIn.MaterialMetaData, string.Empty);
                //添加后如果开始时间在2分钟以内，需要调度一次
                if ((DateTimeFormat.DateTimeFromString(pIn.taskModify.strBegin) - DateTime.Now).TotalSeconds < 120)
                    await _taskManage.UpdateComingTasks();

                var _globalinterface = ApplicationContext.Current.ServiceProvider.GetRequiredService<IIngestGlobalInterface>();
                if (_globalinterface != null)
                {
                    GlobalInternals re = new GlobalInternals() { funtype = IngestDBCore.GlobalInternals.FunctionType.SetGlobalState, State = GlobalStateName.MODTASK };
                    var response1 = await _globalinterface.SubmitGlobalCallBack(re);
                    if (response1.Code != ResponseCodeDefines.SuccessCode)
                    {
                        Logger.Error("SetGlobalState modtask error");
                    }
                }

                return Response;
            }
            catch (Exception e)
            {
                if (e.GetType() == typeof(SobeyRecException))//sobeyexcep会自动打印错误
                {
                    SobeyRecException se = e as SobeyRecException;
                    Response.errStr = se.ErrorCode.ToString();
                }
                else
                {
                    Response.errStr = "error info：" + e.ToString();
                    Logger.Error("PostModifyTaskDb" + e.ToString());
                }
                return Response;
            }
            return Response;
        }

        [HttpPost("GetChannelCapturingTask"), MapToApiVersion("1.0")]
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

                await _taskManage.ModifyTask<TaskContent>(pIn.taskModify, CaptureMeta, ContentMeta, MatiralMeta, PlanningMeta);
                //添加后如果开始时间在2分钟以内，需要调度一次
                if ((DateTimeFormat.DateTimeFromString(pIn.taskModify.strBegin) - DateTime.Now).TotalSeconds < 120)
                    await _taskManage.UpdateComingTasks();

                var _globalinterface = ApplicationContext.Current.ServiceProvider.GetRequiredService<IIngestGlobalInterface>();
                if (_globalinterface != null)
                {
                    GlobalInternals re = new GlobalInternals() { funtype = IngestDBCore.GlobalInternals.FunctionType.SetGlobalState, State = GlobalStateName.MODTASK };
                    var response1 = await _globalinterface.SubmitGlobalCallBack(re);
                    if (response1.Code != ResponseCodeDefines.SuccessCode)
                    {
                        Logger.Error("SetGlobalState modtask error");
                    }
                }

                return Response;
            }
            catch (Exception e)
            {
                if (e.GetType() == typeof(SobeyRecException))//sobeyexcep会自动打印错误
                {
                    SobeyRecException se = e as SobeyRecException;
                    Response.errStr = se.ErrorCode.ToString();
                }
                else
                {
                    Response.errStr = "error info：" + e.ToString();
                    Logger.Error("ModifyTask" + e.ToString());
                }
                return Response;
            }
            return Response;
        }

        [HttpGet("GetTaskByID"), MapToApiVersion("1.0")]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<GetTaskByID_OUT> GetTaskByID(int nTaskID)
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
                if (e.GetType() == typeof(SobeyRecException))//sobeyexcep会自动打印错误
                {
                    SobeyRecException se = e as SobeyRecException;
                    Response.errStr = se.ErrorCode.ToString();
                }
                else
                {
                    Response.errStr = "error info：" + e.ToString();
                    Logger.Error("GetTaskByID" + e.ToString());
                }
                return Response;
            }
        }

        [HttpGet("GetTaskByIDForFSW"), MapToApiVersion("1.0")]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<GetTaskByIDForFSW_OUT> GetTaskByIDForFSW(int nTaskID)
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
                if (e.GetType() == typeof(SobeyRecException))//sobeyexcep会自动打印错误
                {
                    SobeyRecException se = e as SobeyRecException;
                    Response.errStr = se.ErrorCode.ToString();
                }
                else
                {
                    Response.errStr = "error info：" + e.ToString();
                    Logger.Error("GetTaskByIDForFSW" + e.ToString());
                }
                return Response;
            }
        }

        [HttpGet("GetTieUpTaskByChannelID"), MapToApiVersion("1.0")]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<GetTieUpTaskByChannelID_OUT> GetTieUpTaskByChannelID(int nChannelID)
        {
            var Response = new GetTieUpTaskByChannelID_OUT
            {
                bRet = true,
                errStr = "OK",
            };

            try
            {
                Response.nTaskID = await _taskManage.GetTieUpTaskIDByChannelId(nChannelID);
                return Response;
            }
            catch (Exception e)
            {
                if (e.GetType() == typeof(SobeyRecException))//sobeyexcep会自动打印错误
                {
                    SobeyRecException se = e as SobeyRecException;
                    Response.errStr = se.ErrorCode.ToString();
                }
                else
                {
                    Response.errStr = "error info：" + e.ToString();
                    Logger.Error("GetTieUpTaskByChannelID" + e.ToString());
                }
                return Response;
            }
        }

        [HttpGet("GetStopTaskFromFSW"), MapToApiVersion("1.0")]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<StopTaskFromFSW_OUT> GetStopTaskFromFSW(int nTaskID)
        {
            var Response = new StopTaskFromFSW_OUT
            {
                bRet = true,
                errStr = "OK",
            };

            try
            {
                await _taskManage.StopTask(nTaskID, DateTime.MinValue);
                var _globalinterface = ApplicationContext.Current.ServiceProvider.GetRequiredService<IIngestGlobalInterface>();
                if (_globalinterface != null)
                {
                    GlobalInternals re = new GlobalInternals() { funtype = IngestDBCore.GlobalInternals.FunctionType.SetGlobalState, State = GlobalStateName.MODTASK };
                    var response1 = await _globalinterface.SubmitGlobalCallBack(re);
                    if (response1.Code != ResponseCodeDefines.SuccessCode)
                    {
                        Logger.Error("SetGlobalState modtask error");
                    }
                }
                return Response;
            }
            catch (Exception e)
            {
                if (e.GetType() == typeof(SobeyRecException))//sobeyexcep会自动打印错误
                {
                    SobeyRecException se = e as SobeyRecException;
                    Response.errStr = se.ErrorCode.ToString();
                }
                else
                {
                    Response.errStr = "error info：" + e.ToString();
                    Logger.Error("GetStopTaskFromFSW" + e.ToString());
                }
                return Response;
            }
        }

        [HttpGet("GetStopTaskFromFSW"), MapToApiVersion("1.0")]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<StopCapture_OUT> GetStopCapture(int nTaskID, string strEndTime)
        {
            var Response = new StopCapture_OUT
            {
                bRet = true,
                errStr = "OK",
            };

            try
            {
                if (string.IsNullOrEmpty(strEndTime))
                {
                    await _taskManage.StopTask(nTaskID, DateTime.Now);
                }
                else
                    await _taskManage.StopTask(nTaskID, DateTimeFormat.DateTimeFromString(strEndTime));

                var _globalinterface = ApplicationContext.Current.ServiceProvider.GetRequiredService<IIngestGlobalInterface>();
                if (_globalinterface != null)
                {
                    GlobalInternals re = new GlobalInternals() { funtype = IngestDBCore.GlobalInternals.FunctionType.SetGlobalState, State = GlobalStateName.MODTASK };
                    var response1 = await _globalinterface.SubmitGlobalCallBack(re);
                    if (response1.Code != ResponseCodeDefines.SuccessCode)
                    {
                        Logger.Error("SetGlobalState modtask error");
                    }
                }
                return Response;
            }
            catch (Exception e)
            {
                if (e.GetType() == typeof(SobeyRecException))//sobeyexcep会自动打印错误
                {
                    SobeyRecException se = e as SobeyRecException;
                    Response.errStr = se.ErrorCode.ToString();
                }
                else
                {
                    Response.errStr = "error info：" + e.ToString();
                    Logger.Error("GetStopTaskFromFSW" + e.ToString());
                }
                return Response;
            }
        }

        [HttpGet("GetSetTaskState"), MapToApiVersion("1.0")]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<GetSetTaskState_OUT> GetSetTaskState(int nTaskID, int nState)
        {
            var Response = new GetSetTaskState_OUT
            {
                bRet = true,
                errStr = "OK",
            };

            try
            {
                await _taskManage.SetTaskState(nTaskID, nState);
                return Response;
            }
            catch (Exception e)
            {
                if (e.GetType() == typeof(SobeyRecException))//sobeyexcep会自动打印错误
                {
                    SobeyRecException se = e as SobeyRecException;
                    Response.errStr = se.ErrorCode.ToString();
                }
                else
                {
                    Response.errStr = "error info：" + e.ToString();
                    Logger.Error("GetStopTaskFromFSW" + e.ToString());
                }
                return Response;
            }
        }

        [HttpGet("GetSetTaskState"), MapToApiVersion("1.0")]
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
                Response.nVaildDataCount = Response.taskCon.Count;
                return Response;
            }
            catch (Exception e)
            {
                if (e.GetType() == typeof(SobeyRecException))//sobeyexcep会自动打印错误
                {
                    SobeyRecException se = e as SobeyRecException;
                    Response.errStr = se.ErrorCode.ToString();
                }
                else
                {
                    Response.errStr = "error info：" + e.ToString();
                    Logger.Error("GetStopTaskFromFSW" + e.ToString());
                }
                return Response;
            }
        }

        [HttpGet("GetTaskSource"), MapToApiVersion("1.0")]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<GetTaskSource_OUT> GetTaskSource(int nTaskID)
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
                if (e.GetType() == typeof(SobeyRecException))//sobeyexcep会自动打印错误
                {
                    SobeyRecException se = e as SobeyRecException;
                    Response.errStr = se.ErrorCode.ToString();
                }
                else
                {
                    Response.errStr = "error info：" + e.ToString();
                    Logger.Error("GetTaskSource" + e.ToString());
                }
                return Response;
            }
            return Response;

        }

        [HttpGet("GetTrimTaskBeginTime"), MapToApiVersion("1.0")]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<bool> GetTrimTaskBeginTime(int nTaskID, string strStartTime)
        {
            try
            {
                if (nTaskID < 0)
                {
                    return false;
                }
                await _taskManage.TrimTaskBeginTime(nTaskID, strStartTime);
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
                    //Response.errStr = "error info：" + e.ToString();
                    Logger.Error("GetTrimTaskBeginTime" + e.ToString());
                }
                return false;
            }
            return true;

        }

        [HttpPost("PostQueryTaskMetadataGroup"), MapToApiVersion("1.0")]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task<QueryTaskMetadataGroup_OUT> PostQueryTaskMetadataGroup(List<int> nTaskID)
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
                if (e.GetType() == typeof(SobeyRecException))//sobeyexcep会自动打印错误
                {
                    SobeyRecException se = e as SobeyRecException;
                    Response.errStr = se.ErrorCode.ToString();
                }
                else
                {
                    Response.errStr = "error info：" + e.ToString();
                    Logger.Error("GetTrimTaskBeginTime" + e.ToString());
                }
                return Response;
            }
            return Response;

        }
        ////////////////////////////
    }
}
