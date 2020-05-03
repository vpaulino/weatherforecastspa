using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Abstractions
{
    public class WeatherRecomendation
    {
        public DateTime Date { get; set; }

        public Location Location { get; set; }

        public string[] Actions { get; set; }

        public string[] DressCode { get; set; }
    }
}
