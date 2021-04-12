using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using AutoMapper;
using IngestDBCore;
using IngestDBCore.Interface;
using IngestDBCore.Tool;
using IngestTaskPlugin.Dto;
using IngestTaskPlugin.Dto.OldResponse;
using IngestTaskPlugin.Dto.Request;
using IngestTaskPlugin.Dto.Response;
using IngestTaskPlugin.Dto.Response.OldVtr;
using IngestTaskPlugin.Dto.OldResponse.OldVtr;
using IngestTaskPlugin.Extend;
using IngestTaskPlugin.Models;
using IngestTaskPlugin.Stores;
using IngestTaskPlugin.Stores.VTR;
using Microsoft.EntityFrameworkCore;
using Sobey.Core.Log;

using IngestTaskPlugin.Dto.OldVtr;

namespace IngestTaskPlugin.Managers
{
    
    /// <summary>
    /// VTR磁带管理.
    /// </summary>
    public class VtrManager
    {
        /// <summary>
        /// Defines the Logger.
        /// </summary>
        private readonly ILogger Logger = LoggerManager.GetLogger("VtrInfo");

        /// <summary>
        /// Defines the TaskStore.
        /// </summary>
        private readonly ITaskStore TaskStore;

        /// <summary>
        /// Defines the VtrStore.
        /// </summary>
        private readonly IVtrStore VtrStore;

        /// <summary>
        /// Defines the Mapper.
        /// </summary>
        private readonly IMapper Mapper;

        /// <summary>
        /// Defines the TaskManager.
        /// </summary>
        private readonly TaskManager TaskManager;

