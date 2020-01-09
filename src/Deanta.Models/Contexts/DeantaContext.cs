using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Deanta.Models.Contexts
{
    public class DeantaContext : DbContext, IDeantaContext
    {
        private readonly string _connectionString;

        public DeantaContext(DbContextOptions<DeantaContext> options) : base(options)
        {
        }

        public DeantaContext() : base()
        {
        }

        public DbSet<Models.TodoModel> Todos { get; set; }

        public DbSet<Models.User> Users { get; set; }

        public DbSet<Models.HistoryEntry> HistoryEntries { get; set; }

        public override DatabaseFacade Database => base.Database; //add get{} audit logic

        /// <summary>
        /// NF - this method is needed because I define the signature in the interface.... optional to append Audit info like I do in other branches.
        /// </summary>
        /// <returns></returns>
        public override int SaveChanges()
        {
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            return base.SaveChangesAsync(cancellationToken);
        }

        /*
         //removed this because I also add ability to create a noop context / fake db
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {   optionsBuilder.UseSqlServer(connectionString);
            }
        }
        */

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Models.HistoryEntry>().HasOne(h => h.User);

            modelBuilder.Entity<Models.HistoryEntry>().HasMany(r => r.Attributes);
            
            base.OnModelCreating(modelBuilder);
        }
    }
}
