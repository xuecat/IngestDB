using System;


namespace IngestTaskPlugin.Managers
{
    using IngestDBCore;
    using IngestDBCore.Tool;
    using IngestTaskPlugin.Dto.OldResponse;
    using IngestTaskPlugin.Dto.Response;
    using IngestTaskPlugin.Models;
    using IngestTaskPlugin.Stores;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml.Linq;

    using TaskInfoRequest = IngestTaskPlugin.Dto.Response.TaskInfoResponse;

    public partial class TaskManager
    {
        public async Task<DbpTask> AddReScheduleTaskSvr(int oldtaskid)
        {
            var item = await Store.GetTaskAsync(a => a.Where(b => b.Taskid == oldtaskid), true);
            if (item != null)
            {
                if (item.Endtime < DateTime.Now.AddSeconds(3))
                {
                    SobeyRecException.ThrowSelfNoParam("AddReScheduleTaskSvr ", GlobalDictionary.GLOBALDICT_CODE_TASK_END_TIME_IS_SMALLER_THAN_BEING_TIME, Logger, null);
                }

                item.Recunitid = 0;
                item.Taskname = item.Taskname + "_1";
                item.Description = string.Empty;
                item.Taskguid = Guid.NewGuid().ToString("N");
                item.Starttime = DateTime.Now.AddSeconds(5);
                if (item.Tasktype == (int)TaskType.TT_PERIODIC)
                {
                    item.Tasktype = (int)TaskType.TT_NORMAL;
                }

                if (item.Tasktype == (int)TaskType.TT_MANUTASK || item.Tasktype == (int)TaskType.TT_OPENEND || item.Tasktype == (int)TaskType.TT_OPENENDEX)
                {
                    item.Starttime = DateTime.Now.AddSeconds(5);
                    item.Endtime = item.Starttime;
                    item.State = (int)taskState.tsReady;
                }

                string CapatureMetaData = string.Empty, StoreMetaData = string.Empty, ContentMetaData = string.Empty, PlanMetaData = string.Empty, SplitMetaData = string.Empty;
                var lstmeta = await Store.GetTaskMetaDataListAsync(a => a.Where(b => b.Taskid == oldtaskid), true);
                foreach (var itm in lstmeta)
                {
                    if (itm.Metadatatype == (int)MetaDataType.emCapatureMetaData)
                    {
                        CapatureMetaData = itm.Metadatalong;
                    }
                    else if (itm.Metadatatype == (int)MetaDataType.emContentMetaData)
                    {
                        ContentMetaData = itm.Metadatalong;
                    }
                    else if (itm.Metadatatype == (int)MetaDataType.emStoreMetaData)
                    {
                        StoreMetaData = itm.Metadatalong;
                    }
                    else if (itm.Metadatatype == (int)MetaDataType.emPlanMetaData)
                    {
                        PlanMetaData = itm.Metadatalong;
                    }
                    else if (itm.Metadatatype == (int)MetaDataType.emSplitData)
                    {
                        SplitMetaData = itm.Metadatalong;
                    }
                }

                if (string.IsNullOrEmpty(StoreMetaData))
                {
                    var root = XDocument.Parse(StoreMetaData);
                    if (root == null)
                    {
                        Logger.Error("ConverTaskMaterialMetaString error");
                        return null;
                    }
                    var material = root.Element("MATERIAL");
                    if (material == null)
                    {
                        Logger.Error("ConverTaskMaterialMetaString error");
                        return null;
                    }

                    TaskMaterialMetaResponse ret = new TaskMaterialMetaResponse();
                    var title = material?.Element("TITLE");
                    if (title != null)
                    {
                        title.Value = item.Taskname;
                    }

                    StoreMetaData = root.ToString();
                }

                TaskInfoRequest addinfo = new TaskInfoRequest();
                addinfo.TaskContent = _mapper.Map<TaskContentResponse>(item);
                addinfo.BackUpTask = false;
                addinfo.TaskSource = await GetTaskSource(oldtaskid);
                var backinfo = await AddTaskWithPolicy(addinfo, false, CapatureMetaData, ContentMetaData, StoreMetaData, PlanMetaData, false);
                return backinfo;
            }
            else
            {
                SobeyRecException.ThrowSelfNoParam("AddReScheduleTaskSvr ", GlobalDictionary.GLOBALDICT_CODE_TASK_ID_DOES_NOT_EXIST, Logger, null);
            }
            return null;
        }

        public async Task<DbpTask> ReScheduleTaskChannel(int oldtaskid)
        {
            var item = await Store.GetTaskAsync(a => a.Where(b => b.Taskid == oldtaskid));
            if (item != null && item.Signalid > 0)
            {
                var matchlst = await GetMatchedChannelForSignal(item.Signalid.GetValueOrDefault(), item.Channelid.GetValueOrDefault(),
                    new CHSelCondition() { BackupCHSel = false}, TaskSource.emMSVUploadTask);

                if (matchlst != null && matchlst.Count > 0)
                {
                    Logger.Info("CHSelectForNormalTask matchcount {0}", matchlst.Count);

                    DateTime Begin = item.Starttime;
                    DateTime End = item.Endtime;
                    List<int> freeChannelIdList = await Store.GetFreeChannels(matchlst, oldtaskid, 0, Begin, End, true);

                    if (freeChannelIdList != null && freeChannelIdList.Count > 0)
                    {
                        Logger.Info("ReScheduleTaskChannel GetFreeChannels freeChannelIdList {0}", freeChannelIdList.Count);

                        item.Channelid = freeChannelIdList[0];
                        item.SyncState = (int)syncState.ssNot;
                        item.DispatchState = (int)dispatchState.dpsDispatched;

                        await Store.SaveChangeAsync(ITaskStore.VirtualContent);
                        return item;
                    }

                }
                else
                    Logger.Error("ReScheduleTaskChannel CHSelectForNormalTask matchcount error");
            }

            return null;
        }

        public async Task<List<TResult>> GetNeedSynTasks<TResult>()
        {
            return _mapper.Map<List<TResult>>(await Store.GetTaskListAsync(x => x.Where(y => y.State == (int)taskState.tsReady 
                                                                                             && DateTime.Now < y.Endtime
                                                                                             && y.OpType != (int)opType.otDel), true));
        }

    }
}
