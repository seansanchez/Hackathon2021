import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CameraViewModule } from '../camera-view/camera-view.module';
import { ConfirmationDialogComponent } from './confirmation-dialog/confirmation-dialog.component';
import { ImageDetectDialogComponent } from './image-detect-dialog/image-detect-dialog.component';
import { PlayDialogComponent } from './play-dialog/play-dialog.component';

@NgModule({
    declarations: [
        ConfirmationDialogComponent,
        ImageDetectDialogComponent,
        PlayDialogComponent
    ],
    imports: [
        CameraViewModule,
        CommonModule,
        FormsModule
    ],
    entryComponents: [
        ConfirmationDialogComponent,
        ImageDetectDialogComponent,
        PlayDialogComponent
    ],
    exports: [
        ConfirmationDialogComponent,
        ImageDetectDialogComponent,
        PlayDialogComponent
    ]
})
export class DialogModule { }
