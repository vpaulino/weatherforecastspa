using Application.Abstractions;
using Application.Abstractions.Apis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Frontend.Services
{
    public class RecomendationsRepository : IRecomendationsRepository
    {
        ICollection<WeatherRecomendation> recomendationsStored = new List<WeatherRecomendation>();
        public void AddOrUpdate(WeatherRecomendation recomendationToWrite)
        {
            if (!recomendationsStored.Any((recomendation) => recomendation.Location.Equals(recomendationToWrite.Location)))
            {
                recomendationsStored.Add(recomendationToWrite);
                return;
            }
            var matched = recomendationsStored.FirstOrDefault((elemMatched) => elemMatched.Location.Equals(recomendationToWrite.Location));
            matched.Actions = recomendationToWrite.Actions;
            matched.DressCode = recomendationToWrite.DressCode;
            matched.Date = recomendationToWrite.Date;

        }

        public Task<IEnumerable<WeatherRecomendation>> GetAll()
        {
            return Task.FromResult(recomendationsStored.AsEnumerable());
        }
    }
}
