import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { CustomerOption, ChannelMessageDto, QuickReply, SlaEvaluation, TicketDetail, TicketNote, TicketOptions, TicketSummary, TicketTask } from './tickets.models';

/** SDD CRM-004 — tickets command/query API via gateway. */
@Injectable({ providedIn: 'root' })
export class TicketsApi {
  private readonly http = inject(HttpClient);

  options(): Observable<TicketOptions> {
    return this.http.get<TicketOptions>('/api/tickets/options');
  }

  /** SDD CRM-015 */
  listQuickReplies(): Observable<QuickReply[]> {
    return this.http.get<QuickReply[]>('/api/tickets/quick-replies');
  }

  search(q = '', assignedTo?: string): Observable<TicketSummary[]> {
    const params = new URLSearchParams();
    if (q.trim()) params.set('q', q.trim());
    if (assignedTo) params.set('assignedTo', assignedTo);
    const qs = params.toString();
    return this.http.get<TicketSummary[]>(`/api/tickets${qs ? `?${qs}` : ''}`);
  }

  get(id: string): Observable<TicketDetail> {
    return this.http.get<TicketDetail>(`/api/tickets/${id}`);
  }

  /** SDD CRM-016 — add internal collaboration note. */
  addNote(id: string, body: string): Observable<TicketNote> {
    return this.http.post<TicketNote>(`/api/tickets/${id}/notes`, { body });
  }

  /** SDD CRM-014 */
  listTasks(ticketId: string): Observable<TicketTask[]> {
    return this.http.get<TicketTask[]>(`/api/tickets/${ticketId}/tasks`);
  }

  listMyTasks(assignedTo: string, dueBefore?: string): Observable<TicketTask[]> {
    const qs = new URLSearchParams({ assignedTo });
    if (dueBefore) qs.set('dueBefore', dueBefore);
    return this.http.get<TicketTask[]>(`/api/tickets/tasks?${qs.toString()}`);
  }

  createTask(
    ticketId: string,
    body: { title: string; dueAt?: string | null; assigneeUserId?: string | null; assigneeName?: string | null },
  ): Observable<TicketTask> {
    return this.http.post<TicketTask>(`/api/tickets/${ticketId}/tasks`, body);
  }

  completeTask(ticketId: string, taskId: string): Observable<TicketTask> {
    return this.http.post<TicketTask>(`/api/tickets/${ticketId}/tasks/${taskId}/complete`, {});
  }

  cancelTask(ticketId: string, taskId: string): Observable<TicketTask> {
    return this.http.post<TicketTask>(`/api/tickets/${ticketId}/tasks/${taskId}/cancel`, {});
  }

  listChannelMessages(ticketId: string): Observable<ChannelMessageDto[]> {
    return this.http.get<ChannelMessageDto[]>(`/api/channels/tickets/${ticketId}/messages`);
  }

  replyEmail(ticketId: string, body: string, to?: string): Observable<{ messageId: string; to: string }> {
    return this.http.post<{ messageId: string; to: string }>(
      `/api/channels/tickets/${ticketId}/messages/email`,
      { body, to },
    );
  }

  replyWhatsApp(ticketId: string, body: string, to?: string): Observable<{ messageId: string; to: string }> {
    return this.http.post<{ messageId: string; to: string }>(
      `/api/channels/tickets/${ticketId}/messages/whatsapp`,
      { body, to },
    );
  }

  replyChat(ticketId: string, body: string, to?: string): Observable<{ messageId: string; to: string }> {
    return this.http.post<{ messageId: string; to: string }>(
      `/api/channels/tickets/${ticketId}/messages/chat`,
      { body, to },
    );
  }

  replySms(ticketId: string, body: string, to?: string): Observable<{ messageId: string; to: string }> {
    return this.http.post<{ messageId: string; to: string }>(
      `/api/channels/tickets/${ticketId}/messages/sms`,
      { body, to },
    );
  }

  create(body: {
    customerId: string;
    customerName: string;
    subject: string;
    description?: string;
    category: string;
    priority: string;
  }): Observable<TicketSummary> {
    return this.http.post<TicketSummary>('/api/tickets', body);
  }

  updateClassification(id: string, category: string, priority: string): Observable<TicketSummary> {
    return this.http.put<TicketSummary>(`/api/tickets/${id}/classification`, { category, priority });
  }

  assign(id: string, agentId: string | null, agentName: string | null): Observable<TicketSummary> {
    return this.http.post<TicketSummary>(`/api/tickets/${id}/assign`, { agentId, agentName });
  }

  changeStatus(id: string, status: string): Observable<TicketSummary> {
    return this.http.post<TicketSummary>(`/api/tickets/${id}/status`, { status });
  }

  escalate(id: string, assignToAgentId?: string, assignToAgentName?: string): Observable<TicketSummary> {
    return this.http.post<TicketSummary>(`/api/tickets/${id}/escalate`, {
      assignToAgentId,
      assignToAgentName,
    });
  }

  /** SDD CRM-017 — evaluate SLA clocks for a ticket snapshot. */
  evaluateSla(body: {
    priority: string;
    createdAt: string;
    firstResponseAt?: string | null;
    resolvedAt?: string | null;
  }): Observable<SlaEvaluation> {
    return this.http.post<SlaEvaluation>('/api/sla/evaluate', body);
  }

  /** SDD CRM-018 / CRM-019 — apply SLA assign + escalate. */
  runAutomation(id: string): Observable<{
    ticket: TicketSummary;
    assigned: boolean;
    escalated: boolean;
    message?: string;
  }> {
    return this.http.post<{
      ticket: TicketSummary;
      assigned: boolean;
      escalated: boolean;
      message?: string;
    }>(`/api/tickets/${id}/run-automation`, {});
  }

  /** SDD CRM-022 — search published knowledge from agent workspace. */
  searchKnowledge(q: string): Observable<
    { id: string; title: string; kind: string; status: string; score: number; snippet: string }[]
  > {
    const qs = new URLSearchParams({ q, publishedOnly: 'true' });
    return this.http.get<
      { id: string; title: string; kind: string; status: string; score: number; snippet: string }[]
    >(`/api/knowledge/search?${qs.toString()}`);
  }

  searchCustomers(q = ''): Observable<CustomerOption[]> {
    const query = q.trim() ? `?q=${encodeURIComponent(q.trim())}` : '';
    return this.http.get<CustomerOption[]>(`/api/customers${query}`);
  }
}