        private IIngestDeviceInterface _deviceInterface { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="VtrManager"/> class.
        /// </summary>
        /// <param name="mapper">The mapper<see cref="IMapper"/>.</param>
        /// <param name="taskStore">The taskStore<see cref="ITaskStore"/>.</param>
        /// <param name="vtrStore">The vtrStore<see cref="IVtrStore"/>.</param>
        /// <param name="taskManager">The taskManager<see cref="TaskManager"/>.</param>
        /// <param name="device">The taskManager<see />.</param>
        public VtrManager(IMapper mapper, ITaskStore taskStore, IVtrStore vtrStore, IIngestDeviceInterface device, TaskManager taskManager)
        {
            this.TaskStore = taskStore;
            this.VtrStore = vtrStore;
            this.Mapper = mapper;
            this.TaskManager = taskManager;
            _deviceInterface = device;
        }

        /// <summary>
        /// The 修改磁带信息,ID不存在即为添加.
        /// </summary>
        /// <param name="tapeId">The 磁带Id<see cref="int"/>.</param>
        /// <param name="tapeName">The 磁带名称<see cref="string"/>.</param>
        /// <param name="tapeDesc">The 磁带描述<see cref="string"/>.</param>
        /// <returns>The 磁带Id <see cref="Task{int}"/>.</returns>
        public async ValueTask<int> SetTapeInfoAsync<T>(T info)
        {
            var newTapeId = await VtrStore.SaveTaplist(Mapper.Map<VtrTapelist>(info));
            return newTapeId;
        }

        /// <summary>
        /// The 设置VTR与磁带的对应序列.
        /// </summary>
        /// <param name="vtrId">The VtrId <see cref="int"/>.</param>
        /// <param name="tapeId">The 磁带Id <see cref="int"/>.</param>
        /// <returns>The 是否成功<see cref="Task"/>.</returns>
        public ValueTask<bool> SetVtrTapeMapAsync(int vtrId, int tapeId)
        { return VtrStore.SaveTapeVtrMap(new VtrTapeVtrMap { Vtrid = vtrId, Tapeid = tapeId }); }

        /// <summary>
        /// The 获得所有的磁带信息.
        /// </summary>
        /// <returns>The 所有的磁带信息<see cref="Task{List{VTRTapeInfo}}"/>.</returns>
        public async Task<List<T>> GetAllTapeInfoAsync<T>()
        {
            return Mapper.Map<List<T>>(await VtrStore.GetTapelist(a => a.OrderByDescending(x => x.Tapeid)
                .Take(50)));
        }

        /// <summary>
        /// The 获得VTRID对应的默认磁带ID.
        /// </summary>
        /// <param name="vtrId">The vtrId<see cref="int"/>.</param>
        /// <returns>The 默认磁带ID<see cref="Task{int}"/>.</returns>
        public async ValueTask<int> GetVtrTapeItemAsync(int vtrId)
        {
            Logger.Info($"GetVtrTapeItemAsync {vtrId}");
            var result = await VtrStore.GetTapeVtrMap(a => a.Where(x => x.Vtrid == vtrId)
                .Select(x => x.Tapeid)
                .FirstOrDefaultAsync(), true);
            Logger.Info($"GetVtrTapeItemAsync end.");
            return result;
        }

        /// <summary>
        /// The 根据磁带ID获得磁带信息.
        /// </summary>
        /// <param name="tapeId">The 磁带ID<see cref="int"/>.</param>
        /// <returns>磁带信息.</returns>
        public async Task<T> GetTapeInfoByIDAsync<T>(int tapeId)
        {
            Logger.Info("GetTapeInfoByIDAsync before query");
            var result = await VtrStore.GetTapelist(a => a.FirstOrDefaultAsync(x => x.Tapeid == tapeId), true);
            Logger.Info("GetTapeInfoByIDAsync end query");
            return Mapper.Map<T>(result);
        }

        /// <summary>
        /// The 设置VTR任务信息，VTRTaskID不存在则添加,否则则修改.
        /// </summary>
        /// <param name="uploadTaskInfo">The VTR任务信息<see cref="VTRUploadTaskInfo"/>.</param>
        /// <returns>The 影响的Vtr任务Id<see cref="Task{int}"/>.</returns>
        public async ValueTask<int> SetVTRUploadTaskInfoAsync<TRequest>(TRequest uploadTaskInfo)
        {
            VTRUploadTaskInfoResponse info = Mapper.Map<VTRUploadTaskInfoResponse>(uploadTaskInfo);

            int retTaskID = -1;

            var vtrtask = await TaskStore.GetVtrUploadTaskAsync(a => a.Where(b => b.Taskid == info.VtrTaskId), true);

            Logger.Info($"SetVTRUploadTaskInfo vtrtask : {JsonHelper.ToJson(vtrtask)}");

            DateTime beginCheckTime = info.CommitTime;

            TimeSpan tsDuration = new TimeSpan();
            if (info.BlankTaskId > 0)//入点加长度
            {
                tsDuration = new TimeSpan(0, 0, info.TrimOutCtl / info.BlankTaskId);
            }
            else
            {
                SB_TimeCode tcIn = new SB_TimeCode((uint)info.TrimInCtl);
                SB_TimeCode tcOut = new SB_TimeCode((uint)info.TrimOutCtl);
                if ((uint)info.TrimOutCtl < (uint)info.TrimInCtl)
                {
                    tcOut.Hour += 24;
                }
                tsDuration = tcOut - tcIn;
            }

            TimePeriod tp = new TimePeriod(info.VtrId, beginCheckTime, beginCheckTime + tsDuration);

            await IsTimePeriodUsable(tp, info.ChannelId, info.VtrId, info.VtrTaskId);

            if (vtrtask == null)
            {
                retTaskID = info.VtrTaskId = TaskStore.GetNextValId("DBP_SQ_TASKID");

                if (info.CommitTime == DateTime.MinValue)
                {
                    info.CommitTime = DateTime.Now;
                }

                if (string.IsNullOrEmpty(info.TaskGuid))
                {
                    info.TaskGuid = Guid.NewGuid().ToString("N");
                }

                var upload = Mapper.Map<VtrUploadtask>(info);
                if (upload.Tapeid == 0)
                {
                    upload.Tapeid = await VtrStore.GetTapeVtrMap(a => a.Where(x => x.Vtrid == upload.Vtrid)
                        .Select(x => x.Tapeid)
                        .SingleOrDefaultAsync(), true);
                }

                //SB_TimeCode tcIn = new SB_TimeCode((uint)upload.Trimin);
                //SB_TimeCode tcOut = new SB_TimeCode((uint)upload.Trimout);
                //TimeSpan taskDuration = tcOut - tcIn;

                //DateTime begintime = DateTime.Now;
                //DateTime endtime = DateTime.Now + taskDuration;

                //var task = new DbpTask()
                //{
                //    Backtype = (int)CooperantType.emPureTask,
                //    Category = "A",
                //    Channelid = upload.Recchannelid,

                //    DispatchState = (int)dispatchState.dpsNotDispatch,
                //    OpType = (int)opType.otAdd,
                //    State = (int)taskState.tsReady,
                //    SyncState = (int)syncState.ssNot,

                //    Starttime = begintime,
                //    Endtime = endtime,
                //    NewBegintime = begintime,
                //    NewEndtime = endtime,
                //    OldChannelid = 0,
                //    Recunitid = 1,
                //    Signalid = upload.Signalid,
                //    Taskid = upload.Taskid,
                //    Tasklock = "",
                //    Taskname = upload.Taskname,
                //    Tasktype = (int)TaskType.TT_VTRUPLOAD,
                //    Usercode = upload.Usercode,
                //    Taskguid = upload.Taskguid,
                //    Backupvtrid = 0
                //};
                //if (vtrtask.Vtrtasktype == (int)VTRUPLOADTASKTYPE.VTR_MANUAL_UPLOAD)
                //{
                //    task.State = (int)taskState.tsExecuting;
                //    task.DispatchState = (int)dispatchState.dpsDispatched;
                //    task.OpType = (int)opType.otAdd;
                //    task.SyncState = (int)syncState.ssNot;
                //}
                //Logger.Info($"SetVTRUploadTaskInfoAsync {task.Taskid} {task.Starttime} {task.Endtime} {task.Channelid} {task.State}");
                //await TaskStore.AddTask(task, false);
                await VtrStore.AddUploadtask(upload);

                //填充任务来源表
                //vtr uploadtask只能是emVTRUploadTask还有别的选择？

                //var taskSource = new DbpTaskSource();
                //taskSource.Taskid = info.VtrTaskId;
                //taskSource.Tasksource = (int)TaskSource.emVTRUploadTask;

                //await TaskStore.AddTaskSource(taskSource);

                await AddPolicyTaskByUserCode(info.UserCode, info.VtrTaskId);
            }
            else
            {
                if (info.CommitTime == DateTime.MinValue)
                {
                    info.CommitTime = DateTime.Now;
                }
                info.VtrTaskId = vtrtask.Taskid;
                var upload = Mapper.Map<VtrUploadtask>(info);
                if (upload.Tapeid == 0)
                {
                    upload.Tapeid = await VtrStore.GetTapeVtrMap(a => a.Where(x => x.Vtrid == upload.Vtrid)
                        .Select(x => x.Tapeid)
                        .SingleOrDefaultAsync(), true);
                }
                
                await VtrStore.UpdateUploadtask(upload);

                retTaskID = info.VtrTaskId;
            }

            //UpdateTime = DateTime.Now;

            return retTaskID;
        }

        /// <summary>
        /// The 设置VTR任务元数据.
        /// </summary>
        /// <param name="taskId">The 任务Id<see cref="int"/>.</param>
        /// <param name="type">The 元数据类型<see cref="MetaDataType"/>.</param>
        /// <param name="metadata">The 元数据<see cref="string"/>.</param>
        /// <param name="isSubmit">是否需要提交<see cref="bool"/>.</param>
        public async Task SetVBUTasksMetadatasAsync(int taskId, MetaDataType type, string metadata, bool isSubmit = true)
        {
            //需要将其中的三个字符串提取出来
            if (type == MetaDataType.emContentMetaData)
            {
                string materialMeta = string.Empty;
                string planningMeta = string.Empty;
                string originalMeta = string.Empty;
                XElement xElement = XElement.Parse(metadata);
                //xElement.LoadXml(metadata);
                //XmlNode taskContentNode = doc.SelectSingleNode("/TaskContentMetaData");
                //var taskContentNode = xElement.Elements("TaskContentMetaData").FirstOrDefault();
                if (xElement != null)
                {
                    if (xElement.HasElements)
                    {
                        //XmlNode materialNode = doc.SelectSingleNode("/TaskContentMetaData/MetaMaterial");
                        var materialNode = xElement.Element("MetaMaterial");
                        if (materialNode != null)
                        {
                            materialMeta = materialNode.FirstNode?.ToString();
                            //taskContentNode.RemoveChild(materialNode);
                            materialNode.Remove();
                        }

                        //XmlNode planningNode = doc.SelectSingleNode("/TaskContentMetaData/MetaPlanning");
                        var planningNode = xElement.Element("MetaPlanning");
                        if (planningNode != null)
                        {
                            planningMeta = planningNode.FirstNode?.ToString();
                            //taskContentNode.RemoveChild(planningNode);
                            planningNode.Remove();
                        }

                        //XmlNode originalNode = doc.SelectSingleNode("/TaskContentMetaData/MetaOriginal");
                        var originalNode = xElement.Element("MetaOriginal");
                        if (originalNode != null)
                        {
                            originalMeta = originalNode.FirstNode?.ToString();
                            //taskContentNode.RemoveChild(originalNode);
                            originalNode.Remove();
                        }
                    }
                    await TaskStore.UpdateTaskMetaDataAsync(taskId, MetaDataType.emContentMetaData, xElement.ToString(), false);//doc.OuterXml);
                }

                await TaskStore.UpdateTaskMetaDataAsync(taskId, MetaDataType.emStoreMetaData, materialMeta,false);
                await TaskStore.UpdateTaskMetaDataAsync(taskId, MetaDataType.emPlanMetaData, planningMeta, false);
                await TaskStore.UpdateTaskMetaDataAsync(taskId, MetaDataType.emOriginalMetaData, originalMeta, false);
                
            }
            else
            {
                await TaskStore.UpdateTaskMetaDataAsync(taskId, type, metadata,false);
            }

            if (isSubmit)
            {
                await TaskStore.SaveChangeAsync(ITaskStore.VirtualContent& ITaskStore.DefaultContent);
            }

        }

        /// <summary>
        /// The 查询Vtr上传任务信息.
        /// </summary>
        /// <param name="condition">The 查询条件<see cref="TCondition"/>.</param>
        /// <returns>The Vtr上传任务信息<see cref="Task{List{TResult}}"/>.</returns>
        public async Task<List<TResult>> QueryVTRUploadTaskInfoAsync<TResult, TCondition>(TCondition condition)
        {
            var conditionRequest = Mapper.Map<VTRUploadConditionRequest>(condition);
            return Mapper.Map<List<TResult>>(await VtrStore.GetUploadtaskInfo(conditionRequest, true));
        }

        /// <summary>
        /// The 查询Vtr上传任务及内容.
        /// </summary>
        /// <param name="condition">The 查询条件<see cref="TCondition"/>.</param>
        /// <returns>The Vtr上传任务及内容<see cref="Task{List{TResult}}"/>.</returns>
        public async Task<List<TResult>> GetVTRUploadTasksAsync<TResult, TCondition>(TCondition condition)
        {
            var conditionRequest = Mapper.Map<VTRUploadConditionRequest>(condition);
            return Mapper.Map<List<TResult>>(await VtrStore.GetUploadTaskContent(conditionRequest));
        }

        /// <summary>
        /// The 根据任务Id获取 Vtr上传任务 和 内容.
        /// </summary>
        /// <param name="taskId">The 任务Id<see cref="int"/>.</param>
        /// <returns>磁带信息<see cref="Task{TResult}"/>.</returns>
        public async Task<TResult> GetVTRUploadTaskByIdAsync<TResult>(int taskId)
        {
            var conditionRequest = new VTRUploadConditionRequest { TaskId = taskId };
            return Mapper.Map<List<TResult>>(await VtrStore.GetUploadTaskContent(conditionRequest)).FirstOrDefault();
        }

        /// <summary>
        /// The 根据 VtrId 获取指定的VTR信息.
        /// </summary>
        /// <param name="vtrId">The vtrId<see cref="int"/>.</param>
        /// <returns>The VTR信息<see cref="Task{TResult}"/>.</returns>
        public async Task<TResult> GetVTRDetailInfoByIDAsync<TResult>(int vtrId)
        {
            var vtrinfo = Mapper.Map<TResult>(await VtrStore.GetDetailinfo(a => a.SingleOrDefaultAsync(x => x.Vtrid ==
                vtrId)));
            if (vtrinfo == null)
            {
                SobeyRecException.ThrowSelfNoParam(string.Format(GlobalDictionary.Instance
                    .GetMessageByCode(GlobalDictionary.GLOBALDICT_CODE_VTRID_DOES_NOT_EXIST_THE_NVTRID_IS_ONEPARAM),
                                                                 vtrId.ToString()),
                                                   GlobalDictionary.GLOBALDICT_CODE_VTRID_DOES_NOT_EXIST_THE_NVTRID_IS_ONEPARAM,
                                                   Logger,
                                                   null);
            }
            return vtrinfo;
        }

        /// <summary>
        /// The 获取VTR状态.
        /// </summary>
        /// <param name="vtrId">The vtrId<see cref="int"/>.</param>
        /// <returns>The VTR状态<see cref="Task{VtrState}"/>.</returns>
        public async Task<VtrState> GetVtrStateAsync(int vtrId)
        {
            return (VtrState)await VtrStore.GetDetailinfo(a => a.Where(x => x.Vtrid == vtrId)
                .Select(x => x.Vtrstate)
                .SingleOrDefaultAsync());
        }

        /// <summary>
        /// The 根据taskId 更新 Vtr上传任务状态.
        /// </summary>
        /// <param name="taskId">The taskId<see cref="int"/>.</param>
        /// <param name="taskState">The vtr任务状态<see cref="int"/>.</param>
        /// <returns>The 是否更新成功<see cref="Task{bool}"/>.</returns>
        public ValueTask<bool> UpdateUploadTaskStateAsync(int taskId, VTRUPLOADTASKSTATE taskState)
        { return SetVTRUploadTaskStateAsync(taskId, taskState, string.Empty); }

        /// <summary>
        /// The 根据taskId 更新 Vtr上传任务状态..
        /// </summary>
        /// <param name="taskId">The taskId<see cref="int"/>.</param>
        /// <param name="vtrTaskState">The vtr任务状态<see cref="VTRUPLOADTASKSTATE"/>.</param>
        /// <param name="errorContent">The 错误内容<see cref="string"/>.</param>
        /// <returns>The 是否更新成功 <see cref="Task{bool}"/>.</returns>
        public async ValueTask<bool> SetVTRUploadTaskStateAsync(int taskId,
                                                           VTRUPLOADTASKSTATE vtrTaskState,
                                                           string errorContent)
        {
            var upload = await VtrStore.GetUploadtask(a => a.FirstOrDefaultAsync(x => x.Taskid == taskId), true);

            if (upload != null)
            {
                if (vtrTaskState == VTRUPLOADTASKSTATE.VTR_UPLOAD_COMMIT &&
                    (upload.Taskstate == (int)VTRUPLOADTASKSTATE.VTR_UPLOAD_COMPLETE))
                {//已入库素材重新上载是，改变GUID以保证再次入库时不会覆盖前面的素材
                    upload.Taskguid = Guid.NewGuid().ToString("N");
                }

                upload.Taskstate = (int)vtrTaskState;
                if (vtrTaskState == VTRUPLOADTASKSTATE.VTR_UPLOAD_FAIL)
                {
                    upload.Usertoken = errorContent;
                }
                return await VtrStore.UpdateUploadtask(upload);
            }
            //UpdateTime = DateTime.Now;
            return false;
        }

        /// <summary>
        /// The 获取所有vtr信息.
        /// </summary>
        /// <returns>The 所有vtr信息<see cref="Task{List{TResult}}"/>.</returns>
        public async Task<List<TResult>> GetUsableVtrListAsync<TResult>()
        {
            Logger.Debug("before get Detailinfo.");
            var result = await VtrStore.GetDetailinfo(a => a, true);
            Logger.Debug($"after get Detailinfo. {result.Count}");
            return Mapper.Map<List<TResult>>(result); 
        }

        /// <summary>
        /// The 获得需要即将调度的VTR任务.
        /// </summary>
        /// <returns>The 即将调度的VTR任务<see cref="Task{List{TResult}}"/>.</returns>
        public async Task<List<TResult>> GetNeedExecuteVTRUploadTasksAsync<TResult>()
        {
            var uploadTaskList = Mapper.Map<List<VTRUploadTaskContentResponse>>(await VtrStore.GetUploadtask(a => a.Where(x => x.Taskstate ==
                (int)VTRUPLOADTASKSTATE.VTR_UPLOAD_PRE_EXECUTE &&
                x.Vtrtasktype == (int)VTRUPLOADTASKTYPE.VTR_MANUAL_UPLOAD),
                                                                                                             true));

            List<TaskContent> capturingTasks = await TaskManager.GetAllChannelCapturingTask<TaskContent>();

            //再获得计划的上载任务
            List<VTRUploadTaskContentResponse> vtrTasks = await VtrStore.GetNeedScheduleExecuteVTRUploadTasks(DateTime.Now);

            if (vtrTasks != null && vtrTasks.Count > 0)
            {
                //每个通道只能返回一个，每个VTR也只能返回一个，而且只能返回时间最前的那个                
                foreach (VTRUploadTaskContentResponse vtrScheduleTask in vtrTasks)
                {
                    //重新调整开始时间

                    /*
                     * wq 把调整时间去了，每次任务失败都会自动拉伸长度
                     */

                    //DateTime dtEnd = vtrScheduleTask.EndTime;
                    //if (dtEnd <= DateTime.Now)
                    //{
                    //    //重新调整开始时间跟结束时间 
                    //    SB_TimeCode tcIn = new SB_TimeCode((uint)vtrScheduleTask.TrimIn);
                    //    SB_TimeCode tcOut = new SB_TimeCode((uint)vtrScheduleTask.TrimOut);
                    //    TimeSpan tsDuration = tcOut - tcIn;
                    //    vtrScheduleTask.BeginTime = DateTime.Now;
                    //    vtrScheduleTask.EndTime = DateTime.Now + tsDuration;
                    //    await TaskStore.AdjustVtrUploadTasksByChannelId(vtrScheduleTask.ChannelId,
                    //                                                    vtrScheduleTask.TaskId,
                    //                                                    DateTime.Now);
                    //}

                    int i = 0;
                    bool isNeedReplace = false;
                    for (; i < uploadTaskList.Count; i++)
                    {
                        if (uploadTaskList[i].ChannelId == vtrScheduleTask.ChannelId ||
                            uploadTaskList[i].VtrId == vtrScheduleTask.VtrId)
                        {
                            //只返回时间上最小的
                            if (vtrScheduleTask.BeginTime < uploadTaskList[i].BeginTime)
                            {
                                isNeedReplace = true;
                                break;
                            }
                        }
                    }

                    bool isHaveCapturingManulTask = false;
                    if (capturingTasks != null)
                    {
                        foreach (TaskContent capturingTask in capturingTasks)
                        {
                            if (capturingTask.nChannelID == vtrScheduleTask.ChannelId)
                            {
                                if (capturingTask.emCooperantType == CooperantType.emKamataki)
                                {
                                    TaskSource ts = await TaskManager.GetTaskSource(capturingTask.nTaskID);
                                    //加上判断，如果遇到vtr上载任务的话，那么先不管，但是不让返回，等待下次判断
                                    if (ts != TaskSource.emVTRUploadTask)
                                    {
                                        await SetVTRUploadTaskStateAsync(vtrScheduleTask.TaskId,
                                                                         VTRUPLOADTASKSTATE.VTR_UPLOAD_FAIL,
                                                                         "VTRBATCHUPLOAD_ERROR_COLLIDEMANUALORKAMATAKITASK");
                                        await TaskManager.SetTaskState(vtrScheduleTask.TaskId, (int)taskState.tsDelete);
                                    }
                                    isHaveCapturingManulTask = true;
                                }
                                break;
                            }
                        }
                    }

                    //没有找到相同的vtr或者channel的任务
                    if (i == 0 || (i == uploadTaskList.Count && !isNeedReplace))
                    {
                        if (!isHaveCapturingManulTask)
                        {
                            uploadTaskList.Add(vtrScheduleTask);
                        }
                    }

                    if (isNeedReplace)
                    {
                        if (!isHaveCapturingManulTask)
                        {
                            if (i - 1 >= 0)
                            {
                                uploadTaskList.RemoveAt(i - 1);
                                uploadTaskList.Add(vtrScheduleTask);
                            }
                        }
                    }
                }
            }

            return Mapper.Map<List<TResult>>(uploadTaskList);
        }

        /// <summary>
        /// The 获得即将要执行的VTR任务，用来提示TapeID切换的
        /// </summary>
        /// <param name="minute">The 时间段<see cref="int"/>.</param>
        /// <returns>The 即将执行的VTR<see cref="Task{List{TResult}}"/>.</returns>
        public async Task<List<TResult>> GetWillExecuteVTRUploadTasksAsync<TResult>(int minute)
        { return Mapper.Map<List<TResult>>(await VtrStore.GetWillExecuteVTRUploadTasks(minute)); }

        /// <summary>
        /// The 根据UserCode添加任务策略
        /// </summary>
        /// <param name="userCode">The userCode<see cref="string"/>.</param>
        /// <param name="vtrTaskId">The vtrTaskId<see cref="int"/>.</param>
        /// <returns>The 是否添加成功<see cref="Task"/>.</returns>
        private async ValueTask<bool> AddPolicyTaskByUserCode(string userCode, int vtrTaskId)
        {
            List<DbpPolicytask> tasks = new List<DbpPolicytask>();
            //首先根据User ID查找Policy ID
            var policys = await VtrStore.GetPolicyuser(a => a.Where(x => x.Usercode == userCode));
            if (policys !=null && policys.Count > 0)
            {
                var policyIds = policys.Select(x => x.Policyid).ToList();
                tasks = await VtrStore.GetMetadatapolicy(a => a.Where(x => policyIds.Contains(x.Policyid))
                    .Select(x => new DbpPolicytask { Policyid = x.Policyid, Taskid = vtrTaskId }));
            }
            else
            {
                tasks = await VtrStore.GetMetadatapolicy(a => a.Where(x => x.Defaultpolicy != 0)
                    .Select(x => new DbpPolicytask { Policyid = x.Policyid, Taskid = vtrTaskId }));
            }
            /*
             * 设置policy为默认值，现在只有这个了
             */
            if (tasks.Count > 0)
            {
                tasks.ForEach(x => x.Policyid = 1);
            }
            else
                tasks.Add(new DbpPolicytask() { Policyid = 1, Taskid = vtrTaskId});
            
            return await TaskStore.AddPolicyTask(tasks);
        }

        /// <summary>
        /// The 判断是否是可用的时间段。
        /// </summary>
        /// <param name="tp">The 判断的时间段<see cref="TimePeriod"/>.</param>
        /// <param name="channelId">The 通道Id<see cref="int"/>.</param>
        /// <param name="vtrId">The vtrId<see cref="int"/>.</param>
        /// <param name="exTaskId">The exTaskId<see cref="int"/>.</param>
        /// <returns>The 是否可用<see cref="Task{bool}"/>.</returns>
        private async ValueTask<bool> IsTimePeriodUsable(TimePeriod tp, int channelId, int vtrId, int exTaskId)
        {
            //对进行修改的任务的开始与结束时间段，判断其通道和vtr是否可用
            VTRTimePeriods vtrFreeTimePeriods = new VTRTimePeriods(vtrId);
            vtrFreeTimePeriods.Periods = new List<TimePeriod>();
            DateTime beginCheckTime = tp.StartTime;

            await GetFreeTimePeriodByVtrId(vtrFreeTimePeriods, beginCheckTime, exTaskId);

            if (!IsTimePeriodInVTRTimePeriods(tp, vtrFreeTimePeriods))
            {
                SobeyRecException.ThrowSelfNoParam(nameof(IsTimePeriodUsable) + "VTR Collide",
                                                   GlobalDictionary.GLOBALDICT_CODE_IN_ISVTRCOLLIDE_BEGINTIME_IS_WRONG,
                                                   Logger,
                                                   null);
                return false;
            }

            List<int> channelIds = new List<int>();
            List<ChannelTimePeriods> channelsTimePeriods = new List<ChannelTimePeriods>();
            channelIds.Add(channelId);
            channelsTimePeriods = await GetChannelsFreeTimePeriods(beginCheckTime, channelIds, exTaskId);

            if (channelsTimePeriods != null &&
                channelsTimePeriods[0] != null &&
                !IsTimePeriodInChannelTimePeriods(tp, channelsTimePeriods[0]))
            {
                SobeyRecException.ThrowSelfNoParam(nameof(IsTimePeriodUsable) + "No Channel",
                                                   GlobalDictionary.GLOBALDICT_CODE_IN_ISVTRCOLLIDE_BEGINTIME_IS_WRONG,
                                                   Logger,
                                                   null);
                return false;
            }
            return true;
        }

        /// <summary>
        /// The 判断是否在vtr时间段.
        /// </summary>
        /// <param name="tp">The 判断的时间段<see cref="TimePeriod"/>.</param>
        /// <param name="vtrFreeTimePeriods">The vtr时间段<see cref="VTRTimePeriods"/>.</param>
        /// <returns>The 是否在vtr时间段<see cref="bool"/>.</returns>
        private bool IsTimePeriodInVTRTimePeriods(TimePeriod tp, VTRTimePeriods vtrFreeTimePeriods)
        {
            if (vtrFreeTimePeriods.Periods != null)
            {
                return vtrFreeTimePeriods.Periods.Any(x => x.EndTime >= tp.EndTime && x.StartTime <= tp.StartTime);
            }
            return false;
        }

        /// <summary>
        /// The 判断是否在通道时间段.
        /// </summary>
        /// <param name="tp">The 判断的时间段<see cref="TimePeriod"/>.</param>
        /// <param name="channelFreeTimePeriods">The 通道时间段<see cref="ChannelTimePeriods"/>.</param>
        /// <returns>The 是否在通道时间段<see cref="bool"/>.</returns>
        private bool IsTimePeriodInChannelTimePeriods(TimePeriod tp, ChannelTimePeriods channelFreeTimePeriods)
        {
            if (channelFreeTimePeriods.Periods != null)
            {
                return channelFreeTimePeriods.Periods.Any(a => a.EndTime >= tp.EndTime && a.StartTime <= tp.StartTime);
            }
            return false;
        }

        /// <summary>
        /// Defines the taskStates.
        /// </summary>
        private readonly int?[] taskStates = new[]
        {
            (int?)VTRUPLOADTASKSTATE.VTR_UPLOAD_COMMIT,
            (int?)VTRUPLOADTASKSTATE.VTR_UPLOAD_PRE_EXECUTE,
            (int?)VTRUPLOADTASKSTATE.VTR_UPLOAD_EXECUTE
        };

        /// <summary>
        /// The 根据VtrId获取Free时间段.
        /// </summary>
        /// <param name="vtrFreeTimePeriods">The vtrFree时间段<see cref="VTRTimePeriods"/>.</param>
        /// <param name="beginCheckTime">The 开始检测时间<see cref="DateTime"/>.</param>
        /// <param name="exTaskId">The exTaskId<see cref="int"/>.</param>
        /// <returns>The 取Free时间段<see cref="Task{VTRTimePeriods}"/>.</returns>
        private async Task<VTRTimePeriods> GetFreeTimePeriodByVtrId(VTRTimePeriods vtrFreeTimePeriods,
                                                                    DateTime beginCheckTime,
                                                                    int exTaskId)
        {
            if (vtrFreeTimePeriods.VTRId <= 0)
            {
                SobeyRecException.ThrowSelfNoParam($"{nameof(GetFreeTimePeriodByVtrId)}: VTRId is smaller than 0",
                                                   GlobalDictionary.GLOBALDICT_CODE_IN_SETVTRTAPEMAP_TAPEID_IS_NOT_EXIST_ONEPARAM,
                                                   Logger,
                                                   null);
            }

            if (beginCheckTime == DateTime.MinValue)
            {
                beginCheckTime = DateTime.Now;
            }

            vtrFreeTimePeriods.Periods.Clear();
            VTRTimePeriods vtrTimePeriods = new VTRTimePeriods();

            int vtrId = vtrFreeTimePeriods.VTRId;

            int taskType = (int)VTRUPLOADTASKTYPE.VTR_MANUAL_UPLOAD;
            //查询手动的任务占用的VTR时间，已经在执行的
            var manualCapturingTask = Mapper.Map<VTRUploadTaskInfo>(await TaskStore.GetVtrUploadTaskAsync(a => a.Where(x => taskStates.Contains(x.Taskstate) &&
                x.Vtrtasktype == taskType &&
                x.Vtrid == vtrId)));
            if (manualCapturingTask != null)
            {
                DateTime beginTime = DateTimeFormat.DateTimeFromString(manualCapturingTask.strCommitTime);
                TimeSpan tsDuration = new TimeSpan();
                if (manualCapturingTask.nBlankTaskID > 0)//入点加长度
                {
                    tsDuration = new TimeSpan(0, 0, manualCapturingTask.nTrimOutCTL / manualCapturingTask.nBlankTaskID);
                }
                else
                {
                    SB_TimeCode inSTC = new SB_TimeCode((uint)manualCapturingTask.nTrimInCTL);
                    SB_TimeCode outSTC = new SB_TimeCode((uint)manualCapturingTask.nTrimOutCTL);
                    tsDuration = outSTC - inSTC;
                }

                DateTime endTime = beginTime + tsDuration;

                vtrTimePeriods.Periods.Add(new TimePeriod(vtrId, beginTime, endTime));
            }

            List<TimePeriod> scheduleTPs = await TaskStore.GetTimePeriodsByScheduleVBUTasks(vtrId, exTaskId);
            if (scheduleTPs != null && scheduleTPs.Count > 0)
            {
                vtrTimePeriods.Periods.AddRange(scheduleTPs);
            }

            DateTime thirdDay = beginCheckTime.AddDays(3).AddSeconds(-1);
            vtrFreeTimePeriods.Periods = TaskManager.GetFreeTimePeriodsByTieup(vtrId,
                                                                               vtrTimePeriods.Periods,
                                                                               beginCheckTime.AddSeconds(-3),
                                                                               thirdDay);

            vtrFreeTimePeriods.Periods = vtrFreeTimePeriods.Periods.Where(a => a.Duration.TotalSeconds > 3).ToList();
            return vtrFreeTimePeriods;
        }

        /// <summary>
        /// The 获取通道Free时间段.
        /// </summary>
        /// <param name="beginTime">The 开始时间<see cref="DateTime"/>.</param>
        /// <param name="channelIds">The 通道Id集<see cref="List{int}"/>.</param>
        /// <param name="exTaskId">The exTaskId<see cref="int"/>.</param>
        /// <returns>The 通道Free时间段<see cref="List{ChannelTimePeriods}"/>.</returns>
        private async Task<List<ChannelTimePeriods>> GetChannelsFreeTimePeriods(DateTime beginTime,
                                                                                List<int> channelIds,
                                                                                int exTaskId)
        {
            if (beginTime < new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0) ||
                channelIds == null ||
                channelIds.Count <= 0)
            {
                return null;
            }

            //查询出最近48小时之内，有多少个空闲的时间段
            List<ChannelTimePeriods> chsTimePeriods = channelIds.Select(a => new ChannelTimePeriods(a)).ToList();//记录用了多少时间段
            List<ChannelTimePeriods> chsFreeTimePeriods = channelIds.Select(a => new ChannelTimePeriods(a)).ToList();//记录空间的多少时间段

            //取出三天的任务，进行过滤
            List<TaskContent> tasks = new List<TaskContent>();
            for (int i = 0; i < 3; i++)
            {
                List<TaskContent> tasksIn = await TaskManager.QueryTaskContent<TaskContent>(0,
                                                                                            DateTime.Now.AddDays(i),
                                                                                            TimeLineType.em24HourDay);
                if (tasksIn != null && tasksIn.Count > 0)
                {
                    tasks.AddRange(tasksIn);
                }
            }

            if (tasks == null || (tasks.Count == 1 && tasks[0] == null))
            {
                foreach (ChannelTimePeriods ctp in chsFreeTimePeriods)
                {
                    ctp.Periods.Add(new TimePeriod(ctp.ChannelId, beginTime, beginTime.AddDays(2)));
                }
                return chsFreeTimePeriods;
            }

            //收集一些通道的被占用情况
            var preiodsTasks = tasks.Where(a => a.nTaskID != exTaskId &&
                DateTimeFormat.DateTimeFromString(a.strEnd) > beginTime)
                .ToList();
            foreach (var task in preiodsTasks)
            {
                var ctp = chsTimePeriods.FirstOrDefault(a => a.ChannelId == task.nChannelID);
                if (ctp != null)
                {
                    DateTime dtBegin = DateTimeFormat.DateTimeFromString(task.strBegin);
                    DateTime dtEnd = DateTimeFormat.DateTimeFromString(task.strEnd);
                    if ((task.emTaskType == TaskType.TT_MANUTASK || task.emTaskType == TaskType.TT_OPENEND) &&
                        task.emState == taskState.tsExecuting)
                    {
                        dtEnd = dtBegin.AddDays(1);
                        dtBegin = DateTime.Now;
                    }
                    if (dtEnd > beginTime)
                    {
                        ctp.Periods.Add(new TimePeriod(ctp.ChannelId, dtBegin, dtEnd));
                    }
                }
            }

            //对通道里的时间段进行排序
            foreach (var ctp in chsTimePeriods)
            {
                DateTime thirdDay = new DateTime(beginTime.Year, beginTime.Month, beginTime.Day, 23, 59, 59);
                thirdDay = thirdDay.AddDays(2);

                var actp = chsFreeTimePeriods.FirstOrDefault(a => a.ChannelId == ctp.ChannelId);
                if (actp != null)
                {
                    actp.Periods = TaskManager.GetFreeTimePeriodsByTieup(ctp.ChannelId, ctp.Periods, beginTime, thirdDay);
                    actp.Periods = actp.Periods.Where(a => a.Duration.TotalSeconds > 3).ToList();
                }
                
            }

            return chsFreeTimePeriods;
        }


