using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShardingCore.DbContexts.ShardingDbContexts;
using System;
using System.Collections.Generic;
using System.Text;

namespace IngestTaskPlugin.Models
{
    public class TaskDBMap : IEntityTypeConfiguration<DbpTask>
    {
        public void Configure(EntityTypeBuilder<DbpTask> entity)
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
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.NewBegintime)
                    .HasColumnName("NEW_BEGINTIME")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.NewEndtime)
                    .HasColumnName("NEW_ENDTIME")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

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
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

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
            
        }
    }
    public class DBTaskContext : DbContext, IShardingTableDbContext
    {
        public DBTaskContext(DbContextOptions<DBTaskContext> opt) : base(opt)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new TaskDBMap());
        }

        public string ModelChangeKey { get; set; }
    }
}
