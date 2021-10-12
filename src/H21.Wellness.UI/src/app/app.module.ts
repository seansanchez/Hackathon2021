import { A11yModule } from '@angular/cdk/a11y';
import { OverlayModule } from '@angular/cdk/overlay';
import { HttpClientModule } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { CameraViewModule } from './components/camera-view/camera-view.module';
import { ConfirmationDialogComponent } from './components/dialogs/confirmation-dialog/confirmation-dialog.component';
import { PlayDialogComponent } from './components/dialogs/play-dialog/play-dialog.component';
import { LoadingIndicatorModule } from './components/loading-indicator/loading-indicator.module';
import { PreyListModule } from './components/prey-list/prey-list.module';
import { HomeComponent } from './pages/home/home.component';
import { ScavengerHuntComponent } from './pages/scavenger-hunt/scavenger-hunt.component';

@NgModule({
  declarations: [
    AppComponent,
    ConfirmationDialogComponent,
    HomeComponent,
    PlayDialogComponent,
    ScavengerHuntComponent
  ],
  imports: [
    A11yModule,
    BrowserAnimationsModule,
    BrowserModule,
    AppRoutingModule,
    CameraViewModule,
    FormsModule,
    HttpClientModule,
    LoadingIndicatorModule,
    OverlayModule,
    PreyListModule
  ],
  entryComponents: [
    ConfirmationDialogComponent,
    PlayDialogComponent
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
