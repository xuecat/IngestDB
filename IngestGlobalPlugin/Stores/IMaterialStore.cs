using IngestGlobalPlugin.Dto;
using IngestGlobalPlugin.Dto.OldResponse;
using IngestGlobalPlugin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IngestGlobalPlugin.Stores
{
    public interface IMaterialStore
    {
        int GetNextValId(string value);
        Task<int> DbContextSaveChange();
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
        Task<bool> UpdateMqMsgStatusAsync(string msgId, int nActionID, MqmsgStatus msgStatus, int nFailedCount);
        Task<bool> DeleteMqMsgStatusAsync(DateTime dt);
        #region DbpMaterial
        Task<List<TResult>> GetMaterial<TResult>(Func<IQueryable<DbpMaterial>, IQueryable<TResult>> query, bool notrack = false);
        Task<TResult> GetMaterial<TResult>(Func<IQueryable<DbpMaterial>, Task<TResult>> query, bool notrack = false);
        Task<bool> AddMaterial(DbpMaterial material);
        #endregion
        Task<bool> UpdateArchiveAndClipState(int materialID);
        #region DbpMaterialArchive
        Task<List<TResult>> GetMaterialArchive<TResult>(Func<IQueryable<DbpMaterialArchive>, IQueryable<TResult>> query, bool notrack = false);
        Task<TResult> GetMaterialArchive<TResult>(Func<IQueryable<DbpMaterialArchive>, Task<TResult>> query, bool notrack = false);
        Task<bool> AddOrUpdateMaterialArchive(List<DbpMaterialArchive> materialArchives, bool savechange = true);
        #endregion
        #region DbpMaterialAudio
        Task<List<TResult>> GetMaterialAudioListAsync<TResult>(Func<IQueryable<DbpMaterialAudio>, IQueryable<TResult>> query, bool notrack = false);
        Task<TResult> GetMaterialAudioAsync<TResult>(Func<IQueryable<DbpMaterialAudio>, Task<TResult>> query, bool notrack = false);
        Task<bool> AddMaterialAudio(int materialId, List<AudioInfo> audios, bool savechange = true);
        Task<bool> UpdateMaterialAudio(int materialId, List<AudioInfo> audios);
        #endregion
        #region DbpMaterialVideo
        Task<List<TResult>> GetMaterialVideoListAsync<TResult>(Func<IQueryable<DbpMaterialVideo>, IQueryable<TResult>> query, bool notrack = false);
        Task<TResult> GetMaterialVideoAsync<TResult>(Func<IQueryable<DbpMaterialVideo>, Task<TResult>> query, bool notrack = false);
        Task<bool> AddMaterialVideo(int materialId, List<VideoInfo> videos, bool savechange = true);
        Task<bool> UpdateMaterialVideo(int materialId, List<VideoInfo> videos);
        #endregion
        #region DbpMetadatapolicy
        Task<List<TResult>> GetMetadataPolicy<TResult>(Func<IQueryable<DbpMetadatapolicy>, IQueryable<TResult>> query, bool notrack = false);
        Task<TResult> GetMetadataPolicy<TResult>(Func<IQueryable<DbpMetadatapolicy>, Task<TResult>> query, bool notrack = false);
        #endregion
        #region DbpPolicytask
        Task<List<TResult>> GetPolicyTask<TResult>(Func<IQueryable<DbpPolicytask>, IQueryable<TResult>> query, bool notrack = false);
        Task<TResult> GetPolicyTask<TResult>(Func<IQueryable<DbpPolicytask>, Task<TResult>> query, bool notrack = false);
        #endregion
        #region DbpTask
       
        #endregion
    }
}
