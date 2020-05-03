using Application.Abstractions;
using Application.Abstractions.Apis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Frontend.Services
{
    public class LocationsRepository : ILocationsRepository
    {
        public Task<Location[]> GetWellKnownLocations()
        {
            return Task.FromResult<Location[]>(new Location[] {

                new Location("Portugal","Litoral", "Lisbon"),
                new Location("Portugal","Center","Cartaxo"),
                new Location("Portugal","Center" ,"Santarem"),
                new Location("Portugal","Center" ,"Leiria"),
                new Location("Portugal","Center" ,"Vila Franca De Xira"),
                new Location("Portugal", "North" ,"Coimbra"),
                new Location("Portugal", "North" ,"Porto"),
                new Location("Portugal", "North" ,"Guimaraes"),
                new Location("Portugal", "North" ,"Mirandela"),
                new Location("Portugal", "North" ,"Braga"),
                new Location("Portugal", "North" ,"Bragança"),
                new Location("Portugal", "North" ,"Vila Real"),
                new Location("Portugal", "Interior","Castelo Branco"),
                new Location("Portugal", "Interior" ,"Guarda"),
                new Location("Portugal", "Interior" ,"Abrantes"),
                new Location("Portugal", "Interior" ,"Torres Novas"),
                new Location("Portugal", "Alentejo" ,"Evora"),
                new Location("Portugal", "Alentejo" ,"Beja"),
                new Location("Portugal", "Alentejo" ,"Portalegre"),
                new Location("Portugal", "Alentejo" ,"Campo Maior"),
                new Location("Portugal", "South" ,"Faro"),
                new Location("Portugal", "South" ,"Portimao"),
                new Location("Portugal", "South" ,"Albufeira"),
                new Location("Portugal", "South" ,"Vila Real de Santo Antonio"),
            });
        }
    }
}
