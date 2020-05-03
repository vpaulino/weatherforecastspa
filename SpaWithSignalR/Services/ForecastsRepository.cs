using Application.Abstractions;
using Application.Abstractions.Apis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Frontend.Services
{
    public class ForecastsRepository : IForecastsRepository
    {
        private ICollection<WeatherForecast> forecasts = new List<WeatherForecast>();
 
        public void AddOrUpdate(WeatherForecast forecastToWrite)
        {
            WeatherForecast forecastStored = forecasts.FirstOrDefault((forecast) => forecast.Location.Equals(forecastToWrite.Location));
            if (forecastStored == null)
            {
                forecasts.Add(forecastToWrite);
                forecastStored = forecastToWrite;
            }

            forecastStored.TemperatureC = forecastToWrite.TemperatureC;
            forecastStored.Sky = forecastToWrite.Sky;
        }

        public Task<IEnumerable<WeatherForecast>> GetAll()
        {
            return Task.FromResult<IEnumerable<WeatherForecast>>(forecasts);
        }

        public Task<IEnumerable<WeatherForecast>> GetBy(Func<WeatherForecast, bool> predicate)
        {
            if (this.forecasts == null || (this.forecasts != null && this.forecasts.Count == 0))
                this.forecasts = new List<WeatherForecast>();

            return Task.FromResult(this.forecasts.Where(predicate).AsEnumerable());
        }
    }
}
