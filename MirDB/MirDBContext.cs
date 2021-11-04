using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MirAI.AI;
using System;

namespace MirAI.DB
{
    /// <summary>
    /// Контекст БД
    /// </summary>
    public class MirDBContext : DbContext
    {
        public DbSet<Unit> Units { get; set; }
        public DbSet<Program> Programs { get; set; }
        public DbSet<Node> Nodes { get; set; }
        public DbSet<NodeLink> Links { get; set; }

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
            modelBuilder
                 .Entity<NodeLink>()
                 .HasKey(c => new { c.FromId, c.ToId });

            modelBuilder.Entity<Program>().HasAlternateKey(p => p.Name);
        }
    }
}
