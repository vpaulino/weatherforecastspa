using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Application.Abstractions.Apis
{
    public interface IRecomendationsRepository
    {
        Task<IEnumerable<WeatherRecomendation>> GetAll();
        void AddOrUpdate(WeatherRecomendation forecastToWrite);
    }
}
