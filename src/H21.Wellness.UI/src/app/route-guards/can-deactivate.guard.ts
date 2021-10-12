import { Injectable } from "@angular/core";
import { ActivatedRouteSnapshot, CanDeactivate, RouterStateSnapshot, UrlTree } from "@angular/router";
import { Observable } from "rxjs";
import { ScavengerHuntComponent } from "../pages/scavenger-hunt/scavenger-hunt.component";

@Injectable({
    providedIn: 'root'
})
export class CanDeactivateScavengerHunt implements CanDeactivate<ScavengerHuntComponent> {
  public canDeactivate(
    component: ScavengerHuntComponent,
    currentRoute: ActivatedRouteSnapshot,
    currentState: RouterStateSnapshot,
    nextState: RouterStateSnapshot
  ): Observable<boolean|UrlTree>|Promise<boolean|UrlTree>|boolean|UrlTree {
    return component.canDeactivate();
  }
}