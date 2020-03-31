using IngestTaskPlugin.Dto;
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

        Task<TResult> GetTaskMetaDataAsync<TResult>(Func<IQueryable<DbpTaskMetadata>, IQueryable<TResult>> query, bool notrack = false);
        Task<List<TResult>> GetTaskListAsync<TResult>(Func<IQueryable<DbpTask>, IQueryable<TResult>> query, bool notrack = false);
        Task<TResult> GetTaskAsync<TResult>(Func<IQueryable<DbpTask>, IQueryable<TResult>> query, bool notrack = false);
        //Task<List<DbpTask>> GetCapturingTaskListAsync(List<int> lstchannel);
        Task SetVtrUploadTaskListState(List<int> lsttaskid, VTRUPLOADTASKSTATE vtrstate, string errinfo);
        Task SetVtrUploadTaskState(int taskid, VTRUPLOADTASKSTATE vtrstate, string errinfo);
        Task<int> StopCapturingChannelAsync(int Channel);
        Task<List<int>> StopCapturingListChannelAsync(List<int> lstChaneel);
        Task<int> DeleteCapturingChannelAsync(int Channel);
        Task<List<int>> DeleteCapturingListChannelAsync(List<int> lstChaneel);

        Task LockTask(int taskid);
        Task UnLockTask(int taskid);
    }
}