        //#region vtr task update
        //public async Task<TResult> SetVTRUploadTask<TResult, TRequest>(TRequest request)
        //{
        //    SetVTRUploadTask_in reque = Mapper.Map<SetVTRUploadTask_in>(request);
        //    VTRUploadTaskContent vtrTask = reque.vtrTask;
        //    List<VTR_UPLOAD_MetadataPair> metadatas = reque.metadatas;
        //    long lMask = reque.lMask;
        //}
        //public async Task<TResult> SetVTRUploadTask<TResult,TRequest>(TRequest request)
        //{
        //    SetVTRUploadTask_in reque = Mapper.Map<SetVTRUploadTask_in>(request);

        //    //VTRUploadTaskContent vtrTask = reque.vtrTask;
        //    //List<VTR_UPLOAD_MetadataPair> metadatas = reque.metadatas;
        //    //long lMask = reque.lMask;

        #region vtr task update
        public async Task<TResult>
        SetVTRUploadTaskAsync<TResult, TPTwo>(TResult vtrTaskp, List<TPTwo> metadatasp, long lMask)
        {
            var vtrTask = Mapper.Map<VTRUploadTaskContent>(vtrTaskp);
            var metadatas = Mapper.Map<List<VTR_UPLOAD_MetadataPair>>(metadatasp);

            if (vtrTask.nTaskId <= 0)
            {
                return default(TResult);
            }

            //判断vtr任务当前状态，如果是正在采集时，不能改变长度
            VTRUploadConditionRequest Condition = new VTRUploadConditionRequest() {  TaskId = vtrTask.nTaskId };
            List<VTRUploadTaskContentResponse> vtrUploadTasks = await VtrStore.GetUploadTaskContent(Condition);//id是主键，理论上只能查出一条

            // 新增从普通任务转换为VTR任务 VTR表中无法查询到任务，该任务原本可能是一个普通任务
            if (vtrUploadTasks == null || vtrUploadTasks.Count <= 0)
            {
                SobeyRecException.ThrowSelfOneParam( "", GlobalDictionary.GLOBALDICT_CODE_CAN_NOT_FIND_THE_TASK_ONEPARAM, Logger, vtrTask.nTaskId, null);
                //if (vtrTask.emTaskState != VTRUPLOADTASKSTATE.VTR_UPLOAD_COMMIT && vtrTask.emVtrTaskType != VTRUPLOADTASKTYPE.VTR_SCHEDULE_UPLOAD)
                //{
                //    throw new Exception("Can not find the task.TaskId = " + vtrTask.nTaskId);
                //}
                
                //var dbpTask = await TaskStore.GetTaskAsync(a => a.Where(x => x.Taskid == vtrTask.nTaskId));
                //if (dbpTask == null)
                //{
                //    throw new Exception("Can not find the task in DBP_TASK.TaskId = " + vtrTask.nTaskId);
                //}

                //if (dbpTask.State != (int)taskState.tsReady)
                //{
                //    throw new Exception($"Can not modify a normal task to vtr task which is not in ready state.TaskId = {vtrTask.nTaskId} ");
                //}

                //if (dbpTask.Tasktype != (int)TaskType.TT_NORMAL)
                //{
                //    throw new Exception("Can not modify a task to vtr task which is not a normal task.TaskId = " + vtrTask.nTaskId);
                //}

                //try
                //{
                //    Logger.Info("In SetVTRUploadTask.Before ModifyNormalTaskToVTRUploadTask");
                //    await ModifyNormalTaskToVTRUploadTaskAsync(vtrTask, metadatas, dbpTask);


                //    return Mapper.Map<TResult>(vtrTask);
                //}
                //catch (System.Exception ex)
                //{
                //    throw ex;
                //}
            }

            VTRUploadTaskContentResponse vtrTaskNow = vtrUploadTasks[0];
            //正在执行的状态下，不允许更新时间，不允许更新通道，不允许更新信号源，不允许更新vtrId
            if (vtrTaskNow.TaskState == VTRUPLOADTASKSTATE.VTR_UPLOAD_EXECUTE ||
                vtrTaskNow.State == taskState.tsExecuting)
            {
                Logger.Info("begin to into filling vtr jugde modify!");
                Logger.Info(string.Format("vtrTaskNow.strBegin:{0},vtrTask.strBegin:{1}, vtrTaskNow.strEnd:{2}, vtrTask.strEnd:{3},", vtrTaskNow.BeginTime, vtrTask.strBegin, vtrTaskNow.EndTime, vtrTask.strEnd));
                Logger.Info(string.Format("vtrTaskNow.nTrimIn:{0},vtrTask.nTrimIn:{1}, vtrTaskNow.nTrimOut:{2}, vtrTask.nTrimOut:{3}, vtrTaskNow.TrimInCtl：{4}, vtrTask.nTrimInCTL:{5},vtrTaskNow.TrimOutCtl:{6},vtrTask.nTrimOutCTL:{7}",
                    vtrTaskNow.TrimIn, vtrTask.nTrimIn, vtrTaskNow.TrimOut, vtrTask.nTrimOut, vtrTaskNow.TrimInCtl, vtrTask.nTrimInCTL, vtrTaskNow.TrimOutCtl, vtrTask.nTrimOutCTL));
                Logger.Info(string.Format("vtrTaskNow.nChannelId:{0}, vtrTask.nChannelId:{1},vtrTaskNow.nSignalId:{2}, vtrTask.nSignalId:{3}, vtrTaskNow.nVtrId:{4},vtrTask.nVtrId:{5} ,", vtrTaskNow.ChannelId, vtrTask.nChannelId, vtrTaskNow.SignalId, vtrTask.nSignalId, vtrTaskNow.VtrId, vtrTask.nVtrId));
                if (lMask <= 0)
                {
                    DateTime dt1 = vtrTaskNow.BeginTime;
                    DateTime dt2 = DateTimeFormat.DateTimeFromString(vtrTask.strBegin);

                    DateTime dt3 = vtrTaskNow.EndTime;
                    DateTime dt4 = DateTimeFormat.DateTimeFromString(vtrTask.strEnd);

                    if (dt1 != dt2 || dt3 != dt4 ||
                        vtrTaskNow.TrimIn != vtrTask.nTrimIn ||
                        vtrTaskNow.TrimInCtl != vtrTask.nTrimInCTL ||
                        vtrTaskNow.TrimOut != vtrTask.nTrimOut ||
                        vtrTaskNow.TrimOutCtl != vtrTask.nTrimOutCTL)
                    {
                        Logger.Info(string.Format("1Can not modify the duration where the task is running.TaskId = {0},TaskName = {1}", vtrTaskNow.TaskId, vtrTaskNow.TaskName));
                        SobeyRecException.ThrowSelfNoParam(vtrTaskNow.TaskId.ToString(), GlobalDictionary.GLOBALDICT_CODE_CANNOTMODIFYTASK_WHERE_FILING, Logger, null);
                    }

                    if (vtrTaskNow.ChannelId != vtrTask.nChannelId)
                    {
                        Logger.Info(string.Format("2Can not modify the channel where the task is running.TaskId = {0},TaskName = {1}", vtrTaskNow.TaskId, vtrTaskNow.TaskName));
                        SobeyRecException.ThrowSelfNoParam(vtrTaskNow.TaskId.ToString(), GlobalDictionary.GLOBALDICT_CODE_CANNOTMODIFYTASK_WHERE_FILING, Logger, null);
                    }

                    if (vtrTaskNow.SignalId != vtrTask.nSignalId)
                    {
                        Logger.Info(string.Format("3Can not modify the signal where the task is running.TaskId = {0},TaskName = {1}", vtrTaskNow.TaskId, vtrTaskNow.TaskName));
                        SobeyRecException.ThrowSelfNoParam(vtrTaskNow.TaskId.ToString(), GlobalDictionary.GLOBALDICT_CODE_CANNOTMODIFYTASK_WHERE_FILING, Logger, null);
                    }

                    if (vtrTaskNow.VtrId != vtrTask.nVtrId)
                    {
                        Logger.Info(string.Format("4Can not modify the vtr where the task is running.TaskId = {0},TaskName = {1}", vtrTaskNow.TaskId, vtrTaskNow.TaskName));
                        SobeyRecException.ThrowSelfNoParam(vtrTaskNow.TaskId.ToString(), GlobalDictionary.GLOBALDICT_CODE_CANNOTMODIFYTASK_WHERE_FILING, Logger, null);
                    }
                }
                else
                {
                    if (IsInMask(lMask, VTRUploadTaskMask.VTR_Mask_BeginTime) ||
                        IsInMask(lMask, VTRUploadTaskMask.VTR_Mask_EndTime) ||
                        IsInMask(lMask, VTRUploadTaskMask.VTR_Mask_TrimIn) ||
                        IsInMask(lMask, VTRUploadTaskMask.VTR_Mask_TrimOut) ||
                        IsInMask(lMask, VTRUploadTaskMask.VTR_Mask_TrimInCTL) ||
                        IsInMask(lMask, VTRUploadTaskMask.VTR_Mask_TrimOutCTL))
                    {
                        Logger.Info(string.Format("5Can not modify the duration where the task is running.TaskId = {0},TaskName = {1}", vtrTaskNow.TaskId, vtrTaskNow.TaskName));
                        SobeyRecException.ThrowSelfNoParam(vtrTaskNow.TaskId.ToString(), GlobalDictionary.GLOBALDICT_CODE_CANNOTMODIFYTASK_WHERE_FILING, Logger, null);
                    }

                    if (IsInMask(lMask, VTRUploadTaskMask.VTR_Mask_ChannelId))
                    {
                        Logger.Info(string.Format("6Can not modify the channel where the task is running.TaskId = {0},TaskName = {1}", vtrTaskNow.TaskId, vtrTaskNow.TaskName));
                        //throw new Exception(string.Format("Can not modify the channel where the task is running.TaskId = {0},TaskName = {1}", vtrTaskNow.nTaskId, vtrTaskNow.strTaskName));
                        SobeyRecException.ThrowSelfNoParam(vtrTaskNow.TaskId.ToString(), GlobalDictionary.GLOBALDICT_CODE_CANNOTMODIFYTASK_WHERE_FILING, Logger, null);
                    }

                    if (IsInMask(lMask, VTRUploadTaskMask.VTR_Mask_SignalId))
                    {
                        Logger.Info(string.Format("7Can not modify the signal where the task is running.TaskId = {0},TaskName = {1}", vtrTaskNow.TaskId, vtrTaskNow.TaskName));
                        //throw new Exception(string.Format("Can not modify the signal where the task is running.TaskId = {0},TaskName = {1}", vtrTaskNow.nTaskId, vtrTaskNow.strTaskName));
                        SobeyRecException.ThrowSelfNoParam(vtrTaskNow.TaskId.ToString(), GlobalDictionary.GLOBALDICT_CODE_CANNOTMODIFYTASK_WHERE_FILING, Logger, null);
                    }

                    if (IsInMask(lMask, VTRUploadTaskMask.VTR_Mask_VtrId))
                    {
                        Logger.Info(string.Format("8Can not modify the vtr where the task is running.TaskId = {0},TaskName = {1}", vtrTaskNow.TaskId, vtrTaskNow.TaskName));
                        //throw new Exception(string.Format("Can not modify the vtr where the task is running.TaskId = {0},TaskName = {1}", vtrTaskNow.nTaskId, vtrTaskNow.strTaskName));
                        SobeyRecException.ThrowSelfNoParam(vtrTaskNow.TaskId.ToString(), GlobalDictionary.GLOBALDICT_CODE_CANNOTMODIFYTASK_WHERE_FILING, Logger, null);
                    }
                }
            }

            if (vtrTask.emTaskState == VTRUPLOADTASKSTATE.VTR_UPLOAD_COMMIT)
            {
                DateTime beginCheckTime = DateTime.MinValue;
                if (!string.IsNullOrEmpty(vtrTask.strBegin))
                {
                    beginCheckTime = DateTimeFormat.DateTimeFromString(vtrTask.strBegin);
                }

                TimeSpan tsDuration = new TimeSpan();
                if (vtrTask.nBlankTaskId > 0)//入点加长度
                {
                    tsDuration = new TimeSpan(0, 0, vtrTask.nTrimOut / vtrTask.nBlankTaskId);
                }
                else
                {
                    SB_TimeCode tcIn = new SB_TimeCode((uint)vtrTask.nTrimIn);
                    SB_TimeCode tcOut = new SB_TimeCode((uint)vtrTask.nTrimOut);
                    tsDuration = tcOut - tcIn;
                }

                if (beginCheckTime != DateTime.MinValue)
                {
                    TimePeriod tp = new TimePeriod(vtrTask.nVtrId, beginCheckTime, beginCheckTime + tsDuration);
                    string errStr = string.Empty;

                    if (!await IsTimePeriodUsable(tp, vtrTask.nChannelId, vtrTask.nVtrId, vtrTask.nTaskId))
                    {
                        throw new Exception(errStr);
                    }
                }
            }

            // TEMPSAVE任务时间不变
            if (vtrTask.emTaskState == VTRUPLOADTASKSTATE.VTR_UPLOAD_TEMPSAVE)
            {
                vtrTask.strBegin = DateTimeFormat.DateTimeToString(vtrTaskNow.BeginTime);
                vtrTask.strEnd = DateTimeFormat.DateTimeToString(vtrTaskNow.EndTime);
            }

            var dbptask = (await TaskStore.GetTaskNotrackAsync(a => a.Where(x => x.Taskid == vtrTask.nTaskId), true));

            if (dbptask == null)
            {
                throw new Exception("Can not find the row in DBP_TASK table,TaskId = " + vtrTask.nTaskId.ToString());
            }

            dbptask = VTRUploadTaskContent2Dbptask(false, vtrTask, dbptask,  lMask);
            VtrUploadtask vtrUploadtaskResult = Mapper.Map<VtrUploadtask>(vtrTaskNow);
            vtrUploadtaskResult = VTRUploadTaskContent2VTRUPLOADTASK(vtrTask, vtrUploadtaskResult, lMask);

            await TaskStore.UpdateTaskAsync(dbptask, false);
            await VtrStore.UpdateUploadtask(vtrUploadtaskResult);

            if (metadatas != null && metadatas.Count > 0)
            {
                //foreach (var meta in metadatas)
                //{
                //    await SetVBUTasksMetadatasAsync(vtrTask.nTaskId, (MetaDataType)meta.emType, meta.strMetadata);
                //}
                await UpdateTasksMetadatas(metadatas, vtrTask.nTaskId);
            }

            return Mapper.Map<TResult>(vtrTask);
        }

