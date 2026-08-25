import { Component, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { CrmChatComponent, CrmChatMessage } from 'shared';
import { RequestsApi } from '../requests.api';

/** SDD CRM-010 — portal live chat (Materio app-chat shape). */
@Component({
  selector: 'app-live-chat-page',
  standalone: true,
  imports: [FormsModule, RouterLink, CrmChatComponent],
  templateUrl: './live-chat.html',
  styleUrls: ['./live-chat.scss'],
})
export class LiveChatPage {
  private readonly api = inject(RequestsApi);

  name = '';
  email = '';
  draft = '';
  ticketId = '';
  ticketNumber = '';
  readonly sending = signal(false);
  readonly error = signal('');
  private readonly rows = signal<{ id: string; body: string; mine: boolean }[]>([]);

  readonly messages = computed<CrmChatMessage[]>(() =>
    this.rows().map((r) => ({
      id: r.id,
      body: r.body,
      mine: r.mine,
      meta: r.mine ? 'You' : 'Agent',
    })),
  );

  get chatTitle(): string {
    return this.ticketNumber ? `Ticket ${this.ticketNumber}` : 'Support chat';
  }

  get chatSubtitle(): string {
    return this.ticketId
      ? 'Continue this conversation'
      : 'Start a chat — we open a support ticket';
  }

  onSend(body: string): void {
    const text = body.trim();
    if (!this.email.trim() || !text) {
      this.error.set('Email and message are required.');
      return;
    }
    this.sending.set(true);
    this.error.set('');
    this.api
      .chat({
        name: this.name.trim() || this.email.trim(),
        email: this.email.trim(),
        body: text,
        ticketId: this.ticketId || undefined,
      })
      .subscribe({
        next: (result) => {
          this.ticketId = result.ticketId;
          this.ticketNumber = result.ticketNumber;
          this.rows.update((list) => [
            ...list,
            { id: `${Date.now()}`, body: text, mine: true },
          ]);
          this.draft = '';
          this.sending.set(false);
        },
        error: (err) => {
          this.error.set(err?.error?.message ?? err?.error?.error ?? 'Chat failed.');
          this.sending.set(false);
        },
      });
  }
}
