import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { DialogService } from 'src/app/services/dialog.service';

@Component({
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent implements OnInit {

  constructor(
    private readonly dialogService: DialogService,
    private readonly router: Router
  ) { }

  /** Initialization lifecycle hook */
  public ngOnInit(): void {
    sessionStorage.removeItem('GameInProgress');
  }

  public start(): void {
    this.dialogService.displayPlayDialog().subscribe(gameCode => {
      if (gameCode !== undefined && gameCode.length > 0) {
        this.router.navigate(['/scavenger-hunt'], { queryParams: { game: gameCode }});
      } else if (gameCode !== undefined && gameCode.length === 0) {
        this.router.navigate(['/scavenger-hunt']);
      } 
    });
  }
}
