import { Component, computed, effect, input, model, output, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CrmEmailMessage } from './email.models';

/**
 * Materio-inspired email app panel (app-email shape):
 * list + reading pane + reply compose.
 * Original styles only — not ThemeSelection assets.
 */
@Component({
  selector: 'crm-email',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './email.html',
  styleUrls: ['./email.scss'],
})
export class CrmEmailComponent {
  readonly subject = input('Conversation');
  readonly messages = input.required<readonly CrmEmailMessage[]>();
  readonly draft = model('');
  readonly replyToLabel = input('Reply');
  readonly placeholder = input('Write your message…');
  readonly sendLabel = input('Send');
  readonly sending = input(false);
  readonly emptyText = input('No emails yet.');
  readonly composeDisabled = input(false);
  readonly send = output<string>();

  readonly selectedId = signal<string | null>(null);

  readonly selected = computed(() => {
    const id = this.selectedId();
    const list = this.messages();
    if (!list.length) {
      return null;
    }
    return list.find((m) => m.id === id) ?? list[list.length - 1] ?? null;
  });

  constructor() {
    effect(() => {
      const list = this.messages();
      const current = this.selectedId();
      if (!list.length) {
        this.selectedId.set(null);
        return;
      }
      if (!current || !list.some((m) => m.id === current)) {
        this.selectedId.set(list[list.length - 1]!.id);
      }
    });
  }

  select(id: string): void {
    this.selectedId.set(id);
  }

  previewOf(m: CrmEmailMessage): string {
    const raw = (m.preview ?? m.body).replace(/\s+/g, ' ').trim();
    return raw.length > 72 ? `${raw.slice(0, 72)}…` : raw;
  }

  avatarOf(m: CrmEmailMessage): string {
    return (m.avatarText || m.fromName || '?').charAt(0).toUpperCase();
  }

  onSubmit(): void {
    const body = this.draft().trim();
    if (!body || this.sending() || this.composeDisabled()) {
      return;
    }
    this.send.emit(body);
  }
}
