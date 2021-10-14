import { AfterViewInit, Component, ElementRef, EventEmitter, OnInit, Output, ViewChild } from '@angular/core';
import { Camera } from "camera-web-api";
import { PopFadeInAnimation } from 'src/app/animations/popFadeIn.animation';
import { ISnapshot } from 'src/app/models/ISnapshot';

@Component({
    selector: 'camera-view',
    templateUrl: './camera-view-component.html',
    styleUrls: ['./camera-view.component.scss'],
    animations: [PopFadeInAnimation]
})
export class CameraViewComponent implements OnInit, AfterViewInit {

    @Output() public imageCaptured = new EventEmitter<ISnapshot>();
    public startedStreaming = false;

    private camera!: Camera;
    private _processing: boolean = false;

    @ViewChild('video', { static: true, read: ElementRef }) private video!: ElementRef;
    @ViewChild('canvas', { static: true, read: ElementRef }) private canvas!: ElementRef;

    /** Initialization lifecycle hook. */
    public ngOnInit(): void {
        const videoBounds = this.videoPlayer.getBoundingClientRect();
        const ratio = videoBounds.height / videoBounds.width;
        this.camera = new Camera(720 * ratio, 720);
    }

    /** After view initialization lifecycle hook. */
    public ngAfterViewInit(): void {
        this.camera.initialize();
    }

    /** Whether the camera is loading or not. */
    public get isLoading(): boolean {
        return this.camera.isLoading;
    }

    /** Whether the camera is available or not. */
    public get hasCameras(): boolean {
        return this.camera.frontCameras.length > 0 || this.camera.rearCameras.length > 0;
    }


    /** Whether the camera is streaming or not. */
    public get isStreaming(): boolean {
        return this.camera.isStreaming;
    }

    /** Whether the camera can switch directions. */
    public get canSwitchCameras(): boolean {
        return this.camera.canSwitchCameraDirections();
    }

    /** Whether the camera has multiple lenses in the same direction. */
    public get canToggleLenses(): boolean {
        return this.camera.canToggleLenses();
    }

    /** Gets the video player from the view child elementref. */
    public get videoPlayer(): HTMLVideoElement {
        return this.video.nativeElement;
    }

    /** Gets the canvas from the view child elementref. */
    public get captureCanvas(): HTMLCanvasElement {
        return this.canvas.nativeElement;
    }

    /** Whether the image is processing or not. */
    public get imageProcessing(): boolean {
        return this._processing;
    }

    public setImageProcessing(processing: boolean) {
        this._processing = processing;
    }

    /** Starts the camera stream. */
    public startCameraStream(): void {
        this.camera.viewCameraStream(this.videoPlayer);
        this.startedStreaming = true;
    }

    /** Switch camera directions (front to back). */
    public switchCameras(): void {
        this.camera.switchCameraDirection(this.videoPlayer);
    }

    /** Captures a screenshot of the video feed for the game. */
    public captureScreenshot(): void {
        this.setImageProcessing(true);
        if (this.videoPlayer && this.captureCanvas) {
            this.captureCanvas.width = this.videoPlayer.videoWidth;
            this.captureCanvas.height = this.videoPlayer.videoHeight;
            const captureContext = this.captureCanvas.getContext('2d');
            if (captureContext) {
                captureContext.drawImage(this.videoPlayer, 0, 0, this.captureCanvas.width, this.captureCanvas.height);
                const fullSnapshot = this.captureCanvas.toDataURL('image/png');
                if (this.captureCanvas.width >= 2160) {
                    captureContext.scale(0.33, 0.33);
                } else if (this.captureCanvas.width >= 1440) {
                    captureContext.scale(0.5, 0.5);
                } else if (this.captureCanvas.width >= 1080) {
                    captureContext.scale(0.67, 0.67);
                }
                const scaledSnapshot = this.captureCanvas.toDataURL('image/jpeg');
                this.imageCaptured.emit({
                    imageUri: fullSnapshot,
                    scaledImageUri: scaledSnapshot
                });
            }
        }
    }
}