        private VtrUploadtask VTRUploadTaskContent2VTRUPLOADTASK(VTRUploadTaskContent task, VtrUploadtask vtrUploadtask, long lMask)
        {
            if (lMask <= 0)
            {
                //vtrUploadtask = Mapper.Map<VtrUploadtask>(task);
                //VTR任务ID
                vtrUploadtask.Taskid = task.nTaskId;

                //占位上载任务ID
                vtrUploadtask.Vtrtaskid = task.nBlankTaskId;

                //上载VTRID
                vtrUploadtask.Vtrid = task.nVtrId;

                //收录通道ID
                vtrUploadtask.Recchannelid = task.nChannelId;

                //磁带入点
                vtrUploadtask.Trimin = task.nTrimIn;

                //磁带出点
                vtrUploadtask.Trimout = task.nTrimOut;

                //信号源ID
                vtrUploadtask.Signalid = task.nSignalId;

                //磁带状态
                vtrUploadtask.Taskstate = Convert.ToInt32(task.emTaskState);

                //上载用户编码
                vtrUploadtask.Usercode = task.strUserCode;

                //提交时间
                vtrUploadtask.Committime = DateTimeFormat.DateTimeFromString(task.strCommitTime);

                //上载时序
                vtrUploadtask.Uploadorder = task.nOrder;

                //TASKGUID
                vtrUploadtask.Taskguid = task.strTaskGUID;

                vtrUploadtask.Taskname = task.strTaskName;
                //UserToken
                vtrUploadtask.Usertoken = task.strUserToken;

                vtrUploadtask.Tapeid = task.nTapeId;

                vtrUploadtask.Triminctl = task.nTrimInCTL;

                vtrUploadtask.Trimoutctl = task.nTrimOutCTL;
                //zmj2010-05-17
                vtrUploadtask.Vtrtasktype = (int)task.emVtrTaskType;
            }
            else
            {
                #region

                if (IsInMask(lMask, VTRUploadTaskMask.VTR_Mask_TaskId))
                {
                    vtrUploadtask.Taskid = task.nTaskId;
                }

                if (IsInMask(lMask, VTRUploadTaskMask.VTR_Mask_BlankTaskId))
                {
                    vtrUploadtask.Vtrtaskid = task.nBlankTaskId;
                }

                if (IsInMask(lMask, VTRUploadTaskMask.VTR_Mask_VtrId))
                {
                    vtrUploadtask.Vtrid = task.nVtrId;
                }

                if (IsInMask(lMask, VTRUploadTaskMask.VTR_Mask_ChannelId))
                {
                    vtrUploadtask.Recchannelid = task.nChannelId;
                }

                if (IsInMask(lMask, VTRUploadTaskMask.VTR_Mask_TrimIn))
                {
                    vtrUploadtask.Trimin = task.nTrimIn;
                }

                if (IsInMask(lMask, VTRUploadTaskMask.VTR_Mask_TrimOut))
                {
                    vtrUploadtask.Trimout = task.nTrimOut;
                }

                if (IsInMask(lMask, VTRUploadTaskMask.VTR_Mask_SignalId))
                {
                    vtrUploadtask.Signalid = task.nSignalId;
                }

                if (IsInMask(lMask, VTRUploadTaskMask.VTR_Mask_TaskState))
                {
                    vtrUploadtask.Taskstate = Convert.ToInt32(task.emTaskState);
                }

                if (IsInMask(lMask, VTRUploadTaskMask.VTR_Mask_UserCode))
                {
                    vtrUploadtask.Usercode = task.strUserCode;
                }

                if (IsInMask(lMask, VTRUploadTaskMask.VTR_Mask_CommitTime))
                {
                    vtrUploadtask.Committime = DateTimeFormat.DateTimeFromString(task.strCommitTime);
                }

                if (IsInMask(lMask, VTRUploadTaskMask.VTR_Mask_Order))
                {
                    vtrUploadtask.Uploadorder = task.nOrder;
                }

                if (IsInMask(lMask, VTRUploadTaskMask.VTR_Mask_TaskGUID))
                {
                    vtrUploadtask.Taskguid = task.strTaskGUID;
                }

                if (IsInMask(lMask, VTRUploadTaskMask.VTR_Mask_TaskName))
                {
                    vtrUploadtask.Taskname = task.strTaskName;
                }

                if (IsInMask(lMask, VTRUploadTaskMask.VTR_Mask_UserToken))
                {
                    vtrUploadtask.Usertoken = task.strUserToken;
                }

                if (IsInMask(lMask, VTRUploadTaskMask.VTR_Mask_TapeId))
                {
                    vtrUploadtask.Tapeid = task.nTapeId;
                }

                if (IsInMask(lMask, VTRUploadTaskMask.VTR_Mask_TrimInCTL))
                {
                    vtrUploadtask.Triminctl = task.nTrimInCTL;
                }

                if (IsInMask(lMask, VTRUploadTaskMask.VTR_Mask_TrimOutCTL))
                {
                    vtrUploadtask.Trimoutctl = task.nTrimOutCTL;
                }

                if (IsInMask(lMask, VTRUploadTaskMask.VTR_Mask_VtrTaskType))
                {
                    vtrUploadtask.Vtrtasktype = (int)task.emVtrTaskType;
                }

                #endregion
            }

            return vtrUploadtask;
        }

