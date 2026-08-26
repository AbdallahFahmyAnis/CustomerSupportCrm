import { Component, inject } from '@angular/core';
import { LanguageStore } from '../../language.store';
import { CrmModalComponent } from '../modal/modal.component';
import { FormFeedbackStore } from './form-feedback.store';

/** Host for global form success/fail dialogs. */
@Component({
  selector: 'crm-form-feedback',
  standalone: true,
  imports: [CrmModalComponent],
  templateUrl: './form-feedback.html',
  styleUrls: ['./form-feedback.scss'],
})
export class CrmFormFeedbackComponent {
  readonly feedback = inject(FormFeedbackStore);
  readonly lang = inject(LanguageStore);

  onOpenChange(open: boolean): void {
    this.feedback.open.set(open);
  }
}
