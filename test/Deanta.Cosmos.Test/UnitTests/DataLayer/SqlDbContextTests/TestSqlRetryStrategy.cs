
using Deanta.Cosmos.DataLayer.EfCode;
using Microsoft.EntityFrameworkCore;
using TestSupport.Helpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Deanta.Cosmos.Test.UnitTests.DataLayer.DeantaSqlDbContextTests
{
    public class TestSqlRetryStrategy
    {
        [Fact]
        public void TestHasRetryEnabledOk()
        {
            
            var connection = this.GetUniqueDatabaseConnectionString();
            var optionsBuilder = new DbContextOptionsBuilder<DeantaSqlDbContext>();
            optionsBuilder.UseSqlServer(connection,
                option => option.EnableRetryOnFailure());
            var options = optionsBuilder.Options;
            using var context = new DeantaSqlDbContext(options);
            
            var strategy = context.Database.CreateExecutionStrategy();

            
            strategy.RetriesOnFailure.ShouldBeTrue();
        }

        [Fact]
        public void TestNoRetryEnabledOk()
        {
            
            var connection = this.GetUniqueDatabaseConnectionString();
            var optionsBuilder = new DbContextOptionsBuilder<DeantaSqlDbContext>();
            optionsBuilder.UseSqlServer(connection);
            var options = optionsBuilder.Options;
            using var context = new DeantaSqlDbContext(options);
            
            var strategy = context.Database.CreateExecutionStrategy();

            
            strategy.RetriesOnFailure.ShouldBeFalse();
        }
    }
}