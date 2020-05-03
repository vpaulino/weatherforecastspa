import { Component } from '@angular/core';
import { ComponentEvent, ComponentState } from '../viewModels/componentEvent';
import { ComponentEventsBus } from '../services/componentEventsBus.service';
import { trigger, state, style, transition, animate } from '@angular/animations';

@Component({
  selector: 'state-menu',
  templateUrl: './state-menu.component.html',
  styleUrls: ['./state-menu.component.css'],
  animations: [
    trigger('expandCollapse', [
      state('0', style({
        opacity: 0,'height': '0px'
      })),
      state('2', style({
        opacity: 1, 'height': '20px', 'background-color': '#d4edda', 'border': '1px solid transparent','border-radius': '.25rem', 'margin-top': '1rem', 'margin-bottom': '1rem'
      })),
      state('1', style({
        opacity: 1, 'height': '20px', 'background-color': '#d1ecf1', 'border': '1px solid transparent','border-radius': '.25rem', 'margin-top': '1rem', 'margin-bottom': '1rem'
      })),
      state('3', style({
        opacity: 1, 'height': '20px', 'background-color': '#fff3cd', 'border': '1px solid transparent', 'border-radius': '.25rem', 'margin-top': '1rem', 'margin-bottom': '1rem'
      })),
      state('4', style({
        opacity: 1, 'height': '20px', 'background-color': '#f8d7da', 'border-radius': '.25rem', 'margin-top': '1rem', 'margin-bottom': '1rem'
      })),
      transition('* <=> 0', animate(600, style({ opacity: 0 }))),
      transition('0 <=> *', animate(600, style({ opacity: 1 })))
    ])
  ]
})
export class StateMenuComponent {
  
  private componentEventBus: ComponentEventsBus;
  public appstateViewModel: ComponentEvent;

  constructor(componentEventBus: ComponentEventsBus){
    this.componentEventBus = componentEventBus;
  }

  ngOnInit(): void {
  
    this.componentEventBus.componentEventsReceived.subscribe((componentEventReceived) =>
    {
      console.log("state element received notification: " + componentEventReceived);
      this.appstateViewModel = componentEventReceived;
    });
  }

}
