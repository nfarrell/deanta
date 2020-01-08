
using System.Linq;
using System.Threading.Tasks;
using Deanta.Cosmos.DataLayer.EfCode;
using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TestSupport.Attributes;
using TestSupport.EfHelpers;
using TestSupport.Helpers;
using Xunit.Abstractions;

namespace Deanta.Cosmos.Test.UnitTests.DataLayer.NoDeantaSqlDbContextTests
{
    public class TestNoSqlUsingDirestSql
    {
        private readonly ITestOutputHelper _output;

        public TestNoSqlUsingDirestSql(ITestOutputHelper output)
        {
            _output = output;
        }

        [RunnableInDebugOnly]
        public async Task TestGetCosmosClientOnMainDatabasesOk()
        {
            
            var mainConfig = AppSettings.GetConfiguration("../Deanta.Cosmos/");
            var noSQlDatabaseName = mainConfig["database"];
            var noSqlOptions = noSQlDatabaseName.GetCosmosDbToEmulatorOptions<NoDeantaSqlDbContext>();
            await using (var noDeantaSqlDbContext = new NoDeantaSqlDbContext(noSqlOptions))
            {
                var cosmosClient = noDeantaSqlDbContext.Database.GetCosmosClient();
                var database = cosmosClient.GetDatabase(noSQlDatabaseName);
                var container = database.GetContainer(nameof(NoDeantaSqlDbContext));

                
                _output.WriteLine($"SQL count = {noDeantaSqlDbContext.Todos.Select(_ => 1).AsEnumerable().Count()}");
                using (new TimeThings(_output, "NoSQL count, EF Core"))
                {
                    var result = noDeantaSqlDbContext.Todos.Select(_ => 1).AsEnumerable().Count();
                }

                using (new TimeThings(_output, "NoSQL count, EF Core"))
                {
                    var result = noDeantaSqlDbContext.Todos.Select(_ => 1).AsEnumerable().Count();
                }

                using (new TimeThings(_output, "NoSQL count, via client"))
                {
                    var resultSet = container.GetItemQueryIterator<int>(new QueryDefinition("SELECT VALUE Count(c) FROM c"));
                    var result = (await resultSet.ReadNextAsync()).First();
                }

                using (new TimeThings(_output, "NoSQL count, via client"))
                {
                    var resultSet = container.GetItemQueryIterator<int>(new QueryDefinition("SELECT VALUE Count(c) FROM c"));
                    var result = (await resultSet.ReadNextAsync()).First();
                }
            }

            var sqlConnection = mainConfig.GetConnectionString("TodoSqlConnection");
            var builder = new DbContextOptionsBuilder<DeantaSqlDbContext>()
                .UseSqlServer(sqlConnection);
            await using var sqlContext = new DeantaSqlDbContext(builder.Options);
            _output.WriteLine($"SQL count = {sqlContext.Todos.Count()}");
            using (new TimeThings(_output, "SQL count"))
            {
                var num = await sqlContext.Todos.CountAsync();
            }

            using (new TimeThings(_output, "SQL count"))
            {
                var num = await sqlContext.Todos.CountAsync();
            }
        }
    }
}