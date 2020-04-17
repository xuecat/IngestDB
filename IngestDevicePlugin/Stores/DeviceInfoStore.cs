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
        #region Base
        protected TResult GetFirstOrDefault<TDbp, TResult>(DbSet<TDbp> contextSet, Func<IQueryable<TDbp>, IQueryable<TResult>> query, bool notrack = false)
            where TDbp : class
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

        /// <summary> 查询任意表集合返回 </summary>
        protected async Task<List<TResult>> QueryListAsync<TDbp, TResult>(DbSet<TDbp> contextSet, Func<IQueryable<TDbp>, IQueryable<TResult>> query, bool notrack = false)
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
        protected async Task<TResult> QueryModelAsync<TDbp, TResult>(DbSet<TDbp> contextSet, Func<IQueryable<TDbp>, Task<TResult>> query, bool notrack = false)
             where TDbp : class
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return await query(contextSet);
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

        public async Task<bool> HaveMatirxAsync()
        {
            return await Context.DbpMatrixinfo.AsNoTracking().AnyAsync(a => a.Matrixid == 2 && a.Matrixtypeid != 2);//老版本NULL MATRIX是6，现在是2，难道老版本一直返回的是有矩阵？
        }

        public async Task<int> GetBackUpSignalInfoByID(int srgid)
        {
            return await Context.DbpSignalsrcMasterbackup.AsNoTracking()
                .Where(a => a.Mastersignalsrcid == srgid)
                .Select(b => b.Signalsrcid).SingleOrDefaultAsync();
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
                                   SignalSourceType = p == null ? 0 : (emSignalSource)p.Signalsource.GetValueOrDefault(),
                                   GroupID = g == null ? 0 : g.Groupid,
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

        // TODO:待验证生成SQL
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

        //public async Task<CaptureChannelInfoDto> GetCaptureChannelByIDAsync(int channelid)
        //{
        //    var lst = await (from channel in Context.DbpCapturechannels
        //                     join device in Context.DbpCapturedevice on channel.Cpdeviceid equals device.Cpdeviceid into ps1
        //                     join grp in Context.DbpChannelgroupmap on channel.Cpdeviceid equals grp.Channelid into ps3
        //                     from p1 in ps1.DefaultIfEmpty()
        //                     from p3 in ps3.DefaultIfEmpty()
        //                     where channel.Channelid == channelid
        //                     select new CaptureChannelInfoDto
        //                     {
        //                         ID = channel.Channelid,
        //                         Name = channel.Channelname,
        //                         Desc = channel.Channeldesc,
        //                         CPDeviceID = channel.Cpdeviceid,
        //                         ChannelIndex = channel.Channelindex ?? 0,
        //                         DeviceTypeID = (int)CaptureChannelType.emMsvChannel,
        //                         BackState = (emBackupFlag)channel.Backupflag,
        //                         CPSignalType = channel.Cpsignaltype ?? 0,
        //                         OrderCode = p1 != null ? p1.Ordercode.GetValueOrDefault() : -1,
        //                         GroupID = p3 != null ? p3.Groupid : -1
        //                     }).SingleOrDefaultAsync();

        //    if (lst == null)
        //    {
        //        lst = await Context.DbpIpVirtualchannel.AsNoTracking().Where(x => x.Channelid == channelid).Select(x => new CaptureChannelInfoDto
        //        {
        //            ID = x.Channelid,
        //            Name = x.Channelname,
        //            Desc = x.Ipaddress,
        //            CPDeviceID = x.Deviceid,
        //            ChannelIndex = x.Ctrlport ?? 0,
        //            DeviceTypeID = x.Channeltype ?? (int)CaptureChannelType.emMsvChannel,
        //            BackState = (emBackupFlag)x.Backuptype.GetValueOrDefault(),
        //            CarrierID = x.Carrierid ?? 0,
        //            OrderCode = x.Deviceindex ?? -1,
        //            CPSignalType = x.Cpsignaltype ?? 0
        //        }).SingleOrDefaultAsync();
        //    }

        //    return lst;
        //}

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
                DeviceTypeID = x.Channeltype ?? (int)CaptureChannelType.emMsvChannel,
                BackState = (emBackupFlag)x.Backuptype.GetValueOrDefault(),
                CarrierID = x.Carrierid ?? 0,
                OrderCode = x.Deviceindex ?? -1,
                CPSignalType = x.Cpsignaltype ?? 0
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
                (rcin, rcout) => new { ID = rcin.Signalsrcid, Channel = rcout.Channelid })
                .Where(x => x.ID == signalid).Select(f => f.Channel).ToListAsync();

        }

        #endregion

        public async Task<List<TResult>> GetRcdindescAsync<TResult>(Func<IQueryable<DbpRcdindesc>, IQueryable<TResult>> query, bool notrack = false)
        {
            return await QueryListAsync(Context.DbpRcdindesc, query, notrack);
        }
        public async Task<TResult> GetRcdindescAsync<TResult>(Func<IQueryable<DbpRcdindesc>, Task<TResult>> query, bool notrack = false)
        {
            return await QueryModelAsync(Context.DbpRcdindesc, query, notrack);
        }



        public async Task<List<TResult>> GetRcdoutdescAsync<TResult>(Func<IQueryable<DbpRcdoutdesc>, IQueryable<TResult>> query, bool notrack = false)
        {
            return await QueryListAsync(Context.DbpRcdoutdesc, query, notrack);
        }

        public async Task<List<TResult>> GetSignalsrcMasterbackupAsync<TResult>(Func<IQueryable<DbpSignalsrcMasterbackup>, IQueryable<TResult>> query, bool notrack = false)
        {
            return await QueryListAsync(Context.DbpSignalsrcMasterbackup, query, notrack);
        }

        public async Task<TResult> GetSignalsrcMasterbackupAsync<TResult>(Func<IQueryable<DbpSignalsrcMasterbackup>, Task<TResult>> query, bool notrack = false)
        {
            return await QueryModelAsync(Context.DbpSignalsrcMasterbackup, query, notrack);
        }

        public async Task<List<TResult>> GetSignalDeviceMapAsync<TResult>(Func<IQueryable<DbpSignalDeviceMap>, IQueryable<TResult>> query, bool notrack = false)
        {
            return await QueryListAsync(Context.DbpSignalDeviceMap, query, notrack);
        }

        public async Task<List<TResult>> GetSignalsrcAsync<TResult>(Func<IQueryable<DbpSignalsrc>, IQueryable<TResult>> query, bool notrack = false)
        {
            return await QueryListAsync(Context.DbpSignalsrc, query, notrack);
        }


        public async Task<List<TResult>> GetCapturechannelsAsync<TResult>(Func<IQueryable<DbpCapturechannels>, IQueryable<TResult>> query, bool notrack = false)
        {
            return await QueryListAsync(Context.DbpCapturechannels, query, notrack);
        }
        public async Task<TResult> GetCapturechannelsAsync<TResult>(Func<IQueryable<DbpCapturechannels>, Task<TResult>> query, bool notrack = false)
        {
            return await QueryModelAsync(Context.DbpCapturechannels, query, notrack);
        }


        public async Task<List<TResult>> GetIpDatachannelsAsync<TResult>(Func<IQueryable<DbpIpDatachannelinfo>, IQueryable<TResult>> query, bool notrack = false)
        {
            return await QueryListAsync(Context.DbpIpDatachannelinfo, query, notrack);
        }

        public async Task<TResult> GetIpDatachannelsAsync<TResult>(Func<IQueryable<DbpIpDatachannelinfo>, Task<TResult>> query, bool notrack = false)
        {
            return await QueryModelAsync(Context.DbpIpDatachannelinfo, query, notrack);
        }


        public async Task<List<TResult>> GetCapturedeviceAsync<TResult>(Func<IQueryable<DbpCapturedevice>, IQueryable<TResult>> query, bool notrack = false)
        {
            return await QueryListAsync(Context.DbpCapturedevice, query, notrack);
        }


        public async Task<List<TResult>> GetIpVirtualchannelAsync<TResult>(Func<IQueryable<DbpIpVirtualchannel>, IQueryable<TResult>> query, bool notrack = false)
        {
            return await QueryListAsync(Context.DbpIpVirtualchannel, query, notrack);
        }
        public async Task<TResult> GetIpVirtualchannelAsync<TResult>(Func<IQueryable<DbpIpVirtualchannel>, Task<TResult>> query, bool notrack = false)
        {
            return await QueryModelAsync(Context.DbpIpVirtualchannel, query, notrack);
        }


        public async Task<List<TResult>> GetChannelgroupmapAsync<TResult>(Func<IQueryable<DbpChannelgroupmap>, IQueryable<TResult>> query, bool notrack = false)
        {
            return await QueryListAsync(Context.DbpChannelgroupmap, query, notrack);
        }
        public async Task<List<int>> GetSignalIdsByChannelIdForNotMatrix(int channelid)
        {
            return await Context.DbpRcdoutdesc.Where(x => x.Channelid == channelid)
                                              .Join(Context.DbpRcdindesc,
                                                    rcout => rcout.Recoutidx,
                                                    rcin => rcin.Recinidx,
                                                    (rcout, rcin) => rcin.Signalsrcid)
                                              .ToListAsync();
        }

        public async Task<int> SaveChannelExtenddataAsync(UpdateChnExtData_IN model)
        {
            var deviceMap = await Context.DbpChnExtenddata.FirstOrDefaultAsync(a => a.Channaelid == model.nChnID && a.Datatype == model.type);
            if (deviceMap == null)
            {
                await Context.DbpChnExtenddata.AddAsync(new DbpChnExtenddata
                {
                    Channaelid = model.nChnID,
                    Datatype = model.type,
                    Extenddata = model.strData
                });
            }
            else
            {
                deviceMap.Extenddata = model.strData;
            }
            return await Context.SaveChangesAsync();
        }


        public async Task<List<TResult>> GetSignalSrcExsAsync<TResult>(Func<IQueryable<DbpSignalsrcMasterbackup>, IQueryable<TResult>> query, bool notrack = false)
        {
            return await QueryListAsync(Context.DbpSignalsrcMasterbackup, query, notrack);
        }
        public async Task<TResult> GetSignalSrcExsAsync<TResult>(Func<IQueryable<DbpSignalsrcMasterbackup>, Task<TResult>> query, bool notrack = false)
        {
            return await QueryModelAsync(Context.DbpSignalsrcMasterbackup, query, notrack);
        }


        public async Task<List<TResult>> GetSignalsrcgroupmapAsync<TResult>(Func<IQueryable<DbpSignalsrcgroupmap>, IQueryable<TResult>> query, bool notrack = false)
        {
            return await QueryListAsync(Context.DbpSignalsrcgroupmap, query, notrack);
        }
        public async Task<TResult> GetSignalsrcgroupmapAsync<TResult>(Func<IQueryable<DbpSignalsrcgroupmap>, Task<TResult>> query, bool notrack = false)
        {
            return await QueryModelAsync(Context.DbpSignalsrcgroupmap, query, notrack);
        }
        

        public async Task<TResult> GetSignalDeviceMapAsync<TResult>(Func<IQueryable<DbpSignalDeviceMap>, Task<TResult>> query, bool notrack = false)
        {
            return await QueryModelAsync(Context.DbpSignalDeviceMap, query, notrack);
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


        public async Task<List<TResult>> GetMsvchannelStateAsync<TResult>(Func<IQueryable<DbpMsvchannelState>, IQueryable<TResult>> query, bool notrack = false)
        {
            return await QueryListAsync(Context.DbpMsvchannelState, query, notrack);
        }
        public async Task<TResult> GetMsvchannelStateAsync<TResult>(Func<IQueryable<DbpMsvchannelState>, Task<TResult>> query, bool notrack = false)
        {
            return await QueryModelAsync(Context.DbpMsvchannelState, query, notrack);
        }


        public async Task<List<TResult>> GetSignalGroupAsync<TResult>(Func<IQueryable<DbpSignalgroup>, IQueryable<TResult>> query, bool notrack = false)
        {
            return await QueryListAsync(Context.DbpSignalgroup, query, notrack);
        }
        public async Task<TResult> GetSignalGroupAsync<TResult>(Func<IQueryable<DbpSignalgroup>, Task<TResult>> query, bool notrack = false)
        {
            return await QueryModelAsync(Context.DbpSignalgroup, query, notrack);
        }


        public async Task<List<TResult>> GetGPIMapInfoByGPIIDAsync<TResult>(Func<IQueryable<DbpGpiMap>, IQueryable<TResult>> query, bool notrack = false)
        {
            return await QueryListAsync(Context.DbpGpiMap, query, notrack);
        }
        public async Task<TResult> GetGPIMapInfoByGPIIDAsync<TResult>(Func<IQueryable<DbpGpiMap>, Task<TResult>> query, bool notrack = false)
        {
            return await QueryModelAsync(Context.DbpGpiMap, query, notrack);
        }

        public async Task<List<TResult>> GetIpDeviceAsync<TResult>(Func<IQueryable<DbpIpDevice>, IQueryable<TResult>> query, bool notrack = false)
        {
            return await QueryListAsync(Context.DbpIpDevice, query, notrack);
        }

        public async Task<TResult> GetIpDeviceAsync<TResult>(Func<IQueryable<DbpIpDevice>, Task<TResult>> query, bool notrack = false)
        {
            return await QueryModelAsync(Context.DbpIpDevice, query, notrack);
        }


        public async Task<List<TResult>> GetIpProgrammeAsync<TResult>(Func<IQueryable<DbpIpProgramme>, IQueryable<TResult>> query, bool notrack = false)
        {
            return await QueryListAsync(Context.DbpIpProgramme, query, notrack);
        }
        public async Task<TResult> GetIpProgrammeAsync<TResult>(Func<IQueryable<DbpIpProgramme>, Task<TResult>> query, bool notrack = false)
        {
            return await QueryModelAsync(Context.DbpIpProgramme, query, notrack);
        }


        public async Task<List<TResult>> GetStreamMediaAsync<TResult>(Func<IQueryable<DbpStreammedia>, IQueryable<TResult>> query, bool notrack = false)
        {
            return await QueryListAsync(Context.DbpStreammedia, query, notrack);
        }
        public async Task<TResult> GetStreamMediaAsync<TResult>(Func<IQueryable<DbpStreammedia>, Task<TResult>> query, bool notrack = false)
        {
            return await QueryModelAsync(Context.DbpStreammedia, query, notrack);
        }


        public Task<List<SignalGroupState>> GetAllSignalGroupInfoAsync()
        {
            return Context.DbpSignalsrcgroupmap.AsNoTracking().Join(Context.DbpSignalgroup.AsNoTracking(),
                                                     map => map.Groupid,
                                                     group => group.Groupid,
                                                     (map, group) => new SignalGroupState
                                                     {
                                                         signalsrcid = map.Signalsrcid,
                                                         groupid = group.Groupid,
                                                         groupname = group.Groupname,
                                                         groupdesc = group.Groupdesc
                                                     }).ToListAsync();
        }

        public async Task<int?> GetParamTypeByChannleIDAsync(int nChannelID)
        {
            return await Context.DbpRcdoutdesc.AsNoTracking().Where(rcdout => rcdout.Channelid == nChannelID)
                                               .Join(Context.DbpVirtualmatrixportstate.AsNoTracking().Where(state => state.State == 1),
                                                     rcdout => rcdout.Recoutidx,
                                                     state => state.Virtualoutport,
                                                     (state, port) => port.Virtualoutport)
                                               .Join(Context.DbpVirtualmatrixinport.AsNoTracking(),
                                                     state => state,
                                                     port => port.Virtualinport,
                                                     (state, port) => port.Signaltype)
                                               .FirstOrDefaultAsync();
        }

        public async Task<int> UpdateMSVChannelStateAsync(DbpMsvchannelState model)
        {
            Context.DbpMsvchannelState.Update(model);
            return await Context.SaveChangesAsync();
        }
    }
}
