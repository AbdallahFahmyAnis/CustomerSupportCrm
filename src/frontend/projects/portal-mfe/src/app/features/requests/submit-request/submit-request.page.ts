import { Component, OnInit, inject } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { FormFeedbackStore, LanguageStore, SessionApi } from 'shared';
import { RequestsStore } from '../requests.store';

/** SDD CRM-012 / CRM-027 — submit web-form request. */
@Component({
  selector: 'app-submit-request-page',
  standalone: true,
  imports: [FormsModule, RouterLink],
  templateUrl: './submit-request.html',
  styleUrls: ['./submit-request.scss'],
})
export class SubmitRequestPage implements OnInit {
  readonly lang = inject(LanguageStore);
  readonly store = inject(RequestsStore);
  private readonly feedback = inject(FormFeedbackStore);
  private readonly session = inject(SessionApi);

  name = '';
  email = '';
  subject = '';
  message = '';
  signedInCustomer = false;

  ngOnInit(): void {
    const s = this.session.session();
    if (s?.email) {
      this.email = s.email;
      this.name = s.name?.trim() || s.email;
      this.signedInCustomer = true;
    }
  }

  onSubmit(f: NgForm): void {
    if (this.signedInCustomer) {
      const s = this.session.session();
      this.email = s?.email?.trim() || this.email.trim();
      this.name = s?.name?.trim() || this.name.trim() || this.email;
    }
    if (f.invalid || !this.email.trim() || !this.name.trim()) {
      this.feedback.error('formInvalid');
      return;
    }
    this.store.submit(
      {
        name: this.name.trim(),
        email: this.email.trim(),
        subject: this.subject.trim(),
        message: this.message.trim(),
      },
      () => this.feedback.success('submitRequestSuccess'),
      (msg) => this.feedback.errorText(msg),
    );
  }

  resetForm(): void {
    if (!this.signedInCustomer) {
      this.name = '';
      this.email = '';
    }
    this.subject = '';
    this.message = '';
    this.store.clearLastResult();
  }
}
