using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Deanta.Models.Contexts
{
    public interface IDeantaContext
    {
        DbSet<Models.TodoModel> Todos { get; set; }

        DbSet<Models.User> Users { get; set; }

        DbSet<Models.HistoryEntry> HistoryEntries { get; set; }

        DatabaseFacade Database { get; }

        int SaveChanges();

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken());
    }
}
