namespace IngestTaskPlugin.Managers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper;
    using IngestDBCore;
    using IngestDBCore.Tool;
    using IngestTaskPlugin.Dto;
    using IngestTaskPlugin.Dto.Request;
    using IngestTaskPlugin.Dto.Response;
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
        public async Task<int> SetVTRUploadTaskInfo(VTRUploadTaskInfo uploadTaskInfo)
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
        public bool IsTimePeriodInVTRTimePeriods(TimePeriod tp, VTRTimePeriods vtrFreeTimePeriods)
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
        public async Task<VTRTimePeriods> GetFreeTimePeriodByVtrId(VTRTimePeriods vtrFreeTimePeriods, DateTime beginCheckTime, int exTaskId)
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
        public async Task<List<ChannelTimePeriods>> GetChannelsFreeTimePeriods(DateTime beginTime, List<int> channelIds, int exTaskId)
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
    }
}
