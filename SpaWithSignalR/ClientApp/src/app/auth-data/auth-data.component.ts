
import { Component, Inject } from '@angular/core';
import { ActivatedRoute, ParamMap, Router, Params } from '@angular/router';


@Component({
  selector: 'auth-data',
  templateUrl: './auth-data.component.html'
})
export class AuthComponent {

  public login() {
    console.log('login pressed');
  }

}
