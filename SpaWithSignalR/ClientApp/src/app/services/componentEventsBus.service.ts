
import { Inject, Injectable, EventEmitter, Output } from '@angular/core';
import { IComponentEvent, ComponentEvent, ComponentState } from '../viewModels/componentEvent';
import { BehaviorSubject } from 'rxjs';


@Injectable()
export class ComponentEventsBus
{
  constructor(){

  }

  public componentEventsReceived = new BehaviorSubject<IComponentEvent>(new ComponentEvent("EventBusCreated", ComponentState.idle, "EventBus created ready to receive events", {}));

  public publishEvent(componentEvent: IComponentEvent) {
    console.log("ComponentEventsBus event fired: " + componentEvent.toString());
    this.componentEventsReceived.next(componentEvent);
  }
}
