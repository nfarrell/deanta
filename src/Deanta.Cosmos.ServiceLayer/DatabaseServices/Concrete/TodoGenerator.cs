
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Deanta.Cosmos.DataLayer.EfClassesSql;
using Deanta.Cosmos.DataLayer.EfCode;
using Deanta.Cosmos.DataLayer.NoSqlCode;
using Deanta.Cosmos.ServiceLayer.DatabaseServices.Concrete.Internal;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Deanta.Cosmos.ServiceLayer.DatabaseServices.Concrete
{
    public class TodoGenerator
    {
        private readonly DbContextOptions<DeantaSqlDbContext> _sqlOptions;
        private readonly DbContextOptions<NoDeantaSqlDbContext> _noSqlOptions;
        private ImmutableList<TodoData> _loadedTodoData;
        private int NumTodosInSet => _loadedTodoData.Count;

        public TodoGenerator(DbContextOptions<DeantaSqlDbContext> sqlOptions, DbContextOptions<NoDeantaSqlDbContext> noSqlOptions)
        {
            _sqlOptions = sqlOptions;
            _noSqlOptions = noSqlOptions;
        }

        public async Task WriteTodosAsync(string filePath, int totalTodosNeeded, bool makeTodoTitlesDistinct, CancellationToken cancellationToken)
        {
            _loadedTodoData = JsonConvert.DeserializeObject<List<TodoData>>(File.ReadAllText(filePath))
                .ToImmutableList();
            
            int numTodosInDb;

            await using (var context = new DeantaSqlDbContext(_sqlOptions))
            {
                numTodosInDb = await context.Todos.IgnoreQueryFilters().CountAsync(cancellationToken: cancellationToken);
            }

            var numWritten = 0;
            var numToWrite = totalTodosNeeded - numTodosInDb;
            while (numWritten < numToWrite)
            {
                if (cancellationToken.IsCancellationRequested)
                    return;

                //noSql can be null. If so then it doesn't write to CosmosDb
                var noSqlTodoUpdater = _noSqlOptions != null
                    ? new NoSqlTodoUpdater(new NoDeantaSqlDbContext(_noSqlOptions))
                    : null;
                await using var sqlDbContext = new DeantaSqlDbContext(_sqlOptions, noSqlTodoUpdater);
                var ownersFinder = new OwnerFinder(sqlDbContext);
                var batchToAdd = Math.Min(_loadedTodoData.Count, numToWrite - numWritten);
                var batch = GenerateTodos(batchToAdd, numTodosInDb, makeTodoTitlesDistinct, ownersFinder).ToList();
                sqlDbContext.AddRange(batch);
                await sqlDbContext.SaveChangesAsync();
                numWritten += batch.Count;
                numTodosInDb += batch.Count;
            }
        }

        private IEnumerable<Todo> GenerateTodos(int batchToAdd, int numTodosInDb, bool makeTodoTitlesDistinct, OwnerFinder ownersFinder)
        {
            for (var i = numTodosInDb; i < numTodosInDb + batchToAdd; i++)
            {
                var sectionNum = (int)Math.Truncate(i / (double)NumTodosInSet);

                var owners = ownersFinder.GetOwnersOfThisTodo(_loadedTodoData[i % _loadedTodoData.Count].Owners).ToList();
                var title = _loadedTodoData[i % _loadedTodoData.Count].Title;
                if (sectionNum > 0 && makeTodoTitlesDistinct)
                    title += $" (copy {sectionNum})";
                var todo = Todo.CreateTodo(title, owners).Result;
                
                yield return todo;
            }
        }

        public class TodoData
        {
            public DateTime CreatedAt { get; set; }
            public string Title { get; set; }
            public string Owners { get; set; }
        }
    }
}