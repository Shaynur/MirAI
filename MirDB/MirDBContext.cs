using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MirAI.AI;
using System;

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
            optionsBuilder.UseSqlite(@"data source=D:\Projects\MirAI\DB\MirAI.db");
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
    }
}
