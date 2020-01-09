using System;
using System.Threading.Tasks;
using Deanta.Models.Contexts;
using Deanta.Models.Contracts;
using Deanta.Models.Models;

using User = Deanta.Models.Models.User;

namespace Deanta.CommandAPI.Services
{
    public class DeantaRepositoryService : IDeantaRepositoryService
    {
        private readonly IDeantaContext _deantaContext;

        private readonly IHistoryEntryGenerator _historyEntryGenerator;

        public DeantaRepositoryService(IDeantaContext deantaContext, IHistoryEntryGenerator historyEntryGenerator)
        {
            _deantaContext = deantaContext;
            _historyEntryGenerator = historyEntryGenerator;
        }

        public async Task CreateTemplate(TodoDto newDeantaModel, Guid userId)
        {
            using (var transaction = _deantaContext.Database.BeginTransaction())
            {
                try
                {
                    var newTemplate = new TodoModel();

                    await _deantaContext.Todos.AddAsync(newTemplate);

                    var user = new User() { Id = userId } ;

                    var historyEntries = _historyEntryGenerator.GetHistoryEntriesForCreation(
                        "template created", user,
                        newTemplate);

                    await _deantaContext.HistoryEntries.AddRangeAsync(historyEntries);
                    _deantaContext.SaveChanges();
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    //todo: log error
                    transaction.Rollback();
                    Console.WriteLine("Error occurred.");
                }
            }
        }

    }
}
