import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { HomeComponent } from './pages/home/home.component';
import { ScavengerHuntComponent } from './pages/scavenger-hunt/scavenger-hunt.component';
import { CanDeactivateScavengerHunt } from './route-guards/can-deactivate.guard';

const routes: Routes = [
  {
    path: '',
    component: HomeComponent,
    pathMatch: 'full'
  },
  {
    path: 'scavenger-hunt',
    component: ScavengerHuntComponent,
    pathMatch: 'full',
    canDeactivate: [CanDeactivateScavengerHunt]
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
