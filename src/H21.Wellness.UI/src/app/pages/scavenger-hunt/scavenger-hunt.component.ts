import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { Subject, timer } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import * as dayjs from 'dayjs';
import html2canvas from 'html2canvas';
import { IPrey } from 'src/app/components/prey-list/IPrey';
import { PopFadeInAnimation } from 'src/app/animations/popFadeIn.animation';
import { PreyListComponent } from 'src/app/components/prey-list/prey-list.component';
import { CameraViewComponent } from 'src/app/components/camera-view/camera-view.component';
import { CanDeactivate } from '@angular/router';

@Component({
    templateUrl: './scavenger-hunt.component.html',
    styleUrls: ['./scavenger-hunt.component.scss'],
    animations: [PopFadeInAnimation]
})
export class ScavengerHuntComponent implements OnInit, OnDestroy {
    public items: IPrey[] = [
        { name: 'Dog', complete: false },
        { name: 'Mailbox', complete: false },
        { name: 'Tree', complete: false },
        { name: 'Car', complete: false },
        { name: 'House', complete: false },
        { name: 'Bird', complete: false },
        { name: 'Flower', complete: false }
    ];

    private _gameInProgress = false;
    private _gameOverTime: dayjs.Dayjs = dayjs();
    private _secondsRemaining: number = 20 * 60;
    private _ngDestroy = new Subject();
    private _gameComplete = false;
    private _finalScore: number = 0;
    private _sharing = false;
    private _shareError: string = 'Failed to share score. Try again';
    private _errorSharing: boolean = false;
    private _currItem!: IPrey;
    private _processing: boolean = false;

    @ViewChild('cameraView', { read: CameraViewComponent }) private cameraView!: CameraViewComponent;
    @ViewChild('preyList', { read: PreyListComponent }) private preyList!: PreyListComponent;

    /** Initialization lifecycle hook. */
    public ngOnInit(): void {
    }
    /** Destroy lifecycle hook. */
    public ngOnDestroy(): void {
        this._ngDestroy.next();
    }

    public canDeactivate(): boolean {
        return this._gameInProgress ? false : true;
    }

    public startGame(): void {
        this._ngDestroy.next();

        this._gameInProgress = true;

        this._gameOverTime = dayjs().add(20, 'minutes');
        timer(500, 1000).pipe(
            takeUntil(this._ngDestroy)
        ).subscribe(() => {
            this._secondsRemaining = this._gameOverTime.diff(dayjs(), 'seconds');

            if (this._secondsRemaining <= 0) {
                this.gameOver();
            }
        });
    }

    /** Gets the time remaining in MM:SS timestamp for display. */
    public get timeRemaining(): string {
        return this.convertToTimestamp(this._secondsRemaining);
    }

    /** Whether the game is in progress or not. */
    public get gameInProgress(): boolean {
        return this._gameInProgress;
    }

    /** Number of complete items. */
    public get numItemsComplete(): number {
        return this.items.filter(i => i.complete).length;
    }

    /** Gets the final calculated score. */
    public get finalScore(): number {
        return this._finalScore;
    }

    /** Whether the game is complete or not. */
    public get gameComplete(): boolean {
        return this._gameComplete;
    }

    /** Whether the browser is sharing or not. */
    public get sharing(): boolean {
        return this._sharing;
    }

    /** Whether the image is processing or not. */
    public get imageProcessing(): boolean {
        return this._processing;
    }

    /** Updates the selected item. */
    public itemSelected(item: IPrey): void {
        this._currItem = item;
    }

    /** Process image capture and progress the game. */
    public imageCaptured(imageUri: string): void {
        this.updateImageProcessingStatus(true);
        console.log(imageUri);
        setTimeout(() => {
            this.preyList.completeItem(this._currItem);
            this.updateImageProcessingStatus(false);
        }, 200);
    }

    /** Process game over. */
    public gameOver(): void {
        this._ngDestroy.next();
        this._gameInProgress = false;
        this._gameComplete = true;

        const minutesRemaining = Math.floor(this._secondsRemaining / 60);
        const bonusScore = Math.floor(minutesRemaining / 2) * 5;
        this._finalScore = (this.numItemsComplete * 10) + bonusScore;
    }

    /** Shares the score through the browser share feature. */
    public shareScore(scoreContainer: HTMLElement): void {
        this._sharing = true;
        this._errorSharing = false;
        if (this.canShare) {
            html2canvas(scoreContainer)
                .then((canvas: HTMLCanvasElement) => {
                    const scoreImage = canvas.toDataURL('image/png', 1.0);
                    const filesArray = [this.dataURLtoFile(scoreImage, 'MyScavengerHuntScore.png')];
                    navigator.share(<ShareData>{
                        files: filesArray,
                        title: 'Breath of Fresh Where?',
                        text: 'Check out my scavenger hunt score!',
                    })
                        .then(() => this._sharing = false)
                        .catch(() => {
                            this._errorSharing = true; 
                            this._sharing = false;
                        })
                })
                .catch(() => {
                    this._errorSharing = true; 
                    this._sharing = false;
                });
        }
    }

    /** Whether the device can share. */
    public get canShare(): boolean {
        return 'share' in navigator;
    }

    /** The sharing error. */
    public get shareError(): string {
        return this._shareError;
    }

    /** Whether or not there was an error sharing. */
    public get errorSharing(): boolean {
        return this._errorSharing;
    }

    private updateImageProcessingStatus(processing: boolean) {
        this._processing = processing;
        this.cameraView.setImageProcessing(processing);
    }

    private dataURLtoFile(dataurl: string, filename: string) {
        const arr = dataurl.split(',');
        const mime = arr[0].match(/:(.*?);/)![1];
        const bstr = atob(arr[1]);
        let n = bstr.length;
        const u8arr = new Uint8Array(n);
        while (n--) {
            u8arr[n] = bstr.charCodeAt(n);
        }
        return new File([u8arr], filename, { type: mime });
    }

    private convertToTimestamp(totalSeconds: number): string {
        const roundedSeconds = Math.round(totalSeconds);
        const minutes = Math.floor(roundedSeconds / 60);
        const seconds = roundedSeconds - (minutes * 60);
        return `${this.padNumber(minutes, 2)}:${this.padNumber(seconds, 2)}`;
    }

    private padNumber(num: number, digits: number): string {
        let numString = `${num}`;
        for (let i = numString.length; i < digits; i++) {
            numString = `0${numString}`;
        }
        return numString;
    }
}
