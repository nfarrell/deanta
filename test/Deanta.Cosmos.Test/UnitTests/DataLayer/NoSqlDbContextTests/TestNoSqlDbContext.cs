
using System.Threading.Tasks;
using Deanta.Cosmos.DataLayer.EfCode;
using Deanta.Cosmos.Test.Helpers;
using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TestSupport.Attributes;
using TestSupport.Helpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Deanta.Cosmos.Test.UnitTests.DataLayer.NoDeantaSqlDbContextTests
{
    public class TestNoDeantaSqlDbContext
    {
        [Fact]
        public async Task TestCosmosDbCatchFailedRequestOk()
        {
            
            var config = AppSettings.GetConfiguration();
            var builder = new DbContextOptionsBuilder<NoDeantaSqlDbContext>()
                .UseCosmos(
                    config["endpoint"],
                    config["authKey"],
                    "UNKNOWNDATABASE");

            await using var context = new NoDeantaSqlDbContext(builder.Options);
            
            var todo = NoSqlTestData.CreateDummyNoSqlTodo();
            context.Add(todo);
            var ex = await Assert.ThrowsAsync<CosmosException>(async () => await context.SaveChangesAsync());

            
            ex.Message.ShouldStartWith("Response status code does not indicate success: 404 Substatus:");
        }

        [Fact]
        public async Task TestCosmosDbLocalDbEmulatorCreateDatabaseOk()
        {
            
            var config = AppSettings.GetConfiguration();
            var builder = new DbContextOptionsBuilder<NoDeantaSqlDbContext>()
                .UseCosmos(
                    config["endpoint"],
                    config["authKey"],
                    GetType().Name);

            await using var context = new NoDeantaSqlDbContext(builder.Options);
            await context.Database.EnsureCreatedAsync();

            
            var todo = NoSqlTestData.CreateDummyNoSqlTodo();
            context.Add(todo);
            await context.SaveChangesAsync();

            
            context.Todos.Find(todo.TodoId).ShouldNotBeNull();
        }

        [RunnableInDebugOnly]
        public async Task TestCosmosDbAzureCosmosDbOk()
        {
            
            var config = new ConfigurationBuilder()
                .AddUserSecrets<Startup>()
                .Build();
            var builder = new DbContextOptionsBuilder<NoDeantaSqlDbContext>()
                .UseCosmos(
                    config["CosmosUrl"],
                    config["CosmosKey"],
                    GetType().Name);

            await using var context = new NoDeantaSqlDbContext(builder.Options);
            await context.Database.EnsureCreatedAsync();

            
            var todo = NoSqlTestData.CreateDummyNoSqlTodo();
            context.Add(todo);
            await context.SaveChangesAsync();

            
            context.Todos.Find(todo.TodoId).ShouldNotBeNull();
        }
    }
}