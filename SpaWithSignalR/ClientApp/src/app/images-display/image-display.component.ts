
import { Component, OnInit, Output, EventEmitter, Input } from '@angular/core';
import { ImageViewModel } from '../viewModels/ImageViewModel';


@Component({
  selector: 'img-display',
  templateUrl: './image-display.component.html',
  styleUrls: ['./image-display.component.css']
})
export class ImageDisplayComponent {

  @Input("viewModel") viewModel : ImageViewModel[]
}
