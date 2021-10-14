import { Component, OnInit } from "@angular/core";
import { Router } from "@angular/router";
import { Observable, of } from "rxjs";
import { catchError } from "rxjs/operators";
import { ListSlideFadeInAnimation } from "src/app/animations/listItemFadeIn.animation";
import { IItem } from "src/app/models/IItem";
import { INewScavengerHunt } from "src/app/models/INewScavengerHunt";
import { ApiService } from "src/app/services/api.service";
import { DialogService } from "src/app/services/dialog.service";
import { environment } from "src/environments/environment";
import { CanDeactivateBase } from "../CanDeactivateBase";

@Component({
    templateUrl: './create.component.html',
    styleUrls: ['./create.component.scss'],
    animations: [ListSlideFadeInAnimation]
})
export class CreateComponent extends CanDeactivateBase implements OnInit {

    public huntName: string = '';
    public huntTime: number = 20;
    public huntItems: {
        step: number;
        itemId: string;
        itemName: string;
    }[] = [];

    public nameError!: string | null;
    public timeError!: string | null;

    private _formDirty = false;
    private _allItems: IItem[] = []
    private _saving = false;
    private _sharing = false;

    constructor(
        private readonly apiService: ApiService,
        private readonly dialogService: DialogService,
        private readonly router: Router
    ) {
        super();
    }

    /** Initialization lifecycle hook. */
    public ngOnInit(): void {
        this.apiService.getAllItems().subscribe(itemsRespons => {
            this._allItems = itemsRespons.items.sort((a, b) => a.name.localeCompare(b.name));

            this.addStep();
        });
    }

    /** Overriding base class */
    public canDeactivate(): Observable<boolean> | boolean {
        if (this._formDirty) {
            return this.dialogService.displayConfirmationDialog('Are you sure you want to leave this page? <br>You haven\'t saved your new game yet.', 'Unsaved Game', 'Leave', 'Stay', true);
        } else {
            return true;
        }
    }

    public selectHuntItem(index: number, changeEvent: Event): void {
        const select = changeEvent.target as HTMLSelectElement;

        if (select.value === "-1") {
            this.dialogService.displayImageDetectionDialog(true).subscribe(item => {
                if (item === false) {
                    this.dialogService.displayConfirmationDialog('Error instantiating cameras. Refresh and try again.', 'Uh oh', 'Ok')
                        .subscribe();
                } else if (item && item !== true) {
                    this.allItems.push(item);
                    this.allItems.sort((a, b) => a.name.localeCompare(b.name));
                    this.huntItems[index].itemId = item.id;
                    this.huntItems[index].itemName = item.name;
                }
            });
        }

        const matchingItem = this.allItems.find(i => i.id === select.value);
        if (matchingItem) {
            this.huntItems[index].itemId = matchingItem.id;
            this.huntItems[index].itemName = matchingItem.name;
        }
    }

    public roundTime(): void {
        this.huntTime = Math.floor(this.huntTime);
        this._formDirty = true;
    }

    public validateName(): void {
        this.huntName = this.huntName.replace(/[^a-zA-Z' ]/g, "");
        this._formDirty = true;
    }

    public addStep(): void {
        const maxStepNum = this.huntItems.length > 0 ? Math.max(...this.huntItems.map(i => i.step)) : 0;
        const nextAvailableItem = this.availableItems[0];
        this.huntItems.push({
            step: maxStepNum + 1,
            itemId: nextAvailableItem.id,
            itemName: nextAvailableItem.name
        });
        this._formDirty = true;
    }

    public removeStep(index: number): void {
        this.huntItems.splice(index, 1);
        this.huntItems.forEach((value, index) => {
            value.step = index + 1;
        });
        this._formDirty = true;
    }

    public save(): void {
        this._saving = true;
        const newScavengerHunt = <INewScavengerHunt>{
            name: this.huntName,
            description: this.huntName,
            timeLimitInMinutes: this.huntTime,
            itemIds: this.huntItems.map(i => i.itemId)
        };
        this.apiService.saveScavengerHunt(newScavengerHunt)
            .pipe(
                catchError(() => {
                    this.dialogService.displayConfirmationDialog('There was an issue saving your scavenger hunt. Would you like to try again?', 'Uh oh', 'Try Again', 'Cancel', true)
                        .subscribe(res => {
                            if (res) {
                                this.save();
                            }
                        })
                    return of(null);
                })
            )
            .subscribe(res => {
                if (res) {
                    this.shareGameDialog(res.id);
                }
            });
    }

    public get sharing(): boolean {
        return this._sharing;
    }

    public get saving(): boolean {
        return this._saving;
    }

    public get hasHuntName(): boolean {
        return this.huntName.length > 0;
    }

    public get allItems(): IItem[] {
        return this._allItems;
    }

    public get availableItems(): IItem[] {
        const usedItems = this.huntItems.map(i => i.itemId);
        return this.allItems.filter(i => !usedItems.includes(i.id));
    }

    public get hasMaxSteps(): boolean {
        return this.huntItems.length >= Math.min(20, this.allItems.length);
    }

    public get canSave(): boolean {
        return this._formDirty && this.huntItems.length >= 1 && this.huntNameValid && this.huntTimeValid;
    }

    public get huntNameValid(): boolean {
        if (this.huntName.length === 0) {
            this.nameError = 'Required';
            return false;
        }

        if (this.huntName.length < 1 && this.huntName.length <= 40) {
            this.nameError = 'Must be between 1 and 40 characters in length';
            return false;
        }

        const match = /^[a-zA-Z0-9 ]+$/.test(this.huntName);
        if (!match) {
            this.nameError = 'Cannot contain special characters';
            return false;
        }

        this.nameError = null;
        return true;
    }

    public get huntTimeValid(): boolean {
        if (this.huntTime === null) {
            this.nameError = 'Required';
            return false;
        }

        if (this.huntTime < 5 || this.huntTime > 60) {
            this.timeError = 'Must be between 5 and 60 minutes'
            return false;
        }

        this.timeError = null;
        return true;
    }

    private shareGameDialog(gameCode: string) {
        this.dialogService.displayConfirmationDialog('Your scavenger hunt is now live! Share it with your friends and family or play it now', 'Success!', 'Share', 'Play', true)
            .subscribe(confirmRes => {
                this._sharing = true;
                if (confirmRes) {
                    navigator.share(<ShareData>{
                        title: 'Breath of Fresh Where?',
                        text: `I just created a new Scavenger Hunt! Play it and see how quickly you can find everything!`,
                        url: `${environment.uiUrl}/scavenger-hunt?game=${gameCode}`
                    }).then(() => {
                        this._formDirty = false;
                        this.router.navigate(['/scavenger-hunt'], { queryParams: { game: gameCode } });
                    }).catch(() => {
                        this.dialogService.displayConfirmationDialog('There was an issue sharing your game. Try again?', 'Uh oh', 'Try Again', 'Cancel', true)
                            .subscribe(res => {
                                if (res) {
                                    this.shareGameDialog(gameCode);
                                } else {
                                    this._formDirty = false;
                                    this.router.navigate(['/']);
                                }
                            });
                    });
                } else {
                    this._formDirty = false;
                    this.router.navigate(['/scavenger-hunt'], { queryParams: { game: gameCode } });
                }
            });
    }
}