using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

using ValidLoaderShared.Models;

namespace ValidLoaderShared.Context
{
    public class PageLoaderServiceContext : DbContext
    {
        public PageLoaderServiceContext(DbContextOptions<PageLoaderServiceContext> options)
            : base(options)
        {
        }
        public DbSet<Proxy> Proxies { get; set; }
        public DbSet<Domain> Domains { get; set; }
        public DbSet<LoadingInfo> LoadingInfos { get; set; }

        public DbSet<TaskProcessingResult> Results { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure the relationship between Proxy and LoadingInfo
            modelBuilder.Entity<Proxy>()
                .HasMany(p => p.LoadingInfos)
                .WithOne(li => li.Proxy)
                .HasForeignKey(li => li.ProxyAddress)
                .OnDelete(DeleteBehavior.Cascade); // Cascade delete

            // Configure the relationship between Domain and LoadingInfo
            modelBuilder.Entity<Domain>()
                .HasMany(d => d.LoadingInfos)
                .WithOne(li => li.Domain)
                .HasForeignKey(li => li.DomainName)
                .OnDelete(DeleteBehavior.Cascade); // Cascade delete
        }

    }

}
