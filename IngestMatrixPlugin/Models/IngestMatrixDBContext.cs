using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IngestMatrixPlugin.Models.DB;
using Microsoft.EntityFrameworkCore;

namespace IngestMatrixPlugin.Models
{
    public partial class IngestMatrixDBContext : DbContext
    {
        public IngestMatrixDBContext(DbContextOptions<IngestMatrixDBContext> options) : base(options) { }

        public virtual DbSet<DbpLevelrelation> DbpLevelrelation { get; set; }
        public virtual DbSet<DbpMapinport> DbpMapinport { get; set; }
        public virtual DbSet<DbpMapoutport> DbpMapoutport { get; set; }
        public virtual DbSet<DbpMatrixinfo> DbpMatrixinfo { get; set; }
        public virtual DbSet<DbpMatrixrout> DbpMatrixrout { get; set; }
        public virtual DbSet<DbpMatrixtypeinfo> DbpMatrixtypeinfo { get; set; }
        public virtual DbSet<DbpRcdindesc> DbpRcdindesc { get; set; }
        public virtual DbSet<DbpRcdoutdesc> DbpRcdoutdesc { get; set; }
        public virtual DbSet<DbpVirtualmatrixportstate> DbpVirtualmatrixportstate { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
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

                entity.Property(e => e.Ipaddress)
                    .HasColumnName("IPADDRESS")
                    .HasColumnType("varchar(255)");
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
            });

            modelBuilder.Entity<DbpVirtualmatrixportstate>(entity =>
            {
                entity.HasKey(e => new { e.Virtualinport, e.Virtualoutport });

                entity.ToTable("dbp_virtualmatrixportstate");

                entity.Property(e => e.Virtualinport)
                    .HasColumnName("VIRTUALINPORT")
                    .HasColumnType("int(11)");
                entity.Property(e => e.Virtualoutport)
                    .HasColumnName("VIRTUALOUTPORT")
                    .HasColumnType("int(11)");
                entity.Property(e => e.Matrixid)
                    .HasColumnName("MATRIXID")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");
                entity.Property(e => e.State)
                    .HasColumnName("STATE")
                    .HasColumnType("int(11)");
                entity.Property(e => e.Lastoprtime)
                    .HasColumnName("LASTOPRTIME")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("'CURRENT_TIMESTAMP'");
            });
        }
    }
}
