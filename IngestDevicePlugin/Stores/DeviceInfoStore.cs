using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IngestDevicePlugin.Dto;
using IngestDevicePlugin.Models;
using Microsoft.EntityFrameworkCore;
using ProgrammeInfoDto = IngestDevicePlugin.Dto.Response.ProgrammeInfoResponse;
using CaptureChannelInfoDto = IngestDevicePlugin.Dto.Response.CaptureChannelInfoResponse;
using IngestDBCore;
using IngestDevicePlugin.Dto.Enum;
using IngestDevicePlugin.Dto.Response;
using Sobey.Core.Log;
using emSignalSource = IngestDevicePlugin.Dto.Enum.emSignalSource;
using IngestDevicePlugin.Dto.OldResponse;

namespace IngestDevicePlugin.Stores
{
    public class DeviceInfoStore : IDeviceStore
    {
        public DeviceInfoStore(IngestDeviceDBContext baseDataDbContext)
        {
            Context = baseDataDbContext;
        }
        private readonly ILogger Logger = LoggerManager.GetLogger("DeviceInfo");
        protected IngestDeviceDBContext Context { get; }
        #region Base
        /// <summary> 查询任意表集合返回 </summary>
        protected virtual async Task<List<TResult>> QueryListAsync<TDbp, TResult>(DbSet<TDbp> contextSet, Func<IQueryable<TDbp>, IQueryable<TResult>> query, bool notrack = false)
            where TDbp : class
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            if (notrack)
            {
                return await query(contextSet.AsNoTracking()).ToListAsync();
            }
            return await query(contextSet).ToListAsync();
        }

