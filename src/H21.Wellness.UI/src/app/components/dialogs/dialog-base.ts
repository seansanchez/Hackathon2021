import { Component, Input, OnDestroy, Output } from "@angular/core";
import { Observable, Subject } from "rxjs";
import { first } from "rxjs/operators";

/** Class for implementing Dialogs */
@Component({
    template: ''
})
export class DialogBase implements OnDestroy {

    /** Prevents the user from closing the dialog via the close button or clicking the backdrop. */
    @Input() public preventClose: boolean = false;

    /** Output event when dialog is closing. */
    @Output() public onClose = new Subject<any>();

    private _showDialog: boolean = false;
    private _ngDestroy = new Subject();

    /** Destroy lifecycle hook. */
    public ngOnDestroy(): void {
        this._ngDestroy.next();
    }

    /** Show the dialog */
    public show(): Observable<any> {
        this._showDialog = true;
        return this.onClose.asObservable().pipe(first());
    }

    /** Whether the dialog should display or not */
    public shouldShowDialog() {
        return this._showDialog;
    }

    /** Closes the dialog. */
    public close(closeVal?: any) {
        this._showDialog = false;
        setTimeout(() => {
            this.onClose.next(closeVal);
        }, 100);
    }
}
