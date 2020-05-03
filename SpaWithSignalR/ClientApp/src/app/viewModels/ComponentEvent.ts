
export interface IComponentEvent {
  eventId: string;
  status: ComponentState;
  message: string;
  details: any;
}

export enum ComponentState {
  idle,
  information,
  success,
  warning,
  error
}

export class ComponentEvent implements IComponentEvent {

  public eventId: string;
  public status: ComponentState;
  public message: string;
  public details: any;

  constructor(eventId: string, status: ComponentState, message: string, details:any) {
    this.eventId = eventId;
    this.status = status;
    this.message = message;
    this.details = details;
  }
}
