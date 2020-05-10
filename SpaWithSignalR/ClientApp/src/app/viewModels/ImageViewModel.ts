export class ImageViewModel {
    constructor(file: File) {
        let reader = new FileReader();
        reader.readAsDataURL(file); // read file as data url
        reader.onload = (event) => {
          this.src = reader.result;
        };
        this.file = file;
    }
    public src: string | ArrayBuffer;
    public file: File;
}
