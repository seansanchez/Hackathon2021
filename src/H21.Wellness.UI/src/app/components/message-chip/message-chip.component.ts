import { Component } from "@angular/core";
import { Observable } from "rxjs";
import { first } from "rxjs/operators";
import { PopFadeInAnimation } from "src/app/animations/popFadeIn.animation";
import { DialogBase } from "../dialogs/dialog-base";
import { MessageTypeEnum } from "./MessageTypeEnum";

@Component({
    selector: 'message-chip',
    templateUrl: './message-chip.component.html',
    styleUrls: ['./message-chip.component.scss'],
    animations: [PopFadeInAnimation]
})
export class MessageChipComponent extends DialogBase {

    public message!: string;
    public messageType!: MessageTypeEnum;
    public autoDismiss?: boolean = false;

    public show(): Observable<any> {
        this._showDialog = true;

        if (this.autoDismiss) {
            setTimeout(() => {
                this.close();
            }, 5000);
        }

        return this.onClose.asObservable().pipe(first());
    }

    public get infoType(): MessageTypeEnum {
        return MessageTypeEnum.info;
    }

    public get successType(): MessageTypeEnum {
        return MessageTypeEnum.success;
    }

    public get warningType(): MessageTypeEnum {
        return MessageTypeEnum.warning;
    }

    public get errorType(): MessageTypeEnum {
        return MessageTypeEnum.error;
    }

}