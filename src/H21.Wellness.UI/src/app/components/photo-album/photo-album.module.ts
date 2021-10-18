import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { PhotoAlbumComponent } from './photo-album.component';

@NgModule({
    declarations: [
        PhotoAlbumComponent
    ],
    imports: [
        CommonModule
    ],
    exports: [
        PhotoAlbumComponent
    ]
})
export class PhotoAlbumModule { }
