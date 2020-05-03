import * as signalR from '@aspnet/signalr';
import { Inject, Injectable, EventEmitter, Output } from '@angular/core';
import { IWeatherForecast, WeatherForecast } from '../viewModels/weatherForecast';
import {  BehaviorSubject } from 'rxjs';
import { IWeatherConditionRecomendation, WeatherConditionRecomendation } from '../viewModels/recomendationViewModel';
 

@Injectable()
export class SignalRApplicationClient 
{
  private baseUrl: string;

  constructor(@Inject('BASE_URL') baseUrl: string) {
    this.baseUrl = baseUrl;
    
  }

  private isInitialized:boolean = false;

  public weatherForecastReceived = new BehaviorSubject<IWeatherForecast>(new WeatherForecast());
  public recomendationsReceived = new BehaviorSubject<IWeatherConditionRecomendation>(new WeatherConditionRecomendation());

  public Init() {

    if (this.isInitialized)
      return;

    this.isInitialized = true;

    const connection = new signalR.HubConnectionBuilder()
      .configureLogging(signalR.LogLevel.Information)
      .withUrl(`${this.baseUrl}notify`)
      .build();

    connection.start().then(function () {
      console.log('SignalR Connected!');
    }).catch(function (err) {
      return console.error(err.toString());
    });

    connection.on("BroadcastWeatherForecast", (mssg) => {
      this.weatherForecastReceived.next(mssg);
    });

    connection.on("BroadcastRecomendations", (mssg) => {
      console.log(`BroadcastRecomendations - ${mssg}`);
      this.recomendationsReceived.next(mssg);

    });

  }
}
