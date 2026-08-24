import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { RequestsStore } from '../data-access/requests.store';

/** SDD CRM-012 / CRM-027 — submit web-form request. */
@Component({
  selector: 'app-submit-request-page',
  standalone: true,
  imports: [FormsModule, RouterLink],
  templateUrl: './submit-request.page.html',
  styleUrls: ['./submit-request.page.scss'],
})
export class SubmitRequestPage {
  readonly store = inject(RequestsStore);

  name = '';
  email = '';
  subject = '';
  message = '';

  onSubmit(): void {
    this.store.submit({
      name: this.name,
      email: this.email,
      subject: this.subject,
      message: this.message,
    });
  }
}
