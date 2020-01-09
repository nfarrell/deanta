using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Deanta.Models.Contracts;
using Deanta.QueryAPI.Services;

namespace Deanta.QueryAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class DeantaController : ControllerBase
    {
        private readonly IDeantaRepositoryService _deantaRepositoryService;

        public DeantaController(IDeantaRepositoryService deantaRepositoryService)
        {
            _deantaRepositoryService = deantaRepositoryService;
        }

        [HttpGet]
        [Route("templates/{id}")]
        public async Task<ActionResult<TodoDto>> GetTodo(Guid id)
        {
            var result = _deantaRepositoryService.GetTodo(id);
            return Ok(result);
        }

    }
}
