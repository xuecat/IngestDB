﻿using IngestTaskPlugin.Dto;
using IngestTaskPlugin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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
        //IQueryable<TaskInfo> SimpleQuery { get; }
        Task SaveChangeAsync();

        Task<List<DbpTask>> GetTaskListAsync(TaskCondition condition , bool Track, bool uselock);
        Task UpdateTaskListAsync(List<DbpTask> lst);

        Task<TResult> GetTaskMetaDataAsync<TResult>(Func<IQueryable<DbpTaskMetadata>, IQueryable<TResult>> query, bool notrack = false);
        Task<TResult> GetTaskCustomMetaDataAsync<TResult>(Func<IQueryable<DbpTaskCustommetadata>, IQueryable<TResult>> query, bool notrack = false);
        Task UpdateTaskMetaDataAsync(int taskid, MetaDataType type, string metadata);
        Task UpdateTaskCutomMetaDataAsync(int taskid, string metadata);
        Task<List<TResult>> GetTaskListAsync<TResult>(Func<IQueryable<DbpTask>, IQueryable<TResult>> query, bool notrack = false);
        Task<TResult> GetTaskAsync<TResult>(Func<IQueryable<DbpTask>, IQueryable<TResult>> query, bool notrack = false);
        Task<TResult> GetVtrUploadTaskAsync<TResult>(Func<IQueryable<VtrUploadtask>, IQueryable<TResult>> query, bool notrack = false);
        Task<TResult> GetTaskBackupAsync<TResult>(Func<IQueryable<DbpTaskBackup>, IQueryable<TResult>> query, bool notrack = false);
        Task<List<TimePeriod>> GetTimePeriodsByScheduleVBUTasks(int vtrid, int extaskid);
        //Task<List<DbpTask>> GetCapturingTaskListAsync(List<int> lstchannel);
        Task UpdateVtrUploadTaskListStateAsync(List<int> lsttaskid, VTRUPLOADTASKSTATE vtrstate, string errinfo, bool savechange = true);
        Task UpdateVtrUploadTaskStateAsync(int taskid, VTRUPLOADTASKSTATE vtrstate, string errinfo, bool savechange = true);
        //Task DeleteVtrUploadTaskListAsync(List<int> lsttaskid, DbpTask task, bool savechange = true);
        Task DeleteVtrUploadTaskAsync(int taskid, DbpTask task, bool savechange = true);
        Task<int> StopCapturingChannelAsync(int Channel);
        Task<List<int>> StopCapturingListChannelAsync(List<int> lstChaneel);
        Task<int> DeleteCapturingChannelAsync(int Channel);
        Task<List<int>> DeleteCapturingListChannelAsync(List<int> lstChaneel);
        Task<List<int>> GetFreeChannels(List<int> lst, DateTime begin, DateTime end);
        Task<List<int>> GetFreePerodiChannels(List<int> lst, int nTaskID, int nUnitID, int nSigID, int nChannelID, string Category, DateTime begin, DateTime end);
        Task<DbpTask> AddTaskWithPolicys(DbpTask task, bool bAddForInDB, TaskSource taskSrc, string CaptureMeta, string ContentMeta, string MatiralMeta, string PlanningMeta, int[] arrPolicys);
        Task<DbpTask> ModifyTask(DbpTask task, bool bPerodic2Next, string CaptureMeta, string ContentMeta, string MatiralMeta, string PlanningMeta);

        Task LockTask(int taskid);
        Task UnLockTask(int taskid);
    }
}
