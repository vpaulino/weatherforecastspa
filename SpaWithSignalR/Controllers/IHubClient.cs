using Application.Abstractions;
using System.Threading.Tasks;

namespace Application.Frontend.Controllers
{
    public interface IHubClient 
    {
        public Task BroadcastWeatherForecast(WeatherForecast forecast);

        public Task BroadcastRecomendations(WeatherRecomendation recomendation);

    }
}
