import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { RouterModule } from '@angular/router';

import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { HomeComponent } from './home/home.component';
import { InsertDataComponent } from './insert-data/insert-data.component';
import { FetchDataComponent } from './fetch-data/fetch-data.component';
import { RecomendationDataComponent } from './recomendation-data/recomendation-data.component';
import { SignalRApplicationClient } from './services/signalrApplicationClient.service';
import { ComponentEventsBus } from './services/componentEventsBus.service';
import { StateMenuComponent } from './state-menu/state-menu.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { Routes } from '@angular/router';
import { WeatherForecastHttpClient } from './services/weatherforecast.httpclient';
import { UploadComponent } from './uploadfiles/upload-file.component';
import { ImageDisplayComponent } from './images-display/image-display.component';
@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    HomeComponent,
    InsertDataComponent,
    FetchDataComponent,
    RecomendationDataComponent,
    StateMenuComponent,
    UploadComponent,
    ImageDisplayComponent
    
  ],
  imports: [
    BrowserAnimationsModule,
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    RouterModule.forRoot([
      { path: '', component: HomeComponent, pathMatch: 'full' },
      { path: 'insert-data', component: InsertDataComponent },
      { path: 'fetch-data', component: FetchDataComponent },
      { path: 'recomendation-data', component: RecomendationDataComponent}
    ])
  ],
  providers: [SignalRApplicationClient, ComponentEventsBus, WeatherForecastHttpClient],
  bootstrap: [AppComponent]
})
export class AppModule { }
