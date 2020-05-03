using Application.Abstractions;
using System.Threading.Tasks;

namespace Application.Abstractions.Apis
{
    public interface IForecastWriteService
    {
        Task WriteTemperature(WeatherForecast forecast);
    }
}