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
                    Response.errStr = "error info：" + e.ToString();
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
                    Response.errStr = "error info：" + e.ToString();
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
                    Response.errStr = "error info：" + e.ToString();
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
                    Response.errStr = "error info：" + e.ToString();
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
                    Response.errStr = "error info：" + e.ToString();
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
                    Response.errStr = "error info：" + e.ToString();
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
                Response.newTaskId = (await _taskManage.AddTaskWithoutPolicy());

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
                    Response.errStr = "error info：" + e.ToString();
                    Logger.Error(Response.errStr);
                }
                return Response;
            }

        }
        ////////////////////////////
    }
}
