import { ImageViewModel } from '../viewModels/ImageViewModel';



export class ImageDisplayViewModel {

  public imagesViewModels: ImageViewModel[];
  public addImageToDisplay(file: File) {
    this.imagesViewModels.push(new ImageViewModel(file));
  }
}
