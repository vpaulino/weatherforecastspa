using System;
using System.Threading.Tasks;
using Application.Abstractions;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace Application.Azure.TemperatureFunction
{
    public class WeatherRecomendations
    {

        [FunctionName("WeatherRecomendations")]
        [return: ServiceBus("%AppEvents-Topic%", Connection = "ServiceBusConnection")]
        public async Task<WeatherRecomendation> Execute([QueueTrigger("%AppEventsQueue%")]WeatherForecast forecast, ILogger log)
        {
            log.LogInformation($"C# Queue trigger function processed: {forecast.ToString()}");
            return await Task.FromResult(new WeatherRecomendation() { Actions = new string[] { "stay at home" }, Date = forecast.Date, DressCode = new string[] { "Light Jacket", "closed shoes", "scarf", "jeans" }, Location = forecast.Location });
        }
    }
}
