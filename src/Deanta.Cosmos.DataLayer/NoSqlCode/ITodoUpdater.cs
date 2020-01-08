
using System;
using System.Threading.Tasks;
using Deanta.Cosmos.DataLayer.EfCode;
using Microsoft.EntityFrameworkCore;

namespace Deanta.Cosmos.DataLayer.NoSqlCode
{
    public interface ITodoUpdater
    {
        int FindNumTodosChanged(DeantaSqlDbContext sqlContext);

        int CallBaseSaveChangesAndNoSqlWriteInTransaction(DbContext sqlContext, int todoChanges, Func<int> callBaseSaveChanges);

        Task<int> CallBaseSaveChangesWithNoSqlWriteInTransactionAsync(DbContext sqlContext, int todoChanges, Func<Task<int>> callBaseSaveChangesAsync);
    }
}