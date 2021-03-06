import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { Observable, of, Subject, Subscription, timer } from 'rxjs';
import { catchError, takeUntil, timeout } from 'rxjs/operators';
import * as dayjs from 'dayjs';
import html2canvas from 'html2canvas';
import { IPrey } from 'src/app/models/IPrey';
import { PopFadeInAnimation } from 'src/app/animations/popFadeIn.animation';
import { PreyListComponent } from 'src/app/components/prey-list/prey-list.component';
import { CameraViewComponent } from 'src/app/components/camera-view/camera-view.component';
import { DialogService } from 'src/app/services/dialog.service';
import { ActivatedRoute, Router } from '@angular/router';
import { environment } from 'src/environments/environment';
import { ApiService } from 'src/app/services/api.service';
import { ISnapshot } from 'src/app/models/ISnapshot';
import { CanDeactivateBase } from '../CanDeactivateBase';
import { MessageTypeEnum } from 'src/app/components/message-chip/MessageTypeEnum';

@Component({
    templateUrl: './scavenger-hunt.component.html',
    styleUrls: ['./scavenger-hunt.component.scss'],
    animations: [PopFadeInAnimation]
})
export class ScavengerHuntComponent extends CanDeactivateBase implements OnInit, OnDestroy {
    public items: IPrey[] = [];
    public preyImageMap = new Map<IPrey, string>();

    private _gameStarting = false;
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
    private _gameCode?: string;
    private _gameName: string = '';
    private _timeLimitMinutes!: number;
    private _gameLoading: boolean = true;
    private _scoring: boolean = false;
    private _viewPhotos = false;

    @ViewChild('cameraView', { read: CameraViewComponent }) private cameraView!: CameraViewComponent;
    @ViewChild('preyList', { read: PreyListComponent }) private preyList!: PreyListComponent;

    constructor(
        private readonly activatedRoute: ActivatedRoute,
        private readonly apiService: ApiService,
        private readonly dialogService: DialogService,
        private readonly router: Router) {
        super();
    }

    /** Initialization lifecycle hook. */
    public ngOnInit(): void {
        this.activatedRoute.queryParamMap
            .pipe(
                takeUntil(this._ngDestroy)
            ).subscribe(params => {
                const gameCode = params.get("game");
                if (gameCode) {
                    this._gameCode = gameCode;
                    this.router.navigate(['/scavenger-hunt'], { queryParams: {}, replaceUrl: true });
                } else {
                    const gameInProgress = sessionStorage.getItem('GameInProgress');
                    if (gameInProgress && (!this._gameCode || this._gameCode !== gameInProgress)) {
                        sessionStorage.removeItem('GameInProgress');
                        this.router.navigate(['/']);
                        return;
                    } else {
                        this.getGame();
                    }
                }
            });
    }

    /** Destroy lifecycle hook. */
    public ngOnDestroy(): void {
        this._ngDestroy.next();
    }

    /** Overriding base class */
    public canDeactivate(): Observable<boolean> | boolean {
        if (this._gameInProgress) {
            return this.dialogService.displayConfirmationDialog('Are you sure you want to leave this page? <br>All game progress will be lost.', 'Game in Progress', 'Leave', 'Stay', true);
        } else {
            return true;
        }
    }

    public getGame(): void {
        this._gameStarting = true;
        this._gameLoading = true;
        this.apiService.getScavengerHunt(this._gameCode!)
            .pipe(
                catchError(() => {
                    this.dialogService.displayConfirmationDialog('We can\'t find that scavenger hunt game. Try a different code.', 'Oops!', 'Try Again').subscribe(() => {
                        this.router.navigate(['/']);
                    });
                    return of(null);
                })
            )
            .subscribe(scavengerHunt => {
                if (scavengerHunt) {
                    this._gameName = scavengerHunt.name;
                    this._gameCode = scavengerHunt.id;
                    this._timeLimitMinutes = scavengerHunt.timeLimitInMinutes;
                    this.items = scavengerHunt.items.map(i => <IPrey>{
                        id: i.id,
                        name: i.name,
                        complete: false
                    });
                    this._gameLoading = false;
                    sessionStorage.setItem('GameInProgress', this._gameCode);
                }
            });
    }

    public startGame(): void {
        this._ngDestroy.next();

        if (this.cameraView.hasCameras) {
            this.cameraView.startCameraStream();

            this._gameInProgress = true;
            this._gameOverTime = dayjs().add(this._timeLimitMinutes, 'minutes');
            timer(500, 1000).pipe(
                takeUntil(this._ngDestroy)
            ).subscribe(() => {
                this._secondsRemaining = this._gameOverTime.diff(dayjs(), 'seconds');

                if (this._secondsRemaining <= 0) {
                    this.gameOver();
                }
            });
        } else {
            this.dialogService.displayMessageChip(`Camera errors`, MessageTypeEnum.error, true).subscribe();
            this.dialogService.displayConfirmationDialog('Sorry, we aren\'t detecting any cameras for your device... Try refreshing your browser', 'Uh oh', 'Refresh', 'Go Home')
                .subscribe(res => {
                    if (res) {
                        window.location.replace(environment.uiUrl);
                    } else {
                        this.router.navigate(['/']);
                    }
                });
        }
    }

    public shareGame(): void {
        this._sharing = true;
        navigator.share(<ShareData>{
            title: 'Breath of Fresh Where?',
            text: `I'm about to play this scavenger hunt! You should check it out:`,
            url: `${environment.uiUrl}/scavenger-hunt?game=${this._gameCode}`
        })
            .then(() => this._sharing = false)
            .catch(() => {
                this.dialogService.displayMessageChip('Error sharing game.', MessageTypeEnum.error, true).subscribe();
                this._sharing = false;
            });
    }

