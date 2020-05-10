import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { HttpEventType, HttpClient } from '@angular/common/http';
import { UploadFileViewModel } from './UploadFileViewModel';

@Component({
  selector: 'upload-file',
  templateUrl: './upload-file.component.html',
  styleUrls: ['./upload-file.component.css']
})
export class UploadComponent  {
  public progress: number;
  public message: string;

  public viewModel: UploadFileViewModel;

  
  @Output() public onUploadFinished = new EventEmitter();

  constructor(private http: HttpClient) {

    this.viewModel = new UploadFileViewModel();

  }

  ngOnInit() {
  }

  public addFile = (files) => {
    if (files.length === 0) {
      return;
    }

    let fileToUpload = <File>files[0];

    this.viewModel.addFile(fileToUpload);
    this.message = 'File Added to be uploaded.';

    //const formData = new FormData();
    //formData.append('file', fileToUpload, fileToUpload.name);

    //this.http.post('https://localhost:5001/api/upload', formData, { reportProgress: true, observe: 'events' })
    //  .subscribe(event => {
    //    if (event.type === HttpEventType.UploadProgress)
    //      this.progress = Math.round(100 * event.loaded / event.total);
    //    else if (event.type === HttpEventType.Response) {
    //      this.message = 'Upload success.';
    //      this.onUploadFinished.emit(event.body);
    //    }
    //  });
  }
}
