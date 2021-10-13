import { ComponentRef, Injectable, Injector } from "@angular/core";
import { Overlay, OverlayConfig, OverlayRef } from '@angular/cdk/overlay';
import { ComponentPortal } from "@angular/cdk/portal";
import { Observable } from "rxjs";
import { finalize } from "rxjs/operators";
import { DialogBase } from "../components/dialogs/dialog-base";
import { ConfirmationDialogComponent } from "../components/dialogs/confirmation-dialog/confirmation-dialog.component";
import { PlayDialogComponent } from "../components/dialogs/play-dialog/play-dialog.component";

/**
 * A service to display dialog instances
 */
@Injectable({
  providedIn: "root"
})
export class DialogService {

  private openDialogRefs: ComponentRef<any>[] = [];
  private overlayConfig = new OverlayConfig({
    hasBackdrop: false,
    scrollStrategy: this.overlay.scrollStrategies.block()
  });

  constructor(
    private overlay: Overlay,
    private injector: Injector) {
  }

  /** Displays an instance of the ConfirmationDialogComponent
   * @param body Required: The body of the dialog. Can be basic or full HTML string.
   * @param header Optional: The header to display in the header area of the dialog.
   * @param confirmLabel Optional: The label to show on the confirmAction button.
   * @param cancelActionLabel Optional: The label to show on the cancelActionLabel button.
   * @param preventClose Optional: Prevents the user from closing the dialog via the close button or backdrop.
   */
  public displayConfirmationDialog(body: string, header?: string, confirmActionLabel?: string, cancelActionLabel?: string, preventClose?: boolean): Observable<boolean> {
    this.closeAllConfirmationDialogs();
    const overlayRef = this.overlayRef;
    const dialogComponentRef = overlayRef.attach(
      new ComponentPortal(ConfirmationDialogComponent, null, this.injector)
    );

    dialogComponentRef.instance.body = body;
    dialogComponentRef.instance.header = header;
    if (confirmActionLabel) {
      dialogComponentRef.instance.confirmActionLabel = confirmActionLabel;
    }
    if (cancelActionLabel) {
      dialogComponentRef.instance.cancelActionLabel = cancelActionLabel;
    }
    dialogComponentRef.instance.preventClose = preventClose ?? false;

    return this.showDialog<ConfirmationDialogComponent>(overlayRef, dialogComponentRef);
  }

  /** Displays an instance of the PlayDialogComponent
   * @param preventClose Optional: Prevents the user from closing the dialog via the close button or backdrop.
   */
   public displayPlayDialog(preventClose?: boolean): Observable<string> {
    this.closeAllPlayDialogs();
    const overlayRef = this.overlayRef;
    const dialogComponentRef = overlayRef.attach(
      new ComponentPortal(PlayDialogComponent, null, this.injector)
    );

    dialogComponentRef.instance.preventClose = preventClose ?? false;

    return this.showDialog<PlayDialogComponent>(overlayRef, dialogComponentRef);
  }

  /** Force closes all open play dialogs. */
  public closeAllPlayDialogs() {
    this.openDialogRefs.forEach(dialogComponentRef => {
      if (dialogComponentRef.instance instanceof PlayDialogComponent) {
        dialogComponentRef.instance.close(null);
      }
    });
  }

  /** Force closes all open confirmation dialogs. */
  public closeAllConfirmationDialogs() {
    this.openDialogRefs.forEach(dialogComponentRef => {
      if (dialogComponentRef.instance instanceof ConfirmationDialogComponent) {
        dialogComponentRef.instance.close(null);
      }
    });
  }

  private get overlayRef() {
    return this.overlay.create(this.overlayConfig);
  }

  /** Shows the dialog and readies a return payload. */
  private showDialog<T extends DialogBase>(overlayRef: OverlayRef, dialogComponentRef: ComponentRef<T>): Observable<any> {
    this.openDialogRefs.push(dialogComponentRef);
    return dialogComponentRef.instance.show().pipe(
      finalize(() => {
        if (overlayRef.hasAttached()) {
          overlayRef.dispose();
          this.openDialogRefs = this.openDialogRefs.filter(dialogRef => dialogRef !== dialogComponentRef);
        }
      })
    );
  }
}
