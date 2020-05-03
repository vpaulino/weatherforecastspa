using Application.Abstractions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Abstractions.Apis
{
    public interface IForecastsRepository
    {
        Task<IEnumerable<WeatherForecast>> GetAll();
        void AddOrUpdate(WeatherForecast forecastToWrite);
        Task<IEnumerable<WeatherForecast>> GetBy(Func<WeatherForecast, bool> predicate);
    }
}