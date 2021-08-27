using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NZMSA_HYD.Model;

namespace NZMSA_HYD.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options): base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Day> Days { get; set; }
        public DbSet<Event> Events { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<User>()
                .HasMany(u => u.Days)
                .WithOne(d => d.User!)
                .HasForeignKey(d => d.UserId);

            modelBuilder
                .Entity<Day>()
                .HasMany(d => d.Events)
                .WithOne(e => e.Day)
                .HasForeignKey(e => e.DayId);

            modelBuilder
                .Entity<Event>()
                .HasOne(e => e.Day)
                .WithMany(d => d.Events)
                .HasForeignKey(e => e.DayId); 
                
                
                

        }
    }
}
