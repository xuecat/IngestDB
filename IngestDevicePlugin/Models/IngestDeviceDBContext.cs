using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace IngestDevicePlugin.Models
{

    public partial class IngestDeviceDBContext : DbContext
    {
        public IngestDeviceDBContext(DbContextOptions<IngestDeviceDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<DbpCapturechannels> DbpCapturechannels { get; set; }
        public virtual DbSet<DbpCapturedevice> DbpCapturedevice { get; set; }
        public virtual DbSet<DbpChannelgroupmap> DbpChannelgroupmap { get; set; }
        public virtual DbSet<DbpChannelRecmap> DbpChannelRecmap { get; set; }
        public virtual DbSet<DbpChnExtenddata> DbpChnExtenddata { get; set; }

        public virtual DbSet<DbpGpiInfo> DbpGpiInfo { get; set; }
        public virtual DbSet<DbpGpiMap> DbpGpiMap { get; set; }
        public virtual DbSet<DbpIpDatachannelinfo> DbpIpDatachannelinfo { get; set; }
        public virtual DbSet<DbpIpDevice> DbpIpDevice { get; set; }
        public virtual DbSet<DbpIpProgramme> DbpIpProgramme { get; set; }
        public virtual DbSet<DbpIpVirtualchannel> DbpIpVirtualchannel { get; set; }
        public virtual DbSet<DbpMatrixinfo> DbpMatrixinfo { get; set; }
        public virtual DbSet<DbpMatrixrout> DbpMatrixrout { get; set; }
        public virtual DbSet<DbpMatrixtypeinfo> DbpMatrixtypeinfo { get; set; }
        public virtual DbSet<DbpProgramparamMap> DbpProgramparamMap { get; set; }
        public virtual DbSet<DbpRcdindesc> DbpRcdindesc { get; set; }
        public virtual DbSet<DbpRcdoutdesc> DbpRcdoutdesc { get; set; }
        public virtual DbSet<DbpSignalDeviceMap> DbpSignalDeviceMap { get; set; }
        public virtual DbSet<DbpSignalgroup> DbpSignalgroup { get; set; }
        public virtual DbSet<DbpSignalRecmap> DbpSignalRecmap { get; set; }
        public virtual DbSet<DbpSignalSource> DbpSignalSource { get; set; }
        public virtual DbSet<DbpSignalsrc> DbpSignalsrc { get; set; }
        public virtual DbSet<DbpSignalsrcgroupmap> DbpSignalsrcgroupmap { get; set; }
        public virtual DbSet<DbpSignalsrcMasterbackup> DbpSignalsrcMasterbackup { get; set; }
        public virtual DbSet<DbpSignalType> DbpSignalType { get; set; }
        public virtual DbSet<DbpSigRecTypeMap> DbpSigRecTypeMap { get; set; }
        public virtual DbSet<DbpStreammedia> DbpStreammedia { get; set; }
        public virtual DbSet<DbpUsersettings> DbpUsersetting { get; set; }
        public virtual DbSet<DbpVirtualmatrixinport> DbpVirtualmatrixinport { get; set; }
        public virtual DbSet<DbpVirtualmatrixportstate> DbpVirtualmatrixportstate { get; set; }
        public virtual DbSet<DbpXdcamDevice> DbpXdcamDevice { get; set; }
        public virtual DbSet<DbpXdcamDevPlanMap> DbpXdcamDevPlanMap { get; set; }
        public virtual DbSet<DbpXdcamDiscMaterial> DbpXdcamDiscMaterial { get; set; }
        public virtual DbSet<DbpXdcamDiskinfo> DbpXdcamDiskinfo { get; set; }
        public virtual DbSet<DbpXdcamMaterialDevMap> DbpXdcamMaterialDevMap { get; set; }

        public virtual DbSet<VtrDetailinfo> VtrDetailinfo { get; set; }
        public virtual DbSet<VtrDownloadMateriallist> VtrDownloadMateriallist { get; set; }
        public virtual DbSet<DbpMsvchannelState> DbpMsvchannelState { get; set; }
        public virtual DbSet<VtrTapelist> VtrTapelist { get; set; }
        public virtual DbSet<VtrTapeVtrMap> VtrTapeVtrMap { get; set; }

        public virtual DbSet<VtrTypeinfo> VtrTypeinfo { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DbpCapturechannels>(entity =>
            {
                entity.HasKey(e => e.Channelid);

                entity.ToTable("dbp_capturechannels");

                entity.Property(e => e.Channelid)
                    .HasColumnName("CHANNELID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Backupflag)
                    .HasColumnName("BACKUPFLAG")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Channeldesc)
                    .HasColumnName("CHANNELDESC")
                    .HasColumnType("varchar(64)")
                    .HasDefaultValueSql("''");

                entity.Property(e => e.Channelindex)
                    .HasColumnName("CHANNELINDEX")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Channelname)
                    .HasColumnName("CHANNELNAME")
                    .HasColumnType("varchar(64)")
                    .HasDefaultValueSql("''");

                entity.Property(e => e.Cpdeviceid)
                    .HasColumnName("CPDEVICEID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Cpsignaltype)
                    .HasColumnName("CPSIGNALTYPE")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Devicetypeid)
                    .HasColumnName("DEVICETYPEID")
                    .HasColumnType("int(11)");
            });

            modelBuilder.Entity<DbpCapturedevice>(entity =>
            {
                entity.HasKey(e => e.Cpdeviceid);

                entity.ToTable("dbp_capturedevice");

                entity.Property(e => e.Cpdeviceid)
                    .HasColumnName("CPDEVICEID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Devicename)
                    .HasColumnName("DEVICENAME")
                    .HasColumnType("varchar(64)")
                    .HasDefaultValueSql("''");

                entity.Property(e => e.Devicetypeid)
                    .HasColumnName("DEVICETYPEID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Ipaddress)
                    .HasColumnName("IPADDRESS")
                    .HasColumnType("varchar(64)")
                    .HasDefaultValueSql("''");

                entity.Property(e => e.Ordercode)
                    .HasColumnName("ORDERCODE")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");
            });


            modelBuilder.Entity<DbpChannelgroupmap>(entity =>
            {
                entity.HasKey(e => e.Channelid);

                entity.ToTable("dbp_channelgroupmap");

                entity.Property(e => e.Channelid)
                    .HasColumnName("CHANNELID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Groupid)
                    .HasColumnName("GROUPID")
                    .HasColumnType("int(11)");
            });

            modelBuilder.Entity<DbpChannelRecmap>(entity =>
            {
                entity.HasKey(e => new { e.Channelid, e.Recid });

                entity.ToTable("dbp_channel_recmap");

                entity.Property(e => e.Channelid)
                    .HasColumnName("CHANNELID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Recid)
                    .HasColumnName("RECID")
                    .HasColumnType("int(11)");
            });

            modelBuilder.Entity<DbpChnExtenddata>(entity =>
            {
                entity.HasKey(e => e.Channaelid);

                entity.ToTable("dbp_chn_extenddata");

                entity.Property(e => e.Channaelid)
                    .HasColumnName("CHANNAELID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Datatype)
                    .HasColumnName("DATATYPE")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Extenddata)
                    .HasColumnName("EXTENDDATA")
                    .HasColumnType("varchar(4000)");
            });

            modelBuilder.Entity<DbpGpiInfo>(entity =>
            {
                entity.HasKey(e => e.Gpiid);

                entity.ToTable("dbp_gpi_info");

                entity.Property(e => e.Gpiid)
                    .HasColumnName("GPIID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Comport)
                    .HasColumnName("COMPORT")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Description)
                    .HasColumnName("DESCRIPTION")
                    .HasColumnType("varchar(1000)");

                entity.Property(e => e.Gpiname)
                    .HasColumnName("GPINAME")
                    .HasColumnType("varchar(256)");

                entity.Property(e => e.Outputportcount)
                    .HasColumnName("OUTPUTPORTCOUNT")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");
            });

            modelBuilder.Entity<DbpGpiMap>(entity =>
            {
                entity.HasKey(e => new { e.Gpiid, e.Gpioutputport });

                entity.ToTable("dbp_gpi_map");

                entity.Property(e => e.Gpiid)
                    .HasColumnName("GPIID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Gpioutputport)
                    .HasColumnName("GPIOUTPUTPORT")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Avoutputport)
                    .HasColumnName("AVOUTPUTPORT")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Captureparamid)
                    .HasColumnName("CAPTUREPARAMID")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");
            });

            modelBuilder.Entity<DbpIpDatachannelinfo>(entity =>
            {
                entity.HasKey(e => e.Datachannelid);

                entity.ToTable("dbp_ip_datachannelinfo");

                entity.Property(e => e.Datachannelid)
                    .HasColumnName("DATACHANNELID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Datachannelindex)
                    .HasColumnName("DATACHANNELINDEX")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Datachannelname)
                    .HasColumnName("DATACHANNELNAME")
                    .HasColumnType("varchar(128)");

                entity.Property(e => e.Deviceid)
                    .HasColumnName("DEVICEID")
                    .HasColumnType("int(11)");
            });

            modelBuilder.Entity<DbpIpDevice>(entity =>
            {
                entity.HasKey(e => e.Deviceid);

                entity.ToTable("dbp_ip_device");

                entity.Property(e => e.Deviceid)
                    .HasColumnName("DEVICEID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Devicedesc)
                    .HasColumnName("DEVICEDESC")
                    .HasColumnType("varchar(1024)");

                entity.Property(e => e.Devicename)
                    .HasColumnName("DEVICENAME")
                    .HasColumnType("varchar(128)");

                entity.Property(e => e.Ipaddress)
                    .HasColumnName("IPADDRESS")
                    .HasColumnType("varchar(64)");

                entity.Property(e => e.Port)
                    .HasColumnName("PORT")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");
            });

            modelBuilder.Entity<DbpIpProgramme>(entity =>
            {
                entity.HasKey(e => e.Programmeid);

                entity.ToTable("dbp_ip_programme");

                entity.Property(e => e.Programmeid)
                    .HasColumnName("PROGRAMMEID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Datachannelid)
                    .HasColumnName("DATACHANNELID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Extendparams)
                    .HasColumnName("EXTENDPARAMS")
                    .HasColumnType("varchar(2048)")
                    .HasDefaultValueSql("''");

                entity.Property(e => e.Imagetype)
                    .HasColumnName("IMAGETYPE")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Multicastip)
                    .HasColumnName("MULTICASTIP")
                    .HasColumnType("varchar(128)")
                    .HasDefaultValueSql("''");

                entity.Property(e => e.Multicastport)
                    .HasColumnName("MULTICASTPORT")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Programmedesc)
                    .HasColumnName("PROGRAMMEDESC")
                    .HasColumnType("varchar(1024)");

                entity.Property(e => e.Programmeindex)
                    .HasColumnName("PROGRAMMEINDEX")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Programmename)
                    .HasColumnName("PROGRAMMENAME")
                    .HasColumnType("varchar(128)");

                entity.Property(e => e.Programmetype)
                    .HasColumnName("PROGRAMMETYPE")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Pureaudio)
                    .HasColumnName("PUREAUDIO")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Tssignalinfo)
                    .HasColumnName("TSSIGNALINFO")
                    .HasColumnType("varchar(1024)");
            });

            modelBuilder.Entity<DbpIpVirtualchannel>(entity =>
            {
                entity.HasKey(e => e.Channelid);

                entity.ToTable("dbp_ip_virtualchannel");

                entity.Property(e => e.Channelid)
                    .HasColumnName("CHANNELID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Backuptype)
                    .HasColumnName("BACKUPTYPE")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Carrierid)
                    .HasColumnName("CARRIERID")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Channeldesc)
                    .HasColumnName("CHANNELDESC")
                    .HasColumnType("varchar(1024)");

                entity.Property(e => e.Channelname)
                    .HasColumnName("CHANNELNAME")
                    .HasColumnType("varchar(128)");

                entity.Property(e => e.Channelstatus)
                    .HasColumnName("CHANNELSTATUS")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Channeltype)
                    .HasColumnName("CHANNELTYPE")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'2'");

                entity.Property(e => e.Cpsignaltype)
                    .HasColumnName("CPSIGNALTYPE")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Ctrlport)
                    .HasColumnName("CTRLPORT")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Deviceid)
                    .HasColumnName("DEVICEID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Deviceindex)
                    .HasColumnName("DEVICEINDEX")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Ipaddress)
                    .HasColumnName("IPADDRESS")
                    .HasColumnType("varchar(64)");
            });

            modelBuilder.Entity<DbpMatrixinfo>(entity =>
            {
                entity.HasKey(e => e.Matrixid);

                entity.ToTable("dbp_matrixinfo");

                entity.Property(e => e.Matrixid)
                    .HasColumnName("MATRIXID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Comport)
                    .HasColumnName("COMPORT")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Comportbaud)
                    .HasColumnName("COMPORTBAUD")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Inportnum)
                    .HasColumnName("INPORTNUM")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Matrixname)
                    .IsRequired()
                    .HasColumnName("MATRIXNAME")
                    .HasColumnType("varchar(512)");

                entity.Property(e => e.Matrixtypeid)
                    .HasColumnName("MATRIXTYPEID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Otherparam)
                    .HasColumnName("OTHERPARAM")
                    .HasColumnType("varchar(4000)")
                    .HasDefaultValueSql("''");

                entity.Property(e => e.Outportnum)
                    .HasColumnName("OUTPORTNUM")
                    .HasColumnType("int(11)");
            });

            modelBuilder.Entity<DbpMatrixrout>(entity =>
            {
                entity.HasKey(e => new { e.Matrixid, e.Virtualoutport });

                entity.ToTable("dbp_matrixrout");

                entity.Property(e => e.Matrixid)
                    .HasColumnName("MATRIXID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Virtualoutport)
                    .HasColumnName("VIRTUALOUTPORT")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Begintime)
                    .HasColumnName("BEGINTIME")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("'CURRENT_TIMESTAMP'");

                entity.Property(e => e.Endtime)
                    .HasColumnName("ENDTIME")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("'CURRENT_TIMESTAMP'");

                entity.Property(e => e.Inport)
                    .HasColumnName("INPORT")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Outport)
                    .HasColumnName("OUTPORT")
                    .HasColumnType("int(11)");

                entity.Property(e => e.State)
                    .HasColumnName("STATE")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Virtualinport)
                    .HasColumnName("VIRTUALINPORT")
                    .HasColumnType("int(11)");
            });

            modelBuilder.Entity<DbpMatrixtypeinfo>(entity =>
            {
                entity.HasKey(e => e.Matrixtypeid);

                entity.ToTable("dbp_matrixtypeinfo");

                entity.Property(e => e.Matrixtypeid)
                    .HasColumnName("MATRIXTYPEID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Matrixtypename)
                    .IsRequired()
                    .HasColumnName("MATRIXTYPENAME")
                    .HasColumnType("varchar(256)");
            });

            modelBuilder.Entity<DbpProgramparamMap>(entity =>
            {
                entity.HasKey(e => e.Programid);

                entity.ToTable("dbp_programparam_map");

                entity.Property(e => e.Programid)
                    .HasColumnName("PROGRAMID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Paramid)
                    .HasColumnName("PARAMID")
                    .HasColumnType("int(11)");
            });

            modelBuilder.Entity<DbpRcdindesc>(entity =>
            {
                entity.HasKey(e => new { e.Signalsrcid, e.Recinidx, e.Rcdeviceid });

                entity.ToTable("dbp_rcdindesc");

                entity.Property(e => e.Signalsrcid)
                    .HasColumnName("SIGNALSRCID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Recinidx)
                    .HasColumnName("RECINIDX")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Rcdeviceid)
                    .HasColumnName("RCDEVICEID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Signalsource)
                    .HasColumnName("SIGNALSOURCE")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Area)
                    .HasColumnName("AREA")
                    .HasColumnType("int(11)");

            });

            modelBuilder.Entity<DbpRcdoutdesc>(entity =>
            {
                entity.HasKey(e => new { e.Recoutidx, e.Rcdeviceid });

                entity.ToTable("dbp_rcdoutdesc");

                entity.Property(e => e.Recoutidx)
                    .HasColumnName("RECOUTIDX")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Rcdeviceid)
                    .HasColumnName("RCDEVICEID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Channelid)
                    .HasColumnName("CHANNELID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Devicetype)
                    .HasColumnName("DEVICETYPE")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Area)
                    .HasColumnName("AREA")
                    .HasColumnType("int(11)");

            });

            modelBuilder.Entity<DbpSignalDeviceMap>(entity =>
            {
                entity.HasKey(e => e.Signalsrcid);

                entity.ToTable("dbp_signal_device_map");

                entity.Property(e => e.Signalsrcid)
                    .HasColumnName("SIGNALSRCID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Deviceid)
                    .HasColumnName("DEVICEID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Deviceoutportidx)
                    .HasColumnName("DEVICEOUTPORTIDX")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Signalsource)
                    .HasColumnName("SIGNALSOURCE")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");
            });

            modelBuilder.Entity<DbpSignalgroup>(entity =>
            {
                entity.HasKey(e => e.Groupid);

                entity.ToTable("dbp_signalgroup");

                entity.Property(e => e.Groupid)
                    .HasColumnName("GROUPID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Groupdesc)
                    .HasColumnName("GROUPDESC")
                    .HasColumnType("varchar(128)")
                    .HasDefaultValueSql("''");

                entity.Property(e => e.Groupname)
                    .HasColumnName("GROUPNAME")
                    .HasColumnType("varchar(128)")
                    .HasDefaultValueSql("''");
            });

            modelBuilder.Entity<DbpSignalRecmap>(entity =>
            {
                entity.HasKey(e => new { e.Signalsrcid, e.Recid });

                entity.ToTable("dbp_signal_recmap");

                entity.Property(e => e.Signalsrcid)
                    .HasColumnName("SIGNALSRCID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Recid)
                    .HasColumnName("RECID")
                    .HasColumnType("int(11)");
            });

            modelBuilder.Entity<DbpSignalSource>(entity =>
            {
                entity.HasKey(e => e.Signalsource);

                entity.ToTable("dbp_signal_source");

                entity.Property(e => e.Signalsource)
                    .HasColumnName("SIGNALSOURCE")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Sourcename)
                    .IsRequired()
                    .HasColumnName("SOURCENAME")
                    .HasColumnType("varchar(32)");
            });

            modelBuilder.Entity<DbpSignalsrc>(entity =>
            {
                entity.HasKey(e => e.Signalsrcid);

                entity.ToTable("dbp_signalsrc");

                entity.Property(e => e.Signalsrcid)
                    .HasColumnName("SIGNALSRCID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Imagetype)
                    .HasColumnName("IMAGETYPE")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Name)
                    .HasColumnName("NAME")
                    .HasColumnType("varchar(128)")
                    .HasDefaultValueSql("''");

                entity.Property(e => e.Pureaudio)
                    .HasColumnName("PUREAUDIO")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Signaldesc)
                    .HasColumnName("SIGNALDESC")
                    .HasColumnType("varchar(1024)")
                    .HasDefaultValueSql("''");

                entity.Property(e => e.Signaltypeid)
                    .HasColumnName("SIGNALTYPEID")
                    .HasColumnType("int(11)");
            });

            modelBuilder.Entity<DbpSignalsrcgroupmap>(entity =>
            {
                entity.HasKey(e => e.Signalsrcid);

                entity.ToTable("dbp_signalsrcgroupmap");

                entity.Property(e => e.Signalsrcid)
                    .HasColumnName("SIGNALSRCID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Groupid)
                    .HasColumnName("GROUPID")
                    .HasColumnType("int(11)");
            });

            modelBuilder.Entity<DbpSignalsrcMasterbackup>(entity =>
            {
                entity.HasKey(e => e.Signalsrcid);

                entity.ToTable("dbp_signalsrc_masterbackup");

                entity.Property(e => e.Signalsrcid)
                    .HasColumnName("SIGNALSRCID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Ismastersrc)
                    .HasColumnName("ISMASTERSRC")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'1'");

                entity.Property(e => e.Mastersignalsrcid)
                    .HasColumnName("MASTERSIGNALSRCID")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Signalsrctype)
                    .HasColumnName("SIGNALSRCTYPE")
                    .HasColumnType("int(11)");
            });

            modelBuilder.Entity<DbpSignalsrcMasterbackupCopy>(entity =>
            {
                entity.HasKey(e => e.Signalsrcid);

                entity.ToTable("dbp_signalsrc_masterbackup_copy");

                entity.Property(e => e.Signalsrcid)
                    .HasColumnName("SIGNALSRCID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Ismastersrc)
                    .HasColumnName("ISMASTERSRC")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'1'");

                entity.Property(e => e.Mastersignalsrcid)
                    .HasColumnName("MASTERSIGNALSRCID")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Signalsrctype)
                    .HasColumnName("SIGNALSRCTYPE")
                    .HasColumnType("int(11)");
            });

            modelBuilder.Entity<DbpSignalType>(entity =>
            {
                entity.HasKey(e => e.Signaltypeid);

                entity.ToTable("dbp_signal_type");

                entity.Property(e => e.Signaltypeid)
                    .HasColumnName("SIGNALTYPEID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.SignalTypeDesc)
                    .HasColumnName("SIGNAL_TYPE_DESC")
                    .HasColumnType("varchar(128)")
                    .HasDefaultValueSql("''");

                entity.Property(e => e.Signaltype)
                    .HasColumnName("SIGNALTYPE")
                    .HasColumnType("varchar(64)")
                    .HasDefaultValueSql("''");
            });

            modelBuilder.Entity<DbpSigRecTypeMap>(entity =>
            {
                entity.HasKey(e => new { e.Signaltypeid, e.Devicetypeid });

                entity.ToTable("dbp_sig_rec_type_map");

                entity.Property(e => e.Signaltypeid)
                    .HasColumnName("SIGNALTYPEID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Devicetypeid)
                    .HasColumnName("DEVICETYPEID")
                    .HasColumnType("int(11)");
            });

            modelBuilder.Entity<DbpStreammedia>(entity =>
            {
                entity.HasKey(e => e.Streammediaid);

                entity.ToTable("dbp_streammedia");

                entity.Property(e => e.Streammediaid)
                    .HasColumnName("STREAMMEDIAID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Carrierid)
                    .HasColumnName("CARRIERID")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Extendparams)
                    .HasColumnName("EXTENDPARAMS")
                    .HasColumnType("varchar(2048)")
                    .HasDefaultValueSql("''");

                entity.Property(e => e.Imagetype)
                    .HasColumnName("IMAGETYPE")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Pureaudio)
                    .HasColumnName("PUREAUDIO")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Streammediadesc)
                    .HasColumnName("STREAMMEDIADESC")
                    .HasColumnType("varchar(1024)");

                entity.Property(e => e.Streammedianame)
                    .HasColumnName("STREAMMEDIANAME")
                    .HasColumnType("varchar(128)");

                entity.Property(e => e.Streammediatype)
                    .HasColumnName("STREAMMEDIATYPE")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Streammediaurl)
                    .HasColumnName("STREAMMEDIAURL")
                    .HasColumnType("varchar(2048)");

                entity.Property(e => e.Urltype)
                    .HasColumnName("URLTYPE")
                    .HasColumnType("varchar(128)")
                    .HasDefaultValueSql("''");
            });

            modelBuilder.Entity<DbpUsersettings>(entity =>
            {
                entity.HasKey(e => new { e.Usercode, e.Settingtype });
                entity.ToTable("dbp_usersettings");
                entity.Property(e => e.Usercode).HasColumnName("USERCODE").HasColumnType("varchar(255)");
                entity.Property(e => e.Settingtype).HasColumnName("SETTINGTYPE").HasColumnType("varchar(128)");
                entity.Property(e => e.Settingtext).HasColumnName("SETTINGTEXT").HasColumnType("varchar(4000)");
                entity.Property(e => e.Settingtextlong).HasColumnName("SETTINGTEXTLONG").HasColumnType("text");
            });

            modelBuilder.Entity<DbpVirtualmatrixinport>(entity =>
            {
                entity.HasKey(e => e.Virtualinport);

                entity.ToTable("dbp_virtualmatrixinport");

                entity.Property(e => e.Virtualinport)
                    .HasColumnName("VIRTUALINPORT")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Matrixid)
                    .HasColumnName("MATRIXID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Signaltype)
                    .HasColumnName("SIGNALTYPE")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Sourcetype)
                    .HasColumnName("SOURCETYPE")
                    .HasColumnType("int(11)");
            });

            modelBuilder.Entity<DbpVirtualmatrixportstate>(entity =>
            {
                entity.HasKey(e => new { e.Virtualinport, e.Virtualoutport });

                entity.ToTable("dbp_virtualmatrixportstate");

                entity.HasIndex(e => e.State)
                    .HasName("IDX_VMPS_1");

                entity.Property(e => e.Virtualinport)
                    .HasColumnName("VIRTUALINPORT")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Virtualoutport)
                    .HasColumnName("VIRTUALOUTPORT")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Lastoprtime)
                    .HasColumnName("LASTOPRTIME")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("'CURRENT_TIMESTAMP'");

                entity.Property(e => e.Matrixid)
                    .HasColumnName("MATRIXID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.State)
                    .HasColumnName("STATE")
                    .HasColumnType("int(11)");
            });

            modelBuilder.Entity<DbpXdcamDevice>(entity =>
            {
                entity.HasKey(e => e.Deviceid);

                entity.ToTable("dbp_xdcam_device");

                entity.Property(e => e.Deviceid)
                    .HasColumnName("DEVICEID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Devicedesc)
                    .HasColumnName("DEVICEDESC")
                    .HasColumnType("varchar(128)")
                    .HasDefaultValueSql("''");

                entity.Property(e => e.Devicename)
                    .IsRequired()
                    .HasColumnName("DEVICENAME")
                    .HasColumnType("varchar(128)");

                entity.Property(e => e.Devicestate)
                    .HasColumnName("DEVICESTATE")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Devicetype)
                    .HasColumnName("DEVICETYPE")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Discid)
                    .HasColumnName("DISCID")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'-1'");

                entity.Property(e => e.Ftpaddress)
                    .IsRequired()
                    .HasColumnName("FTPADDRESS")
                    .HasColumnType("varchar(32)");

                entity.Property(e => e.Loginname)
                    .IsRequired()
                    .HasColumnName("LOGINNAME")
                    .HasColumnType("varchar(32)");

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasColumnName("PASSWORD")
                    .HasColumnType("varchar(20)");

                entity.Property(e => e.Serverip)
                    .HasColumnName("SERVERIP")
                    .HasColumnType("varchar(16)")
                    .HasDefaultValueSql("''");

                entity.Property(e => e.Storagepath)
                    .HasColumnName("STORAGEPATH")
                    .HasColumnType("varchar(512)");

                entity.Property(e => e.Workmode)
                    .HasColumnName("WORKMODE")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");
            });

            modelBuilder.Entity<DbpXdcamDevPlanMap>(entity =>
            {
                entity.HasKey(e => e.Deviceid);

                entity.ToTable("dbp_xdcam_dev_plan_map");

                entity.Property(e => e.Deviceid)
                    .HasColumnName("DEVICEID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Planningguid)
                    .IsRequired()
                    .HasColumnName("PLANNINGGUID")
                    .HasColumnType("varchar(128)");

                entity.Property(e => e.Planninginfo)
                    .HasColumnName("PLANNINGINFO")
                    .HasColumnType("text");
            });

            modelBuilder.Entity<DbpXdcamDiscMaterial>(entity =>
            {
                entity.ToTable("dbp_xdcam_disc_material");

                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .HasColumnType("varchar(128)");

                entity.Property(e => e.Duration)
                    .HasColumnName("DURATION")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Inpoint)
                    .HasColumnName("INPOINT")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Name)
                    .HasColumnName("NAME")
                    .HasColumnType("varchar(256)")
                    .HasDefaultValueSql("''");

                entity.Property(e => e.Outpoint)
                    .HasColumnName("OUTPOINT")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Planningguid)
                    .HasColumnName("PLANNINGGUID")
                    .HasColumnType("varchar(128)")
                    .HasDefaultValueSql("''");

                entity.Property(e => e.Progress)
                    .HasColumnName("PROGRESS")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Stamp)
                    .HasColumnName("STAMP")
                    .HasColumnType("varchar(1024)")
                    .HasDefaultValueSql("''");

                entity.Property(e => e.Status)
                    .HasColumnName("STATUS")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Streamchannel)
                    .HasColumnName("STREAMCHANNEL")
                    .HasColumnType("varchar(32)")
                    .HasDefaultValueSql("''");
            });

            modelBuilder.Entity<DbpXdcamDiskinfo>(entity =>
            {
                entity.HasKey(e => e.Diskid);

                entity.ToTable("dbp_xdcam_diskinfo");

                entity.Property(e => e.Diskid)
                    .HasColumnName("DISKID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Capacity)
                    .HasColumnName("CAPACITY")
                    .HasDefaultValueSql("'23.3'");

                entity.Property(e => e.Createtime)
                    .HasColumnName("CREATETIME")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("'CURRENT_TIMESTAMP'")
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.Deviceid)
                    .HasColumnName("DEVICEID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.DiskinfoDesc)
                    .HasColumnName("DISKINFO_DESC")
                    .HasColumnType("varchar(4000)")
                    .HasDefaultValueSql("''");

                entity.Property(e => e.DiskinfoUsage)
                    .HasColumnName("DISKINFO_USAGE")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Diskname)
                    .IsRequired()
                    .HasColumnName("DISKNAME")
                    .HasColumnType("varchar(128)");

                entity.Property(e => e.Diskstate)
                    .HasColumnName("DISKSTATE")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Diskumid)
                    .IsRequired()
                    .HasColumnName("DISKUMID")
                    .HasColumnType("varchar(128)");

                entity.Property(e => e.Progress)
                    .HasColumnName("PROGRESS")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Storagepath)
                    .HasColumnName("STORAGEPATH")
                    .HasColumnType("varchar(512)")
                    .HasDefaultValueSql("''");
            });

            modelBuilder.Entity<DbpXdcamMaterialDevMap>(entity =>
            {
                entity.HasKey(e => new { e.Materialid, e.Deviceid });

                entity.ToTable("dbp_xdcam_material_dev_map");

                entity.Property(e => e.Materialid)
                    .HasColumnName("MATERIALID")
                    .HasColumnType("varchar(128)");

                entity.Property(e => e.Deviceid)
                    .HasColumnName("DEVICEID")
                    .HasColumnType("int(11)");
            });

            modelBuilder.Entity<VtrDetailinfo>(entity =>
            {
                entity.HasKey(e => e.Vtrid);

                entity.ToTable("vtr_detailinfo");

                entity.Property(e => e.Vtrid)
                    .HasColumnName("VTRID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Backuptype)
                    .HasColumnName("BACKUPTYPE")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Baudrate)
                    .HasColumnName("BAUDRATE")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'38400'");

                entity.Property(e => e.Framerate)
                    .HasColumnName("FRAMERATE")
                    .HasColumnType("float(10,3)")
                    .HasDefaultValueSql("'25.000'");

                entity.Property(e => e.Looprecord)
                    .HasColumnName("LOOPRECORD")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Prerolframenum)
                    .HasColumnName("PREROLFRAMENUM")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Vtrcomport)
                    .HasColumnName("VTRCOMPORT")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Vtrddescribe)
                    .HasColumnName("VTRDDESCRIBE")
                    .HasColumnType("varchar(128)")
                    .HasDefaultValueSql("''");

                entity.Property(e => e.Vtrname)
                    .IsRequired()
                    .HasColumnName("VTRNAME")
                    .HasColumnType("varchar(128)");

                entity.Property(e => e.Vtrserverip)
                    .HasColumnName("VTRSERVERIP")
                    .HasColumnType("varchar(32)")
                    .HasDefaultValueSql("''");

                entity.Property(e => e.Vtrsignaltype)
                    .HasColumnName("VTRSIGNALTYPE")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Vtrstate)
                    .HasColumnName("VTRSTATE")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Vtrsubtype)
                    .HasColumnName("VTRSUBTYPE")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Vtrtypeid)
                    .HasColumnName("VTRTYPEID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Workmode)
                    .HasColumnName("WORKMODE")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");
            });

            modelBuilder.Entity<VtrDownloadMateriallist>(entity =>
            {
                entity.HasKey(e => e.Materialid);

                entity.ToTable("vtr_download_materiallist");

                entity.Property(e => e.Materialid)
                    .HasColumnName("MATERIALID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Contentid)
                    .HasColumnName("CONTENTID")
                    .HasColumnType("varchar(128)");

                entity.Property(e => e.Createtime)
                    .HasColumnName("CREATETIME")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("'CURRENT_TIMESTAMP'")
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.Creator)
                    .HasColumnName("CREATOR")
                    .HasColumnType("varchar(256)");

                entity.Property(e => e.Fileguid)
                    .HasColumnName("FILEGUID")
                    .HasColumnType("varchar(128)");

                entity.Property(e => e.Fileinpoint)
                    .HasColumnName("FILEINPOINT")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Filename)
                    .HasColumnName("FILENAME")
                    .HasColumnType("varchar(256)");

                entity.Property(e => e.Fileoutpoint)
                    .HasColumnName("FILEOUTPOINT")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Filetypeid)
                    .HasColumnName("FILETYPEID")
                    .HasColumnType("varchar(64)");

                entity.Property(e => e.Groupname)
                    .HasColumnName("GROUPNAME")
                    .HasColumnType("varchar(256)");

                entity.Property(e => e.Refinpoint)
                    .HasColumnName("REFINPOINT")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Refoutpoint)
                    .HasColumnName("REFOUTPOINT")
                    .HasColumnType("int(11)");
            });

            modelBuilder.Entity<DbpMsvchannelState>(entity =>
            {
                entity.HasKey(e => e.Channelid);

                entity.ToTable("dbp_msvchannel_state");

                entity.Property(e => e.Channelid)
                    .HasColumnName("CHANNELID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Curusercode)
                    .HasColumnName("CURUSERCODE")
                    .HasColumnType("varchar(128)")
                    .HasDefaultValueSql("''");

                entity.Property(e => e.Devstate)
                    .HasColumnName("DEVSTATE")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Kamatakiinfo)
                    .HasColumnName("KAMATAKIINFO")
                    .HasColumnType("varchar(512)")
                    .HasDefaultValueSql("''");

                entity.Property(e => e.Msvmode)
                    .HasColumnName("MSVMODE")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Sourcevtrid)
                    .HasColumnName("SOURCEVTRID")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'-1'");

                entity.Property(e => e.Uploadstate)
                    .HasColumnName("UPLOADSTATE")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");
            });

            modelBuilder.Entity<VtrTapelist>(entity =>
            {
                entity.HasKey(e => e.Tapeid);

                entity.ToTable("vtr_tapelist");

                entity.Property(e => e.Tapeid)
                    .HasColumnName("TAPEID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Tapedesc)
                    .HasColumnName("TAPEDESC")
                    .HasColumnType("varchar(512)");

                entity.Property(e => e.Tapename)
                    .HasColumnName("TAPENAME")
                    .HasColumnType("varchar(512)");
            });

            modelBuilder.Entity<VtrTapeVtrMap>(entity =>
            {
                entity.HasKey(e => e.Vtrid);

                entity.ToTable("vtr_tape_vtr_map");

                entity.Property(e => e.Vtrid)
                    .HasColumnName("VTRID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Tapeid)
                    .HasColumnName("TAPEID")
                    .HasColumnType("int(11)");
            });



            modelBuilder.Entity<VtrTypeinfo>(entity =>
            {
                entity.HasKey(e => new { e.Vtrtypeid, e.Vtrsubtype });

                entity.ToTable("vtr_typeinfo");

                entity.Property(e => e.Vtrtypeid)
                    .HasColumnName("VTRTYPEID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Vtrsubtype)
                    .HasColumnName("VTRSUBTYPE")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Vtrtypedescribe)
                    .HasColumnName("VTRTYPEDESCRIBE")
                    .HasColumnType("varchar(512)")
                    .HasDefaultValueSql("''");

                entity.Property(e => e.Vtrtypename)
                    .IsRequired()
                    .HasColumnName("VTRTYPENAME")
                    .HasColumnType("varchar(128)");
            });


        }
    }
}
