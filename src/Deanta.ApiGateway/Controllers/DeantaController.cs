using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Deanta.CommandAPI.Client;
using Deanta.QueryAPI.Client;

namespace Deanta.ApiGateway.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class DeantaController : ControllerBase
    {
        private IDeantaQueryApiClientService _deantaQueryApiClientService;
        private IDeantaCommandApiClientService _deantaCommandApiClientService;
        private readonly IMapper _mapper;
        public DeantaController(IDeantaQueryApiClientService deantaQueryApiClientService,
            IDeantaCommandApiClientService deantaCommandApiClientService, IMapper mapper)
        {
            _deantaQueryApiClientService = deantaQueryApiClientService;
            _deantaCommandApiClientService = deantaCommandApiClientService;
            _mapper = mapper;
        }
    }
}
