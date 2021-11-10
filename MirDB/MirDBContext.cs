using Microsoft.EntityFrameworkCore;
using MirAI.AI;
using System;
using System.IO;

namespace MirAI.DB
{
    public class MirDBContext : DbContext
    {
        public DbSet<Unit> Units { get; set; }
        public DbSet<Program> Programs { get; set; }
        public DbSet<Node> Nodes { get; set; }

        public MirDBContext()
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string datasource = GetFullPathDBFile("MirAI.db");
            optionsBuilder.UseSqlite(@"data source=" + datasource);
            //optionsBuilder.LogTo(Console.WriteLine, LogLevel.Information);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<NodeLink>()
                 .HasKey(c => new { c.FromId, c.ToId });
            modelBuilder.Entity<NodeLink>()
                .HasOne(n => n.From)
                .WithMany(n => n.LinkTo)
                .HasForeignKey(n => n.FromId);
            modelBuilder.Entity<NodeLink>()
                .HasOne(n => n.To)
                .WithMany(n => n.LinkFrom)
                .HasForeignKey(n => n.ToId);

            modelBuilder.Entity<Program>().Property(n => n.Name).HasColumnType("varchar(30)");
        }

        public string GetFullPathDBFile(string fileName)
        {
#if DEBUG
            // Этот путь должен быть на ДЕБАГЕ:
            string datasource = @"D:\Projects\MirAI\DB";
#else
            // Этот путь должен быть на РЕЛИЗЕ:
            string datasource = Path.Combine(Directory.GetCurrentDirectory(), "DB");
#endif
            Directory.CreateDirectory(datasource);
            return Path.Combine(datasource, fileName);
        }
    }
}
