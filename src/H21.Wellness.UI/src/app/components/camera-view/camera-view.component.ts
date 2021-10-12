import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { Camera } from "camera-web-api";

@Component({
    selector: 'camera-view',
    templateUrl: './camera-view-component.html',
    styleUrls: ['./camera-view.component.scss']
})
export class CameraViewComponent implements OnInit {

    public hasScreenshot = false;
    public screenshots: string[] = [];
    public startedStreaming = false;

    private camera!: Camera;

    @ViewChild('video', { static: true, read: ElementRef }) private video!: ElementRef;
    @ViewChild('canvas', { static: true, read: ElementRef }) private canvas!: ElementRef;

    /** Initialization lifecycle hook. */
    public ngOnInit(): void {
        const videoBounds = this.videoPlayer.getBoundingClientRect();
        const ratio = videoBounds.height / videoBounds.width;
        this.camera = new Camera(
            {
                min: 720 * ratio,
                ideal: 1080 * ratio,
                max: 1440 * ratio,
            },
            {
                min: 720,
                ideal: 1080,
                max: 1440,
            }
        );
    }

    public get isLoading(): boolean {
        return this.camera.isLoading;
    }

    public get isStreaming(): boolean {
        return this.camera.isStreaming;
    }

    public get canSwitchCameras(): boolean {
        return this.camera.canSwitchCameraDirections();
    }

    public get canToggleLenses(): boolean {
        return this.camera.canToggleLenses();
    }

    public get videoPlayer(): HTMLVideoElement {
        return this.video.nativeElement;
    }

    public get captureCanvas(): HTMLCanvasElement {
        return this.canvas.nativeElement;
    }

    public startPlaying(): void {
        this.camera.viewCameraStream(this.videoPlayer);
        this.startedStreaming = true;
    }

    public switchCameras(): void {
        this.camera.switchCameraDirection(this.videoPlayer);
    }

    public captureScreenshot(): void {
        if (this.videoPlayer && this.captureCanvas) {
            this.captureCanvas.width = this.videoPlayer.videoWidth;
            this.captureCanvas.height = this.videoPlayer.videoHeight;
            const captureContext = this.captureCanvas.getContext('2d');
            if (captureContext) {
                captureContext.drawImage(this.videoPlayer, 0, 0, this.captureCanvas.width, this.captureCanvas.height);
                const screenshot = this.captureCanvas.toDataURL('image/png')
                this.screenshots.splice(0, 0, screenshot);
                this.hasScreenshot = true;
                console.log(screenshot);
                console.log(this.captureCanvas.width);
                console.log(this.captureCanvas.height);
            }
        }
    }
}
