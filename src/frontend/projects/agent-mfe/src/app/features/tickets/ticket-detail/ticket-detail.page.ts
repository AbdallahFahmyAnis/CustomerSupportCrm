import { DatePipe, DecimalPipe } from '@angular/common';
import { Component, OnInit, computed, effect, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, RouterLink } from '@angular/router';
import {
  CrmChatComponent,
  CrmChatMessage,
  CrmEmailComponent,
  CrmEmailMessage,
  CrmTimelineComponent,
  CrmTimelineItem,
  LanguageStore,
} from 'shared';
import { TicketPriorityBadgeComponent } from '../components/ticket-priority-badge/ticket-priority-badge.component';
import { CustomersApi } from '../../customers/customers.api';
import { CustomerDetail } from '../../customers/customers.models';
import { TicketsApi } from '../tickets.api';
import { SlaEvaluation } from '../tickets.models';
import { TicketsStore } from '../tickets.store';

/** SDD CRM-004…007 / CRM-013 — ticket detail with customer summary. */
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
    DatePipe,
    DecimalPipe,
  ],
  templateUrl: './ticket-detail.html',
  styleUrls: ['./ticket-detail.scss'],
  providers: [DatePipe],
})
export class TicketDetailPage implements OnInit {
  readonly lang = inject(LanguageStore);
  readonly store = inject(TicketsStore);
  private readonly api = inject(TicketsApi);
  private readonly customersApi = inject(CustomersApi);
  private readonly route = inject(ActivatedRoute);
  private readonly datePipe = inject(DatePipe);

  tab: 'overview' | 'email' | 'messaging' | 'work' | 'assist' | 'history' = 'overview';
  category = '';
  priority = '';
  agentId = '';
  status = '';
  escalateTo = '';
  replyChannel = signal<'whatsapp' | 'chat' | 'sms'>('chat');
  /** Recipient for SMS / WhatsApp (E.164). */
  replyToPhone = '';
  chatDraft = '';
  emailDraft = '';
  emailQuickId = '';
  chatQuickId = '';
  private routeId = '';
  /** Ticket id for template actions (CRM-014). */
  get id(): string {
    return this.routeId;
  }

  readonly sla = signal<SlaEvaluation | null>(null);
  knowledgeQ = '';
  readonly knowledgeHits = signal<
    { id: string; title: string; kind: string; status: string; score: number; snippet: string }[]
  >([]);
  knowledgeError = '';
  noteDraft = '';
  taskTitle = '';
  taskDue = '';
  readonly customer = signal<CustomerDetail | null>(null);
  readonly customerError = signal('');

  /** Active contacts for summary strip (Angular templates disallow arrow fns). */
  activeContacts(c: CustomerDetail) {
    return c.contacts.filter((x) => x.isActive).slice(0, 3);
  }

  contactTypeLabel(type: string): string {
    switch ((type || '').toLowerCase()) {
      case 'email':
        return this.lang.t('contactEmail');
      case 'phone':
        return this.lang.t('contactPhone');
      case 'whatsapp':
        return this.lang.t('contactWhatsapp');
      case 'address':
        return this.lang.t('contactAddress');
      default:
        return type;
    }
  }
  readonly aiBusy = signal(false);
  readonly aiError = signal('');
  readonly aiSummary = signal<{ summary: string; highlights: string[] } | null>(null);
  readonly aiSuggestions = signal<{ title: string; body: string }[]>([]);
  readonly aiCategory = signal<{ category: string; priority: string; confidence: number } | null>(
    null,
  );

