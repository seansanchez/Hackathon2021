import { Component } from '@angular/core';
import { Router } from '@angular/router';

@Component({
    templateUrl: './home.component.html',
    styleUrls: ['./home.component.scss']
})
export class HomeComponent {
    constructor(private readonly router: Router) {}
  
    public start(): void {
      this.router.navigate(['/scavenger-hunt'])
    }
}
