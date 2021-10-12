import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { CameraViewModule } from './components/camera-view/camera-view.module';
import { HomeComponent } from './pages/home/home.component';
import { ScavengerHuntComponent } from './pages/scavenger-hunt/scavenger-hunt.component';

@NgModule({
  declarations: [
    AppComponent,
    HomeComponent,
    ScavengerHuntComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    CameraViewModule
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
