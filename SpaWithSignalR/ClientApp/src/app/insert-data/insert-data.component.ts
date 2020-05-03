import { Component, Inject } from '@angular/core';
import { ActivatedRoute, ParamMap, Router, Params } from '@angular/router';
import { Location } from '../viewModels/location';
import { WeatherForecast } from '../viewModels/weatherForecast';
import { HttpClient } from '@angular/common/http';
import { ComponentEventsBus } from '../services/componentEventsBus.service';
import { ComponentEvent, ComponentState } from '../viewModels/componentEvent';
import { WeatherForecastHttpClient } from '../services/weatherforecast.httpclient';
import { EditForecastViewModel } from './EditForecastViewModel';

@Component({
  selector: 'insert-data.component',
  templateUrl: './insert-data.component.html'
})
export class InsertDataComponent {


  private baseUrl: string;
  private httpClient: HttpClient;
  private componentEventsBus: ComponentEventsBus;

  public viewModel: EditForecastViewModel;

  private weatherForecastHttpClient: WeatherForecastHttpClient;

  private activatedRoute: ActivatedRoute;

  ngOnDestroy(): void {
    this.componentEventsBus.publishEvent(new ComponentEvent("componentDestroy", ComponentState.idle, "leaving component", {}));
  }

  private disableDataKeyControls() {
    this.viewModel.isEdit = true;
  }

  private navigateHereWithParameters(params: Params): boolean {
    return params['country'] != undefined && params['region'] != undefined && params['city'] != undefined;
  }

  ngOnInit(): void {

   
    this.activatedRoute.queryParams.subscribe(params => {

      this.viewModel = new EditForecastViewModel();
      var initialStatePromise = this.setInitialFormData();
      initialStatePromise.finally(() => {
        if (this.navigateHereWithParameters(params)) {
          this.disableDataKeyControls();
          var location = this.GetFormStateFromQueryParameters(params);
          var promiseResult = this.weatherForecastHttpClient.getForecastByLocation(location);
          promiseResult.then((forecast) => {
            this.databindForecast(forecast);
            this.componentEventsBus.publishEvent(new ComponentEvent("componentInitialized", ComponentState.success, "data init success", {}));
          }).catch(reason => {
            this.componentEventsBus.publishEvent(new ComponentEvent("componentInitialized", ComponentState.error, `data init failed ${reason}`, {}));

          });
        }
      }).catch((reason) => {
        this.componentEventsBus.publishEvent(new ComponentEvent("componentInitialized", ComponentState.error, `init locations failed ${reason}`, {}));
      });
    });

  }


  constructor(route: ActivatedRoute, componentEventsBus: ComponentEventsBus, weatherForecastHttpClient: WeatherForecastHttpClient) {
    this.weatherForecastHttpClient = weatherForecastHttpClient;
    this.componentEventsBus = componentEventsBus;
    this.activatedRoute = route;
    this.viewModel = new EditForecastViewModel();
  }


  private GetFormStateFromQueryParameters(params: Params): Location {
    var locationQueryParam = new Location();
    locationQueryParam.city = params['city'];
    locationQueryParam.region = params['region'];
    locationQueryParam.country = params['country'];
    return locationQueryParam;
  }

  private databindForecast(forecastEvent: WeatherForecast) {

    this.viewModel.dataBind(forecastEvent);
  }

  private setInitialFormData(): Promise<void> {

    
    var allLocationsPromise = this.weatherForecastHttpClient.getAllLocations();
    var allSkiesPromise = this.weatherForecastHttpClient.getAllSkies();

    var promise = Promise.all([allLocationsPromise, allSkiesPromise]).then((allPromisses) => {
      this.viewModel.dataBindLocations(allPromisses[0]);
      this.viewModel.dataBindSkies(allPromisses[1]);
      this.componentEventsBus.publishEvent(new ComponentEvent("componentInitialized", ComponentState.success, "All Initial data was fetch with sucess", {}));
    }).catch((reason) => {
      this.componentEventsBus.publishEvent(new ComponentEvent("componentInitialized", ComponentState.error, `Not all Initial data was not fetch ${reason} `, {}));
    });
    return promise;
  }



  public submit() {

    var dataToSubmit = this.viewModel.getFormData();

    this.componentEventsBus.publishEvent(new ComponentEvent("SubmitingEntity", ComponentState.information, "Weather forcast is being created...", {}));

    var promiseResult = this.weatherForecastHttpClient.SetForecastByLocation(dataToSubmit);
    promiseResult.then(result => {
      this.componentEventsBus.publishEvent(new ComponentEvent("EntitySubmitted", ComponentState.success, "Weather forcast is Submited with success", {}));
    });
    promiseResult.catch(reason => {
      this.componentEventsBus.publishEvent(new ComponentEvent("EntitySubmitted", ComponentState.error, "Weather forcast request result into error", {}));
    });
  }
}


