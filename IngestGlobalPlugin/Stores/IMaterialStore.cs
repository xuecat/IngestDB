using IngestGlobalPlugin.Dto;
using IngestGlobalPlugin.Dto.Request;
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
        Task<int> DbContextSaveChange();
        Task<bool> AddMQMsg(DbpMsmqmsg info);
        Task<TResult> GetMqMsgAsync<TResult>(Func<IQueryable<DbpMsmqmsg>, IQueryable<TResult>> query, bool notrack = false);
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
        Task<bool> AddOrUpdateMaterialArchive(List<DbpMaterialArchive> materialArchives);
        #endregion
        #region DbpMaterialAudio
        Task<List<TResult>> GetMaterialAudio<TResult>(Func<IQueryable<DbpMaterialAudio>, IQueryable<TResult>> query, bool notrack = false);
        Task<TResult> GetMaterialAudio<TResult>(Func<IQueryable<DbpMaterialAudio>, Task<TResult>> query, bool notrack = false);
        Task<bool> AddMaterialAudio(int materialId, List<AudioInfo> audios);
        Task<bool> UpdateMaterialAudio(int materialId, List<AudioInfo> audios);
        #endregion
        #region DbpMaterialVideo
        Task<List<TResult>> GetMaterialVideo<TResult>(Func<IQueryable<DbpMaterialVideo>, IQueryable<TResult>> query, bool notrack = false);
        Task<TResult> GetMaterialVideo<TResult>(Func<IQueryable<DbpMaterialVideo>, Task<TResult>> query, bool notrack = false);
        Task<bool> AddMaterialVideo(int materialId, List<VideoInfo> videos);
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
        Task<List<TResult>> GetTask<TResult>(Func<IQueryable<DbpTask>, IQueryable<TResult>> query, bool notrack = false);
        Task<TResult> GetTask<TResult>(Func<IQueryable<DbpTask>, Task<TResult>> query, bool notrack = false);
        Task<bool> UpdateTaskBmp(List<(int taskId, string bmpPath)> taskBmps);
        #endregion
    }
}
