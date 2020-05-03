using Application.Abstractions;
using Application.Abstractions.Apis;
using Messaging.Abstractions;
using Messaging.Azure.Storage.Queues;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Frontend.Services
{
    public class ForecastService : IForecastWriteService, IForecastReadService
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };
         

        private readonly IPublisher<string> publisher;
        private readonly ILocationsRepository locationsRepository;
        private readonly IForecastsRepository forecastsRepository;
        
        public ForecastService(ILocationsRepository locationsRepository, IForecastsRepository forecastsRepository, IPublisher<string> publisher)
        {
            this.publisher = publisher;
            this.locationsRepository = locationsRepository;
            this.forecastsRepository = forecastsRepository;

        }

        public virtual async Task Startup()
        {
            var locations = await locationsRepository.GetWellKnownLocations();

            var rng = new Random();

            foreach (var location in locations)
            {
                forecastsRepository.AddOrUpdate(new WeatherForecast
                {
                    Date = DateTime.Now,
                    TemperatureC = rng.Next(-20, 55),
                    Location = location,
                    Summary = Summaries[rng.Next(Summaries.Length)]
                });
            }
        }

        public Task<IEnumerable<WeatherForecast>> GetCurrentTemperatures()
        {
            return this.forecastsRepository.GetAll();
        }

        public async Task<WeatherForecast> GetLocationTemperature(Location location)
        {
            var results = await this.forecastsRepository.GetBy((weatherForecast) => weatherForecast.Location.Equals(location));
            return results.FirstOrDefault();
        }


        public async  Task WriteTemperature(WeatherForecast forecastToWrite) 
        {
            var serialized = JsonConvert.SerializeObject(forecastToWrite);
            
            this.forecastsRepository.AddOrUpdate(forecastToWrite);

            var result = await publisher.PublishAsync(serialized, null, new MessageOptions("ForecastApp", 1), CancellationToken.None);
        }

       
    }
}
