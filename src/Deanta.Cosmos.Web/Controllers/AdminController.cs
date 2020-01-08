using Deanta.Cosmos.DataLayer.EfCode;
using Deanta.Cosmos.ServiceLayer.DatabaseServices.Concrete;
using Deanta.Cosmos.ServiceLayer.TodosSqlWithEvents;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace Deanta.Cosmos.Controllers
{
    public class AdminController : BaseDeantaController
    {
        //------------------------------------------------
        //Admin commands that are called from the top menu

        public IActionResult ResetCacheValues([FromServices]IHardResetCacheService service)
        {
            var status = service.CheckUpdateTodoCacheProperties();
            return View("Message", status.Message);
        }

        public IActionResult ResetDatabase(
            [FromServices]DeantaSqlDbContext context,
            [FromServices]NoDeantaSqlDbContext noDeantaSqlDbContext,
            [FromServices]IWebHostEnvironment env)
        {
            context.DevelopmentWipeCreated(noDeantaSqlDbContext);
            var numTodos = context.SeedDatabase(env.WebRootPath);
            SetupTraceInfo();
            return View("Message", $"Successfully reset the database and added {numTodos} todos.");
        }
    }
}
