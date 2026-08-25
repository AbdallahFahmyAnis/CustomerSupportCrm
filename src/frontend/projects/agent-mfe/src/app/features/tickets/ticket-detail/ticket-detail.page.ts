import { DatePipe } from '@angular/common';
import { Component, OnInit, computed, effect, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, RouterLink } from '@angular/router';
import {
  CrmChatComponent,
  CrmChatMessage,
  CrmEmailComponent,
  CrmEmailMessage,
  CrmTimelineComponent,
  CrmTimelineItem,
} from 'shared';
import { TicketPriorityBadgeComponent } from '../components/ticket-priority-badge/ticket-priority-badge.component';
import { TicketsApi } from '../tickets.api';
import { TicketsStore } from '../tickets.store';

/** Smart detail page — Feature-Based + Signals. */
@Component({
  selector: 'app-ticket-detail-page',
  standalone: true,
  imports: [
    FormsModule,
    RouterLink,
    TicketPriorityBadgeComponent,
    CrmChatComponent,
    CrmEmailComponent,
    CrmTimelineComponent,
  ],
  templateUrl: './ticket-detail.html',
  styleUrls: ['./ticket-detail.scss'],
  providers: [DatePipe],
})
export class TicketDetailPage implements OnInit {
  readonly store = inject(TicketsStore);
  private readonly api = inject(TicketsApi);
  private readonly route = inject(ActivatedRoute);
  private readonly datePipe = inject(DatePipe);

  category = '';
  priority = '';
  agentId = '';
  status = '';
  escalateTo = '';
  replyChannel: 'whatsapp' | 'chat' | 'sms' = 'chat';
  chatDraft = '';
  emailDraft = '';
  private id = '';

  readonly emailMessages = computed<CrmEmailMessage[]>(() =>
    this.store
      .channelMessages()
      .filter((m) => m.channel === 'Email' || m.channel === 'WebForm')
      .map((m) => {
        const mine = m.direction === 'Outbound';
        const fromName = mine
          ? 'Support'
          : this.store.selected()?.customerName || m.fromEmail || 'Customer';
        return {
          id: m.id,
          fromName,
          fromMeta: m.fromEmail || (mine ? 'crm-email' : undefined),
          body: m.body,
          preview: m.body,
          timeLabel: this.datePipe.transform(m.createdAt, 'short') ?? '',
          mine,
          avatarText: fromName.charAt(0),
        };
      }),
  );

  readonly chatMessages = computed<CrmChatMessage[]>(() =>
    this.store
      .channelMessages()
      .filter((m) => m.channel !== 'Email' && m.channel !== 'WebForm')
      .map((m) => ({
        id: m.id,
        body: m.body,
        mine: m.direction === 'Outbound',
        meta: `${m.channel}${m.fromEmail ? ' · ' + m.fromEmail : ''}`,
        timeLabel: this.datePipe.transform(m.createdAt, 'short') ?? '',
      })),
  );

  readonly historyItems = computed<CrmTimelineItem[]>(() => {
    const t = this.store.selected();
    if (!t) {
      return [];
    }
    return t.history.map((h) => ({
      id: h.id,
      title: h.field,
      body: `${h.oldValue || '—'} → ${h.newValue || '—'}`,
      timeLabel: this.datePipe.transform(h.changedAt, 'short') ?? '',
      meta: h.changedBy,
    }));
  });

  readonly emailReplyLabel = computed(() => {
    const name = this.store.selected()?.customerName;
    return name ? `Reply to ${name}` : 'Reply';
  });

  constructor() {
    effect(() => {
      const t = this.store.selected();
      if (!t || t.id !== this.id) return;
      this.category = t.category;
      this.priority = t.priority;
      this.agentId = t.assignedAgentId ?? '';
      this.status = t.status;
    });
  }

  ngOnInit(): void {
    this.store.loadOptions();
    this.id = this.route.snapshot.paramMap.get('id') ?? '';
    this.store.loadDetail(this.id);
  }

  saveClass(): void {
    this.api.updateClassification(this.id, this.category, this.priority).subscribe({
      next: () => this.store.refreshDetail(this.id),
      error: (err) => this.store.error.set(err?.error?.error ?? 'Classification failed.'),
    });
  }

  saveAssign(): void {
    const agent = this.store.options()?.agents.find((a) => a.id === this.agentId);
    this.api.assign(this.id, this.agentId || null, agent?.name ?? null).subscribe({
      next: () => this.store.refreshDetail(this.id),
      error: (err) => this.store.error.set(err?.error?.error ?? 'Assign failed.'),
    });
  }

  saveStatus(): void {
    this.api.changeStatus(this.id, this.status).subscribe({
      next: () => this.store.refreshDetail(this.id),
      error: (err) => this.store.error.set(err?.error?.error ?? 'Status change failed.'),
    });
  }

  escalate(): void {
    const agent = this.store.options()?.agents.find((a) => a.id === this.escalateTo);
    this.api.escalate(this.id, agent?.id, agent?.name).subscribe({
      next: () => this.store.refreshDetail(this.id),
      error: (err) => this.store.error.set(err?.error?.error ?? 'Escalate failed.'),
    });
  }

  onEmailSend(body: string): void {
    const text = body.trim();
    if (!text) {
      this.store.error.set('Reply body is required.');
      return;
    }
    this.store.replyEmail(this.id, text, () => {
      this.emailDraft = '';
    });
  }

  onChatSend(body: string): void {
    const text = body.trim();
    if (!text) {
      this.store.error.set('Reply body is required.');
      return;
    }
    const clear = () => {
      this.chatDraft = '';
    };
    if (this.replyChannel === 'whatsapp') {
      this.store.replyWhatsApp(this.id, text, clear);
      return;
    }
    if (this.replyChannel === 'sms') {
      this.store.replySms(this.id, text, clear);
      return;
    }
    this.store.replyChat(this.id, text, clear);
  }
}
