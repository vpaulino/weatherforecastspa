using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Abstractions;
using Application.Abstractions.Apis;
using Application.Frontend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace Application.Frontend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LocationsController : ControllerBase
    {
        private readonly ILogger<WeatherForecastController> _logger;
        private readonly ILocationsRepository locationsRepository;
        private readonly ILocationsRepository locations;
        
        public LocationsController(ILogger<WeatherForecastController> logger, ILocationsRepository locationsRepository)
        {   
            _logger = logger;
            this.locationsRepository = locationsRepository;
        }


        [HttpGet]
        public async Task<IActionResult> Get()
        { 
           var locations = await this.locationsRepository.GetWellKnownLocations();
           
            return Ok(locations);
        }
    }
}
