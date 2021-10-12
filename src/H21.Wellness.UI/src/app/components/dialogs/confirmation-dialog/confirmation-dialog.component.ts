import { Component } from "@angular/core";
import { PopFadeInAnimation } from "src/app/animations/popFadeIn.animation";
import { DialogBase } from "../dialog-base";

/**
 * A confirmation dialog for presenting the user with a single choice.
 */
@Component({
  templateUrl: "./confirmation-dialog.component.html",
  styleUrls: ["./confirmation-dialog.component.scss"],
  animations: [PopFadeInAnimation]
})
export class ConfirmationDialogComponent extends DialogBase {

  /** String title to display in the header area. */
  public header: string | undefined;

  /** Body of the dialog (Can be HTML). */
  public body: string | undefined;

  /** The label to display on the confirm action button. Defaults to "Yes". */
  public confirmActionLabel: string = "Yes";

  /** The label to display on the cancel action button. Defaults to "No". */
  public cancelActionLabel: string | undefined;
}
