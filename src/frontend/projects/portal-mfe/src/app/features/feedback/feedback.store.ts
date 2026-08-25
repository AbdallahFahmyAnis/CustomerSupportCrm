import { Injectable, inject, signal } from '@angular/core';
import { FeedbackApi } from './feedback.api';
import { TicketFeedback } from './feedback.models';

/** SDD CRM-030 */
@Injectable({ providedIn: 'root' })
export class FeedbackStore {
  private readonly api = inject(FeedbackApi);

  readonly submitting = signal(false);
  readonly error = signal('');
  readonly success = signal<TicketFeedback | null>(null);

  submit(ticketNumber: string, rating: number, comment: string): void {
    this.submitting.set(true);
    this.error.set('');
    this.success.set(null);
    this.api
      .submit({
        ticketNumber: ticketNumber.trim(),
        rating,
        comment: comment.trim() || null,
      })
      .subscribe({
        next: (row) => {
          this.success.set(row);
          this.submitting.set(false);
        },
        error: (err) => {
          const msg =
            err?.error?.error ||
            (typeof err?.error === 'string' ? err.error : null) ||
            'Could not submit feedback.';
          this.error.set(msg);
          this.submitting.set(false);
        },
      });
  }
}
