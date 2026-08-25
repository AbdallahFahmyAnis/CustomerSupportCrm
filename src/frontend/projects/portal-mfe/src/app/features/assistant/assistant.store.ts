import { Injectable, inject, signal } from '@angular/core';
import { AssistantApi } from './assistant.api';
import { ChatSource } from './assistant.models';

export type AssistantTurn = {
  id: string;
  role: 'user' | 'assistant';
  body: string;
  sources?: ChatSource[];
};

/** SDD CRM-026 deferred / 047 — durable sessionId + handoff flag. */
@Injectable({ providedIn: 'root' })
export class AssistantStore {
  private readonly api = inject(AssistantApi);

  readonly turns = signal<AssistantTurn[]>([]);
  readonly sending = signal(false);
  readonly error = signal('');
  readonly handoffNeeded = signal(false);
  sessionId = '';

  ask(message: string): void {
    const text = message.trim();
    if (!text) {
      this.error.set('Enter a question.');
      return;
    }
    this.sending.set(true);
    this.error.set('');
    const userId = `u-${Date.now()}`;
    this.turns.update((list) => [...list, { id: userId, role: 'user', body: text }]);
    this.api.chat(text, this.sessionId || undefined).subscribe({
      next: (row) => {
        if (row.sessionId) this.sessionId = row.sessionId;
        if (row.handoffNeeded) this.handoffNeeded.set(true);
        this.turns.update((list) => [
          ...list,
          {
            id: `a-${Date.now()}`,
            role: 'assistant',
            body: row.reply,
            sources: row.sources ?? [],
          },
        ]);
        this.sending.set(false);
      },
      error: () => {
        this.error.set('Assistant unavailable. Is the AI service running?');
        this.sending.set(false);
      },
    });
  }
}
