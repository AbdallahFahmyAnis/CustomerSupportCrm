import { Component, input, model, output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CrmChatMessage } from './chat.models';

export type CrmChatVariant = 'default' | 'whatsapp' | 'sms';

/**
 * Materio-inspired chat panel (app-chat shape).
 * Variants: default | whatsapp | sms — WhatsApp mimics the familiar WA thread chrome.
 */
@Component({
  selector: 'crm-chat',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './chat.html',
  styleUrls: ['./chat.scss'],
})
export class CrmChatComponent {
  readonly title = input('Conversation');
  readonly subtitle = input('');
  readonly avatarText = input('C');
  readonly messages = input.required<readonly CrmChatMessage[]>();
  readonly draft = model('');
  readonly placeholder = input('Type a message…');
  readonly sendLabel = input('Send');
  readonly sending = input(false);
  readonly emptyText = input('No messages yet. Say hello.');
  readonly composeDisabled = input(false);
  /** Visual skin: WhatsApp / SMS look closer to real messaging apps. */
  readonly variant = input<CrmChatVariant>('default');
  readonly send = output<string>();

  onSubmit(): void {
    const body = this.draft().trim();
    if (!body || this.sending() || this.composeDisabled()) {
      return;
    }
    this.send.emit(body);
  }
}
