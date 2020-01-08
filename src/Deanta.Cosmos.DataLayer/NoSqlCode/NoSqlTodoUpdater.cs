
using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Deanta.Cosmos.DataLayer.EfCode;
using Deanta.Cosmos.DataLayer.NoSqlCode.Internal;
using Microsoft.EntityFrameworkCore;

namespace Deanta.Cosmos.DataLayer.NoSqlCode
{
    /// <summary>
    /// Uses code from https://jimmybogard.com/life-beyond-distributed-transactions-an-apostates-implementation-relational-resources/
    /// --- Understand saga's from https://jimmybogard.com/life-beyond-distributed-transactions-sagas/ 
    /// </summary>
    public class NoSqlTodoUpdater : ITodoUpdater
    {
        private readonly NoDeantaSqlDbContext _noSqlContext;

        private IImmutableList<TodoChangeInfo> _todoChanges;

        public NoSqlTodoUpdater(NoDeantaSqlDbContext noSqlContext)
        {
            _noSqlContext = noSqlContext ?? throw new ArgumentNullException(nameof(noSqlContext));
        }

        /// <summary>
        /// This MUST be called before SavChanges. It finds any Todo changes 
        /// </summary>
        /// <returns>true if there are TodoChanges that need projecting to NoSQL database</returns>
        public int FindNumTodosChanged(DeantaSqlDbContext sqlContext)
        {
            _todoChanges =  TodoChangeInfo.FindTodoChanges(sqlContext.ChangeTracker.Entries().ToList(), sqlContext);
            return _todoChanges.Count;
        }

        /// <summary>
        /// This method will:
        /// 1) start a transaction on the SQL context
        /// 2) Do a SaveChangesAsync on the SQL context
        /// 3) Then project the Todo changes to NoSQL database
        /// 4) ... and call SaveChangesAsync on the NoSQL context
        /// 5) finally commit the transaction
        /// </summary>
        /// <returns></returns>
        public int CallBaseSaveChangesAndNoSqlWriteInTransaction(DbContext sqlContext, int todoChanges, Func<int> callBaseSaveChanges)
        {
            var strategy = sqlContext.Database.CreateExecutionStrategy();
            if (strategy.RetriesOnFailure)
            {
                return strategy.Execute(() => RunSqlTransactionWithNoSqlWrite(sqlContext, todoChanges, callBaseSaveChanges));
            }

            return RunSqlTransactionWithNoSqlWrite(sqlContext, todoChanges, callBaseSaveChanges);
        }

        /// <summary>
        /// This method will:
        /// 1) start a transaction on the SQL context
        /// 2) Do a SaveChangesAsync on the SQL context
        /// 3) Then project the Todo changes to NoSQL database
        /// 4) ... and call SaveChangesAsync on the NoSQL context
        /// 5) finally commit the transaction
        /// </summary>
        /// <returns></returns>
        public async Task<int> CallBaseSaveChangesWithNoSqlWriteInTransactionAsync(DbContext sqlContext, int todoChanges, Func<Task<int>> callBaseSaveChangesAsync)
        {
            var strategy = sqlContext.Database.CreateExecutionStrategy();
            if (strategy.RetriesOnFailure)
            {
                return await strategy.ExecuteAsync(async () => 
                    await RunSqlTransactionWithNoSqlWriteAsync(sqlContext, todoChanges, callBaseSaveChangesAsync));
            }

            return await RunSqlTransactionWithNoSqlWriteAsync(sqlContext, todoChanges, callBaseSaveChangesAsync);
        }

        //--------------------------------------------------------------
        //private methods

        private int RunSqlTransactionWithNoSqlWrite(DbContext sqlContext, int todoChanges, Func<int> callBaseSaveChanges)
        {
            if (sqlContext.Database.CurrentTransaction != null)
                throw new InvalidOperationException("You can't use the NoSqlTodoUpdater if you are using transactions.");

            var applier = new ApplyChangeToNoSql(sqlContext, _noSqlContext);
            using var transaction = sqlContext.Database.BeginTransaction();
            var result = callBaseSaveChanges();            //Save the SQL changes
            applier.UpdateNoSql(_todoChanges);                 //apply the todo changes to the NoSql database
            var numNoSqlChanges = _noSqlContext.SaveChanges(); //And Save to NoSql database
            if (todoChanges != numNoSqlChanges)
                throw new InvalidOperationException($"{todoChanges} todos were changed in SQL, but the NoSQL changed {numNoSqlChanges}");
            transaction.Commit();
            return result;
        }

        private async Task<int> RunSqlTransactionWithNoSqlWriteAsync(DbContext sqlContext, int todoChanges, Func<Task<int>> callBaseSaveChangesAsync)
        {
            if (sqlContext.Database.CurrentTransaction != null)
                throw new InvalidOperationException("You can't use the NoSqlTodoUpdater if you are using transactions.");

            var applier = new ApplyChangeToNoSql(sqlContext, _noSqlContext);
            await using var transaction = sqlContext.Database.BeginTransaction();
            var result = await callBaseSaveChangesAsync();                //Save the SQL changes
            await applier.UpdateNoSqlAsync(_todoChanges);                 //apply the todo changes to the NoSql database
            var numNoSqlChanges = await _noSqlContext.SaveChangesAsync(); //And Save to NoSql database
            if (todoChanges != numNoSqlChanges)
                throw new InvalidOperationException($"{todoChanges} todos were changed in SQL, but the NoSQL changed {numNoSqlChanges}");
            transaction.Commit();
            return result;
        }
    }
}