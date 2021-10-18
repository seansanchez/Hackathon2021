import { animate, group, style, transition, trigger } from "@angular/animations";

export const slideUpFadeInTransition = [
  style({
    opacity: 0,
    transform: "translate3d(0, 48px, 0)"
  }),
  group([
    animate("100ms cubic-bezier(0.1, 0.9, 0.2, 1)", style({ transform: "translate3d(0, 0, 0)" })),
    animate("100ms cubic-bezier(0, 0, 1, 1)", style({ opacity: 1 }))
  ])
];

export const SlideUpFadeInAnimation =
  trigger("slideUpFadeInState", [
    transition("* => active", slideUpFadeInTransition),
    transition("* => void", [
      style({
        opacity: 1
      }),
      animate("100ms cubic-bezier(0, 0, 1, 1)", style({ opacity: 0 }))
    ])
  ]);
