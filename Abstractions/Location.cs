using System.Collections.Generic;

namespace Application.Abstractions
{
    public class Location
    {
        public Location()
        {

        }
        public Location(string country, string region, string city)
        {
            this.Country = country;
            this.Region = region;
            this.City = city;
        }

        public string Region { get; set; }
        public string City { get; set; }
        public string Country { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (!(obj is Location))
                return false;
            
            var castedObj = obj as Location;

            return castedObj.Country.Equals(this.Country) && castedObj.City.Equals(this.City) && castedObj.Region.Equals(this.Region);
        }

        public override int GetHashCode()
        {
            var hashCode = 1195953936;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Region);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(City);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Country);
            return hashCode;
        }
    }
}