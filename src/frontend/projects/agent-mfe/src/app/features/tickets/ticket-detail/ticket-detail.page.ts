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
  readonly store = inject(TicketsStore);
  private readonly api = inject(TicketsApi);
  private readonly customersApi = inject(CustomersApi);
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
      next: (row) => this.customer.set(row),
      error: () => {
        this.customer.set(null);
        this.customerError.set('Could not load customer profile.');
      },
    });
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

  runAutomation(): void {
    this.api.runAutomation(this.id).subscribe({
      next: () => this.store.refreshDetail(this.id),
      error: (err) => this.store.error.set(err?.error?.error ?? 'Automation failed.'),
    });
  }

  searchKnowledge(): void {
    const q = this.knowledgeQ.trim();
    this.knowledgeError = '';
    if (!q) {
      this.knowledgeError = 'Enter a search query.';
      return;
    }
    this.api.searchKnowledge(q).subscribe({
      next: (rows) => this.knowledgeHits.set(rows),
      error: (err) => {
        this.knowledgeError = err?.error?.error ?? 'Knowledge search failed.';
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
      this.store.error.set('Note body is required.');
      return;
    }
    this.store.addNote(this.id, body, () => {
      this.noteDraft = '';
    });
  }

  saveTask(): void {
    const title = this.taskTitle.trim();
    if (!title) {
      this.store.error.set('Task title is required.');
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
        this.aiError.set('Could not generate summary. Is the AI service running?');
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
        this.aiError.set('Could not load suggestions.');
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
        this.aiError.set('Could not suggest classification.');
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