        /// <summary> 查询任意值返回 </summary>
        protected virtual Task<TResult> QueryModelAsync<TDbp, TResult>(DbSet<TDbp> contextSet, Func<IQueryable<TDbp>, Task<TResult>> query, bool notrack = false)
             where TDbp : class
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            if (notrack)
            {
                return query(contextSet.AsNoTracking());
            }
            return query(contextSet);
        }
        #endregion

        #region MyRegion

        public async Task<int> SaveIpVirtualchannelAsync(IEnumerable<DbpIpVirtualchannel> models)
        {
            var modelIds = models.Select(a => a.Channelid);
            var updateIds = Context.DbpIpVirtualchannel.Where(a => modelIds.Contains(a.Channelid)).Select(a => a.Channelid);
            var updateList = models.Where(a => updateIds.Contains(a.Channelid));
            var addList = models.Where(a => !updateIds.Contains(a.Channelid));
            Context.DbpIpVirtualchannel.UpdateRange(updateList);
            await Context.DbpIpVirtualchannel.AddRangeAsync(addList);
            return await Context.SaveChangesAsync();
        }

        public async Task<int> SaveIpProgrammeAsync(IEnumerable<DbpIpProgramme> models)
        {
            var modelIds = models.Select(a => a.Programmeid);
            var updateIds = Context.DbpIpProgramme.Where(a => modelIds.Contains(a.Programmeid)).Select(a => a.Programmeid);
            var updateList = models.Where(a => updateIds.Contains(a.Programmeid));
            var addList = models.Where(a => !updateIds.Contains(a.Programmeid));
            Context.DbpIpProgramme.UpdateRange(updateList);
            await Context.DbpIpProgramme.AddRangeAsync(addList);
            return await Context.SaveChangesAsync();
        }

        public async Task<int> SaveIpDatachannelinfoAsync(IEnumerable<DbpIpDatachannelinfo> models)
        {
            var modelIds = models.Select(a => a.Datachannelid);
            var updateIds = Context.DbpIpDatachannelinfo.Where(a => modelIds.Contains(a.Datachannelid)).Select(a => a.Datachannelid);
            var updateList = models.Where(a => updateIds.Contains(a.Datachannelid));
            var addList = models.Where(a => !updateIds.Contains(a.Datachannelid));
            Context.DbpIpDatachannelinfo.UpdateRange(updateList);
            await Context.DbpIpDatachannelinfo.AddRangeAsync(addList);
            return await Context.SaveChangesAsync();
        }
        public async Task<int> GetBackUpSignalInfoByID(int srgid)
        {
            return await Context.DbpSignalsrcMasterbackup.AsNoTracking()
                .Where(a => a.Mastersignalsrcid == srgid)
                .Select(b => b.Signalsrcid).SingleOrDefaultAsync();
        }

        public async Task<int> SaveIpDeviceAsync(IEnumerable<DbpIpDevice> models)
        {
            var modelIds = models.Select(a => a.Deviceid);
            var updateIds = Context.DbpIpDevice.Where(a => modelIds.Contains(a.Deviceid)).Select(a => a.Deviceid);
            var updateList = models.Where(a => updateIds.Contains(a.Deviceid));
            var addList = models.Where(a => !updateIds.Contains(a.Deviceid));
            Context.DbpIpDevice.UpdateRange(updateList);
            await Context.DbpIpDevice.AddRangeAsync(addList);
            return await Context.SaveChangesAsync();
        }

        public Task<bool> HaveMatirxAsync()
        {
            return Context.DbpMatrixinfo.AsNoTracking().AnyAsync(a => a.Matrixid == 2 && a.Matrixtypeid != 6);//老版本NULL MATRIX是6，现在是2，难道老版本一直返回的是有矩阵？
        }
        
        public async Task<bool> TrueHaveMatirxAsync()
        {
            return await Context.DbpMatrixinfo.AsNoTracking().AnyAsync(a => a.Matrixid == 2 && a.Matrixtypeid != 2);//老版本NULL MATRIX是6，现在是2，难道老版本一直返回的是有矩阵？

        }
        
        public async Task<List<ProgrammeInfoDto>> GetAllProgrammeInfoAsync()
        {
            var query = await (from sig in Context.DbpSignalsrc.AsNoTracking()
                               join recin in Context.DbpRcdindesc.AsNoTracking() on sig.Signalsrcid equals recin.Signalsrcid into ps
                               join grp in Context.DbpSignalsrcgroupmap.AsNoTracking() on sig.Signalsrcid equals grp.Signalsrcid into pg
                               from p in ps.DefaultIfEmpty()
                               from g in pg.DefaultIfEmpty()
                               where p != null 
                               select new ProgrammeInfoDto
                               {
                                   ProgrammeId = sig.Signalsrcid,
                                   ProgrammeName = sig.Name,
                                   ProgrammeDesc = sig.Signaldesc,
                                   TypeId = sig.Signaltypeid,
                                   PgmType = ProgrammeType.PT_SDI,
                                   ImageType = (ImageType)sig.Imagetype,
                                   PureAudio = sig.Pureaudio ?? 0,
                                   SignalSourceType = p == null ? 0 : (emSignalSource)p.Signalsource.GetValueOrDefault(),
                                   GroupId = g == null ? 0 : g.Groupid,
                               }).ToListAsync();
            if (query == null)
            {
                return null;
            }
            foreach (var item in query)
            {
                switch (item.PgmType)
                {
                    case ProgrammeType.PT_SDI:
                        { }
                        break;//上面已经赋值了
                    case ProgrammeType.PT_IPTS:
                        {
                            item.SignalSourceType = emSignalSource.emIPTS;
                        }
                        break;
                    case ProgrammeType.PT_StreamMedia:
                        {
                            item.SignalSourceType = emSignalSource.emStreamMedia;
                        }
                        break;
                    default:
                        break;
                }
            }
            
            return query;
        }
        public async Task<List<ProgrammeInfoDto>> GetSignalInfoByListAsync(List<int> srcid)
        {
            var query = await (from sig in Context.DbpSignalsrc.AsNoTracking()
                               join recin in Context.DbpRcdindesc.AsNoTracking() on sig.Signalsrcid equals recin.Signalsrcid into ps
                               join grp in Context.DbpSignalsrcgroupmap.AsNoTracking() on sig.Signalsrcid equals grp.Signalsrcid into pg
                               from p in ps.DefaultIfEmpty()
                               from g in pg.DefaultIfEmpty()
                               where p != null && srcid.Contains(sig.Signalsrcid)
                               select new ProgrammeInfoDto
                               {
                                   ProgrammeId = sig.Signalsrcid,
                                   ProgrammeName = sig.Name,
                                   ProgrammeDesc = sig.Signaldesc,
                                   TypeId = sig.Signaltypeid,
                                   PgmType = ProgrammeType.PT_SDI,
                                   ImageType = (ImageType)sig.Imagetype,
                                   PureAudio = sig.Pureaudio ?? 0,
                                   SignalSourceType = p == null ? 0 : (emSignalSource)p.Signalsource.GetValueOrDefault(),
                                   GroupId = g == null ? 0 : g.Groupid,
                               }).ToListAsync();
            if (query == null)
            {
                Logger.Error("GetSignalInfoAsync query error" + srcid.ToString());
                return null;
            }

            foreach (var item in query)
            {
                switch (item.PgmType)
                {
                    case ProgrammeType.PT_SDI:
                        { }
                        break;//上面已经赋值了
                    case ProgrammeType.PT_IPTS:
                        {
                            item.SignalSourceType = emSignalSource.emIPTS;
                        }
                        break;
                    case ProgrammeType.PT_StreamMedia:
                        {
                            item.SignalSourceType = emSignalSource.emStreamMedia;
                        }
                        break;
                    default:
                        break;
                }
            }
            return query;
        }
        //老版本 GetProgrammeInfoById
        public async Task<ProgrammeInfoDto> GetSignalInfoAsync(int srcid)
        {
            //DBP_STREAMMEDIA DBP_IP_PROGRAMME 这些没有写，后面写吧

            var query = await (from sig in Context.DbpSignalsrc.AsNoTracking()
                               join recin in Context.DbpRcdindesc.AsNoTracking() on sig.Signalsrcid equals recin.Signalsrcid into ps
                               join grp in Context.DbpSignalsrcgroupmap.AsNoTracking() on sig.Signalsrcid equals grp.Signalsrcid into pg
                               from p in ps.DefaultIfEmpty()
                               from g in pg.DefaultIfEmpty()
                               where p!=null && sig.Signalsrcid == srcid
                               select new ProgrammeInfoDto
                               {
                                   ProgrammeId = sig.Signalsrcid,
                                   ProgrammeName = sig.Name,
                                   ProgrammeDesc = sig.Signaldesc,
                                   TypeId = sig.Signaltypeid,
                                   PgmType = ProgrammeType.PT_SDI,
                                   ImageType = (ImageType)sig.Imagetype,
                                   PureAudio = sig.Pureaudio ?? 0,
                                   SignalSourceType = p == null ? 0 : (emSignalSource)p.Signalsource.GetValueOrDefault(),
                                   GroupId = g == null ? 0 : g.Groupid,
                               }).SingleOrDefaultAsync();
            if (query == null)
            {
                Logger.Error("GetSignalInfoAsync query error" + srcid.ToString());
                return null;
            }

            switch (query.PgmType)
            {
                case ProgrammeType.PT_SDI:
                    { }
                    break;//上面已经赋值了
                case ProgrammeType.PT_IPTS:
                    {
                        query.SignalSourceType = emSignalSource.emIPTS;
                    }
                    break;
                case ProgrammeType.PT_StreamMedia:
                    {
                        query.SignalSourceType = emSignalSource.emStreamMedia;
                    }
                    break;
                default:
                    break;
            }
            return query;
        }
                
        public async Task<int> GetMatrixChannelBySignalAsync(int channelid)
        {
            return await Context.DbpRcdoutdesc.Where(rcdout => rcdout.Channelid == channelid)
                                        .Join(Context.DbpVirtualmatrixportstate.Where(matrix => matrix.State == 1),
                                              rcdout => rcdout.Recoutidx,
                                              matrix => matrix.Virtualoutport,
                                              (rcdout, matrix) => matrix.Virtualoutport)
                                        .Join(Context.DbpRcdindesc,
                                              outport => outport,
                                              rcdin => rcdin.Recinidx,
                                              (outport, rcdin) => rcdin.Signalsrcid)
                                        .SingleOrDefaultAsync();
            //return await (from src in Context.DbpRcdindesc
            //              join device in Context.DbpVirtualmatrixportstate on src.Recinidx equals device.Virtualinport into ps1
            //              join grp in Context.DbpRcdoutdesc on channelid equals grp.Channelid into ps3
            //              from p1 in ps1.DefaultIfEmpty()
            //              from p3 in ps3.DefaultIfEmpty()
            //              where p1.Virtualoutport == p3.Recoutidx && p1.State == 1
            //              select src.Signalsrcid).SingleOrDefaultAsync();
        }
                
        public Task<List<Channel2SignalSrcMap>> GetAllChannel2SignalSrcMapAsync()
        {
            return Context.DbpRcdoutdesc.Join(Context.DbpVirtualmatrixportstate.Where(matrix => matrix.State == 1),
                                                     rcdout => rcdout.Recoutidx,
                                                     matrix => matrix.Virtualoutport,
                                                     (rcdout, matrix) => new
                                                     {
                                                         matrix.Virtualoutport,
                                                         rcdout.Channelid,
                                                         matrix.State,
                                                         matrix.Lastoprtime
                                                     })
                                               .Join(Context.DbpRcdindesc,
                                                     join => join.Virtualoutport,
                                                     rcdin => rcdin.Recinidx,
                                                     (join, rcdin) => new Channel2SignalSrcMap
                                                     {
                                                         nSignalSrcID = rcdin.Signalsrcid,
                                                         nChannelID = join.Channelid,
                                                         state = (Channel2SignalSrc_State)join.State,
                                                         lastOperTime = join.Lastoprtime

                                                     })
                                               .ToListAsync();
        }

        public async Task<CaptureChannelInfoDto> GetCaptureChannelByIDAsync(int channelid)
        {
            var lst = await (from channel in Context.DbpCapturechannels
                             join device in Context.DbpCapturedevice on channel.Cpdeviceid equals device.Cpdeviceid into ps1
                             join grp in Context.DbpChannelgroupmap on channel.Cpdeviceid equals grp.Channelid into ps3
                             from p1 in ps1.DefaultIfEmpty()
                             from p3 in ps3.DefaultIfEmpty()
                             where ps1 !=null && channel.Channelid == channelid
                             select new CaptureChannelInfoDto
                             {
                                 Id = channel.Channelid,
                                 Name = channel.Channelname,
                                 Desc = channel.Channeldesc,
                                 CpDeviceId = channel.Cpdeviceid,
                                 ChannelIndex = channel.Channelindex ?? 0,
                                 DeviceTypeId = p1.Devicetypeid <= 0 ? (int)CaptureChannelType.emMsvChannel : p1.Devicetypeid,
                                 BackState = (emBackupFlag)channel.Backupflag,
                                 CpSignalType = channel.Cpsignaltype ?? 0,
                                 OrderCode = p1 != null ? p1.Ordercode.GetValueOrDefault() : -1,
                                 GroupId = p3 != null ? p3.Groupid : -1
                             }).SingleOrDefaultAsync();

            if (lst == null)
            {
                lst = await Context.DbpIpVirtualchannel.AsNoTracking().Where(x => x.Channelid == channelid).Select(x => new CaptureChannelInfoDto
                {
                    Id = x.Channelid,
                    Name = x.Channelname,
                    Desc = x.Ipaddress,
                    CpDeviceId = x.Deviceid,
                    ChannelIndex = x.Ctrlport ?? 0,
                    DeviceTypeId = x.Channeltype ?? (int)CaptureChannelType.emMsvChannel,
                    BackState = (emBackupFlag)x.Backuptype.GetValueOrDefault(),
                    CarrierId = x.Carrierid ?? 0,
                    OrderCode = x.Deviceindex ?? -1,
                    CpSignalType = x.Cpsignaltype ?? 0
                }).SingleOrDefaultAsync();
            }

            return lst;
        }

        //老接口GetAllCaptureChannels

        public async Task<List<CaptureChannelInfoDto>> GetAllCaptureChannelsAsync(int status)
        {
            //device按nOrderCode排升序
            //channel 按RECOUTIDX排序
            List<CaptureChannelInfoDto> lst = null;
            if (status == 0)
            {
                lst = await (from channel in Context.DbpCapturechannels
                             join device in Context.DbpCapturedevice on channel.Cpdeviceid equals device.Cpdeviceid into ps1
                             join recout in Context.DbpRcdoutdesc on channel.Channelid equals recout.Channelid into ps2
                             join grp in Context.DbpChannelgroupmap on channel.Cpdeviceid equals grp.Channelid into ps3
                             from p1 in ps1.DefaultIfEmpty()
                             from p2 in ps2.DefaultIfEmpty()
                             from p3 in ps3.DefaultIfEmpty()
                             where p2 != null
                             select new CaptureChannelInfoDto
                             {
                                 Id = channel.Channelid,
                                 Name = channel.Channelname,
                                 Desc = channel.Channeldesc,
                                 CpDeviceId = channel.Cpdeviceid,
                                 ChannelIndex = channel.Channelindex ?? 0,
                                 DeviceTypeId = p1.Devicetypeid <= 0 ? (int)CaptureChannelType.emMsvChannel : p1.Devicetypeid,
                                 BackState = (emBackupFlag)channel.Backupflag,
                                 CpSignalType = channel.Cpsignaltype ?? 0,
                                 OrderCode = p1 != null ? p1.Ordercode.GetValueOrDefault() : -1,
                                 GroupId = p3 != null ? p3.Groupid : -1
                             }).ToListAsync();
            }
            else
            {
                lst = await (from channel in Context.DbpCapturechannels
                             join device in Context.DbpCapturedevice on channel.Cpdeviceid equals device.Cpdeviceid into ps1
                             join recout in Context.DbpRcdoutdesc on channel.Channelid equals recout.Channelid into psc
                             join cstatu in Context.DbpMsvchannelState on channel.Channelid equals cstatu.Channelid into ps2
                             join grp in Context.DbpChannelgroupmap on channel.Cpdeviceid equals grp.Channelid into ps3
                             from p1 in ps1.DefaultIfEmpty()
                             from p2 in ps2.DefaultIfEmpty()
                             from p3 in ps3.DefaultIfEmpty()
                             where psc!=null && p2 != null && p2.Devstate != (int)Device_State.DISCONNECTTED && p2.Msvmode != (int)MSV_Mode.LOCAL
                             select new CaptureChannelInfoDto
                             {
                                 Id = channel.Channelid,
                                 Name = channel.Channelname,
                                 Desc = channel.Channeldesc,
                                 CpDeviceId = channel.Cpdeviceid,
                                 ChannelIndex = channel.Channelindex ?? 0,
                                 DeviceTypeId = p1.Devicetypeid <= 0 ? (int)CaptureChannelType.emMsvChannel : p1.Devicetypeid,
                                 BackState = (emBackupFlag)channel.Backupflag,
                                 CpSignalType = channel.Cpsignaltype ?? 0,
                                 OrderCode = p1 != null ? p1.Ordercode.GetValueOrDefault() : -1,
                                 GroupId = p3 != null ? p3.Groupid : -1
                             }).ToListAsync();
            }


            //DBP_IP_VIRTUALCHANNEL 现在应该是没有用了
            var iplst = await Context.DbpIpVirtualchannel.AsNoTracking().Select(x => new CaptureChannelInfoDto
            {
                Id = x.Channelid,
                Name = x.Channelname,
                Desc = x.Ipaddress,
                CpDeviceId = x.Deviceid,
                ChannelIndex = x.Ctrlport ?? 0,
                DeviceTypeId = x.Channeltype ?? (int)CaptureChannelType.emMsvChannel,
                BackState = (emBackupFlag)x.Backuptype.GetValueOrDefault(),
                CarrierId = x.Carrierid ?? 0,
                OrderCode = x.Deviceindex ?? -1,
                CpSignalType = x.Cpsignaltype ?? 0
            }).ToListAsync();

            if (lst == null || lst.Count < 1)
            {
                Logger.Error("GetAllCaptureChannelsAsync lst error");
                return null;
            }

            if (iplst != null && iplst.Count > 0)
            {
                lst.AddRange(iplst);
            }

            return lst.OrderBy(x => x.OrderCode).ToList();
        }

        public Task<List<DbpChannelRecmap>> GetAllChannelUnitMap()
        {
            return Context.DbpChannelRecmap.AsNoTracking().ToListAsync();
        }

        public Task<DbpChannelRecmap> GetChannelUnitMap(int channel)
        {
            return Context.DbpChannelRecmap.Where(x => x.Channelid == channel).SingleAsync();
        }

        #endregion

        public Task<List<TResult>> GetRcdindescAsync<TResult>(Func<IQueryable<DbpRcdindesc>, IQueryable<TResult>> query, bool notrack = false)
        {
            return QueryListAsync(Context.DbpRcdindesc, query, notrack);
        }
        public async Task<TResult> GetRcdindescAsync<TResult>(Func<IQueryable<DbpRcdindesc>, Task<TResult>> query, bool notrack = false)
        {
            return await QueryModelAsync(Context.DbpRcdindesc, query, notrack);
        }


        public async Task<List<TResult>> GetProgramparamMapAsync<TResult>(Func<IQueryable<DbpProgramparamMap>, IQueryable<TResult>> query, bool notrack = false)
        {
            return await QueryListAsync(Context.DbpProgramparamMap, query, notrack);
        }

        public Task<TResult> GetProgramparamMapAsync<TResult>(Func<IQueryable<DbpProgramparamMap>, Task<TResult>> query, bool notrack = false)
        {
            return QueryModelAsync(Context.DbpProgramparamMap, query, notrack);
        }


        public Task<List<TResult>> GetRcdoutdescAsync<TResult>(Func<IQueryable<DbpRcdoutdesc>, IQueryable<TResult>> query, bool notrack = false)
        {
            return QueryListAsync(Context.DbpRcdoutdesc, query, notrack);
        }

        public async Task<List<TResult>> GetSignalsrcMasterbackupAsync<TResult>(Func<IQueryable<DbpSignalsrcMasterbackup>, IQueryable<TResult>> query, bool notrack = false)
        {
            return await QueryListAsync(Context.DbpSignalsrcMasterbackup, query, notrack);
        }

        public Task<TResult> GetSignalsrcMasterbackupAsync<TResult>(Func<IQueryable<DbpSignalsrcMasterbackup>, Task<TResult>> query, bool notrack = false)
        {
            return QueryModelAsync(Context.DbpSignalsrcMasterbackup, query, notrack);
        }

        public Task<List<TResult>> GetSignalDeviceMapAsync<TResult>(Func<IQueryable<DbpSignalDeviceMap>, IQueryable<TResult>> query, bool notrack = false)
        {
            return QueryListAsync(Context.DbpSignalDeviceMap, query, notrack);
        }


        public Task<List<TResult>> GetSignalsrcAsync<TResult>(Func<IQueryable<DbpSignalsrc>, IQueryable<TResult>> query, bool notrack = false)
        {
            return QueryListAsync(Context.DbpSignalsrc, query, notrack);
        }
        public async Task<TResult> GetSignalsrcAsync<TResult>(Func<IQueryable<DbpSignalsrc>, Task<TResult>> query, bool notrack = false)
        {
            return await QueryModelAsync(Context.DbpSignalsrc, query, notrack);
        }


        public Task<List<DbpSignalsrc>> GetAllSignalsrcForRcdinAsync(bool notrack = false)
        {
            return QueryListAsync(Context.DbpSignalsrc,
                                        a => a.Join(Context.DbpRcdindesc.Select(rcdin => rcdin.Signalsrcid),
                                                    src => src.Signalsrcid,
                                                    rcdin => rcdin,
                                                    (src, rcdin) => src),
                                        notrack);
        }

        public async Task<List<TResult>> GetCapturechannelsAsync<TResult>(Func<IQueryable<DbpCapturechannels>, IQueryable<TResult>> query, bool notrack = false)
        {
            return await QueryListAsync(Context.DbpCapturechannels, query, notrack);
        }
        public Task<TResult> GetCapturechannelsAsync<TResult>(Func<IQueryable<DbpCapturechannels>, Task<TResult>> query, bool notrack = false)
        {
            return QueryModelAsync(Context.DbpCapturechannels, query, notrack);
        }


        public Task<List<TResult>> GetIpDatachannelsAsync<TResult>(Func<IQueryable<DbpIpDatachannelinfo>, IQueryable<TResult>> query, bool notrack = false)
        {
            return QueryListAsync(Context.DbpIpDatachannelinfo, query, notrack);
        }

        public async Task<TResult> GetIpDatachannelsAsync<TResult>(Func<IQueryable<DbpIpDatachannelinfo>, Task<TResult>> query, bool notrack = false)
        {
            return await QueryModelAsync(Context.DbpIpDatachannelinfo, query, notrack);
        }


        public Task<List<TResult>> GetCapturedeviceAsync<TResult>(Func<IQueryable<DbpCapturedevice>, IQueryable<TResult>> query, bool notrack = false)
        {
            return QueryListAsync(Context.DbpCapturedevice, query, notrack);
        }


        public Task<List<TResult>> GetIpVirtualchannelAsync<TResult>(Func<IQueryable<DbpIpVirtualchannel>, IQueryable<TResult>> query, bool notrack = false)
        {
            return QueryListAsync(Context.DbpIpVirtualchannel, query, notrack);
        }
        public Task<TResult> GetIpVirtualchannelAsync<TResult>(Func<IQueryable<DbpIpVirtualchannel>, Task<TResult>> query, bool notrack = false)
        {
            return QueryModelAsync(Context.DbpIpVirtualchannel, query, notrack);
        }


        public async Task<List<TResult>> GetChannelgroupmapAsync<TResult>(Func<IQueryable<DbpChannelgroupmap>, IQueryable<TResult>> query, bool notrack = false)
        {
            return await QueryListAsync(Context.DbpChannelgroupmap, query, notrack);
        }

        public async Task<List<int>> GetChannelIdsBySignalIdForNotMatrix(int signalid)
        {
            return await Context.DbpRcdindesc.Where(x => x.Signalsrcid == signalid).Join(Context.DbpRcdoutdesc,
                rcin => rcin.Recinidx,
                rcout => rcout.Recoutidx,
                (rcin, rcout) => rcout.Channelid).ToListAsync();
        }

        public Task<List<int>> GetSignalIdsByChannelIdForNotMatrix(int channelid)
        {
            return Context.DbpRcdoutdesc.Where(x => x.Channelid == channelid)
                                              .Join(Context.DbpRcdindesc,
                                                    rcout => rcout.Recoutidx,
                                                    rcin => rcin.Recinidx,
                                                    (rcout, rcin) => rcin.Signalsrcid)
                                              .ToListAsync();
        }

        public Task<TResult> GetChannelExtendDataAsync<TResult>(Func<IQueryable<DbpChnExtenddata>, Task<TResult>> query, bool notrack = false)
        {
            return QueryModelAsync(Context.DbpChnExtenddata, query, notrack);
        }

        public async Task<int> SaveChannelExtenddataAsync(int channelId, int type, string data)
        {
            var deviceMap = await Context.DbpChnExtenddata.FirstOrDefaultAsync(a => a.Channaelid == channelId && a.Datatype == type);
            if (deviceMap == null)
            {
                await Context.DbpChnExtenddata.AddAsync(new DbpChnExtenddata
                {
                    Channaelid = channelId,
                    Datatype = type,
                    Extenddata = data
                });
            }
            else
            {
                deviceMap.Extenddata = data;
            }
            return await Context.SaveChangesAsync();
        }


        public Task<List<TResult>> GetSignalSrcExsAsync<TResult>(Func<IQueryable<DbpSignalsrcMasterbackup>, IQueryable<TResult>> query, bool notrack = false)
        {
            return QueryListAsync(Context.DbpSignalsrcMasterbackup, query, notrack);
        }
        public Task<TResult> GetSignalSrcExsAsync<TResult>(Func<IQueryable<DbpSignalsrcMasterbackup>, Task<TResult>> query, bool notrack = false)
        {
            return QueryModelAsync(Context.DbpSignalsrcMasterbackup, query, notrack);
        }


        public async Task<List<TResult>> GetSignalsrcgroupmapAsync<TResult>(Func<IQueryable<DbpSignalsrcgroupmap>, IQueryable<TResult>> query, bool notrack = false)
        {
            return await QueryListAsync(Context.DbpSignalsrcgroupmap, query, notrack);
        }
        public async Task<TResult> GetSignalsrcgroupmapAsync<TResult>(Func<IQueryable<DbpSignalsrcgroupmap>, Task<TResult>> query, bool notrack = false)
        {
            return await QueryModelAsync(Context.DbpSignalsrcgroupmap, query, notrack);
        }


        public Task<TResult> GetSignalDeviceMapAsync<TResult>(Func<IQueryable<DbpSignalDeviceMap>, Task<TResult>> query, bool notrack = false)
        {
            return QueryModelAsync(Context.DbpSignalDeviceMap, query, notrack);
        }
        public async Task<int> SaveSignalDeviceMapAsync(DbpSignalDeviceMap model)
        {
            var deviceMap = await Context.DbpSignalDeviceMap.FindAsync(model.Signalsrcid);
            if (deviceMap == null)
            {
                await Context.DbpSignalDeviceMap.AddAsync(model);
            }
            else
            {
                deviceMap.Deviceid = model.Deviceid;
                deviceMap.Deviceoutportidx = model.Deviceid;
                deviceMap.Signalsource = model.Signalsource;
            }

            return await Context.SaveChangesAsync();
        }


        public Task<List<TResult>> GetMsvchannelStateAsync<TResult>(Func<IQueryable<DbpMsvchannelState>, IQueryable<TResult>> query, bool notrack = false)
        {
            return QueryListAsync(Context.DbpMsvchannelState, query, notrack);
        }
        public Task<TResult> GetMsvchannelStateAsync<TResult>(Func<IQueryable<DbpMsvchannelState>, Task<TResult>> query, bool notrack = false)
        {
            return QueryModelAsync(Context.DbpMsvchannelState, query, notrack);
        }


        public Task<List<TResult>> GetSignalGroupAsync<TResult>(Func<IQueryable<DbpSignalgroup>, IQueryable<TResult>> query, bool notrack = false)
        {
            return QueryListAsync(Context.DbpSignalgroup, query, notrack);
        }
        public async Task<TResult> GetSignalGroupAsync<TResult>(Func<IQueryable<DbpSignalgroup>, Task<TResult>> query, bool notrack = false)
        {
            return await QueryModelAsync(Context.DbpSignalgroup, query, notrack);
        }


        public Task<List<TResult>> GetGPIMapInfoByGPIIDAsync<TResult>(Func<IQueryable<DbpGpiMap>, IQueryable<TResult>> query, bool notrack = false)
        {
            return QueryListAsync(Context.DbpGpiMap, query, notrack);
        }
        public async Task<TResult> GetGPIMapInfoByGPIIDAsync<TResult>(Func<IQueryable<DbpGpiMap>, Task<TResult>> query, bool notrack = false)
        {
            return await QueryModelAsync(Context.DbpGpiMap, query, notrack);
        }



        public Task<List<TResult>> GetGPIInfoAsync<TResult>(Func<IQueryable<DbpGpiInfo>, IQueryable<TResult>> query, bool notrack = false)
        {
            return QueryListAsync(Context.DbpGpiInfo, query, notrack);
        }
        public async Task<TResult> GetGPIInfoAsync<TResult>(Func<IQueryable<DbpGpiInfo>, Task<TResult>> query, bool notrack = false)
        {
            return await QueryModelAsync(Context.DbpGpiInfo, query, notrack);
        }



        public Task<List<TResult>> GetIpDeviceAsync<TResult>(Func<IQueryable<DbpIpDevice>, IQueryable<TResult>> query, bool notrack = false)
        {
            return QueryListAsync(Context.DbpIpDevice, query, notrack);
        }

        public async Task<TResult> GetIpDeviceAsync<TResult>(Func<IQueryable<DbpIpDevice>, Task<TResult>> query, bool notrack = false)
        {
            return await QueryModelAsync(Context.DbpIpDevice, query, notrack);
        }


        public Task<List<TResult>> GetIpProgrammeAsync<TResult>(Func<IQueryable<DbpIpProgramme>, IQueryable<TResult>> query, bool notrack = false)
        {
            return QueryListAsync(Context.DbpIpProgramme, query, notrack);
        }
        public async Task<TResult> GetIpProgrammeAsync<TResult>(Func<IQueryable<DbpIpProgramme>, Task<TResult>> query, bool notrack = false)
        {
            return await QueryModelAsync(Context.DbpIpProgramme, query, notrack);
        }


        public Task<List<TResult>> GetStreamMediaAsync<TResult>(Func<IQueryable<DbpStreammedia>, IQueryable<TResult>> query, bool notrack = false)
        {
            return QueryListAsync(Context.DbpStreammedia, query, notrack);
        }
        public async Task<TResult> GetStreamMediaAsync<TResult>(Func<IQueryable<DbpStreammedia>, Task<TResult>> query, bool notrack = false)
        {
            return await QueryModelAsync(Context.DbpStreammedia, query, notrack);
        }


        public Task<List<SignalGroupStateResponse>> GetAllSignalGroupInfoAsync()
        {
            return Context.DbpSignalsrcgroupmap.AsNoTracking().Join(Context.DbpSignalgroup.AsNoTracking(),
                                                     map => map.Groupid,
                                                     group => group.Groupid,
                                                     (map, group) => new SignalGroupStateResponse
                                                     {
                                                         SignalSrcId = map.Signalsrcid,
                                                         GroupId = group.Groupid,
                                                         GroupName = group.Groupname,
                                                         GroupDesc = group.Groupdesc
                                                     }).ToListAsync();
        }


        public Task<int?> GetParamTypeByChannelIDAsync(int channelID)
        {
            return Context.DbpRcdoutdesc.AsNoTracking().Where(rcdout => rcdout.Channelid == channelID)
                                               .Join(Context.DbpVirtualmatrixportstate.AsNoTracking().Where(state => state.State == 1),
                                                     rcdout => rcdout.Recoutidx,
                                                     state => state.Virtualoutport,
                                                     (state, port) => port.Virtualoutport)
                                               .Join(Context.DbpVirtualmatrixinport.AsNoTracking(),
                                                     state => state,
                                                     port => port.Virtualinport,
                                                     (state, port) => port.Signaltype)
                                               .SingleOrDefaultAsync();
        }

        public Task<int?> GetParamTypeBySignalIDAsync(int signalID)
        {
            return Context.DbpRcdindesc.AsNoTracking().Where(rcdin => rcdin.Signalsrcid == signalID)
                                                             .Join(Context.DbpVirtualmatrixinport.AsNoTracking(),
                                                                   rcdin => rcdin.Recinidx,
                                                                   inport => inport.Virtualinport,
                                                                   (rcdin, inport) => inport.Signaltype)
                                                             .SingleOrDefaultAsync();
        }

        public Task<int> UpdateMSVChannelStateAsync(DbpMsvchannelState model)
        {
            var entity = Context.DbpMsvchannelState.Update(model);
            entity.State = EntityState.Modified;
            return Context.SaveChangesAsync();
        }

        public Task<int> ModifySourceVTRIDAndUserCodeAsync(int sourceVTRID, string userCode, params int[] ids)
        {
            var updateList = Context.DbpMsvchannelState.Where(a => ids.Contains(a.Channelid)).ToList();
            updateList.ForEach(a => { a.Sourcevtrid = sourceVTRID; a.Curusercode = userCode; });
            return Context.SaveChangesAsync();
        }

        public Task<List<TResult>> GetUserSettingAsync<TResult>(Func<IQueryable<DbpUsersettings>, IQueryable<TResult>> query, bool notrack = false)
        {
            throw new NotImplementedException();
        }

        public Task<TResult> GetUserSettingAsync<TResult>(Func<IQueryable<DbpUsersettings>, Task<TResult>> query, bool notrack = false)
        {
            return QueryModelAsync(Context.DbpUsersetting,query,notrack);
        }


        public Task<List<TResult>> GetXdcamDeviceListAsync<TResult>(Func<IQueryable<DbpXdcamDevice>, IQueryable<TResult>> query, bool notrack = false)
        {
            return QueryListAsync(Context.DbpXdcamDevice, query, notrack);
        }

    }
}
