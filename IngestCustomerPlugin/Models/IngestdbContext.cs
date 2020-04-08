using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace IngestCustomerPlugin.Models
{
    public partial class IngestdbContext : DbContext
    {
       

        public IngestdbContext(DbContextOptions<IngestdbContext> options)
            : base(options)
        {
        }
        
        public virtual DbSet<DbpArchivetype> DbpArchivetype { get; set; }
        public virtual DbSet<DbpArea> DbpArea { get; set; }
        public virtual DbSet<DbpAudiotype> DbpAudiotype { get; set; }
        public virtual DbSet<DbpCarrieres> DbpCarrieres { get; set; }
        
        public virtual DbSet<DbpDevConn> DbpDevConn { get; set; }
        
       
        
        
        public virtual DbSet<DbpLevelrelation> DbpLevelrelation { get; set; }
        //public virtual DbSet<DbpLogAction> DbpLogAction { get; set; }
        //public virtual DbSet<DbpLogEvent> DbpLogEvent { get; set; }
        //public virtual DbSet<DbpLogEventExtend> DbpLogEventExtend { get; set; }
        //public virtual DbSet<DbpLogFolder> DbpLogFolder { get; set; }
        //public virtual DbSet<DbpLogLanguageinfo> DbpLogLanguageinfo { get; set; }
        //public virtual DbSet<DbpLogLanguagemap> DbpLogLanguagemap { get; set; }
        //public virtual DbSet<DbpLogMember> DbpLogMember { get; set; }
        //public virtual DbSet<DbpLogMonitor> DbpLogMonitor { get; set; }
        //public virtual DbSet<DbpLogObject> DbpLogObject { get; set; }
        //public virtual DbSet<DbpLogScenario> DbpLogScenario { get; set; }
        //public virtual DbSet<DbpLogScenarioactionmap> DbpLogScenarioactionmap { get; set; }
        //public virtual DbSet<DbpLogScenarioeventmap> DbpLogScenarioeventmap { get; set; }
        //public virtual DbSet<DbpLogScenarioteammap> DbpLogScenarioteammap { get; set; }
        //public virtual DbSet<DbpLogScenteammembermap> DbpLogScenteammembermap { get; set; }
        //public virtual DbSet<DbpLogSysseting> DbpLogSysseting { get; set; }
        //public virtual DbSet<DbpLogTeam> DbpLogTeam { get; set; }
        //public virtual DbSet<DbpLogTeammembermap> DbpLogTeammembermap { get; set; }
        //public virtual DbSet<DbpLogUsersetting> DbpLogUsersetting { get; set; }
        //public virtual DbSet<DbpLogUsertemplate> DbpLogUsertemplate { get; set; }
        public virtual DbSet<DbpMapinport> DbpMapinport { get; set; }
        public virtual DbSet<DbpMapoutport> DbpMapoutport { get; set; }
        public virtual DbSet<DbpMaterial> DbpMaterial { get; set; }
        public virtual DbSet<DbpMaterialArchive> DbpMaterialArchive { get; set; }
        public virtual DbSet<DbpMaterialAudio> DbpMaterialAudio { get; set; }
        public virtual DbSet<DbpMaterialAudioBackup> DbpMaterialAudioBackup { get; set; }
        public virtual DbSet<DbpMaterialBackup> DbpMaterialBackup { get; set; }
        public virtual DbSet<DbpMaterialDuration> DbpMaterialDuration { get; set; }
        public virtual DbSet<DbpMaterialVideo> DbpMaterialVideo { get; set; }
        public virtual DbSet<DbpMaterialVideoBackup> DbpMaterialVideoBackup { get; set; }
       
        public virtual DbSet<DbpMessageRegister> DbpMessageRegister { get; set; }
        public virtual DbSet<DbpMessages> DbpMessages { get; set; }
        public virtual DbSet<DbpMetadatapolicy> DbpMetadatapolicy { get; set; }
        public virtual DbSet<DbpMsgcontrol> DbpMsgcontrol { get; set; }
        public virtual DbSet<DbpMsgFailedrecord> DbpMsgFailedrecord { get; set; }
        public virtual DbSet<DbpMsmqmsg> DbpMsmqmsg { get; set; }
        public virtual DbSet<DbpMsmqmsgFailed> DbpMsmqmsgFailed { get; set; }
        
        public virtual DbSet<DbpNewimportClip> DbpNewimportClip { get; set; }
        public virtual DbSet<DbpNewimportRemote> DbpNewimportRemote { get; set; }
        public virtual DbSet<DbpNewimportTask> DbpNewimportTask { get; set; }
        public virtual DbSet<DbpObjectstateinfo> DbpObjectstateinfo { get; set; }
        public virtual DbSet<DbpPlans> DbpPlans { get; set; }
        public virtual DbSet<DbpPolicytask> DbpPolicytask { get; set; }
        public virtual DbSet<DbpPolicyuser> DbpPolicyuser { get; set; }
        public virtual DbSet<DbpPolicyuserclass> DbpPolicyuserclass { get; set; }
        public virtual DbSet<DbpProgramparamMap> DbpProgramparamMap { get; set; }
       
        public virtual DbSet<DbpRecdeviceType> DbpRecdeviceType { get; set; }
        public virtual DbSet<DbpRecunit> DbpRecunit { get; set; }
        public virtual DbSet<DbpRouterctrolsetting> DbpRouterctrolsetting { get; set; }
        public virtual DbSet<DbpScheduler> DbpScheduler { get; set; }
        public virtual DbSet<DbpSchedulerRecunit> DbpSchedulerRecunit { get; set; }
       
        public virtual DbSet<DbpStreammedia> DbpStreammedia { get; set; }
       
        public virtual DbSet<DbpTranscodePolicy> DbpTranscodePolicy { get; set; }
        public virtual DbSet<DbpTranscodeTemplate> DbpTranscodeTemplate { get; set; }
        public virtual DbSet<DbpTransferPolicy> DbpTransferPolicy { get; set; }
        public virtual DbSet<DbpTransferTemplate> DbpTransferTemplate { get; set; }
        public virtual DbSet<DbpUserparamlog> DbpUserparamlog { get; set; }
        public virtual DbSet<DbpUserparamMap> DbpUserparamMap { get; set; }
        public virtual DbSet<DbpUsersettings> DbpUsersettings { get; set; }
        public virtual DbSet<DbpUsertemplate> DbpUsertemplate { get; set; }
        public virtual DbSet<DbpVidoetype> DbpVidoetype { get; set; }
        
        public virtual DbSet<DbpXdcamXmploitPlan> DbpXdcamXmploitPlan { get; set; }
        public virtual DbSet<LoginUserAddress> LoginUserAddress { get; set; }
        public virtual DbSet<Sequence> Sequence { get; set; }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    if (!optionsBuilder.IsConfigured)
        //    {
        //        optionsBuilder.UseMySql("server=172.16.0.205;port=3307;database=ingestdb;userid=sdba;password=sdba;");
        //    }
        //}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DbpArchivetype>(entity =>
            {
                entity.HasKey(e => e.Archivetype);

                entity.ToTable("dbp_archivetype");

                entity.Property(e => e.Archivetype)
                    .HasColumnName("ARCHIVETYPE")
                    .HasColumnType("varchar(128)");

                entity.Property(e => e.Archivedesc)
                    .HasColumnName("ARCHIVEDESC")
                    .HasColumnType("varchar(256)")
                    .HasDefaultValueSql("''");
            });

            modelBuilder.Entity<DbpArea>(entity =>
            {
                entity.ToTable("dbp_area");

                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Comport)
                    .HasColumnName("COMPORT")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Comportbaud)
                    .HasColumnName("COMPORTBAUD")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Devicectrlip)
                    .HasColumnName("DEVICECTRLIP")
                    .HasColumnType("varchar(255)")
                    .HasDefaultValueSql("''");

                entity.Property(e => e.Devicectrlport)
                    .HasColumnName("DEVICECTRLPORT")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Matrixname)
                    .IsRequired()
                    .HasColumnName("MATRIXNAME")
                    .HasColumnType("varchar(512)");

                entity.Property(e => e.Matrixtypeid)
                    .HasColumnName("MATRIXTYPEID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Name)
                    .HasColumnName("NAME")
                    .HasColumnType("varchar(255)")
                    .HasDefaultValueSql("''");
            });

            modelBuilder.Entity<DbpAudiotype>(entity =>
            {
                entity.HasKey(e => e.Audiotypeid);

                entity.ToTable("dbp_audiotype");

                entity.Property(e => e.Audiotypeid)
                    .HasColumnName("AUDIOTYPEID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Audiotypedesc)
                    .HasColumnName("AUDIOTYPEDESC")
                    .HasColumnType("varchar(256)")
                    .HasDefaultValueSql("''");
            });

            
            modelBuilder.Entity<DbpCarrieres>(entity =>
            {
                entity.HasKey(e => e.Carrierid);

                entity.ToTable("dbp_carrieres");

                entity.Property(e => e.Carrierid)
                    .HasColumnName("CARRIERID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Carriername)
                    .IsRequired()
                    .HasColumnName("CARRIERNAME")
                    .HasColumnType("varchar(20)");
            });

            

            modelBuilder.Entity<DbpDevConn>(entity =>
            {
                entity.HasKey(e => e.DevType);

                entity.ToTable("dbp_dev_conn");

                entity.Property(e => e.DevType)
                    .HasColumnName("DEV_TYPE")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.DevMaxconn)
                    .HasColumnName("DEV_MAXCONN")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'1'");
            });

            modelBuilder.Entity<DbpLevelrelation>(entity =>
            {
                entity.HasKey(e => new { e.Matrixid, e.Inport });

                entity.ToTable("dbp_levelrelation");

                entity.Property(e => e.Matrixid)
                    .HasColumnName("MATRIXID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Inport)
                    .HasColumnName("INPORT")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Parentmatrixid)
                    .HasColumnName("PARENTMATRIXID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Parentoutport)
                    .HasColumnName("PARENTOUTPORT")
                    .HasColumnType("int(11)");
            });

            modelBuilder.Entity<DbpMapinport>(entity =>
            {
                entity.HasKey(e => e.Virtualinport);

                entity.ToTable("dbp_mapinport");

                entity.Property(e => e.Virtualinport)
                    .HasColumnName("VIRTUALINPORT")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Inport)
                    .HasColumnName("INPORT")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Matrixid)
                    .HasColumnName("MATRIXID")
                    .HasColumnType("int(11)");
            });

            modelBuilder.Entity<DbpMapoutport>(entity =>
            {
                entity.HasKey(e => e.Virtualoutport);

                entity.ToTable("dbp_mapoutport");

                entity.Property(e => e.Virtualoutport)
                    .HasColumnName("VIRTUALOUTPORT")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Matrixid)
                    .HasColumnName("MATRIXID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Outport)
                    .HasColumnName("OUTPORT")
                    .HasColumnType("int(11)");
            });

            modelBuilder.Entity<DbpMaterial>(entity =>
            {
                entity.HasKey(e => e.Materialid);

                entity.ToTable("dbp_material");

                entity.HasIndex(e => e.Clipstate)
                    .HasName("IDX_IMATERIAL_S");

                entity.Property(e => e.Materialid)
                    .HasColumnName("MATERIALID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Clipstate)
                    .HasColumnName("CLIPSTATE")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Createtime)
                    .HasColumnName("CREATETIME")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("'CURRENT_TIMESTAMP'");

                entity.Property(e => e.Deletedstate)
                    .HasColumnName("DELETEDSTATE")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Guid)
                    .HasColumnName("GUID")
                    .HasColumnType("varchar(64)")
                    .HasDefaultValueSql("''");

                entity.Property(e => e.Name)
                    .HasColumnName("NAME")
                    .HasColumnType("varchar(1024)")
                    .HasDefaultValueSql("''");

                entity.Property(e => e.Remark)
                    .HasColumnName("REMARK")
                    .HasColumnType("varchar(1024)")
                    .HasDefaultValueSql("''");

                entity.Property(e => e.Sectionid)
                    .HasColumnName("SECTIONID")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Taskid)
                    .HasColumnName("TASKID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Usercode)
                    .HasColumnName("USERCODE")
                    .HasColumnType("varchar(256)")
                    .HasDefaultValueSql("''");
            });

            modelBuilder.Entity<DbpMaterialArchive>(entity =>
            {
                entity.HasKey(e => new { e.Materialid, e.Policyid });

                entity.ToTable("dbp_material_archive");

                entity.HasIndex(e => e.Archivestate)
                    .HasName("IDX_IARCHIVE_S");

                entity.Property(e => e.Materialid)
                    .HasColumnName("MATERIALID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Policyid)
                    .HasColumnName("POLICYID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Archiveresult)
                    .HasColumnName("ARCHIVERESULT")
                    .HasColumnType("varchar(1024)")
                    .HasDefaultValueSql("''");

                entity.Property(e => e.Archivestate)
                    .HasColumnName("ARCHIVESTATE")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Failedtimes)
                    .HasColumnName("FAILEDTIMES")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Isprocessing)
                    .HasColumnName("ISPROCESSING")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Lastresult)
                    .HasColumnName("LASTRESULT")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Lastupdatetime)
                    .HasColumnName("LASTUPDATETIME")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("'CURRENT_TIMESTAMP'");

                entity.Property(e => e.Nextretry)
                    .HasColumnName("NEXTRETRY")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("'CURRENT_TIMESTAMP'");
            });

            modelBuilder.Entity<DbpMaterialAudio>(entity =>
            {
                entity.HasKey(e => new { e.Materialid, e.Audiotypeid, e.Audiosource });

                entity.ToTable("dbp_material_audio");

                entity.Property(e => e.Materialid)
                    .HasColumnName("MATERIALID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Audiotypeid)
                    .HasColumnName("AUDIOTYPEID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Audiosource)
                    .HasColumnName("AUDIOSOURCE")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Audiofilename)
                    .IsRequired()
                    .HasColumnName("AUDIOFILENAME")
                    .HasColumnType("varchar(512)")
                    .HasDefaultValueSql("''");
            });

            modelBuilder.Entity<DbpMaterialAudioBackup>(entity =>
            {
                entity.HasKey(e => new { e.Materialid, e.Audiotypeid, e.Audiosource });

                entity.ToTable("dbp_material_audio_backup");

                entity.Property(e => e.Materialid)
                    .HasColumnName("MATERIALID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Audiotypeid)
                    .HasColumnName("AUDIOTYPEID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Audiosource)
                    .HasColumnName("AUDIOSOURCE")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Audiofilename)
                    .IsRequired()
                    .HasColumnName("AUDIOFILENAME")
                    .HasColumnType("varchar(512)")
                    .HasDefaultValueSql("''");
            });

            modelBuilder.Entity<DbpMaterialBackup>(entity =>
            {
                entity.HasKey(e => e.Materialid);

                entity.ToTable("dbp_material_backup");

                entity.HasIndex(e => e.Clipstate)
                    .HasName("IDX_IMATERIAL_SB");

                entity.Property(e => e.Materialid)
                    .HasColumnName("MATERIALID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Clipstate)
                    .HasColumnName("CLIPSTATE")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Createtime)
                    .HasColumnName("CREATETIME")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("'CURRENT_TIMESTAMP'");

                entity.Property(e => e.Deletedstate)
                    .HasColumnName("DELETEDSTATE")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Guid)
                    .HasColumnName("GUID")
                    .HasColumnType("varchar(64)")
                    .HasDefaultValueSql("''");

                entity.Property(e => e.Name)
                    .HasColumnName("NAME")
                    .HasColumnType("varchar(1024)")
                    .HasDefaultValueSql("''");

                entity.Property(e => e.Remark)
                    .HasColumnName("REMARK")
                    .HasColumnType("varchar(1024)")
                    .HasDefaultValueSql("''");

                entity.Property(e => e.Sectionid)
                    .HasColumnName("SECTIONID")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Taskid)
                    .HasColumnName("TASKID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Usercode)
                    .HasColumnName("USERCODE")
                    .HasColumnType("varchar(256)")
                    .HasDefaultValueSql("''");
            });

            modelBuilder.Entity<DbpMaterialDuration>(entity =>
            {
                entity.HasKey(e => e.Materialid);

                entity.ToTable("dbp_material_duration");

                entity.Property(e => e.Materialid)
                    .HasColumnName("MATERIALID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Duration)
                    .HasColumnName("DURATION")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Taskid)
                    .HasColumnName("TASKID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Trimin)
                    .HasColumnName("TRIMIN")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");
            });

            modelBuilder.Entity<DbpMaterialVideo>(entity =>
            {
                entity.HasKey(e => new { e.Materialid, e.Videotypeid, e.Videosource });

                entity.ToTable("dbp_material_video");

                entity.Property(e => e.Materialid)
                    .HasColumnName("MATERIALID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Videotypeid)
                    .HasColumnName("VIDEOTYPEID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Videosource)
                    .HasColumnName("VIDEOSOURCE")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Videofilename)
                    .IsRequired()
                    .HasColumnName("VIDEOFILENAME")
                    .HasColumnType("varchar(512)")
                    .HasDefaultValueSql("''");
            });

            modelBuilder.Entity<DbpMaterialVideoBackup>(entity =>
            {
                entity.HasKey(e => new { e.Materialid, e.Videotypeid, e.Videosource });

                entity.ToTable("dbp_material_video_backup");

                entity.Property(e => e.Materialid)
                    .HasColumnName("MATERIALID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Videotypeid)
                    .HasColumnName("VIDEOTYPEID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Videosource)
                    .HasColumnName("VIDEOSOURCE")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Videofilename)
                    .IsRequired()
                    .HasColumnName("VIDEOFILENAME")
                    .HasColumnType("varchar(512)")
                    .HasDefaultValueSql("''");
            });

           

            modelBuilder.Entity<DbpMessageRegister>(entity =>
            {
                entity.HasKey(e => new { e.Cpdeviceid, e.Msgcontrolid, e.MessageId });

                entity.ToTable("dbp_message_register");

                entity.Property(e => e.Cpdeviceid)
                    .HasColumnName("CPDEVICEID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Msgcontrolid)
                    .HasColumnName("MSGCONTROLID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.MessageId)
                    .HasColumnName("MESSAGE_ID")
                    .HasColumnType("int(11)");
            });

            modelBuilder.Entity<DbpMessages>(entity =>
            {
                entity.HasKey(e => e.MessageId);

                entity.ToTable("dbp_messages");

                entity.Property(e => e.MessageId)
                    .HasColumnName("MESSAGE_ID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.MessageDesc)
                    .HasColumnName("MESSAGE_DESC")
                    .HasColumnType("varchar(128)")
                    .HasDefaultValueSql("''");

                entity.Property(e => e.MessageName)
                    .HasColumnName("MESSAGE_NAME")
                    .HasColumnType("varchar(64)")
                    .HasDefaultValueSql("''");

                entity.Property(e => e.MessageType)
                    .HasColumnName("MESSAGE_TYPE")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");
            });

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

            modelBuilder.Entity<DbpMsgcontrol>(entity =>
            {
                entity.HasKey(e => e.Msgcontrolid);

                entity.ToTable("dbp_msgcontrol");

                entity.Property(e => e.Msgcontrolid)
                    .HasColumnName("MSGCONTROLID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Ipaddress)
                    .HasColumnName("IPADDRESS")
                    .HasColumnType("varchar(64)")
                    .HasDefaultValueSql("''");

                entity.Property(e => e.Msgcontroldesc)
                    .HasColumnName("MSGCONTROLDESC")
                    .HasColumnType("varchar(1024)")
                    .HasDefaultValueSql("''");

                entity.Property(e => e.Msgcontrolname)
                    .HasColumnName("MSGCONTROLNAME")
                    .HasColumnType("varchar(256)")
                    .HasDefaultValueSql("''");
            });

            modelBuilder.Entity<DbpMsgFailedrecord>(entity =>
            {
                entity.HasKey(e => e.MsgGuid);

                entity.ToTable("dbp_msg_failedrecord");

                entity.HasIndex(e => e.TaskId)
                    .HasName("IDX_TASKID");

                entity.Property(e => e.MsgGuid).HasColumnType("char(50)");

                entity.Property(e => e.DealMsg).HasColumnType("varchar(255)");

                entity.Property(e => e.DealTime).HasColumnType("datetime");

                entity.Property(e => e.SectionId)
                    .HasColumnName("SectionID")
                    .HasColumnType("int(5)");

                entity.Property(e => e.TaskId)
                    .HasColumnName("TaskID")
                    .HasColumnType("int(12)");
            });

            modelBuilder.Entity<DbpMsmqmsg>(entity =>
            {
                entity.HasKey(e => e.Msgid);

                entity.ToTable("dbp_msmqmsg");

                entity.Property(e => e.Msgid)
                    .HasColumnName("MSGID")
                    .HasColumnType("varchar(64)");

                entity.Property(e => e.Failedcount)
                    .HasColumnName("FAILEDCOUNT")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Lockdata)
                    .HasColumnName("LOCKDATA")
                    .HasColumnType("varchar(64)");

                entity.Property(e => e.Msgcontent)
                    .HasColumnName("MSGCONTENT")
                    .HasColumnType("text");

                entity.Property(e => e.Msgprocesstime)
                    .HasColumnName("MSGPROCESSTIME")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("'0000-00-00 00:00:00'");

                entity.Property(e => e.Msgrevtime)
                    .HasColumnName("MSGREVTIME")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("'0000-00-00 00:00:00'");

                entity.Property(e => e.Msgsendtime)
                    .HasColumnName("MSGSENDTIME")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("'CURRENT_TIMESTAMP'")
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.Msgstatus)
                    .HasColumnName("MSGSTATUS")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Msgtype)
                    .HasColumnName("MSGTYPE")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Nextretry)
                    .HasColumnName("NEXTRETRY")
                    .HasColumnType("date");
            });

            modelBuilder.Entity<DbpMsmqmsgFailed>(entity =>
            {
                entity.HasKey(e => new { e.Msgid, e.Actionid });

                entity.ToTable("dbp_msmqmsg_failed");

                entity.Property(e => e.Msgid)
                    .HasColumnName("MSGID")
                    .HasColumnType("varchar(64)");

                entity.Property(e => e.Actionid)
                    .HasColumnName("ACTIONID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Failedcount)
                    .HasColumnName("FAILEDCOUNT")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Lockdata)
                    .HasColumnName("LOCKDATA")
                    .HasColumnType("varchar(64)");

                entity.Property(e => e.Msgcontent)
                    .HasColumnName("MSGCONTENT")
                    .HasColumnType("text");

                entity.Property(e => e.Msgprocesstime)
                    .HasColumnName("MSGPROCESSTIME")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("'0000-00-00 00:00:00'");

                entity.Property(e => e.Msgrevtime)
                    .HasColumnName("MSGREVTIME")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("'0000-00-00 00:00:00'");

                entity.Property(e => e.Msgsendtime)
                    .HasColumnName("MSGSENDTIME")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("'CURRENT_TIMESTAMP'")
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.Msgstatus)
                    .HasColumnName("MSGSTATUS")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Msgtype)
                    .HasColumnName("MSGTYPE")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Nextretry)
                    .HasColumnName("NEXTRETRY")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("'0000-00-00 00:00:00'");
            });

            

            modelBuilder.Entity<DbpNewimportClip>(entity =>
            {
                entity.HasKey(e => new { e.ClipId, e.TaskId });

                entity.ToTable("dbp_newimport_clip");

                entity.Property(e => e.ClipId)
                    .HasColumnName("CLIP_ID")
                    .HasColumnType("varchar(128)");

                entity.Property(e => e.TaskId)
                    .HasColumnName("TASK_ID")
                    .HasColumnType("varchar(128)")
                    .HasDefaultValueSql("''");

                entity.Property(e => e.ClipDuration)
                    .HasColumnName("CLIP_DURATION")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.ClipEvent)
                    .HasColumnName("CLIP_EVENT")
                    .HasColumnType("text");

                entity.Property(e => e.ClipIn)
                    .HasColumnName("CLIP_IN")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.ClipMetadata)
                    .HasColumnName("CLIP_METADATA")
                    .HasColumnType("text");

                entity.Property(e => e.ClipName)
                    .HasColumnName("CLIP_NAME")
                    .HasColumnType("varchar(1000)")
                    .HasDefaultValueSql("''");

                entity.Property(e => e.ClipOut)
                    .HasColumnName("CLIP_OUT")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.ClipState)
                    .HasColumnName("CLIP_STATE")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.ClipTctype)
                    .HasColumnName("CLIP_TCTYPE")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.ClipTcvalue)
                    .HasColumnName("CLIP_TCVALUE")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.ClipTrimin)
                    .HasColumnName("CLIP_TRIMIN")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.ClipTrimout)
                    .HasColumnName("CLIP_TRIMOUT")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.ClipType)
                    .HasColumnName("CLIP_TYPE")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.DevId)
                    .HasColumnName("DEV_ID")
                    .HasColumnType("varchar(128)")
                    .HasDefaultValueSql("''");
            });

            modelBuilder.Entity<DbpNewimportRemote>(entity =>
            {
                entity.HasKey(e => e.RemoteId);

                entity.ToTable("dbp_newimport_remote");

                entity.Property(e => e.RemoteId)
                    .HasColumnName("REMOTE_ID")
                    .HasColumnType("varchar(128)");

                entity.Property(e => e.ClipId)
                    .IsRequired()
                    .HasColumnName("CLIP_ID")
                    .HasColumnType("varchar(128)");

                entity.Property(e => e.RemoteState)
                    .HasColumnName("REMOTE_STATE")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.TaskId)
                    .IsRequired()
                    .HasColumnName("TASK_ID")
                    .HasColumnType("varchar(128)");
            });

            modelBuilder.Entity<DbpNewimportTask>(entity =>
            {
                entity.HasKey(e => e.TaskId);

                entity.ToTable("dbp_newimport_task");

                entity.Property(e => e.TaskId)
                    .HasColumnName("TASK_ID")
                    .HasColumnType("varchar(128)");

                entity.Property(e => e.TaskBegintime)
                    .HasColumnName("TASK_BEGINTIME")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("'CURRENT_TIMESTAMP'");

                entity.Property(e => e.TaskCommand)
                    .HasColumnName("TASK_COMMAND")
                    .HasColumnType("text");

                entity.Property(e => e.TaskDuration)
                    .HasColumnName("TASK_DURATION")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.TaskEndtime)
                    .HasColumnName("TASK_ENDTIME")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("'CURRENT_TIMESTAMP'");

                entity.Property(e => e.TaskErr)
                    .HasColumnName("TASK_ERR")
                    .HasColumnType("varchar(1000)")
                    .HasDefaultValueSql("''");

                entity.Property(e => e.TaskFileinfo)
                    .HasColumnName("TASK_FILEINFO")
                    .HasColumnType("text");

                entity.Property(e => e.TaskHProgress)
                    .HasColumnName("TASK_H_PROGRESS")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.TaskLProgress)
                    .HasColumnName("TASK_L_PROGRESS")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.TaskMetadata)
                    .HasColumnName("TASK_METADATA")
                    .HasColumnType("text");

                entity.Property(e => e.TaskName)
                    .HasColumnName("TASK_NAME")
                    .HasColumnType("varchar(1000)")
                    .HasDefaultValueSql("''");

                entity.Property(e => e.TaskServiceid)
                    .HasColumnName("TASK_SERVICEID")
                    .HasColumnType("varchar(128)")
                    .HasDefaultValueSql("''");

                entity.Property(e => e.TaskState)
                    .HasColumnName("TASK_STATE")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.TaskType)
                    .HasColumnName("TASK_TYPE")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.TaskUser)
                    .HasColumnName("TASK_USER")
                    .HasColumnType("varchar(128)")
                    .HasDefaultValueSql("''");
            });

            modelBuilder.Entity<DbpObjectstateinfo>(entity =>
            {
                entity.HasKey(e => new { e.Objectid, e.Objecttypeid });

                entity.ToTable("dbp_objectstateinfo");

                entity.Property(e => e.Objectid)
                    .HasColumnName("OBJECTID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Objecttypeid)
                    .HasColumnName("OBJECTTYPEID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Begintime)
                    .HasColumnName("BEGINTIME")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("'CURRENT_TIMESTAMP'");

                entity.Property(e => e.Locklock)
                    .HasColumnName("LOCKLOCK")
                    .HasColumnType("varchar(128)");

                entity.Property(e => e.Timeout)
                    .HasColumnName("TIMEOUT")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasColumnName("USERNAME")
                    .HasColumnType("varchar(128)");
            });

            modelBuilder.Entity<DbpPlans>(entity =>
            {
                entity.HasKey(e => e.Planid);

                entity.ToTable("dbp_plans");

                entity.Property(e => e.Planid)
                    .HasColumnName("PLANID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Createdate)
                    .HasColumnName("CREATEDATE")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("'CURRENT_TIMESTAMP'")
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.Metadata)
                    .HasColumnName("METADATA")
                    .HasColumnType("varchar(4000)");

                entity.Property(e => e.Plancontent)
                    .HasColumnName("PLANCONTENT")
                    .HasColumnType("varchar(4000)");
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

            modelBuilder.Entity<DbpPolicyuserclass>(entity =>
            {
                entity.HasKey(e => new { e.Policyid, e.Classcode });

                entity.ToTable("dbp_policyuserclass");

                entity.Property(e => e.Policyid)
                    .HasColumnName("POLICYID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Classcode)
                    .HasColumnName("CLASSCODE")
                    .HasColumnType("varchar(64)");
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

            

            modelBuilder.Entity<DbpRecdeviceType>(entity =>
            {
                entity.HasKey(e => e.Devicetypeid);

                entity.ToTable("dbp_recdevice_type");

                entity.Property(e => e.Devicetypeid)
                    .HasColumnName("DEVICETYPEID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.DeviceTypeDesc)
                    .HasColumnName("DEVICE_TYPE_DESC")
                    .HasColumnType("varchar(128)")
                    .HasDefaultValueSql("''");

                entity.Property(e => e.DeviceTypeName)
                    .HasColumnName("DEVICE_TYPE_NAME")
                    .HasColumnType("varchar(64)")
                    .HasDefaultValueSql("''");
            });

            modelBuilder.Entity<DbpRecunit>(entity =>
            {
                entity.HasKey(e => e.Recid);

                entity.ToTable("dbp_recunit");

                entity.Property(e => e.Recid)
                    .HasColumnName("RECID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Name)
                    .HasColumnName("NAME")
                    .HasColumnType("varchar(128)")
                    .HasDefaultValueSql("''");

                entity.Property(e => e.Recdesc)
                    .HasColumnName("RECDESC")
                    .HasColumnType("varchar(1024)")
                    .HasDefaultValueSql("''");
            });

            modelBuilder.Entity<DbpRouterctrolsetting>(entity =>
            {
                entity.HasKey(e => e.Rcdeviceid);

                entity.ToTable("dbp_routerctrolsetting");

                entity.Property(e => e.Rcdeviceid)
                    .HasColumnName("RCDEVICEID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Devicetypeid)
                    .HasColumnName("DEVICETYPEID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Rccomtrolcomport)
                    .HasColumnName("RCCOMTROLCOMPORT")
                    .HasColumnType("varchar(64)")
                    .HasDefaultValueSql("''");

                entity.Property(e => e.Rcdevicedesc)
                    .HasColumnName("RCDEVICEDESC")
                    .HasColumnType("varchar(1024)")
                    .HasDefaultValueSql("''");

                entity.Property(e => e.Rcdevicename)
                    .HasColumnName("RCDEVICENAME")
                    .HasColumnType("varchar(64)")
                    .HasDefaultValueSql("''");

                entity.Property(e => e.Rcdevicetype)
                    .HasColumnName("RCDEVICETYPE")
                    .HasColumnType("varchar(64)")
                    .HasDefaultValueSql("''");

                entity.Property(e => e.Rcinportnum)
                    .HasColumnName("RCINPORTNUM")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Rcipaddress)
                    .HasColumnName("RCIPADDRESS")
                    .HasColumnType("varchar(64)")
                    .HasDefaultValueSql("''");

                entity.Property(e => e.Rcoutportnum)
                    .HasColumnName("RCOUTPORTNUM")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");
            });

            modelBuilder.Entity<DbpScheduler>(entity =>
            {
                entity.HasKey(e => e.Schedulerid);

                entity.ToTable("dbp_scheduler");

                entity.Property(e => e.Schedulerid)
                    .HasColumnName("SCHEDULERID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Ipaddress)
                    .HasColumnName("IPADDRESS")
                    .HasColumnType("varchar(64)")
                    .HasDefaultValueSql("''");

                entity.Property(e => e.Name)
                    .HasColumnName("NAME")
                    .HasColumnType("varchar(256)")
                    .HasDefaultValueSql("''");

                entity.Property(e => e.Schedulerdesc)
                    .HasColumnName("SCHEDULERDESC")
                    .HasColumnType("varchar(1024)")
                    .HasDefaultValueSql("''");
            });

            modelBuilder.Entity<DbpSchedulerRecunit>(entity =>
            {
                entity.HasKey(e => new { e.Schedulerid, e.Recid });

                entity.ToTable("dbp_scheduler_recunit");

                entity.Property(e => e.Schedulerid)
                    .HasColumnName("SCHEDULERID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Recid)
                    .HasColumnName("RECID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Activesch)
                    .HasColumnName("ACTIVESCH")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Respond)
                    .HasColumnName("RESPOND")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("'CURRENT_TIMESTAMP'");
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

            
            modelBuilder.Entity<DbpTranscodePolicy>(entity =>
            {
                entity.HasKey(e => new { e.Trancodeid, e.Policyid });

                entity.ToTable("dbp_transcode_policy");

                entity.Property(e => e.Trancodeid)
                    .HasColumnName("TRANCODEID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Policyid)
                    .HasColumnName("POLICYID")
                    .HasColumnType("int(11)");
            });

            modelBuilder.Entity<DbpTranscodeTemplate>(entity =>
            {
                entity.HasKey(e => e.Trancodeid);

                entity.ToTable("dbp_transcode_template");

                entity.Property(e => e.Trancodeid)
                    .HasColumnName("TRANCODEID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.TranscodeInfo)
                    .HasColumnName("TRANSCODE_INFO")
                    .HasColumnType("varchar(4000)")
                    .HasDefaultValueSql("''");

                entity.Property(e => e.Transcodename)
                    .HasColumnName("TRANSCODENAME")
                    .HasColumnType("varchar(256)");
            });

            modelBuilder.Entity<DbpTransferPolicy>(entity =>
            {
                entity.HasKey(e => new { e.Transferid, e.Policyid });

                entity.ToTable("dbp_transfer_policy");

                entity.Property(e => e.Transferid)
                    .HasColumnName("TRANSFERID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Policyid)
                    .HasColumnName("POLICYID")
                    .HasColumnType("int(11)");
            });

            modelBuilder.Entity<DbpTransferTemplate>(entity =>
            {
                entity.HasKey(e => e.Transferid);

                entity.ToTable("dbp_transfer_template");

                entity.Property(e => e.Transferid)
                    .HasColumnName("TRANSFERID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.TransferInfo)
                    .HasColumnName("TRANSFER_INFO")
                    .HasColumnType("varchar(4000)")
                    .HasDefaultValueSql("''");

                entity.Property(e => e.Transfername)
                    .HasColumnName("TRANSFERNAME")
                    .HasColumnType("varchar(256)");
            });

            modelBuilder.Entity<DbpUserparamlog>(entity =>
            {
                entity.HasKey(e => e.Logid);

                entity.ToTable("dbp_userparamlog");

                entity.Property(e => e.Logid)
                    .HasColumnName("LOGID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Moddifyuserid)
                    .HasColumnName("MODDIFYUSERID")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Opratime)
                    .HasColumnName("OPRATIME")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("'CURRENT_TIMESTAMP'");

                entity.Property(e => e.Opratype)
                    .HasColumnName("OPRATYPE")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Userid)
                    .HasColumnName("USERID")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");
            });

            modelBuilder.Entity<DbpUserparamMap>(entity =>
            {
                entity.HasKey(e => e.Usercode);

                entity.ToTable("dbp_userparam_map");

                entity.Property(e => e.Usercode)
                    .HasColumnName("USERCODE")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.Captureparamid)
                    .HasColumnName("CAPTUREPARAMID")
                    .HasColumnType("int(11)");
            });

            modelBuilder.Entity<DbpUsersettings>(entity =>
            {
                entity.HasKey(e => new { e.Usercode, e.Settingtype });

                entity.ToTable("dbp_usersettings");

                entity.Property(e => e.Usercode)
                    .HasColumnName("USERCODE")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.Settingtype)
                    .HasColumnName("SETTINGTYPE")
                    .HasColumnType("varchar(128)");

                entity.Property(e => e.Settingtext)
                    .HasColumnName("SETTINGTEXT")
                    .HasColumnType("varchar(4000)")
                    .HasDefaultValueSql("''");

                entity.Property(e => e.Settingtextlong)
                    .HasColumnName("SETTINGTEXTLONG")
                    .HasColumnType("text");
            });

            modelBuilder.Entity<DbpUsertemplate>(entity =>
            {
                entity.HasKey(e => e.Templateid);

                entity.ToTable("dbp_usertemplate");

                entity.Property(e => e.Templateid)
                    .HasColumnName("TEMPLATEID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Templatecontent)
                    .HasColumnName("TEMPLATECONTENT")
                    .HasColumnType("text");

                entity.Property(e => e.Templatename)
                    .IsRequired()
                    .HasColumnName("TEMPLATENAME")
                    .HasColumnType("varchar(256)");

                entity.Property(e => e.Usercode)
                    .IsRequired()
                    .HasColumnName("USERCODE")
                    .HasColumnType("varchar(256)");
            });

            modelBuilder.Entity<DbpVidoetype>(entity =>
            {
                entity.HasKey(e => e.Vidoetypeid);

                entity.ToTable("dbp_vidoetype");

                entity.Property(e => e.Vidoetypeid)
                    .HasColumnName("VIDOETYPEID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Vidoetypedesc)
                    .HasColumnName("VIDOETYPEDESC")
                    .HasColumnType("varchar(256)")
                    .HasDefaultValueSql("''");
            });

            

            modelBuilder.Entity<DbpXdcamXmploitPlan>(entity =>
            {
                entity.HasKey(e => e.Diskid);

                entity.ToTable("dbp_xdcam_xmploit_plan");

                entity.Property(e => e.Diskid)
                    .HasColumnName("DISKID")
                    .HasColumnType("varchar(128)");

                entity.Property(e => e.Planningguid)
                    .IsRequired()
                    .HasColumnName("PLANNINGGUID")
                    .HasColumnType("varchar(128)");

                entity.Property(e => e.Planninginfo)
                    .HasColumnName("PLANNINGINFO")
                    .HasColumnType("text");
            });

            modelBuilder.Entity<LoginUserAddress>(entity =>
            {
                entity.HasKey(e => e.Ip);

                entity.ToTable("login_user_address");

                entity.Property(e => e.Ip)
                    .HasColumnName("IP")
                    .HasColumnType("varchar(64)");

                entity.Property(e => e.Logintime)
                    .HasColumnName("LOGINTIME")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("'CURRENT_TIMESTAMP'")
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.Port)
                    .HasColumnName("PORT")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Username)
                    .HasColumnName("USERNAME")
                    .HasColumnType("varchar(128)")
                    .HasDefaultValueSql("''");
            });

            modelBuilder.Entity<Sequence>(entity =>
            {
                entity.HasKey(e => e.Name);

                entity.ToTable("_sequence");

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasColumnType("varchar(100)");

                entity.Property(e => e.CurrentVal)
                    .HasColumnName("current_val")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.IncrementSize)
                    .HasColumnName("increment_size")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'1'");
            });

            

            
        }
    }
}
