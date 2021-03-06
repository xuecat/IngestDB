﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IngestDevicePlugin.Dto;
using IngestDevicePlugin.Models;
using Microsoft.EntityFrameworkCore;

using ProgrammeInfoDto = IngestDevicePlugin.Dto.ProgrammeInfoResponse;
using CaptureChannelInfoDto = IngestDevicePlugin.Dto.CaptureChannelInfoResponse;
using IngestDBCore;
using Sobey.Core.Log;

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

        public async Task<List<TResult>> GetAllRouterInPortInfoAsync<TResult>(Func<IQueryable<DbpRcdindesc>, IQueryable<TResult>> query, bool notrack = false)
        {
            return await QueryModelAsync(Context.DbpRcdindesc, query, notrack);
        }

        public async Task<List<TResult>> GetAllRouterOutPortInfoAsync<TResult>(Func<IQueryable<DbpRcdoutdesc>, IQueryable<TResult>> query, bool notrack = false)
        {
            return await QueryModelAsync(Context.DbpRcdoutdesc, query, notrack);
        }

        public async Task<List<TResult>> GetAllSignalDeviceMapAsync<TResult>(Func<IQueryable<DbpSignalDeviceMap>, IQueryable<TResult>> query, bool notrack = false)
        {
            return await QueryModelAsync(Context.DbpSignalDeviceMap, query, notrack);
        }

        public async Task<List<TResult>> GetAllSignalSrcsAsync<TResult>(Func<IQueryable<DbpSignalsrc>, IQueryable<TResult>> query, bool notrack = false)
        {
            return await QueryModelAsync(Context.DbpSignalsrc, query, notrack);
        }

        public async Task<List<TResult>> GetAllCaptureChannelsAsync<TResult>(Func<IQueryable<DbpCapturechannels>, IQueryable<TResult>> query, bool notrack = false)
        {
            return await QueryModelAsync(Context.DbpCapturechannels, query, notrack);
        }

        public async Task<List<TResult>> GetAllCaptureDevicesAsync<TResult>(Func<IQueryable<DbpCapturedevice>, IQueryable<TResult>> query, bool notrack = false)
        {
            return await QueryModelAsync(Context.DbpCapturedevice, query, notrack);
        }

        public async Task<List<TResult>> GetAllVirtualChannelsAsync<TResult>(Func<IQueryable<DbpIpVirtualchannel>, IQueryable<TResult>> query, bool notrack = false)
        {
            return await QueryModelAsync(Context.DbpIpVirtualchannel, query, notrack);
        }

        public async Task<List<TResult>> GetAllChannelGroupMapAsync<TResult>(Func<IQueryable<DbpChannelgroupmap>, IQueryable<TResult>> query, bool notrack = false)
        {
            return await QueryModelAsync(Context.DbpChannelgroupmap, query, notrack);
        }

        public TResult GetSignalDeviceMap<TResult>(Func<IQueryable<DbpSignalDeviceMap>, IQueryable<TResult>> query, bool notrack = false)
        {
            return GetFirstOrDefault(Context.DbpSignalDeviceMap, query, notrack);
        }

        public async Task<int> SetSignalDeviceMap(DbpSignalDeviceMap model)
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
            try
            {
                return await Context.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {
                throw e;
            }
        }

        protected TResult GetFirstOrDefault<TDbp, TResult>(DbSet<TDbp> contextSet, Func<IQueryable<TDbp>, IQueryable<TResult>> query, bool notrack = false) where TDbp : class
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            if (notrack)
            {
                return query(contextSet.AsNoTracking()).FirstOrDefault();
            }
            return query(contextSet).FirstOrDefault();
        }
        
        public async Task<bool> HaveMatirxAsync()
        {
            return await Context.DbpMatrixinfo.AsNoTracking().AnyAsync(a => a.Matrixid == 2 && a.Matrixtypeid != 2);//老版本NULL MATRIX是6，现在是2，难道老版本一直返回的是有矩阵？
        }

        //老版本 GetProgrammeInfoById
        public async Task<ProgrammeInfoDto> GetSignalInfoAsync(int srcid)
        {
            //DBP_STREAMMEDIA DBP_IP_PROGRAMME 这些没有写，后面写吧

            var query = await (from sig in Context.DbpSignalsrc
                        join recin in Context.DbpRcdindesc on sig.Signalsrcid equals recin.Signalsrcid into ps
                        join grp in Context.DbpSignalsrcgroupmap on sig.Signalsrcid equals grp.Signalsrcid into pg
                        from p in ps.DefaultIfEmpty()
                        from g in pg.DefaultIfEmpty()
                        where sig.Signalsrcid == srcid
                        select new ProgrammeInfoDto
                        {
                            ProgrammeId = sig.Signalsrcid,
                            ProgrammeName = sig.Name,
                            ProgrammeDesc = sig.Signaldesc,
                            TypeId = sig.Signaltypeid,
                            PgmType = ProgrammeType.PT_SDI,
                            ImageType = (ImageType)sig.Imagetype,
                            PureAudio = sig.Pureaudio ?? 0,
                            SignalSourceType = p== null?0:(emSignalSource)p.Signalsource.GetValueOrDefault(),
                            GroupID = g==null?0 : g.Groupid,
                        }).SingleOrDefaultAsync();
            if (query == null)
            {
                Logger.Error("GetSignalInfoAsync query error" + srcid.ToString());
                return null;
            }
            
            switch (query.PgmType)
            {
                case ProgrammeType.PT_SDI:
                    { } break;//上面已经赋值了
                case ProgrammeType.PT_IPTS:
                    {
                        query.SignalSourceType = emSignalSource.emIPTS;
                    } break;
                case ProgrammeType.PT_StreamMedia:
                    {
                        query.SignalSourceType = emSignalSource.emStreamMedia;
                    } break;
                default:
                    break;
            }
            return query;
        }

        public async Task<int> GetMatrixChannelBySignal(int channelid)
        {
            return await (from src in Context.DbpRcdindesc
                   join device in Context.DbpVirtualmatrixportstate on src.Recinidx equals device.Virtualinport into ps1
                   join grp in Context.DbpRcdoutdesc on channelid equals grp.Channelid into ps3
                   from p1 in ps1.DefaultIfEmpty()
                   from p3 in ps3.DefaultIfEmpty()
                   where p1.Virtualoutport == p3.Recoutidx&& p1.State == 1
                   select src.Signalsrcid).SingleOrDefaultAsync();
        }

        public async Task<CaptureChannelInfoDto> GetCaptureChannelByIDAsync(int channelid)
        {
            var lst = await (from channel in Context.DbpCapturechannels
                         join device in Context.DbpCapturedevice on channel.Cpdeviceid equals device.Cpdeviceid into ps1
                         join grp in Context.DbpChannelgroupmap on channel.Cpdeviceid equals grp.Channelid into ps3
                         from p1 in ps1.DefaultIfEmpty()
                         from p3 in ps3.DefaultIfEmpty()
                         where channel.Channelid == channelid
                         select new CaptureChannelInfoDto
                         {
                             ID = channel.Channelid,
                             Name = channel.Channelname,
                             Desc = channel.Channeldesc,
                             CPDeviceID = channel.Cpdeviceid,
                             ChannelIndex = channel.Channelindex ?? 0,
                             DeviceTypeID = (int)CaptureChannelType.emMsvChannel,
                             BackState = (emBackupFlag)channel.Backupflag,
                             CPSignalType = channel.Cpsignaltype ?? 0,
                             OrderCode = p1 != null ? p1.Ordercode.GetValueOrDefault() : -1,
                             GroupID = p3 != null ? p3.Groupid : -1
                         }).SingleOrDefaultAsync();

            if (lst == null)
            {
                 lst = await Context.DbpIpVirtualchannel.AsNoTracking().Where(x=> x.Channelid == channelid).Select(x => new CaptureChannelInfoDto
                {
                    ID = x.Channelid,
                    Name = x.Channelname,
                    Desc = x.Ipaddress,
                    CPDeviceID = x.Deviceid,
                    ChannelIndex = x.Ctrlport ?? 0,
                    DeviceTypeID = x.Channeltype ?? (int)CaptureChannelType.emMsvChannel,
                    BackState = (emBackupFlag)x.Backuptype.GetValueOrDefault(),
                    CarrierID = x.Carrierid ?? 0,
                    OrderCode = x.Deviceindex ?? -1,
                    CPSignalType = x.Cpsignaltype ?? 0
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
                       //join recout in Context.DbpRcdoutdesc on channel.Channelid equals recout.Channelid into ps2
                       join grp in Context.DbpChannelgroupmap on channel.Cpdeviceid equals grp.Channelid into ps3
                       from p1 in ps1.DefaultIfEmpty()
                           //from p2 in ps2.DefaultIfEmpty()
                       from p3 in ps3.DefaultIfEmpty()
                       select new CaptureChannelInfoDto
                       {
                           ID = channel.Channelid,
                           Name = channel.Channelname,
                           Desc = channel.Channeldesc,
                           CPDeviceID = channel.Cpdeviceid,
                           ChannelIndex = channel.Channelindex ?? 0,
                           DeviceTypeID = (int)CaptureChannelType.emMsvChannel,
                           BackState = (emBackupFlag)channel.Backupflag,
                           CPSignalType = channel.Cpsignaltype ?? 0,
                           OrderCode = p1 != null ? p1.Ordercode.GetValueOrDefault() : -1,
                           GroupID = p3 != null ? p3.Groupid : -1
                       }).ToListAsync();
            }
            else
            {
                lst = await (from channel in Context.DbpCapturechannels
                             join device in Context.DbpCapturedevice on channel.Cpdeviceid equals device.Cpdeviceid into ps1
                             join cstatu in Context.DbpMsvchannelState on channel.Channelid equals cstatu.Channelid into ps2
                             join grp in Context.DbpChannelgroupmap on channel.Cpdeviceid equals grp.Channelid into ps3
                             from p1 in ps1.DefaultIfEmpty()
                             from p2 in ps2.DefaultIfEmpty()
                             from p3 in ps3.DefaultIfEmpty()
                             where p2 != null && p2.Devstate != 0 && p2.Msvmode != 0
                             select new CaptureChannelInfoDto
                             {
                                 ID = channel.Channelid,
                                 Name = channel.Channelname,
                                 Desc = channel.Channeldesc,
                                 CPDeviceID = channel.Cpdeviceid,
                                 ChannelIndex = channel.Channelindex ?? 0,
                                 DeviceTypeID = (int)CaptureChannelType.emMsvChannel,
                                 BackState = (emBackupFlag)channel.Backupflag,
                                 CPSignalType = channel.Cpsignaltype ?? 0,
                                 OrderCode = p1 != null ? p1.Ordercode.GetValueOrDefault() : -1,
                                 GroupID = p3 != null ? p3.Groupid : -1
                             }).ToListAsync();
            }
            

            //DBP_IP_VIRTUALCHANNEL 现在应该是没有用了
            var iplst = await Context.DbpIpVirtualchannel.AsNoTracking().Select(x => new CaptureChannelInfoDto
            {
                ID = x.Channelid,
                Name = x.Channelname,
                Desc = x.Ipaddress,
                CPDeviceID = x.Deviceid,
                ChannelIndex = x.Ctrlport ?? 0,
                DeviceTypeID = x.Channeltype?? (int)CaptureChannelType.emMsvChannel,
                BackState = (emBackupFlag)x.Backuptype.GetValueOrDefault(),
                CarrierID = x.Carrierid ?? 0,
                OrderCode = x.Deviceindex ?? -1,
                CPSignalType = x.Cpsignaltype?? 0
            }).ToListAsync();

            if (lst == null ||lst.Count < 1)
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

        public async Task<List<DbpChannelRecmap>> GetAllChannelUnitMap()
        {
            return await Context.DbpChannelRecmap.AsNoTracking().ToListAsync();
        }

        public async Task<DbpChannelRecmap> GetChannelUnitMap(int channel)
        {
            return await Context.DbpChannelRecmap.Where(x => x.Channelid == channel).SingleAsync();
        }

        public async Task<List<int>> GetChannelIdsBySignalIdForNotMatrix(int signalid)
        {
            return await Context.DbpRcdindesc.Join(Context.DbpRcdoutdesc,
                rcin => rcin.Recinidx,
                rcout => rcout.Recoutidx,
                (rcin, rcout) => new { ID = rcin.Signalsrcid, Channel=rcout.Channelid})
                .Where(x => x.ID == signalid).Select(f => f.Channel).ToListAsync();

        }
        public async Task<List<int>> GetSignalIdsByChannelIdForNotMatrix(int channelid)
        {
            return await Context.DbpRcdindesc.Join(Context.DbpRcdoutdesc,
                rcin => rcin.Recinidx,
                rcout => rcout.Recoutidx,
                (rcin, rcout) => new { ID = rcin.Signalsrcid, Channel = rcout.Channelid })
                .Where(x => x.Channel == channelid).Select(f => f.ID).ToListAsync();
        }
        protected async Task<List<TResult>> QueryModelAsync<TDbp, TResult>(DbSet<TDbp> contextSet, Func<IQueryable<TDbp>, IQueryable<TResult>> query, bool notrack = false) where TDbp : class
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
    }
}
