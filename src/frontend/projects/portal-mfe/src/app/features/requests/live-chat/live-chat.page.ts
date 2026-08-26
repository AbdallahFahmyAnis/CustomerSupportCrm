import { DatePipe } from '@angular/common';
import { Component, OnDestroy, OnInit, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import {
  CrmChatComponent,
  CrmChatMessage,
  FormFeedbackStore,
  LanguageStore,
  SessionApi,
} from 'shared';
import { ChannelMessageDto } from '../requests.models';
import { RequestsApi } from '../requests.api';

const CHAT_STORAGE_KEY = 'crm.portal.liveChat';

/** SDD CRM-010 — portal live chat (Materio app-chat shape). */
@Component({
  selector: 'app-live-chat-page',
  standalone: true,
  imports: [FormsModule, RouterLink, CrmChatComponent],
  templateUrl: './live-chat.html',
  styleUrls: ['./live-chat.scss'],
  providers: [DatePipe],
})
export class LiveChatPage implements OnInit, OnDestroy {
  readonly lang = inject(LanguageStore);
  private readonly api = inject(RequestsApi);
  private readonly feedback = inject(FormFeedbackStore);
  private readonly session = inject(SessionApi);
  private readonly datePipe = inject(DatePipe);

  name = '';
  email = '';
  draft = '';
  ticketId = '';
  ticketNumber = '';
  attempted = false;
  signedInCustomer = false;
  readonly sending = signal(false);
  private readonly rows = signal<ChannelMessageDto[]>([]);
  private pollTimer: ReturnType<typeof setInterval> | null = null;

  readonly messages = computed<CrmChatMessage[]>(() =>
    this.rows()
      .filter((m) => m.channel === 'LiveChat' || m.channel === 'WebForm')
      .map((m) => {
        const mine = m.direction === 'Inbound';
        return {
          id: m.id,
          body: m.body,
          mine,
          meta: mine ? this.lang.t('you') : this.lang.t('agentLabel'),
          timeLabel: this.datePipe.transform(m.createdAt, 'short') ?? '',
        };
      }),
  );

  get chatTitle(): string {
    return this.ticketNumber
      ? `${this.lang.t('ticketLabel')} ${this.ticketNumber}`
      : this.lang.t('supportChat');
  }

  get chatSubtitle(): string {
    return this.ticketId ? this.lang.t('chatContinue') : this.lang.t('chatStartHint');
  }

  ngOnInit(): void {
    const s = this.session.session();
    if (s?.email) {
      this.email = s.email;
      this.name = s.name || this.name || s.email;
      this.signedInCustomer = true;
    }
    this.restoreSession();
    if (this.ticketId) {
      this.refreshMessages();
      this.startPolling();
    }
  }

  ngOnDestroy(): void {
    this.stopPolling();
  }

  onSend(body: string): void {
    const text = body.trim();
    this.attempted = true;
    if (!this.email.trim() || !text) {
      this.feedback.error('formInvalid');
      return;
    }
    this.sending.set(true);
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
          this.persistSession();
          this.draft = '';
          this.sending.set(false);
          this.refreshMessages();
          this.startPolling();
        },
        error: (err) => {
          this.feedback.errorText(
            err?.error?.message ?? err?.error?.error ?? this.lang.t('chatFailed'),
          );
          this.sending.set(false);
        },
      });
  }

  private refreshMessages(): void {
    if (!this.ticketId) {
      return;
    }
    this.api.listMessages(this.ticketId).subscribe({
      next: (rows) => this.rows.set(rows ?? []),
      error: () => undefined,
    });
  }

  private startPolling(): void {
    if (this.pollTimer || !this.ticketId) {
      return;
    }
    this.pollTimer = setInterval(() => this.refreshMessages(), 3000);
  }

  private stopPolling(): void {
    if (this.pollTimer) {
      clearInterval(this.pollTimer);
      this.pollTimer = null;
    }
  }

  private persistSession(): void {
    try {
      sessionStorage.setItem(
        CHAT_STORAGE_KEY,
        JSON.stringify({
          email: this.email.trim(),
          ticketId: this.ticketId,
          ticketNumber: this.ticketNumber,
          name: this.name.trim(),
        }),
      );
    } catch {
      /* ignore quota / private mode */
    }
  }

  private restoreSession(): void {
    try {
      const raw = sessionStorage.getItem(CHAT_STORAGE_KEY);
      if (!raw) {
        return;
      }
      const saved = JSON.parse(raw) as {
        email?: string;
        ticketId?: string;
        ticketNumber?: string;
        name?: string;
      };
      if (saved.email && this.email && saved.email.toLowerCase() !== this.email.toLowerCase()) {
        return;
      }
      if (saved.email && !this.email) {
        this.email = saved.email;
      }
      if (saved.name && !this.name) {
        this.name = saved.name;
      }
      if (saved.ticketId) {
        this.ticketId = saved.ticketId;
        this.ticketNumber = saved.ticketNumber ?? '';
      }
    } catch {
      /* ignore */
    }
  }
}
