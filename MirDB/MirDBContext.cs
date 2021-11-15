using Microsoft.EntityFrameworkCore;
using MirAI.AI;
using System;
using System.IO;

namespace MirAI.DB {
    public class MirDBContext : DbContext {
        public DbSet<Unit> Units { get; set; }
        public DbSet<Program> Programs { get; set; }
        public DbSet<Node> Nodes { get; set; }

        public static string DBFileName { get; set; } = string.Empty;
        public MirDBContext() {
            if (DBFileName == string.Empty) {
                SetDefaultDBFileName( "MirAI.db" );
            }
            Database.EnsureCreated();
        }

        protected override void OnConfiguring( DbContextOptionsBuilder optionsBuilder ) {
            //Console.WriteLine( DBFileName );
            optionsBuilder.UseSqlite( @"data source=" + DBFileName );
            //optionsBuilder.LogTo(Console.WriteLine, LogLevel.Information);
        }

        protected override void OnModelCreating( ModelBuilder modelBuilder ) {
            modelBuilder.Entity<NodeLink>()
                 .HasKey( c => new { c.FromId, c.ToId } );
            modelBuilder.Entity<NodeLink>()
                .HasOne( n => n.From )
                .WithMany( n => n.LinkTo )
                .HasForeignKey( n => n.FromId );
            modelBuilder.Entity<NodeLink>()
                .HasOne( n => n.To )
                .WithMany( n => n.LinkFrom )
                .HasForeignKey( n => n.ToId );

            modelBuilder.Entity<Program>().Property( n => n.Name ).HasColumnType( "varchar(30)" );
        }

        public void SetDefaultDBFileName( string fileName ) {
#if DEBUG
            // Этот путь должен быть на ДЕБАГЕ:
            DBFileName = @"D:\Projects\MirAI\DB";
#else
            // Этот путь должен быть на РЕЛИЗЕ:
            DBFileName = Path.Combine(Directory.GetCurrentDirectory(), "DB");
#endif
            Directory.CreateDirectory( DBFileName );
            DBFileName = Path.Combine( DBFileName, fileName );
        }
    }
}
