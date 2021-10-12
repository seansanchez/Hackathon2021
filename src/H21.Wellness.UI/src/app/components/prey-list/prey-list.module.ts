import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { PreyListComponent } from './prey-list.component';

@NgModule({
  declarations: [
    PreyListComponent
  ],
  imports: [
    CommonModule
  ],
  exports: [
    PreyListComponent
  ]
})
export class PreyListModule { }