  readonly emailMessages = computed<CrmEmailMessage[]>(() =>
    this.store
      .channelMessages()
      .filter((m) => m.channel === 'Email' || m.channel === 'WebForm')
      .map((m) => {
        const mine = m.direction === 'Outbound';
        const fromName = mine
          ? this.lang.t('supportFrom')
          : this.store.selected()?.customerName || m.fromEmail || this.lang.t('customer');
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

  readonly chatMessages = computed<CrmChatMessage[]>(() => {
    const selected = this.replyChannel();
    const channel =
      selected === 'whatsapp' ? 'WhatsApp' : selected === 'sms' ? 'Sms' : 'LiveChat';
    return this.store
      .channelMessages()
      .filter((m) => {
        if (selected === 'chat') {
          return m.channel === 'LiveChat' || m.channel === 'WebForm';
        }
        return m.channel === channel;
      })
      .map((m) => ({
        id: m.id,
        body: m.body,
        mine: m.direction === 'Outbound',
        meta: m.fromEmail ? m.fromEmail : undefined,
        timeLabel: this.datePipe.transform(m.createdAt, 'short') ?? '',
      }));
  });

  readonly chatVariant = computed(() => {
    const c = this.replyChannel();
    if (c === 'whatsapp') return 'whatsapp' as const;
    if (c === 'sms') return 'sms' as const;
    return 'default' as const;
  });

  readonly messagingSubtitle = computed(() => {
    const t = this.store.selected();
    const num = t?.ticketNumber ?? '';
    const c = this.replyChannel();
    if (c === 'whatsapp') return `${num} · WhatsApp`;
    if (c === 'sms') return `${num} · SMS`;
    return `${num} · ${this.lang.t('channelThread')}`;
  });

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
    return name ? `${this.lang.t('replyTo')} ${name}` : this.lang.t('reply');
  });

  constructor() {
    effect(() => {
      const t = this.store.selected();
      if (!t || t.id !== this.routeId) return;
      this.category = t.category;
      this.priority = t.priority;
      this.agentId = t.assignedAgentId ?? '';
      this.status = t.status;
      if (t.aiSummary) {
        this.aiSummary.set({
          summary: t.aiSummary,
          highlights: t.aiHighlights ?? [],
        });
      }
      this.refreshSla(t.priority, t.createdAt);
      this.loadCustomer(t.customerId);
    });
  }

  ngOnInit(): void {
    this.store.loadOptions();
    this.routeId = this.route.snapshot.paramMap.get('id') ?? '';
    this.store.loadDetail(this.routeId);
  }

  private loadCustomer(customerId: string): void {
    this.customerError.set('');
    this.customersApi.get(customerId).subscribe({
      next: (row) => {
        this.customer.set(row);
        if (!this.replyToPhone.trim()) {
          const phone =
            row.contacts?.find(
              (c) => c.isActive && c.type.toLowerCase() === 'whatsapp' && c.value,
            )?.value ||
            row.contacts?.find(
              (c) => c.isActive && c.type.toLowerCase() === 'phone' && c.value,
            )?.value ||
            '';
          this.replyToPhone = this.toE164Hint(phone);
        }
      },
      error: () => {
        this.customer.set(null);
        this.customerError.set(this.lang.t('couldNotLoadCustomerProfile'));
      },
    });
  }

  /** Light client hint: Egyptian 01… → +201… for the To field. */
  private toE164Hint(raw: string): string {
    const t = raw.trim().replace(/^whatsapp:/i, '');
    if (!t) return '';
    const digits = t.replace(/\D/g, '');
    if (t.startsWith('+')) return `+${digits}`;
    if (digits.startsWith('0') && digits.length === 11) return `+20${digits.slice(1)}`;
    if (digits.length >= 11) return `+${digits}`;
    return t;
  }

  private refreshSla(priority: string, createdAt: string): void {
    this.api.evaluateSla({ priority, createdAt }).subscribe({
      next: (row) => this.sla.set(row),
      error: () => this.sla.set(null),
    });
  }

  saveClass(): void {
    this.api.updateClassification(this.id, this.category, this.priority).subscribe({
      next: () => this.store.refreshDetail(this.id),
      error: (err) => this.store.error.set(err?.error?.error ?? this.lang.t('classificationFailed')),
    });
  }

  saveAssign(): void {
    const agent = this.store.options()?.agents.find((a) => a.id === this.agentId);
    this.api.assign(this.id, this.agentId || null, agent?.name ?? null).subscribe({
      next: () => this.store.refreshDetail(this.id),
      error: (err) => this.store.error.set(err?.error?.error ?? this.lang.t('assignFailed')),
    });
  }

  saveStatus(): void {
    this.api.changeStatus(this.id, this.status).subscribe({
      next: () => this.store.refreshDetail(this.id),
      error: (err) => this.store.error.set(err?.error?.error ?? this.lang.t('statusChangeFailed')),
    });
  }

  escalate(): void {
    const agent = this.store.options()?.agents.find((a) => a.id === this.escalateTo);
    this.api.escalate(this.id, agent?.id, agent?.name).subscribe({
      next: () => this.store.refreshDetail(this.id),
      error: (err) => this.store.error.set(err?.error?.error ?? this.lang.t('escalateFailed')),
    });
  }

  onEmailSend(body: string): void {
    const text = body.trim();
    if (!text) {
      this.store.error.set(this.lang.t('replyBodyRequired'));
      return;
    }
    this.store.replyEmail(this.id, text, () => {
      this.emailDraft = '';
    });
  }

  onChatSend(body: string): void {
    const text = body.trim();
    if (!text) {
      this.store.error.set(this.lang.t('replyBodyRequired'));
      return;
    }
    const clear = () => {
      this.chatDraft = '';
    };
    const to = this.replyToPhone.trim() || undefined;
    if (
      (this.replyChannel() === 'whatsapp' || this.replyChannel() === 'sms') &&
      !to
    ) {
      this.store.error.set(this.lang.t('recipientPhoneRequired'));
      return;
    }
    if (this.replyChannel() === 'whatsapp') {
      this.store.replyWhatsApp(this.id, text, clear, to);
      return;
    }
    if (this.replyChannel() === 'sms') {
      this.store.replySms(this.id, text, clear, to);
      return;
    }
    this.store.replyChat(this.id, text, clear);
  }

  runAutomation(): void {
    this.api.runAutomation(this.id).subscribe({
      next: () => this.store.refreshDetail(this.id),
      error: (err) => this.store.error.set(err?.error?.error ?? this.lang.t('automationFailed')),
    });
  }

  searchKnowledge(): void {
    const q = this.knowledgeQ.trim();
    this.knowledgeError = '';
    if (!q) {
      this.knowledgeError = this.lang.t('enterSearchQuery');
      return;
    }
    this.api.searchKnowledge(q).subscribe({
      next: (rows) => this.knowledgeHits.set(rows),
      error: (err) => {
        this.knowledgeError = err?.error?.error ?? this.lang.t('knowledgeSearchFailed');
        this.knowledgeHits.set([]);
      },
    });
  }

  /** SDD CRM-016 — insert @Agent Name into the note draft. */
  insertMention(name: string): void {
    const token = `@${name}`;
    const draft = this.noteDraft.trim();
    this.noteDraft = draft ? `${draft} ${token} ` : `${token} `;
  }

  saveNote(): void {
    const body = this.noteDraft.trim();
    if (!body) {
      this.store.error.set(this.lang.t('noteBodyRequired'));
      return;
    }
    this.store.addNote(this.id, body, () => {
      this.noteDraft = '';
    });
  }

  saveTask(): void {
    const title = this.taskTitle.trim();
    if (!title) {
      this.store.error.set(this.lang.t('taskTitleRequired'));
      return;
    }
    const me = this.store.selected()?.assignedAgentId;
    const name = this.store.selected()?.assignedAgentName;
    this.store.createTask(
      this.id,
      {
        title,
        dueAt: this.taskDue ? new Date(this.taskDue).toISOString() : null,
        assigneeUserId: me ?? null,
        assigneeName: name ?? null,
      },
      () => {
        this.taskTitle = '';
        this.taskDue = '';
      },
    );
  }

  /** SDD CRM-015 — insert catalog body into compose draft. */
  insertQuick(target: 'email' | 'chat', replyId: string): void {
    if (!replyId) return;
    const reply = this.store.quickReplies().find((q) => q.id === replyId);
    if (!reply) return;
    if (target === 'email') {
      this.emailDraft = this.emailDraft ? `${this.emailDraft}\n\n${reply.body}` : reply.body;
      this.emailQuickId = '';
    } else {
      this.chatDraft = this.chatDraft ? `${this.chatDraft}\n\n${reply.body}` : reply.body;
      this.chatQuickId = '';
    }
  }

  /** SDD CRM-023 deferred / 046 — stream tokens then finalize. */
  generateAiSummary(): void {
    this.aiBusy.set(true);
    this.aiError.set('');
    this.aiSummary.set({ summary: '', highlights: [] });
    void this.api
      .streamSummary(this.id, (text) => {
        const cur = this.aiSummary();
        this.aiSummary.set({
          summary: (cur?.summary ?? '') + text,
          highlights: cur?.highlights ?? [],
        });
      })
      .then((row) => {
        this.aiSummary.set({ summary: row.summary, highlights: row.highlights ?? [] });
        this.aiBusy.set(false);
      })
      .catch(() => {
        this.aiError.set(this.lang.t('couldNotGenerateSummary'));
        this.aiBusy.set(false);
      });
  }

  /** SDD CRM-024 */
  loadAiSuggestions(): void {
    this.aiBusy.set(true);
    this.aiError.set('');
    this.api.suggestReplies(this.id).subscribe({
      next: (rows) => {
        this.aiSuggestions.set(rows ?? []);
        this.aiBusy.set(false);
      },
      error: () => {
        this.aiError.set(this.lang.t('couldNotLoadSuggestions'));
        this.aiBusy.set(false);
      },
    });
  }

  insertAiSuggestion(target: 'email' | 'chat', body: string): void {
    if (target === 'email') {
      this.emailDraft = this.emailDraft ? `${this.emailDraft}\n\n${body}` : body;
    } else {
      this.chatDraft = this.chatDraft ? `${this.chatDraft}\n\n${body}` : body;
    }
  }

  /** SDD CRM-025 */
  loadAiCategory(): void {
    this.aiBusy.set(true);
    this.aiError.set('');
    this.api.categorize(this.id).subscribe({
      next: (row) => {
        this.aiCategory.set(row);
        this.aiBusy.set(false);
      },
      error: () => {
        this.aiError.set(this.lang.t('couldNotSuggestClassification'));
        this.aiBusy.set(false);
      },
    });
  }

  applyAiCategory(): void {
    const row = this.aiCategory();
    if (!row) return;
    this.category = row.category;
    this.priority = row.priority;
    this.saveClass();
  }
}
