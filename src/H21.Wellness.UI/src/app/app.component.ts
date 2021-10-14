import { Component } from '@angular/core';
import { NavigationEnd, Router } from '@angular/router';
import { DialogService } from './services/dialog.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {

  constructor(
    private dialogService: DialogService,
    private readonly router: Router) {
    this.router.events.subscribe(ev => {
      if (ev instanceof NavigationEnd) {
        this.dialogService.closeAllConfirmationDialogs();
        this.dialogService.closeAllPlayDialogs();
      }
    });
  }

  public goHome(): void {
    this.router.navigate(['']);
  }
}