    public getRandomGame(): void {
        this.dialogService.displayConfirmationDialog('Are you sure you want to get a new random game?', 'Randomize', 'Yes', 'Cancel')
            .subscribe(res => {
                if (res) {
                    this._gameCode = undefined;
                    this.getGame();
                }
            });
    }

    /** Gets the name of the scavenger hunt. */
    public get gameName(): string {
        return this._gameName;
    }

    /** Whether the game is starting or not. */
    public get gameStarting(): boolean {
        return this._gameStarting;
    }

    /** Whether the game is loading or not. */
    public get gameLoading(): boolean {
        return this._gameLoading;
    }

    /** Whether the game is scoring or not. */
    public get scoring(): boolean {
        return this._scoring;
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

    /** Whether the cameras are ready or not. */
    public get cameraReady(): boolean {
        return this.cameraView ? !this.cameraView.isLoading : false;
    }

    /** Whether the image is processing or not. */
    public get imageProcessing(): boolean {
        return this._processing;
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

    /** Whether or not to view photos. */
    public get viewPhotos(): boolean {
        return this._viewPhotos;
    }

    /** Updates the selected item. */
    public itemSelected(item: IPrey): void {
        this._currItem = item;
    }

    /** Process image capture and progress the game. */
    public imageCaptured(snapshot: ISnapshot): void {
        this.updateImageProcessingStatus(true);
        const apiDone = new Subject();
        const apiSub = this.apiService.checkImageMatch(this._currItem.id, snapshot.scaledImageUri)
            .pipe(
                catchError(() => {
                    this.dialogService.displayMessageChip(`Sorry. Something broke.`, MessageTypeEnum.error, true).subscribe();
                    this.dialogService.displayConfirmationDialog('There was an issue processing that picture.', 'Uh oh', 'Try Again', 'Cancel', true)
                        .subscribe(res => {
                            if (res) {
                                this.imageCaptured(snapshot);
                            } else {
                                this.updateImageProcessingStatus(false);
                            }
                        })
                    return of(null);
                })
            )
            .subscribe(res => {
                apiDone.next();
                if (res && res.isMatch) {
                    this.dialogService.closeAllConfirmationDialogs();
                    this.dialogService.displayMessageChip(`You found a ${this._currItem.name}!`, MessageTypeEnum.success, true).subscribe();
                    this.preyImageMap.set(this._currItem, snapshot.imageUri);
                    this.preyList.completeItem(this._currItem);
                    this.updateImageProcessingStatus(false);
                } else if (res && !res.isMatch) {
                    this.dialogService.displayConfirmationDialog(
                        `Hmmm... that picture doesn't seem to contain a "<i>${this._currItem.name}</i>". <br>Try taking another one!`,
                        'No match', 'Ok')
                        .subscribe(() => {
                            this.updateImageProcessingStatus(false);
                        });
                }
            });
    }

    /** Process game over. */
    public gameOver(): void {
        this._ngDestroy.next();
        this._gameInProgress = false;

        this._scoring = true;
        const timeToComplete = this._timeLimitMinutes - Math.floor(this._gameOverTime.diff(dayjs(), 'minutes'));
        this.endGame(timeToComplete);
    }

    private endGame(timeToComplete: number) {
        this.apiService.getScore(this._gameCode!, this.numItemsComplete, timeToComplete)
            .pipe(
                timeout(10000),
                catchError(() => {
                    this.dialogService.displayMessageChip(`Sorry. Something broke.`, MessageTypeEnum.error, true).subscribe();
                    this.dialogService.displayConfirmationDialog('There was an issue calculating your score.', 'Uh oh', 'Try Again', undefined, true)
                        .subscribe(() => {
                            this.endGame(timeToComplete);
                        })
                    return of(null);
                })
            )
            .subscribe(res => {
                if (res) {
                    this._gameComplete = true;
                    this._scoring = false;

                    this._finalScore = res.score;
                    this.dialogService.displayMessageChip(`You finished! Great job!`, MessageTypeEnum.success, true).subscribe();
                }
            });
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
                        text: `I scored ${this.finalScore} points in this scavenger hunt! Play it and see if you can do better:`,
                        url: `${environment.uiUrl}/scavenger-hunt?game=${this._gameCode}`
                    })
                        .then(() => this._sharing = false)
                        .catch(() => {
                            this._errorSharing = true;
                            this._sharing = false;
                        });
                })
                .catch(() => {
                    this._errorSharing = true;
                    this._sharing = false;
                });
        }
    }

    /** Views the photos from the previous game */
    public viewPhotoAlbum(view: boolean): void {
        this._viewPhotos = view;
    }

    private timeoutImageProcessing(apiSub: Subscription, apiDone: Subject<any>) {
        timer(15000).pipe(
            takeUntil(apiDone)
        ).subscribe(() => {
            this.dialogService.displayConfirmationDialog('This is taking longer than normal. Would you like to keep waiting or cancel and try again?', 'Still processing', 'Wait', 'Cancel', true)
                .subscribe(res => {
                    if (!res && apiSub && !apiSub.closed) {
                        apiSub.unsubscribe();
                        this.dialogService.displayMessageChip('Cancelled Image Capture', MessageTypeEnum.info, true);
                    } else {
                        this.timeoutImageProcessing(apiSub, apiDone);
                    }
                });
        });
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
