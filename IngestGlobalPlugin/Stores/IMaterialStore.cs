using IngestGlobalPlugin.Dto;
using IngestGlobalPlugin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IngestGlobalPlugin.Stores
{
    public interface  IMaterialStore
    {
        Task<bool> AddMQMsg(DbpMsmqmsg info);
        Task<TResult> GetMqMsgAsync<TResult>(Func<IQueryable<DbpMsmqmsg>, IQueryable<TResult>> query, bool notrack = false);


        Task<List<TResult>> GetMaterialArchiveListAsync<TResult>(Func<IQueryable<DbpMaterialArchive>, IQueryable<TResult>> query, bool notrack = false);
        Task<List<TResult>> GetMaterialListAsync<TResult>(Func<IQueryable<DbpMaterial>, IQueryable<TResult>> query, bool notrack = false);


        Task<TResult> GetMaterialArchiveAsync<TResult>(Func<IQueryable<DbpMaterialArchive>, IQueryable<TResult>> query, bool notrack = false);
        Task<TResult> GetMaterialAsync<TResult>(Func<IQueryable<DbpMaterial>, IQueryable<TResult>> query, bool notrack = false);
        Task<TResult> GetFormateInfoAsync<TResult>(Func<IQueryable<DbpFileformatinfo>, IQueryable<TResult>> query, bool notrack = false);
        Task<TResult> GetMsgFailedRecordAsync<TResult>(Func<IQueryable<DbpMsgFailedrecord>, IQueryable<TResult>> query, bool notrack = false);
        Task<List<TResult>> GetMsgFailedRecordListAsync<TResult>(Func<IQueryable<DbpMsgFailedrecord>, IQueryable<TResult>> query, bool notrack = false);
        Task<List<DbpMsmqmsg>> GetNeedProcessMsg(int statu, DateTime dtnext);
        Task AddMsgFailedRecord(DbpMsgFailedrecord pin);
        Task AddMaterial(DbpMaterial pin, bool savechange);
        Task UpdateMaterialArchive(DbpMaterialArchive pin, bool savechange);


        Task RemoveMsgFailedRecord(int taskid, int sectionid);
        Task<int> CountFailedRecordTask(int taskid);
        Task<int> CountFailedRecordTaskAndSection(int taskid, int section);
        Task<List<FailedMessageParam>> GetMsgContentByTaskid(int taskid);
        Task UpdateFormateInfo(DbpFileformatinfo file);
        Task SaveChangeAsync();

    }
}
