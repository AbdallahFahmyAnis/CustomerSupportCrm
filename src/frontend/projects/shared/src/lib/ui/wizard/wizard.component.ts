import { DecimalPipe, NgTemplateOutlet } from '@angular/common';
import {
  Component,
  ContentChildren,
  QueryList,
  inject,
  input,
  model,
  output,
} from '@angular/core';
import { LanguageStore } from '../../language.store';
import { CrmWizardStepDirective } from './wizard.directives';
import { CrmWizardStep } from './wizard.models';

/**
 * Materio-inspired multi-step wizard (create-deal shape).
 * Original styles only — not ThemeSelection assets.
 */
@Component({
  selector: 'crm-wizard',
  standalone: true,
  imports: [NgTemplateOutlet, DecimalPipe],
  templateUrl: './wizard.html',
  styleUrls: ['./wizard.scss'],
})
export class CrmWizardComponent {
  readonly lang = inject(LanguageStore);
  readonly steps = input.required<readonly CrmWizardStep[]>();
  readonly step = model(0);
  readonly canNext = input(true);
  readonly finishLabel = input('Submit');
  readonly finishDisabled = input(false);
  readonly finish = output<void>();
  /** Emitted when Next is clicked but `canNext` is false (for field-error UI). */
  readonly advanceBlocked = output<void>();

  @ContentChildren(CrmWizardStepDirective) stepTpls!: QueryList<CrmWizardStepDirective>;

  get activeTpl(): CrmWizardStepDirective | undefined {
    return this.stepTpls?.get(this.step());
  }

  go(index: number): void {
    if (index >= 0 && index <= this.step()) {
      this.step.set(index);
    }
  }

  back(): void {
    if (this.step() > 0) {
      this.step.set(this.step() - 1);
    }
  }

  next(): void {
    if (!this.canNext()) {
      this.advanceBlocked.emit();
      return;
    }
    const last = this.steps().length - 1;
    if (this.step() < last) {
      this.step.set(this.step() + 1);
    }
  }
}
