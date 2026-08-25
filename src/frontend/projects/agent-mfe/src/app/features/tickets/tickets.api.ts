import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { CustomerOption, ChannelMessageDto, TicketDetail, TicketOptions, TicketSummary } from './tickets.models';

/** SDD CRM-004 — tickets command/query API via gateway. */
@Injectable({ providedIn: 'root' })
export class TicketsApi {
  private readonly http = inject(HttpClient);

  options(): Observable<TicketOptions> {
    return this.http.get<TicketOptions>('/api/tickets/options');
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

  searchCustomers(q = ''): Observable<CustomerOption[]> {
    const query = q.trim() ? `?q=${encodeURIComponent(q.trim())}` : '';
    return this.http.get<CustomerOption[]>(`/api/customers${query}`);
  }
}
