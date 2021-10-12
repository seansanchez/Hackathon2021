import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { CameraViewComponent } from './camera-view.component';

@NgModule({
  declarations: [
    CameraViewComponent
  ],
  imports: [
    CommonModule
  ],
  exports: [
    CameraViewComponent
  ]
})
export class CameraViewModule { }
