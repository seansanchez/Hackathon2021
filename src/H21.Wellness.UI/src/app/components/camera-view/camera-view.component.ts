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
        this.camera = new Camera(
            {
                min: 720,
                ideal: 1080,
                max: 1440,
            },
            {
                min: 720,
                ideal: 1080,
                max: 1440,
            }
        );
    }

    public get isLoading() {
        return this.camera.isLoading;
    }

    public get isStreaming() {
        return this.camera.isStreaming;
    }

    public get canSwitchCameras() {
        return this.camera.canSwitchCameraDirections();
    }

    public get canToggleLenses() {
        return this.camera.canToggleLenses();
    }

    public get videoPlayer() {
        return this.video.nativeElement;
    }

    public get captureCanvas() {
        return this.canvas.nativeElement;
    }

    public startPlaying() {
        this.camera.viewCameraStream(this.videoPlayer);
        this.startedStreaming = true;
    }

    public switchCameras() {
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
            }
        }
    }
}
