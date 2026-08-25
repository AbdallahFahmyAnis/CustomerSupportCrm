import { Directive, TemplateRef, inject } from '@angular/core';

/** Marks an ng-template as a wizard step body (order = ContentChildren order). */
@Directive({
  selector: 'ng-template[crmWizardStep]',
  standalone: true,
})
export class CrmWizardStepDirective {
  readonly template = inject(TemplateRef<unknown>);
}
