import { Observable } from "rxjs";

export class CanDeactivateBase {
    public canDeactivate(): Observable<boolean> | boolean {
        // implement in child classes
        return true;
    }
}