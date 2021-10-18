import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { LoadingIndicatorModule } from '../loading-indicator/loading-indicator.module';
import { MessageChipComponent } from '../message-chip/message-chip.component';
import { ConfirmationDialogComponent } from './confirmation-dialog/confirmation-dialog.component';
import { PlayDialogComponent } from './play-dialog/play-dialog.component';

@NgModule({
    declarations: [
        ConfirmationDialogComponent,
        PlayDialogComponent,
        MessageChipComponent
    ],
    imports: [
        CommonModule,
        FormsModule,
        LoadingIndicatorModule
    ],
    entryComponents: [
        ConfirmationDialogComponent,
        PlayDialogComponent,
        MessageChipComponent
    ],
    exports: [
        ConfirmationDialogComponent,
        PlayDialogComponent,
        MessageChipComponent
    ]
})
export class DialogModule { }
