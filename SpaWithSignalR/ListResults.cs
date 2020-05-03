using Application.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Frontend
{
    public class ListResults
    {
        public DateTime Date { get; set; }
        public IEnumerable<WeatherForecast> Forecasts { get; set; }
    }
}
