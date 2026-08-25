import { Directive, TemplateRef, input } from '@angular/core';

/** Column cell template: `<ng-template crmDataCell="email" let-row>` */
@Directive({
  selector: 'ng-template[crmDataCell]',
  standalone: true,
})
export class CrmDataCellDirective {
  readonly column = input.required<string>({ alias: 'crmDataCell' });
  constructor(readonly template: TemplateRef<{ $implicit: unknown }>) {}
}

/** Grid card template: `<ng-template crmDataCard let-row>` */
@Directive({
  selector: 'ng-template[crmDataCard]',
  standalone: true,
})
export class CrmDataCardDirective {
  constructor(readonly template: TemplateRef<{ $implicit: unknown }>) {}
}

/** Toolbar slot: `<ng-template crmDataToolbar>` */
@Directive({
  selector: 'ng-template[crmDataToolbar]',
  standalone: true,
})
export class CrmDataToolbarDirective {
  constructor(readonly template: TemplateRef<unknown>) {}
}
