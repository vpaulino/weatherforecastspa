import { ImageViewModel } from '../viewModels/ImageViewModel';

export class UploadFileViewModel {

  public images: ImageViewModel[];

  constructor() {
    this.images = [];
  }

  public addFile(file: File) {
    this.images.push(new ImageViewModel(file));
  }

  public clearAllFiles() {
    this.images = [];
  }

  public filesAdded() {
    return this.images.length > 0;
  }

  public getAllFiles() : ImageViewModel[] {

    return this.images;
  }



}
