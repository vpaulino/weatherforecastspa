import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { WeatherForecastResults } from '../viewModels/weatherResultsViewModel'
import { WeatherForecast } from '../viewModels/weatherForecast'
import { ComponentEventsBus } from '../services/componentEventsBus.service';
import { ComponentEvent, ComponentState } from '../viewModels/componentEvent';
import { SignalRApplicationClient } from '../services/signalrApplicationClient.service';
import { Location } from '../viewModels/location';
import { NavigationExtras, ActivatedRoute, Router } from '@angular/router';
import { WeatherForecastHttpClient } from '../services/weatherforecast.httpclient';

@Component({
  selector: 'app-fetch-data',
  templateUrl: './fetch-data.component.html',
  styles: [`
    .pulsing {
        animation-iteration-count: 1;
        animation: pulse 1.5s;
        animation-direction: alternate;
    }
    @keyframes pulse {
    0% {
        opacity: 1;
        color: black
    }
    50% {
        opacity: 0.3;
        color: orange;
    }
    100% {
        opacity: 1;
        color: black;
    }
}
  `]
})
export class FetchDataComponent {
  public forecastsViewModel: WeatherForecastResults;
  private componentEventsBus: ComponentEventsBus;
  private signalRClient: SignalRApplicationClient;
  private router: Router;

  public pulsingText = {
    pulsing: false
  }
    weatherForecastHttpClient: WeatherForecastHttpClient;

  ngOnChanges(changes) {
    this.pulsingText.pulsing = true;
  }


  ngOnDestroy(): void {
    this.componentEventsBus.publishEvent(new ComponentEvent("componentDestroy", ComponentState.idle, "leaving component", {}));
  }


  ngOnInit(): void {
    this.componentEventsBus.publishEvent(new ComponentEvent("componentInit", ComponentState.idle, "list data init", {}));

    this.signalRClient.Init();
    
    var forecastsPromise = this.weatherForecastHttpClient.getAllForecasts();

    forecastsPromise.then((forecastsReturned) =>
    {
      this.componentEventsBus.publishEvent(new ComponentEvent("DataReceived", ComponentState.success, "Temperatures Received", {}));

      this.forecastsViewModel = forecastsReturned;
  
    }).catch(reason =>
    {
      this.componentEventsBus.publishEvent(new ComponentEvent("DataReceived", ComponentState.error, `Getting temperatures thrown error ${reason}` , {}));
  
    }).finally(() =>
    {
      this.signalRClient.weatherForecastReceived.subscribe((weatherForecastReceived) => this.databind(weatherForecastReceived));

    });
  }

  private databind(forecastEvent: WeatherForecast) {
    let found: boolean;
    found = false;
    this.forecastsViewModel.forecasts.forEach((forecast) => {
      if (forecast.location.city == forecastEvent.location.city) {
        forecast.temperatureC = forecastEvent.temperatureC;
        forecast.temperatureF = forecastEvent.temperatureF;
        forecast.summary = forecastEvent.summary == null ? forecast.summary : forecastEvent.summary;
        forecast.sky = forecastEvent.sky == null ? forecast.sky : forecastEvent.sky;
        found = true;
      }
    });
    if (!found) {
      this.forecastsViewModel.forecasts.push(forecastEvent);
    }

    this.pulsingText.pulsing = true;
  }


  
  constructor(router: Router, componentEventsBus: ComponentEventsBus, signalRClient: SignalRApplicationClient, weatherForecastHttpClient: WeatherForecastHttpClient ) {
    this.weatherForecastHttpClient = weatherForecastHttpClient;
    this.signalRClient = signalRClient;
    this.componentEventsBus = componentEventsBus;
    this.router = router;
  }


  public navigateToEdit(location: Location) {

    let navigationExtras: NavigationExtras = {
      queryParams: { 'country': location.country, 'region': location.region, 'city': location.city },
      fragment: 'anchor'
    };
    // Navigate to the login page with extras
    this.router.navigate(['/insert-data'], navigationExtras);
  }

}

