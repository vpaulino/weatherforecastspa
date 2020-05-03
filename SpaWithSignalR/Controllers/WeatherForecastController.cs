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
    public class WeatherForecastController : ControllerBase
    {
        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IHubContext<BroadcastHub, IHubClient> hubContext;
        private readonly IForecastReadService temperaturesReadServices;
        private readonly IForecastWriteService forecastWriteService;
        public WeatherForecastController(ILogger<WeatherForecastController> logger, IForecastWriteService forecastWriteService, IForecastReadService temperaturesReadServices,  IHubContext<BroadcastHub, IHubClient> hubContext)
        {
            this.hubContext = hubContext;
            _logger = logger;
            this.temperaturesReadServices = temperaturesReadServices;
            this.forecastWriteService = forecastWriteService;
        }

        [HttpPut]
        public async Task<IActionResult> Put([FromBody]WeatherForecast forecast) 
        {
            await forecastWriteService.WriteTemperature(forecast);
            await hubContext.Clients.All.BroadcastWeatherForecast(forecast);
            return Accepted(forecast);
        }



        [HttpGet("location")]
        public async Task<IActionResult> Get([FromQuery]Location location)
        {
            WeatherForecast forecast = await this.temperaturesReadServices.GetLocationTemperature(location);

            if(forecast == null)
                return NotFound();

            return Ok(forecast);
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        { 
            var forecasts = await this.temperaturesReadServices.GetCurrentTemperatures();
            ListResults results = new ListResults();
            results.Date = DateTime.UtcNow;
            results.Forecasts = forecasts;
            return Ok(results);
        }

        [HttpGet("skies")]
        public IActionResult GetWellKnownSkies()
        {
            return Ok(new string[] { "Clear", "White Clouds", "Light Gray Clouds", "Dense Gray Clouds" });
        }
    }
}
