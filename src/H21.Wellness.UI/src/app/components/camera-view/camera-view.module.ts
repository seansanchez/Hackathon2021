import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { LoadingIndicatorModule } from '../loading-indicator/loading-indicator.module';
import { CameraViewComponent } from './camera-view.component';

@NgModule({
  declarations: [
    CameraViewComponent
  ],
  imports: [
    CommonModule,
    LoadingIndicatorModule
  ],
  exports: [
    CameraViewComponent
  ]
})
export class CameraViewModule { }