        private DbpTask VTRUploadTaskContent2Dbptask(bool isAdd, VTRUploadTaskContent task,DbpTask dbpTask, long lMask)
        {

            if (lMask <= 0)
            {
                dbpTask.Backtype = (int)CooperantType.emPureTask;
                dbpTask.Category = task.strClassify;
                dbpTask.Channelid = task.nChannelId;
                dbpTask.Description = task.strTaskDesc;

                if (isAdd)
                {
                    dbpTask.DispatchState = (int)dispatchState.dpsNotDispatch;
                    dbpTask.OpType = (int)opType.otAdd;
                    dbpTask.State = (int)taskState.tsReady;
                    dbpTask.SyncState = (int)syncState.ssNot;
                }

                dbpTask.Starttime = DateTimeFormat.DateTimeFromString(task.strBegin);
                dbpTask.Endtime = DateTimeFormat.DateTimeFromString(task.strEnd);
                dbpTask.NewBegintime = DateTimeFormat.DateTimeFromString(task.strBegin);
                dbpTask.NewEndtime = DateTimeFormat.DateTimeFromString(task.strEnd);
                dbpTask.OldChannelid = 0;

                dbpTask.Recunitid = 1;
                dbpTask.Signalid = task.nSignalId;

                dbpTask.Taskid = task.nTaskId;
                dbpTask.Tasklock = "";
                dbpTask.Taskname = task.strTaskName;
                dbpTask.Tasktype = (int)TaskType.TT_VTRUPLOAD;
                dbpTask.Usercode = task.strUserCode;
                dbpTask.Taskguid = task.strTaskGUID;
                dbpTask.Tasksource = (int)TaskSource.emVTRUploadTask;
                dbpTask.Backupvtrid = 0;
            }
            else
            {
                #region                 
                if (IsInMask(lMask, VTRUploadTaskMask.VTR_Mask_ChannelId))
                {
                    dbpTask.Channelid = task.nChannelId;
                }

                if (IsInMask(lMask, VTRUploadTaskMask.VTR_Mask_SignalId))
                {
                    dbpTask.Signalid = task.nSignalId;
                }

                if (IsInMask(lMask, VTRUploadTaskMask.VTR_Mask_UserCode))
                {
                    dbpTask.Usercode = task.strUserCode;
                }

                if (IsInMask(lMask, VTRUploadTaskMask.VTR_Mask_TaskGUID))
                {
                    dbpTask.Taskguid = task.strTaskGUID;
                }

                if (IsInMask(lMask, VTRUploadTaskMask.VTR_Mask_TaskName))
                {
                    dbpTask.Taskname = task.strTaskName;
                }

                if (IsInMask(lMask, VTRUploadTaskMask.VTR_Mask_Classify))
                {
                    dbpTask.Category = task.strClassify;
                }

                if (IsInMask(lMask, VTRUploadTaskMask.VTR_Mask_BeginTime))
                {
                    dbpTask.Starttime = DateTimeFormat.DateTimeFromString(task.strBegin);
                    dbpTask.NewBegintime = DateTimeFormat.DateTimeFromString(task.strBegin);
                }

                if (IsInMask(lMask, VTRUploadTaskMask.VTR_Mask_EndTime))
                {
                    dbpTask.NewEndtime = DateTimeFormat.DateTimeFromString(task.strEnd);
                    dbpTask.Endtime = DateTimeFormat.DateTimeFromString(task.strEnd);
                }

                if (IsInMask(lMask, VTRUploadTaskMask.VTR_Mask_TaskType))
                {
                    dbpTask.Tasktype = task.emTaskType;
                }

                if (IsInMask(lMask, VTRUploadTaskMask.VTR_Mask_CooperantType))
                {
                    dbpTask.Backtype = (int)CooperantType.emPureTask;
                }

                if (IsInMask(lMask, VTRUploadTaskMask.VTR_Mask_State))
                {
                    dbpTask.State = (int)taskState.tsReady;
                }

                if (IsInMask(lMask, VTRUploadTaskMask.VTR_Mask_StampImage)
                    || IsInMask(lMask, VTRUploadTaskMask.VTR_Mask_TaskDesc))
                {
                    dbpTask.Description = task.strStampImage;
                }
                #endregion
            }
            return dbpTask;
        }

        //----------------------------------------------------------------
        //!
        //! @brief 用于将普通任务修改为VTR任务
        //! 
        //! @param vtrTask 修改使用的VTR任务信息
        //! @param metadatas 该VTR任务的元数据
        //!
        //! @return true 成功执行
        //!
        //----------------------------------------------------------------
        public async ValueTask<bool> ModifyNormalTaskToVTRUploadTaskAsync(VTRUploadTaskContent vtrTask, List<VTR_UPLOAD_MetadataPair> metadatas, DbpTask dbpTask)
        {
            Logger.Info("ModifyNormalTaskToVTRUploadTask  In ModifyNormalTaskToVTRUploadTask");
            if (vtrTask == null)
            {
                throw new Exception("The Task params is null.");
            }
            #region VTR Check
            VTRTimePeriods freeVtrTimePeriods = new VTRTimePeriods();
            freeVtrTimePeriods.VTRId = vtrTask.nVtrId;
            DateTime dtNow = DateTime.Now;
            DateTime preSetBeginTime = dtNow;
            int preSetChannelId = vtrTask.nChannelId;

            if (!string.IsNullOrEmpty(vtrTask.strBegin))
            {
                preSetBeginTime = DateTimeFormat.DateTimeFromString(vtrTask.strBegin);

                if (preSetBeginTime == DateTime.MinValue
                    || vtrTask.emTaskState == VTRUPLOADTASKSTATE.VTR_UPLOAD_FAIL)
                {
                    preSetBeginTime = dtNow;
                }
                else
                {
                    //提交的时候，肯定是要在当前时间之后
                    if (preSetBeginTime < new DateTime(dtNow.Year, dtNow.Month, dtNow.Day, 0, 0, 0))
                    {
                        preSetBeginTime = dtNow;
                    }
                }
            }

            if (vtrTask.emTaskState == VTRUPLOADTASKSTATE.VTR_UPLOAD_FAIL)
            {
                preSetChannelId = -1;
            }

            TimeSpan tsDuration = new TimeSpan();
            if (vtrTask.nBlankTaskId > 0)//入点加长度
            {
                tsDuration = new TimeSpan(0, 0, vtrTask.nTrimOut / vtrTask.nBlankTaskId);
            }
            else
            {
                SB_TimeCode inSTC = new SB_TimeCode((uint)vtrTask.nTrimIn);
                SB_TimeCode outSTC = new SB_TimeCode((uint)vtrTask.nTrimOut);
                tsDuration = outSTC - inSTC;
            }

            DateTime preSetEndTime = preSetBeginTime + tsDuration;

            freeVtrTimePeriods = await GetFreeTimePeriodByVtrId(freeVtrTimePeriods, preSetBeginTime, vtrTask.nTaskId);

            //首先对vtr进行判断
            if (freeVtrTimePeriods == null
                || freeVtrTimePeriods.Periods == null
                || freeVtrTimePeriods.Periods.Count <= 0)
            {
                throw new Exception("VTR Collide");
            }

            if (preSetBeginTime != dtNow)//只有预设时间的，才需要进行vtr状态
            {
                TimePeriod tp = new TimePeriod(vtrTask.nVtrId, preSetBeginTime, preSetEndTime);
                if (!IsTimePeriodInVTRTimePeriods(tp, freeVtrTimePeriods))
                {
                    throw new Exception("VTR Collide");
                }
            }

            List<int> channelIds = new List<int>();
            List<ChannelTimePeriods> channelsTimePeriods = new List<ChannelTimePeriods>();
            if (preSetChannelId > 0)
            {
                channelIds.Add(vtrTask.nChannelId);
            }
            else
            {
                if (_deviceInterface != null)
                {
                    var response = await _deviceInterface.GetDeviceCallBack(new DeviceInternals()
                    {
                        funtype = IngestDBCore.DeviceInternals.FunctionType.ChannelInfoBySrc,
                        SrcId = vtrTask.nSignalId,
                        Status = 0
                    });

                    var channelInfos = response as ResponseMessage<List<CaptureChannelInfoInterface>>;

                    foreach (CaptureChannelInfoInterface info in channelInfos.Ext)
                    {
                        channelIds.Add(info.Id);
                    }
                }
            }

            channelsTimePeriods = await GetChannelsFreeTimePeriods(preSetBeginTime, channelIds, vtrTask.nTaskId);
            if (channelsTimePeriods == null
                || channelsTimePeriods.Count <= 0)
            {
                throw new Exception("No Channel");
            }

            int selectedChannel = -1;
            TimePeriod preSetChTP = new TimePeriod(vtrTask.nChannelId, preSetBeginTime, preSetEndTime);

            if (preSetBeginTime != dtNow)//预设时间
            {
                foreach (ChannelTimePeriods ctp in channelsTimePeriods)
                {
                    if (IsTimePeriodInChannelTimePeriods(preSetChTP, ctp))
                    {
                        selectedChannel = ctp.ChannelId;
                        break;
                    }
                }
            }
            else
            {
                List<TimePeriod> freeChannelsTimePeriods = new List<TimePeriod>();
                foreach (ChannelTimePeriods ctp in channelsTimePeriods)
                {
                    if (ctp.Periods != null && ctp.Periods.Count > 0)
                    {
                        ChannelTimePeriods newCTP = MergeVTRAndChannelFreeTimePeriods(freeVtrTimePeriods, ctp);
                        foreach (TimePeriod tempTP in newCTP.Periods)
                        {
                            TimePeriod newTP = new TimePeriod(ctp.ChannelId, tempTP.StartTime, tempTP.EndTime);
                            freeChannelsTimePeriods.Add(newTP);
                        }
                    }
                }

                freeChannelsTimePeriods.Sort(TimePeriod.CompareAscByStartTime);

                if (freeChannelsTimePeriods.Count > 0)
                {
                    foreach (TimePeriod tempTP in freeChannelsTimePeriods)
                    {
                        if (tempTP.Duration >= preSetChTP.Duration)
                        {
                            selectedChannel = tempTP.Id;
                            preSetBeginTime = tempTP.StartTime.AddSeconds(3);
                            preSetEndTime = preSetBeginTime + preSetChTP.Duration;
                            break;
                        }
                    }
                }
                else
                {
                    throw new Exception("No Channel");
                }
            }

            if (selectedChannel > 0)
            {
                vtrTask.nChannelId = selectedChannel;
                vtrTask.emTaskState = VTRUPLOADTASKSTATE.VTR_UPLOAD_COMMIT;
                vtrTask.strBegin = DateTimeFormat.DateTimeToString(preSetBeginTime);
                vtrTask.strEnd = DateTimeFormat.DateTimeToString(preSetEndTime);
            }
            else
            {
                throw new Exception("No Channel");
            }
            #endregion

            if (metadatas != null)
            {
                foreach (VTR_UPLOAD_MetadataPair metadata in metadatas)
                {
                    await SetVBUTasksMetadatasAsync(vtrTask.nTaskId, (MetaDataType)metadata.emType, metadata.strMetadata, false);
                }
            }

            dbpTask.Usercode = "VTRBATCHUPLOAD_ERROR_OK";
            dbpTask.Tasklock = string.Empty;
            dbpTask = VTRUploadTaskContent2Dbptask(true, vtrTask, dbpTask, -1);
            await TaskStore.UpdateTaskListAsync( new List<DbpTask> { dbpTask }, false);
            //添加VTR_UPLOADTASK
            Logger.Info("In ModifyNormalTaskToVTRUploadTask.Before Adding VTR_UPLOADTASK");
            //VtrUploadtask vtrUploadtask = Mapper.Map<VtrUploadtask>(vtrTask);
            VtrUploadtask vtrUploadtask = new VtrUploadtask();
            vtrUploadtask = VTRUploadTaskContent2VTRUPLOADTASK(vtrTask, vtrUploadtask, -1);
            await VtrStore.AddUploadtask(vtrUploadtask, false);

            //更新任务来源表
            //vtr uploadtask只能是emVTRUploadTask还有别的选择？
            //Logger.Info("In ModifyNormalTaskToVTRUploadTask.Before Updating DBP_TASK_SOURCE");
            //await TaskStore.UpdateTaskSource(new DbpTaskSource() { Taskid = vtrTask.nTaskId, Tasksource = (int)TaskSource.emVTRUploadTask }, true);

            Logger.Info("In ModifyNormalTaskToVTRUploadTask.Before Updating dataset");
            // 更新MetaData
            //if (metadatas != null)
            //{
            //    Logger.Info("In ModifyNormalTaskToVTRUploadTask.Before Updating metadatas");
            //    //foreach (VTR_UPLOAD_MetadataPair metadata in metadatas)
            //    //{ 
            //    //    await SetVBUTasksMetadatasAsync(vtrTask.nTaskId, (MetaDataType)metadata.emType, metadata.strMetadata);
            //    //}
            //    await UpdateTasksMetadatas(metadatas, vtrTask.nTaskId);
            //}
            Logger.Info("In ModifyNormalTaskToVTRUploadTask.Fin");
            return true;
        }

        public async Task UpdateTasksMetadatas(List<VTR_UPLOAD_MetadataPair> metadatas,int taskid)
        {
            List<SubmitMetadata> list = new List<SubmitMetadata>();
            Logger.Info("In ModifyNormalTaskToVTRUploadTask.Before Updating metadatas");
            foreach (VTR_UPLOAD_MetadataPair metadata in metadatas)
            {
                if (!string.IsNullOrEmpty(metadata.strMetadata))
                {
                    list.AddRange(GetVBUTasksMetadatas(taskid, (MetaDataType)metadata.emType, metadata.strMetadata));
                }
                //await SetVBUTasksMetadatasAsync(vtrTask.nTaskId, (MetaDataType)metadata.emType, metadata.strMetadata);
            }

            await TaskStore.UpdateTaskMetaDataListAsync(list);
        }

        public List<SubmitMetadata> GetVBUTasksMetadatas(int taskId, MetaDataType type, string metadata)
        {
            List<SubmitMetadata> list = new List<SubmitMetadata>();
            //需要将其中的三个字符串提取出来
            if (type == MetaDataType.emContentMetaData)
            {
                string materialMeta = string.Empty;
                string planningMeta = string.Empty;
                string originalMeta = string.Empty;
                XElement xElement = XElement.Parse(metadata);
                //xElement.LoadXml(metadata);
                //XmlNode taskContentNode = doc.SelectSingleNode("/TaskContentMetaData");
                //var taskContentNode = xElement.Descendants("TaskContentMetaData").FirstOrDefault();
                if (xElement != null)
                {
                    if (xElement.HasElements)
                    {
                        //XmlNode materialNode = doc.SelectSingleNode("/TaskContentMetaData/MetaMaterial");
                        var materialNode = xElement.Element("MetaMaterial");
                        if (materialNode != null)
                        {
                            materialMeta = materialNode.FirstNode?.ToString();
                            //taskContentNode.RemoveChild(materialNode);
                            materialNode.Remove();
                        }

                        //XmlNode planningNode = doc.SelectSingleNode("/TaskContentMetaData/MetaPlanning");
                        var planningNode = xElement.Element("MetaPlanning");
                        if (planningNode != null)
                        {
                            planningMeta = planningNode.FirstNode?.ToString();
                            //taskContentNode.RemoveChild(planningNode);
                            planningNode.Remove();
                        }

                        //XmlNode originalNode = doc.SelectSingleNode("/TaskContentMetaData/MetaOriginal");
                        var originalNode = xElement.Element("MetaOriginal");
                        if (originalNode != null)
                        {
                            originalMeta = originalNode.FirstNode?.ToString();
                            //taskContentNode.RemoveChild(originalNode);
                            originalNode.Remove();
                        }
                    }
                }

                if (!string.IsNullOrEmpty(materialMeta))
                {
                    list.Add(new SubmitMetadata() { taskId = taskId, type = MetaDataType.emStoreMetaData, metadata = materialMeta });
                }
                if (!string.IsNullOrEmpty(planningMeta))
                {
                    list.Add(new SubmitMetadata() { taskId = taskId, type = MetaDataType.emPlanMetaData, metadata = planningMeta });
                }
                if (!string.IsNullOrEmpty(originalMeta))
                {
                    list.Add(new SubmitMetadata() { taskId = taskId, type = MetaDataType.emOriginalMetaData, metadata = originalMeta });
                }
                list.Add(new SubmitMetadata() { taskId = taskId, type = MetaDataType.emContentMetaData, metadata = xElement.ToString() });
                //await TaskStore.UpdateTaskMetaDataAsync(taskId, MetaDataType.emStoreMetaData, materialMeta);
                //await TaskStore.UpdateTaskMetaDataAsync(taskId, MetaDataType.emPlanMetaData, planningMeta);
                //await TaskStore.UpdateTaskMetaDataAsync(taskId, MetaDataType.emOriginalMetaData, originalMeta);
                //await TaskStore.UpdateTaskMetaDataAsync(taskId, MetaDataType.emContentMetaData, xElement.Value);//doc.OuterXml);
            }
            else
            {
                //await TaskStore.UpdateTaskMetaDataAsync(taskId, type, metadata);
                list.Add(new SubmitMetadata() { taskId = taskId, type = type, metadata = metadata });
            }
            return list;
        }

