using IngestTaskPlugin.Dto;
using IngestTaskPlugin.Dto.OldResponse;
using IngestTaskPlugin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace IngestTaskPlugin.Stores
{
    class VtrComparer : IEqualityComparer<int>
    {
       
        public bool Equals(int x, int y)
        {
            bool ba = x == (int)TaskType.TT_VTRUPLOAD;
            bool bb = y == (int)TaskType.TT_VTRUPLOAD;

            return ba == bb;
        }
        
        public int GetHashCode(int obj)
        {
            return obj == (int)TaskType.TT_VTRUPLOAD ?1:0;
        }
    }
    
    public interface ITaskStore
    {
        public const int VirtualContent = 2;
        public const int DefaultContent = 4;

        //IQueryable<TaskInfo> SimpleQuery { get; }
        Task SaveChangeAsync(int content);
        int GetNextValId(string value);


        //Task<List<DbpTask>> GetTaskListNotrackAsync(TaskCondition condition , bool uselock, bool sharding);
        Task<List<TResult>> GetTaskListNotrackAsync<TResult>(Func<IQueryable<DbpTask>, IQueryable<TResult>> query, bool sharding);
        Task<TResult> GetTaskNotrackAsync<TResult>(Func<IQueryable<DbpTask>, IQueryable<TResult>> query, bool sharding);

        Task UpdateTaskAsync(DbpTask item, Expression<Func<DbpTask, object>> getUpdatePropertyNames, bool savechange);
        Task UpdateTaskListAsync(List<DbpTask> lst, bool savechange);

        Task<TResult> GetTaskMetaDataAsync<TResult>(Func<IQueryable<DbpTaskMetadata>, IQueryable<TResult>> query, bool sharding);
        Task<List<TResult>> GetTaskMetaDataListAsync<TResult>(Func<IQueryable<DbpTaskMetadata>, IQueryable<TResult>> query, bool sharding);

        Task UpdateTaskMetaDataAsync(DbpTaskMetadata item, Expression<Func<DbpTaskMetadata, object>> getUpdatePropertyNames, bool savechange);
        Task UpdateTaskMetaDataListAsync(List<DbpTaskMetadata> metadatas, bool savechange);

        Task<TResult> GetTaskCustomMetaDataAsync<TResult>(Func<IQueryable<DbpTaskCustommetadata>, IQueryable<TResult>> query, bool notrack = false);
        Task UpdateTaskCutomMetaDataAsync(int taskid, string metadata);
        Task<TResult> GetVtrUploadTaskAsync<TResult>(Func<IQueryable<VtrUploadtask>, IQueryable<TResult>> query, bool notrack = false);
        Task<List<TResult>> GetVtrUploadTaskListAsync<TResult>(Func<IQueryable<VtrUploadtask>, IQueryable<TResult>> query, bool notrack = false);
        Task<TResult> GetTaskBackupAsync<TResult>(Func<IQueryable<DbpTaskBackup>, IQueryable<TResult>> query, bool notrack = false);
        Task<List<TimePeriod>> GetTimePeriodsByScheduleVBUTasks(int vtrid, int extaskid);
        Task<List<DbpTask>> GetTaskListWithMode(int cut, DateTime day, TimeLineType timetype);

        Task<List<DbpTask>> GetNeedFinishTasks();
        Task<List<DbpTask>> GetNeedUnSynTasks();
        //Task<List<DbpTask>> GetCapturingTaskListAsync(List<int> lstchannel);
        Task SetTaskClassify(int taskid, string taskclassify, bool change);
        Task SetVTRUploadTaskState(int TaskId, VTRUPLOADTASKSTATE vtrTaskState, string errorContent, bool savechange);
        Task<bool> AdjustVtrUploadTasksByChannelId(int channelId, int taskId, DateTime dtCurTaskBegin);
        Task UpdateVtrUploadTaskListStateAsync(List<int> lsttaskid, VTRUPLOADTASKSTATE vtrstate, string errinfo, bool savechange = true);
        Task UpdateVtrUploadTaskStateAsync(int taskid, VTRUPLOADTASKSTATE vtrstate, string errinfo, bool savechange = true);
        //Task DeleteVtrUploadTaskListAsync(List<int> lsttaskid, DbpTask task, bool savechange = true);
        Task<DbpTask> DeleteVtrUploadTaskAsync(int taskid, DbpTask task, bool savechange = true);
        Task<DbpTask> StopTask(int taskid, DateTime dt);
        int StopTaskNoChange(DbpTask task, DateTime dt);
        Task<DbpTask> DeleteTask(int taskid);
        Task<int> DeleteTaskDB(int taskid, bool change);
        Task<int> StopCapturingChannelAsync(int Channel);
        Task<List<int>> StopCapturingListChannelAsync(List<int> lstChaneel);
        Task<int> DeleteCapturingChannelAsync(int Channel);
        Task<List<int>> DeleteCapturingListChannelAsync(List<int> lstChaneel);

        string GetConfictTaskInfo();

        Task<List<int>> GetFreeChannels(List<int> lst, int nTaskID,int backVtrId, DateTime begin, DateTime end, bool choosefilter = false);
        Task<List<int>> GetFreePerodiChannels(List<int> lst, int nTaskID, int nUnitID, int nSigID, int nChannelID, string Category, DateTime begin, DateTime end);
        Task<DbpTask> AddTaskWithPolicys(DbpTask task, bool bAddForInDB, string CaptureMeta, string ContentMeta, string MatiralMeta, string PlanningMeta, string SplitMeta, int[] arrPolicys);
        Task<DbpTask> ModifyTask(DbpTask task, bool bPerodic2Next, bool autoupdate, bool savechange, string CaptureMeta, string ContentMeta, string MatiralMeta, string PlanningMeta, string SplitMeta = "");

        Task UnLockAllTask();
        Task LockTask(int taskid);
        Task UnLockTask(int taskid);
        Task UnLockTask(DbpTask taskid, bool savechange);
        bool GetPerodicTaskNextExectueTime(DateTime tmBegin, DateTime tmEnd, string strPerodicDesc, ref DateTime tmExecuteBegin, ref DateTime tmExecuteEnd);
        List<DateTime> GetDateTimeFromString(string str);
        bool IsInvalidPerodicTask(string strClassify, DateTime begin);

        Task<int> ResetTaskErrorInfo(int taskid);
        Task<List<TResult>> GetTaskErrorInfoListAsync<TResult>(Func<IQueryable<DbpTaskErrorinfo>, IQueryable<TResult>> query, bool notrack = false);

        Task<bool> AddTaskErrorInfo(DbpTaskErrorinfo taskSource);


        Task<bool> AddPolicyTask(List<DbpPolicytask> policytasks);


        Task<bool> AddTask(DbpTask tasks, bool savechange);
        Task<bool> AddTaskList(List<DbpTask> tasks, bool savechange);
        Task<bool> AddPolicyTask(List<DbpPolicytask> policytasks, bool submitFlag);
        
        
        Task<bool> UpdateTaskBmp(Dictionary<int, string> taskPmp);

        #region 3.0
        Task<List<DbpTask>> GetTaskListWithModeBySite(int cut, DateTime day, TimeLineType timetype, string site);

        #endregion

    }
}
