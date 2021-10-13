import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { Observable, of, Subject, timer } from 'rxjs';
import { catchError, first, skip, takeUntil } from 'rxjs/operators';
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

@Component({
    templateUrl: './scavenger-hunt.component.html',
    styleUrls: ['./scavenger-hunt.component.scss'],
    animations: [PopFadeInAnimation]
})
export class ScavengerHuntComponent implements OnInit, OnDestroy {
    public items: IPrey[] = [];
    public preyImageMap = new Map<IPrey, string>();

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
    private _gameCode!: string;
    private _gameName: string = '';
    private _gameLoading: boolean = true;
    private _scoring: boolean = false;
    private _viewPhotos = false;

    @ViewChild('cameraView', { read: CameraViewComponent }) private cameraView!: CameraViewComponent;
    @ViewChild('preyList', { read: PreyListComponent }) private preyList!: PreyListComponent;

    constructor(
        private readonly activatedRoute: ActivatedRoute,
        private readonly apiService: ApiService,
        private readonly dialogService: DialogService,
        private readonly router: Router) { }

    /** Initialization lifecycle hook. */
    public ngOnInit(): void {
        (console as any).stdError = console.error.bind(console);
        (console as any).errors = [];
        console.error = function () {
            (console as any).errors.push(Array.from(arguments));
            (console as any).stdError.apply(console, arguments);
        };

        this.activatedRoute.queryParamMap
            .pipe(
                takeUntil(this._ngDestroy)
            ).subscribe(params => {
                const gameCode = params.get("game");
                if (gameCode) {
                    this._gameCode = gameCode;
                    this.router.navigate(['/scavenger-hunt'], { queryParams: {}, replaceUrl: true });
                } else {
                    this.getGame();
                }
            });

        timer(10000).pipe(first()).subscribe(() => {
            if (!this.cameraView.hasCameras) {
                this.dialogService.displayConfirmationDialog('Copy console dump', 'Error instantiating cameras', 'Copy')
                    .subscribe(res => {
                        if (res) {
                            navigator.clipboard.writeText((console as any).errors.join(" --- "))
                            .then(() => {
                                this.dialogService.displayConfirmationDialog('Successfully copied console dump. Paste that into a message to Steven.', 'Copied', 'Ok').subscribe();
                            }).catch(() => {
                                this.dialogService.displayConfirmationDialog('Failed to copy console dump. Steven is sad.', 'Copy Fail', 'Cry').subscribe();
                            });
                        }
                    });
            }
        });
    }

    /** Destroy lifecycle hook. */
    public ngOnDestroy(): void {
        this._ngDestroy.next();
    }

    public canDeactivate(): Observable<boolean> | boolean {
        if (this._gameInProgress) {
            return this.dialogService.displayConfirmationDialog('Are you sure you want to leave this page? <br>All game progress will be lost.', 'Game in Progress');
        } else {
            return true;
        }
    }

    public getGame(): void {
        this._gameLoading = true;
        this.apiService.getScavengerHunt(this._gameCode)
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
                    this.items = scavengerHunt.items.map(i => <IPrey>{
                        name: i.name,
                        complete: false
                    });
                    this._gameLoading = false;
                }
            });
    }

    public startGame(): void {
        this._ngDestroy.next();

        if (this.cameraView.hasCameras) {
            this.cameraView.startCameraStream();

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
        } else {
            this.dialogService.displayConfirmationDialog('Sorry, we aren\'t detecting any cameras for your device... Try refreshing your browser', 'Uh oh', 'Refresh', 'Go Home')
                .subscribe(res => {
                    if (res) {
                        window.location.replace(`${window.location.href}?game=${this._gameCode}`);
                    } else {
                        this.router.navigate(['/']);
                    }
                });
        }
    }

    /** Gets the name of the scavenger hunt. */
    public get gameName(): string {
        return this._gameName;
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
    public imageCaptured(imageUri: string): void {
        this.updateImageProcessingStatus(true);
        this.apiService.checkImageMatch(this._currItem.id, imageUri).subscribe(res => {
            if (res && res.isMatch) {
                this.preyImageMap.set(this._currItem, imageUri);
                this.preyList.completeItem(this._currItem);
                this.updateImageProcessingStatus(false);
            } else {
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
        this.apiService.getScore(this._gameCode, this.numItemsComplete).subscribe(res => {
            if (res) {
                this._gameComplete = true;
                this._scoring = false;

                this._finalScore = res.score;
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
                        })
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
