using AutoMapper;
using IngestDBCore;
using IngestDBCore.Interface;
using IngestDBCore.Notify;
using IngestDBCore.Tool;
using IngestTaskPlugin.Dto.OldResponse;
using IngestTaskPlugin.Dto.Response;
using IngestTaskPlugin.Managers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using Sobey.Core.Log;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IngestTaskPlugin.Controllers.v2._1
{
    
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("2.1")]
    [ApiController]
    public partial class TaskController : ControllerBase
    {
        private readonly ILogger Logger = LoggerManager.GetLogger("TaskInfo21");
        private readonly TaskManager _taskManage;
        private readonly Lazy<IIngestGlobalInterface> _globalInterface;

        public TaskController(TaskManager task, IServiceProvider services, IMapper mapper)
        {
            _taskManage = task;
            _globalInterface = new Lazy<IIngestGlobalInterface>(() => services.GetRequiredService<IIngestGlobalInterface>());
        }

        /// <summary>
        /// 获取任务全部的元数据
        /// </summary>
        /// <remarks>
        /// 例子:
        ///
        /// </remarks>
        /// <param name="taskid">任务id，</param>
        /// <returns>任务内容全部信息包含元数据</returns>
        [HttpGet("taskinfo/{taskid}/full")]
        [ApiExplorerSettings(GroupName = "v2.1")]
        public async Task<ResponseMessage<TaskFullInfoResponse>> GetTaskFullInfoByID([FromRoute, BindRequired]int taskid)
        {
            var Response = new ResponseMessage<TaskFullInfoResponse>();
            if (taskid <= 0)
            {
                Response.Code = ResponseCodeDefines.ModelStateInvalid;
                Response.Msg = "request param error";
            }

            try
            {
                Response.Ext = await _taskManage.GetTaskInfoAll<TaskFullInfoResponse>(taskid);
                if (Response.Ext == null)
                {
                    Response.Code = ResponseCodeDefines.NotFound;
                    Response.Msg = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}:error info: not find data!";
                }
            }
            catch (Exception e)
            {
                if (e.GetType() == typeof(SobeyRecException))//sobeyexcep会自动打印错误
                {
                    SobeyRecException se = e as SobeyRecException;
                    Response.Code = se.ErrorCode.ToString();
                    Response.Msg = se.Message;
                }
                else
                {
                    Response.Code = ResponseCodeDefines.ServiceError;
                    Response.Msg = "GetTaskFullInfoByID 21 error info:" + e.Message;
                    Logger.Error(Response.Msg);
                }
            }
            return Response;
        }

        /// <summary>
        /// 查询1天的任务
        /// </summary>
        /// <remarks>
        /// 例子:
        /// 这个接口是专门为web提供，新功能有个为任务创建临时rtmp url的。创建好了web端要显示那个url
        /// 但是这个url元数据是放在了metada里面的，查询一天任务的接口不提供。
        /// 最好的方法是修改数据库底层把signal改成string，这样搞要重新发网管，msg，task，客户端所有玩意。太坑了。
        /// 所以单独给webingest提供一个接口，当遇到rtmp url的任务时，多查询一次，并返回给他地址
        /// </remarks>
        /// <param name="unitid">随便填</param>
        /// <param name="day">查询时间yyyy/MM/dd HH:mm:ss </param>
        /// <param name="timemode">查询模式0是24小时模式，1是32小时模式</param>
        /// <returns>任务基础元数据</returns>
        [HttpGet("onedaytask")]
        [ApiExplorerSettings(GroupName = "v2.1")]
        public async Task<ResponseMessage<List<TaskContentSignalUrlResponse>>> QueryTaskContentForRtmpUrl([FromQuery, BindRequired]int unitid, [FromQuery, BindRequired]string day, [FromQuery, BindRequired]int timemode)
        {
            var Response = new ResponseMessage<List<TaskContentSignalUrlResponse>>();

            try
            {
                Response.Ext = await _taskManage.QueryTaskSignalUrlContent(unitid, DateTimeFormat.DateTimeFromString(day), (TimeLineType)timemode);
                if (Response.Ext == null)
                {
                    Response.Code = ResponseCodeDefines.NotFound;
                    Response.Msg = $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}:error info: not find data!";
                }
            }
            catch (Exception e)
            {
                if (e.GetType() == typeof(SobeyRecException))//sobeyexcep会自动打印错误
                {
                    SobeyRecException se = e as SobeyRecException;
                    Response.Code = se.ErrorCode.ToString();
                    Response.Msg = se.Message;
                }
                else
                {
                    Response.Code = ResponseCodeDefines.ServiceError;
                    Response.Msg = "QueryTaskContentForRtmpUrl error info:" + e.Message;
                    Logger.Error(Response.Msg);
                }
            }
            return Response;
        }
    }
}
