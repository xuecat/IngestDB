namespace IngestTaskPlugin.Managers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Xml;
    using AutoMapper;
    using IngestDBCore;
    using IngestDBCore.Tool;
    using IngestTaskPlugin.Dto;
    using IngestTaskPlugin.Dto.Request;
    using IngestTaskPlugin.Dto.Response;
    using IngestTaskPlugin.Dto.Response.OldVtr;
    using IngestTaskPlugin.Extend;
    using IngestTaskPlugin.Models;
    using IngestTaskPlugin.Stores;
    using IngestTaskPlugin.Stores.VTR;
    using Microsoft.EntityFrameworkCore;
    using Sobey.Core.Log;

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

        /// <summary>
        /// Initializes a new instance of the <see cref="VtrManager"/> class.
        /// </summary>
        /// <param name="mapper">The mapper<see cref="IMapper"/>.</param>
        /// <param name="taskStore">The taskStore<see cref="ITaskStore"/>.</param>
        /// <param name="vtrStore">The vtrStore<see cref="IVtrStore"/>.</param>
        /// <param name="taskManager">The taskManager<see cref="TaskManager"/>.</param>
        public VtrManager(IMapper mapper, ITaskStore taskStore, IVtrStore vtrStore, TaskManager taskManager)
        {
            this.TaskStore = taskStore;
            this.VtrStore = vtrStore;
            this.Mapper = mapper;
            this.TaskManager = taskManager;
        }

        /// <summary>
        /// The 修改磁带信息,ID不存在即为添加.
        /// </summary>
        /// <param name="tapeId">The 磁带Id<see cref="int"/>.</param>
        /// <param name="tapeName">The 磁带名称<see cref="string"/>.</param>
        /// <param name="tapeDesc">The 磁带描述<see cref="string"/>.</param>
        /// <returns>The 磁带Id <see cref="Task{int}"/>.</returns>
        public async Task<int> SetTapeInfoAsync(int tapeId, string tapeName, string tapeDesc)
        {
            var newTapeId = await VtrStore.SaveTaplist(new VtrTapelist
                { Tapeid = tapeId, Tapename = tapeName, Tapedesc = tapeDesc });
            return newTapeId;
        }

        /// <summary>
        /// The 设置VTR与磁带的对应序列.
        /// </summary>
        /// <param name="vtrId">The VtrId <see cref="int"/>.</param>
        /// <param name="tapeId">The 磁带Id <see cref="int"/>.</param>
        /// <returns>The 是否成功<see cref="Task"/>.</returns>
        public async Task<bool> SetVtrTapeMapAsync(int vtrId, int tapeId)
        { return await VtrStore.SaveTapeVtrMap(new VtrTapeVtrMap { Vtrid = vtrId, Tapeid = tapeId }); }

        /// <summary>
        /// The 获得所有的磁带信息.
        /// </summary>
        /// <returns>The 所有的磁带信息<see cref="Task{List{VTRTapeInfo}}"/>.</returns>
        public async Task<List<VTRTapeInfo>> GetAllTapeInfoAsync()
        {
            return Mapper.Map<List<VTRTapeInfo>>(await VtrStore.GetTapelist(a => a.OrderByDescending(x => x.Tapeid)
                .Take(50)));
        }

        /// <summary>
        /// The 获得VTRID对应的默认磁带ID.
        /// </summary>
        /// <param name="vtrId">The vtrId<see cref="int"/>.</param>
        /// <returns>The 默认磁带ID<see cref="Task{int}"/>.</returns>
        public async Task<int> GetVtrTapeItemAsync(int vtrId)
        {
            return await VtrStore.GetTapeVtrMap(a => a.Where(x => x.Vtrid == vtrId)
                .Select(x => x.Tapeid)
                .FirstOrDefaultAsync());
        }

        /// <summary>
        /// The 根据磁带ID获得磁带信息.
        /// </summary>
        /// <param name="tapeId">The 磁带ID<see cref="int"/>.</param>
        /// <returns>磁带信息.</returns>
        public async Task<VTRTapeInfo> GetTapeInfoByIDAsync(int tapeId)
        {
            return Mapper.Map<VTRTapeInfo>(await VtrStore.GetTapelist(a => a.FirstOrDefaultAsync(x => x.Tapeid == tapeId)));
        }

        /// <summary>
        /// The 设置VTR任务信息，VTRTaskID不存在则添加,否则则修改.
        /// </summary>
        /// <param name="uploadTaskInfo">The VTR任务信息<see cref="VTRUploadTaskInfo"/>.</param>
        /// <returns>The 影响的Vtr任务Id<see cref="Task{int}"/>.</returns>
        public async Task<int> SetVTRUploadTaskInfoAsync<TRequest>(TRequest uploadTaskInfo)
        {
            VTRUploadTaskInfoResponse info = Mapper.Map<VTRUploadTaskInfoResponse>(uploadTaskInfo);

            int retTaskID = -1;

            var vtrtask = await TaskStore.GetVtrUploadTaskAsync(a => a.Where(b => b.Taskid == info.VtrTaskId), true);

            DateTime beginCheckTime = info.CommitTime;

            TimeSpan tsDuration = new TimeSpan();
            if(info.BlankTaskId > 0)//入点加长度
            {
                tsDuration = new TimeSpan(0, 0, info.TrimOutCTL / info.BlankTaskId);
            } else
            {
                SB_TimeCode tcIn = new SB_TimeCode((uint)info.TrimInCTL);
                SB_TimeCode tcOut = new SB_TimeCode((uint)info.TrimOutCTL);
                if((uint)info.TrimOutCTL < (uint)info.TrimInCTL)
                {
                    tcOut.Hour += 24;
                }
                tsDuration = tcOut - tcIn;
            }

            TimePeriod tp = new TimePeriod(info.VtrId, beginCheckTime, beginCheckTime + tsDuration);

            await IsTimePeriodUsable(tp, info.ChannelId, info.VtrId, info.VtrTaskId);

            if(vtrtask == null)
            {
                retTaskID = info.VtrTaskId = await VtrStore.GetTask(a => a.MaxAsync(x => x.Taskid)) + 1;

                if(info.CommitTime == DateTime.MinValue)
                {
                    info.CommitTime = DateTime.Now;
                }

                if(string.IsNullOrEmpty(info.TaskGUID))
                {
                    info.TaskGUID = Guid.NewGuid().ToString();
                }

                var upload = Mapper.Map<VtrUploadtask>(info);
                if(upload.Tapeid == 0)
                {
                    upload.Tapeid = await VtrStore.GetTapeVtrMap(a => a.Where(x => x.Vtrid == upload.Vtrid)
                        .Select(x => x.Tapeid)
                        .SingleOrDefaultAsync());
                }
                await VtrStore.AddUploadtask(upload);

                //填充任务来源表
                var taskSource = new DbpTaskSource();
                taskSource.Taskid = info.VtrTaskId;
                taskSource.Tasksource = (int)TaskSource.emVTRUploadTask;
                await TaskStore.AddTaskSource(taskSource);

                await AddPolicyTaskByUserCode(info.UserCode, info.VtrTaskId);
            } else
            {
                if(info.CommitTime == DateTime.MinValue)
                {
                    info.CommitTime = DateTime.Now;
                }
                info.VtrTaskId = vtrtask.Taskid;
                var upload = Mapper.Map<VtrUploadtask>(info);
                if(upload.Tapeid == 0)
                {
                    upload.Tapeid = await VtrStore.GetTapeVtrMap(a => a.Where(x => x.Vtrid == upload.Vtrid)
                        .Select(x => x.Tapeid)
                        .SingleOrDefaultAsync());
                }
                await VtrStore.UpdateUploadtask(upload);
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
        public async Task SetVBUTasksMetadatasAsync(int taskId, MetaDataType type, string metadata)
        {
            //需要将其中的三个字符串提取出来
            if(type == MetaDataType.emContentMetaData)
            {
                string materialMeta = string.Empty;
                string planningMeta = string.Empty;
                string originalMeta = string.Empty;
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(metadata);
                XmlNode taskContentNode = doc.SelectSingleNode("/TaskContentMetaData");
                if(taskContentNode != null)
                {
                    if(taskContentNode.HasChildNodes)
                    {
                        XmlNode materialNode = doc.SelectSingleNode("/TaskContentMetaData/MetaMaterial");
                        if(materialNode != null)
                        {
                            materialMeta = materialNode.InnerText;
                            taskContentNode.RemoveChild(materialNode);
                        }

                        XmlNode planningNode = doc.SelectSingleNode("/TaskContentMetaData/MetaPlanning");
                        if(planningNode != null)
                        {
                            planningMeta = planningNode.InnerText;
                            taskContentNode.RemoveChild(planningNode);
                        }

                        XmlNode originalNode = doc.SelectSingleNode("/TaskContentMetaData/MetaOriginal");
                        if(originalNode != null)
                        {
                            originalMeta = originalNode.InnerText;
                            taskContentNode.RemoveChild(originalNode);
                        }
                    }
                }

                await TaskStore.UpdateTaskMetaDataAsync(taskId, MetaDataType.emStoreMetaData, materialMeta);
                await TaskStore.UpdateTaskMetaDataAsync(taskId, MetaDataType.emPlanMetaData, planningMeta);
                await TaskStore.UpdateTaskMetaDataAsync(taskId, MetaDataType.emOriginalMetaData, originalMeta);
                await TaskStore.UpdateTaskMetaDataAsync(taskId, MetaDataType.emContentMetaData, doc.OuterXml);
            } else
            {
                await TaskStore.UpdateTaskMetaDataAsync(taskId, type, metadata);
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
            return Mapper.Map<TResult>(await VtrStore.GetUploadtask(a => a.SingleOrDefaultAsync(x => x.Taskid == taskId)));
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
            if(vtrinfo == null)
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
        public async Task<bool> UpdateUploadTaskStateAsync(int taskId, VTRUPLOADTASKSTATE taskState)
        { return await SetVTRUploadTaskStateAsync(taskId, taskState, string.Empty); }

        /// <summary>
        /// The 根据taskId 更新 Vtr上传任务状态..
        /// </summary>
        /// <param name="taskId">The taskId<see cref="int"/>.</param>
        /// <param name="vtrTaskState">The vtr任务状态<see cref="VTRUPLOADTASKSTATE"/>.</param>
        /// <param name="errorContent">The 错误内容<see cref="string"/>.</param>
        /// <returns>The 是否更新成功 <see cref="Task{bool}"/>.</returns>
        public async Task<bool> SetVTRUploadTaskStateAsync(int taskId,
                                                           VTRUPLOADTASKSTATE vtrTaskState,
                                                           string errorContent)
        {
            var upload = await VtrStore.GetUploadtask(a => a.FirstOrDefaultAsync(x => x.Taskid == taskId), true);

            if(upload == null)
            {
                if(vtrTaskState == VTRUPLOADTASKSTATE.VTR_UPLOAD_COMMIT &&
                    (upload.Taskstate == (int)VTRUPLOADTASKSTATE.VTR_UPLOAD_COMPLETE))
                {//已入库素材重新上载是，改变GUID以保证再次入库时不会覆盖前面的素材
                    upload.Taskguid = Guid.NewGuid().ToString();
                }

                upload.Taskstate = (int)vtrTaskState;
                if(vtrTaskState == VTRUPLOADTASKSTATE.VTR_UPLOAD_FAIL)
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
        { return Mapper.Map<List<TResult>>(await VtrStore.GetDetailinfo(a => a, true)); }

        /// <summary>
        /// The 获得需要即将调度的VTR任务.
        /// </summary>
        /// <returns>The 即将调度的VTR任务<see cref="Task{List{TResult}}"/>.</returns>
        public async Task<List<TResult>> GetNeedExecuteVTRUploadTasksAsync<TResult>()
        {
            var uploadTaskList = Mapper.Map<List<VTRUploadTaskContentResponse>>(await VtrStore.GetUploadtask(a => a.Where(x => x.Taskstate ==
                20 &&
                x.Vtrtasktype == 0),
                                                                                                             true));

            List<TaskContent> capturingTasks = await TaskManager.GetAllChannelCapturingTask<TaskContent>();

            //再获得计划的上载任务
            List<VTRUploadTaskContentResponse> vtrTasks = await VtrStore.GetNeedScheduleExecuteVTRUploadTasks(DateTime.Now);
            if(vtrTasks != null && vtrTasks.Count > 0)
            {
                //每个通道只能返回一个，每个VTR也只能返回一个，而且只能返回时间最前的那个                
                foreach(VTRUploadTaskContentResponse vtrScheduleTask in vtrTasks)
                {
                    //重新调整开始时间
                    DateTime dtEnd = vtrScheduleTask.EndTime;
                    if(dtEnd <= DateTime.Now)
                    {
                        //重新调整开始时间跟结束时间 
                        SB_TimeCode tcIn = new SB_TimeCode((uint)vtrScheduleTask.TrimIn);
                        SB_TimeCode tcOut = new SB_TimeCode((uint)vtrScheduleTask.TrimOut);
                        TimeSpan tsDuration = tcOut - tcIn;
                        vtrScheduleTask.BeginTime = DateTime.Now;
                        vtrScheduleTask.EndTime = DateTime.Now + tsDuration;
                        await TaskStore.AdjustVtrUploadTasksByChannelId(vtrScheduleTask.ChannelId,
                                                                        vtrScheduleTask.TaskId,
                                                                        DateTime.Now);
                    }
                    int i = 0;
                    bool isNeedReplace = false;
                    for(; i < uploadTaskList.Count; i++)
                    {
                        if(uploadTaskList[i].ChannelId == vtrScheduleTask.ChannelId ||
                            uploadTaskList[i].VtrId == vtrScheduleTask.VtrId)
                        {
                            //只返回时间上最小的
                            if(vtrScheduleTask.BeginTime < uploadTaskList[i].BeginTime)
                            {
                                isNeedReplace = true;
                                break;
                            }
                        }
                    }

                    bool isHaveCapturingManulTask = false;
                    if(capturingTasks != null)
                    {
                        foreach(TaskContent capturingTask in capturingTasks)
                        {
                            if(capturingTask.nChannelID == vtrScheduleTask.ChannelId)
                            {
                                if(capturingTask.emCooperantType == CooperantType.emKamataki)
                                {
                                    TaskSource ts = await TaskManager.GetTaskSource(capturingTask.nTaskID);
                                    //加上判断，如果遇到vtr上载任务的话，那么先不管，但是不让返回，等待下次判断
                                    if(ts != TaskSource.emVTRUploadTask)
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
                    if(i == 0 || (i == uploadTaskList.Count && !isNeedReplace))
                    {
                        if(!isHaveCapturingManulTask)
                        {
                            uploadTaskList.Add(vtrScheduleTask);
                        }
                    }

                    if(isNeedReplace)
                    {
                        if(!isHaveCapturingManulTask)
                        {
                            uploadTaskList.RemoveAt(i - 1);
                            uploadTaskList.Add(vtrScheduleTask);
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
        private async Task<bool> AddPolicyTaskByUserCode(string userCode, int vtrTaskId)
        {
            List<DbpPolicytask> tasks = new List<DbpPolicytask>();
            //首先根据User ID查找Policy ID
            var policys = await VtrStore.GetPolicyuser(a => a.Where(x => x.Usercode == userCode));
            if(policys.Count > 0)
            {
                var policyIds = policys.Select(x => x.Policyid).ToList();
                tasks = await VtrStore.GetMetadatapolicy(a => a.Where(x => policyIds.Contains(x.Policyid))
                    .Select(x => new DbpPolicytask { Policyid = x.Policyid, Taskid = vtrTaskId }));
            } else
            {
                tasks = await VtrStore.GetMetadatapolicy(a => a.Where(x => x.Defaultpolicy != 0)
                    .Select(x => new DbpPolicytask { Policyid = x.Policyid, Taskid = vtrTaskId }));
            }
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
        private async Task<bool> IsTimePeriodUsable(TimePeriod tp, int channelId, int vtrId, int exTaskId)
        {
            //对进行修改的任务的开始与结束时间段，判断其通道和vtr是否可用
            VTRTimePeriods vtrFreeTimePeriods = new VTRTimePeriods(vtrId);
            vtrFreeTimePeriods.Periods = new List<TimePeriod>();
            DateTime beginCheckTime = tp.StartTime;

            await GetFreeTimePeriodByVtrId(vtrFreeTimePeriods, beginCheckTime, exTaskId);

            if(!IsTimePeriodInVTRTimePeriods(tp, vtrFreeTimePeriods))
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

            if(channelsTimePeriods != null &&
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
            if(vtrFreeTimePeriods.Periods != null)
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
            if(channelFreeTimePeriods.Periods != null)
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
            if(vtrFreeTimePeriods.VTRId <= 0)
            {
                SobeyRecException.ThrowSelfNoParam($"{nameof(GetFreeTimePeriodByVtrId)}: VTRId is smaller than 0",
                                                   GlobalDictionary.GLOBALDICT_CODE_IN_SETVTRTAPEMAP_TAPEID_IS_NOT_EXIST_ONEPARAM,
                                                   Logger,
                                                   null);
            }

            if(beginCheckTime == DateTime.MinValue)
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
            if(manualCapturingTask != null)
            {
                DateTime beginTime = DateTimeFormat.DateTimeFromString(manualCapturingTask.strCommitTime);
                TimeSpan tsDuration = new TimeSpan();
                if(manualCapturingTask.nBlankTaskID > 0)//入点加长度
                {
                    tsDuration = new TimeSpan(0, 0, manualCapturingTask.nTrimOutCTL / manualCapturingTask.nBlankTaskID);
                } else
                {
                    SB_TimeCode inSTC = new SB_TimeCode((uint)manualCapturingTask.nTrimInCTL);
                    SB_TimeCode outSTC = new SB_TimeCode((uint)manualCapturingTask.nTrimOutCTL);
                    tsDuration = outSTC - inSTC;
                }

                DateTime endTime = beginTime + tsDuration;

                vtrTimePeriods.Periods.Add(new TimePeriod(vtrId, beginTime, endTime));
            }

            List<TimePeriod> scheduleTPs = await TaskStore.GetTimePeriodsByScheduleVBUTasks(vtrId, exTaskId);
            if(scheduleTPs != null && scheduleTPs.Count > 0)
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
            if(beginTime < new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0) ||
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
            for(int i = 0; i < 3; i++)
            {
                List<TaskContent> tasksIn = await TaskManager.QueryTaskContent<TaskContent>(0,
                                                                                            DateTime.Now.AddDays(i),
                                                                                            TimeLineType.em24HourDay);
                if(tasksIn != null && tasksIn.Count > 0)
                {
                    tasks.AddRange(tasksIn);
                }
            }

            if(tasks == null || (tasks.Count == 1 && tasks[0] == null))
            {
                foreach(ChannelTimePeriods ctp in chsFreeTimePeriods)
                {
                    ctp.Periods.Add(new TimePeriod(ctp.ChannelId, beginTime, beginTime.AddDays(2)));
                }
                return chsFreeTimePeriods;
            }

            //收集一些通道的被占用情况
            var preiodsTasks = tasks.Where(a => a.nTaskID != exTaskId &&
                DateTimeFormat.DateTimeFromString(a.strEnd) > beginTime)
                .ToList();
            foreach(var task in preiodsTasks)
            {
                var ctp = chsTimePeriods.FirstOrDefault(a => a.ChannelId == task.nChannelID);
                if(ctp != null)
                {
                    DateTime dtBegin = task.strBegin.ToDateTime();
                    DateTime dtEnd = task.strEnd.ToDateTime();
                    if((task.emTaskType == TaskType.TT_MANUTASK || task.emTaskType == TaskType.TT_OPENEND) &&
                        task.emState == taskState.tsExecuting)
                    {
                        dtEnd = dtBegin.AddDays(1);
                        dtBegin = DateTime.Now;
                    }
                    if(dtEnd > beginTime)
                    {
                        ctp.Periods.Add(new TimePeriod(ctp.ChannelId, dtBegin, dtEnd));
                    }
                }
            }

            //对通道里的时间段进行排序
            foreach(var ctp in chsTimePeriods)
            {
                DateTime thirdDay = beginTime.AddDays(3).AddSeconds(-1);
                ctp.Periods = TaskManager.GetFreeTimePeriodsByTieup(ctp.ChannelId, ctp.Periods, beginTime, thirdDay);
                ctp.Periods = ctp.Periods.Where(a => a.Duration.TotalSeconds > 3).ToList();
            }

            return chsFreeTimePeriods;
        }
    }
}
