import { Component, OnInit, ViewChild } from "@angular/core";
import { of } from "rxjs";
import { catchError, timeout } from "rxjs/operators";
import { PopFadeInAnimation } from "src/app/animations/popFadeIn.animation";
import { IImageTag } from "src/app/models/IImageTag";
import { ISnapshot } from "src/app/models/ISnapshot";
import { ApiService } from "src/app/services/api.service";
import { CameraViewComponent } from "../../camera-view/camera-view.component";
import { DialogBase } from "../dialog-base";

/**
 * A confirmation dialog for presenting the user with a single choice.
 */
@Component({
  templateUrl: "./image-detect-dialog.component.html",
  styleUrls: ["./image-detect-dialog.component.scss"],
  animations: [PopFadeInAnimation]
})
export class ImageDetectDialogComponent extends DialogBase implements OnInit {

    public tagName!: string | null;

    private _renderCamera = false;
    private _imageProcessing = false;
    private _imageTags: IImageTag[] = [];
    private _error: string | null = null;
    private _adding: boolean = false;

    @ViewChild('cameraView', { read: CameraViewComponent }) private cameraView!: CameraViewComponent;

    constructor(private readonly apiService: ApiService) {
        super();
    }

    public ngOnInit(): void {
        this._renderCamera = true;
        this.checkCameraReady();
    }

    public startCamera(): void {
        if (!this.cameraView.hasCameras) {
            this.close(false);
        } else {
            this.cameraView.startCameraStream();
        }
    }
    
    public imageCaptured(snapshot: ISnapshot): void {
        this.updateImageProcessing(true);
        this.apiService.detectTags(snapshot.scaledImageUri)
        .pipe(
            timeout(10000),
            catchError(() => {
                this._error = 'There was an issue processing this image. Try again'
                this.updateImageProcessing(false);
                return of(null)
            })
        ).subscribe(res => {
            if (res) {
                this._imageTags = res;
                this.updateImageProcessing(false);
            }
        });
    }

    public tryAgain() {
        this._imageTags = [];
    }

    public choose(): void {
        this._adding = true;
    }

    public get renderCamera(): boolean {
        return this._renderCamera;
    }

    public get imageProcessing(): boolean {
        return this._imageProcessing;
    }

    public get error(): string | null {
        return this._error;
    }

    public get adding(): boolean {
        return this._adding;
    }

    public get imageTags(): IImageTag[] {
        return this._imageTags;
    }

    /** Whether the cameras are ready or not. */
    public get cameraReady(): boolean {
        return this.cameraView ? !this.cameraView.isLoading : false;
    }

    private checkCameraReady(): void {
        if (!this.cameraReady) {
            setTimeout(() => {
                this.checkCameraReady();
            }, 500);
        } else {
            setTimeout(() => {
                this.startCamera();
            }, 500);
        }
    }

    private updateImageProcessing(processing: boolean): void {
        this._imageProcessing = processing;
        this.cameraView.setImageProcessing(this._imageProcessing);
    }
}
