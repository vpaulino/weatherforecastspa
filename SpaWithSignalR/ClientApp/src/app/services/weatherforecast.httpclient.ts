import { Inject, Injectable, EventEmitter, Output } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { WeatherForecastResults } from '../viewModels/weatherResultsViewModel'
import { Location } from '../viewModels/location'
import { WeatherForecast } from '../viewModels/weatherForecast'


@Injectable()
export class WeatherForecastHttpClient {
    baseUrl: string;
    httpClient: HttpClient;

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    
    this.baseUrl = baseUrl;
    this.httpClient = http;
  }


  public async getAllForecasts(): Promise<WeatherForecastResults> {

    var results = await this.httpClient.get<WeatherForecastResults>(this.baseUrl + 'weatherforecast').toPromise();

    return results;
  }


  public async getAllSkies(): Promise<string[]> {
    var results = await this.httpClient.get<string[]>(this.baseUrl + 'weatherforecast/skies').toPromise();
    return results;
  }

  public async getAllLocations(): Promise<Location[]> {
    var results = await this.httpClient.get<Location[]>(this.baseUrl + 'locations').toPromise();
    return results;
  }



  public async getForecastByLocation(location: Location): Promise<WeatherForecast> {

    var results = await this.httpClient.get<WeatherForecast>(this.baseUrl + `weatherforecast/location?country=${location.country}&region=${location.region}&city=${location.city}`).toPromise();

    return results;
  }

  public async SetForecastByLocation(forecast: WeatherForecast): Promise<WeatherForecast> {

    var promiseResult = this.httpClient.put<WeatherForecast>(this.baseUrl + 'WeatherForecast', forecast).toPromise();
    return promiseResult;
  }

}
