using IngestGlobalPlugin.Dto;
using IngestGlobalPlugin.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IngestGlobalPlugin.Stores
{
    public class MaterialStore
    {
        protected IngestGlobalDBContext Context { get; }

        public MaterialStore(IngestGlobalDBContext baseDataDbContext)
        {
            Context = baseDataDbContext;
        }
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
        public async Task<TResult> GetFormateInfoAsync<TResult>(Func<IQueryable<DbpFileformatinfo>, IQueryable<TResult>> query, bool notrack = false)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            if (notrack)
            {
                return await query.Invoke(Context.DbpFileformatinfo.AsNoTracking()).FirstOrDefaultAsync();
            }
            return await query.Invoke(Context.DbpFileformatinfo).FirstOrDefaultAsync();
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
        public async Task<List<TResult>> GetMaterialArchiveListAsync<TResult>(Func<IQueryable<DbpMaterialArchive>, IQueryable<TResult>> query, bool notrack = false)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            if (notrack)
            {
                return await query.Invoke(Context.DbpMaterialArchive.AsNoTracking()).ToListAsync();
            }
            return await query.Invoke(Context.DbpMaterialArchive).ToListAsync();
        }
        public async Task<List<TResult>> GetMaterialListAsync<TResult>(Func<IQueryable<DbpMaterial>, IQueryable<TResult>> query, bool notrack = false)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            if (notrack)
            {
                return await query.Invoke(Context.DbpMaterial.AsNoTracking()).ToListAsync();
            }
            return await query.Invoke(Context.DbpMaterial).ToListAsync();
        }

        public async Task<List<TResult>> GetMsgFailedRecordListAsync<TResult>(Func<IQueryable<DbpMsgFailedrecord>, IQueryable<TResult>> query, bool notrack = false)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            if (notrack)
            {
                return await query.Invoke(Context.DbpMsgFailedrecord.AsNoTracking()).ToListAsync();
            }
            return await query.Invoke(Context.DbpMsgFailedrecord).ToListAsync();
        }

        public async Task<List<DbpMsmqmsg>> GetNeedProcessMsg(int statu, DateTime dtnext)
        {
            return await Context.DbpMsmqmsg.AsNoTracking().Where(a => a.Msgstatus == statu && a.Nextretry < dtnext).ToListAsync();
        }

        public async Task<List<FailedMessageParam>> GetMsgContentByTaskid(int taskid)
        {
            return await (from mq in Context.DbpMsgFailedrecord.AsNoTracking()
                               join fail in Context.DbpMsmqmsg.AsNoTracking() on mq.MsgGuid equals fail.Msgid  into ps
                               from p in ps.DefaultIfEmpty()
                               where mq.TaskId == taskid
                               select new FailedMessageParam
                               {
                                   TaskID = mq.TaskId,
                                   SectionID = mq.SectionId,
                                   MsgContent = p!=null? p.Msgcontent:string.Empty
                               }).ToListAsync();
        }

        public async Task<int> CountFailedRecordTask(int taskid)
        {
            return await Context.DbpMsgFailedrecord.AsNoTracking().CountAsync(a => a.TaskId == taskid);
        }

        public async Task<int> CountFailedRecordTaskAndSection(int taskid, int section)
        {
            return await Context.DbpMsgFailedrecord.AsNoTracking().CountAsync(a => a.TaskId == taskid&&a.SectionId != section);
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

        
        public async Task RemoveMsgFailedRecord(int taskid, int sectionid)
        {
            var f = await Context.DbpMsgFailedrecord.AsNoTracking().Where(a => a.TaskId == taskid && a.SectionId == sectionid).SingleOrDefaultAsync() ;

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
                .Where(x=> x.Key.Equals(file.Key, StringComparison.OrdinalIgnoreCase))
                .SingleOrDefaultAsync();
            if (item == null)
            {
                Context.DbpFileformatinfo.Add(file);
            }
            else
            {
                Context.DbpFileformatinfo.Update(file);
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
    }
}
