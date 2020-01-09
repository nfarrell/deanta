using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Deanta.CommandAPI.Services;
using Deanta.Models.Contracts;

namespace Deanta.CommandAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeantaController : ControllerBase
    {
        private readonly IDeantaRepositoryService _deantaRepositoryService;

        public DeantaController(IDeantaRepositoryService deantaRepositoryService)
        {
            _deantaRepositoryService = deantaRepositoryService;
        }

        [HttpPost]
        [Route("templates")]
        public async Task<ActionResult> CreateTemplate(
            [FromBody] TodoDto newDeanta,
            [FromHeader] Guid userId)
        {
            await _deantaRepositoryService.CreateTemplate(newDeanta, userId);
            return Ok();
        }
    }
}
