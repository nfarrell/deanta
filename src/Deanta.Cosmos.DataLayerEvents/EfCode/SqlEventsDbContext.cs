
using Deanta.Cosmos.DataLayerEvents.EfClasses;
using Deanta.Cosmos.DataLayerEvents.EfCode.Configurations;
using GenericEventRunner.ForDbContext;
using Microsoft.EntityFrameworkCore;

namespace Deanta.Cosmos.DataLayerEvents.EfCode
{
    public class SqlEventsDbContext : DbContextWithEvents<SqlEventsDbContext>
    {
        public SqlEventsDbContext(DbContextOptions<SqlEventsDbContext> options, IEventsRunner eventsRunner)      
            : base(options, eventsRunner)
        {
        }

        public DbSet<TodoWithEvents> Todos { get; set; }
        public DbSet<OwnerWithEvents> Owners { get; set; }
        public DbSet<TodoOwnerWithEvents> TodoOwners { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new TodoWithEventsConfig());       
            modelBuilder.ApplyConfiguration(new TodoOwnerWithEventsConfig());
            modelBuilder.ApplyConfiguration(new OwnerWithEventsConfig());

            modelBuilder.Entity<CommentWithEvents>().ToTable("Comment");
        }
    }
}

