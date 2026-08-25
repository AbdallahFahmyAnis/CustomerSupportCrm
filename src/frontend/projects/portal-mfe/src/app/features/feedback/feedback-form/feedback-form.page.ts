import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { FeedbackStore } from '../feedback.store';

/** SDD CRM-030 — portal CSAT form. */
@Component({
  selector: 'app-feedback-form-page',
  standalone: true,
  imports: [FormsModule, RouterLink],
  templateUrl: './feedback-form.html',
  styleUrls: ['./feedback-form.scss'],
})
export class FeedbackFormPage {
  readonly store = inject(FeedbackStore);
  ticketNumber = '';
  rating = 5;
  comment = '';

  submit(): void {
    if (!this.ticketNumber.trim()) {
      this.store.error.set('Ticket number is required.');
      return;
    }
    this.store.submit(this.ticketNumber, this.rating, this.comment);
  }
}
