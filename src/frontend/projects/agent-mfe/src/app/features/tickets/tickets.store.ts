import { Injectable, computed, inject, signal } from '@angular/core';
import { ChannelMessageDto, TicketDetail, TicketOptions, TicketSummary } from './tickets.models';
import { TicketsApi } from './tickets.api';

/** SDD CRM-004…007 / CRM-008 — feature store (Feature-Based + Signals). */
@Injectable({ providedIn: 'root' })
export class TicketsStore {
  private readonly api = inject(TicketsApi);

  readonly tickets = signal<TicketSummary[]>([]);
  readonly selected = signal<TicketDetail | null>(null);
  readonly channelMessages = signal<ChannelMessageDto[]>([]);
  readonly options = signal<TicketOptions | null>(null);
  readonly loading = signal(false);
  readonly error = signal('');
  readonly query = signal('');
  readonly assignedOnly = signal(false);

  readonly urgentCount = computed(
    () => this.tickets().filter((t) => t.priority === 'High' || t.priority === 'Urgent').length,
  );

  loadOptions(): void {
    this.api.options().subscribe({
      next: (opts) => this.options.set(opts),
      error: () => this.error.set('Could not load ticket options.'),
    });
  }

  loadList(assignedAgentId?: string): void {
    this.loading.set(true);
    this.error.set('');
    const assignedTo = this.assignedOnly() ? assignedAgentId : undefined;
    this.api.search(this.query(), assignedTo).subscribe({
      next: (rows) => {
        this.tickets.set(rows);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Could not load tickets.');
        this.loading.set(false);
      },
    });
  }

  loadDetail(id: string): void {
    this.loading.set(true);
    this.error.set('');
    this.channelMessages.set([]);
    this.api.get(id).subscribe({
      next: (ticket) => {
        this.selected.set(ticket);
        this.loading.set(false);
        this.loadChannelMessages(id);
      },
      error: () => {
        this.error.set('Ticket not found.');
        this.loading.set(false);
      },
    });
  }

  loadChannelMessages(ticketId: string): void {
    this.api.listChannelMessages(ticketId).subscribe({
      next: (rows) => this.channelMessages.set(rows ?? []),
      error: () => this.channelMessages.set([]),
    });
  }

  create(
    body: {
      customerId: string;
      customerName: string;
      subject: string;
      description?: string;
      category: string;
      priority: string;
    },
    onDone: (id: string) => void,
  ): void {
    this.error.set('');
    this.api.create(body).subscribe({
      next: (t) => onDone(t.id),
      error: (err) => this.error.set(err?.error?.error ?? 'Create failed.'),
    });
  }

  refreshDetail(id: string): void {
    this.api.get(id).subscribe({
      next: (ticket) => {
        this.selected.set(ticket);
        this.loadChannelMessages(id);
      },
      error: (err) => this.error.set(err?.error?.error ?? 'Refresh failed.'),
    });
  }

  replyEmail(ticketId: string, body: string, onDone?: () => void): void {
    this.error.set('');
    this.api.replyEmail(ticketId, body).subscribe({
      next: () => {
        this.loadChannelMessages(ticketId);
        onDone?.();
      },
      error: (err) =>
        this.error.set(err?.error?.message ?? err?.error?.error ?? 'Reply failed.'),
    });
  }
}