        private ChannelTimePeriods MergeVTRAndChannelFreeTimePeriods(VTRTimePeriods vtrFreeTP, ChannelTimePeriods channelFreeTP)
        {
            ChannelTimePeriods newChannelFreeTP = new ChannelTimePeriods(channelFreeTP.ChannelId);
            newChannelFreeTP.Periods = new List<TimePeriod>();

            foreach (TimePeriod chTP in channelFreeTP.Periods)
            {
                if (chTP.EndTime < vtrFreeTP.Periods[0].StartTime)
                {
                    continue;
                }

                if (chTP.StartTime > vtrFreeTP.Periods[vtrFreeTP.Periods.Count - 1].EndTime)
                {
                    continue;
                }

                for (int i = 0; i < vtrFreeTP.Periods.Count; i++)
                {
                    //排除前面一些
                    if (vtrFreeTP.Periods[i].EndTime <= chTP.StartTime)
                    {
                        continue;
                    }

                    //该停止的
                    if (vtrFreeTP.Periods[i].StartTime >= chTP.EndTime)
                    {
                        break;
                    }

                    //剩下的，肯定是跟通道空闲时间有交集的
                    TimePeriod tp = TimePeriod.GetIntersect(chTP, vtrFreeTP.Periods[i]);
                    tp.Id = newChannelFreeTP.ChannelId;
                    newChannelFreeTP.Periods.Add(tp);
                }
            }

            return newChannelFreeTP;
        }

        public static bool IsInMask(long lMask, VTRUploadTaskMask vtrMask)
        {
            if ((lMask & ((long)1 << ((int)vtrMask - 1))) > 0)
            {
                return true;
            }

            return false;
        }


        public async Task<TResult> GetUploadTaskInfoByIDAsync<TResult>(int taskID)
        {
            //VTRUploadConditionRequest condition = new VTRUploadConditionRequest() {  TaskId = taskID };
            //var result = await VtrStore.GetUploadtaskInfo(condition, false);
            var result = await VtrStore.GetUploadtask(a => a.Where(x => x.Taskid == taskID), true);

            return Mapper.Map<TResult>(result.FirstOrDefault());
        }

        public async ValueTask<int> CommitVTRBatchUploadTasksAsync(List<int> taskIds, bool ignoreWrong)
        {
            //如果是暂存的任务，将它分配通道，
            //如果是执行失败的任务，可以重新分配通道进行（素材方面不好处理）
            //将任务查出，然后进行提交
            int vtrId = 0;
            VTR_BUT_ErrorCode errorCode = VTR_BUT_ErrorCode.emNormal;
            List<int> taskIdList = new List<int>();
            taskIdList.AddRange(taskIds);

            //List<VTRUploadTaskContent> vtrTaskList = TaskStore.GetVtrUploadTaskListAsync(taskIdList);
            var vtrTaskList = await TaskStore.GetVtrUploadTaskListAsync(a => a.Where(x => taskIds.Contains(x.Taskid)), true);
            int signalId = -1;

            try
            {
                if (vtrTaskList != null && vtrTaskList.Count > 0)
                {
                    List<RouterInInterface> routerIns = null;
                    if (_deviceInterface != null)
                    {
                        DeviceInternals re = new DeviceInternals() { funtype = IngestDBCore.DeviceInternals.FunctionType.AllRouterInPort};
                        var response1 = await _deviceInterface.GetDeviceCallBack(re);
                        if (response1.Code != ResponseCodeDefines.SuccessCode)
                        {
                            Logger.Error("SetGlobalState modtask error");
                        }

                        routerIns = (response1 as ResponseMessage<List<RouterInInterface>>).Ext;
                    }

                    for (int i = 0; i < vtrTaskList.Count; i++)
                    {
                        //zmj 2011-02-25
                        //已经提交的，准备执行和正常在执行的，已经完成的，都不可以再次提交
                        if (vtrTaskList[i].Taskstate == (int)VTRUPLOADTASKSTATE.VTR_UPLOAD_COMMIT ||
                            vtrTaskList[i].Taskstate == (int)VTRUPLOADTASKSTATE.VTR_UPLOAD_PRE_EXECUTE ||
                            vtrTaskList[i].Taskstate == (int)VTRUPLOADTASKSTATE.VTR_UPLOAD_EXECUTE ||
                            vtrTaskList[i].Taskstate == (int)VTRUPLOADTASKSTATE.VTR_UPLOAD_COMPLETE)
                        {
                            vtrTaskList.RemoveAt(i);
                            i--;
                            continue;
                        }

                        //判断信号源通道是否分配好了，如果没有分配的话，那么需要根据Vtr
                        if (vtrTaskList[i].Signalid < 0)
                        {
                            if (signalId < 0)
                            {
                                if (routerIns != null)
                                {
                                    foreach (RouterInInterface inport in routerIns)
                                    {
                                        if (inport.RcDeviceId == vtrTaskList[i].Vtrid)
                                        {
                                            signalId = inport.SignalSrcId;
                                            break;
                                        }
                                    }
                                }
                            }

                            vtrTaskList[i].Signalid = signalId;
                        }
                    }

                    if (vtrTaskList.Count > 0)
                    {
                        vtrId = vtrTaskList[0].Vtrid == null ? -1 : (int)vtrTaskList[0].Vtrid;
                        //AddCommitVTRBUTasks(vtrTaskList, ignoreWrong, null, false, out taskIdList);
                        var vtrtaskcontent = Mapper.Map<List<VTRUploadTaskContent>>(vtrTaskList);
                        await AddCommitVTRBUTasksExAsync(vtrtaskcontent, ignoreWrong, null, false, taskIdList);
                    }
                }
            }
            catch (System.Exception ex)
            {
                errorCode = GetErrorCode(ex.Message);
                string msg = ex.Message;
                if (errorCode == VTR_BUT_ErrorCode.emVTRCollide)
                {
                    if (vtrId > 0)
                    {
                        VtrDetailinfo vtrInfo = (await VtrStore.GetDetailinfo(a => a.Where(x => x.Vtrid == vtrId), true)).FirstOrDefault();
                        msg = string.Format("{0} has been used by other tasks", vtrInfo.Vtrname);
                    }
                }

                if (errorCode == VTR_BUT_ErrorCode.emNoChannel)
                {
                    msg = "No MSV channel can accept all the tasks, do you want to continue?";
                }

                if (errorCode == VTR_BUT_ErrorCode.emSomeSuccessful)
                {
                    msg = "The target MSV channel cannot accept all the tasks, do you want to continue?";
                }

                //重新抛个异常出去
                throw new Exception(msg);
            }

            return (int)errorCode;
        }

        private VTR_BUT_ErrorCode GetErrorCode(string msg)
        {
            if (string.Compare("VTR Collide", msg, true) == 0)
            {
                return VTR_BUT_ErrorCode.emVTRCollide;
            }

            if (string.Compare("No Channel", msg, true) == 0)
            {
                return VTR_BUT_ErrorCode.emNoChannel;
            }

            if (string.Compare("Some Successful", msg, true) == 0)
            {
                return VTR_BUT_ErrorCode.emSomeSuccessful;
            }

            return VTR_BUT_ErrorCode.emNormal;
        }
        
