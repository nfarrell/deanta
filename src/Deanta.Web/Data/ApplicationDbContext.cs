using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Deanta.Web.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

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
