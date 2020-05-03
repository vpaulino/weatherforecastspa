using Application.Abstractions.Apis;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Frontend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RecomendationsController : ControllerBase
    {
        private readonly ILogger<RecomendationsController> _logger;

        private readonly IRecomendationsRepository recomendationsRepository;

        public RecomendationsController(ILogger<RecomendationsController> logger, IRecomendationsRepository recomendationsRepository)
        {
            _logger = logger;
            this.recomendationsRepository = recomendationsRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var results = await this.recomendationsRepository.GetAll();
            
            return Ok(results);
        }

    }

       
}
