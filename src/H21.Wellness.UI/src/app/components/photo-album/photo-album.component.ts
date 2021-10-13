import { Component, EventEmitter, Input, OnChanges, Output } from "@angular/core";
import { ListSlideFadeInAnimation } from "src/app/animations/listItemFadeIn.animation";
import { SlideUpFadeInAnimation } from "src/app/animations/slideUpFadeIn.animation";
import { IPrey } from "src/app/models/IPrey";
import { IPreyPhoto } from "src/app/models/IPreyPhoto";

@Component({
    selector: 'photo-album',
    templateUrl: './photo-album.component.html',
    styleUrls: ['./photo-album.component.scss'],
    animations: [ListSlideFadeInAnimation, SlideUpFadeInAnimation]
})
export class PhotoAlbumComponent implements OnChanges {

    @Input() public gameName: string = '';
    @Input() public preyImageMap = new Map<IPrey, string>();
    @Output() public close = new EventEmitter();

    private _preyPhotos: IPreyPhoto[] = [];

    /** Changes lifecycle hook */
    public ngOnChanges(): void {
        this.preyImageMap.forEach((value, key) => {
            this._preyPhotos.push({
                name: key.name,
                imageUri: value
            });
        });
    }

    /** Gets all photos for the album. */
    public get photos(): IPreyPhoto[] {
        return this._preyPhotos;
    }

    /** Saves the photo to the device. */
    public savePhoto(photo: IPreyPhoto): void {
        const saveLink = document.createElement('a');
        saveLink.download = `${photo.name}.png`;
        saveLink.href = photo.imageUri;

        document.body.appendChild(saveLink);
        saveLink.click();
        document.body.removeChild(saveLink);
    }

    /** Closes the album */
    public closeAlbum(): void {
        this.close.emit();
    }
}