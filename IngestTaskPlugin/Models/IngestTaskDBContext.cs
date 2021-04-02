using System;
using Microsoft.EntityFrameworkCore;

namespace IngestTaskPlugin.Models
{
    public partial class IngestTaskDBContext : DbContext
    {
        public IngestTaskDBContext(DbContextOptions<IngestTaskDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<DbpImportTask> DbpImportTask { get; set; }
        public virtual DbSet<DbpTask> DbpTask { get; set; }
        public virtual DbSet<DbpTaskBackup> DbpTaskBackup { get; set; }
        public virtual DbSet<DbpTaskCustommetadata> DbpTaskCustommetadata { get; set; }
        public virtual DbSet<DbpTaskErrorinfo> DbpTaskErrorinfo { get; set; }
        public virtual DbSet<DbpTaskgroup> DbpTaskgroup { get; set; }
        public virtual DbSet<DbpTaskgroupMap> DbpTaskgroupMap { get; set; }
        public virtual DbSet<DbpTaskMetadata> DbpTaskMetadata { get; set; }
        public virtual DbSet<DbpTaskMetadataBackup> DbpTaskMetadataBackup { get; set; }
        public virtual DbSet<DbpTaskSignalsrcBackuptask> DbpTaskSignalsrcBackuptask { get; set; }
        
        public virtual DbSet<VtrTaskinfo> VtrTaskinfo { get; set; }
        public virtual DbSet<VtrTaskInout> VtrTaskInout { get; set; }
        public virtual DbSet<VtrTaskMeatdata> VtrTaskMeatdata { get; set; }
        public virtual DbSet<DbpXdcamTask> DbpXdcamTask { get; set; }
        public virtual DbSet<DbpXdcamTaskfile> DbpXdcamTaskfile { get; set; }
        public virtual DbSet<DbpXdcamTaskMetadata> DbpXdcamTaskMetadata { get; set; }
        public virtual DbSet<VtrRecordtask> VtrRecordtask { get; set; }
        public virtual DbSet<VtrUploadtask> VtrUploadtask { get; set; }
        public virtual DbSet<DbpPolicytask> DbpPolicytask { get; set; }
        public DbSet<VtrDetailinfo> VtrDetailinfo { get; set; }
        public DbSet<VtrTapelist> VtrTapelist { get; set; }
        public DbSet<VtrTapeVtrMap> VtrTapeVtrMap { get; set; }
        public DbSet<VtrTypeinfo> VtrTypeinfo { get; set; }
        public DbSet<DbpPolicyuser> DbpPolicyuser { get; set; }
        public DbSet<DbpMetadatapolicy> DbpMetadatapolicy { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DbpMetadatapolicy>(entity =>
            {
                entity.HasKey(e => e.Policyid);

                entity.ToTable("dbp_metadatapolicy");

                entity.Property(e => e.Policyid)
                    .HasColumnName("POLICYID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Archivetype)
                    .IsRequired()
                    .HasColumnName("ARCHIVETYPE")
                    .HasColumnType("varchar(256)");

                entity.Property(e => e.Defaultpolicy)
                    .HasColumnName("DEFAULTPOLICY")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Policydesc)
                    .HasColumnName("POLICYDESC")
                    .HasColumnType("varchar(4000)")
                    .HasDefaultValueSql("''");

                entity.Property(e => e.Policyname)
                    .HasColumnName("POLICYNAME")
                    .HasColumnType("varchar(128)")
                    .HasDefaultValueSql("''");
            });

            modelBuilder.Entity<DbpPolicyuser>(entity =>
            {
                entity.HasKey(e => new { e.Policyid, e.Usercode });

                entity.ToTable("dbp_policyuser");

                entity.Property(e => e.Policyid)
                    .HasColumnName("POLICYID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Usercode)
                    .HasColumnName("USERCODE")
                    .HasColumnType("varchar(32)")
                    .HasDefaultValueSql("''");
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

            modelBuilder.Entity<VtrTapelist>(entity =>
            {
                entity.HasKey(e => e.Tapeid);

                entity.ToTable("vtr_tapelist");

                entity.Property(e => e.Tapeid).HasColumnName("TAPEID").HasColumnType("int(11)");

                entity.Property(e => e.Tapedesc).HasColumnName("TAPEDESC").HasColumnType("varchar(512)");

                entity.Property(e => e.Tapename).HasColumnName("TAPENAME").HasColumnType("varchar(512)");
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
            modelBuilder.Entity<DbpImportTask>(entity =>
            {
                entity.ToTable("dbp_import_task");

                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Excursus)
                    .HasColumnName("EXCURSUS")
                    .HasColumnType("varchar(4000)")
                    .HasDefaultValueSql("''");

                entity.Property(e => e.Files)
                    .HasColumnName("FILES")
                    .HasColumnType("varchar(4000)")
                    .HasDefaultValueSql("''");

                entity.Property(e => e.Guid)
                    .IsRequired()
                    .HasColumnName("GUID")
                    .HasColumnType("varchar(64)");

                entity.Property(e => e.Metadatas)
                    .IsRequired()
                    .HasColumnName("METADATAS")
                    .HasColumnType("varchar(512)");

                entity.Property(e => e.Usercode)
                    .IsRequired()
                    .HasColumnName("USERCODE")
                    .HasColumnType("varchar(256)");
            });
            modelBuilder.Entity<DbpTask>(entity =>
            {
                entity.HasKey(e => e.Taskid);

                entity.ToTable("dbp_task");

                entity.HasIndex(e => e.Channelid)
                    .HasName("IDX_ITASK_CHID");

                entity.HasIndex(e => e.DispatchState)
                    .HasName("IDX_ITASK_SD");

                entity.HasIndex(e => e.Endtime)
                    .HasName("IDX_ITASK_END");

                entity.HasIndex(e => e.NewBegintime)
                    .HasName("IDX_ITASK_NBT");

                entity.HasIndex(e => e.Starttime)
                    .HasName("IDX_ITASK_ST");

                entity.HasIndex(e => e.State)
                    .HasName("IDX_ITASK_STATE");

                entity.HasIndex(e => e.SyncState)
                    .HasName("IDX_ITASK_SS");

                entity.HasIndex(e => e.Tasktype)
                    .HasName("IDX_ITASK_TASKTYPE");

                entity.Property(e => e.Taskid)
                    .HasColumnName("TASKID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Backtype)
                    .HasColumnName("BACKTYPE")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Backupvtrid)
                    .HasColumnName("BACKUPVTRID")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Category)
                    .HasColumnName("CATEGORY")
                    .HasColumnType("varchar(4000)")
                    .HasDefaultValueSql("''");

                entity.Property(e => e.Channelid)
                    .HasColumnName("CHANNELID")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Description)
                    .HasColumnName("DESCRIPTION")
                    .HasColumnType("varchar(1024)")
                    .HasDefaultValueSql("''");

                entity.Property(e => e.DispatchState)
                    .HasColumnName("DISPATCH_STATE")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Endtime)
                    .HasColumnName("ENDTIME")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("'CURRENT_TIMESTAMP'");

                entity.Property(e => e.NewBegintime)
                    .HasColumnName("NEW_BEGINTIME")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("'CURRENT_TIMESTAMP'");

                entity.Property(e => e.NewEndtime)
                    .HasColumnName("NEW_ENDTIME")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("'CURRENT_TIMESTAMP'");

                entity.Property(e => e.OldChannelid)
                    .HasColumnName("OLD_CHANNELID")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.OpType)
                    .HasColumnName("OP_TYPE")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Recunitid)
                    .HasColumnName("RECUNITID")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Sgroupcolor)
                    .HasColumnName("SGROUPCOLOR")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Tasksource)
                    .HasColumnName("TASKSOURCE")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Signalid)
                    .HasColumnName("SIGNALID")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Stampimagetype)
                    .HasColumnName("STAMPIMAGETYPE")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Stamptitleindex)
                    .HasColumnName("STAMPTITLEINDEX")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Starttime)
                    .HasColumnName("STARTTIME")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("'CURRENT_TIMESTAMP'");

                entity.Property(e => e.State)
                    .HasColumnName("STATE")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.SyncState)
                    .HasColumnName("SYNC_STATE")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Taskguid)
                    .HasColumnName("TASKGUID")
                    .HasColumnType("varchar(512)")
                    .HasDefaultValueSql("''");

                entity.Property(e => e.Tasklock)
                    .HasColumnName("TASKLOCK")
                    .HasColumnType("varchar(128)")
                    .HasDefaultValueSql("''");

                entity.Property(e => e.Taskname)
                    .HasColumnName("TASKNAME")
                    .HasColumnType("varchar(256)")
                    .HasDefaultValueSql("''");

                entity.Property(e => e.Taskpriority)
                    .HasColumnName("TASKPRIORITY")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Tasktype)
                    .HasColumnName("TASKTYPE")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Usercode)
                    .HasColumnName("USERCODE")
                    .HasColumnType("varchar(256)")
                    .HasDefaultValueSql("''");
            });

            modelBuilder.Entity<DbpTaskBackup>(entity =>
            {
                entity.HasKey(e => e.Taskid);

                entity.ToTable("dbp_task_backup");

                entity.HasIndex(e => e.Channelid)
                    .HasName("IDX_ITASK_CHIDB");

                entity.HasIndex(e => e.DispatchState)
                    .HasName("IDX_ITASK_SDB");

                entity.HasIndex(e => e.Endtime)
                    .HasName("IDX_ITASK_ENDB");

                entity.HasIndex(e => e.Starttime)
                    .HasName("IDX_ITASK_STB");

                entity.HasIndex(e => e.SyncState)
                    .HasName("IDX_ITASK_SSB");

                entity.Property(e => e.Taskid)
                    .HasColumnName("TASKID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Backtype)
                    .HasColumnName("BACKTYPE")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Backupvtrid)
                    .HasColumnName("BACKUPVTRID")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Category)
                    .HasColumnName("CATEGORY")
                    .HasColumnType("varchar(4000)")
                    .HasDefaultValueSql("''");

                entity.Property(e => e.Channelid)
                    .HasColumnName("CHANNELID")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Description)
                    .HasColumnName("DESCRIPTION")
                    .HasColumnType("varchar(1024)")
                    .HasDefaultValueSql("''");

                entity.Property(e => e.DispatchState)
                    .HasColumnName("DISPATCH_STATE")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Endtime)
                    .HasColumnName("ENDTIME")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("'CURRENT_TIMESTAMP'");

                entity.Property(e => e.NewBegintime)
                    .HasColumnName("NEW_BEGINTIME")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("'CURRENT_TIMESTAMP'");

                entity.Property(e => e.NewEndtime)
                    .HasColumnName("NEW_ENDTIME")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("'CURRENT_TIMESTAMP'");

                entity.Property(e => e.OldChannelid)
                    .HasColumnName("OLD_CHANNELID")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.OpType)
                    .HasColumnName("OP_TYPE")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Recunitid)
                    .HasColumnName("RECUNITID")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Signalid)
                    .HasColumnName("SIGNALID")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Stampimagetype)
                    .HasColumnName("STAMPIMAGETYPE")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Stamptitleindex)
                    .HasColumnName("STAMPTITLEINDEX")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Starttime)
                    .HasColumnName("STARTTIME")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("'CURRENT_TIMESTAMP'");

                entity.Property(e => e.State)
                    .HasColumnName("STATE")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.SyncState)
                    .HasColumnName("SYNC_STATE")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Taskguid)
                    .HasColumnName("TASKGUID")
                    .HasColumnType("varchar(512)")
                    .HasDefaultValueSql("''");

                entity.Property(e => e.Tasklock)
                    .HasColumnName("TASKLOCK")
                    .HasColumnType("varchar(128)")
                    .HasDefaultValueSql("''");

                entity.Property(e => e.Taskname)
                    .HasColumnName("TASKNAME")
                    .HasColumnType("varchar(256)")
                    .HasDefaultValueSql("''");

                entity.Property(e => e.Taskpriority)
                    .HasColumnName("TASKPRIORITY")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Tasktype)
                    .HasColumnName("TASKTYPE")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Usercode)
                    .HasColumnName("USERCODE")
                    .HasColumnType("varchar(256)")
                    .HasDefaultValueSql("''");
            });
            modelBuilder.Entity<DbpTaskCustommetadata>(entity =>
            {
                entity.HasKey(e => e.Taskid);

                entity.ToTable("dbp_task_custommetadata");

                entity.Property(e => e.Taskid)
                    .HasColumnName("TASKID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Metadata)
                    .HasColumnName("METADATA")
                    .HasColumnType("longtext");
            });

            modelBuilder.Entity<DbpTaskErrorinfo>(entity =>
            {
                entity.HasKey(e => e.Infoid);

                entity.ToTable("dbp_task_errorinfo");

                entity.Property(e => e.Infoid)
                    .HasColumnName("INFOID")
                    .HasColumnType("varchar(128)");

                entity.Property(e => e.Errdesc)
                    .HasColumnName("ERRDESC")
                    .HasColumnType("varchar(1024)");

                entity.Property(e => e.Errlevel)
                    .HasColumnName("ERRLEVEL")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Errmodule)
                    .HasColumnName("ERRMODULE")
                    .HasColumnType("varchar(512)");

                entity.Property(e => e.Errorcode)
                    .HasColumnName("ERRORCODE")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Errtime)
                    .HasColumnName("ERRTIME")
                    .HasColumnType("varchar(128)");

                entity.Property(e => e.Errtype)
                    .HasColumnName("ERRTYPE")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Taskid)
                    .HasColumnName("TASKID")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");
            });

            modelBuilder.Entity<DbpTaskgroup>(entity =>
            {
                entity.HasKey(e => e.GroupId);

                entity.ToTable("dbp_taskgroup");

                entity.Property(e => e.GroupId)
                    .HasColumnName("GROUP_ID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.ChangeTime)
                    .HasColumnName("CHANGE_TIME")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("'0000-00-00 00:00:00'");

                entity.Property(e => e.CreateTime)
                    .HasColumnName("CREATE_TIME")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("'CURRENT_TIMESTAMP'")
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.GroupDesc)
                    .HasColumnName("GROUP_DESC")
                    .HasColumnType("varchar(128)");

                entity.Property(e => e.GroupName)
                    .IsRequired()
                    .HasColumnName("GROUP_NAME")
                    .HasColumnType("varchar(32)");
            });

            modelBuilder.Entity<DbpTaskgroupMap>(entity =>
            {
                entity.HasKey(e => e.TaskId);

                entity.ToTable("dbp_taskgroup_map");

                entity.Property(e => e.TaskId)
                    .HasColumnName("TASK_ID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.ChangeDate)
                    .HasColumnName("CHANGE_DATE")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("'CURRENT_TIMESTAMP'")
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.GroupId)
                    .HasColumnName("GROUP_ID")
                    .HasColumnType("int(11)");
            });

            modelBuilder.Entity<DbpTaskMetadata>(entity =>
            {
                entity.HasKey(e => new { e.Taskid, e.Metadatatype });

                entity.ToTable("dbp_task_metadata");

                entity.Property(e => e.Taskid)
                    .HasColumnName("TASKID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Metadatatype)
                    .HasColumnName("METADATATYPE")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Metadata)
                    .HasColumnName("METADATA")
                    .HasColumnType("varchar(4000)")
                    .HasDefaultValueSql("''");

                entity.Property(e => e.Metadatalong)
                    .HasColumnName("METADATALONG")
                    .HasColumnType("text");
            });

            modelBuilder.Entity<DbpTaskMetadataBackup>(entity =>
            {
                entity.HasKey(e => new { e.Taskid, e.Metadatatype });

                entity.ToTable("dbp_task_metadata_backup");

                entity.Property(e => e.Taskid)
                    .HasColumnName("TASKID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Metadatatype)
                    .HasColumnName("METADATATYPE")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Metadata)
                    .HasColumnName("METADATA")
                    .HasColumnType("varchar(4000)")
                    .HasDefaultValueSql("''");

                entity.Property(e => e.Metadatalong)
                    .HasColumnName("METADATALONG")
                    .HasColumnType("text");
            });

            modelBuilder.Entity<DbpTaskSignalsrcBackuptask>(entity =>
            {
                entity.HasKey(e => e.Taskid);

                entity.ToTable("dbp_task_signalsrc_backuptask");

                entity.Property(e => e.Taskid)
                    .HasColumnName("TASKID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Maintaskid)
                    .HasColumnName("MAINTASKID")
                    .HasColumnType("int(11)");
            });

            
            modelBuilder.Entity<DbpXdcamTask>(entity =>
            {
                entity.HasKey(e => e.Taskid);

                entity.ToTable("dbp_xdcam_task");

                entity.Property(e => e.Taskid)
                    .HasColumnName("TASKID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Catalogpath)
                    .HasColumnName("CATALOGPATH")
                    .HasColumnType("varchar(512)")
                    .HasDefaultValueSql("''");

                entity.Property(e => e.Clipname)
                    .HasColumnName("CLIPNAME")
                    .HasColumnType("varchar(512)")
                    .HasDefaultValueSql("''");

                entity.Property(e => e.Committime)
                    .HasColumnName("COMMITTIME")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("'0000-00-00 00:00:00'");

                entity.Property(e => e.Createtime)
                    .HasColumnName("CREATETIME")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("'CURRENT_TIMESTAMP'")
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.Diskid)
                    .HasColumnName("DISKID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Objecttype)
                    .HasColumnName("OBJECTTYPE")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Objectumid)
                    .HasColumnName("OBJECTUMID")
                    .HasColumnType("varchar(512)")
                    .HasDefaultValueSql("''");

                entity.Property(e => e.Priority)
                    .HasColumnName("PRIORITY")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Progress)
                    .HasColumnName("PROGRESS")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Statedesc)
                    .HasColumnName("STATEDESC")
                    .HasColumnType("varchar(10)")
                    .HasDefaultValueSql("''");

                entity.Property(e => e.Taskguid)
                    .HasColumnName("TASKGUID")
                    .HasColumnType("varchar(128)")
                    .HasDefaultValueSql("''");

                entity.Property(e => e.Taskstate)
                    .HasColumnName("TASKSTATE")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Usercode)
                    .HasColumnName("USERCODE")
                    .HasColumnType("varchar(64)")
                    .HasDefaultValueSql("''");

                entity.Property(e => e.Visible)
                    .HasColumnName("VISIBLE")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");
            });
            modelBuilder.Entity<DbpXdcamTaskfile>(entity =>
            {
                entity.HasKey(e => e.Taskfileid);

                entity.ToTable("dbp_xdcam_taskfile");

                entity.Property(e => e.Taskfileid)
                    .HasColumnName("TASKFILEID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Filename)
                    .IsRequired()
                    .HasColumnName("FILENAME")
                    .HasColumnType("varchar(512)");

                entity.Property(e => e.Filestatus)
                    .HasColumnName("FILESTATUS")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Filetype)
                    .HasColumnName("FILETYPE")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Fileumid)
                    .HasColumnName("FILEUMID")
                    .HasColumnType("varchar(128)");

                entity.Property(e => e.Inpoint)
                    .HasColumnName("INPOINT")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Outpoint)
                    .HasColumnName("OUTPOINT")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Progress)
                    .HasColumnName("PROGRESS")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Taskid)
                    .HasColumnName("TASKID")
                    .HasColumnType("int(11)");
            });
            modelBuilder.Entity<DbpXdcamTaskMetadata>(entity =>
            {
                entity.HasKey(e => e.Taskid);

                entity.ToTable("dbp_xdcam_task_metadata");

                entity.Property(e => e.Taskid)
                    .HasColumnName("TASKID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Metadata)
                    .IsRequired()
                    .HasColumnName("METADATA")
                    .HasColumnType("varchar(4000)");

                entity.Property(e => e.Type)
                    .HasColumnName("TYPE")
                    .HasColumnType("int(11)");
            });

            modelBuilder.Entity<VtrRecordtask>(entity =>
            {
                entity.HasKey(e => e.Taskid);

                entity.ToTable("vtr_recordtask");

                entity.Property(e => e.Taskid)
                    .HasColumnName("TASKID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Committime)
                    .HasColumnName("COMMITTIME")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("'CURRENT_TIMESTAMP'")
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.Recchannelid)
                    .HasColumnName("RECCHANNELID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Recordorder)
                    .HasColumnName("RECORDORDER")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Tapeid)
                    .HasColumnName("TAPEID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Tapetrimin)
                    .HasColumnName("TAPETRIMIN")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Tapetrimout)
                    .HasColumnName("TAPETRIMOUT")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Taskguid)
                    .HasColumnName("TASKGUID")
                    .HasColumnType("varchar(128)");

                entity.Property(e => e.Taskname)
                    .HasColumnName("TASKNAME")
                    .HasColumnType("varchar(256)");

                entity.Property(e => e.Taskstate)
                    .HasColumnName("TASKSTATE")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Usercode)
                    .HasColumnName("USERCODE")
                    .HasColumnType("varchar(256)");

                entity.Property(e => e.Vtrid)
                    .HasColumnName("VTRID")
                    .HasColumnType("int(11)");
            });
            modelBuilder.Entity<VtrTaskinfo>(entity =>
            {
                entity.HasKey(e => e.Vtrtaskid);

                entity.ToTable("vtr_taskinfo");

                entity.Property(e => e.Vtrtaskid)
                    .HasColumnName("VTRTASKID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Barcode)
                    .HasColumnName("BARCODE")
                    .HasColumnType("varchar(128)")
                    .HasDefaultValueSql("''");

                entity.Property(e => e.Begintime)
                    .HasColumnName("BEGINTIME")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("'CURRENT_TIMESTAMP'");

                entity.Property(e => e.Description)
                    .HasColumnName("DESCRIPTION")
                    .HasColumnType("varchar(1024)")
                    .HasDefaultValueSql("''");

                entity.Property(e => e.Endtime)
                    .HasColumnName("ENDTIME")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("'CURRENT_TIMESTAMP'");

                entity.Property(e => e.Loopflag)
                    .HasColumnName("LOOPFLAG")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Signalid)
                    .HasColumnName("SIGNALID")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Syncflag)
                    .HasColumnName("SYNCFLAG")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Taskguid)
                    .HasColumnName("TASKGUID")
                    .HasColumnType("varchar(128)");

                entity.Property(e => e.Taskname)
                    .IsRequired()
                    .HasColumnName("TASKNAME")
                    .HasColumnType("varchar(512)")
                    .HasDefaultValueSql("''");

                entity.Property(e => e.Taskstate)
                    .HasColumnName("TASKSTATE")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Uploadflag)
                    .HasColumnName("UPLOADFLAG")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Usercode)
                    .HasColumnName("USERCODE")
                    .HasColumnType("varchar(64)")
                    .HasDefaultValueSql("''");

                entity.Property(e => e.Vtrid)
                    .HasColumnName("VTRID")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");
            });

            modelBuilder.Entity<VtrTaskInout>(entity =>
            {
                entity.HasKey(e => new { e.Taskid, e.Inpoint, e.Outpoint });

                entity.ToTable("vtr_task_inout");

                entity.Property(e => e.Taskid)
                    .HasColumnName("TASKID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Inpoint)
                    .HasColumnName("INPOINT")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Outpoint)
                    .HasColumnName("OUTPOINT")
                    .HasColumnType("int(11)");
            });

            modelBuilder.Entity<VtrTaskMeatdata>(entity =>
            {
                entity.HasKey(e => new { e.Vtrtaskid, e.Metadatatype });

                entity.ToTable("vtr_task_meatdata");

                entity.Property(e => e.Vtrtaskid)
                    .HasColumnName("VTRTASKID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Metadatatype)
                    .HasColumnName("METADATATYPE")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Matadata)
                    .HasColumnName("MATADATA")
                    .HasColumnType("varchar(4000)")
                    .HasDefaultValueSql("''");
            });
            modelBuilder.Entity<VtrUploadtask>(entity =>
            {
                entity.HasKey(e => e.Taskid);

                entity.ToTable("vtr_uploadtask");

                entity.HasIndex(e => e.Taskguid)
                    .HasName("IDX_VTASK_TASKGUID");

                entity.HasIndex(e => e.Taskstate)
                    .HasName("IDX_VTASK_TASKSTATE");

                entity.HasIndex(e => e.Vtrtasktype)
                    .HasName("IDX_VTASK_VTRTASKTYPE");

                entity.Property(e => e.Taskid)
                    .HasColumnName("TASKID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Committime)
                    .HasColumnName("COMMITTIME")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("'CURRENT_TIMESTAMP'");

                entity.Property(e => e.Recchannelid)
                    .HasColumnName("RECCHANNELID")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Signalid)
                    .HasColumnName("SIGNALID")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Tapeid)
                    .HasColumnName("TAPEID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Taskguid)
                    .HasColumnName("TASKGUID")
                    .HasColumnType("varchar(128)")
                    .HasDefaultValueSql("''");

                entity.Property(e => e.Taskname)
                    .HasColumnName("TASKNAME")
                    .HasColumnType("varchar(256)")
                    .HasDefaultValueSql("''");

                entity.Property(e => e.Taskstate)
                    .HasColumnName("TASKSTATE")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Trimin)
                    .HasColumnName("TRIMIN")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Triminctl)
                    .HasColumnName("TRIMINCTL")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Trimout)
                    .HasColumnName("TRIMOUT")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Trimoutctl)
                    .HasColumnName("TRIMOUTCTL")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Uploadorder)
                    .HasColumnName("UPLOADORDER")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Usercode)
                    .HasColumnName("USERCODE")
                    .HasColumnType("varchar(256)")
                    .HasDefaultValueSql("''");

                entity.Property(e => e.Usertoken)
                    .HasColumnName("USERTOKEN")
                    .HasColumnType("varchar(512)");

                entity.Property(e => e.Vtrid)
                    .HasColumnName("VTRID")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Vtrtaskid)
                    .HasColumnName("VTRTASKID")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Vtrtasktype)
                    .HasColumnName("VTRTASKTYPE")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");
            });
            modelBuilder.Entity<DbpPolicytask>(entity =>
            {
                entity.HasKey(e => new { e.Policyid, e.Taskid });

                entity.ToTable("dbp_policytask");

                entity.Property(e => e.Policyid)
                    .HasColumnName("POLICYID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Taskid)
                    .HasColumnName("TASKID")
                    .HasColumnType("int(11)");
            });
        }

        [DbFunction]
        public static int next_val(string value)
        {
            throw new NotImplementedException();
        }
    }
}