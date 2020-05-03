import { Location } from './location';

export class WeatherForecast implements IWeatherForecast {

  constructor()
  {
    this.location = new Location();
  }

  date: Date;
  location: Location
  temperatureC: number;
  temperatureF: number;
  summary: string;
  sky: string;
}

export interface IWeatherForecast {
  date: Date;
  location: Location
  temperatureC: number;
  temperatureF: number;
  summary: string;
  sky: string;
}
