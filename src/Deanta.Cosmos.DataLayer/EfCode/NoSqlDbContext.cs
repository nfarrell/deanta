
using Deanta.Cosmos.DataLayer.EfClassesNoSql;
using Microsoft.EntityFrameworkCore;

namespace Deanta.Cosmos.DataLayer.EfCode
{
    public class NoDeantaSqlDbContext : DbContext
    {
        public DbSet<TodoListNoSql> Todos { get; set; }

        public NoDeantaSqlDbContext(DbContextOptions<NoDeantaSqlDbContext> options)
            : base(options) { }
    }
}