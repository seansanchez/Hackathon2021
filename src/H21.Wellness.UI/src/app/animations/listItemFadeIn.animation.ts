import { animate, group, query, stagger, style, transition, trigger } from "@angular/animations";

export const ListSlideFadeInAnimation =
  trigger("listSlideFadeInState", [
    transition("* => *", [
      query(":enter", style({ opacity: 0, transform: "translateX(-20%)" }), { optional: true }),
      query(":enter", stagger("100ms", [
        group([
          animate("300ms cubic-bezier(0.1, 0.9, 0.2, 1)", style({ transform: "none" })),
          animate("300ms cubic-bezier(0, 0, 1, 1)", style({ opacity: 1 }))
        ])
      ]), { optional: true }),
    ]),
  ]);
