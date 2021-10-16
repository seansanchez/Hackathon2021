import { Component } from '@angular/core';
import { NavigationEnd, Router } from '@angular/router';
import { SwUpdate } from '@angular/service-worker';
import { environment } from 'src/environments/environment';
import { MessageTypeEnum } from './components/message-chip/MessageTypeEnum';
import { DialogService } from './services/dialog.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {

  constructor(
    private readonly dialogService: DialogService,
    private readonly router: Router,
    private readonly swUpdate: SwUpdate) {
    this.router.events.subscribe(ev => {
      if (ev instanceof NavigationEnd) {
        this.dialogService.closeAllConfirmationDialogs();
        this.dialogService.closeAllPlayDialogs();
      }
    });

    this.swUpdate.available.subscribe(() => {
      this.dialogService.displayMessageChip('New update available!', MessageTypeEnum.info).subscribe(() => {
        if (this.router.url === '/scavenger-hunt') {
          const gameCode = sessionStorage.getItem('GameInProgress');
          if (gameCode) {
            window.location.replace(`${environment.uiUrl}/scavenger-hunt?game=${gameCode}`);
          } else {
            window.location.reload();
          }
        } else {
          window.location.reload();
        }
      })
    });
  }

  public goHome(): void {
    this.router.navigate(['']);
  }
}
