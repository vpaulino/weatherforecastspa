import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { IWeatherConditionRecomendation } from '../viewModels/recomendationViewModel'
import { SignalRApplicationClient } from '../services/signalrApplicationClient.service';
import { ComponentEventsBus } from '../services/componentEventsBus.service';
import { ComponentEvent, ComponentState } from '../viewModels/componentEvent';
import { Subscription } from 'rxjs';

@Component({
  selector: 'recomendation-data',
  templateUrl: './recomendation-data.component.html',
  
})

export class RecomendationDataComponent {

  public recomendationsViewModel: IWeatherConditionRecomendation[];
  private httpClient: HttpClient;
  private baseUrl: string;
  private signalRClient: SignalRApplicationClient;
  private componentEventsBus: ComponentEventsBus;
  private recomendationsSubscription: Subscription;

  ngOnDestroy(): void {
    this.componentEventsBus.publishEvent(new ComponentEvent('componentDestroy', ComponentState.idle, 'leaving component', {}));
    this.recomendationsSubscription.unsubscribe();
  }


  ngOnInit(): void {
    this.componentEventsBus.publishEvent(new ComponentEvent('componentInit', ComponentState.idle, 'recomendations init', {}));
    this.recomendationsViewModel = [];
    var forecastsPromise = this.getActiveRecomendations();
    forecastsPromise.then((recomendations) =>
    {
      this.componentEventsBus.publishEvent(new ComponentEvent('DataReceived', ComponentState.success, 'Recomendations Received', {}));
      this.recomendationsViewModel = recomendations;

    }).catch(reason =>
    {
      this.componentEventsBus.publishEvent(new ComponentEvent('DataReceived', ComponentState.error, `Getting recomendations thrown error ${reason}`, {}));
    });

    this.signalRClient.Init();

    this.recomendationsSubscription = this.signalRClient.recomendationsReceived.subscribe((weatherConditionRecomendation) => this.databind(weatherConditionRecomendation));
  }

  

  private databind(recomendationEvent: IWeatherConditionRecomendation)
  {
    let found: boolean;
    found = false;
    this.recomendationsViewModel.forEach((recomendation) => {
      
      if (recomendation.location.city == recomendationEvent.location.city) {
        recomendation.actions = recomendationEvent.actions;
        recomendation.dressCode = recomendationEvent.dressCode;
        recomendation.date = recomendationEvent.date;
        found = true;
      }
    });
    if (!found) {
      this.recomendationsViewModel.push(recomendationEvent);
    }
  }

  private async getActiveRecomendations(): Promise<IWeatherConditionRecomendation[]> {
    var results = await this.httpClient.get<IWeatherConditionRecomendation[]>(this.baseUrl + 'recomendations').toPromise();
    return results;
  } 

  constructor(componentEventsBus: ComponentEventsBus,signalRClient: SignalRApplicationClient, http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    this.baseUrl = baseUrl;
    this.httpClient = http;
    this.signalRClient = signalRClient;
    this.componentEventsBus = componentEventsBus;
  }
}

