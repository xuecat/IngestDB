using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IngestDevicePlugin.Dto;
using IngestDevicePlugin.Models;
using Microsoft.EntityFrameworkCore;
using ProgrammeInfoDto = IngestDevicePlugin.Dto.Response.ProgrammeInfoResponse;
using ProgrammeInfoExDto = IngestDevicePlugin.Dto.Response.ProgrammeInfoExResponse;
using CaptureChannelInfoDto = IngestDevicePlugin.Dto.Response.CaptureChannelInfoResponse;
using CaptureChannelInfoExDto = IngestDevicePlugin.Dto.Response.CaptureChannelInfoExResponse;
using DeviceInfoDto = IngestDevicePlugin.Dto.Response.DeviceInfoResponse;
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
            //var query = await (from sig in Context.DbpSignalsrc.AsNoTracking()
            //                   join recin in Context.DbpRcdindesc.AsNoTracking() on sig.Signalsrcid equals recin.Signalsrcid into ps
            //                   join grp in Context.DbpSignalsrcgroupmap.AsNoTracking() on sig.Signalsrcid equals grp.Signalsrcid into pg
            //                   from p in ps.DefaultIfEmpty()
            //                   from g in pg.DefaultIfEmpty()
            //                   where p != null 
            //                   select new ProgrammeInfoDto
            //                   {
            //                       ProgrammeId = sig.Signalsrcid,
            //                       ProgrammeName = sig.Name,
            //                       ProgrammeDesc = sig.Signaldesc,
            //                       TypeId = sig.Signaltypeid,
            //                       PgmType = ProgrammeType.PT_SDI,
            //                       ImageType = (ImageType)sig.Imagetype,
            //                       PureAudio = sig.Pureaudio ?? 0,
            //                       SignalSourceType = p == null ? 0 : (emSignalSource)p.Signalsource.GetValueOrDefault(),
            //                       GroupId = g == null ? 0 : g.Groupid,
            //                   }).ToListAsync();


            var query = await (from sig in Context.DbpSignalsrc.AsNoTracking().Join(Context.DbpRcdindesc.AsNoTracking(),
                src => src.Signalsrcid,
                rcin => rcin.Signalsrcid,
                (src, rcin) => new { src, rcin.Signalsource })
                               join grp in Context.DbpSignalsrcgroupmap.AsNoTracking() on sig.src.Signalsrcid equals grp.Signalsrcid into pg
                               from g in pg.DefaultIfEmpty()
                               select new ProgrammeInfoDto
                               {
                                   ProgrammeId = sig.src.Signalsrcid,
                                   ProgrammeName = sig.src.Name,
                                   ProgrammeDesc = sig.src.Signaldesc,
                                   TypeId = sig.src.Signaltypeid,
                                   PgmType = ProgrammeType.PT_SDI,
                                   ImageType = (ImageType)sig.src.Imagetype,
                                   PureAudio = sig.src.Pureaudio ?? 0,
                                   SignalSourceType = sig.Signalsource == null ? 0 : (emSignalSource)sig.Signalsource.GetValueOrDefault(),
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
            //var query = await (from sig in Context.DbpSignalsrc.AsNoTracking()
            //                   join recin in Context.DbpRcdindesc.AsNoTracking() on sig.Signalsrcid equals recin.Signalsrcid into ps
            //                   join grp in Context.DbpSignalsrcgroupmap.AsNoTracking() on sig.Signalsrcid equals grp.Signalsrcid into pg
            //                   from p in ps.DefaultIfEmpty()
            //                   from g in pg.DefaultIfEmpty()
            //                   where p != null && srcid.Contains(sig.Signalsrcid)
            //                   select new ProgrammeInfoDto
            //                   {
            //                       ProgrammeId = sig.Signalsrcid,
            //                       ProgrammeName = sig.Name,
            //                       ProgrammeDesc = sig.Signaldesc,
            //                       TypeId = sig.Signaltypeid,
            //                       PgmType = ProgrammeType.PT_SDI,
            //                       ImageType = (ImageType)sig.Imagetype,
            //                       PureAudio = sig.Pureaudio ?? 0,
            //                       SignalSourceType = p == null ? 0 : (emSignalSource)p.Signalsource.GetValueOrDefault(),
            //                       GroupId = g == null ? 0 : g.Groupid,
            //                   }).ToListAsync();

            var query = await (from sig in Context.DbpSignalsrc.AsNoTracking().Join(Context.DbpRcdindesc.AsNoTracking(),
                src => src.Signalsrcid,
                rcin => rcin.Signalsrcid,
                (src, rcin) => new { src, rcin.Signalsource })
                               where srcid.Contains(sig.src.Signalsrcid)
                               join grp in Context.DbpSignalsrcgroupmap.AsNoTracking() on sig.src.Signalsrcid equals grp.Signalsrcid into pg
                               from g in pg.DefaultIfEmpty()
                               select new ProgrammeInfoDto
                               {
                                   ProgrammeId = sig.src.Signalsrcid,
                                   ProgrammeName = sig.src.Name,
                                   ProgrammeDesc = sig.src.Signaldesc,
                                   TypeId = sig.src.Signaltypeid,
                                   PgmType = ProgrammeType.PT_SDI,
                                   ImageType = (ImageType)sig.src.Imagetype,
                                   PureAudio = sig.src.Pureaudio ?? 0,
                                   SignalSourceType = sig.Signalsource == null ? 0 : (emSignalSource)sig.Signalsource.GetValueOrDefault(),
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

            //var query = await (from sig in Context.DbpSignalsrc.AsNoTracking()
            //                   join recin in Context.DbpRcdindesc.AsNoTracking() on sig.Signalsrcid equals recin.Signalsrcid into ps
            //                   join grp in Context.DbpSignalsrcgroupmap.AsNoTracking() on sig.Signalsrcid equals grp.Signalsrcid into pg
            //                   from p in ps.DefaultIfEmpty()
            //                   from g in pg.DefaultIfEmpty()
            //                   where p!=null && sig.Signalsrcid == srcid
            //                   select new ProgrammeInfoDto
            //                   {
            //                       ProgrammeId = sig.Signalsrcid,
            //                       ProgrammeName = sig.Name,
            //                       ProgrammeDesc = sig.Signaldesc,
            //                       TypeId = sig.Signaltypeid,
            //                       PgmType = ProgrammeType.PT_SDI,
            //                       ImageType = (ImageType)sig.Imagetype,
            //                       PureAudio = sig.Pureaudio ?? 0,
            //                       SignalSourceType = p == null ? 0 : (emSignalSource)p.Signalsource.GetValueOrDefault(),
            //                       GroupId = g == null ? 0 : g.Groupid,
            //                   }).SingleOrDefaultAsync();

            var query = await (from sig in Context.DbpSignalsrc.AsNoTracking().Join(Context.DbpRcdindesc.AsNoTracking(),
                src => src.Signalsrcid,
                rcin => rcin.Signalsrcid,
                (src, rcin) => new { src, rcin.Signalsource })
                               where srcid == sig.src.Signalsrcid
                               join grp in Context.DbpSignalsrcgroupmap.AsNoTracking() on sig.src.Signalsrcid equals grp.Signalsrcid into pg
                               from g in pg.DefaultIfEmpty()
                               select new ProgrammeInfoDto
                               {
                                   ProgrammeId = sig.src.Signalsrcid,
                                   ProgrammeName = sig.src.Name,
                                   ProgrammeDesc = sig.src.Signaldesc,
                                   TypeId = sig.src.Signaltypeid,
                                   PgmType = ProgrammeType.PT_SDI,
                                   ImageType = (ImageType)sig.src.Imagetype,
                                   PureAudio = sig.src.Pureaudio ?? 0,
                                   SignalSourceType = sig.Signalsource == null ? 0 : (emSignalSource)sig.Signalsource.GetValueOrDefault(),
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

        public async Task<List<DeviceInfoDto>> GetAllDeviceInfoAsync()
        {
            return await Context.DbpCapturedevice.AsNoTracking().Join(Context.DbpCapturechannels.AsNoTracking(),
                                                                        devicea => devicea.Cpdeviceid,
                                                                        channela => channela.Cpdeviceid,
                                                                        (devicea, channela) => new DeviceInfoDto()
                                                                        {
                                                                            ChannelId = channela.Channelid,
                                                                            ChannelIndex = channela.Channelindex.GetValueOrDefault(),
                                                                            ChannelName = channela.Channelname,
                                                                            Id = devicea.Cpdeviceid,
                                                                            DeviceName = devicea.Devicename,
                                                                            DeviceTypeId = devicea.Devicetypeid,
                                                                            Ip = devicea.Ipaddress,
                                                                            OrderCode = devicea.Ordercode.GetValueOrDefault()
                                                                        }).ToListAsync();
        }

        public async Task<DeviceInfoDto> GetDeviceInfoByIdAsync(int deviceid)
        {
            return await Context.DbpCapturedevice.Where(device => device.Cpdeviceid == deviceid)
                .Join(Context.DbpCapturechannels,
                        devicea => devicea.Cpdeviceid,
                        channela => channela.Cpdeviceid,
                        (devicea, channela) => new DeviceInfoDto()
                        {
                            ChannelId = channela.Channelid,
                            ChannelIndex = channela.Channelindex.GetValueOrDefault(),
                            ChannelName = channela.Channelname,
                            Id = devicea.Cpdeviceid,
                            DeviceName = devicea.Devicename,
                            DeviceTypeId = devicea.Devicetypeid,
                            Ip = devicea.Ipaddress,
                            OrderCode = devicea.Ordercode.GetValueOrDefault()
                        }).SingleOrDefaultAsync();
        }


        public async Task<int> GetMatrixChannelBySignalAsync(int channelid)
        {
            return await Context.DbpRcdoutdesc.Where(rcdout => rcdout.Channelid == channelid)
                                        .Join(Context.DbpVirtualmatrixportstate.AsNoTracking().Where(matrix => matrix.State == 1),
                                              rcdout => rcdout.Recoutidx,
                                              matrix => matrix.Virtualoutport,
                                              (rcdout, matrix) => matrix.Virtualinport)
                                        .Join(Context.DbpRcdindesc,
                                              inport => inport,
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
            return Context.DbpRcdoutdesc.Join(Context.DbpVirtualmatrixportstate.AsNoTracking().Where(matrix => matrix.State == 1),
                                                     rcdout => rcdout.Recoutidx,
                                                     matrix => matrix.Virtualoutport,
                                                     (rcdout, matrix) => new
                                                     {
                                                         matrix.Virtualinport,
                                                         matrix.Virtualoutport,
                                                         rcdout.Channelid,
                                                         matrix.State,
                                                         matrix.Lastoprtime
                                                     })
                                               .Join(Context.DbpRcdindesc,
                                                     join => join.Virtualinport,
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
            //var lst = await (from channel in Context.DbpCapturechannels
            //                 join device in Context.DbpCapturedevice on channel.Cpdeviceid equals device.Cpdeviceid into ps1
            //                 join grp in Context.DbpChannelgroupmap on channel.Cpdeviceid equals grp.Channelid into ps3
            //                 from p1 in ps1.DefaultIfEmpty()
            //                 from p3 in ps3.DefaultIfEmpty()
            //                 where ps1 !=null && channel.Channelid == channelid
            //                 select new CaptureChannelInfoDto
            //                 {
            //                     Id = channel.Channelid,
            //                     Name = channel.Channelname,
            //                     Desc = channel.Channeldesc,
            //                     CpDeviceId = channel.Cpdeviceid,
            //                     ChannelIndex = channel.Channelindex ?? 0,
            //                     DeviceTypeId = p1.Devicetypeid <= 0 ? (int)CaptureChannelType.emMsvChannel : p1.Devicetypeid,
            //                     BackState = (emBackupFlag)channel.Backupflag,
            //                     CpSignalType = channel.Cpsignaltype ?? 0,
            //                     OrderCode = p1 != null ? p1.Ordercode.GetValueOrDefault() : -1,
            //                     GroupId = p3 != null ? p3.Groupid : -1
            //                 }).SingleOrDefaultAsync();

            var lst = await (from channel in Context.DbpCapturechannels.AsNoTracking().Join(Context.DbpCapturedevice.AsNoTracking(),
               chn => chn.Cpdeviceid,
               device => device.Cpdeviceid,
               (chn, device) => new { chn, device.Devicetypeid, device.Ordercode })
                             where channelid == channel.chn.Channelid
                             join grp in Context.DbpChannelgroupmap.AsNoTracking() on channel.chn.Channelid equals grp.Channelid into pg
                             from g in pg.DefaultIfEmpty()
                             select new CaptureChannelInfoDto
                             {
                                 Id = channel.chn.Channelid,
                                 Name = channel.chn.Channelname,
                                 Desc = channel.chn.Channeldesc,
                                 CpDeviceId = channel.chn.Cpdeviceid,
                                 ChannelIndex = channel.chn.Channelindex ?? 0,
                                 DeviceTypeId = channel.Devicetypeid <= 0 ? (int)CaptureChannelType.emMsvChannel : channel.Devicetypeid,
                                 BackState = (emBackupFlag)channel.chn.Backupflag,
                                 CpSignalType = channel.chn.Cpsignaltype ?? 0,
                                 OrderCode = channel != null ? channel.Ordercode.GetValueOrDefault() : -1,
                                 GroupId = g != null ? g.Groupid : -1
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
                //lst = await (from channel in Context.DbpCapturechannels
                //             join device in Context.DbpCapturedevice on channel.Cpdeviceid equals device.Cpdeviceid into ps1
                //             join recout in Context.DbpRcdoutdesc on channel.Channelid equals recout.Channelid into ps2
                //             join grp in Context.DbpChannelgroupmap on channel.Cpdeviceid equals grp.Channelid into ps3
                //             join state in Context.DbpMsvchannelState on channel.Channelid equals state.Channelid into ps4
                //             from p1 in ps1.DefaultIfEmpty()
                //             from p2 in ps2.DefaultIfEmpty()
                //             from p3 in ps3.DefaultIfEmpty()
                //             from p4 in ps4.DefaultIfEmpty()
                //             where p2 != null
                //             select new CaptureChannelInfoDto
                //             {
                //                 Id = channel.Channelid,
                //                 Name = channel.Channelname,
                //                 Desc = channel.Channeldesc,
                //                 CpDeviceId = channel.Cpdeviceid,
                //                 ChannelIndex = channel.Channelindex ?? 0,
                //                 DeviceTypeId = p1.Devicetypeid <= 0 ? (int)CaptureChannelType.emMsvChannel : p1.Devicetypeid,
                //                 BackState = (emBackupFlag)channel.Backupflag,
                //                 CpSignalType = channel.Cpsignaltype ?? 0,
                //                 OrderCode = p1 != null ? p1.Ordercode.GetValueOrDefault() : -1,
                //                 GroupId = p3 != null ? p3.Groupid : -1,
                //                 DeviceState = p4 !=null ?(Device_State)p4.Devstate.GetValueOrDefault():Device_State.DISCONNECTTED
                //             }).ToListAsync();
                lst = await (from channel in Context.DbpCapturechannels.AsNoTracking().Join(Context.DbpCapturedevice.AsNoTracking(),
                              chn => chn.Cpdeviceid,
                              device => device.Cpdeviceid,
                              (chn, device) => new { chn, device.Devicetypeid, device.Ordercode }).Join(Context.DbpRcdoutdesc.AsNoTracking(),
                              chn => chn.chn.Channelid,
                              rcout => rcout.Channelid,
                              (rchn, rcout) => rchn).Join(Context.DbpMsvchannelState.AsNoTracking(),
                              chn => chn.chn.Channelid,
                              msvstate => msvstate.Channelid,
                              (rchn, msvstate) => new { rchn, msvstate.Devstate })
                             join grp in Context.DbpChannelgroupmap.AsNoTracking() on channel.rchn.chn.Channelid equals grp.Channelid into pg
                             from g in pg.DefaultIfEmpty()
                             select new CaptureChannelInfoDto
                             {
                                 Id = channel.rchn.chn.Channelid,
                                 Name = channel.rchn.chn.Channelname,
                                 Desc = channel.rchn.chn.Channeldesc,
                                 CpDeviceId = channel.rchn.chn.Cpdeviceid,
                                 ChannelIndex = channel.rchn.chn.Channelindex ?? 0,
                                 DeviceTypeId = channel.rchn.chn.Devicetypeid <= 0 ? (int)CaptureChannelType.emMsvChannel : channel.rchn.Devicetypeid,
                                 BackState = (emBackupFlag)channel.rchn.chn.Backupflag,
                                 CpSignalType = channel.rchn.chn.Cpsignaltype ?? 0,
                                 OrderCode = channel.rchn.chn != null ? channel.rchn.Ordercode.GetValueOrDefault() : -1,
                                 GroupId = g != null ? g.Groupid : -1,
                                 DeviceState = channel.Devstate != null ? (Device_State)channel.Devstate : Device_State.DISCONNECTTED
                             }).ToListAsync();
            }
            else
            {
                //lst = await (from channel in Context.DbpCapturechannels
                //             join device in Context.DbpCapturedevice on channel.Cpdeviceid equals device.Cpdeviceid into ps1
                //             join recout in Context.DbpRcdoutdesc on channel.Channelid equals recout.Channelid into psc
                //             join cstatu in Context.DbpMsvchannelState on channel.Channelid equals cstatu.Channelid into ps2
                //             join grp in Context.DbpChannelgroupmap on channel.Cpdeviceid equals grp.Channelid into ps3
                //             from p1 in ps1.DefaultIfEmpty()
                //             from p2 in ps2.DefaultIfEmpty()
                //             from p3 in ps3.DefaultIfEmpty()
                //             where psc!=null && p2 != null && p2.Devstate != (int)Device_State.DISCONNECTTED && p2.Msvmode != (int)MSV_Mode.LOCAL
                //             select new CaptureChannelInfoDto
                //             {
                //                 Id = channel.Channelid,
                //                 Name = channel.Channelname,
                //                 Desc = channel.Channeldesc,
                //                 CpDeviceId = channel.Cpdeviceid,
                //                 ChannelIndex = channel.Channelindex ?? 0,
                //                 DeviceTypeId = p1.Devicetypeid <= 0 ? (int)CaptureChannelType.emMsvChannel : p1.Devicetypeid,
                //                 BackState = (emBackupFlag)channel.Backupflag,
                //                 CpSignalType = channel.Cpsignaltype ?? 0,
                //                 OrderCode = p1 != null ? p1.Ordercode.GetValueOrDefault() : -1,
                //                 GroupId = p3 != null ? p3.Groupid : -1
                //             }).ToListAsync();

                lst = await (from channel in Context.DbpCapturechannels.AsNoTracking().Join(Context.DbpCapturedevice.AsNoTracking(),
                                  chn => chn.Cpdeviceid,
                                  device => device.Cpdeviceid,
                                  (chn, device) => new { chn, device.Devicetypeid, device.Ordercode }).Join(Context.DbpRcdoutdesc.AsNoTracking(),
                                  chn => chn.chn.Channelid,
                                  rcout => rcout.Channelid,
                                  (rchn, rcout) => rchn).Join(Context.DbpMsvchannelState.AsNoTracking(),
                                  chn => chn.chn.Channelid,
                                  msvstate => msvstate.Channelid,
                                  (rchn, msvstate) => new { rchn, msvstate.Devstate, msvstate.Msvmode }).Where(x => x.Devstate != (int)Device_State.DISCONNECTTED && x.Msvmode != (int)MSV_Mode.LOCAL)
                             join grp in Context.DbpChannelgroupmap.AsNoTracking() on channel.rchn.chn.Channelid equals grp.Channelid into pg
                             from g in pg.DefaultIfEmpty()
                             select new CaptureChannelInfoDto
                             {
                                 Id = channel.rchn.chn.Channelid,
                                 Name = channel.rchn.chn.Channelname,
                                 Desc = channel.rchn.chn.Channeldesc,
                                 CpDeviceId = channel.rchn.chn.Cpdeviceid,
                                 ChannelIndex = channel.rchn.chn.Channelindex ?? 0,
                                 DeviceTypeId = channel.rchn.chn.Devicetypeid <= 0 ? (int)CaptureChannelType.emMsvChannel : channel.rchn.Devicetypeid,
                                 BackState = (emBackupFlag)channel.rchn.chn.Backupflag,
                                 CpSignalType = channel.rchn.chn.Cpsignaltype ?? 0,
                                 OrderCode = channel.rchn.chn != null ? channel.rchn.Ordercode.GetValueOrDefault() : -1,
                                 GroupId = g != null ? g.Groupid : -1
                                 //DeviceState = (Device_State)channel.Devstate
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
            return Context.DbpChannelRecmap.Where(x => x.Channelid == channel).SingleOrDefaultAsync();
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
            await Context.SaveChangesAsync();
            return 1;
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
                                                     (state, port) => port.Virtualinport)
                                               .Join(Context.DbpVirtualmatrixinport.AsNoTracking(),
                                                     inport => inport,
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
            return QueryModelAsync(Context.DbpUsersetting, query, notrack);
        }


        public Task<List<TResult>> GetXdcamDeviceListAsync<TResult>(Func<IQueryable<DbpXdcamDevice>, IQueryable<TResult>> query, bool notrack = false)
        {
            return QueryListAsync(Context.DbpXdcamDevice, query, notrack);
        }

        #region 3.0

        public async Task<List<DbpMsvchannelState>> GetMsvchannelStateBySiteAsync(string site)
        {
            IQueryable<DbpCapturechannels> chns = null;
            if (!string.IsNullOrEmpty(site))
            {
                chns = Context.DbpCapturechannels.Where(x => x.SystemSite == site);
            }
            else
            {
                chns = Context.DbpCapturechannels.Where(x => 1 == 1);
            }

            var query = from channels in chns
                        join devices in Context.DbpCapturedevice
                                   on channels.Cpdeviceid equals devices.Cpdeviceid into chDevice
                        from devices in chDevice.DefaultIfEmpty()
                        join msvstate in Context.DbpMsvchannelState
                            on devices.Cpdeviceid equals msvstate.DeviceId into devMsvstate
                        from msvstate in devMsvstate.DefaultIfEmpty()
                        select new DbpMsvchannelState
                        {
                            Channelid = channels.Channelid,
                            Devstate = msvstate != null ? msvstate.Devstate : 1,
                            Msvmode = msvstate != null ? msvstate.Msvmode : 1,
                            Sourcevtrid = msvstate != null ? msvstate.Sourcevtrid : 0,
                            Curusercode = msvstate != null ? msvstate.Curusercode : "",
                            Kamatakiinfo = msvstate != null ? msvstate.Kamatakiinfo : "",
                            Uploadstate = msvstate != null ? msvstate.Uploadstate : 1
                        };

            return await query.AsNoTracking().ToListAsync();
        }

        private IQueryable<CaptureChannelInfoExDto> GetChannelQuery(int status, string site, int area)
        {
            IQueryable<CaptureChannelInfoExDto> query = null;

            var channeltemp = Context.DbpCapturechannels.AsNoTracking().Join(Context.DbpRcdoutdesc.AsNoTracking(),
                               chn => chn.Channelid,
                               rout => rout.Channelid,
                               (chn, rout) => new { chn, rout.Area, rout.SystemSite });

            if (!string.IsNullOrEmpty(site))
            {
                channeltemp = channeltemp.Where(x => x.SystemSite == site);
            }

            if (area > 0)
            {
                channeltemp = channeltemp.Where(x => x.Area == area);
            }

            var devicestemp = Context.DbpCapturedevice.AsNoTracking().Join(Context.DbpMsvchannelState.AsNoTracking(),
                    devs => devs.Cpdeviceid,
                    msvstate => msvstate.DeviceId,
                    (device, msvs) => new { device, msvs });

            if (status == 1)
            {
                devicestemp = devicestemp.Where(x => x.msvs.Devstate != (int)Device_State.DISCONNECTTED && x.msvs.Msvmode != (int)MSV_Mode.LOCAL);
            }

            query = from channel in channeltemp
                    join devices in devicestemp
                    on channel.chn.Cpdeviceid equals devices.device.Cpdeviceid into devres
                    from dev in devres.DefaultIfEmpty()
                    join grp in Context.DbpChannelgroupmap.AsNoTracking() on channel.chn.Channelid equals grp.Channelid into pg
                    from g in pg.DefaultIfEmpty()
                    select new CaptureChannelInfoExDto
                    {
                        Id = channel.chn.Channelid,
                        Name = channel.chn.Channelname,
                        Desc = channel.chn.Channeldesc,
                        CpDeviceId = channel.chn.Cpdeviceid,
                        ChannelIndex = channel.chn.Channelindex ?? 0,
                        DeviceTypeId = channel.chn.Devicetypeid <= 0 ? (int)CaptureChannelType.emMsvChannel : channel.chn.Devicetypeid,
                        BackState = (emBackupFlag)channel.chn.Backupflag,
                        CpSignalType = channel.chn.Cpsignaltype ?? 0,
                        OrderCode = dev != null && dev.device != null && dev.device.Ordercode != null ? dev.device.Ordercode.GetValueOrDefault() : -1,
                        GroupId = g != null ? g.Groupid : -1,
                        DeviceState = dev != null && dev.msvs != null ? (Device_State)dev.msvs.Devstate : Device_State.CONNECTED,
                        Area = channel.Area != null ? (int)channel.Area : -1,
                        SystemSite = channel.SystemSite
                    };

            return query;
        }

        public async Task<List<CaptureChannelInfoExDto>> GetAllChannelsBySiteAreaAsync(int status, string site, int area)
        {
            //device按nOrderCode排升序
            //channel 按RECOUTIDX排序
            List<CaptureChannelInfoExDto> lst = null;

            var query = GetChannelQuery(status, site, area);
            lst = await query.AsNoTracking().ToListAsync();

            if (lst == null || lst.Count < 1)
            {
                Logger.Error("GetAllCaptureChannelsAsync Site lst error");
                return null;
            }

            return lst.OrderBy(x => x.OrderCode).ToList();
        }


        public async Task<List<ProgrammeInfoExDto>> GetAllProgrammeInfoBySiteAsync(string site)
        {

            var query = await (from sig in Context.DbpSignalsrc.AsNoTracking().Join(Context.DbpRcdindesc.AsNoTracking(),
                src => src.Signalsrcid,
                rcin => rcin.Signalsrcid,
                (src, rcin) => new { src, rcin.Signalsource, rcin.SystemSite, rcin.Area }).Where(x => x.SystemSite == site)
                               join grp in Context.DbpSignalsrcgroupmap.AsNoTracking() on sig.src.Signalsrcid equals grp.Signalsrcid into pg
                               from g in pg.DefaultIfEmpty()
                               select new ProgrammeInfoExDto
                               {
                                   ProgrammeId = sig.src.Signalsrcid,
                                   ProgrammeName = sig.src.Name,
                                   ProgrammeDesc = sig.src.Signaldesc,
                                   TypeId = sig.src.Signaltypeid,
                                   PgmType = ProgrammeType.PT_SDI,
                                   ImageType = (ImageType)sig.src.Imagetype,
                                   PureAudio = sig.src.Pureaudio ?? 0,
                                   SignalSourceType = sig.Signalsource == null ? 0 : (emSignalSource)sig.Signalsource.GetValueOrDefault(),
                                   GroupId = g == null ? 0 : g.Groupid,
                                   Area = sig.Area != null ? (int)sig.Area : -1
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

        public async Task<ProgrammeInfoExDto> GetSignalInfoOnAreaSiteAsync(int srcid)
        {

            var query = await (from sig in Context.DbpSignalsrc.AsNoTracking().Join(Context.DbpRcdindesc.AsNoTracking(),
                src => src.Signalsrcid,
                rcin => rcin.Signalsrcid,
                (src, rcin) => new { src, rcin.Signalsource, rcin.Area, rcin.SystemSite })
                               where srcid == sig.src.Signalsrcid
                               join grp in Context.DbpSignalsrcgroupmap.AsNoTracking() on sig.src.Signalsrcid equals grp.Signalsrcid into pg
                               from g in pg.DefaultIfEmpty()
                               select new ProgrammeInfoExDto
                               {
                                   ProgrammeId = sig.src.Signalsrcid,
                                   ProgrammeName = sig.src.Name,
                                   ProgrammeDesc = sig.src.Signaldesc,
                                   TypeId = sig.src.Signaltypeid,
                                   PgmType = ProgrammeType.PT_SDI,
                                   ImageType = (ImageType)sig.src.Imagetype,
                                   PureAudio = sig.src.Pureaudio ?? 0,
                                   SignalSourceType = sig.Signalsource == null ? 0 : (emSignalSource)sig.Signalsource.GetValueOrDefault(),
                                   GroupId = g == null ? 0 : g.Groupid,
                                   Area = sig.Area != null ? (int)sig.Area : -1,
                                   SystemSite = sig.SystemSite
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


        public Task<List<DbpSignalsrc>> GetAllSignalsrcForRcdinBySiteAsync(string site, bool notrack = false)
        {
            return QueryListAsync(Context.DbpSignalsrc,
                                        a => a.Join(Context.DbpRcdindesc.Where(x => x.SystemSite == site).Select(rcdin => rcdin.Signalsrcid),
                                                    src => src.Signalsrcid,
                                                    rcdin => rcdin,
                                                    (src, rcdin) => src),
                                        notrack);
        }

        public async Task<CaptureChannelInfoDto> GetSiteCaptureChannelByIDAsync(int channelid)
        {
            var lst = await (from channel in Context.DbpCapturechannels.AsNoTracking()
                             join device in Context.DbpCapturedevice.AsNoTracking() on channel.Cpdeviceid equals device.Cpdeviceid into devices
                             from dev in devices.DefaultIfEmpty()
                             where channelid == channel.Channelid
                             join grp in Context.DbpChannelgroupmap.AsNoTracking() on channel.Channelid equals grp.Channelid into pg
                             from g in pg.DefaultIfEmpty()
                             select new CaptureChannelInfoDto
                             {
                                 Id = channel.Channelid,
                                 Name = channel.Channelname,
                                 Desc = channel.Channeldesc,
                                 CpDeviceId = channel.Cpdeviceid,
                                 ChannelIndex = channel.Channelindex ?? 0,
                                 DeviceTypeId = channel.Devicetypeid <= 0 ? (int)CaptureChannelType.emMsvChannel : channel.Devicetypeid,
                                 BackState = (emBackupFlag)channel.Backupflag,
                                 CpSignalType = channel.Cpsignaltype ?? 0,
                                 OrderCode = dev != null ? dev.Ordercode.GetValueOrDefault() : -1,
                                 GroupId = g != null ? g.Groupid : -1
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

        #endregion

    }
}
