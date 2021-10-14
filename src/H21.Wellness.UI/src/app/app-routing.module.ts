import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { CreateComponent } from './pages/create/create.component';
import { HomeComponent } from './pages/home/home.component';
import { ScavengerHuntComponent } from './pages/scavenger-hunt/scavenger-hunt.component';
import { CanDeactivateGuard } from './route-guards/can-deactivate.guard';

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
    canDeactivate: [CanDeactivateGuard]
  },
  {
    path: 'create',
    component: CreateComponent,
    pathMatch: 'full',
    canDeactivate: [CanDeactivateGuard]
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
