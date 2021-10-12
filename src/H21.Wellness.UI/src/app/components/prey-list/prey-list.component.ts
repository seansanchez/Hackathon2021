import { Component, EventEmitter, Input, OnInit, Output } from "@angular/core";
import { IPrey } from "./IPrey";

@Component({
    selector: 'prey-list',
    templateUrl: './prey-list.component.html',
    styleUrls: ['./prey-list.component.scss']
})
export class PreyListComponent implements OnInit {
    @Input() public items: IPrey[] = [];       
    public expanded = false;

    @Output() private listComplete = new EventEmitter();
    @Output() private itemSelected = new EventEmitter<IPrey>();
    private _currItem!: IPrey;
    
    /** Initialization lifecycle hook. */
    public ngOnInit(): void {
        this.selectItem(this.items[0]);
    }

    /** Gets the current item. */
    public get currItem() {
        return this._currItem;
    }

    /** Selects an item from the list. */
    public selectItem(item: IPrey) {
        this._currItem = item;
        this.expanded = false;
        this.itemSelected.emit(this._currItem);
    }

    /** Completes the item and selects the next item in the list. */
    public completeItem(item: IPrey) {
        const matchingItem = this.items.find(i => i.name === item.name);
        if (matchingItem) {
            matchingItem.complete = true;
        }

        if (this.currItem.name === item.name) {
            const newItem = this.items.find(i => !i.complete);
            if (newItem) {
                this.selectItem(newItem);
            } else {
                this.listComplete.emit();
            }
        }
    }
}
