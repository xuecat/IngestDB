using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace IngestGlobalPlugin.Models
{
    public partial class IngestGlobalDBContext : DbContext
    {
        public IngestGlobalDBContext()
        {
        }

        public IngestGlobalDBContext(DbContextOptions<IngestGlobalDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<DbpCaptureparamtemplate> DbpCaptureparamtemplate { get; set; }
        public virtual DbSet<DbpCaptureparamtemplateMap> DbpCaptureparamtemplateMap { get; set; }
        public virtual DbSet<DbpFileformatinfo> DbpFileformatinfo { get; set; }
        public virtual DbSet<DbpGlobal> DbpGlobal { get; set; }
        public virtual DbSet<DbpGlobalProgram> DbpGlobalProgram { get; set; }
        public virtual DbSet<DbpGlobalState> DbpGlobalState { get; set; }
        public virtual DbSet<DbpObjectstateinfo> DbpObjectstateinfo { get; set; }
        public virtual DbSet<DbpUsersettings> DbpUsersetting { get; set; }
        public virtual DbSet<DbpUsertemplate> DbpUsertemplate { get; set; }
        public virtual DbSet<DbpUserparamMap> DbpUserparamMap { get; set; }
        public virtual DbSet<DbpMsgFailedrecord> DbpMsgFailedrecord { get; set; }
        public virtual DbSet<DbpMsmqmsg> DbpMsmqmsg { get; set; }
        public virtual DbSet<DbpMsmqmsgFailed> DbpMsmqmsgFailed { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DbpCaptureparamtemplate>(entity =>
            {
                entity.HasKey(e => e.Captureparamid);

                entity.ToTable("dbp_captureparamtemplate");

                entity.Property(e => e.Captureparamid)
                    .HasColumnName("CAPTUREPARAMID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Captemplatename)
                    .IsRequired()
                    .HasColumnName("CAPTEMPLATENAME")
                    .HasColumnType("varchar(128)")
                    .HasDefaultValueSql("''");

                entity.Property(e => e.Captureparam)
                    .IsRequired()
                    .HasColumnName("captureparam")
                    .HasColumnType("text");
            });

            modelBuilder.Entity<DbpCaptureparamtemplateMap>(entity =>
            {
                entity.HasKey(e => e.Paramid);

                entity.ToTable("dbp_captureparamtemplate_map");

                entity.Property(e => e.Paramid)
                    .HasColumnName("PARAMID")
                    .HasColumnType("int(10)");

                entity.Property(e => e.Hdtemplateid)
                    .HasColumnName("HDTEMPLATEID")
                    .HasColumnType("int(10)");

                entity.Property(e => e.Sdtemplateid)
                    .HasColumnName("SDTEMPLATEID")
                    .HasColumnType("int(10)");

                entity.Property(e => e.Uhdtemplateid)
                    .HasColumnName("UHDTEMPLATEID")
                    .HasColumnType("int(10)");
            });

            modelBuilder.Entity<DbpFileformatinfo>(entity =>
            {
                entity.HasKey(e => e.Key);

                entity.ToTable("dbp_fileformatinfo");

                entity.Property(e => e.Key)
                    .HasColumnName("KEY")
                    .HasColumnType("varchar(128)")
                    .HasDefaultValueSql("''");

                entity.Property(e => e.Formatinfo)
                    .HasColumnName("FORMATINFO")
                    .HasColumnType("text");
            });

            modelBuilder.Entity<DbpGlobal>(entity =>
            {
                entity.HasKey(e => e.GlobalKey);

                entity.ToTable("dbp_global");

                entity.Property(e => e.GlobalKey)
                    .HasColumnName("GLOBAL_KEY")
                    .HasColumnType("varchar(128)");

                entity.Property(e => e.Changetime)
                    .HasColumnName("CHANGETIME")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("'CURRENT_TIMESTAMP'");

                entity.Property(e => e.GlobalValue)
                    .IsRequired()
                    .HasColumnName("GLOBAL_VALUE")
                    .HasColumnType("varchar(2000)");

                entity.Property(e => e.Paramdesc)
                    .HasColumnName("PARAMDESC")
                    .HasColumnType("varchar(1024)")
                    .HasDefaultValueSql("''");

                entity.Property(e => e.Restartmodules)
                    .HasColumnName("RESTARTMODULES")
                    .HasColumnType("varchar(256)")
                    .HasDefaultValueSql("''");
            });

            modelBuilder.Entity<DbpGlobalProgram>(entity =>
            {
                entity.HasKey(e => e.GlobalId);

                entity.ToTable("dbp_global_program");

                entity.Property(e => e.GlobalId)
                    .HasColumnName("GLOBAL_ID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.ProgramId)
                    .HasColumnName("PROGRAM_ID")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.ProgramName)
                    .HasColumnName("PROGRAM_NAME")
                    .HasColumnType("varchar(128)")
                    .HasDefaultValueSql("''");

                entity.Property(e => e.Signalsrcid)
                    .HasColumnName("SIGNALSRCID")
                    .HasColumnType("int(11)");
            });

            modelBuilder.Entity<DbpGlobalState>(entity =>
            {
                entity.HasKey(e => e.Label);

                entity.ToTable("dbp_global_state");

            });

            modelBuilder.Entity<DbpObjectstateinfo>(entity => {
                entity.HasKey(e => new { e.Objectid, e.Objecttypeid });

                entity.ToTable("dbp_objectstateinfo");

                entity.Property(e => e.Objectid).HasColumnName("OBJECTID").HasColumnType("int(11)");
                entity.Property(e => e.Objecttypeid).HasColumnName("OBJECTTYPEID").HasColumnType("int(11)");
                entity.Property(e => e.Username).HasColumnName("USERNAME").HasColumnType("varchar(128)").HasDefaultValueSql("''");
                entity.Property(e => e.Begintime).HasColumnName("BEGINTIME").HasColumnType("timestamp");
                entity.Property(e => e.Timeout).HasColumnName("TIMEOUT").HasColumnType("int(11)");
                entity.Property(e => e.Locklock).HasColumnName("LOCKLOCK").HasColumnType("varchar(128)");
            });

            modelBuilder.Entity<DbpUsersettings>(entity => {
                entity.HasKey(e => new { e.Usercode, e.Settingtype });
                entity.ToTable("dbp_usersettings");
                entity.Property(e => e.Usercode).HasColumnName("USERCODE").HasColumnType("varchar(255)");
                entity.Property(e => e.Settingtype).HasColumnName("SETTINGTYPE").HasColumnType("varchar(128)");
                entity.Property(e => e.Settingtext).HasColumnName("SETTINGTEXT").HasColumnType("varchar(4000)");
                entity.Property(e => e.Settingtextlong).HasColumnName("SETTINGTEXTLONG").HasColumnType("text");
            });

            modelBuilder.Entity<DbpUsertemplate>(entity => {
                entity.HasKey(e => e.Templateid);
                entity.ToTable("dbp_usertemplate");
                entity.Property(e => e.Templateid).HasColumnName("TEMPLATEID").HasColumnType("int(11)");
                entity.Property(e => e.Usercode).HasColumnName("USERCODE").HasColumnType("varchar(256)");
                entity.Property(e => e.Templatename).HasColumnName("TEMPLATENAME").HasColumnType("varchar(256)");
                entity.Property(e => e.Templatecontent)
.HasColumnName("TEMPLATECONTENT").HasColumnType("text");
            });

            modelBuilder.Entity<DbpUserparamMap>(entity =>
            {
                entity.HasKey(e => e.Usercode);
                entity.ToTable("dbp_userparam_map");
                entity.Property(e => e.Usercode).HasColumnName("USERCODE").HasColumnType("varchar(255)");
                entity.Property(e => e.Captureparamid).HasColumnName("CAPTUREPARAMID").HasColumnType("int(11)");
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

        }

        [DbFunction]
        public static int next_val(string value)
        {
            throw new NotImplementedException();
        }

    }
}
