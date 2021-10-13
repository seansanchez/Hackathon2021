import { A11yModule } from '@angular/cdk/a11y';
import { OverlayModule } from '@angular/cdk/overlay';
import { HttpClientModule } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { CameraViewModule } from './components/camera-view/camera-view.module';
import { DialogModule } from './components/dialogs/dialog.module';
import { LoadingIndicatorModule } from './components/loading-indicator/loading-indicator.module';
import { PhotoAlbumModule } from './components/photo-album/photo-album.module';
import { PreyListModule } from './components/prey-list/prey-list.module';
import { HomeComponent } from './pages/home/home.component';
import { ScavengerHuntComponent } from './pages/scavenger-hunt/scavenger-hunt.component';
import { ServiceWorkerModule } from '@angular/service-worker';
import { environment } from '../environments/environment';

@NgModule({
  declarations: [
    AppComponent,
    HomeComponent,
    ScavengerHuntComponent
  ],
  imports: [
    A11yModule,
    BrowserAnimationsModule,
    BrowserModule,
    AppRoutingModule,
    CameraViewModule,
    DialogModule,
    HttpClientModule,
    LoadingIndicatorModule,
    OverlayModule,
    PhotoAlbumModule,
    PreyListModule,
    ServiceWorkerModule.register('ngsw-worker.js', {
      enabled: environment.production,
      registrationStrategy: 'registerWhenStable:30000'
    })
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
