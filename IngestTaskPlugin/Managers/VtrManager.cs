namespace IngestTaskPlugin.Managers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Xml;
    using AutoMapper;
    using IngestDBCore;
    using IngestDBCore.Interface;
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
    using Microsoft.Extensions.DependencyInjection;
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
        /// 修改磁带信息,ID不存在即为添加.
        /// </summary>
        /// <param name="tapeId">磁带Id.</param>
        /// <param name="tapeName">磁带名称.</param>
        /// <param name="tapeDesc">磁带描述.</param>
        /// <returns>磁带Id.</returns>
        public async Task<int> SetTapeInfoAsync(int tapeId, string tapeName, string tapeDesc)
        {
            var newTapeId = await VtrStore.SaveTaplist(new VtrTapelist
            { Tapeid = tapeId, Tapename = tapeName, Tapedesc = tapeDesc });
            return newTapeId;
        }

        /// <summary>
        /// 设置VTR与磁带的对应序列.
        /// </summary>
        /// <param name="vtrId">vtr id.</param>
        /// <param name="tapeId">磁带id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task SetVtrTapeMapAsync(int vtrId, int tapeId)
        {
            await VtrStore.SaveTapeVtrMap(new VtrTapeVtrMap { Vtrid = vtrId, Tapeid = tapeId });
        }

        /// <summary>
        /// 获得所有的磁带信息.
        /// </summary>
        /// <returns>The <see cref="Task{List{VTRTapeInfo}}"/>.</returns>
        public async Task<List<VTRTapeInfo>> GetAllTapeInfoAsync()
        {
            return Mapper.Map<List<VTRTapeInfo>>(await VtrStore.GetTapelist(a => a.OrderByDescending(x => x.Tapeid)
                .Take(50)));
        }

        /// <summary>
        /// 获得VTRID对应的默认磁带ID.
        /// </summary>
        /// <param name="vtrId">VTRID.</param>
        /// <returns>默认磁带ID.</returns>
        public async Task<int> GetVtrTapeItemAsync(int vtrId)
        {
            return await VtrStore.GetTapeVtrMap(a => a.Where(x => x.Vtrid == vtrId)
                .Select(x => x.Tapeid)
                .FirstOrDefaultAsync());
        }

        /// <summary>
        /// 根据ID获得磁带信息.
        /// </summary>
        /// <param name="tapeId">id.</param>
        /// <returns>磁带信息.</returns>
        public async Task<VTRTapeInfo> GetTapeInfoByIDAsync(int tapeId)
        {
            return Mapper.Map<VTRTapeInfo>(await VtrStore.GetTapelist(a => a.FirstOrDefaultAsync(x => x.Tapeid == tapeId)));
        }

        /// <summary>
        /// The 设置VTR任务信息，VTRTaskID不存在则添加,否则则修改..
        /// </summary>
        /// <param name="uploadTaskInfo">The uploadTaskInfo<see cref="VTRUploadTaskInfo"/>.</param>
        /// <returns>The <see cref="Task{int}"/>.</returns>
        public async Task<int> SetVTRUploadTaskInfoAsync(VTRUploadTaskInfo uploadTaskInfo)
        {
            int retTaskID = -1;

            var vtrtask = await TaskStore.GetVtrUploadTaskAsync(a => a.Where(b => b.Taskid == uploadTaskInfo.nVTRTaskID), true);

            //先判断一下，提交的这个任务，时间是否可以用
            DateTime beginCheckTime = DateTime.MinValue;
            if (!string.IsNullOrEmpty(uploadTaskInfo.strCommitTime))
            {
                beginCheckTime = uploadTaskInfo.strCommitTime.ToDateTime(DateTime.Now);
            }

            TimeSpan tsDuration = new TimeSpan();
            if (uploadTaskInfo.nBlankTaskID > 0)//入点加长度
            {
                tsDuration = new TimeSpan(0, 0, uploadTaskInfo.nTrimOutCTL / uploadTaskInfo.nBlankTaskID);
            }
            else
            {
                SB_TimeCode tcIn = new SB_TimeCode((uint)uploadTaskInfo.nTrimInCTL);
                SB_TimeCode tcOut = new SB_TimeCode((uint)uploadTaskInfo.nTrimOutCTL);
                if ((uint)uploadTaskInfo.nTrimOutCTL < (uint)uploadTaskInfo.nTrimInCTL)
                {
                    tcOut.Hour += 24;
                }
                tsDuration = tcOut - tcIn;
            }

            TimePeriod tp = new TimePeriod(uploadTaskInfo.nVtrID, beginCheckTime, beginCheckTime + tsDuration);

            await IsTimePeriodUsable(tp, uploadTaskInfo.nChannelID, uploadTaskInfo.nVtrID, uploadTaskInfo.nVTRTaskID);

            if (vtrtask == null)
            {
                retTaskID = uploadTaskInfo.nVTRTaskID = await VtrStore.GetTask(a => a.MaxAsync(x => x.Taskid)) + 1;

                if (uploadTaskInfo.strCommitTime == DateTimeFormat.DateTimeToString(DateTime.MinValue))
                {
                    uploadTaskInfo.strCommitTime = DateTimeFormat.DateTimeToString(DateTime.Now);
                }

                if (string.IsNullOrEmpty(uploadTaskInfo.strTaskGUID))
                {
                    uploadTaskInfo.strTaskGUID = Guid.NewGuid().ToString();
                }

                var upload = Mapper.Map<VtrUploadtask>(uploadTaskInfo);
                if (upload.Tapeid == 0)
                {
                    upload.Tapeid = await VtrStore.GetTapeVtrMap(a => a.Where(x => x.Vtrid == upload.Vtrid).Select(x => x.Tapeid).SingleOrDefaultAsync());
                }
                await VtrStore.AddUploadtask(upload);

                //填充任务来源表
                var taskSource = new DbpTaskSource();
                taskSource.Taskid = uploadTaskInfo.nVTRTaskID;
                taskSource.Tasksource = (int)TaskSource.emVTRUploadTask;
                await TaskStore.AddTaskSource(taskSource);

                await AddPolicyTaskByUserCode(uploadTaskInfo.strUserCode, uploadTaskInfo.nVTRTaskID);
            }
            else
            {
                if (uploadTaskInfo.strCommitTime == DateTimeFormat.DateTimeToString(DateTime.MinValue))
                {
                    uploadTaskInfo.strCommitTime = DateTimeFormat.DateTimeToString(DateTime.Now);
                }
                uploadTaskInfo.nVTRTaskID = vtrtask.Taskid;
                var upload = Mapper.Map<VtrUploadtask>(uploadTaskInfo);
                if (upload.Tapeid == 0)
                {
                    upload.Tapeid = await VtrStore.GetTapeVtrMap(a => a.Where(x => x.Vtrid == upload.Vtrid).Select(x => x.Tapeid).SingleOrDefaultAsync());
                }
                await VtrStore.UpdateUploadtask(upload);
            }

            //UpdateTime = DateTime.Now;

            return retTaskID;
        }

        public async Task SetVBUTasksMetadatasAsync(int taskId, MetaDataType type, string metadata)
        {
            //需要将其中的三个字符串提取出来
            if (type == MetaDataType.emContentMetaData)
            {
                string materialMeta = string.Empty;
                string planningMeta = string.Empty;
                string originalMeta = string.Empty;
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(metadata);
                XmlNode taskContentNode = doc.SelectSingleNode("/TaskContentMetaData");
                if (taskContentNode != null)
                {
                    if (taskContentNode.HasChildNodes)
                    {
                        XmlNode materialNode = doc.SelectSingleNode("/TaskContentMetaData/MetaMaterial");
                        if (materialNode != null)
                        {
                            materialMeta = materialNode.InnerText;
                            taskContentNode.RemoveChild(materialNode);
                        }

                        XmlNode planningNode = doc.SelectSingleNode("/TaskContentMetaData/MetaPlanning");
                        if (planningNode != null)
                        {
                            planningMeta = planningNode.InnerText;
                            taskContentNode.RemoveChild(planningNode);
                        }

                        XmlNode originalNode = doc.SelectSingleNode("/TaskContentMetaData/MetaOriginal");
                        if (originalNode != null)
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
            }
            else
            {
                await TaskStore.UpdateTaskMetaDataAsync(taskId, (MetaDataType)type, metadata);
            }
        }

        public async Task<List<VTRUploadTaskInfo>> QueryVTRUploadTaskInfoAsync(VTRUploadCondition condition)
        {
            return Mapper.Map<List<VTRUploadTaskInfo>>(await VtrStore.GetUploadtaskInfo(condition, true));
        }

        public async Task<List<VTRUploadTaskContent>> GetVTRUploadTasksAsync(VTRUploadCondition condition)
        {
            return Mapper.Map<List<VTRUploadTaskContent>>(await VtrStore.GetUploadTaskContent(condition));
        }

        public async Task<VTRDetailInfo> GetVTRDetailInfoByID(int vtrId)
        {
            VTRDetailInfo vtrinfo = new VTRDetailInfo();
            var row = Mapper.Map<VTRDetailInfo>(await VtrStore.GetDetailinfo(a => a.SingleOrDefaultAsync(x => x.Vtrid == vtrId)));
            if (row == null)
            {
                SobeyRecException.ThrowSelfNoParam(string.Format(GlobalDictionary.Instance.GetMessageByCode(GlobalDictionary.GLOBALDICT_CODE_VTRID_DOES_NOT_EXIST_THE_NVTRID_IS_ONEPARAM),
                    vtrId.ToString()), GlobalDictionary.GLOBALDICT_CODE_VTRID_DOES_NOT_EXIST_THE_NVTRID_IS_ONEPARAM, Logger, null);
            }
            return vtrinfo;
        }

        public async Task<VtrState> GetVtrStateAsync(int vtrId)
        {
            return (VtrState)await VtrStore.GetDetailinfo(a => a.Where(x => x.Vtrid == vtrId).Select(x => x.Vtrstate).SingleOrDefaultAsync());
        }

        public async Task<bool> UpdateUploadTaskStateAsync(int taskId, int taskState)
        {
            return await SetVTRUploadTaskStateAsync(taskId, (VTRUPLOADTASKSTATE)taskState, string.Empty);
        }

        public async Task<bool> SetVTRUploadTaskStateAsync(int taskId, VTRUPLOADTASKSTATE vtrTaskState, string errorContent)
        {
            var upload = await VtrStore.GetUploadtask(a => a.FirstOrDefaultAsync(x => x.Taskid == taskId), true);

            if (upload == null)
            {
                if (vtrTaskState == VTRUPLOADTASKSTATE.VTR_UPLOAD_COMMIT && (upload.Taskstate == (int)VTRUPLOADTASKSTATE.VTR_UPLOAD_COMPLETE))
                {//已入库素材重新上载是，改变GUID以保证再次入库时不会覆盖前面的素材
                    upload.Taskguid = Guid.NewGuid().ToString();
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


        private async Task AddPolicyTaskByUserCode(string userCode, int vtrTaskId)
        {
            List<DbpPolicytask> tasks = new List<DbpPolicytask>();
            //首先根据User ID查找Policy ID
            var policys = await VtrStore.GetPolicyuser(a => a.Where(x => x.Usercode == userCode));
            if (policys.Count > 0)
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
            await TaskStore.AddPolicyTask(tasks);
        }

        /// <summary>
        /// The IsTimePeriodUsable.
        /// </summary>
        /// <param name="tp">The tp<see cref="TimePeriod"/>.</param>
        /// <param name="channelId">The channelId<see cref="int"/>.</param>
        /// <param name="vtrId">The vtrId<see cref="int"/>.</param>
        /// <param name="exTaskId">The exTaskId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{bool}"/>.</returns>
        private async Task<bool> IsTimePeriodUsable(TimePeriod tp, int channelId, int vtrId, int exTaskId)
        {
            //对进行修改的任务的开始与结束时间段，判断其通道和vtr是否可用
            VTRTimePeriods vtrFreeTimePeriods = new VTRTimePeriods(vtrId);
            vtrFreeTimePeriods.Periods = new List<TimePeriod>();
            DateTime beginCheckTime = tp.StartTime;

            await GetFreeTimePeriodByVtrId(vtrFreeTimePeriods, beginCheckTime, exTaskId);

            if (!IsTimePeriodInVTRTimePeriods(tp, vtrFreeTimePeriods))
            {
                SobeyRecException.ThrowSelfNoParam(nameof(IsTimePeriodUsable) + "VTR Collide", GlobalDictionary.GLOBALDICT_CODE_IN_ISVTRCOLLIDE_BEGINTIME_IS_WRONG, Logger, null);
                return false;
            }

            List<int> channelIds = new List<int>();
            List<ChannelTimePeriods> channelsTimePeriods = new List<ChannelTimePeriods>();
            channelIds.Add(channelId);
            channelsTimePeriods = await GetChannelsFreeTimePeriods(beginCheckTime, channelIds, exTaskId);

            if (channelsTimePeriods != null && channelsTimePeriods[0] != null && !IsTimePeriodInChannelTimePeriods(tp, channelsTimePeriods[0]))
            {
                SobeyRecException.ThrowSelfNoParam(nameof(IsTimePeriodUsable) + "No Channel", GlobalDictionary.GLOBALDICT_CODE_IN_ISVTRCOLLIDE_BEGINTIME_IS_WRONG, Logger, null);
                return false;
            }
            return true;
        }

        /// <summary>
        /// The IsTimePeriodInVTRTimePeriods.
        /// </summary>
        /// <param name="tp">The tp<see cref="TimePeriod"/>.</param>
        /// <param name="vtrFreeTimePeriods">The vtrFreeTimePeriods<see cref="VTRTimePeriods"/>.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        private bool IsTimePeriodInVTRTimePeriods(TimePeriod tp, VTRTimePeriods vtrFreeTimePeriods)
        {
            if (vtrFreeTimePeriods.Periods != null)
            {
                return vtrFreeTimePeriods.Periods.Any(x => x.EndTime >= tp.EndTime && x.StartTime <= tp.StartTime);
            }
            return false;
        }

        /// <summary>
        /// The IsTimePeriodInChannelTimePeriods.
        /// </summary>
        /// <param name="tp">The tp<see cref="TimePeriod"/>.</param>
        /// <param name="channelFreeTimePeriods">The channelFreeTimePeriods<see cref="ChannelTimePeriods"/>.</param>
        /// <returns>The <see cref="bool"/>.</returns>
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
        /// The GetFreeTimePeriodByVtrId.
        /// </summary>
        /// <param name="vtrFreeTimePeriods">The vtrFreeTimePeriods<see cref="VTRTimePeriods"/>.</param>
        /// <param name="beginCheckTime">The beginCheckTime<see cref="DateTime"/>.</param>
        /// <param name="exTaskId">The exTaskId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{VTRTimePeriods}"/>.</returns>
        private async Task<VTRTimePeriods> GetFreeTimePeriodByVtrId(VTRTimePeriods vtrFreeTimePeriods, DateTime beginCheckTime, int exTaskId)
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
            var manualCapturingTask = Mapper.Map<VTRUploadTaskInfo>(await TaskStore.GetVtrUploadTaskAsync(a => a.Where(x =>
                                                                                                                       taskStates.Contains(x.Taskstate) &&
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
            vtrFreeTimePeriods.Periods = TaskManager.GetFreeTimePeriodsByTieup(vtrId, vtrTimePeriods.Periods, beginCheckTime.AddSeconds(-3), thirdDay);

            vtrFreeTimePeriods.Periods = vtrFreeTimePeriods.Periods.Where(a => a.Duration.TotalSeconds > 3).ToList();
            return vtrFreeTimePeriods;
        }

        /// <summary>
        /// The GetChannelsFreeTimePeriods.
        /// </summary>
        /// <param name="beginTime">The beginTime<see cref="DateTime"/>.</param>
        /// <param name="channelIds">The channelIds<see cref="List{int}"/>.</param>
        /// <param name="exTaskId">The exTaskId<see cref="int"/>.</param>
        /// <returns>The <see cref="List{ChannelTimePeriods}"/>.</returns>
        private async Task<List<ChannelTimePeriods>> GetChannelsFreeTimePeriods(DateTime beginTime, List<int> channelIds, int exTaskId)
        {
            if (beginTime < new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0) || channelIds == null || channelIds.Count <= 0)
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
                List<TaskContent> tasksIn = await TaskManager.QueryTaskContent<TaskContent>(0, DateTime.Now.AddDays(i), TimeLineType.em24HourDay);
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
            var preiodsTasks = tasks.Where(a => a.nTaskID != exTaskId && DateTimeFormat.DateTimeFromString(a.strEnd) > beginTime).ToList();
            foreach (var task in preiodsTasks)
            {
                var ctp = chsTimePeriods.FirstOrDefault(a => a.ChannelId == task.nChannelID);
                if (ctp != null)
                {
                    DateTime dtBegin = task.strBegin.ToDateTime();
                    DateTime dtEnd = task.strEnd.ToDateTime();
                    if ((task.emTaskType == TaskType.TT_MANUTASK || task.emTaskType == TaskType.TT_OPENEND) && task.emState == taskState.tsExecuting)
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
                DateTime thirdDay = beginTime.AddDays(3).AddSeconds(-1);
                ctp.Periods = TaskManager.GetFreeTimePeriodsByTieup(ctp.ChannelId, ctp.Periods, beginTime, thirdDay);
                ctp.Periods = ctp.Periods.Where(a => a.Duration.TotalSeconds > 3).ToList();
            }

            return chsFreeTimePeriods;
        }




        #region vtr task update

        public async Task<TResult> SetVTRUploadTask<TResult>(VTRUploadTaskContent vtrTask, VTR_UPLOAD_MetadataPair[] metadatas, long lMask)
        {
            if (vtrTask.nTaskId <= 0)
            {
                return default(TResult);
            }

            VTRUploadCondition Condition = new VTRUploadCondition() { lTaskID = vtrTask.nTaskId };
            List<VTRUploadTaskContent> vtrTasks = await VtrStore.GetUploadTaskContent(Condition);

            // 新增从普通任务转换为VTR任务 VTR表中无法查询到任务，该任务原本可能是一个普通任务
            if (vtrTasks == null || vtrTasks.Count <= 0)
            {
                if (vtrTask.emTaskState != VTRUPLOADTASKSTATE.VTR_UPLOAD_COMMIT && vtrTask.emVtrTaskType != VTRUPLOADTASKTYPE.VTR_SCHEDULE_UPLOAD)
                {
                    throw new Exception("Can not find the task.TaskId = " + vtrTask.nTaskId);
                }

                var dbpTask = await TaskStore.GetTaskAsync(a => a.Where(x => x.Taskid == vtrTask.nTaskId), true);
                if (dbpTask == null)
                {
                    throw new Exception("Can not find the task in DBP_TASK.TaskId = " + vtrTask.nTaskId);
                }

                if (dbpTask.State != (int)taskState.tsReady)
                {
                    throw new Exception($"Can not modify a normal task to vtr task which is not in ready state.TaskId = {vtrTask.nTaskId} ");
                }

                if (dbpTask.Tasktype != (int)TaskType.TT_NORMAL)
                {
                    throw new Exception("Can not modify a task to vtr task which is not a normal task.TaskId = " + vtrTask.nTaskId);
                }

                try
                {
                    Logger.Info("In SetVTRUploadTask.Before ModifyNormalTaskToVTRUploadTask");
                    await ModifyNormalTaskToVTRUploadTask(vtrTask, metadatas);


                    return Mapper.Map<TResult>(vtrTask);
                }
                catch (System.Exception ex)
                {
                    throw;
                }
            }

            VTRUploadTaskContent vtrTaskNow = vtrTasks[0];
            //正在执行的状态下，不允许更新时间，不允许更新通道，不允许更新信号源，不允许更新vtrId
            if (vtrTaskNow.emTaskState == VTRUPLOADTASKSTATE.VTR_UPLOAD_EXECUTE ||
                vtrTaskNow.emState == (int)taskState.tsExecuting)
            {
                Logger.Info("begin to into filling vtr jugde modify!");
                Logger.Info(string.Format("vtrTaskNow.strBegin:{0},vtrTask.strBegin:{1}, vtrTaskNow.strEnd:{2}, vtrTask.strEnd:{3},", vtrTaskNow.strBegin, vtrTask.strBegin, vtrTaskNow.strEnd, vtrTask.strEnd));
                Logger.Info(string.Format("vtrTaskNow.nTrimIn:{0},vtrTask.nTrimIn:{1}, vtrTaskNow.nTrimOut:{2}, vtrTask.strEnd:{3},", vtrTaskNow.nTrimIn, vtrTask.nTrimIn, vtrTaskNow.nTrimOut, vtrTask.nTrimOut));
                Logger.Info(string.Format("vtrTaskNow.nChannelId:{0}, vtrTask.nChannelId:{1},vtrTaskNow.nSignalId:{2}, vtrTask.nSignalId:{3}, vtrTaskNow.nVtrId:{4},vtrTask.nVtrId:{5} ,", vtrTaskNow.nChannelId, vtrTask.nChannelId, vtrTaskNow.nSignalId, vtrTask.nSignalId, vtrTaskNow.nVtrId, vtrTask.nVtrId));
                if (lMask <= 0)
                {
                    DateTime dt1 = DateTimeFormat.DateTimeFromString(vtrTaskNow.strBegin);
                    DateTime dt2 = DateTimeFormat.DateTimeFromString(vtrTask.strBegin);

                    DateTime dt3 = DateTimeFormat.DateTimeFromString(vtrTaskNow.strEnd);
                    DateTime dt4 = DateTimeFormat.DateTimeFromString(vtrTask.strEnd);

                    if (dt1 != dt2 || dt3 != dt4 ||
                        vtrTaskNow.nTrimIn != vtrTask.nTrimIn ||
                        vtrTaskNow.nTrimInCTL != vtrTask.nTrimInCTL ||
                        vtrTaskNow.nTrimOut != vtrTask.nTrimOut ||
                        vtrTaskNow.nTrimOutCTL != vtrTask.nTrimOutCTL)
                    {
                        Logger.Info(string.Format("1Can not modify the duration where the task is running.TaskId = {0},TaskName = {1}", vtrTaskNow.nTaskId, vtrTaskNow.strTaskName));
                        //throw new Exception(string.Format("Can not modify the duration where the task is running.TaskId = {0},TaskName = {1}", vtrTaskNow.nTaskId, vtrTaskNow.strTaskName));
                        SobeyRecException.ThrowSelfNoParam(vtrTaskNow.nTaskId.ToString(), GlobalDictionary.GLOBALDICT_CODE_CANNOTMODIFYTASK_WHERE_FILING, Logger, null);
                    }

                    if (vtrTaskNow.nChannelId != vtrTask.nChannelId)
                    {
                        Logger.Info(string.Format("2Can not modify the channel where the task is running.TaskId = {0},TaskName = {1}", vtrTaskNow.nTaskId, vtrTaskNow.strTaskName));
                        //throw new Exception(string.Format("Can not modify the channel where the task is running.TaskId = {0},TaskName = {1}", vtrTaskNow.nTaskId, vtrTaskNow.strTaskName));
                        SobeyRecException.ThrowSelfNoParam(vtrTaskNow.nTaskId.ToString(), GlobalDictionary.GLOBALDICT_CODE_CANNOTMODIFYTASK_WHERE_FILING, Logger, null);
                    }

                    if (vtrTaskNow.nSignalId != vtrTask.nSignalId)
                    {
                        Logger.Info(string.Format("3Can not modify the signal where the task is running.TaskId = {0},TaskName = {1}", vtrTaskNow.nTaskId, vtrTaskNow.strTaskName));
                        //throw new Exception(string.Format("Can not modify the signal where the task is running.TaskId = {0},TaskName = {1}", vtrTaskNow.nTaskId, vtrTaskNow.strTaskName));
                        SobeyRecException.ThrowSelfNoParam(vtrTaskNow.nTaskId.ToString(), GlobalDictionary.GLOBALDICT_CODE_CANNOTMODIFYTASK_WHERE_FILING, Logger, null);
                    }

                    if (vtrTaskNow.nVtrId != vtrTask.nVtrId)
                    {
                        Logger.Info(string.Format("4Can not modify the vtr where the task is running.TaskId = {0},TaskName = {1}", vtrTaskNow.nTaskId, vtrTaskNow.strTaskName));
                        //throw new Exception(string.Format("Can not modify the vtr where the task is running.TaskId = {0},TaskName = {1}", vtrTaskNow.nTaskId, vtrTaskNow.strTaskName));
                        SobeyRecException.ThrowSelfNoParam(vtrTaskNow.nTaskId.ToString(), GlobalDictionary.GLOBALDICT_CODE_CANNOTMODIFYTASK_WHERE_FILING, Logger, null);
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
                        Logger.Info(string.Format("5Can not modify the duration where the task is running.TaskId = {0},TaskName = {1}", vtrTaskNow.nTaskId, vtrTaskNow.strTaskName));
                        //throw new Exception(string.Format("Can not modify the duration where the task is running.TaskId = {0},TaskName = {1}", vtrTaskNow.nTaskId, vtrTaskNow.strTaskName));
                        SobeyRecException.ThrowSelfNoParam(vtrTaskNow.nTaskId.ToString(), GlobalDictionary.GLOBALDICT_CODE_CANNOTMODIFYTASK_WHERE_FILING, Logger, null);
                    }

                    if (IsInMask(lMask, VTRUploadTaskMask.VTR_Mask_ChannelId))
                    {
                        Logger.Info(string.Format("6Can not modify the channel where the task is running.TaskId = {0},TaskName = {1}", vtrTaskNow.nTaskId, vtrTaskNow.strTaskName));
                        //throw new Exception(string.Format("Can not modify the channel where the task is running.TaskId = {0},TaskName = {1}", vtrTaskNow.nTaskId, vtrTaskNow.strTaskName));
                        SobeyRecException.ThrowSelfNoParam(vtrTaskNow.nTaskId.ToString(), GlobalDictionary.GLOBALDICT_CODE_CANNOTMODIFYTASK_WHERE_FILING, Logger, null);
                    }

                    if (IsInMask(lMask, VTRUploadTaskMask.VTR_Mask_SignalId))
                    {
                        Logger.Info(string.Format("7Can not modify the signal where the task is running.TaskId = {0},TaskName = {1}", vtrTaskNow.nTaskId, vtrTaskNow.strTaskName));
                        //throw new Exception(string.Format("Can not modify the signal where the task is running.TaskId = {0},TaskName = {1}", vtrTaskNow.nTaskId, vtrTaskNow.strTaskName));
                        SobeyRecException.ThrowSelfNoParam(vtrTaskNow.nTaskId.ToString(), GlobalDictionary.GLOBALDICT_CODE_CANNOTMODIFYTASK_WHERE_FILING, Logger, null);
                    }

                    if (IsInMask(lMask, VTRUploadTaskMask.VTR_Mask_VtrId))
                    {
                        Logger.Info(string.Format("8Can not modify the vtr where the task is running.TaskId = {0},TaskName = {1}", vtrTaskNow.nTaskId, vtrTaskNow.strTaskName));
                        //throw new Exception(string.Format("Can not modify the vtr where the task is running.TaskId = {0},TaskName = {1}", vtrTaskNow.nTaskId, vtrTaskNow.strTaskName));
                        SobeyRecException.ThrowSelfNoParam(vtrTaskNow.nTaskId.ToString(), GlobalDictionary.GLOBALDICT_CODE_CANNOTMODIFYTASK_WHERE_FILING, Logger, null);
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
                vtrTask.strBegin = vtrTaskNow.strBegin;
                vtrTask.strEnd = vtrTaskNow.strEnd;
            }

            var vtrResult = await TaskStore.GetVtrUploadTaskAsync(a => a.Where(x => x.Taskid == vtrTask.nTaskId));

            if (vtrResult == null)
            {
                throw new Exception("Can not find the row in VTR_UPLOADTASK table,TaskId = " + vtrTask.nTaskId.ToString());
            }

            var vtrtask = await TaskStore.GetTaskListAsync(a => a.Where(x => x.Taskid == vtrTask.nTaskId));

            if (vtrtask == null)
            {
                throw new Exception("Can not find the row in DBP_TASK table,TaskId = " + vtrTask.nTaskId.ToString());
            }

            await TaskStore.UpdateTaskListAsync(vtrtask);
            await VtrStore.UpdateUploadtask(vtrResult);

            if (metadatas != null && metadatas.Length > 0)
            {
                foreach (VTR_UPLOAD_MetadataPair meta in metadatas)
                {
                    await SetVBUTasksMetadatasAsync(vtrTask.nTaskId, (MetaDataType)meta.emType, meta.strMetadata);
                }
            }

            return Mapper.Map<TResult>(vtrTask);
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
        public async Task<bool> ModifyNormalTaskToVTRUploadTask(VTRUploadTaskContent vtrTask, VTR_UPLOAD_MetadataPair[] metadatas)
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

            await GetFreeTimePeriodByVtrId(freeVtrTimePeriods, preSetBeginTime, vtrTask.nTaskId);

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
                var _deviceinterface = ApplicationContext.Current.ServiceProvider.GetRequiredService<IIngestDeviceInterface>();
                if (_deviceinterface != null)
                {
                    var response = await _deviceinterface.GetDeviceCallBack(new DeviceInternals()
                    {
                        funtype = IngestDBCore.DeviceInternals.FunctionType.ChannelInfoBySrc,
                        SrcId = vtrTask.nSignalId,
                        Status = 0
                    });

                    var channelInfos = response as ResponseMessage<List<CaptureChannelInfoInterface>>;

                    foreach (CaptureChannelInfoInterface info in channelInfos.Ext)
                    {
                        channelIds.Add(info.ID);
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
            var dbpTask = await TaskStore.GetTaskListAsync(a => a.Where(x => x.Taskid == vtrTask.nTaskId));

            //更新DBP_TASK
            Logger.Info("In ModifyNormalTaskToVTRUploadTask.Before Updating DBP_TASK");
            if (dbpTask.Count >= 1)
            {
                vtrTask.strUserToken = "VTRBATCHUPLOAD_ERROR_OK";
                dbpTask[0].Tasklock = string.Empty;
            }
            else
            {
                throw new Exception("In ModifyNormalTaskToVTRUploadTask.Can not find the task in DBP_TASK.TaskId = " + vtrTask.nTaskId.ToString());
            }

            await TaskStore.UpdateTaskListAsync(dbpTask);
            //添加VTR_UPLOADTASK
            Logger.Info("In ModifyNormalTaskToVTRUploadTask.Before Adding VTR_UPLOADTASK");
            await VtrStore.AddUploadtask(Mapper.Map<VtrUploadtask>(vtrTask));

            //更新任务来源表
            Logger.Info("In ModifyNormalTaskToVTRUploadTask.Before Updating DBP_TASK_SOURCE");
            await TaskStore.UpdateTaskSource(new DbpTaskSource() { Taskid = vtrTask.nTaskId, Tasksource = (int)TaskSource.emVTRUploadTask });

            Logger.Info("In ModifyNormalTaskToVTRUploadTask.Before Updating dataset");
            // 更新MetaData
            if (metadatas != null)
            {
                Logger.Info("In ModifyNormalTaskToVTRUploadTask.Before Updating metadatas");
                foreach (VTR_UPLOAD_MetadataPair metadata in metadatas)
                {
                    await SetVBUTasksMetadatasAsync(vtrTask.nTaskId, (MetaDataType)metadata.emType, metadata.strMetadata);
                }
            }
            Logger.Info("In ModifyNormalTaskToVTRUploadTask.Fin");
            return true;
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
                    TimePeriod tp = new TimePeriod();
                    tp = TimePeriod.GetIntersect(chTP, vtrFreeTP.Periods[i]);
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
            VTRUploadCondition condition = new VTRUploadCondition() { lTaskID = taskID };
            var result = await VtrStore.GetUploadtaskInfo(condition, true);
            return Mapper.Map<TResult>(result.FirstOrDefault());
        }

        public async Task<VTR_BUT_ErrorCode> CommitVTRBatchUploadTasks(int[] taskIds, bool ignoreWrong)
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
                    var _globalinterface = ApplicationContext.Current.ServiceProvider.GetRequiredService<IIngestGlobalInterface>();
                    if (_globalinterface != null)
                    {
                        GlobalInternals re = new GlobalInternals() { funtype = IngestDBCore.GlobalInternals.FunctionType.SetGlobalState, State = ClientOperLabelName.VTR_UPLOAD_ModifyTask };
                        var response1 = await _globalinterface.SubmitGlobalCallBack(re);
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
                                        if (inport.RCDeviceID == vtrTaskList[i].Vtrid)
                                        {
                                            signalId = inport.SignalSrcID;
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
                        vtrId = vtrTaskList[0].Vtrid == null?-1: (int)vtrTaskList[0].Vtrid;
                        //AddCommitVTRBUTasks(vtrTaskList, ignoreWrong, null, false, out taskIdList);
                        await AddCommitVTRBUTasksEx(Mapper.Map<List<VTRUploadTaskContent>>(vtrTaskList), ignoreWrong, null, false, taskIdList);
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
                        VTRDetailInfo vtrInfo = Mapper.Map<VTRDetailInfo>((await VtrStore.GetDetailinfo(a=>a.Where(x=>x.Vtrid == vtrId),true)).FirstOrDefault());
                        msg = string.Format("{0} has been used by other tasks", vtrInfo.szVTRDetailName);
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

            return errorCode;
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

        //private async Task<bool> AddCommitVTRBUTasksEx(List<VTRUploadTaskContent> commitTasks, bool ignoreWrong, VTR_UPLOAD_MetadataPair[] metadatas, bool isAdd2DB, out List<int> taskIds)
        private async Task<bool> AddCommitVTRBUTasksEx(List<VTRUploadTaskContent> commitTasks, bool ignoreWrong, VTR_UPLOAD_MetadataPair[] metadatas, bool isAdd2DB, List<int> taskIds)
        {
            taskIds = new List<int>();
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
                //Sobey.Ingest.Log.LoggerService.Info("In RecBusinessRule.VTROper.AddCommitVTRBUTasksEx. preSetEndTime:" + GlobalFun.DateTimeToString(preSetEndTime));

                await GetFreeTimePeriodByVtrId(freeVtrTimePeriods, preSetBeginTime, vtrTask.nTaskId);
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
                    var _deviceinterface = ApplicationContext.Current.ServiceProvider.GetRequiredService<IIngestDeviceInterface>();
                    if (_deviceinterface != null)
                    {
                        var response = await _deviceinterface.GetDeviceCallBack(new DeviceInternals()
                        {
                            funtype = IngestDBCore.DeviceInternals.FunctionType.ChannelInfoBySrc,
                            SrcId = vtrTask.nSignalId,
                            Status = 0
                        });

                        var channelInfos = response as ResponseMessage<List<CaptureChannelInfoInterface>>;

                        foreach (CaptureChannelInfoInterface info in channelInfos.Ext)
                        {
                            channelIds.Add(info.ID);
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
                                //Sobey.Ingest.Log.LoggerService.Info("In RecBusinessRule.VTROper.AddCommitVTRBUTasksEx. preSetEndTime2:" + GlobalFun.DateTimeToString(preSetEndTime));
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


                await GetFreeTimePeriodByVtrId(freeVtrTimePeriods, preSetBeginTime, -1);//这多个通道要提交的，要不就是还没提交过的，要不就是提交过的，但是是失败的任务
                if (freeVtrTimePeriods == null
                    || freeVtrTimePeriods.Periods == null
                    || freeVtrTimePeriods.Periods.Count <= 0)
                {
                    throw new Exception("No Channel");
                }

                List<int> channelIds = new List<int>();
                List<ChannelTimePeriods> channelsTimePeriods = new List<ChannelTimePeriods>();

                var _deviceinterface = ApplicationContext.Current.ServiceProvider.GetRequiredService<IIngestDeviceInterface>();
                if (_deviceinterface != null)
                {
                    var response = await _deviceinterface.GetDeviceCallBack(new DeviceInternals()
                    {
                        funtype = IngestDBCore.DeviceInternals.FunctionType.AllCaptureChannels
                    });

                    var channelInfos = response as ResponseMessage<List<CaptureChannelInfoInterface>>;

                    foreach (CaptureChannelInfoInterface info in channelInfos.Ext)
                    {
                        channelIds.Add(info.ID);
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
            await SetVBUT2DataSet(commitTasks, metadatas, isAdd2DB, taskIds);

            return true;
        }

        private async Task SetVBUT2DataSet(List<VTRUploadTaskContent> vbuTasks, VTR_UPLOAD_MetadataPair[] metadatas, bool isAdd2DB, List<int> taskIds)
        {
            vbuTasks.ForEach(x => {
                if(x.nTaskId <0)
                {
                    x.nTaskId = IngestTaskDBContext.next_val("DBP_SQ_TASKID");
                }

                if (string.IsNullOrEmpty(x.strTaskGUID))
                {
                    x.strTaskGUID = Guid.NewGuid().ToString();
                }
            });

            await TaskStore.UpdateTaskListAsync(Mapper.Map<List<DbpTask>>(vbuTasks));

            foreach (VTRUploadTaskContent task in vbuTasks)
            {
                if (metadatas != null)
                {
                    foreach (VTR_UPLOAD_MetadataPair metadata in metadatas)
                    {
                        //if (metadata.nTaskID == tempId)
                        //{
                        //    //taskOper.SetTaskMetaData(task.nTaskId, (MetaDataType)metadata.emType, metadata.strMetadata);
                        //    SetVBUTasksMetadatas(taskOper, task.nTaskId, (MetaDataType)metadata.emType, metadata.strMetadata);
                        //}
                    }
                }
            }

        }

        public async Task DeleteVtrUploadTask(int taskId)
        {
            if (taskId <= 0)
            {
                throw new Exception("TaskId is smaller than 0.");
            }
            
            var vtrUploadTasks = await TaskStore.GetVtrUploadTaskListAsync(a => a.Where(x => x.Taskid == taskId), true);
            int nCount = vtrUploadTasks.Count;

            if (nCount > 0)
            {
                //foreach (VtrUploadtask item in vtrUploadTasks)
                //{
                //    if (item.Taskstate == (int)VTRUPLOADTASKSTATE.VTR_UPLOAD_COMPLETE)
                //    {
                //        Logger.Info("In DeleteVtrUploadTask,Find a VTR_UPLOAD_COMPLETE task,not delete.taskId = " + taskId.ToString());
                //        return;
                //    }

                //    TaskAccess taskAccess = new TaskAccess();
                //    SelectTaskFactory taskSelector = new SimpleSelectTask(taskId);
                //    TaskSet taskSet = taskSelector.SelectTask();
                //    //TaskSet.DBP_TASKRow taskRow = taskSet.DBP_TASK.FindByTASKID(taskId);
                //    foreach (TaskSet.DBP_TASKRow taskRow in taskSet.DBP_TASK.Rows)
                //    {
                //        if (row.TASKSTATE == (int)VTRUPLOADTASKSTATE.VTR_UPLOAD_TEMPSAVE ||
                //            row.TASKSTATE == (int)VTRUPLOADTASKSTATE.VTR_UPLOAD_FAIL ||
                //            row.TASKSTATE == (int)VTRUPLOADTASKSTATE.VTR_UPLOAD_DELETE ||
                //            row.TASKSTATE == (int)VTRUPLOADTASKSTATE.VTR_UPLOAD_COMMIT)
                //        {
                //            DBACCESS.DeleteVtrUploadTask(taskId);
                //            taskAccess.DeleteTaskFromDB(taskId, true);
                //            ApplicationLog.WriteInfo(string.Format("In DeleteVtrUploadTask,Find a {0} task,delete from table.taskId  = {1}", (VTRUPLOADTASKSTATE)row.TASKSTATE, taskId));
                //            continue;
                //        }

                //        /* zmj 2010-07-08 将提交状态的任务也直接删除素材
                //        if (row.TASKSTATE == (int)VTRUPLOADTASKSTATE.VTR_UPLOAD_COMMIT)
                //        {
                //            row.TASKSTATE = (int)VTRUPLOADTASKSTATE.VTR_UPLOAD_DELETE;

                //            taskRow.STATE = (int)taskState.tsDelete;
                //            taskRow.OP_TYPE = (int)opType.otDel;
                //            taskRow.DISPATCH_STATE = (int)dispatchState.dpsDispatchFailed;
                //        }*/

                //        if (row.TASKSTATE == (int)VTRUPLOADTASKSTATE.VTR_UPLOAD_PRE_EXECUTE)
                //        {
                //            ApplicationLog.WriteInfo(string.Format("In DeleteVtrUploadTask,Find a {0} task,set delete state.taskId  = {1}"
                //                , (VTRUPLOADTASKSTATE)row.TASKSTATE, taskId));

                //            row.TASKSTATE = (int)VTRUPLOADTASKSTATE.VTR_UPLOAD_DELETE;
                //            taskRow.STATE = (int)taskState.tsDelete;
                //            taskRow.OP_TYPE = (int)opType.otDel;
                //            taskRow.DISPATCH_STATE = (int)dispatchState.dpsDispatchFailed;

                //            continue;
                //        }

                //        if (row.TASKSTATE == (int)VTRUPLOADTASKSTATE.VTR_UPLOAD_EXECUTE)
                //        {
                //            ApplicationLog.WriteInfo(string.Format("In DeleteVtrUploadTask,Find a {0} task,set delete state.taskId  = {1}"
                //                , (VTRUPLOADTASKSTATE)row.TASKSTATE, taskId));

                //            row.TASKSTATE = (int)VTRUPLOADTASKSTATE.VTR_UPLOAD_DELETE;
                //            //vtr上载服务，相应把素材删除
                //            taskRow.SYNC_STATE = (int)syncState.ssNot;
                //            taskRow.OP_TYPE = (int)opType.otDel;
                //            taskRow.DISPATCH_STATE = (int)dispatchState.dpsInvalid;
                //            taskRow.STATE = (int)taskState.tsInvaild;
                //            taskRow.ENDTIME = DateTime.Now;
                //            taskRow.NEW_ENDTIME = DateTime.Now;

                //            continue;
                //        }
                //    }

                //    taskAccess.UpdateTasks(ref taskSet);
                //}
                
                //UpdateTime = DateTime.Now;
                //DBACCESS.UpdateVtrSet(ref vtrSet);
            }
        }

        #endregion

    }
}
