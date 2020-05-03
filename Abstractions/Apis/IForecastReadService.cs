using Application.Abstractions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Abstractions.Apis
{
    public interface IForecastReadService
    {
        Task<IEnumerable<WeatherForecast>> GetCurrentTemperatures();
        Task<WeatherForecast> GetLocationTemperature(Location location);
    }
}