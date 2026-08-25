import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { RequestsApi } from '../requests.api';

/** SDD CRM-010 — portal live chat widget. */
@Component({
  selector: 'app-live-chat-page',
  standalone: true,
  imports: [FormsModule, RouterLink],
  templateUrl: './live-chat.html',
  styleUrls: ['./live-chat.scss'],
})
export class LiveChatPage {
  private readonly api = inject(RequestsApi);

  name = '';
  email = '';
  message = '';
  ticketId = '';
  ticketNumber = '';
  readonly sending = signal(false);
  readonly error = signal('');
  readonly transcript = signal<{ direction: string; body: string }[]>([]);

  send(): void {
    const body = this.message.trim();
    if (!this.email.trim() || !body) {
      this.error.set('Email and message are required.');
      return;
    }
    this.sending.set(true);
    this.error.set('');
    this.api
      .chat({
        name: this.name.trim() || this.email.trim(),
        email: this.email.trim(),
        body,
        ticketId: this.ticketId || undefined,
      })
      .subscribe({
        next: (result) => {
          this.ticketId = result.ticketId;
          this.ticketNumber = result.ticketNumber;
          this.transcript.update((rows) => [...rows, { direction: 'You', body }]);
          this.message = '';
          this.sending.set(false);
        },
        error: (err) => {
          this.error.set(err?.error?.message ?? err?.error?.error ?? 'Chat failed.');
          this.sending.set(false);
        },
      });
  }
}
