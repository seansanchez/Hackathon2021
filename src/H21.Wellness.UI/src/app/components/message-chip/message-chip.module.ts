import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { LoadingIndicatorModule } from '../loading-indicator/loading-indicator.module';
import { MessageChipComponent } from './message-chip.component';

@NgModule({
  declarations: [
    MessageChipComponent
  ],
  imports: [
    CommonModule,
    LoadingIndicatorModule
  ],
  exports: [
    MessageChipComponent
  ]
})
export class MessageChipModule { }
