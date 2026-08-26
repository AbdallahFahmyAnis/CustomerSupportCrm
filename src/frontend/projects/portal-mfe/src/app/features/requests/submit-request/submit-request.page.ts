import { Component, inject } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { FormFeedbackStore, LanguageStore } from 'shared';
import { RequestsStore } from '../requests.store';

/** SDD CRM-012 / CRM-027 — submit web-form request. */
@Component({
  selector: 'app-submit-request-page',
  standalone: true,
  imports: [FormsModule, RouterLink],
  templateUrl: './submit-request.html',
  styleUrls: ['./submit-request.scss'],
})
export class SubmitRequestPage {
  readonly lang = inject(LanguageStore);
  readonly store = inject(RequestsStore);
  private readonly feedback = inject(FormFeedbackStore);

  name = '';
  email = '';
  subject = '';
  message = '';

  onSubmit(f: NgForm): void {
    if (f.invalid) {
      this.feedback.error('formInvalid');
      return;
    }
    this.store.submit(
      {
        name: this.name,
        email: this.email,
        subject: this.subject,
        message: this.message,
      },
      () => this.feedback.success('submitRequestSuccess'),
      (msg) => this.feedback.errorText(msg),
    );
  }

  resetForm(): void {
    this.name = '';
    this.email = '';
    this.subject = '';
    this.message = '';
    this.store.clearLastResult();
  }
}