        private async Task<List<int>> AddCommitVTRBUTasksExAsync(List<VTRUploadTaskContent> commitTasks, bool ignoreWrong, List<VTR_UPLOAD_MetadataPair> metadatas, bool isAdd2DB, List<int> taskIds)
        {
            Logger.Info($"AddCommitVTRBUTask {JsonHelper.ToJson(commitTasks)}");
            if (commitTasks == null || commitTasks.Count <= 0)
            {
                throw new Exception("No any commit tasks");
            }

            VTRTimePeriods freeVtrTimePeriods = new VTRTimePeriods();
            freeVtrTimePeriods.VTRId = commitTasks[0].nVtrId;
            DateTime dtNow = DateTime.Now;
            DateTime preSetBeginTime = dtNow;

            if (commitTasks.Count == 1)//单个任务处理
            {
                VTRUploadTaskContent vtrTask = commitTasks[0];

                int preSetChannelId = vtrTask.nChannelId;

                if (!string.IsNullOrEmpty(vtrTask.strBegin))
                {
                    preSetBeginTime = DateTimeFormat.DateTimeFromString(vtrTask.strBegin);

                    if (preSetBeginTime == DateTime.MinValue
                        || vtrTask.emTaskState == VTRUPLOADTASKSTATE.VTR_UPLOAD_FAIL)
                    {
                        preSetBeginTime = dtNow;
                    }
                    else
                    {
                        //提交的时候，肯定是要在当前时间之后
                        if (preSetBeginTime < new DateTime(dtNow.Year, dtNow.Month, dtNow.Day, 0, 0, 0))
                        {
                            preSetBeginTime = dtNow;
                        }
                    }
                }

                if (vtrTask.emTaskState == VTRUPLOADTASKSTATE.VTR_UPLOAD_FAIL)
                {
                    preSetChannelId = -1;
                }

                TimeSpan tsDuration = new TimeSpan();
                if (vtrTask.nBlankTaskId > 0)//入点加长度
                {
                    tsDuration = new TimeSpan(0, 0, vtrTask.nTrimOut / vtrTask.nBlankTaskId);
                }
                else
                {
                    SB_TimeCode inSTC = new SB_TimeCode((uint)vtrTask.nTrimIn);
                    SB_TimeCode outSTC = new SB_TimeCode((uint)vtrTask.nTrimOut);
                    tsDuration = outSTC - inSTC;
                }

                DateTime preSetEndTime = preSetBeginTime + tsDuration;

                freeVtrTimePeriods = await GetFreeTimePeriodByVtrId(freeVtrTimePeriods, preSetBeginTime, vtrTask.nTaskId);
                //首先对vtr进行判断
                if (freeVtrTimePeriods == null
                    || freeVtrTimePeriods.Periods == null
                    || freeVtrTimePeriods.Periods.Count <= 0)
                {
                    throw new Exception("VTR Collide");
                }

                if (preSetBeginTime != dtNow)//只有预设时间的，才需要进行vtr状态
                {
                    TimePeriod tp = new TimePeriod(vtrTask.nVtrId, preSetBeginTime, preSetEndTime);
                    if (!IsTimePeriodInVTRTimePeriods(tp, freeVtrTimePeriods))
                    {
                        throw new Exception("VTR Collide");
                    }
                }

                List<int> channelIds = new List<int>();
                List<ChannelTimePeriods> channelsTimePeriods = new List<ChannelTimePeriods>();

                if (_deviceInterface != null)//对通道进行是否连接判断
                {
                    var response = await _deviceInterface.GetDeviceCallBack(new DeviceInternals()
                    {
                        funtype = IngestDBCore.DeviceInternals.FunctionType.ChannelInfoBySrc,
                        SrcId = vtrTask.nSignalId,
                        Status = 1
                    });

                    var channelInfos = response as ResponseMessage<List<CaptureChannelInfoInterface>>;

                    foreach (CaptureChannelInfoInterface info in channelInfos.Ext)
                    {
                        channelIds.Add(info.Id);
                    }
                }

                if (preSetChannelId > 0)
                {
                    //channelIds.Add(vtrTask.nChannelId);
                    channelIds.RemoveAll(x => x != preSetChannelId);
                }

                channelsTimePeriods = await GetChannelsFreeTimePeriods(preSetBeginTime, channelIds, vtrTask.nTaskId);

                if (channelsTimePeriods == null
                    || channelsTimePeriods.Count <= 0)
                {
                    throw new Exception("No Channel");
                }

                int selectedChannel = -1;
                TimePeriod preSetChTP = new TimePeriod(vtrTask.nChannelId, preSetBeginTime, preSetEndTime);

                if (preSetBeginTime != dtNow)//预设时间
                {
                    foreach (ChannelTimePeriods ctp in channelsTimePeriods)
                    {
                        if (IsTimePeriodInChannelTimePeriods(preSetChTP, ctp))
                        {
                            /*
                             * 优先使用原来通道
                             */
                            if (vtrTask.nChannelId > 0)
                            {
                                if (ctp.ChannelId == vtrTask.nChannelId)
                                {
                                    selectedChannel = ctp.ChannelId;
                                    break;
                                }
                                else
                                {
                                    //没有就优先第一个
                                    if (selectedChannel <= 0) selectedChannel = ctp.ChannelId;
                                    continue;
                                }
                            }
                            else
                            {
                                selectedChannel = ctp.ChannelId;
                                break;
                            }
                        }
                    }
                }
                else
                {
                    List<TimePeriod> freeChannelsTimePeriods = new List<TimePeriod>();
                    foreach (ChannelTimePeriods ctp in channelsTimePeriods)
                    {
                        if (ctp.Periods != null && ctp.Periods.Count > 0)
                        {
                            ChannelTimePeriods newCTP = MergeVTRAndChannelFreeTimePeriods(freeVtrTimePeriods, ctp);
                            foreach (TimePeriod tempTP in newCTP.Periods)
                            {
                                TimePeriod newTP = new TimePeriod(ctp.ChannelId, tempTP.StartTime, tempTP.EndTime);
                                freeChannelsTimePeriods.Add(newTP);
                            }
                        }
                    }

                    freeChannelsTimePeriods.Sort(TimePeriod.CompareAscByStartTime);

                    if (freeChannelsTimePeriods.Count > 0)
                    {
                        foreach (TimePeriod tempTP in freeChannelsTimePeriods)
                        {
                            if (tempTP.Duration >= preSetChTP.Duration)
                            {
                            //    selectedChannel = tempTP.Id;
                            //    preSetBeginTime = tempTP.StartTime.AddSeconds(3);
                            //    preSetEndTime = preSetBeginTime + preSetChTP.Duration;
                            //    break;

                            /*本通道优先*/

                                if (vtrTask.nChannelId > 0)
                                {
                                    if (tempTP.Id == vtrTask.nChannelId)
                                    {
                                        selectedChannel = tempTP.Id;
                                        preSetBeginTime = tempTP.StartTime.AddSeconds(3);
                                        preSetEndTime = preSetBeginTime + preSetChTP.Duration;
                                        break;
                                    }
                                    else
                                    {
                                        //没有就优先第一个
                                        if (selectedChannel <= 0)
                                        {
                                            selectedChannel = tempTP.Id;
                                            preSetBeginTime = tempTP.StartTime.AddSeconds(3);
                                            preSetEndTime = preSetBeginTime + preSetChTP.Duration;
                                        }
                                        continue;
                                    }
                                }
                                else
                                {
                                    selectedChannel = tempTP.Id;
                                    preSetBeginTime = tempTP.StartTime.AddSeconds(3);
                                    preSetEndTime = preSetBeginTime + preSetChTP.Duration;
                                    break;
                                }
                            }

                        }
                    }
                    else
                    {
                        throw new Exception("No Channel");
                    }
                }

                Logger.Info("AddCommitVTRBUTask select one " + selectedChannel);
                if (selectedChannel > 0)
                {
                    commitTasks[0].nChannelId = selectedChannel;
                    commitTasks[0].emTaskState = VTRUPLOADTASKSTATE.VTR_UPLOAD_COMMIT;
                    commitTasks[0].strBegin = DateTimeFormat.DateTimeToString(preSetBeginTime);
                    commitTasks[0].strEnd = DateTimeFormat.DateTimeToString(preSetEndTime);
                    Logger.Info("In RecBusinessRule.VTROper.AddCommitVTRBUTasksEx. strBegin:" + commitTasks[0].strBegin + " strEnd:" + commitTasks[0].strEnd);
                }
                else
                {
                    throw new Exception("No Channel");
                }
            }
            else//多个任务处理，通道和开始时间都重新排列
            {
                int vtrId = commitTasks[0].nVtrId;
                if (!string.IsNullOrEmpty(commitTasks[0].strBegin))
                {
                    preSetBeginTime = DateTimeFormat.DateTimeFromString(commitTasks[0].strBegin);

                    if (preSetBeginTime == DateTime.MinValue
                        || commitTasks[0].emTaskState == VTRUPLOADTASKSTATE.VTR_UPLOAD_FAIL)
                    {
                        preSetBeginTime = dtNow;
                    }
                    else
                    {
                        //提交的时候，肯定是要在当前时间之后
                        if (preSetBeginTime < new DateTime(dtNow.Year, dtNow.Month, dtNow.Day, 0, 0, 0))
                        {
                            preSetBeginTime = dtNow;
                        }
                    }
                }
                
                freeVtrTimePeriods = await GetFreeTimePeriodByVtrId(freeVtrTimePeriods, preSetBeginTime, -1);//这多个通道要提交的，要不就是还没提交过的，要不就是提交过的，但是是失败的任务
                if (freeVtrTimePeriods == null
                    || freeVtrTimePeriods.Periods == null
                    || freeVtrTimePeriods.Periods.Count <= 0)
                {
                    throw new Exception("No Channel");
                }

                List<int> channelIds = new List<int>();
                List<ChannelTimePeriods> channelsTimePeriods = new List<ChannelTimePeriods>();
                
                if (_deviceInterface != null)
                {
                    var response = await _deviceInterface.GetDeviceCallBack(new DeviceInternals()
                    {
                        funtype = IngestDBCore.DeviceInternals.FunctionType.AllCaptureChannels
                    });

                    var channelInfos = response as ResponseMessage<List<CaptureChannelInfoInterface>>;

                    foreach (CaptureChannelInfoInterface info in channelInfos.Ext)
                    {
                        channelIds.Add(info.Id);
                    }
                }

                channelsTimePeriods = await GetChannelsFreeTimePeriods(preSetBeginTime, channelIds, commitTasks[0].nTaskId);

                if (channelsTimePeriods == null
                    || channelsTimePeriods.Count <= 0)
                {
                    throw new Exception("No Channel");
                }
                //----------------The End 2014 - 02 - 27----------------


                for (int i = 0; i < commitTasks.Count; i++)
                {
                    TimeSpan duration = new TimeSpan();
                    if (commitTasks[i].nBlankTaskId > 0)//入点加长度
                    {
                        duration = new TimeSpan(0, 0, commitTasks[i].nTrimOut / commitTasks[i].nBlankTaskId);
                    }
                    else
                    {
                        SB_TimeCode inSTC = new SB_TimeCode((uint)commitTasks[i].nTrimIn);
                        SB_TimeCode outSTC = new SB_TimeCode((uint)commitTasks[i].nTrimOut);
                        duration = outSTC - inSTC;
                    }

                    DateTime dtBegin = DateTime.MinValue;
                    if (commitTasks[i].emTaskState != VTRUPLOADTASKSTATE.VTR_UPLOAD_FAIL)
                    {
                        if (!string.IsNullOrEmpty(commitTasks[i].strBegin))
                        {
                            dtBegin = DateTimeFormat.DateTimeFromString(commitTasks[i].strBegin);
                        }
                    }


                    // ---------------- The End 2014-02-27 ----------------

                    DateTime dtEnd = new DateTime();
                    bool isSuccessfulCommit = false;

                    //根据提交的通道ID来判断是否该任务已经确实了通道
                    if (commitTasks[i].nChannelId > 0)
                    {
                        foreach (ChannelTimePeriods ctp in channelsTimePeriods)
                        {
                            if (ctp.ChannelId == commitTasks[i].nChannelId)
                            {
                                ChannelTimePeriods newCTP = MergeVTRAndChannelFreeTimePeriods(freeVtrTimePeriods, ctp);

                                //判断是否已经确认了时间

                                //if (dtBegin.Year != DateTime.MinValue)
                                if (dtBegin.Year != 1990)
                                {
                                    dtEnd = dtBegin + duration;
                                    //判断该时间段是否是可用的
                                    TimePeriod tp = new TimePeriod(ctp.ChannelId, dtBegin, dtEnd);
                                    if (IsTimePeriodInChannelTimePeriods(tp, ctp))
                                    {
                                        if (IsTimePeriodInVTRTimePeriods(tp, freeVtrTimePeriods))
                                        {
                                            Logger.Info("AddCommitVTRBUTask select two " + ctp.ChannelId);
                                            //成功
                                            commitTasks[i].nChannelId = ctp.ChannelId;
                                            commitTasks[i].emTaskState = VTRUPLOADTASKSTATE.VTR_UPLOAD_COMMIT;
                                            commitTasks[i].strBegin = DateTimeFormat.DateTimeToString(dtBegin);
                                            commitTasks[i].strEnd = DateTimeFormat.DateTimeToString(dtEnd);
                                            isSuccessfulCommit = true;
                                        }
                                        else
                                        {
                                            //如果失败，抛有些成功
                                            if (!ignoreWrong)
                                            {
                                                throw new Exception("Some Successful");
                                            }
                                        }
                                    }
                                    else
                                    {
                                        //如果失败，抛有些成功
                                        if (!ignoreWrong)
                                        {
                                            throw new Exception("Some Successful");
                                        }
                                    }
                                }
                                else
                                {
                                    //没有确定时间段的，只有自己找一个时间段进行判断
                                    for (int j = 0; j < ctp.Periods.Count; j++)
                                    {
                                        if (duration <= ctp.Periods[j].Duration)
                                        {
                                            dtBegin = ctp.Periods[j].StartTime.AddSeconds(10);
                                            dtEnd = dtBegin + duration;

                                            Logger.Info("AddCommitVTRBUTask select three " + ctp.ChannelId);

                                            commitTasks[i].nChannelId = ctp.ChannelId;
                                            commitTasks[i].emTaskState = VTRUPLOADTASKSTATE.VTR_UPLOAD_COMMIT;
                                            commitTasks[i].strBegin = DateTimeFormat.DateTimeToString(dtBegin);
                                            commitTasks[i].strEnd = DateTimeFormat.DateTimeToString(dtEnd);
                                            isSuccessfulCommit = true;
                                            break;
                                        }
                                    }
                                }

                                break;
                            }
                        }
                    }
                    else
                    {
                        ChannelTimePeriods freeChTimePeriods = new ChannelTimePeriods();
                        freeChTimePeriods.ChannelId = -1;

                        foreach (ChannelTimePeriods ctp in channelsTimePeriods)
                        {
                            if (ctp.Periods != null && ctp.Periods.Count > 0)
                            {
                                if (freeChTimePeriods.ChannelId <= 0)
                                {
                                    freeChTimePeriods = ctp;
                                }
                                else
                                {
                                    if (freeChTimePeriods.Periods == null || freeChTimePeriods.Periods.Count <= 0)
                                    {
                                        freeChTimePeriods = ctp;
                                    }
                                    else
                                    {
                                        if (ctp.Periods != null && ctp.Periods.Count > 0)
                                        {
                                            if (freeChTimePeriods.Periods[0].StartTime > ctp.Periods[0].StartTime)
                                            {
                                                freeChTimePeriods = ctp;
                                            }
                                            else if (freeChTimePeriods.Periods[0].StartTime == ctp.Periods[0].StartTime)
                                            {
                                                if (ctp.Periods[0].Duration > freeChTimePeriods.Periods[0].Duration)
                                                {
                                                    freeChTimePeriods = ctp;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        if (freeChTimePeriods.ChannelId <= 0 || freeChTimePeriods.Periods == null || freeChTimePeriods.Periods.Count <= 0)
                        {
                            throw new Exception("Some Successful");
                        }

                        //对vtr和通道的空闲通道进行合并
                        ChannelTimePeriods newChannelFreeTP = MergeVTRAndChannelFreeTimePeriods(freeVtrTimePeriods, freeChTimePeriods);

                        if (newChannelFreeTP.Periods == null || newChannelFreeTP.Periods.Count <= 0)
                        {
                            throw new Exception("Some Successful");
                        }

                        if (!ignoreWrong)
                        {
                            TimeSpan allFreeTimeSpan = new TimeSpan();
                            foreach (TimePeriod tempTP in newChannelFreeTP.Periods)
                            {
                                allFreeTimeSpan += tempTP.Duration;
                            }

                            if (allFreeTimeSpan < duration)
                            {
                                throw new Exception("Some Successful");
                            }
                        }

                        for (int j = 0; j < newChannelFreeTP.Periods.Count; j++)
                        {
                            if (duration <= newChannelFreeTP.Periods[j].Duration)
                            {
                                dtBegin = newChannelFreeTP.Periods[j].StartTime.AddSeconds(3);
                                dtEnd = dtBegin + duration;

                                Logger.Info("AddCommitVTRBUTask select four " + newChannelFreeTP.ChannelId);

                                commitTasks[i].nChannelId = newChannelFreeTP.ChannelId;
                                commitTasks[i].emTaskState = VTRUPLOADTASKSTATE.VTR_UPLOAD_COMMIT;
                                commitTasks[i].strBegin = DateTimeFormat.DateTimeToString(dtBegin);
                                commitTasks[i].strEnd = DateTimeFormat.DateTimeToString(dtEnd);
                                isSuccessfulCommit = true;
                                break;
                            }
                        }
                    }

                    //成功的话，需要把该时间段从vtr和通道中取消掉
                    if (isSuccessfulCommit)
                    {
                        for (int j = 0; j < freeVtrTimePeriods.Periods.Count; j++)
                        {
                            if (dtBegin >= freeVtrTimePeriods.Periods[j].StartTime
                                && dtEnd <= freeVtrTimePeriods.Periods[j].EndTime)
                            {
                                freeVtrTimePeriods.Periods[j].StartTime = dtEnd;
                                break;
                            }
                        }

                        for (int j = 0; j < channelsTimePeriods.Count; j++)
                        {
                            for (int k = 0; k < channelsTimePeriods[j].Periods.Count; k++)
                            {
                                if (dtBegin >= channelsTimePeriods[j].Periods[k].StartTime
                                    && dtEnd <= channelsTimePeriods[j].Periods[k].EndTime)
                                {
                                    channelsTimePeriods[j].Periods[k].StartTime = dtEnd;
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (commitTasks[i].emTaskState != VTRUPLOADTASKSTATE.VTR_UPLOAD_FAIL)
                        {
                            commitTasks[i].emTaskState = VTRUPLOADTASKSTATE.VTR_UPLOAD_TEMPSAVE;
                            commitTasks[i].strBegin = DateTimeFormat.DateTimeToString(DateTime.MinValue);
                            commitTasks[i].strEnd = DateTimeFormat.DateTimeToString(DateTime.MinValue);
                        }
                    }
                }

            }

            //开始往表里加任务
            taskIds = await SetVBUT2DataSetAsync(commitTasks, metadatas, isAdd2DB, taskIds);

            return taskIds;
        }

        private async Task<List<int>> SetVBUT2DataSetAsync(List<VTRUploadTaskContent> vbuTasks, List<VTR_UPLOAD_MetadataPair> metadatas, bool isAdd2DB, List<int> taskIds)
        {
            List<DbpTask> submitTasks = new List<DbpTask>();
            List<DbpPolicytask> submitPolicy = new List<DbpPolicytask>();
            List<VtrUploadtask> vtrUploadtasks = new List<VtrUploadtask>();

            foreach (VTRUploadTaskContent task in vbuTasks)
            {
                int tempId = task.nTaskId;
                if (tempId < 0)
                {
                    task.nTaskId = TaskStore.GetNextValId("DBP_SQ_TASKID") ;//IngestTaskDBContext.next_val("DBP_SQ_TASKID");
                    taskIds.Add(task.nTaskId);
                }

                if (string.IsNullOrEmpty(task.strClassify))//vtr任务一般都a
                {
                    task.strClassify = "A";
                }

                if (string.IsNullOrEmpty(task.strTaskGUID))
                {
                    task.strTaskGUID = Guid.NewGuid().ToString("N");
                }
                //task.emTaskState = VTRUPLOADTASKSTATE.VTR_UPLOAD_COMMIT;

                if (metadatas != null)
                {
                    foreach (VTR_UPLOAD_MetadataPair metadata in metadatas)
                    {
                        if (metadata.nTaskID == tempId)
                        {
                            await SetVBUTasksMetadatasAsync(task.nTaskId, (MetaDataType)metadata.emType, metadata.strMetadata, false);
                        }
                    }
                }
                
                if (isAdd2DB)
                {
                    task.strUserToken = "VTRBATCHUPLOAD_ERROR_OK";
                    var dbpTask = new DbpTask();
                    dbpTask = VTRUploadTaskContent2Dbptask(true, task, dbpTask, -1);

                    submitTasks.Add(dbpTask);
                    vtrUploadtasks.Add(Mapper.Map<VtrUploadtask>(task));

                    /*policy现在就一个，写死*/
                    submitPolicy.Add(new DbpPolicytask() { Policyid= 1, Taskid= task.nTaskId });
                    //List<DbpMetadatapolicy> dbpMetadatapolicies = await VtrStore.GetMetadatapoliciesByUserCode(task.strUserCode);
                    
                    //foreach (DbpMetadatapolicy policy in dbpMetadatapolicies)
                    //{
                    //    submitPolicy.Add(new DbpPolicytask() { Policyid = policy.Policyid, Taskid = task.nTaskId });
                    //}
                }
                else
                {
                    DbpTask dbpTask = await TaskStore.GetTaskNotrackAsync(a => a.Where(x => x.Taskid == task.nTaskId), true);
                    if (dbpTask != null)
                    {
                        dbpTask = VTRUploadTaskContent2Dbptask(true, task, dbpTask, -1);
                        submitTasks.Add(dbpTask);
                    }

                    VtrUploadtask uploadtask = await TaskStore.GetVtrUploadTaskAsync(a => a.Where(x => x.Taskid == task.nTaskId));
                    if (uploadtask != null)
                    {
                        uploadtask = VTRUploadTaskContent2VTRUPLOADTASK(task, uploadtask, - 1);
                        vtrUploadtasks.Add(uploadtask);
                    }
                }
                
            }

            if (isAdd2DB)
            {
                await TaskStore.AddTaskList(submitTasks, false);
                await VtrStore.AddUploadListtask(vtrUploadtasks, false);
                await TaskStore.AddPolicyTask(submitPolicy, true);
            }
            else
            {
                await TaskStore.UpdateTaskListAsync(submitTasks, false);
                await VtrStore.UpdateVtrUploadTaskListAsync(vtrUploadtasks, true);
                
            }

            return taskIds;
        }

        public async Task<TResult> AddVTRBatchUploadTasksAsync<TResult,TPOne, TPTwo>(List<TPOne> vtrTasksp, List<TPTwo> metadatasp, bool ignoreWrong)
        {
            var vtrTasks = Mapper.Map<List<VTRUploadTaskContent>>(vtrTasksp);
            var metada = Mapper.Map<List<VTR_UPLOAD_MetadataPair>>(metadatasp);
            //判断是否为缓存状态，如果是，只存在VTR_UPLOAD表中

            //如果是提交状态，那么再次判断是否自动选择通道
            //1.判断VTR这个设备在当前时间是不是有冲突。
            //2.如果是自动选择，选择一下通道，建起任务
            //3.若已经分配好通道的，只需要判断是否时间上有冲突就行了
            VtrBatchUploadTaskResponse response = new VtrBatchUploadTaskResponse();
            response.taskIds = new List<int>();
            response.errorCode = VTR_BUT_ErrorCode.emNormal;
            
            if (vtrTasks == null || vtrTasks.Count <= 0)
            {
                throw new Exception("The Tasks params is smaller than 0.");
            }

            List<VTRUploadTaskContent> tempSaveTasks = new List<VTRUploadTaskContent>();
            List<VTRUploadTaskContent> commitTasks = new List<VTRUploadTaskContent>();
             
            int vtrId = 0;
            foreach (VTRUploadTaskContent task in vtrTasks)
            {
                if (string.IsNullOrEmpty(task.strBegin) || task.strBegin == "0000-00-00 00:00:00")
                {
                    throw new Exception("request begin of param error");
                }
                if (string.IsNullOrEmpty(task.strEnd) || task.strEnd == "0000-00-00 00:00:00")
                {
                    throw new Exception("request end of param error");
                }

                vtrId = task.nVtrId;

                if (task.emTaskState == VTRUPLOADTASKSTATE.VTR_UPLOAD_TEMPSAVE)
                {
                    tempSaveTasks.Add(task);
                }

                if (task.emTaskState == VTRUPLOADTASKSTATE.VTR_UPLOAD_COMMIT)
                {
                    commitTasks.Add(task);
                }
            }

            List<int> taskIds = new List<int>();
            try
            {
                if (tempSaveTasks.Count > 0)
                {
                    taskIds = await AddTempSaveVTRBUTasksAsync(tempSaveTasks, metada, taskIds);
                }

                if (commitTasks.Count > 0)
                {
                    taskIds = await AddCommitVTRBUTasksExAsync(commitTasks, ignoreWrong, metada, true, taskIds);

                }
                if (taskIds.Count > 0)
                {
                    for (int i = 0; i < taskIds.Count; i++)
                    {
                        response.taskIds.Add(taskIds[i]);
                    }
                }

            }
            catch (System.Exception ex)
            {
                response.errorCode = GetErrorCode(ex.Message);

                string msg = ex.Message;
                if (response.errorCode == VTR_BUT_ErrorCode.emVTRCollide)
                {
                    if (vtrId > 0)
                    {
                        VtrDetailinfo vtrInfo = (await VtrStore.GetDetailinfo(a => a.Where(x => x.Vtrid == vtrId), true)).FirstOrDefault();
                        msg = string.Format("{0} has been used by other tasks", vtrInfo.Vtrname);
                    }
                }

                if (response.errorCode == VTR_BUT_ErrorCode.emNoChannel)
                {
                    msg = "No MSV channel can accept all the tasks, do you want to continue?";
                }

                if (response.errorCode == VTR_BUT_ErrorCode.emSomeSuccessful)
                {
                    msg = "The target MSV channel cannot accept all the tasks, do you want to continue?";
                }

                //重新抛个异常出去
                throw new Exception(msg);
            }

            return Mapper.Map< TResult >(response);
        }

        private async Task<List<int>> AddTempSaveVTRBUTasksAsync(List<VTRUploadTaskContent> tempSaveTasks, List<VTR_UPLOAD_MetadataPair> metadatas, List<int> taskIds)
        {
            if (tempSaveTasks.Count <= 0)
            {
                return new List<int>();
            }

            //占存的任务，只在DBP_TASK表中，占一席之地，以方便查找，开始时间和结束时间，改为最小值，在提交的时候，进行修改
            //不去改变它的时间，等提交时，再去改变时间

            //timestamp范围为1970年，以后修改2037那个bug之后，这里修改回来   edit by:xietao
            //这个开始时间没用，只是占位，随意设置
            foreach (VTRUploadTaskContent task in tempSaveTasks)
            {
                DateTime dtCur = System.DateTime.Now;
                DateTime dtNow = new DateTime(1990, 1, 1, dtCur.Hour, dtCur.Minute, dtCur.Second);


                task.strBegin = dtNow.ToString("yyyy-MM-dd HH:mm:ss");
                //task.strEnd = GlobalFun.DateTimeToString(DateTime.MinValue);
                task.strEnd = dtNow.ToString("yyyy-MM-dd HH:mm:ss");
            }

            //开始往表里加任务
            taskIds = await SetVBUT2DataSetAsync(tempSaveTasks, metadatas, true, taskIds);
            
            return taskIds;
        }
        

        public async Task DeleteVtrUploadTaskAsync(int taskId)
        {
            if (taskId <= 0)
            {
                throw new Exception("TaskId is smaller than 0.");
            }
            
            var vtrUploadtask = (await VtrStore.GetUploadtask(a => a.Where(x => x.Taskid == taskId))).FirstOrDefault();
            if (vtrUploadtask != null)
            {
                //分状态进行删除
                //1.如果该任务是暂存，失败或者是删除状态，那么将它从表中删除
                //2.如果是提交或执行的话，将它设置为删除状态，并且告诉vtr上载服务，相应把素材删除
                //3.完成状态，不允许被删除
                if (vtrUploadtask.Taskstate == (int)VTRUPLOADTASKSTATE.VTR_UPLOAD_COMPLETE)
                {
                    Logger.Info("In DeleteVtrUploadTask,Find a VTR_UPLOAD_COMPLETE task,not delete.taskId = " + taskId.ToString());
                    return;
                }

                var task = await TaskStore.GetTaskNotrackAsync(a => a.Where(x => x.Taskid == taskId), true);

                if (task != null)
                {
                    if (vtrUploadtask.Taskstate == (int)VTRUPLOADTASKSTATE.VTR_UPLOAD_TEMPSAVE ||
                        vtrUploadtask.Taskstate == (int)VTRUPLOADTASKSTATE.VTR_UPLOAD_FAIL ||
                        vtrUploadtask.Taskstate == (int)VTRUPLOADTASKSTATE.VTR_UPLOAD_DELETE ||
                        vtrUploadtask.Taskstate == (int)VTRUPLOADTASKSTATE.VTR_UPLOAD_COMMIT)
                    {
                        await VtrStore.DeleteVtrUploadTask(vtrUploadtask, false);
                        await TaskStore.DeleteTaskDB(taskId, true);

                        Logger.Info(string.Format("In DeleteVtrUploadTask,Find a {0} task,delete from table.taskId  = {1}", vtrUploadtask.Taskstate, taskId));
                        return;
                    }

                    if (vtrUploadtask.Taskstate == (int)VTRUPLOADTASKSTATE.VTR_UPLOAD_PRE_EXECUTE)
                    {
                        Logger.Info(string.Format("In DeleteVtrUploadTask,Find a {0} task,set delete state.taskId  = {1}"
                            , (VTRUPLOADTASKSTATE)vtrUploadtask.Taskstate, taskId));

                        vtrUploadtask.Taskstate = (int)VTRUPLOADTASKSTATE.VTR_UPLOAD_DELETE;
                        await VtrStore.UpdateUploadtask(vtrUploadtask, false);
                        
                        task.State = (int)taskState.tsDelete;
                        task.OpType = (int)opType.otDel;
                        task.DispatchState = (int)dispatchState.dpsDispatchFailed;
                        await TaskStore.UpdateTaskListAsync(new List<DbpTask>() { task }, true);
                        return;
                    }

                    if (vtrUploadtask.Taskstate == (int)VTRUPLOADTASKSTATE.VTR_UPLOAD_EXECUTE)
                    {
                        Logger.Info(string.Format("In DeleteVtrUploadTask,Find a {0} task,set delete state.taskId  = {1}"
                            , (VTRUPLOADTASKSTATE)vtrUploadtask.Taskstate, taskId));

                        vtrUploadtask.Taskstate = (int)VTRUPLOADTASKSTATE.VTR_UPLOAD_DELETE;
                        await VtrStore.UpdateUploadtask(vtrUploadtask, false);

                        //vtr上载服务，相应把素材删除
                        task.SyncState = (int)syncState.ssNot;
                        task.OpType = (int)opType.otDel;
                        task.DispatchState = (int)dispatchState.dpsInvalid;
                        task.State = (int)taskState.tsInvaild;
                        task.Endtime = DateTime.Now;
                        task.NewEndtime = DateTime.Now;
                        await TaskStore.UpdateTaskListAsync(new List<DbpTask>() { task }, true);

                        return;
                    }
                }
            }
        }

        #endregion

        //AddVTRUploadTask_out VTRUploadTaskContent VTR_UPLOAD_MetadataPair
        public async Task<TResult> AddVTRUploadTask<TResult, TPOne, TPTwo>(TPOne vtrTaskp, List<TPTwo> metadatasp)
        {
            VTRUploadTaskContent vtrTask = Mapper.Map<VTRUploadTaskContent>(vtrTaskp);

            List<VTR_UPLOAD_MetadataPair> metadatas = Mapper.Map<List<VTR_UPLOAD_MetadataPair>>(metadatasp);

            var errCode = VTR_BUT_ErrorCode.emNormal;

            if (vtrTask == null)
            {
                throw new Exception("The Task params is null.");
            }
            
            List<VTRUploadTaskContent> taskList = new List<VTRUploadTaskContent> { vtrTask };
            try
            {
                if (vtrTask.emTaskState == VTRUPLOADTASKSTATE.VTR_UPLOAD_TEMPSAVE)
                {
                    await AddTempSaveVTRBUTasks(taskList, metadatas);
                }

                if (vtrTask.emTaskState == VTRUPLOADTASKSTATE.VTR_UPLOAD_COMMIT)
                {
                    await AddCommitVTRBUTasksExAsync(taskList, false, metadatas, true, new List<int>());
                }
            }
            catch (System.Exception ex)
            {
                errCode = GetErrorCode(ex.Message);
                if (errCode == VTR_BUT_ErrorCode.emVTRCollide)
                {
                    if (vtrTask.nTaskId > 0)
                    {
                        VTRDetailInfo vtrInfo = await GetVTRDetailInfoByIDAsync<VTRDetailInfo>(vtrTask.nTaskId);
                        throw new Exception($"{vtrInfo.szVTRDetailName} has been used by other tasks");
                    }
                }

                if (errCode == VTR_BUT_ErrorCode.emNoChannel)
                {
                    //throw new Exception("No MSV channel can accept all the tasks, do you want to continue?");
                    if (vtrTask.nChannelId > 0)
                    {
                        SobeyRecException.ThrowSelfNoParam("", GlobalDictionary.GLOBALDICT_CODE_SELECTED_CHANNEL_IS_BUSY_OR_CAN_NOT_BE_SUITED_TO_PROGRAMME, Logger, null);
                    }
                    else
                    {
                        SobeyRecException.ThrowSelfNoParam("", GlobalDictionary.GLOBALDICT_CODE_ALL_USEABLE_CHANNELS_ARE_BUSY, Logger, null);
                    }
                }
                if (errCode == VTR_BUT_ErrorCode.emSomeSuccessful)
                {
                    throw new Exception("The target MSV channel cannot accept all the tasks, do you want to continue?");
                }
            }
            
            AddVTRUploadTask_out task = new AddVTRUploadTask_out { vtrTask = vtrTask, errorCode = (int)errCode };

            return Mapper.Map<TResult>(task);
        }

        public async ValueTask<string> GetVtrTaskMetaDataAsync(int taskId, int type)
        {
            var metadata = await TaskStore.GetTaskMetaDataAsync(a => a.Where(x => x.Taskid == taskId && x.Metadatatype == type), true);
            var result = string.IsNullOrWhiteSpace(metadata?.Metadata) ? metadata?.Metadatalong : metadata?.Metadata;
            if (string.IsNullOrEmpty(result))
            {
                var backup = await VtrStore.GetTaskMetadataBackup(a => a.FirstOrDefaultAsync(x => x.Taskid == taskId && x.Metadatatype == type), true);
                result = string.IsNullOrWhiteSpace(backup?.Metadata) ? backup?.Metadatalong : backup?.Metadata;
            }
            return result;
        }

        private async ValueTask<bool> AddTempSaveVTRBUTasks(List<VTRUploadTaskContent> tempSaveTasks, List<VTR_UPLOAD_MetadataPair> metadatas)
        {
            if (tempSaveTasks.Count <= 0)
            {
                return true;
            }

            //占存的任务，只在DBP_TASK表中，占一席之地，以方便查找，开始时间和结束时间，改为最小值，在提交的时候，进行修改
            //不去改变它的时间，等提交时，再去改变时间

            //timestamp范围为1970年，以后修改2037那个bug之后，这里修改回来   edit by:xietao
            //这个开始时间没用，只是占位，随意设置
            foreach (VTRUploadTaskContent task in tempSaveTasks)
            {
                DateTime dtCur = System.DateTime.Now;
                DateTime dtNow = new DateTime(1990, 1, 1, dtCur.Hour, dtCur.Minute, dtCur.Second);


                task.strBegin = dtNow.ToString("yyyy-MM-dd HH:mm:ss");
                //task.strEnd = GlobalFun.DateTimeToString(DateTime.MinValue);
                task.strEnd = dtNow.ToString("yyyy-MM-dd HH:mm:ss");
            }

            //开始往表里加任务
            await SetVBUT2DataSetAsync(tempSaveTasks, metadatas, true, new List<int>());
            return true;
        }
        

    }
}
