import { Location } from '../viewModels/location';
import { WeatherForecast } from '../viewModels/weatherForecast';


export class EditForecastViewModel {

  public allLocations: Location[];
  public cities: Set<string>;
  public regions: Set<string>;
  public countries: Set<string>;
  public skies: string[];
  public selectedCountry: string;
  public selectedRegion: string;
  public selectedCity: string;
  public selectedSky: string;
  public selectedTemperature: number;
  public isEdit: boolean;

  constructor() {
    this.isEdit = false;
  }

  private dataBindSelectedLocation(location: Location) {
    this.selectedCity = location.city;
    this.selectedCountry = location.country;
    this.selectedRegion = location.region;
  }

  public dataBindLocations(locations: Location[]) {
    this.allLocations = locations;
    this.cities = new Set(this.allLocations.map((loc) => loc.city));
    this.regions = new Set(this.allLocations.map((loc) => loc.region));
    this.countries = new Set(this.allLocations.map((loc) => loc.country));
    this.dataBindSelectedLocation(locations[0]);
  }

  public EqualsTo(location: Location): boolean {

    if ((location.city == undefined || location.city == null) &&
      (location.country == undefined || location.country == null) &&
      (location.region == undefined || location.region == null)
    ) {
      return false;
    }

    if (this.selectedRegion == null || this.selectedCountry == null || this.selectedCity == null)
      return false;

    return this.selectedCity == location.city && this.selectedCountry == location.country && this.selectedRegion == location.region;
  }



  public dataBind(forecast: WeatherForecast) {

    if (forecast.location == null)
    {
      forecast.location = new Location();
      forecast.location.city = this.cities[0];
      forecast.location.region = this.regions[0];
      forecast.location.country = this.countries[0];
    }

    this.selectedCity = forecast.location.city;
    this.selectedCountry = forecast.location.country;
    this.selectedRegion = forecast.location.region;
    this.selectedTemperature = forecast.temperatureC == null ? 0 : forecast.temperatureC;
    this.selectedSky = forecast.sky == null ? this.skies[0] : forecast.sky;
  }

  public dataBindSkies(skies: string[]) {
    this.skies = skies;
  }

  public getFormData(): WeatherForecast {
    let location = new Location();
    location.country = this.selectedCountry;
    location.region = this.selectedRegion;
    location.city = this.selectedCity;
    let today = new Date();
    let weatherForecast = new WeatherForecast();
    weatherForecast.date = today;
    weatherForecast.location = location;
    weatherForecast.temperatureC = this.selectedTemperature;
    weatherForecast.sky = this.selectedSky;
    return weatherForecast;
  }
}
