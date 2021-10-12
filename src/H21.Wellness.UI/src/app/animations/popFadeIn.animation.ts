import { animate, group, state, style, transition, trigger } from "@angular/animations";

export const PopFadeInAnimation =
  trigger("popFadeInState", [
    state("void", style({ opacity: 0, transform: "scale3d(0.8, 0.8, 0.8)" })),
    state("active", style({ opacity: 1, transform: "scale3d(1, 1, 1)" })),
    transition("* => active", [
      style({
        opacity: 0,
        transform: "scale3d(0.8, 0.8, 0.8)"
      }),
      group([
        animate("200ms cubic-bezier(0.1, 0.9, 0.2, 1)", style({ transform: "scale3d(1, 1, 1)" })),
        animate("100ms cubic-bezier(0, 0, 1, 1)", style({ opacity: 1 }))
      ])
    ]),
    transition("* => void", [
      style({ opacity: 1 }),
      animate("100ms cubic-bezier(0, 0, 1, 1)", style({ opacity: 0 }))
    ]),
  ]);
