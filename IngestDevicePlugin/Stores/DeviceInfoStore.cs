using System;
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

        public async Task<List<DbpRcdindesc>> GetAllRouterInPortInfoAsync<TResult>(Func<IQueryable<DbpRcdindesc>, IQueryable<TResult>> query, bool notrack = false)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            if (notrack)
            {
                return await Context.DbpRcdindesc.AsNoTracking().ToListAsync();
            }
            return await Context.DbpRcdindesc.ToListAsync();
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

        //老接口GetAllCaptureChannels
        public async Task<List<CaptureChannelInfoDto>> GetAllCaptureChannelsAsync()
        {
            //device按nOrderCode排升序
            //channel 按RECOUTIDX排序
            var lst = await (from channel in Context.DbpCapturechannels
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
                 OrderCode = p1 != null? p1.Ordercode.GetValueOrDefault() :-1,
                 GroupID = p3 != null ? p3.Groupid:-1
             }).ToListAsync();

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

        public async Task<List<int>> GetChannelIdsBySignalIdForNotMatrix(int signalid)
        {
            return await Context.DbpRcdindesc.Join(Context.DbpRcdoutdesc,
                rcin => rcin.Recinidx,
                rcout => rcout.Recoutidx,
                (rcin, rcout) => new { ID = rcin.Signalsrcid, Channel=rcout.Channelid})
                .Where(x => x.ID == signalid).Select(f => f.Channel).ToListAsync();

        }

        ////////////////

    }
}
