import { ILocation, Location } from './location';


export interface IWeatherConditionRecomendation
{
  date: Date;
  location: ILocation;
  actions: string[];
  dressCode: string[]; 
}

export class WeatherConditionRecomendation implements IWeatherConditionRecomendation {

  public constructor() {
    this.location = new Location();
    this.actions = [];
    this.dressCode = [];
  }

  date: Date;
  location: ILocation;
  actions: string[];
  dressCode: string[];
   
}
