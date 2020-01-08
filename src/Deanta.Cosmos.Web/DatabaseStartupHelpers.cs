using System.IO;
using Deanta.Cosmos.DataLayer.EfCode;
using Deanta.Cosmos.ServiceLayer.DatabaseServices.Concrete;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Deanta.Cosmos
{
    public static class DatabaseStartupHelpers
    {
        public static void SetupDevelopmentDatabase(this IHost iHost, string currDirectory, bool seedDatabase = true)
        {
            using var scope = iHost.Services.CreateScope();
            var services = scope.ServiceProvider;
            using var sqlContext = services.GetRequiredService<DeantaSqlDbContext>();
            using var noSqlContext = services.GetService<NoDeantaSqlDbContext>();

            var wwwRootPath = Path.Combine(currDirectory, "wwwroot\\");
            sqlContext.DevelopmentEnsureCreated(noSqlContext);
            if (seedDatabase)
                sqlContext.SeedDatabase(wwwRootPath);
        }
    }
}