using System;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace Dairy_Server.Entities
{
    public partial class MyDairyContext : DbContext
    {
        public MyDairyContext()
        {
        }

        public MyDairyContext(DbContextOptions<MyDairyContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Dairy> Dairies { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "Chinese_PRC_CI_AS");

            modelBuilder.Entity<Dairy>(entity =>
            {
                entity.HasKey(e => e.Uid);

                entity.Property(e => e.Enabled)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.Thema).HasMaxLength(20);

                entity.Property(e => e.Uid).ValueGeneratedOnAdd();

                entity.Property(e => e.Wheather)
                    .HasMaxLength(10)
                    .IsFixedLength(true);

                entity.Property(e => e.WroteDate).HasColumnType("date");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
