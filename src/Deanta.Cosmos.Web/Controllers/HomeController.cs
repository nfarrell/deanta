
using Microsoft.AspNetCore.Mvc;

namespace Deanta.Cosmos.Controllers
{
    public class HomeController : BaseDeantaController
    {
        public IActionResult Index()
        {
            return View();              
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
