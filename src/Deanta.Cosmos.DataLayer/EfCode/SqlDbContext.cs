using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Deanta.Cosmos.DataLayer.EfClassesSql;
using Deanta.Cosmos.DataLayer.EfCode.Configurations;
using Deanta.Cosmos.DataLayer.Interfaces;
using Deanta.Cosmos.DataLayer.NoSqlCode;
using Microsoft.EntityFrameworkCore;

namespace Deanta.Cosmos.DataLayer.EfCode
{
    public class DeantaSqlDbContext : DbContext //IdentityDbContext
    {
        private readonly ITodoUpdater _todoUpdater;

        //NOTE: if the todoUpdater isn't provided, then it reverts to a normal SaveChanges.
        public DeantaSqlDbContext(DbContextOptions<DeantaSqlDbContext> options, ITodoUpdater todoUpdater = null)
            : base(options)
        {
            _todoUpdater = todoUpdater;
        }

        public DbSet<Todo> Todos { get; set; }

        //replace with ApplicationUser from .Net identity OR implement ID4
        public DbSet<Owner> Owners { get; set; }

        public DbSet<TodoOwner> TodoOwners { get; set; }

        //I only have to override these two version of SaveChanges, as the other two versions call these
        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            AppendAuditInfo();

            try
            {
                var numTodosChanged = _todoUpdater?.FindNumTodosChanged(this) ?? 0;
                //This stops ChangeTracker being called twice
                ChangeTracker.AutoDetectChangesEnabled = false;
                if (numTodosChanged == 0)
                    return base.SaveChanges(acceptAllChangesOnSuccess);
                return _todoUpdater.CallBaseSaveChangesAndNoSqlWriteInTransaction(this, numTodosChanged,
                    () => base.SaveChanges(acceptAllChangesOnSuccess));
            }
            finally
            {
                ChangeTracker.AutoDetectChangesEnabled = true;
            }

            //return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            AppendAuditInfo();

            try
            {
                var numTodosChanged = _todoUpdater?.FindNumTodosChanged(this) ?? 0;
                //This stops ChangeTracker being called twice
                ChangeTracker.AutoDetectChangesEnabled = false;
                if (numTodosChanged == 0)
                    return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
                return await _todoUpdater.CallBaseSaveChangesWithNoSqlWriteInTransactionAsync(this, numTodosChanged,
                    () => base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken));
            }
            finally
            {
                ChangeTracker.AutoDetectChangesEnabled = true;
            }

            //return await base.SaveChangesAsync();
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

        protected override void
            OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new TodoConfig());
            modelBuilder.ApplyConfiguration(new TodoOwnerConfig());
        }
    }
}

