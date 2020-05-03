using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Abstractions.Apis
{
    public interface ILocationsRepository
    {
         Task<Location[]> GetWellKnownLocations();
    }
}
