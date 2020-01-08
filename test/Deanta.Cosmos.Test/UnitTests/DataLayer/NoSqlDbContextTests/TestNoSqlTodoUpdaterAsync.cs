
using System.Linq;
using System.Threading.Tasks;
using Deanta.Cosmos.DataLayer.EfCode;
using Deanta.Cosmos.DataLayer.NoSqlCode;
using Deanta.Cosmos.Test.Helpers;
using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;
using TestSupport.Attributes;
using TestSupport.EfHelpers;
using TestSupport.Helpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Deanta.Cosmos.Test.UnitTests.DataLayer.NoDeantaSqlDbContextTests
{
    public class TestNoSqlTodoUpdaterAsync
    {
        private DbContextOptions<DeantaSqlDbContext> _sqlOptions;
        public TestNoSqlTodoUpdaterAsync()
        {

            _sqlOptions = this.CreateUniqueClassOptions<DeantaSqlDbContext>();
            using var context = new DeantaSqlDbContext(_sqlOptions);
            context.Database.EnsureCreated();
            context.WipeAllDataFromDatabase();
        }

        [RunnableInDebugOnly]
        public async Task DeleteNoSqlDatabase()
        {
            var config = AppSettings.GetConfiguration();
            var builder = new DbContextOptionsBuilder<NoDeantaSqlDbContext>()
                .UseCosmos(
                    config["endpoint"],
                    config["authKey"],
                    GetType().Name);
            await using var context = new NoDeantaSqlDbContext(builder.Options);
            await context.Database.EnsureDeletedAsync();
        }

        [Fact]
        public async Task TestNoSqlTodoUpdaterFail_NoTodoAddedToSqlDatabase()
        {
            
            var config = AppSettings.GetConfiguration();
            var builder = new DbContextOptionsBuilder<NoDeantaSqlDbContext>()
                .UseCosmos(
                    config["endpoint"],
                    config["authKey"],
                    "UNKNOWNDATASBASENAME");
            await using var sqlContext = new DeantaSqlDbContext(_sqlOptions);
            await using var noSqlContext = new NoDeantaSqlDbContext(builder.Options);
            await sqlContext.Database.EnsureCreatedAsync();
            var updater = new NoSqlTodoUpdater(noSqlContext);

            
            var todo = DddEfTestData.CreateDummyTodoOneOwner();
            sqlContext.Add(todo);
            var numTodosChanged = updater.FindNumTodosChanged(sqlContext);
            var ex = await Assert.ThrowsAsync<CosmosException>(async () =>
                await updater.CallBaseSaveChangesWithNoSqlWriteInTransactionAsync(sqlContext, numTodosChanged, 
                    () => sqlContext.SaveChangesAsync()));

            
            ex.Message.ShouldStartWith("Response status code does not indicate success: 404 Substatus:");
            numTodosChanged.ShouldEqual(1);
            sqlContext.Todos.Count().ShouldEqual(0);
        }

        [Fact]
        public async Task TestNoSqlTodoUpdaterOk()
        {
            
            var config = AppSettings.GetConfiguration();
            var builder = new DbContextOptionsBuilder<NoDeantaSqlDbContext>()
                .UseCosmos(
                    config["endpoint"],
                    config["authKey"],
                    GetType().Name);
            await using var sqlContext = new DeantaSqlDbContext(_sqlOptions);
            await using var noSqlContext = new NoDeantaSqlDbContext(builder.Options);
            
            await sqlContext.Database.EnsureCreatedAsync();
            await noSqlContext.Database.EnsureCreatedAsync();
            var updater = new NoSqlTodoUpdater(noSqlContext);

            
            var todo = DddEfTestData.CreateDummyTodoOneOwner();
            sqlContext.Add(todo);
            var numTodosChanged = updater.FindNumTodosChanged(sqlContext);
            await updater.CallBaseSaveChangesWithNoSqlWriteInTransactionAsync(sqlContext, numTodosChanged,
                () => sqlContext.SaveChangesAsync());

            
            numTodosChanged.ShouldEqual(1);
            sqlContext.Todos.Count().ShouldEqual(1);
            noSqlContext.Todos.Find(todo.TodoId).ShouldNotBeNull();
        }

        [Fact]
        public async Task TestNoSqlTodoUpdaterWithRetryStrategyOk()
        {
            
            var config = AppSettings.GetConfiguration();
            var builder = new DbContextOptionsBuilder<NoDeantaSqlDbContext>()
                .UseCosmos(
                    config["endpoint"],
                    config["authKey"],
                    GetType().Name);
            var connection = this.GetUniqueDatabaseConnectionString();
            var optionsBuilder = new DbContextOptionsBuilder<DeantaSqlDbContext>();
            optionsBuilder.UseSqlServer(connection);
            var options = optionsBuilder.Options;
            await using var sqlContext = new DeantaSqlDbContext(_sqlOptions);
            await using var noSqlContext = new NoDeantaSqlDbContext(builder.Options);
            sqlContext.CreateEmptyViaWipe();
            await noSqlContext.Database.EnsureCreatedAsync();
            var updater = new NoSqlTodoUpdater(noSqlContext);

            
            var todo = DddEfTestData.CreateDummyTodoOneOwner();
            sqlContext.Add(todo);
            var numTodosChanged = updater.FindNumTodosChanged(sqlContext);
            await updater.CallBaseSaveChangesWithNoSqlWriteInTransactionAsync(sqlContext, numTodosChanged,
                () => sqlContext.SaveChangesAsync());

            
            numTodosChanged.ShouldEqual(1);
            sqlContext.Todos.Count().ShouldEqual(1);
            noSqlContext.Todos.Find(todo.TodoId).ShouldNotBeNull();
        }
    }
}