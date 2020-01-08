
using Deanta.Cosmos.DataLayer.EfCode;
using Deanta.Cosmos.ServiceLayer.DatabaseServices.Concrete;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Deanta.Cosmos.Controllers
{
    public class GenerateController : BaseDeantaController
    {
        // GET
        public IActionResult Index([FromServices]DeantaSqlDbContext context)
        {
            return View(context.Todos.Count());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Todos(int totalTodosNeeded, bool wipeDatabase, CancellationToken cancellationToken,
            [FromServices]DeantaSqlDbContext sqlContext,
            [FromServices]NoDeantaSqlDbContext noSqlContext,
            [FromServices]TodoGenerator generator,
            [FromServices]IWebHostEnvironment env)
        {
            if (totalTodosNeeded == 0)
                return View((object) "Error: should contain the number of todos to generate.");

            if (wipeDatabase)
                sqlContext.DevelopmentWipeCreated(noSqlContext);

            var filepath = Path.Combine(env.WebRootPath, SetupHelpers.SeedFileSubDirectory,
                SetupHelpers.TemplateFileName);
            await generator.WriteTodosAsync(filepath, totalTodosNeeded, true, cancellationToken);

            SetupTraceInfo();

            return
                View((object) ((cancellationToken.IsCancellationRequested ? "Cancelled" : "Successful") +
                     $" generate. Num todos in database = {sqlContext.Todos.Count()}."));
        }

        [HttpPost]
        public ActionResult NumTodos([FromServices]DeantaSqlDbContext context)
        {
            var dbExists = (context.Database.GetService<IDatabaseCreator>() as RelationalDatabaseCreator).Exists();
            var message = dbExists ? $"Num todos = {context.Todos.Count()}" : "database being wiped.";
            return Content(message);
        }
    }
}