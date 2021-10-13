import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ConfirmationDialogComponent } from './confirmation-dialog/confirmation-dialog.component';
import { PlayDialogComponent } from './play-dialog/play-dialog.component';

@NgModule({
    declarations: [
        ConfirmationDialogComponent,
        PlayDialogComponent
    ],
    imports: [
        CommonModule,
        FormsModule
    ],
    entryComponents: [
        ConfirmationDialogComponent,
        PlayDialogComponent
    ],
    exports: [
        ConfirmationDialogComponent,
        PlayDialogComponent
    ]
})
export class DialogModule { }
