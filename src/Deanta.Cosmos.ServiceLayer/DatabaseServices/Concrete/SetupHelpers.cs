
using System.IO;
using System.Linq;
using Deanta.Cosmos.DataLayer.EfCode;

namespace Deanta.Cosmos.ServiceLayer.DatabaseServices.Concrete
{
    public static class SetupHelpers
    {
        private const string SeedDataSearchName = "Apress todos*.json";
        public const string TemplateFileName = "Manning todos.json";
        public const string SeedFileSubDirectory = "seedData";

        public static void DevelopmentEnsureCreated(this DeantaSqlDbContext sqlDbContext, NoDeantaSqlDbContext noDeantaSqlDbContext)
        {
            sqlDbContext.Database.EnsureCreated();
            noDeantaSqlDbContext?.Database.EnsureCreated();
        }

        public static void DevelopmentWipeCreated(this DeantaSqlDbContext sqlDbContext, NoDeantaSqlDbContext noDeantaSqlDbContext)
        {
            sqlDbContext.Database.EnsureDeleted();
            noDeantaSqlDbContext?.Database.EnsureDeleted();
            sqlDbContext.DevelopmentEnsureCreated(noDeantaSqlDbContext);
        }

        public static int SeedDatabase(this DeantaSqlDbContext context, string wwwrootDirectory)
        {
            var numTodos = context.Todos.Count();
            if (numTodos == 0)
            {
                //the database is empty so we fill it from a json file
                //This also sets up the NoSql database IF the ITodoUpdater member is registered.
                var todos = TodoJsonLoader.LoadTodos(Path.Combine(wwwrootDirectory, SeedFileSubDirectory),
                    SeedDataSearchName).ToList();
                context.Todos.AddRange(todos);
                context.SaveChanges();

                //we add the special todo 
                context.Todos.Add(SpecialTodo.CreateSampleTodo());
                context.SaveChanges();
                numTodos = todos.Count + 1;
            }

            return numTodos;
        }

    }
}