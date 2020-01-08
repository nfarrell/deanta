
using System;
using System.Linq;
using System.Threading.Tasks;
using Deanta.Cosmos.Infrastructure;
using Deanta.Cosmos.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Deanta.Cosmos.Test.CosmosTestDb
{
    public class DeantaDbContext : DbContext
    {
        public DbSet<CosmosTodo> Todos { get; set; }

        public DeantaDbContext(DbContextOptions<DeantaDbContext> options)
            : base(options) { }

        public override int SaveChanges()
        {
            AppendAuditInfo();
            return base.SaveChanges();
        }

        public async Task<int> SaveChangesAsync()
        {
            AppendAuditInfo();
            return await base.SaveChangesAsync();
        }


        private void PreventDelete()
        {
            var deletedEntries = ChangeTracker.Entries().Where(x => x.Entity is DeantaEntity && (x.State == EntityState.Deleted));

            //todo: needs to be completed

            foreach (var entry in deletedEntries)
            {
                //((DeantaEntity)entry.Entity).SetUpdated(DateTime.UtcNow, "");
            }
        }

        private void AppendAuditInfo()
        {
            var entries = ChangeTracker.Entries().Where(x => x.Entity is IAuditableEntity
                                                             && (x.State == EntityState.Added || x.State == EntityState.Modified));

            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    ((IAuditableEntity)entry.Entity).SetCreated(DateTime.UtcNow, "");
                }

                ((IAuditableEntity)entry.Entity).SetUpdated(DateTime.UtcNow, "");
            }
        }
    }
}