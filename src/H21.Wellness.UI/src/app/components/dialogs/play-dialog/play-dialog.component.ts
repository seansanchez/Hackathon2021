import { Component } from "@angular/core";
import { PopFadeInAnimation } from "src/app/animations/popFadeIn.animation";
import { DialogBase } from "../dialog-base";

/**
 * A confirmation dialog for presenting the user with a play option.
 */
@Component({
  templateUrl: "./play-dialog.component.html",
  styleUrls: ["./play-dialog.component.scss"],
  animations: [PopFadeInAnimation]
})
export class PlayDialogComponent extends DialogBase {

    public gameCode: string = '';

    private guidRegex = /[a-f0-9]{8}(?:-[a-f0-9]{4}){3}-[a-f0-9]{12}/i;

    /** Whether or not a code has been typed in. */
    public get hasCode(): boolean {
        return this.gameCode.length > 0;
    }

    /** Whether or not the game code is valid. */
    public get gameCodeValid(): boolean {
        return this.guidRegex.test(this.gameCode);
    }

    /** Clears the game code and focuses in the input. */
    public clearCode(codeInput: HTMLInputElement): void {
        this.gameCode = '';
        setTimeout(() => {
            if (codeInput) {
                codeInput.focus();
            }   
        });
    }

    /** Starts the game */
    public playGame(): void {
        this.close(this.gameCode);
    }
}
