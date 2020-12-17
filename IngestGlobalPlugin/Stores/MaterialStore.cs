using IngestGlobalPlugin.Dto;
using IngestGlobalPlugin.Dto.OldResponse;
using IngestGlobalPlugin.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IngestGlobalPlugin.Stores
{
    public class MaterialStore : IMaterialStore
    {
        protected IngestGlobalDBContext Context { get; }

        public MaterialStore(IngestGlobalDBContext baseDataDbContext)
        {
            Context = baseDataDbContext;
        }
        #region Base
        public Task<int> DbContextSaveChange()
        {
            return Context.SaveChangesAsync();
        }
        public Task<List<TResult>> QueryList<TModel, TResult>(Func<IQueryable<TModel>, IQueryable<TResult>> query, bool notrack = false)
            where TModel : class
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            if (notrack)
            {
                return query.Invoke(Context.Set<TModel>().AsNoTracking()).ToListAsync();
            }
            return query.Invoke(Context.Set<TModel>()).ToListAsync();
        }
        public Task<TResult> QueryModel<TModel, TResult>(Func<IQueryable<TModel>, Task<TResult>> query, bool notrack = false)
            where TModel : class
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            if (notrack)
            {
                return query.Invoke(Context.Set<TModel>().AsNoTracking());
            }
            return query.Invoke(Context.Set<TModel>());
        }
        #endregion

        public async Task<bool> AddMQMsg(DbpMsmqmsg info)
        {
            if (!Context.DbpMsmqmsg.Any(a => a.Msgid == info.Msgid))
            {
                info.Nextretry = DateTime.Now;
                Context.DbpMsmqmsg.Add(info);

                try
                {
                    await Context.SaveChangesAsync();
                }
                catch (Exception)
                {

                    throw;
                }
            }

            return true;
        }

        public async Task<bool> DeleteMQMsgByTaskId(string taskid)
        {
            string queryStrId = $"<TaskID>{taskid}</TaskID>";
            var result = await Context.DbpMsmqmsg.Where(x => x.Msgcontent.Contains(queryStrId)).ToListAsync();
            if (result != null && result.Count > 0)
            {
                Context.DbpMsmqmsg.RemoveRange(result);

                try
                {
                    await Context.SaveChangesAsync();
                }
                catch (Exception)
                {
                    throw;
                }
            }
            return true;
        }

        public async Task<TResult> GetMqMsgAsync<TResult>(Func<IQueryable<DbpMsmqmsg>, IQueryable<TResult>> query, bool notrack = false)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            if (notrack)
            {
                return await query.Invoke(Context.DbpMsmqmsg.AsNoTracking()).FirstOrDefaultAsync();
            }
            return await query.Invoke(Context.DbpMsmqmsg).FirstOrDefaultAsync();
        }

        public Task<TResult> GetFormateInfoAsync<TResult>(Func<IQueryable<DbpFileformatinfo>, IQueryable<TResult>> query, bool notrack = false)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            if (notrack)
            {
                return query.Invoke(Context.DbpFileformatinfo.AsNoTracking()).FirstOrDefaultAsync();
            }
            return query.Invoke(Context.DbpFileformatinfo).FirstOrDefaultAsync();
        }

        public async Task<TResult> GetMsgFailedRecordAsync<TResult>(Func<IQueryable<DbpMsgFailedrecord>, IQueryable<TResult>> query, bool notrack = false)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            if (notrack)
            {
                return await query.Invoke(Context.DbpMsgFailedrecord.AsNoTracking()).FirstOrDefaultAsync();
            }
            return await query.Invoke(Context.DbpMsgFailedrecord).FirstOrDefaultAsync();
        }

        public async Task<TResult> GetMaterialArchiveAsync<TResult>(Func<IQueryable<DbpMaterialArchive>, IQueryable<TResult>> query, bool notrack = false)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            if (notrack)
            {
                return await query.Invoke(Context.DbpMaterialArchive.AsNoTracking()).FirstOrDefaultAsync();
            }
            return await query.Invoke(Context.DbpMaterialArchive).FirstOrDefaultAsync();
        }

        public async Task<TResult> GetMaterialAsync<TResult>(Func<IQueryable<DbpMaterial>, IQueryable<TResult>> query, bool notrack = false)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            if (notrack)
            {
                return await query.Invoke(Context.DbpMaterial.AsNoTracking()).FirstOrDefaultAsync();
            }
            return await query.Invoke(Context.DbpMaterial).FirstOrDefaultAsync();
        }
        public Task<List<TResult>> GetMaterialArchiveListAsync<TResult>(Func<IQueryable<DbpMaterialArchive>, IQueryable<TResult>> query, bool notrack = false)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            if (notrack)
            {
                return query.Invoke(Context.DbpMaterialArchive.AsNoTracking()).ToListAsync();
            }
            return query.Invoke(Context.DbpMaterialArchive).ToListAsync();
        }
        public Task<List<TResult>> GetMaterialListAsync<TResult>(Func<IQueryable<DbpMaterial>, IQueryable<TResult>> query, bool notrack = false)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            if (notrack)
            {
                return query.Invoke(Context.DbpMaterial.AsNoTracking()).ToListAsync();
            }
            return query.Invoke(Context.DbpMaterial).ToListAsync();
        }

        public Task<List<TResult>> GetMsgFailedRecordListAsync<TResult>(Func<IQueryable<DbpMsgFailedrecord>, IQueryable<TResult>> query, bool notrack = false)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            if (notrack)
            {
                return query.Invoke(Context.DbpMsgFailedrecord.AsNoTracking()).ToListAsync();
            }
            return query.Invoke(Context.DbpMsgFailedrecord).ToListAsync();
        }

        public Task<List<DbpMsmqmsg>> GetNeedProcessMsg(int statu, DateTime dtnext)
        {
            return Context.DbpMsmqmsg.AsNoTracking().Where(a => a.Msgstatus == statu && a.Nextretry < dtnext).ToListAsync();
        }

        public Task<List<FailedMessageParam>> GetMsgContentByTaskid(int taskid)
        {
            return (from mq in Context.DbpMsgFailedrecord.AsNoTracking()
                    join fail in Context.DbpMsmqmsg.AsNoTracking() on mq.MsgGuid equals fail.Msgid into ps
                    from p in ps.DefaultIfEmpty()
                    where mq.TaskId == taskid
                    select new FailedMessageParam
                    {
                        TaskID = mq.TaskId,
                        SectionID = mq.SectionId,
                        MsgContent = p != null ? p.Msgcontent : string.Empty
                    }).ToListAsync();
        }

        public Task<int> CountFailedRecordTask(int taskid)
        {
            return Context.DbpMsgFailedrecord.AsNoTracking().CountAsync(a => a.TaskId == taskid);
        }

        public Task<int> CountFailedRecordTaskAndSection(int taskid, int section)
        {
            return Context.DbpMsgFailedrecord.AsNoTracking().CountAsync(a => a.TaskId == taskid && a.SectionId != section);
        }

        public async Task AddMaterial(DbpMaterial pin, bool savechange)
        {
            Context.DbpMaterial.Add(pin);

            if (savechange)
            {
                try
                {
                    await Context.SaveChangesAsync();
                }
                catch (Exception e)
                {

                    throw e;
                }
            }

        }

        public async Task SaveChangeAsync()
        {
            try
            {
                await Context.SaveChangesAsync();
            }
            catch (Exception e)
            {

                throw e;
            }
        }

        public async Task UpdateMaterialArchive(DbpMaterialArchive pin, bool savechange)
        {
            var item = await Context.DbpMaterialArchive.AsNoTracking().Where(a => a.Materialid == pin.Materialid && a.Policyid == pin.Policyid).FirstOrDefaultAsync();

            if (item == null)
            {
                Context.DbpMaterialArchive.Add(pin);
            }
            else
            {
                Context.DbpMaterialArchive.Update(pin);
            }

            if (savechange)
            {
                try
                {
                    await Context.SaveChangesAsync();
                }
                catch (Exception e)
                {

                    throw e;
                }
            }
        }

        public async Task AddMsgFailedRecord(DbpMsgFailedrecord pin)
        {
            if (!Context.DbpMsgFailedrecord.AsNoTracking().Any(x => x.MsgGuid == pin.MsgGuid))
            {
                Context.DbpMsgFailedrecord.Add(pin);

                try
                {
                    await Context.SaveChangesAsync();
                }
                catch (Exception e)
                {

                    throw e;
                }
            }
        }


        public async Task RemoveMsgFailedRecord(int taskid, int sectionid)
        {
            var f = await Context.DbpMsgFailedrecord.AsNoTracking().Where(a => a.TaskId == taskid && a.SectionId == sectionid).SingleOrDefaultAsync();

            if (f != null)
            {
                Context.DbpMsgFailedrecord.Remove(f);

                try
                {
                    await Context.SaveChangesAsync();
                }
                catch (Exception e)
                {

                    throw e;
                }
            }
        }

        public async Task UpdateFormateInfo(DbpFileformatinfo file)
        {
            var item = await Context.DbpFileformatinfo
                .Where(x => x.Key == file.Key)
                .SingleOrDefaultAsync();
            if (item == null)
            {
                Context.DbpFileformatinfo.Add(file);
            }
            else
            {
                item.Formatinfo = file.Formatinfo;
            }

            try
            {
                await Context.SaveChangesAsync();
            }
            catch (Exception e)
            {

                throw e;
            }
        }
        public async Task<bool> UpdateMqMsgStatusAsync(string msgId, int nActionID, MqmsgStatus msgStatus, int nFailedCount)
        {
            DateTime dtRetry = DateTime.Now;
            if (msgStatus == MqmsgStatus.ProcessFailed)
            {
                int nSpan = nFailedCount;
                if (nFailedCount > 3)
                {
                    nSpan = (int)Math.Pow(2.0, nFailedCount);
                }
                TimeSpan sp = new TimeSpan(0, 0, nSpan);
                dtRetry = dtRetry + sp;
            }
            if (nActionID < 0)
            {
                var mqMsg = await Context.DbpMsmqmsg.FindAsync(msgId);
                if (mqMsg != null)
                {
                    mqMsg.Msgstatus = (int)msgStatus;
                    mqMsg.Msgprocesstime = DateTime.Now;
                    mqMsg.Nextretry = dtRetry;
                    mqMsg.Failedcount = nFailedCount;
                    mqMsg.Lockdata = msgStatus != MqmsgStatus.Processing ? "" : mqMsg.Lockdata;
                    return await Context.SaveChangesAsync() > 0;
                }
                return false;
            }
            else
            {
                var mqMsg = await Context.DbpMsmqmsgFailed.SingleOrDefaultAsync(a => a.Msgid == msgId && a.Actionid == nActionID);
                if (mqMsg != null)
                {
                    mqMsg.Msgstatus = (int)msgStatus;
                    mqMsg.Msgprocesstime = DateTime.Now;
                    mqMsg.Nextretry = dtRetry;
                    mqMsg.Failedcount = nFailedCount;
                    mqMsg.Lockdata = msgStatus != MqmsgStatus.Processing ? "" : mqMsg.Lockdata;
                    return await Context.SaveChangesAsync() > 0;
                }
                return false;
            }
        }

        public async Task<bool> DeleteMqMsgStatusAsync(DateTime dt)
        {
            TimeSpan span = new TimeSpan(2, 0, 0);
            dt = dt - span;

            var list = Context.DbpMsmqmsg.Where(a => (a.Msgstatus == 2 && a.Msgprocesstime < dt)
                                                  || (a.Msgstatus == 3 && a.Failedcount > 20));
            Context.DbpMsmqmsg.RemoveRange(list);

            return await Context.SaveChangesAsync() > 0;
        }

        #region DbpMaterial
        public Task<List<TResult>> GetMaterial<TResult>(Func<IQueryable<DbpMaterial>, IQueryable<TResult>> query, bool notrack = false)
        {
            return QueryList(query, notrack);
        }
        public Task<TResult> GetMaterial<TResult>(Func<IQueryable<DbpMaterial>, Task<TResult>> query, bool notrack = false)
        {
            return QueryModel(query, notrack);
        }
        public async Task<bool> AddMaterial(DbpMaterial material)
        {
            if (material != null)
            {
                await Context.AddAsync(material);
                return await Context.SaveChangesAsync() > 0;
            }
            return false;
        }
        #endregion

        public async Task<bool> UpdateArchiveAndClipState(int materialId)
        {
            using (var tran = await Context.Database.BeginTransactionAsync())
            {
                var material = await Context.DbpMaterial.FindAsync(materialId);
                material.Clipstate = 1;
                if (await Context.SaveChangesAsync() <= 0)
                {
                    tran.Rollback();
                }
                var archive = await Context.DbpMaterialArchive.Where(a => a.Materialid == materialId).ToListAsync();
                archive.ForEach(a => a.Archivestate = 2);
                if (await Context.SaveChangesAsync() <= 0)
                {
                    tran.Rollback();
                }
                tran.Commit();
                return true;
            }
        }

        #region DbpMaterialArchive
        public async Task<List<TResult>> GetMaterialArchive<TResult>(Func<IQueryable<DbpMaterialArchive>, IQueryable<TResult>> query, bool notrack = false)
        {
            return await QueryList(query, notrack);
        }
        public Task<TResult> GetMaterialArchive<TResult>(Func<IQueryable<DbpMaterialArchive>, Task<TResult>> query, bool notrack = false)
        {
            return QueryModel(query, notrack);
        }
        public async Task<bool> AddOrUpdateMaterialArchive(List<DbpMaterialArchive> materialArchives, bool savechange = true)
        {
            var materialIds = materialArchives.Select(a => a.Materialid).ToList();
            var policyIds = materialArchives.Select(a => a.Policyid).ToList();
            var updateArchives = await Context.DbpMaterialArchive.Where(a => materialIds.Contains(a.Materialid) && policyIds.Contains(a.Policyid)).ToListAsync();
            foreach (var archive in updateArchives)
            {
                var material = materialArchives.FirstOrDefault(a => a.Policyid == archive.Policyid && a.Materialid == archive.Materialid);
                if (material != null)
                {
                    archive.Archivestate = material.Archivestate;
                    archive.Lastresult = material.Lastresult;
                    archive.Isprocessing = material.Isprocessing;
                    archive.Failedtimes = material.Failedtimes;
                    archive.Nextretry = material.Nextretry;
                    archive.Lastupdatetime = material.Lastupdatetime;
                    archive.Archiveresult = material.Archiveresult;
                }
                materialArchives.Remove(material);
            }
            if (materialArchives != null && materialArchives.Count > 0)
            {
                await Context.DbpMaterialArchive.AddRangeAsync(materialArchives);
            }
            if (savechange)
            {
                return await Context.SaveChangesAsync() > 0;
            }
            return true;
        }
        #endregion

        #region DbpMaterialAudio
        public async Task<List<TResult>> GetMaterialAudioListAsync<TResult>(Func<IQueryable<DbpMaterialAudio>, IQueryable<TResult>> query, bool notrack = false)
        {
            return await QueryList(query, notrack);
        }
        public async Task<TResult> GetMaterialAudioAsync<TResult>(Func<IQueryable<DbpMaterialAudio>, Task<TResult>> query, bool notrack = false)
        {
            return await QueryModel(query, notrack);
        }
        public async Task<bool> AddMaterialAudio(int materialId, List<AudioInfo> audioInfos, bool savechange = true)
        {
            if (audioInfos != null && audioInfos.Count > 0)
            {
                var audios = audioInfos.Select(a => new DbpMaterialAudio
                {
                    Audiofilename = a.strFilename,
                    Audiosource = a.nAudioSource,
                    Audiotypeid = a.nAudioTypeID,
                    Materialid = materialId
                });
                await Context.DbpMaterialAudio.AddRangeAsync(audios);
                if (savechange)
                {
                    return await Context.SaveChangesAsync() > 0;
                }
                return true;

            }
            return false;
        }
        public async Task<bool> UpdateMaterialAudio(int materialId, List<AudioInfo> audioInfos)
        {
            var list = await Context.DbpMaterialAudio.Where(a => a.Materialid == materialId).ToListAsync();
            if (list != null && list.Count > 0)
            {
                Context.DbpMaterialAudio.RemoveRange(list);
            }
            if (audioInfos != null && audioInfos.Count > 0)
            {
                var audios = audioInfos.Select(a => new DbpMaterialAudio
                {
                    Audiofilename = a.strFilename,
                    Audiosource = a.nAudioSource,
                    Audiotypeid = a.nAudioTypeID,
                    Materialid = materialId
                });
                await Context.DbpMaterialAudio.AddRangeAsync(audios);
                return await Context.SaveChangesAsync() > 0;
            }
            return false;
        }
        #endregion

        #region DbpMaterialVideo
        public async Task<List<TResult>> GetMaterialVideoListAsync<TResult>(Func<IQueryable<DbpMaterialVideo>, IQueryable<TResult>> query, bool notrack = false)
        {
            return await QueryList(query, notrack);
        }
        public async Task<TResult> GetMaterialVideoAsync<TResult>(Func<IQueryable<DbpMaterialVideo>, Task<TResult>> query, bool notrack = false)
        {
            return await QueryModel(query, notrack);
        }
        public async Task<bool> AddMaterialVideo(int materialId, List<VideoInfo> videoInfos, bool savechange = true)
        {
            if (videoInfos != null && videoInfos.Count > 0)
            {
                var videos = videoInfos.Select(a => new DbpMaterialVideo
                {
                    Videofilename = a.strFilename,
                    Videosource = a.nVideoSource,
                    Videotypeid = a.nVideoTypeID,
                    Materialid = materialId
                });
                await Context.DbpMaterialVideo.AddRangeAsync(videos);
                if (savechange)
                {
                    return await Context.SaveChangesAsync() > 0;
                }
                return true;

            }
            return false;
        }
        public async Task<bool> UpdateMaterialVideo(int materialId, List<VideoInfo> videoInfos)
        {
            var list = await Context.DbpMaterialVideo.Where(a => a.Materialid == materialId).ToListAsync();
            if (list != null && list.Count > 0)
            {
                Context.DbpMaterialVideo.RemoveRange(list);
            }
            if (videoInfos != null && videoInfos.Count > 0)
            {
                var videos = videoInfos.Select(a => new DbpMaterialVideo
                {
                    Videofilename = a.strFilename,
                    Videosource = a.nVideoSource,
                    Videotypeid = a.nVideoTypeID,
                    Materialid = materialId
                });
                await Context.DbpMaterialVideo.AddRangeAsync(videos);
                return await Context.SaveChangesAsync() > 0;
            }
            return false;
        }
        #endregion

        #region DbpMetadatapolicy
        public async Task<List<TResult>> GetMetadataPolicy<TResult>(Func<IQueryable<DbpMetadatapolicy>, IQueryable<TResult>> query, bool notrack = false)
        {
            return await QueryList(query, notrack);
        }
        public async Task<TResult> GetMetadataPolicy<TResult>(Func<IQueryable<DbpMetadatapolicy>, Task<TResult>> query, bool notrack = false)
        {
            return await QueryModel(query, notrack);
        }
        #endregion

        #region DbpPolicytask
        public Task<List<TResult>> GetPolicyTask<TResult>(Func<IQueryable<DbpPolicytask>, IQueryable<TResult>> query, bool notrack = false)
        {
            return QueryList(query, notrack);
        }
        public async Task<TResult> GetPolicyTask<TResult>(Func<IQueryable<DbpPolicytask>, Task<TResult>> query, bool notrack = false)
        {
            return await QueryModel(query, notrack);
        }



        #endregion

        public int GetNextValId(string value)
        {
            return Context.DbpMaterial.Select(x => IngestGlobalDBContext.next_val(value)).FirstOrDefault();
        }
    }
}
